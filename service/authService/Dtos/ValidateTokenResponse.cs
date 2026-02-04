namespace MovieHub.AuthService.Dtos;

public class ValidateTokenResponse
{
    public bool Valid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
