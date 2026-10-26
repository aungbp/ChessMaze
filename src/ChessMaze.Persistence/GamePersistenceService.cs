using ChessMaze.Model;
using Microsoft.EntityFrameworkCore;

namespace ChessMaze.Persistence;

public sealed class GamePersistenceService(ISavedGameRepository repository)
{
    public async Task<LoadGameResult> TryLoadAsync(string n, GamePlayer current)
    {
        GameSnapshot? s;
        try
        {
            s = await repository.LoadAsync(n);
        }
        catch (Exception ex)
            when (ex
                    is Microsoft.Data.Sqlite.SqliteException
                        or DbUpdateException
                        or InvalidOperationException
            )
        {
            return LoadGameResult.Unavailable;
        }

        if (s is null)
            return LoadGameResult.NotFound;
        try
        {
            var candidate = new GamePlayer();
            candidate.Restore(s);
            current.Restore(candidate.Capture());
            return LoadGameResult.Loaded;
        }
        catch (InvalidOperationException)
        {
            return LoadGameResult.InvalidData;
        }
    }
}
