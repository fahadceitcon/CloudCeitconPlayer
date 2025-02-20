using System;
using System.ComponentModel;

namespace Ceitcon_Data.Model.Data
{
    public class LogoModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Description;
        private string _FileLocation;
        private long _FileSize;

        public LogoModel()
        {
            _Id = Guid.NewGuid().ToString();
        }

        public LogoModel(LogoModel copy, bool fullCopy = false)
        {
            if (fullCopy) { _Id = copy.Id; } else { Guid.NewGuid().ToString(); }
            _Description = copy.Description;
            _FileLocation = copy.FileLocation;
            _FileSize = copy.FileSize;

        }

        public LogoModel Save()
        {
            return new LogoModel(this, true);
        }

        public void Restore(LogoModel copyObj)
        {
            var copy = copyObj as LogoModel;
            Description = copy.Description;
            FileLocation = copy.FileLocation;
            FileSize = copy.FileSize;

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

        public string FileLocation
        {
            get { return _FileLocation; }
            set
            {
                if (_FileLocation != value)
                {
                    _FileLocation = value;
                    OnPropertyChanged("FileLocation");
                }
            }
        }

        public long FileSize
        {
            get { return _FileSize; }
            set
            {
                if (_FileSize != value)
                {
                    _FileSize = value;
                    OnPropertyChanged("FileSize");
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
