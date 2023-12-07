using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BggExt.Data;

public class BoardGameDbContext : DbContext
{
    public BoardGameDbContext(DbContextOptions<BoardGameDbContext> options)
        : base(options)
    {
    }

    public DbSet<Models.BoardGame> BoardGames { get; set; } = default!;

    public DbSet<Models.Library> Libraries { get; set; } = default!;

    public DbSet<Models.Mechanic> Mechanics { get; set; } = default!;

    public DbSet<Models.Category> Categories { get; set; } = default!;

    public DbSet<Models.Family> Families { get; set; } = default!;

    public DbSet<Models.Image> Images { get; set; } = default!;

    public DbSet<Models.BoardGameLibraryData> LibraryData { get; set; } = default!;
}