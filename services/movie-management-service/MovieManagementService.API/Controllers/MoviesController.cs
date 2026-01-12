using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieManagementService.Application.DTOs;
using MovieManagementService.Application.Interfaces;
using System.Security.Claims;

namespace MovieManagementService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> CreateMovie([FromBody] CreateMovieDto dto)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var movie = await _movieService.CreateMovieAsync(dto, userId);
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetMovie(Guid id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null)
            return NotFound(new { success = false, message = "Movie not found" });

        return Ok(new { success = true, data = movie });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetAllMovies([FromQuery] bool includeUnpublished = false, [FromQuery] int page = 1, [FromQuery] int limit = 24)
    {
        var movies = await _movieService.GetAllMoviesAsync(!includeUnpublished);
        
        // Return in the format expected by frontend: ApiResponse<PaginatedResponse<Movie>>
        var response = new
        {
            success = true,
            data = new
            {
                data = movies,
                total = movies.Count,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling(movies.Count / (double)limit)
            }
        };
        
        return Ok(response);
    }

    [HttpGet("my-movies")]
    public async Task<ActionResult<List<MovieDto>>> GetMyMovies()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var movies = await _movieService.GetUserMoviesAsync(userId);
        return Ok(movies);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MovieDto>> UpdateMovie(Guid id, [FromBody] CreateMovieDto dto)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        try
        {
            var movie = await _movieService.UpdateMovieAsync(id, dto, userId);
            return Ok(movie);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var success = await _movieService.DeleteMovieAsync(id, userId);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishMovie(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var success = await _movieService.PublishMovieAsync(id, userId);
        if (!success)
            return NotFound();

        return Ok(new { message = "Movie published successfully" });
    }

    [HttpPost("{id}/view")]
    [AllowAnonymous]
    public async Task<IActionResult> IncrementView(Guid id)
    {
        await _movieService.IncrementViewCountAsync(id);
        return Ok();
    }
}
