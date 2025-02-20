using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ceitcon_Data.Model.Network
{
    // Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
    public class NetworkModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Name;
        private ObservableCollection<DomainModel> _Domains;

        public NetworkModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Name = "Network";
            _Domains = new ObservableCollection<DomainModel>();
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

        public ObservableCollection<DomainModel> Domains
        {
            get { return _Domains; }
            set
            {
                if (_Domains != value)
                {
                    _Domains = value;
                    OnPropertyChanged("Domains");
                }
            }
        }

        public string Identification
        {
            get
            {
                return Name;
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
