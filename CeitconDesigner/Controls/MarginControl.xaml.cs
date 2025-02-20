using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for MarginControl.xaml
    /// </summary>
    public partial class MarginControl : UserControl
    {
        public MarginControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MarginXProperty = DependencyProperty.Register
        (
             "MarginX",
             typeof(double),
             typeof(MarginControl),
             new PropertyMetadata(0D)
        );

        public double MarginX
        {
            get { return (double)GetValue(MarginXProperty); }
            set
            {
                SetValue(MarginXProperty, value);
            }
        }

        public static readonly DependencyProperty MarginYProperty = DependencyProperty.Register
        (
             "MarginY",
             typeof(double),
             typeof(MarginControl),
             new PropertyMetadata(0D)
        );

        public double MarginY
        {
            get { return (double)GetValue(MarginYProperty); }
            set
            {
                SetValue(MarginYProperty, value);
            }
        }

        public static readonly DependencyProperty MarginWProperty = DependencyProperty.Register
        (
             "MarginW",
             typeof(double),
             typeof(MarginControl),
             new PropertyMetadata(0D)
        );

        public double MarginW
        {
            get { return (double)GetValue(MarginWProperty); }
            set
            {
                SetValue(MarginWProperty, value);
            }
        }

        public static readonly DependencyProperty MarginZProperty = DependencyProperty.Register
        (
             "MarginZ",
             typeof(double),
             typeof(MarginControl),
             new PropertyMetadata(0D)
        );

        public double MarginZ
        {
            get { return (double)GetValue(MarginZProperty); }
            set
            {
                SetValue(MarginZProperty, value);
            }
        }

        public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register
         (
              "MaxWidth",
              typeof(double),
              typeof(MarginControl),
              new FrameworkPropertyMetadata(1366D, new PropertyChangedCallback(MaxWidth_Changed))
         );

        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set
            {
                SetValue(MaxWidthProperty, value);
            }
        }

        private static void MaxWidth_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = sender as MarginControl;
                if (control != null)
                {
                    control.OnMaxWidthChanged();
                }
            }
            catch { }
        }

        protected virtual void OnMaxWidthChanged()
        {
            OnPropertyChanged("MaxWidth");
        }

        public static readonly DependencyProperty MaxHeightProperty = DependencyProperty.Register
        (
             "MaxHeight",
             typeof(double),
             typeof(MarginControl),
             new FrameworkPropertyMetadata(768D, new PropertyChangedCallback(MaxHeight_Changed))
        );

        public double MaxHeight
        {
            get { return (double)GetValue(MaxHeightProperty); }
            set
            {
                SetValue(MaxHeightProperty, value);
            }
        }

        private static void MaxHeight_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = sender as MarginControl;
                if (control != null)
                {
                    control.OnMaxHeightChanged();
                }
            }
            catch { }
        }

        protected virtual void OnMaxHeightChanged()
        {
            OnPropertyChanged("MaxHeight");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
