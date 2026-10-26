using ChessMaze.Model;
namespace BCIT615.Assessment1.GamePlayer.Model;

public sealed class MoveValidator
{
    public bool IsValidMovement(
    Position from,
    Position to,
    PieceType piece)
    {
        int dr = to.Row - from.Row;
        int dc = to.Column - from.Column;

        return piece switch
        {
            PieceType.Rook => (dr == 0) ^ (dc == 0),
            PieceType.Bishop => dr != 0 && Math.Abs(dr) == Math.Abs(dc),
            PieceType.Knight =>
                (Math.Abs(dr), Math.Abs(dc)) is (2, 1) or (1, 2),
            PieceType.King => Math.Max(Math.Abs(dr), Math.Abs(dc)) == 1,
            _ => false
        };
    }


    public bool IsPathBlocked(
    Position from,
    Position to,
    PieceType piece,
    Board board)
    {
        int sr = Math.Sign(to.Row - from.Row);
        int sc = Math.Sign(to.Column - from.Column);

        for (Position q = new(from.Row + sr, from.Column + sc);
             q != to;
             q = new(q.Row + sr, q.Column + sc))
        {
            if (board.GetPieceAt(q) != null)
                return true;
        }

        return false;
    }

    public bool InvalidDestination(
    Position to,
    Position targetPosition,
    Board board)
    {
        PieceType? destinationPiece = board.GetPieceAt(to);

        return destinationPiece is null && to != targetPosition;
    }


}