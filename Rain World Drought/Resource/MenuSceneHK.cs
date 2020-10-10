using Menu;
using Rain_World_Drought.Enums;
using RWCustom;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rain_World_Drought.Resource
{
    internal static class MenuSceneHK
    {
        public static void Patch()
        {
            On.Menu.MainMenu.BackgroundScene += new On.Menu.MainMenu.hook_BackgroundScene(MMBackgroundSceneHK);
            On.Menu.FastTravelScreen.GetRegionOrder += new On.Menu.FastTravelScreen.hook_GetRegionOrder(FTSGetRegionOrderHK);
            On.Menu.FastTravelScreen.TitleSceneID += new On.Menu.FastTravelScreen.hook_TitleSceneID(FTSTitleSceneIDHK);
            On.Menu.MenuScene.BuildScene += new On.Menu.MenuScene.hook_BuildScene(BuildSceneHK);
            On.Menu.SlideShow.ctor += new On.Menu.SlideShow.hook_ctor(SlideShowCtorHK);
        }

        #region CallLandscape

        private static MenuScene.SceneID MMBackgroundSceneHK(On.Menu.MainMenu.orig_BackgroundScene orig, MainMenu self)
        {
            if (self.manager.rainWorld.progression.miscProgressionData.menuRegion == null)
            {
                return orig.Invoke(self);
            }
            switch (self.manager.rainWorld.progression.miscProgressionData.menuRegion)
            {
                case "IS":
                    return EnumExt_Drought.Landscape_IS;
                case "FS":
                    return EnumExt_Drought.Landscape_FS;
                case "MW":
                    return EnumExt_Drought.Landscape_MW;
                case "LM":
                    return EnumExt_Drought.Landscape_LM;
            }
            return orig.Invoke(self);
        }

        private static List<string> FTSGetRegionOrderHK(On.Menu.FastTravelScreen.orig_GetRegionOrder orig)
        {
            List<string> list = orig.Invoke();
            list.Add("IS");
            list.Add("FS");
            list.Add("MW");
            list.Add("LM");
            return list;
        }

        private static MenuScene.SceneID FTSTitleSceneIDHK(On.Menu.FastTravelScreen.orig_TitleSceneID orig, FastTravelScreen self, string regionName)
        {
            switch (regionName)
            {
                default: return orig.Invoke(self, regionName);
                case "IS": return EnumExt_Drought.Landscape_IS;
                case "FS": return EnumExt_Drought.Landscape_FS;
                case "MW": return EnumExt_Drought.Landscape_MW;
                case "LM": return EnumExt_Drought.Landscape_LM;
            }
        }

        #endregion CallLandscape

        #region AddLandscape

        private static void BuildSceneHK(On.Menu.MenuScene.orig_BuildScene orig, MenuScene self)
        {
            EnumSwitch.MenuSceneID id = EnumSwitch.GetMenuSceneID(self.sceneID);
            if (id == EnumSwitch.MenuSceneID.DEFAULT) { orig.Invoke(self); return; }

            if ((self is InteractiveMenuScene)) { (self as InteractiveMenuScene).idleDepths = new List<float>(); }
            switch (id)
            {
                // Intake System
                case EnumSwitch.MenuSceneID.Landscape_IS:
                    self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Landscape - IS";
                    if (self.flatMode)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "Landscape - IS - Flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "IS_Landscape - 4", new Vector2(0, 0), 7f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "IS_Landscape - 3", new Vector2(0, 0), 4.2f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "IS_Landscape - 2", new Vector2(0, 0), 3.7f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "IS_Landscape - 1", new Vector2(0, 0), 0.75f, MenuDepthIllustration.MenuShader.Normal));
                    }
                    if (self.menu.ID == ProcessManager.ProcessID.FastTravelScreen || self.menu.ID == ProcessManager.ProcessID.RegionsOverviewScreen)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_IS_Shadow", new Vector2(0.01f, 0.01f), true, false));
                        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_IS", new Vector2(0.01f, 0.01f), true, false));
                        self.flatIllustrations[self.flatIllustrations.Count - 1].sprite.shader = self.menu.manager.rainWorld.Shaders["MenuText"];
                    }
                    break;
                // Forest Sanctuary
                case EnumSwitch.MenuSceneID.Landscape_FS:
                    // Debug.Log("Loading forest sanctuary");
                    self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Landscape - FS";
                    if (self.flatMode)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "Landscape - FS - Flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "FS_Landscape - 7", new Vector2(0, 0), 13f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "FS_Landscape - 6", new Vector2(0, 0), 11f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "FS_Landscape - 5", new Vector2(0, 0), 9f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "FS_Landscape - 4", new Vector2(0, 0), 7f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "FS_Landscape - 3", new Vector2(0, 0), 4.2f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "FS_Landscape - 2", new Vector2(0, 0), 3.7f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "FS_Landscape - 1", new Vector2(0, 0), 0.75f, MenuDepthIllustration.MenuShader.Normal));
                    }
                    if (self.menu.ID == ProcessManager.ProcessID.FastTravelScreen || self.menu.ID == ProcessManager.ProcessID.RegionsOverviewScreen)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_FS_Shadow", new Vector2(0.01f, 0.01f), true, false));
                        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_FS", new Vector2(0.01f, 0.01f), true, false));
                        self.flatIllustrations[self.flatIllustrations.Count - 1].sprite.shader = self.menu.manager.rainWorld.Shaders["MenuText"];
                    }
                    break;
                // The Fragmented Exterior
                case EnumSwitch.MenuSceneID.Landscape_MW:
                    self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Landscape - MW";
                    if (self.flatMode)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "Landscape - MW - Flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MW_Landscape - 4", new Vector2(0, 0), 7f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MW_Landscape - 3", new Vector2(0, 0), 4.2f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MW_Landscape - 2", new Vector2(0, 0), 3.7f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MW_Landscape - 1", new Vector2(0, 0), 0.75f, MenuDepthIllustration.MenuShader.Normal));
                    }
                    if (self.menu.ID == ProcessManager.ProcessID.FastTravelScreen || self.menu.ID == ProcessManager.ProcessID.RegionsOverviewScreen)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_MW_Shadow", new Vector2(0.01f, 0.01f), true, false));
                        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_MW", new Vector2(0.01f, 0.01f), true, false));
                        self.flatIllustrations[self.flatIllustrations.Count - 1].sprite.shader = self.menu.manager.rainWorld.Shaders["MenuText"];
                    }
                    break;
                // Looks To The Moon
                case EnumSwitch.MenuSceneID.Landscape_LM:
                    self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Landscape - LM";
                    if (self.flatMode)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "Landscape - LM - Flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "LM_Landscape - 7", new Vector2(0, 0), 13f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "LM_Landscape - 6", new Vector2(0, 0), 11f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "LM_Landscape - 5", new Vector2(0, 0), 9f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "LM_Landscape - 4", new Vector2(0, 0), 7f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "LM_Landscape - 3", new Vector2(0, 0), 4.2f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "LM_Landscape - 2", new Vector2(0, 0), 3.7f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "LM_Landscape - 1", new Vector2(0, 0), 0.75f, MenuDepthIllustration.MenuShader.Normal));
                    }
                    if (self.menu.ID == ProcessManager.ProcessID.FastTravelScreen || self.menu.ID == ProcessManager.ProcessID.RegionsOverviewScreen)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_LM_Shadow", new Vector2(0.01f, 0.01f), true, false));
                        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_LM", new Vector2(0.01f, 0.01f), true, false));
                        self.flatIllustrations[self.flatIllustrations.Count - 1].sprite.shader = self.menu.manager.rainWorld.Shaders["MenuText"];
                    }
                    break;

                case EnumSwitch.MenuSceneID.Dream_Message:
                    self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Dream - Message";
                    if (self.flatMode)
                    {
                        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "Dream - Message - Flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Message - 6", new Vector2(71f, 49f), 5f, MenuDepthIllustration.MenuShader.LightEdges));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Message - 5", new Vector2(71f, 49f), 3f, MenuDepthIllustration.MenuShader.Lighten));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Message - 4", new Vector2(71f, 49f), 2.5f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Message - 3", new Vector2(71f, 49f), 2.2f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Message - 2", new Vector2(71f, 49f), 2.3f, MenuDepthIllustration.MenuShader.Normal));
                        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Message - 1", new Vector2(71f, 49f), 1.5f, MenuDepthIllustration.MenuShader.Lighten));
                        ((self as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(3f);
                        ((self as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.5f);
                        ((self as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.3f);
                        ((self as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.3f);
                        ((self as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(1.4f);
                        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "Message - Symbol", new Vector2(683f, 70f), true, false));
                        MenuIllustration menuIllustration = self.flatIllustrations[self.flatIllustrations.Count - 1];
                        menuIllustration.pos.x = menuIllustration.pos.x - self.flatIllustrations[self.flatIllustrations.Count - 1].size.x / 2f;
                    }
                    break;
            }
            SetScenePos(self);
        }

        public static void SetScenePos(MenuScene self)
        {
            string filePath = string.Concat(
                ResourceManager.assetDir,
                "Futile",
                Path.DirectorySeparatorChar,
                "Resources",
                Path.DirectorySeparatorChar,
                self.sceneFolder,
                Path.DirectorySeparatorChar,
                "positions.txt"
                );
            if (self.sceneFolder != string.Empty && File.Exists(filePath))
            {
                string[] array = File.ReadAllLines(filePath);
                int num2 = 0;
                while (num2 < array.Length && num2 < self.depthIllustrations.Count)
                {
                    self.depthIllustrations[num2].pos.x = float.Parse(Regex.Split(array[num2], ", ")[0]);
                    self.depthIllustrations[num2].pos.y = float.Parse(Regex.Split(array[num2], ", ")[1]);
                    self.depthIllustrations[num2].lastPos = self.depthIllustrations[num2].pos;
                    num2++;
                }
            }
        }

        /*
        // Copied from SoonTM;
        private static void IllustLoadFilePatch(On.Menu.MenuIllustration.orig_LoadFile_1 orig, MenuIllustration illust, string folder)
        {
            if (folder.Length >= key.Length && folder.Substring(0, key.Length) == key)
            {
                if (folder.Length == key.Length)
                { CustomIllustLoad(illust, "Illustrations"); illust.folderName = ""; }
                else
                { illust.folderName = folder.Remove(0, key.Length); CustomIllustLoad(illust, illust.folderName); }
                return;
            }
            orig.Invoke(illust, folder);
        }

        public static void CustomIllustLoad(MenuIllustration illust, string folder)
        {
            // Texture2D texture = DataManager.ReadPNG(string.Concat(folder, ".", illust.fileName));
            string tempPath = string.Concat(Custom.RootFolderDirectory(), illust.fileName, ".png");
            File.WriteAllBytes(tempPath, DataManager.ReadBytes(string.Concat(folder, ".", illust.fileName)));

            illust.www = new WWW(string.Concat("file:///", tempPath));
            illust.texture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { wrapMode = TextureWrapMode.Clamp };
            if (illust.crispPixels)
            {
                illust.texture.anisoLevel = 0;
                illust.texture.filterMode = FilterMode.Point;
            }
            illust.www.LoadImageIntoTexture(illust.texture);
            HeavyTexturesCache.LoadAndCacheAtlasFromTexture(illust.fileName, illust.texture);
            illust.www = null;
            File.Delete(tempPath);
        } */

        #endregion AddLandscape

        #region ChangeOutro

        private static void SlideShowCtorHK(On.Menu.SlideShow.orig_ctor orig, SlideShow self, ProcessManager manager, SlideShow.SlideShowID slideShowID)
        {
            orig.Invoke(self, manager, slideShowID);

            if (slideShowID == SlideShow.SlideShowID.WhiteIntro) // Replace Wanderer Outro to dull one
            {
                SlideShowReset(self, manager);
                if (manager.musicPlayer != null)
                {
                    self.waitForMusic = "RW_Outro_Theme";
                    self.stall = true;
                    manager.musicPlayer.MenuRequestsSong(self.waitForMusic, 1.5f, 10f);
                }
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, 0f, 0f, 0f));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Outro_1_Left_Swim, ConvertTime(0, 1, 20), ConvertTime(0, 5, 0), ConvertTime(0, 17, 0)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Outro_2_Up_Swim, ConvertTime(0, 21, 0), ConvertTime(0, 25, 0), ConvertTime(0, 37, 0)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, ConvertTime(1, 1, 0), ConvertTime(1, 1, 0), ConvertTime(1, 6, 0)));
                for (int k = 1; k < self.playList.Count; k++)
                {
                    self.playList[k].startAt -= 1.1f;
                    self.playList[k].fadeInDoneAt -= 1.1f;
                    self.playList[k].fadeOutStartAt -= 1.1f;
                }
                self.nextProcess = ProcessManager.ProcessID.Credits;
            }
            else { return; }

            self.preloadedScenes = new SlideShowMenuScene[self.playList.Count];
            for (int k = 0; k < (int)self.preloadedScenes.Length; k++)
            {
                self.preloadedScenes[k] = new SlideShowMenuScene(self, self.pages[0], self.playList[k].sceneID);
                self.preloadedScenes[k].Hide();
            }
            manager.RemoveLoadingLabel();
            self.NextScene();
        }

        public static void SlideShowReset(SlideShow self, ProcessManager manager)
        {
            // Readd Loading
            manager.loadingLabel = new FLabel("font", manager.rainWorld.inGameTranslator.Translate("Loading..."))
            {
                x = 100.2f,
                y = 50.2f
            };
            Futile.stage.AddChild(manager.loadingLabel);
            // Remove NextScene
            self.scene.RemoveSprites();
            self.pages[0].subObjects.Remove(self.scene);
            self.preloadedScenes[0].Hide();
            // Reset List
            self.playList = new List<SlideShow.Scene>();

            if (manager.musicPlayer != null)
            {
                if (manager.musicPlayer.song != null)
                {
                    manager.musicPlayer.song.StopAndDestroy();
                    manager.musicPlayer.song = null;
                }
                if (manager.musicPlayer.nextSong != null) { manager.musicPlayer.nextSong = null; }
            }
        }

        public static float ConvertTime(int minutes, int seconds, int pps)
        {
            return (float)minutes * 60f + (float)seconds + (float)pps / 100f;
        }

        #endregion ChangeOutro
    }
}
