using Microsoft.EntityFrameworkCore;
using MovieHub.AuthService.Data;
using MovieHub.AuthService.Models;

namespace MovieHub.AuthService.Stores;

public class EfRefreshTokenStore(AuthDbContext dbContext) : IRefreshTokenStore
{
    public async Task StoreAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        dbContext.RefreshTokens.Add(token);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return await dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task InvalidateAsync(string token, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var entity = await dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.Revoke();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task InvalidateAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.Revoked)
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            return;
        }

        foreach (var token in tokens)
        {
            token.Revoke();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
