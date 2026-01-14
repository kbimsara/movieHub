using Library.API.Data;
using Library.API.Interfaces;
using LibraryModel = Library.API.Models.UserLibrary;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Repositories;

public class UserLibraryRepository : IUserLibraryRepository
{
    private readonly LibraryDbContext _context;

    public UserLibraryRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<LibraryModel?> GetByIdAsync(Guid id)
    {
        return await _context.UserLibraries.FindAsync(id);
    }

    public async Task<IEnumerable<LibraryModel>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserLibraries
            .Where(l => l.UserId == userId)
            .Include(l => l.Items)
            .ToListAsync();
    }

    public async Task<LibraryModel?> GetByUserIdAndNameAsync(Guid userId, string name)
    {
        return await _context.UserLibraries
            .FirstOrDefaultAsync(l => l.UserId == userId && l.Name == name);
    }

    public async Task AddAsync(LibraryModel library)
    {
        _context.UserLibraries.Add(library);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LibraryModel library)
    {
        _context.UserLibraries.Update(library);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var library = await _context.UserLibraries.FindAsync(id);
        if (library != null)
        {
            _context.UserLibraries.Remove(library);
            await _context.SaveChangesAsync();
        }
    }
}