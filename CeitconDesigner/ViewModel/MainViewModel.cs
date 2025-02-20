using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Data;
using Ceitcon_Data.Model.Network;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Data.Model.User;
using Ceitcon_Data.Utilities;
using Ceitcon_Designer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using Telerik.Windows.Controls.ScheduleView;

namespace Ceitcon_Designer.ViewModel
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            GenerateFolders();

            Memento.Enable = true;
            _Locations = new ObservableCollection<Ceitcon_Data.Model.LocationModel>(SQLiteHelper.Instance.GetLocations());
            _Countries = new ObservableCollection<string>(Locations.Select(_ => _.Country).Distinct().ToList());
            _Resolutions = GetResolutions();
            _Monitors = GetMonitors();
            _SocialMedias = GetSocialMedias();
            _Appointments = CeitconServerHelper.GetAllSchedularts();

            _SelectedTool = 1;
            _ActivTab = 0;
            ShowNothing();
            string text = Guid.NewGuid().ToString();

            _RTMessages = new ObservableCollection<RTMessageModel>();
            _RTMessages.Add(new RTMessageModel() { MediaType = RTMessageType.Facebook, Message = text, SenderName = Guid.NewGuid().ToString().Substring(0, 10), ApprovedBy = Guid.NewGuid().ToString().Substring(0, 10) });
            _RTMessages.Add(new RTMessageModel() { MediaType = RTMessageType.Instagram, Message = Guid.NewGuid().ToString(), SenderName = Guid.NewGuid().ToString().Substring(0, 10), ApprovedBy = Guid.NewGuid().ToString().Substring(0, 10) });
            _RTMessages.Add(new RTMessageModel() { MediaType = RTMessageType.Twitter, Message = Guid.NewGuid().ToString(), SenderName = Guid.NewGuid().ToString().Substring(0, 10), ApprovedBy = Guid.NewGuid().ToString().Substring(0, 10) });

            _DataSources = CeitconServerHelper.GetDataSources().Sources;
            _OnlineLogos = CeitconServerHelper.GetLogos();
        }


        private void GenerateFolders()
        {
            try
            {
                string pathProjects = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects");
                if (!Directory.Exists(pathProjects))
                    Directory.CreateDirectory(pathProjects);
                string pathThumbs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Thumbs");
                if (Directory.Exists(pathThumbs))
                    Directory.Delete(pathThumbs, true);
                Directory.CreateDirectory(pathThumbs);
            }
            catch (Exception)
            { }
        }

        #region Properties
        private ProjectModel _Project;
        private UserModel _User;
        private bool _DesignerMode;
        private bool _NewProjectMode;
        private int _SelectedTool;
        private int _ActivTab;
        private ObservableCollection<string> _Countries;
        private ObservableCollection<Ceitcon_Data.Model.LocationModel> _Locations;
        private ObservableCollection<ResolutionModel> _Resolutions;
        private ObservableCollection<MonitorModel> _Monitors;
        private ObservableCollection<SocialMediaModel> _SocialMedias;
        private ObservableCollection<Appointment> _Appointments;
        private ObservableCollection<Appointment> _FilteredAppointments;
        private ObservableCollection<NetworkModel> _Network;
        private ObservableCollection<GroupModel> _UserGroups;
        private ObservableCollection<PlayerModel> _Players;
        private PlayerModel _SelectedPlayer;
        private ObservableCollection<RTMessageModel> _RTMessages;
        private RTMessageModel _SelectedRTMessage;
        private ObservableCollection<DataSourceModel> _DataSources;
        private DataSourceModel _SelectedDataSource;
        private ObservableCollection<LogoModel> _OnlineLogos;
        private ObservableCollection<LogoModel> _Logos;
        private LogoModel _SelectedLogo;
        private System.Windows.Media.Brush _CopyBrush;

        public ObservableCollection<Ceitcon_Data.Model.LocationModel> Locations
        {
            get { return _Locations; }
            set
            {
                if (_Locations != value)
                {
                    _Locations = value;
                    OnPropertyChanged("Locations");
                }
            }
        }

        public ObservableCollection<string> Countries
        {
            get { return _Countries; }
            set
            {
                if (_Countries != value)
                {
                    _Countries = value;
                    OnPropertyChanged("Countries");
                }
            }
        }

        public ObservableCollection<ResolutionModel> Resolutions
        {
            get { return _Resolutions; }
            set
            {
                if (_Resolutions != value)
                {
                    _Resolutions = value;
                    OnPropertyChanged("Resolutions");
                }
            }
        }

        public ObservableCollection<MonitorModel> Monitors
        {
            get { return _Monitors; }
            set
            {
                if (_Monitors != value)
                {
                    _Monitors = value;
                    OnPropertyChanged("Monitors");
                }
            }
        }

        public ObservableCollection<SocialMediaModel> SocialMedias
        {
            get { return _SocialMedias; }
            set
            {
                if (_SocialMedias != value)
                {
                    _SocialMedias = value;
                    OnPropertyChanged("SocialMedias");
                }
            }
        }

        public ObservableCollection<Appointment> Appointments
        {
            get { return _Appointments; }
            set
            {
                if (_Appointments != value)
                {
                    _Appointments = value;
                    OnPropertyChanged("Appointments");
                }
            }
        }

        public ObservableCollection<Appointment> FilteredAppointments
        {
            get { return _FilteredAppointments; }
            set
            {
                if (_FilteredAppointments != value)
                {
                    _FilteredAppointments = value;
                    OnPropertyChanged("FilteredAppointments");
                }
            }
        }

        public ObservableCollection<NetworkModel> Network
        {
            get { return _Network; }
            set
            {
                if (_Network != value)
                {
                    _Network = value;
                    OnPropertyChanged("Network");
                }
            }
        }

        public PlayerModel[] GetRegistredPlayers(string path)
        {
            var list = new List<PlayerModel>();
            foreach (var n in _Network)
            {
                foreach (var d in n.Domains)
                {
                    foreach (var c in d.Countries)
                    {
                        foreach (var r in c.Regions)
                        {
                            foreach (var lg in r.LocationGroups)
                            {
                                foreach (var l in lg.Locations)
                                {
                                    foreach (var f in l.Floors)
                                    {
                                        foreach (var z in f.Zones)
                                        {
                                            foreach (var pg in z.PlayerGroups)
                                            {
                                                foreach (var p in pg.Players)
                                                {
                                                    if (p.Active && p.IsRegistred && p.Identification.StartsWith(path))
                                                        list.Add(p);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public void GetPlayers(object item, string nameText = null, string hostText = null, string ipText = null, bool? registred = null)
        {
            try
            {
                if (item == null)
                    return;
                if (item is NetworkModel)
                {
                    var oc = new ObservableCollection<PlayerModel>();
                    foreach (var d in (item as NetworkModel).Domains)
                    {
                        foreach (var c in d.Countries)
                        {
                            foreach (var r in c.Regions)
                            {
                                foreach (var lg in r.LocationGroups)
                                {
                                    foreach (var l in lg.Locations)
                                    {
                                        foreach (var f in l.Floors)
                                        {
                                            foreach (var z in f.Zones)
                                            {
                                                foreach (var pg in z.PlayerGroups)
                                                {
                                                    foreach (var p in pg.Players)
                                                    {

                                                        if ((String.IsNullOrWhiteSpace(nameText) || (!String.IsNullOrWhiteSpace(p.Name) && p.Name.IndexOf(nameText, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                                                        (String.IsNullOrWhiteSpace(hostText) || (!String.IsNullOrWhiteSpace(p.HostName) && p.HostName.IndexOf(hostText, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                                                        (String.IsNullOrWhiteSpace(ipText) || (!String.IsNullOrWhiteSpace(p.IPAddress) && p.IPAddress.IndexOf(ipText, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                                                        (registred == null || p.IsRegistred == registred))
                                                            oc.Add(p);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Players = oc;
                }
                else if (item is DomainModel)
                {
                    var oc = new ObservableCollection<PlayerModel>();
                    foreach (var d in (item as DomainModel).Countries)
                    {
                        foreach (var r in d.Regions)
                        {
                            foreach (var lg in r.LocationGroups)
                            {
                                foreach (var l in lg.Locations)
                                {
                                    foreach (var f in l.Floors)
                                    {
                                        foreach (var z in f.Zones)
                                        {
                                            foreach (var pg in z.PlayerGroups)
                                            {
                                                foreach (var p in pg.Players)
                                                {
                                                    oc.Add(p);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Players = oc;
                }
                else if (item is CountryModel)
                {
                    var oc = new ObservableCollection<PlayerModel>();
                    foreach (var c in (item as CountryModel).Regions)
                    {
                        foreach (var lg in c.LocationGroups)
                        {
                            foreach (var l in lg.Locations)
                            {
                                foreach (var f in l.Floors)
                                {
                                    foreach (var z in f.Zones)
                                    {
                                        foreach (var pg in z.PlayerGroups)
                                        {
                                            foreach (var p in pg.Players)
                                            {
                                                oc.Add(p);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Players = oc;
                }
                else if (item is NetworkRegionModel)
                {
                    var oc = new ObservableCollection<PlayerModel>();
                    foreach (var lg in (item as NetworkRegionModel).LocationGroups)
                    {
                        foreach (var l in lg.Locations)
                        {
                            foreach (var f in l.Floors)
                            {
                                foreach (var z in f.Zones)
                                {
                                    foreach (var pg in z.PlayerGroups)
                                    {
                                        foreach (var p in pg.Players)
                                        {
                                            oc.Add(p);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Players = oc;
                }
                else if (item is LocationGroupModel)
                {
                    var oc = new ObservableCollection<PlayerModel>();
                    foreach (var l in (item as LocationGroupModel).Locations)
                    {
                        foreach (var f in l.Floors)
                        {
                            foreach (var z in f.Zones)
                            {
                                foreach (var pg in z.PlayerGroups)
                                {
                                    foreach (var p in pg.Players)
                                    {
                                        oc.Add(p);
                                    }
                                }
                            }
                        }
                    }
                    Players = oc;
                }
                else if (item is Ceitcon_Data.Model.Network.LocationModel)
                {
                    var oc = new ObservableCollection<PlayerModel>();
                    foreach (var f in (item as Ceitcon_Data.Model.Network.LocationModel).Floors)
                    {
                        foreach (var z in f.Zones)
                        {
                            foreach (var pg in z.PlayerGroups)
                            {
                                foreach (var p in pg.Players)
                                {
                                    oc.Add(p);
                                }
                            }
                        }
                    }
                    Players = oc;
                }
                else if (item is FloorModel)
                {
                    var oc = new ObservableCollection<PlayerModel>();
                    foreach (var z in (item as FloorModel).Zones)
                    {
                        foreach (var pg in z.PlayerGroups)
                        {
                            foreach (var p in pg.Players)
                            {
                                oc.Add(p);
                            }
                        }
                    }
                    Players = oc;
                }
                else if (item is ZoneModel)
                {
                    var oc = new ObservableCollection<PlayerModel>();
                    foreach (var pg in (item as ZoneModel).PlayerGroups)
                    {
                        foreach (var p in pg.Players)
                        {
                            oc.Add(p);
                        }
                    }
                    Players = oc;
                }
                else if (item is PlayerGroupModel)
                {
                    Players = (item as PlayerGroupModel).Players;
                }
                else if (item is PlayerModel)
                {
                    Players = new ObservableCollection<PlayerModel>() { item as PlayerModel };
                }
            }
            catch (Exception) { }
        }

        public ObservableCollection<PlayerModel> Players
        {
            get { return _Players; }
            set
            {
                if (_Players != value)
                {
                    _Players = value;
                    OnPropertyChanged("Players");
                }
            }
        }

        public PlayerModel SelectedPlayer
        {
            get { return _SelectedPlayer; }
            set
            {
                if (_SelectedPlayer != value)
                {
                    _SelectedPlayer = value;
                    OnPropertyChanged("SelectedPlayer");
                }
            }
        }

        public ObservableCollection<RTMessageModel> RTMessages
        {
            get { return _RTMessages; }
            set
            {
                if (_RTMessages != value)
                {
                    _RTMessages = value;
                    OnPropertyChanged("RTMessages");
                }
            }
        }

        public RTMessageModel SelectedRTMessage
        {
            get { return _SelectedRTMessage; }
            set
            {
                if (_SelectedRTMessage != value)
                {
                    _SelectedRTMessage = value;
                    OnPropertyChanged("SelectedRTMessage");
                }
            }
        }

        public ObservableCollection<DataSourceModel> DataSources
        {
            get { return _DataSources; }
            set
            {
                if (_DataSources != value)
                {
                    _DataSources = value;
                    OnPropertyChanged("DataSources");
                }
            }
        }

        public DataSourceModel SelectedDataSource
        {
            get { return _SelectedDataSource; }
            set
            {
                if (_SelectedDataSource != value)
                {
                    _SelectedDataSource = value;
                    OnPropertyChanged("SelectedDataSource");
                }
            }
        }

        public ObservableCollection<LogoModel> OnlineLogos
        {
            get { return _OnlineLogos; }
            set
            {
                if (_OnlineLogos != value)
                {
                    _OnlineLogos = value;
                    OnPropertyChanged("OnlineLogos");
                }
            }
        }

        public ObservableCollection<LogoModel> Logos
        {
            get { return _Logos; }
            set
            {
                if (_Logos != value)
                {
                    _Logos = value;
                    OnPropertyChanged("Logos");
                }
            }
        }

        public LogoModel SelectedLogo
        {
            get { return _SelectedLogo; }
            set
            {
                if (_SelectedLogo != value)
                {
                    _SelectedLogo = value;
                    OnPropertyChanged("SelectedLogo");
                }
            }
        }

        public ObservableCollection<GroupModel> UserGroups
        {
            get { return _UserGroups; }
            set
            {
                if (_UserGroups != value)
                {
                    _UserGroups = value;
                    OnPropertyChanged("UserGroups");
                }
            }
        }

        public ProjectModel Project
        {
            get { return _Project; }
            set
            {
                if (_Project != value)
                {
                    _Project = value;
                    OnPropertyChanged("Project");
                }
            }
        }

        public UserModel User
        {
            get { return _User; }
            set
            {
                if (_User != value)
                {
                    _User = value;
                    OnPropertyChanged("User");
                }
            }
        }

        public bool DesignerMode
        {
            get { return _DesignerMode; }
            set
            {
                if (_DesignerMode != value)
                {
                    Memento.Clear();
                    Memento.ClearR();
                    _DesignerMode = value;
                    OnPropertyChanged("DesignerMode");
                }
            }
        }

        public bool NewProjectMode
        {
            get { return _NewProjectMode; }
            set
            {
                if (_NewProjectMode != value)
                {
                    Memento.Clear();
                    Memento.ClearR();
                    _NewProjectMode = value;
                    OnPropertyChanged("NewProjectMode");
                    OnPropertyChanged("ShowDesignerTab");
                    OnPropertyChanged("ShowAlertTab");
                }
            }
        }

        public bool ShowDesignerTab
        {
            get { return _NewProjectMode && User != null && User.PermissionDesign; }
        }


        public int SelectedTool
        {
            get { return _SelectedTool; }
            set
            {
                if (_SelectedTool != value)
                {

                    _SelectedTool = value;
                    OnPropertyChanged("SelectedTool");
                    OnPropertyChanged("IsAlert");
                    OnPropertyChanged("IsPrayerAlert");
                    OnPropertyChanged("IsGlobalAlert");
                }
            }
        }

        public int ActivTab
        {
            get { return _ActivTab; }
            set
            {
                if (_ActivTab != value)
                {
                    _ActivTab = value;
                    OnPropertyChanged("ActivTab");
                }
            }
        }

        public System.Windows.Media.Brush CopyBrush
        {
            get { return _CopyBrush; }
            set
            {
                if (_CopyBrush != value)
                {
                    _CopyBrush = value;
                    OnPropertyChanged("CopyBrush");
                }
            }
        }

        public bool IsAlert
        {
            get { return _SelectedTool == 3 || _SelectedTool == 4; } //Alert = 3 || 4
        }

        public bool IsPrayerAlert
        {
            get { return _SelectedTool == 3; } //Alert = 3
        }

        public bool IsGlobalAlert
        {
            get { return _SelectedTool == 4; } //Alert = 3
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
        #endregion

        #region Designer Methods
        public void ShowNothing()
        {
            NewProjectMode = false;
            DesignerMode = false;
        }

        public void ShowProject()
        {
            NewProjectMode = true;
            DesignerMode = false;
        }

        public void ShowDesigner()
        {
            NewProjectMode = false;
            DesignerMode = true;
        }

        public void CreateDesigner()
        {
            if (Project != null && Project.Regions != null && Project.Regions.Count > 0 && !String.IsNullOrWhiteSpace(Project.Information.ProjectName))
            {
                ShowDesigner();
            }
            else
            {
                MessageBox.Show("Project must have name and one region!", "Info");
            }
        }
        #endregion

        #region Project
        public void CreateProject()
        {
            Project = new ProjectModel();
            ShowProject();
            _Project.SelectedResolution = _Resolutions.FirstOrDefault();
            _Project.SelectedMonitor = _Monitors.FirstOrDefault();
            ActivTab = 1;
        }

        public bool SaveProject(bool showMessage = true)
        {
            if (Project == null || String.IsNullOrWhiteSpace(Project.Id) || String.IsNullOrWhiteSpace(Project.Information.ProjectName))
                return false;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string projectPath = Path.Combine(path, Project.Id);
            string projectFile = Path.Combine(projectPath, String.Format("{0}{1}", Project.Information.ProjectName, IOManagerProject.ProjectFileExtension));

            if (!IOManagerProject.SaveProject(_Project, projectFile))
            {
                MessageBox.Show("Problem with saving project file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            SQLiteHelper.Instance.InsertProject(_Project.Id, _Project.Information.ProjectName, projectFile);
            if (showMessage)
                MessageBox.Show("Project is saved", "Into", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        public void SaveAsProject()
        {
            if (Project == null)
                return;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = IOManagerProject.ProjectFileFilter;
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.RestoreDirectory = true;
            dialog.FileName = Project.Id;
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = String.Format("{0}\\{1}\\{2}", Path.GetDirectoryName(dialog.FileName), Path.GetFileNameWithoutExtension(dialog.FileName), Path.GetFileName(dialog.FileName));
                if (IOManagerProject.SaveProject(_Project, filePath))
                {
                    SQLiteHelper.Instance.InsertProject(_Project.Id, _Project.Information.ProjectName, filePath);
                    MessageBox.Show("Project is saved", "Into", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Problem with saving project file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void OpenProject()
        {
            try
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = IOManagerProject.ProjectFileFilter;
                dialog.FilterIndex = 0;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OpenProject(dialog.FileName);
                }
                ActivTab = 1;
            }
            catch (Exception) { }
        }

        public void OpenProject(string path)
        {
            try
            {
                Memento.Enable = false;
                Project = IOManagerProject.LoadProject(path);

                //Resolution
                var rm = Resolutions.Where(_ => _.Name == Project.SelectedResolution.Name).FirstOrDefault();
                if (rm == null)
                    Resolutions.Add(Project.SelectedResolution);
                else
                    Project.SelectedResolution = rm;

                //Monitor
                var pm = Monitors.Where(_ => _.Name == Project.SelectedMonitor.Name).FirstOrDefault();
                if (pm == null)
                    Monitors.Add(Project.SelectedMonitor);
                else
                    Project.SelectedMonitor = pm;

                //Location
                var lo = Locations.Where(_ => _.Id == Project?.SelectedAlert?.SelectedSlot?.Location?.Id).FirstOrDefault();
                if (lo != null)
                    Project.SelectedAlert.SelectedSlot.Location = lo;

                //Data Sourced
                foreach (RegionModel rmc in Project.Regions)
                {
                    foreach (SlideModel smc in rmc.Slides)
                    {
                        foreach (LayerModel lmc in smc.Layers)
                        {
                            foreach (ControlModel cmc in lmc.Controls)
                            {
                                if (cmc.Type == ControlType.DataGrid)
                                { }

                                if (cmc.Type == ControlType.DataGrid)
                                {
                                    SetContentModel pl = cmc.Playlist.Where(_ => _.Type == PlaylistType.SetContent).FirstOrDefault() as SetContentModel;
                                    //backup
                                    DataGridModel loadedDG = pl.DataGrid;
                                    //create new
                                    pl.DataGrid = CeitconServerHelper.GetDataSources();
                                    pl.DataGrid.RowBackground = loadedDG.RowBackground;
                                    pl.DataGrid.BorderBrush = loadedDG.BorderBrush;
                                    pl.DataGrid.BorderThickness = loadedDG.BorderThickness;
                                    pl.DataGrid.LinesVisibility = loadedDG.LinesVisibility;
                                    pl.DataGrid.VerticalLineColour = loadedDG.VerticalLineColour;
                                    pl.DataGrid.HorizontalLineColour = loadedDG.HorizontalLineColour;
                                    pl.DataGrid.AlternationCount = loadedDG.AlternationCount;
                                    pl.DataGrid.AlternatingRowBackground = loadedDG.AlternatingRowBackground;
                                    pl.DataGrid.RowCornerRadius = loadedDG.RowCornerRadius;
                                    pl.DataGrid.RowMargin = loadedDG.RowMargin;
                                    pl.DataGrid.IsVisibleShadow = loadedDG.IsVisibleShadow;
                                    pl.DataGrid.RowShadowEffect = loadedDG.RowShadowEffect;
                                    pl.DataGrid.HeaderSize = loadedDG.HeaderSize;
                                    pl.DataGrid.HeaderHeight = loadedDG.HeaderHeight;
                                    pl.DataGrid.HeaderForeground = loadedDG.HeaderForeground;
                                    pl.DataGrid.HeaderBackground = loadedDG.HeaderBackground;
                                    pl.DataGrid.HeaderBorderBrush = loadedDG.HeaderBorderBrush;
                                    pl.DataGrid.HeaderBorderThickness = loadedDG.HeaderBorderThickness;
                                    pl.DataGrid.HeaderCornerRadius = loadedDG.HeaderCornerRadius;
                                    pl.DataGrid.HeaderMargin = loadedDG.HeaderMargin;
                                    pl.DataGrid.HeaderFontFamily = loadedDG.HeaderFontFamily;
                                    pl.DataGrid.HeaderFontWeight = loadedDG.HeaderFontWeight;
                                    pl.DataGrid.HeaderFontStyle = loadedDG.HeaderFontStyle;
                                    pl.DataGrid.HeaderHorizontalAlignment = loadedDG.HeaderHorizontalAlignment;
                                    pl.DataGrid.HeaderVerticalAlignment = loadedDG.HeaderVerticalAlignment;
                                    pl.DataGrid.HeaderIsVisibleShadow = loadedDG.HeaderIsVisibleShadow;
                                    pl.DataGrid.HeaderShadowEffect = loadedDG.HeaderShadowEffect;
                                    pl.DataGrid.MaxRows = loadedDG.MaxRows;
                                    pl.DataGrid.RowHeight = loadedDG.RowHeight;
                                    pl.DataGrid.RefreshTime = loadedDG.RefreshTime;
                                    DataSourceModel ss = pl.DataGrid.Sources.Where(_ => _.Name == loadedDG.SelectedSource.Name).FirstOrDefault();
                                    if (ss != null)
                                    {
                                        pl.DataGrid.SelectedSource = ss;
                                        var ocBackup = ss.Columns;
                                        ss.Columns = new ObservableCollection<DataColumnModel>();

                                        foreach (DataColumnModel cc in loadedDG.SelectedSource.Columns)
                                        {
                                            DataColumnModel dcm = ocBackup.Where(_ => _.Name == cc.Name).FirstOrDefault();
                                            if (dcm != null)
                                            {
                                                dcm.Name = cc.Name;
                                                dcm.Title = cc.Title;
                                                dcm.Width = cc.Width;
                                                dcm.Foreground = cc.Foreground;
                                                dcm.Background = cc.Background;
                                                dcm.TextAlignment = cc.TextAlignment;
                                                dcm.VerticalAlignment = cc.VerticalAlignment;
                                                dcm.WhereOperator = cc.WhereOperator;
                                                dcm.WhereValue = cc.WhereValue;
                                                dcm.MergeColumn = cc.MergeColumn;
                                                dcm.FontFamily = cc.FontFamily;
                                                dcm.FontSize = cc.FontSize;
                                                dcm.FontWeight = cc.FontWeight;
                                                dcm.FontStyle = cc.FontStyle;
                                                dcm.Sort = cc.Sort;
                                                dcm.Type = cc.Type;
                                                dcm.ImageStretch = cc.ImageStretch;
                                                dcm.IsVisible = cc.IsVisible;
                                                foreach (var sc in cc.SpecialCells)
                                                {
                                                    SpecialCellModel scm = new SpecialCellModel(sc, true);
                                                    dcm.SpecialCells.Add(scm);
                                                }
                                                dcm.SelectedSpecialCell = dcm.SpecialCells.FirstOrDefault();
                                                foreach (var tf in cc.TimeFilters)
                                                {
                                                    TimeFilterModel tfm = new TimeFilterModel(tf, true);
                                                    dcm.TimeFilters.Add(tfm);
                                                }
                                                dcm.SelectedTimeFilter = dcm.TimeFilters.FirstOrDefault();
                                            }
                                            ss.Columns.Add(dcm);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Memento.Enable = true;
                ShowProject();
                ActivTab = 1;

                //Delete old tab
                if (Project != null)
                    IOManagerProject.ClearOldFiles(Project, path);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CloseProject()
        {
            ShowNothing();
            Project = null;
            ActivTab = 0;
        }

        public void SelectObject(object item)
        {
            if (Project != null)
            {
                if (Project.SelectedObject != item)
                {
                    Project.SelectedObject = item;
                }
            }
        }
        #endregion

        #region ProjectInfo

        //private ObservableCollection<ProjectInfoModel> GetProject()
        //{
        //    //var result = Utilities.IOManagerScheduler.LoadScheduler();
        //    return new ObservableCollection<ProjectInfoModel>(SQLiteHelper.Instance.GetProject());
        //}

        #endregion

        #region Resolutions
        private ObservableCollection<ResolutionModel> GetResolutions()
        {
            //https://en.wikipedia.org/wiki/Display_resolution#/media/File:Vector_Video_Standards8.svg
            return new ObservableCollection<ResolutionModel>()
            {
                new ResolutionModel("FWXGA (1366x768)", 1366, 768, true),
                new ResolutionModel("Full HD (1920x1080)", 1920, 1080, true),
                new ResolutionModel("VGA (640x480)", 800, 600, true),
                new ResolutionModel("SVGA (800x600)", 800, 600, true),
                new ResolutionModel("WXGA (1024x768)", 1024, 768, true),
                new ResolutionModel("HD720 (1280x720)", 1280, 720, true),
                new ResolutionModel("SXGA (1280x1024)", 1280, 1024, true),
                new ResolutionModel("UXGA (1600x1200)", 1600, 1200, true),
                new ResolutionModel("Recommended (2880x1800)", 2880, 1800, true),
                new ResolutionModel("FWXGAX X 2W (2732x768)", 2732, 768, true),
                new ResolutionModel("FWXGAX X 2H (1366x1536)", 1366, 1536, true),
                new ResolutionModel("Full HD X 2W (3840x1080)", 3840, 1080, true),
                new ResolutionModel("Full HD X 2H (1920x2160)", 1920, 2160, true),
                new ResolutionModel("UHDTV (3840x2160)", 3840, 2160, true),
                new ResolutionModel("4K (4096x2304)", 4096, 2304, true),
                new ResolutionModel("UHDTV (7680x4320)", 7680,  4320, true),
                new ResolutionModel("8K (8192x4608)", 8192, 4608, true),
            };
        }

        public void AddResolution(ResolutionModel item)
        {
            _Resolutions.Add(item);
            _Project.SelectedResolution = item;
        }

        private ObservableCollection<MonitorModel> GetMonitors()
        {
            return new ObservableCollection<MonitorModel>()
            {
                new MonitorModel("Only 1", 1, 1, true),
                new MonitorModel("1 x 2", 1, 2, true),
                new MonitorModel("1 x 3", 1, 3, true),
                new MonitorModel("1 x 4", 1, 4, true),
                new MonitorModel("2 x 1", 2, 1, true),
                new MonitorModel("2 x 2", 2, 2, true),
                new MonitorModel("2 x 3", 2, 3, true),
                new MonitorModel("2 x 4", 2, 4, true),
                new MonitorModel("3 x 1", 3, 1, true),
                new MonitorModel("3 x 2", 3, 2, true),
                new MonitorModel("3 x 3", 3, 3, true),
                new MonitorModel("3 x 4", 3, 4, true),
                new MonitorModel("4 x 1", 4, 1, true),
                new MonitorModel("4 x 2", 4, 2, true),
                new MonitorModel("4 x 3", 4, 3, true),
                new MonitorModel("4 x 4", 4, 4, true)
            };
        }

        public void AddMonitor(MonitorModel item)
        {
            _Monitors.Add(item);
            _Project.SelectedMonitor = item;
        }

        private ObservableCollection<SocialMediaModel> GetSocialMedias()
        {
            return new ObservableCollection<SocialMediaModel>()
            {
                new SocialMediaModel("Facebook Like Icon", new Uri(@"../Images/iconFacebookLikesControl_Active.png", UriKind.Relative)),
                new SocialMediaModel("Facebook Icon", new Uri(@"../Images/iconFacebookControl_Active.png", UriKind.Relative)),
                new SocialMediaModel("Twitter Icon", new Uri(@"../Images/iconTwitterControl_Active.png", UriKind.Relative)),
                new SocialMediaModel("Instagram Icon", new Uri(@"../Images/iconInstagramControl_Active.png", UriKind.Relative)),
                new SocialMediaModel("Facebook Like Sender Image", new Uri(@"../Images/iconFacebookLikesControl_Active.png", UriKind.Relative)),
                new SocialMediaModel("Facebook Sender Image", new Uri(@"../Images/iconFacebookControl_Active.png", UriKind.Relative)),
                new SocialMediaModel("Twitter Sender Image", new Uri(@"../Images/iconTwitterControl_Active.png", UriKind.Relative)),
                new SocialMediaModel("Instagram Sender Image", new Uri(@"../Images/iconInstagramControl_Active.png", UriKind.Relative))
            };
        }

        #endregion

        #region Region
        public RegionModel CreateRegion(double x, double y, double width, double height)
        {
            if (Project != null && Project.Regions != null)
            {
                Memento.Push(Project.Regions.ToArray());

                RegionModel region = new RegionModel(_Project, x, y, width, height);
                Project.Regions.Add(region);
                Project.SelectedRegion = region;
                return region;
            }
            return null;
        }

        public RegionModel CopyRegion(RegionModel item)
        {
            if (Project != null)
            {
                RegionModel region = new RegionModel(item, Project);
                Project.Regions.Add(region);
                Project.SelectedRegion = region;
                return region;
            }
            return null;
        }

        public void DeleteRegion(RegionModel item)
        {
            if (Project != null && Project.Regions != null && item != null)
            {
                Memento.Push(Project.Regions.ToArray());

                Project.Regions.Remove(item);
                Project.SelectedRegion = Project.Regions.FirstOrDefault();
            }
        }

        public void DeleteRegion(RegionModel[] items)
        {
            if (Project != null && Project.Regions != null && items != null && items.Length > 0)
            {
                Memento.Push(Project.Regions.ToArray());

                foreach (var item in items)
                {
                    Project.Regions.Remove(item);
                }
                Project.SelectedRegion = Project.Regions.FirstOrDefault();
            }
        }

        public RegionModel SelectRegion(RegionModel item)
        {
            if (Project != null && Project.Regions != null)
            {
                Project.SelectedRegion = item;
                return item;
            }
            return null;
        }

        public RegionModel FindRegion(RegionModel item)
        {
            try
            {
                return Project.Regions.Where(_ => _.Id == item.Id).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AlignRegion(string text)
        {
            try
            {
                foreach (var region in Project.Regions.Where(_ => _.IsSelected == true))
                {
                    switch (text)
                    {
                        case "Align Left":
                            {
                                region.X = 0;
                            }
                            break;
                        case "Align Center":
                            {
                                region.X = (region.Parent.SelectedResolution.Width - region.Width) / 2;
                            }
                            break;
                        case "Align Right":
                            {
                                region.X = region.Parent.SelectedResolution.Width - region.Width;
                            }
                            break;
                        case "Align Top":
                            {
                                region.Y = 0;
                            }
                            break;
                        case "Align Middle":
                            {
                                region.Y = (region.Parent.SelectedResolution.Height - region.Height) / 2;
                            }
                            break;
                        case "Align Bottom":
                            {
                                region.Y = region.Parent.SelectedResolution.Height - region.Height;
                            }
                            break;
                    }
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region Slide
        public SlideModel CreateSlide()
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null)
            {
                Memento.Push(Project.SelectedRegion.Slides.ToArray());

                SlideModel slide = new SlideModel(Project.SelectedRegion);
                ObservableCollection<SlideModel> oldSlides = new ObservableCollection<SlideModel>();
                foreach (var item in Project.SelectedRegion.Slides)
                {
                    oldSlides.Add(item);
                }
                oldSlides.Add(slide);
                //Project.SelectedRegion.Slides.Add(slide);
                Project.SelectedRegion.Slides.Clear();
                Project.SelectedRegion.Slides = oldSlides;

                Project.SelectedRegion.SelectedSlide = slide;
                return slide;
            }
            return null;
        }

        public SlideModel CopySlide(SlideModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null)
            {
                SlideModel slide = new SlideModel(item, Project.SelectedRegion);
                Project.SelectedRegion.Slides.Add(slide);
                Project.SelectedRegion.SelectedSlide = slide;
                return slide;
            }
            return null;
        }

        public void DeleteSlide()
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null)
            {
                if (Project.SelectedRegion.Slides.Count > 1)
                {
                    Memento.Push(Project.SelectedRegion.Slides.ToArray());

                    Project.SelectedRegion.Slides.Remove(Project.SelectedRegion.SelectedSlide);
                    Project.SelectedRegion.SelectedSlide = Project.SelectedRegion.Slides.FirstOrDefault();
                }
                else
                {
                    MessageBox.Show("Region must have one slide", "Info");
                }
            }
        }

        public void DeleteSlide(SlideModel item)
        {
            try
            {
                if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null)
                {
                    if (Project.SelectedRegion.Slides.Count > 1)
                    {
                        Memento.Push(Project.SelectedRegion.Slides.ToArray());

                        Project.SelectedRegion.Slides.Remove(item);
                        Project.SelectedRegion.SelectedSlide = Project.SelectedRegion.Slides.FirstOrDefault();
                    }
                    else
                    {
                        MessageBox.Show("Region must have one slide", "Info");
                    }
                }
            }
            catch (Exception e)
            { }
        }

        public void SelectSlide(SlideModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null)
            {
                if (Project.SelectedRegion.SelectedSlide != item)
                {
                    Project.SelectedRegion.SelectedSlide = item;
                    if (item != null)
                    {
                        Project.SelectedRegion = item.Parent;
                    }
                }
            }
        }

        public SlideModel FindSlide(SlideModel item)
        {
            try
            {
                return Project.Regions.Where(_ => _.Id == item.Parent.Id).SingleOrDefault().Slides.Where(_ => _.Id == item.Id).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Layer
        public LayerModel CreateLayer()
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.Layers != null)
            {
                Memento.Push(Project.SelectedRegion.SelectedSlide.Layers.ToArray());

                LayerModel layer = new LayerModel(Project.SelectedRegion.SelectedSlide);
                Project.SelectedRegion.SelectedSlide.Layers.Add(layer);
                Project.SelectedRegion.SelectedSlide.SelectedLayer = layer;
                return layer;
            }
            return null;
        }

        public LayerModel CopyLayer(LayerModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null)
            {
                LayerModel layer = new LayerModel(item, Project.SelectedRegion.SelectedSlide);
                Project.SelectedRegion.SelectedSlide.Layers.Add(layer);
                Project.SelectedRegion.SelectedSlide.SelectedLayer = layer;
                return layer;
            }
            return null;
        }

        public void DeleteLayer()
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.Layers != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null)
            {
                if (Project.SelectedRegion.SelectedSlide.Layers.Count > 1)
                {
                    Memento.Push(Project.SelectedRegion.SelectedSlide.Layers.ToArray());

                    Project.SelectedRegion.SelectedSlide.Layers.Remove(Project.SelectedRegion.SelectedSlide.SelectedLayer);
                    Project.SelectedRegion.SelectedSlide.SelectedLayer = Project.SelectedRegion.SelectedSlide.Layers.FirstOrDefault();
                }
                else
                {
                    MessageBox.Show("Slide must have one layer", "Info");
                }
            }
        }

        public void SelectLayer(LayerModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.Layers != null)
            {
                if (Project.SelectedRegion.SelectedSlide.SelectedLayer != item)
                {
                    Project.SelectedRegion.SelectedSlide.SelectedLayer = item;
                    if (item != null)
                        Project.SelectedRegion.SelectedSlide = item.Parent;
                }
            }
        }

        public LayerModel FindLayer(LayerModel item)
        {
            try
            {
                return Project.Regions.Where(_ => _.Id == item.Parent.Parent.Id).SingleOrDefault().Slides.Where(_ => _.Id == item.Parent.Id).SingleOrDefault().Layers.Where(_ => _.Id == item.Id).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal void ReorderLayers()
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.Layers != null)
            {
                int zIndex = 1;
                foreach (var item in Project.SelectedRegion.SelectedSlide.Layers.Reverse())
                {
                    foreach (var con in item.Controls.OrderBy(_ => _.ZIndex))
                    {
                        con.ZIndex = ++zIndex;
                    }
                    zIndex += 5;
                    // item.ZIndex = zIndex;
                }
            }
        }
        #endregion

        #region Control
        public ControlModel CreateControl(ControlType type, double x, double y, double width, double height)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls != null)
            {
                Memento.Push(Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.ToArray());

                ControlModel control = new ControlModel(type, x, y, width, height, Project.SelectedRegion.SelectedSlide.SelectedLayer);

                if (type == ControlType.DataGrid && control.Playlist[0] is SetContentModel)
                {
                    (control.Playlist[0] as SetContentModel).DataGrid = CeitconServerHelper.GetDataSources();
                }
                Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.Add(control);
                Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl = control;
                return control;
            }
            return null;
        }

        public ControlModel CreateAlertControl(ControlType type, double x, double y, double width, double height)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null)
            {
                // Memento.Push(Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.ToArray());

                ControlModel control = new ControlModel(type, x, y, width, height, Project.SelectedAlert);
                Project.SelectedAlert.Controls.Add(control);
                Project.SelectedAlert.SelectedControl = control;
                return control;
            }
            return null;
        }

        public ControlModel CopyControl(ControlModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls != null)
            {
                ControlModel control = new ControlModel(item, Project.SelectedRegion.SelectedSlide.SelectedLayer);
                Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.Add(control);
                Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl = control;
                return control;
            }
            return null;
        }

        public ControlModel CopyAlertControl(ControlModel item)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null)
            {
                ControlModel control = new ControlModel(item, Project);
                Project.SelectedAlert.Controls.Add(control);
                Project.SelectedAlert.SelectedControl = control;
                return control;
            }
            return null;
        }

        public void DeleteControl(ControlModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls != null && item != null)
            {
                Memento.Push(Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.ToArray());

                Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.Remove(item);
                Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl = Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.FirstOrDefault();
            }
        }

        public void DeleteAlertControl(ControlModel item)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null && item != null)
            {
                // Memento.Push(Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.ToArray());

                Project.SelectedAlert.Controls.Remove(item);
                Project.SelectedAlert.SelectedControl = Project.SelectedAlert.Controls.FirstOrDefault();
            }
        }

        public void DeleteControl(ControlModel[] items)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls != null && items != null && items.Length > 0)
            {
                Memento.Push(Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.ToArray());

                foreach (var item in items)
                {
                    Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.Remove(item);
                }
                Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl = Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.FirstOrDefault();
            }
        }

        public void DeleteAlertControl(ControlModel[] items)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null && items != null && items.Length > 0)
            {
                // Memento.Push(Project.SelectedAlert.Controls.ToArray());

                foreach (var item in items)
                {
                    Project.SelectedAlert.Controls.Remove(item);
                }
                Project.SelectedAlert.SelectedControl = Project.SelectedAlert.Controls.FirstOrDefault();
            }
        }

        public void SelectControl(ControlModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls != null)
            {
                if (Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl != item)
                {
                    Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl = item;
                    if (item != null)
                        Project.SelectedRegion.SelectedSlide.SelectedLayer = (item.Parent as LayerModel);
                }
            }
        }

        public void SelectAlertControl(ControlModel item)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null)
            {
                if (Project.SelectedAlert.SelectedControl != item)
                {
                    Project.SelectedAlert.SelectedControl = item;
                    if (item != null)
                        Project.SelectedAlert = (item.Parent as AlertModel);
                }
            }
        }

        public ControlModel FindControl(ControlModel item)
        {
            try
            {
                return Project.Regions.Where(_ => _.Id == (item.Parent as LayerModel).Parent.Parent.Id).SingleOrDefault().Slides.Where(_ => _.Id == (item.Parent as LayerModel).Parent.Id).SingleOrDefault().Layers.Where(_ => _.Id == (item.Parent as LayerModel).Id).SingleOrDefault().Controls.Where(_ => _.Id == item.Id).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ControlModel FindAlertControl(ControlModel item)
        {
            try
            {
                return Project.Alerts.Where(_ => _.Id == (item.Parent as AlertModel).Id).SingleOrDefault().Controls.Where(_ => _.Id == item.Id).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AlignControl(string text, ControlModel item = null)
        {
            try
            {
                foreach (var control in Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.Where(_ => _.IsSelected == true))
                {
                    switch (text)
                    {
                        case "AlignLeft":
                            {
                                control.X = 0;
                                if (control.Width == (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width)
                                    control.Width = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width / 2;
                            }
                            break;
                        case "AlignCenter":
                            {
                                control.X = ((control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width - control.Width) / 2;
                                if (control.Width == (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width)
                                {
                                    control.Width = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width / 2;
                                    control.X = control.Width / 2;
                                }
                            }
                            break;
                        case "AlignRight":
                            {
                                control.X = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width - control.Width;
                                if (control.Width == (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width)
                                {
                                    control.Width = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width / 2;
                                    control.X = control.Width;
                                }
                            }
                            break;
                        case "AlignTop":
                            {
                                control.Y = 0;
                                if (control.Height == (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height)
                                    control.Height = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height / 2;
                            }
                            break;
                        case "AlignMiddle":
                            {
                                control.Y = ((control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height - control.Height) / 2;
                                if (control.Height == (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height)
                                {
                                    control.Height = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height / 2;
                                    control.Y = control.Height / 2;
                                }
                            }
                            break;
                        case "AlignBottom":
                            {
                                control.Y = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height - control.Height;
                                if (control.Height == (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height)
                                {
                                    control.Height = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height / 2;
                                    control.Y = control.Height;
                                }
                            }
                            break;
                        case "AlignFull":
                            {
                                control.X = 0;
                                control.Y = 0;
                                control.Width = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Width;
                                control.Height = (control.Parent as LayerModel).Parent.Parent.Parent.SelectedResolution.Height;
                            }
                            break;
                    }
                }
            }
            catch (Exception) { }
        }

        public void AlignAlertControl(string text, ControlModel item = null)
        {
            try
            {
                foreach (var control in Project.SelectedAlert.Controls.Where(_ => _.IsSelected == true))
                {
                    switch (text)
                    {
                        case "AlignLeft":
                            {
                                control.X = 0;
                            }
                            break;
                        case "AlignCenter":
                            {
                                control.X = ((control.Parent as AlertModel).Parent.SelectedResolution.Width - control.Width) / 2;
                            }
                            break;
                        case "AlignRight":
                            {
                                control.X = (control.Parent as AlertModel).Parent.SelectedResolution.Width - control.Width;
                            }
                            break;
                        case "AlignTop":
                            {
                                control.Y = 0;
                            }
                            break;
                        case "AlignMiddle":
                            {
                                control.Y = ((control.Parent as AlertModel).Parent.SelectedResolution.Height - control.Height) / 2;
                            }
                            break;
                        case "AlignBottom":
                            {
                                control.Y = (control.Parent as AlertModel).Parent.SelectedResolution.Height - control.Height;
                            }
                            break;
                    }
                }
            }
            catch (Exception) { }
        }
        #endregion

        #region Playlist
        public PlaylistModel CreatePlaylist(PlaylistType plt)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null
                && Project.SelectedRegion.SelectedSlide.SelectedLayer != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls != null
                 && Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist != null)
            {
                Memento.Push(Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.ToArray());

                PlaylistModel item = null;
                var selectedControl = Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl;
                switch (plt)
                {
                    case PlaylistType.SetContent:
                        item = new SetContentModel(selectedControl);
                        break;
                    case PlaylistType.Delay:
                        item = new DelayModel(selectedControl);
                        break;
                    case PlaylistType.AnimateMargin:
                        item = new AnimateMarginModel(selectedControl);
                        break;
                    case PlaylistType.AnimateOpacity:
                        item = new AnimateOpacityModel(selectedControl);
                        break;
                    case PlaylistType.AnimateWidth:
                        item = new AnimateWidthModel(selectedControl);
                        break;
                    case PlaylistType.AnimateHeight:
                        item = new AnimateHeightModel(selectedControl);
                        break;
                    case PlaylistType.AnimateBorder:
                        item = new AnimateBorderModel(selectedControl);
                        break;
                    case PlaylistType.SuspendPlayback:
                        item = new SuspendPlaybackModel(selectedControl);
                        break;
                    case PlaylistType.ResumePlayback:
                        item = new ResumePlaybackModel(selectedControl);
                        break;
                    default:
                        return null;
                }
                selectedControl.Playlist.Add(item);
                selectedControl.SelectedPlaylist = item;
                return item;
            }
            return null;
        }

        public PlaylistModel CreatePlaylistAlert(PlaylistType plt)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null &&
                Project.SelectedAlert.SelectedControl != null && Project.SelectedAlert.SelectedControl.Playlist != null)
            {
                //Memento.Push(Project.SelectedAlert.SelectedControl.Playlist.ToArray());

                PlaylistModel item = null;
                var selectedControl = Project.SelectedAlert.SelectedControl;
                switch (plt)
                {
                    case PlaylistType.SetContent:
                        item = new SetContentModel(selectedControl);
                        break;
                    case PlaylistType.Delay:
                        item = new DelayModel(selectedControl);
                        break;
                    case PlaylistType.AnimateMargin:
                        item = new AnimateMarginModel(selectedControl);
                        break;
                    case PlaylistType.AnimateOpacity:
                        item = new AnimateOpacityModel(selectedControl);
                        break;
                    case PlaylistType.AnimateWidth:
                        item = new AnimateWidthModel(selectedControl);
                        break;
                    case PlaylistType.AnimateHeight:
                        item = new AnimateHeightModel(selectedControl);
                        break;
                    case PlaylistType.AnimateBorder:
                        item = new AnimateBorderModel(selectedControl);
                        break;
                    case PlaylistType.SuspendPlayback:
                        item = new SuspendPlaybackModel(selectedControl);
                        break;
                    case PlaylistType.ResumePlayback:
                        item = new ResumePlaybackModel(selectedControl);
                        break;
                    default:
                        return null;
                }
                selectedControl.Playlist.Add(item);
                selectedControl.SelectedPlaylist = item;
                return item;
            }
            return null;
        }

        public PlaylistModel CopyPlaylist(PlaylistModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null
            && Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist != null)
            {

                PlaylistModel pl = null;
                var selectedControl = Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl;
                switch (item.Type)
                {
                    case PlaylistType.SetContent:
                        pl = new SetContentModel(item as SetContentModel, selectedControl);
                        break;
                    case PlaylistType.Delay:
                        pl = new DelayModel(item as DelayModel, selectedControl);
                        break;
                    case PlaylistType.AnimateMargin:
                        pl = new AnimateMarginModel(item as AnimateMarginModel, selectedControl);
                        break;
                    case PlaylistType.AnimateOpacity:
                        pl = new AnimateOpacityModel(item as AnimateOpacityModel, selectedControl);
                        break;
                    case PlaylistType.AnimateWidth:
                        pl = new AnimateWidthModel(item as AnimateWidthModel, selectedControl);
                        break;
                    case PlaylistType.AnimateHeight:
                        pl = new AnimateHeightModel(item as AnimateHeightModel, selectedControl);
                        break;
                    case PlaylistType.AnimateBorder:
                        pl = new AnimateBorderModel(item as AnimateBorderModel, selectedControl);
                        break;
                    case PlaylistType.SuspendPlayback:
                        pl = new SuspendPlaybackModel(item as SuspendPlaybackModel, selectedControl);
                        break;
                    case PlaylistType.ResumePlayback:
                        pl = new ResumePlaybackModel(item as ResumePlaybackModel, selectedControl);
                        break;
                    default:
                        return null;
                }
                selectedControl.Playlist.Add(pl);
                selectedControl.SelectedPlaylist = pl;
                return pl;
            }
            return null;
        }

        public PlaylistModel CopyPlaylistAlert(PlaylistModel item)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null &&
                Project.SelectedAlert.SelectedControl != null && Project.SelectedAlert.SelectedControl.Playlist != null)
            {

                PlaylistModel pl = null;
                var selectedControl = Project.SelectedAlert.SelectedControl;
                switch (item.Type)
                {
                    case PlaylistType.SetContent:
                        pl = new SetContentModel(item as SetContentModel, selectedControl);
                        break;
                    case PlaylistType.Delay:
                        pl = new DelayModel(item as DelayModel, selectedControl);
                        break;
                    case PlaylistType.AnimateMargin:
                        pl = new AnimateMarginModel(item as AnimateMarginModel, selectedControl);
                        break;
                    case PlaylistType.AnimateOpacity:
                        pl = new AnimateOpacityModel(item as AnimateOpacityModel, selectedControl);
                        break;
                    case PlaylistType.AnimateWidth:
                        pl = new AnimateWidthModel(item as AnimateWidthModel, selectedControl);
                        break;
                    case PlaylistType.AnimateHeight:
                        pl = new AnimateHeightModel(item as AnimateHeightModel, selectedControl);
                        break;
                    case PlaylistType.AnimateBorder:
                        pl = new AnimateBorderModel(item as AnimateBorderModel, selectedControl);
                        break;
                    case PlaylistType.SuspendPlayback:
                        pl = new SuspendPlaybackModel(item as SuspendPlaybackModel, selectedControl);
                        break;
                    case PlaylistType.ResumePlayback:
                        pl = new ResumePlaybackModel(item as ResumePlaybackModel, selectedControl);
                        break;
                    default:
                        return null;
                }
                selectedControl.Playlist.Add(pl);
                selectedControl.SelectedPlaylist = pl;
                return pl;
            }
            return null;
        }

        public void DeletePlaylist(PlaylistModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null
                && Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist != null)
            {
                Memento.Push(Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.ToArray());

                Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.Remove(item);
                Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.SelectedPlaylist = Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.FirstOrDefault();
            }
        }

        public void DeletePlaylistAlert(PlaylistModel item)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null &&
                Project.SelectedAlert.SelectedControl != null && Project.SelectedAlert.SelectedControl.Playlist != null)
            {
                //Memento.Push(Project.SelectedAlert.SelectedControl.Playlist.ToArray());

                Project.SelectedAlert.SelectedControl.Playlist.Remove(item);
                Project.SelectedAlert.SelectedControl.SelectedPlaylist = Project.SelectedAlert.SelectedControl.Playlist.FirstOrDefault();
            }
        }

        public PlaylistModel SelectPlaylist(string id)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls != null)
            {
                var item = Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.Where(_ => _.Id == id).FirstOrDefault();
                if (Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.SelectedPlaylist != item)
                {
                    Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.SelectedPlaylist = item;
                    return item;
                }
            }
            return null;
        }

        public PlaylistModel SelectPlaylistAlert(string id)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Controls != null && Project.SelectedAlert.Controls != null)
            {
                var item = Project.SelectedAlert.SelectedControl.Playlist.Where(_ => _.Id == id).FirstOrDefault();
                if (Project.SelectedAlert.SelectedControl.SelectedPlaylist != item)
                {
                    Project.SelectedAlert.SelectedControl.SelectedPlaylist = item;
                    return item;
                }
            }
            return null;
        }

        public void SelectPlaylist(PlaylistModel item)
        {
            if (Project != null && Project.SelectedRegion != null && Project.SelectedRegion.Slides != null && Project.SelectedRegion.SelectedSlide != null && Project.SelectedRegion.SelectedSlide.SelectedLayer != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl != null && Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist != null)
            {
                if (Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.SelectedPlaylist != item)
                {
                    Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.SelectedPlaylist = item;
                    if (item != null)
                        Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl = item.Parent;
                }
            }
        }

        public void SelectPlaylistAlert(PlaylistModel item)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.SelectedControl != null && Project.SelectedAlert.SelectedControl.Playlist != null)
            {
                if (Project.SelectedAlert.SelectedControl.SelectedPlaylist != item)
                {
                    Project.SelectedAlert.SelectedControl.SelectedPlaylist = item;
                    if (item != null)
                        Project.SelectedAlert.SelectedControl = item.Parent;
                }
            }
        }

        public PlaylistModel FindPlaylist(PlaylistModel item)
        {
            try
            {
                return Project.Regions.Where(_ => _.Id == (item.Parent.Parent as LayerModel).Parent.Parent.Id).SingleOrDefault().Slides.Where(_ => _.Id == (item.Parent.Parent as LayerModel).Parent.Id).SingleOrDefault().Layers.Where(_ => _.Id == (item.Parent.Parent as LayerModel).Id).SingleOrDefault().Controls.Where(_ => _.Id == item.Parent.Id).SingleOrDefault().Playlist.Where(_ => _.Id == item.Id).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public PlaylistModel FindPlaylistAlert(PlaylistModel item)
        {
            try
            {
                return Project.Alerts.Where(_ => _.Id == (item.Parent.Parent as AlertModel).Id).SingleOrDefault().Controls.Where(_ => _.Id == item.Parent.Id).SingleOrDefault().Playlist.Where(_ => _.Id == item.Id).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddDepends(PlaylistModel item, PlaylistModel dep)
        {
            if (item != null && dep != null && item != dep)
            {
                item.Depends = dep;
            }
        }

        public void RemoveDepends(PlaylistModel item)
        {
            if (item != null)
            {
                item.Depends = null;
            }
        }

        public void RefreshFilteredAppointments(string name, bool? startChecked, DateTime? startTime, bool? endChecked, DateTime? endTime)
        {
            try
            {
                var list = Appointments.Where(_ =>
                    ((String.IsNullOrWhiteSpace(name) || _.Subject.ToLower().Contains(name.ToLower())) &&
                    (startChecked == false || _.Start >= startTime) &&
                    (endChecked == false || _.End <= endTime))).ToList();

                FilteredAppointments = new ObservableCollection<Appointment>(list);
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Network

        private ObservableCollection<NetworkModel> GetNetwork()
        {
            var nm = new NetworkModel();
            //var dm = new DomainModel(nm);
            //var cm = new CountryModel(dm);
            //var rm = new NetworkRegionModel(cm);
            //var lgm = new LocationGroupModel(rm);
            //var lm = new LocationModel(lgm);
            //var fm = new FloorModel(lm);
            //var zm = new ZoneModel(fm);
            //var pgm = new PlayerGroupModel(zm);
            //var pm = new PlayerModel(pgm);
            //var fgm = new FaceGroupModel(pm);
            //var fm1 = new FaceModel(fgm);

            //fgm.Faces.Add(fm1);
            //pm.FaceGroups.Add(fgm);
            //pgm.Players.Add(pm);
            //zm.PlayerGroups.Add(pgm);
            //fm.Zones.Add(zm);
            //lm.Floors.Add(fm);
            //lgm.Locations.Add(lm);
            //rm.LocationGroups.Add(lgm);
            //cm.Regions.Add(rm);
            //dm.Countries.Add(cm);
            //nm.Domains.Add(dm);

            ObservableCollection<NetworkModel> dmList = new ObservableCollection<NetworkModel> { nm };
            return dmList;

        }

        public PlayerGroupModel GetPlayerGroup(string playerName)
        {
            PlayerGroupModel result = null;
            foreach (var nm in _Network)
            {
                foreach (var d in nm.Domains)
                {
                    foreach (var c in d.Countries)
                    {
                        foreach (var r in c.Regions)
                        {
                            foreach (var lg in r.LocationGroups)
                            {
                                foreach (var l in lg.Locations)
                                {
                                    foreach (var f in l.Floors)
                                    {
                                        foreach (var z in f.Zones)
                                        {
                                            foreach (var pg in z.PlayerGroups)
                                            {
                                                foreach (var p in pg.Players)
                                                {
                                                    if (p.Name == playerName)
                                                    {
                                                        return pg;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public PlayerModel[] GetPlayerList(string groupName)
        {
            PlayerModel[] result = new PlayerModel[0];
            foreach (var nm in _Network)
            {
                foreach (var d in nm.Domains)
                {
                    foreach (var c in d.Countries)
                    {
                        foreach (var r in c.Regions)
                        {
                            foreach (var lg in r.LocationGroups)
                            {
                                foreach (var l in lg.Locations)
                                {
                                    foreach (var f in l.Floors)
                                    {
                                        foreach (var z in f.Zones)
                                        {
                                            foreach (var pg in z.PlayerGroups)
                                            {
                                                if (pg.Identification == groupName)
                                                {
                                                    return pg.Players.ToArray();

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public object CreateNetworkItem(object parent, object item)
        {
            //if (!String.IsNullOrWhiteSpace(displayId))
            //{
            //    Memento.Push(Appointments.ToArray());

            //    var item = new Appointment() { Subject = displayId, Body = project, Start = start, End = end };
            //    Appointments.Add(item);
            //    return item;
            //}
            return null;
        }

        public void DeleteNetworkItem(object item)
        {
            if (item != null)
            {

            }
        }

        public void RefreshNetwork(string name, string host, string ip, bool? isChecked)
        {
            try
            {
                Network = CeitconServerHelper.GetNetwork();
                GetPlayers(Network.FirstOrDefault(), name, host, ip, isChecked);
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Alert
        public AlertModel CreateAlert(AlertType type)
        {
            if (Project != null)
            {
                var alert = new AlertModel(Project);
                alert.Type = type;
                if (type == AlertType.Prayer)
                {
                    var slot = new SlotModel(alert);
                    slot.Location = Locations.FirstOrDefault();
                    alert.Slots.Add(slot);
                    alert.SelectedSlot = slot;
                }
                Project.Alerts.Add(alert);

                if (type == AlertType.Prayer)
                    Project.PrayerAlerts.Add(alert);

                if (type == AlertType.Global)
                    Project.GlobalAlerts.Add(alert);

                Project.SelectedAlert = alert;
                return alert;
            }
            return null;
        }

        public AlertModel CopyAlert(AlertModel item)
        {
            if (Project != null)
            {
                AlertModel alert = new AlertModel(item, Project);
                Project.Alerts.Add(alert);
                if (item.Type == AlertType.Global)
                {
                    Project.GlobalAlerts.Add(alert);
                }
                else if (item.Type == AlertType.Prayer)
                {
                    Project.PrayerAlerts.Add(alert);
                }
                Project.SelectedAlert = alert;
                return alert;
            }
            return null;
        }

        public void DeleteAlert()
        {
            if (Project != null && Project.SelectedAlert != null)
            {
                if (Project.Alerts.Count > 0)
                {
                    Project.Alerts.Remove(Project.SelectedAlert);
                    Project.GlobalAlerts.Remove(Project.SelectedAlert);
                    Project.PrayerAlerts.Remove(Project.SelectedAlert);
                    Project.SelectedAlert = Project.Alerts.FirstOrDefault();
                }
                else
                {
                    //MessageBox.Show("Project must have one alert", "Info");
                }
            }
        }

        public void DeleteAlert(AlertModel item)
        {
            if (Project != null && Project.SelectedAlert != null)
            {
                if (Project.Alerts.Count > 0)
                {
                    Project.Alerts.Remove(item);
                    Project.GlobalAlerts.Remove(Project.SelectedAlert);
                    Project.PrayerAlerts.Remove(Project.SelectedAlert);
                    Project.SelectedAlert = Project.Alerts.FirstOrDefault();
                }
                else
                {
                    //MessageBox.Show("Project must have one alert", "Info");
                }
            }
        }

        public void SelectAlert(AlertModel item)
        {
            if (Project != null && Project.Alerts != null)
            {
                if (Project.SelectedAlert != item)
                {
                    Project.SelectedAlert = item;
                }
            }
        }

        public AlertModel FindAlert(AlertModel item)
        {
            try
            {
                return Project.Alerts.Where(_ => _.Id == item.Id).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Slot
        public SlotModel CreateSlot()
        {
            if (Project != null && Project.SelectedAlert != null)
            {
                SlotModel item = new SlotModel(Project.SelectedAlert);
                item.Location = Locations.FirstOrDefault();
                Project.SelectedAlert.Slots.Add(item);
                Project.SelectedAlert.SelectedSlot = item;
                return item;
            }
            return null;
        }

        public SlotModel CopySlot(SlotModel item)
        {
            if (Project != null && item != null)
            {
                SlotModel slot = new SlotModel(item, Project.SelectedAlert);
                Project.SelectedAlert.Slots.Add(slot);
                Project.SelectedAlert.SelectedSlot = slot;
                return slot;
            }
            return null;
        }

        public void DeleteSlot()
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Slots != null && Project.SelectedAlert.SelectedSlot != null)
            {
                if (Project.SelectedAlert.Slots.Count > 0)
                {
                    Project.SelectedAlert.Slots.Remove(Project.SelectedAlert.SelectedSlot);
                    Project.SelectedAlert.SelectedSlot = Project.SelectedAlert.Slots.FirstOrDefault();
                }
                else
                {
                    //MessageBox.Show("Project must have one alert", "Info");
                }
            }
        }

        public void DeleteSlot(SlotModel item)
        {
            if (Project != null && Project.SelectedAlert != null && Project.SelectedAlert.Slots != null && Project.SelectedAlert.SelectedSlot != null)
            {
                if (Project.SelectedAlert.Slots.Count > 0)
                {
                    Project.SelectedAlert.Slots.Remove(item);
                    Project.SelectedAlert.SelectedSlot = Project.SelectedAlert.Slots.FirstOrDefault();
                }
                else
                {
                    //MessageBox.Show("Project must have one alert", "Info");
                }
            }
        }

        public void SelectAlert(SlotModel item)
        {
            if (Project != null && Project.Alerts != null && Project.SelectedAlert != null && Project.SelectedAlert.Slots != null && Project.SelectedAlert.SelectedSlot != null)
            {
                if (Project.SelectedAlert.SelectedSlot != item)
                {
                    Project.SelectedAlert.SelectedSlot = item;
                }
            }
        }

        public SlotModel FindSlot(SlotModel item)
        {
            try
            {
                return Project.Alerts.Where(_ => _.Id == item.Parent.Id).FirstOrDefault().Slots.Where(_ => _.Id == item.Id).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void LoadLogos(DataSourceModel ds)
        {
            try
            {
                string xmlData = CeitconServerHelper.GetDataRecords(ds.Id);
                if (!String.IsNullOrEmpty(xmlData))
                {
                    XDocument xmldoc;
                    if (xmlData.StartsWith("<?xml") == false)
                    {
                        xmldoc = XDocument.Parse("<ROOT>" + xmlData + "</ROOT>");
                    }
                    else
                    {
                        xmldoc = XDocument.Parse(xmlData);
                    }

                    string directory = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Logos");

                    var collection = new List<LogoModel>();
                    foreach (var item in (from e in xmldoc.Root.Elements("FLIGHT").Elements("AIRLINE") select e).GroupBy(x => x.Value).Select(x => x.First().Parent))
                    {
                        LogoModel lm = new LogoModel();
                        lm.Id = item.Element("AIRLINE").Value;
                        lm.Description = item.Element("AIRLINE_DESCR").Value;
                        lm.FileLocation = Path.Combine(directory, String.Format(lm.Id + ".png"));
                        if (File.Exists(lm.FileLocation))
                            lm.FileSize = (new FileInfo(lm.FileLocation)).Length;
                        collection.Add(lm);
                    }
                    Logos = new ObservableCollection<LogoModel>(collection.OrderBy(_ => _.Description));
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion

        #region Schedular
        public bool DeleteAppointment(Appointment appointment)
        {
            try
            {
                bool response = CeitconServerHelper.DeleteScheduler(appointment);

                if (response)
                {
                    Appointments.Remove(appointment);
                    FilteredAppointments.Remove(appointment);
                }
                else
                {
                    MessageBox.Show("Appointment can't be deleted from server.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }
        #endregion
    }
}
