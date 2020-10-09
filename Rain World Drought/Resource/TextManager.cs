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
        public static string Translate(string text)
        {
            return text.Replace("\\n", Environment.NewLine);
        }

        public static void LoadEventsFromFile(Conversation instance, int fileName)
        {
            LoadEventsFromFile(instance, fileName, false, 0);
        }

        private static void LoadEventsFromFile(Conversation instance, int fileName, bool oneRandomLine, int randomSeed)
        {
            Debug.Log("~~~LOAD DROUGHT CONVO " + fileName);
            string path = string.Concat(
                instance.interfaceOwner.rainWorld.inGameTranslator.SpecificTextFolderDirectory(),
                Path.DirectorySeparatorChar,
                fileName,
                ".txt"
            ); // Change this so it would load from non-vanilla folder
            if (!File.Exists(path))
            {
                Debug.Log("NOT FOUND " + path);
                return;
            }
            string text2 = File.ReadAllText(path, Encoding.UTF8);
            if (text2[0] == '0')
            {
                Conversation.EncryptAllDialogue();
            }
            else
            {
                text2 = Custom.xorEncrypt(text2, (54 + fileName + (int)instance.interfaceOwner.rainWorld.inGameTranslator.currentLanguage * 7));
            }
            string[] array = Regex.Split(text2, Environment.NewLine);
            try
            {
                if (Regex.Split(array[0], "-")[1] == fileName.ToString())
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
                                {
                                    instance.events.Add(new Conversation.SpecialEvent(instance, 0, array3[1]));
                                }
                                else if (array3[0] == "PEBBLESWAIT")
                                {
                                    instance.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(instance, null, int.Parse(array3[1])));
                                }
                            }
                            else if (array3.Length == 1 && array3[0].Length > 0)
                            {
                                instance.events.Add(new Conversation.TextEvent(instance, 0, array3[0], 0));
                            }
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
    }
}
