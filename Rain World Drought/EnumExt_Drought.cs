using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rain_World_Drought.Enums
{
    /// <summary>
    /// Use Get Methods to use switch-case easily
    /// </summary>
    public static class EnumSwitch
    {
        public static CreatureTemplateType GetCreatureTemplateType(CreatureTemplate.Type type)
        {
            if (!DroughtMod.EnumExt) { return CreatureTemplateType.DEFAULT; }
            if (type == EnumExt_Drought.LightWorm) { return CreatureTemplateType.Lightworm; }
            if (type == EnumExt_Drought.CrossBat) { return CreatureTemplateType.CrossBat; }
            if (type == EnumExt_Drought.WalkerBeast) { return CreatureTemplateType.WalkerBeast; }
            if (type == EnumExt_Drought.GreyLizard) { return CreatureTemplateType.GreyLizard; }
            if (type == EnumExt_Drought.SeaDrake) { return CreatureTemplateType.SeaDrake; }
            return CreatureTemplateType.DEFAULT;
        }

        public enum CreatureTemplateType
        {
            /// <summary>
            /// Vanilla or other mod
            /// </summary>
            DEFAULT = -1,
            Lightworm,
            CrossBat,
            WalkerBeast,
            GreyLizard,
            SeaDrake
        };

        public static RoomEffectType GetRoomEffectType(RoomSettings.RoomEffect.Type type)
        {
            if (!DroughtMod.EnumExt) { return RoomEffectType.DEFAULT; }
            if (type == EnumExt_Drought.Drain) { return RoomEffectType.Drain; }
            if (type == EnumExt_Drought.Pulse) { return RoomEffectType.Pulse; }
            if (type == EnumExt_Drought.LMSwarmers) { return RoomEffectType.LMSwarmers; }
            if (type == EnumExt_Drought.ElectricStorm) { return RoomEffectType.ElectricStorm; }
            if (type == EnumExt_Drought.GravityPulse) { return RoomEffectType.GravityPulse; }
            if (type == EnumExt_Drought.TankRoomView) { return RoomEffectType.TankRoomView; }
            return RoomEffectType.DEFAULT;
        }

        public enum RoomEffectType
        {
            /// <summary>
            /// Vanilla or other mod
            /// </summary>
            DEFAULT = -1,
            Drain,
            Pulse,
            LMSwarmers,
            ElectricStorm,
            GravityPulse,
            TankRoomView
        };

        public static PlacedObjectType GetPlacedObjectType(PlacedObject.Type type)
        {
            if (!DroughtMod.EnumExt) { return PlacedObjectType.DEFAULT; }
            if (type == EnumExt_DroughtPlaced.Radio) { return PlacedObjectType.Radio; }
            if (type == EnumExt_DroughtPlaced.SmallPiston) { return PlacedObjectType.SmallPiston; }
            if (type == EnumExt_DroughtPlaced.LargePiston) { return PlacedObjectType.LargePiston; }
            if (type == EnumExt_DroughtPlaced.GiantPiston) { return PlacedObjectType.GiantPiston; }
            if (type == EnumExt_DroughtPlaced.SmallPistonTopDeathMode) { return PlacedObjectType.SmallPistonTopDeathMode; }
            if (type == EnumExt_DroughtPlaced.SmallPistonBotDeathMode) { return PlacedObjectType.SmallPistonBotDeathMode; }
            if (type == EnumExt_DroughtPlaced.SmallPistonDeathMode) { return PlacedObjectType.SmallPistonDeathMode; }
            if (type == EnumExt_DroughtPlaced.LargePistonTopDeathMode) { return PlacedObjectType.LargePistonTopDeathMode; }
            if (type == EnumExt_DroughtPlaced.LargePistonBotDeathMode) { return PlacedObjectType.LargePistonBotDeathMode; }
            if (type == EnumExt_DroughtPlaced.LargePistonDeathMode) { return PlacedObjectType.LargePistonDeathMode; }
            if (type == EnumExt_DroughtPlaced.GiantPistonTopDeathMode) { return PlacedObjectType.GiantPistonTopDeathMode; }
            if (type == EnumExt_DroughtPlaced.GiantPistonBotDeathMode) { return PlacedObjectType.GiantPistonBotDeathMode; }
            if (type == EnumExt_DroughtPlaced.GiantPistonDeathMode) { return PlacedObjectType.GiantPistonDeathMode; }
            if (type == EnumExt_DroughtPlaced.GravityAmplifyer) { return PlacedObjectType.GravityAmplifyer; }
            if (type == EnumExt_DroughtPlaced.ImprovementTrigger) { return PlacedObjectType.ImprovementTrigger; }
            if (type == EnumExt_DroughtPlaced.TreeFruit) { return PlacedObjectType.TreeFruit; }
            return PlacedObjectType.DEFAULT;
        }

        public enum PlacedObjectType
        {
            /// <summary>
            /// Vanilla or other mod
            /// </summary>
            DEFAULT = -1, // Vanilla or other mod
            /// <summary>
            /// Placed Object type for radios on the Spire
            /// </summary>
            Radio,
            // Placed Object type for different types of Pistons
            SmallPiston,
            LargePiston,
            GiantPiston,
            SmallPistonTopDeathMode,
            SmallPistonBotDeathMode,
            SmallPistonDeathMode,
            LargePistonTopDeathMode,
            LargePistonBotDeathMode,
            LargePistonDeathMode,
            GiantPistonTopDeathMode,
            GiantPistonBotDeathMode,
            GiantPistonDeathMode,
            GravityAmplifyer,
            ImprovementTrigger,
            /// <summary>
            /// new fruit
            /// </summary>
            TreeFruit
        };

        public static AbstractPhysicalObjectType GetAbstractPhysicalObjectType(AbstractPhysicalObject.AbstractObjectType type)
        {
            if (!DroughtMod.EnumExt) { return AbstractPhysicalObjectType.DEFAULT; }
            if (type == EnumExt_Drought.SmallPiston) { return AbstractPhysicalObjectType.SmallPiston; }
            if (type == EnumExt_Drought.LargePiston) { return AbstractPhysicalObjectType.LargePiston; }
            if (type == EnumExt_Drought.GiantPiston) { return AbstractPhysicalObjectType.GiantPiston; }
            if (type == EnumExt_Drought.LMOracleSwarmer) { return AbstractPhysicalObjectType.LMOracleSwarmer; }
            return AbstractPhysicalObjectType.DEFAULT;
        }

        public enum AbstractPhysicalObjectType
        {
            /// <summary>
            /// Vanilla or other mod
            /// </summary>
            DEFAULT = -1,
            SmallPiston,
            LargePiston,
            GiantPiston,
            LMOracleSwarmer
        };

        public static DreamsStateID GetDreamsStateID(DreamsState.DreamID id)
        {
            if (!DroughtMod.EnumExt) { return DreamsStateID.DEFAULT; }
            if (id == EnumExt_Drought.SRSDreamPearlLF) { return DreamsStateID.SRSDreamPearlLF; }
            if (id == EnumExt_Drought.SRSDreamPearlLF2) { return DreamsStateID.SRSDreamPearlLF2; }
            if (id == EnumExt_Drought.SRSDreamPearlHI) { return DreamsStateID.SRSDreamPearlHI; }
            if (id == EnumExt_Drought.SRSDreamPearlSH) { return DreamsStateID.SRSDreamPearlSH; }
            if (id == EnumExt_Drought.SRSDreamPearlDS) { return DreamsStateID.SRSDreamPearlDS; }
            if (id == EnumExt_Drought.SRSDreamPearlSB) { return DreamsStateID.SRSDreamPearlSB; }
            if (id == EnumExt_Drought.SRSDreamPearlSB2) { return DreamsStateID.SRSDreamPearlSB2; }
            if (id == EnumExt_Drought.SRSDreamPearlGW) { return DreamsStateID.SRSDreamPearlGW; }
            if (id == EnumExt_Drought.SRSDreamPearlSL) { return DreamsStateID.SRSDreamPearlSL; }
            if (id == EnumExt_Drought.SRSDreamPearlSL2) { return DreamsStateID.SRSDreamPearlSL2; }
            if (id == EnumExt_Drought.SRSDreamPearlSL3) { return DreamsStateID.SRSDreamPearlSL3; }
            if (id == EnumExt_Drought.SRSDreamPearlSI) { return DreamsStateID.SRSDreamPearlSI; }
            if (id == EnumExt_Drought.SRSDreamPearlSI2) { return DreamsStateID.SRSDreamPearlSI2; }
            if (id == EnumExt_Drought.SRSDreamPearlSI3) { return DreamsStateID.SRSDreamPearlSI3; }
            if (id == EnumExt_Drought.SRSDreamPearlSI4) { return DreamsStateID.SRSDreamPearlSI4; }
            if (id == EnumExt_Drought.SRSDreamPearlSI5) { return DreamsStateID.SRSDreamPearlSI5; }
            if (id == EnumExt_Drought.SRSDreamPearlSU) { return DreamsStateID.SRSDreamPearlSU; }
            if (id == EnumExt_Drought.SRSDreamPearlUW) { return DreamsStateID.SRSDreamPearlUW; }
            if (id == EnumExt_Drought.SRSDreamPearlIS) { return DreamsStateID.SRSDreamPearlIS; }
            if (id == EnumExt_Drought.SRSDreamPearlFS) { return DreamsStateID.SRSDreamPearlFS; }
            if (id == EnumExt_Drought.SRSDreamPearlMW) { return DreamsStateID.SRSDreamPearlMW; }
            if (id == EnumExt_Drought.SRSDreamTraitor) { return DreamsStateID.SRSDreamTraitor; }
            if (id == EnumExt_Drought.SRSDreamMissonComplete) { return DreamsStateID.SRSDreamMissonComplete; }
            return DreamsStateID.DEFAULT;
        }

        public enum DreamsStateID
        {
            /// <summary>
            /// Vanilla or other mod
            /// </summary>
            DEFAULT = -1,
            SRSDreamPearlLF = 0,
            SRSDreamPearlLF2 = 1,
            SRSDreamPearlHI = 2,
            SRSDreamPearlSH = 3,
            SRSDreamPearlDS = 4,
            SRSDreamPearlSB = 5,
            SRSDreamPearlSB2 = 15,
            SRSDreamPearlGW = 6,
            /// <summary>
            /// Iterators and Water
            /// </summary>
            SRSDreamPearlSL = 7,
            /// <summary>
            /// Moon's Essay
            /// </summary>
            SRSDreamPearlSL2 = 8,
            /// <summary>
            /// Purposed Organisms
            /// </summary>
            SRSDreamPearlSL3 = 9,
            /// <summary>
            /// FP and SRS Frustration
            /// </summary>
            SRSDreamPearlSI = 10,
            /// <summary>
            /// Erratic Pulse
            /// </summary>
            SRSDreamPearlSI2 = 11,
            /// <summary>
            /// Silver of Straw
            /// </summary>
            SRSDreamPearlSI3 = 12,
            /// <summary>
            /// Moon and FP relations
            /// </summary>
            SRSDreamPearlSI4 = 13,
            /// <summary>
            /// About Messenger
            /// </summary>
            SRSDreamPearlSI5 = 14,
            SRSDreamPearlSU = 16,
            SRSDreamPearlUW = 17,
            /// <summary>
            /// Drought Pearl IS
            /// </summary>
            SRSDreamPearlIS = 19,
            /// <summary>
            /// Drought Pearl FS
            /// </summary>
            SRSDreamPearlFS = 18,
            /// <summary>
            /// Drought Pearl MW
            /// </summary>
            SRSDreamPearlMW = 20,
            SRSDreamTraitor = 22,
            SRSDreamMissonComplete = 21
        };

        public static AbstractDataPearlType GetAbstractDataPearlType(DataPearl.AbstractDataPearl.DataPearlType type)
        {
            if (!DroughtMod.EnumExt) { return AbstractDataPearlType.DEFAULT; }
            if (type == EnumExt_Drought.MoonPearl) { return AbstractDataPearlType.MoonPearl; }
            if (type == EnumExt_Drought.DroughtPearl1) { return AbstractDataPearlType.DroughtPearl1; }
            if (type == EnumExt_Drought.DroughtPearl2) { return AbstractDataPearlType.DroughtPearl2; }
            if (type == EnumExt_Drought.DroughtPearl3) { return AbstractDataPearlType.DroughtPearl3; }
            if (type == EnumExt_Drought.SI_Spire1) { return AbstractDataPearlType.SI_Spire1; }
            if (type == EnumExt_Drought.SI_Spire2) { return AbstractDataPearlType.SI_Spire2; }
            if (type == EnumExt_Drought.SI_Spire3) { return AbstractDataPearlType.SI_Spire3; }
            if (type == EnumExt_Drought.WipedPearl) { return AbstractDataPearlType.WipedPearl; }
            return AbstractDataPearlType.DEFAULT;
        }

        public enum AbstractDataPearlType
        {
            /// <summary>
            /// Vanilla or other mod
            /// </summary>
            DEFAULT = -1,
            /// <summary>
            /// Any of moons pearls
            /// </summary>
            MoonPearl,
            /// <summary>
            /// IS Pearl
            /// </summary>
            DroughtPearl1,
            /// <summary>
            /// FS Pearl
            /// </summary>
            DroughtPearl2,
            /// <summary>
            /// MW Pearl
            /// </summary>
            DroughtPearl3,
            SI_Spire1,
            SI_Spire2,
            SI_Spire3,
            /// <summary>
            /// No Text
            /// </summary>
            WipedPearl
        }
    }

    public static class EnumExt_Drought
    {
        public static CreatureTemplate.Type LightWorm;
        public static CreatureTemplate.Type CrossBat;
        /// <summary>
        /// LONG DOG!!!!!
        /// </summary>
        public static CreatureTemplate.Type WalkerBeast;
        public static CreatureTemplate.Type GreyLizard;
        public static CreatureTemplate.Type SeaDrake;

        public static RoomSettings.RoomEffect.Type Drain;
        public static RoomSettings.RoomEffect.Type Pulse;
        public static RoomSettings.RoomEffect.Type LMSwarmers;
        public static RoomSettings.RoomEffect.Type ElectricStorm;
        public static RoomSettings.RoomEffect.Type GravityPulse;
        public static RoomSettings.RoomEffect.Type TankRoomView;

        public static AbstractPhysicalObject.AbstractObjectType SmallPiston;
        public static AbstractPhysicalObject.AbstractObjectType LargePiston;
        public static AbstractPhysicalObject.AbstractObjectType GiantPiston;
        public static AbstractPhysicalObject.AbstractObjectType LMOracleSwarmer;

        public static ProcessManager.ProcessID MessageScreen;

        public static DreamsState.DreamID SRSDreamPearlLF;
        public static DreamsState.DreamID SRSDreamPearlLF2;
        public static DreamsState.DreamID SRSDreamPearlHI;
        public static DreamsState.DreamID SRSDreamPearlSH;
        public static DreamsState.DreamID SRSDreamPearlDS;
        public static DreamsState.DreamID SRSDreamPearlSB;
        public static DreamsState.DreamID SRSDreamPearlSB2;
        public static DreamsState.DreamID SRSDreamPearlGW;
        /// <summary>
        /// Iterators and Water
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlSL;
        /// <summary>
        /// Moon's Essay
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlSL2;
        /// <summary>
        /// Purposed Organisms
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlSL3;
        /// <summary>
        /// FP and SRS Frustration
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlSI;
        /// <summary>
        /// Erratic Pulse
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlSI2;
        /// <summary>
        /// Silver of Straw
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlSI3;
        /// <summary>
        /// Moon and FP relations
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlSI4;
        /// <summary>
        /// About Messenger
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlSI5;
        public static DreamsState.DreamID SRSDreamPearlSU;
        public static DreamsState.DreamID SRSDreamPearlUW;
        /// <summary>
        /// Drought Pearl IS
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlIS;
        /// <summary>
        /// Drought Pearl FS
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlFS;
        /// <summary>
        /// Drought Pearl MW
        /// </summary>
        public static DreamsState.DreamID SRSDreamPearlMW;
        public static DreamsState.DreamID SRSDreamTraitor;
        public static DreamsState.DreamID SRSDreamMissonComplete;

        /// <summary>
        /// Any of moons pearls
        /// </summary>
        public static DataPearl.AbstractDataPearl.DataPearlType MoonPearl;
        /// <summary>
        /// IS Pearl
        /// </summary>
        public static DataPearl.AbstractDataPearl.DataPearlType DroughtPearl1;
        /// <summary>
        /// FS Pearl
        /// </summary>
        public static DataPearl.AbstractDataPearl.DataPearlType DroughtPearl2;
        /// <summary>
        /// MW Pearl
        /// </summary>
        public static DataPearl.AbstractDataPearl.DataPearlType DroughtPearl3;
        public static DataPearl.AbstractDataPearl.DataPearlType SI_Spire1;
        public static DataPearl.AbstractDataPearl.DataPearlType SI_Spire2;
        public static DataPearl.AbstractDataPearl.DataPearlType SI_Spire3;
        /// <summary>
        /// No Text
        /// </summary>
        public static DataPearl.AbstractDataPearl.DataPearlType WipedPearl;
    }

    /// <summary>
    /// PlacedObjects, because these names are the duplicate of AbstractPhysicalObject.AbstractObjectType
    /// </summary>
    public static class EnumExt_DroughtPlaced
    {
        /// <summary>
        /// Placed Object type for radios on the Spire
        /// </summary>
        public static PlacedObject.Type Radio; //
        // Placed Object type for different types of Pistons
        public static PlacedObject.Type SmallPiston;
        public static PlacedObject.Type LargePiston;
        public static PlacedObject.Type GiantPiston;
        public static PlacedObject.Type SmallPistonTopDeathMode;
        public static PlacedObject.Type SmallPistonBotDeathMode;
        public static PlacedObject.Type SmallPistonDeathMode;
        public static PlacedObject.Type LargePistonTopDeathMode;
        public static PlacedObject.Type LargePistonBotDeathMode;
        public static PlacedObject.Type LargePistonDeathMode;
        public static PlacedObject.Type GiantPistonTopDeathMode;
        public static PlacedObject.Type GiantPistonBotDeathMode;
        public static PlacedObject.Type GiantPistonDeathMode;
        public static PlacedObject.Type GravityAmplifyer;
        public static PlacedObject.Type ImprovementTrigger;
        /// <summary>
        /// New fruit
        /// </summary>
        public static PlacedObject.Type TreeFruit;

        public static SLOracleBehaviorHasMark.MiscItemType LMOracleSwarmer;
        public static SLOracleBehaviorHasMark.MiscItemType SSOracleSwarmer;
    }

    /// <summary>
    /// MultiplayerUnlocks, because these names are the duplicate of CreatureTemplate.Type
    /// </summary>
    public static class EnumExt_Unlocks
    {
        public const int SandboxUnlockIDExtend = 3;
        public static MultiplayerUnlocks.SandboxUnlockID WalkerBeast;
        public static MultiplayerUnlocks.SandboxUnlockID GreyLizard;
        public static MultiplayerUnlocks.SandboxUnlockID SeaDrake;

        public static MultiplayerUnlocks.LevelUnlockID FS;
        public static MultiplayerUnlocks.LevelUnlockID IS;
    }
}
