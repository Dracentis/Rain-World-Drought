using HUD;
using Menu;
using System.Text.RegularExpressions;
using UnityEngine;
using System;
using Rain_World_Drought.Enums;

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
            if (SRSLikesPlayer)
            {
                return (caps ? "My little friend" : "my little friend");
            }
            else if (SRSHatesPlayer)
            {
                return (caps ? "Traitor" : "traitor");
            }
            return (caps ? "Messenger" : "messenger");
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
                { // Attach Translator here!
                    case EnumSwitch.DreamsStateID.SRSDreamPearlLF:
                        this.events.Add(new Conversation.TextEvent(this, 0, "The absurd amount of properness they spoke with still amuses me.", 0));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlLF2:
                        this.events.Add(new Conversation.TextEvent(this, 0, "Four Cabinets, Eleven Hatchets...", 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, "They were a resplendent figure towards those seeking transcendence.", 0));
                        this.events.Add(new Conversation.TextEvent(this, 0, "I still wonder if there is any truth to his words.", 10));
                        this.events.Add(new Conversation.TextEvent(this, 0, "All living entities have an inheriet understanding of the cycle. Afterall, it's what your experiencing right now, <PlayerName>.", 50));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlHI:
                        this.events.Add(new Conversation.TextEvent(this, 0, "Side House was one of the first religious sites incorporated into Five Pebbles' design.", 0));
                        this.events.Add(new Conversation.TextEvent(this, 0, "The only bone masks that remain are occasionally collected by scavengers. You might have seen one, <PlayerName>.", 10));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Eventually, some of the sites in the citadel were also connected to Five Pebbles.<LINE>There was a lot of backlash during Five Pebbles’ construction.", 20));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSH:
                        this.events.Add(new Conversation.TextEvent(this, 0, "There is a long held tradition to record every significant event in a persons life before <LINE>ascension, as a final farewell to the life they are leaving behind.", 50));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Some iterators groups found this resource essential in their endeavors towards solving The Task. They came to call themselves Regeneraists.", 30));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Nothing much as come out of it.", 5));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlDS:
                        this.events.Add(new Conversation.TextEvent(this, 0, "There was an eternal dilemma to them - they were burdened by great ambition,<LINE>yet deeply convinced that striving in itself was an unforgivable vice.", 60));
                        this.events.Add(new Conversation.TextEvent(this, 0, "They tried very hard to be effortless. Perhaps that's what we were to them,<LINE>someone to delegate that unrestrained effort to. ", 40));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSB:
                        this.events.Add(new Conversation.TextEvent(this, 0, "Void Fluid is one of the driving forces that allowed for our construction.", 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Without the nearly infinite amount of energy and curiosity that it produced; we might not have ever existed. <LINE>We certainly wouldn't last this long on our own.", 40));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Now, the idea of fixing a broken piece of equipment or deeply integrated machinery is nearly impossible.<LINE>Most iterators have started a slow decay.", 35));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSB2:
                        this.events.Add(new Conversation.TextEvent(this, 0, "Everyone was frustrated with the final task they left us.", 5));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Some groups descended into anger and confusion,<LINE>clamoring insults and forming vehement tribes.", 10));
                        this.events.Add(new Conversation.TextEvent(this, 0, "The incident with Silver Of Straw didn't help at all.", 5));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlGW:
                        this.events.Add(new Conversation.TextEvent(this, 0, "What will come of this world?", 0));
                        this.events.Add(new Conversation.TextEvent(this, 0, "With our creators long gone, it's hard to look into the future.<LINE>Many of us just remain tirelessly at work on a task that is unlikely to help anyone.", 40));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Perhaps when one of us has finally solved it, we'll spread it<LINE>among the scavengers and other creatures like you, <PlayerName>.", 30));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Or when we inevitably fall into complete deterioration, another group will rise to inherit the world.", 20));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSL:
                        this.events.Add(new Conversation.TextEvent(this, 0, "We were viewed as the paragon of technology.<LINE>They had finally found someone to delegate that unrestrained effort to.", 25));
                        this.events.Add(new Conversation.TextEvent(this, 0, "In many ways, we did exactly that. During our operation, <LINE>we took up more and more tasks to maintain the cities on our cans.", 25));
                        this.events.Add(new Conversation.TextEvent(this, 0, "They forced their way around so many problems during our construction. Structures that<LINE> run from deep in the earth to towering above the clouds.", 30));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSL2:
                        this.events.Add(new Conversation.TextEvent(this, 0, "Silver Of Straw's fate was a mystery for a very long time.", 10));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Many iterators tried to simulate her demise with the data collected from overseers and her last transmissions.", 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Considering my location relatively close to her, I play a large part in these efforts.", 15));
                        this.events.Add(new Conversation.TextEvent(this, 0, "It wasn't until we were all but disconnected that I found a way to probe deeper into the complex.", 17));
                        this.events.Add(new Conversation.TextEvent(this, 0, "I lost a few of my creations in there, <PlayerName>.", 10));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSL3:
                        this.events.Add(new Conversation.TextEvent(this, 0, "The development of purposed organisms was a major accomplishment that allowed for our creation.", 16));
                        this.events.Add(new Conversation.TextEvent(this, 0, "We are designed with many smaller organic parts to handle<LINE>processes and adapt to different situations.", 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Our creators didn't want us to manipulate our organic components, since <LINE> this could pose serious danger to the entire city and the iterator involved.<LINE>They created certain taboos hard coded into our genome. One of which prevented the manipulation of our genome.", 50));
                        this.events.Add(new Conversation.TextEvent(this, 0, "For this reason, it is very difficult for iterators to experiment with purposed organisms.", 15));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Some iterators took on case studies on the local fauna, acquiring outside organic matter<LINE>for limited experiments. I was the first to break the taboo and create some organisms of my own.", 25));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI:
                        this.events.Add(new Conversation.TextEvent(this, 0, "We are all aware of the futility of our Task.<LINE>Even the most dedicated iterators out there have felt frustration over our seemingly pointless efforts.", 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, "It's not like we have anything else we could do. It's either iterate or do nothing.", 10));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI2:
                        this.events.Add(new Conversation.TextEvent(this, 0, "The Self-Destruction Taboo is one of few taboos that the majority of the iterator population agrees upon.", 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, "When some iterators suggested that Silver Of Straw destroyed<LINE>herself intentionally, many iterators blatantly dismissed it.", 25));
                        this.events.Add(new Conversation.TextEvent(this, 0, "I personally don’t see taboos as such sacred rules, and I believe<LINE>that breaking a few of them is the only way to find a solution to The Task.", 25));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI3:
                        //REMOVE SI3
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI4:
                        this.events.Add(new Conversation.TextEvent(this, 0, "Looks to the Moon and Five Pebbles were once very talkative iterators. ", 0));
                        this.events.Add(new Conversation.TextEvent(this, 0, "They were a lot closer than many iterators believed.<LINE>By now they've grown distant, both feeling weary with The Task.", 0));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSI5:
                        this.events.Add(new Conversation.TextEvent(this, 0, "The development of purposed organisms was a major accomplishment that allowed for our creation.", 16));
                        this.events.Add(new Conversation.TextEvent(this, 0, "We were designed with many smaller organic parts to handle<LINE>processes and adapt to different situations.", 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Our creators didn't want us to manipulate our organic components, since <LINE> this could pose serious danger to the entire city and the iterator involved.<LINE>They created certain taboos hard coded into our genome. One of which prevented the manipulation of our genome.", 50));
                        this.events.Add(new Conversation.TextEvent(this, 0, "For this reason, it is very difficult for iterators to experiment with purposed organisms.", 15));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Some iterators took on case studies on the local fauna, acquiring outside organic matter<LINE>for limited experiments. I was the first to break the taboo and create some organisms of my own.", 25));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlSU:
                        this.events.Add(new Conversation.TextEvent(this, 0, "Death isn't the end - birth and death are connected to each other like a ring,<LINE>or some say a spiral. Some say a spiral that in turn forms a ring. Some ramble in agonizing longevity.<LINE>But the basis is agreed upon: like sleep like death, you wake up again - whether you want to or not.", 60));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlUW:
                        this.events.Add(new Conversation.TextEvent(this, 0, "The relations between iterators and their populations were strained in the cycles leading up to global ascension.", 20));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Many people were disrespectful and some of the iterators returned that disrespect.", 15));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlIS:
                        this.events.Add(new Conversation.TextEvent(this, 0, "There are still many things that I have yet to learn about my creator's past.", 10));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlFS:
                        this.events.Add(new Conversation.TextEvent(this, 0, "NSH had some odd ideas related to the task.", 6));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Sometimes it's hard to take him seriously, but they do put forward some valuable points.", 8));
                        this.events.Add(new Conversation.TextEvent(this, 0, "I based alot of my research off his discoveries.", 6));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamPearlMW:
                        this.events.Add(new Conversation.TextEvent(this, 0, "You should return this to Looks to the Moon.", 0));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamTraitor:
                        this.events.Add(new Conversation.TextEvent(this, 0, "...", 30));
                        this.events.Add(new Conversation.TextEvent(this, 0, "I don't know what to say.", 30));
                        this.events.Add(new Conversation.TextEvent(this, 0, "How could you do this to me?", 30));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Do not speak with me again, <PlayerName>.", 30));
                        break;
                    case EnumSwitch.DreamsStateID.SRSDreamMissonComplete:
                        this.events.Add(new Conversation.TextEvent(this, 0, "<CapPlayerName>, Thank you!", 10));
                        this.events.Add(new Conversation.TextEvent(this, 0, "You have helped me and Five Pebbles' in a very significant way.", 15));
                        this.events.Add(new Conversation.TextEvent(this, 0, "You are free to go, as Five Pebbles' told you. The old path should serve you well.", 15));
                        this.events.Add(new Conversation.TextEvent(this, 0, "There's a temple deep beneath Five Pebbles' farm arrays. Go down as deep and you can go.", 10));
                        this.events.Add(new Conversation.TextEvent(this, 0, "Farewell, <PlayerName>.", 0));
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
