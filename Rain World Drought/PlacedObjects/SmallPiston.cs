using System;
using UnityEngine;

namespace Rain_World_Drought.PlacedObjects
{
    public class SmallPiston : PhysicalObject, IDrawable
    {
        public SmallPiston(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 20f, 14f);
            bodyChunkConnections = new BodyChunkConnection[0];
            airFriction = 0.9f;
            gravity = 0f;
            bounce = 0f;
            surfaceFriction = 9.9f;
            collisionLayer = 1;
            deathmode = (abstractPhysicalObject as AbstractSmallPiston).deathmode;
            deathtop = (abstractPhysicalObject as AbstractSmallPiston).deathtop;
            waterFriction = 0.1f;
            buoyancy = 0f;
            bodyChunks[0].collideWithObjects = true;
            bodyChunks[0].collideWithTerrain = false;
            bodyChunks[0].collideWithSlopes = false;
            offset = UnityEngine.Random.Range(-1f, 1f);
        }

        public SmallPiston.AbstractSmallPiston AbstractSmallPist
        {
            get
            {
                return abstractPhysicalObject as SmallPiston.AbstractSmallPiston;
            }
        }

        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);
            if (AbstractSmallPist.placedObjectIndex >= 0 && AbstractSmallPist.placedObjectIndex < placeRoom.roomSettings.placedObjects.Count)
            {
                startPos = placeRoom.MiddleOfTile(placeRoom.roomSettings.placedObjects[AbstractSmallPist.placedObjectIndex].pos);
                placedPos = startPos;
            }
            bodyChunks[0].pos = placedPos + new Vector2(0f, 0f);
            bodyChunks[0].lastPos = placedPos + new Vector2(0f, 0f);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            try
            {
                float pos = 40f * (float)Math.Sin((double)((float)room.world.rainCycle.timer % 500f / 80.15f + offset));
                placedPos.x = startPos.x;
                placedPos.y = startPos.y + pos;
                bodyChunks[0].pos = placedPos + new Vector2(0f, 0f);
                bodyChunks[0].lastPos = placedPos + new Vector2(0f, 0f);

                float W = 60;
                float H = 60;
                if (room != null)
                {
                    if (pos < -28f && !tuchbot)
                    {
                        tuchbot = true;
                        //room.PlaySound(SoundID.Gate_Pillows_Move_In, placedPos, 0.5f, 0.1f);
                    }
                    else if (pos > 28f && tuchbot)
                    {
                        tuchbot = false;
                        //room.PlaySound(SoundID.Gate_Pillows_Move_Out, placedPos, 0.5f, 0.1f);
                    }
                    for (int i = 0; i < room.physicalObjects.Length; i++)
                    {
                        for (int j = 0; j < room.physicalObjects[i].Count; j++)
                        {
                            for (int k = 0; k < room.physicalObjects[i][j].bodyChunks.Length; k++)
                            {
                                if (pos > 79f)
                                {
                                    if (PistonPhysics.CheckVerticalPistonCollision(room.physicalObjects[i][j].bodyChunks[k], placedPos, W + 10, H + 10, false, deathtop))
                                    {
                                    }
                                }
                                else if (pos < -79f)
                                {
                                    if (PistonPhysics.CheckVerticalPistonCollision(room.physicalObjects[i][j].bodyChunks[k], placedPos, W + 10, H + 10, deathmode, false))
                                    {
                                    }
                                }
                                else
                                {
                                    if (PistonPhysics.CheckVerticalPistonCollision(room.physicalObjects[i][j].bodyChunks[k], placedPos, W + 10, H + 10, false, false))
                                    {
                                    }
                                }
                                if (room.physicalObjects[i][j].bodyChunks[k].collideWithTerrain)
                                {
                                    if (PistonPhysics.CheckHorizontalPistonCollision(room.physicalObjects[i][j].bodyChunks[k], placedPos, W + 10, H + 10))
                                    {
                                    }
                                }
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

        public override void HitByWeapon(Weapon weapon)
        {
            base.HitByWeapon(weapon);
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[3];
            sLeaser.sprites[0] = new FSprite("guardHead", true);
            sLeaser.sprites[1] = new FSprite("SmallPistonA", true);
            sLeaser.sprites[2] = new FSprite("SmallPistonB", true);
            for (int i = 0; i < 3; i++)
            {
                sLeaser.sprites[i].x = placedPos.x;
                sLeaser.sprites[i].y = placedPos.y;
                sLeaser.sprites[i].alpha = 1f;
            }
            sLeaser.sprites[0].scaleX = 1.2f;
            sLeaser.sprites[0].scaleY = 1.57894742f;
            //60x60
            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            for (int i = 0; i < 3; i++)
            {
                sLeaser.sprites[i].x = placedPos.x - camPos.x;
                sLeaser.sprites[i].y = placedPos.y - camPos.y;
            }
            sLeaser.sprites[0].scaleY = 1.57894742f;
            sLeaser.sprites[0].scaleX = 1.2f;
            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[0].color = palette.blackColor;
            sLeaser.sprites[1].color = palette.texture.GetPixel(2, 10);
            sLeaser.sprites[2].color = palette.blackColor;
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

        public Vector2 placedPos;

        public Vector2 startPos;

        public float offset;

        public bool deathmode;

        public bool deathtop;

        private LightSource light;

        private bool tuchbot;

        public class AbstractSmallPiston : AbstractPhysicalObject
        {
            public AbstractSmallPiston(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedIndex, bool db, bool dt) : base(world, (AbstractPhysicalObject.AbstractObjectType)patch_AbstractPhysicalObject.AbstractObjectType.SmallPiston, realizedObject, pos, ID)
            {
                this.originRoom = originRoom;
                placedObjectIndex = placedIndex;
                deathmode = db;
                deathtop = dt;
            }

            public int placedObjectIndex;

            public int originRoom;

            public bool deathmode;

            public bool deathtop;
        }
    }
}
