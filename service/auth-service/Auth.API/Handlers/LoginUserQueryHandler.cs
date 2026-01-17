using Auth.API.DTOs;
using Auth.API.Interfaces;
using Auth.API.Queries;
using MediatR;
using Shared.JWT;

namespace Auth.API.Handlers;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserQueryHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new Exception("Invalid credentials");
        }

        var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, user.Email);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}