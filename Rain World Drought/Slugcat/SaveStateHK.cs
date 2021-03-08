using Rain_World_Drought.OverWorld;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rain_World_Drought.Slugcat
{
    internal static class SaveStateHK
    {
        public static void Patch()
        {
            On.PlayerProgression.GetProgLines += new On.PlayerProgression.hook_GetProgLines(ChecksumFix);
            On.SaveState.LoadGame += new On.SaveState.hook_LoadGame(LoadGameHK);
            On.MiscWorldSaveData.ToString += new On.MiscWorldSaveData.hook_ToString(MWSDToStringHK);
            On.MiscWorldSaveData.FromString += new On.MiscWorldSaveData.hook_FromString(MWSDFromStringHK);
            On.SLOrcacleState.ctor += new On.SLOrcacleState.hook_ctor(SLOrcacleStateCtorHK);
        }

        private static string[] ChecksumFix(On.PlayerProgression.orig_GetProgLines orig, PlayerProgression self)
        {
            string[] res = orig.Invoke(self);
            self.gameTinkeredWith = false;
            return res;
        }

        private static void LoadGameHK(On.SaveState.orig_LoadGame orig, SaveState self, string str, RainWorldGame game)
        {
            orig.Invoke(self, str, game);
            if (self.saveStateNumber == WandererSupplement.StoryCharacter && self.denPosition.Equals("SU_C04"))
            {
                Debug.Log("Drought) New Save File Created.");
                self.deathPersistentSaveData.theMark = true;
            }
        }

        public static FPOracleState GetFPState(MiscWorldSaveData saveData)
        {
            MiscWorldSaveDroughtData sub = MiscWorldSaveDroughtData.GetData(saveData);
            if (sub.privFPOracleState != null) { return sub.privFPOracleState; }
            sub.privFPOracleState = new FPOracleState(false, saveData.saveStateNumber);
            return sub.privFPOracleState;
        }

        private static string MWSDToStringHK(On.MiscWorldSaveData.orig_ToString orig, MiscWorldSaveData self)
        {
            string text = orig.Invoke(self);

            MiscWorldSaveDroughtData sub = MiscWorldSaveDroughtData.GetData(self);
            if (sub.privFPOracleState != null && sub.privFPOracleState.playerEncounters > 0)
            { text += "FPaiState<mwB>" + sub.privFPOracleState.ToString() + "<mwA>"; }
            if (sub.isImproved) { text += "ISIMPROVED<mwA>"; }

            return text;
        }

        private static void MWSDFromStringHK(On.MiscWorldSaveData.orig_FromString orig, MiscWorldSaveData self, string s)
        {
            orig.Invoke(self, s);

            MiscWorldSaveDroughtData sub = MiscWorldSaveDroughtData.GetData(self);
            string[] array = Regex.Split(s, "<mwA>");
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = Regex.Split(array[i], "<mwB>");
                string text = array2[0];
                switch (text)
                {
                    case "FPaiState":
                        sub.privFPOracleState = new FPOracleState(false, self.saveStateNumber);
                        sub.privFPOracleState.FromString(array2[1]);
                        break;
                    case "ISIMPROVED":
                        sub.isImproved = true;
                        break;
                }
            }
        }

        private static void SLOrcacleStateCtorHK(On.SLOrcacleState.orig_ctor orig, SLOrcacleState self, bool isDebugState, int saveStateNumber)
        {
            orig.Invoke(self, isDebugState, saveStateNumber);
            // using enumextender, so no need to extend miscItemsDescribed
            self.increaseLikeOnSave = saveStateNumber == WandererSupplement.StoryCharacter;
        }
    }

    public class MiscWorldSaveDroughtData
    {
        public MiscWorldSaveDroughtData(MiscWorldSaveData self)
        {
            this.self = self;
            this.isImproved = false;
        }

        public readonly MiscWorldSaveData self;
        public FPOracleState privFPOracleState;
        public bool isImproved;

        private static Dictionary<MiscWorldSaveData, MiscWorldSaveDroughtData> saves = new Dictionary<MiscWorldSaveData, MiscWorldSaveDroughtData>();

        public static MiscWorldSaveDroughtData GetData(MiscWorldSaveData self)
        {
            if (saves.TryGetValue(self, out MiscWorldSaveDroughtData res)) { return res; }
            MiscWorldSaveDroughtData newData = new MiscWorldSaveDroughtData(self);
            saves.Add(self, newData);
            return newData;
        }
    }
}
