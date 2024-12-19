using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BomberMan.Commands
{
    // En generisk implementation av ICommand som kan användas för att binda kommandon till UI-komponenter
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        

        //behöver inte parametrar
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand()
        {

        }

        // Om kommandot kan köras
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(); 
        }

        //Kör kommandot
        public void Execute(object parameter)
        {
            _execute();  // Kör kommandor utan parametrar
        }

        // Händelse som meddelar när CanExecute ändras
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
