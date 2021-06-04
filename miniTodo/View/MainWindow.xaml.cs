using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using miniTodo.ViewModel;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Diagnostics;
using miniTodo.UserSettings;
using miniTodo.Config;

namespace miniTodo.View {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {
		#region Constructor
		public MainWindow(MainWindowViewModel vm) {
			DataContext = vm;
			_viewModel = vm;
			InitializeComponent();

			//TODO ここでキーバインディングの処理をすると、設定を変更したあと再起動しないと反映されない。設定画面を閉じたらキーバインディングを再読み込みするようにしたい。

			//新規Todo入力テキストボックスのKeyBinding
			var action_to_command = new Dictionary<NewTodoActions, ICommand>() {
				{ NewTodoActions.InsertToFirst, vm.CreateToTopCommand },
				{ NewTodoActions.InsertToLast, vm.CreateCommand },
				{ NewTodoActions.None, null }
			};
			var key_to_action = new Dictionary<KeyGesture, NewTodoActions>() {
				{ new KeyGesture(Key.Enter), Settings.Instance.Operation.EnterOnNewTodoTextBox },
				{ new KeyGesture(Key.Enter, ModifierKeys.Shift), Settings.Instance.Operation.ShiftEnterOnNewTodoTextBox },
				{ new KeyGesture(Key.Enter, ModifierKeys.Control), Settings.Instance.Operation.CtrlEnterOnNewTodoTextBox }
			};

			foreach (var key in key_to_action.Keys) {
				var actions = key_to_action[key];
				var command = action_to_command[actions];
				if (command != null) {
					_newTodoTextBox.InputBindings.Add(new KeyBinding(command, key));
				}
			}

			//Doneの時のサウンド
			_player = new MediaPlayer();

			//起動時にすでにあるアイテムはアニメーションしないように登録しておく
			foreach (var item in vm.Items) {
				_alreadyAnimated.Add(item.Id);
			}
		}
		#endregion

		#region event
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region private member
		MainWindowViewModel _viewModel;
		MediaPlayer _player;
		#endregion
		
		#region Property
		private bool hasTextForNewTodo;
		/// <summary>
		/// 新規入力テキストボックスにテキストが入っているかどうか
		/// </summary>
		public bool HasTextForNewTodo {
			private set {
				if (hasTextForNewTodo != value) {
					hasTextForNewTodo = value;
					var handler = this.PropertyChanged;
					if (handler != null) {
						handler(this, new PropertyChangedEventArgs("HasTextForNewTodo"));
					}
				}
			}
			get {
				return hasTextForNewTodo;
			}
		}
		#endregion

		#region Commands
		public static RoutedUICommand EditItemCommand = new RoutedUICommand("Edit", "EditItemCommand", typeof(MainWindow));
		static public RoutedUICommand DoneCommand = new RoutedUICommand("Complate Todo", "DoneCommand", typeof(MainWindow));

		#endregion


		#region EventHandler

		private void EditItem_Executed(object sender, ExecutedRoutedEventArgs e) {
			//MouseBinding経由の呼び出しではCommandParameterにBindingが使えないのでe.Sourceから取り出す
			var listbox = e.Source as ListBox;
			var viewModel = listbox.SelectedItem as TodoViewModel;
			var window = new EditTodoView(viewModel);
			WindowPositionManager.Instance.RestoreWindowPosition(window);
			window.Closing += (s, eArg) => {
				WindowPositionManager.Instance.RecordWindowPosition(window);
			};

			if (window.ShowDialog() == true) {
			    _viewModel.Save();
			}
		}
		private void Minimize_Click(object sender, RoutedEventArgs e) {
			this.WindowState = WindowState.Minimized;
		}
		private void Close_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void DataPanel_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left) {
				DragMove();
			}
		}

		private void _newTodo_TextChanged(object sender, TextChangedEventArgs e) {
			var textbox = e.Source as TextBox;
			this.HasTextForNewTodo = textbox.Text.Length > 0;
		}
		#endregion

		/// <summary>
		/// 全アイテム管理画面を開くボタンのクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AllItemsView_Click(object sender, RoutedEventArgs e) {
			var mainVM = (MainWindowViewModel)DataContext;
			var allVM = new AllTodoViewModel(mainVM.Items);
			var view = new AllTodoView();
			view.DataContext = allVM;
			view.Owner = this;
			view.ShowDialog();
		}

		/// <summary>
		/// 設定のクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Settings_Click(object sender, RoutedEventArgs e) {
			var mainVM = DataContext as MainWindowViewModel;
			var settingsVM = new SettingsViewModel();
			var view = new SettingsView(settingsVM);
			view.DataContext = settingsVM;
			view.Owner = this;
			view.ShowDialog();
		}

		//以下、ListBoxへ追加・削除時のアニメーションのためのイベントハンドラ
		//MVVM的ではないが、本アプリは本格的に作るつもりもないので、てっとり早い作りにしている

		private void Done_Executed(object sender, ExecutedRoutedEventArgs e) {
			Debug.Assert(e.Parameter is TodoViewModel);
			Debug.Assert(sender is Grid);
			var vm = e.Parameter as TodoViewModel;
			var container = sender as Grid;

			//チェックマークを表示して、透明化するアニメーション処理、サウンド再生→完了後にvmへCompleteイベント発行

			var doneMark = container.FindName("_doneMark") as TextBlock;
			doneMark.Visibility = System.Windows.Visibility.Visible;

			var animation = container.FindResource("_doneAnimation") as Storyboard;
			animation.Completed += (s, eArg) => {
				vm.CompleteCommand.Execute(null);
			};
			animation.Begin();

			_player.Open(new Uri("Resource/ta_ge_ice01.mp3", UriKind.Relative));
			_player.Volume = (float)Settings.Instance.SE_Volume/100;
			_player.Play();
			_player.MediaEnded += (s, eArg) => _player.Close();
		}

		//同じアイテムに複数回アニメーション処理するのを防ぐため、アニメーション済みのアイテムのIDを格納
		private List<long> _alreadyAnimated = new List<long>();

		/// <summary>
		/// １つのアイテムが読み込まれたときのイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Grid_Loaded(object sender, RoutedEventArgs e) {
			var container = sender as Grid;
			var vm = container.Tag as TodoViewModel;//ViewModelを渡す簡単な方法が思いつかないので、Tagに入れている
			var id = vm.Id;
			var animation = container.FindResource("_slideInAnimation") as Storyboard;

			if (!_alreadyAnimated.Contains(id)) {
				animation.Begin();
				_alreadyAnimated.Add(id);
			}
		}

	}
}
