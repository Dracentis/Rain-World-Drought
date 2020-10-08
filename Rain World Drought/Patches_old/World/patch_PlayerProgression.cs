using System;
using System.IO;
using MonoMod;
using System.Text.RegularExpressions;
using RWCustom;
using UnityEngine;

public class patch_PlayerProgression : PlayerProgression
{
    // Token: 0x06000003 RID: 3 RVA: 0x00002065 File Offset: 0x00000265
    [MonoModIgnore]
    public patch_PlayerProgression(RainWorld rainWorld, bool tryLoad) : base(rainWorld, tryLoad)
    {
    }

    // Token: 0x06000004 RID: 4 RVA: 0x00002070 File Offset: 0x00000270
    public new string[] GetProgLines()
    {
        if (File.Exists(saveFilePath))
        {
            string text = File.ReadAllText(saveFilePath);
            string b = text.Substring(0, 32);
            text = text.Substring(32, text.Length - 32);
            if (Custom.Md5Sum(text) == b)
            {
                Debug.Log("Checksum CORRECT!");
            }
            else
            {
                Debug.Log("Checksum WRONG!");
                gameTinkeredWith = false;
            }
            return Regex.Split(text, "<progDivA>");
        }
        Debug.Log("No existing save file at " + saveFilePath + ". Returns empty prog lines");
        return new string[0];
    }
}