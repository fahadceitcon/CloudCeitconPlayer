using Syncfusion.Licensing;
using Syncfusion.OfficeChartToImageConverter;
using Syncfusion.Pdf;
using Syncfusion.Presentation;
using Syncfusion.PresentationToPdfConverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ceitcon_Designer.Utilities
{
    static class PPTHelper
    {
        public static string ConvertToPDF(string source)
        {
            string pdfSource = $"{source}.pdf";
            if (File.Exists(pdfSource))
                return pdfSource;

            if (!File.Exists(source))
                return null;

            try
            {
                SyncfusionLicenseProvider.RegisterLicense("NDMwNzNAMzEzNjJlMzMyZTMwY1lValM2T3kzR3N5TjRvUHJicGtjd1JmemtDTXdWYmcyM2NycW9vVnhhQT0=");

                using (IPresentation presentation = Presentation.Open(source))
                {
                    presentation.ChartToImageConverter = new ChartToImageConverter();
                    using (PdfDocument pdfDocument = PresentationToPdfConverter.Convert(presentation))
                    {
                        pdfDocument.Save(pdfSource);
                    }
                }
                return pdfSource;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string[] ConvertToImages(string source, int customWidth, int customHeight)
        {
            //string pdfSource = $"{source}.pdf";
            //if (File.Exists(pdfSource))
            //    return pdfSource;
            List<string> list = new List<string>();
            string fileName = Path.GetFileNameWithoutExtension(source);
            string directoryName = Path.GetDirectoryName(source);
            string tempDirectory = Path.Combine(directoryName, fileName);
            if (Directory.Exists(tempDirectory))
            {
                string[] fileEntries = Directory.GetFiles(tempDirectory)
                    .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                return fileEntries;
            }

            if (!File.Exists(source))
                return null;

            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            try
            {
                SyncfusionLicenseProvider.RegisterLicense("NDMwNzNAMzEzNjJlMzMyZTMwY1lValM2T3kzR3N5TjRvUHJicGtjd1JmemtDTXdWYmcyM2NycW9vVnhhQT0=");
                using (IPresentation presentation = Presentation.Open(source))
                {
                    presentation.ChartToImageConverter = new ChartToImageConverter();
                    presentation.ChartToImageConverter.ScalingMode = Syncfusion.OfficeChart.ScalingMode.Best;

                    int slideNumber = 0;
                    foreach (var image in presentation.RenderAsImages(Syncfusion.Drawing.ImageType.Metafile))
                    {
                        string fn = $"00000{slideNumber++}.jpg";
                        string name = Path.Combine(tempDirectory, fn.Substring(fn.Length - 9));
                        image.Save(name);
                        list.Add(name);
                    }
                }
            }
            catch (Exception)
            {
                if (Directory.Exists(tempDirectory))
                    Directory.Delete(tempDirectory, true);

                return null;
            }

            return list.ToArray();
        }
    }
}
