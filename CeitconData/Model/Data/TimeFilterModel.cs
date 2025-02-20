using Ceitcon_Data.Utilities;
using System;
using System.ComponentModel;
using System.Windows.Media;


namespace Ceitcon_Data.Model.Data
{
    public class TimeFilterModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Name;
        private int _BeforeDuration;
        private int _AfterDuration;

        public TimeFilterModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Name = "Time filter";
            _BeforeDuration = 0;
            _AfterDuration = 0;
        }

        public TimeFilterModel(TimeFilterModel copy, bool fullCopy = false)
        {
            if (fullCopy) { _Id = copy.Id; } else { Guid.NewGuid().ToString(); }
            _Name = copy.Name;
            _BeforeDuration = copy.BeforeDuration;
            _AfterDuration = copy.AfterDuration;
        }

        public TimeFilterModel Save()
        {
            return new TimeFilterModel(this, true);
        }

        public void Restore(TimeFilterModel copyObj)
        {
            var copy = copyObj as TimeFilterModel;
            Memento.Enable = false;
            Name = copy.Name;
            BeforeDuration = copy.BeforeDuration;
            AfterDuration = copy.AfterDuration;

            Memento.Enable = true;
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
                }
            }
        }

        public int BeforeDuration
        {
            get { return _BeforeDuration; }
            set
            {
                if (_BeforeDuration != value)
                {
                    _BeforeDuration = value;
                    OnPropertyChanged("BeforeDuration");
                }
            }
        }

        public int AfterDuration
        {
            get { return _AfterDuration; }
            set
            {
                if (_AfterDuration != value)
                {
                    _AfterDuration = value;
                    OnPropertyChanged("AfterDuration");
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

