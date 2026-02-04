using FileService.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace FileService.Services.Abstractions;

public record FileLocation(string FileName, string RelativePath, string AbsolutePath);

public interface IFileStorageService
{
    Task<FileLocation> SaveAsync(IFormFile file, FileCategory category, CancellationToken cancellationToken);
    Task DeleteAsync(string absolutePath, CancellationToken cancellationToken);
}
