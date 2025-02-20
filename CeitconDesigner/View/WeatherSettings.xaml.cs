using Ceitcon_Designer.ViewModel;
using System.Windows;


namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for WeatherSettings.xaml
    /// </summary>
    public partial class WeatherSettings : Window
    {
        public WeatherSettings()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MainViewProperty = DependencyProperty.Register
        (
             "MainView",
             typeof(MainViewModel),
             typeof(WeatherSettings),
             new PropertyMetadata(null)
       );

        public MainViewModel MainView
        {
            get { return (MainViewModel)GetValue(MainViewProperty); }
            set { SetValue(MainViewProperty, value); }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
