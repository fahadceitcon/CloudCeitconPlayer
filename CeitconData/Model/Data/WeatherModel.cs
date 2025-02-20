using Ceitcon_Data.Utilities;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Ceitcon_Data.Model.Data
{
    public class WeatherModel : INotifyPropertyChanged
    {
        private string _Id;
        private bool _TitleIsVisible;
        private double _TitleSize;
        private Brush _TitleForeground;
        private Brush _TitleBackground;
        private Brush _TitleBorderBrush;
        private Thickness _TitleBorderThickness;
        private FontFamily _TitleFontFamily;
        private FontWeight _TitleFontWeight;
        private FontStyle _TitleFontStyle;
        private CornerRadius _TitleCornerRadius;
        private string _TitleText;

        private bool _DayTextIsVisible;
        private double _DayTextSize;
        private Brush _DayTextForeground;
        private Brush _DayTextBackground;
        private Brush _DayTextBorderBrush;
        private Thickness _DayTextBorderThickness;
        private FontFamily _DayTextFontFamily;
        private FontWeight _DayTextFontWeight;
        private FontStyle _DayTextFontStyle;
        private CornerRadius _DayTextCornerRadius;

        private bool _WeatherTextIsVisible;
        private double _WeatherTextSize;
        private Brush _WeatherTextForeground;
        private Brush _WeatherTextBackground;
        private Brush _WeatherTextBorderBrush;
        private Thickness _WeatherTextBorderThickness;
        private FontFamily _WeatherTextFontFamily;
        private FontWeight _WeatherTextFontWeight;
        private FontStyle _WeatherTextFontStyle;
        private CornerRadius _WeatherTextCornerRadius;

        private bool _HeightTextIsVisible;
        private double _HeightTextSize;
        private Brush _HeightTextForeground;
        private Brush _HeightTextBackground;
        private Brush _HeightTextBorderBrush;
        private Thickness _HeightTextBorderThickness;
        private FontFamily _HeightTextFontFamily;
        private FontWeight _HeightTextFontWeight;
        private FontStyle _HeightTextFontStyle;
        private CornerRadius _HeightTextCornerRadius;

        private bool _HeightValueIsVisible;
        private double _HeightValueSize;
        private Brush _HeightValueForeground;
        private Brush _HeightValueBackground;
        private Brush _HeightValueBorderBrush;
        private Thickness _HeightValueBorderThickness;
        private FontFamily _HeightValueFontFamily;
        private FontWeight _HeightValueFontWeight;
        private FontStyle _HeightValueFontStyle;
        private CornerRadius _HeightValueCornerRadius;

        private bool _LowTextIsVisible;
        private double _LowTextSize;
        private Brush _LowTextForeground;
        private Brush _LowTextBackground;
        private Brush _LowTextBorderBrush;
        private Thickness _LowTextBorderThickness;
        private FontFamily _LowTextFontFamily;
        private FontWeight _LowTextFontWeight;
        private FontStyle _LowTextFontStyle;
        private CornerRadius _LowTextCornerRadius;

        private bool _LowValueIsVisible;
        private double _LowValueSize;
        private Brush _LowValueForeground;
        private Brush _LowValueBackground;
        private Brush _LowValueBorderBrush;
        private Thickness _LowValueBorderThickness;
        private FontFamily _LowValueFontFamily;
        private FontWeight _LowValueFontWeight;
        private FontStyle _LowValueFontStyle;
        private CornerRadius _LowValueCornerRadius;

        public WeatherModel()
        {
            _Id = Guid.NewGuid().ToString();
            _TitleIsVisible = true;
            _TitleSize = 35;
            _TitleForeground = Brushes.White;
            _TitleBackground = Brushes.Transparent;
            _TitleBorderBrush = Brushes.Transparent;
            _TitleBorderThickness = new Thickness(0);
            _TitleFontFamily = new FontFamily("Arial");
            _TitleFontWeight = FontWeights.Normal;
            _TitleFontStyle = FontStyles.Normal;
            _TitleText = String.Empty;

            _DayTextIsVisible = true;
            _DayTextSize = 20;
            _DayTextForeground = Brushes.White;
            _DayTextBackground = Brushes.Transparent;
            _DayTextBorderBrush = Brushes.Transparent;
            _DayTextBorderThickness = new Thickness(0);
            _DayTextFontFamily = new FontFamily("Arial");
            _DayTextFontWeight = FontWeights.Normal;
            _DayTextFontStyle = FontStyles.Normal;

            _WeatherTextIsVisible = true;
            _WeatherTextSize = 20;
            _WeatherTextForeground = Brushes.White;
            _WeatherTextBackground = Brushes.Transparent;
            _WeatherTextBorderBrush = Brushes.Transparent;
            _WeatherTextBorderThickness = new Thickness(0);
            _WeatherTextFontFamily = new FontFamily("Arial");
            _WeatherTextFontWeight = FontWeights.Normal;
            _WeatherTextFontStyle = FontStyles.Normal;

            _HeightTextIsVisible = true;
            _HeightTextSize = 20;
            _HeightTextForeground = Brushes.White;
            _HeightTextBackground = Brushes.Transparent;
            _HeightTextBorderBrush = Brushes.Transparent;
            _HeightTextBorderThickness = new Thickness(0);
            _HeightTextFontFamily = new FontFamily("Arial");
            _HeightTextFontWeight = FontWeights.Normal;
            _HeightTextFontStyle = FontStyles.Normal;

            _HeightValueIsVisible = true;
            _HeightValueSize = 34;
            _HeightValueForeground = Brushes.White;
            _HeightValueBackground = Brushes.Transparent;
            _HeightValueBorderBrush = Brushes.Transparent;
            _HeightValueBorderThickness = new Thickness(0);
            _HeightValueFontFamily = new FontFamily("Arial");
            _HeightValueFontWeight = FontWeights.Normal;
            _HeightValueFontStyle = FontStyles.Normal;

            _LowTextIsVisible = true;
            _LowTextSize = 20;
            _LowTextForeground = Brushes.White;
            _LowTextBackground = Brushes.Transparent;
            _LowTextBorderBrush = Brushes.Transparent;
            _LowTextBorderThickness = new Thickness(0);
            _LowTextFontFamily = new FontFamily("Arial");
            _LowTextFontWeight = FontWeights.Normal;
            _LowTextFontStyle = FontStyles.Normal;

            _LowValueIsVisible = true;
            _LowValueSize = 34;
            _LowValueForeground = Brushes.White;
            _LowValueBackground = Brushes.Transparent;
            _LowValueBorderBrush = Brushes.Transparent;
            _LowValueBorderThickness = new Thickness(0);
            _LowValueFontFamily = new FontFamily("Arial");
            _LowValueFontWeight = FontWeights.Normal;
            _LowValueFontStyle = FontStyles.Normal;
        }

        public WeatherModel(WeatherModel copy, bool fullCopy = false)
        {
            if (fullCopy) { _Id = copy.Id; } else { Guid.NewGuid().ToString(); }
            _TitleIsVisible = copy.TitleIsVisible;
            _TitleSize = copy.TitleSize;
            _TitleForeground = copy.TitleForeground;
            _TitleBackground = copy.TitleBackground;
            _TitleBorderBrush = copy.TitleBorderBrush;
            _TitleBorderThickness = copy.TitleBorderThickness;
            _TitleFontFamily = copy.TitleFontFamily;
            _TitleFontWeight = copy.TitleFontWeight;
            _TitleFontStyle = copy.TitleFontStyle;
            _TitleText = copy.TitleText;

            _DayTextIsVisible = copy.DayTextIsVisible;
            _DayTextSize = copy.DayTextSize;
            _DayTextForeground = copy.DayTextForeground;
            _DayTextBackground = copy.DayTextBackground;
            _DayTextBorderBrush = copy.DayTextBorderBrush;
            _DayTextBorderThickness = copy.DayTextBorderThickness;
            _DayTextFontFamily = copy.DayTextFontFamily;
            _DayTextFontWeight = copy.DayTextFontWeight;
            _DayTextFontStyle = copy.DayTextFontStyle;

            _WeatherTextIsVisible = copy.WeatherTextIsVisible;
            _WeatherTextSize = copy.WeatherTextSize;
            _WeatherTextForeground = copy.WeatherTextForeground;
            _WeatherTextBackground = copy.WeatherTextBackground;
            _WeatherTextBorderBrush = copy.WeatherTextBorderBrush;
            _WeatherTextBorderThickness = copy.WeatherTextBorderThickness;
            _WeatherTextFontFamily = copy.WeatherTextFontFamily;
            _WeatherTextFontWeight = copy.WeatherTextFontWeight;
            _WeatherTextFontStyle = copy.WeatherTextFontStyle;

            _HeightTextIsVisible = copy.HeightTextIsVisible;
            _HeightTextSize = copy.HeightTextSize;
            _HeightTextForeground = copy.HeightTextForeground;
            _HeightTextBackground = copy.HeightTextBackground;
            _HeightTextBorderBrush = copy.HeightTextBorderBrush;
            _HeightTextBorderThickness = copy.HeightTextBorderThickness;
            _HeightTextFontFamily = copy.HeightTextFontFamily;
            _HeightTextFontWeight = copy.HeightTextFontWeight;
            _HeightTextFontStyle = copy.HeightTextFontStyle;

            _HeightValueIsVisible = copy.HeightValueIsVisible;
            _HeightValueSize = copy.HeightValueSize;
            _HeightValueForeground = copy.HeightValueForeground;
            _HeightValueBackground = copy.HeightValueBackground;
            _HeightValueBorderBrush = copy.HeightValueBorderBrush;
            _HeightValueBorderThickness = copy.HeightValueBorderThickness;
            _HeightValueFontFamily = copy.HeightValueFontFamily;
            _HeightValueFontWeight = copy.HeightValueFontWeight;
            _HeightValueFontStyle = copy.HeightValueFontStyle;

            _LowTextIsVisible = copy.LowTextIsVisible;
            _LowTextSize = copy.LowTextSize;
            _LowTextForeground = copy.LowTextForeground;
            _LowTextBackground = copy.LowTextBackground;
            _LowTextBorderBrush = copy.LowTextBorderBrush;
            _LowTextBorderThickness = copy.LowTextBorderThickness;
            _LowTextFontFamily = copy.LowTextFontFamily;
            _LowTextFontWeight = copy.LowTextFontWeight;
            _LowTextFontStyle = copy.LowTextFontStyle;

            _LowValueIsVisible = copy.LowValueIsVisible;
            _LowValueSize = copy.LowValueSize;
            _LowValueForeground = copy.LowValueForeground;
            _LowValueBackground = copy.LowValueBackground;
            _LowValueBorderBrush = copy.LowValueBorderBrush;
            _LowValueBorderThickness = copy.LowValueBorderThickness;
            _LowValueFontFamily = copy.LowValueFontFamily;
            _LowValueFontWeight = copy.LowValueFontWeight;
            _LowValueFontStyle = copy.LowValueFontStyle;
        }

        public WeatherModel Save()
        {
            return new WeatherModel(this, true);
        }

        public void Restore(WeatherModel copyObj)
        {
            var copy = copyObj as WeatherModel;
            Memento.Enable = false;
            TitleIsVisible = copy.TitleIsVisible;
            TitleSize = copy.TitleSize;
            TitleForeground = copy.TitleForeground;
            TitleBackground = copy.TitleBackground;
            TitleBorderBrush = copy.TitleBorderBrush;
            TitleBorderThickness = copy.TitleBorderThickness;
            TitleFontFamily = copy.TitleFontFamily;
            TitleFontWeight = copy.TitleFontWeight;
            TitleFontStyle = copy.TitleFontStyle;
            TitleText = copy.TitleText;

            DayTextIsVisible = copy.DayTextIsVisible;
            DayTextSize = copy.DayTextSize;
            DayTextForeground = copy.DayTextForeground;
            DayTextBackground = copy.DayTextBackground;
            DayTextBorderBrush = copy.DayTextBorderBrush;
            DayTextBorderThickness = copy.DayTextBorderThickness;
            DayTextFontFamily = copy.DayTextFontFamily;
            DayTextFontWeight = copy.DayTextFontWeight;
            DayTextFontStyle = copy.DayTextFontStyle;

            WeatherTextIsVisible = copy.WeatherTextIsVisible;
            WeatherTextSize = copy.WeatherTextSize;
            WeatherTextForeground = copy.WeatherTextForeground;
            WeatherTextBackground = copy.WeatherTextBackground;
            WeatherTextBorderBrush = copy.WeatherTextBorderBrush;
            WeatherTextBorderThickness = copy.WeatherTextBorderThickness;
            WeatherTextFontFamily = copy.WeatherTextFontFamily;
            WeatherTextFontWeight = copy.WeatherTextFontWeight;
            WeatherTextFontStyle = copy.WeatherTextFontStyle;

            HeightTextIsVisible = copy.HeightTextIsVisible;
            HeightTextSize = copy.HeightTextSize;
            HeightTextForeground = copy.HeightTextForeground;
            HeightTextBackground = copy.HeightTextBackground;
            HeightTextBorderBrush = copy.HeightTextBorderBrush;
            HeightTextBorderThickness = copy.HeightTextBorderThickness;
            HeightTextFontFamily = copy.HeightTextFontFamily;
            HeightTextFontWeight = copy.HeightTextFontWeight;
            HeightTextFontStyle = copy.HeightTextFontStyle;

            HeightValueIsVisible = copy.HeightValueIsVisible;
            HeightValueSize = copy.HeightValueSize;
            HeightValueForeground = copy.HeightValueForeground;
            HeightValueBackground = copy.HeightValueBackground;
            HeightValueBorderBrush = copy.HeightValueBorderBrush;
            HeightValueBorderThickness = copy.HeightValueBorderThickness;
            HeightValueFontFamily = copy.HeightValueFontFamily;
            HeightValueFontWeight = copy.HeightValueFontWeight;
            HeightValueFontStyle = copy.HeightValueFontStyle;

            LowTextIsVisible = copy.LowTextIsVisible;
            LowTextSize = copy.LowTextSize;
            LowTextForeground = copy.LowTextForeground;
            LowTextBackground = copy.LowTextBackground;
            LowTextBorderBrush = copy.LowTextBorderBrush;
            LowTextBorderThickness = copy.LowTextBorderThickness;
            LowTextFontFamily = copy.LowTextFontFamily;
            LowTextFontWeight = copy.LowTextFontWeight;
            LowTextFontStyle = copy.LowTextFontStyle;

            LowValueIsVisible = copy.LowValueIsVisible;
            LowValueSize = copy.LowValueSize;
            LowValueForeground = copy.LowValueForeground;
            LowValueBackground = copy.LowValueBackground;
            LowValueBorderBrush = copy.LowValueBorderBrush;
            LowValueBorderThickness = copy.LowValueBorderThickness;
            LowValueFontFamily = copy.LowValueFontFamily;
            LowValueFontWeight = copy.LowValueFontWeight;
            LowValueFontStyle = copy.LowValueFontStyle;
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

        #region Title
        public bool TitleIsVisible
        {
            get { return _TitleIsVisible; }
            set
            {
                if (_TitleIsVisible != value)
                {
                    _TitleIsVisible = value;
                    OnPropertyChanged("TitleIsVisible");
                }
            }
        }


        public double TitleSize
        {
            get { return _TitleSize; }
            set
            {
                if (_TitleSize != value)
                {
                    _TitleSize = value;
                    OnPropertyChanged("TitleSize");
                }
            }
        }

        public Brush TitleBackground
        {
            get { return _TitleBackground; }
            set
            {
                if (_TitleBackground != value)
                {
                    _TitleBackground = value;
                    OnPropertyChanged("TitleBackground");
                }
            }
        }

        public Brush TitleForeground
        {
            get { return _TitleForeground; }
            set
            {
                if (_TitleForeground != value)
                {
                    _TitleForeground = value;
                    OnPropertyChanged("TitleForeground");
                }
            }
        }

        public Brush TitleBorderBrush
        {
            get { return _TitleBorderBrush; }
            set
            {
                if (_TitleBorderBrush != value)
                {
                    _TitleBorderBrush = value;
                    OnPropertyChanged("TitleBorderBrush");
                }
            }
        }

        public Thickness TitleBorderThickness
        {
            get { return _TitleBorderThickness; }
            set
            {
                if (_TitleBorderThickness != value)
                {
                    _TitleBorderThickness = value;
                    OnPropertyChanged("TitleBorderThickness");
                }
            }
        }

        public double TitleBorderThicknessLeft
        {
            get { return _TitleBorderThickness.Left; }
            set
            {
                _TitleBorderThickness.Left = value;
                OnPropertyChanged("TitleBorderThickness");
            }
        }

        public double TitleBorderThicknessRight
        {
            get { return _TitleBorderThickness.Right; }
            set
            {
                _TitleBorderThickness.Right = value;
                OnPropertyChanged("TitleBorderThickness");
            }
        }

        public double TitleBorderThicknessTop
        {
            get { return _TitleBorderThickness.Top; }
            set
            {
                _TitleBorderThickness.Top = value;
                OnPropertyChanged("TitleBorderThickness");
            }
        }

        public double TitleBorderThicknessBottom
        {
            get { return _TitleBorderThickness.Bottom; }
            set
            {
                _TitleBorderThickness.Bottom = value;
                OnPropertyChanged("TitleBorderThickness");
            }
        }


        public FontFamily TitleFontFamily
        {
            get { return _TitleFontFamily; }
            set
            {
                if (_TitleFontFamily != value)
                {
                    _TitleFontFamily = value;
                    OnPropertyChanged("TitleFontFamily");
                }
            }
        }

        public FontWeight TitleFontWeight
        {
            get { return _TitleFontWeight; }
            set
            {
                if (_TitleFontWeight != value)
                {
                    _TitleFontWeight = value;
                    OnPropertyChanged("TitleFontWeight");
                }
            }
        }

        public FontStyle TitleFontStyle
        {
            get { return _TitleFontStyle; }
            set
            {
                if (_TitleFontStyle != value)
                {
                    _TitleFontStyle = value;
                    OnPropertyChanged("TitleFontStyle");
                }
            }
        }

        public CornerRadius TitleCornerRadius
        {
            get { return _TitleCornerRadius; }
            set
            {
                if (_TitleCornerRadius != value)
                {
                    Memento.Push(Save());
                    _TitleCornerRadius = value;
                    OnPropertyChanged("TitleCornerRadius");
                    OnPropertyChanged("TitleCornerRadiusTopLeft");
                    OnPropertyChanged("TitleCornerRadiusTopRight");
                    OnPropertyChanged("TitleCornerRadiusBottomRight");
                    OnPropertyChanged("TitleCornerRadiusBottomLeft");
                }
            }
        }

        public double TitleCornerRadiusTopLeft
        {
            get { return _TitleCornerRadius.TopLeft; }
            set
            {
                if (_TitleCornerRadius.TopLeft != value)
                {
                    Memento.Push(Save());
                    _TitleCornerRadius.TopLeft = value;
                    OnPropertyChanged("TitleCornerRadiusTopLeft");
                    OnPropertyChanged("TitleCornerRadius");
                }
            }
        }

        public double TitleCornerRadiusTopRight
        {
            get { return _TitleCornerRadius.TopRight; }
            set
            {
                if (_TitleCornerRadius.TopRight != value)
                {
                    Memento.Push(Save());
                    _TitleCornerRadius.TopRight = value;
                    OnPropertyChanged("TitleCornerRadiusTopRight");
                    OnPropertyChanged("TitleCornerRadius");
                }
            }
        }

        public double TitleCornerRadiusBottomRight
        {
            get { return _TitleCornerRadius.BottomRight; }
            set
            {
                if (_TitleCornerRadius.BottomRight != value)
                {
                    Memento.Push(Save());
                    _TitleCornerRadius.BottomRight = value;
                    OnPropertyChanged("TitleCornerRadiusBottomRight");
                    OnPropertyChanged("TitleCornerRadius");
                }
            }
        }

        public double TitleCornerRadiusBottomLeft
        {
            get { return _TitleCornerRadius.BottomLeft; }
            set
            {
                if (_TitleCornerRadius.BottomLeft != value)
                {
                    Memento.Push(Save());
                    _TitleCornerRadius.BottomLeft = value;
                    OnPropertyChanged("TitleCornerRadiusBottomLeft");
                    OnPropertyChanged("TitleCornerRadius");
                }
            }
        }

        public string TitleText
        {
            get { return _TitleText; }
            set
            {
                if (_TitleText != value)
                {
                    _TitleText = value;
                    OnPropertyChanged("TitleText");
                }
            }
        }

        #endregion

        #region DayText
        public bool DayTextIsVisible
        {
            get { return _DayTextIsVisible; }
            set
            {
                if (_DayTextIsVisible != value)
                {
                    _DayTextIsVisible = value;
                    OnPropertyChanged("DayTextIsVisible");
                }
            }
        }

        public double DayTextSize
        {
            get { return _DayTextSize; }
            set
            {
                if (_DayTextSize != value)
                {
                    _DayTextSize = value;
                    OnPropertyChanged("DayTextSize");
                }
            }
        }

        public Brush DayTextBackground
        {
            get { return _DayTextBackground; }
            set
            {
                if (_DayTextBackground != value)
                {
                    _DayTextBackground = value;
                    OnPropertyChanged("DayTextBackground");
                }
            }
        }

        public Brush DayTextForeground
        {
            get { return _DayTextForeground; }
            set
            {
                if (_DayTextForeground != value)
                {
                    _DayTextForeground = value;
                    OnPropertyChanged("DayTextForeground");
                }
            }
        }

        public Brush DayTextBorderBrush
        {
            get { return _DayTextBorderBrush; }
            set
            {
                if (_DayTextBorderBrush != value)
                {
                    _DayTextBorderBrush = value;
                    OnPropertyChanged("DayTextBorderBrush");
                }
            }
        }

        public Thickness DayTextBorderThickness
        {
            get { return _DayTextBorderThickness; }
            set
            {
                if (_DayTextBorderThickness != value)
                {
                    _DayTextBorderThickness = value;
                    OnPropertyChanged("DayTextBorderThickness");
                }
            }
        }

        public double DayTextBorderThicknessLeft
        {
            get { return _DayTextBorderThickness.Left; }
            set
            {
                _DayTextBorderThickness.Left = value;
                OnPropertyChanged("DayTextBorderThickness");
            }
        }

        public double DayTextBorderThicknessRight
        {
            get { return _DayTextBorderThickness.Right; }
            set
            {
                _DayTextBorderThickness.Right = value;
                OnPropertyChanged("DayTextBorderThickness");
            }
        }

        public double DayTextBorderThicknessTop
        {
            get { return _DayTextBorderThickness.Top; }
            set
            {
                _DayTextBorderThickness.Top = value;
                OnPropertyChanged("DayTextBorderThickness");
            }
        }

        public double DayTextBorderThicknessBottom
        {
            get { return _DayTextBorderThickness.Bottom; }
            set
            {
                _DayTextBorderThickness.Bottom = value;
                OnPropertyChanged("DayTextBorderThickness");
            }
        }


        public FontFamily DayTextFontFamily
        {
            get { return _DayTextFontFamily; }
            set
            {
                if (_DayTextFontFamily != value)
                {
                    _DayTextFontFamily = value;
                    OnPropertyChanged("DayTextFontFamily");
                }
            }
        }

        public FontWeight DayTextFontWeight
        {
            get { return _DayTextFontWeight; }
            set
            {
                if (_DayTextFontWeight != value)
                {
                    _DayTextFontWeight = value;
                    OnPropertyChanged("DayTextFontWeight");
                }
            }
        }

        public FontStyle DayTextFontStyle
        {
            get { return _DayTextFontStyle; }
            set
            {
                if (_DayTextFontStyle != value)
                {
                    _DayTextFontStyle = value;
                    OnPropertyChanged("DayTextFontStyle");
                }
            }
        }

        public CornerRadius DayTextCornerRadius
        {
            get { return _DayTextCornerRadius; }
            set
            {
                if (_DayTextCornerRadius != value)
                {
                    Memento.Push(Save());
                    _DayTextCornerRadius = value;
                    OnPropertyChanged("DayTextCornerRadius");
                    OnPropertyChanged("DayTextCornerRadiusTopLeft");
                    OnPropertyChanged("DayTextCornerRadiusTopRight");
                    OnPropertyChanged("DayTextCornerRadiusBottomRight");
                    OnPropertyChanged("DayTextCornerRadiusBottomLeft");
                }
            }
        }

        public double DayTextCornerRadiusTopLeft
        {
            get { return _DayTextCornerRadius.TopLeft; }
            set
            {
                if (_DayTextCornerRadius.TopLeft != value)
                {
                    Memento.Push(Save());
                    _DayTextCornerRadius.TopLeft = value;
                    OnPropertyChanged("DayTextCornerRadiusTopLeft");
                    OnPropertyChanged("DayTextCornerRadius");
                }
            }
        }

        public double DayTextCornerRadiusTopRight
        {
            get { return _DayTextCornerRadius.TopRight; }
            set
            {
                if (_DayTextCornerRadius.TopRight != value)
                {
                    Memento.Push(Save());
                    _DayTextCornerRadius.TopRight = value;
                    OnPropertyChanged("DayTextCornerRadiusTopRight");
                    OnPropertyChanged("DayTextCornerRadius");
                }
            }
        }

        public double DayTextCornerRadiusBottomRight
        {
            get { return _DayTextCornerRadius.BottomRight; }
            set
            {
                if (_DayTextCornerRadius.BottomRight != value)
                {
                    Memento.Push(Save());
                    _DayTextCornerRadius.BottomRight = value;
                    OnPropertyChanged("DayTextCornerRadiusBottomRight");
                    OnPropertyChanged("DayTextCornerRadius");
                }
            }
        }

        public double DayTextCornerRadiusBottomLeft
        {
            get { return _DayTextCornerRadius.BottomLeft; }
            set
            {
                if (_DayTextCornerRadius.BottomLeft != value)
                {
                    Memento.Push(Save());
                    _DayTextCornerRadius.BottomLeft = value;
                    OnPropertyChanged("DayTextCornerRadiusBottomLeft");
                    OnPropertyChanged("DayTextCornerRadius");
                }
            }
        }
        #endregion

        #region WeatherText
        public bool WeatherTextIsVisible
        {
            get { return _WeatherTextIsVisible; }
            set
            {
                if (_WeatherTextIsVisible != value)
                {
                    _WeatherTextIsVisible = value;
                    OnPropertyChanged("WeatherTextIsVisible");
                }
            }
        }

        public double WeatherTextSize
        {
            get { return _WeatherTextSize; }
            set
            {
                if (_WeatherTextSize != value)
                {
                    _WeatherTextSize = value;
                    OnPropertyChanged("WeatherTextSize");
                }
            }
        }

        public Brush WeatherTextBackground
        {
            get { return _WeatherTextBackground; }
            set
            {
                if (_WeatherTextBackground != value)
                {
                    _WeatherTextBackground = value;
                    OnPropertyChanged("WeatherTextBackground");
                }
            }
        }

        public Brush WeatherTextForeground
        {
            get { return _WeatherTextForeground; }
            set
            {
                if (_WeatherTextForeground != value)
                {
                    _WeatherTextForeground = value;
                    OnPropertyChanged("WeatherTextForeground");
                }
            }
        }

        public Brush WeatherTextBorderBrush
        {
            get { return _WeatherTextBorderBrush; }
            set
            {
                if (_WeatherTextBorderBrush != value)
                {
                    _WeatherTextBorderBrush = value;
                    OnPropertyChanged("WeatherTextBorderBrush");
                }
            }
        }

        public Thickness WeatherTextBorderThickness
        {
            get { return _WeatherTextBorderThickness; }
            set
            {
                if (_WeatherTextBorderThickness != value)
                {
                    _WeatherTextBorderThickness = value;
                    OnPropertyChanged("WeatherTextBorderThickness");
                }
            }
        }

        public double WeatherTextBorderThicknessLeft
        {
            get { return _WeatherTextBorderThickness.Left; }
            set
            {
                _WeatherTextBorderThickness.Left = value;
                OnPropertyChanged("WeatherTextBorderThickness");
            }
        }

        public double WeatherTextBorderThicknessRight
        {
            get { return _WeatherTextBorderThickness.Right; }
            set
            {
                _WeatherTextBorderThickness.Right = value;
                OnPropertyChanged("WeatherTextBorderThickness");
            }
        }

        public double WeatherTextBorderThicknessTop
        {
            get { return _WeatherTextBorderThickness.Top; }
            set
            {
                _WeatherTextBorderThickness.Top = value;
                OnPropertyChanged("WeatherTextBorderThickness");
            }
        }

        public double WeatherTextBorderThicknessBottom
        {
            get { return _WeatherTextBorderThickness.Bottom; }
            set
            {
                _WeatherTextBorderThickness.Bottom = value;
                OnPropertyChanged("WeatherTextBorderThickness");
            }
        }


        public FontFamily WeatherTextFontFamily
        {
            get { return _WeatherTextFontFamily; }
            set
            {
                if (_WeatherTextFontFamily != value)
                {
                    _WeatherTextFontFamily = value;
                    OnPropertyChanged("WeatherTextFontFamily");
                }
            }
        }

        public FontWeight WeatherTextFontWeight
        {
            get { return _WeatherTextFontWeight; }
            set
            {
                if (_WeatherTextFontWeight != value)
                {
                    _WeatherTextFontWeight = value;
                    OnPropertyChanged("WeatherTextFontWeight");
                }
            }
        }

        public FontStyle WeatherTextFontStyle
        {
            get { return _WeatherTextFontStyle; }
            set
            {
                if (_WeatherTextFontStyle != value)
                {
                    _WeatherTextFontStyle = value;
                    OnPropertyChanged("WeatherTextFontStyle");
                }
            }
        }

        public CornerRadius WeatherTextCornerRadius
        {
            get { return _WeatherTextCornerRadius; }
            set
            {
                if (_WeatherTextCornerRadius != value)
                {
                    Memento.Push(Save());
                    _WeatherTextCornerRadius = value;
                    OnPropertyChanged("WeatherTextCornerRadius");
                    OnPropertyChanged("WeatherTextCornerRadiusTopLeft");
                    OnPropertyChanged("WeatherTextCornerRadiusTopRight");
                    OnPropertyChanged("WeatherTextCornerRadiusBottomRight");
                    OnPropertyChanged("WeatherTextCornerRadiusBottomLeft");
                }
            }
        }

        public double WeatherTextCornerRadiusTopLeft
        {
            get { return _WeatherTextCornerRadius.TopLeft; }
            set
            {
                if (_WeatherTextCornerRadius.TopLeft != value)
                {
                    Memento.Push(Save());
                    _WeatherTextCornerRadius.TopLeft = value;
                    OnPropertyChanged("WeatherTextCornerRadiusTopLeft");
                    OnPropertyChanged("WeatherTextCornerRadius");
                }
            }
        }

        public double WeatherTextCornerRadiusTopRight
        {
            get { return _WeatherTextCornerRadius.TopRight; }
            set
            {
                if (_WeatherTextCornerRadius.TopRight != value)
                {
                    Memento.Push(Save());
                    _WeatherTextCornerRadius.TopRight = value;
                    OnPropertyChanged("WeatherTextCornerRadiusTopRight");
                    OnPropertyChanged("WeatherTextCornerRadius");
                }
            }
        }

        public double WeatherTextCornerRadiusBottomRight
        {
            get { return _WeatherTextCornerRadius.BottomRight; }
            set
            {
                if (_WeatherTextCornerRadius.BottomRight != value)
                {
                    Memento.Push(Save());
                    _WeatherTextCornerRadius.BottomRight = value;
                    OnPropertyChanged("WeatherTextCornerRadiusBottomRight");
                    OnPropertyChanged("WeatherTextCornerRadius");
                }
            }
        }

        public double WeatherTextCornerRadiusBottomLeft
        {
            get { return _WeatherTextCornerRadius.BottomLeft; }
            set
            {
                if (_WeatherTextCornerRadius.BottomLeft != value)
                {
                    Memento.Push(Save());
                    _WeatherTextCornerRadius.BottomLeft = value;
                    OnPropertyChanged("WeatherTextCornerRadiusBottomLeft");
                    OnPropertyChanged("WeatherTextCornerRadius");
                }
            }
        }

        #endregion

        #region HeightText
        public bool HeightTextIsVisible
        {
            get { return _HeightTextIsVisible; }
            set
            {
                if (_HeightTextIsVisible != value)
                {
                    _HeightTextIsVisible = value;
                    OnPropertyChanged("HeightTextIsVisible");
                }
            }
        }

        public double HeightTextSize
        {
            get { return _HeightTextSize; }
            set
            {
                if (_HeightTextSize != value)
                {
                    _HeightTextSize = value;
                    OnPropertyChanged("HeightTextSize");
                }
            }
        }

        public Brush HeightTextBackground
        {
            get { return _HeightTextBackground; }
            set
            {
                if (_HeightTextBackground != value)
                {
                    _HeightTextBackground = value;
                    OnPropertyChanged("HeightTextBackground");
                }
            }
        }

        public Brush HeightTextForeground
        {
            get { return _HeightTextForeground; }
            set
            {
                if (_HeightTextForeground != value)
                {
                    _HeightTextForeground = value;
                    OnPropertyChanged("HeightTextForeground");
                }
            }
        }

        public Brush HeightTextBorderBrush
        {
            get { return _HeightTextBorderBrush; }
            set
            {
                if (_HeightTextBorderBrush != value)
                {
                    _HeightTextBorderBrush = value;
                    OnPropertyChanged("HeightTextBorderBrush");
                }
            }
        }

        public Thickness HeightTextBorderThickness
        {
            get { return _HeightTextBorderThickness; }
            set
            {
                if (_HeightTextBorderThickness != value)
                {
                    _HeightTextBorderThickness = value;
                    OnPropertyChanged("HeightTextBorderThickness");
                }
            }
        }

        public double HeightTextBorderThicknessLeft
        {
            get { return _HeightTextBorderThickness.Left; }
            set
            {
                _HeightTextBorderThickness.Left = value;
                OnPropertyChanged("HeightTextBorderThickness");
            }
        }

        public double HeightTextBorderThicknessRight
        {
            get { return _HeightTextBorderThickness.Right; }
            set
            {
                _HeightTextBorderThickness.Right = value;
                OnPropertyChanged("HeightTextBorderThickness");
            }
        }

        public double HeightTextBorderThicknessTop
        {
            get { return _HeightTextBorderThickness.Top; }
            set
            {
                _HeightTextBorderThickness.Top = value;
                OnPropertyChanged("HeightTextBorderThickness");
            }
        }

        public double HeightTextBorderThicknessBottom
        {
            get { return _HeightTextBorderThickness.Bottom; }
            set
            {
                _HeightTextBorderThickness.Bottom = value;
                OnPropertyChanged("HeightTextBorderThickness");
            }
        }


        public FontFamily HeightTextFontFamily
        {
            get { return _HeightTextFontFamily; }
            set
            {
                if (_HeightTextFontFamily != value)
                {
                    _HeightTextFontFamily = value;
                    OnPropertyChanged("HeightTextFontFamily");
                }
            }
        }

        public FontWeight HeightTextFontWeight
        {
            get { return _HeightTextFontWeight; }
            set
            {
                if (_HeightTextFontWeight != value)
                {
                    _HeightTextFontWeight = value;
                    OnPropertyChanged("HeightTextFontWeight");
                }
            }
        }

        public FontStyle HeightTextFontStyle
        {
            get { return _HeightTextFontStyle; }
            set
            {
                if (_HeightTextFontStyle != value)
                {
                    _HeightTextFontStyle = value;
                    OnPropertyChanged("HeightTextFontStyle");
                }
            }
        }

        public CornerRadius HeightTextCornerRadius
        {
            get { return _HeightTextCornerRadius; }
            set
            {
                if (_HeightTextCornerRadius != value)
                {
                    Memento.Push(Save());
                    _HeightTextCornerRadius = value;
                    OnPropertyChanged("HeightTextCornerRadius");
                    OnPropertyChanged("HeightTextCornerRadiusTopLeft");
                    OnPropertyChanged("HeightTextCornerRadiusTopRight");
                    OnPropertyChanged("HeightTextCornerRadiusBottomRight");
                    OnPropertyChanged("HeightTextCornerRadiusBottomLeft");
                }
            }
        }

        public double HeightTextCornerRadiusTopLeft
        {
            get { return _HeightTextCornerRadius.TopLeft; }
            set
            {
                if (_HeightTextCornerRadius.TopLeft != value)
                {
                    Memento.Push(Save());
                    _HeightTextCornerRadius.TopLeft = value;
                    OnPropertyChanged("HeightTextCornerRadiusTopLeft");
                    OnPropertyChanged("HeightTextCornerRadius");
                }
            }
        }

        public double HeightTextCornerRadiusTopRight
        {
            get { return _HeightTextCornerRadius.TopRight; }
            set
            {
                if (_HeightTextCornerRadius.TopRight != value)
                {
                    Memento.Push(Save());
                    _HeightTextCornerRadius.TopRight = value;
                    OnPropertyChanged("HeightTextCornerRadiusTopRight");
                    OnPropertyChanged("HeightTextCornerRadius");
                }
            }
        }

        public double HeightTextCornerRadiusBottomRight
        {
            get { return _HeightTextCornerRadius.BottomRight; }
            set
            {
                if (_HeightTextCornerRadius.BottomRight != value)
                {
                    Memento.Push(Save());
                    _HeightTextCornerRadius.BottomRight = value;
                    OnPropertyChanged("HeightTextCornerRadiusBottomRight");
                    OnPropertyChanged("HeightTextCornerRadius");
                }
            }
        }

        public double HeightTextCornerRadiusBottomLeft
        {
            get { return _HeightTextCornerRadius.BottomLeft; }
            set
            {
                if (_HeightTextCornerRadius.BottomLeft != value)
                {
                    Memento.Push(Save());
                    _HeightTextCornerRadius.BottomLeft = value;
                    OnPropertyChanged("HeightTextCornerRadiusBottomLeft");
                    OnPropertyChanged("HeightTextCornerRadius");
                }
            }
        }
        #endregion

        #region HeightValue
        public bool HeightValueIsVisible
        {
            get { return _HeightValueIsVisible; }
            set
            {
                if (_HeightValueIsVisible != value)
                {
                    _HeightValueIsVisible = value;
                    OnPropertyChanged("HeightValueIsVisible");
                }
            }
        }

        public double HeightValueSize
        {
            get { return _HeightValueSize; }
            set
            {
                if (_HeightValueSize != value)
                {
                    _HeightValueSize = value;
                    OnPropertyChanged("HeightValueSize");
                }
            }
        }

        public Brush HeightValueBackground
        {
            get { return _HeightValueBackground; }
            set
            {
                if (_HeightValueBackground != value)
                {
                    _HeightValueBackground = value;
                    OnPropertyChanged("HeightValueBackground");
                }
            }
        }

        public Brush HeightValueForeground
        {
            get { return _HeightValueForeground; }
            set
            {
                if (_HeightValueForeground != value)
                {
                    _HeightValueForeground = value;
                    OnPropertyChanged("HeightValueForeground");
                }
            }
        }

        public Brush HeightValueBorderBrush
        {
            get { return _HeightValueBorderBrush; }
            set
            {
                if (_HeightValueBorderBrush != value)
                {
                    _HeightValueBorderBrush = value;
                    OnPropertyChanged("HeightValueBorderBrush");
                }
            }
        }

        public Thickness HeightValueBorderThickness
        {
            get { return _HeightValueBorderThickness; }
            set
            {
                if (_HeightValueBorderThickness != value)
                {
                    _HeightValueBorderThickness = value;
                    OnPropertyChanged("HeightValueBorderThickness");
                }
            }
        }

        public double HeightValueBorderThicknessLeft
        {
            get { return _HeightValueBorderThickness.Left; }
            set
            {
                _HeightValueBorderThickness.Left = value;
                OnPropertyChanged("HeightValueBorderThickness");
            }
        }

        public double HeightValueBorderThicknessRight
        {
            get { return _HeightValueBorderThickness.Right; }
            set
            {
                _HeightValueBorderThickness.Right = value;
                OnPropertyChanged("HeightValueBorderThickness");
            }
        }

        public double HeightValueBorderThicknessTop
        {
            get { return _HeightValueBorderThickness.Top; }
            set
            {
                _HeightValueBorderThickness.Top = value;
                OnPropertyChanged("HeightValueBorderThickness");
            }
        }

        public double HeightValueBorderThicknessBottom
        {
            get { return _HeightValueBorderThickness.Bottom; }
            set
            {
                _HeightValueBorderThickness.Bottom = value;
                OnPropertyChanged("HeightValueBorderThickness");
            }
        }


        public FontFamily HeightValueFontFamily
        {
            get { return _HeightValueFontFamily; }
            set
            {
                if (_HeightValueFontFamily != value)
                {
                    _HeightValueFontFamily = value;
                    OnPropertyChanged("HeightValueFontFamily");
                }
            }
        }

        public FontWeight HeightValueFontWeight
        {
            get { return _HeightValueFontWeight; }
            set
            {
                if (_HeightValueFontWeight != value)
                {
                    _HeightValueFontWeight = value;
                    OnPropertyChanged("HeightValueFontWeight");
                }
            }
        }

        public FontStyle HeightValueFontStyle
        {
            get { return _HeightValueFontStyle; }
            set
            {
                if (_HeightValueFontStyle != value)
                {
                    _HeightValueFontStyle = value;
                    OnPropertyChanged("HeightValueFontStyle");
                }
            }
        }

        public CornerRadius HeightValueCornerRadius
        {
            get { return _HeightValueCornerRadius; }
            set
            {
                if (_HeightValueCornerRadius != value)
                {
                    Memento.Push(Save());
                    _HeightValueCornerRadius = value;
                    OnPropertyChanged("HeightValueCornerRadius");
                    OnPropertyChanged("HeightValueCornerRadiusTopLeft");
                    OnPropertyChanged("HeightValueCornerRadiusTopRight");
                    OnPropertyChanged("HeightValueCornerRadiusBottomRight");
                    OnPropertyChanged("HeightValueCornerRadiusBottomLeft");
                }
            }
        }

        public double HeightValueCornerRadiusTopLeft
        {
            get { return _HeightValueCornerRadius.TopLeft; }
            set
            {
                if (_HeightValueCornerRadius.TopLeft != value)
                {
                    Memento.Push(Save());
                    _HeightValueCornerRadius.TopLeft = value;
                    OnPropertyChanged("HeightValueCornerRadiusTopLeft");
                    OnPropertyChanged("HeightValueCornerRadius");
                }
            }
        }

        public double HeightValueCornerRadiusTopRight
        {
            get { return _HeightValueCornerRadius.TopRight; }
            set
            {
                if (_HeightValueCornerRadius.TopRight != value)
                {
                    Memento.Push(Save());
                    _HeightValueCornerRadius.TopRight = value;
                    OnPropertyChanged("HeightValueCornerRadiusTopRight");
                    OnPropertyChanged("HeightValueCornerRadius");
                }
            }
        }

        public double HeightValueCornerRadiusBottomRight
        {
            get { return _HeightValueCornerRadius.BottomRight; }
            set
            {
                if (_HeightValueCornerRadius.BottomRight != value)
                {
                    Memento.Push(Save());
                    _HeightValueCornerRadius.BottomRight = value;
                    OnPropertyChanged("HeightValueCornerRadiusBottomRight");
                    OnPropertyChanged("HeightValueCornerRadius");
                }
            }
        }

        public double HeightValueCornerRadiusBottomLeft
        {
            get { return _HeightValueCornerRadius.BottomLeft; }
            set
            {
                if (_HeightValueCornerRadius.BottomLeft != value)
                {
                    Memento.Push(Save());
                    _HeightValueCornerRadius.BottomLeft = value;
                    OnPropertyChanged("HeightValueCornerRadiusBottomLeft");
                    OnPropertyChanged("HeightValueCornerRadius");
                }
            }
        }

        #endregion

        #region LowText
        public bool LowTextIsVisible
        {
            get { return _LowTextIsVisible; }
            set
            {
                if (_LowTextIsVisible != value)
                {
                    _LowTextIsVisible = value;
                    OnPropertyChanged("LowTextIsVisible");
                }
            }
        }

        public double LowTextSize
        {
            get { return _LowTextSize; }
            set
            {
                if (_LowTextSize != value)
                {
                    _LowTextSize = value;
                    OnPropertyChanged("LowTextSize");
                }
            }
        }

        public Brush LowTextBackground
        {
            get { return _LowTextBackground; }
            set
            {
                if (_LowTextBackground != value)
                {
                    _LowTextBackground = value;
                    OnPropertyChanged("LowTextBackground");
                }
            }
        }

        public Brush LowTextForeground
        {
            get { return _LowTextForeground; }
            set
            {
                if (_LowTextForeground != value)
                {
                    _LowTextForeground = value;
                    OnPropertyChanged("LowTextForeground");
                }
            }
        }

        public Brush LowTextBorderBrush
        {
            get { return _LowTextBorderBrush; }
            set
            {
                if (_LowTextBorderBrush != value)
                {
                    _LowTextBorderBrush = value;
                    OnPropertyChanged("LowTextBorderBrush");
                }
            }
        }

        public Thickness LowTextBorderThickness
        {
            get { return _LowTextBorderThickness; }
            set
            {
                if (_LowTextBorderThickness != value)
                {
                    _LowTextBorderThickness = value;
                    OnPropertyChanged("LowTextBorderThickness");
                }
            }
        }

        public double LowTextBorderThicknessLeft
        {
            get { return _LowTextBorderThickness.Left; }
            set
            {
                _LowTextBorderThickness.Left = value;
                OnPropertyChanged("LowTextBorderThickness");
            }
        }

        public double LowTextBorderThicknessRight
        {
            get { return _LowTextBorderThickness.Right; }
            set
            {
                _LowTextBorderThickness.Right = value;
                OnPropertyChanged("LowTextBorderThickness");
            }
        }

        public double LowTextBorderThicknessTop
        {
            get { return _LowTextBorderThickness.Top; }
            set
            {
                _LowTextBorderThickness.Top = value;
                OnPropertyChanged("LowTextBorderThickness");
            }
        }

        public double LowTextBorderThicknessBottom
        {
            get { return _LowTextBorderThickness.Bottom; }
            set
            {
                _LowTextBorderThickness.Bottom = value;
                OnPropertyChanged("LowTextBorderThickness");
            }
        }


        public FontFamily LowTextFontFamily
        {
            get { return _LowTextFontFamily; }
            set
            {
                if (_LowTextFontFamily != value)
                {
                    _LowTextFontFamily = value;
                    OnPropertyChanged("LowTextFontFamily");
                }
            }
        }

        public FontWeight LowTextFontWeight
        {
            get { return _LowTextFontWeight; }
            set
            {
                if (_LowTextFontWeight != value)
                {
                    _LowTextFontWeight = value;
                    OnPropertyChanged("LowTextFontWeight");
                }
            }
        }

        public FontStyle LowTextFontStyle
        {
            get { return _LowTextFontStyle; }
            set
            {
                if (_LowTextFontStyle != value)
                {
                    _LowTextFontStyle = value;
                    OnPropertyChanged("LowTextFontStyle");
                }
            }
        }

        public CornerRadius LowTextCornerRadius
        {
            get { return _LowTextCornerRadius; }
            set
            {
                if (_LowTextCornerRadius != value)
                {
                    Memento.Push(Save());
                    _LowTextCornerRadius = value;
                    OnPropertyChanged("LowTextCornerRadius");
                    OnPropertyChanged("LowTextCornerRadiusTopLeft");
                    OnPropertyChanged("LowTextCornerRadiusTopRight");
                    OnPropertyChanged("LowTextCornerRadiusBottomRight");
                    OnPropertyChanged("LowTextCornerRadiusBottomLeft");
                }
            }
        }

        public double LowTextCornerRadiusTopLeft
        {
            get { return _LowTextCornerRadius.TopLeft; }
            set
            {
                if (_LowTextCornerRadius.TopLeft != value)
                {
                    Memento.Push(Save());
                    _LowTextCornerRadius.TopLeft = value;
                    OnPropertyChanged("LowTextCornerRadiusTopLeft");
                    OnPropertyChanged("LowTextCornerRadius");
                }
            }
        }

        public double LowTextCornerRadiusTopRight
        {
            get { return _LowTextCornerRadius.TopRight; }
            set
            {
                if (_LowTextCornerRadius.TopRight != value)
                {
                    Memento.Push(Save());
                    _LowTextCornerRadius.TopRight = value;
                    OnPropertyChanged("LowTextCornerRadiusTopRight");
                    OnPropertyChanged("LowTextCornerRadius");
                }
            }
        }

        public double LowTextCornerRadiusBottomRight
        {
            get { return _LowTextCornerRadius.BottomRight; }
            set
            {
                if (_LowTextCornerRadius.BottomRight != value)
                {
                    Memento.Push(Save());
                    _LowTextCornerRadius.BottomRight = value;
                    OnPropertyChanged("LowTextCornerRadiusBottomRight");
                    OnPropertyChanged("LowTextCornerRadius");
                }
            }
        }

        public double LowTextCornerRadiusBottomLeft
        {
            get { return _LowTextCornerRadius.BottomLeft; }
            set
            {
                if (_LowTextCornerRadius.BottomLeft != value)
                {
                    Memento.Push(Save());
                    _LowTextCornerRadius.BottomLeft = value;
                    OnPropertyChanged("LowTextCornerRadiusBottomLeft");
                    OnPropertyChanged("LowTextCornerRadius");
                }
            }
        }

        #endregion

        #region LowValue
        public bool LowValueIsVisible
        {
            get { return _LowValueIsVisible; }
            set
            {
                if (_LowValueIsVisible != value)
                {
                    _LowValueIsVisible = value;
                    OnPropertyChanged("LowValueIsVisible");
                }
            }
        }

        public double LowValueSize
        {
            get { return _LowValueSize; }
            set
            {
                if (_LowValueSize != value)
                {
                    _LowValueSize = value;
                    OnPropertyChanged("LowValueSize");
                }
            }
        }

        public Brush LowValueBackground
        {
            get { return _LowValueBackground; }
            set
            {
                if (_LowValueBackground != value)
                {
                    _LowValueBackground = value;
                    OnPropertyChanged("LowValueBackground");
                }
            }
        }

        public Brush LowValueForeground
        {
            get { return _LowValueForeground; }
            set
            {
                if (_LowValueForeground != value)
                {
                    _LowValueForeground = value;
                    OnPropertyChanged("LowValueForeground");
                }
            }
        }

        public Brush LowValueBorderBrush
        {
            get { return _LowValueBorderBrush; }
            set
            {
                if (_LowValueBorderBrush != value)
                {
                    _LowValueBorderBrush = value;
                    OnPropertyChanged("LowValueBorderBrush");
                }
            }
        }

        public Thickness LowValueBorderThickness
        {
            get { return _LowValueBorderThickness; }
            set
            {
                if (_LowValueBorderThickness != value)
                {
                    _LowValueBorderThickness = value;
                    OnPropertyChanged("LowValueBorderThickness");
                }
            }
        }

        public double LowValueBorderThicknessLeft
        {
            get { return _LowValueBorderThickness.Left; }
            set
            {
                _LowValueBorderThickness.Left = value;
                OnPropertyChanged("LowValueBorderThickness");
            }
        }

        public double LowValueBorderThicknessRight
        {
            get { return _LowValueBorderThickness.Right; }
            set
            {
                _LowValueBorderThickness.Right = value;
                OnPropertyChanged("LowValueBorderThickness");
            }
        }

        public double LowValueBorderThicknessTop
        {
            get { return _LowValueBorderThickness.Top; }
            set
            {
                _LowValueBorderThickness.Top = value;
                OnPropertyChanged("LowValueBorderThickness");
            }
        }

        public double LowValueBorderThicknessBottom
        {
            get { return _LowValueBorderThickness.Bottom; }
            set
            {
                _LowValueBorderThickness.Bottom = value;
                OnPropertyChanged("LowValueBorderThickness");
            }
        }


        public FontFamily LowValueFontFamily
        {
            get { return _LowValueFontFamily; }
            set
            {
                if (_LowValueFontFamily != value)
                {
                    _LowValueFontFamily = value;
                    OnPropertyChanged("LowValueFontFamily");
                }
            }
        }

        public FontWeight LowValueFontWeight
        {
            get { return _LowValueFontWeight; }
            set
            {
                if (_LowValueFontWeight != value)
                {
                    _LowValueFontWeight = value;
                    OnPropertyChanged("LowValueFontWeight");
                }
            }
        }

        public FontStyle LowValueFontStyle
        {
            get { return _LowValueFontStyle; }
            set
            {
                if (_LowValueFontStyle != value)
                {
                    _LowValueFontStyle = value;
                    OnPropertyChanged("LowValueFontStyle");
                }
            }
        }

        public CornerRadius LowValueCornerRadius
        {
            get { return _LowValueCornerRadius; }
            set
            {
                if (_LowValueCornerRadius != value)
                {
                    Memento.Push(Save());
                    _LowValueCornerRadius = value;
                    OnPropertyChanged("LowValueCornerRadius");
                    OnPropertyChanged("LowValueCornerRadiusTopLeft");
                    OnPropertyChanged("LowValueCornerRadiusTopRight");
                    OnPropertyChanged("LowValueCornerRadiusBottomRight");
                    OnPropertyChanged("LowValueCornerRadiusBottomLeft");
                }
            }
        }

        public double LowValueCornerRadiusTopLeft
        {
            get { return _LowValueCornerRadius.TopLeft; }
            set
            {
                if (_LowValueCornerRadius.TopLeft != value)
                {
                    Memento.Push(Save());
                    _LowValueCornerRadius.TopLeft = value;
                    OnPropertyChanged("LowValueCornerRadiusTopLeft");
                    OnPropertyChanged("LowValueCornerRadius");
                }
            }
        }

        public double LowValueCornerRadiusTopRight
        {
            get { return _LowValueCornerRadius.TopRight; }
            set
            {
                if (_LowValueCornerRadius.TopRight != value)
                {
                    Memento.Push(Save());
                    _LowValueCornerRadius.TopRight = value;
                    OnPropertyChanged("LowValueCornerRadiusTopRight");
                    OnPropertyChanged("LowValueCornerRadius");
                }
            }
        }

        public double LowValueCornerRadiusBottomRight
        {
            get { return _LowValueCornerRadius.BottomRight; }
            set
            {
                if (_LowValueCornerRadius.BottomRight != value)
                {
                    Memento.Push(Save());
                    _LowValueCornerRadius.BottomRight = value;
                    OnPropertyChanged("LowValueCornerRadiusBottomRight");
                    OnPropertyChanged("LowValueCornerRadius");
                }
            }
        }

        public double LowValueCornerRadiusBottomLeft
        {
            get { return _LowValueCornerRadius.BottomLeft; }
            set
            {
                if (_LowValueCornerRadius.BottomLeft != value)
                {
                    Memento.Push(Save());
                    _LowValueCornerRadius.BottomLeft = value;
                    OnPropertyChanged("LowValueCornerRadiusBottomLeft");
                    OnPropertyChanged("LowValueCornerRadius");
                }
            }
        }
        #endregion

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

