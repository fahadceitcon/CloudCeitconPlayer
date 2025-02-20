using Ceitcon_Data.Model;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.View;
using Ceitcon_Designer.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for RTServerControl.xaml
    /// </summary>
    public partial class RTServerControl : UserControl
    {
        //private string serverIP = String.Empty;
        public RTServerControl()
        {
            InitializeComponent();
            //serverIP = SQLiteHelper.Instance.GetApplication("ServerUrl");
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as MainViewModel).SelectedRTMessage = new RTMessageModel() { Name = "", Topic = "", Message = "" };
            //(DataContext as MainViewModel).RTMessages.Add(new RTMessageModel() { MediaType = RTMessageType.Facebook, Message = Guid.NewGuid().ToString(), ApprovedBy = Guid.NewGuid().ToString().Substring(0, 6) });
            //(DataContext as MainViewModel).RTMessages.Add(new RTMessageModel() { MediaType = RTMessageType.Instagram, Message = Guid.NewGuid().ToString(), ApprovedBy = Guid.NewGuid().ToString().Substring(0, 6) });
            //(DataContext as MainViewModel).RTMessages.Add(new RTMessageModel() { MediaType = RTMessageType.Twitter, Message = Guid.NewGuid().ToString(), ApprovedBy = Guid.NewGuid().ToString().Substring(0, 6) });
            // (DataContext as MainViewModel).RTMessages.Add(new RTMessageModel() { Name = "Name", Message = Guid.NewGuid().ToString(), MediaType = "Facebook", ApprovedBy = Guid.NewGuid().ToString().Substring(0, 6) });
        }

        private void SearchNetwork_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NetworkTreeWindow();
            dialog.ShowPlayer = true;
            dialog.DataContext = this.DataContext;
            if (dialog.ShowDialog() == true)
            {
                (DataContext as MainViewModel).SelectedRTMessage.Name = dialog.SelectedText;
                cbToAll.IsChecked = false;
            }
        }

        private void RefreshListButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if ((DataContext as MainViewModel).RTMessages != null)
            //    (DataContext as MainViewModel).RTMessages.Add(new RTMessageModel() { Name = "Name", Message = Guid.NewGuid().ToString(), MediaType = "Facebook", ApprovedBy = Guid.NewGuid().ToString().Substring(0, 6) });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if((sender as CheckBox).IsChecked == true)
                (DataContext as MainViewModel).SelectedRTMessage.Name = "Network";
        }



        //private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        //{
        //    string authHdr = "Authorization: Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("admin" + ":" + "public")) + "\r\n";

        //    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri, authHdr));
        //    e.Handled = true;
        //}

        //public String ServerUrl
        //{
        //    get { return String.Format(@"http://{0}:18083/", serverIP); }
        //}
    }
}
