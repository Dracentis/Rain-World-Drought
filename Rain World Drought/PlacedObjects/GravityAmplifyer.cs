using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;

// Token: 0x0200049A RID: 1178
public class GravityAmplifier : UpdatableAndDeletable, INotifyWhenRoomIsReady, IDrawable
{
    // Token: 0x06001E3A RID: 7738 RVA: 0x001AF10C File Offset: 0x001AD30C
    public GravityAmplifier(PlacedObject placedObject, Room room)
    {
        this.placedObject = placedObject;
        this.pos = placedObject.pos;
        this.lastPos = this.pos;
        this.depth = 0;
        if (!room.GetTile(placedObject.pos).Solid)
        {
            this.depth = ((!room.GetTile(placedObject.pos).wallbehind) ? 2 : 1);
        }
        this.roomSpecks = new System.Collections.Generic.List<GenericZeroGSpeck>();
        this.mySpecks = new System.Collections.Generic.List<GravityAmplifier.DisruptorSpeck>();
        for (int i = 0; i < 20; i++)
        {
            room.AddObject(new GravityAmplifier.DisruptorSpeck(this.pos + Custom.RNV() * 400f * UnityEngine.Random.value, this));
        }
        this.lights = new float[16, 4];
        for (int j = 0; j < this.lights.GetLength(0); j++)
        {
            this.lights[j, 3] = UnityEngine.Random.value;
        }
        this.pointDir = Custom.RNV();
        this.getToPointDir = this.pointDir;
        this.dirFac = UnityEngine.Random.value;
        this.dirFacGetTo = this.dirFac;
        this.power = 1f;
        this.lastPower = 1f;
    }

    // Token: 0x06001E3B RID: 7739 RVA: 0x001AF244 File Offset: 0x001AD444
    public override void Update(bool eu)
    {
        
        base.Update(eu);

        this.lastPos = this.pos;
        this.pos = this.placedObject.pos;
        if (this.lastPos != this.pos)
        {
            this.debugMode = 80;
        }
        if (this.debugMode > 0)
        {
            this.debugMode--;
        }
        if (Math.Abs(this.power) > 0f)
        {
            for (int i = 0; i < this.room.physicalObjects.Length; i++)
            {
                for (int j = 0; j < this.room.physicalObjects[i].Count; j++)
                {
                    for (int k = 0; k < this.room.physicalObjects[i][j].bodyChunks.Length; k++)
                    {
                        if (Custom.DistLess(this.pos, this.room.physicalObjects[i][j].bodyChunks[k].pos, 450f))
                        {
                            BodyChunk bodyChunk = this.room.physicalObjects[i][j].bodyChunks[k];
                            Vector2 vector = Custom.DirVec(this.pos, bodyChunk.pos);
                            Vector2 a = Custom.PerpendicularVector(vector);
                            float d = Mathf.Sign(Custom.DistanceToLine(bodyChunk.pos + bodyChunk.vel, this.pos, bodyChunk.pos));
                            float num = Vector2.Distance(this.pos, bodyChunk.pos);
                            float d2 = Mathf.Pow(Mathf.InverseLerp(450f, 200f, num), 2f);
                            bodyChunk.pos += vector * (260f - num) * 0.05f * d2 * ((num >= 260f) ? 0.5f : 1f) * this.power;
                            bodyChunk.vel += vector * (260f - num) * 0.05f * d2 * ((num >= 260f) ? 0.5f : 1f) * this.power;
                            bodyChunk.vel += a * d * 0.1f * d2 * this.power;
                        }
                    }
                }
            }
            for (int l = 0; l < this.roomSpecks.Count; l++)
            {
                if (Custom.DistLess(this.pos, this.roomSpecks[l].pos, 450f))
                {
                    GenericZeroGSpeck genericZeroGSpeck = this.roomSpecks[l];
                    Vector2 a2 = Custom.PerpendicularVector(Custom.DirVec(this.pos, genericZeroGSpeck.pos));
                    float d3 = Mathf.Pow(Mathf.InverseLerp(450f, 200f, Vector2.Distance(this.pos, genericZeroGSpeck.pos)), 2f);
                    genericZeroGSpeck.vel += a2 * Mathf.Sign(Custom.DistanceToLine(genericZeroGSpeck.pos + genericZeroGSpeck.vel, this.pos, genericZeroGSpeck.pos)) * 0.5f * d3 * Math.Abs(this.power);
                }
            }
        }
        this.pointDir = Vector3.Slerp(this.pointDir, this.getToPointDir, 0.01f);
        if (UnityEngine.Random.value < 0.0125f)
        {
            this.getToPointDir = Custom.RNV();
        }
        this.dirFac = Mathf.Lerp(this.dirFac, this.dirFacGetTo, 0.01f);
        if (UnityEngine.Random.value < 0.0125f)
        {
            this.dirFacGetTo = UnityEngine.Random.value;
        }
        for (int m = 0; m < this.lights.GetLength(0); m++)
        {
            this.lights[m, 2] = this.lights[m, 1];
            this.lights[m, 1] = this.lights[m, 0];
            this.lights[m, 0] = Mathf.Clamp(this.lights[m, 0] + Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) / 120f + Mathf.Lerp(-1f, 1f, this.lights[m, 3]) / 60f, 0f, 1f);
            float num2 = Vector2.Dot(Custom.DegToVec((float)m / 16f * 360f), this.pointDir);
            this.lights[m, 0] = Mathf.Lerp(this.lights[m, 0], Mathf.Pow(Mathf.InverseLerp(-1f, 1f, num2), 1.5f), Mathf.Pow(Mathf.Abs(num2), 8f) * 0.3f * this.dirFac * Mathf.InverseLerp(0.5f, 0f, Mathf.Abs(0.5f - this.lights[m, 3])));
            float num3 = 0f;
            num3 += this.lights[(m >= this.lights.GetLength(0) - 1) ? 0 : (m + 1), 2];
            num3 += this.lights[(m <= 0) ? (this.lights.GetLength(0) - 1) : (m - 1), 2];
            this.lights[m, 0] = Mathf.Lerp(this.lights[m, 0], num3 / 2f, 0.05f);
            if (UnityEngine.Random.value < 0.005f)
            {
                this.lights[m, 3] = UnityEngine.Random.value;
            }
        }
        this.lastPower = this.power;
        if (room.world.rainCycle.TimeUntilRain <= 1000)
        {
            this.power = Mathf.Lerp(this.power, 0f, 0.01f);
        }
        else
        {
            this.power = -(float)Math.Sin((double)(((float)room.world.rainCycle.timer + 1875f) % 2500f / 400.75f));
        }
        if (this.room.waterObject != null)
        {
            ((patch_Water)this.room.waterObject).GravityForce(this.placedObject.pos.x - 400f, this.placedObject.pos.x + 400f, 2f * power);
        }
    }

    // Token: 0x06001E3C RID: 7740 RVA: 0x001AF890 File Offset: 0x001ADA90
    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[17];
        sLeaser.sprites[0] = new FSprite("Futile_White", true);
        sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["GravityDisruptor"];
        sLeaser.sprites[0].scale = 37.5f;
        for (int i = 0; i < 16; i++)
        {
            sLeaser.sprites[i + 1] = new FSprite("Futile_White", true);
            sLeaser.sprites[i + 1].scaleX = 0.9375f;
            sLeaser.sprites[i + 1].scaleY = 3.125f;
            sLeaser.sprites[i + 1].anchorY = -0.55f;
            sLeaser.sprites[i + 1].rotation = (float)i / 16f * 360f;
            sLeaser.sprites[i + 1].shader = rCam.room.game.rainWorld.Shaders["CustomDepth"];
            sLeaser.sprites[i + 1].alpha = 1f - (10f * (float)this.depth + 2f) / 30f;
        }
        this.AddToContainer(sLeaser, rCam, null);
    }

    // Token: 0x06001E3D RID: 7741 RVA: 0x001AF9E4 File Offset: 0x001ADBE4
    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        float num = Mathf.Lerp(this.lastPower, this.power, timeStacker);
        Vector2 vector = rCam.ApplyDepth(this.placedObject.pos, -3f + 10f * (float)this.depth);
        sLeaser.sprites[0].x = vector.x - camPos.x;
        sLeaser.sprites[0].y = vector.y - camPos.y;
        sLeaser.sprites[0].alpha = Math.Abs(num);
        for (int i = 0; i < 16; i++)
        {
            sLeaser.sprites[i + 1].x = vector.x - camPos.x;
            sLeaser.sprites[i + 1].y = vector.y - camPos.y;
            if (this.debugMode > 0)
            {
                sLeaser.sprites[i + 1].color = new Color((i % 2 != 0) ? 0f : 1f, (i % 2 != 1) ? 0f : 1f, 0f);
            }
            else
            {
                if (this.power > 0)
                {
                    sLeaser.sprites[i + 1].color = new Color(0f, 0f, Mathf.Lerp(this.lights[i, 1], this.lights[i, 0], timeStacker) * Math.Abs(num));
                }
                else
                {
                    sLeaser.sprites[i + 1].color = new Color(Mathf.Lerp(this.lights[i, 1], this.lights[i, 0], timeStacker) * Math.Abs(num), 0f, 0f);
                }
                
            }
        }
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    // Token: 0x06001E3E RID: 7742 RVA: 0x0000592D File Offset: 0x00003B2D
    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
    }

    // Token: 0x06001E3F RID: 7743 RVA: 0x001AFB64 File Offset: 0x001ADD64
    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        sLeaser.sprites[0].RemoveFromContainer();
        rCam.ReturnFContainer("HUD").AddChild(sLeaser.sprites[0]);
        if (newContatiner == null)
        {
            newContatiner = rCam.ReturnFContainer("Foreground");
        }
        for (int i = 1; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }

    // Token: 0x06001E40 RID: 7744 RVA: 0x0000592D File Offset: 0x00003B2D
    public void ShortcutsReady()
    {
    }

    // Token: 0x06001E41 RID: 7745 RVA: 0x001AFBD4 File Offset: 0x001ADDD4
    public void AIMapReady()
    {
        for (int i = 0; i < this.room.updateList.Count; i++)
        {
            if (this.room.updateList[i] is GenericZeroGSpeck)
            {
                this.roomSpecks.Add(this.room.updateList[i] as GenericZeroGSpeck);
            }
        }
    }

    // Token: 0x0400208F RID: 8335
    public PlacedObject placedObject;

    // Token: 0x04002090 RID: 8336
    private int depth;

    // Token: 0x04002091 RID: 8337
    public Vector2 pos;

    // Token: 0x04002092 RID: 8338
    public Vector2 lastPos;

    // Token: 0x04002093 RID: 8339
    public int debugMode;

    // Token: 0x04002094 RID: 8340
    public System.Collections.Generic.List<GenericZeroGSpeck> roomSpecks;

    // Token: 0x04002095 RID: 8341
    public System.Collections.Generic.List<GravityAmplifier.DisruptorSpeck> mySpecks;

    // Token: 0x04002096 RID: 8342
    public float[,] lights;

    // Token: 0x04002097 RID: 8343
    private Vector2 pointDir;

    // Token: 0x04002098 RID: 8344
    private Vector2 getToPointDir;

    // Token: 0x04002099 RID: 8345
    private float dirFac;

    // Token: 0x0400209A RID: 8346
    private float dirFacGetTo;

    // Token: 0x0400209B RID: 8347
    private float power;

    // Token: 0x0400209C RID: 8348
    private float lastPower;

    // Token: 0x0200049B RID: 1179
    public class DisruptorSpeck : CosmeticSprite
    {
        // Token: 0x06001E42 RID: 7746 RVA: 0x001AFC38 File Offset: 0x001ADE38
        public DisruptorSpeck(Vector2 initPos, GravityAmplifier disruptor)
        {
            this.pos = initPos;
            this.lastPos = this.pos;
            this.disruptor = disruptor;
            this.myFloatSpeed = Mathf.Lerp(2f, 8f, UnityEngine.Random.value);
            this.vel = Custom.RNV() * this.myFloatSpeed;
            this.myOrbitRad = Vector2.Distance(disruptor.pos, this.pos);
            this.newOrbitRad = this.myOrbitRad;
        }

        // Token: 0x06001E43 RID: 7747 RVA: 0x001AFCB8 File Offset: 0x001ADEB8
        public override void Update(bool eu)
        {
            base.Update(eu);
            if (UnityEngine.Random.value < 0.01f)
            {
                this.newOrbitRad = Mathf.Lerp(50f, 400f, UnityEngine.Random.value);
            }
            this.myOrbitRad = Mathf.Lerp(this.myOrbitRad, this.newOrbitRad, 0.01f);
            Vector2 vector = Custom.DirVec(this.disruptor.pos, this.pos);
            Vector2 a = Custom.PerpendicularVector(vector);
            float d = Mathf.Sign(Custom.DistanceToLine(this.pos + this.vel, this.disruptor.pos, this.pos));
            float num = Vector2.Distance(this.disruptor.pos, this.pos);
            this.pos += vector * (this.myOrbitRad - num) * 0.05f;
            this.vel += vector * (this.myOrbitRad - num) * 0.05f;
            this.vel += a * d * Custom.LerpMap(num, 400f, 100f, 0.01f, 0.15f);
            this.vel = Vector2.Lerp(this.vel, this.vel.normalized * (this.myFloatSpeed / (Mathf.Lerp(this.myOrbitRad, 100f, 0.5f) * 0.01f)), 0.01f) * Mathf.Abs(this.disruptor.power);
        }

        // Token: 0x06001E44 RID: 7748 RVA: 0x001AFE54 File Offset: 0x001AE054
        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("pixel", true);
            this.AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("Foreground"));
        }

        // Token: 0x06001E45 RID: 7749 RVA: 0x001AFE94 File Offset: 0x001AE094
        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[0].x = Mathf.Lerp(this.lastPos.x, this.pos.x, timeStacker) - camPos.x;
            sLeaser.sprites[0].y = Mathf.Lerp(this.lastPos.y, this.pos.y, timeStacker) - camPos.y;
            sLeaser.sprites[0].color = new Color(Math.Abs(this.disruptor.power), Math.Abs(this.disruptor.power), Math.Abs(this.disruptor.power));
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        // Token: 0x0400209D RID: 8349
        private GravityAmplifier disruptor;

        // Token: 0x0400209E RID: 8350
        private float myFloatSpeed;

        // Token: 0x0400209F RID: 8351
        private float myOrbitRad;

        // Token: 0x040020A0 RID: 8352
        private float newOrbitRad;
    }
}
