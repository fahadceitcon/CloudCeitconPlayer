using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Ceitcon_Designer.Converters
{
    class StringToFontFamilyConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var fontFamily = value as FontFamily;
                if (fontFamily == null)
                    return SystemFonts.MessageFontFamily;

                foreach (Typeface typeface in fontFamily.GetTypefaces())
                {
                    GlyphTypeface face;
                    if (typeface.TryGetGlyphTypeface(out face))
                    {
                        return face.Symbol ? SystemFonts.MessageFontFamily : fontFamily;
                    }
                    else
                    {
                        return SystemFonts.MessageFontFamily;
                    }
                }

                return fontFamily;
            }
            catch (Exception)
            {
                return SystemFonts.MessageFontFamily;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
