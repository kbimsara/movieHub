using FileService.Models.Entities;

namespace FileService.Models.DTOs;

public class FileMetadataDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string OriginalName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string MimeType { get; init; } = string.Empty;
    public string FileType { get; init; } = "other";
    public string Url { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public DateTime UploadedAt { get; init; }
    public string? UserId { get; init; }
    public Guid? MovieId { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public double? Duration { get; init; }
    public string? ThumbnailUrl { get; init; }

    public static FileMetadataDto FromEntity(StoredFile entity)
        => new()
        {
            Id = entity.Id,
            FileName = entity.FileName,
            OriginalName = entity.OriginalName,
            FileSize = entity.FileSize,
            MimeType = entity.MimeType,
            FileType = entity.FileType.ToString().ToLowerInvariant(),
            Url = entity.PublicUrl,
            Path = entity.StoragePath,
            UploadedAt = entity.UploadedAt,
            UserId = entity.UserId,
            MovieId = entity.MovieId,
            Width = entity.Width,
            Height = entity.Height,
            Duration = entity.Duration,
            ThumbnailUrl = entity.ThumbnailUrl
        };
}
