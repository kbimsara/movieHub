namespace MovieManagementService.Domain.Entities;

public class UploadSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public string Status { get; set; } = string.Empty; // uploading, processing, completed, failed
    public int Progress { get; set; }
    public long TotalBytes { get; set; }
    public long UploadedBytes { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
