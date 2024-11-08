﻿using System.Text;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;

namespace SharedKit
{
    // https://github.com/cfan1236/ProxyIpSchedule
    // https://www.cnblogs.com/youring2/p/quartz_net.html
    // https://www.cnblogs.com/CRobot/p/17049838.html
    public class BDProxyScheduleMain
	{
		public BDProxyScheduleMain() {
		}

		private static IConfigurationRoot? Configuration { get; set; }
		private static Task<IScheduler>? scheduler = null;
		static void Main(string[] args)
		{
			#region 配置文件
			var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
							//注意文件及路径的大小写 linux上很敏感
							.AddJsonFile("Configs/AppSettings.json");
			Configuration = builder.Build();
			#endregion
			//注册encoding 后面请求某些服务器页面时防止编码报错
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			//创建定时任务
			var cor = Configuration["ProxyJobExpression"];
			scheduler = StdSchedulerFactory.GetDefaultScheduler();
			CreateJob<ProxySchedule.ProxyIpJob>("proxyHelper", cor??"");
			scheduler.Result.Start();
			//开启时输出控制台, nlog日志组件只用于业务层面。
			Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 调度管理器启动成功...");
			Console.ReadKey();

		}

        /// <summary>
        /// 创建Job
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <param name="cronExpression"></param>
		private static void CreateJob<T>(string uid, string cronExpression) where T : IJob {
			var job = JobBuilder.Create<T>()
				.WithIdentity("job" + uid, "group" + uid)
				.Build();
			var cronTrigger = (ICronTrigger)TriggerBuilder.Create()
												.WithIdentity("trigger" + uid, "group" + uid)
												.StartNow()
												.WithCronSchedule(cronExpression)
												.Build();
			var ft = scheduler.Result.ScheduleJob(job, cronTrigger);
		}
	}
}

