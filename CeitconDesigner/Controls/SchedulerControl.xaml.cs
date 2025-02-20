using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Network;
using Ceitcon_Data.Utilities;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.View;
using Ceitcon_Designer.ViewModel;
using log4net;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for SchedulerControl.xaml
    /// </summary>
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public partial class SchedulerControl : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ProgressWindow progressWindow;
        private const string textNew = "NewSchedular";
        private const string textCancel = "CancelSchedular";

        public SchedulerControl()
        {
            InitializeComponent();
            progressWindow = new ProgressWindow();
        }

        #region Events
        private void xRadScheduleView_AppointmentCreated(object sender, AppointmentCreatedEventArgs e)
        {

        }

        private IAppointment oldApoitment;
        private void xRadScheduleView_AppointmentEditing(object sender, AppointmentEditingEventArgs e)
        {
            Appointment item = e.Appointment as Appointment;
            oldApoitment = item.Copy();
            Slot s = (sender as RadScheduleView).HighlightedSlots.FirstOrDefault();
            if (s != null && SameItemsCount(item.Subject, s.Start, s.End, item.UniqueId) > 0 /* && (item.End - item.Start == s.End - s.Start)*/)
            {
                e.Cancel = true;
                MessageBox.Show("Selected player already have project for selected interval.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void xRadScheduleView_AppointmentEdited(object sender, AppointmentEditedEventArgs e)
        {
            var a = ((sender as RadScheduleView).SchedulerDialogHostFactory as ScheduleViewDialogHostFactory);
            Appointment item = e.Appointment as Appointment;
            if (String.IsNullOrEmpty(item.Subject))
            {
                item.Subject = (oldApoitment as Appointment).Subject;
                item.Body = (oldApoitment as Appointment).Body;
                MessageBox.Show("Player can't be empty string.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            if (String.IsNullOrEmpty(item.Body))
            {
                item.Subject = (oldApoitment as Appointment).Subject;
                item.Body = (oldApoitment as Appointment).Body;
                MessageBox.Show("Project can't be empty string.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            //Create New
            if (item.Url == textNew)
            {
                item.Url = "1";
                return;
            }

            if (SameItemsCount(item.Subject, item.Start, item.End, item.UniqueId) > 0) //1 becouse item is already setted
            {
                item = oldApoitment as Appointment;
                MessageBox.Show("Selected player already have project for selected interval.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            //Edit existing
            int version = 0;
            if (Int32.TryParse(item.Url, out version))
            {
                item.Url = (++version).ToString();
                try
                {
                    string errorResponse = String.Empty;
                    progressWindow.SafeShow("Uploading file in progress...");
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    bool success = CeitconServerHelper.UploadScheduler(item, out errorResponse, progressWindow);
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                    progressWindow.SafeHide();
                    if (success)
                    {
                        //(DataContext as MainViewModel).Appointments.Add(item);
                        log.Info("Edit Scheduler");
                    }
                    else
                    {
                        MessageBox.Show("Schedular is not updated", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        log.Error("Edit Scheduler");
                    }
                }
                catch (Exception ex)
                {
                    progressWindow.SafeHide();
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                    log.Info("xRadScheduleView_AppointmentEdited", ex);
                }

                return;
            }
            item = oldApoitment as Appointment;
        }

        private void xRadScheduleView_AppointmentDeleting(object sender, AppointmentDeletingEventArgs e)
        {
            Appointment item = e.Appointment as Appointment;
            if (String.IsNullOrEmpty(item.UniqueId))
                return;

            bool response = CeitconServerHelper.DeleteScheduler(item);
            if(!response)
            {
                MessageBox.Show("Appointment can't be deleted from server.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                e.Cancel = true;
            }
        }

        private void RadScheduleView_ShowDialog(object sender, ShowDialogEventArgs e)
        {
            //Disable create appoitment on double click
            if (e.DialogViewModel is AppointmentDialogViewModel && (e.DialogViewModel as AppointmentDialogViewModel).ViewMode == AppointmentViewMode.Add)
                e.Cancel = true;
        }

        private void SearchNetwork_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NetworkTreeWindow();
            dialog.DataContext = this.DataContext;
            if (dialog.ShowDialog() == true)
            {
                ((sender as Button).DataContext as Telerik.Windows.Controls.AppointmentDialogViewModel).Occurrence.Appointment.Subject = dialog.SelectedText;
                (((sender as Button).Parent as Grid).Children[1] as TextBox).Text = dialog.SelectedText;
                (((sender as Button).Parent as Grid).Children[1] as TextBox).ToolTip = dialog.SelectedText;
            }
        }

        private void OLD_AddSchedulerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Disable ar-SA date time culture
                CultureInfo _cinfor = System.Threading.Thread.CurrentThread.CurrentCulture;
                if (_cinfor.Name.ToLower() == "ar-sa")
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                //Get time interval
                Slot selected = xRadScheduleView.SelectedSlot;
                DateTime start =  DateTime.Now;//.Date;
                DateTime end = DateTime.Now.AddDays(1);//.Date.AddDays(1);

                if ((DataContext as MainViewModel)?.Project == null)
                {
                    MessageBox.Show("You must have opened project. Please create new or open existing project before adding.", "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    return;
                }

                //Select Network
                string selectedNetwork = String.Empty;
                var dialog = new NetworkTreeWindow();
                dialog.DataContext = this.DataContext;
                if (dialog.ShowDialog() == true)
                {
                    selectedNetwork = dialog.SelectedText;
                }
                else
                {
                    return;
                }
                //MessageBox.Show(start.ToString() + "\r\n" + end.ToString());

                if (SameItemsCount(selectedNetwork, start, end, String.Empty) > 0)
                {
                    MessageBox.Show("Selected group already have project for selected interval.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                    return;
                }

                (DataContext as MainViewModel).SaveProject(false);

                var item = new Appointment() { Subject = selectedNetwork, Body = (DataContext as MainViewModel).Project.Information.ProjectName, Location = (DataContext as MainViewModel).Project.Id, Start = start, End = end, Url = String.Empty };

               
                item.Url = textNew;
                RadScheduleViewCommands.EditAppointment.Execute(item, xRadScheduleView);
                if (item.Url == textNew || item.Url == textCancel) //Brake action
                    return;

                //string errorResponse = String.Empty;
                //progressWindow.SafeShow("Uploading file in progress...");
                //System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                //bool success = CeitconServerHelper.UploadScheduler(item, out errorResponse, progressWindow);
                //System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                //progressWindow.SafeHide();

                if (UploadProject(item))
                {
                    (DataContext as MainViewModel).Appointments.Add(item);
                    log.Info("Added project to Scheduler");
                }
                else
                {
                    MessageBox.Show("Project is not added", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    log.Error("Added project to Scheduler");
                }
            }
            catch (Exception ex)
            {
                progressWindow.SafeHide();
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                log.Info("AddSchedulerButton_Click", ex);
            }
        }
        private void AddSchedulerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Disable ar-SA date time culture
                CultureInfo _cinfor = System.Threading.Thread.CurrentThread.CurrentCulture;
                if (_cinfor.Name.ToLower() == "ar-sa")
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                //Get time interval
                Slot selected = xRadScheduleView.SelectedSlot;
                var start = DateTime.Now;
                var end = DateTime.Now.AddDays(1).AddMinutes(-1);

                if ((DataContext as MainViewModel)?.Project == null)
                {
                    MessageBox.Show("You must have opened project. Please create new or open existing project before adding.", "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    return;
                }

                //Select Network
                string selectedNetwork = String.Empty;
                var dialog = new NetworkTreeWindow();
                dialog.DataContext = this.DataContext;
                if (dialog.ShowDialog() == true)
                {
                    selectedNetwork = dialog.SelectedText;
                }
                else
                {
                    return;
                }
                //MessageBox.Show(start.ToString() + "\r\n" + end.ToString());

                if (SameItemsCount(selectedNetwork, start, end, String.Empty) > 0)
                {
                    MessageBox.Show("Selected group already have project for selected interval.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                    return;
                }

                (DataContext as MainViewModel).SaveProject(false);

                var item = new Appointment() { Subject = selectedNetwork, Body = (DataContext as MainViewModel).Project.Information.ProjectName, Location = (DataContext as MainViewModel).Project.Id, Start = start, End = end, Url = String.Empty };


                item.Url = textNew;
                RadScheduleViewCommands.EditAppointment.Execute(item, xRadScheduleView);
                if (item.Start >= item.End)
                {
                    MessageBox.Show("The end time must be greater than start time.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                    return;
                }

                if (item.Url == textNew || item.Url == textCancel)
                    return;

                if (UploadProject(item))
                {
                    (DataContext as MainViewModel).Appointments.Add(item);
                    log.Info("Added project to Scheduler");
                }
                else
                {
                    MessageBox.Show("Project is not added", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    log.Error("Added project to Scheduler");
                }
            }
            catch (Exception ex)
            {
                progressWindow.SafeHide();
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                log.Info("AddSchedulerButton_Click", ex);
            }
        }
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainViewModel = DataContext as MainViewModel;
                if (mainViewModel?.Project == null)
                {
                    MessageBox.Show("You must have opened project which changes you want to upload.", "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    return;
                }

                var appointments = mainViewModel.Appointments.Where(_ => _.Location == mainViewModel.Project.Id);
                if (appointments == null || appointments.Count() == 0)
                {
                    MessageBox.Show("Activ project is not part of any scheduler.", "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    return;
                }

                //Save project before upload
                mainViewModel.SaveProject(false);

                foreach (var appointment in appointments)
                {
                    int version = 0;
                    if (Int32.TryParse(appointment.Url, out version))
                    {
                        appointment.Url = (++version).ToString();
                    }

                    if (!UploadProject(appointment))
                    {
                        MessageBox.Show("Uploading problem. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Uploading problem. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                log.Info("AddSchedulerButton_Click", ex);
            }
        }
        private bool UploadProject(Appointment appointment)
        {
            bool result = false;
            string errorResponse = String.Empty;
            try
            {
                progressWindow.SafeShow("Uploading file in progress...");
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                result = CeitconServerHelper.UploadScheduler(appointment, out errorResponse, progressWindow);
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                progressWindow.SafeHide();
            }
            catch (Exception ex)
            {
                progressWindow.SafeHide();
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                log.Error("UploadProject", ex);
            }
            return result;
        }

        private int OLD_SameItemsCount(string player, DateTime start, DateTime end, string excludeId)
        {
            return (DataContext as MainViewModel).Appointments.Where(_ => 
            _.Subject == player && (_.Start < end && _.End > start) && _.UniqueId != excludeId).Count();
        }
        private int SameItemsCount(string player, DateTime start, DateTime end, string excludeId)
        {
            var result = (DataContext as MainViewModel).Appointments.Where(_ =>
                        _.Subject == player &&
                        _.UniqueId != excludeId &&
                        ((start >= _.Start && start <= _.End) || (end >= _.Start && end <= _.End))
                ).Count();
            return result;
        }

        private void TestConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log.Info("TestConnection");
                //progressWindow.SafeShow("Please wait, testing connection...");
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (CeitconServerHelper.TestConnection())
                    MessageBox.Show("Successfully connected to server.", "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                else
                    MessageBox.Show("No signal from server.", "Info", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                //progressWindow.SafeHide();
            }
            catch (Exception ex)
            {
                //progressWindow.SafeHide();
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                log.Error("Test server connection: ", ex);
                MessageBox.Show("No signal from server.", "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }
        #endregion


    }

    public class WindowDialogHost : Window, IScheduleViewDialogHost
    {
        public new event EventHandler<WindowClosedEventArgs> Closed;
        public ScheduleViewBase ScheduleView
        {
            get;
            set;
        }
        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            if (this.Closed != null)
            {
                this.Closed(this, new WindowClosedEventArgs());
            }
        }

        public void Show(bool isModal)
        {
            if (this.Owner == null && this.ScheduleView != null)
            {
                this.Owner = this.ScheduleView.ParentOfType<Window>();
            }
            if (isModal)
            {
                this.ShowDialog();
            }
            else
            {
                this.Show();
            }
        }
    }

    public class CustomScheduleViewDialogHostFactory : ScheduleViewDialogHostFactory
    {
        protected override IScheduleViewDialogHost CreateNew(ScheduleViewBase scheduleView, DialogType dialogType)
        {
            var window = new WindowDialogHost
            {
                Content = new SchedulerDialog(),
                ScheduleView = scheduleView,
            };
            if (dialogType == DialogType.AppointmentDialog)
            {
                window.ResizeMode = ResizeMode.CanResize;
                window.Width = 580;
                window.Height = 600;
                window.Background = new SolidColorBrush(Colors.LightGray);
            }
            return window;
        }
    }
}