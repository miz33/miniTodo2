using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;

namespace miniTodo.Model {
	public class TodoAddedEventArgs : EventArgs {
		public TodoAddedEventArgs(Todo newTodo) {
			NewTodo = newTodo;
		}
		public Todo NewTodo { private set; get; }
		public bool IsAddedToTop { set; get; }
	}

	public class Repository {
		private string _filename;
		public class PersistData {
			public int FileFormatVersion = 1;
			public long LastItemId;
			public List<Todo> Items = new List<Todo>();
		}

		private PersistData _data = new PersistData();

		//private List<Todo> _items = new List<Todo>();
		public ReadOnlyCollection<Todo> AllTodos {
			get { return new ReadOnlyCollection<Todo>(_data.Items); }
		}

		public EventHandler<TodoAddedEventArgs> TodoAdded;

		public Repository(string filename) {
			_filename = filename;
		}

		/// <summary>
		/// itemをRepositoryに登録する
		/// itemはUniqueなIDを持っているべきだが、持っていない場合はRepositoryが自動で付加する
		/// </summary>
		/// <param name="item"></param>
		public void Add(Todo item) {
			if (_data.Items.Any(x => x.Id == item.Id)) {
				throw new InvalidOperationException("すでに登録されているIDのアイテムを登録しようとしています");
			}

			if (item.Id == 0) {
				item.Id = PublishUniqueId();
			}

			_data.Items.Add(item);
			var handler = TodoAdded;
			if (handler != null) {
				handler(this, new TodoAddedEventArgs(item));
			}
			Save();
		}

		public void AddToTop(Todo item) {
			_data.Items.Insert(0, item);
			var handler = TodoAdded;
			if (handler != null) {
				var e = new TodoAddedEventArgs(item);
				e.IsAddedToTop = true;
				handler(this, e);
			}
			Save();
		}

		/// <summary>
		/// Repository内でUniqueなIDを発行する
		/// </summary>
		/// <returns></returns>
		public long PublishUniqueId() {
			return ++_data.LastItemId;
		}

		public void MoveToLast(Todo item) {
			_data.Items.Remove(item);
			_data.Items.Add(item);
			Save();
		}

		public void MoveToFirst(Todo item) {
			_data.Items.Remove(item);
			_data.Items.Insert(0, item);
			Save();
		}

		public void Save() {
			var serializer = new XmlSerializer(typeof(PersistData));
			using (var stream = new FileStream(_filename, FileMode.Create)) {
				serializer.Serialize(stream, _data);
			}
		}

		public void Load() {
			if (!File.Exists(_filename)) {
				_data.Items = new List<Todo>();
				return;
			}

			var serializer = new XmlSerializer(typeof(PersistData));
			using (var stream = new FileStream(_filename, FileMode.Open)) {
				_data = serializer.Deserialize(stream) as PersistData;
			}
		} 
	}
}
