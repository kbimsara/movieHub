using System.ComponentModel.DataAnnotations;

namespace MovieService.Models.Entities;

public class CastMember
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid MovieId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Character { get; set; }

    [MaxLength(255)]
    public string? Photo { get; set; }

    public Movie? Movie { get; set; }
}
