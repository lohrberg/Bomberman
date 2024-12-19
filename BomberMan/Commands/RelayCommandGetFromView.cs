using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BomberMan.Commands
{
    [AddINotifyPropertyChangedInterface]  // Fody genererar automatisk INotifyPropertyChanged
    public class RelayCommandGetFromView : ICommand


    {
        private readonly Action<object> _execute;

        public RelayCommandGetFromView(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged;
    }
}