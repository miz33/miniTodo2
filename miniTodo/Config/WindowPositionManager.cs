using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using System.IO;

namespace miniTodo.Config {

	/// <summary>
	/// ウィンドウの位置、サイズ等のプロパティを管理するクラス。
	/// 前回終了時と同じ状態で開く場合などに使う。
	/// </summary>
	public class WindowPositionManager {
		#region Constructor
		public WindowPositionManager() { }
		#endregion Constructor

		#region Private Field
		private const string _filepath = "WindowsPosition.xml";
		#endregion Private Field

		#region Public Field
		/// <summary>
		/// このクラスが管理する対象のデータ
		/// </summary>
		public List<WindowConfig> data = new List<WindowConfig>();
		#endregion Public Field

		#region Property
		private static WindowPositionManager _instance = null;
		public static WindowPositionManager Instance {
			get {
				if (_instance != null) return _instance;

				if (File.Exists(_filepath)) {
					var serializer = new XmlSerializer(typeof(WindowPositionManager));
					using (var stream = new FileStream(_filepath, FileMode.Open)) {
						_instance = serializer.Deserialize(stream) as WindowPositionManager;
					}
				} else {
					_instance = new WindowPositionManager();
				}
				return _instance;

			} 
		}
		#endregion

		#region Public Method

		/// <summary>
		/// targetウィンドウの情報を記録する
		/// </summary>
		/// <param name="target"></param>
		public void RecordWindowPosition(Window target) {
			var item = WindowConfig.Create(target);
			var same = data.SingleOrDefault(x => x.Type == target.GetType().FullName);
			if (same == null) {
				data.Add(item);
			} else {
				data.Remove(same);
				data.Add(item);
			}
			
			var serializer = new XmlSerializer(typeof(WindowPositionManager));
			using (var stream = new FileStream(_filepath, FileMode.Create)) {
				serializer.Serialize(stream, this);
			}

		}

		/// <summary>
		/// このクラスに記録している情報をtargetウィンドウに反映する
		/// </summary>
		/// <param name="target"></param>
		public void RestoreWindowPosition(Window target) {
			var config = data.SingleOrDefault(x => x.Type == target.GetType().FullName);
			if (config != null) {
				config.Restore(target);
			} else {
				//データが登録されていない場合は何もしない
			}
		}

		#endregion Public Method
	}
}
