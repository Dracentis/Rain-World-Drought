using System;
using MonoMod;
using UnityEngine;
using System.Runtime.CompilerServices;
using RWCustom;
using Menu;

public class patch_RainWorldGame : RainWorldGame
{
    [MonoModIgnore]
    public ShortcutHandler shortcuts { get; private set; }

    [MonoModIgnore]
    public RoomCamera[] cameras { get; private set; }

    [MonoModIgnore]
    public patch_RainWorldGame(ProcessManager manager) : base(manager)
    {
    }
    
    public extern void orig_ctor(ProcessManager manager);

    [MonoModConstructor]
    public void ctor(ProcessManager manager)
    {
        orig_ctor(manager);
        //Delegate to call the base constructor
        //Type[] constructorSignature = new Type[2];
        //constructorSignature[0] = typeof(ProcessManager);
        //constructorSignature[1] = typeof(ProcessManager.ProcessID);
        //RuntimeMethodHandle handle = typeof(MainLoopProcess).GetConstructor(constructorSignature).MethodHandle;
        //RuntimeHelpers.PrepareMethod(handle);
        //IntPtr ptr = handle.GetFunctionPointer();
        //Action<ProcessManager, ProcessManager.ProcessID> funct = (Action<ProcessManager, ProcessManager.ProcessID>)Activator.CreateInstance(typeof(Action<ProcessManager, ProcessManager.ProcessID>), this, ptr);
        //funct(manager, ProcessManager.ProcessID.Game);//MainLoopProcess Constructor
        
        if (this.IsStorySession)
        {
            if (world.GetAbstractRoom(Players[0].pos).name == "FS_A01")
            {
                Players[0].pos.Tile = new IntVector2(9, 13);
            }
        }



    }



    public extern void orig_CommunicateWithUpcomingProcess(MainLoopProcess nextProcess);

    public override void CommunicateWithUpcomingProcess(MainLoopProcess nextProcess)
    {
        orig_CommunicateWithUpcomingProcess(nextProcess);
        if (nextProcess is MessageScreen)
        {
            int karma = this.GetStorySession.saveState.deathPersistentSaveData.karma;
            Debug.Log("savKarma: " + karma);
            if (this.sawAGhost > -1)
            {
                Debug.Log("Ghost end of process stuff");
                this.manager.CueAchievement(GhostWorldPresence.PassageAchievementID((GhostWorldPresence.GhostID)this.sawAGhost), 2f);
                if (this.GetStorySession.saveState.deathPersistentSaveData.karmaCap == 8)
                {
                    this.manager.CueAchievement(RainWorld.AchievementID.AllGhostsEncountered, 10f);
                }
                this.GetStorySession.saveState.GhostEncounter(this.sawAGhost, this.rainWorld);
            }
            int num = karma;
            if (nextProcess.ID == ProcessManager.ProcessID.DeathScreen && !this.GetStorySession.saveState.deathPersistentSaveData.reinforcedKarma)
            {
                num = Custom.IntClamp(num - 1, 0, this.GetStorySession.saveState.deathPersistentSaveData.karmaCap);
            }
            Debug.Log("next screen MAP KARMA: " + num);
            this.cameras[0].hud.map.mapData.UpdateData(this.world, 1 + this.GetStorySession.saveState.deathPersistentSaveData.foodReplenishBonus, num, this.GetStorySession.saveState.deathPersistentSaveData.karmaFlowerPosition, true);
            int num2 = this.Players[0].pos.room;
            Vector2 vector = this.Players[0].pos.Tile.ToVector2() * 20f;
            if (nextProcess.ID == ProcessManager.ProcessID.DeathScreen && this.cameras[0].hud != null && this.cameras[0].hud.textPrompt != null)
            {
                num2 = this.cameras[0].hud.textPrompt.deathRoom;
                vector = this.cameras[0].hud.textPrompt.deathPos;
            }
            else if (this.Players[0].realizedCreature != null)
            {
                vector = this.Players[0].realizedCreature.mainBodyChunk.pos;
            }
            if (this.Players[0].realizedCreature != null && this.Players[0].realizedCreature.room != null && num2 == this.Players[0].realizedCreature.room.abstractRoom.index)
            {
                vector = Custom.RestrictInRect(vector, this.Players[0].realizedCreature.room.RoomRect.Grow(50f));
            }
            KarmaLadderScreen.SleepDeathScreenDataPackage package = new KarmaLadderScreen.SleepDeathScreenDataPackage((nextProcess.ID != ProcessManager.ProcessID.SleepScreen && nextProcess.ID != ProcessManager.ProcessID.Dream) ? this.cameras[0].hud.textPrompt.foodInStomach : this.GetStorySession.saveState.food, new IntVector2(karma, this.GetStorySession.saveState.deathPersistentSaveData.karmaCap), this.GetStorySession.saveState.deathPersistentSaveData.reinforcedKarma, num2, vector, this.cameras[0].hud.map.mapData, this.GetStorySession.saveState, this.GetStorySession.characterStats, this.GetStorySession.playerSessionRecords[0], this.GetStorySession.saveState.lastMalnourished, this.GetStorySession.saveState.malnourished);
            (nextProcess as MessageScreen).GetDataFromGame(((this.GetStorySession.saveState.dreamsState as patch_DreamsState).everSeenMissonComplete), ((this.GetStorySession.saveState.dreamsState as patch_DreamsState).everSeenTraitor), this.GetStorySession.saveState.dreamsState.UpcomingDreamID, package);
        }
    }

    public void ExitToVoidSeaSlideShow()
    {
        this.GetStorySession.saveState.deathPersistentSaveData.ascended = true;
        if (this.StoryCharacter == 2)
        {
            this.GetStorySession.saveState.AppendCycleToStatistics(this.Players[0].realizedCreature as Player, this.GetStorySession, true);
            this.GetStorySession.saveState.deathPersistentSaveData.redsDeath = true;
            this.manager.rainWorld.progression.SaveWorldStateAndProgression(false);
        }
        this.manager.rainWorld.progression.miscProgressionData.redUnlocked = true;
        this.ExitGame(false, false);
        if (this.StoryCharacter == 1)
        {
            this.manager.nextSlideshow = SlideShow.SlideShowID.YellowOutro;
        }
        else if (this.StoryCharacter == 2)
        {
            this.manager.nextSlideshow = SlideShow.SlideShowID.RedOutro;
        }
        else
        {
            this.manager.nextSlideshow = (SlideShow.SlideShowID)patch_SlideShow.SlideShowID.WhiteOutro;
        }
        this.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);
    }

    public void Win(bool malnourished)
    {
        if (this.manager.upcomingProcess != null)
        {
            return;
        }
        Debug.Log("MALNOURISHED: " + malnourished);
        if (!malnourished && !this.rainWorld.saveBackedUp)
        {
            this.rainWorld.saveBackedUp = true;
            this.rainWorld.progression.BackUpSave("_Backup");
        }
        DreamsState dreamsState = this.GetStorySession.saveState.dreamsState;
        if (this.manager.rainWorld.progression.miscProgressionData.starvationTutorialCounter > -1)
        {
            this.manager.rainWorld.progression.miscProgressionData.starvationTutorialCounter++;
        }
        if (this.StoryCharacter != 0)
        {
            if (this.GetStorySession.saveState.miscWorldSaveData.EverMetMoon)
            {
                if (!this.GetStorySession.lastEverMetMoon)
                {
                    this.manager.CueAchievement((this.GetStorySession.saveState.miscWorldSaveData.SLOracleState.neuronsLeft != 5) ? RainWorld.AchievementID.MoonEncounterBad : RainWorld.AchievementID.MoonEncounterGood, 5f);
                    if (dreamsState != null)
                    {
                        dreamsState.InitiateEventDream((this.GetStorySession.saveState.miscWorldSaveData.SLOracleState.neuronsLeft != 5) ? DreamsState.DreamID.MoonThief : DreamsState.DreamID.MoonFriend);
                    }
                }
                else if (dreamsState != null && !dreamsState.everAteMoonNeuron && this.GetStorySession.saveState.miscWorldSaveData.SLOracleState.neuronsLeft < 5)
                {
                    dreamsState.InitiateEventDream(DreamsState.DreamID.MoonThief);
                }
            }
            if (!this.GetStorySession.lastEverMetPebbles && this.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad > 0)
            {
                this.manager.CueAchievement(RainWorld.AchievementID.PebblesEncounter, 5f);
                if (this.StoryCharacter == 2)
                {
                    this.manager.rainWorld.progression.miscProgressionData.redHasVisitedPebbles = true;
                }
                if (dreamsState != null)
                {
                    dreamsState.InitiateEventDream(DreamsState.DreamID.Pebbles);
                }
            }
        }
        (this.GetStorySession.saveState as patch_SaveState).BringStomachUpToDate(this);
        if (dreamsState != null)
        {
            dreamsState.EndOfCycleProgress(this.GetStorySession.saveState, this.world.region.name, this.world.GetAbstractRoom(this.Players[0].pos).name);
        }
        this.GetStorySession.saveState.SessionEnded(this, true, malnourished);
        if ((dreamsState as patch_DreamsState).AnyMessageComingUp)
        {
            this.manager.RequestMainProcessSwitch((ProcessManager.ProcessID)patch_ProcessManager.ProcessID.MessageScreen);
        }
        else
        {
            this.manager.RequestMainProcessSwitch((dreamsState == null || !dreamsState.AnyDreamComingUp) ? ProcessManager.ProcessID.SleepScreen : ProcessManager.ProcessID.Dream);
        }
        
    }
}


