using System.ComponentModel.DataAnnotations;

namespace User.API.DTOs;

public class UpdateUserProfileDto
{
    [StringLength(100, MinimumLength = 1)]
    public string? FirstName { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string? LastName { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }
}