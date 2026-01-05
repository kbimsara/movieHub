using MovieManagementService.Domain.Entities;

namespace MovieManagementService.Application.Interfaces;

public interface IFileRepository
{
    Task<MovieFile> CreateAsync(MovieFile file);
    Task<MovieFile?> GetByIdAsync(Guid id);
    Task<List<MovieFile>> GetByMovieIdAsync(Guid movieId);
    Task<bool> DeleteAsync(Guid id);
}
