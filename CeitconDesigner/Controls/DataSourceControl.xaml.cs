using Ceitcon_Data.Model.Data;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.ViewModel;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Xml;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for DataSourceControl.xaml
    /// </summary>
    public partial class DataSourceControl : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DataSourceControl()
        {
            InitializeComponent();
        }

        //public static readonly DependencyProperty NewDataSourceProperty = DependencyProperty.Register
        //(
        //     "NewDataSource",
        //     typeof(DataSourceModel),
        //     typeof(DataSourceControl),
        //     new PropertyMetadata(null)
        //);
        
        //public DataSourceModel NewDataSource
        //{
        //    get { return (DataSourceModel)GetValue(NewDataSourceProperty); }
        //    set { SetValue(NewDataSourceProperty, value); }
        //}

        private void AddSourceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((DataContext as MainViewModel).SelectedDataSource != null)
                {

                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.DefaultExt = ".xml";
                    dlg.Filter = "XML Files (*.xml)|*.xml";
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        (DataContext as MainViewModel).SelectedDataSource.Source = dlg.FileName;
                        //long length = new FileInfo(filename).Length;
                        //((e.OriginalSource as Button).DataContext as SetContentModel).Content = filename;
                        //((e.OriginalSource as Button).DataContext as SetContentModel).ContentSize = length;
                    }
                }
                else
                {
                    MessageBox.Show("Please select data source in list", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception)
            { }
        }

        private void RemoveSourceButton_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).SelectedDataSource != null)
            {
                (DataContext as MainViewModel).SelectedDataSource.Source = null;
            }
            else
            {
                MessageBox.Show("Please select data source in list", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void UploadSourceButton_Click(object sender, RoutedEventArgs e)
        {
            DataSourceModel dsm = (DataContext as MainViewModel).SelectedDataSource;
            if(dsm == null)
            {
                MessageBox.Show("Please select data source in list", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (String.IsNullOrEmpty(dsm.Name))
            {
                MessageBox.Show("Missing name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (this.rb1.IsChecked == true)
            {
                if (String.IsNullOrEmpty(dsm.Source))
                {
                    MessageBox.Show("Missing file path.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Mouse.OverrideCursor = Cursors.Wait;
                bool response = AddSource(dsm.Source, String.Format("Local file {0}", dsm.Source));
                Mouse.OverrideCursor = Cursors.Arrow;
                if (response)
                {
                    MessageBox.Show("Data Source is added on server.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    //dsm = new DataSourceModel();
                }
                else
                    MessageBox.Show("Problem with adding.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (this.rb2.IsChecked == true)
            {
                if (String.IsNullOrEmpty(dsm.Url))
                {
                    MessageBox.Show("Missing Url path.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Mouse.OverrideCursor = Cursors.Wait;
                bool response = downloadFileHTTP(dsm.Url, dsm.UrlUsername, dsm.UrlPassword);
                Mouse.OverrideCursor = Cursors.Arrow;
                if (response)
                {
                    dsm.Source = dsm.Url;
                    MessageBox.Show("Data Source is added on server.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    //dsm = new DataSourceModel();
                }
                else
                    MessageBox.Show("Problem with adding.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AddSource(string path, string description)
        {
            DataSourceModel dsm = (DataContext as MainViewModel).SelectedDataSource;
            if (dsm == null)
            {
                MessageBox.Show("Please select data source in list", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            log.Info(String.Format("Source added from location {0}", path));
            if (String.IsNullOrEmpty(path) || !File.Exists(path))
                return false;
            bool result = false;
            try
            {
                var columns = new List<string>();
                var records = new List<string>();
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                string rootElement = doc.DocumentElement.Name;
                string recordTag = doc.DocumentElement.ChildNodes[1].Name;
                XmlNodeList elemList = doc.DocumentElement.ChildNodes[1].ChildNodes;
                for (int i = 0; i < elemList.Count; i++)
                {
                    columns.Add(elemList[i].Name);
                }

                if (columns.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < columns.Count; j++)
                    {
                        if (sb.Length > 0)
                            sb.Append(";");
                        sb.Append(columns[j]);
                    }

                    result = CeitconServerHelper.UpdateDataSource(dsm.Id, dsm.Name, sb.ToString(), description, dsm.SourceType, doc.DocumentElement.InnerXml, dsm.Url, dsm.UrlUsername, dsm.UrlPassword);
                }
                log.Info("Source added successfully");
            }
            catch (Exception e)
            {
                result = false;
                log.Error("Add Source", e);
            }
            return result;
        }

        int iMaxValue = 0;
        int iValue = 0;
        private bool downloadFileHTTP(string ServerURL, string username, string password)
        {
            try
            {
                bool result = false;
                log.Info("Connecting...");
                if (String.IsNullOrEmpty(ServerURL))
                {
                    log.Info("Server URL is empty");
                    return false;
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServerURL);
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    request.Credentials = new System.Net.NetworkCredential(username, password);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                log.Info("Retrieving Information...");
                int dataLength = (int)request.GetResponse().ContentLength;
                log.Info("Downloading File...");
                request = request = (HttpWebRequest)WebRequest.Create(ServerURL);
                iValue = 0;
                iMaxValue = dataLength;

                //Streams
                HttpWebResponse response2 = request.GetResponse() as HttpWebResponse;
                Stream reader = response2.GetResponseStream();

                MemoryStream memStream = new MemoryStream();
                log.Info("downloads in chuncks");
                byte[] buffer = new byte[1024];

                while (true)
                {

                    int bytesRead = reader.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        //logger_fids.Info("Nothing was read, finished downloading");
                        //logger_fids.Info(dataLength.ToString() + "/" + dataLength.ToString());
                        break;
                    }
                    else
                    {
                        //Write the downloaded data
                        memStream.Write(buffer, 0, bytesRead);

                        //Update the progress bar
                        if (iValue + bytesRead <= iMaxValue)
                        {
                            iValue += bytesRead;
                            //log.Info(iValue.ToString() + " / " + dataLength.ToString());

                        }
                    }
                }

                byte[] downloadedData = memStream.ToArray();
                string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString() + ".xml");
                File.WriteAllBytes(path, downloadedData);
                result = AddSource(path, String.Format("Server URL {0}", ServerURL));
                //File.Delete(path);

                //Clean up
                reader.Close();
                memStream.Close();
                response.Close();

                log.Info("Downloaded Successfully");
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Download File HTTP", ex);
                return false;
                //logger_fids.Debug(ex, "There was an error connecting to the  Server.", new object());
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataSourceModel dsm = new DataSourceModel();

                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.InsertDataSource(dsm.Id, dsm.Name, String.Empty, String.Empty, dsm.SourceType, String.Empty, dsm.Url, dsm.UrlUsername, dsm.UrlPassword))
                {
                    (DataContext as MainViewModel).DataSources.Add(dsm);
                    (DataContext as MainViewModel).SelectedDataSource = dsm;
                }
                else
                {
                    MessageBox.Show("Data source is not created on server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    log.Error("Data source is not created on server");
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data source is not created on server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                log.Error("Data source is not created on server", ex);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((DataContext as MainViewModel).SelectedDataSource != null)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    if (CeitconServerHelper.DeleteDataSource((DataContext as MainViewModel).SelectedDataSource.Id))
                    {
                        (DataContext as MainViewModel).DataSources.Remove((DataContext as MainViewModel).SelectedDataSource);
                        (DataContext as MainViewModel).SelectedDataSource = (DataContext as MainViewModel).DataSources.FirstOrDefault();
                    }
                    else
                    {
                        MessageBox.Show("Error", "Data source is not deleted from server", MessageBoxButton.OK, MessageBoxImage.Error);
                        log.Error("Data source is not deleted on server");
                    }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "Data source is not deleted from server", MessageBoxButton.OK, MessageBoxImage.Error);
                log.Error("Data source is not deleted on server", ex);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }
    }
}

