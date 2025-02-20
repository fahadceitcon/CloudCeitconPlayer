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
    /// Interaction logic for SpecialCellControl.xaml
    /// </summary>
    public partial class SpecialCellControl : UserControl
    {
        public SpecialCellControl()
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
                    var item = new SpecialCellModel();
                    dsm.SpecialCells.Add(item);
                    dsm.SelectedSpecialCell = item;
                }
            }
            catch (Exception) { }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataColumnModel dsm = ((e.OriginalSource as Button).DataContext as DataColumnModel);
                if (dsm.SelectedSpecialCell != null)
                {
                    dsm.SpecialCells.Remove(dsm.SelectedSpecialCell);
                    dsm.SelectedSpecialCell = dsm.SpecialCells.FirstOrDefault();
                }
            }
            catch (Exception) { }
        }
    }
}
