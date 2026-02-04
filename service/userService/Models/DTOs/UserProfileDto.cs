namespace UserService.Models.DTOs;

public record UserProfileDto
(
    Guid Id,
    string Email,
    string Username,
    string? FirstName,
    string? LastName,
    string? Bio,
    string Role,
    string? Avatar,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
