using CatalogService.Domain.Entities;

namespace CatalogService.Application.Interfaces;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(Guid id);
    Task<Movie> CreateAsync(Movie movie);
    Task SaveChangesAsync();
}
