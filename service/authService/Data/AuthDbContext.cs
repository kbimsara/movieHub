using Microsoft.EntityFrameworkCore;
using MovieHub.AuthService.Models;

namespace MovieHub.AuthService.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("auth_users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.Username).HasMaxLength(128).IsRequired();
            entity.Property(u => u.FirstName).HasMaxLength(128);
            entity.Property(u => u.LastName).HasMaxLength(128);
            entity.Property(u => u.Role).HasMaxLength(32).HasDefaultValue("user");
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.AvatarUrl).HasMaxLength(512);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(rt => rt.Token);
            entity.Property(rt => rt.Token).HasMaxLength(256);
            entity.Property(rt => rt.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(rt => rt.ExpiresAt).IsRequired();
            entity.Property(rt => rt.Revoked).HasDefaultValue(false);
            entity.HasIndex(rt => rt.UserId);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
