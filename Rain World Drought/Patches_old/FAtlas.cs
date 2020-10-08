using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using System.Collections;
using MonoMod;



    public class patch_FAtlas : FAtlas
    {
        [MonoMod.MonoModIgnore]
        private Texture _texture;
        [MonoMod.MonoModIgnore]
        private Vector2 _textureSize;
        [MonoMod.MonoModIgnore]
        private string _dataPath;
        [MonoMod.MonoModIgnore]
        private string _imagePath;
        [MonoMod.MonoModIgnore]
        private bool _isTextureAnAsset;
        [MonoMod.MonoModIgnore]
        private List<FAtlasElement> _elements = new List<FAtlasElement>();
        [MonoMod.MonoModIgnore]
        private Dictionary<string, FAtlasElement> _elementsByName = new Dictionary<string, FAtlasElement>();


        [MonoModIgnore]
        public patch_FAtlas(string name, string imagePath, string dataPath, int index, bool shouldLoadAsSingleImage) : base(name, imagePath, dataPath, index, shouldLoadAsSingleImage)
        {
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

        private void LoadTexture()
        {
            WWW www = new WWW("file:///" + RootFolderDirectory() + "Assets" + Path.DirectorySeparatorChar + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + _imagePath + ".png");
            _texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            _texture.anisoLevel = 0;
            _texture.filterMode = FilterMode.Point;
            www.LoadImageIntoTexture(_texture as Texture2D);
            if (_texture == null)
            {
                throw new FutileException("Couldn't load the atlas texture from: " + _imagePath);
            }
            _isTextureAnAsset = true;
            _textureSize = new Vector2((float)_texture.width, (float)_texture.height);
        }

        private void LoadAtlasData()
        {
            string textAsset = File.ReadAllText(RootFolderDirectory() + "Assets" + Path.DirectorySeparatorChar + "Futile" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + _dataPath + ".txt");
            Dictionary<string, object> dictionary = textAsset.dictionaryFromJson();

            if (dictionary == null)
            {
                throw new FutileException("The atlas at " + _dataPath + " was not a proper JSON file. Make sure to select \"Unity3D\" in TexturePacker.");
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
                    throw new NotSupportedException("Futile no longer supports TexturePacker's \"rotated\" flag. Please disable it when creating the " + _dataPath + " atlas.");
                }
                IDictionary dictionary4 = (IDictionary)dictionary3["frame"];
                float num3 = float.Parse(dictionary4["x"].ToString());
                float num4 = float.Parse(dictionary4["y"].ToString());
                float num5 = float.Parse(dictionary4["w"].ToString());
                float num6 = float.Parse(dictionary4["h"].ToString());
                Rect uvRect = new Rect(num3 / _textureSize.x, (_textureSize.y - num4 - num6) / _textureSize.y, num5 / _textureSize.x, num6 / _textureSize.y);
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
                _elements.Add(fatlasElement);
                _elementsByName.Add(fatlasElement.name, fatlasElement);
            }
        }
    }

    public class patch_FAtlasManager : FAtlasManager
    {
        [MonoModIgnore]
        public patch_FAtlasManager()
        {
        }

        public FAtlas LoadAtlas(string atlasPath)
        {
            if (DoesContainAtlas(atlasPath))
            {
                return GetAtlasWithName(atlasPath);
            }
            return ActuallyLoadAtlasOrImage(atlasPath, atlasPath + Futile.resourceSuffix, atlasPath + Futile.resourceSuffix);
        }

        public FAtlas LoadImage(string imagePath)
        {
            if (DoesContainAtlas(imagePath))
            {
                return GetAtlasWithName(imagePath);
            }
            return ActuallyLoadAtlasOrImage(imagePath, imagePath + Futile.resourceSuffix, "");
        }
    }

