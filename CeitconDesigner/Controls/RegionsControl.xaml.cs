using Ceitcon_Data.Model;
using Ceitcon_Data.Utilities;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.View;
using Ceitcon_Designer.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for RegionsControl.xaml
    /// </summary>
    public partial class RegionsControl : UserControl
    {
        private static double zoomSpeed = 0.05;
        protected void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            item.IsSelected = true;
        }

        public RegionsControl()
        {
            InitializeComponent();
            this.KeyDown += HandleKeyPress;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var project = (DataContext as MainViewModel).Project;
            if ((bool)e.NewValue == true && project != null)
            {
                ClearRegions();
                if ((DataContext as MainViewModel).Project.SelectedRegion != null)
                    CreateRegionCanvas((DataContext as MainViewModel).Project.SelectedRegion);
            }
        }
        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DrowLines((DataContext as MainViewModel).Project.SelectedResolution, (DataContext as MainViewModel).Project.SelectedMonitor);
        //}

        private object _Copy;

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

        private ICommand _RegionFillCommand;
        public ICommand RegionFillCommand { get { return _RegionFillCommand ?? (_RegionFillCommand = new CommandHandler(() => RegionFill(), RegionFill_CanExecute())); } }
        public void RegionFill()
        {
            var m = (DataContext as MainViewModel);
            if (m.Project.Regions.Count > 0)
            {
                MessageBox.Show("Region already exist.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            CreateRegionCanvas(m.CreateRegion(0, 0, m.Project.SelectedResolution.Width, m.Project.SelectedResolution.Height));
        }

        public bool RegionFill_CanExecute()
        {
            return true;
        }

        private ICommand _RegionSplitHCommand;
        public ICommand RegionSplitHCommand { get { return _RegionSplitHCommand ?? (_RegionSplitHCommand = new CommandHandler(() => RegionSplitH(), RegionSplitH_CanExecute())); } }
        public void RegionSplitH()
        {
            var m = (DataContext as MainViewModel);
            double w = m.Project.SelectedResolution.Width / 2;
            double h = m.Project.SelectedResolution.Height;
            CreateRegionCanvas((DataContext as MainViewModel).CreateRegion(0, 0, w, h));
            CreateRegionCanvas((DataContext as MainViewModel).CreateRegion(w, 0, w, h));
        }

        public bool RegionSplitH_CanExecute()
        {
            return true;
        }

        private ICommand _RegionSplitVCommand;
        public ICommand RegionSplitVCommand { get { return _RegionSplitVCommand ?? (_RegionSplitVCommand = new CommandHandler(() => RegionSplitV(), RegionSplitV_CanExecute())); } }
        public void RegionSplitV()
        {
            var m = (DataContext as MainViewModel);
            double w = m.Project.SelectedResolution.Width;
            double h = m.Project.SelectedResolution.Height / 2;
            CreateRegionCanvas((DataContext as MainViewModel).CreateRegion(0, 0, w, h));
            CreateRegionCanvas((DataContext as MainViewModel).CreateRegion(0, h, w, h));
        }

        public bool RegionSplitV_CanExecute()
        {
            return true;
        }

        private ICommand _RegionQuadCommand;
        public ICommand RegionQuadCommand { get { return _RegionQuadCommand ?? (_RegionQuadCommand = new CommandHandler(() => RegionQuad(), RegionQuad_CanExecute())); } }
        public void RegionQuad()
        {
            var m = (DataContext as MainViewModel);
            double w = m.Project.SelectedResolution.Width / 2;
            double h = m.Project.SelectedResolution.Height / 2;
            CreateRegionCanvas((DataContext as MainViewModel).CreateRegion(0, 0, w, h));
            CreateRegionCanvas((DataContext as MainViewModel).CreateRegion(0, h, w, h));
            CreateRegionCanvas((DataContext as MainViewModel).CreateRegion(w, 0, w, h));
            CreateRegionCanvas((DataContext as MainViewModel).CreateRegion(w, h, w, h));
        }

        public bool RegionQuad_CanExecute()
        {
            return true;
        }
        #endregion

        #region Regions
        private void DrowLines(ResolutionModel resolution, MonitorModel monitor)
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
                        X1 = resolution.Width / monitor.Horizontal * i,
                        Y1 = 0,
                        X2 = resolution.Width / monitor.Horizontal * i,
                        Y2 = resolution.Height,
                        StrokeThickness = thickness,
                        SnapsToDevicePixels = true,
                    };
                    line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                    Panel.SetZIndex(line, 100);
                    this.RegionCanvas.Children.Add(line);
                }
            }

            if (monitor.Vertical > 1)
            {
                for (int i = 1; i < monitor.Vertical; i++)
                {
                    var line = new Line()
                    {
                        Stroke = colour,
                        X1 = 0,
                        Y1 = resolution.Height / monitor.Vertical * i,
                        X2 = resolution.Width,
                        Y2 = resolution.Height / monitor.Vertical * i,
                        StrokeThickness = thickness,
                        SnapsToDevicePixels = true,
                    };
                    line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                    Panel.SetZIndex(line, 100);
                    this.RegionCanvas.Children.Add(line);
                }
            }
        }

        private void ClearLines()
        {
            this.RegionCanvas.Children.OfType<Line>().Where(_ => _.GetType() == typeof(Line)).ToList().ForEach(_ => RegionCanvas.Children.Remove(_));
        }

        private void ClearRegions()
        {
            this.RegionCanvas.Children.OfType<ContentControl>().Where(_ => _.GetType() == typeof(ContentControl)).ToList().ForEach(_ => this.RegionCanvas.Children.Remove(_));
        }

        private void CreateRegionCanvas(RegionModel item)
        {
            if (item != null)
            {
                DeselectAll();
                ContentControl cc = new ContentControl()
                {
                    Name = item.Name,
                    Width = item.Width,
                    Height = item.Height,
                    MinWidth = 30,
                    MinHeight = 30,
                    Template = this.FindResource("DesignerItemTemplate") as ControlTemplate,
                    DataContext = item,
                };

                Grid grid = new Grid();
                Rectangle rectangle = new Rectangle() { Fill = Brushes.SeaGreen, Stroke = Brushes.DarkSeaGreen, StrokeThickness = 5, StrokeDashArray = { 4, 2 }, IsHitTestVisible = false };
                rectangle.SetBinding(Rectangle.FillProperty, new Binding() { Path = new PropertyPath("IsSelected"), Source = item, Converter = new Converters.BoolToBrushConverter(), Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                grid.Children.Add(rectangle);

                //Component Name
                TextBlock tb = new TextBlock() { Foreground = Brushes.White, FontSize = 30, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, IsHitTestVisible = false };
                tb.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Name"), Source = item, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                grid.Children.Add(tb);
                cc.Content = grid;

                //Context menu
                var cm = new ContextMenu();
                var mi = new MenuItem() { Header = String.Format("Delete {0}", item.Name) };
                mi.Click += delegate { (DataContext as MainViewModel).DeleteRegion(item); this.RegionCanvas.Children.Remove(cc); };
                cm.Items.Add(mi);
                cc.ContextMenu = cm;

                this.RegionCanvas.Children.Add(cc);
                Canvas.SetLeft(cc, item.X);
                Canvas.SetTop(cc, item.Y);

                cc.SetBinding(ContentControl.NameProperty, new Binding() { Path = new PropertyPath("Name"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(ContentControl.WidthProperty, new Binding() { Path = new PropertyPath("Width"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(ContentControl.HeightProperty, new Binding() { Path = new PropertyPath("Height"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(Canvas.LeftProperty, new Binding() { Path = new PropertyPath("X"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                cc.SetBinding(Canvas.TopProperty, new Binding() { Path = new PropertyPath("Y"), Source = item, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
        }

        private void RegionCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is ContentControl)
            {
                var cc = e.Source as ContentControl;
                if (cc.DataContext is RegionModel)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        (cc.DataContext as RegionModel).IsSelected = !(cc.DataContext as RegionModel).IsSelected;
                    }
                    else
                    {
                        DeselectAll();
                        (cc.DataContext as RegionModel).IsSelected = true;
                    }

                    (DataContext as MainViewModel).SelectRegion(cc.DataContext as RegionModel);
                    (DataContext as MainViewModel).SelectObject(cc.DataContext);

                    Memento.Push((cc.DataContext as RegionModel).Save());
                    Memento.Enable = false;
                }
            }
        }

        private void RegionCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Memento.Enable = true;
        }

        private void RegionCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoomSlider.Value += zoomSpeed * (e.Delta > 0 ? 1 : -1);
        }

        private void DeselectAll()
        {
            foreach (var cc in this.RegionCanvas.Children)
            {
                if (cc is ContentControl)
                    ((cc as ContentControl).DataContext as RegionModel).IsSelected = false;
            }
        }

        private void NewSlideButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).CreateSlide();
        }

        private void MoveSlideUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var region = (this.DataContext as MainViewModel).Project.SelectedRegion;
                if (region.Slides.Count > 1)
                {
                    var q = region.Slides.IndexOf(region.Slides.Where(_ => _.Id == region.SelectedSlide.Id).FirstOrDefault());
                    if (q > 0)
                    {
                        region.Slides.Move(q, q - 1);
                    }
                }
            }
            catch (Exception) { }
        }

        private void MoveSlideDownButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var region = (this.DataContext as MainViewModel).Project.SelectedRegion;
                if (region.Slides.Count > 1)
                {
                    var q = region.Slides.IndexOf(region.Slides.Where(_ => _.Id == region.SelectedSlide.Id).FirstOrDefault());
                    if (q > -1 && q < region.Slides.Count() - 1)
                    {
                        region.Slides.Move(q, q + 1);
                    }
                }
            }
            catch (Exception) { }
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
            //int b = 0;
            //int a = 1 / b;
            //throw new OutOfMemoryException();
            //throw new System.InvalidOperationException("Logfile cannot be read-only");
            //throw new System.ArgumentException("Index is out of range", "index");
            zoomSlider.Value = 1;
        }

        private void AlignButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void AlignItemButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).AlignRegion((sender as MenuItem).Header.ToString());
        }

        #endregion

        #region DragDrop Elements

        private Button draggedItem;
        //private Point startDragPoint;

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedItem = sender as Button;
            //startDragPoint = e.GetPosition(null);
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && draggedItem != null)
            {
                Point position = e.GetPosition(null);
                DragDrop.DoDragDrop(draggedItem, draggedItem.Content, DragDropEffects.Copy);
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
            Point position = e.GetPosition(RegionCanvas);
            var m = (DataContext as MainViewModel);
            CreateRegionCanvas(m.CreateRegion(position.X, position.Y,
                m.Project.SelectedResolution.Width - position.X < 400 ? m.Project.SelectedResolution.Width - position.X : 400,
                m.Project.SelectedResolution.Height - position.Y < 300 ? m.Project.SelectedResolution.Height - position.Y : 300));
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
                else if (e.Key == Key.Left)
                {
                    if ((DataContext as MainViewModel).Project.SelectedObject is RegionModel && ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).X > 0)
                    {
                        ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).X--;
                    }
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    if ((DataContext as MainViewModel).Project.SelectedObject is RegionModel && ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).X <
                        ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).Parent.SelectedResolution.Width - ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).Width)
                    {
                        ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).X++;
                    }
                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {
                    if ((DataContext as MainViewModel).Project.SelectedObject is RegionModel && ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).Y > 0)
                    {
                        ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).Y--;
                    }
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    if ((DataContext as MainViewModel).Project.SelectedObject is RegionModel && ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).Y <
                        ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).Parent.SelectedResolution.Height - ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).Height)
                    {
                        ((DataContext as MainViewModel).Project.SelectedObject as RegionModel).Y++;
                    }
                    e.Handled = true;
                }

            }
            catch (Exception) { };
        }

        private void AddResolution_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Resolution();
            if (dialog.ShowDialog() == true)
                (DataContext as MainViewModel).AddResolution(dialog.NewResolution);
        }

        private void ResolutionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                DrowLines((DataContext as MainViewModel).Project.SelectedResolution, (DataContext as MainViewModel).Project.SelectedMonitor);
            }
        }

        private void EditResolution_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Resolution();
            dialog.NewResolution = (sender as Button).DataContext as ResolutionModel;
            ResolutionModel backup = dialog.NewResolution.Save();
            if (dialog.ShowDialog() != true)
            {
                var item = (DataContext as MainViewModel).Resolutions.Where(_ => _.Id == backup.Id).SingleOrDefault();
                item.Restore(backup);
            }
            else
            {
                DrowLines((DataContext as MainViewModel).Project.SelectedResolution, (DataContext as MainViewModel).Project.SelectedMonitor);
            }
        }

        private void DeleteResolution_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project.SelectedResolution != ((sender as Button).DataContext as ResolutionModel))
            {
                (DataContext as MainViewModel).Resolutions.Remove((sender as Button).DataContext as ResolutionModel);
            }
            else
            {
                MessageBox.Show("Resolution is selected in project, can't be deleted.", "Info", MessageBoxButton.OK);
            }
        }

        private void AddMonitor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Monitor();
            if (dialog.ShowDialog() == true)
                (DataContext as MainViewModel).AddMonitor(dialog.NewMonitor);
        }

        private void MonitorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                DrowLines((DataContext as MainViewModel).Project.SelectedResolution, (DataContext as MainViewModel).Project.SelectedMonitor);
            }
        }

        private void EditMonitor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Monitor();
            dialog.NewMonitor = (sender as Button).DataContext as MonitorModel;
            MonitorModel backup = dialog.NewMonitor.Save();
            if (dialog.ShowDialog() != true)
            {
                var item = (DataContext as MainViewModel).Monitors.Where(_ => _.Id == backup.Id).SingleOrDefault();
                item.Restore(backup);
            }
            else
            {
                DrowLines((DataContext as MainViewModel).Project.SelectedResolution, (DataContext as MainViewModel).Project.SelectedMonitor);
            }
        }

        private void DeleteMonitor_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as MainViewModel).Project.SelectedMonitor != ((sender as Button).DataContext as MonitorModel))
            {
                (DataContext as MainViewModel).Monitors.Remove((sender as Button).DataContext as MonitorModel);
            }
            else
            {
                MessageBox.Show("Monitor schema is selected in project, can't be deleted.", "Info", MessageBoxButton.OK);
            }
        }
        #endregion

        #region Methods
        private void Delete()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if ((DataContext as MainViewModel).Project.SelectedObject is RegionModel)
                {
                    var list = (DataContext as MainViewModel).Project.Regions.Where(_ => _.IsSelected == true).ToArray();
                    foreach (var region in list)
                    {
                        foreach (var item in RegionCanvas.Children)
                        {
                            if (item is ContentControl && (item as ContentControl).Name == region.Name)
                            {
                                RegionCanvas.Children.Remove(item as ContentControl);
                                break;
                            }
                        }
                    }
                    (DataContext as MainViewModel).DeleteRegion(list);
                }
                else if ((DataContext as MainViewModel).Project.SelectedObject is SlideModel)
                {
                    (DataContext as MainViewModel).DeleteSlide();
                    (DataContext as MainViewModel).Project.SelectedObject = (DataContext as MainViewModel).Project.SelectedRegion.SelectedSlide;
                }
            }
        }

        private void Copy()
        {
            if ((DataContext as MainViewModel).Project.SelectedObject is RegionModel)
            {
                _Copy = (DataContext as MainViewModel).Project.Regions.Where(_ => _.IsSelected == true).ToArray();
            }
            else
            {
                _Copy = (DataContext as MainViewModel).Project.SelectedObject;
            }
        }

        private void Paste()
        {
            if (_Copy is SlideModel)
            {
                (DataContext as MainViewModel).CopySlide(_Copy as SlideModel);
            }
            else if (_Copy is RegionModel)
            {
                (DataContext as MainViewModel).CopyRegion(_Copy as RegionModel);
            }
            else if (_Copy is RegionModel[])
            {
                foreach (var item in _Copy as RegionModel[])
                {
                    CreateRegionCanvas((DataContext as MainViewModel).CopyRegion(item as RegionModel));
                }
            }
        }

        private void Undo()
        {
            var obj = Memento.Pop();
            if (obj is SlideModel)
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
                //ClearRegions();
                //if ((DataContext as MainViewModel).Project.SelectedRegion != null)
                //    CreateRegionCanvas((DataContext as MainViewModel).Project.SelectedRegion);
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
            else if (obj is RegionModel[])
            {
                Memento.PushR((DataContext as MainViewModel).Project.Regions.ToArray());
                (DataContext as MainViewModel).Project.Regions = new ObservableCollection<RegionModel>(obj as RegionModel[]);
                (DataContext as MainViewModel).SelectRegion((DataContext as MainViewModel).Project.Regions.FirstOrDefault());
                ClearRegions();
                foreach (var item in (DataContext as MainViewModel).Project.Regions)
                {
                    CreateRegionCanvas(item);
                }
            }
        }

        private void Redo()
        {
            var obj = Memento.PopR();
            if (obj is SlideModel)
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
                //ClearRegions();
                //if ((DataContext as MainViewModel).Project.SelectedRegion != null)
                //    CreateRegionCanvas((DataContext as MainViewModel).Project.SelectedRegion);
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
            else if (obj is RegionModel[])
            {
                Memento.Push((DataContext as MainViewModel).Project.Regions.ToArray());
                (DataContext as MainViewModel).Project.Regions = new ObservableCollection<RegionModel>(obj as RegionModel[]);
                (DataContext as MainViewModel).SelectRegion((DataContext as MainViewModel).Project.Regions.FirstOrDefault());
                ClearRegions();
                foreach (var item in (DataContext as MainViewModel).Project.Regions)
                {
                    CreateRegionCanvas(item);
                }
            }
        }
        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception Exp)
            {

            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception Exp)
            {

            }
        }
    }
}
