using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using MonoMod;

namespace Rain_World_Drought.OverWorld
{
    public class MoonPearl : DataPearl
    {
        public MoonPearl(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {
            otherMarbles = new List<MoonPearl>();
            orbitAngle = UnityEngine.Random.value * 360f;
            orbitSpeed = 3f;
            orbitDistance = 50f;
            collisionLayer = 0;
            orbitFlattenAngle = UnityEngine.Random.value * 360f;
            orbitFlattenFac = 0.5f + UnityEngine.Random.value * 0.5f;
        }

        public bool NotCarried
        {
            get
            {
                return grabbedBy.Count == 0;
            }
        }

        public override void NewRoom(Room newRoom)
        {
            base.NewRoom(newRoom);
            UpdateOtherMarbles();
        }

        private void UpdateOtherMarbles()
        {
            for (int i = 0; i < otherMarbles.Count; i++)
            {
                otherMarbles[i].otherMarbles.Remove(this);
            }
            otherMarbles.Clear();
            for (int j = 0; j < room.physicalObjects[collisionLayer].Count; j++)
            {
                if (room.physicalObjects[collisionLayer][j] is MoonPearl && room.physicalObjects[collisionLayer][j] != this)
                {
                    if (!(room.physicalObjects[collisionLayer][j] as MoonPearl).otherMarbles.Contains(this))
                    {
                        (room.physicalObjects[collisionLayer][j] as MoonPearl).otherMarbles.Add(this);
                    }
                    if (!otherMarbles.Contains(room.physicalObjects[collisionLayer][j] as MoonPearl))
                    {
                        otherMarbles.Add(room.physicalObjects[collisionLayer][j] as MoonPearl);
                    }
                }
            }
        }

        public override void Update(bool eu)
        {
            if (!lookForMarbles)
            {
                UpdateOtherMarbles();
                lookForMarbles = true;
            }
            if (oracle != null && oracle.room != room)
            {
                oracle = null;
            }
            abstractPhysicalObject.destroyOnAbstraction = (oracle != null);
            if (label != null)
            {
                label.setPos = new Vector2?(firstChunk.pos);
                if (label.room != room)
                {
                    label.Destroy();
                }
            }
            else
            {
                label = new GlyphLabel(firstChunk.pos, GlyphLabel.RandomString(1, 1, 12842 + (abstractPhysicalObject as MoonPearl.AbstractMoonPearl).number, false));
                room.AddObject(label);
            }
            base.Update(eu);
            float num = orbitAngle;
            float num2 = orbitSpeed;
            float num3 = orbitDistance;
            float axis = orbitFlattenAngle;
            float num4 = orbitFlattenFac;
            if (room.gravity < 1f && NotCarried)
            {
                Vector2 a = firstChunk.pos;
                if (orbitObj != null)
                {
                    int num5 = 0;
                    int num6 = 0;
                    int number = abstractPhysicalObject.ID.number;
                    for (int i = 0; i < otherMarbles.Count; i++)
                    {
                        if (otherMarbles[i].orbitObj == orbitObj && otherMarbles[i].NotCarried && Custom.DistLess(otherMarbles[i].firstChunk.pos, orbitObj.firstChunk.pos, otherMarbles[i].orbitDistance * 4f) && otherMarbles[i].orbitCircle == orbitCircle)
                        {
                            num3 += otherMarbles[i].orbitDistance;
                            if (otherMarbles[i].abstractPhysicalObject.ID.number < abstractPhysicalObject.ID.number)
                            {
                                num6++;
                            }
                            num5++;
                            if (otherMarbles[i].abstractPhysicalObject.ID.number < number)
                            {
                                number = otherMarbles[i].abstractPhysicalObject.ID.number;
                                num = otherMarbles[i].orbitAngle;
                                num2 = otherMarbles[i].orbitSpeed;
                                axis = otherMarbles[i].orbitFlattenAngle;
                                num4 = otherMarbles[i].orbitFlattenFac;
                            }
                        }
                    }
                    num3 /= (float)(1 + num5);
                    num += (float)num6 * (360f / (float)(num5 + 1));
                    Vector2 a2 = orbitObj.firstChunk.pos;
                    if (orbitObj is Oracle && orbitObj.graphicsModule != null)
                    {
                        a2 = (orbitObj.graphicsModule as OracleGraphics).halo.Center(1f);
                    }
                    a = a2 + Custom.FlattenVectorAlongAxis(Custom.DegToVec(num), axis, num4) * num3 * Mathf.Lerp(1f / num4, 1f, 0.5f);
                }
                else if (hoverPos != null)
                {
                    a = hoverPos.Value;
                }
                firstChunk.vel *= Custom.LerpMap(firstChunk.vel.magnitude, 1f, 6f, 0.999f, 0.9f);
                firstChunk.vel += Vector2.ClampMagnitude(a - firstChunk.pos, 100f) / 100f * 0.4f * (1f - room.gravity);
            }
            orbitAngle += num2 * ((orbitCircle % 2 != 0) ? -1f : 1f);
        }

        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);
            if (firstContact && speed > 2f)
            {
                room.PlaySound(SoundID.SS_AI_Marble_Hit_Floor, firstChunk, false, Custom.LerpMap(speed, 0f, 8f, 0.2f, 1f), 1f);
            }
        }

        public PhysicalObject orbitObj;
        public List<MoonPearl> otherMarbles;
        public Vector2? hoverPos;
        public float orbitAngle;
        public float orbitSpeed;
        public float orbitDistance;
        public float orbitFlattenAngle;
        public float orbitFlattenFac;
        public int orbitCircle;
        public int marbleColor;
        private bool lookForMarbles;
        public GlyphLabel label;
        public Oracle oracle;

        public class AbstractMoonPearl : DataPearl.AbstractDataPearl
        {
            public AbstractMoonPearl(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedObjectIndex, PlacedObject.ConsumableObjectData consumableData, int color, int number) : base(world, (AbstractPhysicalObject.AbstractObjectType)patch_AbstractPhysicalObject.AbstractObjectType.MoonPearl, realizedObject, pos, ID, originRoom, placedObjectIndex, consumableData, (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.MoonPearl)
            {
                this.color = color;
                this.number = number;
            }

            public override string ToString()
            {
                return string.Concat(new object[]
                {
                base.ToString(),
                "<oA>",
                color,
                "<oA>",
                number
                });
            }

            public int color;
            public int number;
        }
    }
}
