using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class GlobalRainHK
    {
        public static void Patch()
        {
            On.GlobalRain.Update += new On.GlobalRain.hook_Update(UpdateHK);
        }

        public static void InitBurstRain(GlobalRain self) => self.deathRain = new BurstRain(self);

        public static void LowerWaterLevel(GlobalRain self) => self.flood = -100;

        private static void UpdateHK(On.GlobalRain.orig_Update orig, GlobalRain self)
        {
            float rumbleVolume = self.RumbleSound;

            if (self.deathRain == null || !(self.deathRain is BurstRain)) { orig.Invoke(self); return; }


            if (UnityEngine.Random.value < 0.025f) { self.rainDirectionGetTo = Mathf.Lerp(-1f, 1f, UnityEngine.Random.value); }
            self.lastRainDirection = self.rainDirection;
            self.rainDirection = Mathf.Lerp(self.rainDirection, self.rainDirectionGetTo, 0.01f);
            if (self.rainDirection < self.rainDirectionGetTo) { self.rainDirection = Mathf.Min(self.rainDirection + 0.0125f, self.rainDirectionGetTo); }
            else if (self.rainDirection > self.rainDirectionGetTo) { self.rainDirection = Mathf.Max(self.rainDirection - 0.0125f, self.rainDirectionGetTo); }

            BurstRain br = self.deathRain as BurstRain;
            br.BurstRainUpdate();

            self.RumbleSound = Mathf.Lerp(rumbleVolume, 1f - Mathf.InverseLerp(0f, 0.6f, RainCycleHK.AnyRainApproaching(self.game.world.rainCycle)), 0.2f);

            if (br.burstRainMode >= BurstRain.BurstRainMode.BurstEnd)
            { self.floodSpeed = Mathf.Max(0.0f, self.floodSpeed - (self.game.IsStorySession ? 0.0005f : 0.005f)); }
            else if (br.burstRainMode > BurstRain.BurstRainMode.BurstGradeABuildUp)
            { self.floodSpeed = self.game.IsStorySession ? Mathf.Min(0.2f, self.floodSpeed + 0.0025f) : Mathf.Min(0.4f, self.floodSpeed + 0.006666667f); }
            //self.flood += self.floodSpeed;
            if (br.destroy)
            {
                self.deathRain = null;
                self.floodSpeed = 0;
            }
        }
    }
}
