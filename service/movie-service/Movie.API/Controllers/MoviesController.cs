using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.API.Data;
using Movie.API.DTOs;
using MovieModel = Movie.API.Models.Movie;

namespace Movie.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoviesController : ControllerBase
{
    private readonly MovieDbContext _context;

    public MoviesController(MovieDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies()
    {
        var movies = await _context.Movies.ToListAsync();
        var dtos = movies.Select(m => new MovieDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            PosterUrl = m.PosterUrl,
            ReleaseYear = m.ReleaseYear,
            CreatedAt = m.CreatedAt
        });
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMovie(Guid id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return NotFound();
        var dto = new MovieDto
        {
            Id = movie.Id,
            Name = movie.Name,
            Description = movie.Description,
            PosterUrl = movie.PosterUrl,
            ReleaseYear = movie.ReleaseYear,
            CreatedAt = movie.CreatedAt
        };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto dto)
    {
        var movie = new MovieModel
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            PosterUrl = dto.PosterUrl,
            ReleaseYear = dto.ReleaseYear,
            CreatedAt = DateTime.UtcNow
        };
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovie(Guid id, [FromBody] CreateMovieDto dto)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return NotFound();
        movie.Name = dto.Name;
        movie.Description = dto.Description;
        movie.PosterUrl = dto.PosterUrl;
        movie.ReleaseYear = dto.ReleaseYear;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(Guid id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return NotFound();
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}