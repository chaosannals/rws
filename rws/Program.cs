using System;
using System.ServiceProcess;

namespace rws
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            Logger.Init();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Logger.Log("捕获了漏掉的异常：{0}", e.ExceptionObject.ToString());
            };
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Logger.Quit();
            };
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RedisService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
