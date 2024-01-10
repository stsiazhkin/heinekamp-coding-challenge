using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Heinekamp.CodingChallenge.FileApi.Persistence;

public class FileInformationDataBaseContext : DbContext
{
    public DbSet<FileInformation> Files { get; set; }

    public FileInformationDataBaseContext(DbContextOptions<FileInformationDataBaseContext> options) 
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileInformation>()
            .ToTable("files_information");
        
        modelBuilder.Entity<FileInformation>()
            .HasKey(k => k.FileId);
        
        modelBuilder.Entity<FileInformation>(
            eb =>
            {
                eb.Property(p => p.FileId).HasColumnName("file_id");
                eb.Property(p => p.FileName).HasColumnName("file_name");
                eb.Property(p => p.DownloadedCount).HasColumnName("downloaded_count");
                eb.Property(p => p.UploadedOn).HasColumnName("uploaded_on");
                eb.Property(p => p.UploadedBy).HasColumnName("uploaded_by");
                eb.Property(p => p.ThumbnailImage).HasColumnName("thumbnail_image");
                eb.Property(p => p.FileContentType).HasColumnName("file_content_type");
            });
    }
}