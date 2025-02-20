using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ceitcon_Data.Model.Network
{
    public class LocationGroupModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Name;
        private string _Description;
        private bool _Active;
        private NetworkRegionModel _Parent;
        private ObservableCollection<LocationModel> _Locations;

        public LocationGroupModel(NetworkRegionModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Name = String.Format("LocationGroup_{0}", _Id.Substring(0, 6));
            _Parent = parent;
            _Locations = new ObservableCollection<LocationModel>();
            _Active = true;
        }

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

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("Identification");
                }
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public bool Active
        {
            get { return _Active; }
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    OnPropertyChanged("Active");
                }
            }
        }

        public NetworkRegionModel Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent != value)
                {
                    _Parent = value;
                    OnPropertyChanged("Parent");
                }
            }
        }

        public ObservableCollection<LocationModel> Locations
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

        public string Identification
        {
            get
            {
                return String.Format("{0}/{1}", Parent.Identification, Name);
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
