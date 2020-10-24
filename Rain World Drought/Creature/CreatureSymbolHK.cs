using Rain_World_Drought.Enums;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    internal static class CreatureSymbolHK
    {
        public static void Patch()
        {
            On.CreatureSymbol.SpriteNameOfCreature += new On.CreatureSymbol.hook_SpriteNameOfCreature(SpriteNameOfCreatureHK);
            On.CreatureSymbol.ColorOfCreature += new On.CreatureSymbol.hook_ColorOfCreature(ColorOfCreatureHK);
        }

        private static string SpriteNameOfCreatureHK(On.CreatureSymbol.orig_SpriteNameOfCreature orig, IconSymbol.IconSymbolData iconData)
        {
            switch (EnumSwitch.GetCreatureTemplateType(iconData.critType))
            {
                default:
                case EnumSwitch.CreatureTemplateType.DEFAULT:
                    return orig.Invoke(iconData);

                case EnumSwitch.CreatureTemplateType.GreyLizard:
                    return "Kill_Standard_Lizard";

                case EnumSwitch.CreatureTemplateType.WalkerBeast:
                    return "Kill_WalkerBeast";

                case EnumSwitch.CreatureTemplateType.SeaDrake:
                    return "Kill_SeaDrake";
            }
        }

        private static Color ColorOfCreatureHK(On.CreatureSymbol.orig_ColorOfCreature orig, IconSymbol.IconSymbolData iconData)
        {
            switch (EnumSwitch.GetCreatureTemplateType(iconData.critType))
            {
                default:
                case EnumSwitch.CreatureTemplateType.DEFAULT:
                    return orig.Invoke(iconData);

                case EnumSwitch.CreatureTemplateType.GreyLizard:
                    return new Color(0.725f, 0.748f, 0.807f); // new Color(0.686f * 1.1f, 0.698f * 1.1f, 0.643f * 1.1f);

                case EnumSwitch.CreatureTemplateType.WalkerBeast:
                    return new Color(0.368627459f, 0.368627459f, 0.435294122f);

                case EnumSwitch.CreatureTemplateType.SeaDrake:
                    return new Color(0.368627459f, 0.368627459f, 0.435294122f);
            }
        }
    }
}
