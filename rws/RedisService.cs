using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace rws
{
    public partial class RedisService : ServiceBase
    {
        private RedisProcess process;
        private Timer timer;

        public RedisService()
        {
            InitializeComponent();
            string rp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dist");
            process = new RedisProcess(rp);
            timer = new Timer();
            timer.Elapsed += (sender, args) =>
            {
                lock (process)
                {
                    if (process.IsStop())
                    {
                        Logger.Log("Redis Restart");
                        process.Start();
                    }
                }
            };
            timer.Interval = 5000;
        }

        /// <summary>
        /// 开始进程和常驻。
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            lock(process)
            {
                Logger.Log("Redis Start");
                process.Start();
                timer.Start();
            }
        }

        /// <summary>
        /// 停止重试和进程。
        /// </summary>
        protected override void OnStop()
        {
            lock(process)
            {
                timer.Stop();
                process.Stop();
                Logger.Log("Redis Stop");
            }
        }
    }
}
