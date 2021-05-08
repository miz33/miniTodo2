using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using miniTodo.UserSettings;
using System.Windows.Input;

namespace miniTodo.ViewModel {
	public class SettingsViewModel : ViewModelBase {
		private Settings _model;

		public SettingsViewModel() {
			_model = Settings.Instance;
		}


		public int SE_Volume {
			get { return _model.SE_Volume; }
			set {
				if (_model.SE_Volume != value) {
					_model.SE_Volume = value;
					OnPropertyChanged("SE_Volume");
				}
			}
		}

		public void Save() {
			_model.Save();
		}

	}
}
