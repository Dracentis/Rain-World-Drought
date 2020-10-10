using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RWCustom;

namespace Rain_World_Drought.Creatures
{
    public class LightWormGraphics : GraphicsModule
    {
        public LightWormGraphics(PhysicalObject ow) : base(ow, false)
        {
            numberOfWavesOnBody = 1.8f;
            sinSpeed = 0.0166666675f;
            swallowArray = new float[worm.tentacle.tChunks.Length];
            cullRange = 1000f;
        }

        private LightWorm worm
        {
            get
            {
                return owner as LightWorm;
            }
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update()
        {
            base.Update();
            if (culled)
            {
                return;
            }
            if (worm.Consious)
            {
                /*
                if (this.worm.AI.attackCounter < 20)
                {
                    this.numberOfWavesOnBody = Mathf.Lerp(this.numberOfWavesOnBody, Mathf.Lerp(1.8f, 3.4f, this.worm.AI.stress), 0.1f);
                    this.sinSpeed = Mathf.Lerp(this.sinSpeed, Mathf.Lerp(0.0166666675f, 0.05f, this.worm.AI.stress), 0.05f);
                }
                else
                {
                    this.numberOfWavesOnBody = Mathf.Lerp(this.numberOfWavesOnBody, 5f, 0.01f);
                    this.sinSpeed = Mathf.Lerp(this.sinSpeed, 0.05f, 0.1f);
                }
                this.sinWave += this.sinSpeed;
                if (this.sinWave > 1f)
                {
                    this.sinWave -= 1f;
                }
                if (this.worm.AI.attackCounter > 40 && this.worm.AI.attackCounter < 190 && Random.value < 0.0333333351f)
                {
                    this.swallowArray[this.swallowArray.Length - 1] = Mathf.Pow(Random.value, 0.5f);
                }
                if (Random.value < 0.333333343f)
                {
                    for (int i = 0; i < this.swallowArray.Length - 1; i++)
                    {
                        this.swallowArray[i] = Mathf.Lerp(this.swallowArray[i], this.swallowArray[i + 1], 0.7f);
                    }
                }
                this.swallowArray[this.swallowArray.Length - 1] = Mathf.Lerp(this.swallowArray[this.swallowArray.Length - 1], 0f, 0.7f);
                */
            }
            lastExtended = extended;
            extended = worm.extended;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[4];
            sLeaser.sprites[0] = new FSprite("WormEye", true);
            sLeaser.sprites[1] = TriangleMesh.MakeLongMesh(worm.tentacle.tChunks.Length, false, false);
            sLeaser.sprites[2] = new FSprite("WormHead", true);
            sLeaser.sprites[3] = new FSprite("WormEye", true);
            sLeaser.sprites[2].scale = Mathf.Lerp(worm.bodySize, 1f, 0.5f);
            AddToContainer(sLeaser, rCam, null);
            base.InitiateSprites(sLeaser, rCam);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            if (culled)
            {
                return;
            }
            float num = Mathf.Lerp(lastExtended, extended, timeStacker);
            Vector2 vector = worm.bodyChunks[1].pos + new Vector2(0f, -30f - 100f * (1f - num));
            float num2 = 4f;
            for (int i = 0; i < worm.tentacle.tChunks.Length; i++)
            {
                Vector2 vector2 = Vector2.Lerp(worm.tentacle.tChunks[i].lastPos, worm.tentacle.tChunks[i].pos, timeStacker);
                float num3 = (float)i / (float)(worm.tentacle.tChunks.Length - 1);
                float num4 = Mathf.Pow(Mathf.Max(1f - num3 - num, 0f), 1.5f);
                if (num < 0.2f)
                {
                    num4 = Mathf.Min(1f, num4 + Mathf.InverseLerp(0.2f, 0f, num));
                }
                vector2 = Vector2.Lerp(vector2, worm.bodyChunks[1].pos, num4) + new Vector2(0f, -100f * Mathf.Pow(num4, 0.5f));
                float d = Mathf.Sin((Mathf.Lerp(sinWave - sinSpeed, sinWave, timeStacker) + num3 * numberOfWavesOnBody) * 3.14159274f * 2f);
                vector2 += Custom.PerpendicularVector((vector2 - vector).normalized) * d * 11f * Mathf.Pow(Mathf.Max(0f, Mathf.Sin(num3 * 3.14159274f)), 0.75f) * num;
                Vector2 normalized = (vector2 - vector).normalized;
                Vector2 a = Custom.PerpendicularVector(normalized);
                if (i == worm.tentacle.tChunks.Length - 1)
                {
                    sLeaser.sprites[2].x = vector2.x - camPos.x;
                    sLeaser.sprites[2].y = vector2.y - camPos.y;
                    sLeaser.sprites[2].rotation = Custom.AimFromOneVectorToAnother(-normalized, normalized);
                    float num5 = Mathf.Cos(Custom.AimFromOneVectorToAnother(-normalized, normalized) / 360f * 2f * 3.14159274f);
                    num5 = Mathf.Pow(Mathf.Abs(num5), 0.25f) * Mathf.Sign(num5);
                    int num6 = (num5 * Mathf.Sign(normalized.x) <= 0f) ? 0 : 3;
                    sLeaser.sprites[3 - num6].x = vector2.x - camPos.x + normalized.x * 5f * worm.bodySize + a.x * 3f * Mathf.Lerp(worm.bodySize, 1f, 0.75f) * num5;
                    sLeaser.sprites[3 - num6].y = vector2.y - camPos.y + normalized.y * 5f * worm.bodySize + a.y * 3f * Mathf.Lerp(worm.bodySize, 1f, 0.75f) * num5;
                    sLeaser.sprites[num6].x = vector2.x - camPos.x + normalized.x * 5f * worm.bodySize - a.x * 3f * Mathf.Lerp(worm.bodySize, 1f, 0.75f) * num5;
                    sLeaser.sprites[num6].y = vector2.y - camPos.y + normalized.y * 5f * worm.bodySize - a.y * 3f * Mathf.Lerp(worm.bodySize, 1f, 0.75f) * num5;
                }
                float d2 = Vector2.Distance(vector2, vector) / 7f;
                float num7 = worm.tentacle.tChunks[i].stretchedRad + swallowArray[i] * 5f;
                (sLeaser.sprites[1] as TriangleMesh).MoveVertice(i * 4, vector - a * (num7 + num2) * 0.5f + normalized * d2 - camPos);
                (sLeaser.sprites[1] as TriangleMesh).MoveVertice(i * 4 + 1, vector + a * (num7 + num2) * 0.5f + normalized * d2 - camPos);
                (sLeaser.sprites[1] as TriangleMesh).MoveVertice(i * 4 + 2, vector2 - a * num7 - normalized * d2 - camPos);
                (sLeaser.sprites[1] as TriangleMesh).MoveVertice(i * 4 + 3, vector2 + a * num7 - normalized * d2 - camPos);
                num2 = num7;
                vector = vector2;
            }
            sLeaser.sprites[0].color = Color.black;
            sLeaser.sprites[3].color = Color.black;
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[1].color = Color.white;
            sLeaser.sprites[2].color = Color.white;
            //this.whiteColor = palette.skyColor;
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            base.AddToContainer(sLeaser, rCam, newContatiner);
        }

        //private Color whiteColor;
        private float sinWave;

        private float numberOfWavesOnBody;
        private float sinSpeed;
        private float[] swallowArray;
        private float lastExtended;
        private float extended;
    }
}