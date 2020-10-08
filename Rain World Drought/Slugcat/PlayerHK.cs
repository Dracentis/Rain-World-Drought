using Noise;
using Rain_World_Drought.Creatures;
using RWCustom;
using UnityEngine;
using VoidSea;

namespace Rain_World_Drought.Slugcat
{
    internal static class PlayerHK
    {
        public static void Patch()
        {
            On.Player.ctor += new On.Player.hook_ctor(CtorHK);
            On.Player.SwallowObject += new On.Player.hook_SwallowObject(SwallowObjectHK);
            On.Player.Update += new On.Player.hook_Update(UpdateHK);
            On.Player.MovementUpdate += new On.Player.hook_MovementUpdate(MovementUpdateHK);
            On.Player.ShortCutColor += new On.Player.hook_ShortCutColor(ShortCutColorHK);
            On.Creature.SpearStick += new On.Creature.hook_SpearStick(SpearStickHK);
            On.Creature.Violence += new On.Creature.hook_Violence(ViolenceHK);
            On.Player.GrabUpdate += new On.Player.hook_GrabUpdate(GrabUpdateHK);
            On.Player.Grabbed += new On.Player.hook_Grabbed(GrabbedHK);
            On.Player.LungUpdate += new On.Player.hook_LungUpdate(LungUpdateHK);
            On.VoidSea.VoidSeaScene.VoidSeaTreatment += new On.VoidSea.VoidSeaScene.hook_VoidSeaTreatment(VoidSeaTreatmentHK);
        }

        private static void CtorHK(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            WandererSupplement.GetSub(self);
        }

        private static void SwallowObjectHK(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            if (!WandererSupplement.IsWanderer(self) || grasp < 0 || self.grasps[grasp] == null) { return; }
            if (self.room.game.session is StoryGameSession && (self.room.game.session as StoryGameSession).saveState.miscWorldSaveData.moonRevived)
            { WandererSupplement.GetSub(self).pearlConversation.PlayerSwallowItem(self.grasps[grasp].grabbed); }
            orig.Invoke(self, grasp);
        }

        public static void Click(Player self)
        {
            if (self.bodyChunks[1].submersion == 1f)
            {
                self.room.AddObject(new ShockWave(self.bodyChunks[0].pos, 160f * Mathf.Lerp(0.65f, 1.5f, UnityEngine.Random.value), 0.07f, 9));
            }
            else
            {
                self.room.AddObject(new ShockWave(self.bodyChunks[0].pos, 100f * Mathf.Lerp(0.65f, 1.5f, UnityEngine.Random.value), 0.07f, 6));
                for (int i = 0; i < 10; i++)
                {
                    self.room.AddObject(new WaterDrip(self.bodyChunks[0].pos, Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(4f, 21f, UnityEngine.Random.value), false));
                }
            }
            self.room.PlaySound(SoundID.Lizard_Head_Shield_Deflect, self.mainBodyChunk);
        }

        private static void UpdateHK(On.Player.orig_Update orig, Player self, bool eu)
        {
            WandererSupplement sub = WandererSupplement.GetSub(self);

            if (sub.rad > 0) { sub.rad--; }
            if (sub.bashing)
            {
                sub.dropCounter = 0;
                self.mushroomEffect = Custom.LerpAndTick(self.mushroomEffect, 5f, 0.2f, 0.1f);
            }
            else if (sub.dropCounter == 15)
            {
                SetObjectDown(self, eu);
                sub.dropCounter = 16;
                //Debug.Log("Drop and Count from " + (dropCounter - 1) + " to " + dropCounter);
            }
            else if (sub.dropCounter > 0 && sub.dropCounter < 30)
            {
                sub.dropCounter++;
                //Debug.Log("Count from "+(dropCounter-1)+" to " + dropCounter);
            }

            if (!self.Malnourished)
            {
                if (sub.parry > 0f) { sub.parry--; }
                sub.energy = Mathf.Clamp(sub.energy + WandererSupplement.RECHARGE_RATE, 0, sub.maxEnergy);
                if (sub.uses < 10 & !sub.voidEnergy)
                { sub.maxEnergy = Mathf.Clamp((sub.uses + 5 / 14f), WandererSupplement.BASH_COST, 1f); }
            }
            else
            { sub.energy = 0f; }

            orig.Invoke(self, eu);

            if (WandererSupplement.IsWanderer(self) && self.Consious && !self.Malnourished && self.room != null && self.room.game != null && self.room.game.IsStorySession)
            { sub.pearlConversation.Update(eu); }
        }

        private static void MovementUpdateHK(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            Player.AnimationIndex lastAnimation = self.animation;
            orig.Invoke(self, eu);
            if (!WandererSupplement.IsWanderer(self)) { return; }

            if (self.room == null || self.room.game == null || self.room.abstractRoom == null || self.room.world == null) { return; }

            if (self.animation == Player.AnimationIndex.DeepSwim && lastAnimation != Player.AnimationIndex.DeepSwim)
            { self.room.InGameNoise(new InGameNoise(self.bodyChunks[1].pos, 350f, self, 2f)); }
            else if (self.animation == Player.AnimationIndex.SurfaceSwim && lastAnimation != Player.AnimationIndex.SurfaceSwim)
            { self.room.InGameNoise(new InGameNoise(self.bodyChunks[1].pos, 350f, self, 2f)); }

            WandererSupplement sub = WandererSupplement.GetSub(self);
            bool parryed = false;
            if (!self.Malnourished && sub.uses > 0 && sub.energy > WandererSupplement.PARRY_COST && sub.parry <= 10
                && !sub.jmpDwn && self.input[0].jmp && !sub.bashing)
            {
                for (int p = 0; p < self.room.physicalObjects.Length; p++)
                {
                    for (int q = 0; q < self.room.physicalObjects[p].Count; q++)
                    {
                        if (!parryed && self.room.physicalObjects[p][q] is Spear s && s.mode == Weapon.Mode.Thrown && Custom.DistLess(s.bodyChunks[0].pos, self.mainBodyChunk.pos, 300f))
                        {
                            sub.parry = 45f;
                            sub.energy -= WandererSupplement.PARRY_COST;
                            parryed = true;
                            Click(self);
                            sub.uses--;
                        }
                    }
                }
            }

            /* // Alternative method
            foreach (UpdatableAndDeletable updatableAndDeletable in this.room.updateList)
            {
                if (!parryed && (updatableAndDeletable is Spear) && Custom.DistLess(this.mainBodyChunk.pos, (updatableAndDeletable as PhysicalObject).bodyChunks[0].pos, 200f) && (updatableAndDeletable as Spear).mode == Weapon.Mode.Thrown)
                {
                    Debug.Log("Click");
                    self.parry = 45f;
                    self.energy -= PARRY_COST;
                    parryed = true;
                    Click();
                }
            } */

            if (!sub.bashing && !parryed
                && (self.bodyMode == Player.BodyModeIndex.Default || self.bodyMode == Player.BodyModeIndex.Stand || self.bodyMode == Player.BodyModeIndex.ZeroG)
                && self.canJump <= 0 && !sub.jmpDwn && self.input[0].jmp && self.input[0].pckp)
            {
                if (sub.uses > 0 && sub.energy > WandererSupplement.BASH_COST)
                {
                    if (self.mushroomCounter < 40) { self.mushroomCounter += 40; }
                    sub.bashing = true;
                    sub.jmpDwn = true;
                }
                else
                {
                    self.room.PlaySound(SoundID.MENU_Greyed_Out_Button_Clicked, self.mainBodyChunk, false, 1f, 1f);
                    sub.dropCounter = 0;
                }
            }

            if (sub.uses > 0 && sub.bashing && self.input[0].jmp && !sub.jmpDwn)
            {
                self.room.PlaySound(SoundID.Moon_Wake_Up_Swarmer_Ping, self.mainBodyChunk, false, 1f, 1f);
                self.room.InGameNoise(new InGameNoise(self.bodyChunks[1].pos, 350f, self, 4f));
                if ((self.input[0].y == 0 || self.input[0].x == 0))
                {
                    self.bodyChunks[0].vel.y = 7.5f * (float)self.input[0].y * (sub.energy + 1);
                    self.bodyChunks[1].vel.y = 5.5f * (float)self.input[0].y * (sub.energy + 1);
                    self.bodyChunks[0].vel.x = 7.5f * (float)self.input[0].x * (sub.energy + 1);
                    self.bodyChunks[1].vel.x = 5.5f * (float)self.input[0].x * (sub.energy + 1);
                }
                else
                {
                    const float diagonal = 0.8509035f;
                    self.bodyChunks[0].vel.y = 7.5f * diagonal * (float)self.input[0].y * (sub.energy + 1);
                    self.bodyChunks[1].vel.y = 5.5f * diagonal * (float)self.input[0].y * (sub.energy + 1);
                    self.bodyChunks[0].vel.x = 7.5f * diagonal * (float)self.input[0].x * (sub.energy + 1);
                    self.bodyChunks[1].vel.x = 5.5f * diagonal * (float)self.input[0].x * (sub.energy + 1);
                }
                sub.bashing = false;
                sub.energy -= WandererSupplement.BASH_COST;
                sub.uses--;
                if (self.mushroomCounter <= 40) { self.mushroomCounter = 0; }
                self.mushroomEffect = 0f;
            }
            if (sub.bashing && self.mushroomCounter <= 0)
            {
                sub.bashing = false;
                self.mushroomEffect = 0f;
            }
            if (sub.bashing)
            {
                sub.dropCounter = 0;
                self.bodyChunks[0].vel.y = 0.5f * self.bodyChunks[0].vel.y;
                self.bodyChunks[1].vel.y = 0.5f * self.bodyChunks[1].vel.y;
                self.bodyChunks[0].vel.x = 0.5f * self.bodyChunks[0].vel.x;
                self.bodyChunks[1].vel.x = 0.5f * self.bodyChunks[1].vel.x;
            }
            sub.jmpDwn = self.input[0].jmp;
            if (sub.uses <= sub.hibernationPenalty * WandererSupplement.ENERGY_PER_HUNGER)
            {
                if (!self.abstractCreature.world.game.IsStorySession) { sub.uses += WandererSupplement.ENERGY_PER_HUNGER; }
                else if (self.playerState.foodInStomach >= 1 && self.abstractCreature.world.game.GetStorySession.saveState.totFood >= 1)
                {
                    self.AddFood(-1);
                    sub.uses += WandererSupplement.ENERGY_PER_HUNGER;
                }
                else if (sub.hibernationPenalty > 0)
                {
                    sub.hibernationPenalty--;
                    self.slugcatStats.foodToHibernate++; // AddHibernationCost
                    FoodMeterHK.AddHibernateCost();
                }
            }
        }

        public static void SetObjectDown(Player self, bool eu)
        {
            int grasp = -1;
            for (int i = 0; i < 2; i++)
            {
                if (self.grasps[i] != null)
                {
                    grasp = i;
                    break;
                }
            }
            if (grasp > -1)
            {
                self.ReleaseObject(grasp, eu);
            }
            else if (self.spearOnBack != null && self.spearOnBack.spear != null && self.mainBodyChunk.ContactPoint.y < 0)
            {
                self.room.socialEventRecognizer.CreaturePutItemOnGround(self.spearOnBack.spear, self);
                self.spearOnBack.DropSpear();
            }
        }

        private static Color ShortCutColorHK(On.Player.orig_ShortCutColor orig, Player self)
        {
            if (self.playerState.slugcatCharacter == WandererSupplement.SlugcatCharacter)
            { return new Color(0.4f, 0.49411764705f, 0.8f); }
            return orig.Invoke(self);
        }

        /// <summary>
        /// Parry
        /// </summary>
        private static bool SpearStickHK(On.Creature.orig_SpearStick orig, Creature self,
            Weapon source, float dmg, BodyChunk chunk, PhysicalObject.Appendage.Pos appPos, Vector2 direction)
        {
            if (self is Player p && WandererSupplement.GetSub(p).parry > 0f)
            { return false; }
            return orig.Invoke(self, source, dmg, chunk, appPos, direction);
        }

        /// <summary>
        /// Parry
        /// </summary>
        private static void ViolenceHK(On.Creature.orig_Violence orig, Creature self,
            BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
        {
            if (self is Player p && WandererSupplement.GetSub(p).parry > 0f && type == Creature.DamageType.Stab)
            {
                int amount = Mathf.FloorToInt(Mathf.Min(UnityEngine.Random.value + 10f, 25f));
                for (int i = 0; i < amount; i++)
                {
                    self.room.AddObject(new Spark(source.pos + Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, source.vel * -0.1f + Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(0.2f, 0.4f, UnityEngine.Random.value) * source.vel.magnitude, new Color(1f, 1f, 1f), null, 10, 170));
                }
                return;
            }
            orig.Invoke(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
        }

        private static void GrabUpdateHK(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            bool pk = self.wantToPickUp > 0;
            orig.Invoke(self, eu);
            if (pk)
            {
                #region CheckFlag
                if (self.animation == Player.AnimationIndex.DeepSwim)
                {
                    if (self.grasps[0] == null && self.grasps[1] == null)
                    { pk = false; }
                    else
                    {
                        for (int m = 0; m < 10; m++)
                        { if (self.input[m].y > -1 || self.input[m].x != 0) { pk = false; break; } }
                    }
                }
                else
                {
                    for (int n = 0; n < 5; n++)
                    { if (self.input[n].y > -1) { pk = false; break; } }
                }
                if (self.grasps[0] != null && self.HeavyCarry(self.grasps[0].grabbed)) { pk = true; }
                #endregion CheckFlag

                WandererSupplement sub = WandererSupplement.GetSub(self);
                if (!pk) { sub.dropCounter = 0; }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (self.grasps[i] != null)
                        {
                            self.wantToPickUp = 0;
                            break;
                        }
                    }
                    if ((self.bodyMode == Player.BodyModeIndex.Default || self.bodyMode == Player.BodyModeIndex.Stand || self.bodyMode == Player.BodyModeIndex.ZeroG) && self.canJump <= 0)
                    {
                        if (sub.dropCounter <= 0)
                        {
                            //Debug.Log("Drop Started");
                            sub.dropCounter = 1;
                        }
                        else if (sub.dropCounter < 16)
                        {
                            //Debug.Log("Drop pressed again: double drop!");
                            SetObjectDown(self, eu);
                            SetObjectDown(self, eu);
                            sub.dropCounter = 16;
                        }
                        else if (sub.dropCounter < 30)
                        {
                            //Debug.Log("Drop pressed again after first drop: drop!");
                            SetObjectDown(self, eu);
                            sub.dropCounter = 16;
                        }
                        else
                        {
                            //Debug.Log("Drop Started");
                            sub.dropCounter = 1;
                        }
                    }
                    else
                    {
                        SetObjectDown(self, eu);
                        if (sub.dropCounter > 0 && sub.dropCounter < 16)
                        { SetObjectDown(self, eu); }
                        sub.dropCounter = 16;
                    }
                    if (!sub.jmpDwn && self.input[0].pckp && self.input[0].jmp && !sub.bashing)
                    {
                        sub.dropCounter = 0;//For those speedrunners out there
                    }
                }
            }
        }

        private static void GrabbedHK(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            orig.Invoke(self, grasp);
            if (grasp.grabber is SeaDrake)
            {
                self.dangerGraspTime = 0;
                self.dangerGrasp = grasp;
            }
        }

        private static void LungUpdateHK(On.Player.orig_LungUpdate orig, Player self)
        {
            float lastAirInLungs = self.airInLungs;
            float patchedAirInLungs = -1f;
            if (self.room.game.IsStorySession && WandererSupplement.IsWanderer(self))
            {
                patchedAirInLungs = lastAirInLungs - 1f / (((MiscWorldSaveDroughtData.GetData((self.room.game.session as StoryGameSession).saveState.miscWorldSaveData).isImproved ? 3f : 1f) * 40f * ((!self.lungsExhausted) ? 9f : 4.5f) * ((self.input[0].y != 1 || self.input[0].x != 0 || self.airInLungs >= 0.333333343f) ? 1f : 1.5f) * ((float)self.room.game.setupValues.lungs / 100f)) * self.slugcatStats.lungsFac);
            }
            if (patchedAirInLungs > 0f) { self.airInLungs = Mathf.Max(self.airInLungs, 0.2f); } // prevents drowning early
            orig.Invoke(self);
            if (patchedAirInLungs > 0f) { self.airInLungs = patchedAirInLungs; }
        }

        private static void VoidSeaTreatmentHK(On.VoidSea.VoidSeaScene.orig_VoidSeaTreatment orig, VoidSeaScene self, Player player, float swimSpeed)
        {
            orig.Invoke(self, player, swimSpeed);
            if (player.room != self.room) { return; }

            WandererSupplement sub = WandererSupplement.GetSub(player);
            sub.voidEnergy = true;
            sub.maxEnergy = Custom.LerpMap(player.mainBodyChunk.pos.y, -1000f, -5000f, 1f, 0f);
            if (player.mainBodyChunk.pos.y < -22000)
            { sub.past22000 = true; }
            if (player.mainBodyChunk.pos.y < -25000 || self.deepDivePhase == VoidSeaScene.DeepDivePhase.EggScenario)
            { sub.past25000 = true; }
        }
    }
}
