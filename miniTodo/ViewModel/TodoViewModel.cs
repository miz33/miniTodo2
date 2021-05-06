using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using miniTodo.Model;
using System.Windows.Input;
using System.Windows.Media;

namespace miniTodo.ViewModel {
	public class TodoViewModel : ViewModelBase {
		private Todo _item;
		private Repository _repository;

		public TodoViewModel(Todo item, Repository repository) {
			_item = item;
			_repository = repository;
		}

		public EventHandler RequestToLater;
		public EventHandler RequestToSooner;

		#region Property
		public Todo Item {
			get { return _item; }
		}

		public long Id {
			get { return _item.Id; }
		}

		public string Title {
			get { return _item.Title; }
			set {
				if (_item.Title != value) {
					_item.Title = value;
					OnPropertyChanged("Title");
				}
			}
		}

		public string Memo {
			get { return _item.Memo; }
			set {
				if (_item.Memo != value) {
					_item.Memo = value;
					OnPropertyChanged("Memo");
				}
			}
		}

		public bool IsDone {
			get { return _item.IsDone; }
			set {
				if (_item.IsDone != value) {
					_item.IsDone = value;
					OnPropertyChanged("IsDone");
					StatusUpdated = DateTime.Now;
				}
			}
		}

		public bool IsDeleted {
			get { return _item.IsDeleted;}
			set {
				if (_item.IsDeleted != value) {
					_item.IsDeleted = value;
					OnPropertyChanged("IsDeleted");
					StatusUpdated = DateTime.Now;
				}
			}
		}

		public DateTime Created {
			get { return _item.Created; }
			set {
				if (_item.Created != value) {
					_item.Created = value;
					OnPropertyChanged("Created");
				}
			}
		}

		public DateTime StatusUpdated {
			get { return _item.StatusUpdated; }
			set {
				if (_item.StatusUpdated != value) {
					_item.StatusUpdated = value;
					OnPropertyChanged("StatusUpdated");
				}
			}
		}

		#endregion

		#region Commands
		private ICommand _laterCommand;
		public ICommand LaterCommand {
			get {
				if (_laterCommand == null) {
					_laterCommand = new RelayCommand(
						param => {
							var handler = RequestToLater;
							if (handler != null) {
								handler(this, EventArgs.Empty);
							}
						});
				}
				return _laterCommand;
			}
		}

		private ICommand _soonerCommand;
		public ICommand SoonerCommand {
			get {
				if (_soonerCommand == null) {
					_soonerCommand = new RelayCommand(
						param => {
							var handler = RequestToSooner;
							if (handler != null) {
								handler(this, EventArgs.Empty);
							}
						});
				}
				return _soonerCommand;
			}
		}

		private ICommand _completeCommand;
		public ICommand CompleteCommand {
			get {
				if (_completeCommand == null) {
					_completeCommand = new RelayCommand(
						param => {
							IsDone = true;
							StatusUpdated = DateTime.Now;
							_repository.Save();
						});
				}
				return _completeCommand;
			}
		}

		private ICommand _deleteCommand;
		public ICommand DeleteCommand {
			get {
				if (_deleteCommand == null) {
					_deleteCommand = new RelayCommand(
						param => {
							IsDeleted = true;
							StatusUpdated = DateTime.Now;
							_repository.Save();
						});
				}
				return _deleteCommand;
			}
		}
		#endregion
	}
}
