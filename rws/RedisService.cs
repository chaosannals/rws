using System;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;

namespace rws
{
    public partial class RedisService : ServiceBase
    {
        private Process process;

        public RedisService()
        {
            InitializeComponent();
            string here = AppDomain.CurrentDomain.BaseDirectory;
            process = new Process();
            process.StartInfo.FileName = Path.Combine(here, "redis-server.exe");
            process.StartInfo.WorkingDirectory = here;
            process.StartInfo.Arguments = "";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += new DataReceivedEventHandler(Output);
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Redis Start");
            process.Start();
            process.BeginOutputReadLine();
        }

        protected override void OnStop()
        {
            process.Kill();
            process.Close();
            WriteLog("Redis Stop");
        }

        private void Output(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                WriteLog(args.Data);
            }
        }

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
    }
}
