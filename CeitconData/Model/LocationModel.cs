using System.ComponentModel;

namespace Ceitcon_Data.Model
{
    public class LocationModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Country;
        private string _City;
        private double _Latitude;
        private double _Longnitude;

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

        public string Country
        {
            get { return _Country; }
            set
            {
                if (_Country != value)
                {
                    _Country = value;
                    OnPropertyChanged("Country");
                }
            }
        }

        public string City
        {
            get { return _City; }
            set
            {
                if (_City != value)
                {
                    _City = value;
                    OnPropertyChanged("City");
                }
            }
        }

        public double Latitude
        {
            get { return _Latitude; }
            set
            {
                if (_Latitude != value)
                {
                    _Latitude = value;
                    OnPropertyChanged("Latitude");
                }
            }
        }

        public double Longnitude
        {
            get { return _Longnitude; }
            set
            {
                if (_Longnitude != value)
                {
                    _Longnitude = value;
                    OnPropertyChanged("Longnitude");
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
