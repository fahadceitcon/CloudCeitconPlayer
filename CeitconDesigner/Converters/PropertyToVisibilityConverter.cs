using Ceitcon_Data.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ceitcon_Designer.Converters
{
    class PropertyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is ControlType)
                {
                    // "FontFamily", "FontSize", "Foreground", "Source", "Text", "Type"
                    List<string> list = new List<string>();
                    switch ((ControlType)value)
                    {
                        case ControlType.Image:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                        case ControlType.Video:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                        case ControlType.GifAnim:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                        case ControlType.Text:
                            {
                                list.AddRange(new List<string> { "FontFamily", "FontSize", "Foreground", "FontWeight", "FontStyle", "TextDecoration", "Type" });
                            }
                            break;
                        case ControlType.RichText:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                        case ControlType.Ticker:
                            {
                                list.AddRange(new List<string> { "FontFamily", "FontSize", "Foreground", "FontWeight", "FontStyle", "TextDecoration", "InvertDirection", "Duration", "Type" });
                            }
                            break;
                        case ControlType.Likebox:
                            {
                                list.AddRange(new List<string> { "FontFamily", "FontSize", "Foreground", "Text", "Type", "MediaPageName" });
                            }
                            break;
                        case ControlType.Facebook:
                            {
                                list.AddRange(new List<string> {"FontFamily", "FontSize", "Foreground", "Text", "Type", "MediaPageName" });
                            }
                            break;
                        case ControlType.Instagram:
                            {
                                list.AddRange(new List<string> { "FontFamily", "FontSize", "Foreground", "Text", "Type", "MediaAccountId" });
                            }
                            break;
                        case ControlType.Twitter:
                            {
                                list.AddRange(new List<string> { "FontFamily", "FontSize", "Foreground", "Text", "Type", "MediaAccountId" });
                            }
                            break;
                        case ControlType.Youtube:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                        case ControlType.SocialMediaImage:
                            {
                                list.AddRange(new List<string> { "SocialMediaType", "Type" });
                            }
                            break;
                        case ControlType.Live:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                        case ControlType.Alert:
                            {
                                list.AddRange(new List<string> { "FontFamily", "FontSize", "Foreground", "Type" });
                            }
                            break;
                        case ControlType.DataGrid:
                            {
                                list.AddRange(new List<string> { "FontFamily", "FontSize", "Foreground", "FontWeight", "FontStyle", "FlowDirection", "Type" });
                            }
                            break;
                        case ControlType.Weather:
                            {
                                list.AddRange(new List<string> { "Type", "ItemCount", "FlowDirection" });
                            }
                            break;
                        case ControlType.DateTime:
                            {
                                list.AddRange(new List<string> { "FontFamily", "FontSize", "Foreground", "FontWeight", "FontStyle", "TextDecoration", "FlowDirection", "FlowDirection", "DateTimeFormat", "CustomDateTimeFormat", "FlowDirection", "Type" });
                            }
                            break;
                        case ControlType.PDF:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                        case ControlType.PPT:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                        case ControlType.WebBrowser:
                            {
                                list.AddRange(new List<string> { "Type" });
                            }
                            break;
                    }
                    return list.Contains(parameter.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                }
                
                return Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
