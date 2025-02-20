using Ceitcon_Data.Model.Data;
using Ceitcon_Data.Utilities;

namespace Ceitcon_Data.Model.Playlist
{
    public class SetContentModel : PlaylistModel
    {
        public SetContentModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.SetContent;
            _Name = "Set Content";

            if(parent.Type == ControlType.Weather)
                _Content = "Riyadh";
        }

        public SetContentModel(SetContentModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.SetContent;
            _Name = copy.Name;
            _Content = copy.Content;
            _ContentSize = copy.ContentSize;
            _IsMuted = copy.IsMuted;
            _PageDuration = copy.PageDuration;
            _DocumentFit = copy.DocumentFit;
            _Fajr = copy.Fajr;
            _FajrSize = copy.FajrSize;
            _Dhuhr = copy.Dhuhr;
            _DhuhrSize = copy.DhuhrSize;
            _Asr = copy.Asr;
            _AsrSize = copy.AsrSize;
            _Maghrib = copy.Maghrib;
            _MaghribSize = copy.MaghribSize;
            _Isha = copy.Isha;
            _IshaSize = copy.IshaSize;
            _DataGrid = copy.DataGrid;
            _Weather = copy.Weather;
        }

        public override PlaylistModel Save() => new SetContentModel(this, null, true);

        public override void Restore(object copyObj)
        {
            var copy = copyObj as SetContentModel;
            Memento.Enable = false;
            base.Restore(copy);
            Content = copy.Content;
            IsMuted = copy.IsMuted;
            PageDuration = copy.PageDuration;
            DocumentFit = copy.DocumentFit;
            Fajr = copy.Fajr;
            FajrSize = copy.FajrSize;
            Dhuhr = copy.Dhuhr;
            DhuhrSize = copy.DhuhrSize;
            Asr = copy.Asr;
            AsrSize = copy.AsrSize;
            Maghrib = copy.Maghrib;
            MaghribSize = copy.MaghribSize;
            Isha = copy.Isha;
            IshaSize = copy.IshaSize;
            DataGrid = copy.DataGrid;
            Weather = copy.Weather;
            Memento.Enable = true;
        }

        private string _Content;
        private long _ContentSize;
        private bool _IsMuted;
        private int _PageDuration;
        private string _DocumentFit;
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
        private DataGridModel _DataGrid;
        private WeatherModel _Weather;

        public string Content
        {
            get { return _Content; }
            set
            {
                if (_Content != value)
                {
                    Memento.Push(Save());
                    _Content = value;
                    if (TextMode || RichTextMode || DisabledTextMode)
                        { Parent.Text = value; }
                    else { Parent.Source = value; }
                    OnPropertyChanged("Content");
                }
            }
        }

        public long ContentSize
        {
            get { return _ContentSize; }
            set
            {
                if (_ContentSize != value)
                {
                    _ContentSize = value;
                    OnPropertyChanged("ContentSize");
                }
            }
        }

        public bool IsMuted
        {
            get { return _IsMuted; }
            set
            {
                if (_IsMuted != value)
                {
                    _IsMuted = value;
                    OnPropertyChanged("IsMuted");
                }
            }
        }

        public int PageDuration
        {
            get { return _PageDuration; }
            set
            {
                if (_PageDuration != value)
                {
                    Memento.Push(Save());
                    _PageDuration = value;
                    OnPropertyChanged("PageDuration");
                }
            }
        }

        public string DocumentFit
        {
            get { return _DocumentFit; }
            set
            {
                if (_DocumentFit != value)
                {
                    Memento.Push(Save());
                    _DocumentFit = value;
                    OnPropertyChanged("DocumentFit");
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
                    _Fajr = value;
                    Parent.Fajr = value;
                    OnPropertyChanged("Fajr");
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
                    _FajrSize = value;
                    Parent.FajrSize = value;
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
                    _Dhuhr = value;
                    Parent.Dhuhr = value;
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
                    _DhuhrSize = value;
                    Parent.DhuhrSize = value;
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
                    _Asr = value;
                    Parent.Asr = value;
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
                    _AsrSize = value;
                    Parent.AsrSize = value;
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
                    _Maghrib = value;
                    Parent.Maghrib = value;
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
                    _MaghribSize = value;
                    Parent.MaghribSize = value;
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
                    _Isha = value;
                    Parent.Isha = value;
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
                    _IshaSize = value;
                    Parent.IshaSize = value;
                    OnPropertyChanged("IshaSize");
                }
            }
        }

        public DataGridModel DataGrid
        {
            get { return _DataGrid; }
            set
            {
                if (_DataGrid != value)
                {
                    _DataGrid = value;
                    OnPropertyChanged("DataGrid");
                }
            }
        }

        public WeatherModel Weather
        {
            get { return _Weather; }
            set
            {
                if (_Weather != value)
                {
                    _Weather = value;
                    OnPropertyChanged("Weather");
                }
            }
        }

        public bool SourceMode => Parent.Type == ControlType.Image ||  Parent.Type == ControlType.Video || Parent.Type == ControlType.GifAnim || Parent.Type == ControlType.PDF || Parent.Type == ControlType.PPT;

        public bool AudioMode => Parent.Type == ControlType.Video || Parent.Type == ControlType.PrayerVideo;

        public bool TextMode => Parent.Type == ControlType.Text || Parent.Type == ControlType.Ticker || Parent.Type == ControlType.Likebox 
            || Parent.Type == ControlType.Facebook || Parent.Type == ControlType.Twitter || Parent.Type == ControlType.Instagram
            || Parent.Type == ControlType.Youtube || Parent.Type == ControlType.PrayerYoutube || Parent.Type == ControlType.SocialMediaImage || Parent.Type == ControlType.Live 
            || Parent.Type == ControlType.Alert || Parent.Type == ControlType.SocialMediaImage || Parent.Type == ControlType.WebBrowser;
        
        public bool DisabledTextMode => Parent.Type == ControlType.DateTime;

        public bool RichTextMode => Parent.Type == ControlType.RichText; 

        public bool DataGridMode => Parent.Type == ControlType.DataGrid;

        public bool IntervalMode => Parent.Type == ControlType.Weather;

        public bool PrayerSourceMode => Parent.Type == ControlType.PrayerImage || Parent.Type == ControlType.PrayerVideo;

        public bool PrayerTextMode => Parent.Type == ControlType.PrayerText;

        public bool PDFMode => Parent.Type == ControlType.PDF;

        public bool PPTMode => Parent.Type == ControlType.PPT;
    }
}
