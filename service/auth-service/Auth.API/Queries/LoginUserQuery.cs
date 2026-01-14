using Auth.API.DTOs;
using MediatR;

namespace Auth.API.Queries;

public class LoginUserQuery : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}