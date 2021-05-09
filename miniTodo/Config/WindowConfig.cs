using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace miniTodo.Config {
	
	/// <summary>
	/// 1つのウィンドウの設定を表すクラス
	/// </summary>
	public class WindowConfig {
		public string Type { get; set; }
		public double Top { get; set; }
		public double Left { get; set; }
		public double Height { get; set; }
		public double Width { get; set; }
		public bool IsTopMost { get; set; }

		/// <summary>
		/// sourceウィンドウのプロパティを継承したWindowsConfigクラスを作成する
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static WindowConfig Create(Window source) {
			var item = new WindowConfig {
				Type = source.GetType().FullName,
				Top = source.Top,
				Left = source.Left,
				Height = source.Height,
				Width = source.Width,
				IsTopMost = source.Topmost
			};
			return item;
		}

		/// <summary>
		/// WindowConfigが保持するプロパティを実際のウィンドウに反映する
		/// </summary>
		/// <param name="target"></param>
		public void Restore(Window target) {
			if (target.GetType().FullName != this.Type) throw new Exception("Invalid Argument");

			target.Top = this.Top;
			target.Left = this.Left;
			target.Height = this.Height;
			target.Width = this.Width;
			target.Topmost = this.IsTopMost;
		}
	}
}
