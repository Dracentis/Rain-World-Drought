using System;
using UnityEngine;

namespace Rain_World_Drought.Effects
{
    public class DrainEffect : UpdatableAndDeletable
    {
        public DrainEffect(Room room)
        {
            this.room = room;
            room.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.Drain);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (room == null || room.roomSettings == null || room.waterObject == null)
            {
                return;
            }
            if (room.world.rainCycle.TimeUntilRain <= 1000)
            {
                if (room.world.rainCycle.TimeUntilRain > 0 && room.waterObject.fWaterLevel < room.waterObject.originalWaterLevel + this.room.roomRain.globalRain.flood)
                {
                    room.waterObject.fWaterLevel = room.waterObject.fWaterLevel + room.waterObject.originalWaterLevel / 3000f;
                }
                return;
            }
            float effectAmount = room.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.Drain);
            float num2 = (float)room.world.rainCycle.timer % 2500f / 2500f;
            float from = 0f;
            if (room.world != null & room.world.region.name != null & room.world.region.name.Equals("SL"))
            {
                from = room.waterObject.originalWaterLevel - 1800f;
            }
            if ((double)num2 < 0.4)
            {
                float num3 = num2 / 0.8f;
                room.waterObject.fWaterLevel = Mathf.Lerp(from, room.waterObject.originalWaterLevel + this.room.roomRain.globalRain.flood, (float)Math.Sin((double)(num3 * 3.14159274f)));
                room.waterObject.GeneralUpsetSurface((float)Math.Cos((double)(num3 * 3.14159274f)) * 4f);
                return;
            }
            if ((double)num2 < 0.6)
            {
                room.waterObject.fWaterLevel = room.waterObject.originalWaterLevel + this.room.roomRain.globalRain.flood;
                return;
            }
            float num4 = (num2 - 0.6f) / 0.8f + 0.5f;
            room.waterObject.fWaterLevel = Mathf.Lerp(from, room.waterObject.originalWaterLevel + this.room.roomRain.globalRain.flood, (float)Math.Sin((double)(num4 * 3.14159274f)));
            room.waterObject.GeneralUpsetSurface((float)Math.Cos((double)(num4 * 3.14159274f)) * 4f);
        }
    }
}