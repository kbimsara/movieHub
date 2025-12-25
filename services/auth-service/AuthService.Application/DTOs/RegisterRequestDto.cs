namespace AuthService.Application.DTOs;

/// <summary>
/// Request model for user registration.
/// </summary>
/// <param name="Email">User's email address (will be used as username)</param>
/// <param name="Password">Plain text password (will be hashed before storage)</param>
public record RegisterRequestDto(
    string Email,
    string Password
);
