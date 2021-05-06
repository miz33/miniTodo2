using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miniTodo.ModelVersion0 {
	public class Todo {
		public Todo() {
		}

		public Todo(string title) {
			Title = title;
		}
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
	}
}
