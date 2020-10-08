using System;
using RWCustom;
using UnityEngine;
using MonoMod;

class patch_Weapon : Weapon
{
    [MonoModIgnore]
    public patch_Weapon(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
    }

    public virtual void Launch(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, Vector2 throwDir, float frc, bool eu)
    {
        this.thrownBy = thrownBy;
        this.thrownPos = thrownPos;
        this.throwDir = new IntVector2((int)(throwDir.x*2), (int)(throwDir.y*2));
        this.firstFrameTraceFromPos = firstFrameTraceFromPos;
        changeDirCounter = 3;
        ChangeOverlap(true);
        base.firstChunk.MoveFromOutsideMyUpdate(eu, thrownPos);
        if (throwDir.x != 0)
        {
            base.firstChunk.vel.y = thrownBy.mainBodyChunk.vel.y * 0.5f;
            base.firstChunk.vel.x = thrownBy.mainBodyChunk.vel.x * 0.2f;
            BodyChunk firstChunk = base.firstChunk;
            firstChunk.vel.x = firstChunk.vel.x + (float)throwDir.x * 40f * frc;
            BodyChunk firstChunk2 = base.firstChunk;
            firstChunk2.vel.y = firstChunk2.vel.y + (float)throwDir.y * 40f * frc;
        }
        else
        {
            if (throwDir.y == 0)
            {
                ChangeMode(Mode.Free);
                return;
            }
            firstChunk.vel.x = thrownBy.mainBodyChunk.vel.x * 0.5f;
            firstChunk.vel.y = (float)throwDir.y * 40f * frc;
        }
        ChangeMode(Mode.Thrown);
        setRotation = new Vector2?(throwDir);
        rotationSpeed = 0f;
        meleeHitChunk = null;
    }

}
