using MovieSearchService.Application.DTOs;
using MovieSearchService.Domain.Entities;

namespace MovieSearchService.Application.Interfaces;

/// <summary>
/// Repository interface for Elasticsearch operations
/// </summary>
public interface ISearchRepository
{
    /// <summary>
    /// Search movies with filters and pagination
    /// </summary>
    Task<PagedResultDto<MovieSearchResultDto>> SearchMoviesAsync(MovieSearchRequestDto request);

    /// <summary>
    /// Index a single movie document
    /// </summary>
    Task<bool> IndexMovieAsync(SearchMovie movie);

    /// <summary>
    /// Check if Elasticsearch is available
    /// </summary>
    Task<bool> PingAsync();

    /// <summary>
    /// Ensure the movies index exists with proper mapping
    /// </summary>
    Task EnsureIndexExistsAsync();
}
