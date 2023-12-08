using Microsoft.EntityFrameworkCore;

namespace BggExt.Data;

public class DownloadServiceDbContext : DbContext
{
    public DownloadServiceDbContext(DbContextOptions<DownloadServiceDbContext> options)
        : base(options)
    {
    }
    public DbSet<Models.DownloadLibraryJob> DownloadLibraryJobs { get; set; } = default!;
}
