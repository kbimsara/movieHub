namespace MovieHub.Shared.Kernel.Infrastructure.Storage;

/// <summary>
/// Interface for cloud storage operations
/// </summary>
public interface ICloudStorageService
{
    Task<string> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    Task<string> GenerateSignedUrlAsync(string bucketName, string objectName, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
}
