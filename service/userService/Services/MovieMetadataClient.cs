using System.Net.Http.Json;
using UserService.Models.DTOs;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class MovieMetadataClient(HttpClient httpClient, ILogger<MovieMetadataClient> logger) : IMovieMetadataClient
{
    public async Task<MovieDto?> GetMovieAsync(Guid movieId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/movies/{movieId}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Movie service returned {Status} for movie {MovieId}", response.StatusCode, movieId);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<ApiResponse<MovieDto>>(cancellationToken: cancellationToken);
            return payload?.Data;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch movie {MovieId}", movieId);
            return null;
        }
    }

    public async Task<IReadOnlyDictionary<Guid, MovieDto>> GetMoviesAsync(IEnumerable<Guid> movieIds, CancellationToken cancellationToken)
    {
        var distinctIds = movieIds.Distinct().ToArray();
        if (distinctIds.Length == 0)
        {
            return new Dictionary<Guid, MovieDto>();
        }

        var tasks = distinctIds.Select(async id => new
        {
            Id = id,
            Movie = await GetMovieAsync(id, cancellationToken)
        });

        var resolved = await Task.WhenAll(tasks);
        var result = resolved
            .Where(x => x.Movie is not null)
            .ToDictionary(x => x.Id, x => x.Movie!);

        return result;
    }
}
