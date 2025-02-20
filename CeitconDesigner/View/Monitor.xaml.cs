using Ceitcon_Data.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for Monitor.xaml
    /// </summary>
    public partial class Monitor : Window
    {
        public Monitor()
        {
            InitializeComponent();
            this.KeyDown += HandleKeyPress;

            NewMonitor = new MonitorModel();
        }
        public static readonly DependencyProperty NewMonitorProperty = DependencyProperty.Register
        (
             "NewMonitor",
             typeof(MonitorModel),
             typeof(Monitor),
             new PropertyMetadata(null)
       );

        public MonitorModel NewMonitor
        {
            get { return (MonitorModel)GetValue(NewMonitorProperty); }
            set { SetValue(NewMonitorProperty, value); }
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    this.DialogResult = true;
                    this.Close();
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    this.DialogResult = false;
                    this.Close();
                    e.Handled = true;
                }
            }
            catch (Exception) { };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
