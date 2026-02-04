using System.Security.Claims;
using FileService.Models.DTOs;
using FileService.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;

    public UploadController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UploadStatusDto>>>> GetUploads(CancellationToken cancellationToken)
    {
        var userId = GetUserId() ?? "demo-user";

        var uploads = await _fileUploadService.GetUploadsAsync(userId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UploadStatusDto>>.Ok(uploads));
    }

    [HttpGet("{uploadId:guid}/status")]
    public async Task<ActionResult<ApiResponse<UploadStatusDto>>> GetStatus(Guid uploadId, CancellationToken cancellationToken)
    {
        var status = await _fileUploadService.GetUploadStatusAsync(uploadId, cancellationToken);
        return status is null
            ? NotFound(ApiResponse<UploadStatusDto>.Fail("Upload not found"))
            : Ok(ApiResponse<UploadStatusDto>.Ok(status));
    }

    [HttpDelete("{uploadId:guid}")]
    public ActionResult<ApiResponse<object>> CancelUpload(Guid uploadId)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, ApiResponse<object>.Fail("Upload cancellation is not yet supported"));
    }

    private string? GetUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
}
