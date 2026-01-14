using System;

namespace Library.API.Models;

public class UserLibrary
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "Favorites", "Watch Later", "Watched"
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; } // Default libraries like "Favorites"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<LibraryItem> Items { get; set; } = new List<LibraryItem>();
}