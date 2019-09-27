using System;
using System.Collections;
using System.ServiceProcess;
using System.Configuration.Install;
using System.IO;

namespace rwsmgr
{
    class Program
    {
        static void Main(string[] args)
        {
            string name = "RedisService";
            string here = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(here, "rws.exe");
            if (ExistService(name))
            {
                StopService(name);
                UnistallService(path);
            } else
            {
                InstallService(path);
                StartService(name);
            }
        }

        private static bool ExistService(string name)
        {
            ServiceController[] controllers = ServiceController.GetServices();
            foreach(ServiceController controller in controllers)
            {
                if (controller.ServiceName.ToLower() == name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private static void InstallService(string path)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = path;
                IDictionary savedState = new Hashtable();
                installer.Install(savedState);
                installer.Commit(savedState);
            }
        }

        private static void UnistallService(string path)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = path;
                installer.Uninstall(null);
            }
        }

        private static void StartService(string name)
        {
            using (ServiceController controller = new ServiceController(name))
            {
                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    controller.Start();
                }
            }
        }

        private static void StopService(string name)
        {
            using (ServiceController controller = new ServiceController(name))
            {
                if (controller.Status == ServiceControllerStatus.Running)
                {
                    controller.Stop();
                }
            }
        }
    }
}
