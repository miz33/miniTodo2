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

		#region Public Property
		
		/// <summary>
		/// 新規Todo入力テキストボックスの動作の選択肢。UIに表示する名称と、内部で保持する値の組をDictionaryで保持する
		/// </summary>
		public Dictionary<string, NewTodoActions> NewTodoActionChoice { get; set; } = new Dictionary<string, NewTodoActions>() {
			{"最初に追加", NewTodoActions.InsertToFirst },
			{"最後に追加", NewTodoActions.InsertToLast },
			{"なし", NewTodoActions.None }
		};

		/// <summary>
		/// 新規Todo入力テキストボックスでEnterを押したときの動作
		/// </summary>
		public NewTodoActions ActionWhenEnterOnNewTodoTextBox {
			get { return _model.Operation.EnterOnNewTodoTextBox; }
			set {
				if (_model.Operation.EnterOnNewTodoTextBox != value) {
					_model.Operation.EnterOnNewTodoTextBox = value;
					OnPropertyChanged("ActionWhenEnterInNewTodoTextBox");
				}
			}
		}

		/// <summary>
		/// 新規Todo入力テキストボックスでShift+Enterを押したときの動作
		/// </summary>
		public NewTodoActions ActionWhenShiftEnterOnNewTodoTextBox {
			get { return _model.Operation.ShiftEnterOnNewTodoTextBox; }
			set {
				if (_model.Operation.ShiftEnterOnNewTodoTextBox != value) {
					_model.Operation.ShiftEnterOnNewTodoTextBox = value;
					OnPropertyChanged("ActionWhenShiftEnterInNewTodoTextBox");
				}
			}
		}

		/// <summary>
		/// 新規Todo入力テキストボックスでCtrl+Enterを押したときの動作
		/// </summary>
		public NewTodoActions ActionWhenCtrlEnterOnNewTodoTextBox {
			get { return _model.Operation.CtrlEnterOnNewTodoTextBox; }
			set {
				if (_model.Operation.CtrlEnterOnNewTodoTextBox != value) {
					_model.Operation.CtrlEnterOnNewTodoTextBox = value;
					OnPropertyChanged("ActionWhenCtrlEnterInNewTodoTextBox");
				}
			}
		}

		//TODO 残り二つの項目も追加する

		public int SE_Volume {
			get { return _model.SE_Volume; }
			set {
				if (_model.SE_Volume != value) {
					_model.SE_Volume = value;
					OnPropertyChanged("SE_Volume");
				}
			}
		}
		#endregion Public Property

		#region Public Method
		public void Save() {
			_model.Save();
		}
		#endregion

	}
}
