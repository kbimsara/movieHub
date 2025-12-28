using System.Text.Json.Serialization;

namespace UserService.Application.DTOs;

/// <summary>
/// Response DTO for user profile data
/// </summary>
public class UserProfileResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("username")]
    public string DisplayName { get; set; } = string.Empty;
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Avatar { get; set; }
    public string Role { get; set; } = "user";
    public DateTime CreatedAt { get; set; }
}
