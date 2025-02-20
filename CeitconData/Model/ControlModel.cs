using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Data.Utilities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using log4net;

namespace Ceitcon_Data.Model
{
    public enum ControlType
    {
        Image = 0,
        RichText = 1,
        Video = 2,
        Ticker = 3,
        Likebox = 4,
        Facebook = 5,
        Twitter = 6,
        Instagram = 7,
        Youtube = 8,
        SocialMediaImage = 9,
        Live = 10,
        Alert = 11,
        GifAnim = 12,
        Text = 13,
        DataGrid = 14,
        Weather = 15,
        DateTime = 16,
        PrayerImage = 17,
        PrayerText = 18,
        PrayerVideo = 19,
        PDF = 20,
        PPT = 21,
        WebBrowser = 22,
        PrayerYoutube = 23
    }

    public class ControlModel : INotifyPropertyChanged
    {
        private string _Id;
        //private LayerModel _Parent;
        private object _Parent;
        private string _Name;
        private ObservableCollection<PlaylistModel> _Playlist;
        private PlaylistModel _SelectedPlaylist;
        private double _Width;
        private double _Height;
        private double _X;
        private double _Y;
        private double _Opacity;
        private Brush _Background;
        private Brush _BorderBrush;
        private Thickness _BorderThickness;
        private CornerRadius _CornerRadius;
        private HorizontalAlignment _HorizontalAlignment;
        private VerticalAlignment _VerticalAlignment;
        private Stretch _Stretch;
        private bool _HorizontalFlip;
        private bool _VerticalFlip;
        private int _Rotate;
        private FontFamily _FontFamily;
        private double _FontSize;
        private Brush _Foreground;
        private string _Source;
        private string _Thumbnail;
        private string _Fajr;
        private long _FajrSize;
        private string _Dhuhr;
        private long _DhuhrSize;
        private string _Asr;
        private long _AsrSize;
        private string _Maghrib;
        private long _MaghribSize;
        private string _Isha;
        private long _IshaSize;
        public Uri _Url;
        private string _Text;
        private FontWeight _FontWeight;
        private FontStyle _FontStyle;
        private TextDecorationCollection _TextDecoration;
        private bool _InvertDirection;
        private TimeSpan _Duration;
        private SocialMediaModel _SelectedSocialMedia;
        private string _MediaAccountId;
        private string _MediaPageName;
        private bool _FlowDirection;
        private int _DateTimeFormat;
        private string _CustomDateTimeFormat;
        private int _ItemCount;
        private ControlType _Type;
        private int _ZIndex;
        private bool _IsVisible;
        private bool _IsLocked;
        private bool _IsSelected;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ControlModel(ControlType type, double x, double y, double width, double height, object parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _Name = String.Format("{0}_{1}", type.ToString(), _Id.Substring(0, 6));
            _Type = type;
            _Playlist = new ObservableCollection<PlaylistModel>();
            _Opacity = 1;
            _Stretch = Stretch.Fill;
            _X = x < 0 ? 0 : x;
            _Y = y < 0 ? 0 : y;
            _Width = width < 0 ? 0 : width;
            _Height = height < 0 ? 0 : height;
            log.Info($"x {x} ");
            log.Info($"y {y} ");
            log.Info($"height {height} ");
            log.Info($"widht {width} ");
            if (parent is LayerModel)
            {
                //_ZIndex = parent != null && parent.Controls.Count > 0 ? parent.Controls.Max(_ => _.ZIndex) + 1 : 1;
                _ZIndex = parent != null && (parent as LayerModel).Controls.Count > 0 ? (parent as LayerModel).Controls.Max(_ => _.ZIndex) + 1 : 1;
            }
            _IsVisible = true;

            switch (type)
            {
                case ControlType.Image:
                case ControlType.PrayerImage:
                    {
                        _Playlist.Add(new SetContentModel(this));
                    }
                    break;
                case ControlType.RichText:
                    {
                        _FontSize = 50;
                        _Foreground = Brushes.White;
                        _FontFamily = new FontFamily("Arial");
                        _HorizontalAlignment = HorizontalAlignment.Stretch;
                        _VerticalAlignment = VerticalAlignment.Stretch;
                        _Background = Brushes.Transparent;
                        _Playlist.Add(new SetContentModel(this) { Content = @"<html><body><h1 style=""color:yellow;"">Change Text</h1></body></html>" });
                    }
                    break;
                case ControlType.Video:
                case ControlType.PrayerVideo:
                    {
                        _Playlist.Add(new SetContentModel(this) { IsMuted = false });
                    }
                    break;
                case ControlType.GifAnim:
                    {
                        _Playlist.Add(new SetContentModel(this));
                    }
                    break;
                case ControlType.Ticker:
                    {
                        //_Text = "Text Ticker Test";
                        _FontSize = 100;
                        _Foreground = Brushes.Yellow;
                        _FontFamily = new FontFamily("Arial");
                        _HorizontalAlignment = HorizontalAlignment.Left;
                        _VerticalAlignment = VerticalAlignment.Top;
                        _FontWeight = FontWeights.Normal;
                        _FontStyle = FontStyles.Normal;
                        _TextDecoration = null;
                        _InvertDirection = false;
                        _Duration = new TimeSpan(0, 0, 20);
                        _Playlist.Add(new SetContentModel(this) { Content = "Change text in tab Playlist Set Content" });
                    }
                    break;
                case ControlType.Alert:
                    {
                        _FontSize = 30;
                        _Foreground = Brushes.Yellow;
                        _FontFamily = new FontFamily("Arial");
                        _HorizontalAlignment = HorizontalAlignment.Left;
                        _VerticalAlignment = VerticalAlignment.Top;
                        _Playlist.Add(new SetContentModel(this) { Content = "Change text in tab Playlist Set Content" });
                    }
                    break;
                case ControlType.Likebox:
                    {
                        _Url = new Uri("http://www.facebook.com");
                        _Source = "McDonalds";
                        _Text = "Likebox Content";
                        _FontSize = 50;
                        _Foreground = Brushes.Yellow;
                        _FontFamily = new FontFamily("Arial");
                        _MediaPageName = "McDonalds";
                    }
                    break;
                case ControlType.Facebook:
                    {
                        _Url = new Uri("http://www.facebook.com");
                        _Source = "McDonalds";
                        _Text = "Facebook Content";
                        _FontSize = 50;
                        _Foreground = Brushes.Yellow;
                        _FontFamily = new FontFamily("Arial");
                        _MediaPageName = "McDonalds";
                    }
                    break;
                case ControlType.Twitter:
                    {
                        _Url = new Uri("http://www.twitter.com");
                        _Source = "McDonalds";
                        _Text = "Twitter Content";
                        _FontSize = 50;
                        _Foreground = Brushes.Yellow;
                        _FontFamily = new FontFamily("Arial");
                    }
                    break;
                case ControlType.Instagram:
                    {
                        _Url = new Uri("http://www.instagram.com");
                        _Source = "McDonalds";
                        _Text = "Instagram Content";
                        _FontSize = 50;
                        _Foreground = Brushes.Yellow;
                        _FontFamily = new FontFamily("Arial");
                    }
                    break;
                case ControlType.Youtube:
                case ControlType.PrayerYoutube:
                    {
                        _Url = new Uri("http://www.youtube.com");
                        _Playlist.Add(new SetContentModel(this));
                    }
                    break;
                case ControlType.SocialMediaImage:
                    {
                    }
                    break;
                case ControlType.Live:
                    {
                        //rtsp://184.72.239.149/vod/mp4:BigBuckBunny_175k.mov
                        _Playlist.Add(new SetContentModel(this) { Content = @"https://cdn.flowplayer.com/a30bd6bc-f98b-47bc-abf5-97633d4faea0/hls/de3f6ca7-2db3-4689-8160-0f574a5996ad/playlist.m3u8" });
                    }
                    break;
                case ControlType.DataGrid:
                    {
                        _FontSize = 28;
                        _Foreground = Brushes.White;
                        _FontFamily = new FontFamily("Arial");
                        _HorizontalAlignment = HorizontalAlignment.Stretch;
                        _VerticalAlignment = VerticalAlignment.Stretch;
                        _Background = Brushes.Transparent;
                        _Playlist.Add(new SetContentModel(this) { DataGrid = new Data.DataGridModel()});
                    }
                    break;
                case ControlType.Weather:
                    {
                        _ItemCount = 1;
                        _Foreground = Brushes.White;
                        _FontSize = 20;
                        _FontFamily = new FontFamily("Arial");
                        _FontWeight = FontWeights.Normal;
                        _FontStyle = FontStyles.Normal;
                        _Playlist.Add(new SetContentModel(this) { Weather = new Data.WeatherModel() });
                    }
                    break;
                case ControlType.DateTime:
                    {
                        _FontSize = 50;
                        _Foreground = Brushes.Yellow;
                        _FontFamily = new FontFamily("Arial");
                        _HorizontalAlignment = HorizontalAlignment.Stretch;
                        _VerticalAlignment = VerticalAlignment.Stretch;
                        _Background = Brushes.Transparent;
                        _FlowDirection = false;
                        _DateTimeFormat = 0;
                        _CustomDateTimeFormat = String.Empty;
                        _Playlist.Add(new SetContentModel(this) { Content = DateTime.Now.ToString()});
                    }
                    break;
                case ControlType.Text:
                case ControlType.PrayerText:
                    {
                        _FontSize = 50;
                        _Foreground = Brushes.Yellow;
                        _FontFamily = new FontFamily("Arial");
                        _HorizontalAlignment = HorizontalAlignment.Stretch;
                        _VerticalAlignment = VerticalAlignment.Stretch;
                        _Background = Brushes.Transparent;
                        _Playlist.Add(new SetContentModel(this) { Content = "Change text in tab Playlist Set Content" });
                    }
                    break;
                case ControlType.PDF:
                    {
                        _Playlist.Add(new SetContentModel(this) { PageDuration = 2, DocumentFit = "Normal"});
                    }
                    break;
                case ControlType.PPT:
                    {
                        _Playlist.Add(new SetContentModel(this) { PageDuration = 2, DocumentFit = "Normal" });
                    }
                    break;
                case ControlType.WebBrowser:
                    {
                        _Url = new Uri("http://www.google.com");
                        _Playlist.Add(new SetContentModel(this));
                    }
                    break;
            }

            //For test
            //_Playlist.Add(new PlaylistContentModel(this));
            //_Playlist.Add(new PlaylistContentModel(this));
        }

        public ControlModel(ControlModel copy, object parent, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Parent = fullCopy ? copy.Parent : parent;
            _Name = fullCopy ? copy.Name : String.Format("{0}_{1}", copy.Type.ToString(), _Id.Substring(0, 6));
            _Type = copy.Type;
            _Width = copy.Width;
            _Height = copy.Height;
            _X = copy.X;
            _Y = copy.Y;
            _Opacity = copy.Opacity;
            _Background = copy.Background;
            _BorderBrush = copy.BorderBrush;
            _BorderThickness = copy.BorderThickness;
            _CornerRadius = copy.CornerRadius;
            _HorizontalAlignment = copy.HorizontalAlignment;
            _VerticalAlignment = copy.VerticalAlignment;
            _Stretch = copy.Stretch;
            _HorizontalFlip = copy.HorizontalFlip;
            _VerticalFlip = copy.VerticalFlip;
            _Rotate = copy.Rotate;
            _FontFamily = copy.FontFamily;
            _FontSize = copy.FontSize;
            _Foreground = copy.Foreground;
            _Source = copy.Source;
            _Thumbnail = copy.Thumbnail;
            _Fajr = copy.Fajr;
            _Dhuhr = copy.Dhuhr;
            _Asr = copy.Asr;
            _Maghrib = copy.Maghrib;
            _Isha = copy.Isha;
            _Url = copy.Url;
            _Text = copy.Text;
            _FontWeight = copy.FontWeight;
            _FontStyle = copy.FontStyle;
            _TextDecoration = copy.TextDecoration;
            _InvertDirection = copy.InvertDirection;
            _Duration = copy.Duration;
            _Type = copy.Type;
            _SelectedSocialMedia = copy.SelectedSocialMedia;
            _MediaAccountId = copy.MediaAccountId;
            _MediaPageName = copy.MediaPageName;
            _FlowDirection = copy.FlowDirection;
            _DateTimeFormat = copy.DateTimeFormat;
            _CustomDateTimeFormat = copy.CustomDateTimeFormat;
            _ItemCount = copy.ItemCount;
            _ZIndex = copy.ZIndex;
            _IsVisible = copy.IsVisible;
            _IsLocked = copy.IsLocked;
            _IsSelected = copy.IsSelected;
            _Playlist = new ObservableCollection<PlaylistModel>();
            foreach (PlaylistModel i in copy.Playlist)
            {
                switch (i.Type)
                {
                    case PlaylistType.AnimateBorder:
                        _Playlist.Add(new AnimateBorderModel(i as AnimateBorderModel, this));
                        break;
                    case PlaylistType.AnimateHeight:
                        _Playlist.Add(new AnimateHeightModel(i as AnimateHeightModel, this));
                        break;
                    case PlaylistType.AnimateMargin:
                        _Playlist.Add(new AnimateMarginModel(i as AnimateMarginModel, this));
                        break;
                    case PlaylistType.AnimateOpacity:
                        _Playlist.Add(new AnimateOpacityModel(i as AnimateOpacityModel, this));
                        break;
                    case PlaylistType.AnimateWidth:
                        _Playlist.Add(new AnimateWidthModel(i as AnimateWidthModel, this));
                        break;
                    case PlaylistType.Delay:
                        _Playlist.Add(new DelayModel(i as DelayModel, this));
                        break;
                    case PlaylistType.ResumePlayback:
                        _Playlist.Add(new ResumePlaybackModel(i as ResumePlaybackModel, this));
                        break;
                    case PlaylistType.SetContent:
                        _Playlist.Add(new SetContentModel(i as SetContentModel, this, fullCopy));
                        break;
                    case PlaylistType.SuspendPlayback:
                        _Playlist.Add(new SuspendPlaybackModel(i as SuspendPlaybackModel, this));
                        break;
                }
            }
        }

        public ControlModel Save()
        {
            return new ControlModel(this, null, true);
        }

        public void Restore(ControlModel copy)
        {
            Memento.Enable = false;
            _Id = copy.Id;
            Parent = copy.Parent;
            Name = copy.Name;
            Type = copy.Type;
            Width = copy.Width;
            Height = copy.Height;
            X = copy.X;
            Y = copy.Y;
            Opacity = copy.Opacity;
            Background = copy.Background;
            BorderBrush = copy.BorderBrush;
            BorderThickness = copy.BorderThickness;
            CornerRadius = copy.CornerRadius;
            HorizontalAlignment = copy.HorizontalAlignment;
            VerticalAlignment = copy.VerticalAlignment;
            Stretch = copy.Stretch;
            HorizontalFlip = copy.HorizontalFlip;
            VerticalFlip = copy.VerticalFlip;
            Rotate = copy.Rotate;
            FontFamily = copy.FontFamily;
            FontSize = copy.FontSize;
            Foreground = copy.Foreground;
            Source = copy.Source;
            Thumbnail = copy.Thumbnail;
            Fajr = copy.Fajr;
            Dhuhr = copy.Dhuhr;
            Asr = copy.Asr;
            Maghrib = copy.Maghrib;
            Isha = copy.Isha;
            Url = copy.Url;
            Text = copy.Text;
            FontWeight = copy.FontWeight;
            FontStyle = copy.FontStyle;
            TextDecoration = copy.TextDecoration;
            InvertDirection = copy.InvertDirection;
            Duration = copy.Duration;
            Type = copy.Type;
            SelectedSocialMedia = copy.SelectedSocialMedia;
            MediaAccountId = copy.MediaAccountId;
            MediaPageName = copy.MediaPageName;
            FlowDirection = copy.FlowDirection;
            DateTimeFormat = copy.DateTimeFormat;
            CustomDateTimeFormat = copy.CustomDateTimeFormat;
            ItemCount = copy.ItemCount;
            ZIndex = copy.ZIndex;
            IsVisible = copy.IsVisible;
            IsLocked = copy.IsLocked;
            IsSelected = copy.IsSelected;
            Playlist.Clear();
            foreach (PlaylistModel i in copy.Playlist)
            {
                switch (i.Type)
                {
                    case PlaylistType.AnimateBorder:
                        Playlist.Add(new AnimateBorderModel(i as AnimateBorderModel, this));
                        break;
                    case PlaylistType.AnimateHeight:
                        Playlist.Add(new AnimateHeightModel(i as AnimateHeightModel, this));
                        break;
                    case PlaylistType.AnimateMargin:
                        Playlist.Add(new AnimateMarginModel(i as AnimateMarginModel, this));
                        break;
                    case PlaylistType.AnimateOpacity:
                        Playlist.Add(new AnimateOpacityModel(i as AnimateOpacityModel, this));
                        break;
                    case PlaylistType.AnimateWidth:
                        Playlist.Add(new AnimateWidthModel(i as AnimateWidthModel, this));
                        break;
                    case PlaylistType.Delay:
                        Playlist.Add(new DelayModel(i as DelayModel, this));
                        break;
                    case PlaylistType.ResumePlayback:
                        Playlist.Add(new ResumePlaybackModel(i as ResumePlaybackModel, this));
                        break;
                    case PlaylistType.SetContent:
                        Playlist.Add(new SetContentModel(i as SetContentModel, this, true));
                        break;
                    case PlaylistType.SuspendPlayback:
                        Playlist.Add(new SuspendPlaybackModel(i as SuspendPlaybackModel, this));
                        break;
                }
            }
            //PlaylistModel _SelectedPlaylist = copy.SelectedPlaylist;
            Memento.Enable = true;
        }

        public string Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public object Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent != value)
                {
                    Memento.Push(Save());
                    _Parent = value;
                    OnPropertyChanged("Parent");
                }
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    Memento.Push(Save());
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<PlaylistModel> Playlist
        {
            get { return _Playlist; }
            set
            {
                if (_Playlist != value)
                {
                    //Memento.Push(Save());
                    _Playlist = value;
                    OnPropertyChanged("Playlist");
                    OnPropertyChanged("PlaylistWithoutSelected");
                }
            }
        }

        public ObservableCollection<PlaylistModel> PlaylistWithoutSelected
        {
            get { return new ObservableCollection<PlaylistModel>(_Playlist.ToArray().Where(_ => SelectedPlaylist == null || _.Id != SelectedPlaylist.Id).ToList());}
        }

        public PlaylistModel SelectedPlaylist
        {
            get { return _SelectedPlaylist; }
            set
            {
                if (_SelectedPlaylist != value)
                {
                    //Memento.Push(Save());
                    _SelectedPlaylist = value;
                    OnPropertyChanged("SelectedPlaylist");
                    OnPropertyChanged("PlaylistWithoutSelected");
                }
            }
        }

        public double Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > MaxWidth)
                        value = MaxWidth;

                    Memento.Push(Save());
                    _Width = value;
                    OnPropertyChanged("Width");
                    OnPropertyChanged("X");
                    OnPropertyChanged("W");
                    OnPropertyChanged("CurrentWidth");
                }
            }
        }

        public double MaxWidth
        {
            get
            {
                if (_Parent is LayerModel)
                {
                    return (_Parent as LayerModel).Parent.Parent.Width;
                }
                else if (_Parent is AlertModel)
                {
                    return (_Parent as AlertModel).Parent.SelectedRegion.Width;
                }
                return 0;
            }
        }

        public double CurrentWidth
        {
            get
            {
                double result = Width - BorderThickness.Left - BorderThickness.Right;
                return result > 0 ? result : 0;
            }
        }

        public double Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > MaxHeight)
                        value = MaxHeight;

                    Memento.Push(Save());
                    _Height = value;
                    OnPropertyChanged("Height");
                    OnPropertyChanged("Y");
                    OnPropertyChanged("Z");
                    OnPropertyChanged("CurrentHeight");
                }
            }
        }

        public double MaxHeight
        {
            get
            {
                if (_Parent is LayerModel)
                {
                    return (_Parent as LayerModel).Parent.Parent.Height;
                }
                else if (_Parent is AlertModel)
                {
                    return (_Parent as AlertModel).Parent.SelectedRegion.Height;
                }
                return 0;
            }
        }

        public double CurrentHeight
        {
            get
            {
                double result = Height - BorderThickness.Top - BorderThickness.Bottom;
                return result > 0 ? result : 0;
            }
        }

        public double X
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > MaxWidth)
                        value = MaxWidth - 1;

                    Memento.Push(Save());
                    _X = value;
                    OnPropertyChanged("X");
                    OnPropertyChanged("XX");
                    OnPropertyChanged("Width");
                    OnPropertyChanged("W");
                }
            }
        }

        public double XX
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    _Width -= value - _X;
                    X = value;
                }
            }
        }

        public double Y
        {
            get { return _Y; }
            set
            {
                if (_Y != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > MaxHeight)
                        value = MaxHeight - 1;

                    Memento.Push(Save());
                    _Y = value;
                    OnPropertyChanged("Y");
                    OnPropertyChanged("YY");
                    OnPropertyChanged("Height");
                    OnPropertyChanged("Z");
                }
            }
        }

        public double YY
        {
            get { return _Y; }
            set
            {
                if (_Y != value)
                {
                    _Height -= value - _Y;
                    Y = value;
                }
            }
        }

        public double W
        {
            get
            {
                if (_Parent is AlertModel)
                {
                    return (_Parent as AlertModel).Parent.SelectedRegion.Width - _X - _Width;
                }
                else
                {
                    return (_Parent as LayerModel).Parent.Parent.Width - _X - _Width;
                }
            }
            set
            {
                if (_Parent is AlertModel)
                {
                    Width = (_Parent as AlertModel).Parent.SelectedRegion.Width - value - _X;
                }
                else
                {
                    Width = (_Parent as LayerModel).Parent.Parent.Width - value - _X;
                }
                OnPropertyChanged("W");
            }
        }

        public double Z
        {

            get
            {
                if (_Parent is AlertModel)
                {
                    return (_Parent as AlertModel).Parent.SelectedRegion.Height - _Y - _Height;
                }
                else
                {
                    return (_Parent as LayerModel).Parent.Parent.Height - _Y - _Height;
                }
            }
            set
            {
                if (_Parent is AlertModel)
                {
                    Height = (_Parent as AlertModel).Parent.SelectedRegion.Height - value - _Y;
                }
                else
                {
                    Height = (_Parent as LayerModel).Parent.Parent.Height - value - _Y;
                }
                OnPropertyChanged("Z");
            }
        }

        public double Opacity
        {
            get { return _Opacity; }
            set
            {
                if (_Opacity != value)
                {
                    Memento.Push(Save());
                    _Opacity = value;
                    OnPropertyChanged("Opacity");
                }
            }
        }

        public Brush Background
        {
            get { return _Background; }
            set
            {
                if (_Background != value)
                {
                    Memento.Push(Save());
                    _Background = value;
                    OnPropertyChanged("Background");
                    OnPropertyChanged("BackgroundBrush");
                }
            }
        }

        //private Brush _BackgroundBrush;
        //public Brush BackgroundBrush
        //{
        //    get { return _BackgroundBrush; }
        //    set
        //    {
        //        if (_BackgroundBrush != value)
        //        {
        //            _BackgroundBrush = value;
        //            OnPropertyChanged("BackgroundBrush");
        //        }
        //    }
        //}

        //public Brush BackgroundBrush
        //{
        //    get { return new SolidColorBrush(_Background); }
        //}

        public Brush BorderBrush
        {
            get { return _BorderBrush; }
            set
            {
                if (_BorderBrush != value)
                {
                    Memento.Push(Save());
                    _BorderBrush = value;
                    OnPropertyChanged("BorderBrush");
                    OnPropertyChanged("BorderBrushBrush");
                }
            }
        }

        //public Brush BorderBrushBrush
        //{
        //    get { return new SolidColorBrush(_BorderBrush); }
        //}

        public Thickness BorderThickness
        {
            get { return _BorderThickness; }
            set
            {
                if (_BorderThickness != value)
                {
                    Memento.Push(Save());
                    _BorderThickness = value;
                    OnPropertyChanged("BorderThickness");
                    OnPropertyChanged("BorderThicknessLeft");
                    OnPropertyChanged("BorderThicknessRight");
                    OnPropertyChanged("BorderThicknessTop");
                    OnPropertyChanged("BorderThicknessBottom");
                }
            }
        }

        public double BorderThicknessLeft
        {
            get { return _BorderThickness.Left; }
            set
            {
                if (_BorderThickness.Left != value)
                {
                    Memento.Push(Save());
                    _BorderThickness.Left = value;
                    OnPropertyChanged("BorderThicknessLeft");
                    OnPropertyChanged("BorderThickness");
                    OnPropertyChanged("CurrentWidth");
                }
            }
        }

        public double BorderThicknessRight
        {
            get { return _BorderThickness.Right; }
            set
            {
                if (_BorderThickness.Right != value)
                {
                    Memento.Push(Save());
                    _BorderThickness.Right = value;
                    OnPropertyChanged("BorderThicknessRight");
                    OnPropertyChanged("BorderThickness");
                    OnPropertyChanged("CurrentWidth");
                }
            }
        }

        public double BorderThicknessTop
        {
            get { return _BorderThickness.Top; }
            set
            {
                if (_BorderThickness.Top != value)
                {
                    Memento.Push(Save());
                    _BorderThickness.Top = value;
                    OnPropertyChanged("BorderThicknessTop");
                    OnPropertyChanged("BorderThickness");
                    OnPropertyChanged("CurrentHeight");
                }
            }
        }

        public double BorderThicknessBottom
        {
            get { return _BorderThickness.Bottom; }
            set
            {
                if (_BorderThickness.Bottom != value)
                {
                    Memento.Push(Save());
                    _BorderThickness.Bottom = value;
                    OnPropertyChanged("BorderThicknessBottom");
                    OnPropertyChanged("BorderThickness");
                    OnPropertyChanged("CurrentHeight");
                }
            }
        }

        public CornerRadius CornerRadius
        {
            get { return _CornerRadius; }
            set
            {
                if (_CornerRadius != value)
                {
                    Memento.Push(Save());
                    _CornerRadius = value;
                    OnPropertyChanged("CornerRadius");
                    OnPropertyChanged("CornerRadiusTopLeft");
                    OnPropertyChanged("CornerRadiusTopRight");
                    OnPropertyChanged("CornerRadiusBottomRight");
                    OnPropertyChanged("CornerRadiusBottomLeft");
                }
            }
        }

        public double CornerRadiusTopLeft
        {
            get { return _CornerRadius.TopLeft; }
            set
            {
                if (_CornerRadius.TopLeft != value)
                {
                    Memento.Push(Save());
                    _CornerRadius.TopLeft = value;
                    OnPropertyChanged("CornerRadiusTopLeft");
                    OnPropertyChanged("CornerRadius");
                }
            }
        }

        public double CornerRadiusTopRight
        {
            get { return _CornerRadius.TopRight; }
            set
            {
                if (_CornerRadius.TopRight != value)
                {
                    Memento.Push(Save());
                    _CornerRadius.TopRight = value;
                    OnPropertyChanged("CornerRadiusTopRight");
                    OnPropertyChanged("CornerRadius");
                }
            }
        }

        public double CornerRadiusBottomRight
        {
            get { return _CornerRadius.BottomRight; }
            set
            {
                if (_CornerRadius.BottomRight != value)
                {
                    Memento.Push(Save());
                    _CornerRadius.BottomRight = value;
                    OnPropertyChanged("CornerRadiusBottomRight");
                    OnPropertyChanged("CornerRadius");
                }
            }
        }

        public double CornerRadiusBottomLeft
        {
            get { return _CornerRadius.BottomLeft; }
            set
            {
                if (_CornerRadius.BottomLeft != value)
                {
                    Memento.Push(Save());
                    _CornerRadius.BottomLeft = value;
                    OnPropertyChanged("CornerRadiusBottomLeft");
                    OnPropertyChanged("CornerRadius");
                }
            }
        }


        public HorizontalAlignment HorizontalAlignment
        {
            get { return _HorizontalAlignment; }
            set
            {
                if (_HorizontalAlignment != value)
                {
                    Memento.Push(Save());
                    _HorizontalAlignment = value;
                    OnPropertyChanged("HorizontalAlignment");
                }
            }
        }

        //public IEnumerable<HorizontalAlignment> HorizontalAlignmentCollection
        //{
        //    get
        //    {
        //        return Enum.GetValues(typeof(HorizontalAlignment))
        //            .Cast<HorizontalAlignment>();
        //    }
        //}


        public VerticalAlignment VerticalAlignment
        {
            get { return _VerticalAlignment; }
            set
            {
                if (_VerticalAlignment != value)
                {
                    Memento.Push(Save());
                    _VerticalAlignment = value;
                    OnPropertyChanged("VerticalAlignment");
                }
            }
        }

        public Stretch Stretch
        {
            get { return _Stretch; }
            set
            {
                if (_Stretch != value)
                {
                    Memento.Push(Save());
                    _Stretch = value;
                    OnPropertyChanged("Stretch");
                }
            }
        }

        public bool HorizontalFlip
        {
            get { return _HorizontalFlip; }
            set
            {
                if (_HorizontalFlip != value)
                {
                    Memento.Push(Save());
                    _HorizontalFlip = value;
                    if (value)
                    {
                        Rotate = 0;
                        OnPropertyChanged("Rotate");
                    }
                    OnPropertyChanged("HorizontalFlip");
                    OnPropertyChanged("ScaleTransform");
                }
            }
        }

        public bool VerticalFlip
        {
            get { return _VerticalFlip; }
            set
            {
                if (_VerticalFlip != value)
                {
                    Memento.Push(Save());
                    _VerticalFlip = value;
                    if (value)
                    {
                        Rotate = 0;
                        OnPropertyChanged("Rotate");
                    }
                    OnPropertyChanged("VerticalFlip");
                    OnPropertyChanged("ScaleTransform");
                }
            }
        }

        public int Rotate
        {
            get { return _Rotate; }
            set
            {
                if (_Rotate != value)
                {
                    if (value > 360)
                        value = 360;
                    if (value < -360)
                        value = -360;
                    Memento.Push(Save());
                    _Rotate = value;
                    if (value > 0)
                    {
                        
                        VerticalFlip = false;
                        HorizontalFlip = false;
                        OnPropertyChanged("HorizontalFlip");
                        OnPropertyChanged("VerticalFlip");
                    }
                    OnPropertyChanged("Rotate");
                    OnPropertyChanged("ScaleTransform");
                }
            }
        }

        public object ScaleTransform
        {
            get
            {
                if (Rotate > 0)
                {
                    return new RotateTransform(Rotate);
                }
                else
                {
                    return _HorizontalFlip || _VerticalFlip ? new ScaleTransform(_HorizontalFlip ? -1 : 1, _VerticalFlip ? -1 : 1) : null;
                }
            }
        }

        public FontFamily FontFamily
        {
            get { return _FontFamily; }
            set
            {
                if (_FontFamily != value)
                {
                    Memento.Push(Save());
                    _FontFamily = value;
                    OnPropertyChanged("FontFamily");
                }
            }
        }

        public double FontSize
        {
            get { return _FontSize; }
            set
            {
                if (_FontSize != value)
                {
                    Memento.Push(Save());
                    _FontSize = value;
                    OnPropertyChanged("FontSize");
                }
            }
        }

        public Brush Foreground
        {
            get { return _Foreground; }
            set
            {
                if (_Foreground != value)
                {
                    Memento.Push(Save());
                    _Foreground = value;
                    OnPropertyChanged("Foreground");
                }
            }
        }

        public string Source
        {
            get { return _Source; }
            set
            {
                if (_Source != value)
                {
                    Memento.Push(Save());
                    _Source = value;
                    OnPropertyChanged("Source");

                    if (String.IsNullOrEmpty(value))
                        Thumbnail = null;

                    if (_Type == ControlType.Video)
                    {
                        string thumbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Thumbs", Path.GetFileNameWithoutExtension(value) + "_thumb.jpg");
                        Thumbnail = VideoConverter.GenerateImage(value, thumbPath);
                    }
                }
            }
        }


        public string Thumbnail
        {
            get { return _Thumbnail; }
            set
            {
                if (_Thumbnail != value)
                {
                    _Thumbnail = value;
                    OnPropertyChanged("Thumbnail");
                }
            }
        }

        public string Fajr
        {
            get { return _Fajr; }
            set
            {
                if (_Fajr != value)
                {
                    Memento.Push(Save());
                    _Fajr = value;
                    OnPropertyChanged("Fajr");

                    if (String.IsNullOrEmpty(value))
                        Thumbnail = null;

                    if (_Type == ControlType.PrayerVideo)
                    {
                        string thumbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Thumbs", Path.GetFileNameWithoutExtension(value) + "_thumb.jpg");
                        Thumbnail = VideoConverter.GenerateImage(value, thumbPath);
                    }
                }
            }
        }

        public long FajrSize
        {
            get { return _FajrSize; }
            set
            {
                if (_FajrSize != value)
                {
                    Memento.Push(Save());
                    _FajrSize = value;
                    OnPropertyChanged("FajrSize");
                }
            }
        }

        public string Dhuhr
        {
            get { return _Dhuhr; }
            set
            {
                if (_Dhuhr != value)
                {
                    Memento.Push(Save());
                    _Dhuhr = value;
                    OnPropertyChanged("Dhuhr");
                }
            }
        }

        public long DhuhrSize
        {
            get { return _DhuhrSize; }
            set
            {
                if (_DhuhrSize != value)
                {
                    Memento.Push(Save());
                    _DhuhrSize = value;
                    OnPropertyChanged("DhuhrSize");
                }
            }
        }
        public string Asr
        {
            get { return _Asr; }
            set
            {
                if (_Asr != value)
                {
                    Memento.Push(Save());
                    _Asr = value;
                    OnPropertyChanged("Asr");
                }
            }
        }
        public long AsrSize
        {
            get { return _AsrSize; }
            set
            {
                if (_AsrSize != value)
                {
                    Memento.Push(Save());
                    _AsrSize = value;
                    OnPropertyChanged("AsrSize");
                }
            }
        }
        public string Maghrib
        {
            get { return _Maghrib; }
            set
            {
                if (_Maghrib != value)
                {
                    Memento.Push(Save());
                    _Maghrib = value;
                    OnPropertyChanged("Maghrib");
                }
            }
        }

        public long MaghribSize
        {
            get { return _MaghribSize; }
            set
            {
                if (_MaghribSize != value)
                {
                    Memento.Push(Save());
                    _MaghribSize = value;
                    OnPropertyChanged("MaghribSize");
                }
            }
        }
        public string Isha
        {
            get { return _Isha; }
            set
            {
                if (_Isha != value)
                {
                    Memento.Push(Save());
                    _Isha = value;
                    OnPropertyChanged("Isha");
                }
            }
        }
        public long IshaSize
        {
            get { return _IshaSize; }
            set
            {
                if (_IshaSize != value)
                {
                    Memento.Push(Save());
                    _IshaSize = value;
                    OnPropertyChanged("IshaSize");
                }
            }
        }
        public Uri Url
        {
            get { return _Url; }
            set
            {
                if (_Url != value)
                {
                    Memento.Push(Save());
                    _Url = value;
                    OnPropertyChanged("Url");
                }
            }
        }
        
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    Memento.Push(Save());
                    _Text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public bool InvertDirection
        {
            get { return _InvertDirection; }
            set
            {
                if (_InvertDirection != value)
                {
                    Memento.Push(Save());
                    _InvertDirection = value;
                    OnPropertyChanged("InvertDirection");
                }
            }
        }

        public FontWeight FontWeight
        {
            get { return _FontWeight; }
            set
            {
                if (_FontWeight != value)
                {
                    Memento.Push(Save());
                    _FontWeight = value;
                    OnPropertyChanged("FontWeight");
                }
            }
        }

        public FontStyle FontStyle
        {
            get { return _FontStyle; }
            set
            {
                if (_FontStyle != value)
                {
                    Memento.Push(Save());
                    _FontStyle = value;
                    OnPropertyChanged("FontStyle");
                }
            }
        }

        public TextDecorationCollection TextDecoration
        {
            get { return _TextDecoration; }
            set
            {
                if (_TextDecoration != value)
                {
                    Memento.Push(Save());
                    _TextDecoration = value;
                    OnPropertyChanged("TextDecoration");
                }
            }
        }

        public string TextDecorationText
        {
            get
            {
                string result = "Normal";

                if (_TextDecoration == null)
                    result = "Normal";
                else if (_TextDecoration == TextDecorations.Strikethrough)
                    result = "Strikethrough";
                else if (_TextDecoration == TextDecorations.OverLine)
                    result = "OverLine";
                else if (_TextDecoration == TextDecorations.Baseline)
                    result = "Baseline";
                else if (_TextDecoration == TextDecorations.Underline)
                    result = "Underline";
                else
                    result = "Normal";
                return result;
            }
            set
            {
                switch (value)
                {
                    case "Normal":
                        TextDecoration = null;
                        break;
                    case "Strikethrough":
                        TextDecoration = TextDecorations.Strikethrough;
                        break;
                    case "OverLine":
                        TextDecoration = TextDecorations.OverLine;
                        break;
                    case "Baseline":
                        TextDecoration = TextDecorations.Baseline;
                        break;
                    case "Underline":
                        TextDecoration = TextDecorations.Underline;
                        break;
                    default:
                        TextDecoration = null;
                        break;
                }
                OnPropertyChanged("TextDecorationText");
            }
        }

        public List<string> TextDecorationValues = new List<string> { "Nothing", "Strikethrough", "OverLine", "Baseline", "Underline" };

        public TimeSpan Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration != value)
                {
                    Memento.Push(Save());
                    _Duration = value;
                    OnPropertyChanged("Duration");
                }
            }
        }

        public SocialMediaModel SelectedSocialMedia
        {
            get { return _SelectedSocialMedia; }
            set
            {
                if (_SelectedSocialMedia != value)
                {
                    //Memento.Push(Save());
                    _SelectedSocialMedia = value;
                    Source = _SelectedSocialMedia.Image.ToString();
                    OnPropertyChanged("SelectedSocialMedia");
                }
            }
        }

        public ControlType Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    Memento.Push(Save());
                    _Type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public string MediaAccountId
        {
            get { return _MediaAccountId; }
            set
            {
                if (_MediaAccountId != value)
                {
                    Memento.Push(Save());
                    _MediaAccountId = value;
                    OnPropertyChanged("MediaAccountId");
                }
            }
        }

        public string MediaPageName
        {
            get { return _MediaPageName; }
            set
            {
                if (_MediaPageName != value)
                {
                    Memento.Push(Save());
                    _MediaPageName = value;
                    OnPropertyChanged("MediaPageName");
                }
            }
        }

        public bool FlowDirection
        {
            get { return _FlowDirection; }
            set
            {
                if (_FlowDirection != value)
                {
                    Memento.Push(Save());
                    _FlowDirection = value;
                    OnPropertyChanged("FlowDirection");
                    OnPropertyChanged("DataGridImage");
                    if(_Type==ControlType.DateTime)
                        OnPropertyChanged("DateTimeText");
                }
            }
        }


        public System.Windows.Media.Imaging.BitmapImage DataGridImage
        {
            get
            {
                return FlowDirection ?
                  new System.Windows.Media.Imaging.BitmapImage(new Uri(@"../Images/iconDataGrid_Arabian.png", UriKind.Relative)) :
                  new System.Windows.Media.Imaging.BitmapImage(new Uri(@"../Images/iconDataGrid.png", UriKind.Relative));
            }
        }

        public int DateTimeFormat
        {
            get { return _DateTimeFormat; }
            set
            {
                if (_DateTimeFormat != value)
                {
                    Memento.Push(Save());
                    _DateTimeFormat = value;
                    OnPropertyChanged("DateTimeFormat");
                    OnPropertyChanged("DateTimeText");
                }
            }
        }

        public string CustomDateTimeFormat
        {
            get { return _CustomDateTimeFormat; }
            set
            {
                if (_CustomDateTimeFormat != value)
                {
                    Memento.Push(Save());
                    _CustomDateTimeFormat = value;
                    OnPropertyChanged("CustomDateTimeFormat");
                    OnPropertyChanged("DateTimeText");
                }
            }
        }

        public string DateTimeText
        {
            get {

                string filter = String.Empty;
                if (!String.IsNullOrWhiteSpace(_CustomDateTimeFormat))
                {
                    filter = _CustomDateTimeFormat;
                }
                else
                {
                    switch (_DateTimeFormat)
                    {
                        case 0:
                            {   //DateTime
                                filter = "dd/MM/yyyy HH:mm:ss";
                            }
                            break;
                        case 1:
                            {   //Date
                                filter = "dd/MM/yyyy";
                            }
                            break;
                        case 2:
                            {   //Time
                                filter = "HH:mm:ss";
                            }
                            break;
                    }
                }

                try
                {

                        if (FlowDirection)
                        {
                            return Text = GetArabicData(DateTime.Now.ToString(filter, new CultureInfo("ar-SA")));
                        }
                        else
                        {
                            return Text = DateTime.Now.ToString(filter);
                        }
                }
                catch (Exception)
                {
                    return Text = "Wrong custom format";
                }
            }
        }

        private string GetArabicData(string _input)
        {
            string sResult = "";
            foreach (char item in _input)
            {
                switch (item)
                {
                    case '1':
                        sResult = sResult + "١";
                        break;
                    case '2':
                        sResult = sResult + "٢";
                        break;
                    case '3':
                        sResult = sResult + "٣";
                        break;
                    case '4':
                        sResult = sResult + "٤";
                        break;
                    case '5':
                        sResult = sResult + "٥";
                        break;
                    case '6':
                        sResult = sResult + "٦";
                        break;
                    case '7':
                        sResult = sResult + "٧";
                        break;
                    case '8':
                        sResult = sResult + "٨";
                        break;
                    case '9':
                        sResult = sResult + "٩";
                        break;
                    case '0':
                        sResult = sResult + "٠";
                        break;

                    default:
                        sResult = sResult + item.ToString();
                        break;
                }
            }

            return sResult;
        }

        public int ItemCount
        {
            get { return _ItemCount; }
            set
            {
                if (_ItemCount != value)
                {
                    Memento.Push(Save());
                    _ItemCount = value;
                    OnPropertyChanged("ItemCount");
                }
            }
        }

        public int ZIndex
        {
            get { return _ZIndex; }
            set
            {
                if (_ZIndex != value)
                {
                    Memento.Push(Save());
                    _ZIndex = value;
                    OnPropertyChanged("ZIndex");
                }
            }
        }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (_IsVisible != value)
                {
                    Memento.Push(Save());
                    _IsVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        public bool IsLocked
        {
            get { return _IsLocked; }
            set
            {
                if (_IsLocked != value)
                {
                    Memento.Push(Save());
                    _IsLocked = value;
                    OnPropertyChanged("IsLocked");
                }
            }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                   // Memento.Push(Save());
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
