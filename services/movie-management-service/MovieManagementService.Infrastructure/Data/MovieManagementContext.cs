using Microsoft.EntityFrameworkCore;
using MovieManagementService.Domain.Entities;

namespace MovieManagementService.Infrastructure.Data;

public class MovieManagementContext : DbContext
{
    public MovieManagementContext(DbContextOptions<MovieManagementContext> options)
        : base(options)
    {
    }

    public DbSet<ManagedMovie> Movies { get; set; }
    public DbSet<MovieFile> MovieFiles { get; set; }
    public DbSet<UploadSession> UploadSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ManagedMovie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.HasIndex(e => e.CreatedBy);
            entity.HasIndex(e => e.IsPublished);
        });

        modelBuilder.Entity<MovieFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.Quality).HasMaxLength(20);
            entity.Property(e => e.MimeType).HasMaxLength(100);
            
            entity.HasOne(e => e.Movie)
                .WithMany(m => m.Files)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.MovieId);
            entity.HasIndex(e => e.UploadedBy);
        });

        modelBuilder.Entity<UploadSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.MovieId);
        });
    }
}
