using Music;
using UnityEngine;

namespace Rain_World_Drought.Resource
{
    /// <summary>
    /// Should use separate Folder instead of merging
    /// </summary>
    internal static class MusicPieceHK
    {
        public static void Patch()
        {
            On.Music.MusicPiece.SubTrack.Update += new On.Music.MusicPiece.SubTrack.hook_Update(SubTrackHK);

            rootpath = Application.dataPath;
        }

        public static string rootpath;

        private static void SubTrackHK(On.Music.MusicPiece.SubTrack.orig_Update orig, MusicPiece.SubTrack self)
        {
            if (!self.readyToPlay)
            {
                if (self.source.clip == null)
                {
                    if (self.trackName.StartsWith("TH_IS"))
                    {
                        string filepath = rootpath.Substring(0, rootpath.LastIndexOf("/")) + "/Assets/Futile/Resources/Music/Procedural/" + self.trackName + ".ogg";
                        WWW www = new WWW("file://" + filepath);
                        self.source.clip = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                    }
                    else if (self.trackName.StartsWith("TH_FS"))
                    {
                        string filepath = rootpath.Substring(0, rootpath.LastIndexOf("/")) + "/Assets/Futile/Resources/Music/Procedural/" + self.trackName + ".ogg";
                        WWW www = new WWW("file://" + filepath);
                        self.source.clip = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                    }
                    else if (self.trackName.StartsWith("TH_MW"))
                    {
                        string filepath = rootpath.Substring(0, rootpath.LastIndexOf("/")) + "/Assets/Futile/Resources/Music/Procedural/" + self.trackName + ".ogg";
                        WWW www = new WWW("file://" + filepath);
                        self.source.clip = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                    }
                    else if (self.piece.IsProcedural)
                    {
                        self.source.clip = (UnityEngine.Resources.Load("Music/Procedural/" + self.trackName, typeof(AudioClip)) as AudioClip);
                    }
                    else
                    {
                        self.source.clip = (UnityEngine.Resources.Load("Music/Songs/" + self.trackName, typeof(AudioClip)) as AudioClip);
                    }
                }
                else if (!self.source.isPlaying && self.source.clip.isReadyToPlay)
                {
                    self.readyToPlay = true;
                }
            }
            if (self.piece.startedPlaying)
            {
                self.source.volume = Mathf.Pow(self.volume * self.piece.volume * self.piece.musicPlayer.manager.rainWorld.options.musicVolume, self.piece.musicPlayer.manager.soundLoader.volumeExponent);
            }
        }
    }
}
