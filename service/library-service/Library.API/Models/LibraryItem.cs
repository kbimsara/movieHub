using System;

namespace Library.API.Models;

public class LibraryItem
{
    public Guid Id { get; set; }
    public Guid LibraryId { get; set; }
    public Guid MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty; // Denormalized for performance
    public string MoviePosterUrl { get; set; } = string.Empty; // Denormalized for performance
    public int MovieReleaseYear { get; set; } // Denormalized for performance
    public DateTime AddedAt { get; set; }
    public string Notes { get; set; } = string.Empty; // User's personal notes
    public int Rating { get; set; } // 1-5 stars, 0 means not rated

    public UserLibrary Library { get; set; } = null!;
}