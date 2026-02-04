using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Extensions;
using UserService.Models.Entities;
using UserService.Models.Requests;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class UserProfileService(UserDbContext dbContext, ILogger<UserProfileService> logger) : IUserProfileService
{
    public async Task<UserProfile> GetOrCreateAsync(Guid userId, System.Security.Claims.ClaimsPrincipal claims, CancellationToken cancellationToken)
    {
        var existing = await dbContext.UserProfiles.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (existing != null)
        {
            return existing;
        }

        var email = claims.GetEmail();
        var username = claims.GetUsername() ?? email ?? $"user-{userId.ToString()[..8]}";

        var profile = new UserProfile
        {
            Id = userId,
            Email = email ?? $"{username}@moviehub.local",
            Username = username,
            Role = claims.FindFirst("role")?.Value ?? "user",
            AvatarUrl = $"https://api.dicebear.com/7.x/initials/svg?seed={Uri.EscapeDataString(username)}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.UserProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Created profile for user {UserId}", userId);
        return profile;
    }

    public async Task<UserProfile> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var profile = await dbContext.UserProfiles.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (profile is null)
        {
            throw new InvalidOperationException("Profile not found");
        }

        profile.Email = request.Email.Trim();
        profile.Username = request.Username.Trim();
        profile.FirstName = request.FirstName?.Trim();
        profile.LastName = request.LastName?.Trim();
        profile.Bio = request.Bio?.Trim();
        profile.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }

    public async Task<UserProfile> UpdateAvatarAsync(Guid userId, string avatarUrl, CancellationToken cancellationToken)
    {
        var profile = await dbContext.UserProfiles.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
                       ?? throw new InvalidOperationException("Profile not found");

        profile.AvatarUrl = avatarUrl;
        profile.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }
}
