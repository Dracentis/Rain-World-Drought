using System;
using MonoMod;
using System.Collections.Generic;
using CoralBrain;
using Noise;
using RWCustom;
using ScavTradeInstruction;
using UnityEngine;
using VoidSea;

class patch_BulletDrip : BulletDrip
{
    [MonoModIgnore]
    public patch_BulletDrip(RoomRain roomRain) : base(roomRain) { }

    private void Strike()
    {
        int num = UnityEngine.Random.Range(0, this.room.TileWidth);
        if (this.room.GetTile(num, this.room.TileHeight - 1).Solid)
        {
            return;
        }
        int num2 = this.room.roomRain.rainReach[num];
        if (num2 >= this.room.TileHeight)
        {
            return;
        }
        if (num2 == 0)
        {
            num2 = -10;
        }
        this.pos = this.room.MiddleOfTile(num, num2) + new Vector2(Mathf.Lerp(-10f, 10f, UnityEngine.Random.value), 10f);
        this.skyPos = this.room.MiddleOfTile(num, num2 + this.room.TileHeight + 50) + new Vector2(Mathf.Lerp(-30f, 30f, UnityEngine.Random.value) - 30f * this.roomRain.globalRain.rainDirection, 0f);
        this.falling = 0f;
        this.lastFalling = 0f;
        this.moveTip = true;
        this.fallSpeed = 1f / Mathf.Lerp(0.2f, 1.8f, UnityEngine.Random.value);
        this.delay = UnityEngine.Random.Range(0, 60 - (int)(this.roomRain.globalRain.bulletRainDensity * 60f));
        SharedPhysics.CollisionResult collisionResult = new SharedPhysics.CollisionResult();
        if (roomRain.room.roomSettings.RainIntensity != 0.01086957f)
        {
            collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(null, this.room, this.skyPos, ref this.pos, 0.5f, 1, null, false);
        }
        if (this.room.water && this.room.PointSubmerged(this.pos))
        {
            this.pos.y = this.room.FloatWaterLevel(this.pos.x) - 30f;
            this.room.waterObject.WaterfallHitSurface(this.pos.x, this.pos.x, 1f);
            this.room.PlaySound(SoundID.Small_Object_Into_Water_Fast, this.pos);
        }
        else
        {
            this.room.PlaySound(SoundID.Bullet_Drip_Strike, this.pos);
        }

        if (roomRain.room.roomSettings.RainIntensity != 0.01086957f)
        {
            if (collisionResult.chunk != null)
            {
                this.pos = collisionResult.collisionPoint;
                BodyChunk chunk = collisionResult.chunk;
                chunk.vel.y = chunk.vel.y - 2f / collisionResult.chunk.mass;
                if (collisionResult.chunk.owner is Creature)
                {
                    (collisionResult.chunk.owner as Creature).Stun(UnityEngine.Random.Range(0, 4));
                }
            }
        }
    }
}

