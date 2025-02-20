using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Net.Http;
using System.Windows.Media;
using Ceitcon_Data.Model.Data;

namespace Ceitcon_Designer.Controls
{

   public  class WeatherDetails
    {
        public string Weather { get; set; }
        public string WeatherIcon { get; set; }
        public string WeatherDay { get; set; }
        public string Temperature { get; set; }
        public string MaxTemperature { get; set; }
        public string MinTemperature { get; set; }
        public string WindDirection { get; set; }
        public string WindSpeed { get; set; }
        public string Humidity { get; set; }
    }


    /// <summary>
    /// Interaction logic for ucWeather.xaml
    /// </summary>
    public partial class WeatherControl : UserControl
    {
        public decimal dsLongitude { get; set; }
        public decimal dsLatitude { get; set; }

        // Local Variables
        DispatcherTimer timer = new DispatcherTimer();

        #region Properties
        public WeatherControl()
        {
            try
            {
                InitializeComponent();
            }
            catch { }
        }

        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register
        (
            "Location",
            typeof(string),
            typeof(WeatherControl),
            new PropertyMetadata(String.Empty)
        );

        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set
            {
                SetValue(LocationProperty, value);
            }
        }

        public static readonly DependencyProperty ItemCountProperty = DependencyProperty.Register
        (
            "ItemCount",
            typeof(int),
            typeof(WeatherControl),
            new PropertyMetadata(1)
        );

        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set
            {
                SetValue(ItemCountProperty, value);
            }
        }

        public bool ShowFirstDay
        {
            get { return ItemCount > 0; }
        }

        public bool ShowSecondDay
        {
            get { return ItemCount > 1; }
        }

        public bool ShowThirdDay
        {
            get { return ItemCount > 2; }
        }

        public static readonly DependencyProperty WeatherSettingsProperty = DependencyProperty.Register
         (
             "WeatherSettings",
             typeof(WeatherModel),
             typeof(WeatherControl),
             new PropertyMetadata(new WeatherModel())
         );

        public string TitleText
        {
            get {
                return (WeatherSettings == null || String.IsNullOrEmpty(WeatherSettings.TitleText)) ? Location : WeatherSettings.TitleText;
            }
        }

        public WeatherModel WeatherSettings
        {
            get { return (WeatherModel)GetValue(WeatherSettingsProperty); }
            set
            {
                SetValue(WeatherSettingsProperty, value);
            }
        }
        #endregion

        #region Methods
        //public void ResetControl()
        //{
        //    try
        //    {
        //        GetWeather(Location);
        //        timer.Interval = TimeSpan.FromMinutes(Interval);
        //        timer.Tick += new EventHandler(timer_Tick);
        //        timer.Start();
        //    }
        //    catch { }
        //}

        //void timer_Tick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        GetWeather(Location);
        //    }
        //    catch { }
        //}

        //void ucWeather_Loaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //      //  GetWeather(Location);
        //    }
        //    catch { }
        //}

        //private async Task StopSourceAsync(string location)
        //{
        //    await Dispatcher.BeginInvoke(new Action(async () =>
        //    {
        //        try
        //        {
        //            await Task.Delay(0);
        //        }
        //        catch (Exception e)
        //        {
        //            //log.Error(e);
        //        }
        //    }
        //    ));
        //}

        //public void GetWeather()
        //{
        //    GetWeather2();


        //}

        private string GetDay(int dayOfWeek)
        {
            if (dayOfWeek == 0)
                return "Sunday";
            else if (dayOfWeek == 1)
                return "Monday";
            else if (dayOfWeek == 2)
                return "Tuesday";
            else if (dayOfWeek == 3)
                return "Wednesday";
            else if (dayOfWeek == 4)
                return "Thursday";
            else if (dayOfWeek == 5)
                return "Friday";
            else
                return "Saturday";
        }

        private string GetImagePath(string weatherDescription)
        {
            try
            {
                weatherDescription = weatherDescription.ToLower();

                string imagePath = String.Empty;

                if (weatherDescription == "sunny")
                    imagePath = "../Images/Weather/sunny.png";
                else if (weatherDescription == "mostly sunny")
                    imagePath = "../Images/Weather/sunny.png";
                else if (weatherDescription == "partly sunny")
                    imagePath = "../Images/Weather/partlycloudy.png";
                else if (weatherDescription == "mostly cloudy")
                    imagePath = "../Images/Weather/partlycloudy.png";
                else if (weatherDescription == "cloudy")
                    imagePath = "../Images/Weather/cloudy.png";
                else if (weatherDescription.Contains("clear"))
                    imagePath = "../Images/Weather/sunny.png";
                else if (weatherDescription == "partly cloudy")
                    imagePath = "../Images/Weather/partlycloudy.png";
                else if (weatherDescription.Contains("clouds"))
                    imagePath = "../Images/Weather/partlycloudy.png";
                else if (weatherDescription.Contains("cloudy"))
                    imagePath = "../Images/Weather/partlycloudy.png";
                else if (weatherDescription.Contains("clearing"))
                    imagePath = "../Images/Weather/sunny.png";
                else if (weatherDescription.Contains("sunny"))
                    imagePath = "../Images/Weather/sunny.png";
                else if (weatherDescription.Contains("fog"))
                    imagePath = "../Images/Weather/partlycloudy.png";
                else if (weatherDescription.Contains("snow"))
                    imagePath = "../Images/Weather/snow.png";
                else if (weatherDescription.Contains("flurries"))
                    imagePath = "../Images/Weather/snow.png";
                else if (weatherDescription.Contains("blizzard"))
                    imagePath = "../Images/Weather/snow.png";
                else if (weatherDescription.Contains("ice"))
                    imagePath = "../Images/Weather/wintermix.png";
                else if (weatherDescription.Contains("freezing"))
                    imagePath = "../Images/Weather/wintermix.png";
                else if (weatherDescription.Contains("sleet"))
                    imagePath = "../Images/Weather/wintermix.png";
                else if (weatherDescription.Contains("chance rain showers"))
                    imagePath = "../Images/Weather/rainsun.png";
                else if (weatherDescription.Contains("chance rain"))
                    imagePath = "../Images/Weather/rainsun.png";
                else if (weatherDescription.Contains("rain showers"))
                    imagePath = "../Images/Weather/rain.png";
                else if (weatherDescription.Contains("rain"))
                    imagePath = "../Images/Weather/rain.png";
                else if (weatherDescription.Contains("drizzle"))
                    imagePath = "../Images/Weather/rainsun.png";
                else if (weatherDescription.Contains("rain/snow"))
                    imagePath = "../Images/Weather/wintermix.png";
                else if (weatherDescription.Contains("freezing rain"))
                    imagePath = "../Images/Weather/wintermix.png";
                else if (weatherDescription.Contains("wintry mix"))
                    imagePath = "../Images/Weather/wintermix.png";
                else if (weatherDescription.Contains("thunderstorms"))
                    imagePath = "../Images/Weather/thunderstorm.png";
                else if (weatherDescription.Contains("tstms"))
                    imagePath = "../Images/Weather/thunderstorm.png";
                else
                    imagePath = "../Images/Weather/partlycloudy.png";

                return imagePath;
            }
            catch { return "../Images/Weather/partlycloudy.png"; }
        }

        //private static string AppID = "82ec1cf38c79dd4e914c6fda8a43c3eb";

        //private void GetWeather(string location)
        //{
        //    List<WeatherDetails> list = new List<WeatherDetails>();
        //    string url = string.Format(@"http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&type=accurate&mode=xml&units=metric&cnt=3&appid={1}", location, AppID);
        //    using (WebDownload webclient = new WebDownload())
        //    {
        //        try
        //        {
        //            webclient.Timeout = 2000;
        //            string response = webclient.DownloadString(new Uri(url));

        //            //string response = webclient.DownloadString(url);
        //            if (!(response.Contains("message") && response.Contains("cod")))
        //            {
        //                XElement xEl = XElement.Load(new System.IO.StringReader(response));
        //                list = GetWeatherInfo(xEl);
        //            }
        //            else
        //            {
        //                return;
        //                //list = null;
        //            }
        //            int i = 1;
        //            foreach (WeatherDetails wd in list)
        //            {
        //                if (i == 1)
        //                {
        //                    imgWeatherDay1.Source = new BitmapImage(new Uri(wd.WeatherIcon, UriKind.Relative));
        //                    txtDayOfWeek1.Text = GetDay(Convert.ToInt32(DateTime.Today.DayOfWeek));
        //                    txtWeather1.Text = wd.Weather;
        //                    txtHigh1.Text = wd.MaxTemperature;
        //                    txtLow1.Text = wd.MinTemperature;
        //                    i++;
        //                }
        //                else if (i == 2)
        //                {
        //                    imgWeatherDay2.Source = new BitmapImage(new Uri(wd.WeatherIcon, UriKind.Relative));
        //                    txtDayOfWeek2.Text = GetDay(Convert.ToInt32(DateTime.Today.AddDays(1).DayOfWeek));
        //                    txtWeather2.Text = wd.Weather;
        //                    txtHigh2.Text = wd.MaxTemperature;
        //                    txtLow2.Text = wd.MinTemperature;
        //                    i++;
        //                }
        //                else if (i == 3)
        //                {
        //                    imgWeatherDay3.Source = new BitmapImage(new Uri(wd.WeatherIcon, UriKind.Relative));
        //                    txtDayOfWeek3.Text = GetDay(Convert.ToInt32(DateTime.Today.AddDays(2).DayOfWeek));
        //                    txtWeather3.Text = wd.Weather;
        //                    txtHigh3.Text = wd.MaxTemperature;
        //                    txtLow3.Text = wd.MinTemperature;
        //                }
        //            }
        //        }
        //        catch (HttpRequestException)
        //        {
        //            //list = null;
        //        }
        //        catch (Exception e)
        //        {
        //            //list = null;
        //        }
        //    }
        //}

        //private void GetWeatherFromServer(string location)
        //{
        //    string url = string.Format(@"http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&type=accurate&mode=xml&units=metric&cnt=3&appid={1}", location, AppID);
        //    WebClient webclient = new WebClient();
        //    webclient.DownloadStringCompleted += Webclient_DownloadStringCompleted;
        //    webclient.DownloadStringAsync(new Uri(url));
        //}

        //public Webclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{

        //}
        public void GetWeather(string text)//Webclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                List<WeatherDetails> list = new List<WeatherDetails>();
                if (!(text.Contains("message") && text.Contains("cod")))
                {
                    XElement xEl = XElement.Load(new System.IO.StringReader(text));
                    list = GetWeatherInfo(xEl);
                }
                else
                {
                    return;
                }
                int i = 1;
                foreach (WeatherDetails wd in list)
                {
                    if (i == 1)
                    {
                        imgWeatherDay1.Source = new BitmapImage(new Uri(wd.WeatherIcon, UriKind.Relative));
                        txtDayOfWeek1.Text = GetDay(Convert.ToInt32(DateTime.Today.DayOfWeek));
                        txtWeather1.Text = wd.Weather;
                        txtHigh1.Text = wd.MaxTemperature;
                        txtLow1.Text = wd.MinTemperature;
                        i++;
                    }
                    else if (i == 2)
                    {
                        imgWeatherDay2.Source = new BitmapImage(new Uri(wd.WeatherIcon, UriKind.Relative));
                        txtDayOfWeek2.Text = GetDay(Convert.ToInt32(DateTime.Today.AddDays(1).DayOfWeek));
                        txtWeather2.Text = wd.Weather;
                        txtHigh2.Text = wd.MaxTemperature;
                        txtLow2.Text = wd.MinTemperature;
                        i++;
                    }
                    else if (i == 3)
                    {
                        imgWeatherDay3.Source = new BitmapImage(new Uri(wd.WeatherIcon, UriKind.Relative));
                        txtDayOfWeek3.Text = GetDay(Convert.ToInt32(DateTime.Today.AddDays(2).DayOfWeek));
                        txtWeather3.Text = wd.Weather;
                        txtHigh3.Text = wd.MaxTemperature;
                        txtLow3.Text = wd.MinTemperature;
                    }
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (Exception)
            {
            }
        }

        private static List<WeatherDetails> GetWeatherInfo(XElement xEl)
        {
            IEnumerable<WeatherDetails> w = xEl.Descendants("time").Select((el) =>
                new WeatherDetails
                {
                    Humidity = el.Element("humidity").Attribute("value").Value + "%",
                    MaxTemperature = el.Element("temperature").Attribute("max").Value + "°",
                    MinTemperature = el.Element("temperature").Attribute("min").Value + "°",
                    Temperature = el.Element("temperature").Attribute("day").Value + "°",
                    Weather = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(el.Element("symbol").Attribute("name").Value),
                    WeatherDay = DayOfTheWeek(el),
                    WeatherIcon = WeatherIconPath(el),
                    WindDirection = el.Element("windDirection").Attribute("name").Value,
                    WindSpeed = el.Element("windSpeed").Attribute("mps").Value + "mps"
                });

            return w.ToList();
        }

        private static string DayOfTheWeek(XElement el)
        {
            DayOfWeek dW = Convert.ToDateTime(el.Attribute("day").Value).DayOfWeek;
            return dW.ToString();
        }

        private static string WeatherIconPath(XElement el)
        {
            string symbolVar = el.Element("symbol").Attribute("var").Value;
            string symbolNumber = el.Element("symbol").Attribute("number").Value;
            string dayOrNight = symbolVar.ElementAt(2).ToString(); // d or n
            return String.Format("../Images/Weather/{0}{1}.png", symbolNumber, dayOrNight);
        }
        #endregion
    }
}
