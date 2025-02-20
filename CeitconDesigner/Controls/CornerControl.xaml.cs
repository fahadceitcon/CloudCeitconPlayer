using System;
using System.Windows;
using System.Windows.Controls;


namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for CornerControl.xaml
    /// </summary>
    public partial class CornerControl : UserControl
    {
        public CornerControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty AProperty = DependencyProperty.Register
        (
             "A",
             typeof(double),
             typeof(CornerControl),
             new PropertyMetadata(0D)
        );

        public double A
        {
            get { return (double)GetValue(AProperty); }
            set
            {
                SetValue(AProperty, value);
            }
        }

        public static readonly DependencyProperty BProperty = DependencyProperty.Register
        (
             "B",
             typeof(double),
             typeof(CornerControl),
             new PropertyMetadata(0D)
        );

        public double B
        {
            get { return (double)GetValue(BProperty); }
            set
            {
                SetValue(BProperty, value);
            }
        }

        public static readonly DependencyProperty CProperty = DependencyProperty.Register
        (
             "C",
             typeof(double),
             typeof(CornerControl),
             new PropertyMetadata(0D)
        );

        public double C
        {
            get { return (double)GetValue(CProperty); }
            set
            {
                SetValue(CProperty, value);
            }
        }

        public static readonly DependencyProperty DProperty = DependencyProperty.Register
        (
             "D",
             typeof(double),
             typeof(CornerControl),
             new PropertyMetadata(0D)
        );

        public double D
        {
            get { return (double)GetValue(DProperty); }
            set
            {
                SetValue(DProperty, value);
            }
        }
    }
}
