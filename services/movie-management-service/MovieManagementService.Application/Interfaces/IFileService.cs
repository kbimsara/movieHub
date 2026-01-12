using Microsoft.AspNetCore.Http;
using MovieManagementService.Application.DTOs;

namespace MovieManagementService.Application.Interfaces;

public interface IFileService
{
    Task<UploadResponseDto> UploadMovieFileAsync(Guid movieId, IFormFile file, string fileType, string quality, Guid userId);
    Task<MovieFileDto?> GetFileByIdAsync(Guid fileId);
    Task<List<MovieFileDto>> GetMovieFilesAsync(Guid movieId);
    Task<Stream> GetFileStreamAsync(Guid fileId);
    Task<bool> DeleteFileAsync(Guid fileId, Guid userId);
}
