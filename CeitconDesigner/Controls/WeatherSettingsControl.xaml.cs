using Ceitcon_Designer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for WeatherSettingsControl.xaml
    /// </summary>
    public partial class WeatherSettingsControl : UserControl
    {
        public WeatherSettingsControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MainViewProperty = DependencyProperty.Register
        (
             "MainView",
             typeof(MainViewModel),
             typeof(WeatherSettingsControl),
             new PropertyMetadata(null)
        );

        public MainViewModel MainView
        {
            get { return (MainViewModel)GetValue(MainViewProperty); }
            set { SetValue(MainViewProperty, value); }
        }

        private void CopyBrushClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ColorBox.ColorBox;
                MainView.CopyBrush = cb.Brush.Clone();
            }
            catch (Exception) { }
        }

        private void PasteBrushClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ColorBox.ColorBox;
                cb.Brush = MainView.CopyBrush.Clone();
            }
            catch (Exception) { }
        }
    }
}