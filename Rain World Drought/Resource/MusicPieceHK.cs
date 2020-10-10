﻿using Music;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rain_World_Drought.Resource
{
    internal static class MusicPieceHK
    {
        public static void Patch()
        {
            On.Music.MusicPiece.SubTrack.Update += new On.Music.MusicPiece.SubTrack.hook_Update(SubTrackHK);
            On.Music.ProceduralMusic.ProceduralMusicInstruction.ctor += new On.Music.ProceduralMusic.ProceduralMusicInstruction.hook_ctor(ProceduralMusicInstructionHK);
        }

        private static void SubTrackHK(On.Music.MusicPiece.SubTrack.orig_Update orig, MusicPiece.SubTrack self)
        {
            if (!self.readyToPlay)
            {
                if (self.source.clip == null)
                {
                    if (ResourceManager.IsDroughtTrack(self.trackName))
                    {
                        self.source.clip = ResourceManager.LoadSubTrack(self.trackName, self.piece.IsProcedural);
                    }
                }
            }
            orig.Invoke(self);
        }

        // public static string[] newThreatRegion = new string[] { "FS", "IS" }; // Regions with new threat musics (txt file names in Music/Procedural)

        private static void ProceduralMusicInstructionHK(On.Music.ProceduralMusic.ProceduralMusicInstruction.orig_ctor orig,
            ProceduralMusic.ProceduralMusicInstruction self, string name)
        {
            orig.Invoke(self, name);
            //if (!newThreatRegion.Contains(name)) { return; }

            string folder = string.Concat(
                    ResourceManager.assetDir,
                    Path.DirectorySeparatorChar,
                    "Futile",
                    Path.DirectorySeparatorChar,
                    "Resources",
                    Path.DirectorySeparatorChar,
                    "Music",
                    Path.DirectorySeparatorChar,
                    "Procedural",
                    Path.DirectorySeparatorChar
                );
            if (!File.Exists(folder + name + ".txt")) { return; }
            string[] array = File.ReadAllLines(folder + name + ".txt");
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = Regex.Split(array[i], " : ");
                if (array2.Length > 0 && array2[0].Length > 4 && array2[0] == "Layer")
                {
                    self.layers.Add(new ProceduralMusic.ProceduralMusicInstruction.Layer(self.layers.Count));
                    string[] array3 = Regex.Split(array2[1], ", ");
                    for (int j = 0; j < array3.Length; j++)
                    {
                        if (array3[j].Length > 0)
                        {
                            for (int k = 0; k < self.tracks.Count; k++)
                            {
                                if (array3[j] == self.tracks[k].name)
                                {
                                    self.layers[self.layers.Count - 1].tracks.Add(self.tracks[k]);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (array2.Length > 0 && array2[0].Length > 0 && File.Exists(folder + array2[0] + ".ogg"))
                {
                    self.tracks.Add(new ProceduralMusic.ProceduralMusicInstruction.Track(array2[0]));
                    string[] array4 = Regex.Split(array2[1], ", ");
                    for (int l = 0; l < array4.Length; l++)
                    {
                        if (array4[l].Length > 0)
                        {
                            if (array4[l] == "<PA>")
                            {
                                self.tracks[self.tracks.Count - 1].remainInPanicMode = true;
                            }
                            else
                            {
                                self.tracks[self.tracks.Count - 1].tags.Add(array4[l]);
                            }
                        }
                    }
                }
            }
        }
    }
}