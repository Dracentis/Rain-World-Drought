using Menu;
using Rain_World_Drought.Enums;
using Rain_World_Drought.Slugcat;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class RainWorldGameHK
    {
        public static void Patch()
        {
            On.RainWorldGame.ctor += new On.RainWorldGame.hook_ctor(CtorHK);
            On.RainWorldGame.CommunicateWithUpcomingProcess += new On.RainWorldGame.hook_CommunicateWithUpcomingProcess(CommunicateWithUpcomingProcessHK);
            On.RainWorldGame.Win += new On.RainWorldGame.hook_Win(WinHK);
        }

        private static void CtorHK(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
        {
            orig.Invoke(self, manager);
            if (self.IsStorySession)
            {
                if (self.world.GetAbstractRoom(self.Players[0].pos).name == "FS_A01")
                { // in case with jolly coop
                    foreach (AbstractCreature p in self.Players) { p.pos.Tile = new IntVector2(9, 13); }
                }
            }
        }

        private static void CommunicateWithUpcomingProcessHK(On.RainWorldGame.orig_CommunicateWithUpcomingProcess orig, RainWorldGame self, MainLoopProcess nextProcess)
        {
            orig.Invoke(self, nextProcess);
            if (nextProcess is MessageScreen)
            {
                int karma = self.GetStorySession.saveState.deathPersistentSaveData.karma;
                Debug.Log("savKarma: " + karma);
                if (self.sawAGhost > -1)
                {
                    Debug.Log("Ghost end of process stuff");
                    self.manager.CueAchievement(GhostWorldPresence.PassageAchievementID((GhostWorldPresence.GhostID)self.sawAGhost), 2f);
                    if (self.GetStorySession.saveState.deathPersistentSaveData.karmaCap == 8)
                    {
                        self.manager.CueAchievement(RainWorld.AchievementID.AllGhostsEncountered, 10f);
                    }
                    self.GetStorySession.saveState.GhostEncounter(self.sawAGhost, self.rainWorld);
                }
                int num = karma;
                if (nextProcess.ID == ProcessManager.ProcessID.DeathScreen && !self.GetStorySession.saveState.deathPersistentSaveData.reinforcedKarma)
                {
                    num = Custom.IntClamp(num - 1, 0, self.GetStorySession.saveState.deathPersistentSaveData.karmaCap);
                }
                Debug.Log("next screen MAP KARMA: " + num);
                self.cameras[0].hud.map.mapData.UpdateData(self.world, 1 + self.GetStorySession.saveState.deathPersistentSaveData.foodReplenishBonus, num, self.GetStorySession.saveState.deathPersistentSaveData.karmaFlowerPosition, true);
                int num2 = self.Players[0].pos.room;
                Vector2 vector = self.Players[0].pos.Tile.ToVector2() * 20f;
                if (nextProcess.ID == ProcessManager.ProcessID.DeathScreen && self.cameras[0].hud != null && self.cameras[0].hud.textPrompt != null)
                {
                    num2 = self.cameras[0].hud.textPrompt.deathRoom;
                    vector = self.cameras[0].hud.textPrompt.deathPos;
                }
                else if (self.Players[0].realizedCreature != null)
                {
                    vector = self.Players[0].realizedCreature.mainBodyChunk.pos;
                }
                if (self.Players[0].realizedCreature != null && self.Players[0].realizedCreature.room != null && num2 == self.Players[0].realizedCreature.room.abstractRoom.index)
                {
                    vector = Custom.RestrictInRect(vector, self.Players[0].realizedCreature.room.RoomRect.Grow(50f));
                }
                KarmaLadderScreen.SleepDeathScreenDataPackage package = new KarmaLadderScreen.SleepDeathScreenDataPackage((nextProcess.ID != ProcessManager.ProcessID.SleepScreen && nextProcess.ID != ProcessManager.ProcessID.Dream) ? self.cameras[0].hud.textPrompt.foodInStomach : self.GetStorySession.saveState.food, new IntVector2(karma, self.GetStorySession.saveState.deathPersistentSaveData.karmaCap), self.GetStorySession.saveState.deathPersistentSaveData.reinforcedKarma, num2, vector, self.cameras[0].hud.map.mapData, self.GetStorySession.saveState, self.GetStorySession.characterStats, self.GetStorySession.playerSessionRecords[0], self.GetStorySession.saveState.lastMalnourished, self.GetStorySession.saveState.malnourished);
                (nextProcess as MessageScreen).GetDataFromGame(DreamsStateHK.GetEverReadMessage(self.GetStorySession.saveState.dreamsState, EnumSwitch.DreamsStateID.SRSDreamMissonComplete), DreamsStateHK.GetEverReadMessage(self.GetStorySession.saveState.dreamsState, EnumSwitch.DreamsStateID.SRSDreamTraitor), self.GetStorySession.saveState.dreamsState.UpcomingDreamID, package);
            }
        }

        private static void WinHK(On.RainWorldGame.orig_Win orig, RainWorldGame self, bool malnourished)
        {
            if (self.StoryCharacter != WandererSupplement.StoryCharacter) { orig.Invoke(self, malnourished); return; }

            if (self.manager.upcomingProcess != null) { return; }
            Debug.Log("MALNOURISHED: " + malnourished);
            if (!malnourished && !self.rainWorld.saveBackedUp)
            {
                self.rainWorld.saveBackedUp = true;
                self.rainWorld.progression.BackUpSave("_Backup");
            }
            DreamsState dreamsState = self.GetStorySession.saveState.dreamsState;
            if (self.manager.rainWorld.progression.miscProgressionData.starvationTutorialCounter > -1)
            {
                self.manager.rainWorld.progression.miscProgressionData.starvationTutorialCounter++;
            }

            BringStomachUpToDate(self.GetStorySession.saveState, self);
            if (dreamsState != null)
            {
                dreamsState.EndOfCycleProgress(self.GetStorySession.saveState, self.world.region.name, self.world.GetAbstractRoom(self.Players[0].pos).name);
            }
            self.GetStorySession.saveState.SessionEnded(self, true, malnourished);
            if (DreamsStateHK.AnyMessageComingUp(dreamsState))
            {
                self.manager.RequestMainProcessSwitch(EnumExt_Drought.MessageScreen);
            }
            else
            {
                self.manager.RequestMainProcessSwitch((dreamsState == null || !dreamsState.AnyDreamComingUp) ? ProcessManager.ProcessID.SleepScreen : ProcessManager.ProcessID.Dream);
            }
        }

        public static void BringStomachUpToDate(SaveState state, RainWorldGame game)
        {
            bool hasItemInStomach = false;
            for (int i = 0; i < game.session.Players.Count; i++)
            {
                if ((game.session.Players[i].realizedCreature as Player).objectInStomach != null)
                {
                    hasItemInStomach = true;
                    break;
                }
            }
            if (hasItemInStomach)
            {
                state.swallowedItems = new string[game.session.Players.Count];
                for (int j = 0; j < game.session.Players.Count; j++)
                {
                    if ((game.session.Players[j].realizedCreature as Player).objectInStomach != null)
                    {
                        if ((game.session.Players[j].realizedCreature as Player).objectInStomach is AbstractCreature ac)
                        {
                            if (game.world.GetAbstractRoom(ac.pos.room) == null)
                            {
                                ac.pos = (game.session.Players[j].realizedCreature as Player).coord;
                            }
                            state.swallowedItems[j] = SaveState.AbstractCreatureToString(ac);
                        }
                        else
                        {
                            state.swallowedItems[j] = (game.session.Players[j].realizedCreature as Player).objectInStomach.ToString();
                        }
                    }
                    else
                    {
                        state.swallowedItems[j] = "0";
                    }
                }
            }
            else
            {
                state.swallowedItems = null;
            }
        }
    }
}
