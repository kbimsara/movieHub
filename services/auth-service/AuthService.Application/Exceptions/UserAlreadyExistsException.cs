namespace AuthService.Application.Exceptions;

/// <summary>
/// Exception thrown when attempting to register a user with an email that already exists.
/// </summary>
public class UserAlreadyExistsException : Exception
{
    public string Email { get; }

    public UserAlreadyExistsException(string email) 
        : base($"User with email '{email}' already exists")
    {
        Email = email;
    }
}
