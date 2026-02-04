using System.Security.Claims;
using UserService.Models.Entities;
using UserService.Models.Requests;

namespace UserService.Services.Interfaces;

public interface IUserProfileService
{
    Task<UserProfile> GetOrCreateAsync(Guid userId, ClaimsPrincipal claims, CancellationToken cancellationToken);
    Task<UserProfile> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken);
    Task<UserProfile> UpdateAvatarAsync(Guid userId, string avatarUrl, CancellationToken cancellationToken);
}
