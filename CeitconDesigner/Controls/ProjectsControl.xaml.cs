using Ceitcon_Designer.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for ProjectsControl.xaml
    /// </summary>
    public partial class ProjectsControl : UserControl
    {
        public ProjectsControl()
        {
            InitializeComponent();
            tbName.Text = String.Empty;
            tbStart.SelectedValue = DateTime.Now.Date;
            tbEnd.SelectedValue = DateTime.Now.Date.AddDays(1);
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var project = (DataContext as MainViewModel).Project;
            if ((bool)e.NewValue)
            {
                (DataContext as MainViewModel).RefreshFilteredAppointments(tbName.Text, cbStart.IsChecked, tbStart.SelectedValue, cbEnd.IsChecked, tbEnd.SelectedValue);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).RefreshFilteredAppointments(tbName.Text, cbStart.IsChecked, tbStart.SelectedValue, cbEnd.IsChecked, tbEnd.SelectedValue);
        }
    }
}
