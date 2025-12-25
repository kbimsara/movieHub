using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
    {
        var movies = await _movieRepository.GetAllAsync();
        return movies.Select(MapToDto);
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        return movie != null ? MapToDto(movie) : null;
    }

    public async Task<MovieDto> CreateMovieAsync(CreateMovieDto createMovieDto)
    {
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = createMovieDto.Title,
            Description = createMovieDto.Description,
            Genre = createMovieDto.Genre,
            ReleaseYear = createMovieDto.ReleaseYear,
            DurationMinutes = createMovieDto.DurationMinutes,
            Rating = createMovieDto.Rating,
            CreatedAt = DateTime.UtcNow
        };

        var createdMovie = await _movieRepository.CreateAsync(movie);
        await _movieRepository.SaveChangesAsync();

        return MapToDto(createdMovie);
    }

    private static MovieDto MapToDto(Movie movie)
    {
        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            Genre = movie.Genre,
            ReleaseYear = movie.ReleaseYear,
            DurationMinutes = movie.DurationMinutes,
            Rating = movie.Rating,
            CreatedAt = movie.CreatedAt
        };
    }
}
