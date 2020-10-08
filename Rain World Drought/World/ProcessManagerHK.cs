using Rain_World_Drought.Enums;
using Rain_World_Drought.Slugcat;

namespace Rain_World_Drought.OverWorld
{
    internal static class ProcessManagerHK
    {
        public static void Patch()
        {
            On.ProcessManager.SwitchMainProcess += new On.ProcessManager.hook_SwitchMainProcess(SwitchMainProcessHK);
        }

        private static void SwitchMainProcessHK(On.ProcessManager.orig_SwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
        {
            if (DroughtMod.EnumExt && ID == EnumExt_Drought.MessageScreen)
            {
                self.shadersTime = 0f;
                if (self.menuMic == null)
                {
                    self.menuMic = new MenuMicrophone(self, self.soundLoader);
                    self.sideProcesses.Add(self.menuMic);
                }
                MainLoopProcess oldMainLoop = self.currentMainLoop;
                if (self.currentMainLoop != null)
                {
                    self.currentMainLoop.ShutDownProcess();
                    self.currentMainLoop.processActive = false;
                    self.currentMainLoop = null;
                    self.soundLoader.ReleaseAllUnityAudio();
                    HeavyTexturesCache.ClearRegisteredFutileAtlases();
                    System.GC.Collect();
                    UnityEngine.Resources.UnloadUnusedAssets();
                }
                self.rainWorld.progression.Revert();

                self.currentMainLoop = new MessageScreen(self);

                if (oldMainLoop != null)
                {
                    oldMainLoop.CommunicateWithUpcomingProcess(self.currentMainLoop);
                }
                self.blackFadeTime = self.currentMainLoop.FadeInTime;
                self.blackDelay = self.currentMainLoop.InitialBlackSeconds;
                if (self.fadeSprite != null)
                {
                    self.fadeSprite.RemoveFromContainer();
                    Futile.stage.AddChild(self.fadeSprite);
                }
                if (self.loadingLabel != null)
                {
                    self.loadingLabel.RemoveFromContainer();
                    Futile.stage.AddChild(self.loadingLabel);
                }
                if (self.musicPlayer != null)
                {
                    self.musicPlayer.UpdateMusicContext(self.currentMainLoop);
                }
                self.pauseFadeUpdate = true;

                return;
            }
            orig.Invoke(self, ID);
        }
    }
}
