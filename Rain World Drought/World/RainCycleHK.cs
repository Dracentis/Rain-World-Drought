using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class RainCycleHK
    {
        public static void Patch()
        {
            On.RainCycle.ctor += new On.RainCycle.hook_ctor(CtorHK);
            IDetour hkLCBOR = new Hook(typeof(RainCycle).GetProperty("LightChangeBecauseOfRain", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true),
                typeof(RainCycleHK).GetMethod("LightChangeBecauseOfRainHK", BindingFlags.Static | BindingFlags.Public));
            IDetour hkRGO = new Hook(typeof(RainCycle).GetProperty("RainGameOver", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(),
                typeof(RainCycleHK).GetMethod("RainGameOverHK", BindingFlags.Static | BindingFlags.Public));
            IDetour hkMA = new Hook(typeof(RainCycle).GetProperty("MusicAllowed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(),
                typeof(RainCycleHK).GetMethod("MusicAllowedHK", BindingFlags.Static | BindingFlags.Public));
            IDetour hkSC = new Hook(typeof(RainCycle).GetProperty("ScreenShake", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(),
                typeof(RainCycleHK).GetMethod("ScreenShakeHK", BindingFlags.Static | BindingFlags.Public));
            On.RainCycle.Update += new On.RainCycle.hook_Update(UpdateHK);
        }

        public static int burstNum;
        public static bool burstRainHasHit;

        private static void CtorHK(On.RainCycle.orig_ctor orig, RainCycle self, World world, float minutes)
        {
            orig.Invoke(self, world, minutes);
            burstNum = 0; burstRainHasHit = false;
            if (self.cycleLength > 36000) { burstNum = 3; }
            else if (self.cycleLength > 32000) { burstNum = 2; }
            else if (self.cycleLength > 28000) { burstNum = 1; }
            else if (self.cycleLength < 20000) { } //(this.world.game.globalRain as patch_GlobalRain).LowerWaterLevel();
        }

        public static float BurstApproaching(RainCycle self)
        {
            if (self.world.game.IsStorySession)
            { return Mathf.InverseLerp(0f, 1800f, Mathf.Abs((float)TimeUntilBurst(self, CurrentBurst(self)))); }
            return Mathf.InverseLerp(0f, 400f, (float)TimeUntilBurst(self, CurrentBurst(self)));
        }

        public static float AnyRainApproaching(RainCycle self)
        {
            float burstApproaching = BurstApproaching(self);
            return Mathf.Min(burstApproaching, self.RainApproaching);
        }


#pragma warning disable IDE0060

        public delegate float LightChangeBecauseOfRain(RainCycle self);

        public static float LightChangeBecauseOfRainHK(LightChangeBecauseOfRain orig, RainCycle self)
        {
            float burstApproaching = AnyRainApproaching(self);
            if (burstApproaching < 0.2f)
            { return Mathf.InverseLerp(0.2f, 1f, burstApproaching); }
            else
            { return Mathf.Min(Mathf.InverseLerp(0.4f, 1f, burstApproaching), Mathf.InverseLerp(0.4f, 1f, burstApproaching)); }
        }

        public delegate bool RainGameOver(RainCycle self);

        public static bool RainGameOverHK(RainGameOver orig, RainCycle self)
        {
            return orig.Invoke(self) || burstRainHasHit;
        }

        public delegate bool MusicAllowed(RainCycle self);

        public static bool MusicAllowedHK(MusicAllowed orig, RainCycle self)
        {
            return self.world.game.IsArenaSession || (self.TimeUntilRain >= 2400 && TimeUntilBurst(self, CurrentBurst(self)) > 1200);
        }

        public delegate float ScreenShake(RainCycle self);
        public static float ScreenShakeHK(ScreenShake orig, RainCycle self)
        {
            // Give burst rain screen shake
            float origSc = orig(self);
            float sc = Mathf.Pow(1f - Mathf.InverseLerp(0f, 0.2f, AnyRainApproaching(self)), 2f);
            return Mathf.Max(sc, origSc);
        }

        private static void UpdateHK(On.RainCycle.orig_Update orig, RainCycle self)
        {
            if (!burstRainHasHit && TimeUntilBurst(self, CurrentBurst(self)) < 0)
            {
                BurstRainHit(self);
                burstRainHasHit = true;
            }
            orig.Invoke(self);
        }

        private static void BurstRainHit(RainCycle self)
        {
            GlobalRainHK.InitBurstRain(self.world.game.globalRain);
            Debug.Log("Burst Rain Hit");
        }

        private const int disableBurst = 99999999;

        public static int GetBurstIndex(RainCycle self, int index)
        {
            if (burstNum <= index) { return disableBurst; }
            return Mathf.FloorToInt((self.cycleLength / ((burstNum + 1) * (burstNum - index))) / 1200f);
        }

        public static int GetBurstTime(RainCycle self, int index)
        {
            if (burstNum <= index) { return disableBurst; }
            return Mathf.FloorToInt(GetBurstIndex(self, index) / ((float)(self.cycleLength / 1200f)) * self.cycleLength);
        }

        public static int TimeUntilBurst(RainCycle self, int index)
        {
            if (burstNum <= index) { return disableBurst; }
            return GetBurstTime(self, index) - self.timer;
        }

        public static int CurrentBurst(RainCycle self)
        {
            const int maxBurst = 3;
            for (int b = maxBurst; b > 0; b--)
            {
                int timeUntilBurst = TimeUntilBurst(self, b - 1);
                if (burstNum >= b && timeUntilBurst >= -1800)
                {
                    if (timeUntilBurst > 0 && burstRainHasHit)
                    { burstRainHasHit = false; }
                    return b - 1;
                }
            }
            return maxBurst;
        }

        public static float BurstRainApproaching(RainCycle self)
        {
            int timeUntilBurst = TimeUntilBurst(self, CurrentBurst(self));
            if (timeUntilBurst > 0)
            { return Mathf.InverseLerp(0f, 2400f, (float)timeUntilBurst); }
            else
            { return Mathf.InverseLerp(0f, -1200f, (float)timeUntilBurst); }
        }
    }
}
