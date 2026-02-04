using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models.Entities;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class UserDataService(UserDbContext dbContext) : IUserDataService
{
    public async Task<IReadOnlyList<LibraryItem>> GetLibraryAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.LibraryItems
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AddedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<LibraryItem> AddToLibraryAsync(Guid userId, Guid movieId, CancellationToken cancellationToken)
    {
        var existing = await dbContext.LibraryItems.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, cancellationToken);
        if (existing != null)
        {
            existing.LastWatched = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        var entity = new LibraryItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MovieId = movieId,
            Progress = 0,
            LastWatched = DateTime.UtcNow,
            AddedAt = DateTime.UtcNow
        };

        dbContext.LibraryItems.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> RemoveFromLibraryAsync(Guid userId, Guid movieId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LibraryItems.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dbContext.LibraryItems.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<LibraryItem?> UpdateProgressAsync(Guid userId, Guid movieId, double progress, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LibraryItems.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Progress = progress;
        entity.LastWatched = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<LibraryItem?> ToggleFavoriteAsync(Guid userId, Guid movieId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LibraryItems.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.IsFavorite = !entity.IsFavorite;
        entity.LastWatched = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<WatchHistoryEntry>> GetWatchHistoryAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.WatchHistory
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.WatchedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<WatchHistoryEntry> AddToWatchHistoryAsync(Guid userId, Guid movieId, double progress, CancellationToken cancellationToken)
    {
        var entity = await dbContext.WatchHistory.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, cancellationToken);
        if (entity is null)
        {
            entity = new WatchHistoryEntry
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MovieId = movieId,
                Progress = progress,
                Completed = progress >= 95,
                WatchedAt = DateTime.UtcNow
            };

            dbContext.WatchHistory.Add(entity);
        }
        else
        {
            entity.Progress = progress;
            entity.Completed = progress >= 95;
            entity.WatchedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<WatchLaterItem>> GetWatchLaterAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.WatchLater
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AddedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<WatchLaterItem> AddToWatchLaterAsync(Guid userId, Guid movieId, CancellationToken cancellationToken)
    {
        var existing = await dbContext.WatchLater.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, cancellationToken);
        if (existing != null)
        {
            return existing;
        }

        var entity = new WatchLaterItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MovieId = movieId,
            AddedAt = DateTime.UtcNow
        };

        dbContext.WatchLater.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> RemoveFromWatchLaterAsync(Guid userId, Guid movieId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.WatchLater.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dbContext.WatchLater.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
