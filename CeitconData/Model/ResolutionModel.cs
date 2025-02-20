using System;
using System.ComponentModel;

namespace Ceitcon_Data.Model
{
    public class ResolutionModel : INotifyPropertyChanged
    {
        public ResolutionModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Name = String.Format("Res_{0} (X x Y) ", _Id.Substring(0, 6));
            _Width = 1920;
            _Height = 1080;
            _IsInitial = false;
        }

        public ResolutionModel(string name, int width, int height, bool isInitial)
        {
            _Id = Guid.NewGuid().ToString();
            _Name = name;
            _Width = width;
            _Height = height;
            _IsInitial = isInitial;
        }

        public ResolutionModel(ResolutionModel copy)
        {
            _Id = copy.Id;
            _Name = copy.Name;
            _Width = copy.Width;
            _Height = copy.Height;
            _IsInitial = copy.IsInitial;
        }

        public ResolutionModel Save()
        {
            return new ResolutionModel(this);
        }

        public void Restore(ResolutionModel copy)
        {
            Id = copy.Id;
            Name = copy.Name;
            Width = copy.Width;
            Height = copy.Height;
            IsInitial = copy.IsInitial;
        }

        private string _Id;
        private string _Name;
        private int _Width;
        private int _Height;
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

        public int Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    OnPropertyChanged("Width");
                }
            }
        }

        public int Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    OnPropertyChanged("Height");
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
