using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using miniTodo.View;
using miniTodo.ViewModel;
using miniTodo.Config;
using System.IO;
using System.Xml.Serialization;

namespace miniTodo {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application {
		public readonly string configFile = "WindowConfig.xml";

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			var viewModel = new MainWindowViewModel();
			MainWindow window = new MainWindow(viewModel);


			//前回のウィンドウの状態を復元
			WindowPositionManager.Instance.RestoreWindowPosition(window);

			
			window.Closing += (sender, eArg) => {
				//閉じる前のウィンドウの状態を保存
				WindowPositionManager.Instance.RecordWindowPosition(window);
			};

			window.Show();
		}

	}
}
