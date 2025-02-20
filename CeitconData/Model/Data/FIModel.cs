using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Ceitcon_Data.Model.Data
{ 

    public class FIModel : INotifyPropertyChanged
    {
        //private string _Id;
        private string _Time;
        private string _Flight;
        private string _Description;
        private string _Airline;
        private BitmapImage _Image;
        private string _Gate;
        private string _Status;

        public FIModel()
        {
            //_Id = Guid.NewGuid().ToString();
        }

        //public string Id
        //{
        //    get { return _Id; }
        //    set
        //    {
        //        if (_Id != value)
        //        {
        //            _Id = value;
        //            OnPropertyChanged("Id");
        //        }
        //    }
        //}

        public string Time
        {
            get { return _Time; }
            set
            {
                if (_Time != value)
                {
                    _Time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        public string Flight
        {
            get { return _Flight; }
            set
            {
                if (_Flight != value)
                {
                    _Flight = value;
                    OnPropertyChanged("Flight");
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

        public string Airline
        {
            get { return _Airline; }
            set
            {
                if (_Airline != value)
                {
                    _Airline = value;
                    OnPropertyChanged("Airline");
                }
            }
        }

        public BitmapImage Image
        {
            get { return _Image; }
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    OnPropertyChanged("Image");
                }
            }
        }
        

        public string Gate
        {
            get { return _Gate; }
            set
            {
                if (_Gate != value)
                {
                    _Gate = value;
                    OnPropertyChanged("Gate");
                }
            }
        }

        public string Status
        {
            get { return _Status; }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged("Status");
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
