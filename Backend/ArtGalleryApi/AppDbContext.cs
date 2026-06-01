using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // This tells EF Core to create a table named "Artworks"
    public DbSet<ArtPiece> Artworks => Set<ArtPiece>();

    // This configures the connection to your SQLite database
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=artgallery.db");
}