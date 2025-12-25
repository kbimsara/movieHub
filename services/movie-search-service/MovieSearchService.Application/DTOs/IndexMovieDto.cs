namespace MovieSearchService.Application.DTOs;

/// <summary>
/// DTO for indexing a movie document
/// </summary>
public class IndexMovieDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public float Rating { get; set; }
}
