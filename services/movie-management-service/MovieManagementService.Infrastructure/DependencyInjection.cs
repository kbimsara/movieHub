using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieManagementService.Application.Interfaces;
using MovieManagementService.Infrastructure.Data;
using MovieManagementService.Infrastructure.Repositories;

namespace MovieManagementService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<MovieManagementContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            // Suppress the pending model changes warning for now
            options.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        // Repositories
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IFileRepository, FileRepository>();

        return services;
    }
}
