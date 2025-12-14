using System.Security.Claims;

namespace MovieHub.Shared.Kernel.Infrastructure.Security;

/// <summary>
/// Interface for JWT token generation
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
