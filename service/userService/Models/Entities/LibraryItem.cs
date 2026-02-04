namespace UserService.Models.Entities;

public class LibraryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public double Progress { get; set; }
    public DateTime LastWatched { get; set; } = DateTime.UtcNow;
    public bool IsFavorite { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
