using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;

namespace MovieHub.Shared.Kernel.Infrastructure.Storage;

/// <summary>
/// Google Cloud Storage implementation
/// </summary>
public class GoogleCloudStorageService : ICloudStorageService
{
    private readonly StorageClient _storageClient;
    private readonly ILogger<GoogleCloudStorageService> _logger;

    public GoogleCloudStorageService(StorageClient storageClient, ILogger<GoogleCloudStorageService> logger)
    {
        _storageClient = storageClient;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(
        string bucketName,
        string objectName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var uploadObject = await _storageClient.UploadObjectAsync(
                bucketName,
                objectName,
                contentType,
                fileStream,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Uploaded file {ObjectName} to bucket {BucketName}", objectName, bucketName);

            return $"gs://{bucketName}/{objectName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {ObjectName} to bucket {BucketName}", objectName, bucketName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var memoryStream = new MemoryStream();
            await _storageClient.DownloadObjectAsync(bucketName, objectName, memoryStream, cancellationToken: cancellationToken);
            memoryStream.Position = 0;

            _logger.LogInformation("Downloaded file {ObjectName} from bucket {BucketName}", objectName, bucketName);

            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {ObjectName} from bucket {BucketName}", objectName, bucketName);
            throw;
        }
    }

    public async Task DeleteFileAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _storageClient.DeleteObjectAsync(bucketName, objectName, cancellationToken: cancellationToken);

            _logger.LogInformation("Deleted file {ObjectName} from bucket {BucketName}", objectName, bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {ObjectName} from bucket {BucketName}", objectName, bucketName);
            throw;
        }
    }

    public Task<string> GenerateSignedUrlAsync(
        string bucketName,
        string objectName,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        // For production use, configure UrlSigner with service account credentials
        // Example: var urlSigner = UrlSigner.FromServiceAccountPath("path/to/credentials.json");
        // For now, return a simple URL (not production-ready)
        _logger.LogWarning("GenerateSignedUrlAsync called but not fully implemented. Returning object URL without signature.");
        return Task.FromResult($"https://storage.googleapis.com/{bucketName}/{objectName}");
    }

    public async Task<bool> FileExistsAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _storageClient.GetObjectAsync(bucketName, objectName, cancellationToken: cancellationToken);
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
