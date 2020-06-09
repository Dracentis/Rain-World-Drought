using Music;
using MonoMod;
using RWCustom;
using UnityEngine;

[MonoModPatch("global::Music.MusicPiece")]
class patch_MusicPiece : MusicPiece
{
    [MonoModIgnore]
    public patch_MusicPiece(MusicPlayer musicPlayer, string name, MusicPlayer.MusicContext context) : base(musicPlayer, name, context)
    {
    }

    class patch_SubTrack : MusicPiece.SubTrack
    {
        [MonoModIgnore]
        public patch_SubTrack(MusicPiece piece, int index, string trackName) : base(piece, index, trackName)
        {
        }
        public extern void orig_Update();

        public void Update()
        {
            if (!this.readyToPlay)
            {
                if (this.source.clip == null)
                {
                    if (this.trackName.StartsWith("TH_IS"))
                    {
                        string rootpath = Application.dataPath;
                        string filepath = rootpath.Substring(0, rootpath.LastIndexOf("/")) + "/Assets/Futile/Resources/Music/Procedural/" + this.trackName + ".ogg";
                        WWW www = new WWW("file://" + filepath);
                        this.source.clip = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                    }
                    else if (this.trackName.StartsWith("TH_FS"))
                    {
                        string rootpath = Application.dataPath;
                        string filepath = rootpath.Substring(0, rootpath.LastIndexOf("/")) + "/Assets/Futile/Resources/Music/Procedural/" + this.trackName + ".ogg";
                        WWW www = new WWW("file://" + filepath);
                        this.source.clip = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                    }
                    else if (this.trackName.StartsWith("TH_MW"))
                    {
                        string rootpath = Application.dataPath;
                        string filepath = rootpath.Substring(0, rootpath.LastIndexOf("/")) + "/Assets/Futile/Resources/Music/Procedural/" + this.trackName + ".ogg";
                        WWW www = new WWW("file://" + filepath);
                        this.source.clip = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                    }
                    else if (this.piece.IsProcedural)
                    {
                        this.source.clip = (Resources.Load("Music/Procedural/" + this.trackName, typeof(AudioClip)) as AudioClip);
                    }
                    else
                    {
                        this.source.clip = (Resources.Load("Music/Songs/" + this.trackName, typeof(AudioClip)) as AudioClip);
                    }
                }
                else if (!this.source.isPlaying && this.source.clip.isReadyToPlay)
                {
                    this.readyToPlay = true;
                }
            }
            if (this.piece.startedPlaying)
            {
                this.source.volume = Mathf.Pow(this.volume * this.piece.volume * this.piece.musicPlayer.manager.rainWorld.options.musicVolume, this.piece.musicPlayer.manager.soundLoader.volumeExponent);
            }
        }
    }
}

