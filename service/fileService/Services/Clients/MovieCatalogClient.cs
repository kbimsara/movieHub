using System.Net.Http.Json;
using FileService.Services.Clients.Models;
using Microsoft.Extensions.Logging;

namespace FileService.Services.Clients;

public class MovieCatalogClient : IMovieCatalogClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MovieCatalogClient> _logger;

    public MovieCatalogClient(HttpClient httpClient, ILogger<MovieCatalogClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Guid> CreateMovieAsync(MovieCatalogRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/movies", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Movie service responded with {Status}: {Body}", response.StatusCode, body);
            throw new InvalidOperationException("Unable to create movie record");
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<MovieServiceResponse>(cancellationToken: cancellationToken);
        if (apiResponse?.Data is null)
        {
            throw new InvalidOperationException("Movie service returned an invalid payload");
        }

        return apiResponse.Data.Id;
    }

    private sealed class MovieServiceResponse
    {
        public bool Success { get; set; }
        public MoviePayload? Data { get; set; }
    }

    private sealed class MoviePayload
    {
        public Guid Id { get; set; }
    }
}
