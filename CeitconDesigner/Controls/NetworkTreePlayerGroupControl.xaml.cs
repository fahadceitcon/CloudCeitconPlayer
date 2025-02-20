using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for NetworkTreePlayerGroupControl.xaml
    /// </summary>
    public partial class NetworkTreePlayerGroupControl : UserControl
    {
        public NetworkTreePlayerGroupControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register
        (
             "SelectedItem",
             typeof(object),
             typeof(NetworkTreePlayerGroupControl),
             new PropertyMetadata(null)
        );

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
                OnPropertyChanged("SelectedItem");
            }
        }

        private void dmtv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
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
