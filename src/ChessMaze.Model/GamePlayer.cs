using BCIT615.Assessment1.GamePlayer.Model;
using System.ComponentModel.DataAnnotations;

namespace ChessMaze.Model;

public sealed class GamePlayer : IGamePlayer
{
    private readonly Board _board;
    private readonly MoveValidator _validator;
    private readonly List<MoveRecord> _moveHistory = new();

    private readonly Position _startPosition;
    private readonly Position _targetPosition;

    private Position _currentPosition;
    private bool _isComplete;
    public GamePlayer()
    {
        _board = new Board(
            ReferenceBoardData.Rows,
            ReferenceBoardData.Columns,
            ReferenceBoardData.Pieces);

        _startPosition = ReferenceBoardData.Start;
        _targetPosition = ReferenceBoardData.Target;

        ValidateInitialState();

        _validator = new MoveValidator();
        _currentPosition = _startPosition;
        _isComplete = false;
    }
    public Position StartPosition =>
        _startPosition;

    public Position TargetPosition =>
        _targetPosition;

    public Position CurrentPosition =>
        _currentPosition;

    public bool IsComplete =>
        _isComplete;

    public IReadOnlyList<MoveRecord> MoveHistory =>
        _moveHistory.AsReadOnly();

    public object Board { get; set; }

    public PieceType? GetPieceAt(
        Position position)
    {
        return _board.GetPieceAt(position);
    }

    public MoveResult TryMove(Position destination)
    {
        MoveResult result = ValidateMove(destination);

        if (result != MoveResult.Success)
            return result;

        Position from = _currentPosition;
        PieceType piece = _board.GetPieceAt(from)!.Value;

        _currentPosition = destination;

        _moveHistory.Add(new MoveRecord(
            _moveHistory.Count + 1, from, destination, piece));

        if (destination == _targetPosition)
        {
            _isComplete = true;
            return MoveResult.GameCompleted;
        }

        return MoveResult.Success;
    }

    public void Restart()
    {
        _currentPosition = _startPosition;
        _moveHistory.Clear();
        _isComplete = false;
    }

    public GameSnapshot Capture() =>
        new("Reference-v1.6", CurrentPosition, _moveHistory.ToList(), IsComplete);

public void Restore(GameSnapshot snapshot)
    {
        if (snapshot.BoardId != "Reference-v1.6")
            throw new InvalidOperationException("Unknown board.");

        if (!_board.IsInBounds(snapshot.CurrentPosition))
            throw new InvalidOperationException("Invalid position.");

        if (snapshot.History
            .Select((move, index) => move.SequenceNumber == index + 1)
            .Any(valid => !valid))
        {
            throw new InvalidOperationException("Invalid history ordering.");
        }

        if (snapshot.IsComplete !=
            (snapshot.CurrentPosition == _targetPosition))
        {
            throw new InvalidOperationException("Incoherent completion state.");
        }

        _currentPosition = snapshot.CurrentPosition;

        _moveHistory.Clear();
        _moveHistory.AddRange(snapshot.History);

        _isComplete = snapshot.IsComplete;
    }

    private void ValidateInitialState()
    {
        if (!_board.IsInBounds(_startPosition))
        {
            throw new ArgumentOutOfRangeException(
                nameof(_startPosition));
        }

        if (!_board.IsInBounds(_targetPosition))
        {
            throw new ArgumentOutOfRangeException(
                nameof(_targetPosition));
        }

        if (_startPosition == _targetPosition)
        {
            throw new ArgumentException(
                "Start and target cannot be the same.");
        }

        if (_board.GetPieceAt(_startPosition) is null)
        {
            throw new ArgumentException(
                "Start position must contain a piece.");
        }
    }
    private MoveResult ValidateMove(Position destination)
    {
        // 1. GameAlreadyCompleted
        if (_isComplete)
            return MoveResult.GameAlreadyCompleted;

        // 2. OutOfBounds
        if (!_board.IsInBounds(destination))
            return MoveResult.OutOfBounds;

        // Get the piece at the current position
        PieceType? movementPiece = GetPieceAt(_currentPosition);

        if (movementPiece is null)
            throw new InvalidOperationException(
                "Current position must contain a piece.");

        // 3. InvalidMovement
        if (!_validator.IsValidMovement(
                _currentPosition, destination, movementPiece.Value))
        {
            return MoveResult.InvalidMovement;
        }

        // 4. PathBlocked
        bool pathBlocked = false;

        if (movementPiece is PieceType.Rook or PieceType.Bishop)
        {
            pathBlocked = _validator.IsPathBlocked(
                _currentPosition,
                destination,
                movementPiece.Value,
                _board);
        }

        if (pathBlocked)
        {
            return MoveResult.PathBlocked;
        }

        // 5. InvalidDestination
        if (_validator.InvalidDestination(
                destination, _targetPosition, _board))
        {
            return MoveResult.InvalidDestination;
        }

        // 6. The movement is valid
        return MoveResult.Success;
    }
}
