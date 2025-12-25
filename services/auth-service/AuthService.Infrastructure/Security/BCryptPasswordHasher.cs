using AuthService.Application.Interfaces;

namespace AuthService.Infrastructure.Security;

/// <summary>
/// Password hasher implementation using BCrypt.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
