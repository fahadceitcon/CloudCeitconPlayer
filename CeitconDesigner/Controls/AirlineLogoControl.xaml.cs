using Ceitcon_Data.Model.Data;
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
    /// Interaction logic for AirlineLogoControl.xaml
    /// </summary>
    public partial class AirlineLogoControl : UserControl
    {
        public AirlineLogoControl()
        {
            InitializeComponent();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataSourceModel selected = this.cbDataSource.SelectedItem as DataSourceModel;
                if (selected != null)
                {
                    (DataContext as MainViewModel).LoadLogos(selected);
                }
                //(DataContext as MainViewModel).Network = CeitconServerHelper.GetNetwork();
                //(DataContext as MainViewModel).GetPlayers((DataContext as MainViewModel).Network.FirstOrDefault(), tbName.Text, tbHost.Text, tbIP.Text, cbRegistred.IsChecked);
            }
            catch (Exception)
            {

            }
        }

    }
}
