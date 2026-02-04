using Microsoft.AspNetCore.Mvc;
using MovieService.Models.DTOs;
using MovieService.Models.Filters;
using MovieService.Models.Requests;
using MovieService.Services;

namespace MovieService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieCatalogService _movieCatalogService;

    public MoviesController(IMovieCatalogService movieCatalogService)
    {
        _movieCatalogService = movieCatalogService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<MovieDto>>>> GetMovies([FromQuery] MovieFilter filter, CancellationToken cancellationToken)
    {
        var result = await _movieCatalogService.GetMoviesAsync(filter, cancellationToken);
        return Ok(ApiResponse<PaginatedResponse<MovieDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> GetMovie(Guid id, CancellationToken cancellationToken)
    {
        var movie = await _movieCatalogService.GetMovieByIdAsync(id, cancellationToken);
        return movie is null
            ? NotFound(ApiResponse<MovieDto>.Fail("Movie not found"))
            : Ok(ApiResponse<MovieDto>.Ok(movie));
    }

    [HttpGet("{id:guid}/related")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MovieDto>>>> GetRelated(Guid id, CancellationToken cancellationToken)
    {
        var related = await _movieCatalogService.GetRelatedMoviesAsync(id, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<MovieDto>>.Ok(related));
    }

    [HttpGet("genres")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<string>>>> GetGenres(CancellationToken cancellationToken)
    {
        var genres = await _movieCatalogService.GetGenresAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<string>>.Ok(genres));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MovieDto>>> CreateMovie([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = await _movieCatalogService.CreateMovieAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, ApiResponse<MovieDto>.Ok(movie));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> UpdateMovie(Guid id, [FromBody] UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        var updated = await _movieCatalogService.UpdateMovieAsync(id, request, cancellationToken);
        return updated is null
            ? NotFound(ApiResponse<MovieDto>.Fail("Movie not found"))
            : Ok(ApiResponse<MovieDto>.Ok(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMovie(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _movieCatalogService.DeleteMovieAsync(id, cancellationToken);
        return deleted
            ? Ok(ApiResponse<object>.Ok(new { id }))
            : NotFound(ApiResponse<object>.Fail("Movie not found"));
    }
}
