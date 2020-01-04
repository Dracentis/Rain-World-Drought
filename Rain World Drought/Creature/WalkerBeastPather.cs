using System;
using RWCustom;
using UnityEngine;

// Token: 0x02000465 RID: 1125
public class WalkerBeastPather : BorderExitPather
{
    public WalkerBeastPather(ArtificialIntelligence AI, World world, AbstractCreature creature) : base(AI, world, creature)
    {
        walkPastPointOfNoReturn = true;
        this.stepsPerFrame = 20;
    }
    
    public WalkerBeast walkerBeast
    {
        get
        {
            return creature.realizedCreature as WalkerBeast;
        }
    }
    
    public override void DestinationHasChanged(WorldCoordinate oldDestination, WorldCoordinate newDestination)
    {
    }
    
    public override void Update()
    {
        base.Update();
    }

    public override PathCost CheckConnectionCost(PathFinder.PathingCell start, PathFinder.PathingCell goal, MovementConnection connection, bool followingPath)
    {
        if (start != null && goal != null && connection != null && followingPath != null){
            return base.CheckConnectionCost(start, goal, connection, followingPath);
        }else{
            return new PathCost(1f, PathCost.Legality.Unallowed);
        }
    }
    
    public override PathCost HeuristicForCell(PathFinder.PathingCell cell, PathCost costToGoal)
    {
        return costToGoal;
    }
    
    public MovementConnection FollowPath(WorldCoordinate originPos, bool actuallyFollowingThisPath)
    {
        int num = int.MinValue;
        PathCost costToGoal = new PathCost(0f, PathCost.Legality.Unallowed);
        WorldCoordinate dest = originPos;
        if (!originPos.TileDefined && !originPos.NodeDefined)
        {
            return null;
        }
        WorldCoordinate worldCoordinate = RestrictedOriginPos(originPos);
        if (actuallyFollowingThisPath && debugDrawer != null)
        {
            debugDrawer.Blink(worldCoordinate);
        }
        AItile aitile = realizedRoom.aimap.getAItile(worldCoordinate.x, worldCoordinate.y);
        PathFinder.PathingCell pathingCell = PathingCellAtWorldCoordinate(worldCoordinate);
        if (pathingCell != null)
        {
            if (!pathingCell.reachable || !pathingCell.possibleToGetBackFrom)
            {
                OutOfElement(worldCoordinate);
            }
            MovementConnection movementConnection = null;
            PathCost b = new PathCost(0f, PathCost.Legality.Unallowed);
            int num2 = -acceptablePathAge;
            PathCost.Legality legality = PathCost.Legality.Unallowed;
            int num3 = -acceptablePathAge;
            float num4 = float.MaxValue;
            int num5 = 0;
            for (; ; )
            {
                MovementConnection movementConnection2 = ConnectionAtCoordinate(true, worldCoordinate, num5);
                num5++;
                if (movementConnection2 == null)
                {
                    break;
                }
                if (movementConnection2.type == MovementConnection.MovementType.Standard && movementConnection2.StartTile.FloatDist(movementConnection2.DestTile) > 1f)
                {
                    //Debug.Log("WTF " + movementConnection2);
                }
                if (!movementConnection2.destinationCoord.TileDefined || Custom.InsideRect(movementConnection2.DestTile, coveredArea))
                {
                    PathFinder.PathingCell pathingCell2 = PathingCellAtWorldCoordinate(movementConnection2.destinationCoord);
                    PathCost pathCost = CheckConnectionCost(pathingCell, pathingCell2, movementConnection2, true);
                    if (!pathingCell2.possibleToGetBackFrom && !walkPastPointOfNoReturn)
                    {
                        pathCost.legality = PathCost.Legality.Unallowed;
                    }
                    PathCost pathCost2 = pathingCell2.costToGoal + pathCost;
                    if (movementConnection2.destinationCoord.TileDefined && destination.TileDefined && movementConnection2.destinationCoord.Tile == destination.Tile)
                    {
                        pathCost2.resistance = 0f;
                    }
                    else if (realizedRoom.IsPositionInsideBoundries(creature.pos.Tile) && ConnectionAlreadyFollowedSeveralTimes(movementConnection2))
                    {
                        pathCost += new PathCost(0f, PathCost.Legality.Unwanted);
                    }
                    if (movementConnection2.type == MovementConnection.MovementType.OutsideRoom && !(AI as WalkerBeastAI).AllowMovementBetweenRooms)
                    {
                        pathCost += new PathCost(0f, PathCost.Legality.Unallowed);
                    }
                    if (Input.GetKey("u") && actuallyFollowingThisPath)
                    {
                        Debug.Log("                     ");
                        Debug.Log(movementConnection2.startCoord);
                        CDebug.Log(movementConnection2.type.ToString(), movementConnection2.destinationCoord);
                        Debug.Log(string.Concat(new object[]
                        {
                            "conn: ",
                            pathCost.legality.ToString(),
                            " ",
                            pathCost.resistance
                        }));
                        Debug.Log(string.Concat(new object[]
                        {
                            "costToGoal: ",
                            pathingCell2.costToGoal.legality.ToString(),
                            " ",
                            pathingCell2.costToGoal.resistance
                        }));
                        Debug.Log(string.Concat(new object[]
                        {
                            "totCost: ",
                            pathCost2.legality.ToString(),
                            " ",
                            pathCost2.resistance
                        }));
                        Debug.Log("generation: " + pathingCell2.generation);
                        if (!pathingCell2.possibleToGetBackFrom && !walkPastPointOfNoReturn)
                        {
                            Debug.Log("PONOR");
                        }
                    }
                    if (pathingCell2.generation > num3)
                    {
                        num3 = pathingCell2.generation;
                        num4 = pathCost2.resistance;
                    }
                    else if (pathingCell2.generation == num3 && pathCost2.resistance < num4)
                    {
                        num4 = pathCost2.resistance;
                    }
                    if (pathCost.legality < legality)
                    {
                        movementConnection = movementConnection2;
                        legality = pathCost.legality;
                        num2 = pathingCell2.generation;
                        b = pathCost2;
                    }
                    else if (pathCost.legality == legality)
                    {
                        if (pathingCell2.generation > num2)
                        {
                            movementConnection = movementConnection2;
                            legality = pathCost.legality;
                            num2 = pathingCell2.generation;
                            b = pathCost2;
                        }
                        else if (pathingCell2.generation == num2 && pathCost2 <= b)
                        {
                            movementConnection = movementConnection2;
                            legality = pathCost.legality;
                            num2 = pathingCell2.generation;
                            b = pathCost2;
                        }
                    }
                }
            }
            if (Input.GetKey("u") && actuallyFollowingThisPath)
            {

                Debug.Log(worldCoordinate);
                CDebug.Log("chosen move: ", movementConnection);
            }
            if (legality <= PathCost.Legality.Unwanted)
            {
                if (actuallyFollowingThisPath)
                {
                    if (movementConnection != null && movementConnection.type == MovementConnection.MovementType.ShortCut && realizedRoom.shortcutData(movementConnection.StartTile).shortCutType == ShortcutData.Type.RoomExit)
                    {
                        LeavingRoom();
                    }
                    creatureFollowingGeneration = num2;
                }
                if (!actuallyFollowingThisPath || movementConnection == null || movementConnection.type != MovementConnection.MovementType.OutsideRoom || movementConnection.destinationCoord.TileDefined || walkerBeast.shortcutDelay >= 1)
                {
                    return movementConnection;
                }
                int num6 = 30;
                if (!Custom.InsideRect(originPos.Tile, new IntRect(-num6, -num6, realizedRoom.TileWidth + num6, realizedRoom.TileHeight + num6)))
                {
                    foreach (WorldCoordinate worldCoordinate2 in world.sideAccessNodes)
                    {
                        PathFinder.PathingCell pathingCell3 = PathingCellAtWorldCoordinate(worldCoordinate2);
                        if (pathingCell3.generation > num)
                        {
                            num = pathingCell3.generation;
                            costToGoal = pathingCell3.costToGoal;
                            dest = worldCoordinate2;
                        }
                        else if (pathingCell3.generation == num && pathingCell3.costToGoal < costToGoal)
                        {
                            costToGoal = pathingCell3.costToGoal;
                            dest = worldCoordinate2;
                        }
                        if (worldCoordinate2.CompareDisregardingTile(destination))
                        {
                            dest = worldCoordinate2;
                            break;
                        }
                    }
                    if (!dest.CompareDisregardingTile(movementConnection.destinationCoord))
                    {
                        walkerBeast.AccessSideSpace(movementConnection.destinationCoord, dest);
                        if (dest.room != creaturePos.room)
                        {
                            LeavingRoom();
                        }
                    }
                    return null;
                }
                IntVector2 intVector = new IntVector2(0, 1);
                if (movementConnection.startCoord.x == 0)
                {
                    intVector = new IntVector2(-1, 0);
                }
                else if (movementConnection.startCoord.x == realizedRoom.TileWidth - 1)
                {
                    intVector = new IntVector2(1, 0);
                }
                else if (movementConnection.startCoord.y == 0)
                {
                    intVector = new IntVector2(0, -1);
                }
                return new MovementConnection(MovementConnection.MovementType.Standard, originPos, new WorldCoordinate(originPos.room, originPos.x + intVector.x * 10, originPos.y + intVector.y * 10, originPos.abstractNode), 1);
            }
        }
        return null;
    }
    
    private bool ConnectionAlreadyFollowedSeveralTimes(MovementConnection connection)
    {
        return false;
    }
}
