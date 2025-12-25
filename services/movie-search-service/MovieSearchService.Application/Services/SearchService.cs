using MovieSearchService.Application.DTOs;
using MovieSearchService.Application.Interfaces;
using MovieSearchService.Domain.Entities;

namespace MovieSearchService.Application.Services;

/// <summary>
/// Business logic for movie search operations
/// This service orchestrates search operations without knowing implementation details
/// </summary>
public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepository;

    public SearchService(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<PagedResultDto<MovieSearchResultDto>> SearchMoviesAsync(MovieSearchRequestDto request)
    {
        // Validate pagination
        if (request.Page < 1) request.Page = 1;
        if (request.PageSize < 1) request.PageSize = 10;
        if (request.PageSize > 100) request.PageSize = 100; // Limit max page size

        return await _searchRepository.SearchMoviesAsync(request);
    }

    public async Task<bool> IndexMovieAsync(IndexMovieDto movieDto)
    {
        // Map DTO to domain entity
        var searchMovie = new SearchMovie
        {
            Id = movieDto.Id,
            Title = movieDto.Title,
            Description = movieDto.Description,
            Genre = movieDto.Genre,
            ReleaseYear = movieDto.ReleaseYear,
            Rating = movieDto.Rating,
            CreatedAt = DateTime.UtcNow
        };

        return await _searchRepository.IndexMovieAsync(searchMovie);
    }
}
