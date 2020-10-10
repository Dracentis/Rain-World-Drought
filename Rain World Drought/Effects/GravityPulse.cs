using System;
using Rain_World_Drought.Enums;
using UnityEngine;

namespace Rain_World_Drought.Effects
{
    public class GravityPulse : UpdatableAndDeletable
    {
        public GravityPulse(Room room) : base()
        {
            this.active = true;
            this.room = room;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (!this.active)
            {
                room.gravity = Mathf.Lerp(room.gravity, 1f, 0.1f);
                return;
            }
            if (room.world.rainCycle.TimeUntilRain <= 1000)
            {
                room.gravity = Mathf.Lerp(room.gravity, 1f, 0.1f);
                if (room.world.rainCycle.TimeUntilRain <= 100)
                {
                    active = false;
                    room.gravity = 1f;
                }
                return;
            }
            float state = ((float)Math.Sin((double)(((float)room.world.rainCycle.timer + 1875f) % 2500f / 397.88735f)) * 3f) + 0.75f;
            if (state > 1.5f)
            {
                state = 1.5f;
            }
            else if (state < 0f)
            {
                state = 0f;
            }
            else
            {
                float delta = Mathf.Pow(1f - Mathf.Abs(0.75f - state), 0.5f);
                for (int j = 0; j < this.room.game.cameras.Length; j++)
                {
                    if (this.room.abstractRoom.name.Equals(this.room.game.cameras[j].room.abstractRoom.name))
                    {
                        this.room.game.cameras[j].room.ScreenMovement(null, new Vector2(0f, 0f), delta * this.room.roomSettings.GetEffectAmount(EnumExt_Drought.GravityPulse));
                    }
                }
            }
            this.room.gravity = Mathf.Lerp(1f, state, this.room.roomSettings.GetEffectAmount(EnumExt_Drought.GravityPulse));
        }

        public bool active;
    }
}
