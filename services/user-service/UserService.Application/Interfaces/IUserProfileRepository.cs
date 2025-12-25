using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

/// <summary>
/// Repository interface for UserProfile entity
/// Infrastructure layer will implement this using EF Core
/// </summary>
public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId);
    Task<UserProfile> CreateAsync(UserProfile userProfile);
    Task<UserProfile?> GetByIdAsync(Guid id);
}
