using Ceitcon_Data.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for Resolution.xaml
    /// </summary>
    public partial class Resolution : Window
    {
        public Resolution()
        {
            InitializeComponent();
            this.KeyDown += HandleKeyPress;

            NewResolution = new ResolutionModel();
        }
        public static readonly DependencyProperty NewResolutionProperty = DependencyProperty.Register
        (
             "NewResolution",
             typeof(ResolutionModel),
             typeof(Resolution),
             new PropertyMetadata(null)
       );

        public ResolutionModel NewResolution
        {
            get { return (ResolutionModel)GetValue(NewResolutionProperty); }
            set { SetValue(NewResolutionProperty, value); }
        }

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
