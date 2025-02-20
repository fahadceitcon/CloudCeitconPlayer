using Ceitcon_Data.Utilities;

namespace Ceitcon_Data.Model.Playlist
{
    public class DelayModel : PlaylistModel
    {
        public DelayModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.Delay;
            _Name = "Delay";
        }

        public DelayModel(DelayModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.Delay;
            _Name = copy.Name;
        }

        public override PlaylistModel Save()
        {
            return new DelayModel(this, null, true);
        }

        public override void Restore(object copyObj)
        {
            var copy = copyObj as DelayModel;
            Memento.Enable = false;
            base.Restore(copy);
            Memento.Enable = true;
        }
    }
}
