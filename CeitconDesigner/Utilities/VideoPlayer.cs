using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ceitcon_Designer.Utilities
{

    //public class VideoPlayer : Control
    //{
    //    #region Fields
    //    public Action MediaEnded;
    //    private MediaPlayer player;
    //    #endregion

    //    #region DPs
    //    public Stretch Stretch
    //    {
    //        get { return (Stretch)GetValue(StretchProperty); }
    //        set { SetValue(StretchProperty, value); }
    //    }
    //    public static readonly DependencyProperty StretchProperty =
    //     DependencyProperty.Register("Stretch", typeof(Stretch), typeof(VideoPlayer), new PropertyMetadata());

    //    public DrawingBrush VideoBrush
    //    {
    //        get { return (DrawingBrush)GetValue(VideoBrushProperty); }
    //        private set { SetValue(VideoBrushProperty, value); }
    //    }
    //    public static readonly DependencyProperty VideoBrushProperty =
    //     DependencyProperty.Register("VideoBrush", typeof(DrawingBrush), typeof(VideoPlayer), new PropertyMetadata());

    //    public VideoDrawing VideoDrawing
    //    {
    //        get { return (VideoDrawing)GetValue(VideoDrawingProperty); }
    //        private set { SetValue(VideoDrawingProperty, value); }
    //    }
    //    public static readonly DependencyProperty VideoDrawingProperty =
    //     DependencyProperty.Register("VideoDrawing", typeof(VideoDrawing), typeof(VideoPlayer), new PropertyMetadata());

    //    public Uri Source
    //    {
    //        get { return (Uri)GetValue(SourceProperty); }
    //        set { SetValue(SourceProperty, value); }
    //    }
    //    public static readonly DependencyProperty SourceProperty =
    //     DependencyProperty.Register("Source", typeof(Uri), typeof(VideoPlayer), new PropertyMetadata());
    //    private DispatcherTimer timer;
    //    #endregion

    //    #region Ctor
    //    static VideoPlayer()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoPlayer), new FrameworkPropertyMetadata(typeof(VideoPlayer)));
    //    }

    //    public VideoPlayer()
    //    {
    //        Stretch = Stretch.Fill;
    //        player = new MediaPlayer();
    //        player.Volume = 0.0;
    //        player.MediaEnded += Player_MediaEnded;
    //        player.MediaOpened += Player_MediaOpened;
    //        player.MediaFailed += Player_MediaFailed;
    //        player.ScrubbingEnabled = true;
    //        ResetBrush();
    //        //timer = new DispatcherTimer(DispatcherPriority.Normal);
    //        //timer.Tick += Timer_Tick;
    //        //timer.Interval = TimeSpan.FromSeconds(5);
    //    }
    //    #endregion

    //    private void Player_MediaFailed(object sender, ExceptionEventArgs e)
    //    {
    //        Player_MediaEnded(null, null);
    //    }

    //    //private void Timer_Tick(object sender, EventArgs e)
    //    //{
    //    //    timer.Stop();
    //    //    Player_MediaEnded(null, null);
    //    //}

    //    private void ResetBrush()
    //    {
    //        VideoDrawing = new VideoDrawing();
    //        VideoDrawing.Player = player;
    //        VideoBrush = new DrawingBrush(VideoDrawing);
    //        BindingOperations.SetBinding(VideoBrush, DrawingBrush.StretchProperty, new Binding { Source = this, Path = new PropertyPath(StretchProperty), Mode = BindingMode.OneWay });
    //    }

    //    private void Player_MediaOpened(object sender, EventArgs e)
    //    {
    //        if (IOManager.IsVideo(Source))
    //        {
    //            VideoDrawing.Rect = new Rect(0, 0, player.NaturalVideoWidth, player.NaturalVideoHeight);
    //            player.Play();
    //        }
    //        else
    //        {
    //            VideoDrawing.Rect = new Rect(IOManager.GetImageSize(Source));
    //            //timer.Start();
    //        }
    //    }

    //    private void Player_MediaEnded(object sender, EventArgs e)
    //    {
    //        if (MediaEnded != null)
    //            MediaEnded.Invoke();
    //    }

    //    public void Play(Uri source)
    //    {
    //        Source = source;
    //        player.Open(source);
    //    }

    //    public void Stop(bool clear = false)
    //    {
    //        player.Stop();
    //        if (clear)
    //        {
    //            player.Open(null);
    //            ResetBrush();
    //        }
    //    }

    //    public void Pause()
    //    {
    //        if (player.CanPause)
    //            player.Pause();
    //    }
    //}

    //internal static class IOManager
    //{
    //    public static List<string> VideoFileExtensions
    //    {
    //        get { return _videoFileExtensions; }
    //    }
    //    private static List<string> _videoFileExtensions = new List<string> { ".wmv", ".avi", ".mpg", ".mpeg", ".mp4" };

    //    public static bool IsVideo(Uri uri)
    //    {
    //        if (uri == null)
    //            return false;
    //        return IsVideo(System.IO.Path.GetFileName(uri.LocalPath));
    //    }

    //    public static bool IsVideo(string key)
    //    {
    //        if (String.IsNullOrEmpty(key))
    //            return false;
    //        return VideoFileExtensions.Contains(System.IO.Path.GetExtension(key).ToLower());
    //    }

    //    public static Size GetImageSize(Uri uri)
    //    {
    //        if (uri == null)
    //            return new Size();
    //        BitmapFrame frame = BitmapFrame.Create(uri);
    //        return new Size(frame.PixelWidth, frame.PixelHeight);
    //    }
    //}

    public class VideoPlayer : Control
    {
        #region Fields
        public Action MediaEnded;
        private MediaPlayer player;
        private bool _isVideo;
        #endregion

        #region DPs
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        public static readonly DependencyProperty StretchProperty =
         DependencyProperty.Register("Stretch", typeof(Stretch), typeof(VideoPlayer), new PropertyMetadata());

        public DrawingBrush VideoBrush
        {
            get { return (DrawingBrush)GetValue(VideoBrushProperty); }
            private set { SetValue(VideoBrushProperty, value); }
        }
        public static readonly DependencyProperty VideoBrushProperty =
         DependencyProperty.Register("VideoBrush", typeof(DrawingBrush), typeof(VideoPlayer), new PropertyMetadata());

        public VideoDrawing VideoDrawing
        {
            get { return (VideoDrawing)GetValue(VideoDrawingProperty); }
            private set { SetValue(VideoDrawingProperty, value); }
        }
        public static readonly DependencyProperty VideoDrawingProperty =
         DependencyProperty.Register("VideoDrawing", typeof(VideoDrawing), typeof(VideoPlayer), new PropertyMetadata());

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(VideoPlayer), new PropertyMetadata());
        private DispatcherTimer timer;
        #endregion

        #region Ctor
        static VideoPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoPlayer), new FrameworkPropertyMetadata(typeof(VideoPlayer)));
        }

        public VideoPlayer()
        {
            Stretch = Stretch.Fill;
            player = new MediaPlayer();
            player.Volume = 0.0;
            player.MediaEnded += Player_MediaEnded;
            player.MediaOpened += Player_MediaOpened;
            player.MediaFailed += Player_MediaFailed;
            player.ScrubbingEnabled = true;
            ResetBrush();
            //timer = new DispatcherTimer(DispatcherPriority.Normal);
            //timer.Tick += Timer_Tick;
            //timer.Interval = TimeSpan.FromSeconds(5);
        }
        #endregion

        private void Player_MediaFailed(object sender, ExceptionEventArgs e)
        {
            //if (e.ErrorException != null)
            //  logger.Debug("Media Failed:" + e.ErrorException.ToString());
            Player_MediaEnded(null, null);
            //if (e.ErrorException != null)
            //    logger.Debug("Media Failed2:" + e.ErrorException.ToString());
        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    timer.Stop();
        //    Player_MediaEnded(null, null);
        //}

        public void ResetBrush()
        {
            VideoDrawing = new VideoDrawing();
            VideoDrawing.Player = player;
            VideoBrush = new DrawingBrush(VideoDrawing);
            BindingOperations.SetBinding(VideoBrush, DrawingBrush.StretchProperty, new Binding { Source = this, Path = new PropertyPath(StretchProperty), Mode = BindingMode.OneWay });
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            // if (IOManager.IsVideo(Source))
            if (this._isVideo)
            {
                VideoDrawing.Rect = new Rect(0, 0, player.NaturalVideoWidth, player.NaturalVideoHeight);
                player.Play();
            }
            else
            {
                VideoDrawing.Rect = new Rect(IOManager.GetImageSize(Source));

                //timer.Start();
            }
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            if (MediaEnded != null)
                MediaEnded.Invoke();
        }

        public void Play(Uri source, string sType)
        {
            if (sType.ToLower() == "v")
                this._isVideo = true;
            else
                this._isVideo = false;

            Source = source;
            player.Open(source);
        }

        #region Animation
        public void Play(Uri source, System.Windows.Media.Animation.Storyboard storyboard = null, string sType = "v")
        {
            Play(source, sType);

            if (storyboard != null)
            {
                //myStoryboard = storyboard;
                //storyboard.Begin();
                //storyboard.Begin(player as System.Windows.Media.MediaPlayer);

                storyboard.Begin(this);
            }
        }
        #endregion

        public void Stop(bool clear = false)
        {
            player.Stop();
            if (clear)
            {
                player.Open(null);
                ResetBrush();
            }
        }

        public void Pause()
        {
            if (player.CanPause)
                player.Pause();
        }
    }

    internal static class IOManager
    {
        public static List<string> VideoFileExtensions
        {
            get { return _videoFileExtensions; }
        }
        private static List<string> _videoFileExtensions = new List<string> { ".wmv", ".avi", ".mpg", ".mpeg", ".mp4", ".mkv" };

        //public static bool IsVideo(Uri uri, List<CeitconPlayer.UserControls.VideoExtention> _VideoExtention)
        //{
        //    if (uri == null)
        //        return false;
        //    return IsVideo(System.IO.Path.GetFileName(uri.LocalPath), _VideoExtention);
        //}

        //public static bool IsVideo(string key, List<CeitconPlayer.UserControls.VideoExtention> _VideoExtention)
        //{
        //    if (String.IsNullOrEmpty(key))
        //        return false;
        //    return VideoFileExtensions.Contains(System.IO.Path.GetExtension(key).ToLower());
        //}

        public static Size GetImageSize(Uri uri)
        {
            if (uri == null)
                return new Size();
            BitmapFrame frame = BitmapFrame.Create(uri);
            return new Size(frame.PixelWidth, frame.PixelHeight);
        }
    }
}
