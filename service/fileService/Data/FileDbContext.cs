using FileService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileService.Data;

public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
    {
    }

    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();
    public DbSet<UploadRecord> UploadRecords => Set<UploadRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoredFile>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileType).HasConversion<string>();
            entity.Property(x => x.FileName).HasMaxLength(260);
            entity.Property(x => x.OriginalName).HasMaxLength(260);
            entity.Property(x => x.MimeType).HasMaxLength(128);
            entity.Property(x => x.StoragePath).HasMaxLength(500);
            entity.Property(x => x.AbsolutePath).HasMaxLength(500);
            entity.Property(x => x.PublicUrl).HasMaxLength(500);
            entity.Property(x => x.UserId).HasMaxLength(64);
        });

        modelBuilder.Entity<UploadRecord>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.FileName).HasMaxLength(260);
            entity.Property(x => x.UserId).HasMaxLength(64);
        });
    }
}
