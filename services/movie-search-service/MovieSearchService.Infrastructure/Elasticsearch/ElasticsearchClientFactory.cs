using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Options;
using MovieSearchService.Infrastructure.Configuration;

namespace MovieSearchService.Infrastructure.Elasticsearch;

/// <summary>
/// Factory for creating and configuring ElasticsearchClient
/// </summary>
public class ElasticsearchClientFactory
{
    private readonly ElasticsearchSettings _settings;

    public ElasticsearchClientFactory(IOptions<ElasticsearchSettings> settings)
    {
        _settings = settings.Value;
    }

    public ElasticsearchClient CreateClient()
    {
        var settings = new ElasticsearchClientSettings(new Uri(_settings.Url))
            .DefaultIndex(_settings.IndexName)
            .RequestTimeout(TimeSpan.FromSeconds(30))
            .ServerCertificateValidationCallback((sender, certificate, chain, errors) => true)
            .DisableDirectStreaming();

        // Add authentication if credentials are provided
        if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
        {
            settings.Authentication(new BasicAuthentication(_settings.Username, _settings.Password));
        }

        return new ElasticsearchClient(settings);
    }
}
