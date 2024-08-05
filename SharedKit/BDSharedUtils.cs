using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SharedKit
{
	public enum BDLogState
	{
		None,
		Info,
		Warning,
		Error,
		NotSupported,
		Specific,
	}

	// MiCroNetCore TODO
	// 包含一些常用的操作类，大都是静态类，加密解密，反射操作，权重随机筛选算法，分布式短id，表达式树，linq扩展，文件压缩，多线程下载和FTP客户端，
	// 硬件信息，字符串扩展方法，日期时间扩展操作，中国农历，大文件拷贝，图像裁剪，验证码，断点续传，集合扩展、Excel导出等常用封装。
	// https://github.com/ldqk/Masuit.Tools
	// https://github.com/Jimmey-Jiang/Common.Utility
	// https://github.com/jasonhua95/awesome-dotnet-core
	// https://github.com/luoyunchong/lin-cms-dotnetcore
	// https://github.com/AutoMapper/AutoMapper
	// https://github.com/bing-framework/Bing.NetCore
	// QQ频道机器人 https://github.com/ssccinng/Masuda.Net
	// TODO NetCore获取本地网络IP地址 https://cloud.tencent.com/developer/article/1365597
	// https://github.com/2881099/FreeIM
	// https://github.com/toolgood/ToolGood.Words
	public class BDSharedUtils
	{
		public static String StandardDateCode()
		{
			// https://blog.csdn.net/yinyaling/article/details/3722029
			return DateTime.Now.ToString("yyyyMMddHHmmss");
		}

		public static String DateCodeKey() {
			// Unix timestamp
			// DateTimeOffset.Now.ToUnixTimeSeconds()
			// https://www.cnblogs.com/jhxk/articles/1618194.html
			// yyyy.MM.dd HH:mm:ss.zzz
			// yyyy-MM-dd HH:mm:ss.fff
			//var str2 = DateTime.Now.ToUniversalTime().ToString();
			//var str3 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			//var str1 = DateTime.Now.ToString("yyyyMMddHHmmsszz");
			var ret = DateTime.Now.ToString("yyyyMMddHHmmss");
			//return Int64.Parse(str3);
			return ret;
		}

		public static bool CheckDirPathIfNeeded(String fileUrl)
		{
			try {
				if (!Directory.Exists(fileUrl)) {
					Directory.CreateDirectory(fileUrl);
				}
				return true;
			} catch (Exception) {
				return false;
			}
		}

		public static bool IsDirPathChecked(String path)
		{
			var ret = false;
			if (Directory.Exists(path)) {
				ret = true;
			} else {
				if (File.Exists(path)) {
					ret = true;
				}
			}
			return ret;
		}

		public static string SharedDirPath()
		{
			// Return the Root Directory Path where locates in the Application's path.
			var ret = "";
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				ret = System.Environment.CurrentDirectory;
				//BDSharedUtils.LogOut($"Current Path {ret}", ConsoleColor.DarkMagenta);
			} else {
				System.Diagnostics.Debug.Assert(false);
				BDSharedUtils.LogOut($"Not Supported System. {RuntimeInformation.OSDescription} ", ConsoleColor.DarkMagenta);
			}

			//#if DEBUG
			// HardCode for MacOS System TODO Make it as dynamic, not hard code.
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				var dir1 = "/Library/HOOK/Storage";
				if (CheckDirPathIfNeeded(dir1)) {
					ret = dir1;
				}
				var dir2 = "/Users/KOKOAR/HEYRIX/SkyDrive/Storage";
				if (CheckDirPathIfNeeded(dir2)) {
					ret = dir2;
				}
			}

			// HardCode for WinOS System TODO Make it as dynamic, not hard code.
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				var dir3 = "D:\\BidStore\\BidGardenStore";
				if (CheckDirPathIfNeeded(dir3)) {
					ret = dir3;
				}
			}
			//#endif
			return ret;
		}

		public static void LogOut(string str, ConsoleColor? color = null)
		{
			var tick = BDSharedUtils.DateCodeKey();
			SharedKit.CoreComponent.BDConsoleUtils.WriteLine($"[{tick}] {str}", color);
		}

		public static void LogOut(string str, BDLogState logState)
		{
			var color = ConsoleColor.Black;
			switch (logState) {
				case BDLogState.None: { } break;
				case BDLogState.Info: { color = ConsoleColor.DarkBlue; } break;
				case BDLogState.Warning: { color = ConsoleColor.Red; } break;
				case BDLogState.Error: { color = ConsoleColor.Red; } break;
				case BDLogState.NotSupported: { color = ConsoleColor.Red; } break;
				case BDLogState.Specific: { color = ConsoleColor.Red; } break;
				default: { } break;
			}
			SharedKit.CoreComponent.BDConsoleUtils.WriteLine($"{"time"} {str}", color);
		}

		public static bool InvokeSystemBrowser(String str)
		{
			var ret = false;
			//System.Diagnostics.Process.Start("iexplore.exe", str);
			//System.Diagnostics.Process.Start(str);
			// "http://www.microsoft.com";
			string target = str;
			//Use no more than one assignment when you test this code.
			//string target = "ftp://ftp.microsoft.com";
			//string target = "C:\\Program Files\\Microsoft Visual Studio\\INSTALL.HTM";
			try {
				System.Diagnostics.Process.Start(target);
				ret = true;
			} catch (System.ComponentModel.Win32Exception noBrowser) {
				if (noBrowser.ErrorCode == -2147467259) {
					//MessageBox.Show(noBrowser.Message);
				}
			} catch (System.Exception ex) {
				//MessageBox.Show(ex.Message);
				ret = false;
				System.Console.WriteLine(ex.Message);
			}
			return ret;
		}

		public static String ToDirName(String rawText) {
			//string filename = "example:filename?with*special#chars.txt";
			string cleanFilename = Regex.Replace(rawText, @"[\\/:*?""<>|&%#$^{}[]~]", "_");
			//Console.WriteLine(cleanFilename); // 输出：example_filename_with_special_chars.txt
			return cleanFilename;
		}

		public static void ReadSystemDirItem(String filePath)
		{
			var fileInfo = new FileInfo(filePath);
			if (null == fileInfo) {

			}
			Console.WriteLine("");
		}

		public static String ConvertBaiduWenKuUrl(String str, int width)
		{
			// Source: https://wenku.baidu.com/view/9a5a21cf964bcf84b9d57bea?pn=50
			// Target: https://wenku.baidu.com/share/6c0a664d43323968001c927c?share_api=1&width=1200
			if (string.IsNullOrWhiteSpace(str)) {
				// throw new ArgumentNullException("url");
				return "";
			}
			// https://www.cnblogs.com/cplemom/p/13585051.html
			var uri = new Uri(str);
			if (!uri.IsAbsoluteUri) {
				return "";
			}
			//if (string.IsNullOrWhiteSpace(uri.AbsolutePath)) {
			//    return "";
			//}
			// System.Web.HttpUtility httpUtils = System.Web.HttpUtility.
			var keyArray = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			var ret = $"https://" + $"wenku.baidu.com/share/{keyArray[1]}?share_api=1&width={width}";
			return ret;
		}

		public static String ReadTextContent(Stream stream)
        {
			// https://itextpdf.com/
			// https://api.itextpdf.com/iText7/dotnet/7.2.5/classi_text_1_1_commons_1_1_utils_1_1_filter_reader.html#ac76ff62e5b00eb01320a26ca8c3ebe03
			// Supported Since iText7 for NetCore NuGet.
			// StackOverflow tag [iText7]. [iText] for older version.
			//var pdfReader = new iText.Kernel.Pdf.PdfReader(bufByte);
			//var pdfDocument = new PdfDocument(pdfReader);
			//var pageCount = pdfDocument.GetNumberOfPages();
			//var strContent = new StringBuilder(); var strContent2 = new StringBuilder();
			//for (int i = 1; i <= pageCount; i++) {
			//	//https://stackoverflow.com/questions/40929677/itextsharp-how-to-read-table-in-pdf-file
			//	//This strategy arranges all text it finds in left - to - right lines from top to bottom(actually also taking the text line angle into account).
			//	//   Thus, it clearly is not what you need to extract text from tables with cells with multi-line content.
			//	//Depending on the document in question there are different approaches one can take:
			//	//Use the iText SimpleTextExtractionStrategy if the text drawing operations in the document in question already are in the order one wants for text extraction.
			//	//Use a custom text extraction strategy which makes use of tagging information if the document tables are properly tagged.
			//	//Use a complex custom text extraction strategy which tries to get hints from text arrangements, line paths, or background colors to guess the table cell structure and extract text cell by cell.
			//	//In this case, the OP commented that he changed LocationTextExtractionStrategy with SimpleTextExtractionStrategy, then it worked.
			//	LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
			//	PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
			//	parser.ProcessPageContent(pdfDocument.GetPage(i));
			//	strContent.Append(strategy.GetResultantText());

			//	//ITextExtractionStrategy its1 = new SimpleTextExtractionStrategy();
			//	//string s1 = PdfTextExtractor.GetTextFromPage(pdfDocument.GetFirstPage(), its1); //TODO
			//	////var sss = its1.GetResultantText();
			//	//strContent2.Append(its1.GetResultantText());
			//}
			//try {
			//	pdfReader.Close();
			//} catch (Exception ex) {
			//	Console.WriteLine($"TryCateh Colse PDF occurs some exception.");
			//}

			/////////////////
			//var pdf222 = new Spire.Pdf.PdfDocument();
			//pdf222.LoadFromStream(bufByte);
			//StringBuilder builder = new StringBuilder();
			////抽取表格
			//PdfTableExtractor extractor = new PdfTableExtractor(pdf222);
			//PdfTable[] tableLists = null;
			//for (int pageIndex = 0; pageIndex < pdf222.Pages.Count; pageIndex++) {
			//	tableLists = extractor.ExtractTable(pageIndex);
			//	if (tableLists != null && tableLists.Length > 0) {
			//		foreach (PdfTable table in tableLists) {
			//			int row = table.GetRowCount();
			//			int column = table.GetColumnCount();
			//			for (int i = 0; i < row; i++) {
			//				for (int j = 0; j < column; j++) {
			//					string text = table.GetText(i, j);
			//					builder.Append(text + " ");
			//				}
			//				builder.Append("\r\n");
			//			}
			//		}
			//	}
			//}
			return "";
		}

		public static void WriteConsole(string text, ConsoleColor? color = null) {
			SharedKit.CoreComponent.BDConsoleUtils.WriteLine(text, color);
		}

		public static String TakeNumber(String text) {
			var str = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9]+", "").Trim();
			// 如果是数字，则转换为decimal类型
			//if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$")) {
			//	decimal result = decimal.Parse(str);
			//	Console.WriteLine("使用正则表达式提取数字");
			//	Console.WriteLine(result);
			//}
			return str;
		}

		/// <summary>
		/// 判断一个字符串是否为url
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsUrl(string str) {
			try {
				string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
				return Regex.IsMatch(str, Url);
			} catch (Exception ex) {
				return false;
			}
		}
	}

	public static class Extensions
	{
		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
			where T : class
			=> source.Distinct(new DynamicEqualityComparer<T>(comparer));

		private sealed class DynamicEqualityComparer<T> : IEqualityComparer<T> where T : class
		{
			private readonly Func<T, T, bool> _func;

			public DynamicEqualityComparer(Func<T, T, bool> func) {
				_func = func;
			}

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
            public bool Equals(T x, T y) => _func(x, y);
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).

            public int GetHashCode(T obj) => 0;
		}
	}
}