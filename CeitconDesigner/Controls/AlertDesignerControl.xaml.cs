using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Data.Utilities;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.ViewModel;
using System;
using System.Collections.ObjectModel;
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

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for AlertDesignerControl.xaml
    /// </summary>
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public partial class AlertDesignerControl : UserControl
    {
        private static double zoomSpeed = 0.05;
        private object _Copy;

        public AlertDesignerControl()
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
                if ((DataContext as MainViewModel).Project.SelectedAlert != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
            }
            else
            {
                SpanEffect.Enable = false;
                tbSnap.IsChecked = false;
                ClearCanvas();

            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel)
                (DataContext as MainViewModel).SelectedTool = (sender as TabControl).SelectedIndex;
            
            //Visibility logic
            if ((DataContext as MainViewModel).SelectedTool == 3)
            {
                PropertiesTab.SelectedIndex = 0;
                (DataContext as MainViewModel).Project.SelectedAlert = (DataContext as MainViewModel).Project.PrayerAlerts.FirstOrDefault();
                (DataContext as MainViewModel).SelectObject((DataContext as MainViewModel).Project.PrayerAlerts.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedAlert is AlertModel)
                {
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
                }

            }
            else if ((DataContext as MainViewModel).SelectedTool == 4)
            {
                PropertiesTab.SelectedIndex = 1;
                (DataContext as MainViewModel).Project.SelectedAlert = (DataContext as MainViewModel).Project.GlobalAlerts.FirstOrDefault();
                (DataContext as MainViewModel).SelectObject((DataContext as MainViewModel).Project.GlobalAlerts.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedAlert is AlertModel)
                {
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
                }
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
            this.AlertCanvas.Children.Clear();
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

        private void LoadCanvas(AlertModel alert)
        {
           foreach (ControlModel control in alert.Controls)
           {
               DrowDesignerCanvas(control);
           }
        }

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
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                //Border
                Border border = new Border() { Margin = new Thickness(1, 1, 1, 1),/* CornerRadius = new CornerRadius(2,2,2,2),*/  IsHitTestVisible = false };
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
                    case ControlType.PrayerImage:
                        {
                            //item.Source = @"C:\Temp\2.jpg";
                            var img = new Image() { RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false }; //Source = new BitmapImage(new Uri((item.Source))),
                            img.SetBinding(Image.SourceProperty, new Binding() { Path = new PropertyPath("Fajr"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.StretchProperty, new Binding() { Path = new PropertyPath("Stretch"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            img.SetBinding(Image.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = img;
                        }
                        break;
                    case ControlType.PrayerVideo:
                        {
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
                    case ControlType.Youtube:
                    case ControlType.PrayerYoutube:
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
                    case ControlType.Text:
                        {
                            TextBlock text = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, TextWrapping = TextWrapping.Wrap, RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            text.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Text"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.ForegroundProperty, new Binding() { Path = new PropertyPath("Foreground"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontFamilyProperty, new Binding() { Path = new PropertyPath("FontFamily"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = text;
                        }
                        break;
                    case ControlType.PrayerText:
                        {
                            TextBlock text = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, TextWrapping = TextWrapping.Wrap, RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            text.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Fajr"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.ForegroundProperty, new Binding() { Path = new PropertyPath("Foreground"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.FontFamilyProperty, new Binding() { Path = new PropertyPath("FontFamily"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            text.SetBinding(TextBlock.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            border.Child = text;
                        }
                        break;
                    case ControlType.Video:
                        {
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

                            //var me = new MediaElement() { RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
                            //me.SetBinding(MediaElement.SourceProperty, new Binding() { Path = new PropertyPath("Source"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //me.SetBinding(MediaElement.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("HorizontalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //me.SetBinding(MediaElement.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("VerticalAlignment"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //me.SetBinding(MediaElement.StretchProperty, new Binding() { Path = new PropertyPath("Stretch"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //me.SetBinding(MediaElement.RenderTransformProperty, new Binding() { Path = new PropertyPath("ScaleTransform"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                            //border.Child = me;
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
                    default:
                        //border.Child = new Rectangle() { Fill = fillBrush, Stroke = stokeBrush, StrokeThickness = 5, StrokeDashArray = { 4, 2 }, IsHitTestVisible = false };
                        break;
                }

                //Component Name
                if (item.Type != ControlType.Text)
                {
                    TextBlock tb = new TextBlock() { Foreground = Brushes.White, Background = Brushes.SeaGreen, FontSize = 30, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, IsHitTestVisible = false };
                    tb.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Name"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    grid.Children.Add(tb);
                }
                cc.Content = grid;
                

                //Context menu
                var cm = new ContextMenu();
                var mi = new MenuItem() { Header = String.Format("Delete {0}", item.Name) };
                mi.Click += delegate { (DataContext as MainViewModel).DeleteAlertControl(item); this.AlertCanvas.Children.Remove(cc); };
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

                this.AlertCanvas.Children.Add(cc);
            }
        }
        #endregion

        #region DragDrop Elements

        private Button draggedItem;
        //private Point startDragPoint;

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //draggedItem = sender as RadRibbonButton;
            draggedItem = sender as Button;
            // startDragPoint = e.GetPosition(null);
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && draggedItem != null)
            {
                Point position = e.GetPosition(null);
                //DragDrop.DoDragDrop(draggedItem, draggedItem.Content, DragDropEffects.Copy);
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
            Point position = e.GetPosition(this.AlertCanvas);
            var m = (DataContext as MainViewModel);
            if (m.Project.Alerts.Count == 0)
            {
                MessageBox.Show("Please create one Alert first in Alert tab", "Info", MessageBoxButton.OK);
                return;
            }

            if (draggedItem != null)
            {
                switch (draggedItem.Name)
                {
                    case "PrayerImageButton":
                    case "PrayerImageGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.PrayerImage, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;
                    case "PrayerTextButton":
                    case "PrayerTextGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.PrayerText, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;

                    case "PrayerVideoButton":
                    case "PrayerVideoGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.PrayerVideo, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;

                    case "PrayerYoutubeButton":
                    case "PrayerYoutubeGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.PrayerYoutube, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;

                    case "ImageButton":
                    case "ImageGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.Image, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;
                    case "TextButton":
                    case "TextGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.Text, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;
                    case "VideoButton":
                    case "VideoGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.Video, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;
                    case "GifAnimButton":
                    case "GifAnimGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.GifAnim, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;
                    case "LiveButton":
                    case "LiveGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.Live, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;
                    case "YoutubeButton":
                    case "YoutubeGButton":
                        DrowDesignerCanvas(m.CreateAlertControl(ControlType.Youtube, position.X, position.Y,
                            m.Project.SelectedRegion.Width - position.X < 400 ? m.Project.SelectedRegion.Width - position.X : 400,
                            m.Project.SelectedRegion.Height - position.Y < 300 ? m.Project.SelectedRegion.Height - position.Y : 300));
                        break;
                }
            }
        }

        #endregion

        //#region Preview
        //System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        //Stopwatch stopWatch;
        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{
        //    DispatcherTimerText.Text = new DateTime(stopWatch.Elapsed.Ticks).ToString("T");//dispatcherTimer.Interval.ToString();
        //}

        //private void PlayButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        PreviewControlPanel.DataContext = null;
        //        PreviewControlPanel.DataContext = (DataContext as MainViewModel).Project.SelectedRegion;
        //        RulerH.Visibility = Visibility.Hidden;
        //        RulerV.Visibility = Visibility.Hidden;
        //        DesignerScreen.Visibility = Visibility.Hidden;
        //        PreviewScreen.Visibility = Visibility.Visible;
        //        DispatcherTimerText.Visibility = Visibility.Visible;

        //        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        //        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        //        stopWatch = new Stopwatch();
        //        stopWatch.Start();
        //        dispatcherTimer.Start();
        //    }
        //    catch (Exception) { }
        //}

        //Preview window = null;
        //private void PlayFullButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (window != null)
        //        {
        //            window.Close();
        //            window = null;
        //        }
        //        window = new Preview();
        //        window.DataContext = (DataContext as MainViewModel).Project.SelectedRegion;
        //        window.Show();
        //    }
        //    catch (Exception) { }
        //}

        //private void StopButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        PreviewControlPanel.DataContext = null;
        //        DesignerScreen.Visibility = Visibility.Visible;
        //        RulerH.Visibility = Visibility.Visible;
        //        RulerV.Visibility = Visibility.Visible;
        //        PreviewScreen.Visibility = Visibility.Hidden;
        //        DispatcherTimerText.Visibility = Visibility.Hidden;
        //        dispatcherTimer.Stop();
        //        if (stopWatch != null)
        //            stopWatch.Start();
        //        DispatcherTimerText.Text = "";

        //        //Preview
        //        if (window != null)
        //        {
        //            window.Close();
        //            window = null;
        //        }
        //    }
        //    catch (Exception) { }
        //}


        //#endregion

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

        private void AlertList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBoxItem).DataContext is AlertModel)
            {
                var alert = (sender as ListBoxItem).DataContext as AlertModel;
                (DataContext as MainViewModel).SelectObject(alert);
                ClearCanvas();
                LoadCanvas(alert);
            }
        }

        private void PlaylistList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBoxItem).DataContext is PlaylistModel)
            {
                (DataContext as MainViewModel).SelectPlaylistAlert(((sender as ListBoxItem).DataContext as PlaylistModel).Id);
                (DataContext as MainViewModel).SelectObject((sender as ListBoxItem).DataContext);
            }
        }

        private void AlertCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

                    (DataContext as MainViewModel).SelectAlertControl(cc.DataContext as ControlModel);
                    (DataContext as MainViewModel).SelectObject(cc.DataContext);
                    Memento.Push((cc.DataContext as ControlModel).Save());
                    Memento.Enable = false;
                }
            }
        }

        private void AlertCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
                var item = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl.SelectedPlaylist;
                (DataContext as MainViewModel).AddDepends(item, dep);
                PopupDepends.IsOpen = false;
            }
            catch (Exception) { }
        }

        /*
        private void SlotList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;

                (DataContext as MainViewModel).SelectObject(draggedItem.DataContext);
            }
        }
        
        private void SlotListBox_Drop(object sender, DragEventArgs e)
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
        */
        private void DeselectAll()
        {
            foreach (ContentControl cc in this.AlertCanvas.Children)
            {
                (cc.DataContext as ControlModel).IsSelected = false;

                //var converter = new System.Windows.Media.BrushConverter();
                //var a = (cc.Content as Grid).Children[0] as Rectangle;
                //var converter = new System.Windows.Media.BrushConverter();
                //a.Fill = (Brush)converter.ConvertFromString("#FF657259");
            }
        }

        private void NewPrayerAlertButton_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project.PrayerAlerts.Count == 1)
            {
                MessageBox.Show("Can have only one Prayer alert per project");
                return;
            }
            (DataContext as MainViewModel).CreateAlert(AlertType.Prayer);
            ClearCanvas();
        }

        private void NewGlobalAlertButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).CreateAlert(AlertType.Global);
            ClearCanvas();
        }

        //private void NewSlideButton_Click(object sender, RoutedEventArgs e)
        //{
        //    (DataContext as MainViewModel).CreateSlide();
        //    ClearCanvas();
        //}

        private void AddSlotButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).CreateSlot();
        }

        private void DeleteSlotButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).DeleteSlot();
        }

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            string value = (sender as MenuItem).Header.ToString();
            switch (value)
            {
                case "Set Content":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.SetContent);
                    break;
                case "Delay":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.Delay);
                    break;
                case "Animate Margin":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.AnimateMargin);
                    break;
                case "Animate Opacity":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.AnimateOpacity);
                    break;
                case "Animate Width":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.AnimateWidth);
                    break;
                case "Animate Height":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.AnimateHeight);
                    break;
                case "Animate Border":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.AnimateBorder);
                    break;
                case "Suspend Playback":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.SuspendPlayback);
                    break;
                case "Resume Playback":
                    (DataContext as MainViewModel).CreatePlaylistAlert(PlaylistType.ResumePlayback);
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
            ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
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
            (DataContext as MainViewModel).AlignAlertControl((sender as Button).Name);
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
                    var list = (DataContext as MainViewModel).Project.SelectedAlert.Controls.Where(_ => _.IsSelected == true).ToArray();
                    foreach (var control in list)
                    {
                        foreach (var item in AlertCanvas.Children)
                        {
                            if (item is ContentControl && (item as ContentControl).Name == control.Name)
                            {
                                this.AlertCanvas.Children.Remove(item as ContentControl);
                                break;
                            }
                        }
                    }
                    (DataContext as MainViewModel).DeleteAlertControl(list);
                }
                else if ((DataContext as MainViewModel).Project.SelectedObject is AlertModel)
                {
                    (DataContext as MainViewModel).DeleteAlert();
                    (DataContext as MainViewModel).Project.SelectedObject = (DataContext as MainViewModel).Project.SelectedAlert;
                    ClearCanvas();
                    if ((DataContext as MainViewModel).Project.SelectedAlert != null)
                        LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
                }
            }
        }

        private void Copy()
        {
            if ((DataContext as MainViewModel).Project.SelectedObject is ControlModel)
            {
                _Copy = (DataContext as MainViewModel).Project.SelectedAlert.Controls.Where(_ => _.IsSelected == true).ToArray();
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
                (DataContext as MainViewModel).CopyPlaylistAlert(_Copy as PlaylistModel);
            }
            else if (_Copy is ControlModel)
            {
                DrowDesignerCanvas((DataContext as MainViewModel).CopyAlertControl(_Copy as ControlModel));
            }
            else if (_Copy is ControlModel[])
            {
                foreach (var item in _Copy as ControlModel[])
                {
                    DrowDesignerCanvas((DataContext as MainViewModel).CopyAlertControl(item as ControlModel));
                }
            }
            else if (_Copy is AlertModel)
            {
                foreach (ControlModel control in (DataContext as MainViewModel).CopyAlert(_Copy as AlertModel).Controls)
                {
                    DrowDesignerCanvas(control);
                }
            }
        }

        private void Undo()
        {
            var obj = Memento.Pop();
            if (obj is PlaylistModel)
            {
                var item = (DataContext as MainViewModel).FindPlaylistAlert(obj as PlaylistModel);
                if (item != null)
                {
                    Memento.PushR(item.Save());
                    item.Restore(obj);
                }
            }
            else if (obj is PlaylistModel[])
            {
                Memento.PushR((DataContext as MainViewModel).Project.SelectedAlert.SelectedControl.Playlist.ToArray());
                (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl.Playlist = new ObservableCollection<PlaylistModel>(obj as PlaylistModel[]);
                (DataContext as MainViewModel).SelectPlaylistAlert((DataContext as MainViewModel).Project.SelectedAlert.SelectedControl.Playlist.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedAlert != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
            }
            else if (obj is ControlModel)
            {
                var item = (DataContext as MainViewModel).FindAlertControl(obj as ControlModel);
                if (item != null)
                {
                    Memento.PushR(item.Save());
                    item.Restore(obj as ControlModel);
                }
            }
            else if (obj is ControlModel[])
            {
                Memento.PushR((DataContext as MainViewModel).Project.SelectedAlert.Controls.ToArray());
                (DataContext as MainViewModel).Project.SelectedAlert.Controls = new ObservableCollection<ControlModel>(obj as ControlModel[]);
                (DataContext as MainViewModel).SelectAlertControl((DataContext as MainViewModel).Project.SelectedAlert.Controls.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedAlert != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
            }
            else if (obj is AlertModel)
            {
                var item = (DataContext as MainViewModel).FindAlert(obj as AlertModel);
                if (item != null)
                {
                    Memento.PushR(item.Save());
                    item.Restore(obj as AlertModel);
                }
            }
            else if (obj is AlertModel[])
            {
                Memento.PushR((DataContext as MainViewModel).Project.Alerts.ToArray());
                (DataContext as MainViewModel).Project.Alerts = new ObservableCollection<AlertModel>(obj as AlertModel[]);
                (DataContext as MainViewModel).SelectAlert((DataContext as MainViewModel).Project.Alerts.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedAlert != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
            }
        }

        private void Redo()
        {
            var obj = Memento.PopR();
            if (obj is PlaylistModel)
            {
                var item = (DataContext as MainViewModel).FindPlaylistAlert(obj as PlaylistModel);
                if (item != null)
                {
                    Memento.Push(item.Save());
                    item.Restore(obj as PlaylistModel);
                }
            }
            else if (obj is PlaylistModel[])
            {
                Memento.Push((DataContext as MainViewModel).Project.SelectedAlert.SelectedControl.Playlist.ToArray());
                (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl.Playlist = new ObservableCollection<PlaylistModel>(obj as PlaylistModel[]);
                (DataContext as MainViewModel).SelectPlaylistAlert((DataContext as MainViewModel).Project.SelectedAlert.SelectedControl.Playlist.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedAlert != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
            }
            else if (obj is ControlModel)
            {
                var item = (DataContext as MainViewModel).FindAlertControl(obj as ControlModel);
                if (item != null)
                {
                    Memento.Push(item.Save());
                    item.Restore(obj as ControlModel);
                }
            }
            else if (obj is ControlModel[])
            {
                Memento.Push((DataContext as MainViewModel).Project.SelectedAlert.Controls.ToArray());
                (DataContext as MainViewModel).Project.SelectedAlert.Controls = new ObservableCollection<ControlModel>(obj as ControlModel[]);
                (DataContext as MainViewModel).SelectAlertControl((DataContext as MainViewModel).Project.SelectedAlert.Controls.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedAlert != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
            }
            else if (obj is AlertModel)
            {
                var item = (DataContext as MainViewModel).FindAlert(obj as AlertModel);
                if (item != null)
                {
                    Memento.Push(item.Save());
                    item.Restore(obj as AlertModel);
                }
            }
            else if (obj is AlertModel[])
            {
                Memento.Push((DataContext as MainViewModel).Project.Alerts.ToArray());
                (DataContext as MainViewModel).Project.Alerts = new ObservableCollection<AlertModel>(obj as AlertModel[]);
                (DataContext as MainViewModel).SelectAlert((DataContext as MainViewModel).Project.Alerts.FirstOrDefault());
                ClearCanvas();
                if ((DataContext as MainViewModel).Project.SelectedAlert != null)
                    LoadCanvas((DataContext as MainViewModel).Project.SelectedAlert);
            }
        }

        private string OpenFile()
        {
            try
            {
                ControlModel c = (DataContext as MainViewModel).Project.SelectedAlert.SelectedControl;
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
        #endregion

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
    }


}
