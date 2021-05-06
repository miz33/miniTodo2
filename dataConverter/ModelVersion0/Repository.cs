using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;

namespace miniTodo.ModelVersion0 {
	public class TodoAddedEventArgs : EventArgs {
		public TodoAddedEventArgs(Todo newTodo) {
			NewTodo = newTodo;
		}
		public Todo NewTodo { private set; get; }
		public bool IsAddedToTop { set; get; }
	}

	public class Repository {
		private string _filename;

		private List<Todo> _items = new List<Todo>();
		public ReadOnlyCollection<Todo> AllTodos {
			get { return new ReadOnlyCollection<Todo>(_items); }
		}

		public EventHandler<TodoAddedEventArgs> TodoAdded;

		public Repository(string filename) {
			_filename = filename;
		}

		public void Add(Todo item) {
			_items.Add(item);
			var handler = TodoAdded;
			if (handler != null) {
				handler(this, new TodoAddedEventArgs(item));
			}
			Save();
		}

		public void AddToTop(Todo item) {
			_items.Insert(0, item);
			var handler = TodoAdded;
			if (handler != null) {
				var e = new TodoAddedEventArgs(item);
				e.IsAddedToTop = true;
				handler(this, e);
			}
			Save();
		}

		public void MoveToLast(Todo item) {
			_items.Remove(item);
			_items.Add(item);
			Save();
		}

		public void MoveToFirst(Todo item) {
			_items.Remove(item);
			_items.Insert(0, item);
			Save();
		}

		public void Save() {
			var serializer = new XmlSerializer(typeof(List<Todo>));
			using (var stream = new FileStream(_filename, FileMode.Create)) {
				serializer.Serialize(stream, _items);
			}
		}

		public void Load() {
			if (!File.Exists(_filename)) {
				_items = new List<Todo>();
				return;
			}

			var serializer = new XmlSerializer(typeof(List<Todo>));
			using (var stream = new FileStream(_filename, FileMode.Open)) {
				_items = serializer.Deserialize(stream) as List<Todo>;
			}
		} 
	}
}
