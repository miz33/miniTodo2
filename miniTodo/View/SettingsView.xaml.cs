using System;
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
using System.Windows.Shapes;
using miniTodo.ViewModel;

namespace miniTodo.View {
	/// <summary>
	/// SettingsView.xaml の相互作用ロジック
	/// </summary>
	public partial class SettingsView : Window {
		private MediaPlayer _player = new MediaPlayer();

		public SettingsView(SettingsViewModel viewModel) {
			DataContext = viewModel;
			InitializeComponent();
			BindingGroup.BeginEdit();
		}

		private void SE_Volume_Changed(object sender, MouseButtonEventArgs e) {
			var vm = DataContext as SettingsViewModel;
			_player.Open(new Uri("Resource/ta_ge_ice01.mp3", UriKind.Relative));
			_player.Volume = (float)_slider.Value / 100;
			_player.Play();
		}

		private void Cancel_Clicked(object sender, RoutedEventArgs e) {
			BindingGroup.CancelEdit();
			DialogResult = false;
			Close();
		}

		private void OK_Clicked(object sender, RoutedEventArgs e) {
			BindingGroup.CommitEdit();
			var vm = DataContext as SettingsViewModel;
			vm.Save();
			DialogResult = true;
			Close();
		}
	}
}
