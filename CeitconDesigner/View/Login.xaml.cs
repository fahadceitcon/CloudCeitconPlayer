using Ceitcon_Data.Model.User;
using Ceitcon_Designer.Utilities;
using Ceitcon_Designer.ViewModel;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public partial class Login : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string serverName = @"ServerUrl";
        public Login()
        {
            InitializeComponent();

            if (!CheckDesignerMode())
            {
                Close();
                return;
            }

            this.KeyDown += HandleKeyPress;

            ServerUrl = SQLiteHelper.Instance.GetApplication(serverName);
            CeitconServerHelper.Address = ServerUrlFull;
            HasUrl = String.IsNullOrWhiteSpace(ServerUrl) ? false : true;
        }

        private bool CheckDesignerMode()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Ceitcon_Designer.exe.config");
            return File.Exists(path);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
                this.Left = (System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Width - this.Width) / 2;
                this.Top = (System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Height - this.Height) / 2;
                //this.WindowState = WindowState.Maximized;
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
        (
             "Text",
             typeof(String),
             typeof(Login),
             new PropertyMetadata(string.Empty)
        );

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty PasswordTextProperty = DependencyProperty.Register
        (
             "PasswordText",
             typeof(object),
             typeof(Login),
             new PropertyMetadata(string.Empty)
        );

        public String PasswordText
        {
            get { return (String)GetValue(PasswordTextProperty); }
            set { SetValue(PasswordTextProperty, value); }
        }

        public static readonly DependencyProperty ServerUrlProperty = DependencyProperty.Register
        (
            "ServerUrl",
            typeof(String),
            typeof(Login),
            new PropertyMetadata(string.Empty)
        );

        public String ServerUrl
        {
            get { return (String)GetValue(ServerUrlProperty); }
            set { SetValue(ServerUrlProperty, value); }
        }

        public String ServerUrlFull
        {
            get { return String.Format(@"http://{0}:8000/CeitconServer/service", ServerUrl); }
            //get { return String.Format(@"http://{0}:8733/Design_Time_Addresses/CeitconServer/Service1/", ServerUrl); }
        }

        public static readonly DependencyProperty HasUrlProperty = DependencyProperty.Register
        (
            "HasUrl",
            typeof(bool),
            typeof(Login),
            new PropertyMetadata(false)
        );

        public bool HasUrl
        {
            get { return (bool)GetValue(HasUrlProperty); }
            set
            {
                SetValue(HasUrlProperty, value);

                if (value)
                    userTextBox.Focus();
                else
                    tbServerUrl.Focus();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                passwordTextBox.Visibility = ((PasswordBox)sender).Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;
            }
            catch (Exception) { }
        }


        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            SignIn();
        }

        private void SetUrlButton_Click(object sender, RoutedEventArgs e)
        {
            SetUrl();
        }

        private void ChangeUrlButton_Click(object sender, RoutedEventArgs e)
        {
            HasUrl = false;
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (HasUrl)
                    {
                        SignIn();
                    }
                    else
                    {
                        SetUrl();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception) { };
        }

        #region Events
        private void SignIn()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            string errorResponse = String.Empty;
            UserModel activUser = CeitconServerHelper.LoginUser(Text, PasswordText, out errorResponse);
            if (activUser != null)
            {
                Main dialog = new Main();
                (dialog.DataContext as MainViewModel).User = activUser;

                if(activUser.PermissionUser)
                    (dialog.DataContext as MainViewModel).UserGroups = CeitconServerHelper.GetGroups();
                if (activUser.PermissionNetwork)
                    (dialog.DataContext as MainViewModel).Network = CeitconServerHelper.GetNetwork();

                dialog.Show();
                this.Close();
            }
            else
            {
                log.Info(String.Format("Wrong login: [{0}], [{1}], Error respones: [{2}]", Text, PasswordText, errorResponse));
                if (!String.IsNullOrEmpty(errorResponse))
                    MessageBox.Show(errorResponse, "Error", MessageBoxButton.OK);
                else
                    MessageBox.Show("Wrong username or password", "Info", MessageBoxButton.OK);
                PasswordText = "";
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void SetUrl()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                log.Info(String.Format("Check Server connection: ", ServerUrlFull));
                CeitconServerHelper.Address = ServerUrlFull;
                if (CeitconServerHelper.TestConnection())
                {
                    if (SQLiteHelper.Instance.InsertApplication(serverName, ServerUrl))
                        HasUrl = true;
                }
                else
                    MessageBox.Show("No signal from server. Please insert correct url and try again.", "Info", MessageBoxButton.OK);

            }
            catch (Exception ex)
            {
                log.Error("Check Server connection: ", ex);
                MessageBox.Show("No signal from server. Please insert correct url and try again.", "Info", MessageBoxButton.OK);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }
        #endregion

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
