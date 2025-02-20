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
using System.Windows.Shapes;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for ProofOfPlaySettingsWindow.xaml
    /// </summary>
    public partial class ProofOfPlaySettingsWindow : Window
    {
        private readonly int totalScreens;
        public ProofOfPlaySettingsWindow(int totalScreen)
        {
            InitializeComponent();
            this.totalScreens = totalScreen;
            Screens = 1;
        }

        public static readonly DependencyProperty ScreensProperty = DependencyProperty.Register
        (
             "Screens",
             typeof(int),
             typeof(ProofOfPlaySettingsWindow),
             new PropertyMetadata(1)
        );

        public int Screens
        {
            get { return (int)GetValue(ScreensProperty); }
            set { SetValue(ScreensProperty, value); }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if(Screens < 1  || Screens > totalScreens)
            {
                MessageBox.Show($"Please insert valid number between 1 and {totalScreens}", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
