namespace Shared.JWT;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string email, string username);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    string GetUserIdFromToken(string token);
}