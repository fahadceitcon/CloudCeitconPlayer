using Ceitcon_Data.Utilities;
using System;
using System.ComponentModel;

namespace Ceitcon_Data.Model.Playlist
{
    public enum PlaylistType
    {
        SetContent = 0,
        Delay = 1,
        AnimateMargin = 2,
        AnimateOpacity = 3,
        AnimateWidth = 4,
        AnimateHeight = 5,
        AnimateBorder = 6,
        SuspendPlayback = 7,
        ResumePlayback = 8,
    }

    public class PlaylistModel : INotifyPropertyChanged
    {
        public PlaylistModel(ControlModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _Name = "Playlist_" + _Id.Substring(0, 6);
            _StartTime = new TimeSpan(0);
            _Duration = new TimeSpan(0);
            _Forever = true;
        }

        public PlaylistModel(PlaylistModel copy, ControlModel parent, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Parent = fullCopy ? copy.Parent : parent;
            _Name = fullCopy ? copy.Name : "Playlist_" + _Id.Substring(0, 6);
            _StartTime = copy.StartTime;
            _Duration = copy.Duration;
            _Forever = copy.Forever;
            _Depends = copy.Depends;
            _Type = copy.Type;
        }

        public virtual PlaylistModel Save()
        {
            return new PlaylistModel(this, null, true);
        }

        public virtual void Restore(object copyObj)
        {
            PlaylistModel copy = copyObj as PlaylistModel;
            _Id = copy.Id;
            Parent = copy.Parent;
            Name = copy.Name;
            StartTime = copy.StartTime;
            Duration = copy.Duration;
            Forever = copy.Forever;
            Depends = copy.Depends;
            Type = copy.Type;
        }

        private string _Id;
        private ControlModel _Parent;
        public string _Name;
        private TimeSpan _StartTime;
        private TimeSpan _Duration;
        private bool _Forever;
        private PlaylistModel _Depends;
        public PlaylistType _Type;

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

        public ControlModel Parent
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
                    Memento.Push(Save());
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public TimeSpan StartTime
        {
            get { return _StartTime; }
            set
            {
                if (_StartTime != value)
                {
                    Memento.Push(Save());
                    _StartTime = value;
                    OnPropertyChanged("StartTime");
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
                    Memento.Push(Save());
                    _Duration = value;
                    OnPropertyChanged("Duration");
                }
            }
        }

        public bool Forever
        {
            get { return _Forever; }
            set
            {
                if (_Forever != value)
                {
                    Memento.Push(Save());
                    _Forever = value;
                    OnPropertyChanged("Forever");
                }
            }
        }

        public PlaylistModel Depends
        {
            get { return _Depends; }
            set
            {
                if (_Depends != value)
                {
                    Memento.Push(Save());
                    _Depends = value;
                    OnPropertyChanged("Depends");
                    OnPropertyChanged("HasDepends");
                }
            }
        }

        public PlaylistType Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    Memento.Push(Save());
                    _Type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public bool HasDepends
        {
            get { return _Depends != null; }
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
