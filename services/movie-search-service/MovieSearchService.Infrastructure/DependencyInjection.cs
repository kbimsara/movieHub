using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieSearchService.Application.Interfaces;
using MovieSearchService.Infrastructure.Configuration;
using MovieSearchService.Infrastructure.Elasticsearch;
using MovieSearchService.Infrastructure.Repositories;

namespace MovieSearchService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind Elasticsearch settings
        services.Configure<ElasticsearchSettings>(
            configuration.GetSection("Elasticsearch"));

        // Register Elasticsearch client factory
        services.AddSingleton<ElasticsearchClientFactory>();

        // Register Elasticsearch client
        services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<ElasticsearchClientFactory>();
            return factory.CreateClient();
        });

        // Register repositories
        services.AddScoped<ISearchRepository, SearchRepository>();

        return services;
    }
}
