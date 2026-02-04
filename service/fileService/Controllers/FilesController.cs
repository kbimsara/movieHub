using System.IO;
using System.Security.Claims;
using System.Text.Json;
using FileService.Data;
using FileService.Models.DTOs;
using FileService.Models.Entities;
using FileService.Models.Requests;
using FileService.Options;
using FileService.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FileService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;
    private readonly FileDbContext _dbContext;
    private readonly ILogger<FilesController> _logger;
    private readonly StorageOptions _storageOptions;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public FilesController(
        IFileUploadService fileUploadService,
        FileDbContext dbContext,
        IOptions<StorageOptions> storageOptions,
        ILogger<FilesController> logger)
    {
        _fileUploadService = fileUploadService;
        _dbContext = dbContext;
        _logger = logger;
        _storageOptions = storageOptions.Value;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(12L * 1024 * 1024 * 1024)]
    public async Task<ActionResult<ApiResponse<UploadMovieResultDto>>> UploadMovie([FromForm] UploadMovieRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null)
        {
            return BadRequest(ApiResponse<UploadMovieResultDto>.Fail("No file provided"));
        }

        if (string.IsNullOrWhiteSpace(request.Metadata))
        {
            return BadRequest(ApiResponse<UploadMovieResultDto>.Fail("Metadata is required"));
        }

        MovieUploadMetadata? metadata;
        try
        {
            metadata = JsonSerializer.Deserialize<MovieUploadMetadata>(request.Metadata, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse metadata payload");
            return BadRequest(ApiResponse<UploadMovieResultDto>.Fail("Invalid metadata payload"));
        }

        if (metadata is null)
        {
            return BadRequest(ApiResponse<UploadMovieResultDto>.Fail("Metadata is required"));
        }

        var userId = GetUserId() ?? "demo-user";
        var result = await _fileUploadService.UploadMovieAsync(request.File, request.Poster, metadata, userId, cancellationToken);
        return Ok(ApiResponse<UploadMovieResultDto>.Ok(result));
    }

    [HttpPost("upload/batch")]
    public ActionResult<ApiResponse<object>> UploadBatch()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, ApiResponse<object>.Fail("Batch upload is not implemented yet"));
    }

    [HttpGet("{fileId:guid}/metadata")]
    public async Task<ActionResult<ApiResponse<FileMetadataDto>>> GetMetadata(Guid fileId, CancellationToken cancellationToken)
    {
        var metadata = await _fileUploadService.GetFileMetadataAsync(fileId, cancellationToken);
        return metadata is null
            ? NotFound(ApiResponse<FileMetadataDto>.Fail("File not found"))
            : Ok(ApiResponse<FileMetadataDto>.Ok(metadata));
    }

    [HttpGet("movie/{movieId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<FileMetadataDto>>>> GetMovieFiles(Guid movieId, CancellationToken cancellationToken)
    {
        var files = await _fileUploadService.GetMovieFilesAsync(movieId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<FileMetadataDto>>.Ok(files));
    }

    [HttpGet("user/me")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<FileMetadataDto>>>> GetUserFiles(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse<IReadOnlyList<FileMetadataDto>>.Fail("User context missing"));
        }

        var files = await _fileUploadService.GetUserFilesAsync(userId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<FileMetadataDto>>.Ok(files));
    }

    [HttpDelete("{fileId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid fileId, CancellationToken cancellationToken)
    {
        await _fileUploadService.DeleteFileAsync(fileId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id = fileId }));
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<StorageStatsDto>>> GetStats(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var stats = await _fileUploadService.GetStorageStatsAsync(userId, cancellationToken);
        return Ok(ApiResponse<StorageStatsDto>.Ok(stats));
    }

    [HttpPost("{fileId:guid}/thumbnail")]
    public ActionResult<ApiResponse<object>> GenerateThumbnail(Guid fileId)
    {
        _logger.LogInformation("Thumbnail generation is not implemented. Requested for {FileId}", fileId);
        return StatusCode(StatusCodes.Status501NotImplemented, ApiResponse<object>.Fail("Thumbnail generation is not available yet"));
    }

    [HttpGet("{fileId:guid}/download-url")]
    public async Task<ActionResult<ApiResponse<object>>> GetDownloadUrl(Guid fileId, CancellationToken cancellationToken)
    {
        var metadata = await _fileUploadService.GetFileMetadataAsync(fileId, cancellationToken);
        if (metadata is null)
        {
            return NotFound(ApiResponse<object>.Fail("File not found"));
        }

        return Ok(ApiResponse<object>.Ok(new
        {
            url = BuildUrl(_storageOptions.DownloadPathTemplate, fileId),
            expiresAt = DateTime.UtcNow.AddHours(1)
        }));
    }

    [HttpGet("{fileId:guid}/stream-url")]
    public async Task<ActionResult<ApiResponse<object>>> GetStreamUrl(Guid fileId, CancellationToken cancellationToken)
    {
        var metadata = await _fileUploadService.GetFileMetadataAsync(fileId, cancellationToken);
        if (metadata is null)
        {
            return NotFound(ApiResponse<object>.Fail("File not found"));
        }

        var streamUrl = BuildUrl(_storageOptions.StreamPathTemplate, fileId);
        return Ok(ApiResponse<object>.Ok(new
        {
            url = streamUrl,
            type = "hls"
        }));
    }

    [HttpGet("{fileId:guid}/download")]
    public async Task<IActionResult> Download(Guid fileId, CancellationToken cancellationToken)
    {
        var storedFile = await GetStoredFileAsync(fileId, cancellationToken);
        if (storedFile is null || !System.IO.File.Exists(storedFile.AbsolutePath))
        {
            return NotFound();
        }

        var mimeType = storedFile.MimeType ?? "application/octet-stream";
        var fileName = storedFile.OriginalName ?? storedFile.FileName;
        return PhysicalFile(storedFile.AbsolutePath, mimeType, fileName);
    }

    [HttpGet("{fileId:guid}/stream")]
    public async Task<IActionResult> Stream(Guid fileId, CancellationToken cancellationToken)
    {
        var storedFile = await GetStoredFileAsync(fileId, cancellationToken);
        if (storedFile is null || !System.IO.File.Exists(storedFile.AbsolutePath))
        {
            return NotFound();
        }

        var stream = new FileStream(storedFile.AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(stream, storedFile.MimeType ?? "application/octet-stream", enableRangeProcessing: true);
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
    }

    private string BuildUrl(string template, Guid id)
    {
        var baseUrl = (_storageOptions.PublicBaseUrl ?? string.Empty).TrimEnd('/');
        var path = (template ?? string.Empty).Replace("{id}", id.ToString());
        if (!path.StartsWith('/'))
        {
            path = "/" + path;
        }

        return string.IsNullOrWhiteSpace(baseUrl) ? path : baseUrl + path;
    }

    private async Task<StoredFile?> GetStoredFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        return await _dbContext.StoredFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);
    }
}
