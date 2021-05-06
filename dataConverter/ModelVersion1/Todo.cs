using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miniTodo.ModelVersion1 {
	public class Todo {
		public Todo() {
		}

		public Todo(string title) {
			Title = title;
		}

		public long Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime StatusUpdated { get; set; }
		public bool IsDone { get; set; }
		public bool IsDeleted { get; set; }
		public string Title { get; set; }
		public string Memo { get; set; }

		static public Todo Create(string title) {
			var item = new Todo(title);
			item.Created = DateTime.Now;
			return item;
		}

		public Todo Clone() {
			var item = new Todo();
			item.Created = this.Created;
			item.StatusUpdated = this.StatusUpdated;
			item.IsDone = this.IsDone;
			item.IsDeleted = this.IsDeleted;
			item.Title = this.Title;
			item.Memo = this.Memo;
			return item;
		}

		static public Todo Clone(miniTodo.ModelVersion0.Todo todo) {
			var item = new Todo();
			item.Created = todo.Created;
			item.StatusUpdated = todo.StatusUpdated;
			item.IsDone = todo.IsDone;
			item.IsDeleted = todo.IsDeleted;
			item.Title = todo.Title;
			item.Memo = todo.Memo;
			return item;
		}
	}
}
