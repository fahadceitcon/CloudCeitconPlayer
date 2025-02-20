using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;

namespace Ceitcon_Service
{
    public class CalculatorWindowsService : ServiceBase
    {
        public ServiceHost serviceHost = null;
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Thread _thread;

        public CalculatorWindowsService()
        {
            // Name the Windows Service
            ServiceName = "CeitconServer";
        }

        public static void Main()
        {
            ServiceBase.Run(new CalculatorWindowsService());
        }

        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }
            serviceHost = new ServiceHost(typeof(CeitconServer));
            serviceHost.Open();

            //StartMQTTServer
            //StartMQTTServer();

            //Start clear log thread 
            _thread = new Thread(WorkerThreadFunc);
            _thread.Name = "Clear Old Log Thread";
            _thread.IsBackground = true;
            _thread.Start();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }

            _shutdownEvent.Set();
            if (!_thread.Join(3000))
            { 
                // give the thread 3 seconds to stop
                _thread.Abort();
            }
        }
        //private void StartMQTTServer()
        //{
        //    try
        //    {
        //        string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "emqttd", "bin", "startconsole.bat");
        //        if (File.Exists(path))
        //        {
        //            ProcessStartInfo startInfo = new ProcessStartInfo();
        //            startInfo.FileName = path;
        //            Process.Start(startInfo);
        //        }
        //        else
        //        {
        //            path = Path.Combine(@"C:\emqttd\bin", "startconsole.bat");
        //            if (File.Exists(path))
        //            {
        //                ProcessStartInfo startInfo = new ProcessStartInfo();
        //                startInfo.FileName = path;
        //                Process.Start(startInfo);
        //            }
        //        }
        //    }
        //    catch(Exception) { }
        //}

        private void WorkerThreadFunc()
        {
            while (true)
            {
                try
                {
                    string logDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "logs");
                    Console.WriteLine(String.Format("Clear old logs from {0}", logDirectory));
                    if (Directory.Exists(logDirectory))
                    {
                        var files = new DirectoryInfo(logDirectory).GetFiles("*.log");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.CreationTimeUtc > TimeSpan.FromDays(1))
                            {
                                File.Delete(file.FullName);
                            }
                        }
                    }
                }
                catch (Exception) { }
                Thread.Sleep(new TimeSpan(1, 0, 0, 0));
            }
        }
    }
}
