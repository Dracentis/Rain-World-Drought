using Rain_World_Drought.Enums;
using System;
using UnityEngine;

namespace Rain_World_Drought.PlacedObjects
{
    public class LargePiston : Piston
    {
        public LargePiston(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            spriteName = "LargePiston";
            spriteScale = new Vector2(2.4f, 2.10526315f);
            collisionSize = new Vector2(60, 40);
        }

        protected override float MaxMovement => 50f;
    }
}
