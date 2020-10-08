using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    public class WalkerBeastAbstractAI : AbstractCreatureAI
    {
        public WalkerBeastAbstractAI(World world, AbstractCreature parent) : base(world, parent)
        {
            lastRoom = parent.pos.room;
            allowedNodes = new List<WorldCoordinate>();
            if (world.singleRoomWorld)
            {
                for (int i = 0; i < world.GetAbstractRoom(0).nodes.Length; i++)
                {
                    if (world.GetAbstractRoom(0).nodes[i].type == AbstractRoomNode.Type.SideExit && world.GetAbstractRoom(0).nodes[i].entranceWidth >= 5)
                    {
                        allowedNodes.Add(new WorldCoordinate(0, -1, -1, i));
                    }
                }
            }
            else
            {
                for (int j = 0; j < WalkerBeastAbstractAI.UGLYHARDCODEDALLOWEDROOMS.Length; j++)
                {
                    if (world.GetAbstractRoom(WalkerBeastAbstractAI.UGLYHARDCODEDALLOWEDROOMS[j]) != null)
                    {
                        int index = world.GetAbstractRoom(WalkerBeastAbstractAI.UGLYHARDCODEDALLOWEDROOMS[j]).index;
                        for (int k = 0; k < world.GetAbstractRoom(index).nodes.Length; k++)
                        {
                            if (world.GetAbstractRoom(index).nodes[k].type == AbstractRoomNode.Type.SideExit && world.GetAbstractRoom(index).nodes[k].entranceWidth >= 2)
                            {
                                allowedNodes.Add(new WorldCoordinate(index, -1, -1, k));
                                Debug.Log("UGLY HARD CODE ALLOWED ROOMS: " + index + ", " + k);
                            }
                        }
                    }
                }
            }
        }

        static WalkerBeastAbstractAI()
        {
            UGLYHARDCODEDALLOWEDROOMS = new string[]
            {
            "FS_C01",
            "FS_C02",
            "FS_L04",
            "FS_L05",
            "FS_L08",
            "FS_C04",
            "LF_E04",
            "LF_H01",
            "LF_J01",
            "LF_E05",
            "LF_H02",
            "LF_D07"
            };
        }

        public override void AbstractBehavior(int time)
        {
            if (path.Count > 0 && parent.realizedCreature == null)
            {
                FollowPath(time);
                return;
            }
            if (world.rainCycle.TimeUntilRain < 800)
            {
                if (denPosition == null || !parent.pos.CompareDisregardingTile(denPosition.Value))
                {
                    GoToDen();
                }
                return;
            }
            if (allowedNodes.Count == 0)
            {
                return;
            }
            if (path.Count == 0)
            {
                SetDestination(denPosition.Value);
            }
            if (parent.pos.room == world.offScreenDen.index)
            {
                Raid();
            }
            if (parent.pos.room != lastRoom)
            {
                lastRoom = parent.pos.room;
                timeInRoom = 0;
            }
            timeInRoom += time;
            if (timeInRoom > 1200)
            {
                timeInRoom -= 1200;
            }
        }

        private void Raid()
        {
            WorldCoordinate worldCoordinate = allowedNodes[UnityEngine.Random.Range(0, allowedNodes.Count)];
            WorldCoordinate item = new WorldCoordinate(worldCoordinate.room, -1, -1, -1);
            WorldCoordinate item2 = new WorldCoordinate(worldCoordinate.room, -1, -1, -1);
            int num3 = 0;
            while (num3 < world.GetAbstractRoom(worldCoordinate).nodes.Length && !item.NodeDefined)
            {
                if (num3 != worldCoordinate.abstractNode && world.GetAbstractRoom(worldCoordinate).nodes[num3].type == AbstractRoomNode.Type.SideExit && world.GetAbstractRoom(worldCoordinate).nodes[num3].entranceWidth >= 5 && world.GetAbstractRoom(worldCoordinate).ConnectionPossible(worldCoordinate.abstractNode, num3, parent.creatureTemplate))
                {
                    item.abstractNode = num3;
                }
                num3++;
            }
            SetDestination(parent.pos);
            path.Clear();
            path.Add(denPosition.Value);
            if (item.NodeDefined)
            {
                path.Add(item);
            }
            path.Add(worldCoordinate);
        }

        private new int lastRoom;

        public int timeInRoom;

        public List<WorldCoordinate> allowedNodes;

        public static string[] UGLYHARDCODEDALLOWEDROOMS;
    }
}