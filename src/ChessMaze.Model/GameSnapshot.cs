namespace ChessMaze.Model;

public sealed record GameSnapshot(
    string BoardId,
    Position CurrentPosition,
    IReadOnlyList<MoveRecord> History,
    bool IsComplete
);
