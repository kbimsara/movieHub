using Microsoft.Extensions.DependencyInjection;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;

namespace AuthService.Application;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthenticationService>();
        
        return services;
    }
}
