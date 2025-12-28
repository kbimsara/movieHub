using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Persistence;

/// <summary>
/// Repository implementation for UserProfile using Entity Framework Core
/// This is the ONLY place where EF Core code should exist
/// </summary>
public class UserProfileRepository : IUserProfileRepository
{
    private readonly UserDbContext _context;

    public UserProfileRepository(UserDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get user profile by UserId (from JWT token)
    /// </summary>
    public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// Get user profile by primary key Id
    /// </summary>
    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Create a new user profile
    /// </summary>
    public async Task<UserProfile> CreateAsync(UserProfile userProfile)
    {
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }

    /// <summary>
    /// Update an existing user profile
    /// </summary>
    public async Task<UserProfile> UpdateAsync(UserProfile userProfile)
    {
        _context.UserProfiles.Update(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }
}
