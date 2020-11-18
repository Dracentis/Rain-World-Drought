using Noise;
using Rain_World_Drought.Creatures;
using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;
using VoidSea;
using Random = UnityEngine.Random;

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
            On.Player.Grabbed += new On.Player.hook_Grabbed(GrabbedHK);
            On.Player.LungUpdate += new On.Player.hook_LungUpdate(LungUpdateHK);
            On.VoidSea.VoidSeaScene.VoidSeaTreatment += new On.VoidSea.VoidSeaScene.hook_VoidSeaTreatment(VoidSeaTreatmentHK);
            On.TubeWorm.JumpButton += new On.TubeWorm.hook_JumpButton(JumpButtonHK);
        }

        private static void CtorHK(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            WandererSupplement.GetSub(self, true);
        }

        private static void SwallowObjectHK(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            if (!WandererSupplement.IsWanderer(self) || grasp < 0 || self.grasps[grasp] == null) { return; }
            if (self.room.game.session is StoryGameSession && (self.room.game.session as StoryGameSession).saveState.miscWorldSaveData.moonRevived)
            { WandererSupplement.GetSub(self).pearlConversation.PlayerSwallowItem(self.grasps[grasp].grabbed); }
            orig.Invoke(self, grasp);
        }
        
        public static bool IsGrounded(Player self, bool feetMustBeGrounded)
        {
            if (self.canJump > 0 && self.canWallJump == 0) return true;
            if (self.animation == Player.AnimationIndex.AntlerClimb) return true;
            if (self.animation == Player.AnimationIndex.VineGrab) return true;
            if (self.animation == Player.AnimationIndex.SurfaceSwim) return true;
            if (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam) return true;
            if (self.animation == Player.AnimationIndex.StandOnBeam) return true;
            if (self.animation == Player.AnimationIndex.ZeroGPoleGrab) return true;
            if (self.animation == Player.AnimationIndex.Roll) return true;
            if (feetMustBeGrounded) return false;
            if (self.canWallJump != 0) return true;
            return false;
        }

        // Activate focus state by tapping map
        // While in focus state, leaving the ground (or starting off of the ground) slows down time
        // Tapping jump while slowed activates doublejump
        // If a deadly projectile get close to you, time slows as well
        // Tapping jump during this slow parries and doublejumps
        // If the deadly projectile is your own, it increases its speed and damage
        private static void DoAbilities(Player self)
        {
            if (!WandererSupplement.IsWanderer(self)) return;
            WandererSupplement sub = WandererSupplement.GetSub(self);

            // Keep the player from stopping when the map button is tapped
            self.standStillOnMapButton = sub.mapHeld > 10;
            if (self.input[0].mp) sub.mapHeld++;
            else
            {
                if (self.input[1].mp && sub.mapHeld < 10 && IsGrounded(self, true))
                    EnterFocus(sub);
                sub.mapHeld = 0;
            }

            if (sub.focusLeft > 0)
            {
                bool fromWeapon = false;
                bool enterSlowdown = false;
                // Enter slowdown when the player leaves the ground
                if (self.bodyChunks[0].ContactPoint.y != -1 && self.bodyChunks[1].ContactPoint.y != -1 && !IsGrounded(self, false))
                {
                    Debug.Log("Entered slowdown from jump");
                    enterSlowdown = true;
                }

                // Enter slowdown if a deadly weapon is projected to hit the player
                if (!enterSlowdown && sub.wantToParry == 0)
                {
                    PanicProjectile(sub, out float ticksUntilContact, out bool inDanger);
                    if (inDanger)
                    {
                        Debug.Log("Entered slowdown from threat (" + ticksUntilContact + " ticks until contact)");
                        enterSlowdown = true;
                        fromWeapon = true;
                    }
                }

                // Should enter slowdown if non-weapon deadly projectiles (king tusks) are near
                if (enterSlowdown)
                    EnterSlowdown(sub, fromWeapon);
            }

            if (sub.slowdownLeft > 0 && !sub.panicSlowdown && sub.wantToParry == 0)
            {
                PanicProjectile(sub, out float ticksUntilContact, out bool inDanger);
                if (inDanger)
                {
                    Debug.Log("Switch to panic slowdown from threat (" + ticksUntilContact + " ticks until contact)");
                    sub.panicSlowdown = true;
                    sub.slowdownLeft = Math.Min(sub.slowdownLeft, WandererSupplement.slowdownDuration - 10);
                }
            }
            else if (sub.panicSlowdown)
            {
                PanicProjectile(sub, out float ticksUntilContact, out bool inDanger);
                if (inDanger)
                    sub.ticksUntilPanicHit = ticksUntilContact;
                else
                {
                    sub.panicSlowdown = false;
                    sub.ticksUntilPanicHit = 0;
                }
            }

            // Try to parry
            if (sub.wantToParry > 0)
            {
                sub.wantToParry--;
                bool parried = false;
                Vector2 parryCenter = Vector2.Lerp(self.bodyChunks[0].pos, self.bodyChunks[1].pos, 0.5f);
                for (int i = self.room.updateList.Count - 1; i >= 0; i--)
                {
                    if (Parryable.IsParryable(self.room.updateList[i], out Parryable parryable))
                    {
                        Vector2 pos = parryable.Pos;
                        if (Parryable.WouldHitCircle(pos, parryable.Vel, parryCenter, WandererSupplement.parryRadius))
                        {
                            parried = true;
                            parryable.Parry(self, pos - parryCenter);
                        }
                    }
                }
                if (parried)
                {
                    sub.wantToParry = 0;
                    Click(self);
                }
            }

            HUD.FocusMeter.UpdateFocus(sub.energy, sub.hasHalfEnergyPip);
            if (sub.focusLeft > 0 || sub.slowdownLeft > 0 || sub.canTripleJump) HUD.FocusMeter.ShowMeter(30);
        }

        private static Parryable PanicProjectile(WandererSupplement sub, out float ticksUntilContact, out bool inDanger)
        {
            Parryable closestParryable = FindClosestParryable(sub.self, out ticksUntilContact);
            inDanger = true;
            if (ticksUntilContact < 20 && ticksUntilContact >= 0)
            {
                Vector2 pos = closestParryable.Pos;
                bool canSeeWeapon = false;
                for (int i = 0; i < sub.self.bodyChunks.Length; i++)
                    if (sub.self.room.VisualContact(pos, sub.self.bodyChunks[i].pos))
                    {
                        canSeeWeapon = true;
                        break;
                    }
                if (ticksUntilContact < 10 || canSeeWeapon)
                    return closestParryable;
            }

            inDanger = false;
            return closestParryable;
        }

        private static Parryable FindClosestParryable(Player self, out float ticksUntilContact)
        {
            int minTicksUntilContact = int.MaxValue;
            ticksUntilContact = minTicksUntilContact;
            Parryable closestParryable = default(Parryable);
            if (self.room == null) return closestParryable;

            for (int layer = self.room.physicalObjects.Length - 1; layer >= 0; layer--)
            {
                List<PhysicalObject> objs = self.room.physicalObjects[layer];
                if (objs == null) continue;

                for (int i = objs.Count - 1; i >= 0; i--)
                {
                    if (Parryable.IsParryable(objs[i], out Parryable parryable))
                    {
                        if ((parryable.target is Weapon wep) && wep.thrownBy == self) continue;

                        for (int chunk = self.bodyChunks.Length - 1; chunk >= 0; chunk--)
                        {
                            Vector2 pos = parryable.Pos;
                            Vector2 vel = parryable.Vel;
                            int predictedTicks = PredictTimeUntilContact(pos, vel, self.bodyChunks[chunk].pos, parryable.Gravity, self.bodyChunks[chunk].rad * 4f);
                            if (predictedTicks > 0 && predictedTicks < minTicksUntilContact)
                            {
                                if (predictedTicks < minTicksUntilContact)
                                {
                                    minTicksUntilContact = predictedTicks;
                                    closestParryable = parryable;
                                }
                            }
                        }
                    }
                }
            }

            ticksUntilContact = minTicksUntilContact;
            return closestParryable;
        }

        private static bool IsWeaponDeadly(Weapon wep)
        {
            if (!wep.HeavyWeapon) return false;
            if (wep is Rock) return false;
            return true;
        }

        private static int PredictTimeUntilContact(Vector2 p, Vector2 v, Vector2 target, float gravity, float radius)
        {
            // Approximate collision time by iterating on position
            int tickCtr = 0;
            Vector2 lastPos;
            for (int i = 0; i < 30; i++)
            {
                if (Parryable.WouldHitCircle(p, v, target, radius))
                    return tickCtr;

                // Move the weapon's projected position forwards
                tickCtr++;
                lastPos = p;
                p += v;
                v.y -= gravity;
            }
            return int.MaxValue;
        }

        private static void EnterFocus(WandererSupplement sub)
        {
            if (sub.energy == 0 && !RechargeFocus(sub))
            {
                HUD.FocusMeter.ShowMeter(30);
                HUD.FocusMeter.DenyAnimation();
                Debug.Log("Failed to enter focus");
            }
            else
            {
                Debug.Log("Entered focus");
                sub.focusLeft = WandererSupplement.focusDuration;
            }
        }

        private static void EnterSlowdown(WandererSupplement sub, bool panic)
        {
            Debug.Log($"Entered slowdown (panic: {panic})");
            sub.panicSlowdown = panic;
            sub.ticksUntilPanicHit = int.MaxValue / 2;
            sub.focusLeft = 0;
            sub.slowdownLeft = WandererSupplement.slowdownDuration - (panic ? 10 : 0);
        }

        private static bool RechargeFocus(WandererSupplement sub)
        {
            if (sub.self.FoodInStomach > 0)
            {
                // Intentionally do not reset half pip, so it isn't wasted
                // This means you can have .5 energy more than the max
                sub.self.AddFood(-1);
                sub.self.eatCounter = 0;
                sub.self.dontEatExternalFoodSourceCounter = 40;
                sub.energy = WandererSupplement.maxEnergy;
                HUD.FocusMeter.RechargeAnimation();
                return true;
            }
            else return false;
        }

        private static void UseFocus(WandererSupplement sub, bool half = false)
        {
            if (sub.energy == 0)
            {
                sub.hasHalfEnergyPip = false;
                return;
            }
            if (half)
            {
                sub.hasHalfEnergyPip = !sub.hasHalfEnergyPip;
                if (sub.hasHalfEnergyPip) sub.energy--;
            }
            else
                sub.energy--;
        }

        private static void FocusJump(Player self)
        {
            if (!WandererSupplement.IsWanderer(self)) return;
            WandererSupplement sub = WandererSupplement.GetSub(self);

            bool devTools = self.room.game.rainWorld.setup.devToolsActive;
            UseFocus(sub);

            // Parry weapons
            // The first jump has a longer parry 
            sub.wantToParry = sub.canTripleJump ? 1 : WandererSupplement.parryLength;
            Debug.Log($"Parrying for {sub.wantToParry} frames!");

            self.wantToJump = 0;
            self.canJump = 0;
            self.room.PlaySound(SoundID.Vulture_Feather_Hit_Terrain, self.mainBodyChunk, false, 3.25f, 0.8f + 0.05f * sub.jumpsSinceGrounded);
            self.room.PlaySound(SoundID.Shelter_Little_Hatch_Open, self.mainBodyChunk, false, 3.25f, 1.2f + 0.05f * sub.jumpsSinceGrounded);
            self.room.InGameNoise(new InGameNoise(self.bodyChunks[1].pos, 350f, self, 1f));

            sub.canTripleJump = sub.jumpsSinceGrounded < WandererSupplement.maxExtraJumps;
            float strength = Custom.LerpMap(sub.jumpsSinceGrounded, 0, WandererSupplement.maxExtraJumps, 1f, 0.5f);
            sub.jumpsSinceGrounded++;
            Vector2 jumpDir = self.input[0].IntVec.ToVector2();
            if (self.input[0].gamePad || devTools)
            {
                if (!devTools || (self.input[0].analogueDir.sqrMagnitude > 0.01f))
                {
                    jumpDir = self.input[0].analogueDir;
                    if (Mathf.Abs(jumpDir.x) > 0.9f) jumpDir.Set(Mathf.Sign(jumpDir.x), 0f);
                    else if (Mathf.Abs(jumpDir.y) > 0.9f) jumpDir.Set(0f, Mathf.Sign(jumpDir.y));
                    if (jumpDir.sqrMagnitude < 0.1f * 0.1f)
                        jumpDir.Set(0f, 1f);
                    jumpDir.Normalize();
                }
            }

            jumpDir.y += 0.25f * self.gravity;

            if (jumpDir.x == 0 && jumpDir.y == 0) jumpDir.y = 1;

            // Correct jump power on diagonals
            jumpDir.Normalize();

            Vector2 force = new Vector2(jumpDir.x * strength, jumpDir.y * strength);
            BoostChunk(self.bodyChunks[0], WandererSupplement.jumpForce * force);
            BoostChunk(self.bodyChunks[1], WandererSupplement.jumpForce * force * (5.5f / 7.5f));

            // Create a ring effect
            self.room.AddObject(new JumpPulse(self.bodyChunks[0].pos * 0.5f + self.bodyChunks[1].pos * 0.5f - force.normalized * 10f, -force));

            sub.focusLeft = 0;
            sub.slowdownLeft = 0;
        }

        // Represents an object that can be parried, since not all parryable objects are Weapons
        private struct Parryable
        {
            public object target;
            private enum ParryableType : byte
            {
                None, Weapon, Tusk, NeedleWorm
            }
            private ParryableType type;

            public Vector2 Pos
            {
                get
                {
                    switch (type)
                    {
                        case ParryableType.Weapon:
                            return ((Weapon)target).firstChunk.pos;
                        case ParryableType.Tusk:
                            Vulture king = (Vulture)target;
                            for (int i = 0; i < king.kingTusks.tusks.Length; i++)
                            {
                                KingTusks.Tusk tusk = king.kingTusks.tusks[i];
                                if (tusk.mode == KingTusks.Tusk.Mode.ShootingOut)
                                    return tusk.chunkPoints[i, 0] + tusk.shootDir * 20f;
                            }
                            break;
                        case ParryableType.NeedleWorm:
                            {
                                BigNeedleWorm worm = (BigNeedleWorm)target;
                                if (!worm.swishDir.HasValue) break;
                                //float addLen = 90f + 50f; // Removed because you could parry worms from too far away
                                float addLen = 0f;
                                return worm.firstChunk.pos + worm.swishDir.Value * (worm.fangLength + addLen);
                            }
                    }
                    return new Vector2(0f, 0f);
                }
            }
            public Vector2 Vel
            {
                get
                {
                    switch (type)
                    {
                        case ParryableType.Weapon:
                            return ((Weapon)target).firstChunk.vel;
                        case ParryableType.Tusk:
                            Vulture king = (Vulture)target;
                            for (int i = 0; i < king.kingTusks.tusks.Length; i++)
                            {
                                KingTusks.Tusk tusk = king.kingTusks.tusks[i];
                                if (tusk.mode == KingTusks.Tusk.Mode.ShootingOut)
                                    return tusk.chunkPoints[i, 0] - tusk.chunkPoints[i, 1];
                            }
                            break;
                        case ParryableType.NeedleWorm:
                            {
                                BigNeedleWorm worm = (BigNeedleWorm)target;
                                if (!worm.swishDir.HasValue) break;
                                return worm.firstChunk.pos - worm.firstChunk.lastPos + worm.swishDir.Value * (worm.fangLength + 10f);
                            }
                    }
                    return new Vector2(0f, 0f);
                }
            }

            public float Gravity
            {
                get
                {
                    switch (type)
                    {
                        case ParryableType.Weapon: return ((Weapon)target).gravity;
                        case ParryableType.NeedleWorm: return 0f;
                        case ParryableType.Tusk: return 0.9f;
                    }
                    return 1f;
                }
            }

            public Parryable(object target)
            {
                this.target = target;
                type = GetParryableType(target);
            }

            public void Parry(Player player, Vector2 localPos)
            {
                switch (type)
                {
                    case ParryableType.Weapon:
                        {
                            Weapon wep = (Weapon)target;
                            if (wep.thrownBy == player)
                            {
                                // Boost spears if thrown by yourself
                                if (Mathf.Sign(wep.firstChunk.vel.x) == Mathf.Sign(localPos.x))
                                {
                                    wep.firstChunk.vel *= 1.5f;
                                    if (wep is Spear spr)
                                    {
                                        spr.spearDamageBonus += 0.5f;
                                        for (int j = Random.Range(2, 5); j >= 0; j--)
                                            wep.room.AddObject(new MouseSpark(spr.firstChunk.pos, (Random.insideUnitCircle - spr.firstChunk.vel.normalized * 2f) * 5f, Random.value * 0.25f + 0.25f, Color.white));
                                    }
                                }
                            }
                            else
                            {
                                // Deflect weapon
                                for (int j = Random.Range(2, 5); j >= 0; j--)
                                    wep.room.AddObject(new MouseSpark(wep.firstChunk.pos, (Random.insideUnitCircle + localPos.normalized) * 5f, Random.value * 0.25f + 0.25f, Color.white));
                                wep.firstChunk.vel *= -0.25f;
                                wep.ChangeMode(Weapon.Mode.Free);
                            }
                        }
                        break;
                    case ParryableType.NeedleWorm:
                        {
                            BigNeedleWorm worm = (BigNeedleWorm)target;
                            worm.swishCounter = 0;
                            worm.swishDir = null;
                            worm.attackReady *= 0.5f;
                            worm.Violence(null, localPos.normalized * 100f, worm.mainBodyChunk, null, Creature.DamageType.Blunt, 0.15f, 30f);
                            worm.lameCounter = 60;
                        }
                        break;
                    case ParryableType.Tusk:
                        {
                            Vulture king = (Vulture)target;
                            for (int i = 0; i < king.kingTusks.tusks.Length; i++)
                            {
                                KingTusks.Tusk tusk = king.kingTusks.tusks[i];
                                if (tusk.mode == KingTusks.Tusk.Mode.ShootingOut)
                                {
                                    tusk.SwitchMode(KingTusks.Tusk.Mode.Dangling);
                                    Vector2 addVel = localPos.normalized * 10f;
                                    for (int c = 0; c < tusk.chunkPoints.GetLength(0); c++)
                                        tusk.chunkPoints[c, 2] += addVel + Random.insideUnitCircle * 3f;
                                    break;
                                }
                            }
                        }
                        break;
                }
            }

            public static bool WouldHitCircle(Vector2 pos, Vector2 vel, Vector2 center, float radius)
            {
                // Perform quadratic formula to determine whether or not the line segment
                // ... this weapon traces will contact the body chunk
                float a = Vector2.SqrMagnitude(vel);
                float b = 2f * Vector2.Dot(pos - center, vel);
                float c = Vector2.SqrMagnitude(pos - center) - radius * radius;
                if (c < 0) return true;
                float r = b * b - 4f * a * c;
                if (r >= 0)
                {
                    r = Mathf.Sqrt(r);
                    if (b <= r && 2f * a >= -r - b)
                        return true;
                }
                return false;
            }

            public static bool IsParryable(object target, out Parryable parryable)
            {
                ParryableType type = GetParryableType(target);
                if (type == ParryableType.None)
                {
                    parryable = default(Parryable);
                    return false;
                }
                else
                {
                    parryable = new Parryable();
                    parryable.target = target;
                    parryable.type = type;
                    return true;
                }
            }

            private static ParryableType GetParryableType(object obj)
            {
                if (obj is Weapon wep)
                {
                    if (IsWeaponDeadly(wep) && wep.mode == Weapon.Mode.Thrown) return ParryableType.Weapon;
                }
                else if (obj is Vulture vulture)
                {
                    if (vulture.kingTusks != null)
                    {
                        for (int i = 0; i < vulture.kingTusks.tusks.Length; i++)
                        {
                            if (vulture.kingTusks.tusks[i].mode == KingTusks.Tusk.Mode.ShootingOut)
                                return ParryableType.Tusk;
                        }
                    }
                }
                else if (obj is BigNeedleWorm worm)
                {
                    if (worm.swishCounter > 0)
                        return ParryableType.NeedleWorm;
                }
                return ParryableType.None;
            }
        }

        private static void BoostChunk(BodyChunk chunk, Vector2 vel)
        {
            // Set velocity, keeping the component of motion facing the new direction
            Vector2 dir = vel.normalized;
            dir.Set(dir.y, -dir.x);
            Vector2 oldVel = chunk.vel;
            oldVel -= dir * Vector2.Dot(oldVel, dir);
            if (Vector2.Dot(oldVel, vel) < 0f) oldVel.Set(0f, 0f);
            chunk.vel = vel + oldVel * 0.5f;
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
            orig(self, eu);
            if (!WandererSupplement.IsWanderer(self)) return;

            WandererSupplement sub = WandererSupplement.GetSub(self);

            // Kick out of focus when unconsious
            if(!self.Consious)
            {
                sub.focusLeft = 0;
                sub.slowdownLeft = 0;
                sub.canTripleJump = false;
                sub.noFocusJumpCounter = 0;
            }

            if (sub.focusLeft > 0)
            {
                sub.focusLeft--;
                if (sub.focusLeft == 0)
                {
                    Debug.Log("Exited focus from timer");
                    UseFocus(sub, true);
                }
            }
            if (sub.slowdownLeft > 0)
            {
                sub.slowdownLeft--;
                if (sub.slowdownLeft == 0)
                {
                    Debug.Log("Exited slowdown from timer");
                    UseFocus(sub, true);
                }
            }

            if (IsGrounded(self, true))
            {
                sub.jumpsSinceGrounded = 0;
                sub.canTripleJump = false;
            }

            sub.jumpQueued = false;

            if (self.Consious && !self.Malnourished && self.room != null && self.room.game != null && self.room.game.IsStorySession && sub.pearlConversation != null)
            { sub.pearlConversation.Update(eu); }
        }

        private static void MovementUpdateHK(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            if(!WandererSupplement.IsWanderer(self))
            {
                orig(self, eu);
                return;
            }
            WandererSupplement sub = WandererSupplement.GetSub(self);

            bool focusJumped = false;
            if (sub.noFocusJumpCounter > 0)
                sub.noFocusJumpCounter--;
            else if ((sub.slowdownLeft > 0 || sub.canTripleJump) && (sub.panicSlowdown || !IsGrounded(self, false)))
            {
                if ((self.input[0].jmp || sub.jumpQueued) && !self.input[1].jmp)
                {
                    if (sub.energy > 0 || RechargeFocus(sub))
                    {
                        FocusJump(self);
                        focusJumped = true;
                        sub.jumpQueued = false;
                    }
                }
            }

            // Don't focus jump when pressing jump to get up on a pole
            if (!sub.panicSlowdown)
                if ((self.animation == Player.AnimationIndex.GetUpToBeamTip) || (self.animation == Player.AnimationIndex.GetUpOnBeam))
                    sub.noFocusJumpCounter = 2;


            if (focusJumped)
                sub.jumpForbidden = 3;

            if (sub.jumpForbidden > 0)
            {
                sub.jumpForbidden--;
                self.wantToJump = 0;
                self.canWallJump = 0;
            }

            Player.AnimationIndex lastAnimation = self.animation;

            orig(self, eu);

            if (self.animation == Player.AnimationIndex.DeepSwim && lastAnimation != Player.AnimationIndex.DeepSwim)
                self.room.InGameNoise(new InGameNoise(self.bodyChunks[1].pos, 350f, self, 2f));
            else if (self.animation == Player.AnimationIndex.SurfaceSwim && lastAnimation != Player.AnimationIndex.SurfaceSwim)
                self.room.InGameNoise(new InGameNoise(self.bodyChunks[1].pos, 350f, self, 2f));

            DoAbilities(self);
        }
        
        private static Color ShortCutColorHK(On.Player.orig_ShortCutColor orig, Player self)
        {
            if (self.playerState.slugcatCharacter == WandererSupplement.SlugcatCharacter)
            { return new Color(0.4f, 0.49411764705f, 0.8f); }
            return orig.Invoke(self);
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
            if (WandererSupplement.IsWanderer(self) && self.room.game.IsStorySession && MiscWorldSaveDroughtData.GetData((self.room.game.session as StoryGameSession).saveState.miscWorldSaveData).isImproved)
            {
                // Divide the rate that air leaves the lungs by 3
                float lastAirInLungs = self.airInLungs;
                orig.Invoke(self);
                if (self.airInLungs > 0f && self.airInLungs < lastAirInLungs) self.airInLungs = Mathf.Lerp(lastAirInLungs, self.airInLungs, 0.333f);
            } else
            {
                orig(self);
            }
        }

        private static void VoidSeaTreatmentHK(On.VoidSea.VoidSeaScene.orig_VoidSeaTreatment orig, VoidSeaScene self, Player player, float swimSpeed)
        {
            orig.Invoke(self, player, swimSpeed);
            if (player.room != self.room) { return; }

            WandererSupplement sub = WandererSupplement.GetSub(player);
            sub.voidEnergy = true;
            sub.voidEnergyAmount = Custom.LerpMap(player.mainBodyChunk.pos.y, -1000f, -5000f, 0f, 1f);
            if (player.mainBodyChunk.pos.y < -22000)
            { sub.past22000 = true; }
            if (player.mainBodyChunk.pos.y < -25000 || self.deepDivePhase == VoidSeaScene.DeepDivePhase.EggScenario)
            { sub.past25000 = true; }
        }

        private static bool JumpButtonHK(On.TubeWorm.orig_JumpButton orig, TubeWorm self, Player plr)
        {
            if (WandererSupplement.IsWanderer(plr))
            {
                WandererSupplement sub = WandererSupplement.GetSub(plr);
                if (sub.jumpForbidden > 0)
                    return true;
            }
            return orig(self, plr);
        }
    }
}
