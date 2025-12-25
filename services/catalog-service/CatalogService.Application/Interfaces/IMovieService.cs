using CatalogService.Application.DTOs;

namespace CatalogService.Application.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
    Task<MovieDto?> GetMovieByIdAsync(Guid id);
    Task<MovieDto> CreateMovieAsync(CreateMovieDto createMovieDto);
}
