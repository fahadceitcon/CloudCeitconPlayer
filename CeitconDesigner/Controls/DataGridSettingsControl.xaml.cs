using Ceitcon_Data.Model.Data;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Designer.View;
using Ceitcon_Designer.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for DataGridSettingsControl.xaml
    /// </summary>
    public partial class DataGridSettingsControl : UserControl
    {
        public DataGridSettingsControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MainViewProperty = DependencyProperty.Register
        (
             "MainView",
             typeof(MainViewModel),
             typeof(DataGridSettingsControl),
             new PropertyMetadata(null)
        );

        public MainViewModel MainView
        {
            get { return (MainViewModel)GetValue(MainViewProperty); }
            set { SetValue(MainViewProperty, value); }
        }

        //private void AddLogoButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        SetContentModel c = ((e.OriginalSource as Button).DataContext as SetContentModel);

        //        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
        //        dlg.DefaultExt = ".jpg";
        //        dlg.Filter = "JPG Files (*.jpg)|*.jpg|All Files (*.*)|*.*";

        //        Nullable<bool> result = dlg.ShowDialog();
        //        if (result == true)
        //        {
        //            string filename = dlg.FileName;
        //            long length = new FileInfo(filename).Length;
        //            //((e.OriginalSource as Button).DataContext as SetContentModel).Content = filename;
        //            //((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = length;
        //            //(c.Parent as ControlModel).Source = filename;
        //            c.DataGrid.Logo = filename;
        //            c.DataGrid.LogoSize = length;
        //        }
        //    }
        //    catch (Exception) { }
        //}

        //private void ClearLogoButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        DataGridModel c = ((e.OriginalSource as Button).DataContext as SetContentModel).DataGrid;
        //        if (!String.IsNullOrEmpty(((e.OriginalSource as Button).DataContext as SetContentModel).DataGrid.Logo))
        //        {
        //            c.Logo = null;
        //            c.LogoSize = 0;
        //        }
        //    }
        //    catch (Exception) { }
        //}

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataSourceModel dsm = ((e.OriginalSource as Button).DataContext as SetContentModel).DataGrid.SelectedSource;
                if (dsm.Columns.Count > 1)
                {
                    var q = dsm.Columns.IndexOf(dsm.Columns.Where(_ => _.Id == dsm.SelectedColumn.Id).FirstOrDefault());
                    if (q > 0)
                    {
                        dsm.Columns.Move(q, q-1);
                        
                    }
                }
            }
            catch (Exception) { }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataSourceModel dsm = ((e.OriginalSource as Button).DataContext as SetContentModel).DataGrid.SelectedSource;
                if (dsm.Columns.Count > 1)
                {
                    var q = dsm.Columns.IndexOf(dsm.Columns.Where(_ => _.Id == dsm.SelectedColumn.Id).FirstOrDefault());
                    if (q > -1 && q < dsm.Columns.Count() - 1)
                    {
                        dsm.Columns.Move(q, q + 1);
                    }
                }
            }
            catch (Exception) { }
        }

        private void SpecialCellButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetContentModel sc = (e.OriginalSource as Button).DataContext as SetContentModel;
                //sc.DataGrid.SelectedSource.SelectedColumn = sc.DataGrid.SelectedSource.Columns.FirstOrDefault();
                var dialog = new SpecialCellWindow();
                dialog.DataContext = sc.DataGrid.SelectedSource.SelectedColumn;
                if (dialog.ShowDialog() == true)
                {

                }
            }
            catch (Exception) { }
        }

        private void TimeFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetContentModel sc = (e.OriginalSource as Button).DataContext as SetContentModel;
                //sc.DataGrid.SelectedSource.SelectedColumn = sc.DataGrid.SelectedSource.Columns.FirstOrDefault();
                var dialog = new TimeFilterWindow();
                dialog.DataContext = sc.DataGrid.SelectedSource.SelectedColumn;
                if (dialog.ShowDialog() == true)
                {

                }
            }
            catch (Exception) { }
        }

        private void CopyBrushClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ColorBox.ColorBox;
                if (cb.Brush == null)
                {
                    MainView.CopyBrush = null;
                }
                else
                {
                    MainView.CopyBrush = cb.Brush.Clone();
                }
            }
            catch (Exception) { }
        }

        private void PasteBrushClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ColorBox.ColorBox;
                if (MainView.CopyBrush == null)
                {
                    cb.Brush = null;
                }
                else
                {
                    cb.Brush = MainView.CopyBrush.Clone();
                }
            }
            catch (Exception) { }
        }
    }
}
