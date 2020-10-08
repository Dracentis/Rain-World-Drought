using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    public class FPOracleState
    {
        public FPOracleState(bool isDebugState, int saveStateNumber)
        {
            this.increaseLikeOnSave = true;
            this.isDebugState = isDebugState;
            this.integers = new int[8];
            this.miscBools = new bool[1];
            this.significantPearls = new bool[Enum.GetNames(typeof(DataPearl.AbstractDataPearl.DataPearlType)).Length];
            this.miscItemsDescribed = new bool[Enum.GetNames(typeof(LMOracleBehaviorHasMark.MiscItemType)).Length];
            this.likesPlayer = 0.3f;
            this.alreadyTalkedAboutItems = new List<EntityID>();
            this.chatLogA = -1;
            this.chatLogB = -1;
        }

        public int playerEncounters
        {
            get
            {
                return this.integers[0];
            }
            set
            {
                this.integers[0] = value;
            }
        }

        public int annoyances
        {
            get
            {
                return this.integers[1];
            }
            set
            {
                this.integers[1] = value;
            }
        }

        public int totalInterruptions
        {
            get
            {
                return this.integers[2];
            }
            set
            {
                this.integers[2] = value;
            }
        }

        public int totalItemsBrought
        {
            get
            {
                return this.integers[3];
            }
            set
            {
                this.integers[3] = value;
            }
        }

        public int totalPearlsBrought
        {
            get
            {
                return this.integers[4];
            }
            set
            {
                this.integers[4] = value;
            }
        }

        public int miscPearlCounter
        {
            get
            {
                return this.integers[5];
            }
            set
            {
                this.integers[5] = value;
            }
        }

        public int chatLogA
        {
            get
            {
                return this.integers[6];
            }
            set
            {
                this.integers[6] = value;
            }
        }

        public int chatLogB
        {
            get
            {
                return this.integers[7];
            }
            set
            {
                this.integers[7] = value;
            }
        }

        public bool hasPlayerFinishedMission
        {
            get
            {
                return this.miscBools[0];
            }
            set
            {
                this.miscBools[0] = value;
            }
        }

        public FPOracleState.PlayerOpinion GetOpinion
        {
            get
            {
                return (FPOracleState.PlayerOpinion)Custom.LerpMap(this.likesPlayer, -1f, 1f, 0f, 4f);
            }
        }

        public bool SpeakingTerms
        {
            get
            {
                return this.GetOpinion != FPOracleState.PlayerOpinion.NotSpeaking;
            }
        }

        public void InfluenceLike(float influence)
        {
            this.likesPlayer = Mathf.Clamp(this.likesPlayer + influence, -1f, 1f);
        }

        public void AddItemToAlreadyTalkedAbout(EntityID ID)
        {
            for (int i = 0; i < this.alreadyTalkedAboutItems.Count; i++)
            {
                if (this.alreadyTalkedAboutItems[i] == ID)
                {
                    return;
                }
            }
            this.alreadyTalkedAboutItems.Add(ID);
        }

        public bool HaveIAlreadyDescribedThisItem(EntityID ID)
        {
            for (int i = 0; i < this.alreadyTalkedAboutItems.Count; i++)
            {
                if (this.alreadyTalkedAboutItems[i] == ID)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            string text = string.Empty;
            text += "integersArray<fposB>";
            for (int i = 0; i < this.integers.Length; i++)
            {
                text = text + this.integers[i] + ((i >= this.integers.Length - 1) ? string.Empty : ".");
            }
            text += "<fposA>";
            text += "miscBools<fposB>";
            for (int j = 0; j < this.miscBools.Length; j++)
            {
                text += ((!this.miscBools[j]) ? "0" : "1");
            }
            text += "<fposA>";
            text += "significantPearls<fposB>";
            for (int k = 0; k < this.significantPearls.Length; k++)
            {
                text += ((!this.significantPearls[k]) ? "0" : "1");
            }
            text += "<fposA>";
            text += "miscItemsDescribed<fposB>";
            for (int l = 0; l < this.miscItemsDescribed.Length; l++)
            {
                text += ((!this.miscItemsDescribed[l]) ? "0" : "1");
            }
            text += "<fposA>";
            if (this.increaseLikeOnSave)
            {
                this.InfluenceLike(0.15f);
            }
            string text2 = text;
            text = string.Concat(new object[]
            {
            text2,
            "likesPlayer<fposB>",
            this.likesPlayer,
            "<fposA>"
            });
            if (this.alreadyTalkedAboutItems.Count > 0)
            {
                text += "itemsAlreadyTalkedAbout<fposB>";
                for (int m = 0; m < this.alreadyTalkedAboutItems.Count; m++)
                {
                    text = text + this.alreadyTalkedAboutItems[m].ToString() + ((m >= this.alreadyTalkedAboutItems.Count - 1) ? string.Empty : "<fposC>");
                }
                text += "<fposA>";
            }
            Debug.LogError(text);
            return text;
        }

        public void FromString(string s)
        {
            string[] array = Regex.Split(s, "<fposA>");
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = Regex.Split(array[i], "<fposB>");
                string text = array2[0];
                switch (text)
                {
                    case "integersArray":
                        {
                            string[] array3 = array2[1].Split(new char[]
                            {
                    '.'
                            });
                            int num2 = 0;
                            while (num2 < array3.Length && num2 < this.integers.Length)
                            {
                                this.integers[num2] = int.Parse(array3[num2]);
                                num2++;
                            }
                            break;
                        }
                    case "miscBools":
                        {
                            int num3 = 0;
                            while (num3 < array2[1].Length && num3 < this.miscBools.Length)
                            {
                                this.miscBools[num3] = (array2[1][num3] == '1');
                                num3++;
                            }
                            break;
                        }
                    case "significantPearls":
                        {
                            int num4 = 0;
                            while (num4 < array2[1].Length && num4 < this.significantPearls.Length)
                            {
                                this.significantPearls[num4] = (array2[1][num4] == '1');
                                num4++;
                            }
                            break;
                        }
                    case "miscItemsDescribed":
                        {
                            int num5 = 0;
                            while (num5 < array2[1].Length && num5 < this.miscItemsDescribed.Length)
                            {
                                this.miscItemsDescribed[num5] = (array2[1][num5] == '1');
                                num5++;
                            }
                            break;
                        }
                    case "likesPlayer":
                        this.likesPlayer = float.Parse(array2[1]);
                        break;
                    case "itemsAlreadyTalkedAbout":
                        {
                            string[] array3 = Regex.Split(array2[1], "<fposC>");
                            for (int j = 0; j < array3.Length; j++)
                            {
                                if (array3[j].Length > 0)
                                {
                                    this.alreadyTalkedAboutItems.Add(EntityID.FromString(array3[j]));
                                }
                            }
                            break;
                        }
                }
            }
        }

        private int[] integers;

        public bool[] miscBools;

        public bool[] significantPearls;

        public bool[] miscItemsDescribed;

        public List<EntityID> alreadyTalkedAboutItems;

        public float likesPlayer;

        public bool isDebugState;

        public bool increaseLikeOnSave;

        public enum PlayerOpinion
        {
            NotSpeaking,
            Dislikes,
            Neutral,
            Likes
        }
    }
}
