namespace ChessMaze.Model;

public sealed record MoveRecord(
    int SequenceNumber,
    Position From,
    Position To,
    PieceType MovementPiece
);
