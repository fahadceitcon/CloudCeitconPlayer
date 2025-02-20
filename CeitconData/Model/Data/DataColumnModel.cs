using Ceitcon_Data.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Ceitcon_Data.Model.Data
{

    public class DataColumnModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Name;
        private string _Title;
        private double _Width;
        private int _Type;
        private Brush _Foreground;
        private Brush _Background;
        private TextAlignment _TextAlignment;
        private VerticalAlignment _VerticalAlignment;
        private FontFamily _FontFamily;
        private int _FontSize;
        private FontWeight _FontWeight;
        private FontStyle _FontStyle;
        private string _WhereValue;
        private string _WhereOperator;
        private string _MergeColumn;
        private int _Sort;
        private Stretch _ImageStretch;
        private ObservableCollection<SpecialCellModel> _SpecialCells;
        private SpecialCellModel _SelectedSpecialCell;
        private ObservableCollection<TimeFilterModel> _TimeFilters;
        private TimeFilterModel _SelectedTimeFilter;

        private bool _IsVisible;
        private bool _IsExpand;

        public DataColumnModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Width = 200;
            _Type = 0;
            _Foreground = Brushes.White;
            //_Background = Color.FromRgb(255, 255, 255);
            _TextAlignment = TextAlignment.Left;
            _VerticalAlignment = VerticalAlignment.Center;
            _FontSize = 0;
            _WhereValue = String.Empty;
            _WhereOperator = "==";
            _MergeColumn = String.Empty;
            _Sort = 0;
            _ImageStretch = Stretch.Fill;
            _SpecialCells = new ObservableCollection<SpecialCellModel>();
            _TimeFilters = new ObservableCollection<TimeFilterModel>();

            _IsVisible = false;
        }

        public DataColumnModel(DataColumnModel copy, bool fullCopy = false)
        {
            if (fullCopy) { _Id = copy.Id; } else { Guid.NewGuid().ToString(); }
            _Name = copy.Name;
            _Title = copy.Title;
            _Width = copy.Width;
            _Type = copy.Type;
            _Foreground = copy.Foreground;
            _Background = copy.Background;
            _TextAlignment = copy.TextAlignment;
            _VerticalAlignment = copy.VerticalAlignment;
            _WhereValue = copy.WhereValue;
            _WhereOperator = copy.WhereOperator;
            _MergeColumn = copy.MergeColumn;
            _FontFamily = copy.FontFamily;
            _FontSize = copy.FontSize;
            _FontWeight = copy.FontWeight;
            _FontStyle = copy.FontStyle;
            _Sort = copy.Sort;
            _ImageStretch = copy.ImageStretch;
            _SpecialCells = new ObservableCollection<SpecialCellModel>();
            foreach (SpecialCellModel i in copy.SpecialCells)
            {
                _SpecialCells.Add(new SpecialCellModel(i));
            }
            if (copy.SelectedSpecialCell != null)
                SelectedSpecialCell = SpecialCells.Where(_ => _.Id == copy.SelectedSpecialCell.Id).FirstOrDefault();

            _TimeFilters = new ObservableCollection<TimeFilterModel>();
            foreach (TimeFilterModel i in copy.TimeFilters)
            {
                _TimeFilters.Add(new TimeFilterModel(i));
            }
            if (copy.SelectedTimeFilter != null)
                SelectedTimeFilter = TimeFilters.Where(_ => _.Id == copy.SelectedTimeFilter.Id).FirstOrDefault();

            _IsVisible = copy.IsVisible;
        }

        public DataColumnModel Save()
        {
            return new DataColumnModel(this, true);
        }

        public void Restore(DataColumnModel copyObj)
        {
            var copy = copyObj as DataColumnModel;
            Memento.Enable = false;
            Name = copy.Name;
            Title = copy.Title;
            Width = copy.Width;
            Type = copy.Type;
            Foreground = copy.Foreground;
            Background = copy.Background;
            TextAlignment = copy.TextAlignment;
            VerticalAlignment = copy.VerticalAlignment;
            WhereValue = copy.WhereValue;
            WhereOperator = copy.WhereOperator;
            MergeColumn = copy.MergeColumn;
            FontFamily = copy.FontFamily;
            FontSize = copy.FontSize;
            FontWeight = copy.FontWeight;
            FontStyle = copy.FontStyle;
            Sort = copy.Sort;
            ImageStretch = copy.ImageStretch;
            foreach (SpecialCellModel i in copy.SpecialCells)
            {
                SpecialCells.Add(new SpecialCellModel(i));
            }
            if (copy.SelectedSpecialCell != null)
                SelectedSpecialCell = SpecialCells.Where(_ => _.Id == copy.SelectedSpecialCell.Id).FirstOrDefault();
            foreach (TimeFilterModel i in copy.TimeFilters)
            {
                TimeFilters.Add(new TimeFilterModel(i));
            }
            if (copy.SelectedTimeFilter != null)
                SelectedTimeFilter = TimeFilters.Where(_ => _.Id == copy.SelectedTimeFilter.Id).FirstOrDefault();

            IsVisible = copy.IsVisible;
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

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnPropertyChanged("Title");
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
                    _Width = value;
                    OnPropertyChanged("Width");
                }
            }
        }

        public int Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    OnPropertyChanged("Type");
                    OnPropertyChanged("IsImage");
                }
            }
        }

        public Brush Background
        {
            get { return _Background; }
            set
            {
                if (_Background != value)
                {
                    _Background = value;
                    OnPropertyChanged("Background");
                    //OnPropertyChanged("BackgroundBrush");
                }
            }
        }

        //public Brush BackgroundBrush
        //{
        //    get { return new SolidColorBrush(_Background); }
        //}

        public Brush Foreground
        {
            get { return _Foreground; }
            set
            {
                if (_Foreground != value)
                {
                    _Foreground = value;
                    OnPropertyChanged("Foreground");
                }
            }
        }

        //public Brush ForegroundBrush
        //{
        //    get { return new SolidColorBrush(_Foreground); }
        //}

        public TextAlignment TextAlignment
        {
            get { return _TextAlignment; }
            set
            {
                if (_TextAlignment != value)
                {
                    _TextAlignment = value;
                    OnPropertyChanged("TextAlignment");
                }
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return _VerticalAlignment; }
            set
            {
                if (_VerticalAlignment != value)
                {
                    _VerticalAlignment = value;
                    OnPropertyChanged("VerticalAlignment");
                }
            }
        }

        public string WhereValue
        {
            get { return _WhereValue; }
            set
            {
                if (_WhereValue != value)
                {
                    _WhereValue = value;
                    OnPropertyChanged("WhereValue");
                }
            }
        }

        public string WhereOperator
        {
            get { return _WhereOperator; }
            set
            {
                if (_WhereOperator != value)
                {
                    _WhereOperator = value;
                    OnPropertyChanged("WhereOperator");
                }
            }
        }

        public string MergeColumn
        {
            get { return _MergeColumn; }
            set
            {
                if (_MergeColumn != value)
                {
                    _MergeColumn = value;
                    OnPropertyChanged("MergeColumn");
                }
            }
        }

        public Stretch ImageStretch
        {
            get { return _ImageStretch; }
            set
            {
                if (_ImageStretch != value)
                {
                    _ImageStretch = value;
                    OnPropertyChanged("ImageStretch");
                }
            }
        }

        public ObservableCollection<SpecialCellModel> SpecialCells
        {
            get { return _SpecialCells; }
            set
            {
                if (_SpecialCells != value)
                {
                    _SpecialCells = value;
                    OnPropertyChanged("SpecialCells");
                }
            }
        }

        public SpecialCellModel SelectedSpecialCell
        {
            get { return _SelectedSpecialCell; }
            set
            {
                if (_SelectedSpecialCell != value)
                {
                    _SelectedSpecialCell = value;
                    OnPropertyChanged("SelectedSpecialCell");
                }
            }
        }

        public ObservableCollection<TimeFilterModel> TimeFilters
        {
            get { return _TimeFilters; }
            set
            {
                if (_TimeFilters != value)
                {
                    _TimeFilters = value;
                    OnPropertyChanged("TimeFilters");
                }
            }
        }

        public TimeFilterModel SelectedTimeFilter
        {
            get { return _SelectedTimeFilter; }
            set
            {
                if (_SelectedTimeFilter != value)
                {
                    _SelectedTimeFilter = value;
                    OnPropertyChanged("SelectedTimeFilter");
                }
            }
        }

        public FontFamily FontFamily
        {
            get { return _FontFamily; }
            set
            {
                if (_FontFamily != value)
                {
                    _FontFamily = value;
                    OnPropertyChanged("FontFamily");
                }
            }
        }

        public int FontSize
        {
            get { return _FontSize; }
            set
            {
                if (_FontSize != value)
                {
                    _FontSize = value;
                    OnPropertyChanged("FontSize");
                }
            }
        }

        public FontWeight FontWeight
        {
            get { return _FontWeight; }
            set
            {
                if (_FontWeight != value)
                {
                    _FontWeight = value;
                    OnPropertyChanged("FontWeight");
                }
            }
        }

        public FontStyle FontStyle
        {
            get { return _FontStyle; }
            set
            {
                if (_FontStyle != value)
                {
                    _FontStyle = value;
                    OnPropertyChanged("FontStyle");
                }
            }
        }

        public int Sort
        {
            get { return _Sort; }
            set
            {
                if (_Sort != value)
                {
                    _Sort = value;
                    OnPropertyChanged("Sort");
                }
            }
        }

        public bool IsTimeFilterVisible
        {
            get { return Name == "SCH_TIME" || Name == "EST_TIME"; }
        }

        public bool IsImage
        {
            get { return _Type == 1; }
        }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (_IsVisible != value)
                {
                    _IsVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        public bool IsExpand
        {
            get { return _IsExpand; }
            set
            {
                if (_IsExpand != value)
                {
                    _IsExpand = value;
                    OnPropertyChanged("IsExpand");
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

