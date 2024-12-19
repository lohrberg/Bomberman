using BomberMan.ViewModels;
using BomberMan.Views;
using System.Windows;
using System.Windows.Input;

namespace BomberMan
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this); // Ensure the window is focused
        }

        // Handle KeyDown event
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Forward the KeyDown event to the correct MainViewModel
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                if (mainWindowViewModel.CurrentView is GameView)
                {
                    // If GameView is active, forward the key event to the MainViewModel
                    mainWindowViewModel.MainViewModel.KeyDownCommand.Execute(e.Key);
                }
            }
        }

        // Handle KeyUp event
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            // Forward the KeyUp event to the correct MainViewModel
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                if (mainWindowViewModel.CurrentView is GameView)
                {
                    // If GameView is active, forward the key event to the MainViewModel
                    mainWindowViewModel.MainViewModel.KeyUpCommand.Execute(e.Key);
                }
            }
        }
    }
}
