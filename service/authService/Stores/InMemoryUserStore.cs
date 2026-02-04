using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MovieHub.AuthService.Models;

namespace MovieHub.AuthService.Stores;

public class InMemoryUserStore : IUserStore
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();
    private readonly ConcurrentDictionary<string, Guid> _userIdsByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<InMemoryUserStore> _logger;

    public InMemoryUserStore(IPasswordHasher<User> passwordHasher, ILogger<InMemoryUserStore> logger)
    {
        _logger = logger;
        SeedDemoUser(passwordHasher);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult<User?>(null);
        }

        if (_userIdsByEmail.TryGetValue(email, out var id) && _users.TryGetValue(id, out var user))
        {
            return Task.FromResult<User?>(user);
        }

        return Task.FromResult<User?>(null);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("Email is required", nameof(user));
        }

        _users[user.Id] = user;
        _userIdsByEmail[user.Email] = user.Id;
        return Task.CompletedTask;
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(_userIdsByEmail.ContainsKey(email));
    }

    private void SeedDemoUser(IPasswordHasher<User> passwordHasher)
    {
        var demoUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "demo@moviehub.local",
            Username = "demo",
            FirstName = "Demo",
            LastName = "User",
            Role = "admin",
            CreatedAt = DateTime.UtcNow.AddDays(-14)
        };

        demoUser.PasswordHash = passwordHasher.HashPassword(demoUser, "Pass@123");
        _users[demoUser.Id] = demoUser;
        _userIdsByEmail[demoUser.Email] = demoUser.Id;
        _logger.LogInformation("Seeded demo user demo@moviehub.local with password Pass@123");
    }
}
