using MovieSearchService.Application.DTOs;

namespace MovieSearchService.Application.Interfaces;

/// <summary>
/// Service interface for movie search operations
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Search movies with filters and pagination
    /// </summary>
    Task<PagedResultDto<MovieSearchResultDto>> SearchMoviesAsync(MovieSearchRequestDto request);

    /// <summary>
    /// Index a movie for searching
    /// </summary>
    Task<bool> IndexMovieAsync(IndexMovieDto movieDto);
}
