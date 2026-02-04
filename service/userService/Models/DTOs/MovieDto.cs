namespace UserService.Models.DTOs;

public record MovieDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Poster { get; init; } = string.Empty;
    public string? Backdrop { get; init; }
    public string? Trailer { get; init; }
    public int Year { get; init; }
    public int Duration { get; init; }
    public IReadOnlyCollection<string> Genres { get; init; } = Array.Empty<string>();
    public IReadOnlyCollection<string> Tags { get; init; } = Array.Empty<string>();
    public double Rating { get; init; }
    public string Quality { get; init; } = "1080p";
    public IReadOnlyCollection<CastMemberDto> Cast { get; init; } = Array.Empty<CastMemberDto>();
    public string? Director { get; init; }
    public string? StreamUrl { get; init; }
    public string? DownloadUrl { get; init; }
    public string? TorrentMagnet { get; init; }
    public IReadOnlyCollection<SubtitleDto> Subtitles { get; init; } = Array.Empty<SubtitleDto>();
    public DateTime CreatedAt { get; init; }
    public long Views { get; init; }
}

public record CastMemberDto
(
    Guid Id,
    string Name,
    string Character,
    string? Photo
);

public record SubtitleDto
(
    string Language,
    string Url,
    string Label
);
