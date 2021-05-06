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
			if (File.Exists(configFile)) {
				var serializer = new XmlSerializer(typeof(WindowConfig));
				using (var stream = new FileStream(configFile, FileMode.Open)) {
					var config = serializer.Deserialize(stream) as WindowConfig;
					window.Top = config.Top;
					window.Left = config.Left;
					window.Width = config.Width;
					window.Height = config.Height;
					window.Topmost = config.IsTopMost;
				}
			}

			window.Closing += (sender, eArg) => {
				//閉じる前のウィンドウの状態を保存
				var config = new WindowConfig();
				config.Top = window.Top;
				config.Left = window.Left;
				config.Width = window.Width;
				config.Height = window.Height;
				config.IsTopMost = window.Topmost;
				var serializer = new XmlSerializer(typeof(WindowConfig));
				using (var stream = new FileStream(configFile, FileMode.Create)) {
					serializer.Serialize(stream, config);
				}
			};

			window.Show();
		}

	}
}
