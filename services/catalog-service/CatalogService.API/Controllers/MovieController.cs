using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/movies")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    /// <summary>
    /// Get all movies (public endpoint)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetAllMovies()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        return Ok(movies);
    }

    /// <summary>
    /// Get movie by ID (public endpoint)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MovieDto>> GetMovieById(Guid id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);
        
        if (movie == null)
        {
            return NotFound(new { message = "Movie not found" });
        }

        return Ok(movie);
    }

    /// <summary>
    /// Create a new movie (requires JWT authentication)
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MovieDto>> CreateMovie([FromBody] CreateMovieDto createMovieDto)
    {
        var movie = await _movieService.CreateMovieAsync(createMovieDto);
        return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
    }
}
