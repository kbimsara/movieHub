namespace FileService.Models.DTOs;

public class UploadMovieResultDto
{
    public Guid UploadId { get; init; }
    public Guid MovieId { get; init; }
    public FileUploadResponseDto File { get; init; } = default!;
}
