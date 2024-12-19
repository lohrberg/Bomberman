using BomberMan.Services;
using BomberMan.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace BomberMan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

            mainWindow.Show();
        }
    }

}
