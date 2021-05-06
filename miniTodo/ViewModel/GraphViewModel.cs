using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miniTodo.ViewModel {
	public class GraphViewModel : ViewModelBase {
		public enum Mode {
			Daily,
			Weekly,
			Monthly
		}

		private const int DaysForDisplay = 30; //Daily
		private const int WeeksForDisplay = 24; //Weekly
		private const int MonthsForDisplay = 18; //Monthly
		private const int CountOfTerm = 28;
		private List<KeyValuePair<DateTime, double>> _done = new List<KeyValuePair<DateTime, double>>();
		private List<KeyValuePair<DateTime, double>> _create = new List<KeyValuePair<DateTime, double>>();
		private List<KeyValuePair<DateTime, double>> _point = new List<KeyValuePair<DateTime, double>>();

		#region Constructor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">元データ</param>
		/// <param name="step">X軸の単位（日）</param>
		public GraphViewModel(IEnumerable<TodoViewModel> data, Mode mode) {
			switch (mode) {
				case Mode.Daily:
					CountDaily(data);
					break;
				case Mode.Weekly:
					CountWeekly(data);
					break;
				case Mode.Monthly:
					CountMonthly(data);
					break;
				default:
					throw new InvalidOperationException();
			}
		}
		#endregion

		#region Property
		public List<KeyValuePair<DateTime, double>> DoneNum {
			get {
				return _done;
			}
		}

		public List<KeyValuePair<DateTime, double>> CreateNum {
			get {
				return _create;
			}
		}

		public List<KeyValuePair<DateTime, double>> Point {
			get {
				return _point;
			}
		}
		#endregion

		#region private method
		private void CountDaily(IEnumerable<TodoViewModel> data) {
			for (int i = 0; i < DaysForDisplay; i++) {
				var searchDay = DateTime.Today.AddDays(-1 * i);
				var created = data.Count(item => !item.IsDeleted && item.Created.Date.Equals(searchDay));
				var done = data.Count(item => !item.IsDeleted && item.IsDone
					&& item.StatusUpdated.Date.Equals(searchDay));
				var point = 0.5 * created + 1.0 * done;
				_create.Add(new KeyValuePair<DateTime, double>(searchDay, created));
				_done.Add(new KeyValuePair<DateTime, double>(searchDay, done));
				_point.Add(new KeyValuePair<DateTime, double>(searchDay, point));
			}
		}

		private void CountWeekly(IEnumerable<TodoViewModel> data) {
			var targetData = data.Where(item => !item.IsDeleted);
			
			//mondayから始まる１週間に入っているかどうかを判定する関数
			Func< DateTime, DateTime, bool> isInWeek = (monday, target) => {
				var span = (target.Date - monday.Date).Days;
				return 0 <= span && span< 7;
			};

			for (int i = 0; i < WeeksForDisplay; i++) {
				var dayInWeekForSearch = DateTime.Today.AddDays(-7 * i);
				var lastMonday = dayInWeekForSearch.AddDays((((int)dayInWeekForSearch.DayOfWeek +6)%7)* -1);
				var created = targetData.Count(item => isInWeek(lastMonday, item.Created));
				var done = targetData.Count(item => item.IsDone && isInWeek(lastMonday, item.StatusUpdated));
				var point = 0.5 * created + 1.0 * done;
				_create.Add(new KeyValuePair<DateTime, double>(lastMonday, created));
				_done.Add(new KeyValuePair<DateTime, double>(lastMonday, done));
				_point.Add(new KeyValuePair<DateTime, double>(lastMonday, point));
			}
		}

		private void CountMonthly(IEnumerable<TodoViewModel> data) {
			var targetData = data.Where(item=>!item.IsDeleted);
			for (int i = 0; i < MonthsForDisplay; i++) {
				var searchMonth = DateTime.Today.AddMonths(-1 * i);
				var created = targetData.Count(item =>
					item.Created.Year.Equals(searchMonth.Year) && item.Created.Month.Equals(searchMonth.Month));
				var done = targetData.Count(item => item.IsDone &&
					item.StatusUpdated.Year.Equals(searchMonth.Year) && item.StatusUpdated.Month.Equals(searchMonth.Month));
				var point = 0.5 * created + 1.0 * done;
				var plotDay = searchMonth.AddDays(-1 * searchMonth.Day + 1); //グラフは１日にそろえる
				_create.Add(new KeyValuePair<DateTime, double>(plotDay, created));
				_done.Add(new KeyValuePair<DateTime, double>(plotDay, done));
				_point.Add(new KeyValuePair<DateTime,double>(plotDay, point));
			}
		}
		#endregion
	}
}
