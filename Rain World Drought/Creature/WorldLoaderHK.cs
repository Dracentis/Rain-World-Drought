using Rain_World_Drought.Enums;

namespace Rain_World_Drought.Creatures
{
    internal static class WorldLoaderHK
    {
        public static void Patch()
        {
            On.WorldLoader.CreatureTypeFromString += new On.WorldLoader.hook_CreatureTypeFromString(CreatureTypeFromStringHK);
        }

        private static CreatureTemplate.Type? CreatureTypeFromStringHK(On.WorldLoader.orig_CreatureTypeFromString orig, string s)
        {
            switch (s)
            {
                case "WalkerBeast":
                case "Walker Beast":
                case "Walker":
                case "Dog":
                case "LongDog":
                    if (!DroughtMod.EnumExt) { return null; }
                    return new CreatureTemplate.Type?(EnumExt_Drought.WalkerBeast);
                case "GreyLizard":
                case "Grey Lizard":
                case "Grey":
                    if (!DroughtMod.EnumExt) { return null; }
                    return new CreatureTemplate.Type?(EnumExt_Drought.GreyLizard);
                case "SeaDrake":
                case "Sea Drake":
                case "Drake":
                    if (!DroughtMod.EnumExt) { return null; }
                    return new CreatureTemplate.Type?(EnumExt_Drought.SeaDrake);
                default: return orig.Invoke(s);
            }
        }
    }
}
