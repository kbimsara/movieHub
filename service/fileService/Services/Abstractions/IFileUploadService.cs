using FileService.Models.DTOs;
using FileService.Models.Requests;
using Microsoft.AspNetCore.Http;

namespace FileService.Services.Abstractions;

public interface IFileUploadService
{
    Task<UploadMovieResultDto> UploadMovieAsync(IFormFile file, IFormFile? poster, MovieUploadMetadata metadata, string? userId, CancellationToken cancellationToken);
    Task<FileMetadataDto?> GetFileMetadataAsync(Guid fileId, CancellationToken cancellationToken);
    Task<IReadOnlyList<FileMetadataDto>> GetMovieFilesAsync(Guid movieId, CancellationToken cancellationToken);
    Task<IReadOnlyList<FileMetadataDto>> GetUserFilesAsync(string? userId, CancellationToken cancellationToken);
    Task DeleteFileAsync(Guid fileId, CancellationToken cancellationToken);
    Task<StorageStatsDto> GetStorageStatsAsync(string? userId, CancellationToken cancellationToken);
    Task<UploadStatusDto?> GetUploadStatusAsync(Guid uploadId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UploadStatusDto>> GetUploadsAsync(string? userId, CancellationToken cancellationToken);
}
