using RWCustom;
using System.Collections.Generic;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class OracleHK
    {
        public static void Patch()
        {
            On.Oracle.Collide += new On.Oracle.hook_Collide(CollideHK);
            On.Oracle.HitByWeapon += new On.Oracle.hook_HitByWeapon(HitByWeaponHK);
            On.Oracle.ctor += new On.Oracle.hook_ctor(CtorHK);
            On.Oracle.SetUpSwarmers += Oracle_SetUpSwarmers;
            On.Oracle.OracleArm.ctor += new On.Oracle.OracleArm.hook_ctor(ArmCtorHK);
            On.Oracle.OracleArm.Update += new On.Oracle.OracleArm.hook_Update(ArmUpdateHK);
            On.Oracle.OracleArm.BaseDir += new On.Oracle.OracleArm.hook_BaseDir(ArmBaseDirHK);
            On.Oracle.OracleArm.OnFramePos += new On.Oracle.OracleArm.hook_OnFramePos(ArmOnFramePosHK);
            On.Oracle.OracleArm.Joint.Update += new On.Oracle.OracleArm.Joint.hook_Update(JointUpdateHK);
        }

        #region Oracle

        private static void CollideHK(On.Oracle.orig_Collide orig, Oracle self, PhysicalObject otherObject, int myChunk, int otherChunk)
        {
            orig.Invoke(self, otherObject, myChunk, otherChunk); //base Collide
            if (otherObject is Player)
            {
                if (self.oracleBehavior is FPOracleBehaviorHasMark fpOB)
                { // Collide happens every frame, so this should be lower number
                    fpOB.playerAnnoyingCounter = Custom.IntClamp(fpOB.playerAnnoyingCounter + 2, 0, 150);
                }
                else if (self.oracleBehavior is LMOracleBehaviorHasMark lmOB)
                {
                    lmOB.playerAnnoyingCounter = Custom.IntClamp(lmOB.playerAnnoyingCounter + 2, 0, 150);
                }
            }
        }

        private static void HitByWeaponHK(On.Oracle.orig_HitByWeapon orig, Oracle self, Weapon weapon)
        {
            if (weapon.thrownBy is Player)
            {
                if (self.oracleBehavior is FPOracleBehaviorHasMark fpOB)
                {
                    fpOB.playerAnnoyingCounter = Custom.IntClamp(fpOB.playerAnnoyingCounter + 100, 0, 150);
                }
                else if (self.oracleBehavior is LMOracleBehaviorHasMark lmOB)
                {
                    lmOB.playerAnnoyingCounter = Custom.IntClamp(lmOB.playerAnnoyingCounter + 100, 0, 150);
                }
            }
        }

        public static List<MoonPearl> MoonMarbles;

        public static void SetUpMoonMarbles(Oracle self)
        {
            float transX = 1100f, transY = 0f;
            Debug.Log("Drougnt: Setting Up MoonMarbles");

            for (int i = 0; i < 6; i++)
            {
                CreateMoonMarble(self, self, new Vector2(500f + transX, 300f + transY) + Custom.RNV() * 20f, 0, 35f, (i != 2 && i != 3) ? ((i != 5) ? 0 : 2) : 1);
            }
            for (int j = 0; j < 2; j++)
            {
                CreateMoonMarble(self, self, new Vector2(500f + transX, 300f + transY) + Custom.RNV() * 20f, 1, 100f, (j != 1) ? 0 : 2);
            }
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
                        CreateMoonMarble(self, null, new Vector2(temp.x + transX, temp.y + transY), 0, 0f, ((k != 2 || l != 0) && (k != 1 || l != 3)) ? 1 : 2);
                    }
                }
            }
        }

        public static void CreateMoonMarble(Oracle self, PhysicalObject orbitObj, Vector2 ps, int circle, float dist, int color)
        {
            AbstractPhysicalObject abstractPhysicalObject = new MoonPearl.AbstractMoonPearl(self.room.world, null, self.room.GetWorldCoordinate(ps), self.room.game.GetNewID(), -1, -1, null, color, self.pearlCounter);
            self.pearlCounter++;
            self.room.abstractRoom.entities.Add(abstractPhysicalObject);
            MoonPearl MoonPearl = new MoonPearl(abstractPhysicalObject, self.room.world);
            MoonPearl.oracle = self;
            MoonPearl.firstChunk.HardSetPosition(ps);
            MoonPearl.orbitObj = orbitObj;
            if (orbitObj == null) { MoonPearl.hoverPos = new Vector2?(ps); }
            MoonPearl.orbitCircle = circle;
            MoonPearl.orbitDistance = dist;
            MoonPearl.marbleColor = color;
            self.room.AddObject(MoonPearl);
            MoonMarbles.Add(MoonPearl);
        }

        private static void CtorHK(On.Oracle.orig_ctor orig, Oracle self, AbstractPhysicalObject abstractPhysicalObject, Room room)
        {
            orig.Invoke(self, abstractPhysicalObject, room);

            if (self.ID == Oracle.OracleID.SS)
            { // 5P
                self.oracleBehavior = new FPOracleBehaviorHasMark(self);
            }
            else
            { // LTTM
                self.pearlCounter = 0;
                self.health = 1f;
                self.bodyChunks = new BodyChunk[2];
                for (int i = 0; i < self.bodyChunks.Length; i++)
                { self.bodyChunks[i] = new BodyChunk(self, i, new Vector2(1585f + 5f * (float)i, 148f - 5f * (float)i), 6f, 0.5f); }
                self.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[1];
                self.bodyChunkConnections[0] = new PhysicalObject.BodyChunkConnection(self.bodyChunks[0], self.bodyChunks[1], 9f, PhysicalObject.BodyChunkConnection.Type.Normal, 1f, 0.5f);
                self.mySwarmers = new System.Collections.Generic.List<OracleSwarmer>();
                self.airFriction = 0.99f;
                self.gravity = 0f;
                self.bounce = 0.1f;
                self.surfaceFriction = 0.17f;
                self.collisionLayer = 1;
                self.waterFriction = 0.92f;
                self.buoyancy = 0.95f;
                //this.moonmarbles = new List<MoonPearl>();
                //this.SetUpMoonMarbles();
                MoonMarbles = new List<MoonPearl>();
                SetUpMoonMarbles(self);
                self.oracleBehavior = new LMOracleBehaviorHasMark(self);
                self.arm = new Oracle.OracleArm(self);
                room.gravity = 0f;
            }
        }

        private static void Oracle_SetUpSwarmers(On.Oracle.orig_SetUpSwarmers orig, Oracle self)
        {
            self.health = 1f;
        }

        #endregion Oracle

        #region OracleArm

        private static void ArmCtorHK(On.Oracle.OracleArm.orig_ctor orig, Oracle.OracleArm self, Oracle oracle)
        {
            orig.Invoke(self, oracle);
            if (oracle.ID == Oracle.OracleID.SL)
            { self.baseMoveSoundLoop = new StaticSoundLoop(SoundID.SS_AI_Base_Move_LOOP, oracle.firstChunk.pos, oracle.room, 1f, 1f); }
        }

        private static void ArmUpdateHK(On.Oracle.OracleArm.orig_Update orig, Oracle.OracleArm self)
        {
            if (self.oracle.ID == Oracle.OracleID.SL)
            {
                self.oracle.ID = Oracle.OracleID.SS;
                orig.Invoke(self);
                self.oracle.ID = Oracle.OracleID.SL;
            }
            else
            {
                orig.Invoke(self);
            }
        }

        private static Vector2 ArmBaseDirHK(On.Oracle.OracleArm.orig_BaseDir orig, Oracle.OracleArm self, float timeStacker)
        {
            if (self.oracle.ID == Oracle.OracleID.SL)
            {
                self.oracle.ID = Oracle.OracleID.SS;
                Vector2 res = orig.Invoke(self, timeStacker);
                self.oracle.ID = Oracle.OracleID.SL;
                return res;
            }
            else
            {
                return orig.Invoke(self, timeStacker);
            }
        }

        private static Vector2 ArmOnFramePosHK(On.Oracle.OracleArm.orig_OnFramePos orig, Oracle.OracleArm self, float timeStacker)
        {
            if (self.oracle.ID == Oracle.OracleID.SL)
            {
                self.oracle.ID = Oracle.OracleID.SS;
                Vector2 res = orig.Invoke(self, timeStacker);
                self.oracle.ID = Oracle.OracleID.SL;
                return res;
            }
            else
            {
                return orig.Invoke(self, timeStacker);
            }
        }

        private static void JointUpdateHK(On.Oracle.OracleArm.Joint.orig_Update orig, Oracle.OracleArm.Joint self)
        {
            if (self.arm.oracle.ID == Oracle.OracleID.SL)
            {
                self.arm.oracle.ID = Oracle.OracleID.SS;
                orig.Invoke(self);
                self.arm.oracle.ID = Oracle.OracleID.SL;
            }
            else
            {
                orig.Invoke(self);
            }
        }

        #endregion OracleArm
    }
}
