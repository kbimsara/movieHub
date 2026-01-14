using LibraryModel = Library.API.Models.UserLibrary;
using LibraryItemModel = Library.API.Models.LibraryItem;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<LibraryModel> UserLibraries { get; set; }
    public DbSet<LibraryItemModel> LibraryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
        });

        modelBuilder.Entity<LibraryItemModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LibraryId).IsRequired();
            entity.Property(e => e.MovieId).IsRequired();
            entity.Property(e => e.MovieTitle).IsRequired().HasMaxLength(255);
            entity.Property(e => e.MoviePosterUrl).HasMaxLength(500);
            entity.Property(e => e.AddedAt).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.Rating).HasDefaultValue(0);

            entity.HasIndex(e => new { e.LibraryId, e.MovieId }).IsUnique();

            entity.HasOne(e => e.Library)
                .WithMany(l => l.Items)
                .HasForeignKey(e => e.LibraryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}