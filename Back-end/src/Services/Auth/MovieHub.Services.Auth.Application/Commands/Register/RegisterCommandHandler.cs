using BCrypt.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MovieHub.Services.Auth.Domain.Entities;
using MovieHub.Services.Auth.Domain.Repositories;
using MovieHub.Shared.Kernel.Application;
using MovieHub.Shared.Kernel.Infrastructure.Security;

namespace MovieHub.Services.Auth.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return Result.Failure<RegisterResponse>("Email already registered");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = User.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            UserRole.User
        );

        // Generate tokens
        var accessToken = _tokenGenerator.GenerateAccessToken(user.Id, user.Email, new[] { user.Role.ToString() });
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Email} registered successfully with ID {UserId}", user.Email, user.Id);

        return Result.Success(new RegisterResponse(
            user.Id,
            user.Email,
            accessToken,
            refreshToken
        ));
    }
}
