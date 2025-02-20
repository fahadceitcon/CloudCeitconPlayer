using Ceitcon_Data.Utilities;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace Ceitcon_Data.Model
{

    public class RegionModel : INotifyPropertyChanged
    {
        private const int _markerBorderSize = 20;

        public RegionModel(ProjectModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _Name = "Region_" + _Id.Substring(0, 6);
            _Width = 400;
            _Height = 300;
            _X = 200;
            _Y = 200;
            _Zoom = 100;
            _Slides = new ObservableCollection<SlideModel>();
            var slide = new SlideModel(this);
            _Slides.Add(slide);
            _SelectedSlide = slide;
        }

        public RegionModel(ProjectModel parent, double x, double y, double width, double height, bool fullCopy = false)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _Name = "Region_" + _Id.Substring(0, 6);
            _Width = width;
            _Height = height;
            _X = x;
            _Y = y;
            _Zoom = 100;
            _Slides = new ObservableCollection<SlideModel>();
            var slide = new SlideModel(this);
            _Slides.Add(slide);
            _SelectedSlide = slide;
        }

        public RegionModel(RegionModel copy, ProjectModel parent, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Parent = fullCopy ? copy.Parent : parent;
            _Name = fullCopy ? copy.Name : "Region_" + _Id.Substring(0, 6);
            _Width = copy.Width;
            _Height = copy.Height;
            _X = copy.X;
            _Y = copy.Y;
            _Zoom = copy.Zoom;
            _Slides = new ObservableCollection<SlideModel>();
            foreach (var item in copy.Slides)
            {
                Slides.Add(new SlideModel(item, this, fullCopy));
            }
            _SelectedSlide = copy.SelectedSlide;
        }

        public RegionModel Save()
        {
            return new RegionModel(this, null, true);
        }

        public void Restore(RegionModel copy)
        {
            Memento.Enable = false;
            _Id = copy.Id;
            Parent = copy.Parent;
            Name = copy.Name;
            Width = copy.Width;
            Height = copy.Height;
            X = copy.X;
            Y = copy.Y;
            Zoom = copy.Zoom;
            Slides.Clear();
            foreach (var item in copy.Slides)
            {
                Slides.Add(new SlideModel(item, null, true));
            }
            SelectedSlide = copy.SelectedSlide;
            Memento.Enable = true;
        }

        private string _Id;
        private ProjectModel _Parent;
        private string _Name;
        private double _Width;
        private double _Height;
        private double _X;
        private double _Y;
        private bool _isSelected;
        private double _Zoom;
        private ObservableCollection<SlideModel> _Slides;
        private SlideModel _SelectedSlide;

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

        public ProjectModel Parent
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

        public ObservableCollection<SlideModel> Slides
        {
            //get { return _Slides; }
            get
            {
                ObservableCollection<SlideModel> slides = new ObservableCollection<SlideModel>();
                foreach (var slide in _Slides)
                {
                    if (slide.EnableSchedule)
                    {
                        if (DateTime.Now.Date >= slide.StartDate.Value.Date && DateTime.Now.Date <= slide.EndDate.Value.Date)
                        {
                            DateTime startTime = Convert.ToDateTime(DateTime.Now.Date.ToString("M/dd/yyyy") + " " + slide.StartTime);
                            DateTime endTime = Convert.ToDateTime(DateTime.Now.Date.ToString("M/dd/yyyy") + " " + slide.EndTime);
                            if (DateTime.Now >= startTime && DateTime.Now <= endTime)
                            {
                                slides.Add(slide);
                            }
                        }
                    }
                    else
                        slides.Add(slide);
                }
                return slides;
            }
            set
            {
                if (_Slides != value)
                {
                    //Memento.Push(Save());
                    _Slides = value;
                    OnPropertyChanged("Slides");
                }
            }
        }

        public ObservableCollection<SlideModel> SlidesDownload
        {
            get { return _Slides; }
            set
            {
                if (_Slides != value)
                {
                    //Memento.Push(Save());
                    _Slides = value;
                    OnPropertyChanged("Slides");
                }
            }
        }

        public SlideModel SelectedSlide
        {
            get { return _SelectedSlide; }
            set
            {
                _Parent.SelectedObject = value;
                if (_SelectedSlide != value)
                {
                    //Memento.Push(Save());
                    if (_SelectedSlide != null)
                        _SelectedSlide.IsSelected = false;
                    _SelectedSlide = value;
                    if (_SelectedSlide != null)
                        _SelectedSlide.IsSelected = true;
                    OnPropertyChanged("SelectedSlide");
                }
            }
        }



        public double Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    Memento.Push(Save());
                    _Width = value;
                    OnPropertyChanged("Width");
                    OnPropertyChanged("PercentageWidth");
                    OnPropertyChanged("X");
                    OnPropertyChanged("W");
                }
            }
        }

        public double Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    Memento.Push(Save());
                    _Height = value;
                    OnPropertyChanged("Height");
                    OnPropertyChanged("Y");
                    OnPropertyChanged("Z");
                }
            }
        }
        public int MarkerBorderSize => _markerBorderSize;

        public double HeightWithBorder => _Height + (2 * MarkerBorderSize);
        public double WidthWithBorder => _Width + (2 * MarkerBorderSize);
        public Thickness MarginWithBorder => new Thickness(Margin.Left + MarkerBorderSize, Margin.Top + MarkerBorderSize, Margin.Right + MarkerBorderSize, Margin.Bottom + MarkerBorderSize);

        public double X
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    Memento.Push(Save());
                    //_Width = _Parent.SelectedResolution.Width - value - W;
                    _X = value;

                    OnPropertyChanged("X");
                    OnPropertyChanged("W");
                }
            }
        }

        public double Y
        {
            get { return _Y; }
            set
            {
                if (_Y != value)
                {
                    Memento.Push(Save());
                    //_Height = _Parent.SelectedResolution.Height - value - Z;
                    _Y = value;
                    OnPropertyChanged("Y");
                    OnPropertyChanged("Z");
                }
            }
        }

        public Thickness Margin => new Thickness(X * 2 - _Parent.SelectedResolution.Width + _Width, Y * 2 - _Parent.SelectedResolution.Height + _Height, 0, 0);

        public double W
        {
            get { return _Parent.SelectedResolution.Width - _X - _Width; }
            set
            {
                //X = _Parent.SelectedResolution.Width - value - _Width;
                Width = _Parent.SelectedResolution.Width - value - _X;
                OnPropertyChanged("W");
            }
        }

        public double Z
        {
            get { return _Parent.SelectedResolution.Height - _Y - _Height; }
            set
            {
                //Y = _Parent.SelectedResolution.Height - value - _Height;
                Height = _Parent.SelectedResolution.Height - value - _Y;
                OnPropertyChanged("Z");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    //Memento.Push(Save());
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public double Zoom
        {
            get { return _Zoom; }
            set
            {
                if (_Zoom != value)
                {
                    //Memento.Push(Save());
                    _Zoom = value;
                    OnPropertyChanged("Zoom");
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
