using Ceitcon_Data.Model.Network;
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
using System.Windows.Shapes;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for NetworkTreeWindow.xaml
    /// </summary>
    public partial class NetworkTreeWindow : Window
    {
        public NetworkTreeWindow()
        {
            InitializeComponent();
            this.KeyDown += HandleKeyPress;
        }

        public static readonly DependencyProperty ShowPlayerProperty = DependencyProperty.Register
        (
             "ShowPlayer",
             typeof(bool),
             typeof(NetworkTreeWindow),
             new PropertyMetadata(false)
        );

        public bool ShowPlayer
        {
            get { return (bool)GetValue(ShowPlayerProperty); }
            set { SetValue(ShowPlayerProperty, value); }
        }

        public object SelectedItem
        {
            get {
                if(ShowPlayer)
                    return networkTreePlayer.SelectedItem;
                else
                    return networkTreePlayerGroup.SelectedItem;
            }
        }

        public string SelectedText
        {
            get {
                object item;
                //object item = networkTree.SelectedItem;
                if (ShowPlayer)
                    item  = networkTreePlayer.SelectedItem;
                else
                    item = networkTreePlayerGroup.SelectedItem;

                if (item is NetworkModel)
                {
                    return (item as NetworkModel).Identification;
                }
                else if (item is DomainModel)
                {
                    return (item as DomainModel).Identification;
                }
                else if (item is CountryModel)
                {
                    return (item as CountryModel).Identification;
                }
                else if (item is NetworkRegionModel)
                {
                    return (item as NetworkRegionModel).Identification;
                }
                else if (item is LocationGroupModel)
                {
                    return (item as LocationGroupModel).Identification;
                }
                else if (item is LocationModel)
                {
                    return (item as LocationModel).Identification;
                }
                else if (item is FloorModel)
                {
                    return (item as FloorModel).Identification;
                }
                else if (item is ZoneModel)
                {
                    return (item as ZoneModel).Identification;
                }
                else if (item is PlayerGroupModel)
                {
                    return (item as PlayerGroupModel).Identification;
                }
                else if (item is PlayerModel)
                {
                    return (item as PlayerModel).Identification;
                }
                else if (item is FaceGroupModel)
                {
                    return (item as FaceGroupModel).Identification;
                }
                else if (item is FaceModel)
                {
                    return (item as FaceModel).Identification;
                }
                return String.Empty;
            }
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

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}