using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieManagementService.Application.DTOs;
using MovieManagementService.Application.Interfaces;
using System.Security.Claims;

namespace MovieManagementService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet("{fileId}")]
    public async Task<ActionResult<MovieFileDto>> GetFile(Guid fileId)
    {
        var file = await _fileService.GetFileByIdAsync(fileId);
        if (file == null)
            return NotFound();

        return Ok(file);
    }

    [HttpGet("movie/{movieId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<MovieFileDto>>> GetMovieFiles(Guid movieId)
    {
        var files = await _fileService.GetMovieFilesAsync(movieId);
        return Ok(files);
    }

    [HttpGet("{fileId}/stream")]
    [AllowAnonymous]
    public async Task<IActionResult> StreamFile(Guid fileId)
    {
        try
        {
            var fileInfo = await _fileService.GetFileByIdAsync(fileId);
            if (fileInfo == null)
                return NotFound();

            var stream = await _fileService.GetFileStreamAsync(fileId);
            
            return File(stream, fileInfo.MimeType, fileInfo.FileName, enableRangeProcessing: true);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(Guid fileId)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var success = await _fileService.DeleteFileAsync(fileId, userId);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
