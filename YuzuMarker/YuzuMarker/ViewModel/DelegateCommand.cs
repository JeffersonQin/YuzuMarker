using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace YuzuMarker.ViewModel
{
    public class DelegateCommand<T> : ICommand
    {
        public Action<T> CommandAction { get; set; }

        public Func<T, bool> CanExecuteFunc { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc((T)parameter);
        }

        public void Execute(object parameter)
        {
            CommandAction((T)parameter);
        }
    }
}
