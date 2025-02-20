using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;


namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Preview()
        {
            InitializeComponent();
            this.KeyDown += HandleKeyPress;
            this.Cursor = Cursors.None;
            var s = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var r = s.Bounds;
            this.Top = r.Top;
            this.Left = r.Left;
            this.Width = r.Width;
            this.Height = r.Height;
            log.Info(String.Format("Preview Init > Top: {0}, Left: {1}, Width: {2}, Height: {3}", r.Top, r.Left, r.Width, r.Height));
        }

        public static readonly DependencyProperty FlightListProperty = DependencyProperty.Register
        (
           "FlightList",
           typeof(List<FlightModel>),
           typeof(Preview),
           new PropertyMetadata(null)
        );

        public List<FlightModel> FlightList
        {
            get { return (List<FlightModel>)GetValue(FlightListProperty); }
            set
            {
                SetValue(FlightListProperty, value);
                if(value != null)
                    preview.FlightList = value;
            }
        }

        public static readonly DependencyProperty WeatherListProperty = DependencyProperty.Register
        (
           "WeatherList",
           typeof(Dictionary<string, string>),
           typeof(Preview),
           new PropertyMetadata(null)
        );

        public Dictionary<string, string> WeatherList
        {
            get { return (Dictionary<string, string>)GetValue(WeatherListProperty); }
            set
            {
                SetValue(WeatherListProperty, value);
                if (value != null)
                    preview.WeatherList = value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Second monitor fix (https://social.msdn.microsoft.com/Forums/vstudio/en-US/75d72afa-2fff-44fd-9f0a-87753002daa1/mediaelement-not-working-with-multiple-monitors?forum=wpf)
            if (System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
                HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                if (hwndSource != null)
                {
                    HwndTarget hwndTarget = hwndSource.CompositionTarget;
                    hwndTarget.RenderMode = RenderMode.SoftwareOnly;
                }
            }

            Mouse.OverrideCursor = Cursors.None;

            try
            {
                var window = Window.GetWindow(this);
                var region = (DataContext as RegionModel);

                if (region != null)
                {
                    log.Info(String.Format("Preview Region > Top: {0}, Left: {1}, Width: {2}, Height: {3}", region.X, region.Y, region.Width, region.Height));

                    double scaleFactor = GetScalingFactor();
                    log.Info(String.Format("Scale factor: {0}", scaleFactor));

                    this.Top = region.Y / scaleFactor;
                    this.Width = region.Width / scaleFactor;
                    this.Height = region.Height / scaleFactor;
                    log.Info(String.Format("Screen number {0}", System.Windows.Forms.Screen.AllScreens.Count()));
                    if (System.Windows.Forms.Screen.AllScreens.Count() > 1 && System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Location.X < 0)
                    {
                        log.Info("case 1");
                        this.Left = System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Location.X / scaleFactor;
                    }
                    else
                    {
                        log.Info("case 2");
                        this.Left = region.X / scaleFactor;
                    }
                    log.Info(String.Format("Move left: {0}", this.Left));
                }
            }
            catch (Exception ex)
            {
                log.Info("Previw Window_Loaded", ex);
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    this.preview.DataContext = null;
                    this.Close();
                    e.Handled = true;
                }
            }
            catch (Exception) { };
        }

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
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
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
