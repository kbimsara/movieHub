using UserService.Models.DTOs;

namespace UserService.Services.Interfaces;

public interface IMovieMetadataClient
{
    Task<MovieDto?> GetMovieAsync(Guid movieId, CancellationToken cancellationToken);
    Task<IReadOnlyDictionary<Guid, MovieDto>> GetMoviesAsync(IEnumerable<Guid> movieIds, CancellationToken cancellationToken);
}
