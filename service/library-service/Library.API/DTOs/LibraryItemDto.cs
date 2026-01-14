namespace Library.API.DTOs;

public class LibraryItemDto
{
    public Guid Id { get; set; }
    public Guid LibraryId { get; set; }
    public Guid MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string MoviePosterUrl { get; set; } = string.Empty;
    public int MovieReleaseYear { get; set; }
    public DateTime AddedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int Rating { get; set; }
}