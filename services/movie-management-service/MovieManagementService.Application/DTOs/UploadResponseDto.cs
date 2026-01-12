namespace MovieManagementService.Application.DTOs;

public class UploadResponseDto
{
    public Guid SessionId { get; set; }
    public Guid MovieId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Progress { get; set; }
    public string Message { get; set; } = string.Empty;
    public MovieFileDto? File { get; set; }
}
