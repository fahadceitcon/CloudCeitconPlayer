using Ceitcon_Data.Model;
using Ceitcon_Designer.Utilities;
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
using System.Windows.Shapes;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for OpenProject.xaml
    /// </summary>
    public partial class OpenProject : Window
    {
        public OpenProject()
        {
            InitializeComponent();

            List<ProjectInfoModel> list = new List<ProjectInfoModel>();
            foreach (var item in SQLiteHelper.Instance.GetProjects())
            {
                if (File.Exists(item.Location))
                    list.Add(item);
            }
            PlaylistListBox.ItemsSource = list;
            
            
        }


        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = PlaylistListBox.SelectedItem as ProjectInfoModel;
                if (item == null)
                {
                    MessageBox.Show("Project is not selected. Please select project.", "Info", MessageBoxButton.OK);
                }
                else
                {
                    if ((DataContext as MainViewModel).Project == null)
                    {
                        (DataContext as MainViewModel).OpenProject(item.Location);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                        MessageBox.Show("Project is already open. Please close opened project.", "Info", MessageBoxButton.OK);
                }
            }
            catch (Exception) { }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = false;
            this.Close();
        }

    }
}
