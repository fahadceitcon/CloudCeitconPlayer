using System;
using System.Windows;
using System.Windows.Controls;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for Duration.xaml
    /// </summary>
    public partial class DurationControl : UserControl
    {
        public DurationControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register
        (
             "Duration",
             typeof(TimeSpan),
             typeof(DurationControl),
             new PropertyMetadata(new TimeSpan(0))
        );

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set
            {
                SetValue(DurationProperty, value);
            }
        }

        public static readonly DependencyProperty ForeverProperty = DependencyProperty.Register
        (
             "Forever",
             typeof(bool),
             typeof(DurationControl),
             new PropertyMetadata(false)
        );

        public bool Forever
        {
            get { return (bool)GetValue(ForeverProperty); }
            set
            {
                SetValue(ForeverProperty, value);
            }
        }

        private void hUpButton_Click(object sender, RoutedEventArgs e)
        {
            Duration = Duration.Add(new TimeSpan(1, 0, 0));
        }

        private void mUpButton_Click(object sender, RoutedEventArgs e)
        {
            Duration = Duration.Add(new TimeSpan(0, 1, 0));
        }

        private void sUpButton_Click(object sender, RoutedEventArgs e)
        {
            Duration = Duration.Add(new TimeSpan(0, 0, 1));
        }

        private void hDwButton_Click(object sender, RoutedEventArgs e)
        {
            if(Duration.TotalHours >= 1)
                Duration = Duration.Add(new TimeSpan(-1, 0, 0));
        }

        private void mDwButton_Click(object sender, RoutedEventArgs e)
        {
            if (Duration.TotalMinutes >= 1)
                Duration = Duration.Add(new TimeSpan(0, -1, 0));
        }

        private void sDwButton_Click(object sender, RoutedEventArgs e)
        {
            if (Duration.TotalSeconds >= 1)
                Duration = Duration.Add(new TimeSpan(0, 0, -1));
        }
    }
}
