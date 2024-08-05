using Quartz;

namespace ProxySchedule
{
    public class ProxyIpJob : IJob
    {
        private static bool isRun = false;
        public Task Execute(IJobExecutionContext context) {
            if (!isRun) {
                return Task.Run(() =>
                {
                    isRun = true;
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Start Schedule Task ...");
                    ProxyIpHelper proxyhelper = new ProxyIpHelper();
                    proxyhelper.Start();
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Stop Schedule Task ...");
                    isRun = false;
                });
            } else {
                return Task.Run(() => {
                });
            }
        }
    }
}
