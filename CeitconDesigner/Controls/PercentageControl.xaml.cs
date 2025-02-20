using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for PercentageControl.xaml
    /// </summary>
    public partial class PercentageControl : UserControl
    {
        public PercentageControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
        (
            "Value",
            typeof(double),
            typeof(PercentageControl),
            new FrameworkPropertyMetadata(0D, new PropertyChangedCallback(Value_Changed))
        );

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        private static void Value_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = sender as PercentageControl;
                if (control != null)
                {
                    control.OnValueChanged();
                }
            }
            catch { }
        }

        protected virtual void OnValueChanged()
        {
            double f = Value;
            if (f > MaxValue)
                Value = MaxValue;
            else if (f < 0)
                Value = 0;
            Percentage = Value / MaxValue * 100;

            OnPropertyChanged("Value");
            OnPropertyChanged("Percentage");
        }

        public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register
        (
             "Percentage",
             typeof(double),
             typeof(PercentageControl),
             new FrameworkPropertyMetadata(0D, new PropertyChangedCallback(Percentage_Changed))
            //  new PropertyMetadata(Percentage_Changed)
        );

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set
            {
                SetValue(PercentageProperty, value);
            }
        }

        private static void Percentage_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = sender as PercentageControl;
                if (control != null)
                {
                    control.OnPercentageChanged();
                }
            }
            catch { }
        }

        protected virtual void OnPercentageChanged()
        {
            double f = Percentage;
            if (f > 100)
                Percentage = 100;
            else if (f < 0)
                Percentage = 0;
            else
                Value = Percentage / 100 * MaxValue;

            OnPropertyChanged("Percentage");
            OnPropertyChanged("Value");
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register
        (
             "MaxValue",
             typeof(double),
             typeof(PercentageControl),
             new FrameworkPropertyMetadata(0D, new PropertyChangedCallback(MaxValue_Changed))
        );

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        private static void MaxValue_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = sender as PercentageControl;
                if (control != null)
                {
                    control.OnMaxValueChanged();
                }
            }
            catch { }
        }

        protected virtual void OnMaxValueChanged()
        {
            OnPropertyChanged("MaxValue");
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
