using Menu;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rain_World_Drought.Slugcat
{
    internal static class SlugcatSelectMenuHK
    {
        public static void Patch()
        {
            On.Menu.SlugcatSelectMenu.ctor += new On.Menu.SlugcatSelectMenu.hook_ctor(CtorHK);
            On.Menu.SlugcatSelectMenu.StartGame += new On.Menu.SlugcatSelectMenu.hook_StartGame(StartGameHK);
            On.Menu.SlugcatSelectMenu.SlugcatPageNewGame.ctor += new On.Menu.SlugcatSelectMenu.SlugcatPageNewGame.hook_ctor(PageNewGameHK);
            On.Menu.SlugcatSelectMenu.SlugcatPageContinue.ctor += new On.Menu.SlugcatSelectMenu.SlugcatPageContinue.hook_ctor(PageContinueHK);
        }

        private static void CtorHK(On.Menu.SlugcatSelectMenu.orig_ctor orig, SlugcatSelectMenu self, ProcessManager manager)
        {
            orig.Invoke(self, manager);

            BigArrowButton b = FindButton(self.pages[0].subObjects, "PREV");
            if (b != null) { b.RemoveSprites(); self.pages[0].RemoveSubObject(b); }
            b = FindButton(self.pages[0].subObjects, "NEXT");
            if (b != null) { b.RemoveSprites(); self.pages[0].RemoveSubObject(b); }
        }

        public static BigArrowButton FindButton(List<MenuObject> subObjs, string signalText)
        {
            foreach (MenuObject o in subObjs)
            { if (o is BigArrowButton s && s.signalText == signalText) { return s; } } //Debug.Log($"Removing {signalText} button");
            return null;
        }

        private static void StartGameHK(On.Menu.SlugcatSelectMenu.orig_StartGame orig, SlugcatSelectMenu self, int storyGameCharacter)
        { // Skip Intro
            if (storyGameCharacter != WandererSupplement.StoryCharacter)
            { orig.Invoke(self, storyGameCharacter); return; } // Not Wanderer
            // if (self.manager.rainWorld.progression.gameTinkeredWith) { return; }

            if (!self.restartChecked && self.manager.rainWorld.progression.IsThereASavedGame(storyGameCharacter))
            { orig.Invoke(self, storyGameCharacter); return; } // Continue Game
            // Init
            self.manager.arenaSitting = null;
            self.manager.rainWorld.progression.currentSaveState = null;
            self.manager.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat = storyGameCharacter;
            // Start New Game
            self.manager.rainWorld.progression.WipeSaveState(storyGameCharacter);
            self.manager.menuSetup.startGameCondition = ProcessManager.MenuSetup.StoryGameInitCondition.New;
            self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Game);
            // self.manager.nextSlideshow = SlideShow.SlideShowID.YellowIntro; // Input.GetKey("s")
            // self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);
            self.PlaySound(SoundID.MENU_Start_New_Game);
            // Cut the Music
            if (self.manager.musicPlayer != null && self.manager.musicPlayer.song != null && self.manager.musicPlayer.song is Music.IntroRollMusic)
            { self.manager.musicPlayer.song.FadeOut(20f); }
        }

        private static void PageNewGameHK(On.Menu.SlugcatSelectMenu.SlugcatPageNewGame.orig_ctor orig, SlugcatSelectMenu.SlugcatPageNewGame self,
            Menu.Menu menu, MenuObject owner, int pageIndex, int slugcatNumber)
        {
            orig.Invoke(self, menu, owner, pageIndex, slugcatNumber);
            if (slugcatNumber == WandererSupplement.StoryCharacter)
            {
                self.difficultyLabel.text = DroughtMod.Translate("THE WANDERER");
                string desc = DroughtMod.Translate("Curious and calm, with a deep desire to discover the ancient mysteries around it.<LINE>In tune with the events of the world, your journey will have a significant impact on things much greater than yourself.");
                desc = desc.Replace("<LINE>", Environment.NewLine);
                self.infoLabel.text = desc;
            }
        }

        // This may not be needed with CRS
        private static void PageContinueHK(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_ctor orig, SlugcatSelectMenu.SlugcatPageContinue self,
            Menu.Menu menu, MenuObject owner, int pageIndex, int slugcatNumber)
        {
            orig.Invoke(self, menu, owner, pageIndex, slugcatNumber);

            string text = string.Empty;
            if (self.saveGameData.shelterName != null && self.saveGameData.shelterName.Length > 2)
            {
                string rgnShort = self.saveGameData.shelterName.Substring(0, 2);
                if (rgnShort != null)
                {
                    switch (rgnShort)
                    {
                        case "IS":
                            text = "Intake System";
                            break;
                        case "LM":
                            text = "Looks to the Moon";
                            break;
                        case "MW":
                            text = "The Fragmented Exterior";
                            break;
                        case "FS":
                            text = "Forest Sanctuary";
                            break;
                        default: return; // Vanilla: No need to replace region name
                    }
                    text = DroughtMod.Translate(text);
                }
                if (text.Length > 0)
                {
                    rgnShort = text;
                    text = string.Concat(
                            rgnShort,
                            " - ",
                            menu.Translate("Cycle"),
                            " ",
                            (slugcatNumber != 2) ? self.saveGameData.cycle : (RedsIllness.RedsCycles(self.saveGameData.redsExtraCycles) - self.saveGameData.cycle)
                    );
                }
            }
            self.regionLabel.RemoveSprites();
            self.RemoveSubObject(self.regionLabel);

            self.regionLabel = new MenuLabel(menu, self, text, new Vector2(-1000f, self.imagePos.y - 249f), new Vector2(200f, 30f), true);
            self.regionLabel.label.alignment = FLabelAlignment.Center;
            self.subObjects.Add(self.regionLabel);
        }
    }
}
