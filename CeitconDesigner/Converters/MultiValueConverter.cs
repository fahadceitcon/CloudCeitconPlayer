using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ceitcon_Designer.Converters
{
    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(parameter is bool && values[0] is string && values[1] is string))
                return String.Empty;

            if ((bool)parameter == false)
            {

                return String.Format("{0} {1}", values[0].ToString(), values[1].ToString());
            }
            else
            {
                return String.Format("{1} {0}", values[0].ToString(), values[1].ToString());
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
