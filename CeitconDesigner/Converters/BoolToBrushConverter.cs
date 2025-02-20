using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace Ceitcon_Designer.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Brushes.SeaGreen;
            if ((bool)value)
            {
                return Brushes.DarkSeaGreen;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
