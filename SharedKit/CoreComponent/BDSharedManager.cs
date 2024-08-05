using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKit
{
	public class BDSharedWorkItem : IBDSharedObject
	{
		public String ClientIP { get; set; }
		public String TimeZone { get; set; }
		public Int64 UnixTime { get; set; }
		public DateTime DateTime { get; set; }

		public BDSharedWorkItem()
		{
			this.ClientIP = "";
			this.TimeZone = "";
			this.UnixTime = 0;
		}
	}

	internal class BDWorldTimeBridgeInfo : IBDSharedSerializedItem
	{
		[JsonPropertyName("client_ip")]
		public String ClientIP { get; set; }
		[JsonPropertyName("timezone")]
		public String TimeZone { get; set; }
		[JsonPropertyName("unixtime")]
		public Int64 UnixTime { get; set; }
		[JsonPropertyName("datetime")]
		public DateTime DateTime { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public BDWorldTimeBridgeInfo()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			this.TimeZone = "";
		}
	}

	public class BDSharedManager : BDSharedSingletonClass<BDSharedManager>
	{
		public BDSharedWorkItem SystemInfo { get; set; }

		public BDSharedManager()
		{
			this.SystemInfo = new BDSharedWorkItem();
		}

		public static async Task<bool> RunSharedWorking()
		{
			//_ = await GetNetTimeInfo();
			//_ = await GetNetNetInfo();
			return true;
		}

		public static async Task<String> GetNetTimeInfo()
		{
			// http://worldtimeapi.org/
			// https://worldtimeapi.org/api/timezone
			// curl "http://worldtimeapi.org/api/timezone/Asia/Singapore"
			/*
			curl https://worldtimeapi.org/api/timezone/Asia/Shanghai
			{
				"abbreviation":"CST",
				"client_ip":"2409:8a1e:6c69:17d0:b8b1:1a6e:4d37:11e0",
				"datetime":"2022-08-02T21:25:06.795802+08:00",
				"day_of_week":2,
				"day_of_year":214,
				"dst":false,
				"dst_from":null, "dst_offset":0,"dst_until":null,
				"raw_offset":28800,
				"timezone":"Asia/Shanghai",
				"unixtime":1659446706,
				"utc_datetime":"2022-08-02T13:25:06.795802+00:00",
				"utc_offset":"+08:00",
				"week_number":31
			}
			*/
			var url = $"http://worldtimeapi.org/api/timezone/Asia/Shanghai";
			using var request = new HttpRequestMessage(HttpMethod.Get, url);
			var response = await BDNetManager.GetHttpClient().SendAsync(request);
			var ret = "";
			if (response.StatusCode == System.Net.HttpStatusCode.OK) {
				var data = await response.Content.ReadAsStringAsync();
				//var datObject = JsonSerializer.Deserialize<BDGreenKeepInviteBridgeAllInfo>(data) ?? new BDGreenKeepInviteBridgeAllInfo();
				var dInfo = JsonSerializer.Deserialize<BDWorldTimeBridgeInfo>(data) ?? new BDWorldTimeBridgeInfo();
				var datObject = new BDSharedWorkItem() {
					ClientIP = dInfo.ClientIP,
					TimeZone = dInfo.TimeZone,
					UnixTime = dInfo.UnixTime,
					DateTime = dInfo.DateTime,
				};
				BDSharedManager.Instance().SystemInfo = datObject;
				BDSharedUtils.LogOut($"System Info {datObject.DateTime} {datObject.TimeZone} IP {datObject.ClientIP}");
			} else {
				ret = "属地未知";
			}
			return ret;
		}

		private static async Task<String> GetNetNetInfo()
		{
			// Not Statble
			// https://www.ip.cn/
			var url = $"https://www.ip.cn/api/index?ip=&type=0";
			var httpObject = new BDNetObject() {
				Method = HttpMethod.Get,
				Site = url,
				Referer = "https://www.ip.cn/",

			};
			var data = await BDNetUtils.TryGetNetWorkAsync(httpObject);
			var dInfo = JsonSerializer.Deserialize<BDWorldTimeBridgeInfo>(data) ?? new BDWorldTimeBridgeInfo();
			var datObject = new BDSharedWorkItem() {
				ClientIP = dInfo.ClientIP,
				TimeZone = dInfo.TimeZone,
				UnixTime = dInfo.UnixTime,
				DateTime = dInfo.DateTime,
			};
			BDSharedUtils.LogOut($"System Info {datObject.DateTime} {datObject.TimeZone} IP {datObject.ClientIP}");

			return "";
		}
	}
}
