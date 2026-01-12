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
    private readonly ILogger<UploadController> _logger;

    public UploadController(
        IFileService fileService,
        IMovieService movieService,
        ILogger<UploadController> logger)
    {
        _fileService = fileService;
        _movieService = movieService;
        _logger = logger;
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

        _logger.LogInformation(
            "Upload request received - UserId: {UserId}, FileName: {FileName}",
            userId, request.File?.FileName ?? "null");
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("ModelState validation failed: {Errors}", string.Join(", ", errors));
            return BadRequest(new { message = "Validation failed", errors });
        }

        if (request.File == null || request.File.Length == 0)
        {
            _logger.LogWarning("File validation failed: File is null or empty");
            return BadRequest("Video file is required");
        }

        _logger.LogInformation(
            "File validation passed - FileName: {FileName}, Size: {FileSize} bytes",
            request.File.FileName, request.File.Length);

        // Parse metadata
        MovieMetadata? metadata = null;
        if (!string.IsNullOrEmpty(request.Metadata))
        {
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                metadata = System.Text.Json.JsonSerializer.Deserialize<MovieMetadata>(request.Metadata, options);
                _logger.LogInformation("Metadata parsed successfully - Title: {Title}", metadata?.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Metadata parsing failed");
                return BadRequest("Invalid metadata format");
            }
        }

        if (metadata == null || string.IsNullOrEmpty(metadata.Title))
        {
            _logger.LogWarning("Metadata validation failed - Title is missing");
            return BadRequest("Title is required in metadata");
        }

        try
        {
            _logger.LogInformation("Creating movie with title: {Title}", metadata.Title);
            
            // Create movie metadata first
            var createMovieDto = new CreateMovieDto
            {
                Title = metadata.Title,
                Description = metadata.Description ?? string.Empty,
                Genre = metadata.Genres?.FirstOrDefault() ?? string.Empty,
                ReleaseYear = metadata.Year ?? DateTime.Now.Year,
                DurationMinutes = metadata.Duration ?? 0,
                Rating = metadata.Rating ?? 0,
                PosterUrl = string.Empty, // Will be updated after poster upload
                TrailerUrl = metadata.Trailer ?? string.Empty,
                Quality = metadata.Quality ?? "1080p"
            };

            var movie = await _movieService.CreateMovieAsync(createMovieDto, userId);
            _logger.LogInformation("Movie created successfully - MovieId: {MovieId}", movie.Id);

            // Upload video file
            var videoUpload = await _fileService.UploadMovieFileAsync(
                movie.Id,
                request.File,
                "video",
                metadata.Quality ?? "1080p",
                userId);

            // Upload poster if provided
            if (request.Poster != null && request.Poster.Length > 0)
            {
                var posterUpload = await _fileService.UploadMovieFileAsync(
                    movie.Id,
                    request.Poster,
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

            return Ok(new UploadResponseDto
            {
                SessionId = Guid.NewGuid(),
                Status = "completed",
                Progress = 100,
                Message = "Movie uploaded successfully",
                MovieId = movie.Id,
                File = videoUpload.File
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed - MovieTitle: {Title}, UserId: {UserId}", metadata?.Title, userId);
            return StatusCode(500, new { 
                message = "Upload failed", 
                error = ex.Message, 
                details = ex.InnerException?.Message 
            });
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
    public IFormFile? File { get; set; }
    public IFormFile? Poster { get; set; }
    public string? Metadata { get; set; }
}

public class MovieMetadata
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Year { get; set; }
    public int? Duration { get; set; }
    public List<string>? Genres { get; set; }
    public string? Quality { get; set; }
    public double? Rating { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Cast { get; set; }
    public string? Director { get; set; }
    public string? Trailer { get; set; }
}
