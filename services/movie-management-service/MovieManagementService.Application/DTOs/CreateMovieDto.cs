namespace MovieManagementService.Application.DTOs;

public class CreateMovieDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int DurationMinutes { get; set; }
    public double Rating { get; set; }
    public string PosterUrl { get; set; } = string.Empty;
    public string TrailerUrl { get; set; } = string.Empty;
    public string Quality { get; set; } = string.Empty;
}
