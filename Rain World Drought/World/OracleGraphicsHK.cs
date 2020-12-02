using RWCustom;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class OracleGraphicsHK
    {
        public static void Patch()
        {
            On.OracleGraphics.ctor += new On.OracleGraphics.hook_ctor(CtorHK);
            On.OracleGraphics.InitiateSprites += new On.OracleGraphics.hook_InitiateSprites(InitiateSpritesHK);
            On.OracleGraphics.AddToContainer += new On.OracleGraphics.hook_AddToContainer(AddToContainerHK);
            On.OracleGraphics.Update += new On.OracleGraphics.hook_Update(UpdateHK);
            On.OracleGraphics.DrawSprites += new On.OracleGraphics.hook_DrawSprites(DrawSpritesHK);
            On.OracleGraphics.ApplyPalette += new On.OracleGraphics.hook_ApplyPalette(ApplyPaletteHK);
            On.OracleGraphics.UbilicalCord.SetStuckSegments += new On.OracleGraphics.UbilicalCord.hook_SetStuckSegments(UCSetStuckSegmentsHK);
        }

        private static void CtorHK(On.OracleGraphics.orig_ctor orig, OracleGraphics self, PhysicalObject ow)
        {
            orig.Invoke(self, ow);
            if (self.IsMoon)
            {
                self.totalSprites = 0;
                self.armJointGraphics = new OracleGraphics.ArmJointGraphics[self.oracle.arm.joints.Length];
                for (int i = 0; i < self.oracle.arm.joints.Length; i++)
                {
                    self.armJointGraphics[i] = new OracleGraphics.ArmJointGraphics(self, self.oracle.arm.joints[i], self.totalSprites);
                    self.totalSprites += self.armJointGraphics[i].totalSprites;
                }
                self.firstUmbilicalSprite = self.totalSprites;
                self.umbCord = new OracleGraphics.UbilicalCord(self, self.totalSprites);
                self.totalSprites += self.umbCord.totalSprites;
                self.discUmbCord = null;
                self.firstBodyChunkSprite = self.totalSprites;
                self.totalSprites += 2;
                self.neckSprite = self.totalSprites;
                self.totalSprites++;
                self.firstFootSprite = self.totalSprites;
                self.totalSprites += 4;
                self.halo = new OracleGraphics.Halo(self, self.totalSprites);
                self.totalSprites += self.halo.totalSprites;
                self.gown = new OracleGraphics.Gown(self);
                self.robeSprite = self.totalSprites;
                self.totalSprites++;
                self.firstHandSprite = self.totalSprites;
                self.totalSprites += 4;
                self.head = new GenericBodyPart(self, 5f, 0.5f, 0.995f, self.oracle.firstChunk);
                self.firstHeadSprite = self.totalSprites;
                self.totalSprites += 10;
                self.fadeSprite = self.totalSprites;
                self.totalSprites++;
                self.totalSprites++;
                self.killSprite = self.totalSprites;
                self.totalSprites++;
                self.hands = new GenericBodyPart[2];
                for (int j = 0; j < 2; j++)
                { self.hands[j] = new GenericBodyPart(self, 2f, 0.5f, 0.98f, self.oracle.firstChunk); }
                self.feet = new GenericBodyPart[2];
                for (int k = 0; k < 2; k++)
                { self.feet[k] = new GenericBodyPart(self, 2f, 0.5f, 0.98f, self.oracle.firstChunk); }
                self.knees = new Vector2[2, 2];
                for (int l = 0; l < 2; l++)
                {
                    for (int m = 0; m < 2; m++)
                    { self.knees[l, m] = self.oracle.firstChunk.pos; }
                }
                self.firstArmBaseSprite = self.totalSprites;
                self.armBase = new OracleGraphics.ArmBase(self, self.firstArmBaseSprite);
                self.totalSprites += self.armBase.totalSprites;
                self.voiceFreqSamples = new float[64];
            }
        }

        private static void InitiateSpritesHK(On.OracleGraphics.orig_InitiateSprites orig, OracleGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);
            if (self.oracle.ID == Oracle.OracleID.SL)
            {
                sLeaser.sprites[self.killSprite] = new FSprite("MoonMark", true);
                sLeaser.sprites[self.killSprite].scale = 1f;
                sLeaser.sprites[self.killSprite].scaleX = 1f;
                sLeaser.sprites[self.killSprite].scaleY = 1f;
                sLeaser.sprites[self.killSprite].alpha = 1f;
            }
            self.AddToContainer(sLeaser, rCam, null);
        }

        private static void AddToContainerHK(On.OracleGraphics.orig_AddToContainer orig, OracleGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
        {
            // Skip adding to container if not all sprites have been initialized
            for(int i = 0; i < sLeaser.sprites.Length; i++)
                if (sLeaser.sprites[i] == null) return;
            orig(self, sLeaser, rCam, newContainer);
        }

        private static void UpdateHK(On.OracleGraphics.orig_Update orig, OracleGraphics self)
        {
            //Delegate to call the base Update
            Type[] updateSignature = new Type[0];
            RuntimeMethodHandle handle = typeof(GraphicsModule).GetMethod("Update", updateSignature).MethodHandle;
            RuntimeHelpers.PrepareMethod(handle);
            IntPtr ptr = handle.GetFunctionPointer();
            Action funct = (Action)Activator.CreateInstance(typeof(Action), self, ptr);
            funct();//GraphicsModule Update Method (I hope self works)

            self.breathe += 1f / Mathf.Lerp(10f, 60f, self.oracle.health);
            self.lastBreatheFac = self.breathFac;
            self.breathFac = Mathf.Lerp(0.5f + 0.5f * Mathf.Sin(self.breathe * 3.14159274f * 2f), 1f, Mathf.Pow(self.oracle.health, 2f));
            if (self.gown != null) { self.gown.Update(); }
            if (self.halo != null) { self.halo.Update(); }
            self.armBase.Update();
            self.lastLookDir = self.lookDir;
            if (self.oracle.Consious)
            {
                self.lookDir = Vector2.ClampMagnitude(self.oracle.oracleBehavior.lookPoint - self.oracle.firstChunk.pos, 100f) / 100f;
                self.lookDir = Vector2.ClampMagnitude(self.lookDir + self.randomTalkVector * self.averageVoice * 0.3f, 1f);
            }
            self.head.Update();
            self.head.ConnectToPoint(self.oracle.firstChunk.pos + Custom.DirVec(self.oracle.bodyChunks[1].pos, self.oracle.firstChunk.pos) * 6f, 8f, true, 0f, self.oracle.firstChunk.vel, 0.5f, 0.01f);
            if (self.oracle.Consious)
            {
                self.head.vel += Custom.DirVec(self.oracle.bodyChunks[1].pos, self.oracle.firstChunk.pos) * self.breathFac;
                self.head.vel += self.lookDir * 0.5f * self.breathFac;
            }
            else
            {
                self.head.vel += Custom.DirVec(self.oracle.bodyChunks[1].pos, self.oracle.firstChunk.pos) * 0.75f;
                self.head.vel.y -= 0.7f;
            }
            for (int i = 0; i < 2; i++)
            {
                self.feet[i].Update();
                self.feet[i].ConnectToPoint(self.oracle.bodyChunks[1].pos, (self.oracle.ID != Oracle.OracleID.SL) ? 10f : 20f, false, 0f, self.oracle.bodyChunks[1].vel, 0.3f, 0.01f);
                if (self.oracle.ID == Oracle.OracleID.SL) { self.feet[i].vel.y -= 0.5f; }
                self.feet[i].vel += Custom.DirVec(self.oracle.firstChunk.pos, self.oracle.bodyChunks[1].pos) * 0.3f;
                self.feet[i].vel += Custom.PerpendicularVector(Custom.DirVec(self.oracle.firstChunk.pos, self.oracle.bodyChunks[1].pos)) * 0.15f * ((i != 0) ? 1f : -1f);
                self.hands[i].Update();
                self.hands[i].ConnectToPoint(self.oracle.firstChunk.pos, 15f, false, 0f, self.oracle.firstChunk.vel, 0.3f, 0.01f);
                self.hands[i].vel.y -= 0.5f;
                self.hands[i].vel += Custom.DirVec(self.oracle.firstChunk.pos, self.oracle.bodyChunks[1].pos) * 0.3f;
                self.hands[i].vel += Custom.PerpendicularVector(Custom.DirVec(self.oracle.firstChunk.pos, self.oracle.bodyChunks[1].pos)) * 0.3f * ((i != 0) ? 1f : -1f);
                self.knees[i, 1] = self.knees[i, 0];
                self.knees[i, 0] = (self.feet[i].pos + self.oracle.bodyChunks[1].pos) / 2f + Custom.PerpendicularVector(Custom.DirVec(self.oracle.firstChunk.pos, self.oracle.bodyChunks[1].pos)) * 4f * ((i != 0) ? 1f : -1f);
                if (self.oracle.ID == Oracle.OracleID.SS)
                {
                    self.hands[i].vel += self.randomTalkVector * self.averageVoice * 0.8f;
                    if (i == 0 && (self.oracle.oracleBehavior as FPOracleBehaviorHasMark).HandTowardsPlayer())
                    { self.hands[0].vel += Custom.DirVec(self.hands[0].pos, self.oracle.oracleBehavior.player.mainBodyChunk.pos) * 3f; }
                    self.knees[i, 0] = (self.feet[i].pos + self.oracle.bodyChunks[1].pos) / 2f + Custom.PerpendicularVector(Custom.DirVec(self.oracle.firstChunk.pos, self.oracle.bodyChunks[1].pos)) * 4f * ((i != 0) ? 1f : -1f);
                }
                else
                {
                    /*
                    Vector2? vector = null;
                    Vector2? vector2 = SharedPhysics.ExactTerrainRayTracePos(self.oracle.room, self.oracle.bodyChunks[(!(self.oracle.oracleBehavior as LMOracleBehavior).InSitPosition) ? 1 : 0].pos, self.oracle.bodyChunks[(!(self.oracle.oracleBehavior as LMOracleBehavior).InSitPosition) ? 1 : 0].pos + new Vector2((i != 0) ? -24f : -54f, -40f) * 2f * 1f);

                     if (vector2 != null)
                     {
                        //self.feet[i].vel += Vector2.ClampMagnitude(vector2.Value - self.feet[i].pos, 10f) / 2f;
                     }
                    Vector2 vector3 = (self.feet[i].pos + self.oracle.bodyChunks[1].pos) / 2f;
                    vector3 += Custom.PerpendicularVector(self.oracle.bodyChunks[1].pos, self.oracle.bodyChunks[0].pos) * ((i != 0) ? 1f : -1f) * 5f;

                    //self.knees[i, 0] = Vector2.Lerp(self.knees[i, 0], vector3, 0.4f);
                    if (!Custom.DistLess(self.knees[i, 0], vector3, 15f))
                    {
                        //self.knees[i, 0] = vector3 + Custom.DirVec(vector3, self.knees[i, 0]);
                    }
                    if (self.oracle.Consious && i == 0 && (self.oracle.oracleBehavior as LMOracleBehavior).holdingObject != null)
                    {
                        //self.hands[i].pos = (self.oracle.oracleBehavior as LMOracleBehavior).holdingObject.firstChunk.pos;
                        //self.hands[i].vel *= 0f;
                    }
                    if (i == 0 == oracle.firstChunk.pos.x > oracle.oracleBehavior.player.DangerPos.x && Custom.DistLess(oracle.firstChunk.pos, oracle.oracleBehavior.player.DangerPos, 40f))
                    {
                        //self.hands[i].vel = Vector2.Lerp(self.hands[i].vel, Custom.DirVec(self.hands[i].pos, self.oracle.oracleBehavior.player.mainBodyChunk.pos) * 10f, 0.5f);
                    }
                    else if (i == 0 == oracle.firstChunk.pos.x > oracle.oracleBehavior.player.DangerPos.x && (oracle.oracleBehavior as LMOracleBehavior).armsProtest)
                    {
                        //self.hands[i].vel = Vector2.Lerp(self.hands[i].vel, Custom.DirVec(self.hands[i].pos, self.oracle.oracleBehavior.player.mainBodyChunk.pos) * 10f, 0.5f) + new Vector2(0f, 10f * Mathf.Sin((self.oracle.oracleBehavior as LMOracleBehavior).protestCounter * 3.14159274f * 2f));
                    }
                    else if (!(self.oracle.oracleBehavior as LMOracleBehavior).InSitPosition)
                    {
                        //vector2 = SharedPhysics.ExactTerrainRayTracePos(self.oracle.room, self.oracle.firstChunk.pos, self.oracle.firstChunk.pos + new Vector2(((i != 0) ? 1f : -1f) * (self.oracle.oracleBehavior as LMOracleBehavior).Crawl * 40f, -40f));
                        if (vector2 != null)
                        {
                            //    self.hands[i].vel += Vector2.ClampMagnitude(vector2.Value - self.hands[i].pos, 10f) / 3f;
                        }
                        else
                        {
                            //   GenericBodyPart genericBodyPart4 = self.hands[i];
                            //   genericBodyPart4.vel.x = genericBodyPart4.vel.x + ((i != 0) ? 1f : -1f) * (self.oracle.oracleBehavior as LMOracleBehavior).Crawl;
                        }
                        //self.knees[i, 0] = self.feet[i].pos + Custom.DirVec(self.feet[i].pos, self.oracle.oracleBehavior.OracleGetToPos + new Vector2(-50f, 0f)) * 8f + Custom.PerpendicularVector(self.oracle.bodyChunks[1].pos, self.oracle.bodyChunks[0].pos) * ((i != 0) ? 1f : -1f) * Mathf.Lerp(2f, 6f, (self.oracle.oracleBehavior as LMOracleBehavior).CrawlSpeed);
                    } */
                }
            }
            for (int j = 0; j < self.armJointGraphics.Length; j++) { self.armJointGraphics[j].Update(); }
            if (self.umbCord != null) { self.umbCord.Update(); }
            else if (self.discUmbCord != null) { self.discUmbCord.Update(); }
            if (self.oracle.oracleBehavior.voice != null && self.oracle.oracleBehavior.voice.currentAudioSource != null && self.oracle.oracleBehavior.voice.currentAudioSource.isPlaying)
            {
                self.oracle.oracleBehavior.voice.currentAudioSource.GetSpectrumData(self.voiceFreqSamples, 0, FFTWindow.BlackmanHarris);
                self.averageVoice = 0f;
                for (int k = 0; k < self.voiceFreqSamples.Length; k++)
                { self.averageVoice += self.voiceFreqSamples[k]; }
                self.averageVoice /= (float)self.voiceFreqSamples.Length;
                self.averageVoice = Mathf.InverseLerp(0f, 0.00014f, self.averageVoice);
                if (self.averageVoice > 0.7f && UnityEngine.Random.value < self.averageVoice / 14f)
                { self.randomTalkVector = Custom.RNV(); }
            }
            else
            {
                self.randomTalkVector *= 0.9f;
                if (self.averageVoice > 0f)
                {
                    for (int l = 0; l < self.voiceFreqSamples.Length; l++)
                    { self.voiceFreqSamples[l] = 0f; }
                    self.averageVoice = 0f;
                }
            }
            self.lastEyesOpen = self.eyesOpen;
            self.eyesOpen = ((!self.oracle.oracleBehavior.EyesClosed) ? 1f : 0f);
            if (self.oracle.ID == Oracle.OracleID.SS)
            {
                if (self.lightsource == null)
                {
                    self.lightsource = new LightSource(self.oracle.firstChunk.pos, false, Custom.HSL2RGB(0.1f, 1f, 0.5f), self.oracle);
                    self.lightsource.requireUpKeep = true;
                    self.lightsource.affectedByPaletteDarkness = 0f;
                    self.oracle.room.AddObject(self.lightsource);
                }
                else
                {
                    self.lightsource.setAlpha = new float?((self.oracle.oracleBehavior as FPOracleBehaviorHasMark).working);
                    self.lightsource.setRad = new float?(400f);
                    self.lightsource.setPos = new Vector2?(self.oracle.firstChunk.pos);
                }
            }
            else
            {
                if (self.lightsource == null)
                {
                    self.lightsource = new LightSource(self.oracle.firstChunk.pos, false, Custom.HSL2RGB(0.1012423f, 0.257576f, 0.91322334f), self.oracle);
                    self.lightsource.requireUpKeep = true;
                    self.lightsource.affectedByPaletteDarkness = 0f;
                    self.oracle.room.AddObject(self.lightsource);
                }
                else
                {
                    self.lightsource.setAlpha = 1f;
                    self.lightsource.setRad = new float?(100f);
                    self.lightsource.setPos = new Vector2?(self.oracle.firstChunk.pos);
                }
            }
            if (self.lightsource != null) self.lightsource.stayAlive = true;
        }

        private static void DrawSpritesHK(On.OracleGraphics.orig_DrawSprites orig, OracleGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 pos0 = Vector2.Lerp(self.owner.firstChunk.lastPos, self.owner.firstChunk.pos, timeStacker);
            Vector2 pos1 = Custom.DirVec(Vector2.Lerp(self.owner.bodyChunks[1].lastPos, self.owner.bodyChunks[1].pos, timeStacker), pos0);
            Vector2 ppd1 = Custom.PerpendicularVector(pos1);
            Vector2 look = Vector2.Lerp(self.lastLookDir, self.lookDir, timeStacker);
            Vector2 head = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker);
            for (int i = 0; i < self.owner.bodyChunks.Length; i++)
            {
                sLeaser.sprites[self.firstBodyChunkSprite + i].x = Mathf.Lerp(self.owner.bodyChunks[i].lastPos.x, self.owner.bodyChunks[i].pos.x, timeStacker) - camPos.x;
                sLeaser.sprites[self.firstBodyChunkSprite + i].y = Mathf.Lerp(self.owner.bodyChunks[i].lastPos.y, self.owner.bodyChunks[i].pos.y, timeStacker) - camPos.y;
            }
            sLeaser.sprites[self.firstBodyChunkSprite].rotation = Custom.AimFromOneVectorToAnother(pos0, head) - Mathf.Lerp(14f, 0f, Mathf.Lerp(self.lastBreatheFac, self.breathFac, timeStacker));
            sLeaser.sprites[self.firstBodyChunkSprite + 1].rotation = Custom.VecToDeg(pos1);
            for (int j = 0; j < self.armJointGraphics.Length; j++)
            {
                self.armJointGraphics[j].DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            if (self.oracle.ID == Oracle.OracleID.SS)
            {
                if ((self.oracle.oracleBehavior as FPOracleBehaviorHasMark).killFac > 0f)
                {
                    sLeaser.sprites[self.killSprite].isVisible = true;
                    sLeaser.sprites[self.killSprite].x = Mathf.Lerp(self.oracle.oracleBehavior.player.mainBodyChunk.lastPos.x, self.oracle.oracleBehavior.player.mainBodyChunk.pos.x, timeStacker) - camPos.x;
                    sLeaser.sprites[self.killSprite].y = Mathf.Lerp(self.oracle.oracleBehavior.player.mainBodyChunk.lastPos.y, self.oracle.oracleBehavior.player.mainBodyChunk.pos.y, timeStacker) - camPos.y;
                    float f = Mathf.Lerp((self.oracle.oracleBehavior as FPOracleBehaviorHasMark).lastKillFac, (self.oracle.oracleBehavior as FPOracleBehaviorHasMark).killFac, timeStacker);
                    sLeaser.sprites[self.killSprite].scale = Mathf.Lerp(200f, 2f, Mathf.Pow(f, 0.5f));
                    sLeaser.sprites[self.killSprite].alpha = Mathf.Pow(f, 3f);
                }
                else
                {
                    sLeaser.sprites[self.killSprite].isVisible = false;
                }
            }
            sLeaser.sprites[self.fadeSprite].x = head.x - camPos.x;
            sLeaser.sprites[self.fadeSprite].y = head.y - camPos.y;
            sLeaser.sprites[self.neckSprite].x = pos0.x - camPos.x;
            sLeaser.sprites[self.neckSprite].y = pos0.y - camPos.y;
            sLeaser.sprites[self.neckSprite].rotation = Custom.AimFromOneVectorToAnother(pos0, head);
            sLeaser.sprites[self.neckSprite].scaleY = Vector2.Distance(pos0, head);
            if (self.gown != null)
            {
                self.gown.DrawSprite(self.robeSprite, sLeaser, rCam, timeStacker, camPos);
            }
            if (self.halo != null)
            {
                self.halo.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            self.armBase.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            Vector2 vector4 = Custom.DirVec(head, pos0);
            Vector2 a3 = Custom.PerpendicularVector(vector4);
            sLeaser.sprites[self.HeadSprite].x = head.x - camPos.x;
            sLeaser.sprites[self.HeadSprite].y = head.y - camPos.y;
            sLeaser.sprites[self.HeadSprite].rotation = Custom.VecToDeg(vector4);
            Vector2 vector5 = self.RelativeLookDir(timeStacker);
            Vector2 a4 = Vector2.Lerp(head, pos0, 0.15f);
            a4 += a3 * vector5.x * 2f;
            sLeaser.sprites[self.ChinSprite].x = a4.x - camPos.x;
            sLeaser.sprites[self.ChinSprite].y = a4.y - camPos.y;
            float num = Mathf.Lerp(self.lastEyesOpen, self.eyesOpen, timeStacker);
            for (int k = 0; k < 2; k++)
            {
                float num2 = (k != 0) ? 1f : -1f;
                Vector2 vector6 = head + a3 * Mathf.Clamp(vector5.x * 3f + 2.5f * num2, -5f, 5f) + vector4 * (1f - vector5.y * 3f);
                sLeaser.sprites[self.EyeSprite(k)].rotation = Custom.VecToDeg(vector4);
                sLeaser.sprites[self.EyeSprite(k)].scaleX = 1f + ((k != 0) ? Mathf.InverseLerp(1f, 0.5f, vector5.x) : Mathf.InverseLerp(-1f, -0.5f, vector5.x)) + (1f - num);
                sLeaser.sprites[self.EyeSprite(k)].scaleY = Mathf.Lerp(1f, (self.oracle.ID != Oracle.OracleID.SS) ? 3f : 2f, num);
                sLeaser.sprites[self.EyeSprite(k)].x = vector6.x - camPos.x;
                sLeaser.sprites[self.EyeSprite(k)].y = vector6.y - camPos.y;
                sLeaser.sprites[self.EyeSprite(k)].alpha = 0.5f + 0.5f * num;
                int side = (k < 1 != vector5.x < 0f) ? 1 : 0;
                Vector2 a5 = head + a3 * Mathf.Clamp(Mathf.Lerp(7f, 5f, Mathf.Abs(vector5.x)) * num2, -11f, 11f);
                for (int l = 0; l < 2; l++)
                {
                    sLeaser.sprites[self.PhoneSprite(side, l)].rotation = Custom.VecToDeg(vector4);
                    sLeaser.sprites[self.PhoneSprite(side, l)].scaleY = 5.5f * ((l != 0) ? 0.8f : 1f) / 20f;
                    sLeaser.sprites[self.PhoneSprite(side, l)].scaleX = Mathf.Lerp(3.5f, 5f, Mathf.Abs(vector5.x)) * ((l != 0) ? 0.8f : 1f) / 20f;
                    sLeaser.sprites[self.PhoneSprite(side, l)].x = a5.x - camPos.x;
                    sLeaser.sprites[self.PhoneSprite(side, l)].y = a5.y - camPos.y;
                }
                sLeaser.sprites[self.PhoneSprite(side, 2)].x = a5.x - camPos.x;
                sLeaser.sprites[self.PhoneSprite(side, 2)].y = a5.y - camPos.y;
                sLeaser.sprites[self.PhoneSprite(side, 2)].rotation = Custom.AimFromOneVectorToAnother(pos0, a5 - vector4 * 40f - look * 10f);
                Vector2 vector7 = Vector2.Lerp(self.hands[k].lastPos, self.hands[k].pos, timeStacker);
                Vector2 vector8 = pos0 + ppd1 * 4f * ((k != 1) ? 1f : -1f);
                if (self.oracle.ID == Oracle.OracleID.SL)
                {
                    vector8 += pos1 * 3f;
                }
                Vector2 cB = vector7 + Custom.DirVec(vector7, vector8) * 3f + pos1;
                Vector2 cA = vector8 + ppd1 * 5f * ((k != 1) ? 1f : -1f);
                sLeaser.sprites[self.HandSprite(k, 0)].x = vector7.x - camPos.x;
                sLeaser.sprites[self.HandSprite(k, 0)].y = vector7.y - camPos.y;
                Vector2 vector9 = vector8 - ppd1 * 2f * ((k != 1) ? 1f : -1f);
                float num3 = (self.oracle.ID != Oracle.OracleID.SS) ? 2f : 4f;
                for (int m = 0; m < 7; m++)
                {
                    float f2 = (float)m / 6f;
                    Vector2 vector10 = Custom.Bezier(vector8, cA, vector7, cB, f2);
                    Vector2 vector11 = Custom.DirVec(vector9, vector10);
                    Vector2 a6 = Custom.PerpendicularVector(vector11) * ((k != 0) ? 1f : -1f);
                    float d = Vector2.Distance(vector9, vector10);
                    (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).MoveVertice(m * 4, vector10 - vector11 * d * 0.3f - a6 * num3 - camPos);
                    (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).MoveVertice(m * 4 + 1, vector10 - vector11 * d * 0.3f + a6 * num3 - camPos);
                    (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).MoveVertice(m * 4 + 2, vector10 - a6 * num3 - camPos);
                    (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).MoveVertice(m * 4 + 3, vector10 + a6 * num3 - camPos);
                    vector9 = vector10;
                }
                vector7 = Vector2.Lerp(self.feet[k].lastPos, self.feet[k].pos, timeStacker);
                vector8 = Vector2.Lerp(self.oracle.bodyChunks[1].lastPos, self.oracle.bodyChunks[1].pos, timeStacker);
                Vector2 to = Vector2.Lerp(self.knees[k, 1], self.knees[k, 0], timeStacker);
                cB = Vector2.Lerp(vector7, to, 0.9f);
                cA = Vector2.Lerp(vector8, to, 0.9f);
                sLeaser.sprites[self.FootSprite(k, 0)].x = vector7.x - camPos.x;
                sLeaser.sprites[self.FootSprite(k, 0)].y = vector7.y - camPos.y;
                vector9 = vector8 - ppd1 * 2f * ((k != 1) ? 1f : -1f);
                float num4 = 4f;
                for (int n = 0; n < 7; n++)
                {
                    float f3 = (float)n / 6f;
                    num3 = ((self.oracle.ID != Oracle.OracleID.SS) ? Mathf.Lerp(4f, 2f, Mathf.Pow(f3, 0.5f)) : 2f);
                    Vector2 vector12 = Custom.Bezier(vector8, cA, vector7, cB, f3);
                    Vector2 vector13 = Custom.DirVec(vector9, vector12);
                    Vector2 a7 = Custom.PerpendicularVector(vector13) * ((k != 0) ? 1f : -1f);
                    float d2 = Vector2.Distance(vector9, vector12);
                    (sLeaser.sprites[self.FootSprite(k, 1)] as TriangleMesh).MoveVertice(n * 4, vector12 - vector13 * d2 * 0.3f - a7 * (num4 + num3) * 0.5f - camPos);
                    (sLeaser.sprites[self.FootSprite(k, 1)] as TriangleMesh).MoveVertice(n * 4 + 1, vector12 - vector13 * d2 * 0.3f + a7 * (num4 + num3) * 0.5f - camPos);
                    (sLeaser.sprites[self.FootSprite(k, 1)] as TriangleMesh).MoveVertice(n * 4 + 2, vector12 - a7 * num3 - camPos);
                    (sLeaser.sprites[self.FootSprite(k, 1)] as TriangleMesh).MoveVertice(n * 4 + 3, vector12 + a7 * num3 - camPos);
                    vector9 = vector12;
                    num4 = num3;
                }
            }
            if (self.IsMoon)
            {
                Vector2 p = head + a3 * vector5.x * 2.5f + vector4 * (-2f - vector5.y * 1.5f);
                sLeaser.sprites[self.MoonThirdEyeSprite].x = p.x - camPos.x;
                sLeaser.sprites[self.MoonThirdEyeSprite].y = p.y - camPos.y;
                sLeaser.sprites[self.MoonThirdEyeSprite].rotation = Custom.AimFromOneVectorToAnother(p, head - vector4 * 10f);
                sLeaser.sprites[self.MoonThirdEyeSprite].scaleX = Mathf.Lerp(0.2f, 0.15f, Mathf.Abs(vector5.x));
                sLeaser.sprites[self.MoonThirdEyeSprite].scaleY = Custom.LerpMap(vector5.y, 0f, 1f, 0.2f, 0.05f);
            }
            if (self.umbCord != null)
            {
                self.umbCord.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            else if (self.discUmbCord != null)
            {
                self.discUmbCord.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            if (self.IsMoon)
            {
                Vector2 vector7 = Vector2.Lerp(self.owner.firstChunk.lastPos, self.owner.firstChunk.pos, timeStacker);
                Vector2 vector72 = Custom.DirVec(Vector2.Lerp(self.owner.bodyChunks[1].lastPos, self.owner.bodyChunks[1].pos, timeStacker), vector7);
                Vector2 a7 = Custom.PerpendicularVector(vector72);
                Vector2 a72 = Vector2.Lerp(self.lastLookDir, self.lookDir, timeStacker);
                Vector2 vector73 = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker);
                Vector2 vector74 = Custom.DirVec(vector73, vector7);
                Vector2 up = Custom.DirVec(vector7, vector73);
                Vector2 a73 = Custom.PerpendicularVector(vector74);
                Vector2 vector75 = self.RelativeLookDir(timeStacker);
                Vector2 p = vector73 + a73 * vector75.x * 2.5f + vector74 * (-2f - vector75.y * 1.5f);
                sLeaser.sprites[self.killSprite].x = p.x - camPos.x + (up.x * 5f);
                sLeaser.sprites[self.killSprite].y = p.y - camPos.y + (up.y * 5f);
                sLeaser.sprites[self.killSprite].rotation = Custom.AimFromOneVectorToAnother(p, vector73 - vector74 * 10f);
                sLeaser.sprites[self.killSprite].scaleX = Mathf.Lerp(0.2f, 0.15f, Mathf.Abs(vector75.x));
                sLeaser.sprites[self.killSprite].scaleY = Custom.LerpMap(vector75.y, 0f, 1f, 0.2f, 0.05f);
                sLeaser.sprites[self.killSprite].scale = 1f;
                sLeaser.sprites[self.killSprite].scaleX = 1f;
                sLeaser.sprites[self.killSprite].scaleY = 1f;
                //Vector2 vector = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker);
                //sLeaser.sprites[killSprite].x = vector.x - camPos.x;
                //sLeaser.sprites[killSprite].y = vector.y - camPos.y;
                sLeaser.sprites[self.killSprite].isVisible = true;
            }
            //Delegate to call the self DrawSprites
            Type[] selfSignature = new Type[4] { typeof(RoomCamera.SpriteLeaser), typeof(RoomCamera), typeof(float), typeof(Vector2) };
            RuntimeMethodHandle handle = typeof(GraphicsModule).GetMethod("DrawSprites", selfSignature).MethodHandle;
            RuntimeHelpers.PrepareMethod(handle);
            IntPtr ptr = handle.GetFunctionPointer();
            Action<RoomCamera.SpriteLeaser, RoomCamera, float, Vector2> funct = (Action<RoomCamera.SpriteLeaser, RoomCamera, float, Vector2>)Activator.CreateInstance(typeof(Action<RoomCamera.SpriteLeaser, RoomCamera, float, Vector2>), self, ptr);
            funct(sLeaser, rCam, timeStacker, camPos); //base DrawSprites
        }

        private static void ApplyPaletteHK(On.OracleGraphics.orig_ApplyPalette orig, OracleGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig.Invoke(self, sLeaser, rCam, palette);
            self.SLArmBaseColA = new Color(0.521568656f, 0.521568656f, 0.5137255f);
            self.SLArmHighLightColA = new Color(0.5686275f, 0.5686275f, 0.549019635f);
            self.SLArmBaseColB = palette.texture.GetPixel(5, 1);
            self.SLArmHighLightColB = palette.texture.GetPixel(5, 2);
            for (int i = 0; i < self.armJointGraphics.Length; i++)
            {
                self.armJointGraphics[i].ApplyPalette(sLeaser, rCam, palette);
            }
            Color color;
            if (self.oracle.ID == Oracle.OracleID.SS)
            { color = new Color(1f, 0.4f, 0.796078444f); }
            else
            { color = new Color(0.105882354f * 1.6f, 0.270588249f * 1.6f, 0.34117648f * 1.6f); }
            for (int j = 0; j < self.owner.bodyChunks.Length; j++)
            { sLeaser.sprites[self.firstBodyChunkSprite + j].color = color; }
            sLeaser.sprites[self.neckSprite].color = color;
            sLeaser.sprites[self.HeadSprite].color = color;
            sLeaser.sprites[self.ChinSprite].color = color;
            for (int k = 0; k < 2; k++)
            {
                sLeaser.sprites[self.PhoneSprite(k, 0)].color = self.armJointGraphics[0].BaseColor(default(Vector2));
                sLeaser.sprites[self.PhoneSprite(k, 1)].color = self.armJointGraphics[0].HighLightColor(default(Vector2));
                sLeaser.sprites[self.PhoneSprite(k, 2)].color = self.armJointGraphics[0].HighLightColor(default(Vector2));
                sLeaser.sprites[self.HandSprite(k, 0)].color = color;
                if (self.gown != null)
                {
                    for (int l = 0; l < 7; l++)
                    {
                        (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4] = self.gown.Color(0.4f);
                        (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 1] = self.gown.Color(0f);
                        (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 2] = self.gown.Color(0.4f);
                        (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 3] = self.gown.Color(0f);
                    }
                }
                else
                {
                    sLeaser.sprites[self.HandSprite(k, 1)].color = color;
                }
                sLeaser.sprites[self.FootSprite(k, 0)].color = color;
                sLeaser.sprites[self.FootSprite(k, 1)].color = color;
            }
            if (self.umbCord != null)
            {
                self.umbCord.ApplyPalette(sLeaser, rCam, palette);
                sLeaser.sprites[self.firstUmbilicalSprite].color = palette.blackColor;
            }
            else if (self.discUmbCord != null)
            {
                self.discUmbCord.ApplyPalette(sLeaser, rCam, palette);
            }
            self.armBase.ApplyPalette(sLeaser, rCam, palette);
            if (self.IsMoon)
            {
                sLeaser.sprites[self.MoonThirdEyeSprite].color = Color.Lerp(new Color(1f, 0f, 1f), color, 0.5f);
                sLeaser.sprites[self.killSprite].color = new Color(0.105882354f * 2.1f, 0.270588249f * 2.1f, 0.34117648f * 2.1f);
            }
        }

        private static void UCSetStuckSegmentsHK(On.OracleGraphics.UbilicalCord.orig_SetStuckSegments orig, OracleGraphics.UbilicalCord self)
        {
            if (self.owner.IsMoon)
            {
                self.coord[0, 0] = self.owner.owner.room.MiddleOfTile(78, 1);
                self.coord[0, 2] *= 0f;
                Vector2 pos = self.owner.armJointGraphics[1].myJoint.pos;
                Vector2 vector = self.owner.armJointGraphics[1].myJoint.ElbowPos(1f, self.owner.armJointGraphics[2].myJoint.pos);
                for (int i = -1; i < 2; i++)
                {
                    float num = (i != 0) ? 0.5f : 1f;
                    self.coord[self.coord.GetLength(0) - 20 + i, 0] = Vector2.Lerp(self.coord[self.coord.GetLength(0) - 20 + i, 0], Vector2.Lerp(pos, vector, 0.4f + 0.07f * (float)i) + Custom.PerpendicularVector(pos, vector) * 8f, num);
                    self.coord[self.coord.GetLength(0) - 20 + i, 2] *= 1f - num;
                }
            }
            else { orig.Invoke(self); }
        }
    }
}
