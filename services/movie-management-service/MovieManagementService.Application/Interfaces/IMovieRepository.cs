using MovieManagementService.Domain.Entities;

namespace MovieManagementService.Application.Interfaces;

public interface IMovieRepository
{
    Task<ManagedMovie> CreateAsync(ManagedMovie movie);
    Task<ManagedMovie?> GetByIdAsync(Guid id);
    Task<List<ManagedMovie>> GetAllAsync(bool publishedOnly = true);
    Task<List<ManagedMovie>> GetByUserIdAsync(Guid userId);
    Task<ManagedMovie> UpdateAsync(ManagedMovie movie);
    Task<bool> DeleteAsync(Guid id);
}
