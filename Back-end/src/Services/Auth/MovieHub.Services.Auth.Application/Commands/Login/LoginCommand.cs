using MediatR;
using MovieHub.Shared.Kernel.Application;

namespace MovieHub.Services.Auth.Application.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;

public record LoginResponse(
    Guid UserId,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken
);
