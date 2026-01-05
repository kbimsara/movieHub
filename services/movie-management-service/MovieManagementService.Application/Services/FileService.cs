using Microsoft.AspNetCore.Http;
using MovieManagementService.Application.DTOs;
using MovieManagementService.Application.Interfaces;
using MovieManagementService.Domain.Entities;

namespace MovieManagementService.Application.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly string _uploadPath;

    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        
        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<UploadResponseDto> UploadMovieFileAsync(Guid movieId, IFormFile file, string fileType, string quality, Guid userId)
    {
        var sessionId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var fileName = $"{fileId}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(_uploadPath, fileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Create file record
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
