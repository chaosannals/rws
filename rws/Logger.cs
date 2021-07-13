using System;
using System.Collections.Generic;
using System.Timers;
using System.IO;
using System.Text;

namespace rws
{
    public static class Logger
    {
        public static string Folder { get; private set; }
        public static string Suffix { get; private set; }
        public static Timer Ticker { get; private set; }
        private static List<string> contents = new List<string>();

        public static void Init(string suffix="log")
        {
            if (Folder != null || Ticker != null || Suffix != null)
            {
                throw new Exception("重复初始化日志");
            }
            Suffix = suffix;
            Folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
            Ticker = new Timer();
            Ticker.Elapsed += (sender, args) =>
            {
                try { Write(); } catch { }
            };
            Ticker.Interval = 2000;
            Ticker.Start();
        }

        public static void Quit()
        {
            Ticker.Stop();
            Write();
        }

        /// <summary>
        /// 打印日志。
        /// </summary>
        /// <param name="content"></param>
        /// <param name="args"></param>
        public static void Log(string content, params object[] args)
        {
            string text = string.Format(
                "[{0:S}] - {1:S}\r\n",
                DateTime.Now.ToString("F"),
                string.Format(content, args)
            );
            lock (contents)
            {
                contents.Add(text);
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        private static void Write()
        {
            lock (contents)
            {
                if (contents.Count == 0) return;

                string date = DateTime.Now.ToString("yyyyMMdd");
                string path = Path.Combine(Folder, string.Format("{0:S}.{1}", date, Suffix));

                // 大于 2M 的文件先搬移
                FileInfo info = new FileInfo(path);
                if (info.Exists && info.Length > 2000000)
                {
                    string time = DateTime.Now.ToString("HHmmss");
                    info.MoveTo(Path.Combine(Folder, string.Format("{0:S}-{1:S}.{2}", date, time, Suffix)));
                }

                // 写入日志
                using (FileStream stream = File.Open(path, FileMode.OpenOrCreate | FileMode.Append))
                {
                    foreach (string text in contents)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(text);
                        stream.Write(data, 0, data.Length);
                    }
                    contents.Clear();
                }
            }
        }
    }
}
