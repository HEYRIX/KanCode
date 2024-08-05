using System;
using System.Runtime.InteropServices;

namespace SharedKit
{
	public enum BDSystemCode
	{
		None,
		WinOS,
		MacOS,
		Linux,
	}

	public class BDSharedConst
	{
		private BDSharedConst()
		{
		}

		public static BDSystemCode GetSystemCode()
		{
			var ret = BDSystemCode.None;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				ret = BDSystemCode.MacOS;
			}
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				ret = BDSystemCode.WinOS;
			}
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				ret = BDSystemCode.Linux;
			}
			return ret;
		}

		public static String GetSpaceCode()
		{
			return "&nbsp;";
		}
	}
}
