using Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Rain_World_Drought.Resource
{
    public static class ResourceManager
    {
        public static void Patch()
        {
            On.Menu.MenuIllustration.LoadFile_1 += new On.Menu.MenuIllustration.hook_LoadFile_1(MenuIllustrationLoadFileHK);
            On.CustomDecal.LoadFile += new On.CustomDecal.hook_LoadFile(CustomDecalLoadFileHK);
            On.RoomCamera.LoadPalette += new On.RoomCamera.hook_LoadPalette(RoomCameraLoadPaletteHK);
        }

        public static string assetDir;
        public static string error;

        #region Atlases

        public static bool LoadAtlases()
        {
            string[] names = new string[] { "rainWorld", "uiSprites" };
            if (!Directory.Exists(assetDir)) { error = DroughtMod.Translate("Directory [<assetDir>] is missing: Reinstall DroughtAssets.").Replace("<assetDir>", assetDir); return false; }

            foreach (string name in names)
            {
                string file = assetDir + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Atlases" + Path.DirectorySeparatorChar + name + ".txt";
                try
                {
                    if (!File.Exists(file)) { error = DroughtMod.Translate("File [<file>] is missing: Reinstall DroughtAssets.").Replace("<file>", file); return false; }
                    string data = File.ReadAllText(file);
                    UnloadDuplicate(data);

                    file = assetDir + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Atlases" + Path.DirectorySeparatorChar + name + ".png";
                    if (!File.Exists(file)) { error = DroughtMod.Translate("File [<file>] is missing: Reinstall DroughtAssets.").Replace("<file>", file); return false; }
                    WWW www = new WWW("file:///" + file);
                    Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { anisoLevel = 0, filterMode = FilterMode.Point };
                    www.LoadImageIntoTexture(texture);

                    FAtlas atlas = Futile.atlasManager.LoadAtlasFromTexture(file, texture);
                    LoadAtlasDataFromString(ref atlas, data);
                }
                catch (Exception e)
                {
                    error = DroughtMod.Translate("Error occured while loading Drought Atlas [<file>]: Reinstall DroughtAssets.").Replace("<file>", file);
                    Debug.LogError(error);
                    Debug.LogException(e);
                    return false;
                }
            }

            return true;
        }

        public static void UnloadDuplicate(string data)
        {
            Dictionary<string, object> dictionary = data.dictionaryFromJson();

            if (dictionary == null)
            {
                throw new FutileException($"The data was not a proper JSON file. Make sure to select \"Unity3D\" in TexturePacker.");
            }
            Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["frames"];
            foreach (KeyValuePair<string, object> keyValuePair in dictionary2)
            {
                string name = keyValuePair.Key;
                if (Futile.shouldRemoveAtlasElementFileExtensions)
                {
                    int num2 = name.LastIndexOf(".");
                    if (num2 >= 0)
                    {
                        name = name.Substring(0, num2);
                    }
                }
                if (Futile.atlasManager.DoesContainAtlas(name)) { Futile.atlasManager.UnloadAtlas(name); }
                if (Futile.atlasManager.DoesContainElementWithName(name)) { Futile.atlasManager._allElementsByName.Remove(name); }
            }
        }

        public static void LoadAtlasDataFromString(ref FAtlas atlas, string data)
        {
            Dictionary<string, object> dictionary = data.dictionaryFromJson();
            if (dictionary == null)
            {
                throw new FutileException("This data is not a proper JSON file. Make sure to select \"Unity3D\" in TexturePacker.");
            }
            Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["frames"];
            float resourceScaleInverse = Futile.resourceScaleInverse;
            int num = 0;
            foreach (KeyValuePair<string, object> keyValuePair in dictionary2)
            {
                FAtlasElement fatlasElement = new FAtlasElement
                {
                    indexInAtlas = num++
                };
                string text = keyValuePair.Key;
                if (Futile.shouldRemoveAtlasElementFileExtensions)
                {
                    int num2 = text.LastIndexOf(".");
                    if (num2 >= 0)
                    {
                        text = text.Substring(0, num2);
                    }
                }
                fatlasElement.name = text;
                IDictionary dictionary3 = (IDictionary)keyValuePair.Value;
                fatlasElement.isTrimmed = (bool)dictionary3["trimmed"];
                if ((bool)dictionary3["rotated"])
                {
                    throw new NotSupportedException("Futile no longer supports TexturePacker's \"rotated\" flag. Please disable it when creating the atlas.");
                }
                IDictionary dictionary4 = (IDictionary)dictionary3["frame"];
                float num3 = float.Parse(dictionary4["x"].ToString());
                float num4 = float.Parse(dictionary4["y"].ToString());
                float num5 = float.Parse(dictionary4["w"].ToString());
                float num6 = float.Parse(dictionary4["h"].ToString());
                Rect uvRect = new Rect(num3 / atlas._textureSize.x, (atlas._textureSize.y - num4 - num6) / atlas._textureSize.y, num5 / atlas._textureSize.x, num6 / atlas._textureSize.y);
                fatlasElement.uvRect = uvRect;
                fatlasElement.uvTopLeft.Set(uvRect.xMin, uvRect.yMax);
                fatlasElement.uvTopRight.Set(uvRect.xMax, uvRect.yMax);
                fatlasElement.uvBottomRight.Set(uvRect.xMax, uvRect.yMin);
                fatlasElement.uvBottomLeft.Set(uvRect.xMin, uvRect.yMin);
                IDictionary dictionary5 = (IDictionary)dictionary3["sourceSize"];
                fatlasElement.sourcePixelSize.x = float.Parse(dictionary5["w"].ToString());
                fatlasElement.sourcePixelSize.y = float.Parse(dictionary5["h"].ToString());
                fatlasElement.sourceSize.x = fatlasElement.sourcePixelSize.x * resourceScaleInverse;
                fatlasElement.sourceSize.y = fatlasElement.sourcePixelSize.y * resourceScaleInverse;
                IDictionary dictionary6 = (IDictionary)dictionary3["spriteSourceSize"];
                float left = float.Parse(dictionary6["x"].ToString()) * resourceScaleInverse;
                float top = float.Parse(dictionary6["y"].ToString()) * resourceScaleInverse;
                float width = float.Parse(dictionary6["w"].ToString()) * resourceScaleInverse;
                float height = float.Parse(dictionary6["h"].ToString()) * resourceScaleInverse;
                fatlasElement.sourceRect = new Rect(left, top, width, height);
                atlas._elements.Add(fatlasElement);
                atlas._elementsByName.Add(fatlasElement.name, fatlasElement);
                // Debug.Log(fatlasElement.name);

                fatlasElement.atlas = atlas;
                Futile.atlasManager.AddElement(fatlasElement);
            }
        }

        #endregion Atlases

        #region Music

        public static bool CheckDroughtSongs()
        {
            Debug.Log("Drought) CheckDroughtSongs");
            List<string> songs = new List<string>();
            // Procedural: ogg
            DirectoryInfo dir = new DirectoryInfo(string.Concat(
                assetDir,
                "Futile",
                Path.DirectorySeparatorChar,
                "Resources",
                Path.DirectorySeparatorChar,
                "Music",
                Path.DirectorySeparatorChar,
                "Procedural",
                Path.DirectorySeparatorChar));
            if (!dir.Exists) { error = DroughtMod.Translate("Directory [<assetDir>] is missing: Reinstall DroughtAssets.").Replace("<assetDir>", dir.FullName); return false; }
            foreach (FileInfo f in dir.GetFiles())
            {
                if (f.Name.ToLower().EndsWith(".ogg"))
                {
                    string name = f.Name.Length > 5 ? f.Name.ToUpper().Substring(0, 5) : f.Name.ToUpper();
                    if (!songs.Contains(name)) { songs.Add(name); }
                }
            }
            // Song: mp3
            dir = new DirectoryInfo(string.Concat(
                assetDir,
                "Futile",
                Path.DirectorySeparatorChar,
                "Resources",
                Path.DirectorySeparatorChar,
                "Music",
                Path.DirectorySeparatorChar,
                "Songs",
                Path.DirectorySeparatorChar));
            if (!dir.Exists) { error = DroughtMod.Translate("Directory [<assetDir>] is missing: Reinstall DroughtAssets.").Replace("<assetDir>", dir.FullName); return false; }
            foreach (FileInfo f in dir.GetFiles())
            {
                if (f.Name.ToLower().EndsWith(".mp3"))
                {
                    string name = f.Name.Length > 5 ? f.Name.ToUpper().Substring(0, 5) : f.Name.ToUpper();
                    if (!songs.Contains(name)) { songs.Add(name); }
                }
            }
            droughtSongs = songs.ToArray();

            string dbg = string.Empty;
            for (int i = 0; i < droughtSongs.Length; i++)
            { dbg += droughtSongs[i]; if (i < droughtSongs.Length - 1) { dbg += ", "; } }
            Debug.Log(dbg);
            return true;
        }

        private static string[] droughtSongs = new string[0]; // assuming that we won't replace title song, which comes before initializing this

        public static bool IsDroughtTrack(string trackName)
        {
            trackName = trackName.ToUpper();
            foreach (string test in droughtSongs)
            { if (trackName.StartsWith(test)) { return true; } }
            return false;
        }

        public static AudioClip LoadSubTrack(string trackName, bool procedural)
        {
            WWW www = new WWW(string.Concat(
                "file://",
                assetDir,
                "Futile",
                Path.DirectorySeparatorChar,
                "Resources",
                Path.DirectorySeparatorChar,
                "Music",
                Path.DirectorySeparatorChar,
                procedural ? "Procedural" : "Songs",
                Path.DirectorySeparatorChar,
                trackName,
                procedural ? ".ogg" : ".mp3"));
            return www.GetAudioClip(false, true, AudioType.OGGVORBIS);
        }

        #endregion Music

        #region Replacer

        /// <summary>
        /// Try loading Drought Resources first, then try original files
        /// </summary>
        private static void MenuIllustrationLoadFileHK(On.Menu.MenuIllustration.orig_LoadFile_1 orig, MenuIllustration self, string folder)
        {
            string path = string.Concat(
                assetDir,
                "Futile",
                Path.DirectorySeparatorChar,
                "Resources",
                Path.DirectorySeparatorChar,
                folder,
                Path.DirectorySeparatorChar,
                self.fileName,
                ".png"
                );
            //Debug.Log($"Drought Checks {path}: {File.Exists(path)}");
            if (!File.Exists(path)) { orig.Invoke(self, folder); return; }

            self.www = new WWW("file:///" + path);
            self.texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            self.texture.wrapMode = TextureWrapMode.Clamp;
            if (self.crispPixels)
            {
                self.texture.anisoLevel = 0;
                self.texture.filterMode = FilterMode.Point;
            }
            self.www.LoadImageIntoTexture(self.texture);
            HeavyTexturesCache.LoadAndCacheAtlasFromTexture(self.fileName, self.texture);
            self.www = null;
        }

        private static void CustomDecalLoadFileHK(On.CustomDecal.orig_LoadFile orig, CustomDecal self, string fileName)
        {
            if (Futile.atlasManager.GetAtlasWithName(fileName) != null) { return; }
            string path = string.Concat(
                assetDir,
                "Futile",
                Path.DirectorySeparatorChar,
                "Resources",
                Path.DirectorySeparatorChar,
                "Decals",
                Path.DirectorySeparatorChar,
                fileName,
                ".png"
                );
            if (!File.Exists(path)) { orig.Invoke(self, fileName); return; }

            WWW www = new WWW("file:///" + path);
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture2D.wrapMode = TextureWrapMode.Clamp;
            texture2D.anisoLevel = 0;
            texture2D.filterMode = FilterMode.Point;
            www.LoadImageIntoTexture(texture2D);
            HeavyTexturesCache.LoadAndCacheAtlasFromTexture(fileName, texture2D);
        }

        public static int[] droughtPalettes = new int[] { 36, 37 };

        private static void RoomCameraLoadPaletteHK(On.RoomCamera.orig_LoadPalette orig, RoomCamera self, int pal, ref Texture2D texture)
        {
            if (!droughtPalettes.Contains(pal)) { orig.Invoke(self, pal, ref texture); return; }

            texture = new Texture2D(32, 16, TextureFormat.ARGB32, false);
            texture.anisoLevel = 0;
            texture.filterMode = FilterMode.Point;
            self.www = new WWW(string.Concat(
                "file:///",
                assetDir,
                Path.DirectorySeparatorChar,
                "Futile",
                Path.DirectorySeparatorChar,
                "Resources",
                Path.DirectorySeparatorChar,
                "Palettes",
                Path.DirectorySeparatorChar,
                "palette",
                pal,
                ".png"
                ));
            self.www.LoadImageIntoTexture(texture);
            if (self.room != null)
            { self.ApplyEffectColorsToPaletteTexture(ref texture, self.room.roomSettings.EffectColorA, self.room.roomSettings.EffectColorB); }
            else
            { self.ApplyEffectColorsToPaletteTexture(ref texture, -1, -1); }
            texture.Apply(false);
        }

        #endregion Replacer
    }
}
