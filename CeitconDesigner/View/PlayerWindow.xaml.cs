using System;
using System.Windows;
using System.Windows.Input;
using Ceitcon_Data.Model.Network;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : Window
    {
        public PlayerWindow()
        {
            InitializeComponent();
            this.KeyDown += HandleKeyPress;

           // NewPayer = new PlayerModel(null);
        }

        //public static readonly DependencyProperty NewPayerProperty = DependencyProperty.Register
        //(
        //     "NewPayer",
        //     typeof(PlayerModel),
        //     typeof(PlayerWindow),
        //     new PropertyMetadata(null)
        //);

        //public PlayerModel NewPayer
        //{
        //    get { return (PlayerModel)GetValue(NewPayerProperty); }
        //    set { SetValue(NewPayerProperty, value); }
        //}

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    this.DialogResult = true;
                    this.Close();
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    this.DialogResult = false;
                    this.Close();
                    e.Handled = true;
                }
            }
            catch (Exception) { };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
