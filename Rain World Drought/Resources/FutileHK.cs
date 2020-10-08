using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rain_World_Drought.Resources
{
    internal static class FutileHK
    {
        public static void Patch()
        {
            // These need to be replaced, so only Drought custom resources use custom method.
            On.FAtlas.LoadTexture += new On.FAtlas.hook_LoadTexture(AtlasLoadTextureHK);
            On.FAtlas.LoadAtlasData += new On.FAtlas.hook_LoadAtlasData(AtlasLoadAtlasDataHK);
            On.FAtlasManager.LoadAtlas += new On.FAtlasManager.hook_LoadAtlas(ManagerLoadAtlasHK);
            On.FAtlasManager.LoadImage += new On.FAtlasManager.hook_LoadImage(ManagerLoadImageHK);
        }

        public static string RootFolderDirectory()
        {
            string directory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var pathParts = directory.Split(Path.DirectorySeparatorChar);
            string newPath = "";
            for (int i = 0; i < pathParts.Length - 3; i++)
                newPath = newPath + pathParts[i] + Path.DirectorySeparatorChar;
            return newPath;
        }

#pragma warning disable IDE0060

        private static void AtlasLoadTextureHK(On.FAtlas.orig_LoadTexture orig, FAtlas self)
        {
            WWW www = new WWW("file:///" + RootFolderDirectory() + "Assets" + Path.DirectorySeparatorChar + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + self._imagePath + ".png");
            self._texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            self._texture.anisoLevel = 0;
            self._texture.filterMode = FilterMode.Point;
            www.LoadImageIntoTexture(self._texture as Texture2D);
            if (self._texture == null) { throw new FutileException("Couldn't load the atlas texture from: " + self._imagePath); }
            self._isTextureAnAsset = true;
            self._textureSize = new Vector2((float)self._texture.width, (float)self._texture.height);
        }

        private static void AtlasLoadAtlasDataHK(On.FAtlas.orig_LoadAtlasData orig, FAtlas self)
        {
            string textAsset = File.ReadAllText(RootFolderDirectory() + "Assets" + Path.DirectorySeparatorChar + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + self._dataPath + ".txt");
            Dictionary<string, object> dictionary = textAsset.dictionaryFromJson();

            if (dictionary == null)
            {
                throw new FutileException("The atlas at " + self._dataPath + " was not a proper JSON file. Make sure to select \"Unity3D\" in TexturePacker.");
            }
            Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["frames"];
            float resourceScaleInverse = Futile.resourceScaleInverse;
            int num = 0;
            foreach (KeyValuePair<string, object> keyValuePair in dictionary2)
            {
                FAtlasElement fatlasElement = new FAtlasElement();
                fatlasElement.indexInAtlas = num++;
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
                    throw new NotSupportedException("Futile no longer supports TexturePacker's \"rotated\" flag. Please disable it when creating the " + self._dataPath + " atlas.");
                }
                IDictionary dictionary4 = (IDictionary)dictionary3["frame"];
                float num3 = float.Parse(dictionary4["x"].ToString());
                float num4 = float.Parse(dictionary4["y"].ToString());
                float num5 = float.Parse(dictionary4["w"].ToString());
                float num6 = float.Parse(dictionary4["h"].ToString());
                Rect uvRect = new Rect(num3 / self._textureSize.x, (self._textureSize.y - num4 - num6) / self._textureSize.y, num5 / self._textureSize.x, num6 / self._textureSize.y);
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
                self._elements.Add(fatlasElement);
                self._elementsByName.Add(fatlasElement.name, fatlasElement);
            }
        }

        private static FAtlas ManagerLoadAtlasHK(On.FAtlasManager.orig_LoadAtlas orig, FAtlasManager self, string atlasPath)
        {
            if (self.DoesContainAtlas(atlasPath))
            { return self.GetAtlasWithName(atlasPath); }
            return self.ActuallyLoadAtlasOrImage(atlasPath, atlasPath + Futile.resourceSuffix, atlasPath + Futile.resourceSuffix);
        }

        private static FAtlas ManagerLoadImageHK(On.FAtlasManager.orig_LoadImage orig, FAtlasManager self, string imagePath)
        {
            if (self.DoesContainAtlas(imagePath))
            { return self.GetAtlasWithName(imagePath); }
            return self.ActuallyLoadAtlasOrImage(imagePath, imagePath + Futile.resourceSuffix, "");
        }
    }
}
