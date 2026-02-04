using System.ComponentModel.DataAnnotations;

namespace MovieHub.AuthService.Dtos;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
