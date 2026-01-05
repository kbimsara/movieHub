using MovieManagementService.Application.DTOs;

namespace MovieManagementService.Application.Interfaces;

public interface IMovieService
{
    Task<MovieDto> CreateMovieAsync(CreateMovieDto dto, Guid userId);
    Task<MovieDto?> GetMovieByIdAsync(Guid id);
    Task<List<MovieDto>> GetAllMoviesAsync(bool publishedOnly = true);
    Task<List<MovieDto>> GetUserMoviesAsync(Guid userId);
    Task<MovieDto> UpdateMovieAsync(Guid id, CreateMovieDto dto, Guid userId);
    Task<bool> DeleteMovieAsync(Guid id, Guid userId);
    Task<bool> PublishMovieAsync(Guid id, Guid userId);
    Task IncrementViewCountAsync(Guid id);
}
