using MediatR;
using Microsoft.Extensions.Logging;
using MovieHub.Services.Auth.Domain.Repositories;
using MovieHub.Shared.Kernel.Application;
using MovieHub.Shared.Kernel.Infrastructure.Security;

namespace MovieHub.Services.Auth.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Refresh token not found");
            return Result.Failure<RefreshTokenResponse>("Invalid refresh token");
        }

        if (user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token expired for user {UserId}", user.Id);
            return Result.Failure<RefreshTokenResponse>("Refresh token expired");
        }

        // Generate new tokens
        var accessToken = _tokenGenerator.GenerateAccessToken(user.Id, user.Email, new[] { user.Role.ToString() });
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tokens refreshed for user {UserId}", user.Id);

        return Result.Success(new RefreshTokenResponse(
            accessToken,
            refreshToken
        ));
    }
}
