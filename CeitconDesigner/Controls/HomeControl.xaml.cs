using Ceitcon_Designer.View;
using Ceitcon_Designer.ViewModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for HomeControl.xaml
    /// </summary>
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public partial class HomeControl : UserControl
    {
        public HomeControl()
        {
            InitializeComponent();
        }

        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project == null)
            {
                (DataContext as MainViewModel).CreateProject();
            }
            else
                MessageBox.Show("Project is already open. Please close opened project.", "Info");
        }

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project == null)
            {
                var dialog = new OpenProject();
                dialog.DataContext = DataContext;
                dialog.ShowDialog();
            }
            else
                MessageBox.Show("Project is already open. Please close opened project.", "Info");
        }
    }
}
