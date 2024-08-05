using System;
using SharedGarden.AnalyzeKit;
using SharedStudy;

namespace SharedGarden
{
	public enum BBWorkKind
	{
		None,
		BidGarden,
		BidCompare,
		BidComposite,
		// jzsc.mohurd
		BidService, 
		KidStudy,
		All,
	}

	public class BBSharedRunner
	{
		private static readonly object _locker = new object();
		private static BBSharedRunner _instance;
		private BBSharedRunner() {
		}

		public static BBSharedRunner Instance() {
			lock (_locker) {
				if (null == _instance) {
					_instance = new BBSharedRunner();
				}
			}
			return _instance;
		}

		private static async Task<int> RunGardenWorking() {
			//BDKidManager.RunDiffDetail("", "");
			_ = await SharedKit.BDSharedManager.RunSharedWorking();

			//await BBAnalyzeManager.Run();
			//await SharedKit.BDSharedRunner.Run();
			//await HawkKit.BBHawkManager.Run();
			//await SharedStudy.BDStudyManager.Run();

			var dirRootPath = SharedKit.BDSharedUtils.SharedDirPath();
			SharedKit.BDSharedUtils.LogOut($"{dirRootPath}", ConsoleColor.Blue);
			//_ = BDStudyReadManager.HandlePdfContent();
			await BDGardenManager.RunAllWorking();
			RunAnalyzeWorking();
			return 1;
		}

		public static async Task<int> Run(BBWorkKind kind) {
			//BDKidManager.RunDiffDetail("", "");
			_ = await SharedKit.BDSharedManager.RunSharedWorking();

			//await BBAnalyzeManager.Run();
			//await SharedKit.BDSharedRunner.Run();
			//await HawkKit.BBHawkManager.Run();
			//await SharedStudy.BDStudyManager.Run();

			var dirRootPath = SharedKit.BDSharedUtils.SharedDirPath();
			SharedKit.BDSharedUtils.LogOut($"{dirRootPath}", ConsoleColor.Blue);
			var userInput = ""; var dirPathOut = "";
			switch (kind) {
				case BBWorkKind.BidGarden:
					await BDGardenManager.RunAllWorking();
					break;
				// =================================
				case BBWorkKind.BidComposite:
					Console.WriteLine("Please input Site: ");
#if DEBUG
					//userInput = "https://ciac.zjw.sh.gov.cn/XMJYPTInterWeb/Currently/FileView?wjlx=JYPT_Zbwj&showlb=pdf&id=75810";
					//userInput = "https://ciac.zjw.sh.gov.cn/XMJYPTInterWeb/Currently/FileView?id=77380&wjlx=JYPT_Zbwj&showlb=pdf";
					// 招标文件
					//userInput = "https://ciac.zjw.sh.gov.cn/XMJYPTInterWeb/fbgg/FbggDetails?Dhcs=oNxNFOVDZJl/LrdNYXl6YQf80Lervla9768OCBSnukQNMCUNcZrIXnwJbgtoKbQofAxu2GIQXnsNw3vJ3mBpqi2tiLNJdcSJMDrE0O1BgUEyce3ctPTnygPW9/UFDbBjCHhFhojI2G6yar3lB0LIMXQbOuHsW5dR5kkizceViTJq4KCVqNwXbPAD4q8StxH8";
					// 资格预审文件 + 招标文件
					//userInput = "https://ciac.zjw.sh.gov.cn/XMJYPTInterWeb/fbgg/FbggDetails?Dhcs=OzFUZcp77Z/3ArS2ykLOoJ+42dknkwAlyFbUa7RZnn/ikXkv5cpPXc9dE2D7V0xth7ItMH06Kslmcaj2EdSEZt8XtLFeFXHqOG2u/HjvpMuPvwUaXnJcANyuXLh40G8TYEVSUWlhV1isRk3MeobSerFFzJdkjZ+qTZA5e1snmqGv5N8zx1H+to+5x6O/xdkFp3jlQFG3RtefMEYFtQTGHW/9Db8tG4H1XVpjDgXFu0pkw0pcYZNgPneCxc0dTfiQ7Ptzk/UJ9yXzGteWhM+rlw==";
					userInput = Console.ReadLine() ?? "";
#else
					userInput = Console.ReadLine() ?? "";
#endif
					SharedKit.BDSharedUtils.WriteConsole("Received: " + $"[{userInput}]", ConsoleColor.Blue);

					Console.WriteLine("Please input Directory: ");
#if DEBUG
					//dirPathOut = "/Users/KOKOAR/HEYRIX/BidStore-bak/";
					//dirPathOut = "D:\\BidGardenStore";
					dirPathOut = Console.ReadLine() ?? "";
#else
					dirPathOut = Console.ReadLine() ?? "";
#endif
					SharedKit.BDSharedUtils.WriteConsole("Received: " + $"[{dirPathOut}]", ConsoleColor.Blue);
					await BDBidCompositeManager.RunAllWorking(userInput, dirPathOut);
					break;
				// =================================
				case BBWorkKind.BidService:
					Console.WriteLine("Please input CreditKey: ");
#if DEBUG
					userInput = "91310107132292762H";
#else
					userInput = Console.ReadLine() ?? "";
#endif
					SharedKit.BDSharedUtils.WriteConsole("Received: " + $"[{userInput}]", ConsoleColor.Blue);
#if DEBUG
					dirPathOut = "/Users/KOKOAR/HEYRIX/BidGardenStore/";
					//dirPathOut = "D:\\BidGardenStore";
#else
					dirPathOut = Console.ReadLine() ?? "";
#endif
					SharedKit.BDSharedUtils.WriteConsole("Received: " + $"[{dirPathOut}]", ConsoleColor.Blue);
                    await BDBidServiceManager.RunAllWorking(userInput, dirPathOut);
					break;
				case BBWorkKind.KidStudy:
					await SharedStudy.BDStudyManager.Run();
					break;
				default:
					break;
			}
			RunAnalyzeWorking();
			return 1;
		}

		private static void RunAnalyzeWorking() {
			var filePath = System.IO.Path.Join(SharedKit.BDSharedUtils.SharedDirPath(), "try.docx");
			SharedKit.BDSharedUtils.ReadSystemDirItem(filePath);
		}

		public static async Task RunOnce() {
			// See https://aka.ms/new-console-template for more information
			Console.WriteLine("Hello, World!");

			//await SharedGarden.BBSharedRunner.Run();
			//_ = await SharedGarden.BBSharedRunner.Run();

			//Func<int> func = async () => {
			//	var ret = await SharedGarden.BBSharedRunner.Run();
			//	return ret;
			//};

			//AsyncCallback callback = c => Console.WriteLine("call back");
			//IAsyncResult result = func.BeginInvoke(callback, "first");
			//if (result.AsyncWaitHandle.WaitOne(-1)) {
			//	Console.WriteLine("333 Wonderful World ...");
			//}
			var task = SharedGarden.BBSharedRunner.RunGardenWorking();
			//Task<int> task = Task.Run(func);
			//task.Wait();
			//Task<int> task = SharedGarden.BBSharedRunner.Run();
			//var follow = task.ContinueWith(callback);

			//	Task.Run(async () => {
			//	await SharedGarden.BBSharedRunner.Run();
			//}).Wait(10);

			//task.GetAwaiter().OnCompleted(() => {
			//	Console.WriteLine("Wonderful World ...");
			//});
			// https://www.wake-up-neo.com/zh/c%23/%E8%BF%90%E8%A1%8C%E5%A4%9A%E4%B8%AA%E5%BC%82%E6%AD%A5%E4%BB%BB%E5%8A%A1%E5%B9%B6%E7%AD%89%E5%BE%85%E5%AE%83%E4%BB%AC%E5%85%A8%E9%83%A8%E5%AE%8C%E6%88%90/1048726801/
#pragma warning disable CA1842 // Do not use 'WhenAll' with a single task
			await Task.WhenAll(task);
#pragma warning restore CA1842 // Do not use 'WhenAll' with a single task
			Console.WriteLine("Wonderful World ...");
			//// Waiting for you
			//Console.ReadKey();
		}
	}
}
