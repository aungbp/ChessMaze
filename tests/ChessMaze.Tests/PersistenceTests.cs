using ChessMaze.Model;
using ChessMaze.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessMaze.Tests;

[TestClass]
public class PersistenceTests
{
    [TestMethod]
    public async Task UnavailableDatabaseDoesNotChangeCurrentGame()
    {
        var g = new GamePlayer();
        Assert.AreEqual(MoveResult.Success, g.TryMove(new Position(5, 3)));
        var before = g.Capture();
        var result = await new GamePersistenceService(new UnavailableRepository()).TryLoadAsync(
            "A",
            g
        );
        Assert.AreEqual(LoadGameResult.Unavailable, result);
        var after = g.Capture();
        Assert.AreEqual(before.BoardId, after.BoardId);
        Assert.AreEqual(before.CurrentPosition, after.CurrentPosition);
        Assert.AreEqual(before.IsComplete, after.IsComplete);
        Assert.AreEqual(before.History.Count, after.History.Count);
        for (int i = 0; i < before.History.Count; i++)
            Assert.AreEqual(before.History[i], after.History[i]);
    }

    sealed class UnavailableRepository : ISavedGameRepository
    {
        public Task SaveAsync(string n, GameSnapshot s) => Task.CompletedTask;

        public Task<IReadOnlyList<string>> ListAsync() =>
            Task.FromException<IReadOnlyList<string>>(
                new InvalidOperationException("Controlled unavailable database.")
            );

        public Task<GameSnapshot?> LoadAsync(string n) =>
            Task.FromException<GameSnapshot?>(
                new InvalidOperationException("Controlled unavailable database.")
            );
    }
}
