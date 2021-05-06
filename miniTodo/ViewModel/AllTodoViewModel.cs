using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miniTodo.ViewModel {
	class AllTodoViewModel {
		public AllTodoViewModel(IEnumerable<TodoViewModel> source) {
			//今のところアイテム管理ビューに表示する用途のみなので、
			//更新時間でソートする
			var items = from item in source
						orderby item.StatusUpdated descending
						select item;
			AllItems = items.ToArray<TodoViewModel>();
		}

		public TodoViewModel[] AllItems {
			get;
			private set;
		}
	}
}
