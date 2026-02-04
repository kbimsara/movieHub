using MovieHub.AuthService.Models;

namespace MovieHub.AuthService.Stores;

public interface IRefreshTokenStore
{
    Task StoreAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default);
    Task InvalidateAsync(string token, CancellationToken cancellationToken = default);
    Task InvalidateAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
