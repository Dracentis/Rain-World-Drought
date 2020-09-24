using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;

// Token: 0x02000458 RID: 1112
public class WalkerBeast : Creature
{
    // Token: 0x06001BF7 RID: 7159 RVA: 0x0018DE2C File Offset: 0x0018C02C
    public WalkerBeast(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
        //Right now I've left a few peices of unnessesary deer code as they seemed to be nessesary for the pather and deer movement.
        smoothMoveDirection = new Vector2(0f, 0f);
        GenerateIVars();
        collisionRange = 1000f;
        bodyChunks = new BodyChunk[7];// 7 body chunks 0 = head. {1, 2, 3, 4} = deer body. 5 = antlers. 6 = new MirosBird head 
        bodyChunkConnections = new PhysicalObject.BodyChunkConnection[5];

        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 1f, 1f);//this is the head of the deer originally

        bodyChunks[6] = new BodyChunk(this, 4, new Vector2(0f, 0f), 9f, 0.6f);//this is the new Head chunk
        bodyChunks[6].goThroughFloors = true; 
        for (int i = 1; i < 5; i++)//adds main body chunks
        {
            float num = (float)i / 4f;
            num = (1f - num) * 0.5f + Mathf.Sin(Mathf.Pow(num, 0.5f) * 3.14159274f) * 0.5f;
            num = Mathf.Pow(Mathf.Lerp(num, 1f, 0.2f), 0.7f);
            bodyChunks[i] = new BodyChunk(this, i, new Vector2(0f, 0f), Mathf.Lerp(10f, 20f, num), Mathf.Lerp(1f, 8f, num));
            bodyChunks[i].restrictInRoomRange = 2000f;
        }
        bodyChunkConnections[0] = new PhysicalObject.BodyChunkConnection(bodyChunks[0], bodyChunks[1], 15f, BodyChunkConnection.Type.Normal, 1f, -1f);
        for (int j = 1; j < 4; j++)
        {
            float num2 = (float)j / 3f;
            bodyChunkConnections[j] = new PhysicalObject.BodyChunkConnection(bodyChunks[j], bodyChunks[j + 1], Mathf.Max(bodyChunks[j].rad, bodyChunks[j + 1].rad) * 0.8f, BodyChunkConnection.Type.Normal, 1f, -1f);
        }
        bodyChunks[5] = new BodyChunk(this, 0, new Vector2(0f, 0f), Mathf.Lerp(30f, 70f, abstractCreature.personality.dominance), 0.5f);
        bodyChunkConnections[4] = new PhysicalObject.BodyChunkConnection(bodyChunks[0], antlers, bodyChunks[0].rad + antlers.rad - 10f, BodyChunkConnection.Type.Normal, 1f, 0f);
        antlers.collideWithObjects = false;
        bodyChunks[5].collideWithSlopes = false;
        bodyChunks[5].collideWithTerrain = false;
        //base.bodyChunks[0].rotationChunk = base.bodyChunks[5];
        //base.bodyChunks[5].rotationChunk = base.bodyChunks[0];
        legs = new WalkerBeastTentacle[4];
        for (int k = 0; k < 4; k++)
        {
            legs[k] = new WalkerBeastTentacle(this, bodyChunks[(k >= 2) ? 2 : 1], 30f, k);
        }
        //MY ATTEMPT AT ADDING THE NECK FROM MIROSBIRDS
        neck = new Tentacle(this, bodyChunks[0], 200f);
        neck.tProps = new Tentacle.TentacleProps(false, false, true, 0.5f, 0.1f, 0.5f, 1.8f, 0.2f, 1.2f, 10f, 0.25f, 10f, 15, 20, 20, 0);
        neck.tChunks = new Tentacle.TentacleChunk[8];
        for (int n = 0; n < neck.tChunks.Length; n++)
        {
            neck.tChunks[n] = new Tentacle.TentacleChunk(neck, n, (float)(n + 1) / (float)neck.tChunks.Length, 3f);
        }
        neck.tChunks[neck.tChunks.Length - 1].rad = 5f;
        neck.stretchAndSqueeze = 0f;
        //-----------------------------------------------
        flipDir = ((UnityEngine.Random.value >= 0.5f) ? 1f : -1f);
        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.1f;
        surfaceFriction = 0.4f;
        collisionLayer = 1;
        waterFriction = 0.95f;
        waterRetardationImmunity = 0f;
        buoyancy = 0.93f;
        GoThroughFloors = true;
        playersInAntlers = new List<WalkerBeast.PlayerInAntlers>();//should be removed in the future
    }
    
    public Vector2 HeadDir
    {
        get
        {
            return Vector3.Slerp(Custom.PerpendicularVector(bodDir) * Mathf.Sign(bodDir.x), new Vector2(0f, 1f), Mathf.Pow(Mathf.InverseLerp(1f, 0f, Mathf.Abs(bodDir.x)), 0.5f));
        }
    }
    
    public BodyChunk antlers
    {
        get
        {
            return bodyChunks[5];
        }
    }
    
    public float Hierarchy
    {
        get
        {
            return abstractCreature.personality.dominance;
        }
    }
    
    public float GetUnstuckForce
    {
        get
        {
            return AI.stuckTracker.Utility();
        }
    }
    
    
    public float CloseToEdge
    {
        get
        {
            return Mathf.Max(Mathf.InverseLerp(500f, 300f, mainBodyChunk.pos.x), Mathf.InverseLerp(room.PixelWidth - 500f, room.PixelWidth - 300f, mainBodyChunk.pos.x));
        }
    }
    
    private void GenerateIVars()
    {
        int seed = UnityEngine.Random.seed;
        UnityEngine.Random.seed = abstractCreature.ID.RandomSeed;
        float num = Custom.WrappedRandomVariation(0.65f, 0.2f, 0.8f);
        iVars = new WalkerBeast.IndividualVariations(UnityEngine.Random.value, UnityEngine.Random.Range(0, int.MaxValue), new HSLColor(num, Mathf.Lerp(0.5f, 0.95f, UnityEngine.Random.value), Mathf.Lerp(0.12f, 0.18f, UnityEngine.Random.value)), new HSLColor(num + ((UnityEngine.Random.value >= 0.5f) ? 0.15f : -0.15f), 1f, 0.2f), (UnityEngine.Random.value >= 0.8f));
        UnityEngine.Random.seed = seed;
    }
    
    public override void InitiateGraphicsModule()
    {
        if (graphicsModule == null)
        {
            graphicsModule = new WalkerBeastGraphics(this);
        }
    }
    
    public override void NewRoom(Room newRoom)
    {
        base.NewRoom(newRoom);
        preferredHeight = 25f;
        for (int i = 0; i < 4; i++)
        {
            legs[i].NewRoom(newRoom);
        }
        enterRoomForcePush = 0f;
        neck.NewRoom(room); //fix
    }

    public override void SpitOutOfShortCut(IntVector2 pos, Room newRoom, bool spitOutAllSticks)
    {
        neck.Reset(mainBodyChunk.pos);
        base.SpitOutOfShortCut(pos, newRoom, spitOutAllSticks);
    }

    public override void Update(bool eu)
    {
        if (violenceReaction > 0)
        {
            violenceReaction--;
        }
        WeightedPush(1, 3, Vector3.Slerp(bodDir, new Vector2(flipDir, 0f), (!Consious) ? 0f : 0.3f), 0.35f);
        WeightedPush(1, 4, Vector3.Slerp(bodDir, new Vector2(flipDir, 0f), (!Consious) ? 0f : 0.3f), 0.35f);
        //code for forcing antler chunk down to simulate the forward facing neck
        WeightedPush(0, 1, HeadDir, 0.85f);
        WeightedPush(0, 1, bodDir, 0.6f);
            for (int i = 0; i < 4; i++)
            {
            WeightedPush(5, i, (!Consious) ? Custom.DirVec(bodyChunks[i].pos, antlers.pos) : (bodDir * 0.4f + HeadDir), 1.1f - (float)i * 0.3f);
            }
        WeightedPush(4, 5, new Vector2(flipDir, 0f), 0.6f);
        //------------------------------------
        antlers.vel *= 0.92f;
        base.Update(eu);
        if (room == null || enteringShortCut != null)
        {
            return;
        }
        UpdateNeck();//added neck update code

        legsGrabbing = 0;
        float num = 0f;
        float num2 = float.MinValue;
        WalkerBeastTentacle WalkerBeastTentacle = null;
        for (int k = 0; k < 4; k++)
        {
            legs[k].Update();
            if (legs[k].attachedAtTip)
            {
                legsGrabbing++;
            }
            if (legs[k].ReleaseScore() > num2)
            {
                num2 = legs[k].ReleaseScore();
                WalkerBeastTentacle = legs[k];
            }
            num += legs[k].Support();
        }
        if (legsGrabbing > 3 && WalkerBeastTentacle != null)
        {
            WalkerBeastTentacle.ReleaseGrip();
        }
        num = Mathf.Pow(Mathf.Min(num / 3f, 1f), 0.8f) * Mathf.Pow(1f, 0.3f);
        for (int l = 0; l < 5; l++)
        {
            if (bodyChunks[l].ContactPoint.x != 0 || bodyChunks[l].ContactPoint.y < 0)
            {
                num = Mathf.Lerp(num, 1f, 0.6f);
            }
        }
        num = Mathf.Lerp(num, 1f, CloseToEdge);
        num = Mathf.Pow(num, 0.1f);
        float num3 = Mathf.Lerp(Mathf.Pow((float)legsGrabbing / 4f, 0.8f), num, 0.5f);
        bool flag = false;
        int num4 = 0;
        while (num4 < 4 && !flag)
        {
            if (legs[num4].attachedAtTip && legs[num4].Tip.pos.x > bodyChunks[2].pos.x == moveDirection.x > 0f)
            {
                flag = true;
            }
            num4++;
        }
        if (!flag)
        {
            num3 = Mathf.Lerp(Custom.LerpMap((float)hesistCounter, 20f, 150f, -0.5f, 1f), num3, CloseToEdge);
            hesistCounter++;
        }
        else
        {
            hesistCounter = 0;
        }
        num3 = Mathf.Lerp(num3, 1f, GetUnstuckForce);
        bodDir *= 0f;
        if (room.game.devToolsActive && Input.GetKey("b") && room.game.cameras[0].room == room)
        {
            bodyChunks[0].vel += Custom.DirVec(bodyChunks[0].pos, new Vector2(Input.mousePosition.x + room.game.cameras[0].pos.x, Input.mousePosition.y + room.game.cameras[0].pos.y)) * 14f;
            Stun(12);
        }
        if (Consious)
        {
            Act(eu, num, num3);
        }
        if (room == null)
        {
            return;
        }
        for (int m = 0; m < 3; m++)
        {
            WeightedPush(m, m + 2, -Custom.DirVec(bodyChunks[m].pos, bodyChunks[m + 2].pos), 0.45f);
            bodDir += Custom.DirVec(bodyChunks[m + 2].pos, bodyChunks[m + 1].pos);
        }
        if (Consious)
        {
            bodDir += new Vector2(0.8f * flipDir, 0f);
        }
        bodDir.Normalize();
        if (graphicsModule != null)
        {
            for (int n = 0; n < room.game.Players.Count; n++)
            {
                if (room.game.Players[n].pos.room == room.abstractRoom.index && room.game.Players[n].realizedCreature != null && (room.game.Players[n].realizedCreature as Player).wantToGrab > 0 && Custom.DistLess(room.game.Players[n].realizedCreature.mainBodyChunk.pos, antlers.pos, antlers.rad))
                {
                    (room.game.Players[n].realizedCreature as Player).wantToGrab = 0;
                    bool flag2 = true;
                    int num5 = 0;
                    while (num5 < playersInAntlers.Count && flag2)
                    {
                        flag2 = (playersInAntlers[num5].player != room.game.Players[n].realizedCreature as Player);
                        num5++;
                    }
                    if (flag2)
                    {
                        if ((room.game.Players[n].realizedCreature as Player).playerInAntlers != null)
                        {
                            (room.game.Players[n].realizedCreature as Player).playerInAntlers.playerDisconnected = true;
                        }
                        playersInAntlers.Add(new WalkerBeast.PlayerInAntlers(room.game.Players[n].realizedCreature as Player, this));
                    }
                }
            }
            for (int num6 = playersInAntlers.Count - 1; num6 >= 0; num6--)
            {
                if (playersInAntlers[num6].playerDisconnected)
                {
                    playersInAntlers.RemoveAt(num6);
                }
                else
                {
                    playersInAntlers[num6].Update(eu);
                }
            }
        }
        else
        {
            playersInAntlers.Clear();
        }
        if (grasps[0] != null)
        {
            Carry();
        }

        // Don't die from rain
        rainDeath = 0f;

        // Instead, get stunned for anywhere between 1 and 1.5 minutes if caught
        patch_RainCycle rc = abstractCreature.world.rainCycle as patch_RainCycle;
        if (rc != null) {
            int timeUntilBurst = rc.TimeUntilBurst(rc.CurrentBurst());
            if (timeUntilBurst > -600 && timeUntilBurst < -500 && (rainStun < 800))
            {
                rainStun = UnityEngine.Random.Range(20 * 60, 20 * 90);
            }
        }
        if(rainStun > 150)
        {
            // Full stun
            stun = Math.Max(stun, rainStun - 150);
        } else if(rainStun > 0)
        {
            // Last 7.5 seconds are partially stunned
            blind = Mathf.Max(blind, rainStun);
        }

        if (rainStun > 0) rainStun--;
    }

    // Token: 0x06001C02 RID: 7170 RVA: 0x0018EC4C File Offset: 0x0018CE4C
    private void Act(bool eu, float support, float forwardPower)
    {
        AI.Update();
        lastJawOpen = jawOpen;
        if (grasps[0] != null)
        {
            jawOpen = 0.15f;
        }
        else if (jawSlamPause > 0)
        {
            jawSlamPause--;
        }
        else
        {
            if (jawVel == 0f)
            {
                jawVel = 0.15f;
            }
            jawOpen += jawVel;
            if (jawKeepOpenPause > 0)
            {
                jawKeepOpenPause--;
                jawOpen = Mathf.Clamp(Mathf.Lerp(jawOpen, keepJawOpenPos, UnityEngine.Random.value * 0.5f), 0f, 1f);
            }
            else if (UnityEngine.Random.value < 1f / ((!Blinded) ? 40f : 15f))
            {
                jawKeepOpenPause = UnityEngine.Random.Range(10, UnityEngine.Random.Range(10, 60));
                keepJawOpenPos = ((UnityEngine.Random.value >= 0.5f) ? 1f : 0f);
                jawVel = Mathf.Lerp(-0.4f, 0.4f, UnityEngine.Random.value);
                jawOpen = Mathf.Clamp(jawOpen, 0f, 1f);
            }
            else if (jawOpen <= 0f)
            {
                jawOpen = 0f;
                if (jawVel < -0.4f)
                {
                    JawSlamShut();
                }
                jawVel = 0.15f;
                jawSlamPause = 5;
            }
            else if (jawOpen >= 1f)
            {
                jawOpen = 1f;
                jawVel = -0.5f;
            }
        }
        if (violenceReaction > 0)
        {
            AI.stuckTracker.stuckCounter = AI.stuckTracker.maxStuckCounter;
        }
        if (eatCounter > 0)
        {
            eatCounter--;
            if (eatCounter < 1 || eatObject.room != room || eatObject.grabbedBy.Count > 0 || !Custom.DistLess(base.mainBodyChunk.pos, eatObject.firstChunk.pos, 100f))
            {
                eatObject = null;
            }
            if (eatObject != null)
            {
                WeightedPush(0, 1, Custom.DirVec(base.mainBodyChunk.pos, eatObject.firstChunk.pos), Custom.LerpMap((float)eatCounter, 80f, 10f, 0.1f, 1.2f));
                WeightedPush(0, 2, Custom.DirVec(base.mainBodyChunk.pos, eatObject.firstChunk.pos), Custom.LerpMap((float)eatCounter, 80f, 10f, 0.1f, 1.2f));
                base.mainBodyChunk.vel += Custom.DirVec(base.mainBodyChunk.pos, eatObject.firstChunk.pos) * Custom.LerpMap((float)eatCounter, 80f, 10f, 0.1f, 2.75f);
                if (Custom.DistLess(base.mainBodyChunk.pos, eatObject.firstChunk.pos, base.mainBodyChunk.rad + eatObject.firstChunk.rad + 20f))
                {
                    eatObject.firstChunk.vel = Vector2.Lerp(eatObject.firstChunk.vel, Vector2.ClampMagnitude(base.mainBodyChunk.pos + new Vector2(0f, -14f) - eatObject.firstChunk.pos, 30f) / 10f, 0.8f);
                    base.mainBodyChunk.vel += Custom.RNV() * 2.6f;
                    if (eatCounter == 50)
                    {
                        room.PlaySound(SoundID.Puffball_Eaten_By_Deer, base.mainBodyChunk);
                        if (eatObject is PuffBall)
                        {
                            (eatObject as PuffBall).beingEaten = Mathf.Max((eatObject as PuffBall).beingEaten, 0.01f);
                        }
                        else
                        {
                            eatObject.Destroy();
                        }
                    }
                }
            }
            else
            {
                eatCounter = 0;
            }
        }
        if (AI.AllowMovementBetweenRooms)
        {
            enterRoomForcePush = Mathf.Max(0f, enterRoomForcePush - 0.001f);
        }
        else if (Mathf.Abs(abstractCreature.pos.x) < 10)
        {
            if (room.VisualContact(base.mainBodyChunk.pos, base.mainBodyChunk.pos + new Vector2(400f, 0f)))
            {
                BodyChunk bodyChunk = bodyChunks[1];
                bodyChunk.vel.x = bodyChunk.vel.x + Mathf.Pow(enterRoomForcePush, 2f) * 2f;
                enterRoomForcePush = Mathf.Min(4.5f, enterRoomForcePush + 0.00555555569f);
            }
            for (int i = 0; i < bodyChunks.Length; i++)
            {
                if (bodyChunks[i].ContactPoint.x > 0)
                {
                    bodyChunks[i].vel += Custom.DegToVec(-90f * UnityEngine.Random.value) * 15f;
                }
            }
        }
        else if (Mathf.Abs(abstractCreature.pos.x - (room.TileWidth - 1)) < 10)
        {
            if (room.VisualContact(base.mainBodyChunk.pos, base.mainBodyChunk.pos + new Vector2(-400f, 0f)))
            {
                BodyChunk bodyChunk2 = bodyChunks[1];
                bodyChunk2.vel.x = bodyChunk2.vel.x - Mathf.Pow(enterRoomForcePush, 2f) * 2f;
                enterRoomForcePush = Mathf.Min(4.5f, enterRoomForcePush + 0.00555555569f);
            }
            for (int j = 0; j < bodyChunks.Length; j++)
            {
                if (bodyChunks[j].ContactPoint.x < 0)
                {
                    bodyChunks[j].vel += Custom.DegToVec(90f * UnityEngine.Random.value) * 15f;
                }
            }
        }
        bool flag = false;
        preferredHeight = 5f + 5f;
        IntVector2 tilePosition = room.GetTilePosition(bodyChunks[1].pos);
            MovementConnection movementConnection = (AI.pathFinder as WalkerBeastPather).FollowPath(room.GetWorldCoordinate(base.mainBodyChunk.pos), true);
            if (movementConnection == null)
            {
                int num2 = 1;
                while (num2 < 3 && movementConnection == null && room != null)
                {
                    int num3 = 0;
                    while (num3 < 5 && movementConnection == null && room != null)
                    {
                        int num4 = 0;
                        while (num4 < 5 && movementConnection == null && room != null)
                        {
                            movementConnection = (AI.pathFinder as WalkerBeastPather).FollowPath(room.GetWorldCoordinate(bodyChunks[num3].pos + Custom.fourDirectionsAndZero[num4].ToVector2() * 20f * (float)num2), true);
                            num4++;
                        }
                        num3++;
                    }
                    num2++;
                }
            }
            if (room == null)
            {
                return;
            }
            if (movementConnection != null)
            {
                if (AI.AllowMovementBetweenRooms && AI.pathFinder.GetDestination.room != room.abstractRoom.index && movementConnection.startCoord.x <= 0)
                {
                    moveDirection = new Vector2(-1f, 0f);
                smoothMoveDirection = new Vector2(-1f, 0f);
            }
                else if (AI.AllowMovementBetweenRooms && AI.pathFinder.GetDestination.room != room.abstractRoom.index && movementConnection.startCoord.x >= room.TileWidth - 1)
                {
                    moveDirection = new Vector2(1f, 0f);
                    smoothMoveDirection = new Vector2(1f, 0f);
            }
                else
                {
                    WorldCoordinate destinationCoord = movementConnection.destinationCoord;
                    if (room.IsPositionInsideBoundries(abstractCreature.pos.Tile))
                    {
                        for (int l = 0; l < 15; l++)
                        {
                            if (AI.AllowMovementBetweenRooms && AI.pathFinder.GetDestination.room != room.abstractRoom.index && destinationCoord.x <= 0)
                            {
                                destinationCoord.x--;
                                hesistCounter = 0;
                            }
                            else if (AI.AllowMovementBetweenRooms && AI.pathFinder.GetDestination.room != room.abstractRoom.index && destinationCoord.x >= room.TileWidth - 1)
                            {
                                destinationCoord.x++;
                                hesistCounter = 0;
                            }
                            else
                            {
                                MovementConnection movementConnection2 = (AI.pathFinder as WalkerBeastPather).FollowPath(destinationCoord, false);
                                if (movementConnection2 == null || movementConnection2.type != MovementConnection.MovementType.Standard || !(movementConnection2.DestTile != movementConnection.startCoord.Tile) || !AI.pathFinder.RayTraceInAccessibleTiles(movementConnection.startCoord.Tile, movementConnection2.DestTile))
                                {
                                    break;
                                }
                                destinationCoord = movementConnection2.destinationCoord;
                            }
                        }
                    }
                    if (destinationCoord.TileDefined && Mathf.Abs(room.MiddleOfTile(destinationCoord).x - bodyChunks[0].pos.x) > 100f)
                    {
                        flipDir = Mathf.Sign(room.MiddleOfTile(destinationCoord).x - bodyChunks[0].pos.x);
                    }
                    if (abstractCreature.pos.Tile.x < 0 || abstractCreature.pos.Tile.x >= room.TileWidth || destinationCoord.x < 0 || destinationCoord.x >= room.TileWidth)
                    {
                        nextFloorHeight = 300f;
                    }
                    else
                    {
                        nextFloorHeight = Mathf.Lerp(nextFloorHeight, (float)Custom.IntClamp(room.aimap.getAItile(destinationCoord).smoothedFloorAltitude + 2, 0, 17) * 20f, 0.2f);
                    }
                    moveDirection = Custom.DirVec(base.mainBodyChunk.pos, room.MiddleOfTile(destinationCoord));
                }
            }
        smoothMoveDirection = Vector2.Lerp(smoothMoveDirection, moveDirection, 0.02f);
        for (int m = 0; m < 5; m++)
        {
            float num5 = (float)m / 5f;
            bodyChunks[m].vel *= Mathf.Lerp(1f, 0.92f, support);
            bodyChunks[m].vel *= 0.9f;
            if (m < 4)
            {
                BodyChunk bodyChunk3 = bodyChunks[m];
                bodyChunk3.vel.y = bodyChunk3.vel.y + gravity * Mathf.Lerp(1.3f, 2.5f, Mathf.Sin(Mathf.Pow(num5, 1.7f) * 3.14159274f)) * Mathf.Lerp(support * Custom.LerpMap((float)room.aimap.getClampedAItile(bodyChunks[m].pos).smoothedFloorAltitude, 14f, 18f, 1f, 0.5f) * Custom.LerpMap(moveDirection.y, -1f, 1f, 0.5f, 1f), 0.65f, CloseToEdge);
            }
            //added a "10*" here to increase chasing speed
            bodyChunks[m].vel += 8 * smoothMoveDirection * Mathf.Lerp(0.35f, 0f, num5) * forwardPower * Mathf.Lerp(1f, 3f, GetUnstuckForce);
        }
        BodyChunk mainBodyChunk = base.mainBodyChunk;
        if (GetUnstuckForce > 0f)
        {
            bodyChunks[1].vel += Custom.RNV() * UnityEngine.Random.value * 1f * GetUnstuckForce;
        }
    }
    
    public override void Violence(BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        if (damage > 0.9f)
        {
            //violenceReaction = Math.Max(violenceReaction, UnityEngine.Random.Range(10, (int)Custom.LerpMap(damage, 0.9f, 3f, 20f, 60f)));
            //(base.abstractCreature.abstractAI as WalkerBeastAbstractAI).damageGoHome = true;
            AI.timeInRoom += 100;
        }
        base.Violence(source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
    }

    public void AccessSideSpace(WorldCoordinate start, WorldCoordinate dest)
    {
        room.game.shortcuts.CreatureTakeFlight(this, AbstractRoomNode.Type.SideExit, start, dest);
    }
    
    public void EatObject(PhysicalObject obj)
    {
        if (eatObject != null)
        {
            return;
        }
        eatObject = obj;
        eatCounter = 160;
    }
    
    public override void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        base.Collide(otherObject, myChunk, otherChunk);
        if (AI != null)
        {
            AI.Collide(otherObject, myChunk, otherChunk);
        }
    }
    
    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);
        if (speed > 1.5f && firstContact)
        {
            room.PlaySound((speed >= 8f) ? SoundID.Leviathan_Heavy_Terrain_Impact : SoundID.Leviathan_Light_Terrain_Impact, mainBodyChunk);
        }
    }
    
    public override void Die()
    {
        base.Die();
    }
    
    public override Color ShortCutColor()
    {
        return new Color(0f, 0f, 0f);
    }

    private void UpdateNeck()
    {
        neck.Update();
        for (int i = 0; i < neck.tChunks.Length; i++)
        {
            float t = (float)i / (float)(neck.tChunks.Length - 1);
            neck.tChunks[i].vel *= 0.95f;
            Tentacle.TentacleChunk tentacleChunk = neck.tChunks[i];
            tentacleChunk.vel.y = tentacleChunk.vel.y - ((!neck.limp) ? 0.1f : 0.7f);
            if ((float)neck.backtrackFrom == -1f || neck.backtrackFrom > i)
            {
                //Vector2 adjustedVector = new Vector2(this.moveDirection.x/this.moveDirection.magnitude, this.moveDirection.y/this.moveDirection.magnitude);
                //this.neck.tChunks[i].vel += Custom.DirVec(base.bodyChunks[0].pos, base.bodyChunks[5].pos) * Mathf.Lerp(3f, 1f, t);

                neck.tChunks[i].vel += smoothMoveDirection + Custom.DirVec(bodyChunks[0].pos, bodyChunks[5].pos) * Mathf.Lerp(3f, 1f, t);
            }
            neck.tChunks[i].vel -= neck.connectedChunk.vel;
            neck.tChunks[i].vel *= 0.75f;
            neck.tChunks[i].vel += neck.connectedChunk.vel;
        }
        neck.limp = !Consious;
        float num = (neck.backtrackFrom != -1) ? 0f : 0.5f;
        //if (base.grasps[0] == null)
        //{
            Vector2 a = Custom.DirVec(bodyChunks[6].pos, neck.tChunks[neck.tChunks.Length - 1].pos);
            float num2 = Vector2.Distance(bodyChunks[6].pos, neck.tChunks[neck.tChunks.Length - 1].pos);
        bodyChunks[6].pos -= (6f - num2) * a * (1f - num);
        bodyChunks[6].vel -= (6f - num2) * a * (1f - num);
            neck.tChunks[neck.tChunks.Length - 1].pos += (6f - num2) * a * num;
            neck.tChunks[neck.tChunks.Length - 1].vel += (6f - num2) * a * num;
        bodyChunks[6].vel += Custom.DirVec(neck.tChunks[neck.tChunks.Length - 2].pos, bodyChunks[6].pos) * 6f * (1f - num);
        bodyChunks[6].vel += Custom.DirVec(neck.tChunks[neck.tChunks.Length - 1].pos, bodyChunks[6].pos) * 6f * (1f - num);
            neck.tChunks[neck.tChunks.Length - 1].vel -= Custom.DirVec(neck.tChunks[neck.tChunks.Length - 2].pos, bodyChunks[6].pos) * 6f * num;
            neck.tChunks[neck.tChunks.Length - 2].vel -= Custom.DirVec(neck.tChunks[neck.tChunks.Length - 2].pos, bodyChunks[6].pos) * 6f * num;
        //}
        if (!Consious)
        {
            neck.retractFac = 0.5f;
            neck.floatGrabDest = null;
            return;
        }
        BodyChunk head = Head;
        head.vel.y = head.vel.y + gravity;
        Vector2 vector;
        if (AI.creatureLooker.lookCreature != null && AI.creatureLooker.lookCreature.VisualContact)
        {
            vector = AI.creatureLooker.lookCreature.representedCreature.realizedCreature.DangerPos;
        }
        else
        {
            vector = mainBodyChunk.pos + smoothMoveDirection * 400f;
        }
        float num3 = 0.5f;
        neck.retractFac = Mathf.Lerp(0.5f, 0.8f, num3);
        vector = Vector2.Lerp(vector, mainBodyChunk.pos + smoothMoveDirection * 200f, Mathf.Pow(num3, 6f));
        if (Blinded)
        {
            vector = mainBodyChunk.pos + Custom.RNV() * UnityEngine.Random.value * 400f;
        }
        if ((Custom.DistLess(vector, mainBodyChunk.pos, 220f) && !room.VisualContact(vector, Head.pos)) || num3 > 0.5f)
        {
            neck.MoveGrabDest(vector);
        }
        else if (neck.backtrackFrom == -1)
        {
            neck.floatGrabDest = null;
        }
        Vector2 a2 = Custom.DirVec(Head.pos, vector);
        if (grasps[0] == null)
        {
            neck.tChunks[neck.tChunks.Length - 1].vel += a2 * num * 1.2f;
            neck.tChunks[neck.tChunks.Length - 2].vel -= a2 * 0.5f * num;
            Head.vel += a2 * 6f * (1f - num);
        }
        else
        {
            neck.tChunks[neck.tChunks.Length - 1].vel += a2 * 2f * num;
            neck.tChunks[neck.tChunks.Length - 2].vel -= a2 * 2f * num;
            grasps[0].grabbedChunk.vel += a2 / grasps[0].grabbedChunk.mass;
        }
        //if (Custom.DistLess(this.Head.pos, vector, 80f * Mathf.InverseLerp(1f, 0.5f, this.jawOpen)))
        //{
        //    for (int j = 0; j < this.neck.tChunks.Length; j++)
        //    {
        //        this.neck.tChunks[j].vel -= a2 * Mathf.InverseLerp(80f, 20f, Vector2.Distance(this.Head.pos, vector)) * 8f * num;
        //    }
        //}
    }

    public void JawSlamShut()
    {
        Vector2 a = Custom.DirVec(neck.Tip.pos, Head.pos);
        //neck.Tip.vel -= a * 10f;
        //neck.Tip.pos += a * 20f;
        //Head.pos += a * 20f;
        int num = 0;
        int num2 = 0;
        while (num2 < room.abstractRoom.creatures.Count && grasps[0] == null)
        {
            if (room.abstractRoom.creatures[num2] != abstractCreature && AI.DoIWantToBiteCreature(room.abstractRoom.creatures[num2]) && room.abstractRoom.creatures[num2].realizedCreature != null && room.abstractRoom.creatures[num2].realizedCreature.enteringShortCut == null)
            {
                int num3 = 0;
                while (num3 < room.abstractRoom.creatures[num2].realizedCreature.bodyChunks.Length && grasps[0] == null)
                {
                    if (Custom.DistLess(Head.pos + a * 20f, room.abstractRoom.creatures[num2].realizedCreature.bodyChunks[num3].pos, 20f + room.abstractRoom.creatures[num2].realizedCreature.bodyChunks[num3].rad) && room.VisualContact(Head.pos, room.abstractRoom.creatures[num2].realizedCreature.bodyChunks[num3].pos))
                    {
                        Grab(room.abstractRoom.creatures[num2].realizedCreature, 0, num3, Grasp.Shareability.CanOnlyShareWithNonExclusive, 1f, true, true);
                        jawOpen = 0.15f;
                        jawVel = 0f;
                        num = ((!(room.abstractRoom.creatures[num2].realizedCreature is Player)) ? 1 : 2);
                        room.abstractRoom.creatures[num2].realizedCreature.Violence(Head, new Vector2?(Custom.DirVec(Head.pos, room.abstractRoom.creatures[num2].realizedCreature.bodyChunks[num3].pos) * 4f), room.abstractRoom.creatures[num2].realizedCreature.bodyChunks[num3], null, DamageType.Bite, 1.2f, 0f);
                        break;
                    }
                    num3++;
                }
                if (room.abstractRoom.creatures[num2].realizedCreature is DaddyLongLegs)
                {
                    for (int i = 0; i < (room.abstractRoom.creatures[num2].realizedCreature as DaddyLongLegs).tentacles.Length; i++)
                    {
                        for (int j = 0; j < (room.abstractRoom.creatures[num2].realizedCreature as DaddyLongLegs).tentacles[i].tChunks.Length; j++)
                        {
                            if (Custom.DistLess(Head.pos + a * 20f, (room.abstractRoom.creatures[num2].realizedCreature as DaddyLongLegs).tentacles[i].tChunks[j].pos, 20f))
                            {
                                (room.abstractRoom.creatures[num2].realizedCreature as DaddyLongLegs).tentacles[i].stun = UnityEngine.Random.Range(10, 70);
                                for (int k = j; k < (room.abstractRoom.creatures[num2].realizedCreature as DaddyLongLegs).tentacles[i].tChunks.Length; k++)
                                {
                                    (room.abstractRoom.creatures[num2].realizedCreature as DaddyLongLegs).tentacles[i].tChunks[k].vel += Custom.DirVec((room.abstractRoom.creatures[num2].realizedCreature as DaddyLongLegs).tentacles[i].tChunks[k].pos, (room.abstractRoom.creatures[num2].realizedCreature as DaddyLongLegs).tentacles[i].connectedChunk.pos) * Mathf.Lerp(10f, 50f, UnityEngine.Random.value);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            num2++;
        }
        if (num == 1)
        {
            room.PlaySound(SoundID.Miros_Beak_Snap_Hit_Slugcat, Head);
        }
    }

    public BodyChunk Head
    {
        get
        {
            return bodyChunks[6];
        }
    }

    public void Carry()
    {
        //BodyChunk grabbedChunk = base.grasps[0].grabbedChunk;
        //grabbedChunk.pos = this.Head.pos;
        if (!Consious)
        {
            LoseAllGrasps();
            return;
        }
        BodyChunk grabbedChunk = grasps[0].grabbedChunk;
        if (UnityEngine.Random.value < 0.008333334f && (!(grabbedChunk.owner is Creature) || Template.CreatureRelationship((grabbedChunk.owner as Creature).Template).type != CreatureTemplate.Relationship.Type.Eats))
        {
            LoseAllGrasps();
            return;
        }
        float num = grabbedChunk.mass / (grabbedChunk.mass + bodyChunks[6].mass);
        float num2 = grabbedChunk.mass / (grabbedChunk.mass + bodyChunks[0].mass);
        if (neck.backtrackFrom != -1 || enteringShortCut != null)
        {
            num = 0f;
            num2 = 0f;
        }
        if (!Custom.DistLess(grabbedChunk.pos, neck.tChunks[neck.tChunks.Length - 1].pos, 20f))
        {
            Vector2 a = Custom.DirVec(grabbedChunk.pos, neck.tChunks[neck.tChunks.Length - 1].pos);
            float num3 = Vector2.Distance(grabbedChunk.pos, neck.tChunks[neck.tChunks.Length - 1].pos);
            grabbedChunk.pos -= (20f - num3) * a * (1f - num);
            grabbedChunk.vel -= (20f - num3) * a * (1f - num);
            neck.tChunks[neck.tChunks.Length - 1].pos += (20f - num3) * a * num;
            neck.tChunks[neck.tChunks.Length - 1].vel += (20f - num3) * a * num;
        }
        if (enteringShortCut == null)
        {
            bodyChunks[6].pos = Vector2.Lerp(neck.tChunks[neck.tChunks.Length - 1].pos, grabbedChunk.pos, 0.1f);
            bodyChunks[6].vel = neck.tChunks[neck.tChunks.Length - 1].vel;
        }
        float num4 = 70f;
        if (!Custom.DistLess(mainBodyChunk.pos, grabbedChunk.pos, num4))
        {
            Vector2 a2 = Custom.DirVec(grabbedChunk.pos, bodyChunks[0].pos);
            float num5 = Vector2.Distance(grabbedChunk.pos, bodyChunks[0].pos);
            grabbedChunk.pos -= (num4 - num5) * a2 * (1f - num2);
            grabbedChunk.vel -= (num4 - num5) * a2 * (1f - num2);
            bodyChunks[0].pos += (num4 - num5) * a2 * num2;
            bodyChunks[0].vel += (num4 - num5) * a2 * num2;
        }
    }

    public WalkerBeastAI AI;
    
    // How many updates this creature should remain stunned for after the rain
    // The last few ticks are slightly stunned - it isn't allowed to attack
    public int rainStun;

    public Vector2 bodDir;//This vector represents the orientation of the body could be used for head things

    public float flipDir;//Variable for the process of changing the bodDir
    
    public Vector2 moveDirection;//current vector of movement

    public Vector2 smoothMoveDirection; //smoothed version of the current move direction.

    //Leg system -----------------------\\
    public bool heldBackByLeg;
    //*                                *\\
    public WalkerBeastTentacle[] legs;
    //*                                *\\
    public int legsGrabbing;
    //*                                *\\
    public float nextFloorHeight;
    //*                                *\\
    public float preferredHeight;
    //*                                *\\
    public int hesistCounter;
    //----------------------------------\\

    public PhysicalObject eatObject;//the object being eaten
    
    public int eatCounter;//counter the counts down to full consuption of eatObject

    public bool eatan = false;//variable to determine if the craeture sould continue looking for food

    //Stuff from Miros Bird------------------------

    public Tentacle neck;//neck imported from Miros bird
    
    public float jawOpen;
    
    public float lastJawOpen;

    public float jawVel;
    
    private float keepJawOpenPos;
    
    private int jawSlamPause;
    
    private int jawKeepOpenPause;

    //----------------------------------------------

    private int violenceReaction;// a counter that represents the last time hit with spear or other injury
    
    public WalkerBeast.IndividualVariations iVars;//some random variations unique to deer 
    
    public List<WalkerBeast.PlayerInAntlers> playersInAntlers;//DELETE THIS!
    
    private float enterRoomForcePush;
    
    public struct IndividualVariations
    {
        public IndividualVariations(float patternDisplacement, int finsSeed, HSLColor patternColorA, HSLColor patternColorB, bool largeHorns)
        {
            this.patternDisplacement = patternDisplacement;
            this.finsSeed = finsSeed;
            this.patternColorA = patternColorA;
            this.patternColorB = patternColorB;
            this.largeHorns = largeHorns;
        }
        
        public float patternDisplacement;
        
        public int finsSeed;

        public bool largeHorns;
        
        public HSLColor patternColorA;
        
        public HSLColor patternColorB;
    }
    //useless class here to prevent errors 
    public class PlayerInAntlers
    {
        // Token: 0x06001C0B RID: 7179 RVA: 0x0018FD2C File Offset: 0x0018DF2C
        public PlayerInAntlers(Player player, WalkerBeast walkerBeast)
        {
            this.player = player;
            this.walkerBeast = walkerBeast;
            antlers = (walkerBeast.graphicsModule as WalkerBeastGraphics).antlers;
            antlerChunk = walkerBeast.bodyChunks[5];
            handGrabPoints = new Vector2[2];
            for (int i = 0; i < 2; i++)
            {
                handGrabPoints[i] = player.mainBodyChunk.pos;
            }
            float num = Mathf.Sign(FaceDir);
            int part = -1;
            int segment = -1;
            float side = num;
            float num2 = float.MaxValue;
            for (int j = 0; j < antlers.parts.Length; j++)
            {
                for (int k = 0; k < antlers.parts[j].positions.Length; k++)
                {
                    for (float num3 = -1f; num3 <= 1f; num3 += 2f)
                    {
                        Vector2 a = PlaySpaceCoordinate(j, k, num3);
                        float num4 = Vector2.Distance(a, player.mainBodyChunk.pos) + ((num3 == num) ? 0f : 100f) * Mathf.Abs(FaceDir);
                        if (num4 < num2)
                        {
                            num2 = num4;
                            part = j;
                            segment = k;
                            side = num3;
                        }
                    }
                }
            }
            dangle = true;
            stance = new WalkerBeast.PlayerInAntlers.GrabStance(this, new WalkerBeast.PlayerInAntlers.AntlerPoint(part, segment, side), null);
            walkerBeast.graphicsModule.AddObjectToInternalContainer(player.graphicsModule, 1);
            vultures = new List<VultureAbstractAI>();
            for (int l = 0; l < player.room.world.NumberOfRooms; l++)
            {
                for (int m = 0; m < player.room.world.GetAbstractRoom(player.room.world.firstRoomIndex + l).creatures.Count; m++)
                {
                    if (player.room.world.GetAbstractRoom(player.room.world.firstRoomIndex + l).creatures[m].creatureTemplate.TopAncestor().type == CreatureTemplate.Type.Vulture && player.room.world.GetAbstractRoom(player.room.world.firstRoomIndex + l).creatures[m].abstractAI != null && player.room.world.GetAbstractRoom(player.room.world.firstRoomIndex + l).creatures[m].abstractAI is VultureAbstractAI)
                    {
                        vultures.Add(player.room.world.GetAbstractRoom(player.room.world.firstRoomIndex + l).creatures[m].abstractAI as VultureAbstractAI);
                    }
                }
            }
        }

        // Token: 0x1700045F RID: 1119
        // (get) Token: 0x06001C0C RID: 7180 RVA: 0x00013059 File Offset: 0x00011259
        public float FaceDir
        {
            get
            {
                return (walkerBeast.graphicsModule as WalkerBeastGraphics).CurrentFaceDir(1f);
            }
        }

        // Token: 0x06001C0D RID: 7181 RVA: 0x00190034 File Offset: 0x0018E234
        public void Update(bool eu)
        {
            playerDisconnected = true;
            player.playerInAntlers = null;
            player.animation = Player.AnimationIndex.None;
            walkerBeast.WeightedPush(5, 0, Custom.DirVec(walkerBeast.mainBodyChunk.pos, player.mainBodyChunk.pos), 0.7f);
            for (int i = 0; i < vultures.Count; i++)
            {
                if (vultures[i].parent.pos.room != player.abstractCreature.pos.room)
                {
                    for (int j = vultures[i].checkRooms.Count - 1; j >= 0; j--)
                    {
                        if (vultures[i].checkRooms[j].room == player.abstractCreature.pos.room)
                        {
                            //Debug.Log("vulture repelled by player in antlers");
                            vultures[i].checkRooms.RemoveAt(j);
                        }
                    }
                    if (vultures[i].destination.room == player.abstractCreature.pos.room || vultures[i].MigrationDestination.room == player.abstractCreature.pos.room)
                    {
                        vultures[i].SetDestination(vultures[i].parent.pos);
                        //Debug.Log("vulture destination repelled by player in antlers");
                    }
                }
            }
            if (Custom.DistLess(player.mainBodyChunk.pos, antlerChunk.pos, antlerChunk.rad + 100f))
            {
                (player as patch_Player).playerInAnt = this;
                if (Mathf.Abs(FaceDir) > 0.5f && stance.upper.side != -Mathf.Sign(FaceDir))
                {
                    forceSideChange = true;
                    FindCorrectSideStance(-Mathf.Sign(FaceDir));
                }
                else
                {
                    forceSideChange = false;
                }
                if (forceSideChange)
                {
                    movProg = Mathf.Min(1f, movProg + 0.1f);
                    if (movProg == 1f)
                    {
                        stance = nextStance;
                        nextStance = null;
                        movProg = 0f;
                        forceSideChange = false;
                    }
                }
                else
                {
                    Vector2 normalized = new Vector2(0f, 0f);
                    if (player.input[0].analogueDir.magnitude > 0f)
                    {
                        normalized = player.input[0].analogueDir.normalized;
                    }
                    else if (player.input[0].x != 0 || player.input[0].y != 0)
                    {
                        Vector2 vector = new Vector2((float)player.input[0].x, (float)player.input[0].y);
                        normalized = vector.normalized;
                    }
                    if (normalized.magnitude > 0f)
                    {
                        if (nextStance != null)
                        {
                            movProg = Mathf.Min(1f, movProg + 0.1f);
                            if (movProg == 1f)
                            {
                                stance = nextStance;
                                nextStance = null;
                                movProg = 0f;
                            }
                        }
                        climbGoal += normalized * Custom.LerpMap(Vector2.Dot(normalized, climbGoal.normalized), -1f, 1f, 1.6f, 0.8f);
                        if (climbGoal.magnitude > 30f)
                        {
                            climbGoal = Vector2.ClampMagnitude(climbGoal, 30f);
                        }
                    }
                    else
                    {
                        if (nextStance != null)
                        {
                            movProg = Mathf.Max(0f, movProg - 0.1f);
                        }
                        if (movProg == 0f)
                        {
                            nextStance = null;
                        }
                        if (climbGoal.magnitude < 1f)
                        {
                            climbGoal *= 0f;
                        }
                        else
                        {
                            climbGoal -= climbGoal.normalized;
                        }
                    }
                    if (movProg == 0f)
                    {
                        if (player.input[0].y < 0 && player.input[1].y >= 0 && player.input[0].x == 0)
                        {
                            dangle = true;
                            stance.lower = null;
                        }
                        else if (normalized.magnitude > 0f)
                        {
                            FindNextStance();
                        }
                    }
                    stance.UpdateLower();
                }
                Vector2 vector2;
                if (nextStance != null)
                {
                    vector2 = Vector2.Lerp(stance.upper.PlaySpaceCoordinate(this), nextStance.upper.PlaySpaceCoordinate(this), movProg);
                }
                else
                {
                    vector2 = stance.upper.PlaySpaceCoordinate(this);
                }
                vector2 = PushOutOfHead(vector2, 10f);
                FindHandPositions();
                if (!dangle)
                {
                    player.mainBodyChunk.MoveFromOutsideMyUpdate(eu, vector2);
                    player.mainBodyChunk.vel = antlerChunk.vel;
                }
                else
                {
                    Vector2 a = Custom.DirVec(player.mainBodyChunk.pos, vector2);
                    float num = Vector2.Distance(player.mainBodyChunk.pos, vector2);
                    if (num > 10f)
                    {
                        player.mainBodyChunk.pos += a * (num - 10f);
                        player.mainBodyChunk.vel += a * (num - 10f);
                    }
                }
                if (stance.lower != null)
                {
                    float num2 = 1f - movProg;
                    player.bodyChunks[1].vel = Vector2.Lerp(player.bodyChunks[1].vel, antlerChunk.vel, num2 * 0.9f);
                    player.bodyChunks[1].vel = -Vector2.ClampMagnitude(player.bodyChunks[1].pos - PushOutOfHead(stance.lower.PlaySpaceCoordinate(this), 10f), 10f * num2) / 3f;
                }
                if (!player.Consious)
                {
                    playerDisconnected = true;
                    player.playerInAntlers = null;
                    player.animation = Player.AnimationIndex.None;
                }
                playerDisconnected = true;
                player.playerInAntlers = null;
                player.animation = Player.AnimationIndex.None;
                return;
            }
            playerDisconnected = true;
            player.playerInAntlers = null;
            player.animation = Player.AnimationIndex.None;
        }

        // Token: 0x06001C0E RID: 7182 RVA: 0x001907EC File Offset: 0x0018E9EC
        private Vector2 PushOutOfHead(Vector2 ps, float margin)
        {
            if (Custom.DistLess(ps, walkerBeast.mainBodyChunk.pos, walkerBeast.mainBodyChunk.rad + margin))
            {
                return walkerBeast.mainBodyChunk.pos + Custom.DirVec(walkerBeast.mainBodyChunk.pos, ps) * (walkerBeast.mainBodyChunk.rad + margin);
            }
            return ps;
        }

        // Token: 0x06001C0F RID: 7183 RVA: 0x0019086C File Offset: 0x0018EA6C
        private void FindNextStance()
        {
            Vector2 vector = stance.upper.PlaySpaceCoordinate(this);
            Vector2 vector2 = vector + climbGoal;
            IntVector2 intVector = new IntVector2(-1, -1);
            float dst = Vector2.Distance(vector, vector2) + 1f;
            for (int i = 0; i < antlers.parts.Length; i++)
            {
                for (int j = 0; j < antlers.parts[i].positions.Length; j++)
                {
                    Vector2 vector3 = PlaySpaceCoordinate(i, j, stance.upper.side);
                    if (Custom.DistLess(vector, vector3, 30f) && Custom.DistLess(vector2, vector3, dst))
                    {
                        dst = Vector2.Distance(vector2, vector3);
                        intVector = new IntVector2(i, j);
                    }
                }
            }
            if (intVector.x != -1 && intVector.y != -1 && (intVector.x != stance.upper.part || intVector.y != stance.upper.segment))
            {
                climbGoal *= 0f;
                nextStance = new WalkerBeast.PlayerInAntlers.GrabStance(this, new WalkerBeast.PlayerInAntlers.AntlerPoint(intVector.x, intVector.y, stance.upper.side), (stance.lower == null) ? null : new WalkerBeast.PlayerInAntlers.AntlerPoint(stance.lower.part, stance.lower.segment, stance.lower.side));
                dangle = false;
            }
            else if (player.input[0].y > 0 && player.input[1].y <= 0 && player.input[0].x == 0)
            {
                dangle = false;
            }
        }

        // Token: 0x06001C10 RID: 7184 RVA: 0x00190A94 File Offset: 0x0018EC94
        private void FindCorrectSideStance(float side)
        {
            float dst = float.MaxValue;
            for (int i = 0; i < antlers.parts.Length; i++)
            {
                for (int j = 0; j < antlers.parts[i].positions.Length; j++)
                {
                    Vector2 vector = PlaySpaceCoordinate(i, j, side);
                    if (Custom.DistLess(player.mainBodyChunk.pos, vector, dst))
                    {
                        dst = Vector2.Distance(player.mainBodyChunk.pos, vector);
                    }
                }
            }
            nextStance = new WalkerBeast.PlayerInAntlers.GrabStance(this, new WalkerBeast.PlayerInAntlers.AntlerPoint(stance.upper.part, stance.upper.segment, side), null);
        }

        // Token: 0x06001C11 RID: 7185 RVA: 0x00190B6C File Offset: 0x0018ED6C
        private void FindHandPositions()
        {
            Vector2 vector = stance.upper.PlaySpaceCoordinate(this);
            handGrabPoints[1 - oddHand] = vector;
            if (dangle)
            {
                handGrabPoints[oddHand] = vector;
            }
            else if (nextStance != null)
            {
                handGrabPoints[oddHand] = nextStance.upper.PlaySpaceCoordinate(this);
            }
            else
            {
                if (oddHandGrip != null && (oddHandGrip.side != stance.upper.side || !Custom.DistLess(vector, oddHandGrip.PlaySpaceCoordinate(this), 25f)))
                {
                    oddHandGrip = null;
                }
                if (oddHandGrip == null)
                {
                    float num = float.MaxValue;
                    IntVector2 intVector = new IntVector2(-1, -1);
                    for (int i = 0; i < antlers.parts.Length; i++)
                    {
                        for (int j = 0; j < antlers.parts[i].positions.Length; j++)
                        {
                            Vector2 vector2 = PlaySpaceCoordinate(i, j, stance.upper.side);
                            float num2 = Mathf.Min(Vector2.Distance(vector2, vector + Custom.PerpendicularVector((player.mainBodyChunk.pos - player.bodyChunks[1].pos).normalized) * 35f), Vector2.Distance(vector2, vector + Custom.PerpendicularVector((player.mainBodyChunk.pos - player.bodyChunks[1].pos).normalized) * -35f));
                            if (Custom.DistLess(vector, vector2, 25f) && num2 < num)
                            {
                                num = num2;
                                intVector = new IntVector2(i, j);
                                oddHand = ((Custom.DistanceToLine(vector2, vector, player.bodyChunks[1].pos) >= 0f) ? 1 : 0);
                            }
                        }
                    }
                    if (intVector.x != -1 && intVector.y != -1)
                    {
                        oddHandGrip = new WalkerBeast.PlayerInAntlers.AntlerPoint(intVector.x, intVector.y, stance.upper.side);
                    }
                }
                if (oddHandGrip != null)
                {
                    handGrabPoints[oddHand] = oddHandGrip.PlaySpaceCoordinate(this);
                }
                else
                {
                    handGrabPoints[oddHand] = vector;
                }
            }
        }



        // Token: 0x06001C12 RID: 7186 RVA: 0x00190E50 File Offset: 0x0018F050
        public Vector2 PlaySpaceCoordinate(int part, int segment, float side)
        {
            return antlers.TransformToHeadRotat(antlers.parts[part].GetTransoformedPos(segment, side), antlerChunk.pos, Custom.AimFromOneVectorToAnother(walkerBeast.mainBodyChunk.pos, antlerChunk.pos), side, FaceDir);
        }

        // Token: 0x04001E4F RID: 7759
        public Player player;

        // Token: 0x04001E50 RID: 7760
        public WalkerBeast walkerBeast;

        // Token: 0x04001E51 RID: 7761
        public WalkerBeastGraphics.Antlers antlers;

        // Token: 0x04001E52 RID: 7762
        public BodyChunk antlerChunk;

        // Token: 0x04001E53 RID: 7763
        public WalkerBeast.PlayerInAntlers.GrabStance stance;

        // Token: 0x04001E54 RID: 7764
        public WalkerBeast.PlayerInAntlers.GrabStance nextStance;

        // Token: 0x04001E55 RID: 7765
        public float movProg;

        // Token: 0x04001E56 RID: 7766
        public Vector2 climbGoal;

        // Token: 0x04001E57 RID: 7767
        public bool dangle;

        // Token: 0x04001E58 RID: 7768
        public bool playerDisconnected;

        // Token: 0x04001E59 RID: 7769
        public DebugSprite dbSpr;

        // Token: 0x04001E5A RID: 7770
        public DebugSprite dbSpr2;

        // Token: 0x04001E5B RID: 7771
        public DebugSprite dbSpr3;

        // Token: 0x04001E5C RID: 7772
        private bool forceSideChange;

        // Token: 0x04001E5D RID: 7773
        public Vector2[] handGrabPoints;

        // Token: 0x04001E5E RID: 7774
        public WalkerBeast.PlayerInAntlers.AntlerPoint oddHandGrip;

        // Token: 0x04001E5F RID: 7775
        public int oddHand;

        // Token: 0x04001E60 RID: 7776
        public List<VultureAbstractAI> vultures;

        // Token: 0x0200045B RID: 1115
        public class GrabStance
        {
            // Token: 0x06001C13 RID: 7187 RVA: 0x00013075 File Offset: 0x00011275
            public GrabStance(WalkerBeast.PlayerInAntlers PIA, WalkerBeast.PlayerInAntlers.AntlerPoint upper, WalkerBeast.PlayerInAntlers.AntlerPoint lower)
            {
                this.PIA = PIA;
                this.upper = upper;
                this.lower = lower;
            }

            // Token: 0x17000460 RID: 1120
            // (get) Token: 0x06001C14 RID: 7188 RVA: 0x00013092 File Offset: 0x00011292
            public float LowerBodyFrc
            {
                get
                {
                    return (lower == null) ? 0f : 1f;
                }
            }

            // Token: 0x06001C15 RID: 7189 RVA: 0x00190EB0 File Offset: 0x0018F0B0
            public void UpdateLower()
            {
                Vector2 a = upper.PlaySpaceCoordinate(PIA);
                if (lower == null)
                {
                    if (!PIA.dangle)
                    {
                        Vector2 vector = a + new Vector2(0f, -15f);
                        float dst = float.MaxValue;
                        IntVector2 intVector = new IntVector2(-1, -1);
                        for (int i = 0; i < PIA.antlers.parts.Length; i++)
                        {
                            for (int j = 0; j < PIA.antlers.parts[i].positions.Length; j++)
                            {
                                Vector2 vector2 = PIA.PlaySpaceCoordinate(i, j, upper.side);
                                if (LegalLowerPos(vector2) && Custom.DistLess(vector, vector2, dst))
                                {
                                    dst = Vector2.Distance(vector, vector2);
                                    intVector = new IntVector2(i, j);
                                }
                            }
                        }
                        if (intVector.x != -1 && intVector.y != -1)
                        {
                            lower = new WalkerBeast.PlayerInAntlers.AntlerPoint(intVector.x, intVector.y, upper.side);
                        }
                    }
                }
                else if (!LegalLowerPos(lower.PlaySpaceCoordinate(PIA)))
                {
                    lower = null;
                }
            }

            // Token: 0x06001C16 RID: 7190 RVA: 0x00191018 File Offset: 0x0018F218
            private bool LegalLowerPos(Vector2 testPos)
            {
                Vector2 p = upper.PlaySpaceCoordinate(PIA);
                return Custom.DistLess(p, testPos, 30f) && !Custom.DistLess(p, testPos, 5f) && !Custom.DistLess(testPos, PIA.walkerBeast.mainBodyChunk.pos, PIA.walkerBeast.mainBodyChunk.rad + 9f);
            }

            // Token: 0x04001E61 RID: 7777
            public WalkerBeast.PlayerInAntlers.AntlerPoint upper;

            // Token: 0x04001E62 RID: 7778
            public WalkerBeast.PlayerInAntlers.AntlerPoint lower;

            // Token: 0x04001E63 RID: 7779
            public WalkerBeast.PlayerInAntlers PIA;
        }

        // Token: 0x0200045C RID: 1116
        public class AntlerPoint
        {
            // Token: 0x06001C17 RID: 7191 RVA: 0x000130AE File Offset: 0x000112AE
            public AntlerPoint(int part, int segment, float side)
            {
                this.part = part;
                this.segment = segment;
                this.side = side;
            }

            // Token: 0x06001C18 RID: 7192 RVA: 0x000130CB File Offset: 0x000112CB
            public Vector2 PlaySpaceCoordinate(WalkerBeast.PlayerInAntlers PIA)
            {
                return PIA.PlaySpaceCoordinate(part, segment, side);
            }

            // Token: 0x04001E64 RID: 7780
            public int part;

            // Token: 0x04001E65 RID: 7781
            public int segment;

            // Token: 0x04001E66 RID: 7782
            public float side;
        }
    }//This entire class is here to prevent errors and it should be removed as soon as posible
}
