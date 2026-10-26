namespace ChessMaze.Persistence;

public sealed class SavedGame
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string BoardId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsComplete { get; set; }
    public required string HistoryJson { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
