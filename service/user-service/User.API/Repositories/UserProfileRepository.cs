using User.API.Data;
using User.API.Interfaces;
using UserProfileModel = User.API.Models.UserProfile;
using Microsoft.EntityFrameworkCore;

namespace User.API.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly UserDbContext _context;

    public UserProfileRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileModel?> GetByIdAsync(Guid id)
    {
        return await _context.UserProfiles.FindAsync(id);
    }

    public async Task<UserProfileModel?> GetByUsernameAsync(string username)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserProfileModel?> GetByEmailAsync(string email)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<UserProfileModel>> GetAllAsync()
    {
        return await _context.UserProfiles.ToListAsync();
    }

    public async Task AddAsync(UserProfileModel userProfile)
    {
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserProfileModel userProfile)
    {
        _context.UserProfiles.Update(userProfile);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var userProfile = await _context.UserProfiles.FindAsync(id);
        if (userProfile != null)
        {
            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
        }
    }
}