using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Ceitcon_Designer.Converters
{
    public class ImageCacheConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // load the image, specify CacheOption so the file is not locked
            var path = (string)value;
            if (!File.Exists(path))
                path = Path.Combine(Path.GetDirectoryName(path), "__.png");
            var image = new BitmapImage();
            try
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(path);
                image.EndInit();
            }
            catch (Exception)
            {
            }
            return image;

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Not implemented.");
        }
    }
}
