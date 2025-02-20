using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CeitconSetupInstaller
{
    [RunInstaller(true)]
    public partial class InstallerClass : System.Configuration.Install.Installer
    {
        public InstallerClass()
        {
            InitializeComponent();
        }
        
        /*
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            try
            {
                string path = this.Context.Parameters["assemblypath"];
                string root = Directory.GetParent(Path.GetDirectoryName(path)).FullName;
                string playermsi = Path.Combine(root, "Ceitcon_Player", "mysetup.msi").Replace(" ", "{Space}");
                string playerBat = Path.Combine(root, "Ceitcon_Player", "CeitconBat.exe");

                if (File.Exists(playerBat))
                {
                    if (System.IO.File.Exists(@"C:\ctemp.txt") == false)
                        System.IO.File.Create(@"C:\ctemp.txt");
                    else
                    {
                        System.IO.File.Delete("C:\\ctemp.txt");
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                        System.IO.File.Create(@"C:\ctemp.txt");
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                        System.IO.File.WriteAllText(@"C:\ctemp.txt", playermsi);
                    }

                   // Process process = Process.Start(playerBat, playermsi);

                   // process.WaitForInputIdle();
                    //Thread.Sleep(30000);

                    //string sDoubleQuote = "\"";
                    //string sFileNameRun = @" /i " + sDoubleQuote + player + sDoubleQuote + " /quiet /norestart ADDLOCAL=" + sDoubleQuote + "Server" + sDoubleQuote + " SERVER_REGISTER_AS_SERVICE=1 SERVER_ADD_FIREWALL_EXCEPTION=1 VIEWER_ADD_FIREWALL_EXCEPTION=1 SERVER_ALLOW_SAS=1 SET_USEVNCAUTHENTICATION=1 VALUE_OF_USEVNCAUTHENTICATION=1 SET_PASSWORD=1 VALUE_OF_PASSWORD=PASSWORD SET_USECONTROLAUTHENTICATION=1 VALUE_OF_USECONTROLAUTHENTICATION=1 SET_CONTROLPASSWORD=1 VALUE_OF_CONTROLPASSWORD=P@SSWORD@123!@##@!";
                    //Process process = new Process();
                    //process.StartInfo.FileName = player;// "msiexec";
                    //process.StartInfo.WorkingDirectory = "C:\\";// Path.Combine(root, "Ceitcon_Player"); //@"C:\";
                    //process.StartInfo.Arguments = playermsi;// sFileNameRun;// " /quiet /i Setup.msi ADDLOCAL=test";
                    //process.StartInfo.Verb = "runas";
                    //process.Start();
                    //process.WaitForExit(30000);
                }

            }
            catch (Exception)
            {
                //System.IO.File.WriteAllText(@"C:\Temp\TestError.txt", e.Message);
            }
        }*/

        //public override void Commit(IDictionary savedState)
        //{
        //    base.Commit(savedState);
        //}

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            try
            {
                string serviceName = "CeitconServer";
                string path = this.Context.Parameters["assemblypath"];
                string root = Directory.GetParent(System.IO.Path.GetDirectoryName(path)).FullName;
                string server = Path.Combine(root, "Ceitcon_Server", "Ceitcon_Service.exe");

                if (System.ServiceProcess.ServiceController.GetServices().Any(s => s.ServiceName == serviceName))
                {
                    UnInstallService(server);
                }

                System.IO.Directory.Delete(root, true);

            }
            catch (Exception)
            {
                //System.IO.File.WriteAllText(@"C:\Temp\TestError.txt", e.Message);
            }
        }

        public bool UnInstallService(string fullFileName)
        {
            try
            {
                KillProcess("Ceitcon_Player");
                KillProcess("Ceitcon_Designer");

                var domain = AppDomain.CreateDomain("MyDomain");

                using (AssemblyInstaller installer = domain.CreateInstance(typeof(AssemblyInstaller).Assembly.FullName, typeof(AssemblyInstaller).FullName, false, BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.ExactBinding, null, new object[] { fullFileName, new string[] { } }, null, null, null).Unwrap() as AssemblyInstaller)
                {
                    installer.UseNewContext = true;
                    installer.Uninstall(null);
                    return true;
                }
                AppDomain.Unload(domain);
            }
            catch (Exception)
            {
                return false;
            }
        }

        void KillProcess(string name)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(name))
                {
                    process.Kill();
                }
            }
            catch (Exception) { }
        }
    }
}
