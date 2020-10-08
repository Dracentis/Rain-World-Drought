using Rain_World_Drought.Enums;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class RoomRainHK
    {
        public static void Patch()
        {
            On.RoomRain.Update += new On.RoomRain.hook_Update(UpdateHK);
            On.RoomRain.ThrowAroundObjects += new On.RoomRain.hook_ThrowAroundObjects(ThrowAroundObjectsHK);
            On.BulletDrip.Strike += new On.BulletDrip.hook_Strike(BulletDripStrikeHK);
        }

        public const float HarmlessIntensity = 0.01086957f;

        private static void UpdateHK(On.RoomRain.orig_Update orig, RoomRain self, bool eu)
        {
            float initialWaterLevel = -100f;
            if (self.room.waterObject != null) { initialWaterLevel = self.room.waterObject.fWaterLevel; }

            orig.Invoke(self, eu);

            if (self.globalRain.flood != 0f)
            {
                if (self.room.waterObject != null)
                {
                    if (DroughtMod.EnumExt && (self.room.roomSettings.GetEffectAmount(EnumExt_Drought.Pulse) > 0f || self.room.roomSettings.GetEffectAmount(EnumExt_Drought.Drain) > 0f))
                    {
                        self.room.waterObject.fWaterLevel = initialWaterLevel;
                    }
                    else
                    {
                        self.room.waterObject.fWaterLevel = Mathf.Lerp(self.room.waterObject.fWaterLevel, self.FloodLevel, 0.2f);
                    }
                    self.room.waterObject.GeneralUpsetSurface(Mathf.InverseLerp(0f, 0.5f, self.globalRain.Intensity) * 4f);
                }
                else if (self.room.roomSettings.DangerType == RoomRain.DangerType.Flood || self.room.roomSettings.DangerType == RoomRain.DangerType.FloodAndRain)
                {
                    self.room.AddWater();
                }
            }
            if (self.room.roomSettings.RainIntensity == HarmlessIntensity)
            {
                if (self.dangerType == RoomRain.DangerType.Rain || self.dangerType == RoomRain.DangerType.FloodAndRain)
                {
                    self.intensity = Mathf.Lerp(self.intensity, self.globalRain.Intensity, 0.2f);
                }
                self.intensity = Mathf.Min(self.intensity, 0.17f);
                self.visibilitySetter = 0;
                if (self.intensity == 0f && self.lastIntensity > 0f)
                { self.visibilitySetter = -1; }
                else if (self.intensity > 0f && self.lastIntensity == 0f)
                { self.visibilitySetter = 1; }
                self.lastIntensity = self.intensity;
                self.distantDeathRainSound.Volume = Mathf.InverseLerp(1400f, 0f, (float)self.room.world.rainCycle.TimeUntilRain) * 0.17f;
            }
        }

        private static void ThrowAroundObjectsHK(On.RoomRain.orig_ThrowAroundObjects orig, RoomRain self)
        {
            if (self.room.roomSettings.RainIntensity <= HarmlessIntensity) { return; }
            orig.Invoke(self);
        }

        private static void BulletDripStrikeHK(On.BulletDrip.orig_Strike orig, BulletDrip self)
        {
            if (self.roomRain.room.roomSettings.RainIntensity == HarmlessIntensity)
            { // Harmless BulletDrip
                int num = UnityEngine.Random.Range(0, self.room.TileWidth);
                if (self.room.GetTile(num, self.room.TileHeight - 1).Solid) { return; }
                int num2 = self.room.roomRain.rainReach[num];
                if (num2 >= self.room.TileHeight) { return; }
                if (num2 == 0) { num2 = -10; }
                self.pos = self.room.MiddleOfTile(num, num2) + new Vector2(Mathf.Lerp(-10f, 10f, UnityEngine.Random.value), 10f);
                self.skyPos = self.room.MiddleOfTile(num, num2 + self.room.TileHeight + 50) + new Vector2(Mathf.Lerp(-30f, 30f, UnityEngine.Random.value) - 30f * self.roomRain.globalRain.rainDirection, 0f);
                self.falling = 0f;
                self.lastFalling = 0f;
                self.moveTip = true;
                self.fallSpeed = 1f / Mathf.Lerp(0.2f, 1.8f, UnityEngine.Random.value);
                self.delay = UnityEngine.Random.Range(0, 60 - (int)(self.roomRain.globalRain.bulletRainDensity * 60f));
                if (self.room.water && self.room.PointSubmerged(self.pos))
                {
                    self.pos.y = self.room.FloatWaterLevel(self.pos.x) - 30f;
                    self.room.waterObject.WaterfallHitSurface(self.pos.x, self.pos.x, 1f);
                    self.room.PlaySound(SoundID.Small_Object_Into_Water_Fast, self.pos);
                }
                else
                {
                    self.room.PlaySound(SoundID.Bullet_Drip_Strike, self.pos);
                }
            }
            else
            {
                orig.Invoke(self);
            }
        }
    }
}
