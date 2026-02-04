namespace FileService.Models.DTOs;

public class FileUploadResponseDto
{
    public Guid FileId { get; init; }
    public string Url { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public FileMetadataDto Metadata { get; init; } = default!;
}
