namespace MovieHub.Shared.Kernel.Events;

/// <summary>
/// Base class for integration events
/// </summary>
public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Event published when a movie is uploaded
/// </summary>
public record MovieUploadedEvent : IntegrationEvent
{
    public Guid UploadId { get; init; }
    public Guid MovieId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public Guid UploadedBy { get; init; }
}

/// <summary>
/// Event published when video processing starts
/// </summary>
public record MovieProcessingStartedEvent : IntegrationEvent
{
    public Guid MovieId { get; init; }
    public Guid ProcessingJobId { get; init; }
}

/// <summary>
/// Event published when video processing completes
/// </summary>
public record MovieProcessedEvent : IntegrationEvent
{
    public Guid MovieId { get; init; }
    public Guid ProcessingJobId { get; init; }
    public string HlsManifestUrl { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public List<VideoQuality> Qualities { get; init; } = new();
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record VideoQuality
{
    public string Resolution { get; init; } = string.Empty; // 1080p, 720p, 480p
    public string PlaylistUrl { get; init; } = string.Empty;
    public long Bitrate { get; init; }
}

/// <summary>
/// Event published when a torrent is created
/// </summary>
public record TorrentCreatedEvent : IntegrationEvent
{
    public Guid TorrentId { get; init; }
    public Guid MovieId { get; init; }
    public string TorrentFileUrl { get; init; } = string.Empty;
    public string MagnetLink { get; init; } = string.Empty;
    public string InfoHash { get; init; } = string.Empty;
}

/// <summary>
/// Event published when torrent seeding starts
/// </summary>
public record TorrentSeedingStartedEvent : IntegrationEvent
{
    public Guid TorrentId { get; init; }
    public Guid UserId { get; init; }
    public DateTime StartedAt { get; init; }
}

/// <summary>
/// Event published when user library is updated
/// </summary>
public record LibraryUpdatedEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public Guid MovieId { get; init; }
    public LibraryAction Action { get; init; }
}

public enum LibraryAction
{
    Added,
    Removed,
    WatchProgressUpdated
}
