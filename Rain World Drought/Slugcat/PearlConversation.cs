using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HUD;
using Rain_World_Drought.Enums;
using Rain_World_Drought.Resource;
using UnityEngine;

namespace Rain_World_Drought.Slugcat
{
    public class PearlConversation : Conversation.IOwnAConversation
    {
        public PearlConversation(Player player)
        {
            this.player = player;
        }

        public bool talking = false;

        public DialogBox dialogBox
        {
            get
            {
                if (currentConversation != null)
                {
                    return currentConversation.dialogBox;
                }
                if (player.room.game.cameras[0].hud.dialogBox == null)
                {
                    player.room.game.cameras[0].hud.InitDialogBox();
                }
                return player.room.game.cameras[0].hud.dialogBox;
            }
        }

        public SLOrcacleState State
        {
            get
            {
                if (player != null && player.room != null && player.room.game != null && player.room.game.session is StoryGameSession)
                {
                    return (player.room.game.session as StoryGameSession).saveState.miscWorldSaveData.SLOracleState;
                }
                else
                {
                    return new SLOrcacleState(true, 0);
                }
            }
        }

        protected string NameForPlayer(bool capitalized)
        {
            string text = (capitalized) ? "Little creature" : "little creature";
            if (UnityEngine.Random.value > 0.3f)
            {
                switch (State.GetOpinion)
                {
                    case SLOrcacleState.PlayerOpinion.Dislikes:
                        text = (capitalized) ? "Little tormentor" : "little tormentor";
                        break;
                    case SLOrcacleState.PlayerOpinion.Likes:
                        if (State.totalPearlsBrought > 10)
                        {
                            text = (capitalized) ? "Little archaeologist" : "little archaeologist";
                        }
                        else
                        {
                            text = (capitalized) ? "Little friend" : "little friend";
                        }
                        break;
                }
            }
            return DroughtMod.Translate(text);
        }

        public void Update(bool eu)
        {
            if (this.player == null || this.player.room == null || this.player.room.abstractRoom == null || this.player.room.abstractRoom.name == null || this.player.room.abstractRoom.name.Substring(0, 2).Equals("SS") || this.player.room.abstractRoom.name.Substring(0, 2).Equals("LM") || this.rainWorld == null || this.rainWorld.progression == null || this.rainWorld.progression.currentSaveState == null || !this.rainWorld.progression.currentSaveState.miscWorldSaveData.moonRevived)
            {
                if (currentConversation != null)
                {
                    currentConversation.Destroy();
                    currentConversation = null;
                    currentPearl = null;
                    talking = false;
                }
                return;
            }

            if (!this.rainWorld.progression.currentSaveState.miscWorldSaveData.pebblesSeenGreenNeuron && this.player.room.abstractRoom.name.Equals("SB_D02"))
            {
                Debug.Log("Moon SI conversation");
                this.rainWorld.progression.currentSaveState.miscWorldSaveData.pebblesSeenGreenNeuron = true;
                currentConversation = new PearlConversation.MoonConversation(Conversation.ID.MoonRecieveSwarmer, this);
                talking = true;
            }

            if (currentConversation != null)
            {
                if (currentConversation.id != Conversation.ID.MoonRecieveSwarmer)
                {
                    player.Stun(40);
                }
                currentConversation.Update();
                if (this.talking == false)
                {
                    currentConversation.Destroy();
                    this.currentConversation = null;
                }
                if (this.player.dead || this.player.dangerGraspTime > 1)
                {
                    currentConversation.Interrupt("...oh no...", 0);
                    currentConversation.Destroy();
                    this.currentConversation = null;
                    talking = false;
                }
            }
            else
            {
                talking = false;
            }
        }

        public void PlayerSwallowItem(PhysicalObject item)
        {
            Debug.LogError("Player Swallow Item");
            if (State.HaveIAlreadyDescribedThisItem(item.abstractPhysicalObject.ID))
            {
                Debug.LogError("Pearl Already read");
                return;
            }
            else
            {
                if (item is DataPearl)
                {
                    if (currentConversation != null)
                    {
                        currentConversation.Interrupt("...", 0);
                        currentConversation.Destroy();
                        currentConversation = null;
                        currentPearl = null;
                    }
                    this.currentPearl = item as DataPearl;
                    State.increaseLikeOnSave = true;
                    State.InfluenceLike(0.1f);
                    Debug.Log((item as DataPearl).AbstractPearl.dataPearlType);
                    if ((item as DataPearl).AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Misc)
                    {
                        currentConversation = new PearlConversation.MoonConversation(Conversation.ID.Moon_Pearl_Misc, this);
                        talking = true;
                    }
                    else if ((item as DataPearl).AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Misc2)
                    {
                        currentConversation = new PearlConversation.MoonConversation(Conversation.ID.Moon_Pearl_Misc2, this);
                        talking = true;
                    }
                    else if ((item as DataPearl).AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.PebblesPearl)
                    {
                        currentConversation = new PearlConversation.MoonConversation(Conversation.ID.Moon_Pebbles_Pearl, this);
                        talking = true;
                    }
                    else
                    {
                        Debug.LogError("Pearl Conversation Started.");
                        Conversation.ID id = Conversation.ID.None;
                        switch (EnumSwitch.GetAbstractDataPearlType((item as DataPearl).AbstractPearl.dataPearlType))
                        {
                            default:
                            case EnumSwitch.AbstractDataPearlType.DEFAULT:
                                switch ((item as DataPearl).AbstractPearl.dataPearlType)
                                {
                                    case DataPearl.AbstractDataPearl.DataPearlType.CC:
                                        this.player.room.game.rainWorld.progression.currentSaveState.miscWorldSaveData.moonRevived = true;
                                        id = Conversation.ID.Moon_Pearl_CC;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SI_west:
                                        id = Conversation.ID.Moon_Pearl_SI_west;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SI_top:
                                        id = Conversation.ID.Moon_Pearl_SI_top;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.LF_west:
                                        id = Conversation.ID.Moon_Pearl_LF_west;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.LF_bottom:
                                        id = Conversation.ID.Moon_Pearl_LF_bottom;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.HI:
                                        id = Conversation.ID.Moon_Pearl_HI;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SH:
                                        id = Conversation.ID.Moon_Pearl_SH;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.DS:
                                        id = Conversation.ID.Moon_Pearl_DS;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SB_filtration:
                                        id = Conversation.ID.Moon_Pearl_SB_filtration;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SB_ravine:
                                        id = Conversation.ID.Moon_Pearl_SB_ravine;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.GW:
                                        id = Conversation.ID.Moon_Pearl_GW;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SL_bridge:
                                        id = Conversation.ID.Moon_Pearl_SL_bridge;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SL_moon:
                                        id = Conversation.ID.Moon_Pearl_SL_moon;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SU:
                                        id = Conversation.ID.Moon_Pearl_SU;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.UW:
                                        id = Conversation.ID.Moon_Pearl_UW;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SL_chimney:
                                        id = Conversation.ID.Moon_Pearl_SL_chimney;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.Red_stomach:
                                        id = Conversation.ID.Moon_Pearl_Red_stomach;
                                        break;
                                }
                                break;

                            case EnumSwitch.AbstractDataPearlType.MoonPearl:
                                id = EnumExt_Drought.Moon_Pearl_MoonPearl; break;
                            case EnumSwitch.AbstractDataPearlType.DroughtPearl1:
                                id = EnumExt_Drought.Moon_Pearl_Drought1; break;
                            case EnumSwitch.AbstractDataPearlType.DroughtPearl2:
                                id = EnumExt_Drought.Moon_Pearl_Drought2; break;
                            case EnumSwitch.AbstractDataPearlType.DroughtPearl3:
                                id = EnumExt_Drought.Moon_Pearl_Drought3; break;
                            case EnumSwitch.AbstractDataPearlType.SI_Spire1:
                                id = EnumExt_Drought.SI_Spire1; break;
                            case EnumSwitch.AbstractDataPearlType.SI_Spire2:
                                id = EnumExt_Drought.SI_Spire2; break;
                            case EnumSwitch.AbstractDataPearlType.SI_Spire3:
                                id = EnumExt_Drought.SI_Spire3; break;
                        }

                        currentConversation = new PearlConversation.MoonConversation(id, this);
                        talking = true;
                        State.significantPearls[(int)(item as DataPearl).AbstractPearl.dataPearlType] = true;
                        State.totalPearlsBrought++;
                        Debug.LogError("Drought) Total pearls brought up: " + State.totalPearlsBrought);
                    }
                    State.totalItemsBrought++;
                    State.AddItemToAlreadyTalkedAbout(item.abstractPhysicalObject.ID);
                }
            }
        }

        public string ReplaceParts(string s)
        {
            s = Regex.Replace(s, "<PLAYERNAME>", NameForPlayer(false));
            s = Regex.Replace(s, "<CAPPLAYERNAME>", NameForPlayer(true));
            s = Regex.Replace(s, "<PlayerName>", NameForPlayer(false));
            s = Regex.Replace(s, "<CapPlayerName>", NameForPlayer(true));
            return s;
        }

        public string Translate(string s)
        {
            return ReplaceParts(this.player.room.game.rainWorld.inGameTranslator.Translate(s));
        }

        public RainWorld rainWorld
        {
            get
            {
                return player.room.game.rainWorld;
            }
        }

        public void SpecialEvent(string eventName)
        {
        }

        public MoonConversation currentConversation;

        public DataPearl currentPearl;

        private Player player;

        public class MoonConversation : Conversation
        {
            public virtual void Update()
            {
                if (this.paused)
                {
                    return;
                }
                if (this.events.Count == 0)
                {
                    this.Destroy();
                    this.pearlConversation.currentPearl = null;
                    this.pearlConversation.talking = false;
                    this.pearlConversation = null;
                }
                else
                {
                    this.events[0].Update();
                    if (this.events[0].IsOver)
                    {
                        this.events.RemoveAt(0);
                    }
                }
            }

            public MoonConversation(Conversation.ID id, PearlConversation pearlConversation) : base(pearlConversation, id, pearlConversation.dialogBox)
            {
                this.pearlConversation = pearlConversation;
                this.id = id;
                AddEvents();
            }

            public SLOrcacleState State
            {
                get
                {
                    return pearlConversation.State;
                }
            }

            public string Translate(string s)
            {
                return pearlConversation.Translate(s);
            }

            public override void AddEvents()
            {
                Debug.Log(id.ToString() + " " + State.neuronsLeft);
                switch (EnumSwitch.GetConversationID(id))
                {
                    default:
                    case EnumSwitch.ConversationID.DEFAULT:
                        switch (id)
                        {
                            case ID.Moon_Pearl_Misc:
                                PearlIntro();
                                MiscPearl(false);
                                break;
                            case ID.MoonRecieveSwarmer:
                                events.Add(new Conversation.TextEvent(this, 50, DroughtMod.Translate("You're reaching the end of your journey, little friend."), 40));
                                events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Thank you, for helping me."), 10));
                                break;
                            case ID.Moon_Pearl_Misc2:
                                PearlIntro();
                                MiscPearl(true);
                                break;
                            case ID.Moon_Pebbles_Pearl:
                                PebblesPearl();
                                break;
                            case ID.Moon_Pearl_CC:
                                State.InfluenceLike(1f);
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_CC);
                                break;
                            case ID.Moon_Pearl_SI_west:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SI_west);
                                break;
                            case ID.Moon_Pearl_SI_top:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SI_top);
                                break;
                            case ID.Moon_Pearl_LF_west:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_LF_west);
                                break;
                            case ID.Moon_Pearl_LF_bottom:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_LF_bottom);
                                break;
                            case ID.Moon_Pearl_HI:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_HI);
                                break;
                            case ID.Moon_Pearl_SH:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SH);
                                break;
                            case ID.Moon_Pearl_DS:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_DS);
                                break;
                            case ID.Moon_Pearl_SB_filtration:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SB_filtration);
                                break;
                            case ID.Moon_Pearl_GW:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_GW);
                                break;
                            case ID.Moon_Pearl_SL_bridge:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SL_bridge);
                                break;
                            case ID.Moon_Pearl_SL_moon:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SL_moon);
                                break;
                            case ID.Moon_Pearl_SU:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SU);
                                break;
                            case ID.Moon_Pearl_SB_ravine:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SB_ravine);
                                break;
                            case ID.Moon_Pearl_UW:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_UW);
                                break;
                            case ID.Moon_Pearl_SL_chimney:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_SL_chimney);
                                break;
                        }
                        break;

                    case EnumSwitch.ConversationID.Moon_Pearl_Drought1:// IS
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_Drought1_IS); break;
                    case EnumSwitch.ConversationID.Moon_Pearl_Drought2:// FS
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_Drought2_FS); break;
                    case EnumSwitch.ConversationID.Moon_Pearl_Drought3:// MW
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_Moon_Pearl_Drought3_MW); break;
                    case EnumSwitch.ConversationID.SI_Spire1:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_SI_Spire1); break;
                    case EnumSwitch.ConversationID.SI_Spire2:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_SI_Spire2); break;
                    case EnumSwitch.ConversationID.SI_Spire3:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_SI_Spire3); break;
                }
            }

            private void PearlIntro()
            {
                switch (State.totalPearlsBrought + State.miscPearlCounter)
                {  // vanilla dialogues, so use vanilla InGameTranslator
                    case 0:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Hello, <PlayerName>!"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Would you like me to read this?"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("It's a bit dusty, but I will do my best. Hold on..."), 10));
                        break;
                    case 1:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Oh...") + " " + Translate("Hello again, <PlayerName>!"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Another pearl! You want me to read this one too? Just a moment..."), 10));
                        // [Another pearl! Just a moment...] is not vanilla dilaogue, and I doubt this minor tweak is worth translating the same sentence 8 times
                        break;
                    case 2:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("And yet another one! I will read it to you."), 10));
                        break;
                    case 3:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Another? You're no better than the scavengers!"), 10));
                        if (State.GetOpinion == SLOrcacleState.PlayerOpinion.Likes)
                        {
                            events.Add(new Conversation.TextEvent(this, 0, Translate("Let us see... to be honest, I'm as curious to see it as you are."), 10));
                        }
                        break;
                    default:
                        switch (UnityEngine.Random.Range(0, 5))
                        {
                            case 0:
                                break;
                            case 1:
                                events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                                events.Add(new Conversation.TextEvent(this, 0, Translate("The scavengers must be jealous of you, finding all these") + ".", 10));
                                break;
                            case 2:
                                events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                                events.Add(new Conversation.TextEvent(this, 0, Translate("Here we go again, little archeologist. Let's read your pearl."), 10));
                                break;
                            case 3:
                                events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                                events.Add(new Conversation.TextEvent(this, 0, Translate("... You're getting quite good at this you know. A little archeologist beast.<LINE>Now, let's see what it says."), 10));
                                break;
                            default:
                                events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                                events.Add(new Conversation.TextEvent(this, 0, Translate("And yet another one! I will read it to you."), 10));
                                break;
                        }
                        break;
                }
            }

            private void MiscPearl(bool miscPearl2)
            {
                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_MiscPearl, true, (pearlConversation.currentPearl == null) ? UnityEngine.Random.Range(0, 100000) : pearlConversation.currentPearl.abstractPhysicalObject.ID.RandomSeed);
                State.miscPearlCounter++;
            }

            private void PebblesPearl()
            {
                switch (UnityEngine.Random.Range(0, 5))
                { // vanilla dialogues, so use vanilla InGameTranslator
                    case 0:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Hello, <PlayerName>!"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("You would like me to read this?"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("It's still warm... this was in use recently."), 10));
                        break;
                    case 1:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("A pearl... This one is crystal clear - it was used just recently."), 10));
                        break;
                    case 2:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Would you like me to read this pearl?"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Strange... it seems to have been used not too long ago."), 10));
                        break;
                    case 3:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("This pearl has been written to just now!"), 10));
                        break;
                    default:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Let's see... A pearl..."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("And this one is fresh! It was not long ago this data was written to it!"), 10));
                        break;
                }
                TextManager.LoadEventsFromFile(this, TextManager.EventID.PC_PebblesPearl, true, (pearlConversation.currentPearl == null) ? UnityEngine.Random.Range(0, 100000) : pearlConversation.currentPearl.abstractPhysicalObject.ID.RandomSeed);
            }

            public PearlConversation pearlConversation;
        }
    }
}
