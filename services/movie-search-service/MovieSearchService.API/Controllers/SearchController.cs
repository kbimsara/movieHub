using Microsoft.AspNetCore.Mvc;
using MovieSearchService.Application.DTOs;
using MovieSearchService.Application.Interfaces;

namespace MovieSearchService.API.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        ISearchService searchService,
        ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Search movies with optional filters
    /// </summary>
    /// <param name="q">Search text (searches title and description)</param>
    /// <param name="genre">Filter by genre (exact match)</param>
    /// <param name="year">Filter by release year</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Results per page (default: 10, max: 100)</param>
    /// <returns>Paginated list of movies sorted by relevance</returns>
    [HttpGet("movies")]
    [ProducesResponseType(typeof(PagedResultDto<MovieSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResultDto<MovieSearchResultDto>>> SearchMovies(
        [FromQuery] string? q,
        [FromQuery] string? genre,
        [FromQuery] int? year,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var request = new MovieSearchRequestDto
            {
                Q = q,
                Genre = genre,
                Year = year,
                Page = page,
                PageSize = pageSize
            };

            var result = await _searchService.SearchMoviesAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching movies with query: {Query}", q);
            return StatusCode(500, new { error = "An error occurred while searching movies" });
        }
    }

    /// <summary>
    /// Index a movie document (for testing or internal use)
    /// </summary>
    /// <param name="movieDto">Movie data to index</param>
    /// <returns>Success status</returns>
    [HttpPost("index")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> IndexMovie([FromBody] IndexMovieDto movieDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(movieDto.Id) || string.IsNullOrWhiteSpace(movieDto.Title))
            {
                return BadRequest(new { error = "Id and Title are required" });
            }

            var success = await _searchService.IndexMovieAsync(movieDto);

            if (success)
            {
                return Ok(new { message = "Movie indexed successfully", id = movieDto.Id });
            }

            return StatusCode(500, new { error = "Failed to index movie" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing movie: {Id}", movieDto.Id);
            return StatusCode(500, new { error = "An error occurred while indexing movie" });
        }
    }
}
