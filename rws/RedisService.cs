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
        private Process process;
        private Timer timer;

        public RedisService()
        {
            InitializeComponent();
            string here = AppDomain.CurrentDomain.BaseDirectory;
            process = new Process();
            process.StartInfo.FileName = Path.Combine(here, "redis-server.exe");
            process.StartInfo.WorkingDirectory = here;
            process.StartInfo.Arguments = Configure(here);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += new DataReceivedEventHandler(Output);
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(Ensure);
            timer.Interval = 2000;
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Redis Start");
            process.Start();
            process.BeginOutputReadLine();
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
            WriteLog(Save());
            process.Kill();
            process.Close();
            WriteLog("Redis Stop");
        }

        private void Ensure(object sender, ElapsedEventArgs args)
        {
            if (process.HasExited)
            {
                WriteLog("Redis Restart");
                process.Start();
            }
        }

        /// <summary>
        /// 写入日志。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Output(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                WriteLog(args.Data);
            }
        }

        /// <summary>
        /// 配置。
        /// </summary>
        /// <param name="here"></param>
        /// <returns></returns>
        private string Configure(string here)
        {
            string path = Path.Combine(here, "redis.conf");
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                return path;
            }
            return "";
        }

        /// <summary>
        /// 日志写入。
        /// </summary>
        /// <param name="msg"></param>
        private void WriteLog(string msg)
        {
            string here = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(here, "service.log");
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                File.Create(path).Close();
            }
            using (FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(DateTime.Now.ToString() + "   " + msg);
                }
            }
        }

        /// <summary>
        /// Redis 命令落盘保存。
        /// </summary>
        /// <returns></returns>
        private string Save()
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 6379))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] saveCommand = Encoding.UTF8.GetBytes("SAVE\r\n");
                    stream.Write(saveCommand, 0, saveCommand.Length);
                    byte[] buffer = new byte[1024];
                    int c = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, c);
                }
            }
        }
    }
}
