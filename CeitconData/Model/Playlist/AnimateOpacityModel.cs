using Ceitcon_Data.Utilities;

namespace Ceitcon_Data.Model.Playlist
{
    public class AnimateOpacityModel : PlaylistModel
    {
        public AnimateOpacityModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.AnimateOpacity;
            _Name = "Animate Opacity";
        }

        public AnimateOpacityModel(AnimateOpacityModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.AnimateOpacity;
            _Name = copy.Name;
            _OpacityFrom = copy.OpacityFrom;
            _OpacityTo = copy.OpacityTo;
        }

        public override PlaylistModel Save()
        {
            return new AnimateOpacityModel(this, null, true);
        }

        public override void Restore(object copyObj)
        {
            var copy = copyObj as AnimateOpacityModel;
            Memento.Enable = false;
            base.Restore(copy);
            OpacityFrom = copy.OpacityFrom;
            OpacityTo = copy.OpacityTo;
            Memento.Enable = true;
        }

        private double _OpacityFrom;
        private double _OpacityTo;

        public double OpacityFrom
        {
            get { return _OpacityFrom; }
            set
            {
                if (_OpacityFrom != value)
                {
                    Memento.Push(Save());
                    _OpacityFrom = value;
                    OnPropertyChanged("OpacityFrom");
                }
            }
        }

        public double OpacityTo
        {
            get { return _OpacityTo; }
            set
            {
                if (_OpacityTo != value)
                {
                    Memento.Push(Save());
                    _OpacityTo = value;
                    OnPropertyChanged("OpacityTo");
                }
            }
        }
    }
}
