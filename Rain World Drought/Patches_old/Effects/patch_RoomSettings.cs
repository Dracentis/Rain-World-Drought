using MonoMod;
using UnityEngine;
using RWCustom;
using System.IO;
using System.Collections.Generic;

public class patch_RoomSettings : RoomSettings
{
    [MonoModIgnore]
    public patch_RoomSettings(string name, Region region, bool template, bool firstTemplate, int playerChar) : base(name, region, template, firstTemplate, playerChar)
    {
    }

    [MonoModIgnore]
    public extern void orig_ctor(string name, Region region, bool template, bool firstTemplate, int playerChar);

    [MonoModConstructor]
    public void ctor(string name, Region region, bool template, bool firstTemplate, int playerChar)
    {
        this.name = name;
        this.effects = new List<RoomSettings.RoomEffect>();
        this.ambientSounds = new List<AmbientSound>();
        this.placedObjects = new List<PlacedObject>();
        this.triggers = new List<EventTrigger>();
        this.isTemplate = template;
        this.isFirstTemplate = firstTemplate;
        if (name == "RootTemplate")
        {
            this.filePath = string.Empty;
            return;
        }
        if (template)
        {
            this.filePath = string.Concat(new object[]
            {
                Custom.RootFolderDirectory(),
                "World",
                Path.DirectorySeparatorChar,
                "Regions",
                Path.DirectorySeparatorChar,
                region.name,
                Path.DirectorySeparatorChar,
                name,
                ".txt"
            });
        }
        else
        {
            this.filePath = WorldLoader.FindRoomFileDirectory(name, false) + "_Settings.txt";
        }
        this.Reset();
        this.FindParent(region);
        this.Load(playerChar);
    }


    public class patch_RoomEffect : RoomEffect
    {
        [MonoModIgnore]
        public patch_RoomEffect(RoomEffect.Type type, float amount, bool inherited) : base(type, amount, inherited)
        {
        }

        public enum Type
        {
            None,
            SkyDandelions,
            SkyBloom,
            LightBurn,
            SkyAndLightBloom,
            Bloom,
            Fog,
            Lightning,
            BkgOnlyLightning,
            ExtraLoudThunder,
            GreenSparks,
            VoidMelt,
            ZeroG,
            BrokenZeroG,
            ZeroGSpecks,
            SunBlock,
            SuperStructureProjector,
            ProjectedScanLines,
            CorruptionSpores,
            SSSwarmers,
            SSMusic,
            AboveCloudsView,
            RoofTopView,
            VoidSea,
            ElectricDeath,
            VoidSpawn,
            BorderPushBack,
            Flies,
            FireFlies,
            TinyDragonFly,
            RockFlea,
            RedSwarmer,
            Ant,
            Beetle,
            WaterGlowworm,
            Wasp,
            Moth,
            Drain,
            Pulse,
            LMSwarmers,
            ElectricStorm,
            GravityPulse,
            TankRoomView

        }
    }
}