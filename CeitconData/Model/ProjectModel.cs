using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Ceitcon_Data.Model
{
    //[System.Xml.Serialization.XmlInclude(typeof(RegionModel))]
    public class ProjectModel : INotifyPropertyChanged
    {
        public ProjectModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Information = new InformationModel(this);
            _Regions = new ObservableCollection<RegionModel>();
            _Alerts = new ObservableCollection<AlertModel>();
            _PrayerAlerts = new ObservableCollection<AlertModel>();
            _GlobalAlerts = new ObservableCollection<AlertModel>();
        }

        private string _Id;
        private InformationModel _Information;
        private ResolutionModel _SelectedResolution;
        private ScreenOrientation _SelectedOrientation;
        private MonitorModel _SelectedMonitor;
        private ObservableCollection<RegionModel> _Regions;
        private RegionModel _SelectedRegion;
        private ObservableCollection<AlertModel> _Alerts;
        private ObservableCollection<AlertModel> _PrayerAlerts;
        private ObservableCollection<AlertModel> _GlobalAlerts;
        private AlertModel _SelectedAlert;
        private object _SelectedObject;
        private bool _IsSelected;

        public string Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public ResolutionModel SelectedResolution
        {
            get { return _SelectedResolution; }
            set
            {
                if (_SelectedResolution != value)
                {
                    //Percentage scaling subelements
                    if (_Regions != null && _Regions.Count > 0 && value != null)
                    {
                        var rm = value as ResolutionModel;
                        foreach (var regItem in _Regions)
                        {
                            double wFactor = (double)rm.Width / _SelectedResolution.Width;
                            double hFactor = (double)rm.Height / _SelectedResolution.Height;
                            regItem.Width = regItem.Width * wFactor;
                            regItem.Height = regItem.Height * hFactor;
                            regItem.X = regItem.X * wFactor;
                            regItem.Y = regItem.Y * hFactor;
                            foreach (var slideItem in regItem.Slides)
                            {
                                foreach (var layerItem in slideItem.Layers)
                                {
                                    foreach (var conItem in layerItem.Controls)
                                    {
                                        conItem.Width = conItem.Width * wFactor;
                                        conItem.Height = conItem.Height * hFactor;
                                        conItem.X = conItem.X * wFactor;
                                        conItem.Y = conItem.Y * hFactor;
                                    }
                                }
                            }
                        }
                    }
                    _SelectedResolution = value;
                    OnPropertyChanged("SelectedResolution");
                }
            }
        }


        public MonitorModel SelectedMonitor
        {
            get { return _SelectedMonitor; }
            set
            {
                if (_SelectedMonitor != value)
                {
                    _SelectedMonitor = value;
                    OnPropertyChanged("SelectedMonitor");
                }
            }
        }

        public ScreenOrientation SelectedOrientation
        {
            get { return _SelectedOrientation; }
            set
            {
                if (_SelectedOrientation != value)
                {
                    _SelectedOrientation = value;
                    OnPropertyChanged("SelectedOrientation");
                }
            }
        }

        public ObservableCollection<RegionModel> Regions
        {
            get { return _Regions; }
            set
            {
                if (_Regions != value)
                {
                    _Regions = value;
                    OnPropertyChanged("Regions");
                }
            }
        }

        public RegionModel SelectedRegion
        {
            get { return _SelectedRegion; }
            set
            {
                SelectedObject = value;
                if (_SelectedRegion != value)
                {
                    _SelectedRegion = value;
                    OnPropertyChanged("SelectedRegion");
                    OnPropertyChanged("HasSelectedRegion");
                }
            }
        }

        public bool HasSelectedRegion
        {
            get { return _SelectedRegion != null; }
        }

        public ObservableCollection<AlertModel> Alerts
        {
            get { return _Alerts; }
            set
            {
                if (_Alerts != value)
                {
                    _Alerts = value;
                    OnPropertyChanged("Alerts");
                    OnPropertyChanged("PrayerAlerts");
                    OnPropertyChanged("GlobalAlerts");
                }
            }
        }

        public ObservableCollection<AlertModel> PrayerAlerts
        {
            get { return _PrayerAlerts; }
            set
            {
                if (_PrayerAlerts != value)
                {
                    _PrayerAlerts = value;
                    OnPropertyChanged("PrayerAlerts");
                }
            }
        }

        public ObservableCollection<AlertModel> GlobalAlerts
        {
            get { return _GlobalAlerts; }
            set
            {
                if (_GlobalAlerts != value)
                {
                    _GlobalAlerts = value;
                    OnPropertyChanged("GlobalAlerts");
                }
            }
        }
        //public ObservableCollection<AlertModel> PrayerAlerts
        //{
        //    get
        //    {
        //        var linqResults = _Alerts.Where(_ => _.Type == AlertType.Prayer);
        //        return new ObservableCollection<AlertModel>(linqResults);
        //    }
        //}

        //public ObservableCollection<AlertModel> GlobalAlerts
        //{
        //    get
        //    {
        //        var linqResults = _Alerts.Where(_ => _.Type == AlertType.Global);
        //        return new ObservableCollection<AlertModel>(linqResults);
        //    }
        //}




        public AlertModel SelectedAlert
        {
            get { return _SelectedAlert; }
            set
            {
                SelectedObject = value;
                if (_SelectedAlert != value)
                {
                    _SelectedAlert = value;
                    OnPropertyChanged("SelectedAlert");
                    OnPropertyChanged("HasSelectedAlert");
                }
            }
        }

        public bool HasSelectedAlert
        {
            get { return _SelectedAlert != null; }
        }

        public bool HasSelectedAlertControl
        {
            get { return SelectedAlert != null && SelectedAlert.SelectedControl != null; }
        }


        public InformationModel Information
        {
            get { return _Information; }
            set
            {
                if (_Information != value)
                {
                    _Information = value;
                    OnPropertyChanged("Information");
                }
            }
        }

        public object SelectedObject
        {
            get { return _SelectedObject; }
            set
            {
                if (_SelectedObject != value)
                {
                    _SelectedObject = value;
                    OnPropertyChanged("SelectedObject");
                }
            }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
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
