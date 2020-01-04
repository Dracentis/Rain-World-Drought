using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using MonoMod;

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

    // Token: 0x06002228 RID: 8744 RVA: 0x0020A298 File Offset: 0x00208498
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

    // Token: 0x06002229 RID: 8745 RVA: 0x0020A708 File Offset: 0x00208908
    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);
        if (firstContact && speed > 2f)
        {
            room.PlaySound(SoundID.SS_AI_Marble_Hit_Floor, firstChunk, false, Custom.LerpMap(speed, 0f, 8f, 0.2f, 1f), 1f);
        }
    }

    // Token: 0x040024EE RID: 9454
    public PhysicalObject orbitObj;

    // Token: 0x040024EF RID: 9455
    public List<MoonPearl> otherMarbles;

    // Token: 0x040024F0 RID: 9456
    public Vector2? hoverPos;

    // Token: 0x040024F1 RID: 9457
    public float orbitAngle;

    // Token: 0x040024F2 RID: 9458
    public float orbitSpeed;

    // Token: 0x040024F3 RID: 9459
    public float orbitDistance;

    // Token: 0x040024F4 RID: 9460
    public float orbitFlattenAngle;

    // Token: 0x040024F5 RID: 9461
    public float orbitFlattenFac;

    // Token: 0x040024F6 RID: 9462
    public int orbitCircle;

    // Token: 0x040024F7 RID: 9463
    public int marbleColor;

    // Token: 0x040024F8 RID: 9464
    private bool lookForMarbles;

    // Token: 0x040024F9 RID: 9465
    public GlyphLabel label;

    // Token: 0x040024FA RID: 9466
    public Oracle oracle;

    // Token: 0x0200051D RID: 1309
    public class AbstractMoonPearl : DataPearl.AbstractDataPearl
    {
        // Token: 0x0600222A RID: 8746 RVA: 0x0020A764 File Offset: 0x00208964
        public AbstractMoonPearl(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedObjectIndex, PlacedObject.ConsumableObjectData consumableData, int color, int number) : base(world, (AbstractPhysicalObject.AbstractObjectType)patch_AbstractPhysicalObject.AbstractObjectType.MoonPearl, realizedObject, pos, ID, originRoom, placedObjectIndex, consumableData, (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.MoonPearl)
        {
            this.color = color;
            this.number = number;
        }

        // Token: 0x0600222B RID: 8747 RVA: 0x0020A798 File Offset: 0x00208998
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

        // Token: 0x040024FB RID: 9467
        public int color;

        // Token: 0x040024FC RID: 9468
        public int number;
    }
}
