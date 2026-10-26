using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChessMaze.Model;

namespace ChessMaze.Vieww.ViewModels
{
    public class SquareViewModel : INotifyPropertyChanged
    {
        public Position Position { get; }

        public PieceType? Piece { get; }

        public bool IsStart { get; }

        public bool IsTarget { get; }

        private bool _isCurrentPosition;

        public bool IsCurrentPosition
        {
            get => _isCurrentPosition;
            set
            {
                if (_isCurrentPosition == value)
                    return;

                _isCurrentPosition = value;
                OnPropertyChanged();
            }
        }

        public string? PieceImage => Piece switch
        {
            PieceType.Rook => "pack://application:,,,/ChessMaze.Vieww;component/Images/Rook.jpg",
            PieceType.Bishop => "/Images/Bishop.jpg",
            PieceType.Knight => "/Images/Knight.jpg",
            PieceType.King => "/Images/King.jpg",
            _ => null
        };

        public SquareViewModel(
            Position position,
            PieceType? piece,
            bool isStart,
            bool isTarget,
            bool isCurrentPosition)
        {
            Position = position;
            Piece = piece;
            IsStart = isStart;
            IsTarget = isTarget;
            IsCurrentPosition = isCurrentPosition;
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