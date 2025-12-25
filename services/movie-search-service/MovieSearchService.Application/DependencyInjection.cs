using Microsoft.Extensions.DependencyInjection;
using MovieSearchService.Application.Interfaces;
using MovieSearchService.Application.Services;

namespace MovieSearchService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISearchService, SearchService>();
        
        return services;
    }
}
