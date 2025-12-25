namespace MovieSearchService.Domain.Entities;

/// <summary>
/// Represents a movie document in Elasticsearch
/// This is the search-optimized representation of a movie
/// </summary>
public class SearchMovie
{
    /// <summary>
    /// Unique identifier for the movie
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Movie title - full-text searchable
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Movie description/plot - full-text searchable
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Movie genre - exact match filter
    /// </summary>
    public string Genre { get; set; } = string.Empty;

    /// <summary>
    /// Release year - range filter
    /// </summary>
    public int ReleaseYear { get; set; }

    /// <summary>
    /// Movie rating (0-10) - range filter and sorting
    /// </summary>
    public float Rating { get; set; }

    /// <summary>
    /// When the document was created/indexed
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
