using Music;

namespace Rain_World_Drought.Resource
{
    internal static class MusicPieceHK
    {
        public static void Patch()
        {
            On.Music.MusicPiece.SubTrack.Update += new On.Music.MusicPiece.SubTrack.hook_Update(SubTrackHK);
        }

        private static void SubTrackHK(On.Music.MusicPiece.SubTrack.orig_Update orig, MusicPiece.SubTrack self)
        {
            if (!self.readyToPlay)
            {
                if (self.source.clip == null)
                {
                    if (ResourceManager.IsDroughtTrack(self.trackName))
                    {
                        self.source.clip = ResourceManager.LoadSubTrack(self.trackName);
                    }
                }
            }
            orig.Invoke(self);
        }
    }
}
