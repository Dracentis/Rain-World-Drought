using HUD;
using Menu;
using System.Text.RegularExpressions;
using UnityEngine;
using System;
using Rain_World_Drought.Enums;
using Rain_World_Drought.Resource;

namespace Rain_World_Drought.Slugcat
{
    public class MessageScreen : DreamScreen, IOwnAHUD, Conversation.IOwnAConversation
    {
        public MessageScreen(ProcessManager manager) : base(manager)
        {
            this.hudContainers = new MenuContainer[2];
            for (int j = 0; j < 2; j++)
            {
                this.hudContainers[j] = new MenuContainer(this, this.pages[0], new Vector2(0f, 0f));
                this.pages[0].subObjects.Add(this.hudContainers[j]);
            }
        }

        public string ReplaceParts(string s)
        {
            s = Regex.Replace(s, "<PLAYERNAME>", this.NameForPlayer(false));
            s = Regex.Replace(s, "<CAPPLAYERNAME>", this.NameForPlayer(true));
            s = Regex.Replace(s, "<PlayerName>", this.NameForPlayer(false));
            s = Regex.Replace(s, "<CapPlayerName>", this.NameForPlayer(true));
            s = Regex.Replace(s, "<playername>", this.NameForPlayer(false));
            s = Regex.Replace(s, "<capplayername>", this.NameForPlayer(true));
            return s;
        }

        public string Translate(string s)
        {
            return this.ReplaceParts(s);
        }

        public RainWorld rainWorld { get { return this.manager.rainWorld; } }
        private bool SRSLikesPlayer = false;
        private bool SRSHatesPlayer = false;
        private SRSConversation conversation;

        public void SpecialEvent(String eventName)
        {
        }

        private string NameForPlayer(bool caps)
        {
            string text;
            if (SRSLikesPlayer)
            { text = (caps ? "My little friend" : "my little friend"); }
            else if (SRSHatesPlayer)
            { text = (caps ? "Traitor" : "traitor"); }
            else { text = (caps ? "Messenger" : "messenger"); }
            return DroughtMod.Translate(text);
        }

        public int endTime;

        public void GetDataFromGame(bool seenMissonComplete, bool seenTraitor, DreamsState.DreamID dreamID, KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            SRSLikesPlayer = seenMissonComplete;
            SRSHatesPlayer = seenTraitor;
            base.GetDataFromGame(dreamID, package);
            this.endTime = 340;
            this.hud = new HUD.HUD(new FContainer[]
            {
                this.hudContainers[1].Container,
                this.hudContainers[0].Container
            }, this.manager.rainWorld, this);

            if (this.hud.dialogBox == null)
            {
                Debug.Log("DialogBox Init!!!");
                this.hud.InitDialogBox();
            }
            conversation = new SRSConversation(this, Conversation.ID.None, this.hud.dialogBox, (DreamsState.DreamID)this.dreamID);
        }

        public override void Update()
        {
            if (this.hud != null)
            {
                this.hud.Update();
            }
            if (this.conversation != null)
            {
                this.conversation.Update();
            }
            base.Update();
        }

        public override void GrafUpdate(float timeStacker)
        {
            if (this.hud != null)
            {
                this.hud.Draw(timeStacker);
            }
            base.GrafUpdate(timeStacker);
        }

        public void PlayHUDSound(SoundID soundID)
        {
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
            if (this.hud != null && this.hud.map != null)
            {
                this.hud.map.ClearSprites();
            }
        }

        public bool MapDiscoveryActive
        {
            get
            {
                return false;
            }
        }

        public Player.InputPackage MapInput
        {
            get
            {
                return RWInput.PlayerInput(0, this.hud.rainWorld.options, this.hud.rainWorld.setup);
            }
        }

        public bool RevealMap
        {
            get
            {
                return false;
            }
        }

        public HUD.HUD.OwnerType GetOwnerType()
        {
            return EnumExt_Drought.DreamScreen;
        }

        public int CurrentFood
        {
            get
            {
                return 0;
            }
        }

        public Vector2 MapOwnerInRoomPosition
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        public int MapOwnerRoom
        {
            get
            {
                return 0;
            }
        }

        public void FoodCountDownDone()
        {
        }

        public HUD.HUD hud;
        private MenuContainer[] hudContainers;

        public void NewMessage(string text, float xOrientation, float yPos, int extraLinger)
        {
            if (hud != null)
            {
                if (this.hud.dialogBox == null)
                {
                    Debug.Log("DialogBox Init!");
                    this.hud.InitDialogBox();
                }
                Debug.Log("New Message: " + text);
                this.hud.dialogBox.NewMessage(text, xOrientation, yPos, extraLinger);
            }
            else
            {
                Debug.LogError("hud object is null!");
            }
        }

        public void NewMessage(string text, int extraLinger)
        {
            if (hud != null)
            {
                if (this.hud.dialogBox == null)
                {
                    Debug.Log("DialogBox Init!");
                    this.hud.InitDialogBox();
                }
                Debug.Log("New Message: " + text);
                this.hud.dialogBox.NewMessage(text, extraLinger);
            }
            else
            {
                Debug.LogError("hud object is null!");
            }
        }

        public class SRSConversation : Conversation
        {
            public SRSConversation(IOwnAConversation interfaceOwner, ID id, DialogBox dialogBox, DreamsState.DreamID dreamid) : base(interfaceOwner, id, dialogBox)
            {
                this.dreamID = dreamid;
                //(this.interfaceOwner as MessageScreen).NewMessage("AAAAAAAAAAAAAAAAAAAAA", 500);
                this.AddEvents();
                if (this.events.Count >= 1)
                {
                    this.events[0].initialWait = 150;
                }
                int tempEndTime = 300;
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i] is TextEvent)
                    {
                        tempEndTime = tempEndTime + events[i].initialWait + (events[i] as TextEvent).textLinger;
                    }
                    else
                    {
                        tempEndTime = tempEndTime + events[i].initialWait;
                    }
                }
                if (tempEndTime < 320)
                {
                    tempEndTime = 340;
                }
                (this.interfaceOwner as MessageScreen).endTime = tempEndTime;
            }

            private DreamsState.DreamID dreamID;

            private void AddEvents()
            {
                switch (EnumSwitch.GetDreamsStateID(dreamID))
                {
                    case EnumSwitch.DreamsStateID.SRSDreamPearlLF:
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("The absurd amount of properness they spoke with still amuses me."), 0));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlLF2:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlLF2);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlHI:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlHI);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSH:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlSH);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlDS:
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("There was an eternal dilemma to them - they were burdened by great ambition,<LINE>yet deeply convinced that striving in itself was an unforgivable vice."), 60));
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("They tried very hard to be effortless. Perhaps that's what we were to them,<LINE>someone to delegate that unrestrained effort to."), 40));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSB:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlSB);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSB2:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlSB2);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlGW:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlGW);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSL:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlSL);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSL2:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlSL2);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSL3:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlSL3);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI:
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("We are all aware of the futility of our Task.<LINE>Even the most dedicated iterators out there have felt frustration over our seemingly pointless efforts."), 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("It's not like we have anything else we could do. It's either iterate or do nothing."), 10));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI2:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlSI2);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI3:
                        //REMOVE SI3
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI4:
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Looks to the Moon and Five Pebbles were once very talkative iterators."), 0));
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("They were a lot closer than many iterators believed.<LINE>By now they've grown distant, both feeling weary with The Task."), 0));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI5:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlSI5);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSU:
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Death isn't the end - birth and death are connected to each other like a ring,<LINE>or some say a spiral. Some say a spiral that in turn forms a ring. Some ramble in agonizing longevity.<LINE>But the basis is agreed upon: like sleep like death, you wake up again - whether you want to or not."), 60));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlUW:
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("The relations between iterators and their populations were strained in the cycles leading up to global ascension."), 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Many people were disrespectful and some of the iterators returned that disrespect."), 15));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlIS:
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("There are still many things that I have yet to learn about my creator's past."), 10));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlFS:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamPearlFS);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlMW:
                        this.events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("You should return this to Looks to the Moon."), 0));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamTraitor:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamTraitor);
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamMissonComplete:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.SRSDreamMissonComplete);
                        break;
                }
            }

            public void Update()
            {
                if (this.paused)
                {
                    return;
                }
                if (this.events.Count == 0)
                {
                    this.Destroy();
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
        }
    }
}
