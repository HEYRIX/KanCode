using System;
using System.Runtime.InteropServices;

namespace SharedKit.CoreComponent
{
	public class BDDirInfo {
		public String? fileKey { get; set; }
		public FileInfo? fileInfo { get; set; }
		public FileStream? Stream { get; set; }
	}

	public class BDPathUtils
	{
		public static string GetUserPath()
		{
			// Personal /Users/username
			// Desktop Personal+Desktop
			string dir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return dir;
		}

		public static bool EnsurePathReadyIfNeed(string totalPath) {
			var ret = File.Exists(totalPath);
			if (!ret) {
				EnsureDirPathRecursiveIfNeed(totalPath);
				//var keySplit = '/'; // '\\'
				//string filePath = totalPath.Substring(1, totalPath.LastIndexOf(keySplit));
				//TODOCreateFile(totalPath, null);
				ret = true;
			}
			return ret;
		}

		private static bool EnsureDirPathRecursiveIfNeed(string fullPath) {
			bool ret = false;
			if (File.Exists(fullPath)) {
				ret = true;
			} else {
				var keySplit = '/'; // '/' for MacOS, '\\' for WinOS.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
					keySplit = '\\';
				}
				string dirpath = fullPath.Substring(0, fullPath.LastIndexOf(keySplit));
				string[] pathes = dirpath.Split(keySplit);
				if (pathes.Length > 1) {
					string path = pathes[0];
					for (int i = 1; i < pathes.Length; i++) {
						path += $"{keySplit}" + pathes[i];
						if (!Directory.Exists(path)) {
							Directory.CreateDirectory(path);
						}
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// 创建一个文件,并将字节流写入文件。
		/// </summary>
		/// <param name="filePath">文件的绝对路径</param>
		/// <param name="buffer">二进制流数据</param>
		public static void CreateFile(string filePath, byte[]? buffer) {
			try {
				if (!File.Exists(filePath)) {
					var file = new FileInfo(filePath);
					using FileStream fs = file.Create();
					if (null != buffer) {
						fs.Write(buffer, 0, buffer.Length);
					}
					fs.Close();
				}
			} catch (Exception ex) {
                throw ex;
			}
		}

		/// <summary>
		/// 获取指定目录及子目录中所有文件列表
		/// </summary>
		/// <param name="directoryPath">指定目录的绝对路径</param>
		/// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
		/// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
		/// <param name="isSearchChild">是否搜索子目录</param>
		public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
		{
			if (!Directory.Exists(directoryPath)) {
				throw new FileNotFoundException();
			}

			try {
				var option = isSearchChild ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
				return Directory.GetFiles(directoryPath, searchPattern, option);
			} catch (IOException ex) {
				throw ex;
			}
		}

		public static List<FileInfo> GetFileInfoArray(List<String> filePathArray) {
			var ret = new List<FileInfo>();
			foreach (var filePath in filePathArray) {
				if (File.Exists(filePath)) {
					var fileInfo = new FileInfo(filePath);
					ret.Add(fileInfo);
				}
			}
			return ret;
		}

		/// <summary>
		/// 检测指定目录是否为空
		/// </summary>
		/// <param name="directoryPath">指定目录的绝对路径</param>        
		public static bool IsEmptyDirectory(string directoryPath) {
			try {
				//判断是否存在文件
				string[] fileNames = GetFileNames(directoryPath, null, true);
				if (fileNames.Length > 0) {
					return false;
				}
				//判断是否存在文件夹
				string[] directoryNames = GetDirectories(directoryPath, "*", true);
				return directoryNames.Length == 0 ? true : false;
			} catch {
				return true;
			}
		}

		/// <summary>
		/// 检测指定目录中是否存在指定的文件
		/// </summary>
		/// <param name="directoryPath">指定目录的绝对路径</param>
		/// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
		/// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param> 
		/// <param name="isSearchChild">是否搜索子目录</param>
		public static bool Contains(string directoryPath, string searchPattern, bool isSearchChild)
		{
			try {
				string[] fileNames = GetFileNames(directoryPath, searchPattern, true);
				if (fileNames.Length == 0) {
					return false;
				} else {
					return true;
				}
			} catch (Exception ex) {
				throw new Exception(ex.Message);
				//LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
			}
		}

		/// <summary>
		/// 复制文件夹(递归)
		/// </summary>
		/// <param name="varFromDirectory">源文件夹路径</param>
		/// <param name="varToDirectory">目标文件夹路径</param>
		public static void CopyDirectory(string varFromDirectory, string varToDirectory)
		{
			Directory.CreateDirectory(varToDirectory);
			if (!Directory.Exists(varFromDirectory)) return;

			string[] directories = Directory.GetDirectories(varFromDirectory);
			if (directories.Length > 0) {
				foreach (string d in directories) {
					CopyDirectory(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
				}
			}
			string[] files = Directory.GetFiles(varFromDirectory);
			if (files.Length > 0) {
				foreach (string s in files) {
					File.Copy(s, varToDirectory + s.Substring(s.LastIndexOf("\\")), true);
				}
			}
		}

		/// <summary>
		/// Create a directory path.
		/// </summary>
		/// <param name="directoryPath">目录的绝对路径</param>
		public static void CreateDirectory(string directoryPath)
		{
			if (!Directory.Exists(directoryPath)) {
				Directory.CreateDirectory(directoryPath);
			}
		}

		/// <summary>
		/// 删除指定文件夹对应其他文件夹里的文件
		/// </summary>
		/// <param name="varFromDirectory">指定文件夹路径</param>
		/// <param name="varToDirectory">对应其他文件夹路径</param>
		public static void DeleteFolderFiles(string varFromDirectory, string varToDirectory)
		{
			Directory.CreateDirectory(varToDirectory);
			if (!Directory.Exists(varFromDirectory)) return;
			string[] directories = Directory.GetDirectories(varFromDirectory);
			if (directories.Length > 0) {
				foreach (string d in directories) {
					DeleteFolderFiles(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
				}
			}

			string[] files = Directory.GetFiles(varFromDirectory);
			if (files.Length > 0) {
				foreach (string s in files) {
					File.Delete(varToDirectory + s.Substring(s.LastIndexOf("\\")));
				}
			}
		}

		/// <summary>
		/// 从文件的绝对路径中获取文件名( 包含扩展名 )
		/// </summary>
		/// <param name="filePath">文件的绝对路径</param>        
		public static string GetFileName(string filePath)
		{
			var fi = new FileInfo(filePath);
			return fi.Name;
		}

		/// <summary>
		/// 获取文本文件的行数
		/// </summary>
		/// <param name="filePath">文件的绝对路径</param>        
		public static int GetLineCount(string filePath)
		{
			string[] rows = File.ReadAllLines(filePath);
			return rows.Length;
		}

		/// <summary>
		/// 获取一个文件的长度,单位为Byte
		/// </summary>
		/// <param name="filePath">文件的绝对路径</param>        
		public static int GetFileSize(String filePath)
		{
			var ret = 0;
			if (File.Exists(filePath)) {
				var fi = new FileInfo(filePath);
				ret = (null == fi) ? 0 : (int)fi.Length;
			}
			return ret;
		}

		/// <summary>
		/// 获取指定目录及子目录中所有子目录列表
		/// </summary>
		/// <param name="directoryPath">指定目录的绝对路径</param>
		/// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
		/// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
		/// <param name="isSearchChild">是否搜索子目录</param>
		public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
		{
			try {
				var option = isSearchChild ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
				return Directory.GetDirectories(directoryPath, searchPattern, option);
			} catch (IOException ex) {
				throw ex;
			}
		}

		/// <summary>
		/// 将文件移动到指定目录
		/// </summary>
		/// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
		/// <param name="descDirectoryPath">移动到的目录的绝对路径</param>
		public static void Move(string sourceFilePath, string descDirectoryPath)
		{
			//获取源文件的名称
			//string sourceFileName = GetFileName(sourceFilePath);
			//if (IsExistDirectory(descDirectoryPath)) {
			//	//如果目标中存在同名文件,则删除
			//	if (IsExistFile(descDirectoryPath + "\\" + sourceFileName)) {
			//		DeleteFile(descDirectoryPath + "\\" + sourceFileName);
			//	}
			//	//将文件移动到指定目录
			//	File.Move(sourceFilePath, descDirectoryPath + "\\" + sourceFileName);
			//}
		}

		/// <summary>
		/// 从文件的绝对路径中获取扩展名
		/// </summary>
		/// <param name="filePath">文件的绝对路径</param>        
		public static string GetExtension(string filePath)
		{
			FileInfo fi = new FileInfo(filePath);
			return fi.Extension;
		}

		/// <summary>
		/// 清空文件内容
		/// </summary>
		/// <param name="filePath">文件的绝对路径</param>
		public static void ClearFile(string filePath)
		{
			File.Delete(filePath);
			CreateFile(filePath, null);
		}

		/// <summary>
		/// 删除指定目录及其所有子目录
		/// </summary>
		/// <param name="directoryPath">指定目录的绝对路径</param>
		public static void DeleteDirectory(string directoryPath)
		{
			if (Directory.Exists(directoryPath)) {
				Directory.Delete(directoryPath, true);
			}
		}
	}
}
