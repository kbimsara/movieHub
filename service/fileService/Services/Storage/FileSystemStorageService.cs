using System.IO;
using FileService.Models.Enums;
using FileService.Options;
using FileService.Services.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FileService.Services.Storage;

public class FileSystemStorageService : IFileStorageService
{
    private readonly ILogger<FileSystemStorageService> _logger;
    private readonly string _rootPath;

    public FileSystemStorageService(IOptions<StorageOptions> options, IWebHostEnvironment environment, ILogger<FileSystemStorageService> logger)
    {
        _logger = logger;
        var configuredPath = options.Value.RootPath;
        _rootPath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(environment.ContentRootPath, configuredPath);

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<FileLocation> SaveAsync(IFormFile file, FileCategory category, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var categoryFolder = Path.Combine(_rootPath, category.ToString().ToLowerInvariant());
        Directory.CreateDirectory(categoryFolder);

        var absolutePath = Path.Combine(categoryFolder, fileName);
        await using var stream = new FileStream(absolutePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        var relativePath = Path.GetRelativePath(_rootPath, absolutePath).Replace('\\', '/');
        _logger.LogInformation("Stored file {File} ({Bytes} bytes)", file.FileName, file.Length);

        return new FileLocation(fileName, relativePath, absolutePath);
    }

    public Task DeleteAsync(string absolutePath, CancellationToken cancellationToken)
    {
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }
}
