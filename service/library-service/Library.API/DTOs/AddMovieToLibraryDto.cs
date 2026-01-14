using System.ComponentModel.DataAnnotations;

namespace Library.API.DTOs;

public class AddMovieToLibraryDto
{
    [Required]
    public Guid MovieId { get; set; }

    [Required]
    [StringLength(255)]
    public string MovieTitle { get; set; } = string.Empty;

    public string MoviePosterUrl { get; set; } = string.Empty;

    [Required]
    public int MovieReleaseYear { get; set; }

    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    [Range(0, 5)]
    public int Rating { get; set; } = 0;
}