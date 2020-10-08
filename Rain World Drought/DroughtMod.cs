using Partiality.Modloader;
using Rain_World_Drought.Creatures;
using Rain_World_Drought.Effects;
using Rain_World_Drought.Enums;
using Rain_World_Drought.OverWorld;
using Rain_World_Drought.PlacedObjects;
using Rain_World_Drought.Resources;
using Rain_World_Drought.Slugcat;

namespace Rain_World_Drought
{
    public class DroughtMod : PartialityMod
    {
        public DroughtMod()
        {
            this.Version = "2000";
            this.ModID = "Rain World Drought";
        }

        public override void OnEnable()
        {
            base.OnEnable();

            #region Creatures
            // namespaces must be 'Rain_World_Drought.Creatures', not 'Creature' without s, which confuses the compiler with global::Creature
            AbstractCreatureHK.Patch();
            AImapHK.Patch();
            CreatureSymbolHK.Patch();
            CreatureTemplateHK.Patch();
            DeerHK.Patch();
            DevMapPageHK.Patch();
            LizardHK.Patch();
            MultiplayerUnlocksHK.Patch();
            OverseerHK.Patch();
            TrackersHK.Patch(); //WIP
            WorldLoaderHK.Patch();
            #endregion Creatures

            // Effects
            CoralNeuronSystemHK.Patch();
            // RoomSettingsHK.Patch(); // Not needed

            // Patch PlacedObjs
            AbstractPhysicalObjectHK.Patch();

            // Patch Resources
            FutileHK.Patch();

            #region Slugcat
            DreamsStateHK.Patch();
            FoodMeterHK.Patch();
            PlayerGraphicsHK.Patch();
            PlayerHK.Patch();
            SaveStateHK.Patch();
            #endregion Slugcat

            #region OverWorld
            // namespaces must be 'Rain_World_Drought.OverWorld', not 'World', which confuses the compiler with global::World
            AboveCloudsViewHK.Patch();
            ProcessManagerHK.Patch();
            RainCycleHK.Patch();
            RainMeterHK.Patch();
            RainWorldGameHK.Patch();
            RoomHK.Patch();
            RoomRainHK.Patch();
            WorldHK.Patch();
            #endregion OverWorld

            On.RainWorld.Start += new On.RainWorld.hook_Start(RainWorldHK);
        }

        public static bool EnumExt => (int)EnumExt_Drought.LightWorm > 10;

        private static void RainWorldHK(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig.Invoke(self);
            StaticWorldPatch.AddCreatureTemplate();
            StaticWorldPatch.ModifyRelationship();
        }
    }
}
