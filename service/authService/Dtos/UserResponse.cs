namespace MovieHub.AuthService.Dtos;

public class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Role { get; set; } = "user";
    public string CreatedAt { get; set; } = string.Empty;
}
