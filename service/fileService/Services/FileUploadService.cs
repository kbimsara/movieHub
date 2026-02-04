using FileService.Data;
using FileService.Models.DTOs;
using FileService.Models.Entities;
using FileService.Models.Enums;
using FileService.Models.Requests;
using FileService.Options;
using FileService.Services.Abstractions;
using FileService.Services.Clients;
using FileService.Services.Clients.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FileService.Services;

public class FileUploadService : IFileUploadService
{
    private readonly FileDbContext _dbContext;
    private readonly IFileStorageService _storageService;
    private readonly IMovieCatalogClient _movieCatalogClient;
    private readonly ILogger<FileUploadService> _logger;
    private readonly StorageOptions _storageOptions;
    public FileUploadService(
        FileDbContext dbContext,
        IFileStorageService storageService,
        IMovieCatalogClient movieCatalogClient,
        IOptions<StorageOptions> storageOptions,
        ILogger<FileUploadService> logger)
    {
        _dbContext = dbContext;
        _storageService = storageService;
        _movieCatalogClient = movieCatalogClient;
        _logger = logger;
        _storageOptions = storageOptions.Value;
    }

    public async Task<UploadMovieResultDto> UploadMovieAsync(IFormFile file, IFormFile? poster, MovieUploadMetadata metadata, string? userId, CancellationToken cancellationToken)
    {
        if (file == null)
        {
            throw new ArgumentException("File is required", nameof(file));
        }

        var uploadRecord = new UploadRecord
        {
            FileName = file.FileName,
            FileSize = file.Length,
            Status = UploadStatus.Uploading,
            Progress = 5,
            UserId = userId
        };

        _dbContext.UploadRecords.Add(uploadRecord);
        await _dbContext.SaveChangesAsync(cancellationToken);

        StoredFile? storedVideo = null;
        StoredFile? posterFile = null;

        try
        {
            var videoLocation = await _storageService.SaveAsync(file, FileCategory.Video, cancellationToken);
            storedVideo = new StoredFile
            {
                FileName = videoLocation.FileName,
                OriginalName = file.FileName,
                FileSize = file.Length,
                MimeType = file.ContentType ?? "application/octet-stream",
                FileType = FileCategory.Video,
                StoragePath = videoLocation.RelativePath,
                AbsolutePath = videoLocation.AbsolutePath,
                UploadedAt = DateTime.UtcNow,
                UserId = userId
            };
            storedVideo.PublicUrl = BuildStreamUrl(storedVideo.Id);

            _dbContext.StoredFiles.Add(storedVideo);

            if (poster is not null)
            {
                var posterLocation = await _storageService.SaveAsync(poster, FileCategory.Image, cancellationToken);
                posterFile = new StoredFile
                {
                    FileName = posterLocation.FileName,
                    OriginalName = poster.FileName,
                    FileSize = poster.Length,
                    MimeType = poster.ContentType ?? "image/jpeg",
                    FileType = FileCategory.Image,
                    StoragePath = posterLocation.RelativePath,
                    AbsolutePath = posterLocation.AbsolutePath,
                    UploadedAt = DateTime.UtcNow,
                    UserId = userId
                };
                    posterFile.PublicUrl = BuildStreamUrl(posterFile.Id);
                _dbContext.StoredFiles.Add(posterFile);
            }

            uploadRecord.Status = UploadStatus.Processing;
            uploadRecord.Progress = 50;
            uploadRecord.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            var movieId = await CreateMovieRecordAsync(metadata, storedVideo, posterFile, cancellationToken);

            storedVideo.MovieId = movieId;
            if (posterFile is not null)
            {
                posterFile.MovieId = movieId;
            }

            uploadRecord.MovieId = movieId;
            uploadRecord.FileId = storedVideo.Id;
            uploadRecord.Status = UploadStatus.Ready;
            uploadRecord.Progress = 100;
            uploadRecord.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UploadMovieResultDto
            {
                UploadId = uploadRecord.Id,
                MovieId = movieId,
                File = new FileUploadResponseDto
                {
                    FileId = storedVideo.Id,
                    Url = storedVideo.PublicUrl,
                    Path = storedVideo.StoragePath,
                    Metadata = FileMetadataDto.FromEntity(storedVideo)
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed for file {FileName}", file.FileName);
            uploadRecord.Status = UploadStatus.Failed;
            uploadRecord.Error = ex.Message;
            uploadRecord.Progress = 0;
            uploadRecord.UpdatedAt = DateTime.UtcNow;

            if (storedVideo is not null)
            {
                _dbContext.StoredFiles.Remove(storedVideo);
            }

            if (posterFile is not null)
            {
                _dbContext.StoredFiles.Remove(posterFile);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            if (storedVideo is not null)
            {
                await _storageService.DeleteAsync(storedVideo.AbsolutePath, cancellationToken);
            }

            if (posterFile is not null)
            {
                await _storageService.DeleteAsync(posterFile.AbsolutePath, cancellationToken);
            }

            throw;
        }
    }

    public async Task<FileMetadataDto?> GetFileMetadataAsync(Guid fileId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.StoredFiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);
        return entity is null ? null : FileMetadataDto.FromEntity(entity);
    }

    public async Task<IReadOnlyList<FileMetadataDto>> GetMovieFilesAsync(Guid movieId, CancellationToken cancellationToken)
    {
        var files = await _dbContext.StoredFiles.AsNoTracking()
            .Where(x => x.MovieId == movieId)
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync(cancellationToken);

        return files.Select(FileMetadataDto.FromEntity).ToList();
    }

    public async Task<IReadOnlyList<FileMetadataDto>> GetUserFilesAsync(string? userId, CancellationToken cancellationToken)
    {
        var query = _dbContext.StoredFiles.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(x => x.UserId == userId);
        }

        var files = await query.OrderByDescending(x => x.UploadedAt).ToListAsync(cancellationToken);
        return files.Select(FileMetadataDto.FromEntity).ToList();
    }

    public async Task DeleteFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.StoredFiles.FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);
        if (entity is null)
        {
            return;
        }

        _dbContext.StoredFiles.Remove(entity);
        await _storageService.DeleteAsync(entity.AbsolutePath, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<StorageStatsDto> GetStorageStatsAsync(string? userId, CancellationToken cancellationToken)
    {
        var files = _dbContext.StoredFiles.AsNoTracking();
        var totalFiles = await files.CountAsync(cancellationToken);
        var totalSize = await files.Select(f => (long?)f.FileSize).SumAsync(cancellationToken) ?? 0;
        var videoCount = await files.CountAsync(f => f.FileType == FileCategory.Video, cancellationToken);
        var imageCount = await files.CountAsync(f => f.FileType == FileCategory.Image, cancellationToken);
        var subtitleCount = await files.CountAsync(f => f.FileType == FileCategory.Subtitle, cancellationToken);

        long userQuota = 50L * 1024 * 1024 * 1024; // 50GB default quota
        long usedQuota = totalSize;
        if (!string.IsNullOrWhiteSpace(userId))
        {
            usedQuota = await files.Where(f => f.UserId == userId).Select(f => (long?)f.FileSize).SumAsync(cancellationToken) ?? 0;
        }

        return new StorageStatsDto
        {
            TotalFiles = totalFiles,
            TotalSize = totalSize,
            VideoCount = videoCount,
            ImageCount = imageCount,
            SubtitleCount = subtitleCount,
            UserQuota = userQuota,
            UsedQuota = usedQuota
        };
    }

    public async Task<UploadStatusDto?> GetUploadStatusAsync(Guid uploadId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UploadRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == uploadId, cancellationToken);
        return record is null ? null : MapUploadStatus(record);
    }

    public async Task<IReadOnlyList<UploadStatusDto>> GetUploadsAsync(string? userId, CancellationToken cancellationToken)
    {
        var query = _dbContext.UploadRecords.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(x => x.UserId == userId);
        }

        var uploads = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        return uploads.Select(MapUploadStatus).ToList();
    }

    private UploadStatusDto MapUploadStatus(UploadRecord record)
        => new()
        {
            Id = record.Id,
            FileName = record.FileName,
            FileSize = record.FileSize,
            Progress = record.Progress,
            Status = record.Status.ToString().ToLowerInvariant(),
            Error = record.Error,
            MovieId = record.MovieId
        };

    private async Task<Guid> CreateMovieRecordAsync(MovieUploadMetadata metadata, StoredFile videoFile, StoredFile? posterFile, CancellationToken cancellationToken)
    {
        var request = new MovieCatalogRequest
        {
            Title = metadata.Title,
            Description = metadata.Description,
            Year = metadata.Year,
            Duration = metadata.Duration,
            Quality = metadata.Quality,
            Rating = metadata.Rating,
            Genres = metadata.Genres ?? new List<string>(),
            Tags = metadata.Tags ?? new List<string>(),
            Director = metadata.Director,
            Poster = posterFile?.PublicUrl,
            Trailer = metadata.Trailer,
            StreamUrl = BuildStreamUrl(videoFile.Id),
            DownloadUrl = BuildDownloadUrl(videoFile.Id)
        };

        if ((metadata.Cast?.Count ?? 0) > 0)
        {
            request.Cast = metadata.Cast!
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => new MovieCastMemberRequest { Name = name.Trim() })
                .ToList();
        }

        return await _movieCatalogClient.CreateMovieAsync(request, cancellationToken);
    }

    private string BuildDownloadUrl(Guid fileId)
    {
        return BuildFileUrl(_storageOptions.DownloadPathTemplate, fileId);
    }

    private string BuildStreamUrl(Guid fileId)
    {
        return BuildFileUrl(_storageOptions.StreamPathTemplate, fileId);
    }

    private string BuildFileUrl(string template, Guid fileId)
    {
        var baseUrl = (_storageOptions.PublicBaseUrl ?? string.Empty).TrimEnd('/');
        var path = (template ?? string.Empty).Replace("{id}", fileId.ToString());
        if (!path.StartsWith('/'))
        {
            path = "/" + path;
        }

        return string.IsNullOrWhiteSpace(baseUrl) ? path : baseUrl + path;
    }
}
