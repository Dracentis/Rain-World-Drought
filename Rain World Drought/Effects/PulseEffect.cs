using System;
using UnityEngine;

public class PulseEffect : UpdatableAndDeletable
{
    public PulseEffect(Room room)
    {
        this.room = room;
        room.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.Pulse);
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
                room.waterObject.fWaterLevel = room.waterObject.fWaterLevel + room.waterObject.originalWaterLevel / 300f;
            }
            else if (room.world.rainCycle.TimeUntilRain > 0 && room.waterObject.fWaterLevel > room.waterObject.originalWaterLevel + this.room.roomRain.globalRain.flood)
            {
                room.waterObject.fWaterLevel = room.waterObject.fWaterLevel - room.waterObject.originalWaterLevel / 300f;
            }
            else
            {
                room.waterObject.fWaterLevel = Mathf.Lerp(room.waterObject.fWaterLevel, room.waterObject.originalWaterLevel + this.room.roomRain.globalRain.flood, 0.1f);
            }
            return;
        }
        float effectAmount = room.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.Pulse);
            float num = (600f*effectAmount) * (float)Math.Sin((double)((float)room.world.rainCycle.timer % 2500f / 397.887257729f));
            room.waterObject.fWaterLevel = room.waterObject.originalWaterLevel + num + this.room.roomRain.globalRain.flood;
            room.waterObject.GeneralUpsetSurface((float)Math.Sin((double)((float)room.world.rainCycle.timer % 5000f / 801.5f)) * 2f);
            return;
        
    }
}
