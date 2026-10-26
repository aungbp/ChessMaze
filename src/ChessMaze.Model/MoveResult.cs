namespace ChessMaze.Model;

public enum MoveResult
{
    GameAlreadyCompleted,
    OutOfBounds,
    InvalidMovement,
    PathBlocked,
    InvalidDestination,
    GameCompleted,
    Success,
}
