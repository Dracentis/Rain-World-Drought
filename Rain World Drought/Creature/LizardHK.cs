using Rain_World_Drought.Enums;
using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    internal static class LizardHK
    {
        public static void Patch()
        {
            On.Lizard.ActAnimation += new On.Lizard.hook_ActAnimation(ActAnimationHK);
            On.LizardAI.ctor += new On.LizardAI.hook_ctor(AICtorHK);
            On.LizardAI.Update += new On.LizardAI.hook_Update(AIUpdateHK);
            On.Spear.Update += new On.Spear.hook_Update(SpearUpdateHK);
            On.LizardVoice.GetMyVoiceTrigger += new On.LizardVoice.hook_GetMyVoiceTrigger(GetMyVoiceTriggerHK);
        }

        private static void SpearUpdateHK(On.Spear.orig_Update orig, Spear self, bool eu)
        { // why???
            try
            {
                orig.Invoke(self, eu);
            }
            catch (Exception e)
            {
                self.Destroy();
                Debug.LogException(e);
            }
        }

        private static float ActAnimationHK(On.Lizard.orig_ActAnimation orig, Lizard self)
        {
            if (DroughtMod.EnumExt && self.Template.type == EnumExt_Drought.GreyLizard)
            {
                if (self.animation == Lizard.Animation.Spit)
                {
                    float num = 0f;
                    self.bodyWiggleCounter = 0;
                    self.JawOpen = Mathf.Clamp(self.JawOpen + 0.2f, 0f, 1f);
                    if (!self.AI.redSpitAI.spitting)
                    {
                        self.EnterAnimation(Lizard.Animation.Standard, true);
                    }
                    else
                    {
                        Vector2? vector = self.AI.redSpitAI.AimPos();
                        if (vector != null)
                        {
                            (self.graphicsModule as LizardGraphics).blackLizardLightUpHead = Mathf.Clamp01((greySpitDelay / 2 - self.AI.redSpitAI.delay) / (float)greySpitDelay);

                            if (self.AI.redSpitAI.AtSpitPos)
                            {
                                Vector2 fromPos = self.room.MiddleOfTile(self.AI.redSpitAI.spitFromPos);
                                self.mainBodyChunk.vel += Vector2.ClampMagnitude(fromPos - Custom.DirVec(fromPos, vector.Value) * self.bodyChunkConnections[0].distance - self.mainBodyChunk.pos, 10f) / 5f;
                                self.bodyChunks[1].vel += Vector2.ClampMagnitude(fromPos - self.bodyChunks[1].pos, 10f) / 5f;
                            }
                            if (!self.AI.UnpleasantFallRisk(self.room.GetTilePosition(self.mainBodyChunk.pos)))
                            {
                                self.mainBodyChunk.vel += Custom.DirVec(self.mainBodyChunk.pos, vector.Value) * 4f * (float)self.LegsGripping;
                                self.bodyChunks[1].vel -= Custom.DirVec(self.mainBodyChunk.pos, vector.Value) * 2f * (float)self.LegsGripping;
                                self.bodyChunks[2].vel -= Custom.DirVec(self.mainBodyChunk.pos, vector.Value) * 2f * (float)self.LegsGripping;
                            }
                            if (self.AI.redSpitAI.delay < (UnityEngine.Random.value - 0.8f) * greySpitDelay * 0.8f)
                            {
                                self.room.AddObject(new LizardBubble(self.graphicsModule as LizardGraphics, 1f, 0f, (self.graphicsModule as LizardGraphics).blackLizardLightUpHead * 10f));
                            }

                            if (self.AI.redSpitAI.delay < 1)
                            {
                                Vector2 jawDir10 = self.bodyChunks[0].pos + Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos) * 10f;
                                Vector2 jawDir = Custom.DirVec(jawDir10, vector.Value);
                                if (Vector2.Dot(jawDir, Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos)) > 0.3f)
                                {
                                    self.room.PlaySound(SoundID.Spear_Dislodged_From_Creature, jawDir10);
                                    self.room.PlaySound(SoundID.Slugcat_Throw_Spear, jawDir10);
                                    self.room.PlaySound(SoundID.Spear_Stick_In_Wall, jawDir10);
                                    self.room.PlaySound(SoundID.Red_Lizard_Spit, jawDir10);
                                    //this.room.AddObject(new LizardSpit(vector3, vector4 * 40f, this));
                                    AbstractSpear abstractSpear = new AbstractSpear(self.room.world, null, self.room.GetWorldCoordinate(jawDir10), self.room.game.GetNewID(), false);
                                    self.room.abstractRoom.AddEntity(abstractSpear);
                                    abstractSpear.RealizeInRoom();
                                    LaunchSpear(abstractSpear.realizedObject as Spear, self, jawDir10 + jawDir, jawDir10, jawDir, 0.9f, false);
                                    self.AI.redSpitAI.delay = greySpitDelay;
                                    self.bodyChunks[2].pos -= jawDir * 8f;
                                    self.bodyChunks[1].pos -= jawDir * 4f;
                                    self.bodyChunks[2].vel -= jawDir * 2f;
                                    self.bodyChunks[1].vel -= jawDir * 1f;
                                    self.JawOpen = 1f;
                                }
                            }
                        }
                    }
                    return num;
                }
                else
                {
                    (self.graphicsModule as LizardGraphics).blackLizardLightUpHead = Mathf.Max(0f, (self.graphicsModule as LizardGraphics).blackLizardLightUpHead - 0.05f);
                    if (self.AI.runSpeed > 0.1f)
                    { return Mathf.Lerp(orig.Invoke(self), 1f, Mathf.Lerp(0.2f, 0.7f, self.AI.hunger)); }
                }
            }
            return orig.Invoke(self);
        }

        public static void LaunchSpear(Spear self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, Vector2 throwDir, float frc, bool eu)
        {
            self.thrownBy = thrownBy;
            self.thrownPos = thrownPos;
            self.throwDir = new IntVector2((int)(throwDir.x * 2), (int)(throwDir.y * 2));
            self.firstFrameTraceFromPos = firstFrameTraceFromPos;
            self.changeDirCounter = 3;
            self.ChangeOverlap(true);
            self.firstChunk.MoveFromOutsideMyUpdate(eu, thrownPos);
            if (throwDir.x != 0)
            {
                self.firstChunk.vel.y = thrownBy.mainBodyChunk.vel.y * 0.5f;
                self.firstChunk.vel.x = thrownBy.mainBodyChunk.vel.x * 0.2f;
                self.firstChunk.vel.x += (float)throwDir.x * 40f * frc;
                self.firstChunk.vel.y += (float)throwDir.y * 40f * frc;
            }
            else
            {
                if (throwDir.y == 0) { self.ChangeMode(Weapon.Mode.Free); return; }
                self.firstChunk.vel.x = thrownBy.mainBodyChunk.vel.x * 0.5f;
                self.firstChunk.vel.y = (float)throwDir.y * 40f * frc;
            }
            self.ChangeMode(Weapon.Mode.Thrown);
            self.setRotation = new Vector2?(throwDir);
            self.rotationSpeed = 0f;
            self.meleeHitChunk = null;
        }

        private static void AICtorHK(On.LizardAI.orig_ctor orig, LizardAI self, AbstractCreature creature, World world)
        {
            orig.Invoke(self, creature, world);
            if (DroughtMod.EnumExt && creature.creatureTemplate.type == EnumExt_Drought.GreyLizard)
            {
                self.redSpitAI = new LizardAI.LizardSpitTracker(self);
                self.AddModule(self.redSpitAI);
            }
        }

        private static void AIUpdateHK(On.LizardAI.orig_Update orig, LizardAI self)
        {
            orig.Invoke(self);
            if (DroughtMod.EnumExt && self.creature.creatureTemplate.type == EnumExt_Drought.GreyLizard && self.redSpitAI.spitting)
            {
                if (self.lizard.animation != Lizard.Animation.Spit)
                {
                    self.redSpitAI.delay = greySpitDelay;
                    self.lizard.voice.MakeSound(LizardVoice.Emotion.BloodLust);
                }
                self.lizard.EnterAnimation(Lizard.Animation.Spit, false);
                self.lizard.bubble = 10;
                self.lizard.bubbleIntensity = 1f;
            }
        }

        public const int greySpitDelay = 98;

        private static SoundID GetMyVoiceTriggerHK(On.LizardVoice.orig_GetMyVoiceTrigger orig, LizardVoice self)
        {
            if (DroughtMod.EnumExt && self.lizard.Template.type == EnumExt_Drought.GreyLizard)
            {
                string str = "Green"; // for now
                string[] array = new string[] { "A", "B", "C", "D", "E" };
                List<SoundID> list = new List<SoundID>();
                for (int i = 0; i < 5; i++)
                {
                    SoundID soundID;
                    try { soundID = Custom.ParseEnum<SoundID>("Lizard_Voice_" + str + "_" + array[i]); }
                    catch { soundID = SoundID.None; }
                    if (self.lizard.abstractCreature.world.game.soundLoader.workingTriggers[(int)soundID])
                    { list.Add(soundID); }
                }
                if (list.Count == 0) { return SoundID.None; }
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
            else { return orig.Invoke(self); }
        }
    }
}
