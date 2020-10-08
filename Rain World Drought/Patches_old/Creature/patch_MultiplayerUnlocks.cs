using System;
using System.Collections.Generic;
using MonoMod;
using UnityEngine;
using RWCustom;

public class patch_MultiplayerUnlocks : MultiplayerUnlocks
{
    [MonoModIgnore]
    public patch_MultiplayerUnlocks(PlayerProgression progression, List<string> allLevels) : base(progression, allLevels)
    {
    }

    [MonoModIgnore]
    public extern void orig_ctor(PlayerProgression progression, List<string> allLevels);

    [MonoModConstructor]
    public void ctor(PlayerProgression progression, List<string> allLevels)
    {
        this.progression = progression;
        this.unlockAll = MultiplayerUnlocks.CheckUnlockAll();
        this.unlockNoSpoilers = MultiplayerUnlocks.CheckUnlockNoSpoilers();
        Debug.Log(string.Concat(new object[]
        {
            "unlockAll: ",
            this.unlockAll,
            " , unlockNoSpoilers: ",
            this.unlockNoSpoilers
        }));
        this.creaturesUnlockedForLevelSpawn = new bool[Enum.GetNames(typeof(CreatureTemplate.Type)).Length];
        this.unlockedBatches = new List<MultiplayerUnlocks.Unlock>();
        if (this.unlockNoSpoilers && !this.unlockAll)
        {
            this.unlockedBatches.Add(new MultiplayerUnlocks.Unlock(MultiplayerUnlocks.LevelUnlockID.Default));
            this.unlockedBatches.Add(new MultiplayerUnlocks.Unlock(MultiplayerUnlocks.LevelUnlockID.SU));
            this.unlockedBatches.Add(new MultiplayerUnlocks.Unlock(MultiplayerUnlocks.LevelUnlockID.HI));
            this.unlockedBatches.Add(new MultiplayerUnlocks.Unlock(MultiplayerUnlocks.LevelUnlockID.CC));
            this.unlockedBatches.Add(new MultiplayerUnlocks.Unlock(MultiplayerUnlocks.LevelUnlockID.DS));
        }
        else
        {
            int num = Enum.GetNames(typeof(MultiplayerUnlocks.LevelUnlockID)).Length;
            for (int i = 2; i < num; i++)
            {
                if (this.unlockAll || progression.miscProgressionData.GetTokenCollected((MultiplayerUnlocks.LevelUnlockID)i))
                {
                    this.unlockedBatches.Add(new MultiplayerUnlocks.Unlock((MultiplayerUnlocks.LevelUnlockID)i));
                }
            }
        }
        for (int j = 0; j < this.unlockedBatches.Count; j++)
        {
            for (int k = 0; k < this.unlockedBatches[j].creatures.Count; k++)
            {
                this.creaturesUnlockedForLevelSpawn[(int)this.unlockedBatches[j].creatures[k]] = true;
            }
        }
        //Replaced Code
        foreach (MultiplayerUnlocks.SandboxUnlockID creature in patch_MultiplayerUnlocks.CreatureUnlockList)
        {
            if (this.SandboxItemUnlocked(creature))
                this.creaturesUnlockedForLevelSpawn[(int)MultiplayerUnlocks.SymbolDataForSandboxUnlock(creature).critType] = true;
        }
        //-=-=-=-=-=-=-=-=-=-
        for (int m = 0; m < allLevels.Count; m++)
        {
            MultiplayerUnlocks.LevelUnlockID levelUnlockID = MultiplayerUnlocks.LevelLockID(allLevels[m]);
            for (int n = 0; n < this.unlockedBatches.Count; n++)
            {
                if (this.unlockedBatches[n].ID == levelUnlockID)
                {
                    this.unlockedBatches[n].levels.Add(allLevels[m]);
                    break;
                }
            }
        }
    }


    public static SandboxUnlockID[] CreatureUnlockList;
    public static SandboxUnlockID[] ItemUnlockList;

    public enum SandboxUnlockID
    {
        Slugcat,
        GreenLizard,
        PinkLizard,
        BlueLizard,
        WhiteLizard,
        BlackLizard,
        YellowLizard,
        CyanLizard,
        RedLizard,
        Salamander,
        Fly,
        CicadaA,
        CicadaB,
        Snail,
        Leech,
        SeaLeech,
        PoleMimic,
        TentaclePlant,
        Scavenger,
        VultureGrub,
        Vulture,
        KingVulture,
        SmallCentipede,
        MediumCentipede,
        BigCentipede,
        RedCentipede,
        Centiwing,
        TubeWorm,
        Hazer,
        LanternMouse,
        Spider,
        BigSpider,
        SpitterSpider,
        MirosBird,
        BrotherLongLegs,
        DaddyLongLegs,
        Deer,
        EggBug,
        DropBug,
        BigNeedleWorm,
        SmallNeedleWorm,
        Jetfish,
        BigEel,
        Rock,
        Spear,
        FireSpear,
        ScavengerBomb,
        SporePlant,
        Lantern,
        FlyLure,
        Mushroom,
        FlareBomb,
        PuffBall,
        WaterNut,
        FirecrackerPlant,
        DangleFruit,
        JellyFish,
        BubbleGrass,
        SlimeMold,
        //New Creatures
        WalkerBeast,
        GreyLizard,
        SeaDrake
    }

    public enum LevelUnlockID
    {
        Default,
        Hidden,
        SU,
        HI,
        CC,
        GW,
        SL,
        SH,
        DS,
        SI,
        LF,
        UW,
        SB,
        SS,
        FS,
        IS
    }

    public static MultiplayerUnlocks.LevelUnlockID LevelLockID(string levelName)
    {
        switch (levelName)
        {
            case "SU_Lolipops":
            case "SU_StoneHeads":
            case "SmallRoom":
                return MultiplayerUnlocks.LevelUnlockID.SU;
            case "HI_StovePipes":
            case "FourFingers":
            case "Platforms":
                return MultiplayerUnlocks.LevelUnlockID.HI;
            case "CC_Intersection":
            case "Thrones":
                return MultiplayerUnlocks.LevelUnlockID.CC;
            case "Grid":
            case "deathPit":
            case "Pylons":
                return MultiplayerUnlocks.LevelUnlockID.GW;
            case "WaterWorks":
            case "Refinery":
                return MultiplayerUnlocks.LevelUnlockID.SL;
            case "Cabinets":
            case "SH_BigRoom":
            case "SH_Planters":
                return MultiplayerUnlocks.LevelUnlockID.SH;
            case "DS_Drainage":
            case "DS_Filters":
            case "WaterReactor":
                return MultiplayerUnlocks.LevelUnlockID.DS;
            case "Antenna":
            case "Summit":
            case "SI_Array":
                return MultiplayerUnlocks.LevelUnlockID.SI;
            case "Accelerator":
            case "Pipelands":
                return MultiplayerUnlocks.LevelUnlockID.LF;
            case "swingRoom2":
            case "Joint":
                return MultiplayerUnlocks.LevelUnlockID.UW;
            case "Cave":
            case "Shortcuts":
            case "Nest":
                return MultiplayerUnlocks.LevelUnlockID.SB;
            case "Valves":
                return (MultiplayerUnlocks.LevelUnlockID)patch_MultiplayerUnlocks.LevelUnlockID.IS;
            case "Temple":
                return (MultiplayerUnlocks.LevelUnlockID)patch_MultiplayerUnlocks.LevelUnlockID.FS;
            case "Hub":
                return MultiplayerUnlocks.LevelUnlockID.Hidden;
        }
        return MultiplayerUnlocks.LevelUnlockID.Default;
    }

    public static IconSymbol.IconSymbolData SymbolDataForSandboxUnlock(MultiplayerUnlocks.SandboxUnlockID unlockID)
    {
        if (unlockID == MultiplayerUnlocks.SandboxUnlockID.MediumCentipede)
        {
            return new IconSymbol.IconSymbolData(CreatureTemplate.Type.Centipede, AbstractPhysicalObject.AbstractObjectType.Creature, 2);
        }
        if (unlockID == MultiplayerUnlocks.SandboxUnlockID.BigCentipede)
        {
            return new IconSymbol.IconSymbolData(CreatureTemplate.Type.Centipede, AbstractPhysicalObject.AbstractObjectType.Creature, 3);
        }
        if (unlockID == MultiplayerUnlocks.SandboxUnlockID.FireSpear)
        {
            return new IconSymbol.IconSymbolData(CreatureTemplate.Type.StandardGroundCreature, AbstractPhysicalObject.AbstractObjectType.Spear, 1);
        }
        foreach (MultiplayerUnlocks.SandboxUnlockID creature in patch_MultiplayerUnlocks.CreatureUnlockList)
        {
            if (unlockID == creature)
                return new IconSymbol.IconSymbolData(Custom.ParseEnum<CreatureTemplate.Type>(unlockID.ToString()), AbstractPhysicalObject.AbstractObjectType.Creature, 0);
        }
        return new IconSymbol.IconSymbolData(CreatureTemplate.Type.StandardGroundCreature, Custom.ParseEnum<AbstractPhysicalObject.AbstractObjectType>(unlockID.ToString()), 0);
    }
}