using MovieHub.Shared.Kernel.Domain;

namespace MovieHub.Services.Auth.Domain.Entities;

/// <summary>
/// User aggregate root
/// </summary>
public class User : AggregateRoot
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    private User() { } // For EF Core

    private User(string email, string passwordHash, string firstName, string lastName, UserRole role)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        Status = UserStatus.Active;
        FailedLoginAttempts = 0;
    }

    public static User Create(string email, string passwordHash, string firstName, string lastName, UserRole role = UserRole.User)
    {
        return new User(email, passwordHash, firstName, lastName, role);
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        Update();
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        Update();
    }

    public void SetRefreshToken(string refreshToken, DateTime expiresAt)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = expiresAt;
        Update();
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
        Update();
    }

    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        Update();
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        
        if (FailedLoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        }
        
        Update();
    }

    public bool IsLocked()
    {
        return LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        Update();
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        Update();
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;
        Update();
    }
}

public enum UserRole
{
    User = 0,
    Admin = 1
}

public enum UserStatus
{
    Active = 0,
    Inactive = 1,
    Suspended = 2
}
