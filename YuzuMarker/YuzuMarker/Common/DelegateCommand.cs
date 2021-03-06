using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace YuzuMarker.Common
{
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }

        public Func<bool> CanExecuteFunc { get; set; }

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
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public void Execute(object parameter)
        {
            CommandAction();
        }
    }

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
