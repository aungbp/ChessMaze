using ChessMaze.Model;

namespace ChessMaze.Model;

public sealed class Board
{
    private readonly int _rows;
    private readonly int _columns;
    private readonly IReadOnlyDictionary<Position, PieceType> _pieces;

    public Board(
        int rows,
        int columns,
        IReadOnlyDictionary<Position, PieceType> pieces)
    {
        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows));
        }

        if (columns <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columns));
        }

        ArgumentNullException.ThrowIfNull(pieces);

        _rows = rows;
        _columns = columns;

        // Validate every supplied piece.
        foreach (KeyValuePair<Position, PieceType> item in pieces)
        {
            if (!IsInBounds(item.Key))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pieces),
                    "A piece is outside the board.");
            }

            if (!Enum.IsDefined(item.Value))
            {
                throw new ArgumentException(
                    "The board contains an unsupported piece type.",
                    nameof(pieces));
            }
        }

        // Defensive copy.
        _pieces = new Dictionary<Position, PieceType>(pieces);
    }

    public bool IsInBounds(Position position)
    {
        return position.Row >= 0 &&
               position.Row < _rows &&
               position.Column >= 0 &&
               position.Column < _columns;
    }

    public PieceType? GetPieceAt(Position position)
    {
        if (!IsInBounds(position))
        {
            throw new ArgumentOutOfRangeException(
                nameof(position));
        }

        if (_pieces.TryGetValue(
                position,
                out PieceType piece))
        {
            return piece;
        }

        return null;
    }
}