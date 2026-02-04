using System.IO;

namespace FileService.Options;

public class StorageOptions
{
    public string RootPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "Uploads");
    public string PublicBaseUrl { get; set; } = "http://localhost:5000/api/files";
    public string DownloadPathTemplate { get; set; } = "/api/files/{id}/download";
    public string StreamPathTemplate { get; set; } = "/api/files/{id}/stream";
}
