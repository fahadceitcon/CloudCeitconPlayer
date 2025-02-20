using Ceitcon_Data.Utilities;
using System;
using System.ComponentModel;

namespace Ceitcon_Data.Model
{
    public class SlotModel : INotifyPropertyChanged
    {
        public SlotModel(AlertModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _Name = "Slot_" + _Id.Substring(0, 6);
            _Start = DateTime.Now;
            //_Latitude = parent.Type == AlertType.Prayer ? "46.675296" : String.Empty;
            //_Longitude = parent.Type == AlertType.Prayer ? "24.713552" : String.Empty;
            _Duration = new TimeSpan(0, 1, 0);
            _IsSelected = true;
            _IsVisible = true;
        }

        public SlotModel(SlotModel copy, AlertModel parent, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Parent = fullCopy ? copy.Parent : parent;
            _Name = fullCopy ? copy.Name : "Slot_" + _Id.Substring(0, 6);
            _Start = copy.Start;
            _Location = copy.Location;
            _Duration = copy.Duration;
            _IsVisible = copy.IsVisible;
            _IsLocked = copy.IsLocked;
            _IsSelected = copy.IsSelected;
        }

        public SlotModel Save()
        {
            return new SlotModel(this, null, true);
        }

        public void Restore(SlotModel copy)
        {
            Memento.Enable = false;
            _Id = copy.Id;
            Parent = copy.Parent;
            Name = copy.Name;
            Start = copy.Start;
            Location = copy.Location;
            Duration = copy.Duration;
            IsVisible = copy.IsVisible;
            IsLocked = copy.IsLocked;
            IsSelected = copy.IsSelected;
            Memento.Enable = true;
        }

        private string _Id;
        private AlertModel _Parent;
        private string _Name;
        private DateTime _Start;
        private LocationModel _Location;
        private TimeSpan _Duration;
        private bool _IsVisible;
        private bool _IsLocked;
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

        public AlertModel Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent != value)
                {
                    Memento.Push(Save());
                    _Parent = value;
                    OnPropertyChanged("Parent");
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

        public DateTime Start
        {
            get { return _Start; }
            set
            {
                if (_Start != value)
                {
                    _Start = value;
                    OnPropertyChanged("Start");
                }
            }
        }

        public LocationModel Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    OnPropertyChanged("Location");
                }
            }
        }

        public TimeSpan Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration != value)
                {
                    _Duration = value;
                    OnPropertyChanged("Duration");
                }
            }
        }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (_IsVisible != value)
                {
                    Memento.Push(Save());
                    _IsVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        public bool IsLocked
        {
            get { return _IsLocked; }
            set
            {
                if (_IsLocked != value)
                {
                    Memento.Push(Save());
                    _IsLocked = value;
                    OnPropertyChanged("IsLocked");
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
                    //Memento.Push(Save());
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
