using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UserService.API.Extensions;

/// <summary>
/// Extension methods for configuring JWT authentication
/// This service ONLY validates tokens, it does NOT create them
/// </summary>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get JWT settings from appsettings.json
        // MUST match the Auth Service configuration
        var jwtSecret = configuration["JwtSettings:Secret"] 
            ?? throw new InvalidOperationException("JWT Secret not configured");
        var jwtIssuer = configuration["JwtSettings:Issuer"] 
            ?? throw new InvalidOperationException("JWT Issuer not configured");
        var jwtAudience = configuration["JwtSettings:Audience"] 
            ?? throw new InvalidOperationException("JWT Audience not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
            };
        });

        return services;
    }
}
