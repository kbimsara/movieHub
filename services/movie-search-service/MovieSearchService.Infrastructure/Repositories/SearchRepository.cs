using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;
using MovieSearchService.Application.DTOs;
using MovieSearchService.Application.Interfaces;
using MovieSearchService.Domain.Entities;
using MovieSearchService.Infrastructure.Configuration;

namespace MovieSearchService.Infrastructure.Repositories;

/// <summary>
/// Simplified search repository with mock data for now
/// TODO: Implement full Elasticsearch integration later
/// </summary>
public class SearchRepository : ISearchRepository
{
    private readonly List<SearchMovie> _mockMovies;

    public SearchRepository(IOptions<ElasticsearchSettings> settings)
    {
        // Initialize with some mock data
        _mockMovies = new List<SearchMovie>
        {
            new SearchMovie
            {
                Id = "1",
                Title = "The Shawshank Redemption",
                Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                Genre = "Drama",
                ReleaseYear = 1994,
                Rating = 9.3f,
                CreatedAt = DateTime.UtcNow
            },
            new SearchMovie
            {
                Id = "2",
                Title = "The Godfather",
                Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                Genre = "Crime",
                ReleaseYear = 1972,
                Rating = 9.2f,
                CreatedAt = DateTime.UtcNow
            },
            new SearchMovie
            {
                Id = "3",
                Title = "The Dark Knight",
                Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests.",
                Genre = "Action",
                ReleaseYear = 2008,
                Rating = 9.0f,
                CreatedAt = DateTime.UtcNow
            },
            new SearchMovie
            {
                Id = "4",
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
                Genre = "Crime",
                ReleaseYear = 1994,
                Rating = 8.9f,
                CreatedAt = DateTime.UtcNow
            },
            new SearchMovie
            {
                Id = "5",
                Title = "Inception",
                Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                Genre = "Sci-Fi",
                ReleaseYear = 2010,
                Rating = 8.8f,
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    public async Task<PagedResultDto<MovieSearchResultDto>> SearchMoviesAsync(MovieSearchRequestDto request)
    {
        // Validate pagination
        if (request.Page < 1) request.Page = 1;
        if (request.PageSize < 1) request.PageSize = 10;
        if (request.PageSize > 100) request.PageSize = 100;

        // Filter mock data
        var filteredMovies = _mockMovies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var query = request.Q.ToLower();
            filteredMovies = filteredMovies.Where(m =>
                m.Title.ToLower().Contains(query) ||
                m.Description.ToLower().Contains(query));
        }

        if (!string.IsNullOrWhiteSpace(request.Genre))
        {
            filteredMovies = filteredMovies.Where(m =>
                m.Genre.Equals(request.Genre, StringComparison.OrdinalIgnoreCase));
        }

        if (request.Year.HasValue)
        {
            filteredMovies = filteredMovies.Where(m => m.ReleaseYear == request.Year.Value);
        }

        // Sort by relevance (simple title match for now)
        var sortedMovies = filteredMovies.OrderByDescending(m =>
            string.IsNullOrWhiteSpace(request.Q) ? 0 :
            m.Title.ToLower().Contains(request.Q.ToLower()) ? 2 : 1);

        // Paginate
        var totalCount = sortedMovies.Count();
        var items = sortedMovies
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MovieSearchResultDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Genre = m.Genre,
                ReleaseYear = m.ReleaseYear,
                Rating = m.Rating,
                CreatedAt = m.CreatedAt,
                Score = 1.0 // Mock score
            })
            .ToList();

        return new PagedResultDto<MovieSearchResultDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<bool> IndexMovieAsync(SearchMovie movie)
    {
        // Mock implementation - just add to in-memory list
        _mockMovies.Add(movie);
        return true;
    }

    public async Task<bool> PingAsync()
    {
        // Mock implementation - always return healthy
        return true;
    }

    public async Task EnsureIndexExistsAsync()
    {
        // Mock implementation - do nothing
        return;
    }
}
