using MovieModel = Movie.API.Models.Movie;

namespace Movie.API.Interfaces;

public interface IMovieRepository
{
    Task<MovieModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<MovieModel>> GetAllAsync();
    Task AddAsync(MovieModel movie);
    Task UpdateAsync(MovieModel movie);
    Task DeleteAsync(Guid id);
}