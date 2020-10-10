using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rain_World_Drought.Resource
{
    public static class ResourceManager
    {
        /* To do List to be moved
         * Load Drought Decals
         * Load Drought Illustrations
         * Load Drought Songs (Not procedural)
         * Load Drought Palette
         * Load Drought Scene
         *
         * CRS support later
         */

        public static string assetDir;
        public static string error;

        #region Atlases

        public static bool LoadAtlases()
        {
            string[] names = new string[] { "rainWorld", "uiSprites" };
            if (!Directory.Exists(assetDir)) { error = $"Directory [{assetDir}] is missing!"; return false; }

            foreach (string name in names)
            {
                string file = assetDir + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Atlases" + Path.DirectorySeparatorChar + name + ".txt";
                try
                {
                    if (!File.Exists(file)) { error = $"File [{file}] is missing!"; return false; }
                    string data = File.ReadAllText(file);
                    UnloadDuplicate(data);

                    file = assetDir + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Atlases" + Path.DirectorySeparatorChar + name + ".png";
                    if (!File.Exists(file)) { error = $"File [{file}] is missing!"; return false; }
                    WWW www = new WWW("file:///" + file);
                    Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { anisoLevel = 0, filterMode = FilterMode.Point };
                    www.LoadImageIntoTexture(texture);

                    FAtlas atlas = Futile.atlasManager.LoadAtlasFromTexture(file, texture);
                    LoadAtlasDataFromString(ref atlas, data);
                }
                catch (Exception e)
                {
                    error = $"Error occured while loading Drought Atlas [{file}]: Reinstall DroughtAssets.";
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
                Debug.Log(fatlasElement.name);

                fatlasElement.atlas = atlas;
                Futile.atlasManager.AddElement(fatlasElement);
            }
        }

        #endregion Atlases

        #region Music

        public static bool IsDroughtTrack(string trackName)
        {
            trackName = trackName.ToUpper();
            if (trackName.StartsWith("TH_IS")) { return true; }
            if (trackName.StartsWith("TH_FS")) { return true; }
            if (trackName.StartsWith("TH_MW")) { return true; }
            return false;
        }

        public static AudioClip LoadSubTrack(string trackName)
        {
            WWW www = new WWW("file://" + assetDir + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Music" + Path.DirectorySeparatorChar + "Procedural" + Path.DirectorySeparatorChar + trackName + ".ogg");
            return www.GetAudioClip(false, true, AudioType.OGGVORBIS);
        }

        #endregion Music
    }
}
