using BCIT615.Assessment1.GamePlayer.Model;
using ChessMaze.Model;
using ChessMaze.Persistence;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ChessMaze.Vieww.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GamePlayer _gamePlayer;

        private string _currentGameName = string.Empty;

        private bool _isGameStarted;

        private readonly GamePersistenceService _persistenceService;

        public ObservableCollection<SquareViewModel> Squares { get; }
            = new ObservableCollection<SquareViewModel>();

        public ObservableCollection<string> MoveHistory { get; }
            = new ObservableCollection<string>();

        public ICommand StartGameCommand { get; private set; } = null!;

        public ICommand SelectSquareCommand { get; }

        public ICommand RestartCommand { get; }

        public string CurrentGameName
        {
            get => _currentGameName;
            set
            {
                if (_currentGameName == value)
                    return;

                _currentGameName = value;
                OnPropertyChanged();
            }
        }

        public bool IsGameStarted
        {
            get => _isGameStarted;
            set
            {
                if (_isGameStarted == value)
                    return;

                _isGameStarted = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(StartScreenVisibility));
                OnPropertyChanged(nameof(GameScreenVisibility));
            }
        }

        public Visibility StartScreenVisibility =>
            IsGameStarted ? Visibility.Collapsed : Visibility.Visible;

        public Visibility GameScreenVisibility =>
            IsGameStarted ? Visibility.Visible : Visibility.Collapsed;

        public MainViewModel()
        {
            StartGameCommand = new RelayCommand(_ => StartGame());
            // Create the game using your existing Model.
            _gamePlayer = new GamePlayer();

            // Connect the UI commands.
            SelectSquareCommand = new RelayCommand(
            parameter => _ = SelectSquareAsync(parameter));

            RestartCommand = new RelayCommand(
                _ => RestartGame());

            // Create all 36 squares.
            CreateBoardSquares();
        }

        private void StartGame()
        {
            if (string.IsNullOrWhiteSpace(CurrentGameName))
            {
                MessageBox.Show(
                    "Please enter your name before starting the game.",
                    "Name Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            CurrentGameName = CurrentGameName.Trim();

            IsGameStarted = true;
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

        private async Task SelectSquareAsync(object? parameter)
        {
            if (parameter is not Position destination)
                return;

            MoveResult result = _gamePlayer.TryMove(destination);

            RefreshBoard();

            switch (result)
            {
                case MoveResult.Success:
                    MoveHistory.Add(
                        $"Move {result}: ({destination.Row}, {destination.Column})");

                    await SaveCurrentGameAsync();
                    break;

                case MoveResult.GameCompleted:
                    MoveHistory.Add(
                        $"Game completed at ({destination.Row}, {destination.Column})");

                    await SaveCurrentGameAsync();
                    break;

                case MoveResult.GameAlreadyCompleted:
                case MoveResult.OutOfBounds:
                case MoveResult.InvalidMovement:
                case MoveResult.PathBlocked:
                case MoveResult.InvalidDestination:
                default:
                    MoveHistory.Add(
                        $"Move Rejected: {result} ({destination.Row}, {destination.Column})");
                    break;
            }
        }

        private async Task SaveCurrentGameAsync()
        {
            try
            {
                GameSnapshot snapshot = _gamePlayer.Capture();

                await _persistenceService.SaveGameAsync(
                    CurrentGameName,
                    snapshot);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to save the game: {ex.Message}",
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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