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
    /// Interaction logic for PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl
    {
        public PlaylistControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty AdditionalContentProperty = DependencyProperty.Register
        (
           "AdditionalContent",
           typeof(object),
           typeof(PlaylistControl),
           new PropertyMetadata(null)
        );

        public object AdditionalContent
        {
            get { return (double)GetValue(AdditionalContentProperty); }
            set
            {
                SetValue(AdditionalContentProperty, value);
            }
        }
    }
}
