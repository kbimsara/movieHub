namespace AuthService.Application.Exceptions;

/// <summary>
/// Exception thrown when login credentials are invalid.
/// </summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() 
        : base("The provided email or password is incorrect")
    {
    }
}
