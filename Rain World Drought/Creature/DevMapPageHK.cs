using DevInterface;
using Rain_World_Drought.Enums;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    internal static class DevMapPageHK
    {
        public static void Patch()
        {
            On.DevInterface.MapPage.CreatureVis.CritString += new On.DevInterface.MapPage.CreatureVis.hook_CritString(CritStringHK);
            On.DevInterface.MapPage.CreatureVis.CritCol += new On.DevInterface.MapPage.CreatureVis.hook_CritCol(CritColHK);
        }

        private static string CritStringHK(On.DevInterface.MapPage.CreatureVis.orig_CritString orig, AbstractCreature crit)
        {
            switch (EnumSwitch.GetCreatureTemplateType(crit.creatureTemplate.type))
            {
                case EnumSwitch.CreatureTemplateType.DEFAULT:
                default:
                    switch (crit.creatureTemplate.type)
                    {
                        case CreatureTemplate.Type.PinkLizard:
                            return "pL";
                        case CreatureTemplate.Type.GreenLizard:
                            return "gL";
                        case CreatureTemplate.Type.BlueLizard:
                            return "bL";
                        case CreatureTemplate.Type.YellowLizard:
                            return "yL";
                        case CreatureTemplate.Type.WhiteLizard:
                            return "wL";
                        case CreatureTemplate.Type.RedLizard:
                            return "rL";
                        case CreatureTemplate.Type.BlackLizard:
                            return "blL";
                        case CreatureTemplate.Type.CyanLizard:
                            return "cL";
                        default: return orig.Invoke(crit);
                    }

                case EnumSwitch.CreatureTemplateType.WalkerBeast:
                    return "WB";
                case EnumSwitch.CreatureTemplateType.GreyLizard:
                    return "grL";
                case EnumSwitch.CreatureTemplateType.SeaDrake:
                    return "SD";
            }
        }

        private static Color CritColHK(On.DevInterface.MapPage.CreatureVis.orig_CritCol orig, AbstractCreature crit)
        {
            if (crit.InDen && Random.value < 0.5f)
            { return new Color(0.5f, 0.5f, 0.5f); }
            switch (EnumSwitch.GetCreatureTemplateType(crit.creatureTemplate.type))
            {
                case EnumSwitch.CreatureTemplateType.DEFAULT:
                default: return orig.Invoke(crit);

                // case EnumSwitch.CreatureTemplateType.WalkerBeast:
                // case EnumSwitch.CreatureTemplateType.GreyLizard:
                case EnumSwitch.CreatureTemplateType.SeaDrake:
                    return new Color(0.7f, 0.7f, 0.7f);
            }
        }
    }
}
