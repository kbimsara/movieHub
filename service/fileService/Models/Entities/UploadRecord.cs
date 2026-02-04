using FileService.Models.Enums;

namespace FileService.Models.Entities;

public class UploadRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public UploadStatus Status { get; set; } = UploadStatus.Uploading;
    public int Progress { get; set; }
    public string? Error { get; set; }
    public Guid? FileId { get; set; }
    public Guid? MovieId { get; set; }
    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
