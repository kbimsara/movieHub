namespace AuthService.Application.DTOs;

/// <summary>
/// Request model for user login.
/// </summary>
/// <param name="Email">User's email address</param>
/// <param name="Password">Plain text password</param>
public record LoginRequestDto(
    string Email,
    string Password
);
