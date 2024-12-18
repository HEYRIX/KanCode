﻿using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NLog;

namespace ProxySchedule
{
    public class ProxyIpHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly List<string> _proxyList = new List<string>();
        public void Start() {
            _logger.Info("开始获取代理IP");
            //用5个线程去抓取数据  5*3=15页数据
            Task[] tk = new Task[5];
            int pageindex = 1;
            for (int i = 0; i < 5; i++) {
                PageParam pp = new PageParam(pageindex, 3);
                tk[i] = new Task(() =>
                {
                    GetNewIpList(pp);
                });
                tk[i].Start();
                pageindex += 3;
            }
            //设置20分钟超时
            Task.WaitAll(tk, (1000 * 60) * 20);
            _logger.Info($"共获取到:{_proxyList.Count}个有效IP");
            //运行目录
            var runPath = Path.GetDirectoryName(typeof(ProxyIpHelper).Assembly.Location);
            if (_proxyList.Count > 0) {
                File.WriteAllLines(runPath + "/newIp.txt", _proxyList);
            }
        }
        private void GetNewIpList(PageParam pp) {
            var index = pp.PageIndex;
            var size = pp.PageSize;
            for (int i = index; i < index + size; i++) {
                //高匿名代理
                var url = "https://www.xicidaili.com/nn/" + i;
                try {
                    var html = NetHttpHelper.HttpGetRequest(url, out int status, 5000);
                    if (status == 200) {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        var table_tr = doc.DocumentNode.SelectNodes(".//table[@id='ip_list']/tr");
                        if (table_tr != null) {
                            //第一个是标题 这里从第二个开始
                            for (int j = 1; j < table_tr.Count; j++) {
                                var tr_child = table_tr[j].ChildNodes;
                                var ip_str = tr_child[3].InnerText;
                                var port_str = tr_child[5].InnerText;
                                var speed_html = tr_child[13].InnerHtml;
                                //提取速度
                                var regexStr = string.Format(" <{0}[^>]*?{1}=(['\"\"]?)(?<text>[^'\"\"\\s>]+)\\1[^>]*>", "div", "title");
                                Match TitleMatch = Regex.Match(speed_html, regexStr, RegexOptions.IgnoreCase);
                                string speed_text = TitleMatch.Groups["text"].Value;
                                speed_text = speed_text.Replace("秒", "");
                                var v1 = int.TryParse(port_str, out int port);
                                var v2 = float.TryParse(speed_text, out float speed);
                                //只提取速度<=2s的
                                if (speed <= 2) {
                                    //检查IP有效性
                                    Proxy px = new Proxy(ip_str, port);
                                    if (IsIPCheckVaild(px)) {
                                        //写入列表 稍后一并写入文件
                                        _proxyList.Add(ip_str + "," + port_str);
                                    }
                                }
                            }
                        }
                    }
                } catch (Exception ex) {
					//获取代理IP出错
					_logger.Info("获取代理IP:" + url + " 出错");
                    _logger.Error(ex);
                }
            }
        }
		/// <summary>
		/// 检查IP有效性
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private static bool IsIPCheckVaild(Proxy p) {
			var flag = false;
			try {
				_logger.Debug($"Checking IP: {p.IP}:{p.Port}");
				var rand = new Random();
				var n = rand.Next(100000);
				var html = NetHttpHelper.HttpGetRequest("http://2018.ip138.com/ic.asp?" + n, out int status, 2000, p.IP, p.Port);
				if (status == 200) {
					if (html.Contains(p.IP) && !html.Contains("无效")) {
						flag = true;
						_logger.Debug($"IP:{p.IP}:{p.Port} 有效");
					}
				}
			} catch (Exception) {
			}
			return flag;
		}
    }
}


