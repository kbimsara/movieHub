using MediatR;
using MovieHub.Shared.Kernel.Application;

namespace MovieHub.Services.Auth.Application.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<Result<RegisterResponse>>;

public record RegisterResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken
);
