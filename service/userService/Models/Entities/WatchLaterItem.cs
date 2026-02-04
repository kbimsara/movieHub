namespace UserService.Models.Entities;

public class WatchLaterItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
