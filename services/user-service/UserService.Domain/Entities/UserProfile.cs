namespace UserService.Domain.Entities;

/// <summary>
/// UserProfile entity - represents a user's profile information
/// This is a simple domain entity with no business logic
/// </summary>
public class UserProfile
{
    /// <summary>
    /// Primary key for the user profile
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User ID from JWT token (sub claim)
    /// This links the profile to the authenticated user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Display name for the user
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the profile was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
