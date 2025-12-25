namespace MovieSearchService.Application.DTOs;

/// <summary>
/// DTO for movie search requests with filtering and pagination
/// </summary>
public class MovieSearchRequestDto
{
    /// <summary>
    /// Full-text search query (searches title and description)
    /// </summary>
    public string? Q { get; set; }

    /// <summary>
    /// Filter by specific genre (exact match)
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// Filter by release year
    /// </summary>
    public int? Year { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; } = 10;
}
