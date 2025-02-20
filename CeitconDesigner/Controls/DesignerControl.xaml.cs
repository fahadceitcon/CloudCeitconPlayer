using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Data;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Data.Utilities;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.View;
using Ceitcon_Designer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.FormatProviders.Html;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for DesignerControl.xaml
    /// </summary>
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public partial class DesignerControl : UserControl
    {
        private static double zoomSpeed = 0.05;
        private object _Copy;

        public DesignerControl()
        {
            InitializeComponent();
            tbSnap.IsChecked = SpanEffect.Enable;
            this.KeyDown += HandleKeyPress;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var project = (DataContext as MainViewModel).Project;
            if ((bool)e.NewValue == true && project != null)
            {
                //Drow region
                var obj = project.SelectedRegion; project.SelectedRegion = null; project.SelectedRegion = obj;
                ClearRegions();
                foreach (RegionModel item in project.Regions)
                {
                    var rec = new Rectangle()
                    {
                        Fill = Brushes.SeaGreen,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5,
                        Margin = new Thickness(item.X * 2 - project.SelectedResolution.Width + item.Width, item.Y * 2 - project.SelectedResolution.Height + item.Height, 0, 0),
                        Width = item.Width,
                        Height = item.Height
                    };
                    this.RegionGrid.Children.Add(rec);
                    this.RegionGrid.Children.Add(new TextBlock() { Text = item.Name, FontSize = 36, Margin = new Thickness(item.X * 2 - project.SelectedResolution.Width + item.Width, item.Y * 2 - project.SelectedResolution.Height + item.Height, 0, 0), Width = item.Width, Height = 100, TextAlignment = TextAlignment.Center });
                }

                //Add line
                DrowLines((DataContext as MainViewModel).Project.SelectedResolution, (DataContext as MainViewModel).Project.SelectedMonitor, 0, 0);

                //Drow elements
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion != null)
                {
                    (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide = (DataContext as MainViewModel).Project.SelectedRegion.Slides.FirstOrDefault();
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
                }
            }
            else
            {
                SpanEffect.Enable = false;
                tbSnap.IsChecked = false;
                ClearCanvas();

                //Stop preview
                StopPreview();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel)
            {
                (DataContext as MainViewModel).SelectedTool = (sender as TabControl).SelectedIndex;
            }
        }

        #region Properties
        public ObservableCollection<SocialMediaModel> SocialMedias
        {
            get { return (DataContext as MainViewModel).SocialMedias; }
        }
        #endregion

        #region Commands
        private void CopyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (DataContext != null)
                    e.CanExecute = (DataContext as MainViewModel).Project.SelectedObject != null;
            }
            catch (Exception)
            {
                e.CanExecute = false;
            }
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Copy();
        }

        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_Copy != null);
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Paste();
        }
        private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Memento.HasValue();
        }

        private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
        }

        private void RedoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Memento.HasValueR();
        }

        private void RedoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }
        #endregion

        #region Designer

        private void ClearCanvas()
        {
            this.DesignerCanvas.Children.Clear();
        }

        private void ClearRegions()
        {
            this.RegionGrid.Children.RemoveRange(1, this.RegionGrid.Children.Count - 1); //Clear all except 
        }

        private void DrowLines(ResolutionModel resolution, MonitorModel monitor, double X, double Y)
        {
            ClearLines();
            int thickness = 10;
            Brush colour = Brushes.Yellow;
            if (monitor.Horizontal > 1)
            {
                for (int i = 1; i < monitor.Horizontal; i++)
                {
                    var line = new Line()
                    {
                        Stroke = colour,
                        X1 = resolution.Width / monitor.Horizontal * i + X,
                        Y1 = 0 + Y,
                        X2 = resolution.Width / monitor.Horizontal * i + X,
                        Y2 = resolution.Height + Y,
                        StrokeThickness = thickness,
                        SnapsToDevicePixels = true,
                    };
                    line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                    Panel.SetZIndex(line, 100);
                    this.RegionGrid.Children.Add(line);
                }
            }

            if (monitor.Vertical > 1)
            {
                for (int i = 1; i < monitor.Vertical; i++)
                {

                    var line = new Line()
                    {
                        Stroke = colour,
                        X1 = 0 + X,
                        Y1 = resolution.Height / monitor.Vertical * i + Y,
                        X2 = resolution.Width + X,
                        Y2 = resolution.Height / monitor.Vertical * i + Y,
                        StrokeThickness = thickness,
                        SnapsToDevicePixels = true,
                    };
                    line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                    Panel.SetZIndex(line, 100);
                    this.RegionGrid.Children.Add(line);
                }
            }
        }

        private void ClearLines()
        {
            this.RegionGrid.Children.OfType<Line>().Where(_ => _.GetType() == typeof(Line)).ToList().ForEach(_ => this.RegionGrid.Children.Remove(_));
        }

        private void DrowDots(ResolutionModel resolution)
        {
            int dotSize = 10;
            int step = SpanEffect.Step * 2;
            for (int i = -resolution.Width; i < resolution.Width; i = i + step)
            {
                for (int j = -resolution.Height; j < resolution.Height; j = j + step)
                {
                    var currentDot = new Ellipse();
                    currentDot.Stroke = new SolidColorBrush(Colors.YellowGreen);
                    currentDot.StrokeThickness = dotSize;
                    Canvas.SetZIndex(currentDot, 3);
                    currentDot.Height = dotSize;
                    currentDot.Width = dotSize;
                    currentDot.Fill = new SolidColorBrush(Colors.Green);
                    currentDot.Margin = new Thickness(i, j, 0, 0); // Sets the position.
                    this.RegionGrid.Children.Add(currentDot);
                }
            }
        }

        private void ClearDots()
        {
            this.RegionGrid.Children.OfType<Ellipse>().Where(_ => _.GetType() == typeof(Ellipse)).ToList().ForEach(_ => this.RegionGrid.Children.Remove(_));
        }

        private void LoadCanvas(SlideModel slide)
        {
            foreach (LayerModel layer in slide.Layers)
            {
                foreach (ControlModel control in layer.Controls)
                {
                    DrowDesignerCanvas(control);
                }
            }
        }

        #region Arabic
        private string GetArabicData(string _input)
        {
            string sResult = "";
            foreach (char item in _input)
            {
                switch (item)
                {
                    case '1':
                        sResult = sResult + "١";
                        break;
                    case '2':
                        sResult = sResult + "٢";
                        break;
                    case '3':
                        sResult = sResult + "٣";
                        break;
                    case '4':
                        sResult = sResult + "٤";
                        break;
                    case '5':
                        sResult = sResult + "٥";
                        break;
                    case '6':
                        sResult = sResult + "٦";
                        break;
                    case '7':
                        sResult = sResult + "٧";
                        break;
                    case '8':
                        sResult = sResult + "٨";
                        break;
                    case '9':
                        sResult = sResult + "٩";
                        break;
                    case '0':
                        sResult = sResult + "٠";
                        break;

                    default:
                        sResult = sResult + item.ToString();
                        break;
                }
            }

            return sResult;
        }

        #endregion


        private void DrowDesignerCanvas(ControlModel item)
        {
            if (item != null)
            {
                DeselectAll();

                ContentControl cc = new ContentControl();
                cc.Name = item.Name;
                cc.Width = item.Width;
                cc.Height = item.Height;
                cc.MinWidth = 50;
                cc.MinHeight = 50;
                cc.Template = this.FindResource("DesignerItemTemplate") as ControlTemplate;
                cc.DataContext = item;

                DockPanel grid = new DockPanel() //Grid have problem wwith zoom diagram
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };

                //Border
                Border border = new Border() { Margin = new Thickness(1, 1, 1, 1), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, IsHitTestVisible = false };
                border.SetBinding(Border.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                border.SetBinding(Border.BorderBrushProperty, new Binding() { Path = new PropertyPath("BorderBrush"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                border.SetBinding(Border.BorderThicknessProperty, new Binding() { Path = new PropertyPath("BorderThickness"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                border.SetBinding(Border.CornerRadiusProperty, new Binding() { Path = new PropertyPath("CornerRadius"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                border.SetBinding(Border.OpacityProperty, new Binding() { Path = new PropertyPath("Opacity"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                grid.Children.Add(border);

                //Content
                switch (item.Type)
                {
                    case ControlType.Image:
                        {
                            //item.Source = @"C:\Temp\2.jpg";
                            var img = new Image() { RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false }; //Source = new BitmapImage(new Uri((item.Source))),
                            img.SetBinding(Image.SourceProperty, new Binding() { Path = new PropertyPath("Source"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.StretchProperty, new Binding() { Path = new PropertyPath("Stretch"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = img;
                        }
                        break;
                    case ControlType.Video:
                        {
                            //item.Source = @"C:\Temp\Video10.mov";
                            //<MediaElement Source="../Images/iconButton_Copy.png" Stretch="Fill" />

                            //var me = new MediaElement() { RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            //me.SetBinding(MediaElement.SourceProperty, new Binding() { Path = new PropertyPath("Source"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //me.SetBinding(MediaElement.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //me.SetBinding(MediaElement.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //me.SetBinding(MediaElement.StretchProperty, new Binding() { Path = new PropertyPath("Stretch"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //me.SetBinding(MediaElement.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //border.Child = me;

                            var me = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                            };
                            me.SetBinding(Image.SourceProperty, new Binding() { Path = new PropertyPath("Thumbnail"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            me.SetBinding(Image.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            me.SetBinding(Image.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            me.SetBinding(Image.StretchProperty, new Binding() { Path = new PropertyPath("Stretch"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            me.SetBinding(Image.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = me;
                        }
                        break;
                    case ControlType.GifAnim:
                        {
                            var img = new Image() { RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false }; //Source = new BitmapImage(new Uri((item.Source))),
                            img.SetBinding(WpfAnimatedGif.ImageBehavior.AnimatedSourceProperty, new Binding() { Path = new PropertyPath("Source"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.StretchProperty, new Binding() { Path = new PropertyPath("Stretch"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = img;
                        }
                        break;
                    case ControlType.RichText:
                        {
                            var tbl = new RadRichTextBox()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Name = "richTextBox",
                                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                                IsSpellCheckingEnabled = false
                            };
                            var HtmlDataProvider = new HtmlDataProvider();
                            HtmlDataProvider.SetBinding(HtmlDataProvider.RichTextBoxProperty, new Binding() { Source = tbl });
                            HtmlDataProvider.SetBinding(HtmlDataProvider.HtmlProperty, new Binding() { Path = new PropertyPath("Text"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                            tbl.SetBinding(RadRichTextBox.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tbl.SetBinding(RadRichTextBox.ForegroundProperty, new Binding() { Path = new PropertyPath("Foreground"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tbl.SetBinding(RadRichTextBox.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tbl.SetBinding(RadRichTextBox.FontFamilyProperty, new Binding() { Path = new PropertyPath("FontFamily"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tbl.SetBinding(RadRichTextBox.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tbl.SetBinding(RadRichTextBox.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tbl.SetBinding(RadRichTextBox.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = tbl;
                        }
                        break;
                    case ControlType.Ticker:
                        {
                            var tc = new TickerControl() { RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            tc.SetBinding(TickerControl.TextProperty, new Binding() { Path = new PropertyPath("Text"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.InvertDirectionProperty, new Binding() { Path = new PropertyPath("InvertDirection"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.DurationProperty, new Binding() { Path = new PropertyPath("Duration"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.WidthProperty, new Binding() { Path = new PropertyPath("Width"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.HeightProperty, new Binding() { Path = new PropertyPath("Height"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.ForegroundProperty, new Binding() { Path = new PropertyPath("Foreground"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.CurrentWidthProperty, new Binding() { Path = new PropertyPath("CurrentWidth"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.CurrentHeightProperty, new Binding() { Path = new PropertyPath("CurrentHeight"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.FontFamilyProperty, new Binding() { Path = new PropertyPath("FontFamily"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.FontWeightProperty, new Binding() { Path = new PropertyPath("FontWeight"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.FontStyleProperty, new Binding() { Path = new PropertyPath("FontStyle"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            tc.SetBinding(TickerControl.TextDecorationsProperty, new Binding() { Path = new PropertyPath("TextDecoration"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = tc;
                        }
                        break;
                    case ControlType.DataGrid:
                        {
                            var img = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Stretch = Stretch.Fill,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                //Source = new BitmapImage(new Uri(@"../Images/iconDataGrid.png", UriKind.Relative))
                            };
                            img.SetBinding(Image.SourceProperty, new Binding() { Path = new PropertyPath("DataGridImage"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = img;
                            //ObservableCollection<FIModel> list = new ObservableCollection<FIModel>();
                            ////list.Add(new FIModel() { Time = DateTime.Now, Flight = "IBE-"+ GetArabicData("844"), Description = "Barcelona", Airline = "Iberia", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = 33, Status = "On Time" });
                            ////list.Add(new FIModel() { Time = DateTime.Now, Flight = "LX-355", Description = "Zurich", Airline = "Swiss Intrnational", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = 55, Status = "Delay 15:55" });
                            ////list.Add(new FIModel() { Time = DateTime.Now, Flight = "AF-47", Description = "Paris", Airline = "Air France", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = 12, Status = "On Time" });
                            ////list.Add(new FIModel() { Time = DateTime.Now, Flight = "BA-55", Description = "London", Airline = "British Airlines", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = 22, Status = "On Time" });
                            ////list.Add(new FIModel() { Time = DateTime.Now, Flight = "LX-974", Description = "Tokyo", Airline = "Swiss Intrnational", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = 39, Status = "Delay 18:00" });
                            ////list.Add(new FIModel() { Time = DateTime.Now, Flight = "UA-544", Description = "New Tork", Airline = "Unites Airlines", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = 44, Status = "On Time" });
                            ////list.Add(new FIModel() { Time = DateTime.Now, Flight = "AA-43", Description = "San Francisco", Airline = "American Airlines", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = 32, Status = "On Time" });
                            ////list.Add(new FIModel() { Time = DateTime.Now, Flight = "IBE-544", Description = "Buenos Aires", Airline = "Iberia", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = 33, Status = "On Time" });
                            //list.Add(new FIModel() { Time = GetArabicData((new DateTime(2017,11,2,8,15,0)).ToString("HH:mm:ss", new System.Globalization.CultureInfo("ar-SA"))), Flight = "IBE " + GetArabicData("844"), Description = "البــحــريــن", Airline = "Iberia", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = GetArabicData("33"), Status = "غـــــــــادرت" });
                            //list.Add(new FIModel() { Time = GetArabicData((new DateTime(2017, 11, 2, 8, 20, 0)).ToString("HH:mm:ss", new System.Globalization.CultureInfo("ar-SA"))), Flight = "LX " + GetArabicData("355"), Description = "جــــدّة", Airline = "Swiss Intrnational", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = GetArabicData("55"), Status = "غـــــــــادرت" });
                            //list.Add(new FIModel() { Time = GetArabicData((new DateTime(2017, 11, 2, 8, 25, 0)).ToString("HH:mm:ss", new System.Globalization.CultureInfo("ar-SA"))), Flight = "AF " + GetArabicData("47"), Description = "جــــدّة", Airline = "Air France", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = GetArabicData("12"), Status = "السّـــير مغــــلق" });
                            //list.Add(new FIModel() { Time = GetArabicData((new DateTime(2017, 11, 2, 8, 40, 0)).ToString("HH:mm:ss", new System.Globalization.CultureInfo("ar-SA"))), Flight = "BA " + GetArabicData("55"), Description = "لــندن", Airline = "British Airlines", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = GetArabicData("22"), Status = "السّـــير مغــــلق" });
                            //list.Add(new FIModel() { Time = GetArabicData((new DateTime(2017, 11, 2, 8, 45, 0)).ToString("HH:mm:ss", new System.Globalization.CultureInfo("ar-SA"))), Flight = "LX " + GetArabicData("974"), Description = "أسطنبول", Airline = "Swiss Intrnational", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = GetArabicData("39"), Status = "غـــــــــادرت" });
                            //list.Add(new FIModel() { Time = GetArabicData((new DateTime(2017, 11, 2, 9, 0, 0)).ToString("HH:mm:ss", new System.Globalization.CultureInfo("ar-SA"))), Flight = "UA " + GetArabicData("544"), Description = "السّـــير مغــــلق", Airline = "Unites Airlines", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = GetArabicData("44"), Status = "غـــــــــادرت" });
                            //list.Add(new FIModel() { Time = GetArabicData((new DateTime(2017, 11, 2, 9, 10, 0)).ToString("HH:mm:ss", new System.Globalization.CultureInfo("ar-SA"))), Flight = "AA " + GetArabicData("43"), Description = "غـــــــــادرت", Airline = "American Airlines", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = GetArabicData("32"), Status = "غـــــــــادرت" });
                            //list.Add(new FIModel() { Time = GetArabicData((new DateTime(2017, 11, 2, 9, 15, 0)).ToString("HH:mm:ss", new System.Globalization.CultureInfo("ar-SA"))), Flight = "IBE " + GetArabicData("544"), Description = "السّـــير مغــــلق", Airline = "Iberia", Image = new BitmapImage(new Uri(@"../Images/iconGifAnimControl_Active.png", UriKind.Relative)), Gate = GetArabicData("33"), Status = "غـــــــــادرت" });



                            //var dataGrid = new DataGrid()
                            //{
                            //    RenderTransformOrigin = new Point(0.5, 0.5),
                            //    IsHitTestVisible = false,
                            //    AutoGenerateColumns = false,
                            //    HorizontalAlignment = HorizontalAlignment.Stretch,
                            //    RowHeaderWidth = 0,
                            //    ItemsSource = list,
                            //};
                            //dataGrid.Columns.Clear();
                            //dataGrid.Columns.Add(new DataGridTextColumn() { Header = "الوقت", Binding = new Binding("Time") });
                            //dataGrid.Columns.Add(new DataGridTextColumn() { Header = "الرحلة", Binding = new Binding("Flight") });
                            //dataGrid.Columns.Add(new DataGridTextColumn() { Header = "إلى", Binding = new Binding("Description") });
                            ////dataGrid.Columns.Add(new DataGridTextColumn() { Header = "", Binding = new Binding("Airline") });

                            //DataGridTemplateColumn imgColumn = new DataGridTemplateColumn();
                            //imgColumn.Header = "الصورة";
                            //FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
                            //imageFactory.SetBinding(Image.SourceProperty, new Binding("Image"));
                            //DataTemplate dataTemplate = new DataTemplate();
                            //dataTemplate.VisualTree = imageFactory;
                            //imgColumn.CellTemplate = dataTemplate;
                            //dataGrid.Columns.Add(imgColumn);

                            //dataGrid.Columns.Add(new DataGridTextColumn() { Header = "البوابة", Width=100, Binding = new Binding("Gate") });
                            //dataGrid.Columns.Add(new DataGridTextColumn() { Header = "الملاحظات", Binding = new Binding("Status") });
                            //Viewbox vb = new Viewbox() { Stretch = Stretch.Fill };
                            //vb.Child = dataGrid;
                            //border.Child = vb;
                        }
                        break;
                    case ControlType.Alert:
                        {
                            var al = new AlertControl() { RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            al.SetBinding(AlertControl.TextProperty, new Binding() { Path = new PropertyPath("Text"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //al.SetBinding(AlertControl.SourceProperty, new Binding() { Path = new PropertyPath("Source"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            al.SetBinding(AlertControl.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            al.SetBinding(AlertControl.ForegroundProperty, new Binding() { Path = new PropertyPath("Foreground"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            al.SetBinding(AlertControl.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            al.SetBinding(AlertControl.FontFamilyProperty, new Binding() { Path = new PropertyPath("FontFamily"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            al.SetBinding(AlertControl.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            al.SetBinding(AlertControl.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            al.SetBinding(AlertControl.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = al;
                        }
                        break;

                    case ControlType.Likebox:
                    case ControlType.Facebook:
                    case ControlType.Twitter:
                    case ControlType.Instagram:
                        {
                            ScrollViewer sv = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
                            sv.SetBinding(ScrollViewer.WidthProperty, new Binding() { Path = new PropertyPath("Width"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            TextBlock text = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, TextWrapping = TextWrapping.Wrap, RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            text.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Text"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.ForegroundProperty, new Binding() { Path = new PropertyPath("Foreground"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontFamilyProperty, new Binding() { Path = new PropertyPath("FontFamily"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            sv.Content = text;
                            border.Child = sv;
                        }
                        break;
                    case ControlType.Youtube:
                        {
                            var img = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Stretch = Stretch.Fill,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Source = new BitmapImage(new Uri(@"../Images/iconYoutube.png", UriKind.Relative))
                            };
                            border.Child = img;
                        }
                        break;
                    case ControlType.SocialMediaImage:
                        {
                            //item.Source = @"C:\Temp\2.jpg";
                            var img = new Image() { RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false }; //Source = new BitmapImage(new Uri((item.Source))),
                            img.SetBinding(Image.SourceProperty, new Binding() { Path = new PropertyPath("Source"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.StretchProperty, new Binding() { Path = new PropertyPath("Stretch"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = img;
                        }
                        break;
                    case ControlType.Live:
                        {
                            var img = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Stretch = Stretch.Fill,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Source = new BitmapImage(new Uri(@"../Images/iconLive.png", UriKind.Relative))
                            };
                            border.Child = img;
                        }
                        break;
                    case ControlType.Weather:
                        {
                            var img = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Stretch = Stretch.Fill,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Source = new BitmapImage(new Uri(@"../Images/iconWeather.png", UriKind.Relative))
                            };
                            border.Child = img;
                        }
                        break;
                    case ControlType.DateTime:
                        {
                            TextBlock text = new TextBlock() { TextWrapping = TextWrapping.Wrap, RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            text.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("DateTimeText"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.ForegroundProperty, new Binding() { Path = new PropertyPath("Foreground"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontFamilyProperty, new Binding() { Path = new PropertyPath("FontFamily"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontWeightProperty, new Binding() { Path = new PropertyPath("FontWeight"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontStyleProperty, new Binding() { Path = new PropertyPath("FontStyle"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.TextDecorationsProperty, new Binding() { Path = new PropertyPath("TextDecoration"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                            border.Child = text;
                        }
                        break;
                    case ControlType.Text:
                        {
                            TextBlock text = new TextBlock() { TextWrapping = TextWrapping.Wrap, RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            text.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Text"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.ForegroundProperty, new Binding() { Path = new PropertyPath("Foreground"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontFamilyProperty, new Binding() { Path = new PropertyPath("FontFamily"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontWeightProperty, new Binding() { Path = new PropertyPath("FontWeight"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontStyleProperty, new Binding() { Path = new PropertyPath("FontStyle"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.TextDecorationsProperty, new Binding() { Path = new PropertyPath("TextDecoration"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                            border.Child = text;
                        }
                        break;
                    case ControlType.PDF:
                        {
                            var img = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Stretch = Stretch.Fill,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Source = new BitmapImage(new Uri(@"../Images/iconPDF.png", UriKind.Relative))
                            };
                            border.Child = img;
                        }
                        break;
                    case ControlType.PPT:
                        {
                            var img = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Stretch = Stretch.Fill,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Source = new BitmapImage(new Uri(@"../Images/iconPPT.png", UriKind.Relative))
                            };
                            border.Child = img;
                        }
                        break;
                    case ControlType.WebBrowser:
                        {
                            var img = new Image()
                            {
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                IsHitTestVisible = false,
                                Stretch = Stretch.Fill,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Source = new BitmapImage(new Uri(@"../Images/iconWebBrowser.png", UriKind.Relative))
                            };
                            border.Child = img;
                        }
                        break;
                    default:
                        //border.Child = new Rectangle() { Fill = fillBrush, Stroke = stokeBrush, StrokeThickness = 5, StrokeDashArray = { 4, 2 }, IsHitTestVisible = false };
                        break;
                }

                //Component Name
                //TextBlock tb = new TextBlock() { Foreground = Brushes.White, Background = Brushes.SeaGreen, FontSize = 30, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, IsHitTestVisible = false };
                //tb.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Name"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                //grid.Children.Add(tb);
                cc.Content = grid;

                //Context menu
                var cm = new ContextMenu();
                var mi = new MenuItem() { Header = String.Format("Delete {0}", item.Name) };
                mi.Click += delegate { (DataContext as MainViewModel).DeleteControl(item); this.DesignerCanvas.Children.Remove(cc); };
                cm.Items.Add(mi);
                cc.ContextMenu = cm;

                //Bindings
                Canvas.SetLeft(cc, item.X);
                Canvas.SetTop(cc, item.Y);
                Canvas.SetZIndex(cc, item.ZIndex);

                cc.SetBinding(ContentControl.NameProperty, new Binding() { Path = new PropertyPath("Name"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(ContentControl.WidthProperty, new Binding() { Path = new PropertyPath("Width"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(ContentControl.HeightProperty, new Binding() { Path = new PropertyPath("Height"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(ContentControl.VisibilityProperty, new Binding() { Path = new PropertyPath("IsVisible"), Source = item, Converter = Converters.BoolToVisibilityConverter.Hidden, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(Canvas.LeftProperty, new Binding() { Path = new PropertyPath("X"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(Canvas.TopProperty, new Binding() { Path = new PropertyPath("Y"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(Canvas.ZIndexProperty, new Binding() { Path = new PropertyPath("ZIndex"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                this.DesignerCanvas.Children.Add(cc);
            }
        }
        #endregion

        #region DragDrop Elements

        private Button draggedItem;

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedItem = sender as Button;
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && draggedItem != null)
            {
                Point position = e.GetPosition(null);
                DragDrop.DoDragDrop(draggedItem, draggedItem, DragDropEffects.Copy);
            }
        }

        private void Canvas_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            Point position = e.GetPosition(this.DesignerCanvas);
            var m = (DataContext as MainViewModel);
            if (draggedItem != null)
            {
                ControlModel control = null;
                switch (draggedItem.Name)
                {
                    case "ImageButton":
                        control = m.CreateControl(ControlType.Image, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300);
                        break;
                    case "VideoButton":
                        control = m.CreateControl(ControlType.Video, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300);
                        break;
                    case "GifAnimButton":
                        control = m.CreateControl(ControlType.GifAnim, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300);
                        break;
                    case "RichTextButton":
                        control = m.CreateControl(ControlType.RichText, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300);
                        break;
                    case "TickerButton":
                        control = m.CreateControl(ControlType.Ticker, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 800 ? m.Project.SelectedRegion.Width - position.X : 800,
                            m.Project.SelectedRegion.Height - position.Y < 150 ? m.Project.SelectedRegion.Height - position.Y : 150);
                        break;
                    case "DataGridButton":
                        control = m.CreateControl(ControlType.DataGrid, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300);
                        break;
                    case "AlertButton":
                        control = m.CreateControl(ControlType.Alert, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 800 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 100 ? m.Project.SelectedRegion.Height - position.Y : 300);
                        break;
                    case "LikeboxButton":
                        control = m.CreateControl(ControlType.Likebox, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "FacebookButton":
                        control = m.CreateControl(ControlType.Facebook, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "TwitterButton":
                        control = m.CreateControl(ControlType.Twitter, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "InstagramButton":
                        control = m.CreateControl(ControlType.Instagram, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "SocialMediaImageButton":
                        control = m.CreateControl(ControlType.SocialMediaImage, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 100 ? m.Project.SelectedRegion.Width - position.X : 100,
                            m.Project.SelectedRegion.Height - position.Y < 100 ? m.Project.SelectedRegion.Height - position.Y : 100);
                        break;
                    case "YoutubeButton":
                        control = m.CreateControl(ControlType.Youtube, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "LiveButton":
                        control = m.CreateControl(ControlType.Live, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "WeatherButton":
                        control = m.CreateControl(ControlType.Weather, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 300 ? m.Project.SelectedRegion.Width - position.X : 300,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "DateTimeButton":
                        control = m.CreateControl(ControlType.DateTime, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 100 ? m.Project.SelectedRegion.Height - position.Y : 100);
                        break;
                    case "TextButton":
                        control = m.CreateControl(ControlType.Text, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 100 ? m.Project.SelectedRegion.Height - position.Y : 100);
                        break;
                    case "PDFButton":
                        control = m.CreateControl(ControlType.PDF, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "PPTButton":
                        control = m.CreateControl(ControlType.PPT, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                    case "WebBrowserButton":
                        control = m.CreateControl(ControlType.WebBrowser, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 500 ? m.Project.SelectedRegion.Width - position.X : 500,
                            m.Project.SelectedRegion.Height - position.Y < 400 ? m.Project.SelectedRegion.Height - position.Y : 400);
                        break;
                }

                if (control != null)
                {
                    DrowDesignerCanvas(control);
                    DeselectAll();
                    control.IsSelected = true;
                }
            }
        }

        #endregion

        #region Preview
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        Stopwatch stopWatch;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimerText.Text = new DateTime(stopWatch.Elapsed.Ticks).ToString("T");//dispatcherTimer.Interval.ToString();
        }

        Preview window = null;
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            StartPreview();
        }

        private void PlayFullButton_Click(object sender, RoutedEventArgs e)
        {
            StartFullPreview();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopPreview();
        }

        private void StartPreview()
        {
            try
            {
                //Load flight data if exist
                RegionModel rm = (DataContext as MainViewModel).Project.SelectedRegion;
                if (rm != null)
                {
                    string[] data = IOManagerProject.GetFlightId(rm);
                    if (data != null && data.Count() > 0 && !String.IsNullOrEmpty(data[0]))
                    {
                        string xml = CeitconServerHelper.GetDataRecords(data[0]);
                        PreviewControlPanel.FlightList = Utilities.SQLiteHelper.Instance.GetFlightsDataFromXml(xml);
                    }

                    string[] weatherData = Ceitcon_Data.Utilities.IOManagerProject.GetWeatherLocations((DataContext as MainViewModel).Project);
                    if (weatherData != null && weatherData.Count() > 0)
                    {
                        var dict = new Dictionary<string, string>();
                        foreach (string item in weatherData.Distinct().ToList())
                        {
                            string xmlData = CeitconServerHelper.GetWeathers(item);
                            dict.Add(item, xmlData);
                        }
                        PreviewControlPanel.WeatherList = dict;
                    }
                }

                PreviewControlPanel.DataContext = null;
                PreviewControlPanel.DataContext = (DataContext as MainViewModel).Project.SelectedRegion;
                RulerH.Visibility = Visibility.Hidden;
                RulerV.Visibility = Visibility.Hidden;
                DesignerScreen.Visibility = Visibility.Hidden;
                PreviewScreen.Visibility = Visibility.Visible;
                DispatcherTimerText.Visibility = Visibility.Visible;

                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                stopWatch = new Stopwatch();
                stopWatch.Start();
                dispatcherTimer.Start();
            }
            catch (Exception) { }
        }

        private void StartFullPreview()
        {
            try
            {
                if (window != null)
                {
                    window.Close();
                    window = null;
                }

                //Load flight data if exist
                var flightList = new List<FlightModel>();
                var weatherList = new Dictionary<string, string>();
                RegionModel rm = (DataContext as MainViewModel).Project.SelectedRegion;
                if (rm != null)
                {
                    string[] data = IOManagerProject.GetFlightId(rm);
                    if (data != null && data.Count() > 0 && !String.IsNullOrEmpty(data[0]))
                    {
                        string xml = CeitconServerHelper.GetDataRecords(data[0]);
                        flightList = Utilities.SQLiteHelper.Instance.GetFlightsDataFromXml(xml);
                    }

                    string[] weatherData = Ceitcon_Data.Utilities.IOManagerProject.GetWeatherLocations((DataContext as MainViewModel).Project);
                    if (weatherData != null && weatherData.Count() > 0)
                    {
                        var dict = new Dictionary<string, string>();
                        foreach (string item in weatherData.Distinct().ToList())
                        {
                            string xmlData = CeitconServerHelper.GetWeathers(item);
                            dict.Add(item, xmlData);
                        }
                        weatherList = dict;
                    }
                }

                window = new Preview();
                window.FlightList = flightList;
                window.WeatherList = weatherList;
                window.DataContext = (DataContext as MainViewModel).Project.SelectedRegion;
                window.ShowDialog();
            }
            catch (Exception) { }
        }

        private void StopPreview()
        {
            try
            {
                PreviewControlPanel.DataContext = null;
                DesignerScreen.Visibility = Visibility.Visible;
                RulerH.Visibility = Visibility.Visible;
                RulerV.Visibility = Visibility.Visible;
                PreviewScreen.Visibility = Visibility.Hidden;
                DispatcherTimerText.Visibility = Visibility.Hidden;
                dispatcherTimer.Stop();
                if (stopWatch != null)
                    stopWatch.Start();
                DispatcherTimerText.Text = "";

                //Preview
                if (window != null)
                {
                    window.Close();
                    window = null;
                }
            }
            catch (Exception) { }
        }


        #endregion

        #region Events

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete)
                {
                    Delete();
                    e.Handled = true;
                }

                else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    Copy();
                    e.Handled = true;
                }
                else if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    Paste();
                    e.Handled = true;

                }
                else if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    Undo();
                    e.Handled = true;

                }
                else if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    Redo();
                    e.Handled = true;
                }
            }
            catch (Exception) { };
        }

        void RegionItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearCanvas();
            LoadCanvas(((sender as ComboBoxItem).DataContext as RegionModel).SelectedSlide);
        }

        private void SlideList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBoxItem).DataContext is SlideModel)
            {

                SlideModel slide;
                if (e.OriginalSource is System.Windows.Controls.Image)
                {
                    //Check if click on remove button
                    slide = (sender as ListBoxItem).DataContext as SlideModel;
                    if (slide == (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide)
                    {
                        if (slide == (DataContext as MainViewModel).Project.SelectedRegion.Slides.FirstOrDefault())
                        {
                            slide = null;
                            if ((DataContext as MainViewModel).Project.SelectedRegion.Slides.Count > 1)
                                slide = (DataContext as MainViewModel).Project.SelectedRegion.Slides[1];
                        }
                        else
                        {
                            slide = (DataContext as MainViewModel).Project.SelectedRegion.Slides.FirstOrDefault();
                        }
                        (DataContext as MainViewModel).SelectObject(slide);
                        ClearCanvas();
                        LoadCanvas(slide);
                    }
                    //slide = (DataContext as MainViewModel).Project.SelectedRegion.Slides.FirstOrDefault();
                    //(DataContext as MainViewModel).SelectObject(slide);
                }
                else
                {
                    slide = (sender as ListBoxItem).DataContext as SlideModel;
                    (DataContext as MainViewModel).SelectObject(slide);
                    ClearCanvas();
                    LoadCanvas(slide);
                }
            }
        }

        private void PlaylistList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBoxItem).DataContext is PlaylistModel)
            {
                (DataContext as MainViewModel).SelectPlaylist(((sender as ListBoxItem).DataContext as PlaylistModel).Id);
                (DataContext as MainViewModel).SelectObject((sender as ListBoxItem).DataContext);
            }
        }

        private void DesignerCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is ContentControl)
            {
                var cc = e.Source as ContentControl;
                if (cc.DataContext is ControlModel)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        (cc.DataContext as ControlModel).IsSelected = !(cc.DataContext as ControlModel).IsSelected;
                    }
                    else
                    {
                        DeselectAll();
                        (cc.DataContext as ControlModel).IsSelected = true;
                    }

                    (DataContext as MainViewModel).SelectControl(cc.DataContext as ControlModel);
                    (DataContext as MainViewModel).SelectObject(cc.DataContext);
                    Memento.Push((cc.DataContext as ControlModel).Save());
                    Memento.Enable = false;
                }
            }
        }

        private void DesignerCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Memento.Enable = true;
        }

        private void RegionCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoomSlider.Value += zoomSpeed * (e.Delta > 0 ? 1 : -1);
        }

        private void DependList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dep = (e.OriginalSource as TextBlock).DataContext as PlaylistModel;
                var item = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.SelectedPlaylist;
                (DataContext as MainViewModel).AddDepends(item, dep);
                PopupDepends.IsOpen = false;
            }
            catch (Exception) { }
        }

        private void LayersList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;

                (DataContext as MainViewModel).SelectObject(draggedItem.DataContext);
            }
        }

        private void LayersListBox_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var droppedData = e.Data.GetData(typeof(LayerModel)) as LayerModel;
                var target = ((ListBoxItem)(sender)).DataContext as LayerModel;

                int removedIdx = LayerListBox.Items.IndexOf(droppedData);
                int targetIdx = LayerListBox.Items.IndexOf(target);
                if (removedIdx == targetIdx)
                {
                    if (e.OriginalSource is Image && (e.OriginalSource as Image).Parent is ToggleButton)
                    {
                        var tb = ((e.OriginalSource as Image).Parent as ToggleButton);
                        if (tb.Name == "Folder_ToggleButton")
                            droppedData.IsSelected = !droppedData.IsSelected;
                        else if (tb.Name == "Locked_ToggleButton")
                            droppedData.IsLocked = !droppedData.IsLocked;
                        else if (tb.Name == "Visible_ToggleButton")
                            (tb.DataContext as ControlModel).IsVisible = !(tb.DataContext as ControlModel).IsVisible;
                        else if (tb.Name == "LockedSub_ToggleButton")
                            (tb.DataContext as ControlModel).IsLocked = !(tb.DataContext as ControlModel).IsLocked;
                    }
                    return;
                }


                var _empList = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.Layers;
                var _selected = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer;
                if (removedIdx < targetIdx)
                {
                    _empList.Insert(targetIdx + 1, droppedData);
                    _empList.RemoveAt(removedIdx);
                }
                else if (removedIdx > targetIdx)
                {
                    int remIdx = removedIdx + 1;
                    if (_empList.Count + 1 > remIdx)
                    {
                        _empList.Insert(targetIdx, droppedData);
                        _empList.RemoveAt(remIdx);
                    }
                }
                (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer = _selected;
                (DataContext as MainViewModel).ReorderLayers();
            }
            catch (Exception) { }
        }

        private void DeselectAll()
        {
            foreach (ContentControl cc in this.DesignerCanvas.Children)
            {
                (cc.DataContext as ControlModel).IsSelected = false;

                //var converter = new System.Windows.Media.BrushConverter();
                //var a = (cc.Content as Grid).Children[0] as Rectangle;
                //var converter = new System.Windows.Media.BrushConverter();
                //a.Fill = (Brush)converter.ConvertFromString("#FF657259");
            }
        }

        private void NewSlideButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).CreateSlide();
            ClearCanvas();
        }

        private void AddLayerButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).CreateLayer();
        }

        private void DeleteLayerButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).DeleteLayer();
            ClearCanvas();
            LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
        }

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            string value = (sender as MenuItem).Header.ToString();
            switch (value)
            {
                case "Set Content":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.SetContent);
                    break;
                case "Delay":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.Delay);
                    break;
                case "Animate Margin":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.AnimateMargin);
                    break;
                case "Animate Opacity":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.AnimateOpacity);
                    break;
                case "Animate Width":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.AnimateWidth);
                    break;
                case "Animate Height":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.AnimateHeight);
                    break;
                case "Animate Border":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.AnimateBorder);
                    break;
                case "Suspend Playback":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.SuspendPlayback);
                    break;
                case "Resume Playback":
                    (DataContext as MainViewModel).CreatePlaylist(PlaylistType.ResumePlayback);
                    break;
            }
        }

        private void SnapButton_Click(object sender, RoutedEventArgs e)
        {
            SpanEffect.Enable = (sender as ToggleButton).IsChecked == true;
        }

        private void GridButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                DrowDots((DataContext as MainViewModel).Project.SelectedResolution);
            }
            else
            {
                ClearDots();
            }
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value -= zoomSpeed;
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value += zoomSpeed;
        }

        private void ZoomFullButton_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 1;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            ControlModel c = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl;
            c.Source = OpenFile();
        }

        private void PopupCloseButton_Click(object sender, RoutedEventArgs e)
        {
            PopupDepends.IsOpen = false;
        }

        private void AlignButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void AlignItemButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).AlignControl((sender as Button).Name);
        }

        #endregion

        #region Methods
        private void Delete()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if ((DataContext as MainViewModel).Project.SelectedObject is ControlModel)
                {
                    var list = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.Where(_ => _.IsSelected == true).ToArray();
                    foreach (var control in list)
                    {
                        foreach (var item in DesignerCanvas.Children)
                        {
                            if (item is ContentControl && (item as ContentControl).Name == control.Name)
                            {
                                this.DesignerCanvas.Children.Remove(item as ContentControl);
                                break;
                            }
                        }
                    }
                    (DataContext as MainViewModel).DeleteControl(list);
                }
                else if ((DataContext as MainViewModel).Project.SelectedObject is SlideModel)
                {
                    (DataContext as MainViewModel).DeleteSlide();
                    (DataContext as MainViewModel).Project.SelectedObject = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide;
                    ClearCanvas();
                    if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                        LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
                }
            }
        }

        private void Copy()
        {
            if ((DataContext as MainViewModel).Project.SelectedObject is ControlModel)
            {
                _Copy = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.Where(_ => _.IsSelected == true).ToArray();
            }
            else
            {
                _Copy = (DataContext as MainViewModel).Project.SelectedObject;
            }
        }

        private void Paste()
        {
            if (_Copy is PlaylistModel)
            {
                (DataContext as MainViewModel).CopyPlaylist(_Copy as PlaylistModel);
            }
            else if (_Copy is ControlModel)
            {
                DrowDesignerCanvas((DataContext as MainViewModel).CopyControl(_Copy as ControlModel));
            }
            else if (_Copy is ControlModel[])
            {
                foreach (var item in _Copy as ControlModel[])
                {
                    DrowDesignerCanvas((DataContext as MainViewModel).CopyControl(item as ControlModel));
                }
            }
            else if (_Copy is LayerModel)
            {
                foreach (ControlModel control in (DataContext as MainViewModel).CopyLayer(_Copy as LayerModel).Controls)
                {
                    DrowDesignerCanvas(control);
                }
            }
            else if (_Copy is SlideModel)
            {
                (DataContext as MainViewModel).CopySlide(_Copy as SlideModel);
            }
            else if (_Copy is RegionModel)
            {
                (DataContext as MainViewModel).CopyRegion(_Copy as RegionModel);
            }
        }

        private void Undo()
        {
            var obj = Memento.Pop();
            if (obj is PlaylistModel)
            {
                var item = (DataContext as MainViewModel).FindPlaylist(obj as PlaylistModel);
                if (item != null)
                {
                    Memento.PushR(item.Save());
                    item.Restore(obj);
                }
            }
            else if (obj is PlaylistModel[])
            {
                Memento.PushR((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.ToArray());
                (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist = new ObservableCollection<PlaylistModel>(obj as PlaylistModel[]);
                (DataContext as MainViewModel).SelectPlaylist((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
            }
            else if (obj is ControlModel)
            {
                var item = (DataContext as MainViewModel).FindControl(obj as ControlModel);
                if (item != null)
                {
                    Memento.PushR(item.Save());
                    item.Restore(obj as ControlModel);
                }
            }
            else if (obj is ControlModel[])
            {
                Memento.PushR((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.ToArray());
                (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls = new ObservableCollection<ControlModel>(obj as ControlModel[]);
                (DataContext as MainViewModel).SelectControl((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
            }
            else if (obj is LayerModel)
            {
                var item = (DataContext as MainViewModel).FindLayer(obj as LayerModel);
                if (item != null)
                {
                    Memento.PushR(item.Save());
                    item.Restore(obj as LayerModel);
                }
            }
            else if (obj is LayerModel[])
            {
                Memento.PushR((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.Layers.ToArray());
                (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.Layers = new ObservableCollection<LayerModel>(obj as LayerModel[]);
                (DataContext as MainViewModel).SelectLayer((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.Layers.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
            }
            else if (obj is SlideModel)
            {
                var item = (DataContext as MainViewModel).FindSlide(obj as SlideModel);
                if (item != null)
                {
                    Memento.PushR(item.Save());
                    item.Restore(obj as SlideModel);
                }
            }
            else if (obj is SlideModel[])
            {
                Memento.PushR((DataContext as MainViewModel).Project.SelectedRegion.Slides.ToArray());
                (DataContext as MainViewModel).Project.SelectedRegion.Slides = new ObservableCollection<SlideModel>(obj as SlideModel[]);
                (DataContext as MainViewModel).SelectSlide((DataContext as MainViewModel).Project.SelectedRegion.Slides.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
            }
            else if (obj is RegionModel)
            {
                var item = (DataContext as MainViewModel).FindRegion(obj as RegionModel);
                if (item != null)
                {
                    Memento.PushR(item.Save());
                    item.Restore(obj as RegionModel);
                }
            }
        }

        private void Redo()
        {
            var obj = Memento.PopR();
            if (obj is PlaylistModel)
            {
                var item = (DataContext as MainViewModel).FindPlaylist(obj as PlaylistModel);
                if (item != null)
                {
                    Memento.Push(item.Save());
                    item.Restore(obj as PlaylistModel);
                }
            }
            else if (obj is PlaylistModel[])
            {
                Memento.Push((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.ToArray());
                (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist = new ObservableCollection<PlaylistModel>(obj as PlaylistModel[]);
                (DataContext as MainViewModel).SelectPlaylist((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl.Playlist.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
            }
            else if (obj is ControlModel)
            {
                var item = (DataContext as MainViewModel).FindControl(obj as ControlModel);
                if (item != null)
                {
                    Memento.Push(item.Save());
                    item.Restore(obj as ControlModel);
                }
            }
            else if (obj is ControlModel[])
            {
                Memento.Push((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.ToArray());
                (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls = new ObservableCollection<ControlModel>(obj as ControlModel[]);
                (DataContext as MainViewModel).SelectControl((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.Controls.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
            }
            else if (obj is LayerModel)
            {
                var item = (DataContext as MainViewModel).FindLayer(obj as LayerModel);
                if (item != null)
                {
                    Memento.Push(item.Save());
                    item.Restore(obj as LayerModel);
                }
            }
            else if (obj is LayerModel[])
            {
                Memento.Push((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.Layers.ToArray());
                (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.Layers = new ObservableCollection<LayerModel>(obj as LayerModel[]);
                (DataContext as MainViewModel).SelectLayer((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.Layers.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
            }
            else if (obj is SlideModel)
            {
                var item = (DataContext as MainViewModel).FindSlide(obj as SlideModel);
                if (item != null)
                {
                    Memento.Push(item.Save());
                    item.Restore(obj as SlideModel);
                }
            }
            else if (obj is SlideModel[])
            {
                Memento.Push((DataContext as MainViewModel).Project.SelectedRegion.Slides.ToArray());
                (DataContext as MainViewModel).Project.SelectedRegion.Slides = new ObservableCollection<SlideModel>(obj as SlideModel[]);
                (DataContext as MainViewModel).SelectSlide((DataContext as MainViewModel).Project.SelectedRegion.Slides.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide);
            }
            else if (obj is RegionModel)
            {
                var item = (DataContext as MainViewModel).FindRegion(obj as RegionModel);
                if (item != null)
                {
                    Memento.Push(item.Save());
                    item.Restore(obj as RegionModel);
                }
            }
        }

        private string OpenFile()
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide.SelectedLayer.SelectedControl;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                switch (c.Type)
                {
                    //case ControlType.Image:
                    //    {
                    //        dlg.DefaultExt = ".jpg";
                    //        dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|All Files (*.*)|*.*";
                    //    }
                    //    break;
                    //case ControlType.Video:
                    //    {
                    //        dlg.DefaultExt = ".mp4";
                    //        dlg.Filter = "MP4 Files (*.mp4)|*.mp4|All Files (*.*)|*.*";
                    //    }
                    //    break;
                    //case ControlType.GifAnim:
                    //    {
                    //        dlg.DefaultExt = ".gif";
                    //        dlg.Filter = "GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";
                    //    }
                    //    break;
                    case ControlType.Image:
                        {
                            dlg.DefaultExt = ".jpg";
                            dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|All Files (*.*)|*.*";
                        }
                        break;
                    case ControlType.Video:
                        {
                            dlg.DefaultExt = ".mp4";
                            dlg.Filter = "MP4 Files (*.mp4)|*.mp4|AVI Files (*.avi)|*.avi|MOV Files (*.mov)|*.mov|WMV Files (*.wmv)|*.wmv|MKV Files (*.mkv)|*.mkv|All Files (*.*)|*.*";
                        }
                        break;
                    case ControlType.GifAnim:
                        {
                            dlg.DefaultExt = ".gif";
                            dlg.Filter = "GIF Files (*.gif)|*.gif";
                        }
                        break;
                    default:
                        dlg.DefaultExt = ".*";
                        dlg.Filter = "All Files (*.*)|*.*";
                        break;
                }
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    return filename;
                }
            }
            catch (Exception) { }
            return null;
        }


        #region Colours
        private void CopyBrushClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ColorBox.ColorBox;
                if (cb.Brush == null)
                {
                    (DataContext as MainViewModel).CopyBrush = null;
                }
                else
                {
                    (DataContext as MainViewModel).CopyBrush = cb.Brush.Clone();
                }
            }
            catch (Exception) { }
        }

        private void PasteBrushClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ColorBox.ColorBox;
                if ((DataContext as MainViewModel).CopyBrush == null)
                {
                    cb.Brush = null;
                }
                else
                {
                    cb.Brush = (DataContext as MainViewModel).CopyBrush.Clone();
                }
            }
            catch (Exception) { }
        }
        #endregion

        #endregion

    }

    public class WebBrowserUtility
    {
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = !String.IsNullOrEmpty(uri) ? new Uri(uri) : null;
            }
        }

    }
}
