using MovieModel = Movie.API.Models.Movie;
using Microsoft.EntityFrameworkCore;

namespace Movie.API.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

    public DbSet<MovieModel> Movies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.PosterUrl).IsRequired();
            entity.Property(e => e.ReleaseYear).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}