using MediatR;
using MovieHub.Shared.Kernel.Application;

namespace MovieHub.Services.Auth.Application.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<Result<RefreshTokenResponse>>;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken
);
