using BomberMan.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace BomberMan.Views.MainMenu
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : UserControl
    {
        public MainMenuView()
        {
            InitializeComponent();
        }

        private void HelpOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Grid)  // If the click is on the overlay background (not the inner content)
            {
                var viewModel = DataContext as MainWindowViewModel;
                viewModel?.CloseHelpOverlayCommand.Execute(null);
            }
        }
    }
}
