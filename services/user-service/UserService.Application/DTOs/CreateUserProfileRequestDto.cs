namespace UserService.Application.DTOs;

/// <summary>
/// Request DTO for creating a user profile
/// </summary>
public class CreateUserProfileRequestDto
{
    public string DisplayName { get; set; } = string.Empty;
}
