using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dataConverter {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {
		string _filename = "data.xml";
		public MainWindow() {
			InitializeComponent();
			try {
				if (!backupData()) {
					ResultTextBlock.Text = "バックアップに失敗しました";
					return;
				}
			} catch (Exception e) {
				ResultTextBlock.Text = e.Message;
				return;
			}

			if (TryLoadLatestVersion()) {
				ResultTextBlock.Text = "すでに最新版です";
				return;
			}

			try {
				ConvertData();
			} catch (Exception e) {
				ResultTextBlock.Text = e.Message;
				return;
			}

			ResultTextBlock.Text = "変換しました";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Is Succeed Backup</returns>
		private bool backupData() {
			string backupfile = string.Format("{0}.{1}", _filename, DateTime.Now.ToString("yyyyMMddHHmm"));
			if (File.Exists(backupfile)) return false;

			File.Copy(_filename, backupfile);
			return File.Exists(backupfile);
		}

		private bool TryLoadLatestVersion() {
			bool result;
			try {
				var repo = new miniTodo.ModelVersion1.Repository(_filename);
				repo.Load();
				result = true;
			} catch {
				result = false;
			}
			return result;
		}

		private void ConvertData() {
			IList<miniTodo.ModelVersion1.Todo> items;
			try {
				var repo = new miniTodo.ModelVersion0.Repository(_filename);
				repo.Load();

				items = (from item in repo.AllTodos
							orderby item.Created ascending
							select miniTodo.ModelVersion1.Todo.Clone(item)).ToList<miniTodo.ModelVersion1.Todo>();

				long index = 1;
				foreach (var item in items) {
					item.Id = index++;
				}
				
			} catch  {
				throw new Exception("古いフォーマットのデータの読込に失敗しました");
			}

			var newRepo = new miniTodo.ModelVersion1.Repository(_filename);
			newRepo.Import(items);
			newRepo.Save();
		}

	}
}
