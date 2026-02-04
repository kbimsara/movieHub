using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Requests;

public class UpdateProfileRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? FirstName { get; set; }

    [MaxLength(128)]
    public string? LastName { get; set; }

    [MaxLength(1024)]
    public string? Bio { get; set; }
}
