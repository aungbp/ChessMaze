using ChessMaze.Model;
using ChessMaze.Persistence;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using BCIT615.Assessment1.GamePlayer.Model;

namespace ChessMaze.Vieww.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GamePlayer _gamePlayer;

        public ObservableCollection<SquareViewModel> Squares { get; }
            = new ObservableCollection<SquareViewModel>();

        public ObservableCollection<string> MoveHistory { get; }
            = new ObservableCollection<string>();

        public ICommand SelectSquareCommand { get; }

        public ICommand RestartCommand { get; }

        public MainViewModel()
        {
            // Create the game using your existing Model.
            _gamePlayer = new GamePlayer();

            // Connect the UI commands.
            SelectSquareCommand = new RelayCommand(SelectSquare);

            RestartCommand = new RelayCommand(
                _ => RestartGame());

            // Create all 36 squares.
            CreateBoardSquares();
        }

        private void CreateBoardSquares()
        {
            Squares.Clear();

            // Get dimensions from the reference board data.
            int rows = ReferenceBoardData.Rows;
            int columns = ReferenceBoardData.Columns;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    // Create the position for this square.
                    Position position = new Position(row, column);

                    // Ask the Model which piece is at this position.
                    PieceType? piece =
                        _gamePlayer.GetPieceAt(position);

                    // Create the ViewModel for this square.
                    SquareViewModel square = new SquareViewModel(
                        position: position,
                        piece: piece,
                        isStart: position == _gamePlayer.StartPosition,
                        isTarget: position == _gamePlayer.TargetPosition,
                        isCurrentPosition:
                            position == _gamePlayer.CurrentPosition
                    );

                    Squares.Add(square);
                }
            }
        }

        private void SelectSquare(object? parameter)
        {
            if (parameter is not Position destination)
                return;

            // Ask the Model to validate and perform the move.
            MoveResult result = _gamePlayer.TryMove(destination);
            // Update the current-position highlight.
            RefreshBoard();

            // Display the result in Move History.
            switch (result)
            {
                case MoveResult.Success:
                    MoveHistory.Add(
                        $"Move {result}: ({destination.Row}, {destination.Column})");
                    break;

                case MoveResult.GameCompleted:
                    MoveHistory.Add(
                        $"Game completed at ({destination.Row}, {destination.Column})");
                    break;

                case MoveResult.GameAlreadyCompleted:
                    MoveHistory.Add(
                        $"Move Rejected: Game is {result} " +
                        $"({destination.Row}, {destination.Column})");
                    break;

                case MoveResult.OutOfBounds:
                    MoveHistory.Add(
                        $"Move Rejected: {result} " +
                        $"({destination.Row}, {destination.Column})");
                    break;

                case MoveResult.InvalidMovement:
                    MoveHistory.Add(
                        $"Move Rejected: {result} " +
                        $"({destination.Row}, {destination.Column})");
                    break;

                case MoveResult.PathBlocked:
                    MoveHistory.Add(
                        $"Move Rejected: {result} " +
                        $"({destination.Row}, {destination.Column})");
                    break;

                case MoveResult.InvalidDestination:
                    MoveHistory.Add(
                        $"Move Rejected: {result} " +
                        $"({destination.Row}, {destination.Column})");
                    break;

                default:
                    MoveHistory.Add(
                        $"Move Rejected: {result} " +
                        $"({destination.Row}, {destination.Column})");
                    break;
            }
        }

        private void RefreshBoard()
        {
            foreach (SquareViewModel square in Squares)
            {
                square.IsCurrentPosition =
                    square.Position == _gamePlayer.CurrentPosition;
            }
        }

        private void RestartGame()
        {
            // Reset the game using the Model.
            _gamePlayer.Restart();

            // Clear the displayed history.
            MoveHistory.Clear();

            // Highlight the starting position again.
            RefreshBoard();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}