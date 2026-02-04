using MovieHub.AuthService.Models;

namespace MovieHub.AuthService.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
}
