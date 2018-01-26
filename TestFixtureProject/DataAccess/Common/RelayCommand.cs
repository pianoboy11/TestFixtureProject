using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestFixtureProject.Common
{
    public class RelayCommand : ICommand
    {
        readonly Func<Boolean> _canExecute;
        readonly Action<object> _executecommand;

        public RelayCommand(Action<object> execute)
            : this(null, execute)
        {
        }

        public RelayCommand(Func<Boolean> canExecute, Action<object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");

            }
            this._executecommand = execute;
            this._canExecute = canExecute;
        }

       public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }



        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(Object parameter)
        {
            _executecommand(parameter);
        }

    }
}
