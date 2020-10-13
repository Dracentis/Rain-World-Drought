using HUD;
using Menu;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class RainMeterHK
    {
        public static void Patch()
        {
            On.HUD.RainMeter.ctor += new On.HUD.RainMeter.hook_ctor(CtorHK);
            On.HUD.RainMeter.Update += new On.HUD.RainMeter.hook_Update(UpdateHK);
            On.HUD.HUD.InitSleepHud += new On.HUD.HUD.hook_InitSleepHud(InitSleepHudHK);
            On.Menu.SleepAndDeathScreen.GetDataFromGame += new On.Menu.SleepAndDeathScreen.hook_GetDataFromGame(GetDataFromGameHK);
            On.HUD.KarmaMeter.KarmaSymbolSprite += new On.HUD.KarmaMeter.hook_KarmaSymbolSprite(KarmaSymbolSpriteHK);
            On.HUD.HUD.InitSinglePlayerHud += new On.HUD.HUD.hook_InitSinglePlayerHud(InitSinglePlayerHudHK);
        }

        public static bool[] danger;

        private static void CtorHK(On.HUD.RainMeter.orig_ctor orig, RainMeter self, HUD.HUD hud, FContainer fContainer)
        {
            if(hud.owner.GetOwnerType() != HUD.HUD.OwnerType.Player)
            {
                self.hud = hud;
                return;
            }
            orig.Invoke(self, hud, fContainer);
            danger = new bool[self.circles.Length];
            for (int q = 0; q < 3; q++)
            {
                int b = RainCycleHK.GetBurstIndex((hud.owner as Player).room.world.rainCycle, q);
                if (b < self.circles.Length) { danger[b] = true; }
            }
        }

        private static void UpdateHK(On.HUD.RainMeter.orig_Update orig, RainMeter self)
        {
            orig.Invoke(self);
            for (int i = 0; i < self.circles.Length; i++)
            {
                if (danger[i]) { self.circles[i].color = 1; }
            }
        }

        private static void InitSleepHudHK(On.HUD.HUD.orig_InitSleepHud orig, HUD.HUD self,
            SleepAndDeathScreen sleepAndDeathScreen, Map.MapData mapData, SlugcatStats charStats)
        {
            orig.Invoke(self, sleepAndDeathScreen, mapData, charStats);

            for (int i = 0; i < 3; i++)
            {
                int dayType;
                if (NextRainMeter.nextcycleLength[i] < 20000) { dayType = 1; }
                else if (NextRainMeter.nextcycleLength[i] < 28000) { dayType = 2; }
                else { dayType = 3; }

                NextKarmaMeter karmaMeter = new NextKarmaMeter(self, self.fContainers[1], new IntVector2(dayType, NextKarmaSymbol), false);
                self.AddPart(karmaMeter);
                karmaMeter.pos = new Vector2(self.rainWorld.options.ScreenSize.x - 280f + 105f * i, self.rainWorld.options.ScreenSize.y - 70f);
                karmaMeter.lastPos = karmaMeter.pos;

                NextRainMeter rainMeter = new NextRainMeter(self, self.fContainers[1], i);
                self.AddPart(rainMeter);
                rainMeter.pos = new Vector2(self.rainWorld.options.ScreenSize.x - 280f + 105f * i, self.rainWorld.options.ScreenSize.y - 70f);
                rainMeter.lastPos = rainMeter.pos;
            }
        }

        private static void GetDataFromGameHK(On.Menu.SleepAndDeathScreen.orig_GetDataFromGame orig, SleepAndDeathScreen self, KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            int oldSeed = UnityEngine.Random.seed;
            for (int i = 0; i < 3; i++)
            {
                UnityEngine.Random.seed = package.saveState.seed + package.saveState.cycleNumber + i;
                /// game.rainWorld.setup.cycleTimeMin, game.rainWorld.setup.cycleTimeMax
                NextRainMeter.nextcycleLength[i] = (int)(Mathf.Lerp(300f, 1000f, UnityEngine.Random.value) / 60f * 40f * 60f);
                if (NextRainMeter.nextcycleLength[i] > 36000) { NextRainMeter.burstNum[i] = 3; }
                else if (NextRainMeter.nextcycleLength[i] > 32000) { NextRainMeter.burstNum[i] = 2; }
                else if (NextRainMeter.nextcycleLength[i] > 28000) { NextRainMeter.burstNum[i] = 1; }
                else { NextRainMeter.burstNum[i] = 0; }
            }
            UnityEngine.Random.seed = oldSeed;
            orig.Invoke(self, package);
        }

        public const int NextKarmaSymbol = 12;

        private static string KarmaSymbolSpriteHK(On.HUD.KarmaMeter.orig_KarmaSymbolSprite orig, bool small, IntVector2 k)
        {
            if (k.y == NextKarmaSymbol) { return string.Concat("smallKarma", k.x, "-", k.y); }
            return orig.Invoke(small, k);
        }

        private static void InitSinglePlayerHudHK(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
        {
            orig(self, cam);
            if(self.owner is Player ply && Slugcat.WandererSupplement.IsWanderer(ply))
            {
                self.AddPart(new FocusMeter(self, self.fContainers[1]));
            }
        }
    }
}
