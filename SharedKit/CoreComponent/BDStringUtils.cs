using System.Text.RegularExpressions;

namespace SharedKit
{
    // https://tool.oschina.net/uploads/apidocs/jquery/regexp.html
    public enum BDRegexKind
	{
		None, // 无
		Digital, // 提取纯数字
	}
	public class BDStringUtils
	{
		//TODO
		//Format方法将多个对象格式化成一个字符串Format方法解析格式字符串的原理 https://www.cnblogs.com/GreenLeaves/p/9171455.html
		//C# 正则表达式大全（代码篇） https://www.cnblogs.com/zhaoshujie/p/9718301.html
		// 提取公司名称Regex [\x{4e00}-\x{9fa5}\(\)（）\da-zA-Z&]{2,50}
		// ([\u4e00-\u9fa5\(\)（）\da-zA-Z&]{2,50})(\u516c\u53f8) // 公(\u516c)司(\u53f8)
		// ([\u4e00-\u9fa5\(\)（）a-zA-Z&]{2,50})(\u516c\u53f8)   // 仅支持中文和英文且以公司结尾的正则表达式
		// https://www.cnblogs.com/init-007/p/11757232.html

		//		private static BDStringUtils _instance = null;
		//#pragma warning disable IDE0090 // Use 'new(...)'
		//		private static readonly object _locker = new object();
		//#pragma warning restore IDE0090 // Use 'new(...)'
		//		private BDStringUtils()
		//		{
		//		}

		//public static BDStringUtils GetInstance()
		//{
		//	if (_instance == null) {
		//		lock (_locker) {
		//			if (_instance == null) {
		//				_instance = new BDStringUtils();
		//			}
		//		}
		//	}
		//	return _instance;
		//}

		/// <summary>
		/// 判断输入的字符串只包含汉字
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsChineseChar(string input)
		{
			return IsMatch(input, @"^[\u4e00-\u9fa5]+$");
		}

		/// <summary>
		/// 匹配3位或4位区号的电话号码，其中区号可以用小括号括起来，
		/// 也可以不用，区号与本地号间可以用连字号或空格间隔，
		/// 也可以没有间隔
		/// \(0\d{2}\)[- ]?\d{8}|0\d{2}[- ]?\d{8}|\(0\d{3}\)[- ]?\d{7}|0\d{3}[- ]?\d{7}
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsPhone(string input)
		{
			string pattern = "^\\(0\\d{2}\\)[- ]?\\d{8}$|^0\\d{2}[- ]?\\d{8}$|^\\(0\\d{3}\\)[- ]?\\d{7}$|^0\\d{3}[- ]?\\d{7}$";
			return IsMatch(input, pattern);
		}

		/// <summary>
		/// 判断输入的字符串是否是一个合法的手机号
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsMobilePhone(string input)
		{
			return IsMatch(input, @"^13\\d{9}$");
		}

		/// <summary>
		/// 判断输入的字符串只包含数字
		/// 可以匹配整数和浮点数
		/// ^-?\d+$|^(-?\d+)(\.\d+)?$
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsNumber(string input)
		{
			string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
			return IsMatch(input, pattern);
		}

		/// <summary>
		/// 匹配非负整数
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsNotNagtive(string input)
		{
			return IsMatch(input, @"^\d+$");
		}

		/// <summary>
		/// 匹配正整数
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsUint(string input)
		{
			return IsMatch(input, @"^[0-9]*[1-9][0-9]*$");
		}

		/// <summary>
		/// 判断输入的字符串字包含英文字母
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool HasEnglishChar(string input)
		{
			return IsMatch(input, @"^[A-Za-z]+$");
		}

		/// <summary>
		/// 判断输入的字符串是否是一个合法的Email地址
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsEmail(string input)
		{
			string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
			return IsMatch(input, pattern);
		}

		/// <summary>
		/// 判断输入的字符串是否只包含数字和英文字母
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsNumAndEnglishChar(string input)
		{
			return IsMatch(input, @"^[A-Za-z0-9]+$");
		}

		/// <summary>
		/// 判断输入的字符串是否是一个超链接
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsURL(string input)
		{
			string pattern = @"^[a-zA-Z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$";
			return IsMatch(input, pattern);
		}

		/// <summary>
		/// 从html中通过正则找到ip信息(只支持ipv4地址)
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string GetIP(String input)
		{
			//验证ipv4地址
			var reg = @"(?:(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))\.){3}(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))";
			var ip = "";
			var m = System.Text.RegularExpressions.Regex.Match(input, reg);
			if (m.Success) {
				ip = m.Value;
			}
			return ip;
		}

		/// <summary>
		/// 判断输入的字符串是否是表示一个IP地址
		/// </summary>
		/// <param name="input">被比较的字符串</param>
		/// <returns>是IP地址则为True</returns>
		public static bool IsIPv4(string input)
		{
			string[] IPs = input.Split('.');
			for (int i = 0; i < IPs.Length; i++) {
				if (!IsMatch(IPs[i], @"^\d+$")) {
					return false;
				}
				if (Convert.ToUInt16(IPs[i]) > 255) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 判断输入的字符串是否是合法的IPV6 地址
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsIPV6(string input)
		{
			string temp = input;
			string[] strs = temp.Split(':');
			if (strs.Length > 8) {
				return false;
			}
			int count = GetStringCount(input, "::");
			string pattern;
			if (count > 1) {
				return false;
			} else if (count == 0) {
				pattern = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";
				return IsMatch(input, pattern);
			} else {
				pattern = @"^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
				return IsMatch(input, pattern);
			}
		}

		/// <summary>
		/// 计算字符串的字符长度，一个汉字字符将被计算为两个字符
		/// </summary>
		/// <param name="input">需要计算的字符串</param>
		/// <returns>返回字符串的长度</returns>
		public static int GetCount(string input)
		{
			return Regex.Replace(input, @"[\u4e00-\u9fa5/g]", "aa").Length;
		}

		/// <summary>
		/// 调用Regex中IsMatch函数实现一般的正则表达式匹配
		/// </summary>
		/// <param name="input">要搜索匹配项的字符串</param>
		/// <param name="pattern">要匹配的正则表达式模式。</param>
		/// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false。</returns>
		public static bool IsMatch(string input, string pattern)
		{
			if (null == pattern || pattern.Length == 0) {
				throw new ArgumentException("不允许为空或null的正则表达式");
			}
			if (input == null) {
				throw new ArgumentNullException(nameof(input));
			}
			var regex = new Regex(pattern);
			return regex.IsMatch(input);
		}

		public static string GetPattern(BDRegexKind type) {
			var ret = "";
			switch (type) {
				case BDRegexKind.None:
					break;
				case BDRegexKind.Digital:
					ret = @"[^0-9]+";
					break;
				default:
					break;
			}
			return ret;
		}

		/// <summary>
		/// 从输入字符串中的第一个字符开始，用替换字符串替换指定的正则表达式模式的所有匹配项。
		/// </summary>
		/// <param name="pattern">模式字符串</param>
		/// <param name="input">输入字符串</param>
		/// <param name="replacement">用于替换的字符串</param>
		/// <returns>返回被替换后的结果</returns>
		public static string Replace(string input, string pattern, string replacement = "") {
			var regex = new Regex(pattern);
			//var ret = System.Text.RegularExpressions.Regex.Replace(input, pattern, replacement);
			return regex.Replace(input, replacement);
		}

		/// <summary>
		/// 在由正则表达式模式定义的位置拆分输入字符串。
		/// </summary>
		/// <param name="pattern">模式字符串</param>
		/// <param name="input">输入字符串</param>
		/// <returns></returns>
		public static string[] Split(string pattern, string input)
		{
			var regex = new Regex(pattern);
			return regex.Split(input);
		}

		public static bool IsWholeNumber(string strNumber)
		{
			var g = new Regex(@"^[0-9]\d*$");
			return g.IsMatch(strNumber);
		}

		public static string GetValue(string str, string s, string e)
		{ // TODO NOT Check https://developer.aliyun.com/article/346041
			Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
			return rg.Match(str).Value;
		}

		/* *******************************************************************
         * 1、通过“:”来分割字符串看得到的字符串数组长度是否小于等于8
         * 2、判断输入的IPV6字符串中是否有“::”。
         * 3、如果没有“::”采用 ^([\da-f]{1,4}:){7}[\da-f]{1,4}$ 来判断
         * 4、如果有“::” ，判断"::"是否止出现一次
         * 5、如果出现一次以上 返回false
         * 6、^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$
         * ******************************************************************/
		/// <summary>
		/// 判断字符串compare 在 input字符串中出现的次数
		/// </summary>
		/// <param name="input">源字符串</param>
		/// <param name="compare">用于比较的字符串</param>
		/// <returns>字符串compare 在 input字符串中出现的次数</returns>
		public static int GetStringCount(string input, string compare)
		{
			int index = input.IndexOf(compare);
			if (index != -1) {
				return 1 + GetStringCount(input.Substring(index + compare.Length), compare);
			} else {
				return 0;
			}
		}

		public static string ToMD5(string input)
		{
			// https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
			// Use input string to calculate MD5 hash
			using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = md5.ComputeHash(inputBytes);

			return Convert.ToHexString(hashBytes); // .NET 5 +

			// Convert the byte array to hexadecimal string prior to .NET 5
			// StringBuilder sb = new System.Text.StringBuilder();
			// for (int i = 0; i < hashBytes.Length; i++)
			// {
			//     sb.Append(hashBytes[i].ToString("X2"));
			// }
			// return sb.ToString();
		}

		public static string ToString(int digit, string fmt)
		{
			// https://stackoverflow.com/questions/4325267/c-sharp-convert-int-to-string-with-padding-zeros
			// i.ToString().PadLeft(4, '0') - okay, but doesn't work for negative numbers
			// i.ToString("0000"); - explicit form
			// i.ToString("D4"); - short form format specifier
			// $"{i:0000}"; - string interpolation(C# 6.0+)
			return digit.ToString(fmt);
		}
	}

	public static class StringExtensions
	{
		public static int SubCount(this string input, string substr) {
			return Regex.Matches(input, substr).Count;
		}

		public static Array SubArray(this string input, string substr) {
			var arra = Regex.Matches(input, substr).ToArray();
			return Regex.Matches(input, substr).ToArray();
		}
	}

	/// <summary>
	/// 字符串操作 - 字符串生成器
	/// </summary>
	//public partial class String
	//{
	//	/// <summary>
	//	/// 初始化字符串操作
	//	/// </summary>
	//	public String()
	//	{
	//		Builder = new StringBuilder();
	//	}

	//	/// <summary>
	//	/// 字符串生成器
	//	/// </summary>
	//	private StringBuilder Builder { get; set; }

	//	/// <summary>
	//	/// 追加内容
	//	/// </summary>
	//	/// <typeparam name="T">值的类型</typeparam>
	//	/// <param name="value">值</param>
	//	public String Append<T>(T value)
	//	{
	//		Builder.Append(value);
	//		return this;
	//	}

	//	/// <summary>
	//	/// 追加内容
	//	/// </summary>
	//	/// <param name="value">值</param>
	//	/// <param name="args">参数</param>
	//	public String Append(string value, params object[] args)
	//	{
	//		if (args == null)
	//			args = new object[] { string.Empty };
	//		if (args.Length == 0)
	//			Builder.Append(value);
	//		else
	//			Builder.AppendFormat(value, args);
	//		return this;
	//	}

	//	/// <summary>
	//	/// 追加内容并换行
	//	/// </summary>
	//	public String AppendLine()
	//	{
	//		Builder.AppendLine();
	//		return this;
	//	}

	//	/// <summary>
	//	/// 追加内容并换行
	//	/// </summary>
	//	/// <typeparam name="T">值的类型</typeparam>
	//	/// <param name="value">值</param>
	//	public String AppendLine<T>(T value)
	//	{
	//		Append(value);
	//		Builder.AppendLine();
	//		return this;
	//	}

	//	/// <summary>
	//	/// 追加内容并换行
	//	/// </summary>
	//	/// <param name="value">值</param>
	//	/// <param name="args">参数</param>
	//	public String AppendLine(string value, params object[] args)
	//	{
	//		Append(value, args);
	//		Builder.AppendLine();
	//		return this;
	//	}

	//	/// <summary>
	//	/// 替换内容
	//	/// </summary>
	//	/// <param name="value">值</param>
	//	public String Replace(string value)
	//	{
	//		Builder.Clear();
	//		Builder.Append(value);
	//		return this;
	//	}

	//	/// <summary>
	//	/// 移除末尾字符串
	//	/// </summary>
	//	/// <param name="end">末尾字符串</param>
	//	public String RemoveEnd(string end)
	//	{
	//		string result = Builder.ToString();
	//		if (!result.EndsWith(end))
	//			return this;
	//		Builder = new StringBuilder(result.TrimEnd(end.ToCharArray()));
	//		return this;
	//	}

	//	/// <summary>
	//	/// 清空字符串
	//	/// </summary>
	//	public String Clear()
	//	{
	//		Builder = Builder.Clear();
	//		return this;
	//	}

	//	/// <summary>
	//	/// 字符串长度
	//	/// </summary>
	//	public int Length => Builder.Length;

	//	/// <summary>
	//	/// 空字符串
	//	/// </summary>
	//	public static string Empty => string.Empty;

	//	/// <summary>
	//	/// 转换为字符串
	//	/// </summary>
	//	public string ToShow()
	//	{
	//		return Builder.ToString();
	//	}
	//}
}
