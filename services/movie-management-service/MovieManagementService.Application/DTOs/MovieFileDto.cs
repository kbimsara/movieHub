namespace MovieManagementService.Application.DTOs;

public class MovieFileDto
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string Quality { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public Guid UploadedBy { get; set; }
    public bool IsProcessed { get; set; }
}
