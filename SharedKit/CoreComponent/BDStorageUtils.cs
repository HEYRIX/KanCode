using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HPSF;
using SharedKit.CoreComponent;

namespace SharedKit
{
	internal class BDDirUtils
	{
		/****************************************
         * 函数名称：GetPostfixStr
         * 功能说明：取得文件后缀名
         * 参    数：filename:文件名称
         * 调用示列：
         *           string filename = "aaa.aspx";        
         *           string s = Utilities.FileOperate.GetPostfixStr(filename);         
        *****************************************/
		/// <summary>
		/// 取后缀名
		/// </summary>
		/// <param name="filename">文件名</param>
		/// <returns>.gif|.html格式</returns>
		public static string GetPostfixStr(string filename) {
			int start = filename.LastIndexOf(".");
			int length = filename.Length;
			string postfix = filename.Substring(start, length - start);
			return postfix;
		}

		/****************************************
         * 函数名称：WriteFile
         * 功能说明：当文件不存时，则创建文件，并追加文件
         * 参    数：Path:文件路径,Strings:文本内容
         * 调用示列：
         *           string Path = Server.MapPath("Default2.aspx");       
         *           string Strings = "这是我写的内容啊";
         *           Utilities.FileOperate.WriteFile(Path,Strings);
        *****************************************/
		/// <summary>
		/// 写文件
		/// </summary>
		/// <param name="Path">文件路径</param>
		/// <param name="Strings">文件内容</param>
		public static void WriteFile(string Path, string data) {
			if (!System.IO.File.Exists(Path)) {
				System.IO.FileStream f = System.IO.File.Create(Path);
				f.Close();
				f.Dispose();
			}
			var f2 = new System.IO.StreamWriter(Path, true, System.Text.Encoding.UTF8);
			f2.WriteLine(data);
			f2.Close();
			f2.Dispose();
		}

		/****************************************
         * 函数名称：ReadFile
         * 功能说明：读取文本内容
         * 参    数：Path:文件路径
         * 调用示列：
         *           string Path = Server.MapPath("Default2.aspx");       
         *           string s = Utilities.FileOperate.ReadFile(Path);
        *****************************************/
		/// <summary>
		/// 读文件
		/// </summary>
		/// <param name="Path">文件路径</param>
		/// <returns></returns>
		public static string ReadFile(string Path) {
			string s = "";
			if (!System.IO.File.Exists(Path))
				s = $"File not found in {Path}";
			else {
				StreamReader f2 = new StreamReader(Path, System.Text.Encoding.GetEncoding("gb2312"));
				s = f2.ReadToEnd();
				f2.Close();
				f2.Dispose();
			}
			return s;
		}

		/****************************************
         * 函数名称：FileAdd
         * 功能说明：追加文件内容
         * 参    数：Path:文件路径,strings:内容
         * 调用示列：
         *           string Path = Server.MapPath("Default2.aspx");     
         *           string Strings = "新追加内容";
         *           Utilities.FileOperate.FileAdd(Path, Strings);
        *****************************************/
		/// <summary>
		/// 追加文件
		/// </summary>
		/// <param name="Path">文件路径</param>
		/// <param name="text">内容</param>
		public static void FileAdd(string Path, string text) {
			StreamWriter sw = File.AppendText(Path);
			sw.Write(text);
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}

		/****************************************
         * 函数名称：FileCoppy
         * 功能说明：拷贝文件
         * 参    数：OrignFile:原始文件,NewFile:新文件路径
         * 调用示列：
         *           string OrignFile = Server.MapPath("Default2.aspx");     
         *           string NewFile = Server.MapPath("Default3.aspx");
         *           Utilities.FileOperate.FileCoppy(OrignFile, NewFile);
        *****************************************/
		/// <summary>
		/// 拷贝文件
		/// </summary>
		/// <param name="OrignFile">原始文件</param>
		/// <param name="NewFile">新文件路径</param>
		//public static void FileCoppy(string OrignFile, string NewFile)
		//{
		//	File.Copy(OrignFile, NewFile, true);
		//}

		/****************************************
         * 函数名称：FileDel
         * 功能说明：删除文件
         * 参    数：Path:文件路径
         * 调用示列：
         *           string Path = Server.MapPath("Default3.aspx");    
         *           Utilities.FileOperate.FileDel(Path);
        *****************************************/
		/// <summary>
		/// 删除文件
		/// </summary>
		/// <param name="Path">路径</param>
		//public static void FileDel(string Path)
		//{
		//	File.Delete(Path);
		//}

		/****************************************
         * 函数名称：FileMove
         * 功能说明：移动文件
         * 参    数：OrignFile:原始路径,NewFile:新文件路径
         * 调用示列：
         *            string OrignFile = Server.MapPath("../说明.txt");    
         *            string NewFile = Server.MapPath("../../说明.txt");
         *            Utilities.FileOperate.FileMove(OrignFile, NewFile);
        *****************************************/
		/// <summary>
		/// 移动文件
		/// </summary>
		/// <param name="OrignFile">原始路径</param>
		/// <param name="NewFile">新路径</param>
		//public static void FileMove(string OrignFile, string NewFile) {
		//	File.Move(OrignFile, NewFile);
		//}

		/****************************************
         * 函数名称：FolderCreate
         * 功能说明：在当前目录下创建目录
         * 参    数：OrignFolder:当前目录,NewFloder:新目录
         * 调用示列：
         *           string OrignFolder = Server.MapPath("test/");    
         *           string NewFloder = "new";
         *           Utilities.FileOperate.FolderCreate(OrignFolder, NewFloder); 
        *****************************************/
		/// <summary>
		/// 在当前目录下创建目录
		/// </summary>
		/// <param name="OrignFolder">当前目录</param>
		/// <param name="NewFloder">新目录</param>
		public static void FolderCreate(string OrignFolder, string NewFloder) {
			Directory.SetCurrentDirectory(OrignFolder);
			Directory.CreateDirectory(NewFloder);
		}

		/// <summary>
		/// 创建文件夹
		/// </summary>
		/// <param name="Path"></param>
		public static void FolderCreate(string Path) {
			// 判断目标目录是否存在如果不存在则新建之
			if (!Directory.Exists(Path))
				Directory.CreateDirectory(Path);
		}

		public static FileInfo Create(string Path) {
			var file = new FileInfo(Path);
			if (!file.Exists) {
				FileStream fs = file.Create();
				fs.Close();
			}
			return file;
		}

		/****************************************
         * 函数名称：DeleteFolder
         * 功能说明：递归删除文件夹目录及文件
         * 参    数：dir:文件夹路径
         * 调用示列：
         *           string dir = Server.MapPath("test/");  
         *           Utilities.FileOperate.DeleteFolder(dir);       
        *****************************************/
		/// <summary>
		/// 递归删除文件夹目录及文件
		/// </summary>
		/// <param name="dir"></param>  
		/// <returns></returns>
		public static void DeleteFolder(string dir) {
			if (Directory.Exists(dir)) {
				foreach (string d in Directory.GetFileSystemEntries(dir)) {
					if (File.Exists(d)) {
						File.Delete(d); //直接删除其中的文件                        
					} else {
						DeleteFolder(d);
					}
				}
				Directory.Delete(dir, true); //删除已空文件夹                 
			}
		}

		/****************************************
         * 函数名称：CopyDir
         * 功能说明：将指定文件夹下面的所有内容copy到目标文件夹下面 果目标文件夹为只读属性就会报错。
         * 参    数：srcPath:原始路径,aimPath:目标文件夹
         * 调用示列：
         *           string srcPath = Server.MapPath("test/");  
         *           string aimPath = Server.MapPath("test1/");
         *           Utilities.FileOperate.CopyDir(srcPath,aimPath);   
        *****************************************/
		/// <summary>
		/// 指定文件夹下面的所有内容copy到目标文件夹下面
		/// </summary>
		/// <param name="srcPath">原始路径</param>
		/// <param name="aimPath">目标文件夹</param>
		public static void CopyDir(string srcPath, string aimPath) {
			try {
				// 检查目标目录是否以目录分割字符结束如果不是则添加之
				if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar) { 
					aimPath += Path.DirectorySeparatorChar;
				}
				// 判断目标目录是否存在如果不存在则新建之
				if (!Directory.Exists(aimPath)) {
					Directory.CreateDirectory(aimPath);
				}
				// 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
				//如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
				//string[] fileList = Directory.GetFiles(srcPath);
				string[] fileList = Directory.GetFileSystemEntries(srcPath);
				//遍历所有的文件和目录
				foreach (string file in fileList) {
					//先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
					if (Directory.Exists(file)) { 
						CopyDir(file, aimPath + Path.GetFileName(file));
					} else { 
						File.Copy(file, aimPath + Path.GetFileName(file), true);
					}
				}
			} catch (Exception ee) {
				throw new Exception(ee.ToString());
			}
		}

		/****************************************
         * 函数名称：GetFoldAll(string Path)
         * 功能说明：获取指定文件夹下所有子目录及文件(树形)
         * 参    数：Path:详细路径
         * 调用示列：
         *           string strDirlist = Server.MapPath("templates");       
         *           this.Literal1.Text = Utilities.FileOperate.GetFoldAll(strDirlist);  
        *****************************************/
		/// <summary>
		/// 获取指定文件夹下所有子目录及文件
		/// </summary>
		/// <param name="Path">详细路径</param>
		public static string GetFoldAll(string Path) {
			string str = "";
			DirectoryInfo thisOne = new DirectoryInfo(Path);
			str = ListTreeShow(thisOne, 0, str);
			return str;
		}

		/// <summary>
		/// 获取指定文件夹下所有子目录及文件函数 递归目录 文件
		/// </summary>
		/// <param name="theDir">指定目录</param>
		/// <param name="nLevel">默认起始值,调用时,一般为0</param>
		/// <param name="Rn">用于迭加的传入值,一般为空</param>
		/// <returns></returns>
		public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string Rn) {
			DirectoryInfo[] subDirectories = theDir.GetDirectories();
			foreach (DirectoryInfo dirinfo in subDirectories) {
				if (nLevel == 0) {
					Rn += "├";
				} else {
					string _s = "";
					for (int i = 1; i <= nLevel; i++) {
						_s += "│&nbsp;";
					}
					Rn += _s + "├";
				}
				Rn += "<b>" + dirinfo.Name.ToString() + "</b><br />";
				FileInfo[] fileInfo = dirinfo.GetFiles();   //目录下的文件
				foreach (FileInfo fInfo in fileInfo) {
					if (nLevel == 0) {
						Rn += "│&nbsp;├";
					} else {
						string _f = "";
						for (int i = 1; i <= nLevel; i++) {
							_f += "│&nbsp;";
						}
						Rn += _f + "│&nbsp;├";
					}
					Rn += fInfo.Name.ToString() + " <br />";
				}
				Rn = ListTreeShow(dirinfo, nLevel + 1, Rn);
			}
			return Rn;
		}

		/****************************************
         * 函数名称：GetFoldAll(string Path)
         * 功能说明：获取指定文件夹下所有子目录及文件(下拉框形)
         * 参    数：Path:详细路径
         * 调用示列：
         *            string strDirlist = Server.MapPath("templates");      
         *            this.Literal2.Text = Utilities.FileOperate.GetFoldAll(strDirlist,"tpl","");
        *****************************************/
		/// <summary>
		/// 获取指定文件夹下所有子目录及文件(下拉框形)
		/// </summary>
		/// <param name="Path">详细路径</param>
		///<param name="DropName">下拉列表名称</param>
		///<param name="tplPath">默认选择模板名称</param>
		public static string GetFoldAll(string Path, string DropName, string tplPath) {
			string strDrop = "<select name=\"" + DropName + "\" id=\"" + DropName + "\"><option value=\"\">--请选择详细模板--</option>";
			string str = "";
			DirectoryInfo thisOne = new DirectoryInfo(Path);
			str = ListTreeShow(thisOne, 0, str, tplPath);
			return strDrop + str + "</select>";
		}

		/// <summary>
		/// 获取指定文件夹下所有子目录及文件函数
		/// </summary>
		/// <param name="theDir">指定目录</param>
		/// <param name="nLevel">默认起始值,调用时,一般为0</param>
		/// <param name="Rn">用于迭加的传入值,一般为空</param>
		/// <param name="tplPath">默认选择模板名称</param>
		/// <returns></returns>
		public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string Rn, string tplPath) {
			DirectoryInfo[] subDirectories = theDir.GetDirectories();
			foreach (DirectoryInfo dirinfo in subDirectories) {
				Rn += "<option value=\"" + dirinfo.Name.ToString() + "\"";
				if (tplPath.ToLower() == dirinfo.Name.ToString().ToLower()) {
					Rn += " selected ";
				}
				Rn += ">";

				if (nLevel == 0) {
					Rn += "┣";
				} else {
					string _s = "";
					for (int i = 1; i <= nLevel; i++) {
						_s += "│&nbsp;";
					}
					Rn += _s + "┣";
				}
				Rn += "" + dirinfo.Name.ToString() + "</option>";


				FileInfo[] fileInfo = dirinfo.GetFiles();   //目录下的文件
				foreach (FileInfo fInfo in fileInfo) {
					Rn += "<option value=\"" + dirinfo.Name.ToString() + "/" + fInfo.Name.ToString() + "\"";
					if (tplPath.ToLower() == fInfo.Name.ToString().ToLower()) {
						Rn += " selected ";
					}
					Rn += ">";

					if (nLevel == 0) {
						Rn += "│&nbsp;├";
					} else {
						string _f = "";
						for (int i = 1; i <= nLevel; i++) {
							_f += "│&nbsp;";
						}
						Rn += _f + "│&nbsp;├";
					}
					Rn += fInfo.Name.ToString() + "</option>";
				}
				Rn = ListTreeShow(dirinfo, nLevel + 1, Rn, tplPath);
			}
			return Rn;
		}

		/****************************************
         * 函数名称：GetDirectoryLength(string dirPath)
         * 功能说明：获取文件夹大小
         * 参   数： dirPath:文件夹详细路径
         * 调用示列：
         *          string Path = Server.MapPath("templates"); 
         *          Response.Write(Utilities.FileOperate.GetDirectoryLength(Path));       
        *****************************************/
		/// <summary>
		/// 获取文件夹大小
		/// </summary>
		/// <param name="dirPath">文件夹路径</param>
		/// <returns></returns>
		public static long GetDirectoryLength(string dirPath) {
			if (!Directory.Exists(dirPath)) {
				return 0;
			}
			long len = 0;
			DirectoryInfo di = new DirectoryInfo(dirPath);
			foreach (FileInfo fi in di.GetFiles()) {
				len += fi.Length;
			}
			DirectoryInfo[] dis = di.GetDirectories();
			if (dis.Length > 0) {
				for (int i = 0; i < dis.Length; i++) {
					len += GetDirectoryLength(dis[i].FullName);
				}
			}
			return len;
		}

		/****************************************
         * 函数名称：GetFileAttibe(string filePath)
         * 功能说明：获取指定文件详细属性
         * 参    数：filePath:文件详细路径
         * 调用示列：
         *           string file = Server.MapPath("robots.txt");  
         *            Response.Write(Utilities.FileOperate.GetFileAttibe(file));         
        *****************************************/
		/// <summary>
		/// 获取指定文件详细属性
		/// </summary>
		/// <param name="filePath">文件详细路径</param>
		/// <returns></returns>
		public static string GetAttribute(string filePath) {
			string str = "";
			System.IO.FileInfo objFI = new System.IO.FileInfo(filePath);
			str += "详细路径:" + objFI.FullName + "<br>文件名称:" + objFI.Name + "<br>文件长度:" + objFI.Length.ToString() + "字节<br>创建时间" + objFI.CreationTime.ToString() + "<br>最后访问时间:" + objFI.LastAccessTime.ToString() + "<br>修改时间:" + objFI.LastWriteTime.ToString() + "<br>所在目录:" + objFI.DirectoryName + "<br>扩展名:" + objFI.Extension;
			return str;
		}
	}

	public class BDStorageUtils
	{
		public static string ReadContent(string filePath) {
			// The files used in this example are created in the topic
			// How to: Write to a Text File. You can change the path and
			// file name to substitute text files of your own.
			if (!File.Exists(filePath)) {
				return "";
			}

			// Read the file as one string.
			var ret = System.IO.File.ReadAllText(filePath);
			// Display the file contents to the console. Variable text is a string.
			//System.Console.WriteLine("Contents of WriteText.txt = {0}", text);

			// Example #2
			// Read each line of the file into a string array. Each element
			// of the array is one line of the file.
			//string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\WriteLines2.txt");

			// Display the file contents by using a foreach loop.
			//System.Console.WriteLine("Contents of WriteLines2.txt = ");
			//foreach (string line in lines) {
			//	// Use a tab to indent each line of the file.
			//	Console.WriteLine("\t" + line);
			//}

			//// Keep the console window open in debug mode.
			//Console.WriteLine("Press any key to exit.");
			return ret;
		}

		public static async Task<bool> SaveStorage(HttpContent httpContent, String filePath, long totalSize)
		{
			_ = BDPathUtils.EnsurePathReadyIfNeed(filePath);

			var totalStream = await httpContent.ReadAsStreamAsync();
			var fileInfo = BDDirUtils.Create(filePath);
			var newStream = fileInfo.Create();
			//var buffer = new byte[1024 * 1024];
			//long readCount = 0;
			//long len = 0;
			//while ((len = totalStream.Read(buffer, 0, buffer.Length)) != 0) {
			//	readCount += len;
			//	newStream.Write(buffer, 0, (int)len);
			//}
			await totalStream.CopyToAsync(newStream);
			//totalStream.CopyTo(newStream);
			//System.Diagnostics.Debug.Assert(readCount == totalSize);
			//newStream.Flush();
			//newStream.Close();
			System.Diagnostics.Debug.Assert(newStream.Length == totalSize);
			return (newStream.Length == totalSize);
		}

		public static void Save(String text, String filePath) {
			// (1).使用FileStream类创建文件，然后将数据写入到文件里。
            var fs = new FileStream(filePath, FileMode.Create);
			if (null != fs) {
				byte[] data = System.Text.Encoding.Default.GetBytes(text ?? "");
				if (null != data) {
					fs.Write(data, 0, data.Length);
					fs.Flush();
				}
				fs.Close();
			}
			// (2).使用FileStream类创建文件，使用StreamWriter类，将数据写入到文件。
			//var fs = new FileStream(filePath, FileMode.Create);
			//if (null != fs) {
			//	var sw = new StreamWriter(fs);
			//	if (null != sw) {
			//		sw.Write(text ?? "");
			//		sw.Flush();
			//		sw.Close();
			//	}
			//	fs.Close();
			//}
		}

		public static void Save(List<String> strArray, String filePath) {
			BDPathUtils.EnsurePathReadyIfNeed(filePath);
#pragma warning disable IDE0090 // Use 'new(...)'
            using StreamWriter sw = new StreamWriter(filePath);
#pragma warning restore IDE0090 
            foreach (string str in strArray) {
				sw.WriteLine(str);
			}
			sw.Flush();
			sw.Close();
		}

		public static bool Save(Byte[] bufByte, String filePath) {
			BDPathUtils.EnsurePathReadyIfNeed(filePath);
            //using StreamWriter sw = new StreamWriter(filePath);
            try {
                //using var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                //fs.Write(bufByte, 0, bufByte.Length);
				using (var fs = File.Create(filePath)) {
					fs.Write(bufByte, 0, bufByte.Length);
					fs.Close();
                }
                return true;
            } catch (Exception ex) {
				Console.WriteLine("Exception caught in process: {0}", ex);
				return false;
			}
		}
	}
}
