using AuthService.Application.DTOs;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;

namespace AuthService.Application.Services;

/// <summary>
/// Service responsible for authentication business logic.
/// </summary>
public class AuthenticationService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new UserAlreadyExistsException(request.Email);

        // Hash password and create user entity
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash);

        // Save to database
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        // Generate JWT token
        var claims = new TokenClaimsDto(user.Id, user.Email);
        var token = _tokenGenerator.GenerateToken(claims);

        return new AuthResponseDto(token, user.Email);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            throw new InvalidCredentialsException();

        // Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        // Generate JWT token
        var claims = new TokenClaimsDto(user.Id, user.Email);
        var token = _tokenGenerator.GenerateToken(claims);

        return new AuthResponseDto(token, user.Email);
    }
}
