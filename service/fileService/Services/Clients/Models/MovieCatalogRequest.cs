namespace FileService.Services.Clients.Models;

public class MovieCatalogRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Duration { get; set; }
    public string Quality { get; set; } = "1080p";
    public double? Rating { get; set; }
    public List<string> Genres { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string? Director { get; set; }
    public string? Poster { get; set; }
    public string? Backdrop { get; set; }
    public string? Trailer { get; set; }
    public string? StreamUrl { get; set; }
    public string? DownloadUrl { get; set; }
    public List<MovieCastMemberRequest> Cast { get; set; } = new();
}

public class MovieCastMemberRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Character { get; set; }
    public string? Photo { get; set; }
}
