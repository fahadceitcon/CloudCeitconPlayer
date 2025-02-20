using Ceitcon_Data.Utilities;

namespace Ceitcon_Data.Model.Playlist
{
    public class ResumePlaybackModel : PlaylistModel
    {
        public ResumePlaybackModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.ResumePlayback;
            _Name = "Resume Playback";
        }

        public ResumePlaybackModel(ResumePlaybackModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.ResumePlayback;
            _Name = copy.Name;
        }

        public override PlaylistModel Save()
        {
            return new ResumePlaybackModel(this, null, true);
        }

        public override void Restore(object copyObj)
        {
            var copy = copyObj as ResumePlaybackModel;
            Memento.Enable = false;
            base.Restore(copy);
            Memento.Enable = true;
        }
    }
}
