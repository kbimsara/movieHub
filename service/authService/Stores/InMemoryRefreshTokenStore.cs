using System.Collections.Concurrent;
using System.Linq;
using MovieHub.AuthService.Models;

namespace MovieHub.AuthService.Stores;

public class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private readonly ConcurrentDictionary<string, RefreshToken> _tokens = new(StringComparer.Ordinal);

    public Task StoreAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _tokens[token.Token] = token;
        return Task.CompletedTask;
    }

    public Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult<RefreshToken?>(null);
        }

        _tokens.TryGetValue(token, out var storedToken);
        return Task.FromResult(storedToken);
    }

    public Task InvalidateAsync(string token, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.CompletedTask;
        }

        if (_tokens.TryGetValue(token, out var storedToken))
        {
            storedToken.Revoke();
        }

        return Task.CompletedTask;
    }

    public Task InvalidateAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var token in _tokens.Values.Where(t => t.UserId == userId))
        {
            token.Revoke();
        }

        return Task.CompletedTask;
    }
}
