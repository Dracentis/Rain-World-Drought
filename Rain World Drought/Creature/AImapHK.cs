using Rain_World_Drought.Enums;
using RWCustom;

namespace Rain_World_Drought.Creatures
{
    internal static class AImapHK
    {
        public static void Patch()
        {
            On.AImap.TileAccessibleToCreature_2 += new On.AImap.hook_TileAccessibleToCreature_2(TileAccessibleToCreatureHK);
        }

        private static bool TileAccessibleToCreatureHK(On.AImap.orig_TileAccessibleToCreature_2 orig, AImap self, IntVector2 pos, CreatureTemplate crit)
        {
            if (DroughtMod.EnumExt && crit.type == EnumExt_Drought.WalkerBeast)
            {
                crit.type = CreatureTemplate.Type.Deer;
                bool res = orig.Invoke(self, pos, crit);
                crit.type = EnumExt_Drought.WalkerBeast;
                return res;
            }
            return orig.Invoke(self, pos, crit);
        }
    }
}
