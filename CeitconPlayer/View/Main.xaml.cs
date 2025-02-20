using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Data.Utilities;
using Ceitcon_Player.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Ceitcon_Player.View
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Display.Orientations DefaultOrientation { get; set; }
        private int totalScreen = 0;
        private int changeDisplayCount = 0;
        private string screenString = String.Empty;
        private const string tickerName = "Ceitcon_Ticker";
        private const string fidFileName = "FID.xml";
        private const string weatherFileName = "_Weather.xml";

        public Main()
        {
            InitializeComponent();

            log.Info("Starting application. Version 1.95");

            this.Cursor = Cursors.None;

            var window = Window.GetWindow(this);
            window.KeyDown += HandleKeyPress;

            try
            {
                //Load activ screen
                string showOnScreen = ConfigurationManager.AppSettings["ShowOnScreen"];
                if (String.IsNullOrWhiteSpace(showOnScreen))
                    MessageBox.Show("ShowOnScreen property is not set in config file. Main screen will be automatically selected.", "Info");

                int numValue;
                bool hasValue = Int32.TryParse(showOnScreen, out numValue);

                //Default
                var s = System.Windows.Forms.Screen.AllScreens[hasValue ? numValue : 0];
                var r = s.Bounds;
                this.Top = r.Top;
                this.Left = r.Left;
                this.Width = r.Width;
                this.Height = r.Height;

                Watcher();
                Init();
            }
            catch (Exception e)
            {
                log.Error("Starting application error", e);
                MessageBox.Show("Loading project error", "Error", MessageBoxButton.OK);
            }
        }

        private void RotateResolution(RegionModel region)
        {
            //log.Info(String.Format("Display {0} ; Name: {0} ; Primary: {1} ; Bounds: {2} ; WorkingArea: {3}", i, screen.DeviceName, screen.Primary, screen.Bounds, screen.WorkingArea));

            log.Error($"Before Region Height {region.Height.ToString()} ; Width {region.Width.ToString()}");
            double temp = region.Height;
            region.Height = region.Width;
            region.Width = temp;
            log.Error($"After Region Height {region.Height.ToString()} ; Width {region.Width.ToString()}");

            log.Error($"Before Region Y {region.Y.ToString()} ; X {region.X.ToString()}");
            temp = region.Y;
            region.Y = region.X;
            region.X = temp;
            log.Error($"Before Region Y {region.Y.ToString()} ; X {region.X.ToString()}");

            foreach (var slide in region.Slides)
            {
                foreach (var layer in slide.Layers)
                {
                    foreach (var control in layer.Controls)
                    {
                        temp = control.Height;
                        control.Height = control.Width;
                        control.Width = temp;

                        temp = control.Y;
                        control.Y = control.X;
                        control.X = temp;
                    }
                }
            }
        }
        private void WriteToRegistery(string Width, string Height)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = null;
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\TightVNC\Server\", true);
                if (key != null)
                {
                    string sValue = "5901:" + Width + "x" + Height + "+0+0,5902:" + Width + "x" + Height + "+" + Width + "+0";

                    key.SetValue("ExtraPorts", "", Microsoft.Win32.RegistryValueKind.String);
                    key.SetValue("ExtraPorts", sValue, Microsoft.Win32.RegistryValueKind.String);
                    key.Close();
                    RestartTightVNCServer();
                }
            }
            catch (Exception)
            {


            }
        }
        private void RestartTightVNCServer()
        {
            try
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
                    catch (Exception ex)
                    {
                        // bResult = false;
                    }

                }
            }
            catch (Exception)
            {

                //  bResult = false;
            }
        }

        // CancellationTokenSource cts = null;
        System.Timers.Timer objPrayerTimer = null;
        ProjectModel project;
        public async void Init()
        {
            try
            {
                log.Info("Initialization start");
                if (objPrayerTimer != null)
                {
                    try
                    {
                        log.Info("Stoping prayer alert timer");
                        objPrayerTimer.Stop();
                        objPrayerTimer.Dispose();
                        objPrayerTimer = null;
                    }
                    catch (Exception ex)
                    {
                        log.Info($"Error occur stoping prayer timer {ex.Message}");
                    }
                }



                //var project = LoadProject();
                project = LoadProject();
                if (project != null && project.Regions.Count > 0)
                {
                    log.Info(String.Format("Load project {0}", project.Information.ProjectName));



                    //Resolution and Location
                    var region = project.Regions.FirstOrDefault();
                    log.Info(String.Format("Preview Region > Top: {0}, Left: {1}, Width: {2}, Height: {3}", region.X, region.Y, region.Width, region.Height));

                    //Orientation
                    log.Info("Rotate display.");
                    var orientation = project.SelectedOrientation;
                    log.Info($"Screen Orientation {orientation.ToString()}");
                    switch ((int)orientation)
                    {
                        case 0:
                            DefaultOrientation = Display.Orientations.DEGREES_CW_0;
                            break;
                        case 1:
                            DefaultOrientation = Display.Orientations.DEGREES_CW_90;
                            //RotateResolution(region);
                            break;
                        case 2:
                            DefaultOrientation = Display.Orientations.DEGREES_CW_180;
                            break;
                        case 3:
                            DefaultOrientation = Display.Orientations.DEGREES_CW_270;
                            //RotateResolution(region);
                            break;
                        default:
                            DefaultOrientation = Display.Orientations.DEGREES_CW_0;
                            break;
                    }
                    uint i = 0;
                    foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                    {
                        Display.Rotate(++i, DefaultOrientation);
                    }

                    log.Info(String.Format("After Rotation Preview Region > Top: {0}, Left: {1}, Width: {2}, Height: {3}", region.X, region.Y, region.Width, region.Height));

                    //Scale
                    double scaleFactor = GetScalingFactor();
                    log.Info(String.Format("Scale factor: {0}", scaleFactor));
                    //this.Left = region.X;
                    this.Top = region.Y / scaleFactor;
                    this.Width = region.Width / scaleFactor;
                    this.Height = region.Height / scaleFactor;
                    log.Info(String.Format("Screen number {0}", System.Windows.Forms.Screen.AllScreens.Count()));
                    if (System.Windows.Forms.Screen.AllScreens.Count() > 1 && System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Location.X < 0)
                    {
                        this.Left = System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Location.X / scaleFactor;
                    }
                    else
                    {
                        this.Left = region.X / scaleFactor;
                    }

                    //Run Player
                    string[] args = Environment.GetCommandLineArgs();

                    int mode = args.Count() > 1 ? Convert.ToInt32(args[1]) : 3;
                    mode = 0;
                    log.Info(String.Format("Arguments mode: {0}", mode));
                    this.Background = Brushes.Black;
                    this.preview.Mode = mode;

                    //Set data
                    string[] data = Ceitcon_Data.Utilities.IOManagerProject.GetFlightId(region);
                    if (data != null && data.Count() > 0)
                    {
                        string MediaDirectory = ConfigurationManager.AppSettings["CeitconDirectory"];
                        //string dataPath = Path.Combine(MediaDirectory, String.Format("{0}.xml", data[0]));
                        string dataPath = Path.Combine(MediaDirectory, "FID.xml");
                        if (File.Exists(dataPath))
                        {
                            string xmlData = System.IO.File.ReadAllText(dataPath);
                            this.preview.FlightList = Ceitcon_Designer.Utilities.SQLiteHelper.Instance.GetFlightsDataFromXml(xmlData);
                        }
                    }

                    //Set Weather
                    string[] weatherData = Ceitcon_Data.Utilities.IOManagerProject.GetWeatherLocations(project);
                    if (weatherData != null && weatherData.Count() > 0)
                    {
                        string MediaDirectory = ConfigurationManager.AppSettings["CeitconDirectory"];
                        foreach (var item in weatherData)
                        {
                            string dataPath = Path.Combine(MediaDirectory, item + weatherFileName);
                            if (File.Exists(dataPath))
                            {
                                FileInfo fi1 = new FileInfo(dataPath);
                                int j = 0;
                                while (IsFileLocked(fi1) && j <= 15)
                                {
                                    j++;
                                    Thread.Sleep(TimeSpan.FromSeconds(2));
                                }
                                string xmlData = System.IO.File.ReadAllText(dataPath);
                                if (this.preview.WeatherList.ContainsKey(item))
                                    this.preview.WeatherList[item] = xmlData;
                                else
                                    this.preview.WeatherList.Add(item, xmlData);
                            }
                        }
                    }

                    //Get set region
                    DataContext = region;
                    // cts = new CancellationTokenSource();

                    try
                    {
                        //await Task.Run(() => ExecuteRealTimeDataAsync(this, project));//, cts.Token);
                        if (project.Alerts != null && project.Alerts.Count > 0 && project.Alerts.Where(_ => _.Type == AlertType.Prayer).Count() > 0)
                        {
                            bShowTime = true;
                            objPrayerTimer = new System.Timers.Timer();
                            objPrayerTimer.Interval = (new TimeSpan(0, 0, 1)).TotalSeconds;
                            objPrayerTimer.Elapsed += ObjPrayerTimer_Elapsed;
                            objPrayerTimer.Start();

                            await Task.Run(() => ExecuteAlertGlobalAsync(this, project));//, cts.Token);
                        }
                        else
                        {
                            //   await Task.Run(() => ExecuteAlertPrayerAsync(this, project));//, cts.Token);
                            await Task.Run(() => ExecuteAlertGlobalAsync(this, project));//, cts.Token);
                        }
                    }
                    catch (Exception /*OperationCanceledException*/)
                    {
                        // TODO: update the GUI to indicate the method was canceled.
                    }
                }
                log.Info("Initialization end");
            }
            catch (Exception ex)
            {
                log.Error("System Events Display Settings Changed error", ex);
            }
        }


        bool bShowTime = true;
        bool bStartTimer = true;
        private void ObjPrayerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                objPrayerTimer.Stop();

                if (bShowTime)
                    log.Info("Prayer timer started");
                if (project.Alerts != null && project.Alerts.Count > 0 && project.Alerts.Where(_ => _.Type == AlertType.Prayer).Count() > 0)
                {

                    if (bShowTime)
                        log.Info("Stop Prayer Alert");
                    try
                    {
                        var alert = project.Alerts.Where(_ => _.Type == AlertType.Prayer).FirstOrDefault();
                        var slot = alert.Slots.FirstOrDefault();

                        List<TimeSpan> list = getPrayerTime(slot);

                        //log.Info($"Alert Prayer time count: {list.Count}");
                        if (list.Count == 0)
                        {
                            log.Info("No Prayer time count found");
                            objPrayerTimer.Start();
                            return;
                        }
                        int i = 0;
                        if (bShowTime)
                        {
                            foreach (var item in list)
                            {
                                log.Info($"Alert Prayer time {i++} : {item}");
                            }
                            bShowTime = false;
                        }

                        i = -1;
                        foreach (var start in list)
                        {
                            i++;
                            TimeSpan end = start + slot.Duration;
                            if (end <= DateTime.Now.TimeOfDay)
                            {
                                //log.Info("Alert Prayer go to next");
                                continue;
                            }

                            //log.Info($"Start: {start}  End: {end} Current: {DateTime.Now.TimeOfDay}");

                            if (start <= DateTime.Now.TimeOfDay && end > DateTime.Now.TimeOfDay)
                            {
                                //this.Dispatcher.Invoke(() =>
                                bStartTimer = false;
                                log.Info("Stop Timer to display Prayer Alert");
                                var bResult = Dispatcher.BeginInvoke(new Action(async () =>

                                {


                                    log.Info($"Start Alert Prayer {alert.Name} Start time {start}");
                                    object tempDataContext = DataContext;
                                    int tempMode = this.preview.Mode;

                                    this.Background = Brushes.Black;
                                    this.preview.Mode = 1; //Alert is without ticker

                                    //Calculate prayer values
                                    CalculatePrayerSource(alert, i);

                                    DataContext = alert;
                                    TimeSpan wait = end - DateTime.Now.TimeOfDay;

                                    log.Info($"Start Alert Prayer Task.Delay: {slot.Duration}");
                                    await Task.Delay(slot.Duration);
                                    //System.Threading.Thread.Sleep(slot.Duration);
                                    log.Info($"End Alert Prayer Task.Delay: {slot.Duration}");
                                    //cts.CancelAfter(wait);
                                    this.preview.Mode = tempMode;
                                    DataContext = tempDataContext;
                                    log.Info($"Stop Prayer Alert {alert.Name} End time {end}");
                                    log.Info($"breakLoop");
                                    log.Info("Re Init is called");


                                }));
                                System.Threading.Thread.Sleep(slot.Duration);
                                log.Info("starting Timer again");
                                bStartTimer = true; ;
                                bShowTime = true;
                            }
                        }



                    }
                    catch (Exception ex)
                    {
                        log.Info($"ExecuteAlertPrayerAsync Error:{ex.Message}");
                    }
                    if (bStartTimer)
                        objPrayerTimer.Start();
                    if (bShowTime)
                        log.Info("Start Prayer Alert Again");
                }

            }
            catch (Exception ex)
            {
                if (objPrayerTimer != null)
                {
                    if (objPrayerTimer.Enabled == false)
                    {
                        objPrayerTimer.Enabled = true;
                        objPrayerTimer.Start();
                    }
                }
                log.Info($"Error:Prayer timer event:{ex.Message}");

            }
        }

        async Task ExecuteRealTimeDataAsync(Main gui, ProjectModel project)
        {
            //MQTT
            this.Dispatcher.Invoke(() =>
            {
                if (!(DataContext is RegionModel))
                    return;
            });


            //if (DataContext is RegionModel)
            //{
            await Dispatcher.BeginInvoke(new Action(async () =>
            {
                log.Info("Start MQTT Client client");
                while (true)
                {
                    //if (DataContext is RegionModel)
                    //{
                    foreach (SlideModel slide in project.Regions[0].Slides)
                    {
                        foreach (LayerModel layer in slide.Layers)
                        {
                            foreach (ControlModel control in layer.Controls)
                            {
                                if (control.Type == ControlType.Ticker || control.Type == ControlType.Likebox ||
                                        control.Type == ControlType.Facebook || control.Type == ControlType.Instagram ||
                                        control.Type == ControlType.Twitter || control.Type == ControlType.RichText)
                                    foreach (var playlist in control.Playlist)
                                    {
                                        if (playlist.Type == PlaylistType.SetContent)
                                        {
                                            string content = (playlist as SetContentModel).Content;
                                            if (content.StartsWith("topic:"))
                                                control.Text = SQLiteHelper.Instance.GetRTMessage(content.Substring(6));
                                        }
                                    }
                            }
                        }
                    }
                    //}
                    TimeSpan ts = new TimeSpan(0, 0, 10);
                    await Task.Delay(ts);
                }
            }
            ));
            //}
        }
        void ReInit()
        {
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    log.Info("ReInit Called");
                    DataContext = null;
                    Init();
                }
                catch (Exception ex)
                {
                    log.Error("ReInit", ex);
                }
            });

        }

        async Task ExecuteAlertPrayerAsync(Main gui, ProjectModel project)
        {
            //Check Alert


            if (project.Alerts != null && project.Alerts.Count > 0 && project.Alerts.Where(_ => _.Type == AlertType.Prayer).Count() > 0)
            {
                log.Info($"Alert Prayer background thread started. Alert count: {project.Alerts.Where(_ => _.Type == AlertType.Prayer).Count()}");
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        var alert = project.Alerts.Where(_ => _.Type == AlertType.Prayer).FirstOrDefault();
                        var slot = alert.Slots.FirstOrDefault();

                        while (true)
                        {
                            List<TimeSpan> list = getPrayerTime(slot);
                            //For test

                            //list[0] = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(10));
                            //list[1] = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(100));
                            //list[2] = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(200));
                            //list[3] = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(300));
                            //list[4] = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(400));

                            log.Info($"Alert Prayer time count: {list.Count}");
                            if (list.Count == 0)
                                return;

                            int i = 0;
                            foreach (var item in list)
                            {
                                log.Info($"Alert Prayer time {i++} : {item}");
                            }

                            log.Info($"Alert Prayer Curant Time {DateTime.Now.TimeOfDay}");

                            i = -1;
                            foreach (var start in list)
                            {
                                i++;



                                TimeSpan end = start + slot.Duration;
                                if (end <= DateTime.Now.TimeOfDay)
                                {
                                    //log.Info("Alert Prayer go to next");
                                    continue;
                                }
                                log.Info($"11 Start Alert Prayer {alert.Name} Start time {start}");
                                //////if (start > DateTime.Now.TimeOfDay)
                                //////{
                                //////    var wait = start - DateTime.Now.TimeOfDay;
                                //////    log.Info($" Start Alert Prayer Task.Delay: {wait}");
                                //////    await Task.Delay(wait);
                                //////}

                                await Task.Delay(new TimeSpan(0, 0, 1));
                                log.Info($"Start: {start}  End: {end} Current: {DateTime.Now.TimeOfDay}");

                                if (start <= DateTime.Now.TimeOfDay && end > DateTime.Now.TimeOfDay)
                                {
                                    log.Info($"Start Alert Prayer {alert.Name} Start time {start}");
                                    object tempDataContext = DataContext;
                                    int tempMode = this.preview.Mode;

                                    this.Background = Brushes.Black;
                                    this.preview.Mode = 1; //Alert is without ticker

                                    //Calculate prayer values
                                    CalculatePrayerSource(alert, i);

                                    DataContext = alert;
                                    TimeSpan wait = end - DateTime.Now.TimeOfDay;

                                    log.Info($"End Alert Prayer Task.Delay: {wait}");
                                    await Task.Delay(wait);
                                    //cts.CancelAfter(wait);
                                    this.preview.Mode = tempMode;
                                    DataContext = tempDataContext;
                                    log.Info($"Stop Prayer Alert {alert.Name} End time {end}");
                                    log.Info($"breakLoop");
                                    log.Info("Re Init is called");
                                    /*
                                    try
                                    {
                                        //System.Windows.Forms.Application.Restart();
                                        //System.Windows.Application.Current.Shutdown();
                                        if (cts.IsCancellationRequested)
                                        {
                                            log.Info($"cts.IsConcelledRequest 1 {cts.IsCancellationRequested.ToString()}");
                                            cts.Cancel();
                                            ReInit();
                                            
                                        }
                                        else
                                        {
                                            log.Info($"cts.IsConcelledRequest2 {cts.IsCancellationRequested.ToString()}");
                                            cts.Cancel();
                                            ReInit();
                                        }



                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error("Cancel Task:" + ex.Message);
                                    }
                                    */
                                    //ReInit();


                                }
                            }


                            //log.Info("1");
                            ////Wait for next day
                            //TimeSpan finalTime = list.Last() + slot.Duration;
                            //if (DateTime.Now.TimeOfDay >= finalTime)
                            //{
                            //    var wait = new TimeSpan(24, 0, 1) - DateTime.Now.TimeOfDay;
                            //    log.Info($"Alert Prayer wait next day Task.Delay: {wait}");
                            //    await Task.Delay(wait);
                            //}

                        }
                        log.Info("2");
                    }
                    catch (Exception ex)
                    {
                        log.Info($"ExecuteAlertPrayerAsync Error:{ex.Message}");
                    }

                }

                ));

                log.Info("Cancelled Task");

            }
            log.Info("4");
        }

        private void CalculatePrayerSource(AlertModel alert, int prayer)
        {
            foreach (ControlModel item in alert.Controls.Where(_ => _.Type == ControlType.PrayerImage || _.Type == ControlType.PrayerText || _.Type == ControlType.PrayerVideo))
            {
                var sm = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).FirstOrDefault() as SetContentModel;
                if (sm == null)
                    continue;

                switch (prayer)
                {
                    case 0:
                        sm.Content = item.Fajr;
                        log.Info($"Fajr Content:{sm.Content}");
                        break;
                    case 1:
                        sm.Content = item.Dhuhr ?? item.Fajr;
                        log.Info($"Dhur Content:{sm.Content}");
                        break;
                    case 2:
                        sm.Content = item.Asr ?? item.Dhuhr ?? item.Fajr;
                        log.Info($"Asr Content:{sm.Content}");
                        break;
                    case 3:
                        sm.Content = item.Maghrib ?? item.Asr ?? item.Dhuhr ?? item.Fajr;
                        log.Info($"Maghrib Content:{sm.Content}");
                        break;
                    case 4:
                        sm.Content = item.Isha ?? item.Maghrib ?? item.Asr ?? item.Dhuhr ?? item.Fajr;
                        log.Info($"Isha Content:{sm.Content}");
                        break;
                }
            }
        }

        async Task ExecuteAlertGlobalAsync(Main gui, ProjectModel project)
        {
            //Check Alert
            if (project.Alerts != null && project.Alerts.Count > 0 && project.Alerts.Where(_ => _.Type == AlertType.Global).Count() > 0)
            {
                log.Info(String.Format("Alert Global background thread started. Alert count: {0}", project.Alerts.Where(_ => _.Type == AlertType.Global).Count()));
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    while (true)
                    {
                        TimeSpan nextTime = new TimeSpan(1000, 0, 0, 0);
                        SlotModel nextSlot = null;

                        //Get all data
                        foreach (var alert in project.Alerts.Where(_ => _.Type == AlertType.Global))
                        {
                            foreach (var slot in alert.Slots)
                            {
                                TimeSpan start = slot.Start - DateTime.Now; //start slide time span
                                TimeSpan end = start + slot.Duration; //end slide time span

                                //Slide was ended - skip
                                if (end <= new TimeSpan(0))
                                    continue;

                                //Active - start imidiatly
                                if (start < new TimeSpan(0) && end > new TimeSpan(0))
                                {
                                    if (nextTime > start)
                                    {
                                        nextTime = start;
                                        nextSlot = slot;
                                    }
                                    break;
                                }

                                if (end > new TimeSpan(0))
                                {
                                    if (nextTime > start)
                                    {
                                        nextTime = start;
                                        nextSlot = slot;
                                    }
                                }

                            }
                        }

                        if (nextSlot == null) //Stop thread if all is done
                        {
                            log.Info("No new Global Alerts - stop branch");
                            break;
                        }

                        //Wait to next alert
                        if (nextTime > new TimeSpan(0))
                        {
                            log.Info(String.Format("Alert Global Task.Delay: {0}", nextTime));
                            await Task.Delay(nextTime);
                        }

                        //Run Alert
                        log.Info(String.Format("Alert Global Start {0} current {1} end {2}", nextSlot.Start, DateTime.Now, nextSlot.Start + nextSlot.Duration));
                        if (nextSlot.Start <= DateTime.Now && nextSlot.Start + nextSlot.Duration >= DateTime.Now)
                        {
                            log.Info(String.Format("Start Alert Global {0} Start time {1}", nextSlot.Parent.Name, nextSlot.Start));
                            object tempDataContext = DataContext;
                            int tempMode = this.preview.Mode;
                            this.Background = Brushes.Black;
                            this.preview.Mode = 1; //Alert is without ticker
                            DataContext = nextSlot.Parent; // Alert
                            TimeSpan ts = nextSlot.Start + nextSlot.Duration - DateTime.Now;
                            //TimeSpan ts = new TimeSpan(0, 0, 10);
                            log.Info(String.Format("Alert Global Task.Delay: {0}", ts.ToString()));
                            await Task.Delay(ts);
                            this.preview.Mode = tempMode;
                            DataContext = tempDataContext;
                            log.Info(String.Format("Stop Alert Global {0} End time {1}", nextSlot.Parent.Name, nextSlot.Start + nextSlot.Duration));
                            //break;
                        }

                        await Task.Delay(new TimeSpan(0, 0, 1));
                    }
                }
                ));
            }
        }

        public List<TimeSpan> getPrayerTime(SlotModel slot)
        {
            List<TimeSpan> list = new List<TimeSpan>();
            try
            {
                double lo = Convert.ToDouble(slot.Location.Longnitude);
                double la = Convert.ToDouble(slot.Location.Latitude);

                PrayTime p = new PrayTime();
                int y = 0, m = 0, d = 0, tz = 0;
                DateTime cc = DateTime.Now;
                y = cc.Year;
                m = cc.Month;
                d = cc.Day;
                tz = TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime(y, m, d)).Hours;
                String[] s;
                p.setCalcMethod(4);
                p.setAsrMethod(0);
                s = p.getDatePrayerTimes(y, m, d, la, lo, tz);
                if (s.Length > 0)
                {
                    list.Add(TimeSpan.Parse(s[0])); // Fajr Image
                    list.Add(TimeSpan.Parse(s[2])); // Dhuhr Image
                    list.Add(TimeSpan.Parse(s[3])); // Asr Image
                    list.Add(TimeSpan.Parse(s[4])); // Maghrib Image
                    list.Add(TimeSpan.Parse(s[6])); // Isha Image
                }
            }
            catch (Exception e)
            {
                log.Info($"Alert getPrayerTime error Longitude {slot.Location.Longnitude} , Latitude {slot.Location.Latitude}, error: {e.Message}");
            }
            return list;
        }

        async Task ExecuteAlertAsync(Main gui, ProjectModel project)
        {
            //Check Alert
            if (project.Alerts != null && project.Alerts.Count > 0)
            {
                log.Info(String.Format("Alert background thread started. Alert count: {0}", project.Alerts.Count));
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    while (true)
                    {
                        object tempDataContext = DataContext;
                        int tempMode = this.preview.Mode;
                        foreach (var alert in project.Alerts)
                        {
                            foreach (var slot in alert.Slots)
                            {
                                DateTime endTime = slot.Start.Add(slot.Duration);
                                if (slot.Start < DateTime.Now && endTime > DateTime.Now)
                                {
                                    log.Info(String.Format("Start Alert {0} Slot {1}", alert.Name, slot.Name));
                                    this.Background = Brushes.Black;
                                    this.preview.Mode = 1; //Without ticker
                                    DataContext = alert;
                                    TimeSpan ts = endTime - DateTime.Now;
                                    await Task.Delay(ts);
                                    this.preview.Mode = tempMode;
                                    DataContext = tempDataContext;
                                    log.Info(String.Format("Stop Alert {0} Slot {1}", alert.Name, slot.Name));
                                    break;
                                }
                            }
                        }
                        await Task.Delay(new TimeSpan(0, 0, 1)); // Check every second
                    }
                }
                ));
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Watcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = ConfigurationManager.AppSettings["CeitconDirectory"];
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*"; //
            //watcher.Filter = "*.cdp";

            // Add event handlers.
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        #region Events
        void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                log.Info("------Display settings start-----");
                uint i = 0;
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    i++;
                    var r = screen.Bounds;
                    log.Info(String.Format("Display {0} ; Name: {0} ; Primary: {1} ; Bounds: {2} ; WorkingArea: {3}", i, screen.DeviceName, screen.Primary, screen.Bounds, screen.WorkingArea));
                    if ((r.Width < r.Height && (DefaultOrientation == Display.Orientations.DEGREES_CW_0 || DefaultOrientation == Display.Orientations.DEGREES_CW_180))
                        || (r.Width > r.Height && (DefaultOrientation == Display.Orientations.DEGREES_CW_90 || DefaultOrientation == Display.Orientations.DEGREES_CW_270))
                        || (r.Width < r.Height && (DefaultOrientation == Display.Orientations.DEGREES_CW_90 || DefaultOrientation == Display.Orientations.DEGREES_CW_270))
)
                    {
                        Display.Rotate(i, DefaultOrientation);
                        log.Info(String.Format("Display rotate : {0} : {1}", i, r.ToString()));
                    }
                    //Extra Code Line
                    //log.Info(String.Format("Before Display rotate : {0} ", DefaultOrientation.ToString()));
                    //log.Info(String.Format("Before Display rotate : {0} : {1}", i, r.ToString()));

                    //Display.Rotate(i, DefaultOrientation);
                    //log.Info(String.Format("After Display rotate : {0} ", DefaultOrientation.ToString()));
                    //log.Info(String.Format("After Display rotate : {0} : {1}", i, r.ToString()));
                }

                //Disconect/Connect monitor
                if (totalScreen > System.Windows.Forms.Screen.AllScreens.Count())
                {
                    log.Info(String.Format("Disconected monitor : {0} : {1}", totalScreen, System.Windows.Forms.Screen.AllScreens.Count()));
                }
                else if (totalScreen < System.Windows.Forms.Screen.AllScreens.Count())
                {
                    log.Info(String.Format("Connected monitor : {0} : {1}", totalScreen, System.Windows.Forms.Screen.AllScreens.Count()));
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                    log.Info("Restart appication");
                }
                else if (System.Windows.Forms.Screen.AllScreens.Count() == 1 && changeDisplayCount > 1)
                {
                    var screen = System.Windows.Forms.Screen.AllScreens[0];
                    if (screenString != String.Format("Name: {0} ; Primary: {1} ; Bounds: {2} ; WorkingArea: {3}", screen.DeviceName, screen.Primary, screen.Bounds, screen.WorkingArea))
                    {
                        log.Info("Restart appication - Single monitor");
                        System.Windows.Forms.Application.Restart();
                        System.Windows.Application.Current.Shutdown();
                    }
                }

                if (System.Windows.Forms.Screen.AllScreens.Count() > 0)
                {
                    var screenOne = System.Windows.Forms.Screen.AllScreens[0];
                    screenString = String.Format("Name: {0} ; Primary: {1} ; Bounds: {2} ; WorkingArea: {3}", screenOne.DeviceName, screenOne.Primary, screenOne.Bounds, screenOne.WorkingArea);
                }
                totalScreen = System.Windows.Forms.Screen.AllScreens.Count();
                changeDisplayCount++;

                log.Info("----Display settings end------");
            }
            catch (Exception ex)
            {
                log.Error("System Events Display Settings Changed error", ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Second monitor fix (https://social.msdn.microsoft.com/Forums/vstudio/en-US/75d72afa-2fff-44fd-9f0a-87753002daa1/mediaelement-not-working-with-multiple-monitors?forum=wpf)
                //if (System.Windows.Forms.Screen.AllScreens.Length > 1)
                //{
                //    HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                //    if (hwndSource != null)
                //    {
                //        HwndTarget hwndTarget = hwndSource.CompositionTarget;
                //        hwndTarget.RenderMode = RenderMode.SoftwareOnly;
                //    }
                //}

                Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

                totalScreen = System.Windows.Forms.Screen.AllScreens.Count();
            }
            catch (Exception ex)
            {
                log.Error("Window Loaded error", ex);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            KillProcess(tickerName);
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    string path = Environment.GetCommandLineArgs()[0];
                    KillProcess(tickerName);
                    KillProcess(Path.GetFileNameWithoutExtension(path));
                    e.Handled = true;
                    Application.Current.Shutdown();
                }
                //else if (e.Key == Key.D && (Keyboard.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
                //{
                //    this.WindowState = WindowState.Minimized;
                //    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                log.Error("Handle Key Press error", ex);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (e.Name.EndsWith(".cdp"))
                    {
                        log.Info("CDP file recived");
                        DataContext = null;
                        Init();
                    }
                    else if (e.ChangeType != WatcherChangeTypes.Deleted && e.Name.EndsWith(fidFileName))
                    {
                        log.Info("FID.xml file recived");
                        FileInfo fi1 = new FileInfo(e.FullPath);

                        //Wait 30 seconds 
                        int i = 0;
                        while (IsFileLocked(fi1) && i <= 15)
                        {
                            i++;
                            Thread.Sleep(TimeSpan.FromSeconds(2));
                        }

                        string xmlData = System.IO.File.ReadAllText(e.FullPath);
                        this.preview.FlightList = Ceitcon_Designer.Utilities.SQLiteHelper.Instance.GetFlightsDataFromXml(xmlData);

                    }
                    else if (e.ChangeType != WatcherChangeTypes.Deleted && e.Name.EndsWith(weatherFileName))
                    {
                        log.Info("Weather.xml file recived");
                        FileInfo fi1 = new FileInfo(e.FullPath);

                        //Wait 30 seconds 
                        int i = 0;
                        while (IsFileLocked(fi1) && i <= 15)
                        {
                            i++;
                            Thread.Sleep(TimeSpan.FromSeconds(2));
                        }

                        string location = fi1.Name.Replace(weatherFileName, String.Empty);
                        string xmlData = System.IO.File.ReadAllText(fi1.FullName);
                        if (this.preview.WeatherList.ContainsKey(location))
                            this.preview.WeatherList[location] = xmlData;
                        else
                            this.preview.WeatherList.Add(location, xmlData);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("OnChanged", ex);
                }
            });
        }

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ioex)
            {
                //the file is unavailable because it is: still being written to or being processed by another thread or does not exist (has already been processed)
                log.Info($"Error:{ioex.Message}");
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
        #endregion

        #region Methods

        private bool HaveTickers(RegionModel region)
        {
            try
            {
                if (region == null)
                    return false;
                foreach (var slide in region.Slides)
                {
                    foreach (var layer in slide.Layers)
                    {
                        foreach (var control in layer.Controls)
                        {
                            if (control.Type == ControlType.Ticker)
                                return true;
                        }
                    }
                }
            }
            catch (Exception) { }
            return false;
        }

        public static void RunAsync(string path, string argument)
        {
            Process process = null;
            try
            {
                process = new Process();
                ProcessStartInfo info = new ProcessStartInfo(Path.GetFileName(path), argument);
                process.StartInfo = info;
                process.Start();
            }
            catch (Exception e)
            {
                log.Error("Run application error", e);
                if (process != null)
                    process.Dispose();
            }
            finally
            {
                if (process != null)
                    process.Dispose();
            }
        }

        static void KillProcess(string name)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(name))
                {
                    process.Kill();
                }
            }
            catch (Exception)
            {
            }
        }

        private ProjectModel LoadProject()
        {
            ProjectModel result = null;
            try
            {
                string _projectFileExtension = ".cdp";
                string MediaDirectory = ConfigurationManager.AppSettings["CeitconDirectory"];
                string[] files = Directory.GetFiles(MediaDirectory, "*" + _projectFileExtension);
                if (files.Count() == 1)
                {
                    result = IOManagerProject.LoadProject(files[0]);
                }
            }
            catch (Exception e)
            {
                log.Error("Load Project error", e);
            }
            return result;
        }

        #endregion

        #region CalculateScalarFactor
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }


        private float GetScalingFactor()
        {
            var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            int logpixelsy = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
            float screenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            float dpiScalingFactor = (float)logpixelsy / (float)96;
            return dpiScalingFactor; // 1.25 = 125%
        }
        #endregion
    }
}
