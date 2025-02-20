using Ceitcon_Data.Model.Data;
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
    /// Interaction logic for TimeFilterControl.xaml
    /// </summary>
    public partial class TimeFilterControl : UserControl
    {
        public TimeFilterControl()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataColumnModel dsm = ((e.OriginalSource as Button).DataContext as DataColumnModel);
                if (dsm != null)
                {
                    if (dsm.TimeFilters.Count == 0)
                    {
                        var item = new TimeFilterModel();
                        dsm.TimeFilters.Add(item);
                        dsm.SelectedTimeFilter = item;
                    }
                    else
                    {
                        MessageBox.Show("You can have only one fime filter", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception) { }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataColumnModel dsm = ((e.OriginalSource as Button).DataContext as DataColumnModel);
                if (dsm.SelectedTimeFilter != null)
                {
                    dsm.TimeFilters.Remove(dsm.SelectedTimeFilter);
                    dsm.SelectedTimeFilter = dsm.TimeFilters.FirstOrDefault();
                }
            }
            catch (Exception) { }
        }
    }
}
