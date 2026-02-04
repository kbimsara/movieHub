namespace FileService.Models.Requests;

public class MovieUploadMetadata
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Duration { get; set; }
    public string Quality { get; set; } = "1080p";
    public double? Rating { get; set; }
    public List<string> Genres { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<string> Cast { get; set; } = new();
    public string? Director { get; set; }
    public string? Trailer { get; set; }
}
