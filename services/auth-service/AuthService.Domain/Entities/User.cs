namespace AuthService.Domain.Entities;

/// <summary>
/// Represents a user in the authentication system.
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// User's email address (used as username).
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// BCrypt hashed password.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Timestamp when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private User() { }

    /// <summary>
    /// Creates a new user instance.
    /// </summary>
    public static User Create(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }
}
