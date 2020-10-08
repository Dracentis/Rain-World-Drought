using MonoMod;
using UnityEngine;
using RWCustom;
using System;
using System.Runtime.CompilerServices;


class patch_OracleGraphics : OracleGraphics
{
    [MonoModIgnore]
    public patch_OracleGraphics(PhysicalObject ow) : base(ow)
    {
    }

    [MonoModIgnore]
    private Color SLArmBaseColA;

    [MonoModIgnore]
    private Color SLArmBaseColB;

    [MonoModIgnore]
    private Color SLArmHighLightColA;

    [MonoModIgnore]
    private Color SLArmHighLightColB;

    [MonoModIgnore]
    private float breathFac;

    [MonoModIgnore]
    private float lastBreatheFac;

    [MonoModIgnore]
    public Vector2 lookDir;

    [MonoModIgnore]
    private Vector2 lastLookDir;

    public extern void orig_ctor(PhysicalObject ow);

    [MonoModConstructor]
    public void ctor(PhysicalObject ow)
    {
        orig_ctor(ow);
        if (IsMoon)
        {
            totalSprites = 0;
            armJointGraphics = new OracleGraphics.ArmJointGraphics[oracle.arm.joints.Length];
            for (int i = 0; i < oracle.arm.joints.Length; i++)
            {
                armJointGraphics[i] = new OracleGraphics.ArmJointGraphics(this, oracle.arm.joints[i], totalSprites);
                totalSprites += armJointGraphics[i].totalSprites;
            }
            firstUmbilicalSprite = totalSprites;
            umbCord = new OracleGraphics.UbilicalCord(this, totalSprites);
            totalSprites += umbCord.totalSprites;
            discUmbCord = null;
            firstBodyChunkSprite = totalSprites;
            totalSprites += 2;
            neckSprite = totalSprites;
            totalSprites++;
            firstFootSprite = totalSprites;
            totalSprites += 4;
            halo = new OracleGraphics.Halo(this, totalSprites);
            totalSprites += halo.totalSprites;
            gown = new OracleGraphics.Gown(this);
            robeSprite = totalSprites;
            totalSprites++;
            firstHandSprite = totalSprites;
            totalSprites += 4;
            head = new GenericBodyPart(this, 5f, 0.5f, 0.995f, oracle.firstChunk);
            firstHeadSprite = totalSprites;
            totalSprites += 10;
            fadeSprite = totalSprites;
            totalSprites++;
            totalSprites++;
            killSprite = totalSprites;
            totalSprites++;
            hands = new GenericBodyPart[2];
            for (int j = 0; j < 2; j++)
            {
                hands[j] = new GenericBodyPart(this, 2f, 0.5f, 0.98f, oracle.firstChunk);
            }
            feet = new GenericBodyPart[2];
            for (int k = 0; k < 2; k++)
            {
                feet[k] = new GenericBodyPart(this, 2f, 0.5f, 0.98f, oracle.firstChunk);
            }
            knees = new Vector2[2, 2];
            for (int l = 0; l < 2; l++)
            {
                for (int m = 0; m < 2; m++)
                {
                    knees[l, m] = oracle.firstChunk.pos;
                }
            }
            firstArmBaseSprite = totalSprites;
            armBase = new OracleGraphics.ArmBase(this, firstArmBaseSprite);
            totalSprites += armBase.totalSprites;
            voiceFreqSamples = new float[64];
        }
    }

    public extern void orig_InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[totalSprites];
        for (int i = 0; i < owner.bodyChunks.Length; i++)
        {
            sLeaser.sprites[firstBodyChunkSprite + i] = new FSprite("Circle20", true);
            sLeaser.sprites[firstBodyChunkSprite + i].scale = owner.bodyChunks[i].rad / 10f;
            sLeaser.sprites[firstBodyChunkSprite + i].color = new Color(1f, (i != 0) ? 0f : 0.5f, (i != 0) ? 0f : 0.5f);
        }
        if (oracle.ID == Oracle.OracleID.SL)
        {
            sLeaser.sprites[firstBodyChunkSprite].scaleY = (owner.bodyChunks[0].rad + 2f) / 10f;
        }
        for (int j = 0; j < armJointGraphics.Length; j++)
        {
            armJointGraphics[j].InitiateSprites(sLeaser, rCam);
        }
        if (gown != null)
        {
            gown.InitiateSprite(robeSprite, sLeaser, rCam);
        }
        if (halo != null)
        {
            halo.InitiateSprites(sLeaser, rCam);
        }
        armBase.InitiateSprites(sLeaser, rCam);
        sLeaser.sprites[neckSprite] = new FSprite("pixel", true);
        sLeaser.sprites[neckSprite].scaleX = ((oracle.ID != Oracle.OracleID.SS) ? 4f : 3f);
        sLeaser.sprites[neckSprite].anchorY = 0f;
        sLeaser.sprites[HeadSprite] = new FSprite("Circle20", true);
        sLeaser.sprites[ChinSprite] = new FSprite("Circle20", true);
        for (int k = 0; k < 2; k++)
        {
            sLeaser.sprites[EyeSprite(k)] = new FSprite("pixel", true);
            if (oracle.ID == Oracle.OracleID.SL)
            {
                sLeaser.sprites[EyeSprite(k)].color = new Color(0.02f, 0f, 0f);
            }
            sLeaser.sprites[PhoneSprite(k, 0)] = new FSprite("Circle20", true);
            sLeaser.sprites[PhoneSprite(k, 1)] = new FSprite("Circle20", true);
            sLeaser.sprites[PhoneSprite(k, 2)] = new FSprite("LizardScaleA1", true);
            sLeaser.sprites[PhoneSprite(k, 2)].anchorY = 0f;
            sLeaser.sprites[PhoneSprite(k, 2)].scaleY = 0.8f;
            sLeaser.sprites[PhoneSprite(k, 2)].scaleX = ((k != 0) ? 1f : -1f) * 0.75f;
            sLeaser.sprites[HandSprite(k, 0)] = new FSprite("haloGlyph-1", true);
            sLeaser.sprites[HandSprite(k, 1)] = TriangleMesh.MakeLongMesh(7, false, true);
            sLeaser.sprites[FootSprite(k, 0)] = new FSprite("haloGlyph-1", true);
            sLeaser.sprites[FootSprite(k, 1)] = TriangleMesh.MakeLongMesh(7, false, true);
        }
        if (IsMoon)
        {
            sLeaser.sprites[MoonThirdEyeSprite] = new FSprite("Circle20", true);
        }
        if (umbCord != null)
        {
            umbCord.InitiateSprites(sLeaser, rCam);
        }
        else if (discUmbCord != null)
        {
            discUmbCord.InitiateSprites(sLeaser, rCam);
        }
        sLeaser.sprites[HeadSprite].scaleX = head.rad / 9f;
        sLeaser.sprites[HeadSprite].scaleY = head.rad / 11f;
        sLeaser.sprites[ChinSprite].scale = head.rad / 15f;
        sLeaser.sprites[fadeSprite] = new FSprite("Futile_White", true);
        sLeaser.sprites[fadeSprite].scale = 12.5f;
        sLeaser.sprites[fadeSprite].color = ((oracle.ID != Oracle.OracleID.SS) ? new Color(0.1f, 0.2f, 0.7f) : new Color(0f, 0f, 0f));
        sLeaser.sprites[fadeSprite].shader = rCam.game.rainWorld.Shaders["FlatLightBehindTerrain"];
        sLeaser.sprites[fadeSprite].alpha = ((oracle.ID != Oracle.OracleID.SS) ? 0.4f : 0.5f);
        if (oracle.ID == Oracle.OracleID.SS)
        {
            sLeaser.sprites[killSprite] = new FSprite("Futile_White", true);
            sLeaser.sprites[killSprite].shader = rCam.game.rainWorld.Shaders["FlatLight"];
            AddToContainer(sLeaser, rCam, null);
        }
        else { 
            sLeaser.sprites[killSprite] = new FSprite("MoonMark", true);
            sLeaser.sprites[killSprite].scale = 1f;
            sLeaser.sprites[killSprite].scaleX = 1f;
            sLeaser.sprites[killSprite].scaleY = 1f;
            sLeaser.sprites[killSprite].alpha = 1f;
            sLeaser.RemoveAllSpritesFromContainer();
            FContainer newContatiner = rCam.ReturnFContainer("Midground");
            for (int i = firstArmBaseSprite; i < firstArmBaseSprite + armBase.totalSprites; i++)
            {
                rCam.ReturnFContainer((i >= firstArmBaseSprite + 6 && i != firstArmBaseSprite + 8) ? "Shortcuts" : "Midground").AddChild(sLeaser.sprites[i]);
            }
            if (halo == null)
            {
                for (int j = 0; j < firstArmBaseSprite; j++)
                {
                    newContatiner.AddChild(sLeaser.sprites[j]);
                }
            }
            else
            {
                for (int k = 0; k < halo.firstSprite; k++)
                {
                    newContatiner.AddChild(sLeaser.sprites[k]);
                }
                FContainer fcontainer = rCam.ReturnFContainer("BackgroundShortcuts");
                for (int l = halo.firstSprite; l < halo.firstSprite + halo.totalSprites; l++)
                {
                    fcontainer.AddChild(sLeaser.sprites[l]);
                }
                for (int m = halo.firstSprite + halo.totalSprites; m < firstArmBaseSprite; m++)
                {
                    if (m != fadeSprite)
                    {
                        newContatiner.AddChild(sLeaser.sprites[m]);
                    }
                }
            }
            rCam.ReturnFContainer("Shortcuts").AddChild(sLeaser.sprites[fadeSprite]);
        }
        
        
        //Delegate to call the base InitiateSprites
        Type[] baseSignature = new Type[2];
        baseSignature[0] = typeof(RoomCamera.SpriteLeaser);
        baseSignature[1] = typeof(RoomCamera);
        RuntimeMethodHandle handle = typeof(GraphicsModule).GetMethod("InitiateSprites", baseSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action<RoomCamera.SpriteLeaser, RoomCamera> funct = (Action<RoomCamera.SpriteLeaser, RoomCamera>)Activator.CreateInstance(typeof(Action<RoomCamera.SpriteLeaser, RoomCamera>), this, ptr);
        funct(sLeaser, rCam);//base InitiateSprites
    }

    public extern void orig_DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
            Vector2 vector = Vector2.Lerp(base.owner.firstChunk.lastPos, base.owner.firstChunk.pos, timeStacker);
            Vector2 vector2 = Custom.DirVec(Vector2.Lerp(base.owner.bodyChunks[1].lastPos, base.owner.bodyChunks[1].pos, timeStacker), vector);
            Vector2 a = Custom.PerpendicularVector(vector2);
            Vector2 a2 = Vector2.Lerp(this.lastLookDir, this.lookDir, timeStacker);
            Vector2 vector3 = Vector2.Lerp(this.head.lastPos, this.head.pos, timeStacker);
            for (int i = 0; i < base.owner.bodyChunks.Length; i++)
            {
                sLeaser.sprites[this.firstBodyChunkSprite + i].x = Mathf.Lerp(base.owner.bodyChunks[i].lastPos.x, base.owner.bodyChunks[i].pos.x, timeStacker) - camPos.x;
                sLeaser.sprites[this.firstBodyChunkSprite + i].y = Mathf.Lerp(base.owner.bodyChunks[i].lastPos.y, base.owner.bodyChunks[i].pos.y, timeStacker) - camPos.y;
            }
            sLeaser.sprites[this.firstBodyChunkSprite].rotation = Custom.AimFromOneVectorToAnother(vector, vector3) - Mathf.Lerp(14f, 0f, Mathf.Lerp(this.lastBreatheFac, this.breathFac, timeStacker));
            sLeaser.sprites[this.firstBodyChunkSprite + 1].rotation = Custom.VecToDeg(vector2);
            for (int j = 0; j < this.armJointGraphics.Length; j++)
            {
                this.armJointGraphics[j].DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            if (this.oracle.ID == Oracle.OracleID.SS)
            {
                if ((this.oracle.oracleBehavior as FPOracleBehaviorHasMark).killFac > 0f)
                {
                    sLeaser.sprites[this.killSprite].isVisible = true;
                    sLeaser.sprites[this.killSprite].x = Mathf.Lerp(this.oracle.oracleBehavior.player.mainBodyChunk.lastPos.x, this.oracle.oracleBehavior.player.mainBodyChunk.pos.x, timeStacker) - camPos.x;
                    sLeaser.sprites[this.killSprite].y = Mathf.Lerp(this.oracle.oracleBehavior.player.mainBodyChunk.lastPos.y, this.oracle.oracleBehavior.player.mainBodyChunk.pos.y, timeStacker) - camPos.y;
                    float f = Mathf.Lerp((this.oracle.oracleBehavior as FPOracleBehaviorHasMark).lastKillFac, (this.oracle.oracleBehavior as FPOracleBehaviorHasMark).killFac, timeStacker);
                    sLeaser.sprites[this.killSprite].scale = Mathf.Lerp(200f, 2f, Mathf.Pow(f, 0.5f));
                    sLeaser.sprites[this.killSprite].alpha = Mathf.Pow(f, 3f);
                }
                else
                {
                    sLeaser.sprites[this.killSprite].isVisible = false;
                }
            }
            sLeaser.sprites[this.fadeSprite].x = vector3.x - camPos.x;
            sLeaser.sprites[this.fadeSprite].y = vector3.y - camPos.y;
            sLeaser.sprites[this.neckSprite].x = vector.x - camPos.x;
            sLeaser.sprites[this.neckSprite].y = vector.y - camPos.y;
            sLeaser.sprites[this.neckSprite].rotation = Custom.AimFromOneVectorToAnother(vector, vector3);
            sLeaser.sprites[this.neckSprite].scaleY = Vector2.Distance(vector, vector3);
            if (this.gown != null)
            {
                this.gown.DrawSprite(this.robeSprite, sLeaser, rCam, timeStacker, camPos);
            }
            if (this.halo != null)
            {
                this.halo.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            this.armBase.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            Vector2 vector4 = Custom.DirVec(vector3, vector);
            Vector2 a3 = Custom.PerpendicularVector(vector4);
            sLeaser.sprites[this.HeadSprite].x = vector3.x - camPos.x;
            sLeaser.sprites[this.HeadSprite].y = vector3.y - camPos.y;
            sLeaser.sprites[this.HeadSprite].rotation = Custom.VecToDeg(vector4);
            Vector2 vector5 = this.RelativeLookDir(timeStacker);
            Vector2 a4 = Vector2.Lerp(vector3, vector, 0.15f);
            a4 += a3 * vector5.x * 2f;
            sLeaser.sprites[this.ChinSprite].x = a4.x - camPos.x;
            sLeaser.sprites[this.ChinSprite].y = a4.y - camPos.y;
            float num = Mathf.Lerp(this.lastEyesOpen, this.eyesOpen, timeStacker);
            for (int k = 0; k < 2; k++)
            {
                float num2 = (k != 0) ? 1f : -1f;
                Vector2 vector6 = vector3 + a3 * Mathf.Clamp(vector5.x * 3f + 2.5f * num2, -5f, 5f) + vector4 * (1f - vector5.y * 3f);
                sLeaser.sprites[this.EyeSprite(k)].rotation = Custom.VecToDeg(vector4);
                sLeaser.sprites[this.EyeSprite(k)].scaleX = 1f + ((k != 0) ? Mathf.InverseLerp(1f, 0.5f, vector5.x) : Mathf.InverseLerp(-1f, -0.5f, vector5.x)) + (1f - num);
                sLeaser.sprites[this.EyeSprite(k)].scaleY = Mathf.Lerp(1f, (this.oracle.ID != Oracle.OracleID.SS) ? 3f : 2f, num);
                sLeaser.sprites[this.EyeSprite(k)].x = vector6.x - camPos.x;
                sLeaser.sprites[this.EyeSprite(k)].y = vector6.y - camPos.y;
                sLeaser.sprites[this.EyeSprite(k)].alpha = 0.5f + 0.5f * num;
                int side = (k < 1 != vector5.x < 0f) ? 1 : 0;
                Vector2 a5 = vector3 + a3 * Mathf.Clamp(Mathf.Lerp(7f, 5f, Mathf.Abs(vector5.x)) * num2, -11f, 11f);
                for (int l = 0; l < 2; l++)
                {
                    sLeaser.sprites[this.PhoneSprite(side, l)].rotation = Custom.VecToDeg(vector4);
                    sLeaser.sprites[this.PhoneSprite(side, l)].scaleY = 5.5f * ((l != 0) ? 0.8f : 1f) / 20f;
                    sLeaser.sprites[this.PhoneSprite(side, l)].scaleX = Mathf.Lerp(3.5f, 5f, Mathf.Abs(vector5.x)) * ((l != 0) ? 0.8f : 1f) / 20f;
                    sLeaser.sprites[this.PhoneSprite(side, l)].x = a5.x - camPos.x;
                    sLeaser.sprites[this.PhoneSprite(side, l)].y = a5.y - camPos.y;
                }
                sLeaser.sprites[this.PhoneSprite(side, 2)].x = a5.x - camPos.x;
                sLeaser.sprites[this.PhoneSprite(side, 2)].y = a5.y - camPos.y;
                sLeaser.sprites[this.PhoneSprite(side, 2)].rotation = Custom.AimFromOneVectorToAnother(vector, a5 - vector4 * 40f - a2 * 10f);
                Vector2 vector7 = Vector2.Lerp(this.hands[k].lastPos, this.hands[k].pos, timeStacker);
                Vector2 vector8 = vector + a * 4f * ((k != 1) ? 1f : -1f);
                if (this.oracle.ID == Oracle.OracleID.SL)
                {
                    vector8 += vector2 * 3f;
                }
                Vector2 cB = vector7 + Custom.DirVec(vector7, vector8) * 3f + vector2;
                Vector2 cA = vector8 + a * 5f * ((k != 1) ? 1f : -1f);
                sLeaser.sprites[this.HandSprite(k, 0)].x = vector7.x - camPos.x;
                sLeaser.sprites[this.HandSprite(k, 0)].y = vector7.y - camPos.y;
                Vector2 vector9 = vector8 - a * 2f * ((k != 1) ? 1f : -1f);
                float num3 = (this.oracle.ID != Oracle.OracleID.SS) ? 2f : 4f;
                for (int m = 0; m < 7; m++)
                {
                    float f2 = (float)m / 6f;
                    Vector2 vector10 = Custom.Bezier(vector8, cA, vector7, cB, f2);
                    Vector2 vector11 = Custom.DirVec(vector9, vector10);
                    Vector2 a6 = Custom.PerpendicularVector(vector11) * ((k != 0) ? 1f : -1f);
                    float d = Vector2.Distance(vector9, vector10);
                    (sLeaser.sprites[this.HandSprite(k, 1)] as TriangleMesh).MoveVertice(m * 4, vector10 - vector11 * d * 0.3f - a6 * num3 - camPos);
                    (sLeaser.sprites[this.HandSprite(k, 1)] as TriangleMesh).MoveVertice(m * 4 + 1, vector10 - vector11 * d * 0.3f + a6 * num3 - camPos);
                    (sLeaser.sprites[this.HandSprite(k, 1)] as TriangleMesh).MoveVertice(m * 4 + 2, vector10 - a6 * num3 - camPos);
                    (sLeaser.sprites[this.HandSprite(k, 1)] as TriangleMesh).MoveVertice(m * 4 + 3, vector10 + a6 * num3 - camPos);
                    vector9 = vector10;
                }
                vector7 = Vector2.Lerp(this.feet[k].lastPos, this.feet[k].pos, timeStacker);
                vector8 = Vector2.Lerp(this.oracle.bodyChunks[1].lastPos, this.oracle.bodyChunks[1].pos, timeStacker);
                Vector2 to = Vector2.Lerp(this.knees[k, 1], this.knees[k, 0], timeStacker);
                cB = Vector2.Lerp(vector7, to, 0.9f);
                cA = Vector2.Lerp(vector8, to, 0.9f);
                sLeaser.sprites[this.FootSprite(k, 0)].x = vector7.x - camPos.x;
                sLeaser.sprites[this.FootSprite(k, 0)].y = vector7.y - camPos.y;
                vector9 = vector8 - a * 2f * ((k != 1) ? 1f : -1f);
                float num4 = 4f;
                for (int n = 0; n < 7; n++)
                {
                    float f3 = (float)n / 6f;
                    num3 = ((this.oracle.ID != Oracle.OracleID.SS) ? Mathf.Lerp(4f, 2f, Mathf.Pow(f3, 0.5f)) : 2f);
                    Vector2 vector12 = Custom.Bezier(vector8, cA, vector7, cB, f3);
                    Vector2 vector13 = Custom.DirVec(vector9, vector12);
                    Vector2 a7 = Custom.PerpendicularVector(vector13) * ((k != 0) ? 1f : -1f);
                    float d2 = Vector2.Distance(vector9, vector12);
                    (sLeaser.sprites[this.FootSprite(k, 1)] as TriangleMesh).MoveVertice(n * 4, vector12 - vector13 * d2 * 0.3f - a7 * (num4 + num3) * 0.5f - camPos);
                    (sLeaser.sprites[this.FootSprite(k, 1)] as TriangleMesh).MoveVertice(n * 4 + 1, vector12 - vector13 * d2 * 0.3f + a7 * (num4 + num3) * 0.5f - camPos);
                    (sLeaser.sprites[this.FootSprite(k, 1)] as TriangleMesh).MoveVertice(n * 4 + 2, vector12 - a7 * num3 - camPos);
                    (sLeaser.sprites[this.FootSprite(k, 1)] as TriangleMesh).MoveVertice(n * 4 + 3, vector12 + a7 * num3 - camPos);
                    vector9 = vector12;
                    num4 = num3;
                }
            }
            if (this.IsMoon)
            {
                Vector2 p = vector3 + a3 * vector5.x * 2.5f + vector4 * (-2f - vector5.y * 1.5f);
                sLeaser.sprites[this.MoonThirdEyeSprite].x = p.x - camPos.x;
                sLeaser.sprites[this.MoonThirdEyeSprite].y = p.y - camPos.y;
                sLeaser.sprites[this.MoonThirdEyeSprite].rotation = Custom.AimFromOneVectorToAnother(p, vector3 - vector4 * 10f);
                sLeaser.sprites[this.MoonThirdEyeSprite].scaleX = Mathf.Lerp(0.2f, 0.15f, Mathf.Abs(vector5.x));
                sLeaser.sprites[this.MoonThirdEyeSprite].scaleY = Custom.LerpMap(vector5.y, 0f, 1f, 0.2f, 0.05f);
            }
            if (this.umbCord != null)
            {
                this.umbCord.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            else if (this.discUmbCord != null)
            {
                this.discUmbCord.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            if (IsMoon)
            {
                Vector2 vector7 = Vector2.Lerp(owner.firstChunk.lastPos, owner.firstChunk.pos, timeStacker);
                Vector2 vector72 = Custom.DirVec(Vector2.Lerp(owner.bodyChunks[1].lastPos, owner.bodyChunks[1].pos, timeStacker), vector7);
                Vector2 a7 = Custom.PerpendicularVector(vector72);
                Vector2 a72 = Vector2.Lerp(lastLookDir, lookDir, timeStacker);
                Vector2 vector73 = Vector2.Lerp(head.lastPos, head.pos, timeStacker);
                Vector2 vector74 = Custom.DirVec(vector73, vector7);
                Vector2 up = Custom.DirVec(vector7, vector73);
                Vector2 a73 = Custom.PerpendicularVector(vector74);
                Vector2 vector75 = RelativeLookDir(timeStacker);
                Vector2 p = vector73 + a73 * vector75.x * 2.5f + vector74 * (-2f - vector75.y * 1.5f);
                sLeaser.sprites[killSprite].x = p.x - camPos.x + (up.x * 5f);
                sLeaser.sprites[killSprite].y = p.y - camPos.y + (up.y * 5f);
                sLeaser.sprites[killSprite].rotation = Custom.AimFromOneVectorToAnother(p, vector73 - vector74 * 10f);
                sLeaser.sprites[killSprite].scaleX = Mathf.Lerp(0.2f, 0.15f, Mathf.Abs(vector75.x));
                sLeaser.sprites[killSprite].scaleY = Custom.LerpMap(vector75.y, 0f, 1f, 0.2f, 0.05f);
                sLeaser.sprites[killSprite].scale = 1f;
                sLeaser.sprites[killSprite].scaleX = 1f;
                sLeaser.sprites[killSprite].scaleY = 1f;
                //Vector2 vector = Vector2.Lerp(this.head.lastPos, this.head.pos, timeStacker);
                //sLeaser.sprites[killSprite].x = vector.x - camPos.x;
                //sLeaser.sprites[killSprite].y = vector.y - camPos.y;
                sLeaser.sprites[killSprite].isVisible = true;
            }
        //Delegate to call the base DrawSprites
        Type[] baseSignature = new Type[4];
        baseSignature[0] = typeof(RoomCamera.SpriteLeaser);
        baseSignature[1] = typeof(RoomCamera);
        baseSignature[2] = typeof(float);
        baseSignature[3] = typeof(Vector2);
        RuntimeMethodHandle handle = typeof(GraphicsModule).GetMethod("DrawSprites", baseSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action<RoomCamera.SpriteLeaser, RoomCamera, float, Vector2> funct = (Action<RoomCamera.SpriteLeaser, RoomCamera, float, Vector2>)Activator.CreateInstance(typeof(Action<RoomCamera.SpriteLeaser, RoomCamera, float, Vector2>), this, ptr);
        funct(sLeaser, rCam, timeStacker, camPos);//base DrawSprites
    }
    [MonoModIgnore]
    private Vector2 RelativeLookDir(float timeStacker)
    {
        return Custom.RotateAroundOrigo(Vector2.Lerp(lastLookDir, lookDir, timeStacker), -Custom.AimFromOneVectorToAnother(Vector2.Lerp(oracle.bodyChunks[1].lastPos, oracle.bodyChunks[1].pos, timeStacker), Vector2.Lerp(oracle.firstChunk.lastPos, oracle.firstChunk.pos, timeStacker)));
    }

    public extern void orig_ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig_ApplyPalette(sLeaser, rCam, palette);
        SLArmBaseColA = new Color(0.521568656f, 0.521568656f, 0.5137255f);
        SLArmHighLightColA = new Color(0.5686275f, 0.5686275f, 0.549019635f);
        SLArmBaseColB = palette.texture.GetPixel(5, 1);
        SLArmHighLightColB = palette.texture.GetPixel(5, 2);
        for (int i = 0; i < armJointGraphics.Length; i++)
        {
            armJointGraphics[i].ApplyPalette(sLeaser, rCam, palette);
        }
        Color color;
        if (oracle.ID == Oracle.OracleID.SS)
        {
            color = new Color(1f, 0.4f, 0.796078444f);
        }
        else
        {
            color = new Color(0.105882354f * 1.6f, 0.270588249f * 1.6f, 0.34117648f * 1.6f);
        }
        for (int j = 0; j < owner.bodyChunks.Length; j++)
        {
            sLeaser.sprites[firstBodyChunkSprite + j].color = color;
        }
        sLeaser.sprites[neckSprite].color = color;
        sLeaser.sprites[HeadSprite].color = color;
        sLeaser.sprites[ChinSprite].color = color;
        for (int k = 0; k < 2; k++)
        {
            sLeaser.sprites[PhoneSprite(k, 0)].color = armJointGraphics[0].BaseColor(default(Vector2));
            sLeaser.sprites[PhoneSprite(k, 1)].color = armJointGraphics[0].HighLightColor(default(Vector2));
            sLeaser.sprites[PhoneSprite(k, 2)].color = armJointGraphics[0].HighLightColor(default(Vector2));
            sLeaser.sprites[HandSprite(k, 0)].color = color;
            if (gown != null)
            {
                for (int l = 0; l < 7; l++)
                {
                    (sLeaser.sprites[HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4] = gown.Color(0.4f);
                    (sLeaser.sprites[HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 1] = gown.Color(0f);
                    (sLeaser.sprites[HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 2] = gown.Color(0.4f);
                    (sLeaser.sprites[HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 3] = gown.Color(0f);
                }
            }
            else
            {
                sLeaser.sprites[HandSprite(k, 1)].color = color;
            }
            sLeaser.sprites[FootSprite(k, 0)].color = color;
            sLeaser.sprites[FootSprite(k, 1)].color = color;
        }
        if (umbCord != null)
        {
            umbCord.ApplyPalette(sLeaser, rCam, palette);
            sLeaser.sprites[firstUmbilicalSprite].color = palette.blackColor;
        }
        else if (discUmbCord != null)
        {
            discUmbCord.ApplyPalette(sLeaser, rCam, palette);
        }
        armBase.ApplyPalette(sLeaser, rCam, palette);
        if (IsMoon)
        {
            sLeaser.sprites[MoonThirdEyeSprite].color = Color.Lerp(new Color(1f, 0f, 1f), color, 0.5f);
        }
        if (IsMoon)
        {
            sLeaser.sprites[killSprite].color = new Color(0.105882354f*2.1f, 0.270588249f* 2.1f, 0.34117648f* 2.1f);
        }
    }


    public override void Update()
    {
        //Delegate to call the base Update
        Type[] updateSignature = new Type[0];
        RuntimeMethodHandle handle = typeof(GraphicsModule).GetMethod("Update", updateSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action funct = (Action)Activator.CreateInstance(typeof(Action), this, ptr);
        funct();//GraphicsModule Update Method (I hope this works)

        breathe += 1f / Mathf.Lerp(10f, 60f, oracle.health);
        lastBreatheFac = breathFac;
        breathFac = Mathf.Lerp(0.5f + 0.5f * Mathf.Sin(breathe * 3.14159274f * 2f), 1f, Mathf.Pow(oracle.health, 2f));
        if (gown != null)
        {
            gown.Update();
        }
        if (halo != null)
        {
            halo.Update();
        }
        armBase.Update();
        bool flag = false;
        lastLookDir = lookDir;
        if (oracle.Consious)
        {
            lookDir = Vector2.ClampMagnitude(oracle.oracleBehavior.lookPoint - oracle.firstChunk.pos, 100f) / 100f;
            lookDir = Vector2.ClampMagnitude(lookDir + randomTalkVector * averageVoice * 0.3f, 1f);
        }
        head.Update();
        head.ConnectToPoint(oracle.firstChunk.pos + Custom.DirVec(oracle.bodyChunks[1].pos, oracle.firstChunk.pos) * 6f, 8f, true, 0f, oracle.firstChunk.vel, 0.5f, 0.01f);
        if (oracle.Consious)
        {
            if (flag && oracle.oracleBehavior.EyesClosed)
            {
                head.vel += Custom.DegToVec(-90f);
            }
            else
            {
                head.vel += Custom.DirVec(oracle.bodyChunks[1].pos, oracle.firstChunk.pos) * breathFac;
                head.vel += lookDir * 0.5f * breathFac;
            }
        }
        else
        {
            head.vel += Custom.DirVec(oracle.bodyChunks[1].pos, oracle.firstChunk.pos) * 0.75f;
            GenericBodyPart genericBodyPart = head;
            genericBodyPart.vel.y = genericBodyPart.vel.y - 0.7f;
        }
        for (int i = 0; i < 2; i++)
        {
            feet[i].Update();
            feet[i].ConnectToPoint(oracle.bodyChunks[1].pos, (oracle.ID != Oracle.OracleID.SL) ? 10f : 20f, false, 0f, oracle.bodyChunks[1].vel, 0.3f, 0.01f);
            if (oracle.ID == Oracle.OracleID.SL)
            {
                GenericBodyPart genericBodyPart2 = feet[i];
                genericBodyPart2.vel.y = genericBodyPart2.vel.y - 0.5f;
            }
            feet[i].vel += Custom.DirVec(oracle.firstChunk.pos, oracle.bodyChunks[1].pos) * 0.3f;
            feet[i].vel += Custom.PerpendicularVector(Custom.DirVec(oracle.firstChunk.pos, oracle.bodyChunks[1].pos)) * 0.15f * ((i != 0) ? 1f : -1f);
            hands[i].Update();
            hands[i].ConnectToPoint(oracle.firstChunk.pos, 15f, false, 0f, oracle.firstChunk.vel, 0.3f, 0.01f);
            GenericBodyPart genericBodyPart3 = hands[i];
            genericBodyPart3.vel.y = genericBodyPart3.vel.y - 0.5f;
            hands[i].vel += Custom.DirVec(oracle.firstChunk.pos, oracle.bodyChunks[1].pos) * 0.3f;
            hands[i].vel += Custom.PerpendicularVector(Custom.DirVec(oracle.firstChunk.pos, oracle.bodyChunks[1].pos)) * 0.3f * ((i != 0) ? 1f : -1f);
            knees[i, 1] = knees[i, 0];
            knees[i, 0] = (feet[i].pos + oracle.bodyChunks[1].pos) / 2f + Custom.PerpendicularVector(Custom.DirVec(oracle.firstChunk.pos, oracle.bodyChunks[1].pos)) * 4f * ((i != 0) ? 1f : -1f);
            if (oracle.ID == Oracle.OracleID.SS)
            {
                hands[i].vel += randomTalkVector * averageVoice * 0.8f;
                if (i == 0 && (oracle.oracleBehavior as FPOracleBehaviorHasMark).HandTowardsPlayer())
                {
                    hands[0].vel += Custom.DirVec(hands[0].pos, oracle.oracleBehavior.player.mainBodyChunk.pos) * 3f;
                }
                knees[i, 0] = (feet[i].pos + oracle.bodyChunks[1].pos) / 2f + Custom.PerpendicularVector(Custom.DirVec(oracle.firstChunk.pos, oracle.bodyChunks[1].pos)) * 4f * ((i != 0) ? 1f : -1f);
            }
            else
            {
                Vector2? vector = null;
                Vector2? vector2;
                if (flag)
                {
                    vector2 = SharedPhysics.ExactTerrainRayTracePos(oracle.room, oracle.firstChunk.pos, oracle.firstChunk.pos + new Vector2((i != 0) ? -14f : -24f, -40f));
                }
                else
                {
                    vector2 = SharedPhysics.ExactTerrainRayTracePos(oracle.room, oracle.bodyChunks[(!(oracle.oracleBehavior as LMOracleBehavior).InSitPosition) ? 1 : 0].pos, oracle.bodyChunks[(!(oracle.oracleBehavior as LMOracleBehavior).InSitPosition) ? 1 : 0].pos + new Vector2((i != 0) ? -24f : -54f, -40f) * 2f * 1f);
                }
                if (vector2 != null)
                {
                    //this.feet[i].vel += Vector2.ClampMagnitude(vector2.Value - this.feet[i].pos, 10f) / 2f;
                }
                Vector2 vector3 = (feet[i].pos + oracle.bodyChunks[1].pos) / 2f;
                if (flag && vector2 != null)
                {
                    Vector2 vector4 = feet[i].pos + Custom.DirVec(oracle.bodyChunks[1].pos, oracle.bodyChunks[0].pos) * 15f;
                    vector4 += Custom.DirVec(oracle.firstChunk.pos, vector4) * 5f;
                    vector3 = Vector2.Lerp(vector4, (feet[i].pos + oracle.bodyChunks[1].pos) / 2f, Mathf.InverseLerp(7f, 14f, Vector2.Distance(feet[i].pos, oracle.bodyChunks[1].pos)));
                }
                else
                {
                    vector3 += Custom.PerpendicularVector(oracle.bodyChunks[1].pos, oracle.bodyChunks[0].pos) * ((i != 0) ? 1f : -1f) * 5f;
                }
                //this.knees[i, 0] = Vector2.Lerp(this.knees[i, 0], vector3, 0.4f);
                if (!Custom.DistLess(knees[i, 0], vector3, 15f))
                {
                    //this.knees[i, 0] = vector3 + Custom.DirVec(vector3, this.knees[i, 0]);
                }
                if (oracle.Consious && i == 0 && (oracle.oracleBehavior as LMOracleBehavior).holdingObject != null)
                {
                    //this.hands[i].pos = (this.oracle.oracleBehavior as LMOracleBehavior).holdingObject.firstChunk.pos;
                    //this.hands[i].vel *= 0f;
                }
                if (i == 0 == oracle.firstChunk.pos.x > oracle.oracleBehavior.player.DangerPos.x && Custom.DistLess(oracle.firstChunk.pos, oracle.oracleBehavior.player.DangerPos, 40f))
                {
                    //this.hands[i].vel = Vector2.Lerp(this.hands[i].vel, Custom.DirVec(this.hands[i].pos, this.oracle.oracleBehavior.player.mainBodyChunk.pos) * 10f, 0.5f);
                }
                else if (i == 0 == oracle.firstChunk.pos.x > oracle.oracleBehavior.player.DangerPos.x && (oracle.oracleBehavior as LMOracleBehavior).armsProtest)
                {
                    //this.hands[i].vel = Vector2.Lerp(this.hands[i].vel, Custom.DirVec(this.hands[i].pos, this.oracle.oracleBehavior.player.mainBodyChunk.pos) * 10f, 0.5f) + new Vector2(0f, 10f * Mathf.Sin((this.oracle.oracleBehavior as LMOracleBehavior).protestCounter * 3.14159274f * 2f));
                }
                else if (flag)
                {
                    //this.hands[i].vel += Vector2.ClampMagnitude(this.knees[i, 0] - this.hands[i].pos, 10f) / 3f;
                }
                else if (!(oracle.oracleBehavior as LMOracleBehavior).InSitPosition)
                {
                    //vector2 = SharedPhysics.ExactTerrainRayTracePos(this.oracle.room, this.oracle.firstChunk.pos, this.oracle.firstChunk.pos + new Vector2(((i != 0) ? 1f : -1f) * (this.oracle.oracleBehavior as LMOracleBehavior).Crawl * 40f, -40f));
                    if (vector2 != null)
                    {
                    //    this.hands[i].vel += Vector2.ClampMagnitude(vector2.Value - this.hands[i].pos, 10f) / 3f;
                    }
                    else
                    {
                     //   GenericBodyPart genericBodyPart4 = this.hands[i];
                     //   genericBodyPart4.vel.x = genericBodyPart4.vel.x + ((i != 0) ? 1f : -1f) * (this.oracle.oracleBehavior as LMOracleBehavior).Crawl;
                    }
                    //this.knees[i, 0] = this.feet[i].pos + Custom.DirVec(this.feet[i].pos, this.oracle.oracleBehavior.OracleGetToPos + new Vector2(-50f, 0f)) * 8f + Custom.PerpendicularVector(this.oracle.bodyChunks[1].pos, this.oracle.bodyChunks[0].pos) * ((i != 0) ? 1f : -1f) * Mathf.Lerp(2f, 6f, (this.oracle.oracleBehavior as LMOracleBehavior).CrawlSpeed);
                }
            }
        }
        for (int j = 0; j < armJointGraphics.Length; j++)
        {
            armJointGraphics[j].Update();
        }
        if (umbCord != null)
        {
            umbCord.Update();
        }
        else if (discUmbCord != null)
        {
            discUmbCord.Update();
        }
        if (oracle.oracleBehavior.voice != null && oracle.oracleBehavior.voice.currentAudioSource != null && oracle.oracleBehavior.voice.currentAudioSource.isPlaying)
        {
            oracle.oracleBehavior.voice.currentAudioSource.GetSpectrumData(voiceFreqSamples, 0, FFTWindow.BlackmanHarris);
            averageVoice = 0f;
            for (int k = 0; k < voiceFreqSamples.Length; k++)
            {
                averageVoice += voiceFreqSamples[k];
            }
            averageVoice /= (float)voiceFreqSamples.Length;
            averageVoice = Mathf.InverseLerp(0f, 0.00014f, averageVoice);
            if (averageVoice > 0.7f && UnityEngine.Random.value < averageVoice / 14f)
            {
                randomTalkVector = Custom.RNV();
            }
        }
        else
        {
            randomTalkVector *= 0.9f;
            if (averageVoice > 0f)
            {
                for (int l = 0; l < voiceFreqSamples.Length; l++)
                {
                    voiceFreqSamples[l] = 0f;
                }
                averageVoice = 0f;
            }
        }
        lastEyesOpen = eyesOpen;
        eyesOpen = ((!oracle.oracleBehavior.EyesClosed) ? 1f : 0f);
        if (oracle.ID == Oracle.OracleID.SS)
        {
            if (lightsource == null)
            {
                lightsource = new LightSource(oracle.firstChunk.pos, false, Custom.HSL2RGB(0.1f, 1f, 0.5f), oracle);
                lightsource.affectedByPaletteDarkness = 0f;
                oracle.room.AddObject(lightsource);
            }
            else
            {
                lightsource.setAlpha = new float?((oracle.oracleBehavior as FPOracleBehaviorHasMark).working);
                lightsource.setRad = new float?(400f);
                lightsource.setPos = new Vector2?(oracle.firstChunk.pos);
            }
        }
        else
        {
            if (lightsource == null)
            {
                lightsource = new LightSource(oracle.firstChunk.pos, false, Custom.HSL2RGB(0.1012423f, 0.257576f, 0.91322334f), oracle);
                lightsource.affectedByPaletteDarkness = 0f;
                oracle.room.AddObject(lightsource);
            }
            else
            {
                lightsource.setAlpha = 1f;
                lightsource.setRad = new float?(100f);
                lightsource.setPos = new Vector2?(oracle.firstChunk.pos);
            }
        }
    }

    public class UbilicalCord
    {
        public UbilicalCord(patch_OracleGraphics owner, int firstSprite)
        {
            this.owner = owner;
            this.firstSprite = firstSprite;
            totalSprites = 1;
            coord = new Vector2[80, 3];
            for (int i = 0; i < coord.GetLength(0); i++)
            {
                coord[i, 0] = owner.owner.firstChunk.pos;
                coord[i, 1] = coord[i, 0];
            }
            totalSprites += coord.GetLength(0) * 2;
            smallCords = new Vector2[14, 20, 3];
            smallCordsLengths = new float[smallCords.GetLength(0)];
            smallCordsHeadDirs = new Vector2[smallCords.GetLength(0)];
            smallCoordColors = new int[smallCords.GetLength(0)];
            for (int j = 0; j < smallCords.GetLength(0); j++)
            {
                smallCordsLengths[j] = ((UnityEngine.Random.value >= 0.5f) ? Mathf.Lerp(50f, 200f, Mathf.Pow(UnityEngine.Random.value, 1.5f)) : (50f + UnityEngine.Random.value * 15f));
                smallCoordColors[j] = UnityEngine.Random.Range(0, 3);
                smallCordsHeadDirs[j] = Custom.RNV() * UnityEngine.Random.value;
                for (int k = 0; k < smallCords.GetLength(1); k++)
                {
                    coord[k, 0] = owner.owner.firstChunk.pos;
                    coord[k, 1] = coord[k, 0];
                }
            }
            totalSprites += smallCords.GetLength(0);
        }
        
        public int SegmentSprite(int seg, int part)
        {
            return firstSprite + 1 + seg * 2 + part;
        }
        
        public int SmallCordSprite(int c)
        {
            return firstSprite + 1 + coord.GetLength(0) * 2 + c;
        }
        
        public void Update()
        {
            for (int i = 0; i < coord.GetLength(0); i++)
            {
                float num = (float)i / (float)(coord.GetLength(0) - 1);
                coord[i, 1] = coord[i, 0];
                coord[i, 0] += coord[i, 2];
                coord[i, 2] *= 0.995f;
                coord[i, 2].y += Mathf.InverseLerp(0.2f, 0f, num);
                coord[i, 2].y -= owner.owner.room.gravity * 0.9f;
                SharedPhysics.TerrainCollisionData terrainCollisionData = new SharedPhysics.TerrainCollisionData(coord[i, 0], coord[i, 1], coord[i, 2], 5f, new IntVector2(0, 0), true);
                terrainCollisionData = SharedPhysics.VerticalCollision(owner.owner.room, terrainCollisionData);
                terrainCollisionData = SharedPhysics.HorizontalCollision(owner.owner.room, terrainCollisionData);
                terrainCollisionData = SharedPhysics.SlopesVertically(owner.owner.room, terrainCollisionData);
                coord[i, 0] = terrainCollisionData.pos;
                coord[i, 2] = terrainCollisionData.vel;
            }
            SetStuckSegments();
            for (int j = 1; j < coord.GetLength(0); j++)
            {
                Vector2 vector = Custom.DirVec(coord[j, 0], coord[j - 1, 0]);
                float num2 = Vector2.Distance(coord[j, 0], coord[j - 1, 0]);
                coord[j, 0] -= (10f - num2) * vector * 0.5f;
                coord[j, 2] -= (10f - num2) * vector * 0.5f;
                coord[j - 1, 0] += (10f - num2) * vector * 0.5f;
                coord[j - 1, 2] += (10f - num2) * vector * 0.5f;
            }
            SetStuckSegments();
            for (int k = 0; k < coord.GetLength(0) - 1; k++)
            {
                Vector2 vector2 = Custom.DirVec(coord[k, 0], coord[k + 1, 0]);
                float num3 = Vector2.Distance(coord[k, 0], coord[k + 1, 0]);
                coord[k, 0] -= (10f - num3) * vector2 * 0.5f;
                coord[k, 2] -= (10f - num3) * vector2 * 0.5f;
                coord[k + 1, 0] += (10f - num3) * vector2 * 0.5f;
                coord[k + 1, 2] += (10f - num3) * vector2 * 0.5f;
            }
            SetStuckSegments();
            float num4 = 0.5f;
            for (int l = 2; l < 4; l++)
            {
                for (int m = l; m < coord.GetLength(0) - l; m++)
                {
                    coord[m, 2] += Custom.DirVec(coord[m - l, 0], coord[m, 0]) * num4;
                    coord[m - l, 2] -= Custom.DirVec(coord[m - l, 0], coord[m, 0]) * num4;
                    coord[m, 2] += Custom.DirVec(coord[m + l, 0], coord[m, 0]) * num4;
                    coord[m + l, 2] -= Custom.DirVec(coord[m + l, 0], coord[m, 0]) * num4;
                }
                num4 *= 0.75f;
            }
            if (!Custom.DistLess(coord[coord.GetLength(0) - 1, 0], owner.owner.firstChunk.pos, 80f))
            {
                Vector2 vector3 = Custom.DirVec(coord[coord.GetLength(0) - 1, 0], owner.owner.firstChunk.pos);
                float num5 = Vector2.Distance(coord[coord.GetLength(0) - 1, 0], owner.owner.firstChunk.pos);
                coord[coord.GetLength(0) - 1, 0] -= (80f - num5) * vector3 * 0.25f;
                coord[coord.GetLength(0) - 1, 2] -= (80f - num5) * vector3 * 0.5f;
            }
            for (int n = 0; n < smallCords.GetLength(0); n++)
            {
                for (int num6 = 0; num6 < smallCords.GetLength(1); num6++)
                {
                    smallCords[n, num6, 1] = smallCords[n, num6, 0];
                    smallCords[n, num6, 0] += smallCords[n, num6, 2];
                    smallCords[n, num6, 2] *= Custom.LerpMap(smallCords[n, num6, 2].magnitude, 2f, 6f, 0.999f, 0.9f);
                    smallCords[n, num6, 2].y -= owner.owner.room.gravity * 0.9f;
                }
                float num7 = smallCordsLengths[n] / (float)smallCords.GetLength(1);
                for (int num8 = 1; num8 < smallCords.GetLength(1); num8++)
                {
                    Vector2 vector4 = Custom.DirVec(smallCords[n, num8, 0], smallCords[n, num8 - 1, 0]);
                    float num9 = Vector2.Distance(smallCords[n, num8, 0], smallCords[n, num8 - 1, 0]);
                    smallCords[n, num8, 0] -= (num7 - num9) * vector4 * 0.5f;
                    smallCords[n, num8, 2] -= (num7 - num9) * vector4 * 0.5f;
                    smallCords[n, num8 - 1, 0] += (num7 - num9) * vector4 * 0.5f;
                    smallCords[n, num8 - 1, 2] += (num7 - num9) * vector4 * 0.5f;
                }
                for (int num10 = 0; num10 < smallCords.GetLength(1) - 1; num10++)
                {
                    Vector2 vector5 = Custom.DirVec(smallCords[n, num10, 0], smallCords[n, num10 + 1, 0]);
                    float num11 = Vector2.Distance(smallCords[n, num10, 0], smallCords[n, num10 + 1, 0]);
                    smallCords[n, num10, 0] -= (num7 - num11) * vector5 * 0.5f;
                    smallCords[n, num10, 2] -= (num7 - num11) * vector5 * 0.5f;
                    smallCords[n, num10 + 1, 0] += (num7 - num11) * vector5 * 0.5f;
                    smallCords[n, num10 + 1, 2] += (num7 - num11) * vector5 * 0.5f;
                }
                smallCords[n, 0, 0] = coord[coord.GetLength(0) - 1, 0];
                smallCords[n, 0, 2] *= 0f;
                smallCords[n, 1, 2] += Custom.DirVec(coord[coord.GetLength(0) - 2, 0], coord[coord.GetLength(0) - 1, 0]) * 5f;
                smallCords[n, 2, 2] += Custom.DirVec(coord[coord.GetLength(0) - 2, 0], coord[coord.GetLength(0) - 1, 0]) * 3f;
                smallCords[n, 3, 2] += Custom.DirVec(coord[coord.GetLength(0) - 2, 0], coord[coord.GetLength(0) - 1, 0]) * 1.5f;
                smallCords[n, smallCords.GetLength(1) - 1, 0] = owner.head.pos;
                smallCords[n, smallCords.GetLength(1) - 1, 2] *= 0f;
                smallCords[n, smallCords.GetLength(1) - 2, 2] -= (owner.lookDir + smallCordsHeadDirs[n]) * 2f;
                smallCords[n, smallCords.GetLength(1) - 3, 2] -= owner.lookDir + smallCordsHeadDirs[n];
            }
        }
        
        private void SetStuckSegments()
        {
            if (owner.IsMoon)
            {
                coord[0, 0] = owner.owner.room.MiddleOfTile(78, 1);
                coord[0, 2] *= 0f;
                Vector2 pos = owner.armJointGraphics[1].myJoint.pos;
                Vector2 vector = owner.armJointGraphics[1].myJoint.ElbowPos(1f, owner.armJointGraphics[2].myJoint.pos);
                for (int i = -1; i < 2; i++)
                {
                    float num = (i != 0) ? 0.5f : 1f;
                    coord[coord.GetLength(0) - 20 + i, 0] = Vector2.Lerp(coord[coord.GetLength(0) - 20 + i, 0], Vector2.Lerp(pos, vector, 0.4f + 0.07f * (float)i) + Custom.PerpendicularVector(pos, vector) * 8f, num);
                    coord[coord.GetLength(0) - 20 + i, 2] *= 1f - num;
                }
            }
            else
            {
                coord[0, 0] = owner.owner.room.MiddleOfTile(24, 2);
                coord[0, 2] *= 0f;
                Vector2 pos = owner.armJointGraphics[1].myJoint.pos;
                Vector2 vector = owner.armJointGraphics[1].myJoint.ElbowPos(1f, owner.armJointGraphics[2].myJoint.pos);
                for (int i = -1; i < 2; i++)
                {
                    float num = (i != 0) ? 0.5f : 1f;
                    coord[coord.GetLength(0) - 20 + i, 0] = Vector2.Lerp(coord[coord.GetLength(0) - 20 + i, 0], Vector2.Lerp(pos, vector, 0.4f + 0.07f * (float)i) + Custom.PerpendicularVector(pos, vector) * 8f, num);
                    coord[coord.GetLength(0) - 20 + i, 2] *= 1f - num;
                }
            }
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites[firstSprite] = TriangleMesh.MakeLongMesh(coord.GetLength(0), false, false);
            for (int i = 0; i < coord.GetLength(0); i++)
            {
                sLeaser.sprites[SegmentSprite(i, 0)] = new FSprite("CentipedeSegment", true);
                sLeaser.sprites[SegmentSprite(i, 1)] = new FSprite("CentipedeSegment", true);
                sLeaser.sprites[SegmentSprite(i, 0)].scaleX = 0.5f;
                sLeaser.sprites[SegmentSprite(i, 0)].scaleY = 0.3f;
                sLeaser.sprites[SegmentSprite(i, 1)].scaleX = 0.4f;
                sLeaser.sprites[SegmentSprite(i, 1)].scaleY = 0.15f;
            }
            for (int j = 0; j < smallCords.GetLength(0); j++)
            {
                sLeaser.sprites[SmallCordSprite(j)] = TriangleMesh.MakeLongMesh(smallCords.GetLength(1), false, false);
            }
        }
        
        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = coord[0, 0];
            Vector2 vector2;
            vector2 = new Vector2(0f, 0f);
            float num = 1.2f;
            for (int i = 0; i < coord.GetLength(0); i++)
            {
                Vector2 vector3 = Vector2.Lerp(coord[i, 1], coord[i, 0], timeStacker);
                Vector2 vector4 = Custom.DirVec(vector, vector3);
                Vector2 vector5 = Custom.PerpendicularVector(vector4);
                float num2 = Vector2.Distance(vector, vector3);
                (sLeaser.sprites[firstSprite] as TriangleMesh).MoveVertice(i * 4, vector3 - vector4 * num2 * 0.5f - vector5 * num - camPos);
                (sLeaser.sprites[firstSprite] as TriangleMesh).MoveVertice(i * 4 + 1, vector3 - vector4 * num2 * 0.5f + vector5 * num - camPos);
                (sLeaser.sprites[firstSprite] as TriangleMesh).MoveVertice(i * 4 + 2, vector3 - vector5 * num - camPos);
                (sLeaser.sprites[firstSprite] as TriangleMesh).MoveVertice(i * 4 + 3, vector3 + vector5 * num - camPos);
                Vector2 vector6 = vector4;
                if (i < coord.GetLength(0) - 1)
                {
                    vector6 = Custom.DirVec(vector3, Vector2.Lerp(coord[i + 1, 1], coord[i + 1, 0], timeStacker));
                }
                sLeaser.sprites[SegmentSprite(i, 0)].x = vector3.x - camPos.x;
                sLeaser.sprites[SegmentSprite(i, 0)].y = vector3.y - camPos.y;
                sLeaser.sprites[SegmentSprite(i, 0)].rotation = Custom.VecToDeg((vector4 + vector6).normalized) + 90f;
                sLeaser.sprites[SegmentSprite(i, 1)].x = vector3.x - camPos.x;
                sLeaser.sprites[SegmentSprite(i, 1)].y = vector3.y - camPos.y;
                sLeaser.sprites[SegmentSprite(i, 1)].rotation = Custom.VecToDeg((vector4 + vector6).normalized) + 90f;
                vector = vector3;
                vector2 = vector4;
            }
            for (int j = 0; j < smallCords.GetLength(0); j++)
            {
                Vector2 vector7 = Vector2.Lerp(smallCords[j, 0, 1], smallCords[j, 0, 0], timeStacker);
                float num3 = 0.5f;
                for (int k = 0; k < smallCords.GetLength(1); k++)
                {
                    Vector2 vector8 = Vector2.Lerp(smallCords[j, k, 1], smallCords[j, k, 0], timeStacker);
                    Vector2 normalized = (vector7 - vector8).normalized;
                    Vector2 vector9 = Custom.PerpendicularVector(normalized);
                    float num4 = Vector2.Distance(vector7, vector8) / 5f;
                    (sLeaser.sprites[SmallCordSprite(j)] as TriangleMesh).MoveVertice(k * 4, vector7 - normalized * num4 - vector9 * num3 - camPos);
                    (sLeaser.sprites[SmallCordSprite(j)] as TriangleMesh).MoveVertice(k * 4 + 1, vector7 - normalized * num4 + vector9 * num3 - camPos);
                    (sLeaser.sprites[SmallCordSprite(j)] as TriangleMesh).MoveVertice(k * 4 + 2, vector8 + normalized * num4 - vector9 * num3 - camPos);
                    (sLeaser.sprites[SmallCordSprite(j)] as TriangleMesh).MoveVertice(k * 4 + 3, vector8 + normalized * num4 + vector9 * num3 - camPos);
                    vector7 = vector8;
                }
            }
        }
        
        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[firstSprite].color = owner.armJointGraphics[0].metalColor;
            for (int i = 0; i < coord.GetLength(0); i++)
            {
                sLeaser.sprites[SegmentSprite(i, 0)].color = Color.Lerp(owner.armJointGraphics[0].BaseColor(default(Vector2)), owner.armJointGraphics[0].metalColor, 0.5f);
                sLeaser.sprites[SegmentSprite(i, 1)].color = Color.Lerp(owner.armJointGraphics[0].HighLightColor(default(Vector2)), owner.armJointGraphics[0].metalColor, 0.35f);
            }
            for (int j = 0; j < smallCords.GetLength(0); j++)
            {
                if (smallCoordColors[j] == 0)
                {
                    sLeaser.sprites[SmallCordSprite(j)].color = owner.armJointGraphics[0].metalColor;
                }
                else if (smallCoordColors[j] == 1)
                {
                    sLeaser.sprites[SmallCordSprite(j)].color = Color.Lerp(new Color(1f, 0f, 0f), owner.armJointGraphics[0].metalColor, 0.5f);
                }
                else if (smallCoordColors[j] == 2)
                {
                    sLeaser.sprites[SmallCordSprite(j)].color = Color.Lerp(new Color(0f, 0f, 1f), owner.armJointGraphics[0].metalColor, 0.5f);
                }
            }
        }
        
        private patch_OracleGraphics owner;
        
        public int firstSprite;
        
        public int totalSprites;
        
        public float[] smallCordsLengths;
        
        public Vector2[] smallCordsHeadDirs;
        
        public int[] smallCoordColors;
        
        public Vector2[,] coord;
        
        public Vector2[,,] smallCords;
    }

}

