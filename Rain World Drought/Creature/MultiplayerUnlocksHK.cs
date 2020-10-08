using Menu;
using Rain_World_Drought.Enums;
using RWCustom;

namespace Rain_World_Drought.Creatures
{
    internal static class MultiplayerUnlocksHK
    {
        public static void Patch()
        {
            MultiplayerUnlocks.CreaturesUnlocks += EnumExt_Unlocks.SandboxUnlockIDExtend;
            On.MultiplayerUnlocks.LevelLockID += new On.MultiplayerUnlocks.hook_LevelLockID(LevelLockIDHK);
            On.Menu.SandboxSettingsInterface.AddScoreButton += new On.Menu.SandboxSettingsInterface.hook_AddScoreButton(SandboxSIAddButtonHK);
            On.Menu.SandboxSettingsInterface.AddScoreButton_1 += new On.Menu.SandboxSettingsInterface.hook_AddScoreButton_1(SandboxSIAddButton1HK);
            On.Menu.SandboxSettingsInterface.DefaultKillScores += new On.Menu.SandboxSettingsInterface.hook_DefaultKillScores(SandboxSIDefaultKillScoreHK);
            On.Menu.SandboxEditorSelector.ctor += new On.Menu.SandboxEditorSelector.hook_ctor(SandboxESCtorHK);
            On.Menu.SandboxEditorSelector.AddButton += new On.Menu.SandboxEditorSelector.hook_AddButton(SandboxESAddButtonHK);
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

        /// <summary>
        /// Removes WalkerBeast score button
        /// </summary>
        private static void SandboxSIAddButtonHK(On.Menu.SandboxSettingsInterface.orig_AddScoreButton orig, SandboxSettingsInterface self,
            MultiplayerUnlocks.SandboxUnlockID unlockID, ref IntVector2 ps)
        {
            if (DroughtMod.EnumExt && unlockID == EnumExt_Unlocks.WalkerBeast) { return; }
            orig.Invoke(self, unlockID, ref ps);
        }

        private static void SandboxSIAddButton1HK(On.Menu.SandboxSettingsInterface.orig_AddScoreButton_1 orig, SandboxSettingsInterface self,
            SandboxSettingsInterface.ScoreController button, ref IntVector2 ps)
        {
            // if (button == null) { return; } // Remove gap between kill score buttons & action score buttons
            IntVector2 psb = ps;
            orig.Invoke(self, button, ref ps);
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
            killScores[(int)EnumExt_Drought.GreyLizard] = 15;
            killScores[(int)EnumExt_Drought.SeaDrake] = 10;
        }

        private static void SandboxESCtorHK(On.Menu.SandboxEditorSelector.orig_ctor orig, SandboxEditorSelector self,
            Menu.Menu menu, MenuObject owner, SandboxOverlayOwner overlayOwner)
        {
            wait = true;
            orig.Invoke(self, menu, owner, overlayOwner);
            wait = false;
            int counter = 3; // clearall + 2 gap
            int crits = MultiplayerUnlocks.CreaturesUnlocks - EnumExt_Unlocks.SandboxUnlockIDExtend;
            // Item
            for (int j = crits; j < crits + MultiplayerUnlocks.ItemsUnlocks; j++)
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
            for (int j = 0; j < crits; j++)
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
            for (int j = crits + MultiplayerUnlocks.ItemsUnlocks; j < crits + MultiplayerUnlocks.ItemsUnlocks + EnumExt_Unlocks.SandboxUnlockIDExtend; j++)
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
                button.RemoveSprites(); // destroy button
                orig.Invoke(self, null, ref counter);
                return;
            }
            orig.Invoke(self, button, ref counter);
        }
    }
}
