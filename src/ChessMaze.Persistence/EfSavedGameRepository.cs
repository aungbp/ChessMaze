using System.Text.Json;
using ChessMaze.Model;
using Microsoft.EntityFrameworkCore;

namespace ChessMaze.Persistence;

public sealed class EfSavedGameRepository(IDbContextFactory<GameDbContext> factory) : ISavedGameRepository
{
    public async Task SaveAsync(string n, GameSnapshot s)
    {
        await using var db = await factory.CreateDbContextAsync();
        var e = await db.SavedGames.SingleOrDefaultAsync(x => x.Name == n);
        if (e is null)
        {
            e = new()
            {
                Name = n,
                BoardId = s.BoardId,
                HistoryJson = "",
                UpdatedUtc = DateTime.UtcNow,
            };
            db.Add(e);
        }

        e.BoardId = s.BoardId;
        e.Row = s.CurrentPosition.Row;
        e.Column = s.CurrentPosition.Column;
        e.IsComplete = s.IsComplete;
        e.HistoryJson = JsonSerializer.Serialize(s.History);
        e.UpdatedUtc = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<string>> ListAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.SavedGames.OrderBy(x => x.Name).Select(x => x.Name).ToListAsync();
    }

    public async Task<GameSnapshot?> LoadAsync(string n)
    {
        await using var db = await factory.CreateDbContextAsync();
        var e = await db.SavedGames.AsNoTracking().SingleOrDefaultAsync(x => x.Name == n);
        return e is null
            ? null
            : new(
                e.BoardId,
                new(e.Row, e.Column),
                JsonSerializer.Deserialize<List<MoveRecord>>(e.HistoryJson) ?? [],
                e.IsComplete
            );
    }
}
