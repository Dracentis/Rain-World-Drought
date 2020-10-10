using Menu;
using Partiality.Modloader;
using Rain_World_Drought.Creatures;
using Rain_World_Drought.Effects;
using Rain_World_Drought.Enums;
using Rain_World_Drought.OverWorld;
using Rain_World_Drought.PlacedObjects;
using Rain_World_Drought.Resource;
using Rain_World_Drought.Slugcat;
using System;
using System.IO;
using UnityEngine;

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
            ResourceReady = false;

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
            AbstractPhysicalObjectHK.Patch(); // + RedLight
            DataPearlHK.Patch();

            #region Resource
            //FutileHK.Patch(); // Replaced with ResourceManager
            MenuSceneHK.Patch(); // Check this after CRS implement
            MusicPieceHK.Patch();
            TextManager.Patch();
            #endregion Resource

            #region Slugcat
            DreamsStateHK.Patch();
            FoodMeterHK.Patch();
            PlayerGraphicsHK.Patch();
            PlayerHK.Patch();
            SaveStateHK.Patch(); // + MiscWorldSaveData
            SlugcatSelectMenuHK.Patch(); // Check this after CRS implement
            #endregion Slugcat

            #region OverWorld
            // namespaces must be 'Rain_World_Drought.OverWorld', not 'World', which confuses the compiler with global::World
            AboveCloudsViewHK.Patch();
            GlobalRainHK.Patch();
            OracleGraphicsHK.Patch();
            OracleHK.Patch();
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
        public static bool ResourceReady;
        public static bool Enabled => EnumExt && ResourceReady;

        private static void RainWorldHK(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig.Invoke(self);

            ResourceManager.assetDir = string.Concat(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Path.DirectorySeparatorChar, "Language");
            error = "";

            bool check = ResourceManager.LoadAtlases();
            if (!check) { error = ResourceManager.error; goto handleError; }

        handleError:
            if (!EnumExt) { error = "EnumExtender is missing! Download EnumExtender from RainDB - Tools Category."; }
            if (!string.IsNullOrEmpty(error))
            {
                ResourceReady = false;
                On.Menu.MainMenu.ctor += new On.Menu.MainMenu.hook_ctor(DisplayErrorHK);
            }
            else { ResourceReady = true; }

            StaticWorldPatch.AddCreatureTemplate();
            StaticWorldPatch.ModifyRelationship();
        }

        public static string error;

        private static void DisplayErrorHK(On.Menu.MainMenu.orig_ctor orig, MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            orig.Invoke(self, manager, showRegionSpecificBkg);

            foreach (MenuObject o in self.pages[0].subObjects)
            { if (o is SimpleButton s && s.signalText == "SINGLE PLAYER") { s.buttonBehav.greyedOut = true; } }

            MenuLabel l = new MenuLabel(self, self.pages[0], error, new Vector2(483f, 740f), new Vector2(400f, 15f), false);
            l.label.color = Color.red;
            self.pages[0].subObjects.Add(l);
        }

        /// <summary>
        /// Translate newly added dialogue from Drought
        /// </summary>
        public static string Translate(string text) => TextManager.Translate(text);
    }
}
