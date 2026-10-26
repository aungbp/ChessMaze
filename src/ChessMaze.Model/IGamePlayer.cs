namespace ChessMaze.Model;

/// <summary>Defines the observable behaviour of a Game Player Model.</summary>
public interface IGamePlayer
{
    Position StartPosition { get; }
    Position TargetPosition { get; }
    Position CurrentPosition { get; }
    bool IsComplete { get; }
    IReadOnlyList<MoveRecord> MoveHistory { get; }
    PieceType? GetPieceAt(Position position);
    MoveResult TryMove(Position destination);
    void Restart();
}
