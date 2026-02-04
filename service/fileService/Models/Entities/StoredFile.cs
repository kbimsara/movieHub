using FileService.Models.Enums;

namespace FileService.Models.Entities;

public class StoredFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public FileCategory FileType { get; set; } = FileCategory.Other;
    public string StoragePath { get; set; } = string.Empty;
    public string AbsolutePath { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public Guid? MovieId { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public double? Duration { get; set; }
    public string? ThumbnailUrl { get; set; }
}
