using System;
using System.ComponentModel;

namespace Ceitcon_Data.Model
{
    public class MonitorModel : INotifyPropertyChanged
    {
        public MonitorModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Name = String.Format("Monitor_{0}", _Id.Substring(0, 6));
            _Horizontal = 1;
            _Vertical = 1;
            _IsInitial = false;
        }

        public MonitorModel(string name, int horizontal, int vertical, bool isInitial)
        {
            _Id = Guid.NewGuid().ToString();
            _Name = name;
            _Horizontal = horizontal;
            _Vertical = vertical;
            _IsInitial = isInitial;
        }

        public MonitorModel(MonitorModel copy)
        {
            _Id = copy.Id;
            _Name = copy.Name;
            _Horizontal = copy.Horizontal;
            _Vertical = copy.Vertical;
            _IsInitial = copy.IsInitial;
        }

        public MonitorModel Save()
        {
            return new MonitorModel(this);
        }

        public void Restore(MonitorModel copy)
        {
            Id = copy.Id;
            Name = copy.Name;
            Horizontal = copy.Horizontal;
            Vertical = copy.Vertical;
            IsInitial = copy.IsInitial;
        }

        private string _Id;
        private string _Name;
        private int _Horizontal;
        private int _Vertical;
        private bool _IsInitial;

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
                }
            }
        }
        public int Horizontal
        {
            get { return _Horizontal; }
            set
            {
                if (_Horizontal != value)
                {
                    _Horizontal = value;
                    OnPropertyChanged("Horizontal");
                }
            }
        }

        public int Vertical
        {
            get { return _Vertical; }
            set
            {
                if (_Vertical != value)
                {
                    _Vertical = value;
                    OnPropertyChanged("Vertical");
                }
            }
        }

        public bool IsInitial
        {
            get { return _IsInitial; }
            set
            {
                if (_IsInitial != value)
                {
                    _IsInitial = value;
                    OnPropertyChanged("IsInitial");
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
