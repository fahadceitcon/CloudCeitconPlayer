using System;
using System.ComponentModel;

namespace Ceitcon_Data.Model
{
    public class SocialMediaModel : INotifyPropertyChanged
    {
        public SocialMediaModel(string name, Uri image)
        {
            _Name = name;
            _Image = image;
        }

        private string _Name;
        private Uri _Image;

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

        public Uri Image
        {
            get { return _Image; }
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    OnPropertyChanged("Image");
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
