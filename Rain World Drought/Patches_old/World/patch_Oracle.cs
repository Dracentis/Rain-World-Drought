using MonoMod;
using UnityEngine;
using RWCustom;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

class patch_Oracle : Oracle
{
    [MonoModIgnore]
    public patch_Oracle(AbstractPhysicalObject abstractPhysicalObject, Room room) : base(abstractPhysicalObject, room)
    {
    }



    public virtual void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        if (oracleBehavior is FPOracleBehaviorHasMark && otherObject is Player)
        {
            (oracleBehavior as FPOracleBehaviorHasMark).playerAnnoyingCounter = Custom.IntClamp((oracleBehavior as FPOracleBehaviorHasMark).playerAnnoyingCounter + 80, 0, 150);
        }
        else if (oracleBehavior is LMOracleBehaviorHasMark && otherObject is Player)
        {
            (oracleBehavior as LMOracleBehaviorHasMark).playerAnnoyingCounter = Custom.IntClamp((oracleBehavior as LMOracleBehaviorHasMark).playerAnnoyingCounter + 80, 0, 150);
        }
    }

    public virtual void HitByWeapon(Weapon weapon)
    {
        if (oracleBehavior is FPOracleBehaviorHasMark)
        {
            (oracleBehavior as FPOracleBehaviorHasMark).playerAnnoyingCounter = Custom.IntClamp((oracleBehavior as FPOracleBehaviorHasMark).playerAnnoyingCounter + 80, 0, 150);
        }
        else if (oracleBehavior is LMOracleBehaviorHasMark)
        {
            (oracleBehavior as LMOracleBehaviorHasMark).playerAnnoyingCounter = Custom.IntClamp((oracleBehavior as LMOracleBehaviorHasMark).playerAnnoyingCounter + 80, 0, 150);
        }
    }

    [MonoModIgnore]
    patch_Oracle.OracleArm arm;

    [MonoModIgnore]
    private int pearlCounter;

    public List<MoonPearl> moonmarbles;

    public extern void orig_ctor(AbstractPhysicalObject abstractPhysicalObject, Room room);

    [MonoModConstructor]
    public void ctor(AbstractPhysicalObject abstractPhysicalObject, Room room)
    {
        orig_ctor(abstractPhysicalObject, room);
        if (ID == OracleID.SS)
        {
            this.oracleBehavior = new FPOracleBehaviorHasMark(this);
        }
        else
        {
            health = 1f;
            bodyChunks = new BodyChunk[2];
            for (int i = 0; i < bodyChunks.Length; i++)
            {
                bodyChunks[i] = new BodyChunk(this, i, (ID != OracleID.SS) ? new Vector2(1585f + 5f * (float)i, 148f - 5f * (float)i) : new Vector2(350f, 350f), 6f, 0.5f);
            }
            bodyChunkConnections = new PhysicalObject.BodyChunkConnection[1];
            bodyChunkConnections[0] = new PhysicalObject.BodyChunkConnection(bodyChunks[0], bodyChunks[1], 9f, BodyChunkConnection.Type.Normal, 1f, 0.5f);
            mySwarmers = new System.Collections.Generic.List<OracleSwarmer>();
            airFriction = 0.99f;
            gravity = 0f;
            bounce = 0.1f;
            surfaceFriction = 0.17f;
            collisionLayer = 1;
            waterFriction = 0.92f;
            buoyancy = 0.95f;
            //this.moonmarbles = new List<MoonPearl>();
            //this.SetUpMoonMarbles();
            moonmarbles = new List<MoonPearl>();
            SetUpMoonMarbles();
            oracleBehavior = new LMOracleBehaviorHasMark(this);
            arm = new patch_Oracle.OracleArm(this);
            room.gravity = 0f;
        }
    }

    private void SetUpMoonMarbles()
    {
        float TransX = 1100f;
        float TransY = 0f;
        
        Debug.Log("Setting Up Marbles");
        //this.CreateMoonMarble(null, new Vector2(687f+TransX, 318f+TransY), 0, 0f, 1);
        for (int i = 0; i < 6; i++)
        {
            CreateMoonMarble(this, new Vector2(500f + TransX, 300f + TransY) + Custom.RNV() * 20f, 0, 35f, (i != 2 && i != 3) ? ((i != 5) ? 0 : 2) : 1);
        }
        for (int j = 0; j < 2; j++)
        {
            CreateMoonMarble(this, new Vector2(500f + TransX, 300f + TransY) + Custom.RNV() * 20f, 1, 100f, (j != 1) ? 0 : 2);
        }
        //this.CreateMoonMarble(null, new Vector2(220f, 300f), 0, 0f, 1);
        Vector2 a = new Vector2(280f, 130f);
        Vector2 vector = Custom.DegToVec(-32.7346f);
        Vector2 a2 = Custom.PerpendicularVector(vector);
        for (int k = 0; k < 3; k++)
        {
            for (int l = 0; l < 5; l++)
            {
                if (k != 2 || l != 2)
                {
                    Vector2 temp = a + a2 * (float)k * 17f + vector * (float)l * 17f;
                    CreateMoonMarble(null, new Vector2(temp.x+TransX, temp.y+TransY), 0, 0f, ((k != 2 || l != 0) && (k != 1 || l != 3)) ? 1 : 2);
                }
            }
        }
        //this.CreateMoonMarble(null, new Vector2(687f, 318f), 0, 0f, 1);
        //this.CreateMoonMarble(this.marbles[this.marbles.Count - 1], new Vector2(687f, 318f), 0, 18f, 0);
        //this.CreateMoonMarble(null, new Vector2(650f, 567f), 0, 0f, 2);
        //this.CreateMoonMarble(this.marbles[this.marbles.Count - 1], new Vector2(640f, 577f), 0, 38f, 1);
        //this.CreateMoonMarble(this.marbles[this.marbles.Count - 2], new Vector2(640f, 577f), 0, 38f, 2);
        //this.CreateMoonMarble(null, new Vector2(317f + TransX, 100f + TransY), 0, 0f, 2);
        //this.CreateMoonMarble(null, new Vector2(747f + TransX, 474f + TransY), 0, 0f, 0);
        //this.CreateMoonMarble(null, new Vector2(314f + TransX, 600f + TransY), 0, 0f, 2);
        //this.CreateMoonMarble(null, new Vector2(308f + TransX, 611f + TransY), 0, 0f, 2);
        //this.CreateMoonMarble(null, new Vector2(751f + TransX, 231f + TransY), 0, 0f, 1);
        //this.CreateMoonMarble(null, new Vector2(760f + TransX, 224f + TransY), 0, 0f, 1);
        //this.CreateMoonMarble(null, new Vector2(720f + TransX, 234f + TransY), 0, 0f, 0);
        //this.CreateMoonMarble(null, new Vector2(309f + TransX, 452f + TransY), 0, 0f, 0);
        //this.CreateMoonMarble(this.marbles[this.marbles.Count - 1], new Vector2(309f, 452f), 0, 42f, 1);
        //this.moonmarbles[this.marbles.Count - 1].orbitSpeed = 0.8f;
        //this.CreateMoonMarble(this.marbles[this.marbles.Count - 1], new Vector2(309f, 452f), 0, 12f, 0);
    }

    private void CreateMoonMarble(PhysicalObject orbitObj, Vector2 ps, int circle, float dist, int color)
    {
        AbstractPhysicalObject abstractPhysicalObject = new MoonPearl.AbstractMoonPearl(room.world, null, room.GetWorldCoordinate(ps), room.game.GetNewID(), -1, -1, null, color, pearlCounter);
        pearlCounter++;
        room.abstractRoom.entities.Add(abstractPhysicalObject);
        MoonPearl MoonPearl = new MoonPearl(abstractPhysicalObject, room.world);
        MoonPearl.oracle = this;
        MoonPearl.firstChunk.HardSetPosition(ps);
        MoonPearl.orbitObj = orbitObj;
        if (orbitObj == null)
        {
            MoonPearl.hoverPos = new Vector2?(ps);
        }
        MoonPearl.orbitCircle = circle;
        MoonPearl.orbitDistance = dist;
        MoonPearl.marbleColor = color;
        room.AddObject(MoonPearl);
        moonmarbles.Add(MoonPearl);
    }

    private void SetUpSwarmers()
    {
        health = 1f;
    }

    

    [MonoModIgnore]
    private void SetUpMarbles() { }

    public class patch_OracleArm : OracleArm
    {
        public extern void orig_ctor(Oracle oracle);

        [MonoModConstructor]
        public void ctor(Oracle oracle)
        {
            this.oracle = oracle;
            //if (oracle.ID == Oracle.OracleID.SS)
            //{
            baseMoveSoundLoop = new StaticSoundLoop(SoundID.SS_AI_Base_Move_LOOP, oracle.firstChunk.pos, oracle.room, 1f, 1f);
            //}
            cornerPositions = new Vector2[4];
            cornerPositions[0] = oracle.room.MiddleOfTile(10, 31);
            cornerPositions[1] = oracle.room.MiddleOfTile(38, 31);
            cornerPositions[2] = oracle.room.MiddleOfTile(38, 3);
            cornerPositions[3] = oracle.room.MiddleOfTile(10, 3);
            if (oracle.ID == Oracle.OracleID.SL)
            {
                for (int i = 0; i < cornerPositions.Length; i++)
                {
                    cornerPositions[i] += new Vector2(1040f, -20f);
                }
            }
            joints = new patch_Oracle.OracleArm.Joint[4];
            for (int j = 0; j < joints.Length; j++)
            {
                joints[j] = new patch_Oracle.OracleArm.Joint(this, j);
                if (j > 0)
                {
                    joints[j].previous = joints[j - 1];
                    joints[j - 1].next = joints[j];
                }
            }
            framePos = 10002.5f;
            lastFramePos = framePos;
        }

        public Vector2 BasePos(float timeStacker)
        {
            return OnFramePos(Mathf.Lerp(lastFramePos, framePos, timeStacker));
        }

        public void Update()
        {
            oracle.bodyChunks[1].vel *= 0.4f;
            oracle.bodyChunks[0].vel *= 0.4f;
            oracle.bodyChunks[0].vel += Vector2.ClampMagnitude(oracle.oracleBehavior.OracleGetToPos - oracle.bodyChunks[0].pos, 100f) / 100f * 6.2f;
            oracle.bodyChunks[1].vel += Vector2.ClampMagnitude(oracle.oracleBehavior.OracleGetToPos - oracle.oracleBehavior.GetToDir * oracle.bodyChunkConnections[0].distance - oracle.bodyChunks[0].pos, 100f) / 100f * 3.2f;
            Vector2 baseGetToPos = oracle.oracleBehavior.BaseGetToPos;
            Vector2 vector = new Vector2(Mathf.Clamp(baseGetToPos.x, cornerPositions[0].x, cornerPositions[1].x), cornerPositions[0].y);
            float num = Vector2.Distance(vector, baseGetToPos);
            float num2 = Mathf.InverseLerp(cornerPositions[0].x, cornerPositions[1].x, baseGetToPos.x);
            for (int i = 1; i < 4; i++)
            {
                Vector2 vector2;
                if (i % 2 == 0)
                {
                    vector2 = new Vector2(Mathf.Clamp(baseGetToPos.x, cornerPositions[0].x, cornerPositions[1].x), cornerPositions[i].y);
                }
                else
                {
                    vector2 = new Vector2(cornerPositions[i].x, Mathf.Clamp(baseGetToPos.y, cornerPositions[2].y, cornerPositions[0].y));
                }
                float num3 = Vector2.Distance(vector2, baseGetToPos);
                if (num3 < num)
                {
                    vector = vector2;
                    num = num3;
                    if (i == 1)
                    {
                        num2 = (float)i + Mathf.InverseLerp(cornerPositions[0].y, cornerPositions[2].y, baseGetToPos.y);
                    }
                    else if (i == 2)
                    {
                        num2 = (float)i + Mathf.InverseLerp(cornerPositions[1].x, cornerPositions[0].x, baseGetToPos.x);
                    }
                    else if (i == 3)
                    {
                        num2 = (float)i + Mathf.InverseLerp(cornerPositions[2].y, cornerPositions[0].y, baseGetToPos.y);
                    }
                }
            }
            baseMoving = (Vector2.Distance(BasePos(1f), vector) > ((!baseMoving) ? 350f : 50f) && oracle.oracleBehavior.consistentBasePosCounter > 30);
            lastFramePos = framePos;
            if (baseMoving)
            {
                framePos = Mathf.MoveTowardsAngle(framePos * 90f, num2 * 90f, 1f) / 90f;
                if (baseMoveSoundLoop != null)
                {
                    baseMoveSoundLoop.volume = Mathf.Min(baseMoveSoundLoop.volume + 0.1f, 1f);
                    baseMoveSoundLoop.pitch = Mathf.Min(baseMoveSoundLoop.pitch + 0.025f, 1f);
                }
            }
            else if (baseMoveSoundLoop != null)
            {
                baseMoveSoundLoop.volume = Mathf.Max(baseMoveSoundLoop.volume - 0.1f, 0f);
                baseMoveSoundLoop.pitch = Mathf.Max(baseMoveSoundLoop.pitch - 0.025f, 0.5f);
            }
            if (baseMoveSoundLoop != null)
            {
                baseMoveSoundLoop.pos = BasePos(1f);
                baseMoveSoundLoop.Update();
            }

            for (int j = 0; j < joints.Length; j++)
            {
                joints[j].Update();
            }
        }

        public Vector2 BaseDir(float timeStacker)
        {
            //if (this.oracle.ID == Oracle.OracleID.SL)
            //{
            //    return new Vector2(-1f, 0f);
            //}
            float num = Mathf.Lerp(lastFramePos, framePos, timeStacker) % 4f;
            float num2 = 0.1f;
            if (num < num2)
            {
                return Vector3.Slerp(new Vector2(1f, 0f), new Vector2(0f, -1f), 0.5f + Mathf.InverseLerp(0f, num2, num) * 0.5f);
            }
            if (num < 1f - num2)
            {
                return new Vector2(0f, -1f);
            }
            if (num < 1f + num2)
            {
                return Vector3.Slerp(new Vector2(0f, -1f), new Vector2(-1f, 0f), Mathf.InverseLerp(1f - num2, 1f + num2, num));
            }
            if (num < 2f - num2)
            {
                return new Vector2(-1f, 0f);
            }
            if (num < 2f + num2)
            {
                return Vector3.Slerp(new Vector2(-1f, 0f), new Vector2(0f, 1f), Mathf.InverseLerp(2f - num2, 2f + num2, num));
            }
            if (num < 3f - num2)
            {
                return new Vector2(0f, 1f);
            }
            if (num < 3f + num2)
            {
                return Vector3.Slerp(new Vector2(0f, 1f), new Vector2(1f, 0f), Mathf.InverseLerp(3f - num2, 3f + num2, num));
            }
            if (num < 4f - num2)
            {
                return new Vector2(1f, 0f);
            }
            return Vector3.Slerp(new Vector2(1f, 0f), new Vector2(0f, -1f), Mathf.InverseLerp(4f - num2, 4f, num) * 0.5f);
        }

        // Token: 0x060021A6 RID: 8614 RVA: 0x001FBF94 File Offset: 0x001FA194
        public Vector2 OnFramePos(float timeStacker)
        {
            //if (this.oracle.ID == Oracle.OracleID.SL)
            //{
            //    return new Vector2(1810f, 356f);
            //}
            float num = Mathf.Lerp(lastFramePos, framePos, timeStacker) % 4f;
            float num2 = 0.1f;
            float num3 = Mathf.Abs(cornerPositions[0].x - cornerPositions[1].x) * num2;
            Vector2 a = default(Vector2);
            float ang;
            if (num < num2)
            {
                a = new Vector2(cornerPositions[0].x + num3, cornerPositions[1].y - num3);
                ang = -45f + Mathf.InverseLerp(0f, num2, num) * 45f;
            }
            else
            {
                if (num < 1f - num2)
                {
                    return Vector2.Lerp(cornerPositions[0], cornerPositions[1], Mathf.InverseLerp(0f, 1f, num));
                }
                if (num < 1f + num2)
                {
                    a = new Vector2(cornerPositions[1].x - num3, cornerPositions[1].y - num3);
                    ang = Mathf.InverseLerp(1f - num2, 1f + num2, num) * 90f;
                }
                else
                {
                    if (num < 2f - num2)
                    {
                        return Vector2.Lerp(cornerPositions[1], cornerPositions[2], Mathf.InverseLerp(1f, 2f, num));
                    }
                    if (num < 2f + num2)
                    {
                        a = new Vector2(cornerPositions[2].x - num3, cornerPositions[2].y + num3);
                        ang = 90f + Mathf.InverseLerp(2f - num2, 2f + num2, num) * 90f;
                    }
                    else
                    {
                        if (num < 3f - num2)
                        {
                            return Vector2.Lerp(cornerPositions[2], cornerPositions[3], Mathf.InverseLerp(2f, 3f, num));
                        }
                        if (num < 3f + num2)
                        {
                            a = new Vector2(cornerPositions[3].x + num3, cornerPositions[3].y + num3);
                            ang = 180f + Mathf.InverseLerp(3f - num2, 3f + num2, num) * 90f;
                        }
                        else
                        {
                            if (num < 4f - num2)
                            {
                                return Vector2.Lerp(cornerPositions[3], cornerPositions[0], Mathf.InverseLerp(3f, 4f, num));
                            }
                            a = new Vector2(cornerPositions[0].x + num3, cornerPositions[0].y - num3);
                            ang = 270f + Mathf.InverseLerp(4f - num2, 4f, num) * 45f;
                        }
                    }
                }
            }
            return a + Custom.DegToVec(ang) * num3;
        }

        // Token: 0x04002455 RID: 9301
        public Oracle oracle;

        // Token: 0x04002456 RID: 9302
        public patch_Oracle.OracleArm.Joint[] joints;

        // Token: 0x04002457 RID: 9303
        public Vector2[] cornerPositions;

        // Token: 0x04002458 RID: 9304
        public float lastFramePos;

        // Token: 0x04002459 RID: 9305
        public float framePos;

        // Token: 0x0400245A RID: 9306
        public bool baseMoving;

        // Token: 0x0400245B RID: 9307
        public StaticSoundLoop baseMoveSoundLoop;

        [MonoModIgnore]
        public patch_OracleArm(Oracle oracle) : base(oracle)
        {
        }

        public class patch_Joint : Joint
        {
            // Token: 0x060021A7 RID: 8615 RVA: 0x001FC2E4 File Offset: 0x001FA4E4
            public extern void orig_ctor(Oracle.OracleArm arm, int index);

            [MonoModConstructor]
            public void ctor(Oracle.OracleArm arm, int index)
            {
                this.arm = arm;
                this.index = index;
                currentInvKinFlip = ((UnityEngine.Random.value >= 0.5f) ? 1f : -1f);
                switch (index)
                {
                    case 0:
                        totalLength = 300f;
                        break;
                    case 1:
                        totalLength = 150f;
                        break;
                    case 2:
                        totalLength = 90f;
                        break;
                    case 3:
                        totalLength = 30f;
                        break;
                }
                pos = arm.BasePos(1f);
                lastPos = pos;
            }

            // Token: 0x060021A8 RID: 8616 RVA: 0x001FC390 File Offset: 0x001FA590
            public Vector2 ElbowPos(float timeStacker, Vector2 Tip)
            {
                Vector2 vc = Vector2.Lerp(lastPos, pos, timeStacker);
                if (next != null)
                {
                    return Custom.InverseKinematic(Tip, vc, totalLength * 0.333333343f, totalLength * 0.6666667f, (index % 2 != 0) ? -1f : 1f);
                }
                return Custom.InverseKinematic(Tip, vc, totalLength / 3f, totalLength / 3f, (index % 2 != 0) ? -1f : 1f);
            }

            // Token: 0x060021A9 RID: 8617 RVA: 0x001FC424 File Offset: 0x001FA624
            public void Update()
            {
                lastPos = pos;
                pos += vel;
                vel *= 0.8f;
                if ((float)index == 0f)
                {
                    pos = arm.BasePos(1f);
                }
                else if (index < arm.joints.Length - 1)
                {
                    if (index == 1 && (arm.baseMoving || arm.oracle.room.GetTile(previous.ElbowPos(1f, pos)).Solid))
                    {
                        Vector2 vector = Custom.InverseKinematic(previous.pos, next.pos, previous.totalLength, totalLength, currentInvKinFlip);
                        Vector2 from = Custom.InverseKinematic(previous.pos, next.pos, previous.totalLength, totalLength, -currentInvKinFlip);
                        float num = (!arm.oracle.room.GetTile(vector).Solid) ? 0f : 10f;
                        num += ((!arm.oracle.room.GetTile(previous.ElbowPos(1f, Vector2.Lerp(vector, previous.pos, 0.2f))).Solid) ? 0f : 1f);
                        float num2 = (!arm.oracle.room.GetTile(from).Solid) ? 0f : 10f;
                        num2 += ((!arm.oracle.room.GetTile(previous.ElbowPos(1f, Vector2.Lerp(from, previous.pos, 0.2f))).Solid) ? 0f : 1f);
                        if (num > num2)
                        {
                            currentInvKinFlip *= -1f;
                            vector = Custom.InverseKinematic(previous.pos, next.pos, previous.totalLength, totalLength, currentInvKinFlip);
                        }
                        else if (num == 0f)
                        {
                            vel += Vector2.ClampMagnitude(vector - pos, 100f) / 100f * 1.8f;
                        }
                    }
                    SharedPhysics.TerrainCollisionData terrainCollisionData = new SharedPhysics.TerrainCollisionData(pos, lastPos, vel, 1f, new IntVector2(0, 0), true);
                    terrainCollisionData = SharedPhysics.VerticalCollision(arm.oracle.room, terrainCollisionData);
                    terrainCollisionData = SharedPhysics.HorizontalCollision(arm.oracle.room, terrainCollisionData);
                    terrainCollisionData = SharedPhysics.SlopesVertically(arm.oracle.room, terrainCollisionData);
                    pos = terrainCollisionData.pos;
                    vel = terrainCollisionData.vel;
                }
                if (next != null)
                {
                    Vector2 vector2 = Custom.DirVec(pos, next.pos);
                    float num3 = Vector2.Distance(pos, next.pos);
                    float num4 = 0.5f;
                    if (index == 0)
                    {
                        num4 = 0f;
                    }
                    else if (index == arm.joints.Length - 2)
                    {
                        num4 = 1f;
                    }
                    float num5 = -1f;
                    float num6 = 0.5f;
                    if (previous != null)
                    {
                        Vector2 lhs = Custom.DirVec(previous.pos, pos);
                        num6 = Custom.LerpMap(Vector2.Dot(lhs, vector2), -1f, 1f, 1f, 0.2f);
                    }
                    if (num3 > totalLength)
                    {
                        num5 = totalLength;
                    }
                    else if (num3 < totalLength * num6)
                    {
                        num5 = totalLength * num6;
                    }
                    if (num5 > 0f)
                    {
                        pos += vector2 * (num3 - num5) * num4;
                        vel += vector2 * (num3 - num5) * num4;
                        next.vel -= vector2 * (num3 - num5) * (1f - num4);
                    }
                }
                else
                {
                    Vector2 a = arm.oracle.bodyChunks[1].pos;
                    //if (this.arm.oracle.ID == Oracle.OracleID.SS)
                    //{
                    a -= arm.oracle.oracleBehavior.GetToDir * totalLength / 2f;
                    //}
                    //else
                    //{
                    //    a -= Custom.PerpendicularVector(this.arm.oracle.oracleBehavior.GetToDir) * this.totalLength / 2f;
                    //}
                    a += Custom.DirVec(arm.oracle.bodyChunks[1].pos, pos) * totalLength / 2f;
                    vel += Vector2.ClampMagnitude(a - pos, 50f) / 50f * 1.2f;
                    pos += Vector2.ClampMagnitude(a - pos, 50f) / 50f * 1.2f;
                    Vector2 a2 = Custom.DirVec(pos, arm.oracle.bodyChunks[0].pos);
                    float num7 = Vector2.Distance(pos, arm.oracle.bodyChunks[0].pos);
                    pos += a2 * (num7 - totalLength);
                    vel += a2 * (num7 - totalLength);
                }
            }

            // Token: 0x0400245C RID: 9308
            public patch_Oracle.OracleArm arm;

            // Token: 0x0400245D RID: 9309
            public patch_Oracle.OracleArm.Joint previous;

            // Token: 0x0400245E RID: 9310
            public patch_Oracle.OracleArm.Joint next;

            // Token: 0x0400245F RID: 9311
            public int index;

            // Token: 0x04002460 RID: 9312
            public Vector2 pos;

            // Token: 0x04002461 RID: 9313
            public Vector2 lastPos;

            // Token: 0x04002462 RID: 9314
            public Vector2 vel;

            // Token: 0x04002463 RID: 9315
            public float totalLength;

            // Token: 0x04002464 RID: 9316
            public float currentInvKinFlip;

            [MonoMod.MonoModIgnore]
            public patch_Joint(Oracle.OracleArm arm, int index) : base(arm, index)
            {
            }
        }
    }



}
