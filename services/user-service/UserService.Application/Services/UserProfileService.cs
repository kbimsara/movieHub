using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

/// <summary>
/// Application service for user profile operations
/// This layer contains business logic (though minimal in this case)
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _repository;

    public UserProfileService(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Get user profile by UserId from JWT token
    /// </summary>
    public async Task<UserProfileResponseDto?> GetUserProfileAsync(Guid userId)
    {
        var userProfile = await _repository.GetByUserIdAsync(userId);
        
        if (userProfile == null)
            return null;

        return new UserProfileResponseDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            Email = userProfile.Email,
            DisplayName = userProfile.DisplayName,
            CreatedAt = userProfile.CreatedAt
        };
    }

    /// <summary>
    /// Create a new user profile
    /// UserId and Email come from JWT token claims
    /// </summary>
    public async Task<UserProfileResponseDto> CreateUserProfileAsync(Guid userId, string email, string displayName)
    {
        // Check if profile already exists
        var existing = await _repository.GetByUserIdAsync(userId);
        if (existing != null)
        {
            throw new InvalidOperationException("User profile already exists");
        }

        var userProfile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email,
            DisplayName = displayName,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(userProfile);

        return new UserProfileResponseDto
        {
            Id = created.Id,
            UserId = created.UserId,
            Email = created.Email,
            DisplayName = created.DisplayName,
            CreatedAt = created.CreatedAt
        };
    }

    /// <summary>
    /// Update an existing user profile
    /// </summary>
    public async Task<UserProfileResponseDto?> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequestDto request)
    {
        var userProfile = await _repository.GetByUserIdAsync(userId);
        
        if (userProfile == null)
            return null;

        // Update fields if provided
        if (!string.IsNullOrEmpty(request.Username))
            userProfile.DisplayName = request.Username;
        
        if (!string.IsNullOrEmpty(request.Email))
            userProfile.Email = request.Email;

        var updated = await _repository.UpdateAsync(userProfile);

        return new UserProfileResponseDto
        {
            Id = updated.Id,
            UserId = updated.UserId,
            Email = updated.Email,
            DisplayName = updated.DisplayName,
            CreatedAt = updated.CreatedAt
        };
    }
}
