using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Documents.FormatProviders.Html;
using Telerik.Windows.Documents.RichTextBoxCommands;

namespace Ceitcon_Designer.Controls
{
    /// <summary>
    /// Interaction logic for RichTextEditorControl.xaml
    /// </summary>
    public partial class RichTextEditorControl : UserControl
    {
        public RichTextEditorControl()
        {
            InitializeComponent();
        }

        public HtmlDataProvider HtmlDataProvider = null;
        //public static readonly DependencyProperty HtmlTextProperty = DependencyProperty.Register
        //(
        //    "HtmlText",
        //    typeof(string),
        //    typeof(RichTextEditorControl),
        //    new PropertyMetadata(String.Empty)
        //);

        //public string HtmlText
        //{
        //    get { return HtmlDataProvider.Html; }
        //    set
        //    {
        //        SetValue(HtmlTextProperty, value);
        //        HtmlDataProvider = new HtmlDataProvider()
        //        {
        //            Html = value,
        //        };
        //        HtmlDataProvider.SetBinding(HtmlDataProvider.RichTextBoxProperty, new Binding() { Source = this.radRichTextBox });
        //}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HtmlDataProvider = new HtmlDataProvider()
            {
                Html = this.DataContext.ToString(),
            };
            HtmlDataProvider.SetBinding(HtmlDataProvider.RichTextBoxProperty, new Binding() { Source = this.radRichTextBox });
        }

        //private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    this.DataContext = HtmlDataProvider.Html;
        //}
    }


    //public class ExampleDataContext : INotifyPropertyChanged
    //{
    //    private string htmlData;

    //    public string HtmlData
    //    {
    //        get
    //        {
    //            return this.htmlData;
    //        }
    //        set
    //        {
    //            if (value != this.htmlData)
    //            {
    //                this.htmlData = value;
    //                OnPropertyChanged("HtmlData");
    //            }
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    private void OnPropertyChanged(string propertyName)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }
    //}
}
