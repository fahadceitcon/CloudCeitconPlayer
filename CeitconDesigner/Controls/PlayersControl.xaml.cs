using Ceitcon_Designer.ViewModel;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for PlayersControl.xaml
    /// </summary>
    public partial class PlayersControl : UserControl
    {
        public PlayersControl()
        {
            InitializeComponent();

            //Task.Run(() => ServerSync(new TimeSpan(0, 1, 0))); -- this is Refresh
        }

        async void ServerSync(TimeSpan refreshTime)
        {
            try
            {
                while (true)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        (DataContext as MainViewModel).RefreshNetwork(tbName.Text, tbHost.Text, tbIP.Text, cbRegistred.IsChecked);
                    });
                    await Task.Delay(refreshTime);
                }
            }
            catch (Exception) { }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var project = (DataContext as MainViewModel).Project;
            if ((bool)e.NewValue)
            {
                (DataContext as MainViewModel).RefreshNetwork(tbName.Text, tbHost.Text, tbIP.Text, cbRegistred.IsChecked);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).RefreshNetwork(tbName.Text, tbHost.Text, tbIP.Text, cbRegistred.IsChecked);
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
