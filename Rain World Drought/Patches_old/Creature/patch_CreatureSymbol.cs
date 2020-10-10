using MonoMod;
using UnityEngine;
using RWCustom;
using Menu;


class patch_CreatureSymbol : CreatureSymbol
{
    [MonoModIgnore]
    public patch_CreatureSymbol(IconSymbolData iconData, FContainer container) : base(iconData, container)
    {
    }

    public static string SpriteNameOfCreature(IconSymbol.IconSymbolData iconData)
    {
        switch (iconData.critType)
        {
            case CreatureTemplate.Type.Slugcat:
                return "Kill_Slugcat";
            case CreatureTemplate.Type.PinkLizard:
            case CreatureTemplate.Type.BlueLizard:
            case CreatureTemplate.Type.RedLizard:
            case CreatureTemplate.Type.CyanLizard:
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard:
                return "Kill_Standard_Lizard";
            case CreatureTemplate.Type.GreenLizard:
                return "Kill_Green_Lizard";
            case CreatureTemplate.Type.YellowLizard:
                return "Kill_Yellow_Lizard";
            case CreatureTemplate.Type.WhiteLizard:
                return "Kill_White_Lizard";
            case CreatureTemplate.Type.BlackLizard:
                return "Kill_Black_Lizard";
            case CreatureTemplate.Type.Salamander:
                return "Kill_Salamander";
            case CreatureTemplate.Type.Fly:
                return "Kill_Bat";
            case CreatureTemplate.Type.Leech:
            case CreatureTemplate.Type.SeaLeech:
                return "Kill_Leech";
            case CreatureTemplate.Type.Snail:
                return "Kill_Snail";
            case CreatureTemplate.Type.Vulture:
                return "Kill_Vulture";
            case CreatureTemplate.Type.GarbageWorm:
                return "Kill_Garbageworm";
            case CreatureTemplate.Type.LanternMouse:
                return "Kill_Mouse";
            case CreatureTemplate.Type.CicadaA:
            case CreatureTemplate.Type.CicadaB:
                return "Kill_Cicada";
            case CreatureTemplate.Type.Spider:
                return "Kill_SmallSpider";
            case CreatureTemplate.Type.JetFish:
                return "Kill_Jetfish";
            case CreatureTemplate.Type.BigEel:
                return "Kill_BigEel";
            case CreatureTemplate.Type.Deer:
                return "Kill_RainDeer";
            case CreatureTemplate.Type.TubeWorm:
                return "Kill_Tubeworm";
            case CreatureTemplate.Type.DaddyLongLegs:
            case CreatureTemplate.Type.BrotherLongLegs:
                return "Kill_Daddy";
            case CreatureTemplate.Type.TentaclePlant:
                return "Kill_TentaclePlant";
            case CreatureTemplate.Type.PoleMimic:
                return "Kill_PoleMimic";
            case CreatureTemplate.Type.MirosBird:
                return "Kill_MirosBird";
            case CreatureTemplate.Type.Centipede:
                return "Kill_Centipede" + Custom.IntClamp(iconData.intData, 1, 3);
            case CreatureTemplate.Type.RedCentipede:
                return "Kill_Centipede3";
            case CreatureTemplate.Type.Centiwing:
                return "Kill_Centiwing";
            case CreatureTemplate.Type.SmallCentipede:
                return "Kill_Centipede1";
            case CreatureTemplate.Type.Scavenger:
                return "Kill_Scavenger";
            case CreatureTemplate.Type.Overseer:
                return "Kill_Overseer";
            case CreatureTemplate.Type.VultureGrub:
                return "Kill_VultureGrub";
            case CreatureTemplate.Type.EggBug:
                return "Kill_EggBug";
            case CreatureTemplate.Type.BigSpider:
            case CreatureTemplate.Type.SpitterSpider:
                return "Kill_BigSpider";
            case CreatureTemplate.Type.SmallNeedleWorm:
                return "Kill_SmallNeedleWorm";
            case CreatureTemplate.Type.BigNeedleWorm:
                return "Kill_NeedleWorm";
            case CreatureTemplate.Type.DropBug:
                return "Kill_DropBug";
            case CreatureTemplate.Type.KingVulture:
                return "Kill_KingVulture";
            case CreatureTemplate.Type.Hazer:
                return "Kill_Hazer";
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast:
                return "Kill_WalkerBeast";
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake:
                return "Kill_SeaDrake";
        }
        return "Futile_White";
    }

    // Token: 0x06000670 RID: 1648 RVA: 0x000356E4 File Offset: 0x000338E4
    public static Color ColorOfCreature(IconSymbol.IconSymbolData iconData)
    {
        switch (iconData.critType)
        {
            case CreatureTemplate.Type.Slugcat:
                return PlayerGraphics.SlugcatColor(iconData.intData);
            case CreatureTemplate.Type.PinkLizard:
                return (StaticWorld.GetCreatureTemplate(iconData.critType).breedParameters as LizardBreedParams).standardColor;
            case CreatureTemplate.Type.GreenLizard:
                return (StaticWorld.GetCreatureTemplate(iconData.critType).breedParameters as LizardBreedParams).standardColor;
            case CreatureTemplate.Type.BlueLizard:
                return (StaticWorld.GetCreatureTemplate(iconData.critType).breedParameters as LizardBreedParams).standardColor;
            case CreatureTemplate.Type.YellowLizard:
            case CreatureTemplate.Type.Centipede:
            case CreatureTemplate.Type.SmallCentipede:
                return new Color(1f, 0.6f, 0f);
            case CreatureTemplate.Type.WhiteLizard:
                return (StaticWorld.GetCreatureTemplate(iconData.critType).breedParameters as LizardBreedParams).standardColor;
            case CreatureTemplate.Type.RedLizard:
                return new Color(0.9019608f, 0.05490196f, 0.05490196f);
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard:
                return new Color(0.686f * 1.1f, 0.698f * 1.1f, 0.643f * 1.1f);
            case CreatureTemplate.Type.BlackLizard:
                return new Color(0.368627459f, 0.368627459f, 0.435294122f);
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake:
                return new Color(0.368627459f, 0.368627459f, 0.435294122f);
            case CreatureTemplate.Type.Salamander:
                return new Color(0.933333337f, 0.78039217f, 0.894117653f);
            case CreatureTemplate.Type.CyanLizard:
            case CreatureTemplate.Type.Overseer:
                return new Color(0f, 0.9098039f, 0.9019608f);
            case CreatureTemplate.Type.Leech:
            case CreatureTemplate.Type.SpitterSpider:
                return new Color(0.68235296f, 0.156862751f, 0.117647059f);
            case CreatureTemplate.Type.SeaLeech:
            case CreatureTemplate.Type.TubeWorm:
                return new Color(0.05f, 0.3f, 0.7f);
            case CreatureTemplate.Type.CicadaA:
                return new Color(1f, 1f, 1f);
            case CreatureTemplate.Type.CicadaB:
                return new Color(0.368627459f, 0.368627459f, 0.435294122f);
            case CreatureTemplate.Type.DaddyLongLegs:
                return new Color(0f, 0f, 1f);
            case CreatureTemplate.Type.BrotherLongLegs:
                return new Color(0.454901963f, 0.5254902f, 0.305882365f);
            case CreatureTemplate.Type.RedCentipede:
                return new Color(0.9019608f, 0.05490196f, 0.05490196f);
            case CreatureTemplate.Type.Centiwing:
                return new Color(0.05490196f, 0.698039234f, 0.235294119f);
            case CreatureTemplate.Type.VultureGrub:
                return new Color(0.831372559f, 0.7921569f, 0.435294122f);
            case CreatureTemplate.Type.EggBug:
                return new Color(0f, 1f, 0.470588237f);
            case CreatureTemplate.Type.SmallNeedleWorm:
            case CreatureTemplate.Type.BigNeedleWorm:
                return new Color(1f, 0.596078455f, 0.596078455f);
            case CreatureTemplate.Type.Hazer:
                return new Color(0.211764708f, 0.7921569f, 0.3882353f);
        }
        return Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);
    }
}

