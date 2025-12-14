using Microsoft.EntityFrameworkCore;
using MovieHub.Services.Auth.Domain.Entities;
using MovieHub.Shared.Kernel.Infrastructure.Persistence;

namespace MovieHub.Services.Auth.Infrastructure.Persistence;

public class AuthDbContext : BaseDbContext
{
    public DbSet<User> Users => Set<User>();

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
    }
}
