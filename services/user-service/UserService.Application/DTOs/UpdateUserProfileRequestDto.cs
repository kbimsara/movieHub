namespace UserService.Application.DTOs;

/// <summary>
/// Request DTO for updating user profile
/// </summary>
public class UpdateUserProfileRequestDto
{
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
}
