using ChessMaze.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChessMaze.DatabaseTool;

public class Program
{
    private static async Task Main(string[] args)
    {
        await using var db = new GameDbContextFactory().CreateDbContext([]);
        await db.Database.MigrateAsync();
        Console.WriteLine(DatabasePath.Resolve());
    }
}
