using LibVLCSharp.Shared;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace RTSPControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LivePlayer : IDisposable
    {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
        public LivePlayer()
        {
            InitializeComponent();
            //int bits = IntPtr.Size * 8;
            //if (bits == 8)
            //    Core.Initialize(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LibVlc", "win-x64"));
            //else
            //    Core.Initialize(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LibVlc", "win-x86"));

            _libVLC = new LibVLC(enableDebugLogs: true);
            //_libVLC.SetLogFile("abc.txt");

            _mediaPlayer = new MediaPlayer(_libVLC);



            this.VideoView.MediaPlayer = _mediaPlayer;
        }

        public void Dispose()
        {
            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Stop();
                _libVLC.Dispose();
                _mediaPlayer.Dispose();
            }
        }

        //public void StartPlay(Uri uri)
        //{
        //    var media = new Media(_libVLC, uri);
        //    //media.AddOption(new MediaConfiguration {  EnableHardwareDecoding = true });


        //    this.VideoView.MediaPlayer.Play(media);
        //}
        public async void StartPlay(Uri uri)
        {
            var media = new Media(_libVLC, uri);
            _ = media.Parse(MediaParseOptions.ParseNetwork).Result;
            if (media.SubItems.Count > 0)
            {
                this.VideoView.MediaPlayer.Play(media.SubItems.First());
            }
            else
            {
                this.VideoView.MediaPlayer.Play(media);
            }
        }
        public void StopPlay()
        {
            try
            {
                if (_mediaPlayer.IsPlaying)
                    _mediaPlayer.Stop();

                _libVLC.Dispose();
                _mediaPlayer.Dispose();
            }
            catch (Exception)
            {


            }
        }

    }
}
