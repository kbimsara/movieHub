namespace MovieSearchService.Application.DTOs;

/// <summary>
/// DTO for movie search results
/// </summary>
public class MovieSearchResultDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public float Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Elasticsearch relevance score
    /// </summary>
    public double? Score { get; set; }
}
