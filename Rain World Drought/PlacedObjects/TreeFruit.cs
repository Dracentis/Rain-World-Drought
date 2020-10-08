using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.PlacedObjects
{
    public class TreeFruit : PlayerCarryableItem, IDrawable, IPlayerEdible
    {
        // Token: 0x060018D4 RID: 6356 RVA: 0x0013EABC File Offset: 0x0013CCBC
        public TreeFruit(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            bites = 2;
            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 8f, 0.2f);
            bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.2f;
            surfaceFriction = 0.7f;
            collisionLayer = 1;
            waterFriction = 0.95f;
            buoyancy = 1.1f;
        }

        // Token: 0x170003E4 RID: 996
        // (get) Token: 0x060018D5 RID: 6357 RVA: 0x0013EB60 File Offset: 0x0013CD60
        public AbstractConsumable AbstrConsumable
        {
            get
            {
                return abstractPhysicalObject as AbstractConsumable;
            }
        }

        // Token: 0x060018D6 RID: 6358 RVA: 0x0013EB70 File Offset: 0x0013CD70
        public override void Update(bool eu)
        {
            base.Update(eu);
            if (room.game.devToolsActive && Input.GetKey("b"))
            {
                firstChunk.vel += Custom.DirVec(firstChunk.pos, Input.mousePosition) * 3f;
            }
            lastRotation = rotation;
            if (grabbedBy.Count > 0)
            {
                rotation = Custom.PerpendicularVector(Custom.DirVec(firstChunk.pos, grabbedBy[0].grabber.mainBodyChunk.pos));
                rotation.y = Mathf.Abs(rotation.y);
            }
            if (setRotation != null)
            {
                rotation = setRotation.Value;
                setRotation = null;
            }
            if (base.firstChunk.ContactPoint.y < 0)
            {
                rotation = (rotation - Custom.PerpendicularVector(rotation) * 0.1f * base.firstChunk.vel.x).normalized;
                BodyChunk firstChunk = base.firstChunk;
                firstChunk.vel.x = firstChunk.vel.x * 0.8f;
            }
            if (Submersion > 0.5f && room.abstractRoom.creatures.Count > 0 && grabbedBy.Count == 0)
            {
                AbstractCreature abstractCreature = room.abstractRoom.creatures[UnityEngine.Random.Range(0, room.abstractRoom.creatures.Count)];
                if (abstractCreature.creatureTemplate.type == CreatureTemplate.Type.JetFish && abstractCreature.realizedCreature != null && !abstractCreature.realizedCreature.dead && (abstractCreature.realizedCreature as JetFish).AI.goToFood == null && (abstractCreature.realizedCreature as JetFish).AI.WantToEatObject(this))
                {
                    (abstractCreature.realizedCreature as JetFish).AI.goToFood = this;
                }
            }
        }

        // Token: 0x060018D7 RID: 6359 RVA: 0x0013EDC0 File Offset: 0x0013CFC0
        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);
            if (!AbstrConsumable.isConsumed && AbstrConsumable.placedObjectIndex >= 0 && AbstrConsumable.placedObjectIndex < placeRoom.roomSettings.placedObjects.Count)
            {
                firstChunk.HardSetPosition(placeRoom.roomSettings.placedObjects[AbstrConsumable.placedObjectIndex].pos);
                stalk = new TreeFruit.Stalk(this, placeRoom, firstChunk.pos);
                placeRoom.AddObject(stalk);
            }
            else
            {
                firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos));
                rotation = Custom.RNV();
                lastRotation = rotation;
            }
        }

        // Token: 0x060018D8 RID: 6360 RVA: 0x0013EE94 File Offset: 0x0013D094
        public override void HitByWeapon(Weapon weapon)
        {
            base.HitByWeapon(weapon);
            if (stalk != null && stalk.releaseCounter == 0)
            {
                stalk.releaseCounter = UnityEngine.Random.Range(30, 50);
            }
        }

        // Token: 0x060018D9 RID: 6361 RVA: 0x0013EED1 File Offset: 0x0013D0D1
        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[2];
            sLeaser.sprites[0] = new FSprite("TreeFruit0A", true);
            sLeaser.sprites[1] = new FSprite("TreeFruit0B", true);
            AddToContainer(sLeaser, rCam, null);
        }

        // Token: 0x060018DA RID: 6362 RVA: 0x0013EF10 File Offset: 0x0013D110
        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            Vector2 v = Vector3.Slerp(lastRotation, rotation, timeStacker);
            lastDarkness = darkness;
            darkness = rCam.room.Darkness(pos) * (1f - rCam.room.LightSourceExposure(pos));
            if (darkness != lastDarkness)
            {
                ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
            for (int i = 0; i < 2; i++)
            {
                sLeaser.sprites[i].x = pos.x - camPos.x;
                sLeaser.sprites[i].y = pos.y - camPos.y;
                sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
                sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName("TreeFruit" + Custom.IntClamp(3 - bites, 0, 2) + ((i != 0) ? "B" : "A"));
            }
            if (blink > 0 && UnityEngine.Random.value < 0.5f)
            {
                sLeaser.sprites[1].color = blinkColor;
            }
            else
            {
                sLeaser.sprites[1].color = color;
            }
            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        // Token: 0x060018DB RID: 6363 RVA: 0x0013F0A4 File Offset: 0x0013D2A4
        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[0].color = palette.blackColor;
            color = Color.Lerp(new Color(0f, 0f, 1f), palette.blackColor, darkness);
        }

        // Token: 0x060018DC RID: 6364 RVA: 0x0013F0F4 File Offset: 0x0013D2F4
        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
            {
                newContatiner = rCam.ReturnFContainer("Items");
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].RemoveFromContainer();
            }
            newContatiner.AddChild(sLeaser.sprites[0]);
            newContatiner.AddChild(sLeaser.sprites[1]);
        }

        // Token: 0x170003E5 RID: 997
        // (get) Token: 0x060018DD RID: 6365 RVA: 0x0013F14D File Offset: 0x0013D34D
        public int BitesLeft
        {
            get
            {
                return bites;
            }
        }

        // Token: 0x060018DE RID: 6366 RVA: 0x0013F158 File Offset: 0x0013D358
        public void BitByPlayer(Creature.Grasp grasp, bool eu)
        {
            bites--;
            room.PlaySound((bites != 0) ? SoundID.Slugcat_Bite_Dangle_Fruit : SoundID.Slugcat_Eat_Dangle_Fruit, firstChunk.pos);
            firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
            if (bites < 1)
            {
                (grasp.grabber as Player).ObjectEaten(this);
                grasp.Release();
                Destroy();
            }
        }

        // Token: 0x170003E6 RID: 998
        // (get) Token: 0x060018DF RID: 6367 RVA: 0x0001A74F File Offset: 0x0001894F
        public int FoodPoints
        {
            get
            {
                return 1;
            }
        }

        // Token: 0x170003E7 RID: 999
        // (get) Token: 0x060018E0 RID: 6368 RVA: 0x0001A74F File Offset: 0x0001894F
        public bool Edible
        {
            get
            {
                return true;
            }
        }

        // Token: 0x170003E8 RID: 1000
        // (get) Token: 0x060018E1 RID: 6369 RVA: 0x0001A74F File Offset: 0x0001894F
        public bool AutomaticPickUp
        {
            get
            {
                return true;
            }
        }

        // Token: 0x060018E2 RID: 6370 RVA: 0x00005929 File Offset: 0x00003B29
        public void ThrowByPlayer()
        {
        }

        // Token: 0x04001B15 RID: 6933
        public TreeFruit.Stalk stalk;

        // Token: 0x04001B16 RID: 6934
        public Vector2 rotation;

        // Token: 0x04001B17 RID: 6935
        public Vector2 lastRotation;

        // Token: 0x04001B18 RID: 6936
        public Vector2? setRotation;

        // Token: 0x04001B19 RID: 6937
        public float darkness;

        // Token: 0x04001B1A RID: 6938
        public float lastDarkness;

        // Token: 0x04001B1B RID: 6939
        public int bites;

        // Token: 0x020003F8 RID: 1016
        public class Stalk : UpdatableAndDeletable, IDrawable
        {
            // Token: 0x060018E3 RID: 6371 RVA: 0x0013F1E0 File Offset: 0x0013D3E0
            public Stalk(TreeFruit fruit, Room room, Vector2 fruitPos)
            {
                this.fruit = fruit;
                fruit.firstChunk.HardSetPosition(fruitPos);
                stuckPos.x = fruitPos.x;
                ropeLength = -1f;
                int x = room.GetTilePosition(fruitPos).x;
                for (int i = room.GetTilePosition(fruitPos).y; i < room.TileHeight; i++)
                {
                    if (room.GetTile(x, i).Solid)
                    {
                        stuckPos.y = room.MiddleOfTile(x, i).y - 10f;
                        ropeLength = Mathf.Abs(stuckPos.y - fruitPos.y);
                        break;
                    }
                }
                segs = new Vector2[Math.Max(1, (int)(ropeLength / 15f)), 3];
                for (int j = 0; j < segs.GetLength(0); j++)
                {
                    float t = (float)j / (float)(segs.GetLength(0) - 1);
                    segs[j, 0] = Vector2.Lerp(stuckPos, fruitPos, t);
                    segs[j, 1] = segs[j, 0];
                }
                connRad = ropeLength / Mathf.Pow((float)segs.GetLength(0), 1.1f);
                displacements = new Vector2[segs.GetLength(0)];
                int seed = UnityEngine.Random.seed;
                UnityEngine.Random.seed = fruit.abstractPhysicalObject.ID.RandomSeed;
                for (int k = 0; k < displacements.Length; k++)
                {
                    displacements[k] = Custom.RNV();
                }
                UnityEngine.Random.seed = seed;
            }

            // Token: 0x060018E4 RID: 6372 RVA: 0x0013F3B0 File Offset: 0x0013D5B0
            public override void Update(bool eu)
            {
                base.Update(eu);
                if (ropeLength == -1f)
                {
                    Destroy();
                    return;
                }
                ConnectSegments(true);
                ConnectSegments(false);
                for (int i = 0; i < segs.GetLength(0); i++)
                {
                    float num = (float)i / (float)(segs.GetLength(0) - 1);
                    segs[i, 1] = segs[i, 0];
                    segs[i, 0] += segs[i, 2];
                    segs[i, 2] *= 0.99f;
                    segs[i, 2].y -= 0.9f;
                }
                ConnectSegments(false);
                ConnectSegments(true);
                List<Vector2> list = new List<Vector2>();
                list.Add(stuckPos);
                for (int j = 0; j < segs.GetLength(0); j++)
                {
                    list.Add(segs[j, 0]);
                }
                if (releaseCounter > 0)
                {
                    releaseCounter--;
                }
                if (fruit != null)
                {
                    list.Add(fruit.firstChunk.pos);
                    fruit.setRotation = new Vector2?(Custom.DirVec(fruit.firstChunk.pos, segs[segs.GetLength(0) - 1, 0]));
                    if (!Custom.DistLess(fruit.firstChunk.pos, stuckPos, ropeLength * 1.4f + 10f) || fruit.slatedForDeletetion || fruit.bites < 3 || fruit.room != room || releaseCounter == 1)
                    {
                        fruit.AbstrConsumable.Consume();
                        fruit = null;
                    }
                }
            }

            // Token: 0x060018E5 RID: 6373 RVA: 0x0013F5D8 File Offset: 0x0013D7D8
            private void ConnectSegments(bool dir)
            {
                int num = (!dir) ? (segs.GetLength(0) - 1) : 0;
                bool flag = false;
                while (!flag)
                {
                    if (num == 0)
                    {
                        if (!Custom.DistLess(segs[num, 0], stuckPos, connRad))
                        {
                            Vector2 b = Custom.DirVec(segs[num, 0], stuckPos) * (Vector2.Distance(segs[num, 0], stuckPos) - connRad);
                            segs[num, 0] += b;
                            segs[num, 2] += b;
                        }
                    }
                    else
                    {
                        if (!Custom.DistLess(segs[num, 0], segs[num - 1, 0], connRad))
                        {
                            Vector2 a = Custom.DirVec(segs[num, 0], segs[num - 1, 0]) * (Vector2.Distance(segs[num, 0], segs[num - 1, 0]) - connRad);
                            segs[num, 0] += a * 0.5f;
                            segs[num, 2] += a * 0.5f;
                            segs[num - 1, 0] -= a * 0.5f;
                            segs[num - 1, 2] -= a * 0.5f;
                        }
                        if (num == segs.GetLength(0) - 1 && fruit != null && !Custom.DistLess(segs[num, 0], fruit.firstChunk.pos, connRad))
                        {
                            Vector2 a2 = Custom.DirVec(segs[num, 0], fruit.firstChunk.pos) * (Vector2.Distance(segs[num, 0], fruit.firstChunk.pos) - connRad);
                            segs[num, 0] += a2 * 0.75f;
                            segs[num, 2] += a2 * 0.75f;
                            fruit.firstChunk.vel -= a2 * 0.25f;
                        }
                    }
                    num += ((!dir) ? -1 : 1);
                    if (dir && num >= segs.GetLength(0))
                    {
                        flag = true;
                    }
                    else if (!dir && num < 0)
                    {
                        flag = true;
                    }
                }
            }

            // Token: 0x060018E6 RID: 6374 RVA: 0x0013F90C File Offset: 0x0013DB0C
            public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                sLeaser.sprites = new FSprite[1];
                sLeaser.sprites[0] = TriangleMesh.MakeLongMesh(segs.GetLength(0), false, false);
                AddToContainer(sLeaser, rCam, null);
            }

            // Token: 0x060018E7 RID: 6375 RVA: 0x0013F94C File Offset: 0x0013DB4C
            public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
            {
                Vector2 a = stuckPos;
                float d = 1.5f;
                for (int i = 0; i < segs.GetLength(0); i++)
                {
                    float num = (float)i / (float)(segs.GetLength(0) - 1);
                    float num2 = Custom.LerpMap(num, 0f, 0.5f, 1f, 0f) + Mathf.Lerp(1f, 0.5f, Mathf.Sin(Mathf.Pow(num, 3.5f) * 3.14159274f));
                    Vector2 vector = Vector2.Lerp(segs[i, 1], segs[i, 0], timeStacker);
                    if (i == segs.GetLength(0) - 1 && fruit != null)
                    {
                        vector = Vector2.Lerp(fruit.firstChunk.lastPos, fruit.firstChunk.pos, timeStacker);
                    }
                    Vector2 normalized = (a - vector).normalized;
                    Vector2 a2 = Custom.PerpendicularVector(normalized);
                    if (i < segs.GetLength(0) - 1)
                    {
                        vector += (normalized * displacements[i].y + a2 * displacements[i].x) * Custom.LerpMap(Vector2.Distance(a, vector), connRad, connRad * 5f, 4f, 0f);
                    }
                    vector = new Vector2(Mathf.Floor(vector.x) + 0.5f, Mathf.Floor(vector.y) + 0.5f);
                    (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4, a - a2 * d - camPos);
                    (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 1, a + a2 * d - camPos);
                    (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 2, vector - a2 * num2 - camPos);
                    (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 3, vector + a2 * num2 - camPos);
                    a = vector;
                    d = num2;
                }
                if (slatedForDeletetion || room != rCam.room)
                {
                    sLeaser.CleanSpritesAndRemove();
                }
            }

            // Token: 0x060018E8 RID: 6376 RVA: 0x0013FBD4 File Offset: 0x0013DDD4
            public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
            {
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    sLeaser.sprites[i].color = palette.blackColor;
                }
            }

            // Token: 0x060018E9 RID: 6377 RVA: 0x0013FC08 File Offset: 0x0013DE08
            public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
            {
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    sLeaser.sprites[i].RemoveFromContainer();
                    rCam.ReturnFContainer("Background").AddChild(sLeaser.sprites[i]);
                }
            }

            // Token: 0x04001B1C RID: 6940
            public TreeFruit fruit;

            // Token: 0x04001B1D RID: 6941
            public Vector2 stuckPos;

            // Token: 0x04001B1E RID: 6942
            public float ropeLength;

            // Token: 0x04001B1F RID: 6943
            public Vector2[] displacements;

            // Token: 0x04001B20 RID: 6944
            public Vector2[,] segs;

            // Token: 0x04001B21 RID: 6945
            public int releaseCounter;

            // Token: 0x04001B22 RID: 6946
            private float connRad;
        }
    }
}
