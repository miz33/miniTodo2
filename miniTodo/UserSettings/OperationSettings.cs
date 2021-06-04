using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miniTodo.UserSettings {
	public enum NewTodoActions {
		None,
		InsertToFirst,
		InsertToLast

	}
	public class OperationSettings {
		public NewTodoActions EnterOnNewTodoTextBox { get; set; } = NewTodoActions.InsertToLast;
		public NewTodoActions ShiftEnterOnNewTodoTextBox { get; set; } = NewTodoActions.None;
		public NewTodoActions CtrlEnterOnNewTodoTextBox { get; set; } = NewTodoActions.InsertToFirst;
	}
}
