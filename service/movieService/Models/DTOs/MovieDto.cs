namespace MovieService.Models.DTOs;

public record CastMemberDto(
    Guid Id,
    string Name,
    string? Character,
    string? Photo
);

public record MovieDto(
    Guid Id,
    string Title,
    string Description,
    string? Poster,
    string? Backdrop,
    string? Trailer,
    string? StreamUrl,
    string? DownloadUrl,
    string? TorrentMagnet,
    int Year,
    int Duration,
    double? Rating,
    string Quality,
    IReadOnlyList<string> Genres,
    IReadOnlyList<string> Tags,
    string? Director,
    long Views,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<CastMemberDto> Cast
);
