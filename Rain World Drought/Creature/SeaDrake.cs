using System;
using System.Collections.Generic;
using RWCustom;
using SplashWater;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    public class SeaDrake : Creature
    {
        public SeaDrake(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
        {
            this.albino = (world.RegionNumberOfSpawner(abstractCreature.ID) == 10);
            this.GenerateIVars();
            if (this.albino)
            {
                this.iVars.eyeColor = Color.red;
            }
            float num = 3f;
            base.bodyChunks = new BodyChunk[3];
            base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 14.5f, num / 2f);
            base.bodyChunks[1] = new BodyChunk(this, 1, new Vector2(0f, 0f), 12f, num / 2f);
            base.bodyChunks[2] = new BodyChunk(this, 2, new Vector2(0f, 0f), 5f, num / 4f);
            this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[1];
            this.bodyChunkConnections[0] = new PhysicalObject.BodyChunkConnection(base.bodyChunks[0], base.bodyChunks[1], 14f, PhysicalObject.BodyChunkConnection.Type.Normal, 1f, 0.5f);
            base.airFriction = 0.999f;
            base.gravity = 0.9f;
            this.bounce = 0.1f;
            this.surfaceFriction = 0.4f;
            this.collisionLayer = 1;
            base.buoyancy = 0.99f;
            base.GoThroughFloors = true;
            this.trail = new List<Vector2>();
            //Neck
            neck = new Tentacle(this, bodyChunks[0], 40f);
            neck.tProps = new Tentacle.TentacleProps(false, false, true, 0.5f, 0.1f, 0.5f, 1.8f, 0.2f, 1.2f, 10f, 0.25f, 10f, 15, 20, 20, 0);
            neck.tChunks = new Tentacle.TentacleChunk[4];
            for (int n = 0; n < neck.tChunks.Length; n++)
            {
                neck.tChunks[n] = new Tentacle.TentacleChunk(neck, n, (float)(n + 1) / (float)neck.tChunks.Length, 3f);
            }
            neck.tChunks[neck.tChunks.Length - 1].rad = 5f;
            neck.stretchAndSqueeze = 0f;
            //-----------------------------------------------
        }

        private void GenerateIVars()
        {
            int seed = UnityEngine.Random.seed;
            UnityEngine.Random.seed = base.abstractCreature.ID.RandomSeed;
            float fatness = Custom.ClampedRandomVariation(0.5f, 0.1f, 0.5f) * 2f;
            int whiskers = 3;
            this.iVars = new SeaDrake.IndividualVariations(fatness, UnityEngine.Random.value, UnityEngine.Random.Range(0, 3), whiskers, UnityEngine.Random.Range(0, int.MaxValue), UnityEngine.Random.Range(0, 5), Mathf.Lerp(0.7f, 1.1f, UnityEngine.Random.value), UnityEngine.Random.value);
            UnityEngine.Random.seed = seed;
        }

        public override void InitiateGraphicsModule()
        {
            if (base.graphicsModule == null)
            {
                base.graphicsModule = new SeaDrakeGraphics(this);
            }
        }

        public override void NewRoom(Room newRoom)
        {
            base.NewRoom(newRoom);
            this.jetSound = new StaticSoundLoop(SoundID.Water_Surface_Upset_LOOP, base.bodyChunks[1].pos, this.room, 0f, 1f);
            neck.NewRoom(room); //fix
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (this.room == null)
            {
                return;
            }
            UpdateNeck();//added neck update code
            if (this.room.game.devToolsActive && Input.GetKey("b") && this.room.game.cameras[0].room == this.room)
            {
                this.Stun(12);
            }
            if (!base.Consious || this.room.GetTilePosition(base.mainBodyChunk.pos).y >= this.room.defaultWaterLevel - 1 || this.room.GetTilePosition(base.bodyChunks[1].pos).y >= this.room.defaultWaterLevel - 1 || base.Submersion < 1f)
            {
                this.grabable = 40;
            }
            else if (this.grabable > 0)
            {
                this.grabable--;
            }
            base.mainBodyChunk.terrainSqueeze = 1f;
            base.bodyChunks[1].terrainSqueeze = 1f;
            this.trail.Insert(0, (base.bodyChunks[1].pos - base.bodyChunks[0].pos).normalized);
            if (this.trail.Count > 20)
            {
                this.trail.RemoveAt(this.trail.Count - 1);
            }
            if (this.trail.Count > 1)
            {
                float num = 0f;
                for (int i = 0; i < this.trail.Count - 1; i++)
                {
                    num += Vector2.Dot(this.trail[i], this.trail[i + 1]);
                }
                num /= (float)(this.trail.Count - 1);
                num = Mathf.InverseLerp(0.99f, 0.98f, num);
                if (this.turnSpeed < num)
                {
                    this.turnSpeed = Mathf.Min(this.turnSpeed + 0.05f, num);
                }
                else
                {
                    this.turnSpeed = Mathf.Max(this.turnSpeed - 0.0125f, num);
                }
            }
            if (this.allDry)
            {
                if (base.Submersion == 1f)
                {
                    this.diveSpeed = Mathf.Pow(Mathf.InverseLerp(1f, -0.5f, Custom.DirVec(base.bodyChunks[1].pos, base.mainBodyChunk.pos).y) * Mathf.InverseLerp(0f, -20f, base.mainBodyChunk.vel.y), 0.65f);
                    this.allDry = false;
                }
            }
            else if (base.Submersion == 0f)
            {
                this.allDry = true;
            }
            this.diveSpeed = Mathf.Max(this.diveSpeed - 0.0166666675f, 0f);
            this.jetSound.Update();
            this.jetSound.pos = base.bodyChunks[1].pos;
            this.jetSound.pitch = Custom.LerpMap(this.jetWater, 0f, 0.2f, 0.5f, 1f);
            this.jetSound.volume = this.jetActive;
            float num2 = 0f;
            if (base.Submersion > 0f && base.Submersion < 1f)
            {
                num2 = Mathf.Abs((base.bodyChunks[0].pos - base.bodyChunks[1].pos).normalized.y);
            }
            if (this.surfSpeed < num2)
            {
                this.surfSpeed = Mathf.Min(this.surfSpeed + 0.05f, num2);
            }
            else
            {
                this.surfSpeed = Mathf.Max(this.surfSpeed - 0.0125f, num2);
            }
            if (base.Consious)
            {
                base.waterFriction = 0.995f;
                this.waterRetardationImmunity = 0.8f;
                this.turnSpeed = 0f;
                this.diveSpeed *= 0.8f;
                this.surfSpeed *= 0.9f;
                this.Act();
            }
            else
            {
                jetActive = 0f;
                base.waterFriction = 0.95f;
                this.waterRetardationImmunity = 0f;
            }
            if (base.grasps[0] != null)
            {
                this.CarryObject(eu);
            }
        }

        private void UpdateNeck()
        {
            neck.Update();
            Vector2 lookPoint = bodyChunks[0].pos + Custom.DirVec(bodyChunks[1].pos, bodyChunks[0].pos) * 60f;
            //if (this.AI.pathFinder.GetDestination != null)
            //    lookPoint = bodyChunks[0].pos + Custom.DirVec(bodyChunks[0].pos, room.MiddleOfTile(this.AI.pathFinder.GetDestination))*60f;

            //if (this.AI.behavior == SeaDrakeAI.Behavior.Hunt)
            //{
            //    lookPoint = bodyChunks[0].pos + Custom.DirVec(bodyChunks[0].pos, room.MiddleOfTile(this.AI.preyTracker.MostAttractivePrey.BestGuessForPosition())) * 60f;
            //}
            this.bodyChunks[2].pos = neck.tChunks[neck.tChunks.Length - 2].pos;
            if (Consious)
            {
                this.neck.MoveGrabDest(lookPoint);
            }
            else
            {
                this.neck.limp = true;
            }
            //for (int i = 0; i < neck.tChunks.Length; i++)
            //{
            //    float t = (float)i / (float)(neck.tChunks.Length - 1);
            //    neck.tChunks[i].vel *= 0.95f;
            //    Tentacle.TentacleChunk tentacleChunk = neck.tChunks[i];
            //    tentacleChunk.vel.y = tentacleChunk.vel.y - ((!neck.limp) ? 0.1f : 0.7f);
            //    if ((float)neck.backtrackFrom == -1f || neck.backtrackFrom > i)
            //    {
            //        //Vector2 adjustedVector = new Vector2(this.moveDirection.x/this.moveDirection.magnitude, this.moveDirection.y/this.moveDirection.magnitude);
            //        //this.neck.tChunks[i].vel += Custom.DirVec(base.bodyChunks[0].pos, base.bodyChunks[5].pos) * Mathf.Lerp(3f, 1f, t);

            //        neck.tChunks[i].vel += lookDirection + Custom.DirVec(bodyChunks[1].pos, bodyChunks[0].pos) * Mathf.Lerp(3f, 1f, t);
            //    }
            //    neck.tChunks[i].vel -= neck.connectedChunk.vel;
            //    neck.tChunks[i].vel *= 0.75f;
            //    neck.tChunks[i].vel += neck.connectedChunk.vel;
            //}
            //neck.limp = !Consious;
            //float num = (neck.backtrackFrom != -1) ? 0f : 0.5f;
            ////if (base.grasps[0] == null)
            ////{
            //Vector2 a = Custom.DirVec(bodyChunks[2].pos, neck.tChunks[neck.tChunks.Length - 1].pos);
            //float num2 = Vector2.Distance(bodyChunks[2].pos, neck.tChunks[neck.tChunks.Length - 1].pos);
            //bodyChunks[2].pos -= (6f - num2) * a * (1f - num);
            //bodyChunks[2].vel -= (6f - num2) * a * (1f - num);
            //neck.tChunks[neck.tChunks.Length - 1].pos += (6f - num2) * a * num;
            //neck.tChunks[neck.tChunks.Length - 1].vel += (6f - num2) * a * num;
            //bodyChunks[2].vel += Custom.DirVec(neck.tChunks[neck.tChunks.Length - 2].pos, bodyChunks[2].pos) * 6f * (1f - num);
            //bodyChunks[2].vel += Custom.DirVec(neck.tChunks[neck.tChunks.Length - 1].pos, bodyChunks[2].pos) * 6f * (1f - num);
            //neck.tChunks[neck.tChunks.Length - 1].vel -= Custom.DirVec(neck.tChunks[neck.tChunks.Length - 2].pos, bodyChunks[2].pos) * 6f * num;
            //neck.tChunks[neck.tChunks.Length - 2].vel -= Custom.DirVec(neck.tChunks[neck.tChunks.Length - 2].pos, bodyChunks[2].pos) * 6f * num;
            ////}
            //if (!Consious)
            //{
            //    neck.retractFac = 0.5f;
            //    neck.floatGrabDest = null;
            //    return;
            //}
            //BodyChunk head = bodyChunks[2];
            ////head.vel.y = head.vel.y + gravity;
            //Vector2 vector;
            //vector = mainBodyChunk.pos + lookDirection * 400f;
            //float num3 = 0.5f;
            //neck.retractFac = Mathf.Lerp(0.5f, 0.8f, num3);
            //vector = Vector2.Lerp(vector, mainBodyChunk.pos + lookDirection * 200f, Mathf.Pow(num3, 6f));
            //if (Blinded)
            //{
            //    vector = mainBodyChunk.pos + Custom.RNV() * UnityEngine.Random.value * 400f;
            //}
            //if ((Custom.DistLess(vector, mainBodyChunk.pos, 220f) && !room.VisualContact(vector, head.pos)) || num3 > 0.5f)
            //{
            //    neck.MoveGrabDest(vector);
            //}
            //else if (neck.backtrackFrom == -1)
            //{
            //    neck.floatGrabDest = null;
            //}
            //Vector2 a2 = Custom.DirVec(head.pos, vector);
            //if (grasps[0] == null)
            //{
            //    neck.tChunks[neck.tChunks.Length - 1].vel += a2 * num * 1.2f;
            //    neck.tChunks[neck.tChunks.Length - 2].vel -= a2 * 0.5f * num;
            //    head.vel += a2 * 6f * (1f - num);
            //}
            //else
            //{
            //    neck.tChunks[neck.tChunks.Length - 1].vel += a2 * 2f * num;
            //    neck.tChunks[neck.tChunks.Length - 2].vel -= a2 * 2f * num;
            //    grasps[0].grabbedChunk.vel += a2 / grasps[0].grabbedChunk.mass;
            //}
        }

        private void Act()
        {
            this.AI.Update();
            if (this.AI.behavior == SeaDrakeAI.Behavior.GetUnstuck && base.Submersion > 0f)
            {
                float num = Mathf.InverseLerp(0.5f, 1f, this.AI.stuckTracker.Utility());
                for (int i = 0; i < base.bodyChunks.Length; i++)
                {
                    base.bodyChunks[i].vel += Custom.RNV() * UnityEngine.Random.value * 10f * num;
                    base.bodyChunks[i].terrainSqueeze = Mathf.Lerp(1f, 0.1f, num);
                }
            }
            MovementConnection movementConnection = (this.AI.pathFinder as FishPather).FollowPath(this.room.GetWorldCoordinate(base.mainBodyChunk.pos), true);
            if (movementConnection != null && (movementConnection.type == MovementConnection.MovementType.ShortCut || movementConnection.type == MovementConnection.MovementType.NPCTransportation))
            {
                this.enteringShortCut = new IntVector2?(movementConnection.StartTile);
                if (movementConnection.type == MovementConnection.MovementType.NPCTransportation)
                {
                    this.NPCTransportationDestination = movementConnection.destinationCoord;
                }
                return;
            }
            if (this.AI.floatGoalPos != null && this.AI.pathFinder.GetDestination.TileDefined && this.AI.pathFinder.GetDestination.room == this.room.abstractRoom.index && this.room.VisualContact(base.mainBodyChunk.pos, this.AI.floatGoalPos.Value))
            {
                this.swimDir = Custom.DirVec(base.mainBodyChunk.pos, this.AI.floatGoalPos.Value);
            }
            else if (movementConnection != null)
            {
                this.swimDir = Custom.DirVec(base.mainBodyChunk.pos, this.room.MiddleOfTile(movementConnection.DestTile));
                WorldCoordinate destinationCoord = movementConnection.destinationCoord;
                for (int j = 0; j < 4; j++)
                {
                    MovementConnection movementConnection2 = (this.AI.pathFinder as FishPather).FollowPath(destinationCoord, false);
                    if (movementConnection2 != null && movementConnection2.destinationCoord.TileDefined && movementConnection2.destinationCoord.room == this.room.abstractRoom.index && this.room.VisualContact(movementConnection.destinationCoord.Tile, movementConnection2.DestTile))
                    {
                        this.swimDir += Custom.DirVec(base.mainBodyChunk.pos, this.room.MiddleOfTile(movementConnection2.DestTile));
                        destinationCoord = movementConnection2.destinationCoord;
                        if (this.room.aimap.getAItile(movementConnection2.DestTile).narrowSpace)
                        {
                            this.slowDownForPrecision += 0.3f;
                            break;
                        }
                    }
                }
                this.swimDir = this.swimDir.normalized;
            }
            this.slowDownForPrecision = Mathf.Clamp(this.slowDownForPrecision - 0.1f, 0f, 1f);
            if (base.Submersion < 0.4f && (base.bodyChunks[0].ContactPoint.y < 0 || base.bodyChunks[1].ContactPoint.y < 0) && UnityEngine.Random.value < 0.05f)
            {
                base.mainBodyChunk.vel += Custom.DegToVec(Mathf.Lerp(-45f, 45f, UnityEngine.Random.value)) * Mathf.Lerp(6f, 26f, UnityEngine.Random.value);
                this.room.PlaySound(SoundID.Jet_Fish_On_Land_Jump, base.mainBodyChunk);
            }
            this.Swim(0.6f);
        }

        private void Swim(float speedFac)
        {
            if ((this.room.aimap.getAItile(base.bodyChunks[2].pos).narrowSpace || this.room.GetTile(base.bodyChunks[2].pos).Terrain == Room.Tile.TerrainType.ShortcutEntrance) && this.room.GetTile(base.bodyChunks[2].pos + Custom.DirVec(base.bodyChunks[1].pos, base.mainBodyChunk.pos) * 20f).Terrain != Room.Tile.TerrainType.Air)
            {
                MovementConnection movementConnection = (this.AI.pathFinder as FishPather).FollowPath(this.room.GetWorldCoordinate(base.mainBodyChunk.pos), true);
                bool flag = false;
                if (movementConnection == null)
                {
                    flag = true;
                    movementConnection = (this.AI.pathFinder as FishPather).FollowPath(this.room.GetWorldCoordinate(base.bodyChunks[1].pos), true);
                }
                if (movementConnection != null && movementConnection.destinationCoord.TileDefined)
                {
                    base.mainBodyChunk.vel += Custom.DirVec(base.bodyChunks[(!flag) ? 0 : 1].pos, this.room.MiddleOfTile(movementConnection.DestTile)) * 1.8f;
                    base.bodyChunks[1].vel += Custom.RNV() * 2f * UnityEngine.Random.value + Custom.DirVec(base.mainBodyChunk.pos, base.bodyChunks[1].pos) * UnityEngine.Random.value;
                }
                base.mainBodyChunk.terrainSqueeze = 0.1f;
                base.bodyChunks[1].terrainSqueeze = 0.1f;
                return;
            }
            this.jetWater = Mathf.Clamp(this.jetWater + Mathf.Lerp(-0.04f, 0.0333333351f, Mathf.Pow(base.bodyChunks[1].submersion, 0.2f)), 0f, 1f);
            base.mainBodyChunk.vel += Custom.DirVec(base.bodyChunks[1].pos, base.mainBodyChunk.pos) * 1.4f * (1f - Mathf.Pow(base.bodyChunks[1].submersion, 0.2f)) * Mathf.InverseLerp(0f, 0.2f, this.jetWater) * speedFac;
            this.jetActive = Mathf.Lerp(this.jetActive, (1f - Mathf.Pow(base.bodyChunks[1].submersion, 0.2f)) * Mathf.InverseLerp(0f, 0.2f, this.jetWater), 0.3f);
            this.swimSpeed = Mathf.Lerp(Mathf.Lerp(1.8f, 3.2f + 6f * this.diveSpeed, this.turnSpeed) * (1f + this.surfSpeed * 0.5f), 1.4f, this.slowDownForPrecision) * ((this.grabbedBy.Count != 0) ? 1f : 0.85f);
            base.mainBodyChunk.vel += Custom.DirVec(base.bodyChunks[1].pos, base.mainBodyChunk.pos) * this.swimSpeed * Mathf.Pow(base.bodyChunks[1].submersion, 0.2f) * speedFac;
            base.bodyChunks[1].vel *= Mathf.Lerp(1f, 0.8f, Mathf.Pow(base.bodyChunks[1].submersion, 0.2f));
            if (this.swimDir.magnitude > 0f)
            {
                this.swimDir = Vector3.Slerp(this.swimDir.normalized, Custom.RNV(), 0.2f);
            }
            else
            {
                this.swimDir = Vector3.Slerp(Custom.DirVec(base.bodyChunks[1].pos, base.bodyChunks[0].pos), Custom.RNV(), 0.3f);
            }
            base.mainBodyChunk.vel += this.swimDir * ((this.grabbedBy.Count != 0) ? 1f : 0.7f) * 2.5f * speedFac;
            base.bodyChunks[1].vel -= this.swimDir * ((this.grabbedBy.Count != 0) ? 1f : 0.7f) * 2.5f * speedFac;
            if (base.mainBodyChunk.submersion < base.bodyChunks[1].submersion && this.swimDir.y > 0f)
            {
                BodyChunk mainBodyChunk = base.mainBodyChunk;
                mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f * base.bodyChunks[1].submersion;
            }
            this.swimDir *= 0f;
        }

        public void CarryObject(bool eu)
        {
            base.grasps[0].grabbedChunk.MoveFromOutsideMyUpdate(eu, base.bodyChunks[2].pos + Custom.DirVec(neck.tChunks[neck.tChunks.Length - 2].pos, neck.tChunks[neck.tChunks.Length - 1].pos) * 10f);
            base.grasps[0].grabbedChunk.vel = base.mainBodyChunk.vel;
        }

        public override void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
        {
            base.Collide(otherObject, myChunk, otherChunk);
            if (!base.Consious)
            {
                return;
            }
            float num = Vector2.Distance(base.bodyChunks[myChunk].vel, otherObject.bodyChunks[otherChunk].vel);
            if (otherObject is Player && num < 8f)
            {
                this.grabable = Math.Max(this.grabable, 7);
            }
            if (myChunk == 2 && num > 12f && otherObject is Creature && !(otherObject is SeaDrake))
            {
                (otherObject as Creature).Violence(base.bodyChunks[myChunk], new Vector2?(base.bodyChunks[myChunk].vel * base.bodyChunks[myChunk].mass), otherObject.bodyChunks[otherChunk], null, Creature.DamageType.Bite, 5f, 10f);
                this.room.PlaySound(SoundID.White_Lizard_Tongue_Shoot_Out, base.mainBodyChunk);
                Vector2 pos = base.bodyChunks[myChunk].pos + Custom.DirVec(base.bodyChunks[myChunk].pos, otherObject.bodyChunks[otherChunk].pos) * base.bodyChunks[myChunk].rad;
                for (int i = 0; i < 5; i++)
                {
                    this.room.AddObject(new Bubble(pos, Custom.RNV() * 18f * UnityEngine.Random.value, false, false));
                }
                return;
            }
            if (myChunk == 2 && base.grasps[0] == null && this.AI.WantToEatObject(otherObject))
            {
                this.Grab(otherObject, 0, otherChunk, Creature.Grasp.Shareability.CanNotShare, 1f, true, true);
                this.room.PlaySound(SoundID.Vulture_Grab_Player, base.mainBodyChunk);
            }
            else if (!(otherObject is SeaDrake) && otherObject.bodyChunks[otherChunk].pos.y < base.bodyChunks[myChunk].pos.y && this.AI.attackCounter > 0)
            {
                BodyChunk bodyChunk = otherObject.bodyChunks[otherChunk];
                bodyChunk.vel.y = bodyChunk.vel.y - num / otherObject.bodyChunks[otherChunk].mass;
                BodyChunk bodyChunk2 = base.bodyChunks[myChunk];
                bodyChunk2.vel.y = bodyChunk2.vel.y + num / 2f;
                int num2 = 30;
                if (otherObject is Creature)
                {
                    SocialMemory.Relationship relationship = base.abstractCreature.state.socialMemory.GetRelationship((otherObject as Creature).abstractCreature.ID);
                    if (relationship != null)
                    {
                        if (relationship.like > -0.5f)
                        {
                            relationship.like = Mathf.Lerp(relationship.like, 0f, 0.001f);
                        }
                        if (relationship.like >= 0f)
                        {
                            num2 = 10 + (int)(20f * Mathf.InverseLerp(1f, 0f, relationship.like));
                        }
                        else
                        {
                            num2 = 30 + (int)(220f * Mathf.InverseLerp(0f, -1f, relationship.like));
                        }
                    }
                }
                if (this.AI.attackCounter > num2)
                {
                    this.AI.attackCounter = num2;
                }
            }
            if (myChunk == 2 && base.grasps[0] == null && otherObject is Creature && ((otherObject as Creature).dead || otherObject.TotalMass < base.TotalMass * 0.9f) && base.Template.CreatureRelationship((otherObject as Creature).Template).type == CreatureTemplate.Relationship.Type.Eats)
            {
                this.Grab(otherObject, 0, otherChunk, Creature.Grasp.Shareability.CanNotShare, 1f, true, true);
                this.room.PlaySound((!(otherObject is Player)) ? SoundID.Jet_Fish_Grab_NPC : SoundID.Jet_Fish_Grab_Player, base.mainBodyChunk);
            }
        }

        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);
            if (speed > 1.5f && firstContact)
            {
                this.room.PlaySound((speed >= 8f) ? SoundID.Jet_Fish_Heavy_Terrain_Impact : SoundID.Jet_Fish_Light_Terrain_Impact, base.mainBodyChunk);
            }
        }

        public override void Die()
        {
            base.Die();
            this.waterRetardationImmunity = 0.2f;
            base.buoyancy = 0.92f;
        }

        public override Color ShortCutColor()
        {
            return new Color(0.7f, 0.7f, 0.7f);
        }

        public override void Stun(int st)
        {
            if (UnityEngine.Random.value < 0.5f)
            {
                this.LoseAllGrasps();
            }
            base.Stun(st);
        }

        public override void SpitOutOfShortCut(IntVector2 pos, Room newRoom, bool spitOutAllSticks)
        {
            neck.Reset(mainBodyChunk.pos);
            base.SpitOutOfShortCut(pos, newRoom, spitOutAllSticks);
            Vector2 a = Custom.IntVector2ToVector2(newRoom.ShorcutEntranceHoleDirection(pos));
            for (int i = 0; i < base.bodyChunks.Length; i++)
            {
                base.bodyChunks[i].pos = newRoom.MiddleOfTile(pos) - a * (-1.5f + (float)i) * 15f;
                base.bodyChunks[i].lastPos = newRoom.MiddleOfTile(pos);
                base.bodyChunks[i].vel = a * 8f;
            }
            if (base.graphicsModule != null)
            {
                base.graphicsModule.Reset();
            }
        }

        public SeaDrakeAI AI;

        public SeaDrake.IndividualVariations iVars;

        public Vector2 swimDir;

        public float swimSpeed;

        public float jetActive;

        public float jetWater;

        public float slowDownForPrecision;

        public List<Vector2> trail;

        public float turnSpeed;

        public float diveSpeed;

        public float surfSpeed;

        public bool allDry;

        private StaticSoundLoop jetSound;

        public int grabable;

        public bool albino;

        public Tentacle neck;

        public struct IndividualVariations
        {
            public IndividualVariations(float fatness, float tentacleLength, int tentacleContour, int whiskers, int whiskerSeed, int flipper, float flipperSize, float flipperOrientation)
            {
                this.fatness = fatness;
                this.tentacleLength = tentacleLength;
                this.tentacleFatness = 1f - tentacleLength;
                this.tentacleContour = tentacleContour;
                this.eyeColor = Custom.HSL2RGB(0.8333333f, 1f, 0.5f);
                this.whiskers = whiskers;
                this.whiskerSeed = whiskerSeed;
                this.flipper = flipper;
                this.flipperSize = flipperSize;
                this.flipperOrientation = flipperOrientation;
            }

            public float fatness;

            public float tentacleLength;

            public float tentacleFatness;

            public float flipperSize;

            public float flipperOrientation;

            public int tentacleContour;

            public int whiskers;

            public int flipper;

            public int whiskerSeed;

            public Color eyeColor;
        }
    }
}