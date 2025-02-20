using Ceitcon_Data.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media;

namespace Ceitcon_Data.Model
{
    public class SlideModel : INotifyPropertyChanged
    {
        public SlideModel(RegionModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _Name = "Slide_" + _Id.Substring(0, 6);
            _Marker = Brushes.Transparent;
            _Forever = false;
            _Duration = TimeSpan.FromSeconds(10);
            _IsSelected = false;
            _EnableSchedule = false;
            _StartDate = DateTime.Now;
            _EndDate = DateTime.Now.AddDays(3);
            _StartTime = DateTime.Now.ToShortTimeString();
            _EndTime = DateTime.Now.AddHours(6).ToShortTimeString();
            _Layers = new ObservableCollection<LayerModel>();
            var layer = new LayerModel(this);
            _Layers.Add(layer);
            SelectedLayer = layer;
        }

        public SlideModel(SlideModel copy, RegionModel parent, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Parent = fullCopy ? copy.Parent : parent;
            _Name = fullCopy ? copy.Name : "Slide_" + _Id.Substring(0, 6);
            _Marker = copy.Marker;
            _Forever = copy.Forever;
            _Duration = copy.Duration;
            _IsSelected = copy.IsSelected;
            _EnableSchedule = copy._EnableSchedule;
            _StartDate = copy._StartDate;
            _EndDate = copy._EndDate;
            _StartTime = copy._StartTime;
            _EndTime = copy._EndTime;
            _Layers = new ObservableCollection<LayerModel>();
            foreach (var i in copy.Layers)
            {
                _Layers.Add(new LayerModel(i, this, fullCopy));
            }
            _SelectedLayer = _Layers.FirstOrDefault();
        }

        public SlideModel Save()
        {
            return new SlideModel(this, null, true);
        }

        public void Restore(SlideModel copy)
        {
            Memento.Enable = false;
            _Id = copy.Id;
            Parent = copy.Parent;
            Name = copy.Name;
            Marker = copy.Marker;
            Forever = copy.Forever;
            Duration = copy.Duration;
            IsSelected = copy.IsSelected;
            EnableSchedule = copy.EnableSchedule;
            StartDate = copy.StartDate;
            EndDate = copy.EndDate;
            StartTime = copy.StartTime;
            EndTime = copy.EndTime;
            Layers.Clear();
            foreach (LayerModel item in copy.Layers)
            {
                Layers.Add(new LayerModel(item, null, true));
            }
            SelectedLayer = copy.SelectedLayer;
            Memento.Enable = true;
        }

        private string _Id;
        private RegionModel _Parent;
        private string _Name;
        private Brush _Marker;
        private bool _Forever;
        private TimeSpan _Duration;
        private ObservableCollection<LayerModel> _Layers;
        private LayerModel _SelectedLayer;
        private bool _IsSelected;
        private bool _EnableSchedule;
        private DateTime? _StartDate;
        private DateTime? _EndDate;
        private string _StartTime;
        private string _EndTime;

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

        public RegionModel Parent
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

        public Brush Marker
        {
            get { return _Marker; }
            set
            {
                if (_Marker != value)
                {

                    _Marker = value;
                    OnPropertyChanged("Marker");
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

        public ObservableCollection<LayerModel> Layers
        {
            get { return _Layers; }
            set
            {
                if (_Layers != value)
                {
                    _Layers = value;
                    OnPropertyChanged("Layers");
                }
            }
        }

        public LayerModel SelectedLayer
        {
            get { return _SelectedLayer; }
            set
            {
                if (_Parent != null && _Parent.Parent != null)
                    _Parent.Parent.SelectedObject = value;
                if (_SelectedLayer != value)
                {
                    _SelectedLayer = value;
                    OnPropertyChanged("SelectedLayer");
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
        public bool EnableSchedule
        {
            get { return _EnableSchedule; }
            set
            {
                if (_EnableSchedule != value)
                {
                    //Memento.Push(Save());
                    _EnableSchedule = value;
                    OnPropertyChanged("EnableSchedule");
                }
            }
        }

        public DateTime? StartDate
        {
            get { return _StartDate; }
            set
            {
                if (_StartDate != value)
                {
                    Memento.Push(Save());
                    _StartDate = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public DateTime? EndDate
        {
            get { return _EndDate; }
            set
            {
                if (_EndDate != value)
                {
                    Memento.Push(Save());
                    _EndDate = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string StartTime
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

        public string EndTime
        {
            get { return _EndTime; }
            set
            {
                if (_EndTime != value)
                {
                    Memento.Push(Save());
                    _EndTime = value;
                    OnPropertyChanged("EndTime");
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