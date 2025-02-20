using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for RichTextEditorWindow.xaml
    /// </summary>
    public partial class RichTextEditorWindow : Window
    {
        public RichTextEditorWindow()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = RichTextEditor.HtmlDataProvider.Html;
            this.DialogResult = true;
            this.Close();

        }
    }
}
