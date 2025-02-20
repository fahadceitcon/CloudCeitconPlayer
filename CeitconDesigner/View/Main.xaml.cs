using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Network;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Designer.Controls;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using log4net;
using System.Globalization;
using Ceitcon_Data.Model.Data;
using Telerik.Windows.Controls.ScheduleView;
using System.Configuration;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Main()
        {
            InitializeComponent();

            logger.Info("Connect Async Called at Window Load Event");
           // SigalrHelper.ConnectAsync();

            //MQTTConnect();

            //RTSP Test Links with IP:
            //  rtsp://184.72.239.149/vod/mp4:BigBuckBunny_175k.mov
            //H.264 Test Video codec Streaming:
            //http://archive.org/download/SampleMpeg4_201307/sample_mpeg4.mp4

            //manual gather - NewPrimary name ----------------------------------

            //ChangeDisplaySettings();
            //this.Height = SystemParameters.MaximizedPrimaryScreenHeight;
            //this.Width = SystemParameters.MaximizedPrimaryScreenWidth;
            //this.Top = 0;
            //this.Left = 0;


            //if (System.Windows.Forms.Screen.AllScreens.Length == 1)
            //{
            //   // MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            //}

            //    this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            //this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            //this.Left = 0;
            //this.Top = 0;
            //this.WindowState = WindowState.Normal;
            //MaxHeight = SystemParameters.MaximumWindowTrackHeight - 40;
            //MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;




            //this.MaxHeight = SystemParameters.VirtualScreenHeight; //Maximize toolbox
            //Rect workArea = SystemParameters.WorkArea;
            //this.MaxHeight = SystemParameters.WorkArea.Height;// SystemParameters.MaximizedPrimaryScreenHeight;

            //foreach (System.Windows.Forms.Screen s in System.Windows.Forms.Screen.AllScreens)
            //{
            //    if (s.Primary)
            //    {
            //        s.Bounds.
            //    }
            //    else
            //    {

            //    }
            //}
            //    var sTwo = System.Windows.Forms.Screen.AllScreens[1];
            //sTwo.M
            //var scr = GetCurrentScreen();
            //if (scr != null)
            //{
            //    if (scr.Primary)
            //    {
            //        this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            //        this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            //    }
            //    else
            //    {
            //        this.MaxHeight = double.PositiveInfinity; //even ridiculous values don't work
            //        this.MaxWidth = double.PositiveInfinity;
            //        this.Height = scr.WorkingArea.Height; // correct values of 2nd screen
            //        this.Width = scr.WorkingArea.Width;
            //    }
            //}
            //else
            //{
            //    this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            //    this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            //}
        }

        #region Commands
        private void NewProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project == null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void NewProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).CreateProject();
            mainTab.SelectedIndex = 2; //Information tab
        }

        private void OpenProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project == null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void OpenProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenProject();
            
            dialog.DataContext = DataContext;
            //dialog.ShowDialog();
            dialog.ShowInTaskbar = true;
            dialog.Show();

        }

        private void OpenFileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project == null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void OpenFileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).OpenProject();
        }

        private void SaveProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void SaveProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).SaveProject(true);
        }

        private void SaveAsProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void SaveAsProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).SaveAsProject();
        }

        private void CloseProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CloseProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).CloseProject();
            ucRegions.RegionCanvas.Children.Clear();
            ucDesigner.DesignerCanvas.Children.Clear();
            mainTab.SelectedIndex = 0;
        }

        private void ExitProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Source is Button)
            {
                if (MessageBox.Show("Are you sure?", "Close application confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    Application.Current.Shutdown();
            }
        }

        private void AddSchedulerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void AddSchedulerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Disable ar-SA date time culture
            //CultureInfo _cinfor = System.Threading.Thread.CurrentThread.CurrentCulture;
            //if (_cinfor.Name.ToLower() == "ar-sa")
            //    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            //(DataContext as MainViewModel).CreateAppointment("Dysplay 1", (DataContext as MainViewModel).Project, DateTime.Now.Date, DateTime.Now.Date + new TimeSpan(23, 59, 59));
        }

        private void RemoveSlideCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RemoveSlideCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sm = (e.OriginalSource as Button).DataContext as SlideModel;
            (DataContext as MainViewModel).DeleteSlide(sm);
        }

        private void RemovePlaylistCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RemovePlaylistCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistModel plm = (e.OriginalSource as Button).DataContext as PlaylistModel;
            if (plm.Parent.Parent is AlertModel)
            {
                (DataContext as MainViewModel).DeletePlaylistAlert(plm);
            }
            else
            {
                (DataContext as MainViewModel).DeletePlaylist(plm);
            }
        }

        private void AddDependsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddDependsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var PopupDepends = (e.Source as DesignerControl).FindName("PopupDepends") as Popup;
            PopupDepends.IsOpen = true;
            //PopupDepends popup = new PopupDepends();
            //var point = Mouse.GetPosition(Application.Current.MainWindow);
            //popup.Margin = new Thickness(point.X, point.Y, 0, 0);
            //popup.ShowDialog();

            (DataContext as MainViewModel).AddDepends((e.OriginalSource as Button).DataContext as PlaylistModel, (e.OriginalSource as Button).DataContext as PlaylistModel);
        }

        private void RemoveDependsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RemoveDependsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).RemoveDepends((e.OriginalSource as Button).DataContext as PlaylistModel);
        }

        private void AddSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = null;
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Parent.Parent is AlertModel)
                {
                    c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                }
                else
                {
                    c = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl;
                }

                string filename = ShowDialog(c);

                if (!String.IsNullOrEmpty(filename))
                {
                    long length = new FileInfo(filename).Length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).Content = filename;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = length;
                    c.Source = filename;
                }
            }
            catch (Exception) { }
        }

        private void AddFajrSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddFajrSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                string filename = ShowDialog(c);
                if (!String.IsNullOrEmpty(filename))
                {
                    //((e.OriginalSource as Button).DataContext as SetContentModel).Fajr = filename;
                    //c.Fajr = filename;

                    long length = new FileInfo(filename).Length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).Fajr = filename;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).FajrSize = length;
                    c.Fajr = filename;
                    c.FajrSize = length;
                    logger.Info($"Add Fajr Image : {filename} length {length} ");


                }
            }
            catch (Exception) { }
        }

        private void AddDhuhrSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddDhuhrSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                string filename = ShowDialog(c);
                if (!String.IsNullOrEmpty(filename))
                {
                    //((e.OriginalSource as Button).DataContext as SetContentModel).Dhuhr = filename;
                    //c.Dhuhr = filename;

                    long length = new FileInfo(filename).Length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).Dhuhr = filename;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).DhuhrSize = length;
                    c.Dhuhr = filename;
                    c.DhuhrSize = length;
                }
            }
            catch (Exception) { }
        }

        private void AddAsrSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddAsrSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                string filename = ShowDialog(c);
                if (!String.IsNullOrEmpty(filename))
                {
                    //((e.OriginalSource as Button).DataContext as SetContentModel).Asr = filename;
                    //c.Asr = filename;

                    long length = new FileInfo(filename).Length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).Asr = filename;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).AsrSize = length;
                    c.Asr = filename;
                    c.AsrSize = length;
                }
            }
            catch (Exception) { }
        }

        private void AddMaghribSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddMaghribSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                string filename = ShowDialog(c);
                if (!String.IsNullOrEmpty(filename))
                {
                    //((e.OriginalSource as Button).DataContext as SetContentModel).Maghrib = filename;
                    //c.Maghrib = filename;
                    long length = new FileInfo(filename).Length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).Maghrib = filename;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).MaghribSize = length;
                    c.Maghrib = filename;
                    c.MaghribSize = length;
                }
            }
            catch (Exception) { }
        }

        private void AddIshaSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddIshaSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                string filename = ShowDialog(c);
                if (!String.IsNullOrEmpty(filename))
                {
                    //((e.OriginalSource as Button).DataContext as SetContentModel).Isha = filename;
                    //c.Isha = filename;

                    long length = new FileInfo(filename).Length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).Isha = filename;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = length;
                    ((e.OriginalSource as Button).DataContext as SetContentModel).IshaSize = length;
                    c.Isha = filename;
                    c.IshaSize = length;
                }
            }
            catch (Exception) { }
        }

        private void ClearSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Content != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = true;
            }
        }

        private void ClearSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = null;
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Parent.Parent is AlertModel)
                {
                    c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                }
                else
                {
                    c = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl;
                }
                ((e.OriginalSource as Button).DataContext as SetContentModel).Content = null;
                ((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = 0;
                c.Source = null;
            }
            catch (Exception) { }
        }

        private void ClearFajrSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Fajr != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = true;
            }
        }

        private void ClearFajrSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                ((e.OriginalSource as Button).DataContext as SetContentModel).Fajr = null;
                c.Fajr = null;


            }
            catch (Exception) { }
        }

        private void ClearDhuhrSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Dhuhr != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = true;
            }
        }

        private void ClearDhuhrSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                ((e.OriginalSource as Button).DataContext as SetContentModel).Dhuhr = null;
                c.Dhuhr = null;
            }
            catch (Exception) { }
        }

        private void ClearAsrSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Asr != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = true;
            }
        }

        private void ClearAsrSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                ((e.OriginalSource as Button).DataContext as SetContentModel).Asr = null;
                c.Asr = null;
            }
            catch (Exception) { }
        }

        private void ClearMaghribSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Maghrib != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = true;
            }
        }

        private void ClearMaghribSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                ((e.OriginalSource as Button).DataContext as SetContentModel).Maghrib = null;
                c.Maghrib = null;
            }
            catch (Exception) { }
        }

        private void ClearIshaSourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Isha != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = true;
            }
        }

        private void ClearIshaSourceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
                ((e.OriginalSource as Button).DataContext as SetContentModel).Isha = null;
                c.Isha = null;
            }
            catch (Exception) { }
        }

        private void DataGridSettingsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (((e.OriginalSource as Button).DataContext as SetContentModel).DataGrid.SelectedSource != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = true;
            }
        }

        private void DataGridSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SetContentModel sc = (e.OriginalSource as Button).DataContext as SetContentModel;
                sc.DataGrid.SelectedSource.SelectedColumn = sc.DataGrid.SelectedSource.Columns.FirstOrDefault();
                var dialog = new DataGridSettings();
                dialog.MainView = (DataContext as MainViewModel);
                dialog.DataContext = sc;
                if (dialog.ShowDialog() == true)
                {

                }
            }
            catch (Exception) { }
        }

        private void WeatherSettingsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (((e.OriginalSource as Button).DataContext as SetContentModel).Weather != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = true;
            }
        }

        private void WeatherSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SetContentModel sc = (e.OriginalSource as Button).DataContext as SetContentModel;
                var dialog = new WeatherSettings();
                dialog.MainView = (DataContext as MainViewModel);
                dialog.DataContext = sc;
                if (dialog.ShowDialog() == true)
                {

                }
            }
            catch (Exception) { }
        }

        private void OpenTextEditorCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if ((e.OriginalSource as Button).DataContext != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = false;
            }
        }

        private void OpenTextEditorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SetContentModel sc = (e.OriginalSource as Button).DataContext as SetContentModel;
                var dialog = new RichTextEditorWindow();
                dialog.DataContext = sc.Content;
                if (dialog.ShowDialog() == true)
                {
                    sc.Content = dialog.DataContext.ToString();
                }
            }
            catch (Exception) { }
        }

        private void RemoveAppointmentCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if ((e.OriginalSource as Button).DataContext != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            catch (Exception)
            {
                e.CanExecute = false;
            }
        }

        private void RemoveAppointmentCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var appointment = (e.OriginalSource as Button).DataContext as Appointment;
                if (appointment != null)
                    (DataContext as MainViewModel).DeleteAppointment(appointment);
            }
            catch (Exception) { }
        }

        private void ShowProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void ShowProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).ShowProject();
        }

        private void ShowDesignerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void ShowDesignerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).CreateDesigner();
        }

        private void RemoveAlertCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RemoveAlertCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainViewModel).DeleteAlert();
        }

        private void UploadPlayerRefreshCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void UploadPlayerRefreshCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var model = (e.OriginalSource as Button).DataContext as PlayerModel;

                if (String.IsNullOrEmpty(model.Name))
                {
                    MessageBox.Show("Missing player name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (model.RefreshTime < 30)
                {
                    MessageBox.Show("Refresh time must be number and can't be less then 30 sec.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Mouse.OverrideCursor = Cursors.Wait;
                bool response = CeitconServerHelper.UpdatePlayerRefresh(model.Name, model.RefreshTime);
                Mouse.OverrideCursor = Cursors.Arrow;
                if (response)
                {
                    MessageBox.Show($"Player '{model.Name}' refresh time has updated on {model.RefreshTime} seconds.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Problem with uploading player refresh time.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with uploading data on server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void ProofOfPlayCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ProofOfPlayCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string popApplicationName = ConfigurationManager.AppSettings["POP"];
                if (String.IsNullOrEmpty(popApplicationName))
                {
                    MessageBox.Show("Missing key.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var model = (e.OriginalSource as Button).DataContext as PlayerModel;

                // Which screen do you want?
                int ScreenNo = 1;
                if(model.Screens > 1)
                { 
                    var dialog = new ProofOfPlaySettingsWindow(model.Screens);
                    if (dialog.ShowDialog() == true)
                    {
                        ScreenNo = dialog.Screens;
                    }
                    else
                    {
                        //stop all
                        return;
                    }
                }

                // Call POP application
                string exeFileLocation = Path.Combine(Environment.CurrentDirectory, popApplicationName);
                if (File.Exists(exeFileLocation))
                {
                    System.Diagnostics.Process.Start(exeFileLocation, $"{model.IPAddress} {ScreenNo}");
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with running Proof Of Play action.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterPlayerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RegisterPlayerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var model = (e.OriginalSource as Button).DataContext as PlayerModel;
                Mouse.OverrideCursor = Cursors.Wait;
                bool haveLicence = CeitconServerHelper.CheckFreeLicence(0);
                Mouse.OverrideCursor = Cursors.Arrow;
                if (haveLicence)
                {
                    var dialog = new PlayerWindow();
                    var pm = (e.OriginalSource as Button).DataContext as PlayerModel;
                    dialog.DataContext = pm;
                    string bacupHost = pm.HostName;
                    string bacupIp = pm.IPAddress;
                    if (dialog.ShowDialog() == true)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (pm.HostName.Trim() == "" )
                        {
                            MessageBox.Show("Enter Host Name ", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        if (pm.IPAddress.Trim() == "")
                        {
                            MessageBox.Show("Enter IPAddress ", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        if (CeitconServerHelper.RegistratePlayer(pm.Name, pm.HostName, pm.IPAddress, 1, 1,0))
                        {
                            SendRegistrateCommand(true, pm.Name, pm.IPAddress, pm.HostName);
                            model.Licence = 1;
                            model.Screens = 1;
                            model.Status = 3;
                            model.RefreshTime = 30;
                            model.RegistredTime = DateTime.UtcNow;
                            model.LastConnection = DateTime.UtcNow;
                            MessageBox.Show("Player is registred", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            pm.HostName = bacupHost;
                            pm.IPAddress = bacupIp;
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else
                    {
                        pm.HostName = bacupHost;
                        pm.IPAddress = bacupIp;
                    }
                }
                else
                {
                    MessageBox.Show("No more licence.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with registrate player.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void UnregisterPlayerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void UnregisterPlayerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                var model = (e.OriginalSource as Button).DataContext as PlayerModel;
                if (CeitconServerHelper.DisconectPlayer(model.Name))
                {
                    //SendRegistrateCommand(false, model.Name, model.IPAddress, model.HostName);
                    model.Screens = 0;
                    model.Licence = 0;
                    model.Status = 0;
                    model.RefreshTime = 30;
                    model.RegistredTime = null;
                    model.LastConnection = null;
                    MessageBox.Show("Player is unregistred.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with unregistrate player.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void EditPlayerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void EditPlayerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var dialog = new PlayerWindow();
                var pm = (e.OriginalSource as Button).DataContext as PlayerModel;
                dialog.DataContext = pm;
                string bacupHost = pm.HostName;
                string bacupIp = pm.IPAddress;
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    if (CeitconServerHelper.RegistratePlayer(pm.Name, pm.HostName, pm.IPAddress, pm.Screens, pm.Licence, 3))
                    {
                        //Send MQTT message
                        //PlayerGroupModel pg = (DataContext as MainViewModel).GetPlayerGroup(pm.Name);
                        //if (pg != null)
                        //    MQTTRegistrate(pg.Name, pm.Name);
                        SendRegistrateCommand(true, pm.Name, pm.IPAddress, pm.HostName);
                        MessageBox.Show("Player is updated.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        pm.HostName = bacupHost;
                        pm.IPAddress = bacupIp;
                    }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                else
                {
                    pm.HostName = bacupHost;
                    pm.IPAddress = bacupIp;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with update player.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void SendRTMessageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SendRTMessageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var model = ((e.OriginalSource as Button).DataContext as MainViewModel).SelectedRTMessage;
                if (String.IsNullOrWhiteSpace(model.Name) || String.IsNullOrWhiteSpace(model.Topic) || String.IsNullOrWhiteSpace(model.Message))
                {
                    MessageBox.Show("All field must be filled", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //if (this.Connection != null && this.Connection.State == ConnectionState.Connected && this.HubProxy != null)
                //{
                Mouse.OverrideCursor = Cursors.Wait;
                //HubProxy.Invoke("SendRTMessage", new object[] { model.Name, model.Topic, model.Message });
                if (SendMessage(model.Name, model.Topic, model.Message) > 0)
                {
                    MessageBox.Show("Message has been sent", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    model.Message = null;
                }
                else
                {
                    MessageBox.Show("Don't exist any registered player in selected network item.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
                //}
                //else
                //{
                //    MessageBox.Show("Not connected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with sending message to server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private int SendMessage(string path, string topic, string message)
        {
            int i = 0;
            foreach (PlayerModel item in (DataContext as MainViewModel).GetRegistredPlayers(path))
            {
                //SigalrHelper.SendMessage(item.Name, topic, message);
                // HubProxy.Invoke("SendRTMessage", new object[] { item.Name, topic, message });
                i++;
            }
            return i;
        }

        private void SendRegistrateCommand(bool bRegistered, string PlayerName, string IPaddress, string HostName)
        {

            try
            {
               // SigalrHelper._SendRegistrateCommand(bRegistered, PlayerName, IPaddress, HostName);
            }
            catch (Exception)
            {


            }
        }

        private void UploadLogoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void UploadLogoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var model = (e.OriginalSource as Button).DataContext as LogoModel;

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".png";
                dlg.Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|All Files (*.*)|*.*";
                if (dlg.ShowDialog() == true)
                {
                    //Convert to PNG (256x100) and save 
                    var image = ImageHelper.ResizeImage(System.Drawing.Image.FromFile(dlg.FileName, true), 256, 100);

                    string path = Path.Combine(System.Environment.CurrentDirectory, "Logos", String.Format("{0}.png", model.Id));
                    if (!Directory.Exists(Path.GetDirectoryName(path)))
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                    model.FileLocation = path;
                    model.FileSize = (new FileInfo(path)).Length;

                    string errorResponse = String.Empty;
                    Mouse.OverrideCursor = Cursors.Wait;
                    if (CeitconServerHelper.UploadLogo(model, out errorResponse))
                    {
                        MessageBox.Show("Logo file is uploaded. Refresh data grid to see changes.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Problem with uploading file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with uploading file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void DeleteLogoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DeleteLogoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var model = (e.OriginalSource as Button).DataContext as LogoModel;
                string errorResponse = String.Empty;
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteLogo(model.Id))
                {
                    File.Copy(Path.Combine(Path.GetDirectoryName(model.FileLocation), "__.png"), model.FileLocation, true);
                    MessageBox.Show("Logo is deleted. Refresh data grid to see changes.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Problem with deleting logo on server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with deleting logo on server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }
        #endregion

        #region Events
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Second monitor fix (https://social.msdn.microsoft.com/Forums/vstudio/en-US/75d72afa-2fff-44fd-9f0a-87753002daa1/mediaelement-not-working-with-multiple-monitors?forum=wpf)
            if (System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
                this.Left = (System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Width - this.Width) / 2;
                this.Top = (System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Height - this.Height) / 2;
                this.WindowState = WindowState.Maximized;

                HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                if (hwndSource != null)
                {
                    HwndTarget hwndTarget = hwndSource.CompositionTarget;
                    hwndTarget.RenderMode = RenderMode.SoftwareOnly;
                }
            }

            KillProcess("cpop");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //SigalrHelper.DisconnectSignalR();
            KillProcess("cpop");

            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Utilities.IOManagerScheduler.SaveScheduler((DataContext as MainViewModel).Appointments.ToArray(), Utilities.IOManagerScheduler.SchedulerFile );
            //SQLiteHelper.Instance.SaveScheduler((DataContext as MainViewModel).Appointments.ToArray());
        }

        private void DragMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point position = e.GetPosition(hotspotRectangle);
                if (position.Y < hotspotRectangle.ActualHeight)
                {
                    DragMove();
                }
            }
            catch (Exception) { }
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            var Target = Window.GetWindow(this);
            if (Target.WindowState == WindowState.Maximized)
            {
                Target.WindowState = WindowState.Normal;
            }
            else if (Target.WindowState == WindowState.Normal)
            {
                Target.WindowState = WindowState.Maximized;
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void ShrinkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (TabItem item in mainTab.Items)
            {
                (item.Header as StackPanel).Width = (item.Header as StackPanel).Width == 200 ? 30 : 200;
            }
        }

        #endregion

        #region Private
        private string ShowDialog(ControlModel c)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            switch (c.Type)
            {
                case ControlType.Image:
                case ControlType.PrayerImage:
                    {
                        dlg.DefaultExt = ".jpg";
                        dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|All Files (*.*)|*.*";
                    }
                    break;
                case ControlType.Video:
                case ControlType.PrayerVideo:
                    {
                        dlg.DefaultExt = ".mp4";
                        dlg.Filter = "MP4 Files (*.mp4)|*.mp4|AVI Files (*.avi)|*.avi|MOV Files (*.mov)|*.mov|WMV Files (*.wmv)|*.wmv|MKV Files (*.mkv)|*.mkv|All Files (*.*)|*.*";
                    }
                    break;
                case ControlType.GifAnim:
                    {
                        dlg.DefaultExt = ".gif";
                        dlg.Filter = "GIF Files (*.gif)|*.gif";
                    }
                    break;
                case ControlType.PDF:
                    {
                        dlg.DefaultExt = ".pdf";
                        dlg.Filter = "PDF Files (*.pdf)|*.pdf";
                    }
                    break;
                case ControlType.PPT:
                    {
                        dlg.DefaultExt = ".pptx";
                        dlg.Filter = "Power Point Files (*.pptx)|*.pptx|Power Point Files (*.ppt)|*.ppt";
                    }
                    break;
                default:
                    dlg.DefaultExt = ".*";
                    dlg.Filter = "All Files (*.*)|*.*";
                    break;
            }
            Nullable<bool> result = dlg.ShowDialog();
            return result == null ? null : dlg.FileName;
        }

        static void KillProcess(string name)
        {
            try
            {
                foreach (var process in System.Diagnostics.Process.GetProcessesByName(name))
                {
                    process.Kill();
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand NewProject = new RoutedUICommand("NewProject", "NewProject", typeof(CustomCommands));
        public static readonly RoutedUICommand OpenProject = new RoutedUICommand("OpenProject", "OpenProject", typeof(CustomCommands));
        public static readonly RoutedUICommand OpenFile = new RoutedUICommand("OpenFile", "OpenFile", typeof(CustomCommands));
        public static readonly RoutedUICommand SaveProject = new RoutedUICommand("SaveProject", "SaveProject", typeof(CustomCommands));
        public static readonly RoutedUICommand SaveAsProject = new RoutedUICommand("SaveAsProject", "SaveAsProject", typeof(CustomCommands));
        public static readonly RoutedUICommand CloseProject = new RoutedUICommand("CloseProject", "CloseProject", typeof(CustomCommands));
        public static readonly RoutedUICommand ExitProject = new RoutedUICommand("ExitProject", "ExitProject", typeof(CustomCommands));
        public static readonly RoutedUICommand AddScheduler = new RoutedUICommand("AddScheduler", "AddScheduler", typeof(CustomCommands));
        public static readonly RoutedUICommand RemoveSlide = new RoutedUICommand("RemoveSlide", "RemoveSlide", typeof(CustomCommands));
        public static readonly RoutedUICommand RemovePlaylist = new RoutedUICommand("RemovePlaylist", "RemovePlaylist", typeof(CustomCommands));
        public static readonly RoutedUICommand AddDepends = new RoutedUICommand("AddDepends", "AddDepends", typeof(CustomCommands));
        public static readonly RoutedUICommand RemoveDepends = new RoutedUICommand("RemoveDepends", "RemoveDepends", typeof(CustomCommands));
        public static readonly RoutedUICommand AddSource = new RoutedUICommand("AddSource", "AddSource", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearSource = new RoutedUICommand("ClearSource", "ClearSource", typeof(CustomCommands));
        public static readonly RoutedUICommand ShowProject = new RoutedUICommand("ShowProject", "ShowProject", typeof(CustomCommands));
        public static readonly RoutedUICommand ShowDesigner = new RoutedUICommand("ShowDesigner", "ShowDesigner", typeof(CustomCommands));
        public static readonly RoutedUICommand RemoveAlert = new RoutedUICommand("RemoveAlert", "RemoveAlert", typeof(CustomCommands));
        public static readonly RoutedUICommand UploadPlayerRefresh = new RoutedUICommand("UploadPlayerRefreshRefresh", "UploadPlayerRefreshRefresh", typeof(CustomCommands));
        public static readonly RoutedUICommand ProofOfPlay = new RoutedUICommand("ProofOfPlay", "ProofOfPlay", typeof(CustomCommands));
        public static readonly RoutedUICommand RegisterPlayer = new RoutedUICommand("RegisterPlayer", "RegisterPlayer", typeof(CustomCommands));
        public static readonly RoutedUICommand UnregisterPlayer = new RoutedUICommand("UnregisterPlayer", "UnregisterPlayer", typeof(CustomCommands));
        public static readonly RoutedUICommand EditPlayer = new RoutedUICommand("EditPlayer", "EditPlayer", typeof(CustomCommands));
        public static readonly RoutedUICommand SendRTMessage = new RoutedUICommand("SendRTMessage", "SendRTMessage", typeof(CustomCommands));
        public static readonly RoutedUICommand DataGridSettings = new RoutedUICommand("DataGridSettings", "DataGridSettings", typeof(CustomCommands));
        public static readonly RoutedUICommand WeatherSettings = new RoutedUICommand("WeatherSettings", "WeatherSettings", typeof(CustomCommands));
        public static readonly RoutedUICommand OpenTextEditor = new RoutedUICommand("OpenTextEditor", "OpenTextEditor", typeof(CustomCommands));
        public static readonly RoutedUICommand RemoveAppointment = new RoutedUICommand("RemoveAppointment", "RemoveAppointment", typeof(CustomCommands));
        public static readonly RoutedUICommand UploadLogo = new RoutedUICommand("UploadLogo", "UploadLogo", typeof(CustomCommands));
        public static readonly RoutedUICommand DeleteLogo = new RoutedUICommand("DeleteLogo", "DeleteLogo", typeof(CustomCommands));
        public static readonly RoutedUICommand AddFajrSource = new RoutedUICommand("AddFajrSource", "AddFajrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand AddDhuhrSource = new RoutedUICommand("AddDhuhrSource", "AddDhuhrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand AddAsrSource = new RoutedUICommand("AddAsrSource", "AddAsrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand AddMaghribSource = new RoutedUICommand("AddMaghribSource", "AddFajrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand AddIshaSource = new RoutedUICommand("AddIshaSource", "AddFajrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearFajrSource = new RoutedUICommand("ClearFajrSource", "ClearFajrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearDhuhrSource = new RoutedUICommand("ClearDhuhrSource", "ClearDhuhrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearAsrSource = new RoutedUICommand("ClearAsrSource", "ClearAsrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearMaghribSource = new RoutedUICommand("ClearMaghribSource", "ClearFajrSource", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearIshaSource = new RoutedUICommand("ClearIshaSource", "ClearFajrSource", typeof(CustomCommands));
    }
}
