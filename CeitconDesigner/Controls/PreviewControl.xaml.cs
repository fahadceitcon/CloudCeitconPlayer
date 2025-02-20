using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Data;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Designer.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Fixed;
using Telerik.Windows.Documents.FormatProviders.Html;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for PreviewControl.xaml
    /// </summary>
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public partial class PreviewControl : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int preloadingTime = 1000;
        private static int projectNumber = 0;
        private static long slideNumber = 0;
        private static int dataNumber = 0;
        private int icamcalled = 1;

        public PreviewControl()
        {
            InitializeComponent();
            try
            {
                preloadingTime = Convert.ToInt32(ConfigurationManager.AppSettings["PreloadingTime"]);
            }
            catch (Exception) { }
            WeatherList = new Dictionary<string, string>();
        }

        public static readonly DependencyProperty ContentStretchProperty = DependencyProperty.Register
        (
            "ContentStretch",
            typeof(Stretch),
            typeof(PreviewControl),
            new PropertyMetadata(Stretch.Uniform)
        );

        public Stretch ContentStretch
        {
            get { return (Stretch)GetValue(ContentStretchProperty); }
            set
            {
                SetValue(ContentStretchProperty, value);
            }
        }

        public static readonly DependencyProperty FlightListProperty = DependencyProperty.Register
        (
           "FlightList",
           typeof(List<FlightModel>),
           typeof(PreviewControl),
           new PropertyMetadata(null)
        );

        public List<FlightModel> FlightList
        {
            get { return (List<FlightModel>)GetValue(FlightListProperty); }
            set
            {
                SetValue(FlightListProperty, value);
                log.Info("FlightList:" + icamcalled.ToString());
                ParseFlightList();
                icamcalled++;
                dataNumber++;
            }
        }

        public static readonly DependencyProperty WeatherListProperty = DependencyProperty.Register
        (
           "WeatherList",
           typeof(Dictionary<string, string>),
           typeof(PreviewControl),
           new PropertyMetadata(null)
        );

        public Dictionary<string, string> WeatherList
        {
            get { return (Dictionary<string, string>)GetValue(WeatherListProperty); }
            set
            {
                SetValue(WeatherListProperty, value);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            log.Info("UserControl Changed");
            projectNumber++;
            if (DataContext == null)
                this.DesignerCanvas.Children.Clear();
            Run(projectNumber);
        }


        #region PrivateMethods
        private bool GetIsArabic(RegionModel rm)
        {
            bool result = false;
            foreach (SlideModel sm in rm.Slides)
            {
                foreach (LayerModel lm in sm.Layers)
                {
                    foreach (ControlModel cm in lm.Controls.Where(_ => _.Type == ControlType.DataGrid))
                    {
                        result = cm.FlowDirection;
                        if (result) return result;
                    }
                }
            }
            return result;
        }

        private bool GetIsEnglish(RegionModel rm)
        {
            bool result = false;
            foreach (SlideModel sm in rm.Slides)
            {
                foreach (LayerModel lm in sm.Layers)
                {
                    foreach (ControlModel cm in lm.Controls.Where(_ => _.Type == ControlType.DataGrid))
                    {
                        result = !cm.FlowDirection;
                        if (result) return result;
                    }
                }
            }
            return result;
        }

        private void ParseFlightList()
        {
            //Arabic
            try
            {
                if (DataContext is RegionModel && GetIsArabic(DataContext as RegionModel))
                {
                    log.Info("ParseFlightList: I am called Arabic");
                    foreach (FlightModel _flights in FlightList)
                    {
                        _flights.FL_NUMBER_KA = GetArabicData(_flights.FL_NUMBER);
                        int _index = _flights.SCH_TIME.LastIndexOf(" ");
                        if (_index == -1)
                        {

                            _flights.SCH_TIME_KA = GetArabicData(_flights.SCH_TIME);
                        }
                        else
                        {
                            _flights.FL_NUMBER_KA = GetArabicData(_flights.FL_NUMBER);
                            string sData = _flights.SCH_TIME.Substring(_flights.SCH_TIME.LastIndexOf(" ") + 1);
                            string sData1 = sData.Substring(0, 2);
                            string sData2 = sData.Substring(2, 2);
                            _flights.SCH_TIME_KA = GetArabicData(sData1) + ":" + GetArabicData(sData2);
                        }
                        int _index2 = _flights.EST_TIME.LastIndexOf(" ");
                        if (_index2 == -1)
                        {


                            _flights.EST_TIME_KA = GetArabicData(_flights.EST_TIME);
                        }
                        else
                        {
                            string sData2x = _flights.EST_TIME.Substring(_flights.EST_TIME.LastIndexOf(" ") + 1);
                            string sData11 = sData2x.Substring(0, 2);
                            string sData12 = sData2x.Substring(2, 2);
                            _flights.EST_TIME_KA = GetArabicData(sData11) + ":" + GetArabicData(sData12);
                        }
                        if (_flights.GATE_1.Trim() != "")
                        {
                            string _Gate = "";
                            if (_flights.GATE_1.ToLower().StartsWith("gt"))
                            {
                                _Gate = _flights.GATE_1.Substring(2);
                            }
                            else
                                _Gate = _flights.GATE_1;

                            bool result = _Gate.Any(x => !char.IsLetter(x));
                            if (result == false)
                                _flights.GATE_1_KA = GetArabicData(_Gate);
                            else
                            {
                                string sddd = _Gate.Substring(0, (_Gate.Length));
                                string ddd = "";
                                for (int i = 0; i < sddd.Length; i++)
                                {
                                    if (char.IsDigit(sddd[i]))
                                    {
                                        ddd = ddd + sddd[i];
                                    }
                                    else
                                    {
                                        ddd = sddd[i] + " " + ddd;
                                    }
                                }
                                _flights.GATE_1_KA = GetArabicData(ddd);
                            }
                        }

                        _flights.BAGGAGE_1_KA = GetArabicData(_flights.BAGGAGE_1);
                        _flights.TERMINAL_KA = GetArabicData(_flights.TERMINAL);
                        string sT = GetArabicData(_flights.TERMINAL);
                        if (sT.Length > 1)
                        {
                            _flights.TERMINAL_KA = sT[1] + " " + sT[0];
                        }
                        else
                            _flights.TERMINAL_KA = sT;

                    }
                }

                if (DataContext is RegionModel && GetIsEnglish(DataContext as RegionModel))
                {
                    log.Info("ParseFlightList: I am called English");
                    try
                    {
                        foreach (FlightModel _flights in FlightList)
                        {
                            int _index = _flights.SCH_TIME.LastIndexOf(" ");
                            if (_index != -1)
                            {

                                try
                                {
                                    string sData = _flights.SCH_TIME.Substring(_flights.SCH_TIME.LastIndexOf(" ") + 1);
                                    string sData1 = sData.Substring(0, 2);
                                    string sData2 = sData.Substring(2, 2);
                                    _flights.SCH_TIME = sData1 + ":" + sData2;
                                    //log.Info("SCH_TIME" + _flights.SCH_TIME.ToString());
                                }
                                catch (Exception exs)
                                {
                                    log.Error("Fligh_SCh:", exs);
                                }

                            }
                            else
                            {
                                //log.Info("Index:" + _index.ToString() + ":_SCH_TIME:" + _flights.SCH_TIME);
                                string sDAta = _flights.SCH_TIME;
                            }

                            if (_flights.GATE_1.Trim() != "")
                            {
                                try
                                {
                                    if (_flights.GATE_1.ToLower().StartsWith("gt"))
                                    {
                                        _flights.GATE_1 = _flights.GATE_1.Substring(2);
                                    }
                                    else
                                    {
                                        //log.Info("GATE_1:" + _flights.GATE_1);
                                    }
                                }
                                catch (Exception exd)
                                {
                                    log.Error("GATe:" + exd);
                                }
                            }

                            int _index2 = _flights.EST_TIME.LastIndexOf(" ");
                            if (_index2 != -1)
                            {
                                try
                                {
                                    string sData2x = _flights.EST_TIME.Substring(_flights.EST_TIME.LastIndexOf(" ") + 1);
                                    string sData11 = sData2x.Substring(0, 2);
                                    string sData12 = sData2x.Substring(2, 2);
                                    _flights.EST_TIME = sData11 + ":" + sData12;
                                    //log.Info("EST_TIME:" + _flights.EST_TIME.ToString());
                                }
                                catch (Exception exf)
                                {
                                    log.Error("EST_time:", exf);
                                }

                            }
                            else
                            {
                                //log.Info("Index:" + _index2.ToString() + ":EST_TIME:" + _flights.EST_TIME);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Error Parese english", e);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("ParseFlightList:", ex);
            }
        }

        private async Task ConvertContent(RegionModel region)
        {
            try
            {
                foreach (SlideModel slide in region.Slides)
                {
                    foreach (LayerModel layer in slide.Layers)
                    {
                        foreach (ControlModel control in layer.Controls)
                        {
                            if (control.Type == ControlType.PPT)
                            {
                                SetContentModel sm = control.Playlist.Where(_ => _.Type == PlaylistType.SetContent).FirstOrDefault() as SetContentModel;
                                PPTHelper.ConvertToImages(sm.Content, Convert.ToInt32(control.Width), Convert.ToInt32(control.Height));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private async void Run(int projectId)
        {
            if (DataContext != null && (DataContext is RegionModel || DataContext is AlertModel))
            {
                slideNumber = 0;
                int shown = 0;

                ParseFlightList();

                while (DataContext is RegionModel)
                {
                    //Check for new content
                    if (projectId != projectNumber)
                        return;

                    Cursor tempC = Mouse.OverrideCursor;
                    Mouse.OverrideCursor = Cursors.Wait;
                    await ConvertContent(DataContext as RegionModel);
                    Mouse.OverrideCursor = tempC;

                    int slideCount = (DataContext as RegionModel).Slides.Count;
                    if (slideCount == 0) return;
                    log.Info("Total Slide Count: " + slideCount);
                    SlideModel nextSlide;

                    for (int i = 0; i < slideCount; i++)
                    {
                        //Check for new content
                        if (projectId != projectNumber)
                            return;

                        slideNumber++;
                        SlideModel slide = null;
                        log.Info("Total Slide Count: " + (DataContext as RegionModel).Slides.Count + " Slide Number: " + slideNumber + " i: " + i);
                        if (i >= (DataContext as RegionModel).Slides.Count)
                        //slide = (DataContext as RegionModel).Slides[i - 1];
                        {
                            slide = (DataContext as RegionModel).Slides[0];
                            //i = 0;
                        }
                        else
                            slide = (DataContext as RegionModel).Slides[i];
                        try
                        {
                            nextSlide = i + 1 < slideCount ? (DataContext as RegionModel).Slides[i + 1] : (DataContext as RegionModel).Slides[0];
                        }
                        catch (Exception e)
                        {
                            nextSlide = (DataContext as RegionModel).Slides[0];
                            log.Error(e);
                        }


                        log.Info(String.Format("Show slide {0}, duration {1}", slide.Name, slide.Duration));
                        ClearCanvas();

                        if (!DrawCanvas(slide, false, slideNumber == 1 ? true : false))
                        {
                            log.Info(String.Format("No content in slide {0}, go to next slide.", slide.Name));
                            continue;
                        }
                        else
                        {
                            shown++;
                        }

                        if (slide.Forever)
                            return;

                        //Calculate current video
                        int currentVideoTime = 0;
                        int cpl = CountVideo(slide);
                        if (cpl > 1)
                        {
                            currentVideoTime = (cpl - 1) * 1000;
                        }
                        TimeSpan cpltime = new TimeSpan(0, 0, 0, 0, preloadingTime + currentVideoTime);


                        //Calculate preloading video
                        int multipleVideoTime = 0;
                        int pl = CountVideo(nextSlide);
                        if (pl > 1)
                        {
                            multipleVideoTime = (pl - 1) * 1000;
                        }
                        TimeSpan pltime = new TimeSpan(0, 0, 0, 0, preloadingTime + multipleVideoTime);

                        TimeSpan slideDuration;//time before preloading
                        if (slideNumber == 0)
                        {
                            slideDuration = (new TimeSpan(0, 0, (int)slide.Duration.TotalSeconds)).Add(pltime);
                        }
                        else
                        {
                            slideDuration = (new TimeSpan(0, 0, (int)slide.Duration.TotalSeconds)).Add(-pltime).Add(-cpltime);
                        }

                        await Task.Delay(slideDuration);

                        //Check for new content
                        if (projectId != projectNumber)
                            return;

                        //Start preloading video
                        DrawCanvas(nextSlide, true);
                        await Task.Delay(pltime);
                        if (DataContext is AlertModel)
                            return;
                    }
                    if (shown == 0)
                    {
                        log.Info(String.Format("There is not any content."));
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        slideNumber = 0;
                        // return;
                    }
                    shown = 0;
                }

                if (DataContext is AlertModel)
                {
                    var alert = DataContext as AlertModel;
                    {
                        log.Info(String.Format("Show alert {0}", alert.Name));
                        ClearCanvas();
                        DrawCanvas(alert);
                    }
                }
            }
        }

        private int CountVideo(SlideModel sm)
        {
            int result = 0;
            foreach (LayerModel lm in sm.Layers)
            {
                foreach (ControlModel cm in lm.Controls)
                {
                    if (cm.Type == ControlType.Video)
                        result++;
                }
            }
            return result;
        }
        #endregion

        #region Canvas

        private bool DrawCanvas(SlideModel slide, bool preloading, bool showAll = false)
        {
            bool result = false;

            if (preloading)
            {
                //Loding video for next slide
                foreach (LayerModel layer in slide.Layers)
                {
                    foreach (ControlModel control in layer.Controls)
                    {
                        if (control.Type == ControlType.Video)
                        {
                            bool tresult = CreateDesignerCanvas(control, true, false);
                        }
                    }
                }
            }
            else
            {
                //Load current slide
                foreach (LayerModel layer in slide.Layers)
                {
                    foreach (ControlModel control in layer.Controls)
                    {
                        if (control.Type == ControlType.Video)
                        {
                            if (showAll) //First slide
                            {
                                bool tresult = CreateDesignerCanvas(control, false, false);
                                if (!result && tresult)//if any content file existing in slide
                                    result = true;
                            }
                            else
                            {
                                //Video opacity is automaticly changd in clear process
                                if (!result && File.Exists(control.Source))
                                    result = true;

                                CreateDesignerCanvas(control, false, true);//GORAN ADDED
                            }
                        }
                        else
                        {
                            bool tresult = CreateDesignerCanvas(control, false, false);
                            if (!result && tresult)//if any content file existing in slide
                                result = true;
                        }
                    }
                }
            }
            return result;
        }

        private void DrawCanvas(AlertModel alert)
        {
            foreach (ControlModel control in alert.Controls)
            {
                CreateDesignerCanvas(control, false, false);
            }
        }

        //Run on slide end
        private void ClearCanvas()
        {
            //Clear all elements except video, video can clear.
            List<string> opacityList = new List<string>();
            List<string> hidenList = new List<string>();
            List<double> statusList = new List<double>();

            for (int i = 0; i < this.DesignerCanvas.Children.Count; i++)
            {
                statusList.Add((this.DesignerCanvas.Children[i] as ContentControl).Opacity);

                if ((this.DesignerCanvas.Children[i] as ContentControl).Opacity == 1)
                {

                    opacityList.Add((this.DesignerCanvas.Children[i] as ContentControl).Name);
                    this.DesignerCanvas.Children.RemoveAt(i);
                    i--;
                }
                else
                {
                    hidenList.Add((this.DesignerCanvas.Children[i] as ContentControl).Name);
                    this.DesignerCanvas.Children[i].Opacity = 1;
                }
            }
        }

        //Run for multiple content
        async Task ExecuteSourceAsync(ControlModel control, TimeSpan delay, int project, long slide, bool preloader)
        {
            if (control != null)
            {
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        if (projectNumber > project || slideNumber > slide)
                            return;

                        if (control.Type == ControlType.Video)
                            await Task.Delay(delay - new TimeSpan(preloadingTime));
                        else
                            await Task.Delay(delay);

                        if (projectNumber > project || slideNumber > slide)
                            return;

                        slide = preloader ? slide + 1 : slide;
                        log.Info($"Execute Source Async {control.Name} slide {slide} source {control.Source}");

                        CreateDesignerCanvas(control, false, false);

                        if (control.Type == ControlType.Video)
                            await Task.Delay(new TimeSpan(preloadingTime));//wait one second before finish preloading

                        //Delete old content
                        log.Info($"ExecuteSourceAsync - RemoveItemFromCanvas{control.Name}slide{slide}");
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }
                }
                ));
            }
        }

        private bool RemoveItemFromCanvas(string name)
        {
            foreach (var item in DesignerCanvas.Children)
            {
                var contentControl = item as ContentControl;

                if (contentControl.Name == name)
                {
                    if ((contentControl?.DataContext as ControlModel).Type == ControlType.Video)
                    {
                        MediaElement me = ((contentControl.Content as DockPanel)?.Children[0] as Border)?.Child as MediaElement;
                        me.Stop();
                        me.Source = null;
                    }
                    log.Info($"Remove Item From Canvas {contentControl.Name} source {(contentControl.DataContext as ControlModel).Source}");
                    this.DesignerCanvas.Children.Remove(contentControl);
                    return true;
                }
            }
            return false;
        }

        async Task RegreshSourceAsync(ControlModel control, TimeSpan delay, long activSlide, int pageSize)
        {
            try
            {
                int page = 1;
                int localDataNuber = dataNumber; //Flight list id
                if (control != null)
                {
                    DateTime start = DateTime.Now;
                    await Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        try
                        {
                            ContentControl cc = null;
                            int co = 1;

                        Again:

                            TimeSpan ts = (start + TimeSpan.FromTicks(delay.Ticks * co)) - DateTime.Now;
                            co++;
                            log.Info(String.Format("dalay: {0}", ts.ToString()));
                            if (ts.Milliseconds > 0)
                                await Task.Delay(ts);

                            if (this.DataContext == null || activSlide != slideNumber)
                            {
                                return;
                            }

                            if (localDataNuber < dataNumber)
                            {
                                localDataNuber = dataNumber;
                                page = 1; //return to first page, we have new data colection
                            }
                            else
                            {
                                page++; //Go to another
                            }

                            //DateTime t1 = DateTime.Now;
                            for (int i = 0; i < this.DesignerCanvas.Children.Count; i++)
                            {
                                if ((this.DesignerCanvas.Children[i] as ContentControl).Name == control.Name)
                                {

                                    cc = this.DesignerCanvas.Children[i] as ContentControl;
                                    var dg = ((((cc.Content as DockPanel).Children[0] as Border).Child as Grid).Children[0] as DataGrid);
                                    SetContentModel sm = (cc.DataContext as ControlModel).Playlist.Where(_ => _.Type == PlaylistType.SetContent).FirstOrDefault() as SetContentModel;
                                    List<DataColumnModel> conditions = sm.DataGrid.SelectedSource.Columns.Where(_ => (!String.IsNullOrEmpty(_.WhereOperator) && !String.IsNullOrEmpty(_.WhereValue)) || (_.TimeFilters.Count > 0)).ToList();
                                    List<DataColumnModel> sorts = sm.DataGrid.SelectedSource.Columns.Where(_ => _.Sort > 0).ToList();
                                    pageSize = sm.DataGrid.MaxRows == 0 ? 10 : sm.DataGrid.MaxRows;
                                    dg.ItemsSource = NextRecords(conditions, sorts, control.FlowDirection, pageSize, ref page);
                                }
                            }
                            //DateTime t2 = DateTime.Now;
                            //TimeSpan ts = t2 - t1;
                            //Console.Write(ts.TotalSeconds);
                            //Console.Write(ts.TotalMilliseconds);

                            if (cc == null)
                            {
                                return;
                            }
                            goto Again;

                        }
                        catch (Exception e)
                        {
                            log.Error(e);
                        }
                    }
                    ));
                }
            }
            catch (Exception ex)
            {
                log.Debug("RegreshSourceAsync:" + ex.Message);
            }
        }

        async Task StopSourceAsync(ControlModel control, TimeSpan duration, long slide, bool preloader)
        {
            if (control != null)
            {
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        //if (preloader /*&& control.Type == ControlType.Video*/)
                        //    await Task.Delay(delay + new TimeSpan(preloadingTime));
                        //else
                        //    await Task.Delay(delay);
                        await Task.Delay(duration);
                        log.Info($"StopSourceAsync control:{control.Name} slide: {slide}");
                        RemoveItemFromCanvas($"{control.Name}slide{slide}");
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }
                }
                ));
            }
        }

        async Task RunWeatherAsync(WeatherControl wc, TimeSpan delay)
        {
            long slide = slideNumber;
            await Dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    while (slideNumber == slide && DataContext != null)
                    {
                        if (WeatherList != null && WeatherList.Count > 0)
                        {
                            if (WeatherList.ContainsKey(wc.Location))
                            {
                                wc.GetWeather(WeatherList[wc.Location]);
                            }
                        }
                        await Task.Delay(delay);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
            ));
        }

        async Task TimerAsync(UIElement cc, ControlModel cm, TimeSpan delay)
        {
            long slide = slideNumber;
            string filter = String.Empty;
            if (!String.IsNullOrWhiteSpace(cm.CustomDateTimeFormat))
            {
                filter = cm.CustomDateTimeFormat;
            }
            else
            {
                switch (cm.DateTimeFormat)
                {
                    case 0:
                        {   //DateTime
                            filter = "dd/MM/yyyy HH:mm:ss";
                        }
                        break;
                    case 1:
                        {   //Date
                            filter = "dd/MM/yyyy";
                        }
                        break;
                    case 2:
                        {   //Time
                            filter = "HH:mm:ss";
                        }
                        break;
                }
            }
            await Dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    while (slideNumber == slide && cc != null && DataContext != null)
                    {
                        if (cm.FlowDirection)
                        {
                            (cc as TextBlock).Text = GetArabicData(DateTime.Now.ToString(filter, new CultureInfo("ar-SA")));
                        }
                        else
                        {
                            (cc as TextBlock).Text = DateTime.Now.ToString(filter);
                        }
                        await Task.Delay(delay);
                    }
                }
                catch (Exception e)
                {
                    (cc as TextBlock).Text = "Wrong custom format";
                    log.Error(e);
                }
            }
            ));
        }

        private List<FlightModel> NextRecords(List<DataColumnModel> conditions, List<DataColumnModel> sorts, bool isArabic, int pageSize, ref int page)
        {
            log.Info("Next Record : " + pageSize);
            List<FlightModel> list = FlightList;
            int skip = 0;
            try
            {
                //Conditions
                if (conditions != null && conditions.Count > 0)
                {
                    foreach (DataColumnModel item in conditions)
                    {
                        //Where filter
                        if (!String.IsNullOrWhiteSpace(item.WhereValue))
                        {
                            switch (item.WhereOperator)
                            {
                                case "==":
                                    list = list.Where(_ => _.GetPropertyValue(item.Name).ToUpper() == item.WhereValue.ToUpper()).ToList();
                                    break;
                                case "!=":
                                    list = list.Where(_ => _.GetPropertyValue(item.Name).ToUpper() != item.WhereValue.ToUpper()).ToList();
                                    break;
                            }
                        }

                        //Time filter
                        if (item.TimeFilters.Count > 0)
                        {
                            DateTime start = DateTime.Now - new TimeSpan(0, item.TimeFilters.FirstOrDefault().BeforeDuration, 0);
                            DateTime end = DateTime.Now + new TimeSpan(0, item.TimeFilters.FirstOrDefault().AfterDuration, 0);
                            list = list.Where(_ => (DateTime.ParseExact(_.GetPropertyValue(item.Name + "_O"), "ddMMyyyy HHmmss", CultureInfo.InvariantCulture) >= start)
                            && DateTime.ParseExact(_.GetPropertyValue(item.Name + "_O"), "ddMMyyyy HHmmss", CultureInfo.InvariantCulture) <= end).ToList();
                        }
                    }
                }

                //Sorts
                foreach (DataColumnModel item in sorts)
                {
                    if (item.Sort == 1)
                        list = list.OrderBy(_ => _.GetType().GetProperty(item.Name).GetValue(_, null)).ToList();
                    else if (item.Sort == 2)
                        list = list.OrderByDescending(_ => _.GetType().GetProperty(item.Name).GetValue(_, null)).ToList();
                }


                //Pagging
                int total = list.Count();
                skip = pageSize * (page - 1);
                if (skip >= total)
                {
                    page = 1;
                    skip = 0;
                }
            }
            catch (Exception e)
            {
                log.Error("Next Record :Eroor", e);
            }

            log.Info("skip" + skip.ToString() + ":pagesize:" + pageSize.ToString());
            return list.Skip(skip).Take(pageSize).ToList();
        }

        private string GetArabicColumName(string name)
        {
            switch (name)
            {
                case "FL_NUMBER":
                case "SCH_TIME":
                case "EST_TIME":
                case "GATE_1":
                case "BAGGAGE_1":
                case "TERMINAL":
                    return String.Format("{0}_KA", name);
                default:
                    return name;
            }
        }

        private string GetArabicData(string _input)
        {
            string sResult = "";
            foreach (char item in _input)
            {
                switch (item)
                {
                    case '1':
                        sResult = sResult + "١";
                        break;
                    case '2':
                        sResult = sResult + "٢";
                        break;
                    case '3':
                        sResult = sResult + "٣";
                        break;
                    case '4':
                        sResult = sResult + "٤";
                        break;
                    case '5':
                        sResult = sResult + "٥";
                        break;
                    case '6':
                        sResult = sResult + "٦";
                        break;
                    case '7':
                        sResult = sResult + "٧";
                        break;
                    case '8':
                        sResult = sResult + "٨";
                        break;
                    case '9':
                        sResult = sResult + "٩";
                        break;
                    case '0':
                        sResult = sResult + "٠";
                        break;

                    default:
                        sResult = sResult + item.ToString();
                        break;
                }
            }

            return sResult;
        }

        private bool CreateDesignerCanvas(ControlModel item, bool preloader, bool skipFirst)
        {
            try
            {
                if (item == null)
                    return false;

                //Check content
                SetContentModel setContentModel = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).FirstOrDefault() as SetContentModel;
                string fileName = setContentModel.Content;
                if (item.Type == ControlType.Image ||
                    item.Type == ControlType.Video ||
                    item.Type == ControlType.GifAnim ||
                    item.Type == ControlType.PDF ||
                    item.Type == ControlType.PPT ||
                    item.Type == ControlType.PrayerImage ||
                    item.Type == ControlType.PrayerVideo)
                {
                    if (!File.Exists(fileName))
                    {
                        log.Info(String.Format("Content missing {0}", fileName));
                        return false;
                    }
                }

                if (item.Type == ControlType.Image ||
                    item.Type == ControlType.Video ||
                    item.Type == ControlType.GifAnim ||
                    item.Type == ControlType.PDF ||
                    item.Type == ControlType.PPT)
                {
                    if (setContentModel.ContentSize != (new FileInfo(fileName).Length))
                    {
                        log.Info(String.Format("Content size are not same {0}", fileName));
                        return false;
                    }
                }

                DockPanel grid = new DockPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                //Border
                Border border = new Border()
                {
                    Margin = new Thickness(0, 0, 0, 0),
                    Background = item.Background,
                    //BorderBrush = new SolidColorBrush(item.BorderBrush),
                    BorderBrush = item.BorderBrush,
                    BorderThickness = item.BorderThickness,
                    CornerRadius = item.CornerRadius,
                    Opacity = item.Opacity,
                    IsHitTestVisible = false
                };
                grid.Children.Add(border);

                //Content
                UIElement controlObject = null;
                switch (item.Type)
                {
                    case ControlType.Image:
                    case ControlType.PrayerImage:
                        {
                            controlObject = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                Source = new BitmapImage(new Uri(setContentModel.Content)),
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                Stretch = item.Stretch,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false
                            };

                            if (item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count() > 1)
                            {
                                for (int i = 0; i < item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count(); i++)
                                {
                                    if (i == 0)
                                        continue;
                                    var c = new ControlModel(item, item.Parent, true);
                                    var sc = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).ElementAt(i) as SetContentModel;
                                    c.Playlist.Clear();
                                    c.Playlist.Add(sc);
                                    Task.Run(() => ExecuteSourceAsync(c, sc.StartTime, projectNumber, slideNumber, preloader));
                                }
                            }

                            if (!setContentModel.Forever && setContentModel.Duration > TimeSpan.Zero)
                            {
                                var c = new ControlModel(item, item.Parent, true);
                                Task.Run(() => StopSourceAsync(c, setContentModel.Duration, slideNumber, preloader));
                            }
                        }
                        break;
                    case ControlType.RichText:
                        {
                            controlObject = new RadRichTextBox()
                            {
                                BorderThickness = new Thickness(0),
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsSpellCheckingEnabled = false,
                                FontSize = item.FontSize,
                                Foreground = item.Foreground,
                                Background = item.Background,
                                FontFamily = item.FontFamily,
                                HorizontalAlignment = HorizontalAlignment.Stretch,//item.HorizontalAlignment,
                                VerticalAlignment = VerticalAlignment.Stretch,// item.VerticalAlignment,
                                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false,
                                IsReadOnly = true,
                                Name = "richTextBox",
                            };
                            var HtmlDataProvider = new HtmlDataProvider()
                            {
                                Html = setContentModel.Content.StartsWith("topic:") ? item.Text : setContentModel.Content,
                            };
                            HtmlDataProvider.SetBinding(HtmlDataProvider.RichTextBoxProperty, new Binding() { Source = controlObject });

                            if (item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count() > 1)
                            {
                                for (int i = 0; i < item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count(); i++)
                                {
                                    if (i == 0)
                                        continue;
                                    var c = new ControlModel(item, item.Parent, true);
                                    var sc = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).ElementAt(i) as SetContentModel;
                                    c.Playlist.Clear();
                                    c.Playlist.Add(sc);
                                    Task.Run(() => ExecuteSourceAsync(c, sc.StartTime, projectNumber, slideNumber, preloader));
                                }
                            }

                            if (!setContentModel.Forever && setContentModel.Duration > TimeSpan.Zero)
                            {
                                var c = new ControlModel(item, item.Parent, true);
                                Task.Run(() => StopSourceAsync(c, setContentModel.Duration, slideNumber, preloader));
                            }
                        }
                        break;
                    case ControlType.Video:
                    case ControlType.PrayerVideo:
                        {
                            if (item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count() > 1 && !preloader)
                            {
                                for (int i = 0; i < item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count(); i++)
                                {
                                    if (i == 0)
                                        continue;
                                    var c = new ControlModel(item, item.Parent, true);
                                    var sc = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).ElementAt(i) as SetContentModel;
                                    c.Playlist.Clear();
                                    c.Playlist.Add(sc);
                                    c.Source = sc.Content;//Need for log
                                    Task.Run(() => ExecuteSourceAsync(c, sc.StartTime, projectNumber, slideNumber, preloader));
                                }
                            }

                            if (!setContentModel.Forever && setContentModel.Duration > TimeSpan.Zero)
                            {
                                var c = new ControlModel(item, item.Parent, true);
                                Task.Run(() => StopSourceAsync(c, setContentModel.Duration, slideNumber, preloader));
                            }

                            if (skipFirst)
                                break;

                            controlObject = new MediaElement()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                Source = new Uri(setContentModel.Content),
                                //new Uri(sm.Content),
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                Stretch = item.Stretch,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false,
                                Width = item.Width,
                                Height = item.Height,
                                ScrubbingEnabled = true,
                                IsMuted = setContentModel.IsMuted,
                                LoadedBehavior = MediaState.Manual
                            };

                            (controlObject as MediaElement).Play();

                            (controlObject as MediaElement).MediaEnded += new RoutedEventHandler(me_MediaEnded);
                        }
                        break;
                    case ControlType.GifAnim:
                        {
                            controlObject = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                Stretch = item.Stretch,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false
                            };

                            var image = new BitmapImage();
                            image.BeginInit();
                            image.UriSource = new Uri((item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).FirstOrDefault() as SetContentModel).Content);
                            image.EndInit();
                            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(controlObject as Image, image);

                            if (item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count() > 1)
                            {
                                for (int i = 0; i < item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count(); i++)
                                {
                                    if (i == 0)
                                        continue;
                                    var c = new ControlModel(item, item.Parent, true);
                                    var sc = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).ElementAt(i) as SetContentModel;
                                    c.Playlist.Clear();
                                    c.Playlist.Add(sc);
                                    Task.Run(() => ExecuteSourceAsync(c, sc.StartTime, projectNumber, slideNumber, preloader));
                                }
                            }

                            if (!setContentModel.Forever && setContentModel.Duration > TimeSpan.Zero)
                            {
                                var c = new ControlModel(item, item.Parent, true);
                                Task.Run(() => StopSourceAsync(c, setContentModel.Duration, slideNumber, preloader));
                            }
                        }
                        break;
                    case ControlType.Ticker:
                        {
                            controlObject = new TickerControl()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                Text = setContentModel.Content.StartsWith("topic:") ? item.Text : setContentModel.Content,//item.Text,
                                InvertDirection = item.InvertDirection,
                                Duration = item.Duration,
                                Width = (int)item.Width,
                                Height = (int)item.Height,
                                CurrentWidth = (int)item.CurrentWidth,
                                CurrentHeight = (int)item.CurrentHeight,
                                FontSize = (int)item.FontSize,
                                Foreground = item.Foreground,
                                Background = item.Background,
                                FontFamily = item.FontFamily,
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                FontWeight = item.FontWeight,
                                FontStyle = item.FontStyle,
                                TextDecorations = item.TextDecoration,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false
                            };

                            //Create another sources treads()
                            if (item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count() > 1)
                            {
                                for (int i = 0; i < item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count(); i++)
                                {
                                    if (i == 0)
                                        continue;
                                    var c = new ControlModel(item, item.Parent, true);
                                    var sc = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).ElementAt(i) as SetContentModel;
                                    c.Playlist.Clear();
                                    c.Playlist.Add(sc);
                                    Task.Run(() => ExecuteSourceAsync(c, sc.StartTime, projectNumber, slideNumber, preloader));
                                }
                            }

                            if (!setContentModel.Forever && setContentModel.Duration > TimeSpan.Zero)
                            {
                                var c = new ControlModel(item, item.Parent, true);
                                Task.Run(() => StopSourceAsync(c, setContentModel.Duration, slideNumber, preloader));
                            }
                        }
                        break;

                    case ControlType.DataGrid:
                        {
                            if (setContentModel == null || setContentModel.DataGrid == null || setContentModel.DataGrid.SelectedSource == null)
                                break;

                            //Get next record
                            List<DataColumnModel> conditions = setContentModel.DataGrid.SelectedSource.Columns.Where(_ => (!String.IsNullOrEmpty(_.WhereOperator) && !String.IsNullOrEmpty(_.WhereValue)) || (_.TimeFilters.Count > 0)).ToList();
                            List<DataColumnModel> sorts = setContentModel.DataGrid.SelectedSource.Columns.Where(_ => _.Sort > 0).ToList();
                            //pageSize = sm.DataGrid.MaxRows == 0 ? 10 : sm.DataGrid.MaxRows;
                            //page = 1;

                            //Create canvas item
                            Grid s = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                            RowDefinition c1 = new RowDefinition();
                            c1.Height = new GridLength(0, GridUnitType.Auto);
                            RowDefinition c2 = new RowDefinition();
                            c2.Height = new GridLength(1, GridUnitType.Star);
                            s.RowDefinitions.Add(c1);
                            s.RowDefinitions.Add(c2);
                            s.Width = (int)item.Width;
                            s.Height = (int)item.Height;

                            s.FlowDirection = item.FlowDirection ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                            int page = 1;
                            var dataGrid = new DataGrid()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Background = Brushes.Transparent,
                                CanUserAddRows = false,
                                IsReadOnly = true,
                                //IsSynchronizedWithCurrentItem = true,
                                AutoGenerateColumns = false,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                RowHeaderWidth = 0,
                                BorderThickness = new Thickness(0),
                                BorderBrush = Brushes.Transparent,
                                FlowDirection = item.FlowDirection ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                GridLinesVisibility = setContentModel.DataGrid.LinesVisibility,
                                HorizontalGridLinesBrush = setContentModel.DataGrid.HorizontalLineColour,
                                VerticalGridLinesBrush = setContentModel.DataGrid.VerticalLineColour,
                                ColumnHeaderHeight = setContentModel.DataGrid.HeaderHeight,
                                RowHeight = setContentModel.DataGrid.RowHeight,
                                HeadersVisibility = DataGridHeadersVisibility.Column,//Remove buton in first colum
                                                                                     //VerticalContentAlignment = VerticalAlignment.Center,

                                //EnableColumnVirtualization = true,
                                //EnableRowVirtualization = true,
                                //RowBackground = new ImageBrush(new BitmapImage(new Uri(@"../Images/iconAlert.png", UriKind.Relative))),
                                MaxWidth = item.Width,
                                MaxHeight = item.Height,
                                ItemsSource = NextRecords(conditions, sorts, item.FlowDirection, setContentModel.DataGrid.MaxRows == 0 ? 10 : setContentModel.DataGrid.MaxRows, ref page),
                            };


                            //Alternation
                            if (setContentModel.DataGrid.AlternationCount > 1)
                            {
                                dataGrid.AlternationCount = setContentModel.DataGrid.AlternationCount;
                                dataGrid.AlternatingRowBackground = setContentModel.DataGrid.AlternatingRowBackground;
                                dataGrid.RowBackground = setContentModel.DataGrid.RowBackground;
                                //dataGrid.AlternatingRowBackground = gradient;//sm.DataGrid.AlternatingRowBackgroundBrush;
                            }

                            dataGrid.SetValue(Grid.RowProperty, 1);

                            //Styles
                            Style styleHeader = new Style
                            {
                                TargetType = typeof(DataGridColumnHeader)
                            };

                            //Corner Radius and Margin Template
                            if (setContentModel.DataGrid.HeaderCornerRadius != new CornerRadius(0) || setContentModel.DataGrid.HeaderMargin != new Thickness(0))
                            {
                                ControlTemplate ct = new ControlTemplate();
                                FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
                                borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(DataGridColumnHeader.BackgroundProperty));
                                borderFactory.SetValue(Border.BorderBrushProperty, setContentModel.DataGrid.HeaderBorderBrush);
                                borderFactory.SetValue(Border.BorderThicknessProperty, setContentModel.DataGrid.HeaderBorderThickness);
                                borderFactory.SetValue(Border.MarginProperty, setContentModel.DataGrid.HeaderMargin);
                                borderFactory.SetValue(Border.CornerRadiusProperty, setContentModel.DataGrid.HeaderCornerRadius);
                                if (setContentModel.DataGrid.HeaderIsVisibleShadow)
                                    borderFactory.SetValue(Border.EffectProperty, setContentModel.DataGrid.HeaderShadowEffect);//Shedow
                                FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));
                                FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                                textBlockFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                                textBlockFactory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                                textBlockFactory.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(GridViewRowPresenter.ContentProperty));
                                gridFactory.AppendChild(textBlockFactory);
                                borderFactory.AppendChild(gridFactory);
                                ct.VisualTree = borderFactory;
                                styleHeader.Setters.Add(new Setter(DataGridColumnHeader.TemplateProperty, ct));
                            }

                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, setContentModel.DataGrid.HeaderBackground));
                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, setContentModel.DataGrid.HeaderForeground));
                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, setContentModel.DataGrid.HeaderHorizontalAlignment));
                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.FontFamilyProperty, setContentModel.DataGrid.HeaderFontFamily));
                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.FontSizeProperty, (double)setContentModel.DataGrid.HeaderSize));
                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.FontStyleProperty, setContentModel.DataGrid.HeaderFontStyle));
                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.FontWeightProperty, setContentModel.DataGrid.HeaderFontWeight));
                            if (setContentModel.DataGrid.LinesVisibility == DataGridGridLinesVisibility.All || setContentModel.DataGrid.LinesVisibility == DataGridGridLinesVisibility.Vertical)
                            {
                                styleHeader.Setters.Add(new Setter(DataGridColumnHeader.BorderThicknessProperty, new Thickness(0, 0, 1, 0)));
                                styleHeader.Setters.Add(new Setter(DataGridColumnHeader.BorderBrushProperty, setContentModel.DataGrid.VerticalLineColour));
                            }
                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.BorderThicknessProperty, setContentModel.DataGrid.HeaderBorderThickness));
                            styleHeader.Setters.Add(new Setter(DataGridColumnHeader.BorderBrushProperty, setContentModel.DataGrid.HeaderBorderBrush));
                            dataGrid.ColumnHeaderStyle = styleHeader;

                            Style style = new Style
                            {
                                TargetType = typeof(DataGridRow)
                            };

                            //Row Template
                            if (setContentModel.DataGrid.RowCornerRadius != new CornerRadius(0) || setContentModel.DataGrid.RowMargin != new Thickness(0))
                            {
                                ControlTemplate ctr = new ControlTemplate();
                                FrameworkElementFactory rBorderFactory = new FrameworkElementFactory(typeof(Border));
                                rBorderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(DataGridRow.BackgroundProperty));
                                rBorderFactory.SetValue(Border.BorderBrushProperty, setContentModel.DataGrid.BorderBrush);
                                rBorderFactory.SetValue(Border.BorderThicknessProperty, setContentModel.DataGrid.BorderThickness);
                                rBorderFactory.SetValue(Border.MarginProperty, setContentModel.DataGrid.RowMargin);
                                rBorderFactory.SetValue(Border.CornerRadiusProperty, setContentModel.DataGrid.RowCornerRadius);
                                rBorderFactory.SetValue(Border.PaddingProperty, new Thickness(setContentModel.DataGrid.RowCornerRadiusTopLeft / 2, 0, setContentModel.DataGrid.RowCornerRadiusTopRight / 2, 0));
                                if (setContentModel.DataGrid.IsVisibleShadow)
                                    rBorderFactory.SetValue(Border.EffectProperty, setContentModel.DataGrid.RowShadowEffect);//Shedow
                                var dgcp = new FrameworkElementFactory(typeof(DataGridCellsPresenter));
                                dgcp.SetValue(DataGridCellsPresenter.ItemsPanelProperty, new TemplateBindingExtension(ItemsControl.ItemsPanelProperty));
                                dgcp.SetValue(DataGridCellsPresenter.SnapsToDevicePixelsProperty, new TemplateBindingExtension(UIElement.SnapsToDevicePixelsProperty));
                                rBorderFactory.AppendChild(dgcp);
                                ctr.VisualTree = rBorderFactory;
                                style.Setters.Add(new Setter(DataGridRow.TemplateProperty, ctr));
                            }
                            style.Setters.Add(new Setter(DataGridRow.BackgroundProperty, setContentModel.DataGrid.RowBackground));
                            style.Setters.Add(new Setter(DataGridRow.ForegroundProperty, item.Foreground));
                            style.Setters.Add(new Setter(DataGridRow.FontFamilyProperty, item.FontFamily));
                            style.Setters.Add(new Setter(DataGridRow.FontSizeProperty, item.FontSize));
                            style.Setters.Add(new Setter(DataGridRow.FontStyleProperty, item.FontStyle));
                            style.Setters.Add(new Setter(DataGridRow.FontWeightProperty, item.FontWeight));
                            style.Setters.Add(new Setter(DataGridRow.BorderThicknessProperty, setContentModel.DataGrid.BorderThickness));
                            style.Setters.Add(new Setter(DataGridRow.BorderBrushProperty, setContentModel.DataGrid.BorderBrush));
                            dataGrid.ItemContainerStyle = style;

                            //Headers
                            dataGrid.Columns.Clear();

                            //Calculate pixel size
                            double totalSize = 0;
                            foreach (var column in setContentModel.DataGrid.SelectedSource.Columns)
                            {
                                if (column.IsVisible)
                                {
                                    totalSize += column.Width;
                                }
                            }
                            double pixelSize = item.Width / totalSize;

                            //Row triger
                            List<DataTrigger> rowDTList = new List<DataTrigger>();

                            foreach (var column in setContentModel.DataGrid.SelectedSource.Columns)
                            {
                                string columnName = column.Name;
                                if (item.FlowDirection)
                                    columnName = GetArabicColumName(column.Name);

                                if (column.IsVisible)
                                {
                                    if (column.Type == 1)
                                    {
                                        DataGridTemplateColumn imgColumn = new DataGridTemplateColumn();
                                        imgColumn.Header = column.Title;
                                        FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));

                                        imageFactory.SetBinding(Image.SourceProperty, new Binding("IMAGE"));
                                        imageFactory.SetValue(Image.StretchProperty, column.ImageStretch);
                                        DataTemplate dataTemplate = new DataTemplate();
                                        dataTemplate.VisualTree = imageFactory;
                                        imgColumn.CellTemplate = dataTemplate;
                                        imgColumn.Width = (column.Width * pixelSize);
                                        dataGrid.Columns.Add(imgColumn);
                                    }
                                    else
                                    {
                                        DataGridTextColumn dgtc = null;
                                        if (String.IsNullOrEmpty(column.MergeColumn))
                                        {
                                            dgtc = new DataGridTextColumn() { Header = column.Title, Width = column.Width * pixelSize, Binding = new Binding(columnName) };
                                        }
                                        else
                                        {
                                            DataColumnModel col = setContentModel.DataGrid.SelectedSource.Columns.Where(_ => _.Name.ToUpper() == column.MergeColumn.ToUpper()).FirstOrDefault();
                                            if (col == null)
                                            {
                                                dgtc = new DataGridTextColumn() { Header = column.Title, Width = column.Width * pixelSize, Binding = new Binding(columnName) };
                                            }
                                            else
                                            {
                                                MultiBinding multiBinding = new MultiBinding();
                                                multiBinding.Mode = BindingMode.OneWay;
                                                multiBinding.Converter = new Ceitcon_Designer.Converters.MultiValueConverter();
                                                multiBinding.ConverterParameter = item.FlowDirection;
                                                if (item.FlowDirection)
                                                {
                                                    multiBinding.Bindings.Add(new Binding() { Path = new PropertyPath(columnName), Mode = BindingMode.OneWay });
                                                    multiBinding.Bindings.Add(new Binding() { Path = new PropertyPath(GetArabicColumName(col.Name)), Mode = BindingMode.OneWay });
                                                }
                                                else
                                                {
                                                    multiBinding.Bindings.Add(new Binding() { Path = new PropertyPath(columnName), Mode = BindingMode.OneWay });
                                                    multiBinding.Bindings.Add(new Binding() { Path = new PropertyPath(col.Name), Mode = BindingMode.OneWay });
                                                }
                                                multiBinding.NotifyOnSourceUpdated = true;


                                                dgtc = new DataGridTextColumn() { Header = column.Title, Width = column.Width * pixelSize, Binding = multiBinding };
                                            }
                                        }

                                        Style sty = new Style
                                        {
                                            TargetType = typeof(TextBlock)
                                        };
                                        //sty.Setters.Add(new Setter(TextBlock.HeightProperty, 30));
                                        sty.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, column.TextAlignment));
                                        sty.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
                                        sty.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Stretch));
                                        sty.Setters.Add(new Setter(TextBlock.FontFamilyProperty, column.FontFamily == null ? item.FontFamily : column.FontFamily));
                                        sty.Setters.Add(new Setter(TextBlock.FontSizeProperty, column.FontSize == 0 ? item.FontSize : column.FontSize));
                                        sty.Setters.Add(new Setter(TextBlock.FontStyleProperty, column.FontStyle == null ? item.FontStyle : column.FontStyle));
                                        sty.Setters.Add(new Setter(TextBlock.FontWeightProperty, column.FontWeight == null ? item.FontWeight : column.FontWeight));
                                        sty.Setters.Add(new Setter(TextBlock.BackgroundProperty, column.Background));
                                        sty.Setters.Add(new Setter(TextBlock.ForegroundProperty, column.Foreground));
                                        //sty.Setters.Add(new Setter(TextBlock.TextWrappingProperty, true));

                                        //Alinght
                                        try
                                        {
                                            if (setContentModel.DataGrid.SelectedSource.Columns.Where(_ => _.IsVisible).Count() > 0)
                                            {
                                                if (setContentModel.DataGrid.SelectedSource.Columns.Where(_ => _.IsVisible && _.Type == 1).Any())
                                                {
                                                    double imgWidth = setContentModel.DataGrid.SelectedSource.Columns.Where(_ => _.IsVisible && _.Type == 1).FirstOrDefault().Width;
                                                    //double rowHight = sm.DataGrid.RowMarginTop + sm.DataGrid.RowMarginBottom + sm.DataGrid.RowHeight;
                                                    double pad =/* (imgWidth * pixelSize) * 0.390625*/ setContentModel.DataGrid.RowHeight - (column.FontSize > 0 ? column.FontSize : item.FontSize) * 1.3333;

                                                    if (pad > 0)
                                                    {
                                                        if (column.VerticalAlignment == VerticalAlignment.Center)
                                                        {
                                                            sty.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(0, pad / 2, 0, pad / 2)));
                                                        }
                                                        else if (column.VerticalAlignment == VerticalAlignment.Bottom)
                                                        {
                                                            sty.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(0, pad, 0, 0)));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    double pad = setContentModel.DataGrid.RowHeight - (column.FontSize > 0 ? column.FontSize : item.FontSize) * 1.3333;
                                                    if (pad > 0)
                                                    {
                                                        if (column.VerticalAlignment == VerticalAlignment.Center)
                                                        {
                                                            sty.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(0, pad / 2, 0, pad / 2)));
                                                        }
                                                        else if (column.VerticalAlignment == VerticalAlignment.Bottom)
                                                        {
                                                            sty.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(0, pad, 0, 0)));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception) { }


                                        //Colour by Value
                                        if (column.SpecialCells.Count > 0)
                                        {
                                            foreach (var scell in column.SpecialCells)
                                            {
                                                if (String.IsNullOrEmpty(scell.Text))
                                                    continue;

                                                if (scell.IsBlink)//This is for blink
                                                {
                                                    var animation = new DoubleAnimation(0.3, 1, new Duration(new TimeSpan(0, 0, 1)));
                                                    animation.RepeatBehavior = RepeatBehavior.Forever;
                                                    Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
                                                    Storyboard blinkAnimation = new Storyboard() { Children = { animation } };
                                                    var action = new BeginStoryboard() { Storyboard = blinkAnimation };

                                                    DataTrigger dtrigger = new DataTrigger()
                                                    {
                                                        Binding = new Binding(columnName), //This is column for compare
                                                        Value = scell.Text, //This is value ( if dada value = scell.Text then apply filter )
                                                        Setters = {// this is colours
                                                                new Setter(TextBlock.BackgroundProperty, scell.Background == null ? column.Background : scell.Background),
                                                                new Setter(TextBlock.ForegroundProperty, scell.Foreground == null ? column.Foreground : scell.Foreground),
                                                            },
                                                        EnterActions = { action }
                                                    };
                                                    sty.Triggers.Add(dtrigger);
                                                    if (scell.IsRow)
                                                        rowDTList.Add(dtrigger);
                                                }
                                                else
                                                {
                                                    DataTrigger dtrigger = new DataTrigger()
                                                    {
                                                        Binding = new Binding(columnName),
                                                        Value = scell.Text,
                                                        Setters = {
                                                                new Setter(TextBlock.BackgroundProperty, scell.Background == null ? column.Background : scell.Background),
                                                                new Setter(TextBlock.ForegroundProperty, scell.Foreground == null ? column.Foreground : scell.Foreground)}
                                                    };
                                                    sty.Triggers.Add(dtrigger);
                                                    if (scell.IsRow)
                                                        rowDTList.Add(dtrigger);
                                                }
                                            }
                                        }
                                        dgtc.ElementStyle = sty;
                                        dataGrid.Columns.Add(dgtc);
                                    }
                                }
                            }

                            //This is for row - just copy triger
                            //Check row colours
                            foreach (var dtrigger in rowDTList)
                            {
                                foreach (DataGridColumn column in dataGrid.Columns)
                                {
                                    if (column is DataGridTextColumn)
                                    {
                                        (column as DataGridTextColumn).ElementStyle.Triggers.Add(dtrigger);
                                    }
                                }
                            }
                            s.Children.Add(dataGrid);
                            controlObject = s;
                            log.Info($"Refresh time {setContentModel.DataGrid.RefreshTime} : MAxRow: {setContentModel.DataGrid.MaxRows}");
                            Task.Run(() => RegreshSourceAsync(item, new TimeSpan(0, 0, setContentModel.DataGrid.RefreshTime < 1 ? 1 : setContentModel.DataGrid.RefreshTime), slideNumber, setContentModel.DataGrid.MaxRows == 0 ? 10 : setContentModel.DataGrid.MaxRows));


                        }
                        break;

                    case ControlType.Alert:
                        {
                            controlObject = new AlertControl()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                Text = setContentModel.Content,
                                Width = (int)item.Width,
                                Height = (int)item.Height,
                                FontSize = (int)item.FontSize,
                                Foreground = item.Foreground,
                                Background = item.Background,
                                FontFamily = item.FontFamily,
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false
                            };

                            if (item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count() > 1)
                            {
                                for (int i = 0; i < item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count(); i++)
                                {
                                    if (i == 0)
                                        continue;
                                    var c = new ControlModel(item, item.Parent, true);
                                    var sc = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).ElementAt(i) as SetContentModel;
                                    c.Playlist.Clear();
                                    c.Playlist.Add(sc);
                                    Task.Run(() => ExecuteSourceAsync(c, sc.StartTime, projectNumber, slideNumber, preloader));
                                }
                            }

                            if (!setContentModel.Forever && setContentModel.Duration > TimeSpan.Zero)
                            {
                                var c = new ControlModel(item, item.Parent, true);
                                Task.Run(() => StopSourceAsync(c, setContentModel.Duration, slideNumber, preloader));
                            }
                        }
                        break;

                    case ControlType.Likebox:
                    case ControlType.Facebook:
                    case ControlType.Twitter:
                    case ControlType.Instagram:
                        {
                            controlObject = new ScrollViewer()
                            {
                                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                                Width = item.Width
                            };
                            TextBlock text = new TextBlock()
                            {
                                TextWrapping = TextWrapping.Wrap,
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                FontSize = (int)item.FontSize,
                                Foreground = item.Foreground,
                                Background = item.Background,
                                //Foreground = new SolidColorBrush(item.Foreground),
                                //Background = new SolidColorBrush(item.Background),
                                FontFamily = item.FontFamily,
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                RenderTransform = (Transform)item.ScaleTransform,
                            };
                            text.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Text"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            (controlObject as ScrollViewer).Content = text;
                            border.Child = controlObject;
                        }
                        break;

                    case ControlType.Youtube:
                    case ControlType.PrayerYoutube:
                        {
                            Uri link = setContentModel.Content == null ? item.Url : new Uri(item.Url, $"embed/{setContentModel.Content}?autoplay=1");
                            controlObject = new WebBrowser()
                            {
                                Source = link,
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                IsHitTestVisible = false
                            };
                            (controlObject as WebBrowser).Unloaded += new RoutedEventHandler(web_Unloaded);
                            border.Child = controlObject;
                        }
                        break;

                    case ControlType.SocialMediaImage:
                        {
                            controlObject = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                Source = new BitmapImage(new Uri(item.Source, UriKind.Relative)),
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                Stretch = item.Stretch,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false
                            };
                        }
                        break;

                    case ControlType.Live:
                        {
                            //controlObject = new WebEye.StreamPlayerControl()
                            controlObject = new RTSPControl.LivePlayer()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                Visibility = Visibility.Visible,
                                DataContext = setContentModel.Content,
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                RenderTransform = (Transform)item.ScaleTransform,
                                Width = item.Width,
                                Height = item.Height,


                                IsHitTestVisible = false,
                            };

                            //(controlObject as WebEye.StreamPlayerControl).Loaded += LoadStream;
                            //(controlObject as WebEye.StreamPlayerControl).Unloaded += UnloadedStream;
                            (controlObject as RTSPControl.LivePlayer).Loaded += LoadedRTSP;
                            (controlObject as RTSPControl.LivePlayer).Unloaded += UnLoadedRTSP;
                        }
                        break;

                    case ControlType.Text:
                    case ControlType.PrayerText:
                        {
                            controlObject = new TextBlock()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                TextWrapping = TextWrapping.Wrap,
                                Text = setContentModel.Content,
                                Width = (int)item.Width,
                                Height = (int)item.Height,
                                FontSize = (int)item.FontSize,
                                Foreground = item.Foreground,
                                Background = item.Background,
                                FontFamily = item.FontFamily,
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false
                            };

                            if (item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count() > 1)
                            {
                                for (int i = 0; i < item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count(); i++)
                                {
                                    if (i == 0)
                                        continue;
                                    var c = new ControlModel(item, item.Parent, true);
                                    var sc = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).ElementAt(i) as SetContentModel;
                                    c.Playlist.Clear();
                                    c.Playlist.Add(sc);
                                    Task.Run(() => ExecuteSourceAsync(c, sc.StartTime, projectNumber, slideNumber, preloader));
                                }
                            }

                            if (!setContentModel.Forever && setContentModel.Duration > TimeSpan.Zero)
                            {
                                var c = new ControlModel(item, item.Parent, true);
                                Task.Run(() => StopSourceAsync(c, setContentModel.Duration, slideNumber, preloader));
                            }
                        }
                        break;

                    case ControlType.Weather:
                        {
                            var wc = new WeatherControl()
                            {
                                FlowDirection = item.FlowDirection ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                Location = setContentModel.Content,
                                ItemCount = item.ItemCount,
                                WeatherSettings = setContentModel.Weather,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false
                            };
                            if (WeatherList != null && WeatherList.Count > 0)
                            {
                                if (WeatherList.ContainsKey(wc.Location))
                                {
                                    wc.GetWeather(WeatherList[wc.Location]);
                                }
                            }

                            controlObject = new Viewbox() { Stretch = Stretch.Fill, Child = wc };

                            if (item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count() > 1)
                            {
                                for (int i = 0; i < item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).Count(); i++)
                                {
                                    if (i == 0)
                                        continue;
                                    var c = new ControlModel(item, item.Parent, true);
                                    var sc = item.Playlist.Where(_ => _.Type == PlaylistType.SetContent).ElementAt(i) as SetContentModel;
                                    c.Playlist.Clear();
                                    c.Playlist.Add(sc);
                                    Task.Run(() => ExecuteSourceAsync(c, sc.StartTime, projectNumber, slideNumber, preloader));
                                }
                            }

                            if (!setContentModel.Forever && setContentModel.Duration > TimeSpan.Zero)
                            {
                                var c = new ControlModel(item, item.Parent, true);
                                Task.Run(() => StopSourceAsync(c, setContentModel.Duration, slideNumber, preloader));
                            }
                        }
                        break;

                    case ControlType.DateTime:
                        {
                            controlObject = new TextBlock()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                TextWrapping = TextWrapping.Wrap,
                                Text = String.Empty,
                                Width = (int)item.Width,
                                Height = (int)item.Height,
                                FontSize = (int)item.FontSize,
                                Foreground = item.Foreground,
                                Background = item.Background,
                                //Foreground = new SolidColorBrush(item.Foreground),
                                //Background = new SolidColorBrush(item.Background),
                                FontFamily = item.FontFamily,
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false
                            };

                            Task.Run(() => TimerAsync(controlObject, item, TimeSpan.FromSeconds(1)));
                        }
                        break;

                    case ControlType.PDF:
                        {
                            Uri location = location = new Uri(setContentModel.Content, System.UriKind.RelativeOrAbsolute);
                            //if (item.Type == ControlType.PPT)
                            //{
                            //    string file = PPTHelper.ConvertToPDF((obj as SetContentModel).Content);
                            //    if(!String.IsNullOrEmpty(file))
                            //        location = new Uri(file, System.UriKind.RelativeOrAbsolute);
                            //}
                            //else
                            //    location = new Uri((obj as SetContentModel).Content, System.UriKind.RelativeOrAbsolute);

                            controlObject = new RadPdfViewer()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                Width = item.Width,
                                Height = item.Height,
                                Foreground = item.Foreground,
                                Background = item.Background,
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                //Stretch = item.Stretch,
                                RenderTransform = (Transform)item.ScaleTransform,
                                IsHitTestVisible = false,
                                ScaleMode = ConvertStringToFit(setContentModel.DocumentFit),
                                DocumentSource = new PdfDocumentSource(location),
                            };

                            Task.Run(async () =>
                            {
                                try
                                {
                                    bool firstPass = true;
                                    bool toTop = true;
                                    long slide = slideNumber;
                                    TimeSpan interval = TimeSpan.FromSeconds(setContentModel.PageDuration > 0 ? setContentModel.PageDuration : 2);

                                    while (true)
                                    {
                                        if (slideNumber != slide)
                                            break;

                                        await this.Dispatcher.Invoke(async () =>
                                        {
                                            if (controlObject != null && (controlObject as RadPdfViewer).IsLoaded && (controlObject as RadPdfViewer).PagesCount > 1)
                                            {
                                                if (firstPass)
                                                {
                                                    (controlObject as RadPdfViewer).ScaleMode = ConvertStringToFit(setContentModel.DocumentFit);
                                                    //(controlObject as RadPdfViewer).VerticalScrollBar.Visibility = Visibility.Hidden;
                                                    //(controlObject as RadPdfViewer).HorizontalScrollBar.Visibility = Visibility.Hidden;
                                                    firstPass = false;
                                                }

                                                int cp = (controlObject as RadPdfViewer).CurrentPageNumber;
                                                if (toTop || ((controlObject as RadPdfViewer).CurrentPageNumber >= (controlObject as RadPdfViewer).PagesCount))
                                                {
                                                    (controlObject as RadPdfViewer).GoToPage(1);
                                                    toTop = false;
                                                }
                                                else
                                                {
                                                    (controlObject as RadPdfViewer).PageDown();
                                                    await Task.Delay(100);
                                                    if (cp == (controlObject as RadPdfViewer).CurrentPageNumber)
                                                        toTop = true;
                                                }

                                            }
                                        });

                                        if (firstPass)
                                        {
                                            await Task.Delay(100);
                                        }
                                        else
                                        {
                                            await Task.Delay(interval);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            });
                        }
                        break;

                    case ControlType.WebBrowser:
                        {
                            Uri url = String.IsNullOrEmpty(setContentModel.Content) ? null : new Uri(setContentModel.Content);
                            controlObject = new WebBrowser()
                            {
                                Source = url,
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                HorizontalAlignment = item.HorizontalAlignment,
                                VerticalAlignment = item.VerticalAlignment,
                                IsHitTestVisible = false
                            };
                            (controlObject as WebBrowser).Unloaded += new RoutedEventHandler(web_Unloaded);
                            border.Child = controlObject;
                        }
                        break;

                    case ControlType.PPT:
                        {
                            string[] files = PPTHelper.ConvertToImages(setContentModel.Content, Convert.ToInt32(item.Width), Convert.ToInt32(item.Height));
                            if (files == null || files.Length == 0)
                                return false;

                            var c = new ControlModel(item, item.Parent, true);
                            c.Type = ControlType.Image;
                            c.Playlist.Clear();
                            for (int i = 0; i < files.Length; i++)
                            {
                                c.Playlist.Add(new SetContentModel(c)
                                {
                                    Content = files[i],
                                    ContentSize = new System.IO.FileInfo(files[i]).Length,
                                    Duration = TimeSpan.FromSeconds(setContentModel.PageDuration),
                                    Forever = setContentModel.Forever,
                                    StartTime = TimeSpan.FromSeconds(setContentModel.PageDuration * i)
                                });
                            }

                            Task.Run(() => ExecuteSourceAsync(c, new TimeSpan(0), projectNumber, slideNumber, preloader));
                            return true;
                            //controlObject = new Image()
                            //{
                            //    RenderTransformOrigin = new Point(0.5, 0.5),
                            //    //Source = new BitmapImage(new Uri((obj as SetContentModel).Content)),
                            //    Source = new BitmapImage(new Uri(files[0])),
                            //    HorizontalAlignment = item.HorizontalAlignment,
                            //    VerticalAlignment = item.VerticalAlignment,
                            //    Stretch = item.Stretch,
                            //    RenderTransform = (Transform)item.ScaleTransform,
                            //    IsHitTestVisible = false
                            //};

                            //if (files.Length > 1)
                            //{
                            //    for (int i = 0; i < files.Length; i++)
                            //    {
                            //        if (i == 0)
                            //            continue;
                            //        var c = new ControlModel(item, item.Parent, true);
                            //        c.Type = ControlType.Image;
                            //        c.Playlist.Clear();
                            //        c.Playlist.Add(new SetContentModel(c) { Content = files[i] });

                            //        Task.Run(() => ExecuteSourceAsync(c, TimeSpan.FromSeconds(sm.PageDuration * i), slideNumber, preloader));
                            //    }
                            //}

                            //if (!sm.Forever && sm.Duration > TimeSpan.Zero)
                            //{
                            //    var c = new ControlModel(item, item.Parent, true);
                            //    Task.Run(() => StopSourceAsync(c, sm.Duration, slideNumber, preloader));
                            //}
                        }
                        break;

                    default:
                        //border.Child = new Rectangle() { Fill = fillBrush, Stroke = stokeBrush, StrokeThickness = 5, StrokeDashArray = { 4, 2 }, IsHitTestVisible = false };
                        break;
                }

                ///Other playlists
                foreach (var playItem in item.Playlist)
                {
                    switch (playItem.Type)
                    {
                        case PlaylistType.SetContent:
                            {
                                //ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();
                                //animation.BeginTime = TimeSpan.FromSeconds(0);
                                //Storyboard.SetTarget(animation, image);
                                //Storyboard.SetTargetProperty(animation, new PropertyPath("(Image.Source)"));
                                //DiscreteObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame(BitmapFrame.Create(uri), TimeSpan.FromSeconds(0.7));
                                //animation.KeyFrames.Add(keyFrame);
                                //myStoryboard.Children.Add(animation);
                                //controlObject.Begin();
                            }
                            break;
                        case PlaylistType.Delay:
                            {
                                //var animation = new DoubleAnimation(0, 1, playItem.Duration);
                                //animation.BeginTime = playItem.StartTime;
                                //border.BeginAnimation(Border.VisibilityProperty, animation);
                            }
                            break;
                        case PlaylistType.AnimateMargin:
                            {
                                var animation = new ThicknessAnimation((playItem as AnimateMarginModel).MarginThicknessFrom, (playItem as AnimateMarginModel).MarginThicknessTo, playItem.Duration);
                                animation.BeginTime = playItem.StartTime;
                                border.BeginAnimation(Border.BorderThicknessProperty, animation);
                            }
                            break;
                        case PlaylistType.AnimateOpacity:
                            {
                                var animation = new DoubleAnimation((playItem as AnimateOpacityModel).OpacityFrom, (playItem as AnimateOpacityModel).OpacityTo, playItem.Duration);
                                animation.BeginTime = playItem.StartTime;
                                border.BeginAnimation(Border.OpacityProperty, animation);
                            }
                            break;
                        case PlaylistType.AnimateWidth:
                            {
                                var animation = new DoubleAnimation((playItem as AnimateWidthModel).WidthFrom, (playItem as AnimateWidthModel).WidthTo, playItem.Duration);
                                animation.BeginTime = playItem.StartTime;
                                border.BeginAnimation(Border.WidthProperty, animation);
                            }
                            break;
                        case PlaylistType.AnimateHeight:
                            {
                                var animation = new DoubleAnimation((playItem as AnimateHeightModel).HeightFrom, (playItem as AnimateHeightModel).HeightTo, playItem.Duration);
                                animation.BeginTime = playItem.StartTime;
                                border.BeginAnimation(Border.HeightProperty, animation);
                            }
                            break;
                        case PlaylistType.AnimateBorder:
                            {
                                var animation = new ThicknessAnimation((playItem as AnimateBorderModel).BorderThicknessFrom, (playItem as AnimateBorderModel).BorderThicknessTo, playItem.Duration);
                                animation.BeginTime = playItem.StartTime;
                                border.BeginAnimation(Border.BorderThicknessProperty, animation);
                            }
                            break;
                        case PlaylistType.SuspendPlayback:
                            {
                                var animation = new DoubleAnimation(1, 0, playItem.Duration);
                                animation.BeginTime = playItem.StartTime;
                                border.BeginAnimation(Border.VisibilityProperty, animation);
                            }
                            break;
                        case PlaylistType.ResumePlayback:
                            {
                                var animation = new DoubleAnimation(0, 1, playItem.Duration);
                                animation.BeginTime = playItem.StartTime;
                                border.BeginAnimation(Border.VisibilityProperty, animation);
                            }
                            break;
                    }
                }

                border.Child = controlObject;
                long sn = preloader ? slideNumber + 1 : slideNumber;
                ContentControl cc = new ContentControl()
                {
                    Name = $"{item.Name}slide{sn}",
                    Width = item.Width,
                    Height = item.Height,
                    Visibility = Visibility.Visible,
                    Content = grid,
                    DataContext = item,
                };

                cc.Content = grid;
                Canvas.SetLeft(cc, item.X);
                Canvas.SetTop(cc, item.Y);
                Canvas.SetZIndex(cc, item.ZIndex);

                //Video is preloading object
                if (preloader)
                    cc.Opacity = 0;

                this.DesignerCanvas.Children.Add(cc);
                log.Info(String.Format("Show element {0}", item.Name));
            }
            catch (Exception e)
            {
                log.Error("Create element error: {0}", e);
                return false;
            }
            return true;
        }

        //private void PDFControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    int a = (sender as RadPdfViewer).PagesCount;
        //    int ba = (sender as RadPdfViewer).CurrentPageNumber;
        //    bool ba2 = (sender as RadPdfViewer).IsLoaded;
        //    (sender as RadPdfViewer).PageDown();
        //}

        Telerik.Windows.Documents.Fixed.UI.ScaleMode ConvertStringToFit(string value)
        {
            switch (value)
            {
                case "Normal":
                    return Telerik.Windows.Documents.Fixed.UI.ScaleMode.Normal;
                case "Fit To Page":
                    return Telerik.Windows.Documents.Fixed.UI.ScaleMode.FitToPage;
                case "Fit To Width":
                    return Telerik.Windows.Documents.Fixed.UI.ScaleMode.FitToWidth;
            }
            return Telerik.Windows.Documents.Fixed.UI.ScaleMode.Normal;
        }

        void web_Unloaded(object sender, EventArgs e)
        {
            if (sender != null)
                (sender as WebBrowser).Dispose();
        }

        void me_MediaEnded(object sender, EventArgs e)
        {
            //play video again
            (sender as MediaElement).Position = TimeSpan.FromSeconds(0);
        }
        void UnLoadedRTSP(object sender, RoutedEventArgs e)
        {
            try
            {
                log.Info(String.Format("Stop stream"));

                (sender as RTSPControl.LivePlayer).StopPlay();
                (sender as RTSPControl.LivePlayer).Loaded -= LoadStream;
                (sender as RTSPControl.LivePlayer).Unloaded -= UnloadedStream;


            }
            catch (Exception ex)
            {
                log.Error("UnloadedStream: {0}", ex);
            }
        }
        private void LoadedRTSP(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = new Uri((sender as RTSPControl.LivePlayer).DataContext.ToString());
                log.Info(String.Format("Start RTSP stream {0}", uri.AbsoluteUri));
                log.Info(String.Format("Start RTSP isVisible {0}", (sender as RTSPControl.LivePlayer).IsVisible.ToString()));
                (sender as RTSPControl.LivePlayer).StartPlay(uri);
            }
            catch (Exception ex)
            {
                log.Error("LoadedRTSP: {0}", ex);
            }
        }

        void LoadStream(object sender, RoutedEventArgs e)
        {
            //var uri = new Uri(@"rtsp://184.72.239.149/vod/mp4:BigBuckBunny_115k.mov");
            try
            {
                var uri = new Uri((sender as WebEye.StreamPlayerControl).DataContext.ToString());
                log.Info(String.Format("Start stream {0}", uri.AbsoluteUri));
                (sender as WebEye.StreamPlayerControl).StartPlay(uri, TimeSpan.FromSeconds(100));
            }
            catch (Exception ex)
            {
                log.Error("LoadStream: {0}", ex);
            }
        }

        void UnloadedStream(object sender, RoutedEventArgs e)
        {
            try
            {
                log.Info(String.Format("Stop stream"));
                var obj = sender as WebEye.StreamPlayerControl;
                (sender as WebEye.StreamPlayerControl).Stop();
                (sender as WebEye.StreamPlayerControl).Loaded -= LoadStream;
                (sender as WebEye.StreamPlayerControl).Unloaded -= UnloadedStream;
                (sender as WebEye.StreamPlayerControl).Dispose();

            }
            catch (Exception ex)
            {
                log.Error("UnloadedStream: {0}", ex);
            }
        }
        #endregion

        #region Property
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register
        (
             "Mode",
             typeof(int),
             typeof(PreviewControl),
             new FrameworkPropertyMetadata(0, new PropertyChangedCallback(Mode_Changed))
        );

        //0 - All, 1 - Without ticker, 2 - Ticker
        public int Mode
        {
            get { return (int)GetValue(ModeProperty); }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

        private static void Mode_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = sender as PreviewControl;
                if (control != null)
                {
                    control.OnModeChanged();
                }
            }
            catch { }
        }

        protected virtual void OnModeChanged()
        {
            OnPropertyChanged("Mode");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }

    public enum ShowModeEnum
    {
        All = 0,
        WithoutTickers = 1,
        Tickers = 2,
        SeparateTickets = 3,
    }

    class DispatcherTaskScheduler : TaskScheduler
    {
        private readonly Dispatcher dispatcher;
        private readonly DispatcherPriority priority;

        public DispatcherTaskScheduler(
            Dispatcher dispatcher, DispatcherPriority priority)
        {
            this.dispatcher = dispatcher;
            this.priority = priority;
        }

        protected override void QueueTask(Task task)
        {
            dispatcher.BeginInvoke(new Action(() => TryExecuteTask(task)), priority);
        }

        protected override bool TryExecuteTaskInline(
            Task task, bool taskWasPreviouslyQueued)
        {
            // don't support inlining; inling would make sense if somebody blocked
            // the UI thread waiting for a Task that was scheduled on this scheduler
            // and we wanted to avoid the deadlock
            return false;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // this is only useful for debugging, so we can ignore it
            throw new NotSupportedException();
        }
    }
}