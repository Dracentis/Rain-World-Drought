using Rain_World_Drought.Enums;
using System.Collections.Generic;

namespace Rain_World_Drought.Creatures
{
    internal static class CreatureTemplateHK
    {
        public static void Patch()
        {
            On.CreatureTemplate.ctor += new On.CreatureTemplate.hook_ctor(CtorHK);
        }

        private static void CtorHK(On.CreatureTemplate.orig_ctor orig, CreatureTemplate self,
            CreatureTemplate.Type type, CreatureTemplate ancestor, List<TileTypeResistance> tileResistances, List<TileConnectionResistance> connectionResistances, CreatureTemplate.Relationship defaultRelationship)
        {
            orig.Invoke(self, type, ancestor, tileResistances, connectionResistances, defaultRelationship);
            switch (EnumSwitch.GetCreatureTemplateType(type))
            {
                case EnumSwitch.CreatureTemplateType.SeaDrake:
                    self.name = "SeaDrake"; break;
                case EnumSwitch.CreatureTemplateType.WalkerBeast:
                    self.name = "WalkerBeast"; break;
                case EnumSwitch.CreatureTemplateType.GreyLizard:
                    self.name = "GreyLizard"; break;

                // unused
                case EnumSwitch.CreatureTemplateType.CrossBat:
                    self.name = "CrossBat"; break;
                case EnumSwitch.CreatureTemplateType.Lightworm:
                    self.name = "Lightworm"; break;

                case EnumSwitch.CreatureTemplateType.DEFAULT:
                default:
                    return;
            }
        }
    }
}