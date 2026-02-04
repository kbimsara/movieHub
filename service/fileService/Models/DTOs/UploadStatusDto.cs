namespace FileService.Models.DTOs;

public class UploadStatusDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public int Progress { get; init; }
    public string Status { get; init; } = "uploading";
    public string? Error { get; init; }
    public Guid? MovieId { get; init; }
}
