using System;
using RWCustom;
using MonoMod;


    class patch_PlacedObject : PlacedObject
    {
        [MonoModIgnore]
        patch_PlacedObject(PlacedObject.Type type, PlacedObject.Data data) : base(type, data) { }

        public enum Type
        {
            None,
            LightSource,
            FlareBomb,
            PuffBall,
            TempleGuard,
            LightFixture,
            DangleFruit,
            CoralStem,
            CoralStemWithNeurons,
            CoralNeuron,
            CoralCircuit,
            WallMycelia,
            ProjectedStars,
            ZapCoil,
            SuperStructureFuses,
            GravityDisruptor,
            SpotLight,
            DeepProcessing,
            Corruption,
            CorruptionTube,
            CorruptionDarkness,
            StuckDaddy,
            SSLightRod,
            CentipedeAttractor,
            DandelionPatch,
            GhostSpot,
            DataPearl,
            UniqueDataPearl,
            SeedCob,
            DeadSeedCob,
            WaterNut,
            JellyFish,
            KarmaFlower,
            Mushroom,
            SlimeMold,
            FlyLure,
            CosmeticSlimeMold,
            CosmeticSlimeMold2,
            FirecrackerPlant,
            VultureGrub,
            DeadVultureGrub,
            VoidSpawnEgg,
            ReliableSpear,
            SuperJumpInstruction,
            ProjectedImagePosition,
            ExitSymbolShelter,
            ExitSymbolHidden,
            NoSpearStickZone,
            LanternOnStick,
            ScavengerOutpost,
            TradeOutpost,
            ScavengerTreasury,
            ScavTradeInstruction,
            CustomDecal,
            InsectGroup,
            PlayerPushback,
            MultiplayerItem,
            SporePlant,
            GoldToken,
            BlueToken,
            DeadTokenStalk,
            NeedleEgg,
            BrokenShelterWaterLevel,
            BubbleGrass,
            Filter,
            ReliableIggyDirection,
            Hazer,
            DeadHazer,
            Rainbow,
            LightBeam,
            NoLeviathanStrandingZone,

            Radio,//Placed Object type for radios on the Spire
            SmallPiston,//Placed Object type for different types of Pistons\\
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
            TreeFruit//new fruit
    }
    }


