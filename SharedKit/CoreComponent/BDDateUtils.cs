using System;
using System.Globalization;

namespace SharedKit
{
	public enum BDDateStyle
	{
		kNone,
		kDateNormal,
		kUnixTime,
	}

	public enum DateUnit
	{
		None,
		Hourly,
		Daily,
		Weekly,// 以周日为每周的起始
		Monthly,
		Quarterly,
		Yearly,
	}

	public class BDDateUtils
	{
		private BDDateUtils() {
		}

		/// <summary>
		/// 该日期的该月的第一天
		/// </summary>
		/// <param name="datetime"><see cref="DateTime"/>一个日期</param>
		/// <returns><see cref="DateTime"/>第一天的日期</returns>
		//public static DateTime FirstDayOfMonth(DateTime datetime)
		//{
		//	return new DateTime(datetime.Year, datetime.Month, 1);
		//}

		//public static DateTime LastDayOfMonth(DateTime datetime)
		//{
		//	return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
		//}

		public static (DateTime, DateTime) GetDateArray(DateTime date, DateUnit unit, int delta) {
			// Return [date1, date2]
			var date1 = date;
			var date2 = date;
			switch (unit) {
				case DateUnit.Hourly:
					date1 = date.AddHours(delta).AddMinutes(1 - date.Minute);
					date2 = date.AddHours(delta).AddMinutes(1 - date.Minute).AddHours(1).AddSeconds(-1);
					break;
				case DateUnit.Daily:
					date1 = date.AddDays(delta).AddHours(1 - date.Hour).Date;
					date2 = date.AddDays(delta).AddHours(1 - date.Hour).Date.AddDays(1).AddSeconds(-1);
					break;
				case DateUnit.Weekly:
					//DateTime startWeek = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")));  //本周周一  
					//DateTime endWeek = startWeek.AddDays(6);  //本周周日
					date1 = date.AddDays(1 - Convert.ToInt32(date.DayOfWeek.ToString("d")));
					date2 = date.AddDays(1 - Convert.ToInt32(date.DayOfWeek.ToString("d")));
					break;
				case DateUnit.Monthly:
					//date1 = date.AddDays(1 - date.Day).AddMonths(delta).Date;
					//date2 = date.AddDays(1 - date.Day).AddMonths(delta + 1).AddDays(-1).AddSeconds(-1);
					//当月第一天0时0分0秒
					date1 = date.AddMonths(delta).AddDays(1 - date.Day).Date;
					//当月最后一天23时59分59秒
					date2 = date.AddMonths(delta).AddDays(1 - date.Day).Date.AddMonths(1).AddSeconds(-1);
					break;
				case DateUnit.Quarterly:
					date1 = date.AddMonths(0 - (date.Month - 1) % 3).AddDays(1 - date.Day);
					date2 = date1.AddMonths(3).AddDays(-1);
					break;
				case DateUnit.Yearly:
					//date1 = new DateTime(date.Year + delta, 1, 1);
					//date2 = new DateTime(date.Year + delta, 12, 31);
					date1 = date.AddYears(delta).AddMonths(1 - date.Month).AddDays(1 - date.Day).Date;
					date2 = date1.AddYears(1).Date.AddSeconds(-1);
					break;
				case DateUnit.None:
				default:
					break;
			}
			return (date1, date2);
		}

		public static (DateTime, DateTime) DayOfPreviousMonth(DateTime date) {
			var first = date.AddDays(1 - date.Day).AddMonths(-1);
			var last = date.AddDays(1 - date.Day).AddMonths(0).AddDays(-1);
			return (first, last);
		}

		public static (DateTime, DateTime) DayOfCurrentMonth(DateTime date) {
			//当月第一天0时0分0秒
			//DateTime.Now.AddDays(1 - DateTime.Now.Day).Date
			//当月最后一天23时59分59秒：
			//DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.AddMonths(1).AddSeconds(-1)
			var first = new DateTime(date.Year, date.Month, 1);
			var last = date.AddDays(1 - date.Day).AddMonths(1).AddDays(-1);
			return (first, last);
		}

		public static (DateTime, DateTime) DayRangeOfMonth(DateTime date, DateUnit rangeStyle) {
			var first = new DateTime(date.Year, date.Month, 1);
			var last = date.AddDays(1 - date.Day).AddMonths(1).AddDays(-1);
			return (first, last);
		}

		/// <summary>
		/// 根据时间得到目录名yyyyMMdd
		/// </summary>
		/// <returns></returns>
		//public static String GetDate(String format)
		//{
		//	var fmt = (0 == format.Length) ? "yyyyMMdd" : format;
		//	return DateTime.Now.ToString(fmt);
		//}
		public static DateTime ToDate(String str, Boolean isUnixTimeStamp = false) {
			//Use of Convert.ToDateTime()   
			DateTime date = isUnixTimeStamp ? ConvertUnixTimeToDate(str) : Convert.ToDateTime(str);
			return date;
		}

		public static String ReUpdateDate(String format, DateTime date, int delta) {
			// str.Replace("年", "-").Replace("月", "-").Replace("日", "");
			var fmt = (0 == format.Length) ? "yyyyMMdd" : format;
			var newDate = date.AddDays(delta);
			return newDate.ToString(fmt);
		}

		//public static DateTime GetDate(String value)
		//{ //TODO NOT Check
		//  //DateTime dt;
		//  //DateTimeFormatInfo dtFormat = new System.Globalization.CultureInfo.get;

		//	//dtFormat.ShortDatePattern = "yyyy/MM/dd";
		//	//dt = Convert.ToDateTime("2011/05/26", dtFormat);
		//	DateTime ret = Convert.ToDateTime(value);
		//	//try {
		//	//	//ret = Convert.ToDateTime(value);
		//	//	Console.WriteLine("'{0}' converts to {1} {2} time.", value, ret);
		//	//} catch (FormatException) {
		//	//	Console.WriteLine("'{0}' is not in the proper format.", value);
		//	//}
		//	return ret;
		//}

		public static String GetDate(String format, DateTime date, int delta) {
			var fmt = (0 == format.Length) ? "yyyyMMdd" : format;
			var newDate = date.AddDays(delta);
			return newDate.ToString(fmt);
		}

		/// <summary>
		/// 根据时间得到文件名HHmmssff
		/// </summary>
		/// <returns></returns>
		public static string GetTime(String format = "") {
			var fmt = format.Length > 0 ? format : "HHmmssff";
			return DateTime.Now.ToString(fmt);
		}

		private static DateTime ConvertUnixTimeToDate(string str) {
			// Unix timestamp is seconds past epoch
			double unixTimeStamp = double.Parse(str);
			var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			date = date.AddMilliseconds(unixTimeStamp).ToLocalTime();
			return date;
		}

		public static double DateTimeToUnixTimestamp(DateTime dateTime)
		{
			var date0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			return (TimeZoneInfo.ConvertTimeToUtc(dateTime) - date0).TotalSeconds;
		}

		/// <summary>
		/// 该日期的该月的最后一天
		/// </summary>
		/// <param name="datetime"><see cref="DateTime"/>一个日期</param>
		/// <returns><see cref="DateTime"/>最后一天的日期</returns>
		//public static DateTime LastDayOfMonth(this DateTime datetime)
		//{
		//	return datetime.FirstDayOfMonth().AddMonths(1).AddDays(-1);
		//}

		#region WatchRegion
		private static Dictionary<String, System.Diagnostics.Stopwatch>? WatchSet { get; set; }
		public static bool StartWatch(String key)
		{
			if (WatchSet == null) {
				// https://stackoverflow.com/questions/55686928/using-stopwatch-in-c-sharp
				// https://blog.darkthread.net/blog/measure-microsecond-with-stopwatch/
				WatchSet = new Dictionary<string, System.Diagnostics.Stopwatch>();
			}
			var ret = false;
			System.Diagnostics.Debug.Assert(key.Trim().Length > 0);
			if (key.Trim().Length > 0) {
				if (WatchSet.ContainsKey(key)) {
					var watch = WatchSet.ContainsKey(key) ? WatchSet[key] : new System.Diagnostics.Stopwatch();
					watch.Restart();
				} else {
					var watch = new System.Diagnostics.Stopwatch();
					watch.Start();
					WatchSet.Add(key, watch);
					ret = true;
				}
			}
			return ret;
		}

		public static TimeSpan StopWatch(String key)
		{
			System.Diagnostics.Debug.Assert(WatchSet != null);
			System.Diagnostics.Debug.Assert(key.Trim().Length > 0);

			var ret = new TimeSpan();
			if (key.Trim().Length > 0 && WatchSet.ContainsKey(key)) {
				var watch = WatchSet[key];
				watch.Stop();

				var timeTaken = watch.Elapsed;
				//var foo = "Time taken: " + timeTaken.ToString(@"mm\:ss\.ffff");
				//SharedKit.BDSharedUtils.LogOut($"{foo}", ConsoleColor.DarkGreen);
				ret = timeTaken;

				WatchSet.Remove(key);
				watch = null;
			} else {
				System.Diagnostics.Debug.Assert(false);
			}
			return ret;
		}

		public static String SpanWatch(TimeSpan span)
		{
			return span.ToString(@"mm\:ss\.ffff");
		}
		#endregion
	}
}
