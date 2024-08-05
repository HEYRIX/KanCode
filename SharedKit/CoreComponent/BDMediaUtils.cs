using System;
using System.Diagnostics;

namespace SharedKit
{
	// Work with ffmpeg
	// https://www.cnblogs.com/Can-daydayup/p/15780172.html
	// https://www.drx-home.com/thread-1-24.html
	// https://gist.github.com/zxhfighter/6b212416245f86d01b85984d5d3b69e1
	// 
	public class BDMediaUtils
	{
		public BDMediaUtils()
		{
		}

		private static string FFCorePath { get; set; }
		public static void Init(string ffPath)
		{
			BDMediaUtils.FFCorePath = ffPath;
			//if (ffPath.Length == 0) {
			//	BDMediaUtils.FFCorePath = hardPath;
			//}
		}

		#region 视频合并
		/// <summary>
		/// 视频合并
		/// </summary>
		/// <param name="video1">合并视频1</param>
		/// <param name="video2">合并视频2</param>
		/// <param name="saveFilePath">保存文件名</param>
		/// <returns></returns>
		public static void VideoCombine(string video1, string video2, string saveFilePath)
		{
			string strTmp1 = video1 + ".ts";
			string strTmp2 = video2 + ".ts";
			string strCmd1 = " -i " + video1 + " -c copy -bsf:v h264_mp4toannexb -f mpegts " + strTmp1 + " -y ";
			string strCmd2 = " -i " + video2 + " -c copy -bsf:v h264_mp4toannexb -f mpegts " + strTmp2 + " -y ";

			string videoMerge = " -i \"concat:" + strTmp1 + "|" +
				strTmp2 + "\" -c copy -bsf:a aac_adtstoasc -movflags +faststart " + saveFilePath + " -y ";

			//1、转换文件类型，由于不是所有类型的视频文件都支持直接合并，需要先转换格式
			CommandManager(strCmd1);
			CommandManager(strCmd2);
			//2、视频合并
			CommandManager(videoMerge);
		}
		#endregion

		#region 音频合并
		/// <summary>
		/// 音频合并
		/// </summary>
		public static void AudioMerge(string physicalPath, List<string> mergeFile)
		{
			//将多个音频混合成一个音频文件输出 http://www.ffmpeg.org/ffmpeg-all.html#amix

			//ffmpeg -i INPUT1 -i INPUT2 -i INPUT3 -filter_complex amix=inputs=3:duration=first:dropout_transition=3 OUTPUT

			//合并两个音频
			//ffmpeg -i input1.mp3 -i input2.mp3 -filter_complex amerge -ac 2 - c:a libmp3lame -q:a 4 output.mp3

			//获取视频中的音频
			//ffmpeg -i input.mp4 -vn -y -acodec copy output.m4a

			//去掉视频中的音频
			//ffmpeg -i input.mp4 -an output.mp4

			// https://www.cnblogs.com/simadi/p/10649345.html
			// ffmpeg -i "concat:123.mp3|124.mp3" -acodec copy output.mp3
			// 说明：-i代表输入参数
			// contact: 123.mp3 | 124.mp3代表着需要连接到一起的音频文件 -acodec copy output.mp3 重新编码并复制到新文件中

			string strCommand = $"-i \"concat:{string.Join("|", mergeFile.ToArray())}\" -acodec copy {physicalPath}AudioMerge.mp3  -y";
			CommandManager(strCommand);
		}

		public static void GetAudioContent(string physicalPath, string dirOutPath)
		{
			//http://www.ffmpeg.org/ffmpeg-all.html#amix
			//获取视频中的音频
			//ffmpeg -i input.mp4 -vn -y -acodec copy output.m4a
			var fileHeadOut = SharedKit.CoreComponent.BDPathUtils.GetFileName(physicalPath)+ ".m4a";
			var filePathOut = Path.Combine(dirOutPath, fileHeadOut);
			string strCommand = $"-i {physicalPath} -vn -y -acodec copy {filePathOut}";
			CommandManager(strCommand);
		}
		#endregion

		#region 音频与视频合并成视频
		/// <summary>
		/// 音频与视频合并成视频
		/// </summary>
		/// <param name="physicalPath">物理路径</param>
		public static void AudioAndVideoMerge(string physicalPath)
		{
			//1、视频文件中没有音频。
			//ffmpeg -i video.mp4 -i audio.wav -c:v copy -c:a aac -strict experimental output.mp4
			//string mergeCommandStr = $"-i {physicalPath}video2.mp4 -i {physicalPath}music1.mp3 -c:v copy -c:a aac -strict experimental {physicalPath}output.mp4  -y";

			//video.mp4,audio.wav分别是要合并的视频和音频，output.mp4是合并后输出的音视频文件。
			//2、下面的命令是用audio音频替换video中的音频 ffmpeg -i video.mp4 -i audio.wav -c:v copy -c:a aac -strict experimental -map 0:v:0 -map 1:a: 0 output.mp4
			string strCommand = $"-i {physicalPath}video3.mp4 -i {physicalPath}AudioMerge.mp3 -c:v copy -c:a aac -strict experimental -map 0:v:0 -map 1:a:0 {physicalPath}AudioAndVideoMerge.mp4  -y";

			//3、c++音频视频合并(视频文件中没有音频的情况下)
			//"ffmpeg -i /tmp/mergeMp3/392118469203595327/392118469203595327.aac  -i /tmp/mergeMp3/392118469203595327/bg.mp4 -c copy -bsf:a aac_adtstoasc /tmp/mergeMp3/392118469203595327/392118469203595327.mp4 -y"
			//string mergeCommandStr3 = $"-i {physicalPath}video5.mp4  -i {physicalPath}AudioMerge.mp3 -c copy -bsf:a aac_adtstoasc {physicalPath}AudioAndVideoMerge1.mp4 -y";

			CommandManager(strCommand);
		}
		#endregion

		public static void GrabMediaShot(string physicalPath, string dirOutPath)
		{
			// "./ffmpeg -i ./index.mp4 -vf "scale=400:-1,fps=0.1" thumb/%05d.jpg"
			// "./ffmpeg -i ./index.mp4 -vf "fps=0.1" thumb/%05d.jpg"
			var fileHeadOut = SharedKit.CoreComponent.BDPathUtils.GetFileName(physicalPath);
			string strCommand = $"-i {physicalPath} -vf \"fps=0.1\" {dirOutPath}/{fileHeadOut}.%05d.jpg";

			//3、c++音频视频合并(视频文件中没有音频的情况下)
			//"ffmpeg -i /tmp/mergeMp3/392118469203595327/392118469203595327.aac  -i /tmp/mergeMp3/392118469203595327/bg.mp4 -c copy -bsf:a aac_adtstoasc /tmp/mergeMp3/392118469203595327/392118469203595327.mp4 -y"
			//string mergeCommandStr3 = $"-i {physicalPath}video5.mp4  -i {physicalPath}AudioMerge.mp3 -c copy -bsf:a aac_adtstoasc {physicalPath}AudioAndVideoMerge1.mp4 -y";

			CommandManager(strCommand);
		}

		/// <summary>
		/// 执行
		/// C# Process进程调用 https://docs.microsoft.com/zh-cn/dotnet/api/system.diagnostics.process?view=net-5.0
		/// </summary>
		/// <param name="commandStr">执行命令</param>
		private static void CommandManager(string commandStr)
		{
			// "D:\\FFmpeg\\bin\\ffmpeg.exe"
			var ffPath = BDMediaUtils.FFCorePath;
			try {
				using (Process process = new Process()) {
					process.StartInfo.FileName = ffPath;//要执行的程序名称(属性，获取或设置要启动的应用程序或文档。FileName 属性不需要表示可执行文件。 它可以是其扩展名已经与系统上安装的应用程序关联的任何文件类型。)
					process.StartInfo.Arguments = " " + commandStr;//启动该进程时传递的命令行参数
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardInput = false;//可能接受来自调用程序的输入信息  
					process.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息   
					process.StartInfo.RedirectStandardError = false;//重定向标准错误输出
					process.StartInfo.CreateNoWindow = false;//不显示程序窗口
					process.Start();//启动程序
					process.WaitForExit();//等待程序执行完退出进程(避免进程占用文件或者是合成文件还未生成)*
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}
	}
}

