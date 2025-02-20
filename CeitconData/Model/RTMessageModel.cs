using System;
using System.ComponentModel;

namespace Ceitcon_Data.Model
{
    public enum RTMessageType
    {
        Facebook = 0,
        Twitter = 1,
        Instagram = 2,
       
    }

    public class RTMessageModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Name;
        private string _Topic;
        private string _Message;
        private RTMessageType _MediaType;
        private string _SenderName;
        private string _SenderImage;
        private string _ApprovedBy;

        public RTMessageModel()
        {
            _Id = Guid.NewGuid().ToString();
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

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Topic
        {
            get { return _Topic; }
            set
            {
                if (_Topic != value)
                {
                    _Topic = value;
                    OnPropertyChanged("Topic");
                }
            }
        }

        public string Message
        {
            get { return _Message; }
            set
            {
                if (_Message != value)
                {
                    _Message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public RTMessageType MediaType
        {
            get { return _MediaType; }
            set
            {
                if (_MediaType != value)
                {
                    _MediaType = value;
                    OnPropertyChanged("MediaType");
                    OnPropertyChanged("MediaTypeImmage");
                    OnPropertyChanged("HasImagePath");
                }
            }
        }

        public string MediaTypeImage
        {
            get
            {
                switch (_MediaType) {
                    case RTMessageType.Facebook:
                        return "../Images/iconFacebookControl_Active.png";
                    case RTMessageType.Twitter:
                        return "../Images/iconTwitterControl_Active.png";
                    case RTMessageType.Instagram:
                        return "../Images/iconInstagramControl_Active.png";
                    default:
                        return "";
                }

            }
        }

        public bool HasImagePath
        {
            get
            {
                return (int)_MediaType == (int)RTMessageType.Instagram;
            }
        }

        public string SenderName
        {
            get { return _SenderName; }
            set
            {
                if (_SenderName != value)
                {
                    _SenderName = value;
                    OnPropertyChanged("SenderName");
                }
            }
        }

        public string SenderImage
        {
            get { return _SenderImage; }
            set
            {
                if (_SenderImage != value)
                {
                    _SenderImage = value;
                    OnPropertyChanged("SenderImage");
                }
            }
        }

        public string ApprovedBy
        {
            get { return _ApprovedBy; }
            set
            {
                if (_ApprovedBy != value)
                {
                    _ApprovedBy = value;
                    OnPropertyChanged("ApprovedBy");
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
