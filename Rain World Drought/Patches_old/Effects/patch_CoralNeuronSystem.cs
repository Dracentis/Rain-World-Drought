using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using MonoMod;

[MonoModPatch("global::CoralBrain.CoralNeuronSystem")]
class patch_CoralNeuronSystem : CoralBrain.CoralNeuronSystem
{

    public extern void orig_AIMapReady();

    public void AIMapReady()
    {
        orig_AIMapReady();
        if (room.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.LMSwarmers) > 0f)
        {
            bool dark = room.roomSettings.Palette == 24 || (room.roomSettings.fadePalette != null && room.roomSettings.fadePalette.palette == 24);
            IntVector2[] accessableTiles = room.aimap.CreatureSpecificAImap(StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Fly)).accessableTiles;
            int num = (int)((float)accessableTiles.Length * 0.05f * room.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.LMSwarmers));
            System.Collections.Generic.List<IntVector2> list = new System.Collections.Generic.List<IntVector2>();
            for (int i = 0; i < num; i++)
            {
                list.Add(accessableTiles[UnityEngine.Random.Range(0, accessableTiles.Length)]);
            }
            LMOracleSwarmer.Behavior behavior = null;
            for (int j = 0; j < list.Count; j++)
            {
                LMOracleSwarmer LMOracleSwarmer = new LMOracleSwarmer(new AbstractPhysicalObject(room.world, (AbstractPhysicalObject.AbstractObjectType)patch_AbstractPhysicalObject.AbstractObjectType.LMOracleSwarmer, null, room.GetWorldCoordinate(list[j]), room.game.GetNewID()), room.world);
                LMOracleSwarmer.abstractPhysicalObject.destroyOnAbstraction = true;
                LMOracleSwarmer.firstChunk.HardSetPosition(room.MiddleOfTile(list[j]));
                LMOracleSwarmer.system = this;
                LMOracleSwarmer.waitToFindOthers = j;
                LMOracleSwarmer.dark = dark;
                if (behavior == null)
                {
                    behavior = LMOracleSwarmer.currentBehavior;
                }
                else
                {
                    LMOracleSwarmer.currentBehavior = behavior;
                }
                room.AddObject(LMOracleSwarmer);
                LMOracleSwarmer.NewRoom(room);
            }
        }
    }
}
