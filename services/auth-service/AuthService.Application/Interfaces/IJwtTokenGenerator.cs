using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces;

/// <summary>
/// Interface for JWT token generation.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token with the provided user claims.
    /// </summary>
    string GenerateToken(TokenClaimsDto claims);
}
