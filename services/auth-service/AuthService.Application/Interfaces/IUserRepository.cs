namespace AuthService.Application.Interfaces;

/// <summary>
/// Repository interface for User entity operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    Task<Domain.Entities.User?> GetByEmailAsync(string email);

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    Task AddAsync(Domain.Entities.User user);

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    Task<int> SaveChangesAsync();
}
