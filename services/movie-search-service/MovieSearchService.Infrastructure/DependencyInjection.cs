using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieSearchService.Application.Interfaces;
using MovieSearchService.Infrastructure.Configuration;
using MovieSearchService.Infrastructure.Repositories;

namespace MovieSearchService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind Elasticsearch settings (kept for future use)
        services.Configure<ElasticsearchSettings>(
            configuration.GetSection("Elasticsearch"));

        // Register repositories
        services.AddScoped<ISearchRepository, SearchRepository>();

        return services;
    }
}
