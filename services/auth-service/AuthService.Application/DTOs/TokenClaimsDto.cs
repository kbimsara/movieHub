namespace AuthService.Application.DTOs;

/// <summary>
/// Data transfer object for token claims used in JWT generation.
/// </summary>
/// <param name="UserId">Unique identifier of the user</param>
/// <param name="Email">User's email address</param>
public record TokenClaimsDto(
    Guid UserId,
    string Email
);
