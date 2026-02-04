using System.ComponentModel.DataAnnotations;

namespace MovieService.Models.Entities;

public class Movie
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Poster { get; set; }

    [MaxLength(255)]
    public string? Backdrop { get; set; }

    [MaxLength(255)]
    public string? Trailer { get; set; }

    [MaxLength(255)]
    public string? StreamUrl { get; set; }

    [MaxLength(255)]
    public string? DownloadUrl { get; set; }

    [MaxLength(255)]
    public string? TorrentMagnet { get; set; }

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(1, 1000)]
    public int Duration { get; set; }

    public double? Rating { get; set; }

    [Required]
    [MaxLength(32)]
    public string Quality { get; set; } = "1080p";

    public List<string> Genres { get; set; } = new();

    public List<string> Tags { get; set; } = new();

    [MaxLength(120)]
    public string? Director { get; set; }

    public long Views { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CastMember> Cast { get; set; } = new List<CastMember>();
}
