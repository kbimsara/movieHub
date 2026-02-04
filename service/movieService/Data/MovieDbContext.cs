using Microsoft.EntityFrameworkCore;
using MovieService.Models.Entities;

namespace MovieService.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseSerialColumns();

        var movie = modelBuilder.Entity<Movie>();
        movie.ToTable("movies");
        movie.HasKey(m => m.Id);
        movie.Property(m => m.Title).HasMaxLength(200).IsRequired();
        movie.Property(m => m.Description).HasColumnType("text");
        movie.Property(m => m.Quality).HasMaxLength(32).IsRequired();
        movie.Property(m => m.Genres).HasColumnType("text[]");
        movie.Property(m => m.Tags).HasColumnType("text[]");
        movie.Property(m => m.CreatedAt).HasDefaultValueSql("NOW()");
        movie.Property(m => m.UpdatedAt).HasDefaultValueSql("NOW()");
        movie.HasMany(m => m.Cast)
             .WithOne(c => c.Movie)
             .HasForeignKey(c => c.MovieId)
             .OnDelete(DeleteBehavior.Cascade);

        var cast = modelBuilder.Entity<CastMember>();
        cast.ToTable("cast_members");
        cast.HasKey(c => c.Id);
        cast.Property(c => c.Name).HasMaxLength(120).IsRequired();
        cast.Property(c => c.Character).HasMaxLength(120);
    }
}
