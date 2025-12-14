using BCrypt.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MovieHub.Services.Auth.Domain.Repositories;
using MovieHub.Shared.Kernel.Application;
using MovieHub.Shared.Kernel.Infrastructure.Security;

namespace MovieHub.Services.Auth.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login attempt for non-existent email: {Email}", request.Email);
            return Result.Failure<LoginResponse>("Invalid email or password");
        }

        // Check if account is locked
        if (user.IsLocked())
        {
            _logger.LogWarning("Login attempt for locked account: {Email}", request.Email);
            return Result.Failure<LoginResponse>("Account is temporarily locked. Please try again later.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return Result.Failure<LoginResponse>("Invalid email or password");
        }

        // Generate tokens
        var accessToken = _tokenGenerator.GenerateAccessToken(user.Id, user.Email, new[] { user.Role.ToString() });
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        user.RecordSuccessfulLogin();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Email} logged in successfully", user.Email);

        return Result.Success(new LoginResponse(
            user.Id,
            user.Email,
            user.Role.ToString(),
            accessToken,
            refreshToken
        ));
    }
}
