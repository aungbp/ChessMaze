using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChessMaze.Persistence;

public sealed class GameDbContextFactory :
    IDbContextFactory<GameDbContext>,
    IDesignTimeDbContextFactory<GameDbContext>
{
    private static GameDbContext CreateContext()
    {
        var path = DatabasePath.Resolve();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseSqlite($"Data Source={path}")
            .Options;

        return new GameDbContext(options);
    }

    public GameDbContext CreateDbContext() => CreateContext();

    public GameDbContext CreateDbContext(string[] args) => CreateContext();

    public Task<GameDbContext> CreateDbContextAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(CreateContext());
    }
}