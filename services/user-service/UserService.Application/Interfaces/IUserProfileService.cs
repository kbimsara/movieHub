using UserService.Application.DTOs;

namespace UserService.Application.Interfaces;

/// <summary>
/// Application service interface for user profile operations
/// </summary>
public interface IUserProfileService
{
    Task<UserProfileResponseDto?> GetUserProfileAsync(Guid userId);
    Task<UserProfileResponseDto> CreateUserProfileAsync(Guid userId, string email, string displayName);
    Task<UserProfileResponseDto?> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequestDto request);
}
