using System;
using UnityEngine;



    public class GiantPiston : PhysicalObject, IDrawable
    {
        public GiantPiston(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
    {
        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 30f, 14f);
        bodyChunkConnections = new BodyChunkConnection[0];
        airFriction = 0.9f;
        gravity = 0f;
        bounce = 0f;
        surfaceFriction = 9.9f;
        collisionLayer = 1;
        deathmode = (abstractPhysicalObject as AbstractGiantPiston).deathmode;
        deathtop = (abstractPhysicalObject as AbstractGiantPiston).deathtop;
        waterFriction = 0.1f;
        buoyancy = 0f;
        bodyChunks[0].collideWithObjects = true;
        bodyChunks[0].collideWithTerrain = false;
        bodyChunks[0].collideWithSlopes = false;
        offset = UnityEngine.Random.Range(-1f, 1f);
    }

        
        public GiantPiston.AbstractGiantPiston AbstractGiantPist
        {
            get
            {
                return abstractPhysicalObject as GiantPiston.AbstractGiantPiston;
            }
        }
        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);
            if (AbstractGiantPist.placedObjectIndex >= 0 && AbstractGiantPist.placedObjectIndex < placeRoom.roomSettings.placedObjects.Count)
            {
                startPos = placeRoom.MiddleOfTile(placeRoom.roomSettings.placedObjects[AbstractGiantPist.placedObjectIndex].pos);
            startPos.x = startPos.x - 10f;

            startPos.y = startPos.y - 10f;
            placedPos = startPos;
        }
        bodyChunks[0].pos = placedPos + new Vector2(0f, 0f);
        bodyChunks[0].lastPos = placedPos + new Vector2(0f, 0f);
    }

        public override void Update(bool eu)
        {
            base.Update(eu);
        try { 
            float pos = 60f * (float)Math.Sin((double)((float)room.world.rainCycle.timer % 500f / 80.15f + offset));
            placedPos.x = startPos.x;
            placedPos.y = startPos.y + pos;
            bodyChunks[0].pos = placedPos + new Vector2(0f, 0f);
            bodyChunks[0].lastPos = placedPos + new Vector2(0f, 0f);
        if (room != null)
        {
            if (pos < -78f && !tuchbot)
            {
                tuchbot = true;
                //room.PlaySound(SoundID.Gate_Pillows_Move_In, placedPos, 0.7f, 0.3f);
                //room.PlaySound(SoundID.Shelter_Piston_In_Soft, placedPos, 0.8f, 0.5f);
            }
            else if (pos > 78f && tuchbot)
            {
                tuchbot = false;
                //room.PlaySound(SoundID.Gate_Pillows_Move_Out, placedPos, 0.7f, 0.3f);
                //room.PlaySound(SoundID.Shelter_Piston_Out, placedPos, 0.8f, 0.5f);
            }
                float W = 160;
                float H = 120;
                for (int i = 0; i < room.physicalObjects.Length; i++)
                {
                    for (int j = 0; j < room.physicalObjects[i].Count; j++)
                    {
                        for (int k = 0; k < room.physicalObjects[i][j].bodyChunks.Length; k++)
                        {
                            if (pos > 79f)
                            {
                                if ((room.physicalObjects[i][j].bodyChunks[k] as patch_BodyChunk).CheckVerticalPistonCollision(placedPos, W + 10, H + 10, false, deathtop))
                                {
                                }
                            }
                            else if (pos < -79f)
                            {
                                if ((room.physicalObjects[i][j].bodyChunks[k] as patch_BodyChunk).CheckVerticalPistonCollision(placedPos, W + 10, H + 10, deathmode, false))
                                {
                                }
                            }
                            else
                            {
                                if ((room.physicalObjects[i][j].bodyChunks[k] as patch_BodyChunk).CheckVerticalPistonCollision(placedPos, W + 10, H + 10, false, false))
                                {
                                }
                            }
                            if (room.physicalObjects[i][j].bodyChunks[k].collideWithTerrain)
                            {
                                if ((room.physicalObjects[i][j].bodyChunks[k] as patch_BodyChunk).CheckHorizontalPistonCollision(placedPos, W + 10, H + 10))
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
        
    }catch (Exception e)
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
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("guardHead", true);
        sLeaser.sprites[0].x = placedPos.x;
        sLeaser.sprites[0].y = placedPos.y;
        sLeaser.sprites[0].alpha = 1f;
        
        sLeaser.sprites[0].scaleX = 3.2f;
        sLeaser.sprites[0].scaleY = 3.1578947368f;
        //160x120
        AddToContainer(sLeaser, rCam, null);
    }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        sLeaser.sprites[0].x = placedPos.x - camPos.x;
        sLeaser.sprites[0].y = placedPos.y - camPos.y;
        sLeaser.sprites[0].scaleY = 3.1578947368f;
        sLeaser.sprites[0].scaleX = 3.2f;
            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = palette.blackColor;
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

        public class AbstractGiantPiston : AbstractPhysicalObject
        {
            public AbstractGiantPiston(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedIndex, bool dt, bool db) : base(world, (AbstractPhysicalObject.AbstractObjectType)patch_AbstractPhysicalObject.AbstractObjectType.GiantPiston, realizedObject, pos, ID)
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


