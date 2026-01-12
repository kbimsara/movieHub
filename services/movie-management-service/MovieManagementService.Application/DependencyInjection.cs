using Microsoft.Extensions.DependencyInjection;
using MovieManagementService.Application.Interfaces;
using MovieManagementService.Application.Services;

namespace MovieManagementService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IFileService, FileService>();
        
        return services;
    }
}
