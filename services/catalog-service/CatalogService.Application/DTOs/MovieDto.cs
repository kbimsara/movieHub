namespace CatalogService.Application.DTOs;

public class MovieDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int DurationMinutes { get; set; }
    public double Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
