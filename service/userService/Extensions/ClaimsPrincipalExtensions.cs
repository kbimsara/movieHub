using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UserService.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var raw = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                  principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(raw, out var userId) ? userId : null;
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value ??
               principal.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string? GetUsername(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value ??
               principal.FindFirst(ClaimTypes.Name)?.Value ??
               principal.Identity?.Name;
    }
}
