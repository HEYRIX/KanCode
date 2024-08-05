using System;
namespace SharedKit
{
	public class BDSharedSetting : BDSharedSingletonClass<BDSharedSetting>
	{
		private Dictionary<String, List<String>> Dict { get; set; }
		public BDSharedSetting()
		{
			this.Dict = new Dictionary<string, List<string>>();
		}

		public static void Init(String filePath)
		{
		}
	}
}
