using Library.API.Data;
using Library.API.Interfaces;
using LibraryItemModel = Library.API.Models.LibraryItem;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Repositories;

public class LibraryItemRepository : ILibraryItemRepository
{
    private readonly LibraryDbContext _context;

    public LibraryItemRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<LibraryItemModel?> GetByIdAsync(Guid id)
    {
        return await _context.LibraryItems.FindAsync(id);
    }

    public async Task<IEnumerable<LibraryItemModel>> GetByLibraryIdAsync(Guid libraryId)
    {
        return await _context.LibraryItems
            .Where(i => i.LibraryId == libraryId)
            .OrderByDescending(i => i.AddedAt)
            .ToListAsync();
    }

    public async Task<LibraryItemModel?> GetByLibraryIdAndMovieIdAsync(Guid libraryId, Guid movieId)
    {
        return await _context.LibraryItems
            .FirstOrDefaultAsync(i => i.LibraryId == libraryId && i.MovieId == movieId);
    }

    public async Task AddAsync(LibraryItemModel item)
    {
        _context.LibraryItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LibraryItemModel item)
    {
        _context.LibraryItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _context.LibraryItems.FindAsync(id);
        if (item != null)
        {
            _context.LibraryItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}