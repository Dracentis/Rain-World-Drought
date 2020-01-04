using System;
using RWCustom;
using UnityEngine;

// Token: 0x0200049D RID: 1181
public class SeaDrakeGraphics : GraphicsModule, HasDanglers
{
    public SeaDrakeGraphics(SeaDrake ow) : base(ow, false)
    {
        this.fish = ow;
        headDanglers = 6;
        neckDanglers = 8;
        wingDanglers = 18;
        this.danglers = new Dangler[headDanglers+neckDanglers+2*wingDanglers];
        this.danglerSeeds = new int[this.danglers.Length];
        this.danglerProps = new float[this.danglers.Length, 3];
        this.danglerVals = new Dangler.DanglerProps();

        for (int i = 0; i < this.danglers.Length; i++)
        {
            this.danglerSeeds[i] = UnityEngine.Random.Range(0, int.MaxValue);
            this.danglers[i] = new Dangler(this, i, UnityEngine.Random.Range(4, 12), 5f, 5f);
            this.danglerProps[i, 0] = Mathf.Pow(UnityEngine.Random.value, 0.6f);
            this.danglerProps[i, 1] = Mathf.Lerp(2f, 5f, UnityEngine.Random.value);
            this.danglerProps[i, 2] = UnityEngine.Random.value;
            float num = Mathf.Lerp(4f, 8f, UnityEngine.Random.value) * Mathf.Lerp(0.5f, 1.5f, UnityEngine.Random.value);
            for (int j = 0; j < this.danglers[i].segments.Length; j++)
            {
                this.danglers[i].segments[j].rad = Mathf.Lerp(Mathf.Lerp(1f, 0.5f, UnityEngine.Random.value), 0.5f + Mathf.Sin(Mathf.Pow(UnityEngine.Random.value, 2.5f) * 3.14159274f) * 0.5f, UnityEngine.Random.value) * num;
                this.danglers[i].segments[j].conRad = UnityEngine.Random.Range(0f, 1.3f);
            }
        }

        this.bodyParts = new BodyPart[6 + 2 + 16 + this.fish.iVars.whiskers * 2];

        this.tail = new TailSegment[6];
            for (int j = 0; j < this.tail.Length; j++)
            {
                this.tail[j] = new TailSegment(this, Mathf.Lerp(3f, 1f, (float)j / (float)(this.tail.Length - 1)), 15f, (j != 0) ? this.tail[j - 1] : null, 0.5f, 0.99f, 0.4f, false);
                this.bodyParts[j] = this.tail[j];
            }

        this.wings = new TailSegment[2, 10];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < this.wings.GetLength(1); j++)
            {
                this.wings[i, j] = new TailSegment(this, Mathf.Lerp(3f, 1f, (float)j / (float)(this.wings.GetLength(1) - 1)), 15f, (j != 0) ? this.wings[i, j - 1] : null, 0.5f, 0.99f, 0.4f, false);
                this.bodyParts[8 + (8 * i) + j] = this.wings[i, j];
            }
        }
        
        int seed = UnityEngine.Random.seed;
        UnityEngine.Random.seed = this.fish.iVars.whiskerSeed;
        
        this.whiskers = new GenericBodyPart[2, this.fish.iVars.whiskers];
        this.whiskerDirections = new Vector2[this.fish.iVars.whiskers];
        this.whiskerProps = new float[this.fish.iVars.whiskers, 5];
        for (int k = 0; k < this.fish.iVars.whiskers; k++)
        {
            this.whiskers[0, k] = new GenericBodyPart(this, 1f, 0.6f, 0.9f, this.fish.bodyChunks[2]);
            this.whiskers[1, k] = new GenericBodyPart(this, 1f, 0.6f, 0.9f, this.fish.bodyChunks[2]);
            this.whiskerDirections[k] = Custom.DegToVec(Mathf.Lerp(4f, 80f, UnityEngine.Random.value));
            this.whiskerProps[k, 0] = Mathf.Lerp(7f, 40f, Mathf.Pow(UnityEngine.Random.value, 2f));
            this.whiskerProps[k, 1] = Mathf.Lerp(-1f, 0.5f, UnityEngine.Random.value);
            this.whiskerProps[k, 2] = Mathf.Lerp(5f, 720f, Mathf.Pow(UnityEngine.Random.value, 1.5f)) / this.whiskerProps[k, 0];
            this.whiskerProps[k, 3] = UnityEngine.Random.value;
            this.whiskerProps[k, 4] = Mathf.Lerp(0.6f, 1.6f, Mathf.Pow(UnityEngine.Random.value, 2f));
            this.bodyParts[8 + 16 + k * 2] = this.whiskers[0, k];
            this.bodyParts[8 + 16 + k * 2 + 1] = this.whiskers[1, k];
        }
        
        UnityEngine.Random.seed = seed;

        this.flippers = new GenericBodyPart[2];
        for (int l = 0; l < 2; l++)
        {
            this.flippers[l] = new GenericBodyPart(this, 1f, 0.7f, 0.99f, this.fish.bodyChunks[1]);
            this.bodyParts[6 + l] = this.flippers[l];
        }
    }
    private int BodySprite
    {
        get
        {
            return 0;
        }
    }
    
    private int WhiskerSprite(int side, int whisker)
    {
        return 1 + whisker * 2 + side;
    }
    
    private int TentacleSprite()
    {
        return this.TotalWhiskerSprites + 1;
    }
    
    private int FlipperSprite(int flipper)
    {
        return this.TotalWhiskerSprites + 2 + flipper;
    }
    
    private int TotalSprites
    {
        get
        {
            return this.TotalWhiskerSprites + 9 + headDanglers + neckDanglers + wingDanglers*2;
        }
    }
    
    private int TotalWhiskerSprites
    {
        get
        {
            return this.fish.iVars.whiskers * 2;
        }
    }

    private int NeckSprite
    {
        get
        {
            return TotalWhiskerSprites + 4;
        }
    }

    private int HeadSprite
    {
        get
        {
            return TotalWhiskerSprites + 5;
        }
    }

    private int JawSprite
    {
        get
        {
            return TotalWhiskerSprites + 6;
        }
    }

    private int WingSprite(int wing)
    {
        return TotalWhiskerSprites + 7 + wing;
    }

    private int HeadDanglerSprite(int index)
    {
       return this.TotalWhiskerSprites + 9 + index;
    }

    private int NeckDanglerSprite(int index)
    {
        return this.TotalWhiskerSprites + 9 + headDanglers + index;
    }

    private int WingDanglerSprite(int wing, int index)
    {
        return this.TotalWhiskerSprites + 9 + headDanglers + neckDanglers + wingDanglers * wing + index;
    }

    public override void Reset()
    {
        base.Reset();
        for (int i = 0; i < this.danglers.Length; i++)
        {
            this.danglers[i].Reset();
        }
    }

    public override void Update()
    {
        base.Update();
        if (this.culled)
        {
            return;
        }
        for (int i = 0; i < this.danglers.Length; i++)
        {
            this.danglers[i].Update();
        }
        if (this.fish.Consious)
        {
            this.swim -= Custom.LerpMap(this.fish.swimSpeed, 1.6f, 5f, 0.0333333351f, 0.06666667f);
        }
        this.tail[0].connectedPoint = new Vector2?(this.fish.bodyChunks[1].pos);
        this.wings[0, 0].connectedPoint = new Vector2?(this.fish.bodyChunks[0].pos);
        this.wings[1, 0].connectedPoint = new Vector2?(this.fish.bodyChunks[0].pos);
        Vector2 vector = Custom.DirVec(this.fish.bodyChunks[1].pos, this.fish.bodyChunks[0].pos);
        Vector2 a = Custom.PerpendicularVector(vector);
        this.lastZRotation = this.zRotation;
        if (Mathf.Abs(vector.x) > 0.5f && this.fish.Consious)
        {
            this.zRotation = Vector2.Lerp(this.zRotation, new Vector2((vector.x <= 0f) ? 1f : -1f, 0f), 0.2f);
        }
        else
        {
            this.zRotation = Vector2.Lerp(this.zRotation, -vector, 0.5f);
        }
        this.zRotation = this.zRotation.normalized;
        if (this.fish.Consious)
        {
            float num = 1f - this.fish.bodyChunks[1].submersion;
            if (this.airEyes < num)
            {
                this.airEyes = Mathf.Min(this.airEyes + 0.1f, num);
            }
            else
            {
                this.airEyes = Mathf.Max(this.airEyes - 0.0333333351f, num);
            }
        }
        for (int i = 0; i < 2; i++)
        {
            this.flippers[i].Update();
            this.flippers[i].ConnectToPoint(this.fish.bodyChunks[1].pos, (this.flipperGraphWidth + 7f) * this.fish.iVars.flipperSize, false, 0f, this.fish.bodyChunks[1].vel, 0.3f, 0f);
            Vector2 vector2 = a * this.zRotation.y * ((i != 0) ? 1f : -1f);
            vector2 += new Vector2(0f, -0.5f) * Mathf.Abs(this.zRotation.x);
            vector2 += vector * this.fish.iVars.flipperOrientation * 1.5f;
            if (this.fish.Consious)
            {
                if (i == 0 == this.zRotation.x < 0f)
                {
                    vector2 += vector * Mathf.Sin(this.swim * 3.14159274f * 2f) * 0.3f * (1f - this.fish.jetActive);
                }
                else
                {
                    vector2 += vector * Mathf.Cos(this.swim * 3.14159274f * 2f) * 0.3f * (1f - this.fish.jetActive);
                }
                vector2 = Vector2.Lerp(vector2, -vector, this.fish.jetActive * this.fish.jetWater);
            }
            this.flippers[i].vel += (this.fish.bodyChunks[1].pos + vector2 * (this.flipperGraphWidth + 7f) * this.fish.iVars.flipperSize - this.flippers[i].pos) / ((!this.fish.Consious) ? 16f : 8f);
            if (this.fish.room.PointSubmerged(this.flippers[i].pos))
            {
                this.flippers[i].vel *= 0.9f;
            }
            else
            {
                GenericBodyPart genericBodyPart = this.flippers[i];
                genericBodyPart.vel.y = genericBodyPart.vel.y - 0.6f;
            }
            if (this.fish.iVars.whiskers > 0)
            {
                for (int j = 0; j < this.fish.iVars.whiskers; j++)
                {
                    this.whiskers[i, j].vel += this.whiskerDir(i, j, this.zRotation, vector) * this.whiskerProps[j, 2];
                    if (this.fish.room.PointSubmerged(this.whiskers[i, j].pos))
                    {
                        this.whiskers[i, j].vel *= 0.8f;
                    }
                    else
                    {
                        GenericBodyPart genericBodyPart2 = this.whiskers[i, j];
                        genericBodyPart2.vel.y = genericBodyPart2.vel.y - 0.6f;
                    }
                    this.whiskers[i, j].Update();
                    this.whiskers[i, j].ConnectToPoint(this.fish.bodyChunks[2].pos - vector * 5f + this.whiskerDir(i, j, this.zRotation, vector) * 5f, this.whiskerProps[j, 0], false, 0f, this.fish.bodyChunks[2].vel, 0f, 0f);
                }
            }
            
            for (int x = 0; x < 2; x++)
            {
                for (int k = 0; k < this.wings.GetLength(1); k++)
                {
                    
                    Vector2 look = Custom.PerpendicularVector(Custom.DirVec(this.fish.bodyChunks[1].pos, this.fish.bodyChunks[0].pos));
                    if (x == 0)
                    {
                        look = -look;
                    }
                    float push = Custom.AimFromOneVectorToAnother(this.fish.bodyChunks[0].pos + look, this.fish.bodyChunks[0].pos);
                    this.wings[x, k].Update();
                    float num2 = Mathf.InverseLerp(0f, (float)(this.wings.GetLength(1) - 1), (float)k);
                    if (!Custom.DistLess(this.wings[x, k].pos, this.fish.bodyChunks[1].pos, 15f * (float)(k + 1)))
                    {
                        this.wings[x, k].pos = this.fish.bodyChunks[1].pos + Custom.DirVec(this.fish.bodyChunks[1].pos, this.wings[x, k].pos) * 15f * (float)(k + 1);
                    }
                    float num3 = this.fish.jetActive;
                    if (this.fish.room.PointSubmerged(this.wings[x, k].pos))
                    {
                        this.wings[x, k].vel *= 0.7f;
                        num3 = Mathf.Lerp(num3, 0f, 0.5f);
                    }
                    else
                    {
                        TailSegment wingSegment = this.wings[x, k];
                        wingSegment.vel.y = wingSegment.vel.y - 0.9f * Mathf.Pow((float)k / (float)(this.wings.GetLength(1) - 1), 3f);
                    }
                    this.wings[x, k].vel += a * Mathf.Sin((this.swim + (float)k / 5f) * 3.14159274f * 2f) * ((i != 0) ? -1f : 1f) * Mathf.Pow(1f - num2, 2f) * Custom.LerpMap(this.fish.swimSpeed, 1.6f, 5f, 8f, 16f) * (1f - num3);
                    this.wings[x, k].vel -= look * (0.2f * (1f - num3) + Mathf.Pow(Mathf.InverseLerp(0.5f, 0f, num2), 2f) * Mathf.Lerp(27f, 11f, num3));
                    float num4 = 30f + Mathf.Sin(Mathf.Pow(num2, 1f) * 3.14159274f * -2f) * -100f;
                    this.wings[x, k].vel -= Custom.DegToVec(push + num4 * ((i != 0) ? -1f : 1f)) * Mathf.Lerp(12f, 6f, num2) * num3;
                    this.wings[x, k].connectionRad = Mathf.Lerp(10f, 0.5f, Mathf.Lerp(0f, num3, Mathf.Pow(num2, 0.2f))) * Mathf.Lerp(0.5f, 1.5f, this.fish.iVars.tentacleLength);
                    this.wings[x, k].rad = Mathf.Lerp(this.TentacleContour(num2, k), Mathf.Lerp(8f, 2f, num2) * (0.5f + 0.5f * this.fish.jetWater), num3) * Mathf.Lerp(0.7f, 1.2f, this.fish.iVars.tentacleFatness);
                }
            }
            for (int k = 0; k < this.tail.Length; k++)
            {
                this.tail[k].Update();
                float num2 = Mathf.InverseLerp(0f, (float)(this.tail.Length - 1), (float)k);
                if (!Custom.DistLess(this.tail[k].pos, this.fish.bodyChunks[1].pos, 15f * (float)(k + 1)))
                {
                    this.tail[k].pos = this.fish.bodyChunks[1].pos + Custom.DirVec(this.fish.bodyChunks[1].pos, this.tail[k].pos) * 15f * (float)(k + 1);
                }
                float num3 = this.fish.jetActive;
                if (this.fish.room.PointSubmerged(this.tail[k].pos))
                {
                    this.tail[k].vel *= 0.7f;
                    num3 = Mathf.Lerp(num3, 0f, 0.5f);
                }
                else
                {
                    TailSegment tailSegment = this.tail[k];
                    tailSegment.vel.y = tailSegment.vel.y - 0.9f * Mathf.Pow((float)k / (float)(this.tail.Length - 1), 3f);
                }
                this.tail[k].vel += a * Mathf.Sin((this.swim + (float)k / 5f) * 3.14159274f * 2f) * ((i != 0) ? -1f : 1f) * Mathf.Pow(1f - num2, 2f) * Custom.LerpMap(this.fish.swimSpeed, 1.6f, 5f, 8f, 16f) * (1f - num3);
                this.tail[k].vel -= vector * (0.2f * (1f - num3) + Mathf.Pow(Mathf.InverseLerp(0.5f, 0f, num2), 2f) * Mathf.Lerp(27f, 11f, num3));
                float num4 = 30f + Mathf.Sin(Mathf.Pow(num2, 1f) * 3.14159274f * -2f) * -100f;
                this.tail[k].vel -= Custom.DegToVec(Custom.AimFromOneVectorToAnother(this.fish.bodyChunks[1].pos, this.fish.bodyChunks[0].pos) + num4 * ((i != 0) ? -1f : 1f)) * Mathf.Lerp(12f, 6f, num2) * num3;
                this.tail[k].connectionRad = Mathf.Lerp(10f, 0.5f, Mathf.Lerp(0f, num3, Mathf.Pow(num2, 0.2f))) * Mathf.Lerp(0.5f, 1.5f, this.fish.iVars.tentacleLength);
                this.tail[k].rad = Mathf.Lerp(this.TentacleContour(num2, k), Mathf.Lerp(8f, 2f, num2) * (0.5f + 0.5f * this.fish.jetWater), num3) * Mathf.Lerp(0.7f, 1.2f, this.fish.iVars.tentacleFatness);
            }
        }
    }

    // Token: 0x06001E25 RID: 7717 RVA: 0x001BDDA8 File Offset: 0x001BBFA8
    private Vector2 whiskerDir(int side, int m, Vector2 zRot, Vector2 bodyDir)
    {
        Vector2 vector = new Vector2(((side != 0) ? 1f : -1f) * Mathf.Abs(zRot.y) * this.whiskerDirections[m].x + zRot.x * this.whiskerProps[m, 1], this.whiskerDirections[m].y);
        return Custom.RotateAroundOrigo(vector.normalized, Custom.VecToDeg(bodyDir));
    }

    // Token: 0x06001E26 RID: 7718 RVA: 0x001BDE2C File Offset: 0x001BC02C
    private float TentacleContour(float f, int i)
    {
        int tentacleContour = this.fish.iVars.tentacleContour;
        if (tentacleContour == 0)
        {
            return Mathf.Lerp(5f, 1f, Mathf.Pow(f, 0.5f));
        }
        if (tentacleContour != 1)
        {
            return Mathf.Lerp(4f, 1f, Mathf.Pow(f, 1.5f));
        }
        if (i == this.tail.Length - 2)
        {
            return 3.5f;
        }
        return Mathf.Lerp(4f, 1f, Mathf.Pow(f, 0.5f));
    }

    // Token: 0x06001E27 RID: 7719 RVA: 0x001BDEC8 File Offset: 0x001BC0C8
    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[this.TotalSprites];
        sLeaser.sprites[this.BodySprite] = new FSprite("pixel", true);
        sLeaser.sprites[this.BodySprite].scale = Mathf.Lerp(0.4f, 0.7f, this.fish.iVars.fatness);
        sLeaser.sprites[this.HeadSprite] = new FSprite("pixel", true);
        sLeaser.sprites[this.HeadSprite].scale = 0.6f;
        sLeaser.sprites[this.JawSprite] = new FSprite("pixel", true);
        sLeaser.sprites[this.JawSprite].scale = 0.7f;
        sLeaser.sprites[NeckSprite] = TriangleMesh.MakeLongMesh(fish.neck.tChunks.Length, false, false);
        (sLeaser.sprites[NeckSprite] as TriangleMesh).alpha = 1f;
        for (int i = 0; i < this.danglers.Length; i++)
        {
            this.danglers[i].InitSprite(sLeaser, HeadDanglerSprite(i));
            sLeaser.sprites[HeadDanglerSprite(i)].shader = rCam.room.game.rainWorld.Shaders["TentaclePlant"];
            sLeaser.sprites[HeadDanglerSprite(i)].alpha = Mathf.InverseLerp(2f, 12f, (float)this.danglers[i].segments.Length) * 0.9f + 0.1f * this.danglerProps[i, 2];
        }
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < this.fish.iVars.whiskers; j++)
            {
                sLeaser.sprites[this.WhiskerSprite(i, j)] = TriangleMesh.MakeLongMesh(4, true, false);
            }
        }
        sLeaser.sprites[this.TentacleSprite()] = TriangleMesh.MakeLongMesh(this.tail.Length, true, false);
        sLeaser.sprites[this.WingSprite(0)] = TriangleMesh.MakeLongMesh(10, true, false);
        sLeaser.sprites[this.WingSprite(1)] = TriangleMesh.MakeLongMesh(10, true, false);
        for (int k = 0; k < 2; k++)
        {
            sLeaser.sprites[this.FlipperSprite(k)] = new FSprite("JetFishFlipper" + this.fish.iVars.flipper, true);
            this.flipperGraphWidth = Futile.atlasManager.GetElementWithName("JetFishFlipper" + this.fish.iVars.flipper).sourcePixelSize.x;
            sLeaser.sprites[this.FlipperSprite(k)].alpha = 0f;
            sLeaser.sprites[this.FlipperSprite(k)].anchorX = 0f;
            sLeaser.sprites[this.FlipperSprite(k)].anchorY = SeaDrakeGraphics.flipperGraphConPointHeights[this.fish.iVars.flipper] / Futile.atlasManager.GetElementWithName("JetFishFlipper" + this.fish.iVars.flipper).sourcePixelSize.y;
        }
        this.AddToContainer(sLeaser, rCam, null);
        base.InitiateSprites(sLeaser, rCam);
    }

    // Token: 0x06001E28 RID: 7720 RVA: 0x001BE108 File Offset: 0x001BC308
    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        Color color = palette.blackColor;
        sLeaser.sprites[this.BodySprite].color = color;
        sLeaser.sprites[this.HeadSprite].color = color;
        sLeaser.sprites[this.JawSprite].color = color;
        sLeaser.sprites[this.TentacleSprite()].color = color;
        sLeaser.sprites[this.WingSprite(0)].color = color;
        sLeaser.sprites[this.WingSprite(1)].color = color;
        sLeaser.sprites[NeckSprite].color = color;
        for (int i = 0; i < this.danglers.Length; i++)
        {
            Color a;
            a = Custom.HSL2RGB(0.5f, 0f, Mathf.Lerp(0f, 0.5f, this.danglerProps[i, 0]));
            sLeaser.sprites[this.HeadDanglerSprite(i)].color = Color.Lerp(a, palette.blackColor, rCam.room.Darkness(this.fish.mainBodyChunk.pos));
        }
        for (int i = 0; i < 2; i++)
        {
            sLeaser.sprites[this.FlipperSprite(i)].color = color;
            for (int k = 0; k < this.fish.iVars.whiskers; k++)
            {
                    sLeaser.sprites[this.WhiskerSprite(i, k)].color = color;
            }
        }
    }

    // Token: 0x06001E29 RID: 7721 RVA: 0x001BE334 File Offset: 0x001BC534
    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        base.AddToContainer(sLeaser, rCam, newContatiner);
    }

    // Token: 0x06001E2A RID: 7722 RVA: 0x001BE340 File Offset: 0x001BC540
    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        if (this.culled)
        {
            return;
        }
        for (int i = 0; i < this.danglers.Length; i++)
        {
            this.danglers[i].DrawSprite(HeadDanglerSprite(i), sLeaser, rCam, timeStacker, camPos);
        }
        Vector2 vector23 = Vector2.Lerp(this.fish.bodyChunks[2].lastPos, this.fish.bodyChunks[2].pos, timeStacker);
        Vector2 vector24 = Vector2.Lerp(this.fish.mainBodyChunk.lastPos, this.fish.mainBodyChunk.pos, timeStacker);
        Vector2 a2 = Custom.DirVec(Vector2.Lerp(this.fish.bodyChunks[1].lastPos, this.fish.bodyChunks[1].pos, timeStacker), Vector2.Lerp(this.fish.bodyChunks[0].lastPos, this.fish.bodyChunks[0].pos, timeStacker)); ;
        Vector2 vector25 = Vector2.Lerp(this.fish.neck.connectedChunk.lastPos, this.fish.neck.connectedChunk.pos, timeStacker);
        float thick = 2f;
        float num23 = 3f;
        for (int l = 0; l < this.fish.neck.tChunks.Length; l++)
        {
            Vector2 vector6 = Vector2.Lerp(this.fish.neck.tChunks[l].lastPos, this.fish.neck.tChunks[l].pos, timeStacker);
            if (l == this.fish.neck.tChunks.Length - 1)
            {
                vector6 = Vector2.Lerp(vector6, vector23, 0.5f);
                thick = 1f;
            }
            else if (l == 0)
            {
                vector6 = Vector2.Lerp(vector6, vector24 + a2 * 40f, 0.3f);
            }
            Vector2 normalized1 = (vector6 - vector25).normalized;
            Vector2 a3 = Custom.PerpendicularVector(normalized1);
            float d = Vector2.Distance(vector6, vector25) / 5f;
            (sLeaser.sprites[this.NeckSprite] as TriangleMesh).MoveVertice(l * 4, vector25 - a3 * (this.fish.neck.tChunks[l].stretchedRad + num23) * 0.5f * thick + normalized1 * d * ((l != 0) ? 1f : 0f) - camPos);
            (sLeaser.sprites[this.NeckSprite] as TriangleMesh).MoveVertice(l * 4 + 1, vector25 + a3 * (this.fish.neck.tChunks[l].stretchedRad + num23) * 0.5f * thick + normalized1 * d * ((l != 0) ? 1f : 0f) - camPos);
            if (l == this.fish.neck.tChunks.Length - 1)
            {
                thick = 0.2f;
            }
            (sLeaser.sprites[this.NeckSprite] as TriangleMesh).MoveVertice(l * 4 + 2, vector6 - a3 * this.fish.neck.tChunks[l].stretchedRad *thick  - normalized1 * d * ((l != this.fish.neck.tChunks.Length - 1) ? 1f : 0f) - camPos);
            (sLeaser.sprites[this.NeckSprite] as TriangleMesh).MoveVertice(l * 4 + 3, vector6 + a3 * this.fish.neck.tChunks[l].stretchedRad *thick  - normalized1 * d * ((l != this.fish.neck.tChunks.Length - 1) ? 1f : 0f) - camPos);
            num23 = this.fish.neck.tChunks[l].stretchedRad;
            vector25 = vector6;
        }

        Vector2 vector = Vector3.Slerp(this.lastZRotation, this.zRotation, timeStacker);
        Vector2 vector2 = Vector2.Lerp(Vector2.Lerp(this.fish.bodyChunks[0].lastPos, this.fish.bodyChunks[0].pos, timeStacker), Vector2.Lerp(this.fish.bodyChunks[1].lastPos, this.fish.bodyChunks[1].pos, timeStacker), 0.3f);
        Vector2 normalized = (Vector2.Lerp(this.fish.bodyChunks[0].lastPos, this.fish.bodyChunks[0].pos, timeStacker) - Vector2.Lerp(this.fish.bodyChunks[1].lastPos, this.fish.bodyChunks[1].pos, timeStacker)).normalized;
        Vector2 a = Custom.PerpendicularVector(-normalized);
        float num = Custom.AimFromOneVectorToAnother(normalized, -normalized);
        float num2 = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), vector);
        int num3 = Custom.IntClamp(8 - (int)(Mathf.Abs(num2 / 180f) * 9f), 0, 8);
        float num4 = (float)(8 - num3) * Mathf.Sign(num2) * 22.5f;
        //float wingLength = Mathf.Abs(Mathf.Clamp(8f - (Mathf.Abs(Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), Vector3.Slerp(this.lastZRotation, this.zRotation, timeStacker)) / 180f) * 9f), 0f, 8f) - 4f) / 4f;
        sLeaser.sprites[this.BodySprite].x = vector2.x - camPos.x;
        sLeaser.sprites[this.BodySprite].y = vector2.y - camPos.y;
        sLeaser.sprites[this.BodySprite].element = Futile.atlasManager.GetElementWithName("SeaDrake" + num3);
        sLeaser.sprites[this.BodySprite].rotation = num - num4;
        sLeaser.sprites[this.BodySprite].scaleX = ((num2 <= 0f) ? Mathf.Lerp(0.3f, 0.6f, this.fish.iVars.fatness) : -Mathf.Lerp(0.3f, 0.6f, this.fish.iVars.fatness));

        //vector = Custom.DirVec(Vector2.Lerp(this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 3].lastPos, this.fish.neck.tChunks[this.fish.neck.tChunks.Length-3].pos, timeStacker), Vector2.Lerp(this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].lastPos, this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].pos, timeStacker));
        num2 = Custom.VecToDeg(-vector);
        num3 = Custom.IntClamp(8 - (int)(Mathf.Abs(num2 / 180f) * 9f), 0, 8);
        num4 = (float)(8 - num3) * Mathf.Sign(num2) * 22.5f;

        sLeaser.sprites[this.HeadSprite].x = Vector2.Lerp(this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].lastPos, this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].pos, timeStacker).x - camPos.x;
        sLeaser.sprites[this.HeadSprite].y = Vector2.Lerp(this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].lastPos, this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].pos, timeStacker).y - camPos.y;
        sLeaser.sprites[this.HeadSprite].element = Futile.atlasManager.GetElementWithName("SeaDrakeHead" + num3);
        sLeaser.sprites[this.HeadSprite].rotation = num2 - num4;
        sLeaser.sprites[this.HeadSprite].scaleX = ((num2 <= 0f) ? 0.6f : -0.6f);
        
        num2 = Custom.VecToDeg(-vector);
        num3 = Custom.IntClamp(8 - (int)(Mathf.Abs(num2 / 180f) * 9f), 0, 8);
        num4 = (float)(8 - num3) * Mathf.Sign(num2) * 22.5f;

        sLeaser.sprites[this.JawSprite].x = Vector2.Lerp(this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].lastPos, this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].pos, timeStacker).x - camPos.x + Custom.DegToVec(num2).x * 10f;
        sLeaser.sprites[this.JawSprite].y = Vector2.Lerp(this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].lastPos, this.fish.neck.tChunks[this.fish.neck.tChunks.Length - 2].pos, timeStacker).y - camPos.y + Custom.DegToVec(num2).y * 10f;
        sLeaser.sprites[this.JawSprite].element = Futile.atlasManager.GetElementWithName("SeaDrakeJaw" + num3);
        sLeaser.sprites[this.JawSprite].rotation = num2 - num4;
        sLeaser.sprites[this.JawSprite].scaleX = ((num2 <= 0f) ? 0.7f : -0.7f);

        vector = Vector3.Slerp(this.lastZRotation, this.zRotation, timeStacker);
        vector2 = Vector2.Lerp(Vector2.Lerp(this.fish.bodyChunks[0].lastPos, this.fish.bodyChunks[0].pos, timeStacker), Vector2.Lerp(this.fish.bodyChunks[1].lastPos, this.fish.bodyChunks[1].pos, timeStacker), 0.3f);
        normalized = (Vector2.Lerp(this.fish.bodyChunks[0].lastPos, this.fish.bodyChunks[0].pos, timeStacker) - Vector2.Lerp(this.fish.bodyChunks[1].lastPos, this.fish.bodyChunks[1].pos, timeStacker)).normalized;
        a = Custom.PerpendicularVector(-normalized);
        num = Custom.AimFromOneVectorToAnother(normalized, -normalized);
        num2 = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), vector);
        num3 = Custom.IntClamp(8 - (int)(Mathf.Abs(num2 / 180f) * 9f), 0, 8);
        num4 = (float)(8 - num3) * Mathf.Sign(num2) * 22.5f;

        Vector2 vector33 = Vector2.Lerp(this.fish.bodyChunks[1].lastPos, this.fish.bodyChunks[1].pos, timeStacker);
        float num55 = this.fish.bodyChunks[1].rad/2f;
        for (int j = 0; j < this.tail.Length; j++)
        {
            Vector2 vector4 = Vector2.Lerp(this.tail[j].lastPos, this.tail[j].pos, timeStacker);
            Vector2 normalized2 = (vector4 - vector33).normalized;
            Vector2 a3 = Custom.PerpendicularVector(normalized2);
            float d = Vector2.Distance(vector4, vector33) / 5f;
            (sLeaser.sprites[this.TentacleSprite()] as TriangleMesh).MoveVertice(j * 4, vector33 - a3 * 2f * (num55 + this.tail[j].StretchedRad) * 0.5f + normalized2 * d - camPos);
            (sLeaser.sprites[this.TentacleSprite()] as TriangleMesh).MoveVertice(j * 4 + 1, vector33 + a3 * 2f * (num55 + this.tail[j].StretchedRad) * 0.5f + normalized2 * d - camPos);
            if (j < this.tail.Length - 1)
            {
                (sLeaser.sprites[this.TentacleSprite()] as TriangleMesh).MoveVertice(j * 4 + 2, vector4 - a3 * 2f * this.tail[j].StretchedRad - normalized2 * d - camPos);
                (sLeaser.sprites[this.TentacleSprite()] as TriangleMesh).MoveVertice(j * 4 + 3, vector4 + a3 * 2f * this.tail[j].StretchedRad - normalized2 * d - camPos);
            }
            else
            {
                (sLeaser.sprites[this.TentacleSprite()] as TriangleMesh).MoveVertice(j * 4 + 2, vector4 - camPos);
            }
            num55 = this.tail[j].StretchedRad;
            vector33 = vector4;
        }

        float wingLength = Mathf.Abs(Mathf.Clamp(8f - (Mathf.Abs(Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), this.zRotation) / 180f) * 9f), 0f, 8f) - 4f) / 4f;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < this.wings.GetLength(1); j++)
            {

                vector33 = Vector2.Lerp(this.fish.bodyChunks[0].lastPos, this.fish.bodyChunks[0].pos, timeStacker);
                num55 = this.fish.bodyChunks[0].rad;
                Vector2 vector4 = Vector2.Lerp(this.wings[i,j].lastPos, this.wings[i, j].pos, timeStacker);
                Vector2 normalized2 = (vector4 - vector33).normalized;
                Vector2 a3 = Custom.PerpendicularVector(normalized2);
                float d = Vector2.Distance(vector4, vector33) / 5f;
                (sLeaser.sprites[this.WingSprite(i)] as TriangleMesh).MoveVertice(j * 4, vector33 - a3 * (num55 + this.wings[i, j].StretchedRad) * 0.5f + normalized2 * d - camPos);
                (sLeaser.sprites[this.WingSprite(i)] as TriangleMesh).MoveVertice(j * 4 + 1, vector33 + a3 * (num55 + this.wings[i, j].StretchedRad) * 0.5f + normalized2 * d - camPos);
                //(sLeaser.sprites[this.WingSprite(i)] as TriangleMesh).MoveVertice(j * 4 + 1, Vector2.Lerp(this.fish.bodyChunks[0].lastPos, this.fish.bodyChunks[0].pos, timeStacker) - camPos);
                if (j < this.wings.GetLength(1) - 1)
                {
                    (sLeaser.sprites[this.WingSprite(i)] as TriangleMesh).MoveVertice(j * 4 + 2, vector4 - a3 * this.wings[i, j].StretchedRad - normalized2 * d - camPos);
                    (sLeaser.sprites[this.WingSprite(i)] as TriangleMesh).MoveVertice(j * 4 + 3, vector4 + a3 * this.wings[i, j].StretchedRad - normalized2 * d - camPos);
                    //(sLeaser.sprites[this.WingSprite(i)] as TriangleMesh).MoveVertice(j * 4 + 3, Vector2.Lerp(this.fish.bodyChunks[1].lastPos, this.fish.bodyChunks[1].pos, timeStacker) - camPos);
                }
                else
                {
                    (sLeaser.sprites[this.WingSprite(i)] as TriangleMesh).MoveVertice(j * 4 + 2, vector4 - camPos);
                }
                //(sLeaser.sprites[this.WingSprite(i)] as TriangleMesh).scale = wingLength;
                num55 = this.wings[i,j].StretchedRad;
                vector33 = vector4;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            //Vector2 a2 = vector2 + normalized * 13f + a * Mathf.Cos(num2 / 360f * 3.14159274f * 2f) * Mathf.Lerp(7.5f, 5f, this.airEyes) * ((i == 0 != num2 > 0f) ? 1f : -1f);
            sLeaser.sprites[this.FlipperSprite(i)].x = Mathf.Lerp(this.flippers[i].lastPos.x, this.flippers[i].pos.x, timeStacker) - camPos.x;
            sLeaser.sprites[this.FlipperSprite(i)].y = Mathf.Lerp(this.flippers[i].lastPos.y, this.flippers[i].pos.y, timeStacker) - camPos.y;
            sLeaser.sprites[this.FlipperSprite(i)].rotation = Custom.AimFromOneVectorToAnother(Vector2.Lerp(this.flippers[i].lastPos, this.flippers[i].pos, timeStacker), vector2) - 90f;
            sLeaser.sprites[this.FlipperSprite(i)].scaleY = Mathf.Sign(Custom.DistanceToLine(Vector2.Lerp(this.flippers[i].lastPos, this.flippers[i].pos, timeStacker), vector2 - normalized, vector2 + normalized)) * this.fish.iVars.flipperSize;
            sLeaser.sprites[this.FlipperSprite(i)].scaleX = Mathf.Lerp(Vector2.Distance(Vector2.Lerp(this.flippers[i].lastPos, this.flippers[i].pos, timeStacker), vector2) / this.flipperGraphWidth, this.fish.iVars.flipperSize, 0.5f);
            Vector2 vector3 = Vector2.Lerp(this.fish.bodyChunks[1].lastPos, this.fish.bodyChunks[1].pos, timeStacker);
            float num5 = this.fish.bodyChunks[1].rad;
            for (int k = 0; k < this.fish.iVars.whiskers; k++)
            {
                for (int l = 0; l < 2; l++)
                {
                    Vector2 vector5 = Vector2.Lerp(this.whiskers[l, k].lastPos, this.whiskers[l, k].pos, timeStacker);
                    Vector2 a4 = this.whiskerDir(l, k, vector, normalized);
                    Vector2 vector6 = Vector2.Lerp(this.fish.bodyChunks[2].lastPos, this.fish.bodyChunks[2].pos, timeStacker) + normalized * Mathf.Lerp(10f, 5f, this.whiskerProps[k, 3]) + a4 * 5f * this.whiskerProps[k, 3];
                    a4 = (a4 + normalized).normalized;
                    vector3 = vector6;
                    num5 = this.whiskerProps[k, 4];
                    float num6 = 1f;
                    for (int m = 0; m < 4; m++)
                    {
                        Vector2 vector7;
                        if (m < 3)
                        {
                            vector7 = Vector2.Lerp(vector6, vector5, (float)(m + 1) / 4f);
                            vector7 += a4 * num6 * this.whiskerProps[k, 0] * 0.2f;
                        }
                        else
                        {
                            vector7 = vector5;
                        }
                        num6 *= 0.7f;
                        Vector2 normalized3 = (vector7 - vector3).normalized;
                        Vector2 a5 = Custom.PerpendicularVector(normalized3);
                        float d2 = Vector2.Distance(vector7, vector3) / ((m != 0) ? 5f : 1f);
                        float num7 = Custom.LerpMap((float)m, 0f, 3f, this.whiskerProps[k, 4], 0.5f);
                        (sLeaser.sprites[this.WhiskerSprite(l, k)] as TriangleMesh).MoveVertice(m * 4, vector3 - a5 * (num7 + num5) * 0.5f + normalized3 * d2 - camPos);
                        (sLeaser.sprites[this.WhiskerSprite(l, k)] as TriangleMesh).MoveVertice(m * 4 + 1, vector3 + a5 * (num7 + num5) * 0.5f + normalized3 * d2 - camPos);
                        if (m < 3)
                        {
                            (sLeaser.sprites[this.WhiskerSprite(l, k)] as TriangleMesh).MoveVertice(m * 4 + 2, vector7 - a5 * num7 - normalized3 * d2 - camPos);
                            (sLeaser.sprites[this.WhiskerSprite(l, k)] as TriangleMesh).MoveVertice(m * 4 + 3, vector7 + a5 * num7 - normalized3 * d2 - camPos);
                        }
                        else
                        {
                            (sLeaser.sprites[this.WhiskerSprite(l, k)] as TriangleMesh).MoveVertice(m * 4 + 2, vector7 + normalized3 * 2.1f - camPos);
                        }
                        num5 = num7;
                        vector3 = vector7;
                    }
                }
            }
        }
    }

    public Vector2 DanglerConnection(int index, float timeStacker)
    {
        int seed = UnityEngine.Random.seed;
        UnityEngine.Random.seed = danglerSeeds[index];
        Vector2 value = new Vector2(0f, 0f);
        Vector2 randomDir = Custom.DegToVec(UnityEngine.Random.Range(0f, 360f));
        if (index < this.headDanglers)
        {
            value = Vector2.Lerp(this.fish.bodyChunks[2].lastPos, this.fish.bodyChunks[2].pos, timeStacker) + randomDir * this.fish.bodyChunks[2].rad;
        }
        else if (index < this.neckDanglers+this.headDanglers)
        {
            int loc = (int)UnityEngine.Random.Range(0f, 2f);
            if (loc == 0)
            {
                value = Vector2.Lerp(this.fish.neck.tChunks[loc].lastPos, this.fish.neck.tChunks[loc].pos, timeStacker) + UnityEngine.Random.Range(0f, 5f) * Custom.DirVec(this.fish.neck.tChunks[loc].pos, this.fish.neck.tChunks[loc + 1].pos);
            }
            else
            {
                value = Vector2.Lerp(this.fish.neck.tChunks[loc].lastPos, this.fish.neck.tChunks[loc].pos, timeStacker) + UnityEngine.Random.Range(0f, 5f) * Custom.DirVec(this.fish.neck.tChunks[loc].pos, this.fish.neck.tChunks[loc - 1].pos);
            }
        }
        else if (index - this.neckDanglers + this.headDanglers < this.wingDanglers )
        {
            int loc = (int)UnityEngine.Random.Range(0f, 9f);
            if (loc == 0)
            {
                value = Vector2.Lerp(this.wings[0, loc].lastPos, this.wings[0, loc].pos, timeStacker) + UnityEngine.Random.Range(0f, 5f) * Custom.DirVec(this.wings[0, loc].pos, this.wings[0, loc + 1].pos);
            }
            else
            {
                value = Vector2.Lerp(this.wings[0, loc].lastPos, this.wings[0, loc].pos, timeStacker) + UnityEngine.Random.Range(0f, 5f) * Custom.DirVec(this.wings[0, loc].pos, this.wings[0, loc - 1].pos);
            }
            value = Vector2.Lerp(value, Vector2.Lerp(this.fish.bodyChunks[0].lastLastPos, this.fish.bodyChunks[0].pos, timeStacker), 0.3f);
        }
        else
        {
            int loc = (int)UnityEngine.Random.Range(0f, 9f);
            if (loc == 0)
            {
                value = Vector2.Lerp(this.wings[1, loc].lastPos, this.wings[1, loc].pos, timeStacker) + UnityEngine.Random.Range(0f, 5f) * Custom.DirVec(this.wings[1, loc].pos, this.wings[1, loc + 1].pos);
            }
            else
            {
                value = Vector2.Lerp(this.wings[1, loc].lastPos, this.wings[1, loc].pos, timeStacker) + UnityEngine.Random.Range(0f, 5f) * Custom.DirVec(this.wings[1, loc].pos, this.wings[1, loc - 1].pos);
            }
            value = Vector2.Lerp(value, Vector2.Lerp(this.fish.bodyChunks[0].lastLastPos, this.fish.bodyChunks[0].pos, timeStacker), 0.3f);
        }
        UnityEngine.Random.seed = seed;
        return value;
    }

    // Token: 0x060028D1 RID: 10449 RVA: 0x002A04D8 File Offset: 0x0029E6D8
    public Dangler.DanglerProps Props(int index)
    {
        return this.danglerVals;
    }

    public Dangler[] danglers;

    public float[,] danglerProps;

    public int[] danglerSeeds;
    
    public Dangler.DanglerProps danglerVals;

    public int headDanglers;

    public int neckDanglers;

    public int wingDanglers;

    // Token: 0x040020B6 RID: 8374
    private SeaDrake fish;

    // Token: 0x040020B7 RID: 8375
    public TailSegment[] tail;

    public TailSegment[,] wings;

    // Token: 0x040020B8 RID: 8376
    public GenericBodyPart[] flippers;

    // Token: 0x040020B9 RID: 8377
    public GenericBodyPart[,] whiskers;

    // Token: 0x040020BA RID: 8378
    public Vector2[] whiskerDirections;

    // Token: 0x040020BB RID: 8379
    public float[,] whiskerProps;

    // Token: 0x040020BC RID: 8380
    public float swim;

    // Token: 0x040020BD RID: 8381
    public Vector2 zRotation;

    // Token: 0x040020BE RID: 8382
    private Vector2 lastZRotation;

    // Token: 0x040020BF RID: 8383
    private float airEyes;

    // Token: 0x040020C0 RID: 8384
    private static float[] flipperGraphConPointHeights = new float[]
    {
        10f,
        10f,
        11f,
        17f,
        9f
    };

    // Token: 0x040020C1 RID: 8385
    private float flipperGraphWidth;
}
