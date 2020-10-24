using Menu;
using Rain_World_Drought.Enums;
using RWCustom;
using System;
using System.Collections.Generic;

namespace Rain_World_Drought.Creatures
{
    internal static class MultiplayerUnlocksHK
    {
        public static void Patch()
        {
            //MultiplayerUnlocks.CreaturesUnlocks += EnumSwitch.SandboxUnlockIDExtend;
            On.MultiplayerUnlocks.ctor += new On.MultiplayerUnlocks.hook_ctor(MUCtorHK);
            On.MultiplayerUnlocks.LevelLockID += new On.MultiplayerUnlocks.hook_LevelLockID(LevelLockIDHK);
            On.MultiplayerUnlocks.SymbolDataForSandboxUnlock += new On.MultiplayerUnlocks.hook_SymbolDataForSandboxUnlock(MUSymbolDataForSandboxUnlockHK);
            On.Menu.SandboxSettingsInterface.ctor += new On.Menu.SandboxSettingsInterface.hook_ctor(SandboxSICtorHK);
            On.Menu.SandboxSettingsInterface.AddScoreButton += new On.Menu.SandboxSettingsInterface.hook_AddScoreButton(SandboxSIAddButtonHK);
            On.Menu.SandboxSettingsInterface.AddScoreButton_1 += new On.Menu.SandboxSettingsInterface.hook_AddScoreButton_1(SandboxSIAddButton1HK);
            On.Menu.SandboxSettingsInterface.DefaultKillScores += new On.Menu.SandboxSettingsInterface.hook_DefaultKillScores(SandboxSIDefaultKillScoreHK);
            On.Menu.SandboxEditorSelector.ctor += new On.Menu.SandboxEditorSelector.hook_ctor(SandboxESCtorHK);
            On.Menu.SandboxEditorSelector.AddButton += new On.Menu.SandboxEditorSelector.hook_AddButton(SandboxESAddButtonHK);
        }

        #region MultiplayerUnlocks

        private static void MUCtorHK(On.MultiplayerUnlocks.orig_ctor orig, MultiplayerUnlocks self, PlayerProgression progression, List<string> allLevels)
        {
            orig.Invoke(self, progression, allLevels);
            for (int l = MultiplayerUnlocks.CreaturesUnlocks + MultiplayerUnlocks.ItemsUnlocks; l < MultiplayerUnlocks.CreaturesUnlocks + MultiplayerUnlocks.ItemsUnlocks + EnumSwitch.SandboxUnlockIDExtend; l++)
            {
                if (self.SandboxItemUnlocked((MultiplayerUnlocks.SandboxUnlockID)l))
                {
                    self.creaturesUnlockedForLevelSpawn[(int)MultiplayerUnlocks.SymbolDataForSandboxUnlock((MultiplayerUnlocks.SandboxUnlockID)l).critType] = true;
                }
            }
        }

        private static MultiplayerUnlocks.LevelUnlockID LevelLockIDHK(On.MultiplayerUnlocks.orig_LevelLockID orig, string levelName)
        {
            switch (levelName)
            {
                case "Valves":
                    if (!DroughtMod.EnumExt) { return MultiplayerUnlocks.LevelUnlockID.Hidden; }
                    return EnumExt_Unlocks.IS;
                case "Temple":
                    if (!DroughtMod.EnumExt) { return MultiplayerUnlocks.LevelUnlockID.Hidden; }
                    return EnumExt_Unlocks.FS;
                case "Hub":
                    return MultiplayerUnlocks.LevelUnlockID.Hidden;
                default:
                    return orig.Invoke(levelName);
            }
        }

        private static IconSymbol.IconSymbolData MUSymbolDataForSandboxUnlockHK(On.MultiplayerUnlocks.orig_SymbolDataForSandboxUnlock orig, MultiplayerUnlocks.SandboxUnlockID unlockID)
        {
            if ((int)unlockID >= MultiplayerUnlocks.CreaturesUnlocks + MultiplayerUnlocks.ItemsUnlocks && (int)unlockID < MultiplayerUnlocks.CreaturesUnlocks + MultiplayerUnlocks.ItemsUnlocks + EnumSwitch.SandboxUnlockIDExtend)
            {
                return new IconSymbol.IconSymbolData(Custom.ParseEnum<CreatureTemplate.Type>(unlockID.ToString()), AbstractPhysicalObject.AbstractObjectType.Creature, 0);
            }
            return orig.Invoke(unlockID);
        }

        #endregion MultiplayerUnlocks

        #region SandboxSettingsInterface

        private static void SandboxSICtorHK(On.Menu.SandboxSettingsInterface.orig_ctor orig, SandboxSettingsInterface self,
            Menu.Menu menu, MenuObject owner)
        {
            backupCritUnlk = MultiplayerUnlocks.CreaturesUnlocks;
            MultiplayerUnlocks.CreaturesUnlocks += EnumSwitch.SandboxUnlockIDExtend + MultiplayerUnlocks.ItemsUnlocks;
            orig.Invoke(self, menu, owner);
            MultiplayerUnlocks.CreaturesUnlocks = backupCritUnlk;
        }

        public static int backupCritUnlk;

        /// <summary>
        /// Removes WalkerBeast score button
        /// </summary>
        private static void SandboxSIAddButtonHK(On.Menu.SandboxSettingsInterface.orig_AddScoreButton orig, SandboxSettingsInterface self,
            MultiplayerUnlocks.SandboxUnlockID unlockID, ref IntVector2 ps)
        {
            if ((int)unlockID >= backupCritUnlk && (int)unlockID < backupCritUnlk + MultiplayerUnlocks.ItemsUnlocks) { return; }
            if (unlockID == EnumExt_Unlocks.WalkerBeast) { return; }
            orig.Invoke(self, unlockID, ref ps);
        }

        private static void SandboxSIAddButton1HK(On.Menu.SandboxSettingsInterface.orig_AddScoreButton_1 orig, SandboxSettingsInterface self,
            SandboxSettingsInterface.ScoreController button, ref IntVector2 ps)
        {
            // if (button == null) { return; } // Remove gap between kill score buttons & action score buttons
            IntVector2 psb = ps;
            orig.Invoke(self, button, ref ps);
            psb.y++;
            if (psb.y > 9) // extend the row by 1
            {
                psb.y = 0; psb.x++;
            }
            ps = psb;
        }

        private static void SandboxSIDefaultKillScoreHK(On.Menu.SandboxSettingsInterface.orig_DefaultKillScores orig, ref int[] killScores)
        {
            orig.Invoke(ref killScores);
            if (!DroughtMod.EnumExt) { return; }
            Array.Resize(ref killScores, killScores.Length + EnumSwitch.SandboxUnlockIDExtend);
            killScores[(int)EnumExt_Drought.GreyLizard] = 15;
            killScores[(int)EnumExt_Drought.SeaDrake] = 10;
        }

        #endregion SandboxSettingsInterface

        #region SandboxEditorSelector

        private static void SandboxESCtorHK(On.Menu.SandboxEditorSelector.orig_ctor orig, SandboxEditorSelector self,
            Menu.Menu menu, MenuObject owner, SandboxOverlayOwner overlayOwner)
        {
            wait = true;
            orig.Invoke(self, menu, owner, overlayOwner);
            wait = false;
            int counter = 3; // clearall + 2 gap
            // Item
            for (int j = MultiplayerUnlocks.CreaturesUnlocks; j < MultiplayerUnlocks.CreaturesUnlocks + MultiplayerUnlocks.ItemsUnlocks; j++)
            {
                if (self.unlocks.SandboxItemUnlocked((MultiplayerUnlocks.SandboxUnlockID)j))
                {
                    self.AddButton(new SandboxEditorSelector.CreatureOrItemButton(menu, self, MultiplayerUnlocks.SymbolDataForSandboxUnlock((MultiplayerUnlocks.SandboxUnlockID)j)), ref counter);
                }
                else
                {
                    self.AddButton(new SandboxEditorSelector.LockedButton(menu, self), ref counter);
                }
            }
            // Non-Drought Creatures
            for (int j = 0; j < MultiplayerUnlocks.CreaturesUnlocks; j++)
            {
                if (self.unlocks.SandboxItemUnlocked((MultiplayerUnlocks.SandboxUnlockID)j))
                {
                    self.AddButton(new SandboxEditorSelector.CreatureOrItemButton(menu, self, MultiplayerUnlocks.SymbolDataForSandboxUnlock((MultiplayerUnlocks.SandboxUnlockID)j)), ref counter);
                }
                else
                {
                    self.AddButton(new SandboxEditorSelector.LockedButton(menu, self), ref counter);
                }
            }
            // Drought Creatures
            for (int j = MultiplayerUnlocks.CreaturesUnlocks + MultiplayerUnlocks.ItemsUnlocks; j < MultiplayerUnlocks.CreaturesUnlocks + MultiplayerUnlocks.ItemsUnlocks + EnumSwitch.SandboxUnlockIDExtend; j++)
            {
                if (self.unlocks.SandboxItemUnlocked((MultiplayerUnlocks.SandboxUnlockID)j))
                {
                    self.AddButton(new SandboxEditorSelector.CreatureOrItemButton(menu, self, MultiplayerUnlocks.SymbolDataForSandboxUnlock((MultiplayerUnlocks.SandboxUnlockID)j)), ref counter);
                }
                else
                {
                    self.AddButton(new SandboxEditorSelector.LockedButton(menu, self), ref counter);
                }
            }
            // Init
            for (int l = 0; l < SandboxEditorSelector.Width; l++)
            {
                for (int m = 0; m < SandboxEditorSelector.Height; m++)
                {
                    if (self.buttons[l, m] != null)
                    {
                        if (self.buttons[l, m].rightDivider != null) { continue; } // Already Initiated
                        self.buttons[l, m].Initiate(new IntVector2(l, m));
                    }
                }
            }
        }

        public static bool wait;

        private static void SandboxESAddButtonHK(On.Menu.SandboxEditorSelector.orig_AddButton orig, SandboxEditorSelector self,
            SandboxEditorSelector.Button button, ref int counter)
        {
            if (wait)
            {
                if (button != null) { button.RemoveSprites(); } // destroy button
                orig.Invoke(self, null, ref counter);
                return;
            }
            orig.Invoke(self, button, ref counter);
        }

        #endregion SandboxEditorSelector
    }
}
