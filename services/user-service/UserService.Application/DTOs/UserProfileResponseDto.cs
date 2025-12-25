namespace UserService.Application.DTOs;

/// <summary>
/// Response DTO for user profile data
/// </summary>
public class UserProfileResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
