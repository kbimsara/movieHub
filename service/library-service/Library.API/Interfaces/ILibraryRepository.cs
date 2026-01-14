using LibraryModel = Library.API.Models.UserLibrary;
using LibraryItemModel = Library.API.Models.LibraryItem;

namespace Library.API.Interfaces;

public interface IUserLibraryRepository
{
    Task<LibraryModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<LibraryModel>> GetByUserIdAsync(Guid userId);
    Task<LibraryModel?> GetByUserIdAndNameAsync(Guid userId, string name);
    Task AddAsync(LibraryModel library);
    Task UpdateAsync(LibraryModel library);
    Task DeleteAsync(Guid id);
}

public interface ILibraryItemRepository
{
    Task<LibraryItemModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<LibraryItemModel>> GetByLibraryIdAsync(Guid libraryId);
    Task<LibraryItemModel?> GetByLibraryIdAndMovieIdAsync(Guid libraryId, Guid movieId);
    Task AddAsync(LibraryItemModel item);
    Task UpdateAsync(LibraryItemModel item);
    Task DeleteAsync(Guid id);
}