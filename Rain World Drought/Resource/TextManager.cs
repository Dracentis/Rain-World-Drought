using Menu;
using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rain_World_Drought.Resource
{
    public static class TextManager
    {
        public static void Patch()
        {
            On.GhostConversation.AddEvents += new On.GhostConversation.hook_AddEvents(TextManager.GhostEvents);
            On.Menu.CreditsTextAndImage.ctor += new On.Menu.CreditsTextAndImage.hook_ctor(CreditReplace);
        }

        private static void CreditReplace(On.Menu.CreditsTextAndImage.orig_ctor orig, CreditsTextAndImage self,
           Menu.Menu menu, MenuObject owner, EndCredits.Stage stage)
        {
            orig.Invoke(self, menu, owner, stage);
            if (stage == EndCredits.Stage.VideoCult)
            {
                string path = string.Concat(ResourceManager.assetDir,
                    "Text",
                    Path.DirectorySeparatorChar,
                    "Credits",
                    Path.DirectorySeparatorChar,
                    "01 - VIDEOCULT.txt"
                    );
                if (File.Exists(path))
                {
                    string[] array = File.ReadAllLines(path, Encoding.Default);
                    // Remove old Credits
                    for (int j = self.subObjects.Count - 1; j >= 0; j--)
                    {
                        if (self.subObjects[j] is MenuLabel) { self.subObjects[j].RemoveSprites(); self.RemoveSubObject(self.subObjects[j]); }
                    }
                    // Add new one
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = array[i].Replace("<LINE>", Environment.NewLine);
                        self.subObjects.Add(new MenuLabel(menu, self, array[i], new Vector2(433f, (float)(-(float)i) * 40f), new Vector2(500f, 30f), false));
                        (self.subObjects[self.subObjects.Count - 1] as MenuLabel).label.alignment = FLabelAlignment.Center;
                        (self.subObjects[self.subObjects.Count - 1] as MenuLabel).label.x = -1000f;
                    }
                }
            }
        }

        public static string Translate(string text)
        { // Add Translator Here!
            return text.Replace("\\n", Environment.NewLine);
        }

        private static void GhostEvents(On.GhostConversation.orig_AddEvents orig, GhostConversation self)
        {
            switch (self.id)
            {
                case Conversation.ID.Ghost_CC:
                    LoadEventsFromFile(self, EventID.Ghost_CC);
                    break;
                case Conversation.ID.Ghost_SI:
                    LoadEventsFromFile(self, EventID.Ghost_SI);
                    break;
                case Conversation.ID.Ghost_LF:
                    LoadEventsFromFile(self, EventID.Ghost_LF);
                    break;
                case Conversation.ID.Ghost_SH:
                    LoadEventsFromFile(self, EventID.Ghost_SH);
                    break;
                case Conversation.ID.Ghost_UW:
                    LoadEventsFromFile(self, EventID.Ghost_UW);
                    break;
                case Conversation.ID.Ghost_SB:
                    LoadEventsFromFile(self, EventID.Ghost_SB);
                    break;
                default: orig.Invoke(self); break;
            }
        }

        public static void LoadEventsFromFile(Conversation instance, EventID id) => LoadEventsFromFile(instance, id, false, 0);

        public static void LoadEventsFromFile(Conversation instance, EventID id, bool oneRandomLine, int randomSeed)
        {
            Debug.Log($"~~~LOAD DROUGHT CONVO {id} ({(int)id})");
            string path = string.Concat(
                ResourceManager.assetDir,
                "Text",
                Path.DirectorySeparatorChar,
                "Text_",
                "Eng", // Change this!
                Path.DirectorySeparatorChar,
                (int)id,
                ".txt"
            );
            if (!File.Exists(path))
            { // Use English if there's no translation
                path = string.Concat(
                    ResourceManager.assetDir,
                    "Text",
                    Path.DirectorySeparatorChar,
                    "Text_Eng",
                    Path.DirectorySeparatorChar,
                    (int)id,
                    ".txt"
                );
                if (!File.Exists(path))
                {
                    Debug.Log("NOT FOUND " + path);
                    instance.events.Add(new Conversation.TextEvent(instance, 0, "NOT FOUND " + path, 100));
                    return;
                }
            }
            string text2 = File.ReadAllText(path, Encoding.UTF8);
            if (text2[0] == '0')
            {
                // Conversation.EncryptAllDialogue(); // Make Encryptor for Drought
            }
            else
            {
                text2 = Custom.xorEncrypt(text2, (54 + (int)id + (int)instance.interfaceOwner.rainWorld.inGameTranslator.currentLanguage * 7));
            }
            string[] array = Regex.Split(text2, Environment.NewLine);
            try
            {
                if (Regex.Split(array[0], "-")[1] == ((int)id).ToString())
                {
                    if (oneRandomLine)
                    {
                        List<Conversation.TextEvent> list = new List<Conversation.TextEvent>();
                        for (int i = 1; i < array.Length; i++)
                        {
                            string[] array2 = Regex.Split(array[i], " : ");
                            if (array2.Length == 3)
                            {
                                list.Add(new Conversation.TextEvent(instance, int.Parse(array2[0]), array2[2], int.Parse(array2[1])));
                            }
                            else if (array2.Length == 1 && array2[0].Length > 0)
                            {
                                list.Add(new Conversation.TextEvent(instance, 0, array2[0], 0));
                            }
                        }
                        if (list.Count > 0)
                        {
                            int seed = UnityEngine.Random.seed;
                            UnityEngine.Random.seed = randomSeed;
                            Conversation.TextEvent item = list[UnityEngine.Random.Range(0, list.Count)];
                            UnityEngine.Random.seed = seed;
                            instance.events.Add(item);
                        }
                    }
                    else
                    {
                        for (int j = 1; j < array.Length; j++)
                        {
                            string[] array3 = Regex.Split(array[j], " : ");
                            if (array3.Length == 3)
                            {
                                instance.events.Add(new Conversation.TextEvent(instance, int.Parse(array3[0]), array3[2], int.Parse(array3[1])));
                            }
                            else if (array3.Length == 2)
                            {
                                if (array3[0] == "SPECEVENT")
                                { instance.events.Add(new Conversation.SpecialEvent(instance, 0, array3[1])); }
                                else if (array3[0] == "PEBBLESWAIT")
                                { instance.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(instance, null, int.Parse(array3[1]))); }
                            }
                            else if (array3.Length == 1 && array3[0].Length > 0)
                            { instance.events.Add(new Conversation.TextEvent(instance, 0, array3[0], 0)); }
                        }
                    }
                }
            }
            catch
            {
                Debug.Log("TEXT ERROR");
                instance.events.Add(new Conversation.TextEvent(instance, 0, "TEXT ERROR", 100));
            }
        }

        public enum EventID
        {
            // GhostConversation
            Ghost_CC = 1,
            Ghost_SI = 2,
            Ghost_LF = 3,
            Ghost_SH = 4,
            Ghost_UW = 5,
            Ghost_SB = 6,
            // PearlConversation
            PC_Moon_Pearl_CC = 7,
            PC_Moon_Pearl_SI_west = 20,
            PC_Moon_Pearl_SI_top = 21,
            PC_Moon_Pearl_LF_west = 10,
            PC_Moon_Pearl_LF_bottom = 11,
            PC_Moon_Pearl_HI = 12,
            PC_Moon_Pearl_SH = 13,
            PC_Moon_Pearl_DS = 14,
            PC_Moon_Pearl_SB_filtration = 15,
            PC_Moon_Pearl_GW = 16,
            PC_Moon_Pearl_SL_bridge = 17,
            PC_Moon_Pearl_SL_moon = 18,
            PC_Moon_Pearl_SU = 41,
            PC_Moon_Pearl_SB_ravine = 43,
            PC_Moon_Pearl_UW = 42,
            PC_Moon_Pearl_SL_chimney = 54,
            PC_SI_Spire1 = 22,
            PC_SI_Spire2 = 23,
            PC_SI_Spire3 = 24,
            PC_MiscPearl = 38,
            PC_PebblesPearl = 40,
            // LMOracleBehaviorHasMark (duplicate of PearlConversation)
            LM_Moon_Pearl_CC = 7,
            LM_Moon_Pearl_SI_west = 20,
            LM_Moon_Pearl_SI_top = 21,
            LM_Moon_Pearl_LF_west = 10,
            LM_Moon_Pearl_LF_bottom = 11,
            LM_Moon_Pearl_HI = 12,
            LM_Moon_Pearl_SH = 13,
            LM_Moon_Pearl_DS = 14,
            LM_Moon_Pearl_SB_filtration = 15,
            LM_Moon_Pearl_GW = 16,
            LM_Moon_Pearl_SL_bridge = 17,
            LM_Moon_Pearl_SL_moon = 18,
            LM_Moon_Pearl_SU = 41,
            LM_Moon_Pearl_SB_ravine = 43,
            LM_Moon_Pearl_UW = 42,
            LM_Moon_Pearl_SL_chimney = 54,
            LM_SI_Spire1 = 22,
            LM_SI_Spire2 = 23,
            LM_SI_Spire3 = 24,
            LM_MiscPearl = 38,
            LM_PebblesPearl = 40,
            // LMOracleBehaviorHasMark
            LM_KarmaFlower = 25,
            LM_SSOracleSwarmer = 19,
            LM_DangleFruit = 26,
            LM_FlareBomb = 27,
            LM_VultureMask = 28,
            LM_PuffBall = 29,
            LM_JellyFish = 30,
            LM_Lantern = 31,
            LM_Mushroom = 32,
            LM_FirecrackerPlant = 33,
            LM_SlimeMold = 34,
            LM_ScavBomb = 44,
            LM_BubbleGrass = 53,
            LM_OverseerRemains = 52,
            LM_Moon_Pearl_Red_stomach = 51,
            LM_Moon_Red_First_Conversation = 50,
            LM_Moon_Red_Second_Conversation = 55,
            LM_Moon_Yellow_First_Conversation = 49,
            // FPOracleBehaviorHasMark
            FP_Moon_Pearl_CC = 8,
            FP_Moon_Pearl_SI_west = 64,
            FP_Moon_Pearl_SI_top = 65,
            FP_Moon_Pearl_LF_west = 55,
            FP_Moon_Pearl_LF_bottom = 56,
            FP_Moon_Pearl_HI = 57,
            FP_Moon_Pearl_SH = 58,
            FP_Moon_Pearl_DS = 59,
            FP_Moon_Pearl_SB_filtration = 60,
            FP_Moon_Pearl_GW = 61,
            FP_Moon_Pearl_SL_bridge = 62,
            FP_Moon_Pearl_SL_moon = 63,
            FP_Moon_Pearl_SU = 69,
            FP_Moon_Pearl_SB_ravine = 60,
            FP_Moon_Pearl_UW = 70,
            FP_Moon_Pearl_SL_chimney = 62,
            FP_SI_Spire1 = 66,
            FP_SI_Spire2 = 67,
            FP_SI_Spire3 = 68,
            FP_MiscPearl = 74,
            FP_PebblesPearl = 73,
            FP_Moon_Pearl_Red_stomach = 51,
            FP_Moon_Red_First_Conversation = 50,
            FP_Moon_Red_Second_Conversation = 55,
            FP_Moon_Yellow_First_Conversation = 49,
            FP_KarmaFlower = 35,
            FP_DangleFruit = 36,
            FP_FlareBomb = 37,
            FP_VultureMask = 39,
            FP_PuffBall = 45,
            FP_JellyFish = 46,
            FP_Lantern = 47,
            FP_Mushroom = 48,
            FP_FirecrackerPlant = 49,
            FP_SlimeMold = 50,
            FP_ScavBomb = 75,
            FP_BubbleGrass = 53,
            FP_OverseerRemains = 52
        }
    }
}
