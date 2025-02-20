using Ceitcon_Data.Utilities;

namespace Ceitcon_Data.Model.Playlist
{
    public class SuspendPlaybackModel : PlaylistModel
    {
        public SuspendPlaybackModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.SuspendPlayback;
            _Name = "Suspend Playback";
        }

        public SuspendPlaybackModel(SuspendPlaybackModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.SuspendPlayback;
            _Name = copy.Name;
        }

        public override PlaylistModel Save()
        {
            return new SuspendPlaybackModel(this, null, true);
        }

        public override void Restore(object copyObj)
        {
            var copy = copyObj as SuspendPlaybackModel;
            Memento.Enable = false;
            base.Restore(copy);
            Memento.Enable = true;
        }
    }
}
