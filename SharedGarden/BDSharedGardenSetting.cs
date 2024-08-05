using System;
using System.Runtime.InteropServices;
using SharedKit;

namespace SharedGarden
{
	public class BDSharedGardenSetting : SharedKit.BDSharedSetting
	{
		public BDSharedGardenSetting()
		{
		}

		internal static bool IsDevModeChecked()
		{
			return false;
		}

		internal static bool IsBidRemoteChecked() {
			return true;
		}

		internal static bool IsBidSlaveChecked()
		{
			return true;
		}

		internal static int UpdateDayCount()
		{
			return 210;
		}

		internal static String[] DevDatKeyArray() {
			// 2202MH0676 https://ciac.zjw.sh.gov.cn/XMJYPTInterWeb/Gktb/GktbIndex?gkzbXh=187058
			// 20230210.135449.A02 20220629.127793.A02 // 20230210.135217.A02, 20230209.135345.A02
			// 20230209.135169.A02 "20230208.135658.A02", 20220705.129348.A02 20230120.135087.A02 20230704.138820.A02 20230704.138808.A02
			//var dDebugArray = new List<string>() { "*.A02", /*TODODEBUG(2.1)"20230609.138058.A02",".A02", "20230607.138118.A02", "20230522.137351.A02", "20220705.129348.A02"*/};
			var dDebugArray = new List<string>() { /*"*.A02",TODODEBUG(2.1)  "20230110.134547.A02", 20230609.138058.A02",".A02", "20230607.138118.A02", "20230522.137351.A02", "20220705.129348.A02"*/};
			return dDebugArray.ToArray();
		}

		internal static bool IsBidInSideDirCacheChecked()
		{
			return false;
		}

		internal static bool IsBidInSideDataBaseChecked()
		{
			return true;
		}

		internal static bool IsBidReportChecked()
		{
			return true;
		}

		internal static bool IsMailChecked()
		{
			return true;
		}

		internal static String DirOutPath() {
			var ret = "";
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				ret = System.Environment.CurrentDirectory;
				//BDSharedUtils.LogOut($"Current Path {ret}", ConsoleColor.DarkMagenta);
			} else {
				System.Diagnostics.Debug.Assert(false);
				BDSharedUtils.LogOut($"Not Supported System. {RuntimeInformation.OSDescription} ", ConsoleColor.DarkMagenta);
			}
			// HardCode for Debug Only
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				var dir1 = "/Library/HOOK/Storage";
				if (BDSharedUtils.CheckDirPathIfNeeded(dir1)) {
					ret = dir1;
				}
				var dir2 = "/Users/KOKOAR/HEYRIX/SkyDrive/Storage";
				if (BDSharedUtils.CheckDirPathIfNeeded(dir2)) {
					ret = dir2;
				}
			}
			return ret;
		}

		public static bool IsDevStateChecked()
        {
			var ret = false;
		#if DEBUG
			ret = true;
		#else
			ret = false;
		#endif
			return ret;
		}

		public static string GetVersion() {
			return "0.1.0";
		}
	}
}

