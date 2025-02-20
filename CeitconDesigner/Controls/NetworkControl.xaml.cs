using Ceitcon_Data.Model.Network;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for NetworkControl.xaml
    /// </summary>
    public partial class NetworkControl : UserControl
    {
        public NetworkControl()
        {
            InitializeComponent();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            object parent = networkTree.SelectedItem;
            if (parent is NetworkModel)
            {
                var u = new DomainModel(parent as NetworkModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("Domain", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as NetworkModel).Domains.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is DomainModel)
            {
                var u = new CountryModel(parent as DomainModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("Country", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as DomainModel).Countries.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is CountryModel)
            {
                var u = new NetworkRegionModel(parent as CountryModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("Region", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as CountryModel).Regions.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is NetworkRegionModel)
            {
                var u = new LocationGroupModel(parent as NetworkRegionModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("LocationGroup", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as NetworkRegionModel).LocationGroups.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is LocationGroupModel)
            {
                var u = new LocationModel(parent as LocationGroupModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("Location", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as LocationGroupModel).Locations.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is LocationModel)
            {
                var u = new FloorModel(parent as LocationModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("Floor", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as LocationModel).Floors.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is FloorModel)
            {
                var u = new ZoneModel(parent as FloorModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("Zone", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as FloorModel).Zones.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is ZoneModel)
            {
                var u = new PlayerGroupModel(parent as ZoneModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("PlayerGroup", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as ZoneModel).PlayerGroups.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is PlayerGroupModel)
            {
                var u = new PlayerModel(parent as PlayerGroupModel);
                Mouse.OverrideCursor = Cursors.Wait;
                bool bResult = CeitconServerHelper.UpdateNetwork("Player", u.Id, u.Parent.Id, u.Name, u.Description, u.Active);
                if (bResult)
                {
                    (parent as PlayerGroupModel).Players.Add(u);
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                else
                {
                    MessageBox.Show("Can not add more player's \r\n Your total player's license is equal to be created player's", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Mouse.OverrideCursor = Cursors.Arrow;
                    
                }
            }
            else if (parent is PlayerModel)
            {
                var u = new FaceGroupModel(parent as PlayerModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("FaceGroup", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as PlayerModel).FaceGroups.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is FaceGroupModel)
            {
                var u = new FaceModel(parent as FaceGroupModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateNetwork("Face", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                    (parent as FaceGroupModel).Faces.Add(u);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is FaceModel)
            {

            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            object obj = networkTree.SelectedItem;
            if (obj is NetworkModel)
            {

            }
            if (obj is DomainModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("Domain", (obj as DomainModel).Id))
                {
                    (obj as DomainModel).Parent.Domains.Remove(obj as DomainModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is CountryModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("Country", (obj as CountryModel).Id))
                {
                    (obj as CountryModel).Parent.Countries.Remove(obj as CountryModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is NetworkRegionModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("Region", (obj as NetworkRegionModel).Id))
                {
                    (obj as NetworkRegionModel).Parent.Regions.Remove(obj as NetworkRegionModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is LocationGroupModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("LocationGroup", (obj as LocationGroupModel).Id))
                {
                    (obj as LocationGroupModel).Parent.LocationGroups.Remove(obj as LocationGroupModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is LocationModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("Location", (obj as LocationModel).Id))
                {
                    (obj as LocationModel).Parent.Locations.Remove(obj as LocationModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is FloorModel)
            {

                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("Floor", (obj as FloorModel).Id))
                {
                    (obj as FloorModel).Parent.Floors.Remove(obj as FloorModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is ZoneModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("Zone", (obj as ZoneModel).Id))
                {
                    (obj as ZoneModel).Parent.Zones.Remove(obj as ZoneModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is PlayerGroupModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("PlayerGroup", (obj as PlayerGroupModel).Id))
                {
                    (obj as PlayerGroupModel).Parent.PlayerGroups.Remove(obj as PlayerGroupModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is PlayerModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("Player", (obj as PlayerModel).Id))
                {
                    (obj as PlayerModel).Parent.Players.Remove(obj as PlayerModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is FaceGroupModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("FaceGroup", (obj as FaceGroupModel).Id))
                {
                    (obj as FaceGroupModel).Parent.FaceGroups.Remove(obj as FaceGroupModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (obj is FaceModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteNetwork("Face", (obj as FaceModel).Id))
                {
                    (obj as FaceModel).Parent.Faces.Remove(obj as FaceModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            string errorText = @"Changes will not uploaded to server. Item with same name already exists or have problem with connection.";
            object item = networkTree.SelectedItem;
            if (item is NetworkModel)
            {

            }
            else if (item is DomainModel)
            {
                var u = item as DomainModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("Domain", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is CountryModel)
            {
                var u = item as CountryModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("Country", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is NetworkRegionModel)
            {
                var u = item as NetworkRegionModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("Region", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is LocationGroupModel)
            {
                var u = item as LocationGroupModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("LocationGroup", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is LocationModel)
            {
                var u = item as LocationModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("Location", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is FloorModel)
            {
                var u = item as FloorModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("Floor", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is ZoneModel)
            {
                var u = item as ZoneModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("Zone", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is PlayerGroupModel)
            {
                var u = item as PlayerGroupModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("PlayerGroup", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is PlayerModel)
            {
                var u = item as PlayerModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("Player", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is FaceGroupModel)
            {
                var u = item as FaceGroupModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("FaceGroup", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (item is FaceModel)
            {
                var u = item as FaceModel;
                Mouse.OverrideCursor = Cursors.Wait;
                if (!CeitconServerHelper.UpdateNetwork("Face", u.Id, u.Parent.Id, u.Name, u.Description, u.Active))
                {
                    MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    u.Name = networkTree.BackupName;
                    u.Description = networkTree.BackupDescription;
                    u.Active = networkTree.BackupActive;
                }
                else
                {
                    networkTree.BackupName = u.Name;
                    networkTree.BackupDescription = u.Description;
                    networkTree.BackupActive = u.Active;
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }
    }
}
