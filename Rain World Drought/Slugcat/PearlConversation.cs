using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HUD;
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
            return text;
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
                            case (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.MoonPearl:
                                id = (Conversation.ID)patch_Conversation.ID.Moon_Pearl_MoonPearl;
                                break;
                            case (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.DroughtPearl1:
                                id = (Conversation.ID)patch_Conversation.ID.Moon_Pearl_Drought1;
                                break;
                            case (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.DroughtPearl2:
                                id = (Conversation.ID)patch_Conversation.ID.Moon_Pearl_Drought2;
                                break;
                            case (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.DroughtPearl3:
                                id = (Conversation.ID)patch_Conversation.ID.Moon_Pearl_Drought3;
                                break;
                            case (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SI_Spire1:
                                id = (Conversation.ID)patch_Conversation.ID.SI_Spire1;
                                break;
                            case (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SI_Spire2:
                                id = (Conversation.ID)patch_Conversation.ID.SI_Spire2;
                                break;
                            case (DataPearl.AbstractDataPearl.DataPearlType)patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SI_Spire3:
                                id = (Conversation.ID)patch_Conversation.ID.SI_Spire3;
                                break;
                        }
                        currentConversation = new PearlConversation.MoonConversation(id, this);
                        talking = true;
                        State.significantPearls[(int)(item as DataPearl).AbstractPearl.dataPearlType] = true;
                        State.totalPearlsBrought++;
                        Debug.LogError("pearls brought up: " + State.totalPearlsBrought);
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
                switch (id)
                {
                    case ID.Moon_Pearl_Misc:
                        PearlIntro();
                        MiscPearl(false);
                        break;
                    case ID.MoonRecieveSwarmer:
                        events.Add(new Conversation.TextEvent(this, 50, Translate("You're reaching the end of your journey, little friend."), 40));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Thank you, for helping me."), 10));
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
                        LoadEventsFromFile(7);
                        break;
                    case ID.Moon_Pearl_SI_west:
                        PearlIntro();
                        LoadEventsFromFile(20);
                        break;
                    case ID.Moon_Pearl_SI_top:
                        PearlIntro();
                        LoadEventsFromFile(21);
                        break;
                    case ID.Moon_Pearl_LF_west:
                        PearlIntro();
                        LoadEventsFromFile(10);
                        break;
                    case ID.Moon_Pearl_LF_bottom:
                        PearlIntro();
                        LoadEventsFromFile(11);
                        break;
                    case ID.Moon_Pearl_HI:
                        PearlIntro();
                        LoadEventsFromFile(12);
                        break;
                    case ID.Moon_Pearl_SH:
                        PearlIntro();
                        LoadEventsFromFile(13);
                        break;
                    case ID.Moon_Pearl_DS:
                        PearlIntro();
                        LoadEventsFromFile(14);
                        break;
                    case ID.Moon_Pearl_SB_filtration:
                        PearlIntro();
                        LoadEventsFromFile(15);
                        break;
                    case ID.Moon_Pearl_GW:
                        PearlIntro();
                        LoadEventsFromFile(16);
                        break;
                    case ID.Moon_Pearl_SL_bridge:
                        PearlIntro();
                        LoadEventsFromFile(17);
                        break;
                    case ID.Moon_Pearl_SL_moon:
                        PearlIntro();
                        LoadEventsFromFile(18);
                        break;
                    case ID.Moon_Pearl_SU:
                        PearlIntro();
                        LoadEventsFromFile(41);
                        break;
                    case ID.Moon_Pearl_SB_ravine:
                        PearlIntro();
                        LoadEventsFromFile(43);
                        break;
                    case ID.Moon_Pearl_UW:
                        PearlIntro();
                        LoadEventsFromFile(42);
                        break;
                    case ID.Moon_Pearl_SL_chimney:
                        PearlIntro();
                        LoadEventsFromFile(54);
                        break;
                    case (ID)patch_Conversation.ID.Moon_Pearl_Drought1:// IS
                        events.Add(new Conversation.TextEvent(this, 0, Translate("It's an old pearl related to our local intake system."), 3));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Specifically, it details the designed microorganisms used in the main reservoir."), 5));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("The complete report is thousands of entries long, discussing everything from the <LINE>central gravity-amplifier-style intake solution to the interactions with the developed fauna."), 18));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I\'ll give you the gist of it."), 2));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Back when the major cities were closer to the surface, filtered water reservoirs<LINE>were created as bioreactors to purify water and remove unwanted contaminants."), 16));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("The organisms used in these tanks were not very well controlled and many different<LINE>unwanted strains of filter tissue corrupted early tanks."), 14));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Some strains were even observed blending with the natural fauna, creating extremely<LINE>resilient organisms of a very large scale."), 12));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I hope you didn’t go down there to get this, I’m not sure how the tank would react to your biology."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I suppose the mark Suns\' gave you would help."), 3));
                        break;
                    case (ID)patch_Conversation.ID.Moon_Pearl_Drought2:// FS
                        events.Add(new Conversation.TextEvent(this, 0, Translate("This is..."), 3));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("..."), 5));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("...It has data written on it to be sure, but the format is nothing like I’ve ever seen before."), 6));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("It bears some resemblance to the creature logs created by the overseers."), 5));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Most iterators simply ignore it, as it has little importance towards the Task."), 6));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I occasionally look back on those files, as it reminds me of the time back when I worked on the<LINE>tasks of an entire lively city. It was a much more complex time; I worked on everything<LINE>from coordinating farming systems to individuals’ problems or inquiries. "), 20));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I do miss the days when they visited my chamber."), 3));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Anyways, back to the pearl. It appears that someone used that data format as the basis to create a<LINE>new log of individual creatures’ karmic properties and social relationships."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("There is a section written in internal language as well. I can try my best to translate it."), 6));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("\"...organizations of the fourth axiom of karmic staging, It can be ascertained that the quintessence of<LINE>an organic body can never be entirely sequestered from the continuous flow of related<LINE>materials. I propose a contemporary project to aggregate coordinated karmic<LINE>networking\" or perhaps... \"encompassment of an overall site\""), 25));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I am sorry little creature, are you understanding this?"), 3));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I would say this is the remnants or perhaps an older log of a far-reaching project by one of the iterators in the local group."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("It is hard to say which iterator, although some of the internal language suggests a slightly cynical<LINE>or humorous tone, which could narrow the possiblities."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Without knowing where it came from, I’m afraid I can’t tell you much more."), 3));
                        break;
                    case (ID)patch_Conversation.ID.Moon_Pearl_Drought3:// MW
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Oh, wow! I didn\'t think I would ever see this again!"), 3));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("It is an old qualia that I recorded from one of the sky-sail journeys long ago."), 4));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I lost it back when one of my capacitor coils burst a few thousand cycle ago. Secondary systems and my biological<LINE>components did their best to bring it back online, but the damage shut down a few disruptors<LINE>leading to some more serious collapses."), 13));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("I do not remember much of the subject, but this pearl always held an importance to me."), 5));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Thank You, <PlayerName>"), 0));
                        break;
                    case (ID)patch_Conversation.ID.SI_Spire1:
                        LoadEventsFromFile(22);
                        break;
                    case (ID)patch_Conversation.ID.SI_Spire2:
                        LoadEventsFromFile(23);
                        break;
                    case (ID)patch_Conversation.ID.SI_Spire3:
                        LoadEventsFromFile(24);
                        break;
                }
            }

            private void PearlIntro()
            {
                switch (State.totalPearlsBrought + State.miscPearlCounter)
                {
                    case 0:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Hello <PlayerName>!"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Would you like me to read this?"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("It's a bit dusty, but I will do my best. Hold on..."), 10));
                        break;
                    case 1:
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Oh.. Hello again, <PlayerName>!"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, Translate("Another pearl! Just a moment..."), 10));
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
                                events.Add(new Conversation.TextEvent(this, 0, Translate("The scavengers must be jealous of you, finding all these"), 10));
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
                LoadEventsFromFile(38, true, (pearlConversation.currentPearl == null) ? UnityEngine.Random.Range(0, 100000) : pearlConversation.currentPearl.abstractPhysicalObject.ID.RandomSeed);
                State.miscPearlCounter++;
            }

            private void PebblesPearl()
            {
                switch (UnityEngine.Random.Range(0, 5))
                {
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
                LoadEventsFromFile(40, true, (pearlConversation.currentPearl == null) ? UnityEngine.Random.Range(0, 100000) : pearlConversation.currentPearl.abstractPhysicalObject.ID.RandomSeed);
            }

            public PearlConversation pearlConversation;
        }
    }
}
