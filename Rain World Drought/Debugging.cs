using System;
using UnityEngine;
using RWCustom;

namespace Rain_World_Drought
{
    internal static class Debugging
    {
        public static void Patch()
        {
            //On.OverseersWorldAI.ShelterFinder.Update += ShelterFinder_Update;
        }

        private static void ShelterFinder_Update(On.OverseersWorldAI.ShelterFinder.orig_Update orig, OverseersWorldAI.ShelterFinder self)
        {
            if (self.done)
            {
                return;
            }
            if (self.checkNext.Count < 1)
            {
                self.NextShelter();
            }
            else
            {
                float num = float.MaxValue;
                int num2 = -1;
                for (int i = 0; i < self.checkNext.Count; i++)
                {
                    float num3 = self.ResistanceOfCell(self.checkNext[i]);
                    if (num3 > -1f && num3 < num)
                    {
                        num = num3;
                        num2 = i;
                    }
                }
                if (num2 < 0)
                {
                    self.NextShelter();
                    return;
                }
                IntVector2 testCell = self.checkNext[num2];
                self.checkNext.RemoveAt(num2);
                AbstractRoom abstractRoom = self.world.GetAbstractRoom(testCell.x + self.world.firstRoomIndex);
                float num4 = self.ResistanceOfCell(testCell);
                float[] o = null;
                for (int j = 0; j < abstractRoom.connections.Length; j++)
                {
                    WorldCoordinate worldCoordinate = new WorldCoordinate();
                    float num5 = 0;
                    if (j == testCell.y && abstractRoom.connections[j] > -1)
                    {
                        worldCoordinate = new WorldCoordinate(abstractRoom.connections[j], -1, -1, self.world.GetAbstractRoom(abstractRoom.connections[j]).ExitIndex(abstractRoom.index));
                        num5 = (float)self.world.TotalShortCutLengthBetweenTwoConnectedRooms(abstractRoom.index, abstractRoom.connections[j]);
                    }
                    else
                    {
                        try
                        {
                            worldCoordinate = new WorldCoordinate(abstractRoom.index, -1, -1, j);
                            num5 = (float)abstractRoom.nodes[j].ConnectionLength(testCell.y, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Fly));
                            if (num5 == -1f)
                            {
                                num5 = 10000f;
                            }
                        } catch(Exception e)
                        {
                            Debug.LogError($"Failed at 1 in room {abstractRoom.name}! {j}/[{abstractRoom.connections.Length},{abstractRoom.nodes.Length}]");
                            Debug.LogError(e);
                        }
                    }
                    try
                    {
                        o = self.matrix[self.currentlyMappingShelter, worldCoordinate.room - self.world.firstRoomIndex];
                        if (num5 > -1f && o[worldCoordinate.abstractNode] == -1f)
                        {
                            self.matrix[self.currentlyMappingShelter, worldCoordinate.room - self.world.firstRoomIndex][worldCoordinate.abstractNode] = num4 + num5;
                            self.checkNext.Add(new IntVector2(worldCoordinate.room - self.world.firstRoomIndex, worldCoordinate.abstractNode));
                        }
                    } catch(Exception e)
                    {
                        Debug.LogError($"Failed at 2! [{self.currentlyMappingShelter},{worldCoordinate.room - self.world.firstRoomIndex}][{worldCoordinate.abstractNode}]/[{self.matrix.GetLength(0)},{self.matrix.GetLength(0)}][{o?.Length ?? -1}]");
                        Debug.LogError(e);
                    }
                }
            }
        }
    }
}
