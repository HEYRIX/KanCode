using System;
using System.Linq.Expressions;
using System.Web;
using SharedKit.CoreComponent;

namespace SharedKit
{
	public class BDSharedCoreContext
	{
		public BDSharedCoreContext()
		{
		}
	}

	/// <summary>
	/// 基础数据拓展类
	/// </summary>
	public static class TypeConverter
	{
		/// <summary>
		/// 字符串转换为日期时
		/// </summary>
		/// <param name="str">表示日期的字符串<see cref="String"/></param>
		/// <returns>转换后的日期，转换失败则返回日期1970/1/1<see cref="DateTime"/></returns>
		public static DateTime ToDateTime(this string str)
		{
			try {
				return Convert.ToDateTime(str);
			} catch (Exception ex) {
				throw new StringIsNotTartgetFormatException($"类型{typeof(string).Name}的值{str}不是符合类型{typeof(DateTime).Name}要求的格式", ex.GetInnermostException());
			}

		}

		/// <summary>
		/// 字符串是否全为空格或null
		/// </summary>
		/// <param name="str">用于判断的字符串<see cref="String"/></param>
		/// <returns>判断结果<see cref="Boolean"/></returns>
		public static bool IsNullOrWhite(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}

		/// <summary>
		/// 字符串是否不全为空格或null<seealso cref="IsNullOrWhite(string)"/>
		/// </summary>
		/// <param name="str">用于判断的字符串<see cref="String"/></param>
		/// <returns>判断结果<see cref="Boolean"/></returns>
		//[Obsolete("一个冗余的方法，可用IsNullOrWhite替代")]
		//public static bool IsNotNullOrWhite(this string str)
		//{
		//	return !string.IsNullOrWhiteSpace(str);
		//}

		/// <summary>
		/// 一组字符串是否全部为（null或空格）
		/// </summary>
		/// <param name="str">用于判断的字符串数组<see cref="String[]"/></param>
		/// <returns>判断结果<see cref="Boolean"/></returns>
		public static bool StringsIsNullOrWhite(params string[] str)
		{
			if (str == null) {
				return true;
			}
			return !str.Any(s => s != null && !string.IsNullOrWhiteSpace(s));
		}

		/// <summary>
		/// 对象是否全部为null
		/// </summary>
		/// <param name="obj">对象<see cref="Object"/></param>
		/// <returns>判断结果<see cref="Boolean"/></returns>
		public static bool ObjectsIsNull(params object[] obj)
		{
			if (obj == null || obj.Length == 0) {
				return true;
			}
			return !obj.Any(it => it != null);
		}

		/// <summary>
		/// 判断对象是否为null
		/// </summary>
		/// <param name="obj">对象<see cref="Object"/></param>
		/// <returns>判断结果<see cref="Boolean"/></returns>
		public static bool IsNull(this object obj)
		{
			return obj == null;
		}
		/// <summary>
		/// 判断对象是否不为null<seealso cref="IsNull(Object)"/>
		/// </summary>
		/// <param name="obj">对象<see cref="Object"/></param>
		/// <returns><see cref="Boolean"/><seealso cref="bool"/></returns>
		[Obsolete("一个冗余的扩展方法，IsNull方法的一个取反而已")]
		public static bool IsNotNull(this object obj)
		{
			return obj != null;
		}

		/// <summary>
		/// 字符串为空则返回指定类型的结果
		/// </summary>
		/// <param name="str">字符串<see cref="string"/></param>
		/// <param name="ifnull_result"><typeparamref name="T"/>如果字符串为空则返回的结果</param>
		/// <param name="ifnotnull_result"><typeparamref name="T"/>如果字符串不为空则返回的结果</param>
		/// <returns><typeparamref name="T"/>传入的具体类型</returns>
		public static T IsNullThen<T>(this string str, T ifnull_result, T ifnotnull_result)
		{
			return str.IsNullOrWhite() ? ifnull_result : ifnotnull_result;
		}
		/// <summary>
		/// 字符串为空则返回其他字符串
		/// </summary>
		/// <param name="current_string">字符串<see cref="string"/></param>
		/// <param name="ifnull_string"><see cref="string"/>如果为空则返回的字符串</param>
		/// <param name="ifnotnull_string"><see cref="string"/>如果不为空则返回的字符串（默认返回当前字符串）</param>
		/// <returns><see cref="string"/>结果字符串</returns>
		public static string IfNullThen(this string current_string, string ifnull_string, string ifnotnull_string = null)
		{
			return current_string.IsNullOrWhite() ? ifnull_string : ifnotnull_string ?? current_string;
		}

		/// <summary>
		/// 字符串不为空则返回的字符串<seealso cref="IfNullThen(string, string, string)"/>
		/// </summary>
		/// <param name="current_string"><see cref="string"/>当前字符串</param>
		/// <param name="ifnotnull_string"><see cref="string"/>如果不为空则返回的字符串</param>
		/// <param name="ifnull_string"><see cref="string"/>如果为空则返回的字符串</param>
		/// <returns><see cref="string"/>结果字符串</returns>
		[Obsolete("一个冗余方法，也可以使用IfNullThen")]
		public static string IfNotNullThen(this string current_string, string ifnotnull_string, string ifnull_string = null)
		{
			return !current_string.IsNullOrWhite() ? ifnotnull_string : ifnull_string ?? current_string;
		}

		/// <summary>
		/// 判断字符串是否为空(或null)并和其他字符串比较
		/// </summary>
		/// <param name="current_string"><see cref="string"/>当前字符串</param>
		/// <param name="other_string"><see cref="string"/>如果当前字符串为null(或空)，和哪个字符串比较</param>
		/// <param name="ifnotnull_expression"><see cref="string"/>如果字符串不为空，返回的结果<see cref="Boolean"/></param>
		/// <returns><see cref="Boolean"/>比较的结果</returns>
		public static bool IfNullThenEqualWith(this string current_string, string other_string, bool ifnotnull_expression = true)
		{
			return current_string.IsNullOrWhite() ? current_string == other_string : ifnotnull_expression;
		}

		/// <summary>
		/// 判断字符串是否不为空(或null)并和其他字符串比较
		/// </summary>
		/// <param name="current_string"><see cref="string"/>当前字符串</param>
		/// <param name="other_string"><see cref="string"/>如果字符串不为空（或null）要比较的字符串</param>
		/// <param name="ifnull_expression"><see cref="Boolean"/>如果字符串为空，则返回布尔结果</param>
		/// <returns><see cref="Boolean"/>比较的结果</returns>
		public static bool IfNotNullThenEqualWith(this string current_string, string other_string, bool ifnull_expression = true)
		{
			return !current_string.IsNullOrWhite() ? current_string == other_string : ifnull_expression;
		}

		/// <summary>
		/// 判断字符串是否为空（或null）并返回其他判断
		/// </summary>
		/// <param name="current_string"><see cref="string"/>当前字符串</param>
		/// <param name="ifnull_expression"><see cref="Func{Boolean}"/>当字符串为空进行时的其他判断</param>
		/// <param name="ifnotnull_expression"><see cref="Func{Boolean}"/>当字符串不为空进行时的其他判断</param>
		/// <returns><see cref="Boolean"/>判断结果</returns>
		public static bool IfNullThen(this string current_string, Func<bool> ifnull_expression, Func<bool> ifnotnull_expression)
		{
			return current_string.IsNullOrWhite() ? ifnull_expression.Invoke() : ifnotnull_expression.Invoke();
		}

		/// <summary>
		/// 判断字符串是否不为空（或null）并返回其他判断
		/// </summary>
		/// <param name="current_string"><see cref="string"/>字符串</param>
		/// <param name="ifnotnull_expression"><see cref="Func{Boolean}"/>当字符串不为空进行时的其他判断</param>
		/// <param name="ifnull_expression"><see cref="Func{Boolean}"/>当字符串为空进行时的其他判断</param>
		/// <returns></returns>
		public static bool IfNotNullThen(this string current_string, Func<bool> ifnotnull_expression, Func<bool> ifnull_expression)
		{
			return !current_string.IsNullOrWhite() ? ifnotnull_expression.Invoke() : ifnull_expression.Invoke();
		}

		/// <summary>
		/// 判断是否是日期
		/// </summary>
		/// <param name="str"><see cref="string"/>日期字符串</param>
		/// <returns><see cref="Boolean"/>判断结果</returns>
		public static bool IsDateTime(this string str)
		{
			return DateTime.TryParse(str, out DateTime dt);
		}

		/// <summary>
		/// 判断字符串是否可以转10进制整数
		/// </summary>
		/// <param name="str"><see cref="string"/>字符串</param>
		/// <returns><see cref="Boolean"/>判断结果</returns>
		public static bool IsInt32(this string str)
		{
			return Int32.TryParse(str, out int i);
		}

		/// <summary>
		/// <see cref="IsNullOrEmpty(IEnumerable{string})"/>
		/// </summary>
		/// <param name="current_strings">字符串集合</param>
		/// <returns>判断结果</returns>
		[Obsolete("请使用新方法IsNullOrEmpty")]
		public static bool IsNull(this IEnumerable<string> current_strings)
		{
			return current_strings == null || current_strings.Count() == 0;
		}

		/// <summary>
		/// 判断数据集对象是否为null或集合个数为0个
		/// </summary>
		/// <param name="current_strings">字符串集合</param>
		/// <returns><see cref="Boolean"/>判断结果</returns>
		public static bool IsNullOrEmpty(this IEnumerable<string> current_strings)
		{
			return current_strings == null || !current_strings.Any();
		}
		/// <summary>
		/// 判断泛型集是否为null或集合个数为0个
		/// </summary>
		/// <typeparam name="T">具体的对象类型</typeparam>
		/// <param name="current_objects">泛型集</param>
		/// <returns><see cref="Boolean"/>判断结果</returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> current_objects)
		{
			return current_objects == null || !current_objects.Any();
		}

		/// <summary>
		/// 去前后字符/空格升级版，null不会报错,而是返回null
		/// </summary>
		/// <param name="str"><see cref="string"/>当前字符串</param>
		/// <param name="char_of_trim"><see cref="Char"/>去除的字符（不传则去前后空格）</param>
		/// <returns><see cref="string"/>去字符/空格后的结果</returns>
		public static string TrimUpgraded(this string str, char char_of_trim = ' ')
		{
			return str.IsNullOrWhite() ? str : (char_of_trim == ' ' ? str.Trim() : str.Trim(char_of_trim));
		}

		/// <summary>
		/// 字符串转小写升级版，null不会报错，而是返回null
		/// </summary>
		/// <param name="str"><see cref="string"/>当前字符串</param>
		/// <returns><see cref="string"/>全部为小写的字符串</returns>
		public static string ToLowerUpgraded(this string str)
		{
			return str.IsNullOrWhite() ? str : str.ToLower();
		}

		/// <summary>
		/// 字符串转大写升级版，null不会报错，而是返回null
		/// </summary>
		/// <param name="str"><see cref="string"/>当前字符串</param>
		/// <returns><see cref="string"/>全部为大写的字符串</returns>
		public static string ToUpperUpgraded(this string str)
		{
			return str.IsNullOrWhite() ? str : str.ToUpper();
		}

		/// <summary>
		/// 获取一个字符串变量的变量名
		/// </summary>
		/// <param name="Expression"><see cref="Expression{Func{string}}"/>成员表达式</param>
		/// <returns><see cref="string"/></returns>
		public static string VariableName(this Expression<Func<string>> @Expression)
		{
			if (@Expression == null) {
				throw new ArgumentNullException("Expression");
			} else {
				if (@Expression.Body is not MemberExpression memberExpression) {
					throw new InvalidCastException("无法将传入的表达式转换成MemberExpression");
				}
				return memberExpression.Member.Name;
			}
			throw new ExpressionIsNotTureException(typeof(MemberExpression));
		}

		/// <summary>
		/// 解码字符串
		/// </summary>
		/// <param name="str"><see cref="string"/>Url编码过的字符串</param>
		/// <returns><see cref="string"/>解码后的字符串</returns>
		public static string UrlDecode(this string str)
		{
			return HttpUtility.UrlDecode(str);
		}
		/// <summary>
		/// 编码字符串
		/// </summary>
		/// <param name="str"><see cref="string"/>需要Url编码的字符串</param>
		/// <returns><see cref="string"/>编码后的字符串</returns>
		public static string UrlEncode(this string str)
		{
			return HttpUtility.UrlEncode(str);
		}

		/// <summary>
		/// 给url添加参数
		/// </summary>
		/// <param name="url"><see cref="string"/>url</param>
		/// <param name="Expression"><see cref="Expression{Func{string}}"/></param>
		/// <returns><see cref="string"/>添加参数后的Url</returns>
		public static string AddParam(this string url, Expression<Func<string>> @Expression)
		{
			try {
				if (url.IndexOf("?") == -1) {
					url = $"{url}?";
				}
				string param_name = VariableName(@Expression);
				string? compile_value = @Expression.Compile()?.Invoke();
#pragma warning disable CS8604 // Possible null reference argument.
				string param_value = compile_value.IsNullOrWhite() ? "" : compile_value.UrlEncode();
#pragma warning restore CS8604 // Possible null reference argument.
				string param_string = $"&{param_name}={param_value}";
				url = $"{url}{param_string}";
				return url;
			} catch (Exception ex) {
				throw ex.GetInnermostException();
			}
		}

		/// <summary>
		/// 将一个泛型对象转换成decimal类型
		/// </summary>
		/// <param name="t"><typeparamref name="T"/>这里传入具体的类型</param>
		/// <returns><see cref="decimal"/>转换后的decim对象</returns>
		public static decimal ToDecimal<T>(this T t)
		{
			try {
				return Convert.ToDecimal(t);
			} catch (Exception ex) {
				throw new InvalidCastException($"类型{typeof(T).Name}的值{t.ConvertToString()}无法转换成{typeof(decimal).Name}", ex.GetInnermostException());
			}
		}


		/// <summary>
		/// 将一个对象转成String
		/// </summary>
		/// <param name="t"><typeparamref name="T"/>具体的类型</param>
		/// <returns><see cref="string"/>转换后的string对象</returns>
		public static string ConvertToString<T>(this T t)
		{
			try {
#pragma warning disable CS8603 // Possible null reference return.
				return Convert.ToString(t);
#pragma warning restore CS8603 // Possible null reference return.
			} catch (Exception ex) {
				throw new InvalidCastException($"类型{typeof(T).Name}的值{t.ConvertToString()}无法转换成{typeof(string).Name}", ex.GetInnermostException());
			}
		}

		/// <summary>
		/// 转换为的10进制数
		/// </summary>
		/// <param name="t"><typeparamref name="T"/>要转换的对象</param>
		/// <returns><see cref="int"/>10进制数</returns>
		public static int ToInt32<T>(this T t)
		{
			try {
				return Convert.ToInt32(t);
			} catch (Exception ex) {
				throw new InvalidCastException($"类型{typeof(T).Name}的值{t.ConvertToString()}无法转换成{typeof(int).Name}", ex.GetInnermostException());
			}
		}

		/// <summary>
		/// 转成Double类型
		/// </summary>
		/// <param name="t"><typeparamref name="T"/>需要转换的对象</param>
		/// <returns><see cref="double"/>转换后的结果</returns>
		public static double ToDouble<T>(this T t)
		{
			try {
				return Convert.ToDouble(t);
			} catch (Exception ex) {
				throw new InvalidCastException($"类型{typeof(T).Name}的值{t.ConvertToString()}无法转换成{typeof(double).Name}", ex.GetInnermostException());
			}
		}

		/// <summary>
		/// 获取最内部的异常
		/// </summary>
		/// <param name="ex"><see cref="Exception"/>当前异常</param>
		/// <returns><see cref="Exception"/>最内部异常</returns>
		public static Exception GetInnermostException(this Exception ex)
		{
			if (ex.InnerException == null) return ex;
			return ex.InnerException.GetInnermostException();
		}


		/// <summary>
		/// 转换成Json字符串
		/// </summary>
		/// <typeparam name="T">泛型</typeparam>
		/// <param name="obj">原对象</param>
		/// <param name="ignore_null_exceptions"><see cref="bool"/>是否忽略异常</param>
		/// <returns><see cref="string"/>json字符串</returns>
		//public static string ToJsonString<T>(this T obj, bool ignore_null_exceptions = false)
		//{
		//	try {
		//		if (obj == null) {
		//			if (ignore_null_exceptions) {
		//				return null;
		//			}
		//			throw new ArgumentNullException($"类型为{typeof(T).Name}的参数值为null");
		//		}
		//		return JsonConvert.SerializeObject(obj);
		//	} catch (Exception ex) {
		//		throw new InvalidCastException($"无法对类型{typeof(T).Name}进行Json序列化", ex.GetInnermostException());
		//	}
		//}

		/// <summary>
		/// 转换成Json字符串
		/// </summary>
		/// <typeparam name="T">将要转换的类型</typeparam>
		/// <param name="json_string">Json字符串</param>
		/// <returns><see cref="string"/>json字符串</returns>
		//public static T ToObject<T>(this string json_string)
		//{
		//	try {
		//		return JsonConvert.DeserializeObject<T>(json_string);
		//	} catch (Exception ex) {
		//		if (json_string.IsNullOrWhite()) {
		//			throw new ArgumentNullException($"参数值json_string的值为空或null");
		//		}
		//		throw new FormatException($"参数json_string的值不符合类型{typeof(T).Name}的格式", ex.GetInnermostException());
		//	}
		//}

		/// <summary>
		/// 转换成JArray类型
		/// </summary>
		/// <typeparam name="T">原类型</typeparam>
		/// <param name="list">List集合</param>
		/// <returns><see cref="JArray"/>jarray结果集</returns>
		//public static JArray ToJArray<T>(this IEnumerable<T> list)
		//{
		//	try {
		//		return list == null ? null : JArray.FromObject(list);
		//	} catch (Exception ex) {
		//		throw new InvalidCastException($"类型{list.GetType().Name}无法转换成类型{typeof(JArray).Name}", ex.GetInnermostException());
		//	}
		//}

		/// <summary>
		/// 转换成JObject类型
		/// </summary>
		/// <param name="t">对象</param>
		/// <returns><see cref="JObject"/>JObject对象</returns>
		//public static JObject ToJObject<T>(this T t)
		//{
		//	try {
		//		return t == null ? null : JObject.FromObject(t);
		//	} catch (Exception ex) {
		//		throw new InvalidCastException($"类型{t.GetType().Name}无法转换成类型{typeof(JObject).Name}", ex.GetInnermostException());
		//	}
		//}

		/// <summary>
		/// DataTable转换成Jarray
		/// </summary>
		/// <param name="dt"><see cref="DataTable"/>DataTable 表</param>
		/// <returns><see cref="JArray"/>jarray结果集</returns>
		//public static JArray ToJArray(this DataTable dt)
		//{
		//	try {
		//		if (dt == null) {
		//			return null;
		//		} else if (dt.Rows.Count == 0) {
		//			return new JArray();
		//		}
		//		return JArray.FromObject(dt.Rows[0].Table);
		//	} catch (Exception ex) {
		//		throw new InvalidCastException($"类型{typeof(DataTable).Name}无法转换成类型{typeof(JArray).Name}", ex.GetInnermostException());
		//	}
		//}
	}

	public class BDSharedExceptionConstant
	{
		public BDSharedExceptionConstant()
		{
		}
	}

	public class StringIsNotTartgetFormatException : ApplicationException
	{
		public StringIsNotTartgetFormatException(string msg) : base(msg)
		{
		}

		public StringIsNotTartgetFormatException(string msg, Exception innerException) : base(msg, innerException)
		{
		}

		public StringIsNotTartgetFormatException(Type type) : base($"字符串不符合{type.Name}类型的格式！")
		{
		}

		public StringIsNotTartgetFormatException(Type type, Exception innerException) : base($"字符串不符合{type.Name}类型的格式！", innerException)
		{
		}
	}

	public class ExpressionIsNotTureException : ApplicationException
	{
		public ExpressionIsNotTureException(string msg) : base(msg)
		{
		}

		public ExpressionIsNotTureException(string msg, Exception innerExceptiion) : base(msg, innerExceptiion)
		{
		}

		public ExpressionIsNotTureException(Type type) : base($"该表达式不是{type.Name}表达式")
		{
		}

		public ExpressionIsNotTureException(Type type, Exception innerExceptiion) : base($"该表达式不是{type.Name}表达式", innerExceptiion)
		{
		}
	}
}
