using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;

namespace miniTodo.ModelVersion1 {
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

		public void Add(Todo item) {
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

		public void Import(IEnumerable<miniTodo.ModelVersion1.Todo> source) {
			_data.Items.AddRange(source);
			_data.LastItemId = _data.Items.Last().Id;
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
