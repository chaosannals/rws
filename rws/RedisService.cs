using System;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;
using System.Timers;
using System.Text;
using System.Net.Sockets;

namespace rws
{
    public partial class RedisService : ServiceBase
    {
        private RedisProcess process;
        private Timer timer;

        public RedisService()
        {
            InitializeComponent();
            process = new RedisProcess(AppDomain.CurrentDomain.BaseDirectory);
            timer = new Timer();
            timer.Elapsed += (sender, args) =>
            {
                if (process.IsStop())
                {
                    Logger.Log("Redis Restart");
                    process.Start();
                }
            };
            timer.Interval = 2000;
        }

        protected override void OnStart(string[] args)
        {
            Logger.Log("Redis Start");
            process.Start();
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
            process.Stop();
            Logger.Log("Redis Stop");
        }
    }
}
