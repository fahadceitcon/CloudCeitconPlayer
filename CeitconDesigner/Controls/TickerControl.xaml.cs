using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for TickerControl.xaml
    /// </summary>
    public partial class TickerControl : UserControl
    {
        public TickerControl()
        {
            //Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 60 });
            InitializeComponent();
            DataContext = this;
        }

        private static bool _InvertDirection = false;
        private static TimeSpan _Duration = TimeSpan.Parse("0:0:20");

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ActivateAnimation(this);
        }

        private static void ActivateAnimation(FrameworkElement d, string newValue = "")
        {
            try
            {
                FrameworkElement element = d as FrameworkElement;
                var c = element.FindName("canMain") as Canvas;
                var tb = c.Children[0] as TextBlock;
                double textGraphicalWidth = new FormattedText(String.IsNullOrEmpty(newValue) ? tb.Text : newValue, System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(tb.FontFamily.Source), tb.FontSize, tb.Foreground).WidthIncludingTrailingWhitespace;
                tb.Width = textGraphicalWidth;
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = _InvertDirection ? -textGraphicalWidth : c.ActualWidth;
                doubleAnimation.To = _InvertDirection ? c.ActualWidth : -textGraphicalWidth;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(_Duration);
                tb.BeginAnimation(Canvas.LeftProperty, doubleAnimation);

                //FrameworkElement element = d as FrameworkElement;
                //var c = element.FindName("canMain") as Canvas;
                //var tb = c.Children[0] as TextBlock;
                //double textGraphicalWidth = new FormattedText(tb.Text, System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(tb.FontFamily.Source), tb.FontSize, tb.Foreground).WidthIncludingTrailingWhitespace;
                //tb.Width = textGraphicalWidth;
                //DoubleAnimation doubleAnimation = new DoubleAnimation();
                //doubleAnimation.From = _InvertDirection ? -textGraphicalWidth : c.ActualWidth;
                //doubleAnimation.To = _InvertDirection ? c.ActualWidth : -textGraphicalWidth;
                //doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                //doubleAnimation.Duration = new Duration(_Duration);
                //tb.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
            }
            catch (Exception) { }
        }

        #region Properties
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register
        (
            "AnimationDuration",
            typeof(TimeSpan),
            typeof(TickerControl),
            new PropertyMetadata(new TimeSpan(0,0,10))
        );

        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }


        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register
            (
                 "Width",
                 typeof(int),
                 typeof(TickerControl),
                 new PropertyMetadata(1000, OnWidthChanged)
            );

        public int Width
        {
            get { return (int)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ActivateAnimation(d as FrameworkElement);
        }


        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register
        (
             "Height",
             typeof(int),
             typeof(TickerControl),
             new PropertyMetadata(100)
        );

        public int Height
        {
            get { return (int)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty CurrentWidthProperty = DependencyProperty.Register
        (
             "CurrentWidth",
             typeof(int),
             typeof(TickerControl),
             new PropertyMetadata(1000)
        );

        public int CurrentWidth
        {
            get { return (int)GetValue(CurrentWidthProperty); }
            set { SetValue(CurrentWidthProperty, value); }
        }

        public static readonly DependencyProperty CurrentHeightProperty = DependencyProperty.Register
        (
             "CurrentHeight",
             typeof(int),
             typeof(TickerControl),
             new PropertyMetadata(100)
         );

        public int CurrentHeight
        {
            get { return (int)GetValue(CurrentHeightProperty); }
            set { SetValue(CurrentHeightProperty, value); }
        }

        public int CurrentlyHeight
        {
            get
            {
                int result = Height - (int)BorderThickness.Top - (int)BorderThickness.Bottom;
                return result > 0 ? result : 0;
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
        (
             "Text",
             typeof(object),
             typeof(TickerControl),
             new PropertyMetadata(null, OnTextChanged)
        );

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ActivateAnimation(d as FrameworkElement, e.NewValue.ToString());
        }

        public object Text
        {
            get { return (object)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty InvertDirectionProperty = DependencyProperty.Register
        (
             "InvertDirection",
             typeof(bool),
             typeof(TickerControl),
             new PropertyMetadata(false, OnInvertDirectionChanged)
        );

        public bool InvertDirection
        {
            get { return (bool)GetValue(InvertDirectionProperty); }
            set { SetValue(InvertDirectionProperty, value); }
        }

        private static void OnInvertDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _InvertDirection = (bool)e.NewValue;
            ActivateAnimation(d as FrameworkElement);
        }

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register
        (
             "Duration",
             typeof(TimeSpan),
             typeof(TickerControl),
             new PropertyMetadata(new TimeSpan(0,0,20), OnDurationChanged)
        );

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _Duration = (TimeSpan)e.NewValue;
            ActivateAnimation(d as FrameworkElement);
        }

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register
        (
             "FontSize",
             typeof(int),
             typeof(TickerControl),
             new PropertyMetadata(80)
        );

        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register
        (
            "FontFamily",
            typeof(FontFamily),
            typeof(TickerControl),
            new PropertyMetadata(new FontFamily("Arial"))
        );

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register
        (
             "Foreground",
             typeof(Brush),
             typeof(TickerControl),
             new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 0, 255, 0)))
        );

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register
        (
             "Background",
             typeof(Brush),
             typeof(TickerControl),
             new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)))
        );

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.Register
        (
            "HorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(TickerControl),
            new PropertyMetadata(HorizontalAlignment.Left)
        );

        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty VerticalAlignmentProperty = DependencyProperty.Register
        (
            "VerticalAlignment",
            typeof(VerticalAlignment),
            typeof(TickerControl),
            new PropertyMetadata(VerticalAlignment.Top)
        );

        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register
        (
            "FontWeight",
            typeof(FontWeight),
            typeof(TickerControl),
            new PropertyMetadata(FontWeights.Normal)
        );

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register
        (
            "FontStyle",
            typeof(FontStyle),
            typeof(TickerControl),
            new PropertyMetadata(FontStyles.Normal)
        );

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register
        (
            "TextDecorations",
            typeof(TextDecorationCollection),
            typeof(TickerControl),
            new PropertyMetadata(null)
        );

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }
        #endregion

    }
}
