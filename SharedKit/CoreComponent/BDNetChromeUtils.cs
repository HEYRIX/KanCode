using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using SharedKit.CoreComponent;

namespace SharedKit
{
	public class BDNetChromeUtils
	{
		public static void Run()
		{
			BDNetUtils.GetGatewayAddressList();

			// our text to protect
			var text = "Hello!";
			// get bytes from text
			var bytes = Encoding.UTF8.GetBytes(text);
			// optional entropy
			var entropy = new byte[] { 100, 25, 31, 213 };
			// protect (encrypt)
			var protectedBytes = CrossProtect.Protect(bytes, entropy, DataProtectionScope.CurrentUser);
			// unprotect (decrypt)
			var unprotected = CrossProtect.Unprotect(protectedBytes, entropy, DataProtectionScope.CurrentUser);
			// convert bytes back to text
			var result = Encoding.UTF8.GetString(unprotected);
			var strChromeCookie = BDNetChromeUtils.GetChromeCookie("");
			var macip = BDNetUtils.GetMACIp();
		}

		// https://stackoverflow.com/questions/60230456/dpapi-fails-with-cryptographicexception-when-trying-to-decrypt-chrome-cookies
		// https://stackoverflow.com/questions/68643057/decrypt-google-cookies-in-c-sharp-net-framework
		public static List<Cookie> GetCookies(string hostname)
		{
			string ChromeCookiePath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Google\Chrome\User Data\Default\Cookies";
			List<Cookie> data = new List<Cookie>();
			if (File.Exists(ChromeCookiePath)) {
				try {
					using var conn = new SqliteConnection($"Data Source={ChromeCookiePath}");
					using var cmd = conn.CreateCommand();
					cmd.CommandText = $"SELECT name,encrypted_value,host_key FROM cookies WHERE host_key = '{hostname}'";
					byte[] key = AesGcm256.GetKey();

					conn.Open();
					using (var reader = cmd.ExecuteReader()) {
						while (reader.Read()) {
							if (!data.Any(a => a.Name == reader.GetString(0))) {
								byte[] encryptedData = GetBytes(reader, 1);
								byte[] nonce, ciphertextTag;
								AesGcm256.Prepare(encryptedData, out nonce, out ciphertextTag);
								string value = AesGcm256.Decrypt(ciphertextTag, key, nonce);

								data.Add(new Cookie() {
									Name = reader.GetString(0),
									Value = value
								});
							}
						}
					}
					conn.Close();
				} catch { }
			}
			return data;
		}

		public static string GetChromeCookie(string url)
		{
			// Read Chrome Cookie
			// http://www.meilongkui.com/archives/1904
			// C# https://www.cnblogs.com/zhouyg2017/p/14296398.html
			// https://github.com/QAX-A-Team/BrowserGhost
			var hostKey = ".baidu.com";
			var ret = "";
			using (var dbConnection = new SqliteConnection()) {
				var userprofilePath = Environment.GetEnvironmentVariable("USERPROFILE");
				// WinOS
				// MacOS https://apple.stackexchange.com/questions/232433/where-are-google-chrome-cookies-stored-on-a-mac
				// db.location "~/Library/Application Support/Google/Chrome/Default/Cookies"
				// ro "~/Library/Application\ Support/Google/Chrome/Profile\ 10/Cookies" not login?
				//connection.ConnectionString = $@"DataSource={userprofilePath}\AppData\Local\Google\Chrome\User Data\Default\Cookies";
				var strUserPath = BDPathUtils.GetUserPath();
				var filePath = $@"{strUserPath}/Library/Application Support/Google/Chrome/Default/Cookies";
				filePath = BDSharedUtils.SharedDirPath() + "/Chrome.Cookies.db";
				dbConnection.ConnectionString = $@"DataSource={filePath}";
				var dbState = dbConnection.State;
				if (dbState == System.Data.ConnectionState.Closed) {
					dbConnection.Open();
				}
				dbConnection.Open();
				var command = new SqliteCommand($"SELECT host_key, name, encrypted_value FROM cookies WHERE host_key='{hostKey}'", dbConnection);

				SqliteDataReader dataReader = command.ExecuteReader();
				var isRead = dataReader.Read();
				byte[] encryptedValue = (byte[])dataReader["encrypted_value"];

				int keyLength = 256 / 8;
				int nonceLength = 96 / 8;
				var kEncryptionVersionPrefix = "v10";
				int GCM_TAG_LENGTH = 16;

				//字符串内容取自C:\Users\用户名\AppData\Local\Google\Chrome\User Data\Local State文件的encrypted_key
				byte[] encryptedKeyBytes = Convert.FromBase64String("RFBBUEkBAAAA0Iyd3wEV0RGMegDAT8KX6wEAAAA3PHk3a5NmQpRxjGtdwCCCAAAAAAIAAAAAABBmAAAAAQAAIAAAALd7GZJyVqp7yQUBIEUvv0cwGN/mdUVrvAqqgbdJyJwoAAAAAA6AAAAAAgAAIAAAAPjIbfKCXRBggBNixV8sG409GYD9QRUHpiRMf/7s7Nm7MAAAABobpenJlhdxFJQw5PI1Fk/X0COpn+HZUxNl+GahUsmydEdXWJg0w5KmZjC7QjKJ/EAAAAA/rz1g3B2SdeXFMesLCZ/5O+xEDYxjeUP1hCw4Fa9rrLeUWpLkmmgL9JRNvSaiMfISpGXcWsr5zvhOLaF2kJ81");
				encryptedKeyBytes = encryptedKeyBytes.Skip("DPAPI".Length).Take(encryptedKeyBytes.Length - "DPAPI".Length).ToArray();

				try {
					// for WinOS
					//var keyBytes = ProtectedData.Unprotect(encryptedKeyBytes, null, DataProtectionScope.CurrentUser);
					//var entropy = new byte[] { };
					var keyBytes = CrossProtect.Unprotect(encryptedKeyBytes, null, DataProtectionScope.CurrentUser);
					// for Non-WinOS
					// https://github.com/integrativesoft/CrossProtectedData
					var nonce = encryptedValue.Skip(kEncryptionVersionPrefix.Length).Take(nonceLength).ToArray();
					encryptedValue = encryptedValue.Skip(kEncryptionVersionPrefix.Length + nonceLength).Take(encryptedValue.Length - (kEncryptionVersionPrefix.Length + nonceLength)).ToArray();

					var str = AesGcm256.AesGcmDecrypt(keyBytes, nonce, encryptedValue);
					Console.WriteLine($"{dataReader["host_key"]}-{dataReader["name"]}-{str}");
					ret = str;
				} catch (Exception ex) {
					System.Console.WriteLine(ex.Message);
				}
				dbConnection.Close();
			}

			return ret;
		}

		private static byte[] GetBytes(SqliteDataReader reader, int columnIndex)
		{
			const int CHUNK_SIZE = 2 * 1024;
			byte[] buffer = new byte[CHUNK_SIZE];
			long bytesRead;
			long fieldOffset = 0;
			var stream = new MemoryStream();
			while ((bytesRead = reader.GetBytes(columnIndex, fieldOffset, buffer, 0, buffer.Length)) > 0) {
				stream.Write(buffer, 0, (int)bytesRead);
				fieldOffset += bytesRead;
			}
			return stream.ToArray();
		}

		public class Cookie
		{
			public string Name { get; set; }
			public string Value { get; set; }

			public Cookie()
			{
				this.Name = "";
				this.Value = "";
			}
		}

		class AesGcm256
		{
			public static byte[] GetKey()
			{
				var sR = string.Empty;
				var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string path = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Google\Chrome\User Data\Local State";
				_ = File.ReadAllText(path);

				dynamic json = "";// JsonConvert.DeserializeObject(v);
				string key = json.os_crypt.encrypted_key;

				byte[] src = Convert.FromBase64String(key);
				byte[] encryptedKey = src.Skip(5).ToArray();
#pragma warning disable CA1416 // Validate platform compatibility
				byte[] decryptedKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
#pragma warning restore CA1416 // Validate platform compatibility

				return decryptedKey;
			}

			public static string Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
			{
				string sR = string.Empty;
				try {
					var cipher = new GcmBlockCipher(new AesEngine());
					var parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

					cipher.Init(false, parameters);
					byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
					Int32 retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
					cipher.DoFinal(plainBytes, retLen);

					sR = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
				}

				return sR;
			}

			public static void Prepare(byte[] encryptedData, out byte[] nonce, out byte[] ciphertextTag)
			{
				nonce = new byte[12];
				ciphertextTag = new byte[encryptedData.Length - 3 - nonce.Length];

				System.Array.Copy(encryptedData, 3, nonce, 0, nonce.Length);
				System.Array.Copy(encryptedData, 3 + nonce.Length, ciphertextTag, 0, ciphertextTag.Length);
			}

			public static string AesGcmDecrypt(byte[] keyBytes, byte[] nonce, byte[] encryptedValue)
			{
				var gcmBlockCipher = new GcmBlockCipher(new AesEngine());
				var aeadParameters = new AeadParameters(new KeyParameter(keyBytes), 128, nonce);
				gcmBlockCipher.Init(false, aeadParameters);
				byte[] plaintext = new byte[gcmBlockCipher.GetOutputSize(encryptedValue.Length)];
				int length = gcmBlockCipher.ProcessBytes(encryptedValue, 0, encryptedValue.Length, plaintext, 0);
				gcmBlockCipher.DoFinal(plaintext, length);
				return Encoding.UTF8.GetString(plaintext);
			}

			/// <summary>
			/// 获取页面html
			/// </summary>
			/// <param name="url">请求的地址</param>
			/// <param name="encoding">编码方式</param>
			/// <returns></returns>
			public static string GetWebHtmlContent(string url, string encoding)
			{
				string pageHtml = string.Empty;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
				try {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
					var webClient = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
					Encoding encode = Encoding.GetEncoding(encoding);
					webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.84 Safari/537.36");
					webClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
					Byte[] pageData = webClient.DownloadData(url); //从指定网站下载数据
					pageHtml = encode.GetString(pageData);
				} catch (Exception) {

				}
#pragma warning restore IDE0059 // Unnecessary assignment of a value
				return pageHtml;
			}
		}
	}
}
