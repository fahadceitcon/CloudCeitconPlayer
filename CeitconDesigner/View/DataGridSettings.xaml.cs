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
using System.Windows.Shapes;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for DataGridSettings.xaml
    /// </summary>
    public partial class DataGridSettings : Window
    {
        public DataGridSettings()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MainViewProperty = DependencyProperty.Register
        (
             "MainView",
             typeof(MainViewModel),
             typeof(DataGridSettings),
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
