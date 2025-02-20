using Ceitcon_Data.Model.Network;
using Ceitcon_Designer.Utilities;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for NetworkTreeControl.xaml
    /// </summary>
    public partial class NetworkTreeControl : UserControl
    {
        public NetworkTreeControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register
        (
             "SelectedItem",
             typeof(object),
             typeof(NetworkTreeControl),
             new PropertyMetadata(null)
        );

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
                OnPropertyChanged("SelectedItem");
                Backup(value);
                //(DataContext as MainViewModel).GetPlayers(value);
            }
        }

        public string BackupName;
        public string BackupDescription;
        public bool BackupActive;
        private void Backup(object item)
        {
            if (item is NetworkModel)
            {
                BackupName = (item as NetworkModel).Name;
            }
            else if (item is DomainModel)
            {
                BackupName = (item as DomainModel).Name;
                BackupDescription = (item as DomainModel).Description;
                BackupActive = (item as DomainModel).Active;
            }
            else if (item is CountryModel)
            {
                BackupName = (item as CountryModel).Name;
                BackupDescription = (item as CountryModel).Description;
                BackupActive = (item as CountryModel).Active;
            }
            else if (item is NetworkRegionModel)
            {
                BackupName = (item as NetworkRegionModel).Name;
                BackupDescription = (item as NetworkRegionModel).Description;
                BackupActive = (item as NetworkRegionModel).Active;
            }
            else if (item is LocationGroupModel)
            {
                BackupName = (item as LocationGroupModel).Name;
                BackupDescription = (item as LocationGroupModel).Description;
                BackupActive = (item as LocationGroupModel).Active;
            }
            else if (item is LocationModel)
            {
                BackupName = (item as LocationModel).Name;
                BackupDescription = (item as LocationModel).Description;
                BackupActive = (item as LocationModel).Active;
            }
            else if (item is FloorModel)
            {
                BackupName = (item as FloorModel).Name;
                BackupDescription = (item as FloorModel).Description;
                BackupActive = (item as FloorModel).Active;
            }
            else if (item is ZoneModel)
            {
                BackupName = (item as ZoneModel).Name;
                BackupDescription = (item as ZoneModel).Description;
                BackupActive = (item as ZoneModel).Active;
            }
            else if (item is PlayerGroupModel)
            {
                BackupName = (item as PlayerGroupModel).Name;
                BackupDescription = (item as PlayerGroupModel).Description;
                BackupActive = (item as PlayerGroupModel).Active;
            }
            else if (item is PlayerModel)
            {
                BackupName = (item as PlayerModel).Name;
                BackupDescription = (item as PlayerModel).Description;
                BackupActive = (item as PlayerModel).Active;
            }
            else if (item is FaceGroupModel)
            {
                BackupName = (item as FaceGroupModel).Name;
                BackupDescription = (item as FaceGroupModel).Description;
                BackupActive = (item as FaceGroupModel).Active;
            }
            else if (item is FaceModel)
            {
                BackupName = (item as FaceModel).Name;
                BackupDescription = (item as FaceModel).Description;
                BackupActive = (item as FaceModel).Active;
            }
        }

        private void dmtv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
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

        #region Drag-Drop
        Point _lastMouseDown;
        TreeViewItem ddItemTree;
        object ddItem;

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(dmtv);

                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        //draggedItem = (TreeViewItem)dmtv.SelectedItem;
                        //draggedItem = (object)dmtv.SelectedItem;
                        ddItem = dmtv.SelectedItem;
                        //if (draggedItem != null)
                        if (ddItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(dmtv, dmtv.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            //if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            //{
                            // A Move drop was accepted
                            //if (!draggedItem.Header.ToString().Equals(_target.Header.ToString()))
                            //{
                            //    CopyItem(draggedItem, _target);
                            //    _target = null;
                            //    draggedItem = null;
                            //}

                            //}
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(dmtv);

                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    var tvi = (TreeViewItem)GetNearestContainer(e.OriginalSource as UIElement);
                    if (tvi.DataContext == ddItem)
                        ddItemTree = (TreeViewItem)GetNearestContainer(e.OriginalSource as UIElement);
                    //ddItem = ddItemTree.DataContext;
                    // Verify that this is a valid drop and then store the drop target
                    //TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (CheckDropTarget(ddItem, tvi.DataContext))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                TreeViewItem parentTree = GetNearestContainer(e.OriginalSource as UIElement);
                object parent = parentTree.DataContext;
                if (parent != null && ddItem != null && ddItem != parent)
                {
                    if (ddItem is DomainModel && parent is NetworkModel)
                    {
                        var u = ddItem as DomainModel;
                        var p = parent as NetworkModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("Domain", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.Domains.Remove(u);
                            u.Parent = p;
                            p.Domains.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is CountryModel && parent is DomainModel)
                    {
                        var u = ddItem as CountryModel;
                        var p = parent as DomainModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("Country", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.Countries.Remove(u);
                            u.Parent = p;
                            p.Countries.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is NetworkRegionModel && parent is CountryModel)
                    {
                        var u = ddItem as NetworkRegionModel;
                        var p = parent as CountryModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("Region", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.Regions.Remove(u);
                            u.Parent = p;
                            p.Regions.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is LocationGroupModel && parent is NetworkRegionModel)
                    {
                        var u = ddItem as LocationGroupModel;
                        var p = parent as NetworkRegionModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("LocationGroup", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.LocationGroups.Remove(u);
                            u.Parent = p;
                            p.LocationGroups.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is LocationModel && parent is LocationGroupModel)
                    {
                        var u = ddItem as LocationModel;
                        var p = parent as LocationGroupModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("Location", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.Locations.Remove(u);
                            u.Parent = p;
                            p.Locations.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is FloorModel && parent is LocationModel)
                    {
                        var u = ddItem as FloorModel;
                        var p = parent as LocationModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("Floor", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.Floors.Remove(u);
                            u.Parent = p;
                            p.Floors.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is ZoneModel && parent is FloorModel)
                    {
                        var u = ddItem as ZoneModel;
                        var p = parent as FloorModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("Zone", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.Zones.Remove(u);
                            u.Parent = p;
                            p.Zones.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is PlayerGroupModel && parent is ZoneModel)
                    {
                        var u = ddItem as PlayerGroupModel;
                        var p = parent as ZoneModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("PlayerGroup", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.PlayerGroups.Remove(u);
                            u.Parent = p;
                            p.PlayerGroups.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is PlayerModel && parent is PlayerGroupModel)
                    {
                        var u = ddItem as PlayerModel;
                        var p = parent as PlayerGroupModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("Player", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.Players.Remove(u);
                            u.Parent = p;
                            p.Players.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is FaceGroupModel && parent is PlayerModel)
                    {
                        var u = ddItem as FaceGroupModel;
                        var p = parent as PlayerModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("FaceGroup", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.FaceGroups.Remove(u);
                            u.Parent = p;
                            p.FaceGroups.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (ddItem is FaceModel && parent is FaceGroupModel)
                    {
                        var u = ddItem as FaceModel;
                        var p = parent as FaceGroupModel;
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (CeitconServerHelper.UpdateNetwork("Face", u.Id, p.Id, u.Name, u.Description, u.Active))
                        {
                            u.Parent.Faces.Remove(u);
                            u.Parent = p;
                            p.Faces.Add(u);
                        }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    else if (parent is FaceModel)
                    {

                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with uploading changes on server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }
        private bool CheckDropTarget(object item, object parent)
        {
            return ((item is DomainModel && parent is NetworkModel) ||
                (item is CountryModel && parent is DomainModel) ||
                (item is NetworkRegionModel && parent is CountryModel) ||
                (item is LocationGroupModel && parent is NetworkRegionModel) ||
                (item is LocationModel && parent is LocationGroupModel) ||
                (item is FloorModel && parent is LocationModel) ||
                (item is ZoneModel && parent is FloorModel) ||
                (item is PlayerGroupModel && parent is ZoneModel) ||
                (item is PlayerModel && parent is PlayerGroupModel) ||
                (item is FaceGroupModel && parent is PlayerModel) ||
                (item is FaceModel && parent is FaceGroupModel));
        }

        #endregion
    }
}
