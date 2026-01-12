namespace MovieManagementService.Domain.Entities;

public class MovieFile
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // video, subtitle, thumbnail
    public long FileSize { get; set; }
    public string Quality { get; set; } = string.Empty; // 480p, 720p, 1080p, 4K
    public string MimeType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public Guid UploadedBy { get; set; }
    public bool IsProcessed { get; set; }
    
    // Navigation property
    public ManagedMovie Movie { get; set; } = null!;
}
