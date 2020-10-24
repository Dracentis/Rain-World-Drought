using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    public class GreySpear : Spear
    {
        public GreySpear(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {
        }

        public Color effectColor = new Color(0.7f, 0.7f, 0.7f);

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[2];
            sLeaser.sprites[1] = new FSprite("GreySpearA", true);
            sLeaser.sprites[0] = new FSprite("GreySpearB", true);
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 a = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
            if (this.vibrate > 0)
            {
                a += Custom.DegToVec(UnityEngine.Random.value * 360f) * 2f * UnityEngine.Random.value;
            }
            Vector3 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            for (int i = 1; i >= 0; i--)
            {
                sLeaser.sprites[i].x = a.x - camPos.x;
                sLeaser.sprites[i].y = a.y - camPos.y;
                sLeaser.sprites[i].anchorY = Mathf.Lerp((!this.lastPivotAtTip) ? 0.5f : 0.85f, (!this.pivotAtTip) ? 0.5f : 0.85f, timeStacker);
                sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), v);
            }
            if (this.blink > 0 && UnityEngine.Random.value < 0.5f)
            {
                sLeaser.sprites[1].color = base.blinkColor;
            }
            else
            {
                sLeaser.sprites[1].color = this.color;
            }
            sLeaser.sprites[0].color = this.effectColor;

            if (base.slatedForDeletetion || this.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            this.color = palette.blackColor;
            sLeaser.sprites[1].color = this.color;
            sLeaser.sprites[0].color = this.effectColor;
        }
    }
}
