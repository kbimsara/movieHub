using MovieService.Models.DTOs;
using MovieService.Models.Filters;
using MovieService.Models.Requests;

namespace MovieService.Services;

public interface IMovieCatalogService
{
    Task<PaginatedResponse<MovieDto>> GetMoviesAsync(MovieFilter filter, CancellationToken cancellationToken);
    Task<MovieDto?> GetMovieByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<MovieDto> CreateMovieAsync(CreateMovieRequest request, CancellationToken cancellationToken);
    Task<MovieDto?> UpdateMovieAsync(Guid id, UpdateMovieRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteMovieAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> GetRelatedMoviesAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetGenresAsync(CancellationToken cancellationToken);
}
