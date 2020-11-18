using Rain_World_Drought.Enums;
using System;
using UnityEngine;

namespace Rain_World_Drought.PlacedObjects
{
    public class SmallPiston : Piston
    {
        public SmallPiston(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            spriteName = "SmallPiston";
            spriteScale = new Vector2(1.2f, 1.57894742f);
            collisionSize = new Vector2(30, 30);
        }

        protected override float MaxMovement => 40f;
    }
}
