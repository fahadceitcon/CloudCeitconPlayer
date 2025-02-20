using Ceitcon_Downloader.Utilities;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using log4net;
using System.Globalization;
using System.ServiceProcess;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net.Repository.Hierarchy;

namespace Ceitcon_Downloader
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static readonly string PlayerProcessName = "Ceitcon_Player";
        static readonly string playerText = "Player";
        static string PlayerPath = Path.Combine(Directory.GetCurrentDirectory(), @"CPlayer\Ceitcon_Player.exe");
        static string LogoPath = Path.Combine(Directory.GetCurrentDirectory(), "Logos");
        static TimeSpan Interval = new TimeSpan(0, 1, 0);
        static TimeSpan StatusInterval = new TimeSpan(0, 0, 30);
        static string[] WeatherLocation;
        static string MediaDirectory = String.Empty;
        static string PlayerName = String.Empty;
        static string PlayerGroup = "Player Group";
        static string HostName = String.Empty;
        static string IpAddress = String.Empty;
        static string ServerIP = String.Empty;//"192.168.0.1(localhost)" Or Server Host Name 
        static int ServerPort = 8000;
        static string mediaText = "Media";
        static string dataFileName = "FID.xml";
        static string weatherFileName = "_Weather.xml";
        static string projectSeverFolder = "Upload";
        static string logoServerFolder = "Logos";
        static int playerStatus = 0;
        //static List<string> FileNames = new List<string>();

        //Statuses
        static bool canDownload = false;
        static bool updateStatusStarted = false;

        #region Properties
        static string GetServerUrl()
        {
            return String.Format(@"http://{0}:{1}/CeitconServer/service", ServerIP, ServerPort);
            //return String.Format(@"http://{0}:8733/Design_Time_Addresses/CeitconServer/Service1/", serverUrl);
        }
        #endregion

        #region SleepCancel
        static bool canSleep = true;
        static bool RestartPlay = true;
        static void WaitOrCancel(TimeSpan waitInterval)
        {
            canSleep = true;
            TimeSpan ts = new TimeSpan(0);
            log.Info($"WaitOrCanel is called:{waitInterval.ToString()}");
            Console.WriteLine($"Wait Interval:{waitInterval.ToString()}");


            while (canSleep)
            {
                if (ts > waitInterval)
                {
                    log.Info($"ts: {ts.ToString()} and waitinterval:{waitInterval.ToString()}");
                    break;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
                ts = ts.Add(TimeSpan.FromSeconds(1));
            }
        }
        #endregion

        #region Exit event
        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        static bool ConsoleEventCallback(int eventType)
        {
            log.Info($"eventType:{eventType.ToString()}");
            if (eventType == 2)
            {
                // DisconnectSignalR();
                log.Info($"PlayerName:{PlayerName}");
                if (!String.IsNullOrEmpty(PlayerName))
                {
                    playerStatus = 5;
                    CeitconServerHelper.StopPlayer(PlayerName);
                }
                log.Info($"PlayerProcessName:{PlayerProcessName}");
                KillProcess(PlayerProcessName);
                log.Info("Exit");
                Console.WriteLine("Exit");
            }
            return false;
        }
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("Ceitcon Downloader");
            log.Info("Ceitcon Downloader");

            SetProcessDPIAware();
            InstallTightVNC();
            while (true)
            {
                if (!Init())
                {
                    //break;


                }
                else
                {
                    ConnectToSignalR();

                    if (!updateStatusStarted)
                        StartStatusConnection();

                    canDownload = true;
                    DownloadContent();
                    canDownload = false;
                }
            }
        }

        static bool Init()
        {
            WriteResolution();

            //Exit event
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            MediaDirectory = ConfigurationManager.AppSettings["CeitconDirectory"];
            if (!String.IsNullOrWhiteSpace(MediaDirectory) && !Directory.Exists(MediaDirectory))
                Directory.CreateDirectory(MediaDirectory);

            Console.WriteLine("Clear old log files...");
            log.Info("Clear old log files...");
            ClearLogs(1);

            //============= Server Name ================
            Console.WriteLine(String.Format("Loading Server name..."));
            log.Info("Loading Server name...");
            ServerIP = SQLiteHelper.Instance.GetApplication("Server");
            PlayerName = SQLiteHelper.Instance.GetApplication("Player");
            log.Info(String.Format("ServerIP: {0}", ServerIP));
            if (String.IsNullOrEmpty(ServerIP))
            {
                Console.WriteLine("Please insert Server IP or Server Name and press Enter key: ");
                while (true)
                {
                    ServerIP = Console.ReadLine();
                    if (String.IsNullOrEmpty(ServerIP))
                    {
                        Console.WriteLine(String.Format("Empty server string, please try again:"));
                        continue;
                    }
                    else
                    {
                        CeitconServerHelper.Address = GetServerUrl();
                        if (!CeitconServerHelper.TestConnection())
                        {
                            Console.WriteLine(String.Format("Server is not correct or server is not in function, please try again:"));
                            continue;
                        }
                    }
                    log.Info($"172:Server:{ServerIP}");
                    SQLiteHelper.Instance.UpdateApplication("Server", ServerIP);
                    break;
                }
            }
            else
            {
                CeitconServerHelper.Address = GetServerUrl();
            }
            //string IPAddress = ConfigurationManager.AppSettings["LocalIP"];
            Console.WriteLine(String.Format("Server IP / Name is: {0} . To reset settings delete CeitconDownload.db", ServerIP));
            log.Info(String.Format("Server IP / Name is: {0} . To reset settings delete Ceitcon.db", ServerIP));
            //MQTTServerIP = ServerIP;

            //============== Get host name / ip4 ===================
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                HostName = host.HostName;
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IpAddress = ip.ToString();
                    }
                }
            }

            Console.WriteLine(String.Format("local ip Address : {0}", IpAddress));
            log.Info(String.Format("Total ip Address: {0}", IpAddress));
            //=================== Licences counter ====================
            int totalScreens = System.Windows.Forms.Screen.AllScreens.Count();
            Console.WriteLine(String.Format("Total screens: {0}", totalScreens));
            log.Info(String.Format("Total screens: {0}", totalScreens));
            int licence = 0;// (totalScreens / 2) + (totalScreens % 2);
            //if (totalScreens <= 2)
            //{
            //    licence = 0;
            //}
            //else
            //{
            //    licence = totalScreens - 2;
            //}
            //Console.WriteLine(String.Format("Total Screen Connected with player: {0}", licence));
            //log.Info(String.Format("Total licences: {0}", licence));

            Console.WriteLine("Player registration...");
            log.Info("Player registration...");

            // Remove ar-SA date time culture
            CultureInfo _cinfor = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (_cinfor.Name.ToLower() == "ar-sa")
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            //=================== Get Online Player Name ================
            if (!string.IsNullOrEmpty(PlayerName))
            {
                string response = CeitconServerHelper.GetPlayerNameInfo(PlayerName, HostName, IpAddress);
                log.Info($"1. GetPlayerNameInfo Response:{response}");

                if (response == "Player already registerd with another IP Address")
                {
                    Console.WriteLine(response);
                    return false;
                }
                else if (response == "Player was unregistered from the server, try to input the player name again")
                {

                    Unregistrate();
                    Console.WriteLine("Run the application again");
                    return false;
                }
                //else if (string.IsNullOrEmpty(response))
                //{
                //    Console.WriteLine($"This player { PlayerName } entry does not found on the server ");
                //    return false;
                //}

                while (response == "Error")
                {

                    Console.WriteLine("Problem with connection on server...");
                    log.Info("Problem with connection on server...");
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                    GetIPAddress();
                    response = CeitconServerHelper.GetPlayerNameInfo(PlayerName, HostName, IpAddress);
                    log.Info($"2. GetPlayerNameInfo Response:{response}");
                    if (response == "Error")
                    {
                        string scheduler = SQLiteHelper.Instance.GetApplication(mediaText);
                        if (String.IsNullOrEmpty(scheduler))
                        {
                            Console.WriteLine("No content, close player...");
                            log.Info("No content, close player...");
                            return false;
                        }
                        else if (scheduler.ToLower() == "no data")
                        {
                            Console.WriteLine("No content, close player...");
                            log.Info("No content, close player...");
                            return false;
                        }
                        else if (scheduler.Contains(",") == false)
                        {
                            Console.WriteLine("No content, close player...");
                            log.Info("No content, close player...");
                            return false;
                        }
                        string[] sch = scheduler.Split(',');
                        DateTime start = DateTime.ParseExact(sch[3], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end = DateTime.ParseExact(sch[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        if (start <= DateTime.Now && end >= DateTime.Now)
                        {
                            //string s = @"Domain2/Country2/Region2/location2/locationgroup2/floors2/zones2/zone groups2/faces2,Project_c3f402.zip,2017-05-01 12:30:00,2017-06-09 14:30:00";
                            if (Process.GetProcessesByName(PlayerProcessName).Count() == 0)
                            {

                                Console.WriteLine(String.Format("Try start Player with old data: {0}", PlayerPath));
                                log.Info(String.Format("Try start Player with old data: {0}", PlayerPath));
                                RunAsync(PlayerPath);
                                Console.WriteLine(String.Format("Player Started with old data"));
                            }
                        }
                        else
                        {
                            Console.WriteLine("No content, close player...");
                            log.Info("No content, close player...");
                            return false;
                        }
                        //return false;
                    }
                    else
                    {
                        if (Process.GetProcessesByName(PlayerProcessName).Count() > 0)
                        {
                            KillProcess(PlayerProcessName);
                        }
                        break;
                    }
                }
                GetIPAddress();
                response = CeitconServerHelper.GetPlayerName(HostName, IpAddress);
                if (response == "Error")
                {
                    Console.WriteLine("Problem with connection on server...");
                    log.Info("Problem with connection on server...");
                    return false;
                }
                SetPlayerGroup(response);

                log.Info($"320:playerText:{playerText}, PlayerName:{PlayerName}");
                SQLiteHelper.Instance.UpdateApplication(playerText, PlayerName);
            }
            //=================== Player Name =======================
            if (String.IsNullOrEmpty(PlayerName))
            {
                Console.WriteLine("Please insert Player name and press Enter key: ");
                while (true)
                {
                    PlayerName = Console.ReadLine();
                    if (String.IsNullOrEmpty(PlayerName))
                    {
                        Console.WriteLine(String.Format("Empty Player name, please try again:"));
                        continue;
                    }
                    else if (!CeitconServerHelper.CheckPlayerName(PlayerName))
                    {
                        Console.WriteLine(String.Format("Player name does not exist on server, please try again:"));
                        log.Info(String.Format("Player name does not exist on server, please try again: {0}", PlayerName));
                        continue;
                    }

                    //Free Licence
                    Console.WriteLine("Check Free Licence...");
                    if (!CeitconServerHelper.CheckFreeLicence(licence))
                    {
                        playerStatus = 2;
                        Console.WriteLine("Total player registed is equal to licensed player");
                        log.Info("Total player registed is equal to licensed player");
                        Console.ReadKey();
                        return false;
                    }

                    //Registry Player
                    int screens = System.Windows.Forms.Screen.AllScreens.Count();
                    int defaultlic = 1;
                    GetIPAddress();
                    if (CeitconServerHelper.RegistratePlayer(PlayerName, HostName, IpAddress, screens, defaultlic, 1))
                    {
                        playerStatus = 1;
                        log.Info($"359:playerText:{playerText}, PlayerName:{PlayerName}");
                        SQLiteHelper.Instance.UpdateApplication(playerText, PlayerName);
                        Console.WriteLine("Player is registred");
                        log.Info("Player is registred");
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Player is already registered on another IPAddress"));
                        Console.ReadKey();
                        return false;
                    }
                    break;
                }
            }
            else
            {
                GetIPAddress();
                if (!CeitconServerHelper.CheckPlayerExist(PlayerName, HostName, IpAddress, licence))
                {
                    Console.WriteLine(String.Format("Player is already registered on another IPAddress"));
                    Console.ReadKey();
                    return false;
                }
            }
            Console.WriteLine(String.Format("Player name: {0} and path: {1}", PlayerName, PlayerPath));
            log.Info(String.Format("Player name: {0} and path: {1}", PlayerName, PlayerPath));



            return true;
        }

        static void DownloadContent()
        {
            Console.WriteLine(String.Format("Start CeitconPlayerDownloader {0}", DateTime.Now));
            log.Info(String.Format("Start CeitconPlayerDownloader {0}", DateTime.Now));

            if (!Directory.Exists(MediaDirectory))
                Directory.CreateDirectory(MediaDirectory);

            //Remove ar-SA date time culture
            CultureInfo _cinfor = Thread.CurrentThread.CurrentCulture;
            if (_cinfor.Name.ToLower() == "ar-sa")
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            while (true)
            {
                if (!canDownload)
                {
                    WaitOrCancel(Interval);
                    continue;
                }

                try
                {
                    Console.WriteLine(String.Format("Checking for new project... {0}", DateTime.Now));

                    #region "Restart Player"
                    Console.WriteLine($"Time of the Day:{DateTime.Now.TimeOfDay.Hours.ToString()} Mints:{DateTime.Now.Minute.ToString()}");
                    string sRestartTime = "";
                    int _Hour = 2;
                    int _Mints = 00;
                    try
                    {
                        sRestartTime = ConfigurationManager.AppSettings["PlayerRestartTime"];
                        Console.WriteLine($"Restart Time:{sRestartTime}");
                        string[] sTime = sRestartTime.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (sTime.Length > 0)
                        {
                            _Hour = Convert.ToInt32(sTime[0]);
                            _Mints = Convert.ToInt32(sTime[1]);
                            Console.WriteLine($"Hours:{_Hour}");
                            Console.WriteLine($"Mints:{_Mints}");
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                        log.Info("Error:" + ex.Message);
                    }


                    if (DateTime.Now.Hour == _Hour)
                    {
                        log.Info($"RestartPlayer:{RestartPlay.ToString()}");
                        if (RestartPlay)
                        {

                            if (DateTime.Now.Minute >= _Mints)
                            {
                                Console.WriteLine("I am In");
                                RestartPlay = false;
                                KillProcess2(PlayerProcessName);


                            }
                            else
                            {
                                Console.WriteLine("DateTime.Now.Minute");
                                Console.WriteLine(DateTime.Now.Minute);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("DateTime.Now.Hour");
                        Console.WriteLine(DateTime.Now.Hour);
                        RestartPlay = true;
                        log.Info("Making Restart Player Again to true");
                    }
                    #endregion

                    log.Info(String.Format("Checking for new project... {0}", DateTime.Now));

                    string scheduler = CeitconServerHelper.GetSceduler(PlayerName);
                    //string scheduler = @"Network/Domain/Country/Region/Location Group/Location/Floor/Zone/Player Group,b465b133-a1b8-4bd5-a66b-02508031bdc0.cdp,3,2017-09-30 00:00:00,2017-10-01 00:00:00";
                    if (!String.IsNullOrEmpty(scheduler) && scheduler.Length > 150)
                    {
                        Console.WriteLine($"Scheduler {scheduler.Substring(0, 150)} ...");
                        log.Info($"Scheduler {scheduler.Substring(0, 100)} ...");
                    }

                    if (String.IsNullOrEmpty(scheduler))
                    {
                        WaitOrCancel(Interval);
                        continue;
                    }

                    if (scheduler == "No data")
                    {
                        Console.WriteLine("No Content...");
                        log.Info("No Content...");
                        playerStatus = 1; //downloader is running but no content to show which mean the player is not running
                        KillProcess(PlayerProcessName);
                        ClearDirectory(true);
                        log.Info($"434:mediaText:{mediaText}, scheduler:{scheduler}");
                        SQLiteHelper.Instance.UpdateApplication(mediaText, scheduler);
                        WaitOrCancel(Interval);
                        continue;
                    }

                    if (scheduler.StartsWith("License"))
                    {
                        Console.WriteLine(scheduler);
                        log.Info(scheduler);
                        playerStatus = 2; // mean that the downloader is running but the player is not running becoz it was unregisterd from the server 
                        KillProcess(PlayerProcessName);
                        ClearDirectory(true);
                        log.Info($"447:mediaText:{mediaText}, scheduler:{scheduler}");
                        SQLiteHelper.Instance.UpdateApplication(mediaText, scheduler);
                        WaitOrCancel(Interval);
                        continue;
                    }

                    //string s = @"Domain2/Country2/Region2/location2/locationgroup2/floors2/zones2/zone groups2/faces2,Project_c3f402.zip,2,2017-05-01 12:30:00,2017-06-09 14:30:00";
                    string[] sch = scheduler.Split(',');
                    if (sch.Count() != 7 || String.IsNullOrEmpty(sch[0]) || String.IsNullOrEmpty(sch[1]) || String.IsNullOrEmpty(sch[6]))
                    {
                        WaitOrCancel(Interval);
                        continue;
                    }

                    playerStatus = 3; //mean the downloader and player both are running

                    string path = Path.Combine(MediaDirectory, Path.GetFileNameWithoutExtension(sch[1]) + ".cdp");
                    string text = SQLiteHelper.Instance.GetApplication(mediaText);

                    if (!String.IsNullOrEmpty(text) && scheduler == text && File.Exists(path))
                    {
                        Console.WriteLine("Server and local content are same.");
                        log.Info("Server and local content are same.");

                        // Weather
                        WeatherLocation = Ceitcon_Data.Utilities.IOManagerProject.GetWeatherLocations(path);
                        WeatherFunc();

                        //========== Content is already downloaded, check maybe not complited
                        string xmlPath = Path.Combine(MediaDirectory, sch[1]);
                        string[] list = Ceitcon_Data.Utilities.IOManagerProject.GetContents(xmlPath);
                        foreach (string item in list)
                        {
                            int v = item.IndexOf(";");
                            string file = item.Substring(0, v);
                            long size = Convert.ToInt64(item.Substring(v + 1));

                            if (!File.Exists(Path.Combine(MediaDirectory, file))
                                || (new System.IO.FileInfo(Path.Combine(MediaDirectory, file)).Length) != size)
                            {
                                //Download from server
                                string fileUrl = Path.Combine(projectSeverFolder, sch[0].Replace("/", @"\"), file);
                                string filePath = Path.Combine(MediaDirectory, file);
                                CeitconServerHelper.Download(fileUrl, filePath);
                            }
                        }

                        //if (CheckContentChange(list))
                        //{
                        //    Console.WriteLine("Content Changed In project...");
                        //    log.Info("Content Changed In project...");

                        //    //Clear old cdp
                        //    ClearProjectFiles();
                        //    log.Info("Content Changed 2");
                        //    File.WriteAllText(xmlPath, sch[6]);
                        //    log.Info("Content Changed 3");
                        //    if (File.Exists(xmlPath))
                        //    {
                        //        log.Info("Content Changed 4");
                        //        // Interval
                        //        Interval = Ceitcon_Data.Utilities.IOManagerProject.GetInterval(xmlPath);
                        //        Console.WriteLine(String.Format("Apply check interval [s]: {0}", Interval.TotalSeconds));
                        //        log.Info(String.Format("Apply check interval [s]: {0}", Interval.TotalSeconds));

                        //        // Weather
                        //        WeatherLocation = Ceitcon_Data.Utilities.IOManagerProject.GetWeatherLocations(xmlPath);
                        //        WeatherFunc();

                        //        //Get content
                        //        List<string> clearList = new List<string>();
                        //        clearList.Add(Path.GetFileName(xmlPath));
                        //        FileNames.Clear();
                        //        foreach (string item in list)
                        //        {

                        //            int v = item.IndexOf(";");
                        //            string file = item.Substring(0, v);
                        //            long size = Convert.ToInt64(item.Substring(v + 1));
                        //            clearList.Add(file);
                        //            FileNames.Add(file);
                        //            log.Info("Content Changed 5: " + file);
                        //        }

                        //        //Clear old files
                        //        ClearOtherFies(clearList.ToArray());
                        //        log.Info($"550:mediaText:{mediaText}, scheduler:{scheduler}");
                        //        SQLiteHelper.Instance.UpdateApplication(mediaText, scheduler);
                        //        StartPlayer();
                        //    }
                        //}
                        //else
                        //{
                        StartPlayer();

                        WaitOrCancel(Interval);
                        continue;
                        //}
                    }
                    else
                    {
                        //============ Download new project ===============
                        Console.WriteLine("Downloading project...");
                        log.Info("Downloading project...");

                        //Clear old cdp
                        ClearProjectFiles();

                        //Save cdp
                        string xmlPath = Path.Combine(MediaDirectory, sch[1]);
                        File.WriteAllText(xmlPath, sch[6]);

                        if (File.Exists(xmlPath))
                        {
                            // Interval
                            Interval = Ceitcon_Data.Utilities.IOManagerProject.GetInterval(xmlPath);
                            Console.WriteLine(String.Format("Apply check interval [s]: {0}", Interval.TotalSeconds));
                            log.Info(String.Format("Apply check interval [s]: {0}", Interval.TotalSeconds));

                            // Weather
                            WeatherLocation = Ceitcon_Data.Utilities.IOManagerProject.GetWeatherLocations(xmlPath);
                            WeatherFunc();

                            //Get content
                            string[] list = Ceitcon_Data.Utilities.IOManagerProject.GetContents(xmlPath);
                            List<string> clearList = new List<string>();
                            clearList.Add(Path.GetFileName(xmlPath));
                            //FileNames.Clear();
                            foreach (string item in list)
                            {
                                int v = item.IndexOf(";");
                                string file = item.Substring(0, v);
                                long size = Convert.ToInt64(item.Substring(v + 1));
                                clearList.Add(file);
                                //FileNames.Add(file);
                                if (!File.Exists(Path.Combine(MediaDirectory, file))
                                    || (new FileInfo(Path.Combine(MediaDirectory, file)).Length) != size)
                                {
                                    //Download from server
                                    string fileUrl = Path.Combine(projectSeverFolder, sch[0].Replace("/", @"\"), file);
                                    string filePath = Path.Combine(MediaDirectory, file);

                                    Console.WriteLine(String.Format("Downloading file... {0}", file));
                                    log.Info(String.Format("Downloading file... {0}", file));
                                    CeitconServerHelper.Download(fileUrl, filePath);
                                }
                            }

                            //Clear old files
                            ClearOtherFies(clearList.ToArray());
                            log.Info($"550:mediaText:{mediaText}, scheduler:{scheduler}");
                            SQLiteHelper.Instance.UpdateApplication(mediaText, scheduler);

                            StartPlayer();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    log.Error(e);
                }
                WaitOrCancel(Interval);
            }
        }

        //private static bool CheckContentChange(string[] list)
        //{
        //    try
        //    {
        //        if (list.Count() != FileNames.Count)
        //            return true;
        //        else
        //        {
        //            foreach (string file in list)
        //            {
        //                int v = file.IndexOf(";");
        //                string oldfile = file.Substring(0, v);
        //                if (!FileNames.Contains(oldfile))
        //                    return true;
        //            }
        //            foreach (var file in FileNames)
        //            {
        //                if (!list.Contains(file))
        //                    return true;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return false;
        //}

        #region Private methods
        static void StartStatusConnection()
        {
            Console.WriteLine(String.Format("Starting Update Status Connection {0}", DateTime.Now));
            log.Info(String.Format("Starting Update Status Connection {0}", DateTime.Now));
            Thread worker = new Thread(UpdateStatusWorker);
            worker.IsBackground = true;
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();
            updateStatusStarted = true;
        }

        static void UpdateStatusWorker()
        {
            while (true)
            {
                CheckPlayerStatus();
                if (playerStatus == 3)
                {
                    WeatherFunc();
                    //CheckLogos();
                }
                CheckPlayerRefreshTime();
                Thread.Sleep(StatusInterval);
            }
        }

        static void CheckPlayerRefreshTime()
        {
            try
            {
                int response = CeitconServerHelper.GetPlayerRefresh(PlayerName);
                if (response >= 30 && response != StatusInterval.Seconds)
                {
                    StatusInterval = TimeSpan.FromSeconds(response);
                    Console.WriteLine($"Status refresh time changed: {response}");
                    log.Info($"Status refresh time changed: {response}");
                }
            }
            catch (Exception e)
            {
                log.Error("Check Player Refresh Time", e);
            }
        }

        static void CheckPlayerStatus()
        {
            try
            {
                GetIPAddress();
                string response = CeitconServerHelper.GetPlayerName(HostName, IpAddress);
                if (response == "Error")
                {
                    Console.WriteLine("Problem with connection on server...");
                    log.Info("Problem with connection on server...");
                    return;
                }
                SetPlayerGroup(response);

                log.Info($"624:playerText:{playerText}, PlayerName:{PlayerName}");
                SQLiteHelper.Instance.UpdateApplication(playerText, PlayerName);

                if (String.IsNullOrEmpty(PlayerName))
                {
                    Console.Write($"1 . Player name {PlayerName}before unregistering it");
                    Unregistrate();
                    return;
                }

                Console.Write($"1 . Player Status {playerStatus.ToString()}");

                if (playerStatus == 1 || playerStatus == 3)
                    canDownload = true; // Continue downloading

                if (playerStatus != 0) // unknowns status, only on start before first pass
                {
                    int totalScreens = System.Windows.Forms.Screen.AllScreens.Count();
                    int _status = playerStatus == 4 ? 3 : playerStatus;

                    Console.Write($"2 . Player _status {_status.ToString()}");

                    int licence = CeitconServerHelper.UpdatePlayerStatus(PlayerName, totalScreens, _status);
                    Console.WriteLine($"Updated player status {_status}");//. Confirmed licence {licence}");
                    if (licence == 4) // mean if the status is disconnected from the server/Designer
                    {
                        Unregistrate();
                    }
                    else
                    {
                        StartPlayer();
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Check Player Status", e);
            }
        }

        static bool StartPlayer()
        {
            if (Process.GetProcessesByName(PlayerProcessName).Count() > 0)
                return false;

            if (playerStatus == 1)
            {
                Console.WriteLine(String.Format("Player can'not started as no content is found : {0}", PlayerPath));
                canDownload = true;
                return false;
            }
            //Start player
            Console.WriteLine(String.Format("Start Player: {0}", PlayerPath));
            log.Info(String.Format("Start Player: {0}", PlayerPath));
            RunAsync(PlayerPath);
            canDownload = true;
            Console.WriteLine(String.Format("Player Started"));

            return true;
        }

        static void Unregistrate()
        {
            try
            {
                playerStatus = 4;
                PlayerName = String.Empty;
                log.Info($"691:playerText:{playerText}, PlayerName:{PlayerName}");
                SQLiteHelper.Instance.UpdateApplication(playerText, PlayerName);
                KillProcess(PlayerProcessName); // Kill player
                canDownload = false; // Paussed downloading
                Console.WriteLine("Player is unregistred from server");
                log.Info("Player is unregistred from server. Please contact administrator.");
                return;
            }
            catch (Exception e)
            {
                log.Error("Check Player Status", e);
            }
        }

        static void WeatherFunc()
        {
            log.Info("Checking weather");
            try
            {
                if (WeatherLocation != null && WeatherLocation.Count() > 0)
                {
                    log.Info($"WeatherLocation Count: {WeatherLocation.Count().ToString()}");
                    foreach (string item in WeatherLocation)
                    {
                        try
                        {
                            string weatherText = CeitconServerHelper.GetWeathers(item);
                            if (!String.IsNullOrEmpty(weatherText))
                            {
                                string path = Path.Combine(MediaDirectory, weatherFileName);
                                //string extractPath = Path.Combine(MediaDirectory + "_New", item + weatherFileName);
                                string extractPath = Path.Combine(MediaDirectory, item + weatherFileName);
                                if (File.Exists(extractPath))
                                    File.Delete(extractPath);
                                Thread.Sleep(TimeSpan.FromSeconds(2));
                                File.WriteAllText(extractPath, weatherText);
                                if (File.Exists(extractPath))
                                {
                                    File.Copy(extractPath, path, true);
                                }
                                Console.WriteLine($"Update Weather Data {DateTime.Now}");
                                log.Info($"Update Weather Data {DateTime.Now}");
                            }
                            else
                            {
                                Console.WriteLine($"No Weather Data {DateTime.Now}");
                                log.Info($"No Weather Data {DateTime.Now}");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error - Update Weather Worker");
                            log.Error("Update Weather Worker", e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Weather update stoped");
                log.Error("Weather update stoped", ex);
            }
        }

        static void CheckLogos()
        {
            Console.WriteLine("Check Logos");
            log.Info("Check Logos");
            try
            {
                //if(DataGridCount > 0)
                string response = CeitconServerHelper.GetLogos();
                if (!String.IsNullOrEmpty(response))
                {
                    string[] list = response.Split(';');

                    foreach (var item in list)
                    {
                        string file = String.Format("{0}.png", item.Substring(0, 2));
                        long size = Convert.ToInt64(item.Substring(2));

                        string fileUrl = Path.Combine(logoServerFolder, file);
                        string filePath = Path.Combine(LogoPath, file);
                        string fileTempPath = Path.Combine(LogoPath, String.Format("{0}_.png", item.Substring(0, 2)));
                        if (File.Exists(fileTempPath))
                            File.Delete(fileTempPath);

                        long l = (new FileInfo(filePath)).Length;
                        if (!File.Exists(filePath) || new FileInfo(filePath).Length != size)
                        {
                            Console.WriteLine(String.Format("Download Logo File: {0}", file));
                            log.Info(String.Format("Download Logo File: {0}", file));
                            if (CeitconServerHelper.Download(fileUrl, fileTempPath))
                            {
                                if (File.Exists(filePath))
                                    File.Delete(filePath);
                                File.Move(fileTempPath, filePath);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Checklogo Weather Worker Error");
                log.Error("Update Weather Worker {0}", e);
            }
        }

        static void ClearProjectFiles()
        {
            log.Info("ClearProjectFiles Called 1...");
            var di = new DirectoryInfo(MediaDirectory);
            foreach (var file in di.GetFiles())
            {
                if (file.Extension == ".cdp")
                {
                    file.Delete();
                    log.Info("ClearProjectFiles Called 2.");
                }
            }
        }

        static void ClearDirectory(bool skipData = false)
        {
            var di = new DirectoryInfo(MediaDirectory);
            foreach (var file in di.GetFiles())
            {
                if (skipData && file.Name == dataFileName)
                    continue;
                try
                {
                    if (!IsFileLocked(file))
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception) { }
            }
            foreach (var dar in di.GetDirectories())
            {
                foreach (var file in dar.GetFiles())
                {
                    if (skipData && file.Name == dataFileName)
                        continue;
                    try
                    {
                        if (!IsFileLocked(file))
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        static void ClearOtherFies(string[] list)
        {
            var di = new DirectoryInfo(MediaDirectory);
            foreach (var file in di.GetFiles())
            {
                if (list.Contains(file.Name))
                    continue;
                try
                {
                    if (!IsFileLocked(file))
                    {
                        file.Delete();
                    }
                }
                catch (Exception) { }
            }
        }

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is: still being written to or being processed by another thread or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        static void RunAsync(string path)
        {
            Process process = null;
            try
            {
                process = new Process();
                ProcessStartInfo info = new ProcessStartInfo(path, "");
                process.StartInfo = info;

                process.Start();
            }
            catch (Exception)
            {
                if (process != null)
                    process.Dispose();
            }
            finally
            {
                if (process != null)
                    process.Dispose();
            }
        }

        static bool ProcessExist(string name)
        {
            return Process.GetProcessesByName(name).Count() > 0 ? true : false;
        }

        static void KillProcess(string name)
        {
            try
            {
                //name = PlayerPath;
                log.Info($"Player Name:{name}");
                //var d = Process.GetProcessesByName(name);
                log.Info($"PlayerPath:{PlayerPath}");
                foreach (var process in Process.GetProcessesByName(name))
                {
                    string sModuleFilePath = process.MainModule.FileName.Trim().ToLower();
                    log.Info($"process.MainModule.FileName:{sModuleFilePath}");
                    log.Info($"process.MainModule.FileName:{PlayerPath.Trim().ToLower()}");
                    if (PlayerPath.Trim().ToLower() == sModuleFilePath)
                    {
                        log.Info($"Killing :{name}");
                        Console.WriteLine($"Killing :{name}");
                        process.Kill();
                        log.Info($"Killed :{name}");
                        Console.WriteLine($"Killed :{name}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info($"Killing Error:{ex.Message}");
            }
        }

        static void KillProcess2(string name)
        {
            try
            {
                //name = PlayerPath;
                Console.WriteLine($"KillProcess2 Player Name:{name}");
                //var d = Process.GetProcessesByName(name);
                log.Info($" KillProcess2 PlayerPath:{PlayerPath}");
                Console.WriteLine($" KillProcess2 PlayerPath:{PlayerPath}");
                foreach (var process in Process.GetProcessesByName(name))
                {
                    string sModuleFilePath = process.MainModule.FileName.Trim().ToLower();
                    log.Info($"process.MainModule.FileName:{sModuleFilePath}");
                    Console.WriteLine($"process.MainModule.FileName:{sModuleFilePath}");
                    log.Info($"process.MainModule.FileName:{PlayerPath.Trim().ToLower()}");
                    Console.WriteLine($"process.MainModule.FileName:{PlayerPath.Trim().ToLower()}");
                    if (PlayerPath.Trim().ToLower() == sModuleFilePath)
                    {
                        log.Info($"Killing :{name}");
                        Console.WriteLine($"Killing :{name}");
                        process.Kill();
                        log.Info($"Killed :{name}");
                        Console.WriteLine($"Killed :{name}");
                    }
                }
                //Start Player Again
                if (Process.GetProcessesByName(PlayerProcessName).Count() == 0)
                {

                    Console.WriteLine($"Try start Player: {PlayerPath}");
                    log.Info($"Try start Player: {PlayerPath}");
                    RunAsync(PlayerPath);
                    Console.WriteLine($"Player Started: {PlayerPath}");
                    log.Info($"Player Started: {PlayerPath}");
                }
                else
                {
                    Console.WriteLine($"Already Running Player: {PlayerPath}");
                    log.Info($"Already Running Player: {PlayerPath}");
                }
            }
            catch (Exception ex)
            {
                log.Info($"Killing Error:{ex.Message}");
            }
        }

        static void ClearLogs(int days)
        {
            try
            {
                string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
                if (Directory.Exists(logDirectory))
                {
                    var files = new DirectoryInfo(logDirectory).GetFiles("*.log");
                    foreach (var file in files)
                    {
                        if ((DateTime.UtcNow - file.CreationTimeUtc).Days > days)
                        {
                            try
                            {
                                File.Delete(file.FullName);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        static void SetPlayerGroup(string text)
        {
            int i = text.IndexOf(';');
            if (i > 0)
            {
                PlayerGroup = text.Substring(0, i);
                PlayerName = text.Substring(i + 1);
            }
        }
        #endregion

        #region "TightVNCServer"
        private static void InstallTightVNC()
        {
            try
            {
                if (CheckTightVNCServer() == false)
                {
                    log.Info("Installing Proof of Play Service has started.");
                    Console.WriteLine("Installing Proof of Play Service has started.");
                    Console.WriteLine("Please wait one minute for completing configuration. The console will be automatically closed on the end.");

                    string path = Environment.CurrentDirectory + "\\mysetup.msi"; ;// System.IO.File.ReadAllText("C:\\ctemp.txt");// args[0].Replace("{Space}", " ");
                    log.Info($" Argument {path} was passed");

                    string sDoubleQuote = "\"";
                    string sFileNameRun = @" /i " + sDoubleQuote + path + sDoubleQuote + " /quiet /norestart ADDLOCAL=" + sDoubleQuote + "Server" + sDoubleQuote + " SERVER_REGISTER_AS_SERVICE=1 SERVER_ADD_FIREWALL_EXCEPTION=1 VIEWER_ADD_FIREWALL_EXCEPTION=1 SERVER_ALLOW_SAS=1 SET_USEVNCAUTHENTICATION=1 VALUE_OF_USEVNCAUTHENTICATION=1 SET_PASSWORD=1 VALUE_OF_PASSWORD=P@SSWORD@123!@##@! SET_USECONTROLAUTHENTICATION=1 VALUE_OF_USECONTROLAUTHENTICATION=1 SET_CONTROLPASSWORD=1 VALUE_OF_CONTROLPASSWORD=P@SSWORD@123!@##@!";
                    log.Info($"Parameters: {sFileNameRun}");

                    Process process = new Process();
                    process.StartInfo.FileName = "msiexec";
                    process.StartInfo.WorkingDirectory = @"C:\";
                    process.StartInfo.Arguments = sFileNameRun;
                    process.StartInfo.Verb = "runas";
                    process.Start();
                    process.WaitForExit(60000);

                    log.Info("Success");
                }
                else
                {
                    log.Info("Installing Proof of Play Service has insalled already.");
                    Console.WriteLine("Installing Proof of Play Service has insalled already.");
                }
            }
            catch (Exception e)
            {
                log.Error("InstallTightVNC", e);
            }
        }

        private static void WriteToRegistery(int width, int height)
        {
            Microsoft.Win32.RegistryKey key = null;
            key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\TightVNC\Server\", true);
            if (key != null)
            {
                string sValue = "5901:" + width + "x" + height + "+0+0,5902:" + width + "x" + height + "+" + width + "+0";

                key.SetValue("ExtraPorts", "", Microsoft.Win32.RegistryValueKind.String);
                key.SetValue("ExtraPorts", sValue, Microsoft.Win32.RegistryValueKind.String);
                key.Close();
                RestartTightVNCServer();
            }
        }

        private static void RestartTightVNCServer()
        {
            using (ServiceController sc = new ServiceController("tvnserver"))
            {
                try
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        sc.Start();
                    }
                    else
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        sc.Start();

                    }
                }
                catch (Exception)
                {
                    // bResult = false;
                }
            }
        }

        private static bool CheckTightVNCServer()
        {
            bool bResult = false;
            try
            {
                using (ServiceController sc = new ServiceController("tvnserver"))
                {
                    if (sc.DisplayName != null)
                        bResult = true;
                }
            }
            catch (Exception ex)
            {

            }
            return bResult;
        }

        private static void WriteResolution()
        {
            try
            {
                var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = Screen.PrimaryScreen.Bounds.Height;
                Console.WriteLine($"Primary Screen Width: {screenWidth} Primary Screen Height: {screenHeight}");
                log.Info($"Primary Screen Width: {screenWidth} Primary Screen Height: {screenHeight}");
                WriteToRegistery(screenWidth, screenHeight);
            }
            catch (Exception e)
            {
                Console.WriteLine("Write Resolution Error");
                log.Error("Write Resolution: ", e);
            }
        }
        private static void GetIPAddress()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                HostName = host.HostName;
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IpAddress = ip.ToString();
                    }
                }
            }
        }
        #endregion

        #region [SignalR Variables Declearation]
        //private static HubProxy HubProxy { get; set; }
        private static HubConnection connection;

        //private static string LocalServerURI = "https://localhost:7055/realtime";
        private static string CloudServerURI = "https://64.251.7.228:6678/realtime";

        #endregion

        #region "SignalR Functions and Methods"
        static int reconnectTime = 10000;
        static int iAttempts = 10;
        async static void ConnectToSignalR()
        {
            try
            {
                log.Info("Trying to Connect to RealTime Server");
                Console.WriteLine("Trying to Connect to RealTime Server");
                CloudServerURI = ConfigurationManager.AppSettings["RealTimeServerURL"];
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                connection = new HubConnectionBuilder()
                .WithUrl(CloudServerURI)
                .Build();

                connection.Reconnecting += (error) =>
                {
                    Console.WriteLine($"Reconnecting Error Msg: {error.Message}");
                    log.Error($"Reconnecting Error Msg: {error.Message}");
                    //int milliseconds = 5 * 1000;
                    //Console.WriteLine($"Trying to Reconnected again {TimeSpan.FromMilliseconds(milliseconds).TotalSeconds} in seconds ");
                    //await Task.Delay(milliseconds);
                    return Task.CompletedTask;
                };
                connection.Closed += async (error) =>
                {
                    Console.WriteLine("Connection_Closed with RealTime Server");
                    log.Info("Connection_Closed with RealTime Server");
                    int milliseconds = 1000; ;
                    Console.WriteLine($"Trying to connect again {TimeSpan.FromMilliseconds(reconnectTime).TotalSeconds} in seconds ");
                    log.Info($"Trying to connect again {TimeSpan.FromMilliseconds(reconnectTime).TotalSeconds} in seconds ");
                    await Task.Delay(reconnectTime);
                    reconnectTime = reconnectTime + milliseconds;


                    //return Task.CompletedTask;
                    //await connection.StartAsync();
                    ConnectToSignalR();
                    //Console.WriteLine("connection started");
                };
                connection.Reconnected += (error) =>
                {

                    //int milliseconds = 5 * 1000;
                    //Console.WriteLine($"Trying to Reconnected again {TimeSpan.FromMilliseconds(milliseconds).TotalSeconds} in seconds ");
                    // await Task.Delay(milliseconds);
                    Console.WriteLine("Connection_Reconnected");
                    log.Info("Connection_Reconnected with RealTime Server");
                    return Task.CompletedTask;

                };

                connection.On<string>("ReceiveMessage", ReceiveMessage);
                connection.On<List<string>, string, string, DateTime, int>("SendToAllPlayers", SendToAllPlayers);
                connection.On<List<string>>("RestartAllPlayers", RestartAllPlayers);
                connection.On<List<string>, string, string, int, int, int>("SendAlertToAllPlayers", SendAlertToAllPlayers);
                await connection.StartAsync();
                Console.WriteLine("Connected to RealTime Server");
                log.Info("Connected to RealTime Server");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Connection to the RealTime Server:{ex.Message}");
                log.Info($"Error in Connection to the RealTime Server:{ex.Message}");
                int milliseconds = 1000;
                Console.WriteLine($"Trying to connect again {TimeSpan.FromMilliseconds(reconnectTime).TotalSeconds} in seconds ");

                await Task.Delay(reconnectTime);
                reconnectTime = reconnectTime + milliseconds;
                ConnectToSignalR();
            }
        }
        private static void ReceiveMessage(string obj)
        {
            try
            {
                Console.WriteLine($"ReceiveMessage Method Called with Parameter from Server:{obj}");
                log.Info($"ReceiveMessage Method Called with Parameter from Server:{obj}");
                connection.InvokeAsync("RegisterPlayer", obj, PlayerName, DateTime.Now);
                log.Info($"RegisterPlayer Method Called with Parameter :{obj} :{PlayerName}");
            }
            catch (Exception ex)
            {


            }
        }
        private static void SendToAllPlayers(List<string> sPlayerName, string sType, string sData, DateTime StartDateTime, int DuraitonInSeconds)
        {
            try
            {
                canSleep = false;
                canDownload = false;
                string OldContent = "";
                log.Info($"SendToAllPlayers Received from the RealTime Server");
                Console.WriteLine($"SendToAllPlayers Received from the RealTime Server");
                Console.Write($"SendToAllPlayers Received from the RealTime Server");
                log.Info($"sType:{sType}, Text:{sData}");
                Console.WriteLine($"sType:{sType}, Text:{sData}");
                if (sPlayerName == null) // Which mean All Players
                {


                    Console.WriteLine("All Player Inside");
                    log.Info("All Player Inside");
                    try
                    {
                        string sDirectoryPath = "C:\\CeitconDirectory";
                        try
                        {
                            sDirectoryPath = ConfigurationManager.AppSettings["CeitconDirectory"];
                        }
                        catch (Exception ex)
                        {

                        }
                        string[] strFiles = System.IO.Directory.GetFiles(sDirectoryPath, "*.cdp");
                        if (strFiles.Length > 0)
                        {
                            string sFilePath = Path.Combine(sDirectoryPath, strFiles[0]);
                            string cryptedText = System.IO.File.ReadAllText(sFilePath);
                            string _plantext = MyClass.Decrypt(cryptedText);
                            OldContent = cryptedText;

                            try
                            {
                                XDocument xdoc = XDocument.Parse(_plantext);
                                XElement item_project = xdoc.Descendants("Project").FirstOrDefault();
                                XElement itemalerts = item_project.Descendants("Alerts").FirstOrDefault();


                                //XDocument doc = XDocument.Load(sPath);
                                //IEnumerable<XElement> policyChangeSetCollection = doc.Elements("Alerts");
                                bool bWrite = false;
                                foreach (var alertitem in itemalerts.Descendants("Alert"))
                                {
                                    string AlertType = alertitem.Element("Type").Value;
                                    if (AlertType == "Global")
                                    {
                                        XElement _controls = alertitem.Descendants("Controls").FirstOrDefault();
                                        foreach (var _control in alertitem.Descendants("Control"))
                                        {
                                            string sControlType = _control.Element("Type").Value;
                                            if (sControlType.Trim().ToLower() == "live")
                                            {
                                                XElement _Playlists = _control.Descendants("Playlists").FirstOrDefault();
                                                XElement _Playlist = _Playlists.Descendants("Playlist").FirstOrDefault();
                                                string _oldContent = _Playlist.Element("Content").Value;
                                                Console.WriteLine($"Old Content {_oldContent}");
                                                if (sType != "")
                                                    _Playlist.Element("Content").Value = sType;

                                                Console.WriteLine($"New Content {sType}");

                                                XElement _slots = alertitem.Descendants("Slots").FirstOrDefault();
                                                XElement _slot = _slots.Descendants("Slot").FirstOrDefault();
                                                string _startTime = _slot.Element("StartTime").Value;
                                                string _Duration = _slot.Element("Duration").Value;


                                                _slot.Element("StartTime").Value = DateTime.Now.Ticks.ToString();
                                                _slot.Element("Duration").Value = DuraitonInSeconds.ToString();
                                                bWrite = true;
                                            }
                                            if (sControlType.Trim().ToLower() == "text")
                                            {
                                                XElement _Playlists = _control.Descendants("Playlists").FirstOrDefault();
                                                XElement _Playlist = _Playlists.Descendants("Playlist").FirstOrDefault();
                                                string _oldContent = _Playlist.Element("Content").Value;
                                                if (sData != "")
                                                    _Playlist.Element("Content").Value = sData;

                                                Console.WriteLine($"Old Text Content {_oldContent}");
                                                Console.WriteLine($"New Text Content {sData}");

                                                XElement _slots = alertitem.Descendants("Slots").FirstOrDefault();
                                                XElement _slot = _slots.Descendants("Slot").FirstOrDefault();
                                                string _startTime = _slot.Element("StartTime").Value;
                                                string _Duration = _slot.Element("Duration").Value;



                                                _slot.Element("StartTime").Value = DateTime.Now.Ticks.ToString();
                                                _slot.Element("Duration").Value = DuraitonInSeconds.ToString();
                                                bWrite = true;
                                            }
                                        }

                                    }

                                }
                                if (bWrite)
                                {
                                    string sEncrpt = MyClass.Encrypt(xdoc.ToString());

                                    if (System.IO.File.Exists(sFilePath))
                                    {
                                        System.IO.File.Delete(sFilePath);
                                    }
                                    System.IO.File.WriteAllText(sFilePath, sEncrpt);
                                    log.Info($"Stop Downloading New Content for Duration in Seconds {DuraitonInSeconds.ToString()}");
                                    Console.WriteLine($"Stop Downloading New Content for Duration in Seconds {DuraitonInSeconds.ToString()}");
                                    //Task.Delay(new TimeSpan(0, 0, DuraitonInSeconds));
                                    System.Threading.Thread.Sleep(new TimeSpan(0, 0, DuraitonInSeconds));


                                    if (System.IO.File.Exists(sFilePath))
                                    {
                                        System.IO.File.Delete(sFilePath);
                                    }
                                    System.IO.File.WriteAllText(sFilePath, OldContent);

                                    Console.WriteLine($"Resume Downloading New Content");
                                    log.Info($"Resume Downloading New Content");
                                    canDownload = true;
                                    canSleep = true;

                                    //xdoc.Save(sEncrpt);
                                }
                            }
                            catch (Exception ex)
                            {
                                canDownload = true;
                                canSleep = true;
                                log.Info($"Send To All Players:Error: {ex.Message}");
                            }
                        }





                    }
                    catch (Exception ex)
                    {
                        canDownload = true;
                        canSleep = true;
                        log.Info($"SendToAllPlayers: All Players Error: {ex.Message}");
                    }
                    canDownload = true;
                    canSleep = true;
                }
                else
                {
                    canDownload = false;
                    canSleep = false;
                    string _playerexits = sPlayerName.SingleOrDefault(p => p == PlayerName);
                    if (_playerexits == null || _playerexits == "")
                    {
                        log.Info($"SendToAllPlayers Not for me");
                        Console.WriteLine($"SendToAllPlayers Not for me");
                        canDownload = true;
                        canSleep = true;
                        return;
                    }
                    else
                    {
                        //canDownload = false;
                        log.Info($"SendToAllPlayers for me");
                        Console.WriteLine($"SendToAllPlayers  for me");

                        #region "Processing CDP File"
                        try
                        {
                            //canDownload = false;
                            string sDirectoryPath = "C:\\CeitconDirectory";
                            try
                            {
                                sDirectoryPath = ConfigurationManager.AppSettings["CeitconDirectory"];
                            }
                            catch (Exception ex)
                            {

                            }
                            string[] strFiles = System.IO.Directory.GetFiles(sDirectoryPath, "*.cdp");
                            if (strFiles.Length > 0)
                            {
                                string sFilePath = Path.Combine(sDirectoryPath, strFiles[0]);
                                string cryptedText = System.IO.File.ReadAllText(sFilePath);
                                OldContent = cryptedText;
                                string _plantext = MyClass.Decrypt(cryptedText);

                                try
                                {
                                    XDocument xdoc = XDocument.Parse(_plantext);
                                    XElement item_project = xdoc.Descendants("Project").FirstOrDefault();
                                    XElement itemalerts = item_project.Descendants("Alerts").FirstOrDefault();


                                    //XDocument doc = XDocument.Load(sPath);
                                    //IEnumerable<XElement> policyChangeSetCollection = doc.Elements("Alerts");
                                    bool bWrite = false;
                                    foreach (var alertitem in itemalerts.Descendants("Alert"))
                                    {
                                        string AlertType = alertitem.Element("Type").Value;
                                        if (AlertType == "Global")
                                        {
                                            XElement _controls = alertitem.Descendants("Controls").FirstOrDefault();
                                            foreach (var _control in alertitem.Descendants("Control"))
                                            {
                                                string sControlType = _control.Element("Type").Value;
                                                if (sControlType.Trim().ToLower() == "live")
                                                {
                                                    XElement _Playlists = _control.Descendants("Playlists").FirstOrDefault();
                                                    XElement _Playlist = _Playlists.Descendants("Playlist").FirstOrDefault();
                                                    string _oldContent = _Playlist.Element("Content").Value;
                                                    Console.WriteLine($"Old Content {_oldContent}");
                                                    if (sType != "")
                                                        _Playlist.Element("Content").Value = sType;

                                                    Console.WriteLine($"New Content {sType}");

                                                    XElement _slots = alertitem.Descendants("Slots").FirstOrDefault();
                                                    XElement _slot = _slots.Descendants("Slot").FirstOrDefault();
                                                    string _startTime = _slot.Element("StartTime").Value;
                                                    string _Duration = _slot.Element("Duration").Value;


                                                    _slot.Element("StartTime").Value = DateTime.Now.Ticks.ToString();
                                                    _slot.Element("Duration").Value = DuraitonInSeconds.ToString();
                                                    bWrite = true;
                                                }
                                                if (sControlType.Trim().ToLower() == "text")
                                                {
                                                    XElement _Playlists = _control.Descendants("Playlists").FirstOrDefault();
                                                    XElement _Playlist = _Playlists.Descendants("Playlist").FirstOrDefault();
                                                    string _oldContent = _Playlist.Element("Content").Value;
                                                    if (sData != "")
                                                        _Playlist.Element("Content").Value = sData;

                                                    Console.WriteLine($"Old Text Content {_oldContent}");
                                                    Console.WriteLine($"New Text Content {sData}");

                                                    XElement _slots = alertitem.Descendants("Slots").FirstOrDefault();
                                                    XElement _slot = _slots.Descendants("Slot").FirstOrDefault();
                                                    string _startTime = _slot.Element("StartTime").Value;
                                                    string _Duration = _slot.Element("Duration").Value;



                                                    _slot.Element("StartTime").Value = DateTime.Now.Ticks.ToString();
                                                    _slot.Element("Duration").Value = DuraitonInSeconds.ToString();
                                                    bWrite = true;
                                                }
                                            }

                                        }

                                    }
                                    if (bWrite)
                                    {
                                        string sEncrpt = MyClass.Encrypt(xdoc.ToString());

                                        if (System.IO.File.Exists(sFilePath))
                                        {
                                            System.IO.File.Delete(sFilePath);
                                        }
                                        System.IO.File.WriteAllText(sFilePath, sEncrpt);
                                        log.Info($"Stop Downloading New Content for Duration in Seconds {DuraitonInSeconds.ToString()}");
                                        Console.WriteLine($"Stop Downloading New Content for Duration in Seconds {DuraitonInSeconds.ToString()}");
                                        //Task.Delay(new TimeSpan(0, 0, DuraitonInSeconds));
                                        System.Threading.Thread.Sleep(new TimeSpan(0, 0, DuraitonInSeconds));
                                        OldContent = cryptedText;

                                        if (System.IO.File.Exists(sFilePath))
                                        {
                                            System.IO.File.Delete(sFilePath);
                                        }
                                        System.IO.File.WriteAllText(sFilePath, OldContent);

                                        Console.WriteLine($"Resume Downloading New Content");
                                        log.Info($"Resume Downloading New Content");
                                        canDownload = true;
                                        canSleep = true;

                                        //xdoc.Save(sEncrpt);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    canDownload = true;
                                    canSleep = true;
                                    log.Info($"Send To All Players:Error: {ex.Message}");
                                }
                            }





                        }
                        catch (Exception ex)
                        {
                            canDownload = true;
                            canSleep = true;
                            log.Info($"SendToAllPlayers: All Players Error: {ex.Message}");
                        }
                        #endregion
                        canDownload = true;
                        canSleep = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"canDownload:{canDownload.ToString()}");
                Console.WriteLine($"Error in SendToAllPlayers:{ex.Message}");
                log.Info($"canDownload:{canDownload.ToString()}");
                log.Info($"Error in SendToAllPlayers:{ex.Message}");

                canDownload = true;
            }
        }
        private static void RestartAllPlayers(List<string> sPlayerName)
        {
            try
            {
                log.Info($"RestartAllPlayers Received from the RealTime Server");
                Console.WriteLine($"RestartAllPlayers Received from the RealTime Server");
                string _playerexits = sPlayerName.SingleOrDefault(p => p == PlayerName);
                if (sPlayerName == null) // Which mean All Players
                {
                    log.Info($"RestartAllPlayers I am included");
                    Console.WriteLine($"RestartAllPlayers I am included");
                    KillProcess2(PlayerProcessName);
                    log.Info($"Restarted the Player");
                    Console.WriteLine($"Restarted the Player");
                }
                else
                {
                    if (_playerexits == null || _playerexits == "")
                    {
                        log.Info($"RestartAllPlayers Not for me");
                        return;
                    }
                    else
                    {
                        log.Info($"RestartAllPlayers I am included");
                        KillProcess2(PlayerProcessName);
                        log.Info($"Restarted the Player");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("RestartAllPlayers", ex);

            }
        }
        private static void SendAlertToAllPlayers(List<string> sPlayerName, string sMediaURL, string sTextMessage, int DuraitonInSeconds, int Width, int Height)
        {
            try
            {
                string _playerexits = sPlayerName.SingleOrDefault(p => p == PlayerName);

                Console.WriteLine($"SendAlertToAllPlayers Method Called with Parameter from Server sPlayerName:" + sPlayerName[0] + " sMediaURL: " + sMediaURL + " sTextMessage: " + sTextMessage + " DurationInSeconds: " + DuraitonInSeconds + " Width: " + Width + " Height: " + Height);

                if (_playerexits == null || _playerexits == "")
                {
                    log.Info($"SendAlertToAllPlayers Not for me");
                    return;
                }
                else
                {
                    log.Info($"SendAlertToAllPlayers I am included");
                    string sPath = ConfigurationManager.AppSettings["PopUpPath"];
                    Process process = new Process();
                    //ProcessStartInfo info = new ProcessStartInfo(sPath, "\"" + sMediaURL + "\" \"" + sTextMessage + "\" 20 "
                    //    + DuraitonInSeconds + "  " + Height + " " + Width);
                    ProcessStartInfo info = new ProcessStartInfo(sPath, "\"" + sMediaURL + "\" " + DuraitonInSeconds + "  " + Height + " " + Width);
                    log.Info("File Path: " + info.FileName + " Argument: " + info.Arguments);
                    process.StartInfo = info;
                    process.Start();
                    log.Info($"SendAlertToAllPlayers Method Called with Parameter from Server sPlayerName:" + sPlayerName[0] + " sMediaURL: " + sMediaURL + " sTextMessage: " + sTextMessage + " DurationInSeconds: " + DuraitonInSeconds + " Width: " + Width + " Height: " + Height);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendAlertToAllPlayers:{ex.Message}");
                log.Info($"Error in SendAlertToAllPlayers:{ex.Message}");
            }
        }
        #endregion


        /*
        #region [SignalR Variables Declearation]
        private static IHubProxy HubProxy { get; set; }
        private static string ServerURI = "http://{0}:9896";
        private static HubConnection Connection { get; set; }
        #endregion

        #region SignalR Function Events"
        private static void DisconnectSignalR()
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Stop();
                }
                catch (Exception e)
                {
                    log.Error("Disconnect SignalR:", e);
                }
            }
        }

        static bool bCall = false;

        private static async void ConnectAsync()
        {
            bCall = false;
            log.Info("Connect Async Called");
            if (Connection != null)
            {
                log.Info("Connect Async Called : Connection was not NULL: State" + Connection.State.ToString());

                if (Connection.State == ConnectionState.Disconnected)
                {
                    try
                    {
                        Connection.Closed -= Connection_Closed;
                        Connection.StateChanged -= Connection_StateChanged;
                        Connection.ConnectionSlow -= Connection_ConnectionSlow;
                        Connection.Error -= Connection_Error;
                    }
                    catch (Exception)
                    {
                    }
                    Connection.Dispose();

                    log.Info("Connect Async Called : State was Disconnected");
                    ServerURI = string.Format(ServerURI, ServerIP);
                    Connection = new HubConnection(ServerURI);

                    Connection.Closed += Connection_Closed;
                    HubProxy = Connection.CreateHubProxy("chatHub");
                    Connection.StateChanged += Connection_StateChanged;
                    Connection.ConnectionSlow += Connection_ConnectionSlow;

                    HubProxy.On("registeredplayer", RegisteredPlayer);
                    HubProxy.On("unregisteredplayer", UnregisteredPlayer);
                    HubProxy.On<string>("sendrtmessage", RTMessage);

                    HubProxy.On<byte[]>("sendxml", ReadXml);
                    HubProxy.On<byte[]>("sendxmltoall", ReadXmlAll);
                    HubProxy.On<byte[]>("sendcpfile", ReadCDPFile);

                    try
                    {
                        await Connection.Start();
                        log.Info("Connection Stated Successfully with " + ServerURI);
                        // PlayerSignIn(string playername, string IPAdress, string ComputerName)
                        await HubProxy.Invoke("PlayerSignIn",
                            new object[] { PlayerName.ToLower(), IpAddress, HostName.ToLower() });
                    }
                    catch (Exception ex)
                    {
                        log.Info(" Exception: SignalR Connect Sync " + ex.Message);
                        log.Debug("SignalR Connect Sync:", ex);
                        bCall = true;
                    }
                }
            }
            else
            {
                log.Info("Connect Async Called : When Connection State was NULL");
                ServerURI = string.Format(ServerURI, ServerIP);
                Connection = new HubConnection(ServerURI);
                // Connection.TransportConnectTimeout = TimeSpan.FromDays(1);

                Connection.Closed += Connection_Closed;

                Connection.StateChanged += Connection_StateChanged;
                Connection.ConnectionSlow += Connection_ConnectionSlow;
                // Connection.Error += Connection_Error;

                HubProxy = Connection.CreateHubProxy("chatHub");

                HubProxy.On("registeredplayer", RegisteredPlayer);
                HubProxy.On("unregisteredplayer", UnregisteredPlayer);
                HubProxy.On<string>("sendrtmessage", RTMessage);

                HubProxy.On<byte[]>("sendxml", ReadXml);
                HubProxy.On<byte[]>("sendxmltoall", ReadXmlAll);
                HubProxy.On<byte[]>("sendcpfile", ReadCDPFile);


                try
                {
                    await Connection.Start();

                    await HubProxy.Invoke("PlayerSignIn",
                           new object[] { PlayerName.ToLower(), IpAddress, HostName.ToLower() });

                }
                catch (Exception ex)
                {
                    log.Info(" Exception: SignalR Connect Sync " + ex.Message);
                    log.Debug("SignalR Connect Sync:", ex);
                    bCall = true;
                }

            }

        }

        private static void RegisteredPlayer()
        {
            log.Info("SignalR: RegisteredPlayer");
            Console.WriteLine(String.Format("SignalR: RegisteredPlayer"));
            CheckPlayerStatus();

            KillProcess(PlayerProcessName);
            RunAsync(PlayerPath);
        }

        private static void UnregisteredPlayer()
        {
            log.Info("SignalR: UnregisteredPlayer");
            Console.WriteLine(String.Format("SignalR: UnregisteredPlayer"));
            CheckPlayerStatus();
        }

        private static void RTMessage(string message)
        {
            //Console.WriteLine(String.Format("RTMessage Topic: {0}, Message: {1}", topic, message));
            Console.WriteLine(String.Format("SignalR: RTMessage {0}", message));
            log.Info(String.Format("SignalR: RTMessage {0}", message));

            if (String.IsNullOrEmpty(message))
                return;

            int pos = message.IndexOf(':');
            if (pos > 0)
            {
                string topic = message.Substring(0, pos);
                string text = message.Substring(pos + 1);
                if (!String.IsNullOrEmpty(topic) && !String.IsNullOrEmpty(text))
                    SQLiteHelper.Instance.InsertRTMessage(topic, text);
            }
        }

        private static void ReadXml(byte[] ms)
        {
            try
            {
                Console.WriteLine(String.Format("SignalR: Recived XML"));
                log.Info(String.Format("SignalR: Recived XML"));

                string path = Path.Combine(MediaDirectory, dataFileName);
                string extractPath = Path.Combine(MediaDirectory + "_New", dataFileName);
                if (File.Exists(extractPath))
                    File.Delete(extractPath);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                File.WriteAllBytes(extractPath, ms);
                if (File.Exists(extractPath))
                {
                    File.Copy(extractPath, path, true);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
            }
        }

        private static void ReadXmlAll(byte[] ms)
        {
            try
            {
                Console.WriteLine(String.Format("SignalR: Recived new XML"));
                log.Info(String.Format("SignalR: Recived new XML"));

                string path = Path.Combine(MediaDirectory, dataFileName);
                string extractPath = Path.Combine(MediaDirectory + "_New", dataFileName);
                if (File.Exists(extractPath))
                    File.Delete(extractPath);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                File.WriteAllBytes(extractPath, ms);
                if (File.Exists(extractPath))
                {
                    File.Copy(extractPath, path, true);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
            }
        }

        private static void ReadCDPFile(byte[] byt)
        {
            try
            {
                Console.WriteLine(String.Format("SignalR: Recived new CPD"));
                log.Info(String.Format("SignalR: Recived new CPD"));

                canSleep = false;

            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
            }
        }

        private static void Connection_Error(Exception obj)
        {
            log.Debug("Connection_Error", obj);
            log.Info("ConnectAsync Called from Connection_Error with Connection State:" + Connection.State.ToString());
        }

        private static void Connection_ConnectionSlow()
        {
            log.Info("Connection is getting Slow");
        }
        static void Connection_StateChanged(StateChange obj)
        {
            objState = obj;

            log.Info("Connection_StateChanged:" + " Connection.State:" + Connection.State.ToString());

        }

        static StateChange objState;

        static void Connection_Closed()
        {
            log.Info("Connection Closed Event Called:" + Connection.State.ToString());
        }

        #endregion
        */
    }
}