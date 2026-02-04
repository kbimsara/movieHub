using FileService.Services.Clients.Models;

namespace FileService.Services.Clients;

public interface IMovieCatalogClient
{
    Task<Guid> CreateMovieAsync(MovieCatalogRequest request, CancellationToken cancellationToken);
}
