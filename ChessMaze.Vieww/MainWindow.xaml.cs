using ChessMaze.Vieww.ViewModels;
using System.Windows;

namespace ChessMaze.Vieww
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}