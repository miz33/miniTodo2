using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace miniTodo.UserSettings {
	public class Settings {
		#region private field
		private const string _filepath = "UserSettings.conf";
		#endregion

		#region public property

		public OperationSettings Operation { get; set; } = new OperationSettings();

		/// <summary>
		/// 操作等の効果音の音量(min:0, max:100)
		/// </summary>
		public int SE_Volume { get; set; } = 5;

		private static Settings _instance;
		public static Settings Instance {
			get {
				if (_instance == null) {
					_instance = Settings.Load();
				}
				return _instance;
			}
		}
		#endregion public property

		#region Method
		public Settings() {
		}

		public void Save() {
			var serializer = new XmlSerializer(typeof(Settings));
			using (var stream = new FileStream(_filepath, FileMode.Create)) {
				serializer.Serialize(stream, this);
			}
		}
		private static Settings Load() {
			if (File.Exists(_filepath)) {
				Settings data = null;
				var serializer = new XmlSerializer(typeof(Settings));
				using (var stream = new FileStream(_filepath, FileMode.Open)) {
					data = serializer.Deserialize(stream) as Settings;
				}
				return data;
			} else {
				return new Settings() {
					SE_Volume = 20
				};
			}
		}
		#endregion Method
	}
}

