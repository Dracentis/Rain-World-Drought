using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using HUD;
using MonoMod;
using UnityEngine;
using RWCustom;

public class patch_Conversation : Conversation
    {
        [MonoModIgnore]
        public patch_Conversation(IOwnAConversation interfaceOwner, Conversation.ID id, DialogBox dialogBox) : base(interfaceOwner, id, dialogBox)
        {
        }
    

    public static void EncryptAllDialogue()
    {
        Debug.Log("Encrypt all dialogue");
        System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(string.Concat(new object[]
        {
            Custom.RootFolderDirectory(),
            "Assets",
            System.IO.Path.DirectorySeparatorChar,
            "Text",
            System.IO.Path.DirectorySeparatorChar,
            "Short_Strings",
            System.IO.Path.DirectorySeparatorChar
        }));
        System.IO.FileInfo[] files = directoryInfo.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.Substring(files[i].Name.Length - 4, 4) == ".txt")
            {
                int num = int.Parse(files[i].Name.Substring(0, files[i].Name.Length - 4));
                string path = string.Concat(new object[]
                {
                    Custom.RootFolderDirectory(),
                    System.IO.Path.DirectorySeparatorChar,
                    "Assets",
                    System.IO.Path.DirectorySeparatorChar,
                    "Text",
                    System.IO.Path.DirectorySeparatorChar,
                    "Short_Strings",
                    System.IO.Path.DirectorySeparatorChar,
                    files[i].Name
                });
                string text = System.IO.File.ReadAllText(path, System.Text.Encoding.Default);
                if (text[0] == '0')
                {
                    string text2 = Custom.xorEncrypt(text, 12467 - num);
                    text2 = '1' + text2.Remove(0, 1);
                    Debug.Log("encrypting short string: " + num);
                    System.IO.File.WriteAllText(path, text2);
                }
            }
        }
        for (int j = 0; j < System.Enum.GetNames(typeof(InGameTranslator.LanguageID)).Length; j++)
        {
            for (int k = 1; k <= 75; k++)
            {
                string path = string.Concat(new object[]
                {
                    Custom.RootFolderDirectory(),
                    "Assets",
                    System.IO.Path.DirectorySeparatorChar,
                    "Text",
                    System.IO.Path.DirectorySeparatorChar,
                    "Text_",
                    LocalizationTranslator.LangShort((InGameTranslator.LanguageID)j),
                    System.IO.Path.DirectorySeparatorChar,
                    k,
                    ".txt"
                });
                if (System.IO.File.Exists(path))
                {
                    string text = System.IO.File.ReadAllText(path, System.Text.Encoding.Default);
                    if (text[0] == '0' && Regex.Split(text, System.Environment.NewLine).Length > 1)
                    {
                        /*
                        int tempK = k;
                        if (k > 75)
                        {
                            tempK = k - 50;
                        }*/
                        string text3 = Custom.xorEncrypt(text, 54 + k + j * 7);
                        text3 = '1' + text3.Remove(0, 1);
                        System.IO.File.WriteAllText(path, text3);
                    }
                }
            }
        }
    }

    public enum ID
        {
            None,
            MoonFirstPostMarkConversation,
            MoonSecondPostMarkConversation,
            MoonRecieveSwarmer,
            Moon_Pearl_Misc,
            Moon_Pearl_Misc2,
            Moon_Pebbles_Pearl,
            Moon_Pearl_CC,
            Moon_Pearl_SI_west,
            Moon_Pearl_SI_top,
            Moon_Pearl_LF_west,
            Moon_Pearl_LF_bottom,
            Moon_Pearl_HI,
            Moon_Pearl_SH,
            Moon_Pearl_DS,
            Moon_Pearl_SB_filtration,
            Moon_Pearl_GW,
            Moon_Pearl_SL_bridge,
            Moon_Pearl_SL_moon,
            Moon_Misc_Item,
            Moon_Pearl_SU,
            Moon_Pearl_SB_ravine,
            Moon_Pearl_UW,
            Moon_Pearl_SL_chimney,
            Moon_Pearl_Red_stomach,
            Moon_Red_First_Conversation,
            Moon_Red_Second_Conversation,
            Moon_Yellow_First_Conversation,
            Ghost_CC,
            Ghost_SI,
            Ghost_LF,
            Ghost_SH,
            Ghost_UW,
            Ghost_SB,
            Pebbles_White,
            Pebbles_Red_Green_Neuron,
            Pebbles_Red_No_Neuron,
            Pebbles_Yellow,
            Moon_Pearl_MoonPearl,
            Moon_Pearl_Drought1,
            Moon_Pearl_Drought2,
            Moon_Pearl_Drought3,
            SI_Spire1,
            SI_Spire2,
            SI_Spire3,
            MoonPostMark
        }
    }

