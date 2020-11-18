using Rain_World_Drought.Enums;
using System;
using UnityEngine;

namespace Rain_World_Drought.PlacedObjects
{
    public class GiantPiston : Piston
    {
        public GiantPiston(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            spriteName = null;
            spriteScale = new Vector2(3.2f, 3.1578947368f);
            collisionSize = new Vector2(80, 60);
        }

        protected override float MaxMovement => 60f;

        protected override void PlaySound(bool atTop)
        {
            if (atTop)
            {
                room.PlaySound(SoundID.Gate_Pillows_Move_Out, placedPos, 0.7f, 0.3f);
                room.PlaySound(SoundID.Shelter_Piston_Out, placedPos, 0.8f, 0.5f);
            }
            else
            {
                room.PlaySound(SoundID.Gate_Pillows_Move_In, placedPos, 0.7f, 0.3f);
                room.PlaySound(SoundID.Shelter_Piston_In_Soft, placedPos, 0.8f, 0.5f);
            }
        }
    }
}