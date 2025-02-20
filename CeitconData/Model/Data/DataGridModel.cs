using Ceitcon_Data.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Ceitcon_Data.Model.Data
{

    public class DataGridModel : INotifyPropertyChanged
    {
        private string _Id;
        private Brush _RowBackground;
        private Brush _BorderBrush;
        private Thickness _BorderThickness;
        private CornerRadius _RowCornerRadius;
        private Thickness _RowMargin;
        private DataGridGridLinesVisibility _LinesVisibility;
        private int _AlternationCount;
        private Brush _AlternatingRowBackground;
        private Brush _VerticalLineColour;
        private Brush _HorizontalLineColour;
        private bool _IsVisibleShadow;
        private DropShadowEffect _RowShadowEffect;
        private int _HeaderSize;
        private int _HeaderHeight;
        private Brush _HeaderForeground;
        private Brush _HeaderBackground;
        private Brush _HeaderBorderBrush;
        private Thickness _HeaderBorderThickness;
        private CornerRadius _HeaderCornerRadius;
        private Thickness _HeaderMargin;
        private FontFamily _HeaderFontFamily;
        private FontWeight _HeaderFontWeight;
        private FontStyle _HeaderFontStyle;
        private HorizontalAlignment _HeaderHorizontalAlignment;
        private VerticalAlignment _HeaderVerticalAlignment;
        private bool _HeaderIsVisibleShadow;
        private DropShadowEffect _HeaderShadowEffect;
        private int _MaxRows;
        private double _RowHeight;
        private int _RefreshTime;
        private ObservableCollection<DataSourceModel> _Sources;
        private DataSourceModel _SelectedSource;
        private bool _IsExpand;

        public DataGridModel()
        {
            _Id = Guid.NewGuid().ToString();
           // _RowBackground = Brushes.Transparent;
            _BorderBrush = null;
            _BorderThickness = new Thickness(0);
            _RowCornerRadius = new CornerRadius(0);
            _RowMargin = new Thickness(0);
            _LinesVisibility = DataGridGridLinesVisibility.All;
            _VerticalLineColour = Brushes.Black;
            _HorizontalLineColour = Brushes.Black;
            _AlternationCount = 0;
            _AlternatingRowBackground = Brushes.Gray;
            _IsVisibleShadow = false;
            _RowShadowEffect = new DropShadowEffect()
            {
                BlurRadius = 0,
                Opacity = 1,
                Color = Colors.Gray,
                ShadowDepth = 10,
                Direction = -45,
                RenderingBias = RenderingBias.Performance,
            };
            _HeaderSize = 30;
            _HeaderHeight = 65;
            _HeaderForeground = Brushes.White;
            _HeaderBackground = Brushes.Blue;
            _HeaderBorderBrush = null;
            _HeaderCornerRadius = new CornerRadius(0);
            _HeaderMargin = new Thickness(0);
            _HeaderBorderThickness = new Thickness(0);
            _HeaderFontFamily = new FontFamily("Arial");
            _HeaderFontWeight = FontWeights.Normal;
            _HeaderFontStyle = FontStyles.Normal;
            _HeaderHorizontalAlignment = HorizontalAlignment.Center;
            _HeaderVerticalAlignment = VerticalAlignment.Center;
            _HeaderIsVisibleShadow = false;
            _HeaderShadowEffect = new DropShadowEffect()
            {
                BlurRadius = 0,
                Opacity = 1,
                Color = Colors.Gray,
                ShadowDepth = 10,
                Direction = -45,
                RenderingBias = RenderingBias.Performance,
            };
            _MaxRows = 20;
            _RowHeight = 50;
            _RefreshTime = 20;
            _Sources = new ObservableCollection<DataSourceModel>();
        }

        public DataGridModel(DataGridModel copy, bool fullCopy = false)
        {
            if (fullCopy) { _Id = copy.Id; } else { Guid.NewGuid().ToString(); }
            _RowBackground = copy.RowBackground;
            _BorderBrush = copy.BorderBrush;
            _BorderThickness = copy.BorderThickness;
            _RowCornerRadius = copy.RowCornerRadius;
            _RowMargin = copy.RowMargin;
            _LinesVisibility = copy.LinesVisibility;
            _VerticalLineColour = copy.VerticalLineColour;
            _HorizontalLineColour = copy.HorizontalLineColour;
            _AlternationCount = copy.AlternationCount;
            _AlternatingRowBackground = copy.AlternatingRowBackground;
            _IsVisibleShadow = copy.IsVisibleShadow;
            _RowShadowEffect = copy.RowShadowEffect;
            _HeaderSize = copy.HeaderSize;
            _HeaderHeight = copy.HeaderHeight;
            _HeaderForeground = copy.HeaderForeground;
            _HeaderBackground = copy.HeaderBackground;
            _HeaderBorderBrush = copy.HeaderBorderBrush;
            _HeaderCornerRadius = copy.HeaderCornerRadius;
            _HeaderMargin = copy.HeaderMargin;
            _HeaderBorderThickness = copy.HeaderBorderThickness;
            _HeaderFontFamily = copy.HeaderFontFamily;
            _HeaderFontWeight = copy.HeaderFontWeight;
            _HeaderFontStyle = copy.HeaderFontStyle;
            _HeaderHorizontalAlignment = copy.HeaderHorizontalAlignment;
            _HeaderVerticalAlignment = copy.HeaderVerticalAlignment;
            _HeaderIsVisibleShadow = copy.HeaderIsVisibleShadow;
            _HeaderShadowEffect = copy.HeaderShadowEffect;
            _MaxRows = copy.MaxRows;
            _RowHeight = copy.RowHeight;
            _RefreshTime = copy.RefreshTime;
            _Sources = new ObservableCollection<DataSourceModel>();
            foreach (DataSourceModel i in copy.Sources)
            {
                _Sources.Add(new DataSourceModel(i));
            }
            if (copy.SelectedSource != null)
                SelectedSource = Sources.Where(_ => _.Name == copy.SelectedSource.Name).FirstOrDefault();
        }

        public DataGridModel Save()
        {
            return new DataGridModel(this, true);
        }

        public void Restore(DataGridModel copyObj)
        {
            var copy = copyObj as DataGridModel;
            Memento.Enable = false;
            RowBackground = copy.RowBackground;
            BorderBrush = copy.BorderBrush;
            BorderThickness = copy.BorderThickness;
            RowCornerRadius = copy.RowCornerRadius;
            RowMargin = copy.RowMargin;
            LinesVisibility = copy.LinesVisibility;
            VerticalLineColour = copy.VerticalLineColour;
            HorizontalLineColour = copy.HorizontalLineColour;
            AlternationCount = copy.AlternationCount;
            AlternatingRowBackground = copy.AlternatingRowBackground;
            IsVisibleShadow = copy.IsVisibleShadow;
            RowShadowEffect = copy.RowShadowEffect;
            HeaderSize = copy.HeaderSize;
            HeaderHeight = copy.HeaderHeight;
            HeaderForeground = copy.HeaderForeground;
            HeaderBackground = copy.HeaderBackground;
            HeaderBorderBrush = copy.HeaderBorderBrush;
            HeaderBorderThickness = copy.HeaderBorderThickness;
            HeaderCornerRadius = copy.HeaderCornerRadius;
            HeaderMargin = copy.HeaderMargin;
            HeaderFontFamily = copy.HeaderFontFamily;
            HeaderFontWeight = copy.HeaderFontWeight;
            HeaderFontStyle = copy.HeaderFontStyle;
            HeaderHorizontalAlignment = copy.HeaderHorizontalAlignment;
            HeaderVerticalAlignment = copy.HeaderVerticalAlignment;
            HeaderIsVisibleShadow = copy.HeaderIsVisibleShadow;
            HeaderShadowEffect = copy.HeaderShadowEffect;
            MaxRows = copy.MaxRows;
            RowHeight = copy.RowHeight;
            RefreshTime = copy.RefreshTime;
            _Sources.Clear();
            foreach (DataSourceModel i in copy.Sources)
            {
                Sources.Add(new DataSourceModel(i));
            }
            if (copy.SelectedSource != null) 
                SelectedSource = Sources.Where(_ => _.Name == copy.SelectedSource.Name).FirstOrDefault();

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

        public Brush RowBackground
        {
            get { return _RowBackground; }
            set
            {
                if (_RowBackground != value)
                {
                    _RowBackground = value;
                    OnPropertyChanged("RowBackground");
                }
            }
        }

        public Brush BorderBrush
        {
            get { return _BorderBrush; }
            set
            {
                if (_BorderBrush != value)
                {
                    _BorderBrush = value;
                    OnPropertyChanged("BorderBrush");
                }
            }
        }


        public CornerRadius RowCornerRadius
        {
            get { return _RowCornerRadius; }
            set
            {
                if (_RowCornerRadius != value)
                {
                    _RowCornerRadius = value;
                    OnPropertyChanged("RowCornerRadius");
                }
            }
        }

        public double RowCornerRadiusTopLeft
        {
            get { return _RowCornerRadius.TopLeft; }
            set
            {
                if (_RowCornerRadius.TopLeft != value)
                {
                    _RowCornerRadius.TopLeft = value;
                    OnPropertyChanged("RowCornerRadius");
                }
            }
        }

        public double RowCornerRadiusTopRight
        {
            get { return _RowCornerRadius.TopRight; }
            set
            {
                if (_RowCornerRadius.TopRight != value)
                {
                    _RowCornerRadius.TopRight = value;
                    OnPropertyChanged("RowCornerRadius");
                }
            }
        }

        public double RowCornerRadiusBottomLeft
        {
            get { return _RowCornerRadius.BottomLeft; }
            set
            {
                if (_RowCornerRadius.BottomLeft != value)
                {
                    _RowCornerRadius.BottomLeft = value;
                    OnPropertyChanged("RowCornerRadius");
                }
            }
        }

        public double RowCornerRadiusBottomRight
        {
            get { return _RowCornerRadius.BottomRight; }
            set
            {
                if (_RowCornerRadius.BottomRight != value)
                {
                    _RowCornerRadius.BottomRight = value;
                    OnPropertyChanged("RowCornerRadius");
                }
            }
        }


        public Thickness RowMargin
        {
            get { return _RowMargin; }
            set
            {
                if (_RowMargin != value)
                {
                    _RowMargin = value;
                    OnPropertyChanged("RowMargin");
                }
            }
        }

        public double RowMarginLeft
        {
            get { return _RowMargin.Left; }
            set
            {
                if (_RowMargin.Left != value)
                {
                    _RowMargin.Left = value;
                    OnPropertyChanged("RowMargin");
                }
            }
        }

        public double RowMarginRight
        {
            get { return _RowMargin.Right; }
            set
            {
                if (_RowMargin.Right != value)
                {
                    _RowMargin.Right = value;
                    OnPropertyChanged("RowMargin");
                }
            }
        }

        public double RowMarginTop
        {
            get { return _RowMargin.Top; }
            set
            {
                if (_RowMargin.Top != value)
                {
                    _RowMargin.Top = value;
                    OnPropertyChanged("RowMargin");
                }
            }
        }

        public double RowMarginBottom
        {
            get { return _RowMargin.Bottom; }
            set
            {
                if (_RowMargin.Bottom != value)
                {
                    _RowMargin.Bottom = value;
                    OnPropertyChanged("RowMargin");
                }
            }
        }

        public Thickness BorderThickness
        {
            get { return _BorderThickness; }
            set
            {
                if (_BorderThickness != value)
                {
                    _BorderThickness = value;
                    OnPropertyChanged("BorderThickness");
                }
            }
        }

        public double BorderThicknessLeft
        {
            get { return _BorderThickness.Left; }
            set
            {
                _BorderThickness.Left = value;
                OnPropertyChanged("BorderThickness");
            }
        }

        public double BorderThicknessRight
        {
            get { return _BorderThickness.Right; }
            set
            {
                _BorderThickness.Right = value;
                OnPropertyChanged("BorderThickness");
            }
        }

        public double BorderThicknessTop
        {
            get { return _BorderThickness.Top; }
            set
            {
                _BorderThickness.Top = value;
                OnPropertyChanged("BorderThickness");
            }
        }

        public double BorderThicknessBottom
        {
            get { return _BorderThickness.Bottom; }
            set
            {
                _BorderThickness.Bottom = value;
                OnPropertyChanged("BorderThickness");
            }
        }

        public bool IsVisibleShadow
        {
            get { return _IsVisibleShadow; }
            set
            {
                if (_IsVisibleShadow != value)
                {
                    _IsVisibleShadow = value;
                    OnPropertyChanged("IsVisibleShadow");
                }
            }
        }

        public DropShadowEffect RowShadowEffect
        {
            get { return _RowShadowEffect; }
            set
            {
                if (_RowShadowEffect != value)
                {
                    _RowShadowEffect = value;
                    OnPropertyChanged("RowShadowEffect");
                }
            }
        }

        public double RowShadowEffectBlurRadius
        {
            get { return _RowShadowEffect.BlurRadius; }
            set
            {
                if (_RowShadowEffect.BlurRadius != value)
                {
                    _RowShadowEffect.BlurRadius = value;
                    OnPropertyChanged("RowShadowEffect");
                }
            }
        }

        public double RowShadowEffectOpacity
        {
            get { return _RowShadowEffect.Opacity; }
            set
            {
                if (_RowShadowEffect.Opacity != value)
                {
                    _RowShadowEffect.Opacity = value;
                    OnPropertyChanged("RowShadowEffect");
                }
            }
        }

        //public Color RowShadowEffectColor
        //{
        //    get { return _RowShadowEffect.Color; }
        //    set
        //    {
        //        if (_RowShadowEffect.Color != value)
        //        {
        //            _RowShadowEffect.Color = value;
        //            OnPropertyChanged("RowShadowEffect");
        //        }
        //    }
        //}

        public double RowShadowEffectShadowDepth
        {
            get { return _RowShadowEffect.ShadowDepth; }
            set
            {
                if (_RowShadowEffect.ShadowDepth != value)
                {
                    _RowShadowEffect.ShadowDepth = value;
                    OnPropertyChanged("RowShadowEffect");
                }
            }
        }

        public double RowShadowEffectDirection
        {
            get { return _RowShadowEffect.Direction; }
            set
            {
                if (_RowShadowEffect.Direction != value)
                {
                    _RowShadowEffect.Direction = value;
                    OnPropertyChanged("RowShadowEffect");
                }
            }
        }

        public int HeaderSize
        {
            get { return _HeaderSize; }
            set
            {
                if (_HeaderSize != value)
                {
                    _HeaderSize = value;
                    OnPropertyChanged("HeaderSize");
                }
            }
        }

        public int HeaderHeight
        {
            get { return _HeaderHeight; }
            set
            {
                if (_HeaderHeight != value)
                {
                    _HeaderHeight = value;
                    OnPropertyChanged("HeaderHeight");
                }
            }
        }

        public Brush HeaderBackground
        {
            get { return _HeaderBackground; }
            set
            {
                if (_HeaderBackground != value)
                {
                    _HeaderBackground = value;
                    OnPropertyChanged("HeaderBackground");
                }
            }
        }

        public Brush HeaderForeground
        {
            get { return _HeaderForeground; }
            set
            {
                if (_HeaderForeground != value)
                {
                    _HeaderForeground = value;
                    OnPropertyChanged("HeaderForeground");
                }
            }
        }

        public Brush HeaderBorderBrush
        {
            get { return _HeaderBorderBrush; }
            set
            {
                if (_HeaderBorderBrush != value)
                {
                    _HeaderBorderBrush = value;
                    OnPropertyChanged("HeaderBorderBrush");
                }
            }
        }

        public Thickness HeaderBorderThickness
        {
            get { return _HeaderBorderThickness; }
            set
            {
                if (_HeaderBorderThickness != value)
                {
                    _HeaderBorderThickness = value;
                    OnPropertyChanged("HeaderBorderThickness");
                }
            }
        }


        public CornerRadius HeaderCornerRadius
        {
            get { return _HeaderCornerRadius; }
            set
            {
                if (_HeaderCornerRadius != value)
                {
                    _HeaderCornerRadius = value;
                    OnPropertyChanged("HeaderCornerRadius");
                }
            }
        }

        public double HeaderCornerRadiusTopLeft
        {
            get { return _HeaderCornerRadius.TopLeft; }
            set
            {
                if (_HeaderCornerRadius.TopLeft != value)
                {
                    _HeaderCornerRadius.TopLeft = value;
                    OnPropertyChanged("HeaderCornerRadius");
                }
            }
        }

        public double HeaderCornerRadiusTopRight
        {
            get { return _HeaderCornerRadius.TopRight; }
            set
            {
                if (_HeaderCornerRadius.TopRight != value)
                {
                    _HeaderCornerRadius.TopRight = value;
                    OnPropertyChanged("HeaderCornerRadius");
                }
            }
        }

        public double HeaderCornerRadiusBottomLeft
        {
            get { return _HeaderCornerRadius.BottomLeft; }
            set
            {
                if (_HeaderCornerRadius.BottomLeft != value)
                {
                    _HeaderCornerRadius.BottomLeft = value;
                    OnPropertyChanged("HeaderCornerRadius");
                }
            }
        }

        public double HeaderCornerRadiusBottomRight
        {
            get { return _HeaderCornerRadius.BottomRight; }
            set
            {
                if (_HeaderCornerRadius.BottomRight != value)
                {
                    _HeaderCornerRadius.BottomRight = value;
                    OnPropertyChanged("HeaderCornerRadius");
                }
            }
        }


        public Thickness HeaderMargin
        {
            get { return _HeaderMargin; }
            set
            {
                if (_HeaderMargin != value)
                {
                    _HeaderMargin = value;
                    OnPropertyChanged("HeaderMargin");
                }
            }
        }

        public double HeaderMarginLeft
        {
            get { return _HeaderMargin.Left; }
            set
            {
                if (_HeaderMargin.Left != value)
                {
                    _HeaderMargin.Left = value;
                    OnPropertyChanged("HeaderMargin");
                }
            }
        }

        public double HeaderMarginRight
        {
            get { return _HeaderMargin.Right; }
            set
            {
                if (_HeaderMargin.Right != value)
                {
                    _HeaderMargin.Right = value;
                    OnPropertyChanged("HeaderMargin");
                }
            }
        }

        public double HeaderMarginTop
        {
            get { return _HeaderMargin.Top; }
            set
            {
                if (_HeaderMargin.Top != value)
                {
                    _HeaderMargin.Top = value;
                    OnPropertyChanged("HeaderMargin");
                }
            }
        }

        public double HeaderMarginBottom
        {
            get { return _HeaderMargin.Bottom; }
            set
            {
                if (_HeaderMargin.Bottom != value)
                {
                    _HeaderMargin.Bottom = value;
                    OnPropertyChanged("HeaderMargin");
                }
            }
        }

        public double HeaderBorderThicknessLeft
        {
            get { return _HeaderBorderThickness.Left; }
            set
            {
                _HeaderBorderThickness.Left = value;
                OnPropertyChanged("HeaderBorderThickness");
            }
        }

        public double HeaderBorderThicknessRight
        {
            get { return _HeaderBorderThickness.Right; }
            set
            {
                _HeaderBorderThickness.Right = value;
                OnPropertyChanged("HeaderBorderThickness");
            }
        }

        public double HeaderBorderThicknessTop
        {
            get { return _HeaderBorderThickness.Top; }
            set
            {
                _HeaderBorderThickness.Top = value;
                OnPropertyChanged("HeaderBorderThickness");
            }
        }

        public double HeaderBorderThicknessBottom
        {
            get { return _HeaderBorderThickness.Bottom; }
            set
            {
                _HeaderBorderThickness.Bottom = value;
                OnPropertyChanged("HeaderBorderThickness");
            }
        }


        public FontFamily HeaderFontFamily
        {
            get { return _HeaderFontFamily; }
            set
            {
                if (_HeaderFontFamily != value)
                {
                    _HeaderFontFamily = value;
                    OnPropertyChanged("HeaderFontFamily");
                }
            }
        }

        public FontWeight HeaderFontWeight
        {
            get { return _HeaderFontWeight; }
            set
            {
                if (_HeaderFontWeight != value)
                {
                    _HeaderFontWeight = value;
                    OnPropertyChanged("HeaderFontWeight");
                }
            }
        }

        public FontStyle HeaderFontStyle
        {
            get { return _HeaderFontStyle; }
            set
            {
                if (_HeaderFontStyle != value)
                {
                    _HeaderFontStyle = value;
                    OnPropertyChanged("HeaderFontStyle");
                }
            }
        }

        public HorizontalAlignment HeaderHorizontalAlignment
        {
            get { return _HeaderHorizontalAlignment; }
            set
            {
                if (_HeaderHorizontalAlignment != value)
                {
                    _HeaderHorizontalAlignment = value;
                    OnPropertyChanged("HeaderHorizontalAlignment");
                }
            }
        }

        public VerticalAlignment HeaderVerticalAlignment
        {
            get { return _HeaderVerticalAlignment; }
            set
            {
                if (_HeaderVerticalAlignment != value)
                {
                    _HeaderVerticalAlignment = value;
                    OnPropertyChanged("HeaderVerticalAlignment");
                }
            }
        }

        public DataGridGridLinesVisibility LinesVisibility
        {
            get { return _LinesVisibility; }
            set
            {
                if (_LinesVisibility != value)
                {
                    _LinesVisibility = value;
                    OnPropertyChanged("LinesVisibility");
                }
            }
        }

        public Brush VerticalLineColour
        {
            get { return _VerticalLineColour; }
            set
            {
                if (_VerticalLineColour != value)
                {
                    _VerticalLineColour = value;
                    OnPropertyChanged("VerticalLineColour");
                }
            }
        }

        public Brush HorizontalLineColour
        {
            get { return _HorizontalLineColour; }
            set
            {
                if (_HorizontalLineColour != value)
                {
                    _HorizontalLineColour = value;
                    OnPropertyChanged("HorizontalLineColour");
                }
            }
        }

        public bool HeaderIsVisibleShadow
        {
            get { return _HeaderIsVisibleShadow; }
            set
            {
                if (_HeaderIsVisibleShadow != value)
                {
                    _HeaderIsVisibleShadow = value;
                    OnPropertyChanged("HeaderIsVisibleShadow");
                }
            }
        }

        public DropShadowEffect HeaderShadowEffect
        {
            get { return _HeaderShadowEffect; }
            set
            {
                if (_HeaderShadowEffect != value)
                {
                    _HeaderShadowEffect = value;
                    OnPropertyChanged("HeaderShadowEffect");
                }
            }
        }

        public double HeaderShadowEffectBlurRadius
        {
            get { return _HeaderShadowEffect.BlurRadius; }
            set
            {
                if (_HeaderShadowEffect.BlurRadius != value)
                {
                    _HeaderShadowEffect.BlurRadius = value;
                    OnPropertyChanged("HeaderShadowEffect");
                }
            }
        }

        public double HeaderShadowEffectOpacity
        {
            get { return _HeaderShadowEffect.Opacity; }
            set
            {
                if (_HeaderShadowEffect.Opacity != value)
                {
                    _HeaderShadowEffect.Opacity = value;
                    OnPropertyChanged("HeaderShadowEffect");
                }
            }
        }

        //public Color RowShadowEffectColor
        //{
        //    get { return _RowShadowEffect.Color; }
        //    set
        //    {
        //        if (_RowShadowEffect.Color != value)
        //        {
        //            _RowShadowEffect.Color = value;
        //            OnPropertyChanged("RowShadowEffect");
        //        }
        //    }
        //}

        public double HeaderShadowEffectShadowDepth
        {
            get { return _HeaderShadowEffect.ShadowDepth; }
            set
            {
                if (_HeaderShadowEffect.ShadowDepth != value)
                {
                    _HeaderShadowEffect.ShadowDepth = value;
                    OnPropertyChanged("HeaderShadowEffect");
                }
            }
        }

        public double HeaderShadowEffectDirection
        {
            get { return _HeaderShadowEffect.Direction; }
            set
            {
                if (_HeaderShadowEffect.Direction != value)
                {
                    _HeaderShadowEffect.Direction = value;
                    OnPropertyChanged("HeaderShadowEffect");
                }
            }
        }

        public int AlternationCount
        {
            get { return _AlternationCount; }
            set
            {
                if (_AlternationCount != value)
                {
                    _AlternationCount = value;
                    OnPropertyChanged("AlternationCount");
                }
            }
        }

        public Brush AlternatingRowBackground
        {
            get { return _AlternatingRowBackground; }
            set
            {
                if (_AlternatingRowBackground != value)
                {
                    _AlternatingRowBackground = value;
                    OnPropertyChanged("AlternatingRowBackground");
                }
            }
        }

        public int MaxRows
        {
            get { return _MaxRows; }
            set
            {
                if (_MaxRows != value)
                {
                    _MaxRows = value;
                    OnPropertyChanged("MaxRows");
                }
            }
        }

        public double RowHeight
        {
            get { return _RowHeight; }
            set
            {
                if (_RowHeight != value)
                {
                    _RowHeight = value;
                    OnPropertyChanged("RowHeight");
                }
            }
        }

        public int RefreshTime
        {
            get { return _RefreshTime; }
            set
            {
                if (_RefreshTime != value)
                {
                    _RefreshTime = value;
                    OnPropertyChanged("RefreshTime");
                }
            }
        }

        public ObservableCollection<DataSourceModel> Sources
        {
            get { return _Sources; }
            set
            {
                if (_Sources != value)
                {
                    _Sources = value;
                    OnPropertyChanged("Sources");
                }
            }
        }

        public DataSourceModel SelectedSource
        {
            get { return _SelectedSource; }
            set
            {
                if (_SelectedSource != value)
                {
                    _SelectedSource = value;
                    OnPropertyChanged("SelectedSource");
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

