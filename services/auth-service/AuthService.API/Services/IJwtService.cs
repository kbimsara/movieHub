using AuthService.API.Models;

namespace AuthService.API.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    Guid? ValidateToken(string token);
}
