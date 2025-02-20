using Ceitcon_Data.Utilities;

namespace Ceitcon_Data.Model.Playlist
{
    public class AnimateWidthModel : PlaylistModel
    {
        public AnimateWidthModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.AnimateWidth;
            _Name = "Animate Width";
        }

        public AnimateWidthModel(AnimateWidthModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.AnimateWidth;
            _Name = copy.Name;
            _WidthFrom = copy.WidthFrom;
            _WidthTo = copy.WidthTo;
        }

        public override PlaylistModel Save()
        {
            return new AnimateWidthModel(this, null, true);
        }

        public override void Restore(object copyObj)
        {
            var copy = copyObj as AnimateWidthModel;
            Memento.Enable = false;
            base.Restore(copy);
            WidthFrom = copy.WidthFrom;
            WidthTo = copy.WidthTo;
            Memento.Enable = true;
        }

        private double _WidthFrom;
        private double _WidthTo;

        public double WidthFrom
        {
            get { return _WidthFrom; }
            set
            {
                if (_WidthFrom != value)
                {
                    Memento.Push(Save());
                    _WidthFrom = value;
                    OnPropertyChanged("WidthFrom");
                }
            }
        }

        public double WidthTo
        {
            get { return _WidthTo; }
            set
            {
                if (_WidthTo != value)
                {
                    Memento.Push(Save());
                    _WidthTo = value;
                    OnPropertyChanged("WidthTo");
                }
            }
        }
    }
}
