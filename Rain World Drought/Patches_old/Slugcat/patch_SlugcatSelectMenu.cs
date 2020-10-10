using System;
using MonoMod;
using UnityEngine;
using System.Runtime.CompilerServices;
using Menu;
using HUD;
using RWCustom;

[MonoModPatch("global::Menu.SlugcatSelectMenu")]
public class patch_SlugcatSelectMenu : SlugcatSelectMenu
{
    [MonoModIgnore]
    private bool restartChecked;
    [MonoModIgnore]
    private SaveState redSaveState;
    [MonoModIgnore]
    private void UpdateStartButtonText() { }
    [MonoModIgnore]
    private void UpdateSelectedSlugcatInMiscProg() { }


    [MonoModIgnore]
    public patch_SlugcatSelectMenu(ProcessManager manager) : base(manager)
    {
    }

    //Link to method in the base of inherited class
    //[MonoModLinkTo("System.Void Menu.SlugcatSelectMenu::PlaySound(soundID)")]
    //public static void base_PlaySound(SoundID soundID)
    //{
    //}

    //Link to method in the base of the inherited class
    //[MonoModLinkTo("System.String Menu.SlugcatSelectMenu::Translate(s)")]
    //public static String base_Translate(String s)
    //{
    //    return s;
    //}

    public extern void orig_ctor(ProcessManager manager);

    [MonoModConstructor]
    public void ctor(ProcessManager manager)
    {
        //Delegate to call the base constructor
        Type[] constructorSignature = new Type[2];
        constructorSignature[0] = typeof(ProcessManager);
        constructorSignature[1] = typeof(ProcessManager.ProcessID);
        RuntimeMethodHandle handle = typeof(Menu.Menu).GetConstructor(constructorSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action<ProcessManager, ProcessManager.ProcessID> funct = (Action<ProcessManager, ProcessManager.ProcessID>)Activator.CreateInstance(typeof(Action<ProcessManager, ProcessManager.ProcessID>), this, ptr);
        funct(manager, ProcessManager.ProcessID.SlugcatSelect);//Menu.Menu Constructor


        pages.Add(new Menu.Page(this, null, "main", 0));
        if (!manager.rainWorld.flatIllustrations)
        {
            rainEffect = new Menu.RainEffect(this, pages[0]);
            pages[0].subObjects.Add(rainEffect);
        }
        if (CheckUnlockRed())
        {
            manager.rainWorld.progression.miscProgressionData.redUnlocked = true;
        }
        slugcatColorOrder = new int[]
        {
                1,
                0,
                2
        };
        for (int i = 0; i < slugcatColorOrder.Length; i++)
        {
            if (slugcatColorOrder[i] == manager.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat)
            {
                slugcatPageIndex = i;
            }
        }
        slugcatPageIndex = 1;
        saveGameData = new Menu.SlugcatSelectMenu.SaveGameData[3];
        for (int j = 0; j < 3; j++)
        {
            saveGameData[j] = MineForSaveData(manager, slugcatColorOrder[j]);
        }
        if (saveGameData[2] != null && ((saveGameData[2].redsDeath && saveGameData[2].cycle >= RedsIllness.RedsCycles(saveGameData[2].redsExtraCycles)) || saveGameData[2].ascended))
        {
            redIsDead = true;
        }
        int num = 0;
        for (int k = 0; k < 3; k++)
        {
            if (saveGameData[k] != null)
            {
                num++;
            }
        }
        if (num == 1)
        {
            for (int l = 0; l < 3; l++)
            {
                if (saveGameData[l] != null)
                {
                    slugcatPageIndex = l;
                    break;
                }
            }
        }
        slugcatPages = new Menu.SlugcatSelectMenu.SlugcatPage[3];
        for (int m = 0; m < slugcatPages.Length; m++)
        {
            if (saveGameData[m] != null)
            {
                slugcatPages[m] = new Menu.SlugcatSelectMenu.SlugcatPageContinue(this, null, 1 + m, slugcatColorOrder[m]);
            }
            else
            {
                slugcatPages[m] = new Menu.SlugcatSelectMenu.SlugcatPageNewGame(this, null, 1 + m, slugcatColorOrder[m]);
            }
            pages.Add(slugcatPages[m]);
        }
        startButton = new Menu.HoldButton(this, pages[0], string.Empty, "START", new Vector2(683f, 85f), 40f);
        pages[0].subObjects.Add(startButton);
        pages[0].subObjects.Add(new Menu.SimpleButton(this, pages[0], "BACK", "BACK", new Vector2(200f, 668f), new Vector2(110f, 30f)));
        //this.pages[0].subObjects.Add(new Menu.BigArrowButton(this, this.pages[0], "PREV", new Vector2(200f, 50f), -1));
        //this.pages[0].subObjects.Add(new Menu.BigArrowButton(this, this.pages[0], "NEXT", new Vector2(1116f, 50f), 1));
        //Removed access to other characters
        float textWidth = 85f;
        restartCheckbox = new Menu.CheckBox(this, pages[0], this, new Vector2(startButton.pos.x + 200f, 30f), textWidth, "Restart game", "RESTART");
        pages[0].subObjects.Add(restartCheckbox);
        UpdateStartButtonText();
        UpdateSelectedSlugcatInMiscProg();
        mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
    }

    private void StartGame(int storyGameCharacter)
    {
        if (manager.rainWorld.progression.gameTinkeredWith)
        {
            return;
        }
        manager.arenaSitting = null;
        manager.rainWorld.progression.currentSaveState = null;
        manager.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat = storyGameCharacter;
        if (!restartChecked && manager.rainWorld.progression.IsThereASavedGame(storyGameCharacter))
        {
            if (storyGameCharacter == 2 && redIsDead)
            {
                redSaveState = manager.rainWorld.progression.GetOrInitiateSaveState(2, null, manager.menuSetup, false);
                manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Statistics);
                PlaySound(SoundID.MENU_Switch_Page_Out);
            }
            else
            {
                manager.menuSetup.startGameCondition = ProcessManager.MenuSetup.StoryGameInitCondition.Load;
                manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Game);
                PlaySound(SoundID.MENU_Continue_Game);
            }
        }
        else
        {
            manager.rainWorld.progression.WipeSaveState(storyGameCharacter);
            manager.menuSetup.startGameCondition = ProcessManager.MenuSetup.StoryGameInitCondition.New;
            manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Game);
            PlaySound(SoundID.MENU_Start_New_Game);
        }
    }

    public class patch_SlugcatPageNewGame : SlugcatPageNewGame
    {
        [MonoModIgnore]
        public patch_SlugcatPageNewGame(Menu.Menu menu, Menu.MenuObject owner, int pageIndex, int slugcatNumber) : base(menu, owner, pageIndex, slugcatNumber)
        {
        }

        public extern void orig_ctor(Menu.Menu menu, Menu.MenuObject owner, int pageIndex, int slugcatNumber);

        [MonoModConstructor]
        public void ctor(Menu.Menu menu, Menu.MenuObject owner, int pageIndex, int slugcatNumber)
        {
            string text = string.Empty;
            string text2 = string.Empty;

            orig_ctor(menu, owner, pageIndex, slugcatNumber);

            text = menu.Translate("THE WANDERER");
            text2 = menu.Translate("Curious and calm, with a deep desire to discover the ancient mysteries around it.<LINE>In tune with the events of the world, your journey will have a significant impact on things much greater than yourself.");

            difficultyLabel = new Menu.MenuLabel(menu, this, text, new Vector2(-1000f, imagePos.y - 249f), new Vector2(200f, 30f), true);
            difficultyLabel.label.alignment = FLabelAlignment.Center;
            subObjects.Add(difficultyLabel);
            text2 = text2.Replace("<LINE>", Environment.NewLine);
            infoLabel = new Menu.MenuLabel(menu, this, text2, new Vector2(-1000f, imagePos.y - 249f - 40f), new Vector2(200f, 30f), false);
            infoLabel.label.alignment = FLabelAlignment.Center;
            subObjects.Add(infoLabel);
            difficultyLabel.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);
            infoLabel.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
        }



    }

    public class patch_SlugcatPageContinue : SlugcatPageContinue
    {
        [MonoMod.MonoModIgnore]
        public patch_SlugcatPageContinue(Menu.Menu menu, MenuObject owner, int pageIndex, int slugcatNumber) : base(menu, owner, pageIndex, slugcatNumber)
        {
        }

        public extern void orig_ctor(Menu.Menu menu, MenuObject owner, int pageIndex, int slugcatNumber);

        //This constructor seems a bit messy, but it is the only way I could get it to work since calling base.AddImage(bool) was a nightmare
        [MonoModConstructor]
        public void ctor(Menu.Menu menu, MenuObject owner, int pageIndex, int slugcatNumber)
        {
            orig_ctor(menu, owner, pageIndex, slugcatNumber);
            hud.ClearAllSprites();
            hudContainers = new FContainer[2];
            for (int i = 0; i < hudContainers.Length; i++)
            {
                hudContainers[i] = new FContainer();
                Container.AddChild(hudContainers[i]);
            }
            hud = new HUD.HUD(hudContainers, menu.manager.rainWorld, this);
            saveGameData.karma = Custom.IntClamp(saveGameData.karma, 0, saveGameData.karmaCap);
            saveGameData.food = Custom.IntClamp(saveGameData.food, 0, SlugcatStats.SlugcatFoodMeter(slugcatNumber).y);
            hud.AddPart(new KarmaMeter(hud, hudContainers[1], new IntVector2(saveGameData.karma, saveGameData.karmaCap), saveGameData.karmaReinforced));
            hud.AddPart(new FoodMeter(hud, SlugcatStats.SlugcatFoodMeter(slugcatNumber).x, SlugcatStats.SlugcatFoodMeter(slugcatNumber).y));
            string text = String.Empty;
            if (saveGameData.shelterName != null && saveGameData.shelterName.Length > 2)
            {
                string text2 = saveGameData.shelterName.Substring(0, 2);
                switch (text2)
                {
                    case "CC":
                        text = "Chimney Canopy";
                        break;
                    case "DS":
                        text = "Drainage System";
                        break;
                    case "HI":
                        text = "Industrial Complex";
                        break;
                    case "GW":
                        text = "Garbage Wastes";
                        break;
                    case "SI":
                        text = "Sky Islands";
                        break;
                    case "SU":
                        text = "Outskirts";
                        break;
                    case "SH":
                        text = "Shaded Citadel";
                        break;
                    case "IS":
                        text = "Intake System";
                        break;
                    case "SL":
                        text = "Shoreline";
                        break;
                    case "LF":
                        text = "Farm Arrays";
                        break;
                    case "UW":
                        text = "The Exterior";
                        break;
                    case "SB":
                        text = "Subterranean";
                        break;
                    case "SS":
                        text = "Five Pebbles";
                        break;
                    case "LM":
                        text = "Looks To the Moon";
                        break;
                    case "MW":
                        text = "The Fragmented Exterior";
                        break;
                    case "FS":
                        text = "Forest Sanctuary";
                        break;
                }
                if (text.Length > 0)
                {
                    text2 = text;
                    text = string.Concat(new object[]
                    {
                                text2,
                                " - ",
                                menu.Translate("Cycle"),
                                " ",
                                (slugcatNumber != 2) ? saveGameData.cycle : (RedsIllness.RedsCycles(saveGameData.redsExtraCycles) - saveGameData.cycle)
                    });
                }
            }
            regionLabel = new MenuLabel(menu, this, text, new Vector2(-1000f, imagePos.y - 249f), new Vector2(200f, 30f), true);
            regionLabel.label.alignment = FLabelAlignment.Center;
            subObjects.Add(regionLabel);
        }

        [MonoModIgnore]
        private HUD.HUD hud;
    }
}


