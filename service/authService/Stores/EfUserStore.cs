using Microsoft.EntityFrameworkCore;
using MovieHub.AuthService.Data;
using MovieHub.AuthService.Models;

namespace MovieHub.AuthService.Stores;

public class EfUserStore(AuthDbContext dbContext) : IUserStore
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("Email is required", nameof(user));
        }

        user.Email = user.Email.Trim().ToLowerInvariant();
        user.Username = user.Username.Trim();

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
    }
}
