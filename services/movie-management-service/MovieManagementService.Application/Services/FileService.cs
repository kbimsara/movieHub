using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MovieManagementService.Application.DTOs;
using MovieManagementService.Application.Interfaces;
using MovieManagementService.Domain.Entities;

namespace MovieManagementService.Application.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<FileService> _logger;
    private readonly string _uploadPath;
    private readonly long _maxFileSize;
    private readonly string[] _allowedVideoExtensions;
    private readonly string[] _allowedImageExtensions;

    public FileService(
        IFileRepository fileRepository,
        IConfiguration configuration,
        ILogger<FileService> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
        
        // Get configuration with fallback defaults
        _uploadPath = configuration["Storage:UploadPath"] ?? "/app/uploads";
        _maxFileSize = configuration.GetValue<long>("Storage:MaxFileSizeBytes", 5368709120); // 5GB default
        _allowedVideoExtensions = configuration.GetSection("Storage:AllowedVideoExtensions").Get<string[]>() 
            ?? new[] { ".mp4", ".mkv", ".avi", ".mov", ".wmv" };
        _allowedImageExtensions = configuration.GetSection("Storage:AllowedImageExtensions").Get<string[]>() 
            ?? new[] { ".jpg", ".jpeg", ".png", ".webp" };
        
        // Ensure upload directory exists
        try
        {
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
                _logger.LogInformation("Created upload directory: {UploadPath}", _uploadPath);
            }
            else
            {
                _logger.LogInformation("Upload directory exists: {UploadPath}", _uploadPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create upload directory: {UploadPath}", _uploadPath);
            throw;
        }
    }

    public async Task<UploadResponseDto> UploadMovieFileAsync(Guid movieId, IFormFile file, string fileType, string quality, Guid userId)
    {
        _logger.LogInformation(
            "Starting file upload - MovieId: {MovieId}, FileName: {FileName}, FileType: {FileType}, Size: {FileSize} bytes",
            movieId, file.FileName, fileType, file.Length);

        // Validate file size
        if (file.Length > _maxFileSize)
        {
            var errorMsg = $"File size {file.Length} bytes exceeds maximum allowed size {_maxFileSize} bytes";
            _logger.LogWarning(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }

        // Validate file extension
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = fileType == "video" ? _allowedVideoExtensions : _allowedImageExtensions;
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            var errorMsg = $"File extension {fileExtension} not allowed. Allowed: {string.Join(", ", allowedExtensions)}";
            _logger.LogWarning(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }

        var sessionId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var sanitizedFileName = Path.GetFileName(file.FileName);
        var fileName = $"{fileId}_{sanitizedFileName}";
        var filePath = Path.Combine(_uploadPath, fileName);

        _logger.LogInformation("Saving file to: {FilePath}", filePath);

        try
        {
            // Save file to disk with error handling
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
                await stream.FlushAsync();
            }

            // Verify file was written
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                throw new IOException($"File was not saved to disk: {filePath}");
            }

            _logger.LogInformation(
                "File saved successfully - Path: {FilePath}, Size: {FileSize} bytes",
                filePath, fileInfo.Length);
        }
        catch (IOException ioEx)
        {
            _logger.LogError(ioEx, "I/O error while saving file: {FilePath}", filePath);
            throw new InvalidOperationException($"Failed to save file: {ioEx.Message}", ioEx);
        }
        catch (UnauthorizedAccessException authEx)
        {
            _logger.LogError(authEx, "Permission denied while saving file: {FilePath}", filePath);
            throw new InvalidOperationException($"Permission denied: {authEx.Message}", authEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while saving file: {FilePath}", filePath);
            throw;
        }

        // Create file record in database
        _logger.LogInformation("Creating database record for file: {FileId}", fileId);
        
        var movieFile = new MovieFile
        {
            Id = fileId,
            MovieId = movieId,
            FileName = file.FileName,
            FilePath = filePath,
            FileType = fileType,
            FileSize = file.Length,
            Quality = quality,
            MimeType = file.ContentType,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = userId,
            IsProcessed = false
        };

        var created = await _fileRepository.CreateAsync(movieFile);
        _logger.LogInformation(
            "File upload completed successfully - FileId: {FileId}, MovieId: {MovieId}",
            created.Id, movieId);

        return new UploadResponseDto
        {
            SessionId = sessionId,
            MovieId = movieId,
            Status = "completed",
            Progress = 100,
            Message = "Upload completed successfully",
            File = new MovieFileDto
            {
                Id = created.Id,
                MovieId = created.MovieId,
                FileName = created.FileName,
                FilePath = created.FilePath,
                FileType = created.FileType,
                FileSize = created.FileSize,
                Quality = created.Quality,
                MimeType = created.MimeType,
                UploadedAt = created.UploadedAt,
                UploadedBy = created.UploadedBy,
                IsProcessed = created.IsProcessed
            }
        };
    }

    public async Task<MovieFileDto?> GetFileByIdAsync(Guid fileId)
    {
        var file = await _fileRepository.GetByIdAsync(fileId);
        if (file == null) return null;

        return new MovieFileDto
        {
            Id = file.Id,
            MovieId = file.MovieId,
            FileName = file.FileName,
            FilePath = file.FilePath,
            FileType = file.FileType,
            FileSize = file.FileSize,
            Quality = file.Quality,
            MimeType = file.MimeType,
            UploadedAt = file.UploadedAt,
            UploadedBy = file.UploadedBy,
            IsProcessed = file.IsProcessed
        };
    }

    public async Task<List<MovieFileDto>> GetMovieFilesAsync(Guid movieId)
    {
        var files = await _fileRepository.GetByMovieIdAsync(movieId);
        return files.Select(f => new MovieFileDto
        {
            Id = f.Id,
            MovieId = f.MovieId,
            FileName = f.FileName,
            FilePath = f.FilePath,
            FileType = f.FileType,
            FileSize = f.FileSize,
            Quality = f.Quality,
            MimeType = f.MimeType,
            UploadedAt = f.UploadedAt,
            UploadedBy = f.UploadedBy,
            IsProcessed = f.IsProcessed
        }).ToList();
    }

    public async Task<Stream> GetFileStreamAsync(Guid fileId)
    {
        var file = await _fileRepository.GetByIdAsync(fileId);
        if (file == null || !File.Exists(file.FilePath))
            throw new FileNotFoundException("File not found");

        return new FileStream(file.FilePath, FileMode.Open, FileAccess.Read);
    }

    public async Task<bool> DeleteFileAsync(Guid fileId, Guid userId)
    {
        var file = await _fileRepository.GetByIdAsync(fileId);
        if (file == null || file.UploadedBy != userId)
            return false;

        // Delete physical file
        if (File.Exists(file.FilePath))
        {
            File.Delete(file.FilePath);
        }

        return await _fileRepository.DeleteAsync(fileId);
    }
}
