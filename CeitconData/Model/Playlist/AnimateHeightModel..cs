
using Ceitcon_Data.Utilities;

namespace Ceitcon_Data.Model.Playlist
{
    public class AnimateHeightModel : PlaylistModel
    {
        public AnimateHeightModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.AnimateHeight;
            _Name = "Animate Height";
        }

        public AnimateHeightModel(AnimateHeightModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.AnimateHeight;
            _Name = copy.Name;
            _HeightFrom = copy.HeightFrom;
            _HeightTo = copy.HeightTo;
        }

        public override PlaylistModel Save()
        {
            return new AnimateHeightModel(this, null, true);
        }

        public override void Restore(object copyObj)
        {
            var copy = copyObj as AnimateHeightModel;
            Memento.Enable = false;
            base.Restore(copy);
            HeightFrom = copy.HeightFrom;
            HeightTo = copy.HeightTo;
            Memento.Enable = true;
        }

        private double _HeightFrom;
        private double _HeightTo;

        public double HeightFrom
        {
            get { return _HeightFrom; }
            set
            {
                if (_HeightFrom != value)
                {
                    Memento.Push(Save());
                    _HeightFrom = value;
                    OnPropertyChanged("HeightFrom");
                }
            }
        }

        public double HeightTo
        {
            get { return _HeightTo; }
            set
            {
                if (_HeightTo != value)
                {
                    Memento.Push(Save());
                    _HeightTo = value;
                    OnPropertyChanged("HeightTo");
                }
            }
        }
    }
}
