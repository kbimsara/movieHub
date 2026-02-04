using UserService.Models.Entities;

namespace UserService.Services.Interfaces;

public interface IUserDataService
{
    Task<IReadOnlyList<LibraryItem>> GetLibraryAsync(Guid userId, CancellationToken cancellationToken);
    Task<LibraryItem> AddToLibraryAsync(Guid userId, Guid movieId, CancellationToken cancellationToken);
    Task<bool> RemoveFromLibraryAsync(Guid userId, Guid movieId, CancellationToken cancellationToken);
    Task<LibraryItem?> UpdateProgressAsync(Guid userId, Guid movieId, double progress, CancellationToken cancellationToken);
    Task<LibraryItem?> ToggleFavoriteAsync(Guid userId, Guid movieId, CancellationToken cancellationToken);

    Task<IReadOnlyList<WatchHistoryEntry>> GetWatchHistoryAsync(Guid userId, CancellationToken cancellationToken);
    Task<WatchHistoryEntry> AddToWatchHistoryAsync(Guid userId, Guid movieId, double progress, CancellationToken cancellationToken);

    Task<IReadOnlyList<WatchLaterItem>> GetWatchLaterAsync(Guid userId, CancellationToken cancellationToken);
    Task<WatchLaterItem> AddToWatchLaterAsync(Guid userId, Guid movieId, CancellationToken cancellationToken);
    Task<bool> RemoveFromWatchLaterAsync(Guid userId, Guid movieId, CancellationToken cancellationToken);
}
