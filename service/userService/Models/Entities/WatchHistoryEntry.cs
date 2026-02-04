namespace UserService.Models.Entities;

public class WatchHistoryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public double Progress { get; set; }
    public bool Completed { get; set; }
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
}
