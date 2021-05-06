using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace miniTodo.ViewModel {
	public class ViewModelBase : INotifyPropertyChanged {
		protected void OnPropertyChanged(string propertyName) {
			var handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#region INotifyPropertyChanged メンバ

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
