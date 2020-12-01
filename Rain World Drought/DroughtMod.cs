using Menu;
using Partiality.Modloader;
using Rain_World_Drought.Creatures;
using Rain_World_Drought.Effects;
using Rain_World_Drought.Enums;
using Rain_World_Drought.OverWorld;
using Rain_World_Drought.PlacedObjects;
using Rain_World_Drought.Resource;
using Rain_World_Drought.Slugcat;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rain_World_Drought
{
    public partial class DroughtMod : PartialityMod
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
            ResourceManager.assetDir = string.Concat(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Path.DirectorySeparatorChar, "DroughtAssets", Path.DirectorySeparatorChar);

            // Debugging!
            Debugging.Patch();

            #region Creatures
            // namespaces must be 'Rain_World_Drought.Creatures', not 'Creature' without s, which confuses the compiler with global::Creature
            AbstractCreatureHK.Patch();
            AImapHK.Patch();
            CreatureSymbolHK.Patch();
            CreatureTemplateHK.Patch();
            DeerHK.Patch();
            DevMapPageHK.Patch();
            LizardGraphicsHK.Patch();
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
            PistonPhysics.Patch();

            #region Resource
            // namespaces must be 'Rain_World_Drought.Resource', not 'Resources' with s, which confuses the compiler with UnityEngine.Resources
            //FutileHK.Patch(); // Replaced with ResourceManager
            MenuSceneHK.Patch(); // Check this after CRS implement
            MusicPieceHK.Patch();
            ResourceManager.Patch();
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
            MainLoopProcessHK.Patch();
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

            #region GetLanguage

            // if ConfigMachine is detected, use that

            TextManager.OptionToID = new Dictionary<int, TextManager.LanguageID>()
            {
                { 0 , TextManager.LanguageID.English },
                { 1 , TextManager.LanguageID.French },
                { 2 , TextManager.LanguageID.Italian },
                { 3 , TextManager.LanguageID.German },
                { 4 , TextManager.LanguageID.Spanish },
                { 5 , TextManager.LanguageID.Portuguese },
                { 6 , TextManager.LanguageID.Japanese },
                { 7 , TextManager.LanguageID.Korean }
            };
            if (TextManager.OptionToID.TryGetValue(self.options.language, out TextManager.LanguageID code)) { TextManager.curLang = code; }
            else { TextManager.curLang = TextManager.LanguageID.English; }

            #endregion GetLanguage

            error = "";

            if (!Directory.Exists(ResourceManager.assetDir)) { error = Translate("DroughtAssets folder is missing! Put DroughtAssets with [Rain World Drought.dll]!"); goto handleError; }
            if (!ResourceManager.LoadAtlases()) { error = ResourceManager.error; goto handleError; }
            if (!ResourceManager.LoadSprites()) { error = ResourceManager.error; goto handleError; }
            if (!ResourceManager.CheckDroughtSongs()) { error = ResourceManager.error; goto handleError; }

        handleError:
            if (!EnumExt) { error = Translate("EnumExtender is missing! Download EnumExtender from RainDB - Tools Category."); }
            if (!string.IsNullOrEmpty(error))
            {
                ResourceReady = false;
                On.Menu.MainMenu.ctor += new On.Menu.MainMenu.hook_ctor(DisplayErrorHK);
            }
            else { ResourceReady = true; }

            StaticWorldPatch.AddCreatureTemplate();
            StaticWorldPatch.ModifyRelationship();

            // TextManager.DecryptAllDialogue();
        }

        public static string error;

        private static void DisplayErrorHK(On.Menu.MainMenu.orig_ctor orig, MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            orig.Invoke(self, manager, showRegionSpecificBkg);

            foreach (MenuObject o in self.pages[0].subObjects)
            { if (o is SimpleButton s && s.signalText != "OPTIONS" && s.signalText != "EXIT") { s.buttonBehav.greyedOut = true; } }

            MenuLabel l = new MenuLabel(self, self.pages[0], error, new Vector2(483f, 740f), new Vector2(400f, 15f), false);
            l.label.color = Color.red;
            self.pages[0].subObjects.Add(l);
        }

        /// <summary>
        /// Translate new short strings from Drought
        /// </summary>
        public static string Translate(string text) => TextManager.Translate(text);
    }
}
