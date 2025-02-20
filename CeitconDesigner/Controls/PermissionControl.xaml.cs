using System.Windows;
using System.Windows.Controls;
using Ceitcon_Data.Model.User;
using Ceitcon_Designer.ViewModel;
using System.Windows.Input;
using Ceitcon_Designer.Utilities;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for PermissionControl.xaml
    /// </summary>
    public partial class PermissionControl : UserControl
    {
        public PermissionControl()
        {
            InitializeComponent();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            object parent = dmtv.SelectedItem;
            if (parent is GroupModel)
            {
                var user = new UserModel(parent as GroupModel);
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.UpdateUser(user))
                    (parent as GroupModel).Users.Add(user);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (parent is UserModel)
            {
                var user = new UserModel((parent as UserModel).Parent);
                Mouse.OverrideCursor = Cursors.Wait;
                if(CeitconServerHelper.UpdateUser(user))
                    ((parent as UserModel).Parent).Users.Add(user);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            object obj = dmtv.SelectedItem;
            if (obj is GroupModel)
            {

            }
            else if (obj is UserModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (CeitconServerHelper.DeleteUser(obj as UserModel))
                {
                    (obj as UserModel).Parent.Users.Remove(obj as UserModel);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            object user = dmtv.SelectedItem;
            if (user is UserModel)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                CeitconServerHelper.UpdateUser(user as UserModel);
                Mouse.OverrideCursor = Cursors.Arrow;
                // (DataContext as MainViewModel).UpdateUser(user as UserModel);
            }
        }
    }
}
