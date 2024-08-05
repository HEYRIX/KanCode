using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;

namespace SharedKit
{
	public interface IBDNetInterface {
	}

	public sealed class BDNetObject : IBDNetInterface {
		public HttpMethod Method { get; set; }
		public string Site { get; set; }
		public string Referer { get; set; }
		public string Origin { get; set; }
		public string Cookie { get; set; }
		public string Content { get; set; }
		public StringContent StringContent { get; set; } // TODO NOT Check
		public string ContentType { get; set; }
		public bool IsAutoReDericted { get; set; }
		public Dictionary<String, String> HeadSet { get; set; }
		public Dictionary<String, String> BodySet { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public BDNetObject() {
			this.Origin = "";
			this.Cookie = "";
			this.Referer = "";
			this.Content = "";
			this.IsAutoReDericted = true;
			this.HeadSet = new Dictionary<string, string>();
			this.BodySet = new Dictionary<string, string>();
		}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	}

	public class BDNetManager
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		private static BDNetManager _pInstance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE0090 // Use 'new(...)'
		private static readonly object _locker = new object();
#pragma warning restore IDE0090 // Use 'new(...)'
		private BDNetManager() {
			this.HttpClientSet = new Dictionary<string, HttpClient>();
		}

		/// <summary>
		/// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
		/// </summary>
		/// <returns></returns>
		public static BDNetManager Instance() {
			// 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
			// 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
			// lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
			// 双重锁定只需要一句判断就可以了
			if (_pInstance == null) {
				lock (_locker) {
					if (_pInstance == null) {
						_pInstance = new BDNetManager();
					}
				}
			}
			return _pInstance;
		}

		private Dictionary<String, HttpClient> HttpClientSet { get; set; }
		//private static HttpClient _httpClient = new HttpClient();

		public static HttpClient GetHttpClient(bool isAutoReDericted = true) {
			var net = BDNetManager.Instance();
			// In production code, don't destroy the HttpClient through using, but better use IHttpClientFactory factory or at least reuse an existing HttpClient instance
			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests
			// https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
			var handler = new HttpClientHandler {
				// If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
				AutomaticDecompression = System.Net.DecompressionMethods.All,
				UseCookies = false, // TODO TOBE Validate
				AllowAutoRedirect = isAutoReDericted,
			};

			var kNetHttpKey1 = "AllowAutoRedirect";
			var kNetHttpKey2 = "Not.AllowAutoRedirect";
			var cur = isAutoReDericted ? kNetHttpKey1 : kNetHttpKey2;
			if (null == net.HttpClientSet) {
				net.HttpClientSet = new Dictionary<string, HttpClient>();
			}
			var isContained = false;
			foreach (var key in net.HttpClientSet.Keys) {
				if (key.Equals(cur) && key.Trim().Length > 0) {
					isContained = true;
					break;
				}
			}
			var retVal = isContained ? net.HttpClientSet[cur] : new HttpClient(handler);
			if (!isContained) {
				// In production code, don't destroy the HttpClient through using, but better use IHttpClientFactory factory or at least reuse an existing HttpClient instance
				// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests
				// https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
				//using (var httpClient = new HttpClient(handler)) {
				//}
				//retVal = new HttpClient(handler);
				BDNetManager.Instance().HttpClientSet.Add(cur, retVal);
				System.Diagnostics.Debug.Assert(BDNetManager.Instance().HttpClientSet.ContainsKey(cur));
			}
			System.Diagnostics.Debug.Assert(null != retVal);

			return retVal;
		}
	}

	internal static class HttpClientHelper
	{
		// https://www.cnblogs.com/Wddpct/p/6229090.html
		// https://www.cnblogs.com/dudu/p/csharp-httpclient-attention.html
		private static readonly object _lockHandler = new object();
		private static HttpClient _httpClient = null;
		public static HttpClient HttpClient {
			get {
				if (null == _httpClient) {
					lock (_lockHandler) {
						if (null == _httpClient) {
							_httpClient = new HttpClient();
							_httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
							// http://byterot.blogspot.com/2016/07/singleton-httpclient-dns.html
							_httpClient.DefaultRequestHeaders.ConnectionClose = true;
						}
					}
				}
				return _httpClient;
			}
		}
	}

	// cURL -> C#
	// https://github.com/olsh/curl-to-csharp
	// https://curl.olsh.me/
	/*
	 curl 'https://fawkes.bilibili.co/x/admin/fawkes/app/ci/list?id=&id_type=1&gl_job_id=&pkg_type=0&status=0&git_type=0&git_keyword=&operator=&order=id&sort=desc&pn=1&ps=5&app_key=iphone' \
	  -H 'Connection: keep-alive' \
	  -H 'x1-bilispy-user: fawkes_huangliang' \
	  -H 'Referer: https://fawkes.bilibili.co/' \
	  -H 'Cookie: username=huangliang; UM_distinctid=17c6d96a68541-046d6d7927e92e-123b6650-1fa400-17c6d96a686329; X-CSRF=$2b$10$80RmmtXnsnxG4nFJtRBZ3eTqWT91HM1K7rAwDcpdBRPktVFRqBnr2; b_nut=1640831986; buvid3=23860CC5-F95B-C0CE-C694-634177CA1A9786979infoc; buvid_fp=13b447cfe2c3764d6b4f02c5d37bc842; buvid4=FBF19000-09DE-B2EA-5C80-294D7615747584218-022012118-C34HN9S73zrywuG4/UVeSw%3D%3D; _AJSESSIONID=fa8dcff32e193812bcf3064865e459ce; SecurityProxySessionID=V1_OGYxYjJhZDItMjI4ZS0zNjI1LWEwMDUtZTVjZjliZGNkNjE5; b_lsid=A8C941E10_17E9E91F226; mng-go=dfcf0f77abc9bac002fe9c891da9d72e0b70f0363b322890ad83b6d1cd78d0f9' \
	  --compressed
	 */
	public class BDNetUtils
	{
		private BDNetUtils() {
		}

		public static HttpRequestMessage InitHttpRequest(BDNetObject httpObject) {
			System.Diagnostics.Debug.Assert(null != httpObject);
			System.Diagnostics.Debug.Assert(null != httpObject.Site);
			System.Diagnostics.Debug.Assert(null != httpObject.Method);

			using var request = new HttpRequestMessage(httpObject.Method, httpObject.Site);
			{
				if (httpObject.Method == HttpMethod.Get) {
					//request.Headers.TryAddWithoutValidation("authority", "www.green-gcgl.com");
					request.Headers.TryAddWithoutValidation("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
					request.Headers.TryAddWithoutValidation("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,zh-TW;q=0.7");
					request.Headers.TryAddWithoutValidation("cookie", httpObject.Cookie ?? "");
					request.Headers.TryAddWithoutValidation("dnt", "1");
					request.Headers.TryAddWithoutValidation("referer", httpObject.Referer ?? "");
					request.Headers.TryAddWithoutValidation("sec-ch-ua", "\".Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"103\", \"Chromium\";v=\"103\"");
					request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
					request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"macOS\"");
					request.Headers.TryAddWithoutValidation("sec-fetch-dest", "document");
					request.Headers.TryAddWithoutValidation("sec-fetch-mode", "navigate");
					request.Headers.TryAddWithoutValidation("sec-fetch-site", "same-origin");
					request.Headers.TryAddWithoutValidation("sec-fetch-user", "?1");
					request.Headers.TryAddWithoutValidation("upgrade-insecure-requests", "1");
					request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
				} else if (httpObject.Method == HttpMethod.Post) {
					//System.Diagnostics.Debug.Assert(false);
					//request.Content = new StringContent(httpObject.Content);
					request.Content = httpObject.StringContent;
					request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
				} else {
					System.Diagnostics.Debug.Assert(false);
				}

				var hDictSet = httpObject.HeadSet ?? new Dictionary<string, string>();
				foreach (var kk in hDictSet.Keys) {
					var vv = hDictSet[kk];
					request.Headers.TryAddWithoutValidation(kk, vv);
				}
			}
			return request;
		}

		public static async Task<String> TryGetNetWorkAsync(BDNetObject httpObject)
		{
			var ret = "";
			var response = await HandleNetWorkAsync(httpObject);
			if (response.StatusCode == System.Net.HttpStatusCode.OK) {
				//var htmlPageObject2 = BDGardenUtils.InitHtmlPageContent(response);
				//var datPageArray2 = HandleBidInvitePageArray(htmlPageObject2);
				//ret.AddRange(datPageArray2);
				var data = await response.Content.ReadAsStringAsync();
				ret = data;
			} else {
				System.Diagnostics.Debug.Assert(false);
				//BDSharedUtils.LogOut($"{""}");
			}
			return ret;
		}

		public static String GetMediaType(HttpResponseMessage response)
		{
			var type = response.Content.Headers.ContentType;
			if (type.MediaType == "application/pdf") { return "pdf"; }
			if (type.MediaType == "text/html") { return "html"; }
			System.Diagnostics.Debug.Assert(false);
			return "txt";
		}

		public static List<System.String> GetGatewayAddressList()
		{
			var ret = new List<string>();
			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface adapter in adapters) {
				IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
				GatewayIPAddressInformationCollection addrArray = adapterProperties.GatewayAddresses;
				if (addrArray.Count > 0) {
					Console.WriteLine(adapter.Description);
					foreach (GatewayIPAddressInformation address in addrArray) {
						//Console.WriteLine("  Gateway Address ......................... : {0}", address.Address.ToString());
						ret.Add(address.Address.ToString());
					}
				}
			}
			return ret;
		}

		public static HttpClientHandler InitHandler()
		{
			//https://support.microsoft.com/en-us/topic/update-to-enable-tls-1-1-and-tls-1-2-as-default-secure-protocols-in-winhttp-in-windows-c4bd73d2-31d7-761e-0178-11268bb10392
			var handler = new HttpClientHandler();
			handler.UseCookies = false;
			// https://stackoverflow.com/questions/52939211/the-ssl-connection-could-not-be-established
			handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
			// https://stackoverflow.com/questions/69643209/ssl-connection-could-not-be-established
			handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
			// If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
			//handler.AutomaticDecompression = ~DecompressionMethods.None;
			return handler;
		}

		public static string ToHttpBodyContent(Dictionary<String, String> dict, String join)
		{
			var ret = String.Join("&", dict.Select(kvItem => String.Format($"{kvItem.Key}={System.Web.HttpUtility.UrlEncode(kvItem.Value)}")));
			return ret;
		}

		/// <summary>
		/// 获取电脑M的AC物理地址
		/// </summary>
		/// <returns></returns>
		public static string GetMACIp()
		{
			//本地计算机网络连接信息
			IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
			//获取本机所有网络连接
			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
			//获取本机电脑名
			var HostName = computerProperties.HostName;
			//获取域名
			var DomainName = computerProperties.DomainName;
			if (nics == null || nics.Length < 1) {
				return "";
			}

			var MACIp = "";
			foreach (NetworkInterface adapter in nics) {
				var adapterName = adapter.Name;
				var adapterDescription = adapter.Description;
				var NetworkInterfaceType = adapter.NetworkInterfaceType;
				if (adapterName == "本地连接") {
					PhysicalAddress address = adapter.GetPhysicalAddress();
					byte[] bytes = address.GetAddressBytes();

					for (int i = 0; i < bytes.Length; i++) {
						MACIp += bytes[i].ToString("X2");
						if (i != bytes.Length - 1) {
							MACIp += "-";
						}
					}
				}
			}
			return MACIp;
		}

		public static async Task<System.String> GetCookie(string url)
		{
			var ret = "";
			try {
				//var sbCookie = new StringBuilder();
				var cookieContainer = new CookieContainer();
				var uri = new Uri(url);
				var handler = new HttpClientHandler();
				handler.CookieContainer = cookieContainer;
				HttpClient httpClinet = new HttpClient(handler);
				await httpClinet.GetAsync(uri);
				var cookies = cookieContainer.GetCookies(uri).ToList<Cookie>();
				foreach (var item in cookies) {
					//sbCookie.Append(item.Name);
					//sbCookie.Append(item.ToString());
					//sbCookie.Append(item.Name);
					//sbCookie.Append(";");
					if (uri.Host.Equals(item.Domain)) {
						ret = item.ToString();
						break;
					}
				}
				//ret = sbCookie.ToString();
				//System.Console.WriteLine("");
			} catch (Exception ex) {
				System.Console.WriteLine(ex.Message);
			}
			return ret;
		}

		public static async Task<HttpResponseMessage> HandleNetWorkAsync(BDNetObject httpObject)
		{
			System.Diagnostics.Debug.Assert(null != httpObject.Method);
			System.Diagnostics.Debug.Assert(null != httpObject.Site);

			using var request = new HttpRequestMessage(httpObject.Method, httpObject.Site);
			request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
			request.Headers.TryAddWithoutValidation("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,zh-TW;q=0.7");
			request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
			if (null != httpObject.Referer && httpObject.Referer.Length > 0) {
				request.Headers.TryAddWithoutValidation("Referer", httpObject.Referer);
			}
			request.Headers.TryAddWithoutValidation("DNT", "1");
			request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "iframe");
			request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
			request.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
			request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
			request.Headers.TryAddWithoutValidation("sec-ch-ua", "\".Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"103\", \"Chromium\";v=\"103\"");
			request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
			request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"macOS\"");
			if (null != httpObject.Cookie && httpObject.Cookie.Length > 0) {
				request.Headers.TryAddWithoutValidation("Cookie", httpObject.Cookie);
			}

			if (httpObject.Method == HttpMethod.Get) {
				request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
			} else if (httpObject.Method == HttpMethod.Post) {
				request.Headers.TryAddWithoutValidation("Cache-Control", "max-age=0");
				if (null != httpObject.Origin && httpObject.Origin.Length > 0) {
					request.Headers.TryAddWithoutValidation("Origin", httpObject.Origin);
				}
				request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
				request.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
				//request.Headers.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

				System.Diagnostics.Debug.Assert(null != httpObject.Content);
				System.Diagnostics.Debug.Assert(null != httpObject.ContentType);
				request.Content = new StringContent(httpObject.Content);
				//request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded; charset=UTF-8");
				request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
			} else {
				System.Diagnostics.Debug.Assert(false);
			}

			var httpClient = BDNetManager.GetHttpClient(httpObject.IsAutoReDericted);
			var response = await httpClient.SendAsync(request);
			//_ = response.EnsureSuccessStatusCode();
			//if (response.StatusCode == System.Net.HttpStatusCode.OK) {
			//	//var htmlPageObject2 = BDGardenUtils.InitHtmlPageContent(response);
			//	//var datPageArray2 = HandleBidInvitePageArray(htmlPageObject2);
			//	//ret.AddRange(datPageArray2);
			//	//Console.WriteLine($"BidInvite.Net.QueryMode has {datPageArray2.Count} items in {curPageIndex}/{totalPageCount} {date1}");
			//	var data = await response.Content.ReadAsStringAsync();
			//	ret = data;
			//} else {
			//	System.Diagnostics.Debug.Assert(false);
			//	BDSharedUtils.LogOut($"{""}");
			//}
			//return ret;
			//BDSharedUtils.LogOut($"HttpStatusCode:{response.StatusCode} {httpObject.Url}");
			return response;
		}
	}
}
