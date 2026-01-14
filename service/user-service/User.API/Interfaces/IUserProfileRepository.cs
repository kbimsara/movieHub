using UserProfileModel = User.API.Models.UserProfile;

namespace User.API.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfileModel?> GetByIdAsync(Guid id);
    Task<UserProfileModel?> GetByUsernameAsync(string username);
    Task<UserProfileModel?> GetByEmailAsync(string email);
    Task<IEnumerable<UserProfileModel>> GetAllAsync();
    Task AddAsync(UserProfileModel userProfile);
    Task UpdateAsync(UserProfileModel userProfile);
    Task DeleteAsync(Guid id);
}