using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using miniTodo.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Threading;
using miniTodo.View;

namespace miniTodo.ViewModel {
	public class MainWindowViewModel : ViewModelBase {
		#region private member 
		private Repository _repository;
		private string _editingText;
		private CollectionViewSource _viewSource;
		private ObservableCollection<TodoViewModel> _allTodos = new ObservableCollection<TodoViewModel>();
		private int _todaysDoneNum;
		private int _yesterdayDoneNum;
		private int _remainTodosNum;
		private DispatcherTimer _refleshTimer;
		private DateTime _lastUpdateTime;
		#endregion

		public MainWindowViewModel() {
			_repository = new Repository("data.xml");
			_repository.TodoAdded += this.OnTodoAddedToRepository;
			_repository.Load();
			foreach (var item in _repository.AllTodos) {
				var newItem = new TodoViewModel(item, _repository);
				newItem.PropertyChanged += this.OnTodoPropertyChanged;
				newItem.RequestToLater += this.OnRequestedMoveTodoToLater;
				newItem.RequestToSooner += this.OnRequestedMoveTodoToSooner;
				Items.Add(newItem);

			}

			_viewSource = new CollectionViewSource();
			_viewSource.Source = Items;
			_viewSource.Filter += this.OnFilter;

			RefleshData();

			_refleshTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
			_refleshTimer.Interval = TimeSpan.FromSeconds(30);
			_refleshTimer.Tick += (sender, e) => {
				if (_lastUpdateTime.Date < DateTime.Today) {
					this.RefleshData();
				}
			};
			_refleshTimer.Start();
		}

		#region Property
		public ObservableCollection<TodoViewModel> Items {
			get {return _allTodos;}
		}

		public CollectionViewSource ViewSource {
			get { return _viewSource; }
		}

		public string EditingText {
			get { return _editingText; }
			set {
				if (_editingText != value) {
					_editingText = value;
					OnPropertyChanged("EditingText");
				}
			}
		}

		public int TodaysDoneNum {
			get { return _todaysDoneNum; }
			set {
				if (_todaysDoneNum != value) {
					_todaysDoneNum = value;
					OnPropertyChanged("TodaysDoneNum");
				}
			}
		}

		public int YesterdaysDoneNum {
			get { return _yesterdayDoneNum; }
			set {
				if (_yesterdayDoneNum != value) {
					_yesterdayDoneNum = value;
					OnPropertyChanged("YesterdaysDoneNum");
				}
			}
		}

		public int RemainTodosNum {
			get { return _remainTodosNum; }
			set {
				if (_remainTodosNum != value) {
					_remainTodosNum = value;
					OnPropertyChanged("RemainTodosNum");
				}
			}
		}

		#endregion

		#region Commands
		private ICommand _createCommand;
		public ICommand CreateCommand {
			get {
				if (_createCommand == null) {
					_createCommand = new RelayCommand(
						param => {
							if (!string.IsNullOrEmpty(EditingText)) {
								var item = Todo.Create(EditingText);
								item.Id = _repository.PublishUniqueId();
								_repository.Add(item);
								EditingText = "";
							}
						});
				}
				return _createCommand;
			}
		}

		private ICommand _createToTopCommand;
		public ICommand CreateToTopCommand {
			get {
				if (_createToTopCommand == null) {
					_createToTopCommand = new RelayCommand(
						param => {
							if (!string.IsNullOrEmpty(EditingText)) {
								var item = Todo.Create(EditingText);
								item.Id = _repository.PublishUniqueId();
								_repository.AddToTop(item);
								EditingText = "";
							}
						});
				}
				return _createToTopCommand;
			}
		}

		private ICommand _settingsCommand;
		public ICommand SettingsCommand {
			get {
				if (_settingsCommand == null) {
					_settingsCommand = new RelayCommand(
						param => {
							//※ViewModelでViewを呼び出しているのはよくないが、本格的なプログラムじゃないので今回は目をつむる
							var view = new SettingsView(new SettingsViewModel());
							view.Owner = App.Current.MainWindow;
							view.ShowDialog();
						}
					);
				}
				return _settingsCommand;
			}
		}

		private ICommand _showDailyGraphCommand;
		public ICommand ShowDailyGraphCommand {
			get {
				if (_showDailyGraphCommand == null) {
					_showDailyGraphCommand = new RelayCommand(
						(param) => {
							//※ViewModelでViewを呼び出しているのはよくないが、本格的なプログラムじゃないので今回は目をつむる
							var view = new GraphView(new GraphViewModel(this.Items, GraphViewModel.Mode.Daily));
							view.Owner = App.Current.MainWindow;
							view.ShowDialog();
						}
					);
				}
				return _showDailyGraphCommand;
			}
		}

		private ICommand _showWeeklyGraphCommand;
		public ICommand ShowWeeklyGraphCommand {
			get {
				if (_showWeeklyGraphCommand == null) {
					_showWeeklyGraphCommand = new RelayCommand(
						(param) => {
							var view = new GraphView(new GraphViewModel(this.Items, GraphViewModel.Mode.Weekly));
							view.Owner = App.Current.MainWindow;
							view.ShowDialog();
						});
				}
				return _showWeeklyGraphCommand;
			}
		}

		private ICommand _showMonthlyGraphCommand;
		public ICommand ShowMonthlyGraphCommand {
			get {
				return _showMonthlyGraphCommand = _showMonthlyGraphCommand ?? new RelayCommand(
					(param) => {
						var view = new GraphView(new GraphViewModel(this.Items, GraphViewModel.Mode.Monthly));
						view.Owner = App.Current.MainWindow;
						view.ShowDialog();
					});
			}
		}
		#endregion 

		#region event handler
		private void OnTodoAddedToRepository(object sender, TodoAddedEventArgs e) {
			var newItem = new TodoViewModel(e.NewTodo, _repository);
			newItem.PropertyChanged += this.OnTodoPropertyChanged;
			newItem.RequestToLater += this.OnRequestedMoveTodoToLater;
			newItem.RequestToSooner += this.OnRequestedMoveTodoToSooner;
			if (e.IsAddedToTop) {
				this._allTodos.Insert(0, newItem);
			} else {
				this._allTodos.Add(newItem);
			}
			RefleshData();
		}

		private void OnFilter(object sender, FilterEventArgs e) {
			var item = e.Item as TodoViewModel;
			if (item.IsDone || item.IsDeleted) {
				e.Accepted = false;
			} else {
				e.Accepted = true;
			}
		}

		private void OnTodoPropertyChanged(object sender, PropertyChangedEventArgs e) {
				RefleshData();
				ViewSource.View.Refresh();
		}

		private void OnRequestedMoveTodoToLater(object sender, EventArgs e) {
			var item = sender as TodoViewModel;
			Items.Remove(item);
			Items.Add(item);
			_repository.MoveToLast(item.Item);
			ViewSource.View.Refresh();
		}

		private void OnRequestedMoveTodoToSooner(object sender, EventArgs e) {
			var item = sender as TodoViewModel;
			Items.Remove(item);
			Items.Insert(0, item);
			_repository.MoveToFirst(item.Item);
			ViewSource.View.Refresh();
		}
		#endregion

		#region Public Method
		public void Save() {
			_repository.Save();
		}
		#endregion

		#region private method
		private void RefleshData() {
			RemainTodosNum  = Items.Count(item => !item.IsDone && !item.IsDeleted);
			YesterdaysDoneNum = Items.Count( item => 
				item.IsDone && item.StatusUpdated.Date == DateTime.Today.AddDays(-1));
			TodaysDoneNum = Items.Count(item =>
				item.IsDone && item.StatusUpdated.Date == DateTime.Today);
			_lastUpdateTime = DateTime.Now;
		}
		#endregion
	}
}
