using System.ComponentModel.DataAnnotations;

namespace MovieService.Models.Requests;

public class CreateMovieRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(1, 1000)]
    public int Duration { get; set; }

    [Required]
    [MaxLength(32)]
    public string Quality { get; set; } = "1080p";

    public double? Rating { get; set; }

    public List<string> Genres { get; set; } = new();

    public List<string> Tags { get; set; } = new();

    [MaxLength(120)]
    public string? Director { get; set; }

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

    public List<CastMemberRequest> Cast { get; set; } = new();
}
