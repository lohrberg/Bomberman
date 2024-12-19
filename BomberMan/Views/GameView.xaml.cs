using BomberMan.Models;
using BomberMan.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BomberMan.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        MainViewModel MAIN = new MainViewModel();

        public GameView()
        {
            InitializeComponent();
        }

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure the UserControl is focusable and explicitly request focus
            this.Focusable = true;
            Keyboard.Focus(this);  // This makes sure the UserControl receives focus and captures key events
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MAIN.LoadPlayerHighScoresFromJson();
        }
    }
}
