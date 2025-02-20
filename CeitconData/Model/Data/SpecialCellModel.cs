using Ceitcon_Data.Utilities;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Ceitcon_Data.Model.Data
{

    public class SpecialCellModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Text;
        private bool _IsRow;
        private bool _IsBlink;
        private Brush _Foreground;
        private Brush _Background;

        public SpecialCellModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Text = String.Empty;
            _IsRow = false;
            _IsBlink = false;
        }

        public SpecialCellModel(SpecialCellModel copy, bool fullCopy = false)
        {
            if (fullCopy) { _Id = copy.Id; } else { Guid.NewGuid().ToString(); }
            _Text = copy.Text;
            _IsRow = copy.IsRow;
            _IsBlink = copy.IsBlink;
            _Foreground = copy.Foreground;
            _Background = copy.Background;
        }

        public SpecialCellModel Save()
        {
            return new SpecialCellModel(this, true);
        }

        public void Restore(SpecialCellModel copyObj)
        {
            var copy = copyObj as SpecialCellModel;
            Memento.Enable = false;
            Text = copy.Text;
            IsRow = copy.IsRow;
            IsBlink = copy.IsBlink;
            Foreground = copy.Foreground;
            Background = copy.Background;
            
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
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public bool IsRow
        {
            get { return _IsRow; }
            set
            {
                if (_IsRow != value)
                {
                    _IsRow = value;
                    OnPropertyChanged("IsRow");
                }
            }
        }

        public bool IsBlink
        {
            get { return _IsBlink; }
            set
            {
                if (_IsBlink != value)
                {
                    _IsBlink = value;
                    OnPropertyChanged("IsBlink");
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
                    _Foreground = value;
                    OnPropertyChanged("Foreground");
                }
            }
        }

        //public Brush ForegroundBrush
        //{
        //    get { return new SolidColorBrush(_Foreground); }
        //}

        public Brush Background
        {
            get { return _Background; }
            set
            {
                if (_Background != value)
                {
                    _Background = value;
                    OnPropertyChanged("Background");
                    //OnPropertyChanged("BackgroundBrush");
                }
            }
        }

        //public Brush BackgroundBrush
        //{
        //    get { return new SolidColorBrush(_Background); }
        //}

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

