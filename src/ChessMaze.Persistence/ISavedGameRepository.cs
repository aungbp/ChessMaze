using ChessMaze.Model;

namespace ChessMaze.Persistence;

public interface ISavedGameRepository
{
    Task SaveAsync(string n, GameSnapshot s);
    Task<IReadOnlyList<string>> ListAsync();
    Task<GameSnapshot?> LoadAsync(string n);
}
