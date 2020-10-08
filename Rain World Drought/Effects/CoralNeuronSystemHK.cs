using CoralBrain;
using Rain_World_Drought.Enums;
using Rain_World_Drought.OverWorld;
using RWCustom;
using System.Collections.Generic;

namespace Rain_World_Drought.Effects
{
    internal static class CoralNeuronSystemHK
    {
        public static void Patch()
        {
            On.CoralBrain.CoralNeuronSystem.AIMapReady += new On.CoralBrain.CoralNeuronSystem.hook_AIMapReady(AIMapReadyHK);
        }

        private static void AIMapReadyHK(On.CoralBrain.CoralNeuronSystem.orig_AIMapReady orig, CoralNeuronSystem self)
        {
            orig.Invoke(self);
            if (!DroughtMod.EnumExt) { return; }
            if (self.room.roomSettings.GetEffectAmount(EnumExt_Drought.LMSwarmers) > 0f)
            {
                bool dark = self.room.roomSettings.Palette == 24 || (self.room.roomSettings.fadePalette != null && self.room.roomSettings.fadePalette.palette == 24);
                IntVector2[] accessableTiles = self.room.aimap.CreatureSpecificAImap(StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Fly)).accessableTiles;
                int num = (int)((float)accessableTiles.Length * 0.05f * self.room.roomSettings.GetEffectAmount(EnumExt_Drought.LMSwarmers));
                List<IntVector2> list = new List<IntVector2>();
                for (int i = 0; i < num; i++)
                {
                    list.Add(accessableTiles[UnityEngine.Random.Range(0, accessableTiles.Length)]);
                }
                LMOracleSwarmer.Behavior behavior = null;
                for (int j = 0; j < list.Count; j++)
                {
                    LMOracleSwarmer LMOracleSwarmer = new LMOracleSwarmer(new AbstractPhysicalObject(self.room.world, (AbstractPhysicalObject.AbstractObjectType)patch_AbstractPhysicalObject.AbstractObjectType.LMOracleSwarmer, null, self.room.GetWorldCoordinate(list[j]), self.room.game.GetNewID()), self.room.world);
                    LMOracleSwarmer.abstractPhysicalObject.destroyOnAbstraction = true;
                    LMOracleSwarmer.firstChunk.HardSetPosition(self.room.MiddleOfTile(list[j]));
                    LMOracleSwarmer.system = self;
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
                    self.room.AddObject(LMOracleSwarmer);
                    LMOracleSwarmer.NewRoom(self.room);
                }
            }
        }
    }
}
