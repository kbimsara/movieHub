using Movie.API.Data;
using Movie.API.Interfaces;
using MovieModel = Movie.API.Models.Movie;
using Microsoft.EntityFrameworkCore;

namespace Movie.API.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MovieDbContext _context;

    public MovieRepository(MovieDbContext context)
    {
        _context = context;
    }

    public async Task<MovieModel?> GetByIdAsync(Guid id)
    {
        return await _context.Movies.FindAsync(id);
    }

    public async Task<IEnumerable<MovieModel>> GetAllAsync()
    {
        return await _context.Movies.ToListAsync();
    }

    public async Task AddAsync(MovieModel movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MovieModel movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie != null)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
    }
}