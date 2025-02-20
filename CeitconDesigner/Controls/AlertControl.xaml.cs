using System;
using System.Collections.Generic;
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
    /// Interaction logic for AlertControl.xaml
    /// </summary>
    public partial class AlertControl : UserControl
    {
        public AlertControl()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register
            (
                 "Source",
                 typeof(String),
                 typeof(AlertControl),
                 new PropertyMetadata(String.Empty)
            );

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
        (
             "Text",
             typeof(string),
             typeof(AlertControl),
             new PropertyMetadata(String.Empty)
        );

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
            }
        }
    }
}