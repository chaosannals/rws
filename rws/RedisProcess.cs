using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace rws
{
    public class RedisProcess
    {
        public string Here { get; private set; }
        public Process Redis { get; private set; }

        public RedisProcess(string here)
        {
            Here = here;
            Redis = null;
        }

        public void Start()
        {
            Redis = new Process();
            Redis.StartInfo.FileName = Path.Combine(Here, "redis-server.exe");
            Redis.StartInfo.WorkingDirectory = Here;
            Redis.StartInfo.Arguments = Configure(Here);
            Redis.StartInfo.CreateNoWindow = true;
            Redis.StartInfo.UseShellExecute = false;
            Redis.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Redis.StartInfo.RedirectStandardOutput = true;
            Redis.StartInfo.RedirectStandardError = true;
            Redis.EnableRaisingEvents = true;
            Redis.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Logger.Log("Redis: {0}", args.Data);
                }
            };
            Redis.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Logger.Log("Redis Error: {0}", args.Data);
                }
            };
            Redis.Start();
            Redis.BeginOutputReadLine();
        }

        public void Stop()
        {
            Logger.Log(Save());
            Redis.Kill();
            Redis.Close();
            Redis = null;
        }

        public bool IsStop()
        {
            return Redis == null || Redis.HasExited;
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
