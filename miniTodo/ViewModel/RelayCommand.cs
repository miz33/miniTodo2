using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;

namespace miniTodo.ViewModel {

	public class RelayCommand : ICommand {
		readonly Action<object> _execute;
		readonly Predicate<object> _canExecute;

		public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
			Debug.Assert(execute != null);

			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommand(Action<object> execute)
			: this(execute, null) {
		}


		#region ICommand メンバ

		public bool CanExecute(object parameter) {
			return _canExecute == null ? true : _canExecute(parameter);
		}

		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameter) {
			_execute(parameter);
		}

		#endregion
	}

}
