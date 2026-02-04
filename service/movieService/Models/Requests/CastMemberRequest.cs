using System.ComponentModel.DataAnnotations;

namespace MovieService.Models.Requests;

public class CastMemberRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Character { get; set; }

    [MaxLength(255)]
    public string? Photo { get; set; }
}
