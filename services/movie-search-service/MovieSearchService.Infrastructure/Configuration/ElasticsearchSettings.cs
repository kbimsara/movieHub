namespace MovieSearchService.Infrastructure.Configuration;

/// <summary>
/// Elasticsearch configuration settings
/// </summary>
public class ElasticsearchSettings
{
    public string Url { get; set; } = "http://localhost:9200";
    public string IndexName { get; set; } = "movies";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
