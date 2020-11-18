using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Rain_World_Drought.Enums;
using Random = UnityEngine.Random;
using RWCustom;

namespace Rain_World_Drought.PlacedObjects
{
    public abstract class Piston : PhysicalObject, IDrawable
    {
        public Piston(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 0.1f, 14f);
            bodyChunkConnections = new BodyChunkConnection[0];
            airFriction = 0.9f;
            gravity = 0f;
            bounce = 0f;
            surfaceFriction = 9.9f;
            collisionLayer = 1;
            deathmode = AbstractPist.deathmode;
            deathtop = AbstractPist.deathtop;
            waterFriction = 0.1f;
            buoyancy = 0f;
            bodyChunks[0].collideWithObjects = false;
            bodyChunks[0].collideWithTerrain = false;
            bodyChunks[0].collideWithSlopes = false;
            offset = Random.Range(0, Mathf.PI);
        }

        public AbstractPiston AbstractPist => abstractPhysicalObject as AbstractPiston;

        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);
            if (AbstractPist.placedObjectIndex >= 0 && AbstractPist.placedObjectIndex < placeRoom.roomSettings.placedObjects.Count)
            {
                startPos = placeRoom.MiddleOfTile(placeRoom.roomSettings.placedObjects[AbstractPist.placedObjectIndex].pos);
                placedPos = startPos;
            }
            bodyChunks[0].pos = placedPos;
            bodyChunks[0].lastPos = placedPos;
        }

        public override void Update(bool eu)
        {
            lastCrush = crush;
            crush = Custom.LerpAndTick(crush, 0f, 0.1f, 0.15f);
            base.Update(eu);
            try
            {
                Vector2 lastPos = placedPos;
                float pos = MaxMovement * Mathf.Sin(room.world.rainCycle.timer % 500 * (Mathf.PI * 2f / 500f) + offset);
                placedPos.x = startPos.x;
                placedPos.y = startPos.y + pos;
                bodyChunks[0].pos = placedPos;
                bodyChunks[0].lastPos = placedPos;

                if (room != null)
                {
                    // Play a sound when about to turn around
                    bool playSound = Mathf.Abs(pos) > MaxMovement * 0.9f;
                    if (playSound && !hasPlayedSound)
                        PlaySound(pos > 0f);
                    hasPlayedSound = playSound;

                    for (int i = 0; i < room.physicalObjects.Length; i++)
                    {
                        for (int j = 0; j < room.physicalObjects[i].Count; j++)
                        {
                            if (room.physicalObjects[i][j].abstractPhysicalObject.type == EnumExt_Drought.Piston) continue;
                            for (int k = 0; k < room.physicalObjects[i][j].bodyChunks.Length; k++)
                            {
                                BodyChunk chunk = room.physicalObjects[i][j].bodyChunks[k];
                                if (!chunk.collideWithTerrain) continue;

                                // Do crush animation
                                Vector2 idealPos = chunk.pos;

                                if ((Mathf.Abs(idealPos.x - placedPos.x) - chunk.rad * 0.5f < collisionSize.x - 3f) && (Mathf.Abs(idealPos.y - placedPos.y) - chunk.rad < collisionSize.y + 5f))
                                {
                                    float dir = Mathf.Sign(idealPos.y - placedPos.y);

                                    // Find approximately where the chunk would rest if it wasn't squished
                                    idealPos.y = placedPos.y + dir * (collisionSize.y + chunk.rad);
                                    if (room.GetTile(idealPos + Vector2.up * dir * chunk.rad).Solid)
                                    {
                                        // Apparently broken
                                        //idealPos = SharedPhysics.TraceTerrainCollision(room, idealPos, new Vector2(idealPos.x, placedPos.y), chunk.rad, true);
                                        Vector2 from = new Vector2(idealPos.x, placedPos.y);
                                        idealPos = CastChunkAgainstTerrain(idealPos, from, chunk.rad, true);
                                        
                                        // Update crush animation
                                        float newCrush = (idealPos.y - chunk.rad * dir) - (placedPos.y + dir * collisionSize.y);
                                        if (Mathf.Abs(newCrush) > Mathf.Abs(crush)) crush = newCrush;
                                    }
                                }

                                PushOutChunk(chunk, placedPos);
                            }
                        }
                    }
                    /*
                    if (room.Darkness(placedPos) > 0f)
                    {
                        if (light == null)
                        {
                            light = new LightSource(placedPos, false, Color.blue, this);
                            room.AddObject(light);
                        }
                        light.setPos = new Vector2?(placedPos);
                        light.setAlpha = 0.68f;
                        light.setRad = W*1.2f;
                    }
                    else if (light != null)
                    {
                        light.Destroy();
                        light = null;
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        protected abstract float MaxMovement { get; }

        protected virtual void PlaySound(bool atTop)
        {
            if (atTop)
                room.PlaySound(SoundID.Gate_Pillows_Move_Out, placedPos, 0.75f, 0.1f);
            else
                room.PlaySound(SoundID.Gate_Pillows_Move_In, placedPos, 0.75f, 0.1f);
        }

        private Vector2 CastChunkAgainstTerrain(Vector2 pos, Vector2 lastPos, float rad, bool goThroughFloors)
        {
            SharedPhysics.TerrainCollisionData cd = new SharedPhysics.TerrainCollisionData(pos, lastPos, pos - lastPos, rad, new IntVector2(0, 0), true);
            cd = SharedPhysics.VerticalCollision(room, cd);
            cd = SharedPhysics.SlopesVertically(room, cd);
            cd = SharedPhysics.HorizontalCollision(room, cd);
            return cd.pos;
        }
        
        private void PushOutChunk(BodyChunk chunk, Vector2 pos)
        {
            Vector2 startPos = chunk.pos;
            float rad = chunk.TerrainRad + 0.01f;
            
            // Exit early if the chunk could not be in contact with the piston
            if (Mathf.Abs(startPos.x - pos.x) > collisionSize.x + rad || Mathf.Abs(startPos.y - pos.y) > collisionSize.y + rad) return;

            Vector2 vert  = PistonPhysics.CastChunk(startPos + new Vector2(0f, 10000f * Mathf.Sign(startPos.y - pos.y)), startPos, rad, this);
            Vector2 horiz = PistonPhysics.CastChunk(startPos + new Vector2(10000f * Mathf.Sign(startPos.x - pos.x), 0f), startPos, rad, this);
            
            // Move in the direction that would require the least movement
            if(Vector2.SqrMagnitude(vert - startPos) <= Vector2.SqrMagnitude(horiz - startPos))
            {
                if(vert != startPos)
                {
                    Vector2 outOfPistonPos = vert;
                    // Make sure that the chunk is not forced into a wall
                    vert = CastChunkAgainstTerrain(vert, startPos, chunk.rad, true);

                    // Pushed vertically
                    chunk.pos = vert;
                    if (outOfPistonPos != vert && Mathf.Abs(crush) > 1f)
                    {
                        // This chunk has been pushed into a wall vertically
                        // KILL
                        Crush(chunk, true);
                    }
                }
            } else
            {
                if (horiz != startPos)
                {
                    Vector2 outOfPistonPos = horiz;
                    // Make sure that the chunk is not forced into a wall
                    horiz = CastChunkAgainstTerrain(horiz, startPos, chunk.rad, true);

                    // Pushed horizontally
                    chunk.pos = horiz;
                    if (outOfPistonPos != horiz && Mathf.Abs(crush) > 1f)
                    {
                        // This chunk has been pushed into a wall horizontally
                        // KILL
                        Crush(chunk, false);
                    }
                }
            }
        }

        public void Crush(BodyChunk chunk, bool vertical)
        {
            if (chunk.owner is Creature crit && !crit.dead)
                crit.Violence(null, null, chunk, null, Creature.DamageType.Blunt, 10f, 0f);
        }

        public override void HitByWeapon(Weapon weapon)
        {
            base.HitByWeapon(weapon);
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[spriteName == null ? 1 : 3];
            sLeaser.sprites[0] = new FSprite("guardHead", true);
            if (spriteName != null)
            {
                sLeaser.sprites[1] = new FSprite(spriteName + "A", true);
                sLeaser.sprites[2] = new FSprite(spriteName + "B", true);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].x = placedPos.x;
                sLeaser.sprites[i].y = placedPos.y;
                sLeaser.sprites[i].alpha = 1f;
            }
            //60x60
            //120x80
            //160x120
            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 offset = new Vector2(0f, Mathf.Lerp(lastCrush, crush, timeStacker) * (Random.value * 0.5f + 0.25f));
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].x = placedPos.x - camPos.x + offset.x;
                sLeaser.sprites[i].y = placedPos.y - camPos.y + offset.y;
            }
            sLeaser.sprites[0].scaleY = spriteScale.x;
            sLeaser.sprites[0].scaleX = spriteScale.y;
            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[0].color = palette.blackColor;
            if (sLeaser.sprites.Length >= 3)
            {
                sLeaser.sprites[1].color = palette.texture.GetPixel(2, 10);
                sLeaser.sprites[2].color = palette.blackColor;
            }
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
            {
                newContatiner = rCam.ReturnFContainer("Items");
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].RemoveFromContainer();
                newContatiner.AddChild(sLeaser.sprites[i]);
            }
        }

        protected string spriteName;
        protected Vector2 spriteScale;
        // Technically the half-size of the block
        public Vector2 collisionSize;

        public Vector2 placedPos;

        public Vector2 startPos;

        public float offset;

        public bool deathmode;

        public bool deathtop;

        public bool hasPlayedSound = false;

        private LightSource light;

        // Represents the offset necessary to not crush a chunk
        private float crush;
        private float lastCrush;

        public enum PistonType
        {
            Giant, Large, Small
        }

        public class AbstractPiston : AbstractPhysicalObject
        {
            public AbstractPiston(World world, PistonType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedIndex, bool db, bool dt) : base(world, EnumExt_Drought.Piston, realizedObject, pos, ID)
            {
                this.originRoom = originRoom;
                placedObjectIndex = placedIndex;
                pistonType = type;
                deathmode = db;
                deathtop = dt;
            }

            public PistonType pistonType;
            public int placedObjectIndex;
            public int originRoom;
            public bool deathmode;
            public bool deathtop;
        }
    }
}
