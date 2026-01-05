using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieManagementService.Application.DTOs;
using MovieManagementService.Application.Interfaces;
using System.Security.Claims;

namespace MovieManagementService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IMovieService _movieService;

    public UploadController(IFileService fileService, IMovieService movieService)
    {
        _fileService = fileService;
        _movieService = movieService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpPost]
    [RequestSizeLimit(5368709120)] // 5GB limit
    public async Task<ActionResult<UploadResponseDto>> UploadMovie([FromForm] UploadMovieRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        if (request.VideoFile == null || request.VideoFile.Length == 0)
            return BadRequest("Video file is required");

        try
        {
            // Create movie metadata first
            var createMovieDto = new CreateMovieDto
            {
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                Genre = request.Genre ?? string.Empty,
                ReleaseYear = request.ReleaseYear,
                DurationMinutes = request.DurationMinutes,
                Rating = request.Rating,
                PosterUrl = string.Empty, // Will be updated after poster upload
                TrailerUrl = request.TrailerUrl ?? string.Empty,
                Quality = request.Quality ?? "1080p"
            };

            var movie = await _movieService.CreateMovieAsync(createMovieDto, userId);

            // Upload video file
            var videoUpload = await _fileService.UploadMovieFileAsync(
                movie.Id,
                request.VideoFile,
                "video",
                request.Quality ?? "1080p",
                userId);

            // Upload poster if provided
            if (request.PosterFile != null && request.PosterFile.Length > 0)
            {
                var posterUpload = await _fileService.UploadMovieFileAsync(
                    movie.Id,
                    request.PosterFile,
                    "poster",
                    "original",
                    userId);

                // Update movie with poster URL
                var updatedMovie = await _movieService.GetMovieByIdAsync(movie.Id);
                if (updatedMovie != null)
                {
                    createMovieDto.PosterUrl = posterUpload.File?.FilePath ?? string.Empty;
                    await _movieService.UpdateMovieAsync(movie.Id, createMovieDto, userId);
                }
            }

            return Ok(videoUpload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Upload failed", error = ex.Message });
        }
    }

    [HttpGet("{sessionId}")]
    public ActionResult<UploadResponseDto> GetUploadStatus(Guid sessionId)
    {
        // For now, return a simple response
        // In a real implementation, this would track upload progress
        return Ok(new UploadResponseDto
        {
            SessionId = sessionId,
            Status = "completed",
            Progress = 100,
            Message = "Upload completed"
        });
    }
}

public class UploadMovieRequest
{
    public required IFormFile VideoFile { get; set; }
    public IFormFile? PosterFile { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public int ReleaseYear { get; set; }
    public int DurationMinutes { get; set; }
    public double Rating { get; set; }
    public string? TrailerUrl { get; set; }
    public string? Quality { get; set; }
}
