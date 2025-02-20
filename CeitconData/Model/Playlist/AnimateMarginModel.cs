using Ceitcon_Data.Utilities;
using System.Windows;

namespace Ceitcon_Data.Model.Playlist
{
    public class AnimateMarginModel : PlaylistModel
    {
        public AnimateMarginModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.AnimateMargin;
            _Name = "Animate Margin";
        }

        public AnimateMarginModel(AnimateMarginModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.AnimateMargin;
            _Name = copy.Name;
            _MarginThicknessFrom = copy.MarginThicknessFrom;
            _MarginThicknessTo = copy.MarginThicknessTo;
        }

        public override PlaylistModel Save()
        {
            return new AnimateMarginModel(this, null, true);
        }

        public override void Restore(object copyObj)
        {
            var copy = copyObj as AnimateMarginModel;
            Memento.Enable = false;
            base.Restore(copy);
            MarginThicknessFrom = copy.MarginThicknessFrom;
            MarginThicknessTo = copy.MarginThicknessTo;
            Memento.Enable = true;
        }

        private Thickness _MarginThicknessFrom;
        private Thickness _MarginThicknessTo;

        public Thickness MarginThicknessFrom
        {
            get { return _MarginThicknessFrom; }
            set
            {
                if (_MarginThicknessFrom != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessFrom = value;
                    OnPropertyChanged("MarginThicknessFrom");
                    OnPropertyChanged("MarginThicknessLeftFrom");
                    OnPropertyChanged("MarginThicknessRightFrom");
                    OnPropertyChanged("MarginThicknessTopFrom");
                    OnPropertyChanged("MarginThicknessBottomFrom");
                }
            }
        }

        public double MarginThicknessLeftFrom
        {
            get { return _MarginThicknessFrom.Left; }
            set
            {
                if (_MarginThicknessFrom.Left != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessFrom.Left = value;
                    OnPropertyChanged("MarginThicknessLeftFrom");
                    OnPropertyChanged("MarginThicknessFrom");
                }
            }
        }

        public double MarginThicknessRightFrom
        {
            get { return _MarginThicknessFrom.Right; }
            set
            {
                if (_MarginThicknessFrom.Right != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessFrom.Right = value;
                    OnPropertyChanged("MarginThicknessRightFrom");
                    OnPropertyChanged("MarginThicknessFrom");
                }
            }
        }

        public double MarginThicknessTopFrom
        {
            get { return _MarginThicknessFrom.Top; }
            set
            {
                if (_MarginThicknessFrom.Top != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessFrom.Top = value;
                    OnPropertyChanged("MarginThicknessTopFrom");
                    OnPropertyChanged("MarginThicknessFrom");
                }
            }
        }

        public double MarginThicknessBottomFrom
        {
            get { return _MarginThicknessFrom.Bottom; }
            set
            {
                if (_MarginThicknessFrom.Bottom != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessFrom.Bottom = value;
                    OnPropertyChanged("MarginThicknessBottomFrom");
                    OnPropertyChanged("MarginThicknessFrom");
                }
            }
        }

        public Thickness MarginThicknessTo
        {
            get { return _MarginThicknessTo; }
            set
            {
                if (_MarginThicknessTo != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessTo = value;
                    OnPropertyChanged("MarginThicknessTo");
                    OnPropertyChanged("MarginThicknessLeftTo");
                    OnPropertyChanged("MarginThicknessRightTo");
                    OnPropertyChanged("MarginThicknessTopTo");
                    OnPropertyChanged("MarginThicknessBottomTo");
                }
            }
        }

        public double MarginThicknessLeftTo
        {
            get { return _MarginThicknessTo.Left; }
            set
            {
                if (_MarginThicknessTo.Left != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessTo.Left = value;
                    OnPropertyChanged("MarginThicknessLeftTo");
                    OnPropertyChanged("MarginThicknessTo");
                }
            }
        }

        public double MarginThicknessRightTo
        {
            get { return _MarginThicknessTo.Right; }
            set
            {
                if (_MarginThicknessTo.Right != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessTo.Right = value;
                    OnPropertyChanged("MarginThicknessRightTo");
                    OnPropertyChanged("MarginThicknessTo");
                }
            }
        }

        public double MarginThicknessTopTo
        {
            get { return _MarginThicknessTo.Top; }
            set
            {
                if (_MarginThicknessTo.Top != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessTo.Top = value;
                    OnPropertyChanged("MarginThicknessTopTo");
                    OnPropertyChanged("MarginThicknessTo");
                }
            }
        }

        public double MarginThicknessBottomTo
        {
            get { return _MarginThicknessTo.Bottom; }
            set
            {
                if (_MarginThicknessTo.Bottom != value)
                {
                    Memento.Push(Save());
                    _MarginThicknessTo.Bottom = value;
                    OnPropertyChanged("MarginThicknessBottomTo");
                    OnPropertyChanged("MarginThicknessTo");
                }
            }
        }
    }
}

//public AnimateMarginModel(ControlModel parent) : base(parent)
//        {
//    _Type = PlaylistType.AnimateMargin;
//    _Name = "Animate Margin";
//}

//public AnimateMarginModel(AnimateMarginModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
//        {
//    _Type = PlaylistType.AnimateMargin;
//    _Name = copy.Name;
//    //_XFrom = copy.XFrom;
//    //_YFrom = copy.YFrom;
//    //_WFrom = copy.WFrom;
//    //_ZFrom = copy.ZFrom;
//    //_XTo = copy.XTo;
//    //_YTo = copy.YTo;
//    //_WTo = copy.WTo;
//    //_ZTo = copy.ZTo;
//    _MarginThicknessFrom = copy.MarginThicknessFrom;
//    _MarginThicknessTo = copy.MarginThicknessTo;
//}

//public override void Restore(object copyObj)
//{
//    var copy = copyObj as AnimateMarginModel;
//    Memento.Enable = false;
//    base.Restore(copy);
//    XFrom = copy.XFrom;
//    YFrom = copy.YFrom;
//    WFrom = copy.WFrom;
//    ZFrom = copy.ZFrom;
//    XTo = copy.XTo;
//    YTo = copy.YTo;
//    WTo = copy.WTo;
//    ZTo = copy.ZTo;
//    Memento.Enable = true;
//}

//private double _XFrom;
//private double _YFrom;
//private double _WFrom;
//private double _ZFrom;
//private double _XTo;
//private double _YTo;
//private double _WTo;
//private double _ZTo;

//public double XFrom
//{
//    get { return _XFrom; }
//    set
//    {
//        if (_XFrom != value)
//        {
//            Memento.Push(Save());
//            _XFrom = value;
//            OnPropertyChanged("XFrom");
//        }
//    }
//}
//public double YFrom
//{
//    get { return _YFrom; }
//    set
//    {
//        if (_YFrom != value)
//        {
//            Memento.Push(Save());
//            _YFrom = value;
//            OnPropertyChanged("YFrom");
//        }
//    }
//}

//public double WFrom
//{
//    get { return _WFrom; }
//    set
//    {
//        if (_WFrom != value)
//        {
//            Memento.Push(Save());
//            _WFrom = value;
//            OnPropertyChanged("WFrom");
//        }
//    }
//}
//public double ZFrom
//{
//    get { return _ZFrom; }
//    set
//    {
//        if (_ZFrom != value)
//        {
//            Memento.Push(Save());
//            _ZFrom = value;
//            OnPropertyChanged("ZFrom");
//        }
//    }
//}

//public double XTo
//{
//    get { return _XTo; }
//    set
//    {
//        if (_XTo != value)
//        {
//            Memento.Push(Save());
//            _XTo = value;
//            OnPropertyChanged("XTo");
//        }
//    }
//}
//public double YTo
//{
//    get { return _YTo; }
//    set
//    {
//        if (_YTo != value)
//        {
//            Memento.Push(Save());
//            _YTo = value;
//            OnPropertyChanged("YTo");
//        }
//    }
//}

//public double WTo
//{
//    get { return _WTo; }
//    set
//    {
//        if (_WTo != value)
//        {
//            Memento.Push(Save());
//            _WTo = value;
//            OnPropertyChanged("WTo");
//        }
//    }
//}
//public double ZTo
//{
//    get { return _ZTo; }
//    set
//    {
//        if (_ZTo != value)
//        {
//            Memento.Push(Save());
//            _ZTo = value;
//            OnPropertyChanged("ZTo");
//        }
//    }
//}

