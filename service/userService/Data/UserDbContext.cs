using Microsoft.EntityFrameworkCore;
using UserService.Models.Entities;

namespace UserService.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<LibraryItem> LibraryItems => Set<LibraryItem>();
    public DbSet<WatchHistoryEntry> WatchHistory => Set<WatchHistoryEntry>();
    public DbSet<WatchLaterItem> WatchLater => Set<WatchLaterItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Username).HasMaxLength(128).IsRequired();
            entity.Property(x => x.FirstName).HasMaxLength(128);
            entity.Property(x => x.LastName).HasMaxLength(128);
            entity.Property(x => x.Role).HasMaxLength(32).HasDefaultValue("user");
            entity.Property(x => x.AvatarUrl).HasMaxLength(512);
            entity.Property(x => x.Bio).HasMaxLength(1024);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<LibraryItem>(entity =>
        {
            entity.ToTable("library_items");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Progress).HasDefaultValue(0);
            entity.Property(x => x.LastWatched).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.AddedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.UserId, x.MovieId }).IsUnique();
        });

        modelBuilder.Entity<WatchHistoryEntry>(entity =>
        {
            entity.ToTable("watch_history");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.WatchedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.UserId, x.MovieId }).IsUnique();
        });

        modelBuilder.Entity<WatchLaterItem>(entity =>
        {
            entity.ToTable("watch_later");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.AddedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.UserId, x.MovieId }).IsUnique();
        });
    }
}
