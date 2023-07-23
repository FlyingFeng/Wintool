using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinTool
{
    internal class NormalCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<object?>? _act;
        private readonly Func<object?, Task>? _func;
        private readonly Func<bool>? _canExecute;

        public NormalCommand(Action<object?>? act, Func<bool>? canExecute = null)
        {
            _act = act;
            _canExecute = canExecute;
        }

        public NormalCommand(Func<object?, Task>? func, Func<bool>? canExecute = null)
        {
            _func = func;
            _canExecute = canExecute;
        }


        public bool CanExecute(object? parameter)
        {
            var flag = _canExecute?.Invoke();
            return flag ?? true;
        }

        public void Execute(object? parameter)
        {
            if (_act != null)
            {
                _act.Invoke(parameter);
            }
            else
            {
                _func?.Invoke(parameter);
            }
        }
    }
}
