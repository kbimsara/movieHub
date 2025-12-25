namespace AuthService.Application.DTOs;

/// <summary>
/// Response model containing authentication token and user info.
/// </summary>
/// <param name="Token">JWT authentication token</param>
/// <param name="Email">User's email address</param>
public record AuthResponseDto(
    string Token,
    string Email
);
