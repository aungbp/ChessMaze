using Microsoft.EntityFrameworkCore;

namespace ChessMaze.Persistence;

public sealed class GameDbContext(DbContextOptions<GameDbContext> o) : DbContext(o)
{
    public DbSet<SavedGame> SavedGames => Set<SavedGame>();

    protected override void OnModelCreating(ModelBuilder b) =>
        b.Entity<SavedGame>().HasIndex(x => x.Name).IsUnique();
}
