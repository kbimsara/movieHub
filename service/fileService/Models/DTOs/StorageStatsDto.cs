namespace FileService.Models.DTOs;

public class StorageStatsDto
{
    public int TotalFiles { get; init; }
    public long TotalSize { get; init; }
    public int VideoCount { get; init; }
    public int ImageCount { get; init; }
    public int SubtitleCount { get; init; }
    public long UserQuota { get; init; }
    public long UsedQuota { get; init; }
}
