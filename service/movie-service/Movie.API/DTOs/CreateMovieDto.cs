using System.ComponentModel.DataAnnotations;

namespace Movie.API.DTOs;

public class CreateMovieDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string PosterUrl { get; set; } = string.Empty;
    [Required]
    [Range(1900, 2100)]
    public int ReleaseYear { get; set; }
}