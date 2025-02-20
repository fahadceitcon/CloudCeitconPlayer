using System;
using System.IO;

namespace Ceitcon_Data.Utilities
{
    internal static class VideoConverter
    {
        public static string GenerateImage(string pathFrom, string pathTo, int? frameTime = 5)
        {
            if (!File.Exists(pathFrom))
                return null;

            //string thumbJpegStream = Path.Combine(Directory.GetCurrentDirectory(), Thumbs,  Path.GetFileNameWithoutExtension(pathFrom) + "_thumb.jpg");
            // todo : create temp folder....
            if (File.Exists(pathTo))
                return pathTo;
            try
            {
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                ffMpeg.GetVideoThumbnail(pathFrom, pathTo, frameTime);

                long length = new System.IO.FileInfo(pathTo).Length;
                if(length == 0)
                    ffMpeg.GetVideoThumbnail(pathFrom, pathTo, 1);
            }
            catch (Exception)
            {
                return null;
            }
            return pathTo;
        }
    }
}
