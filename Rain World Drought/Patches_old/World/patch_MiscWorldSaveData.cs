using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MonoMod;
using RWCustom;

class patch_MiscWorldSaveData : MiscWorldSaveData
{
    [MonoModIgnore]
    public patch_MiscWorldSaveData(int saveStateNumber) : base(saveStateNumber)
    {
    }

    public FPOracleState FPOracleState
    {
        get
        {
            if (this.privFPOracleState == null)
            {
                this.privFPOracleState = new FPOracleState(false, this.saveStateNumber);
            }
            return this.privFPOracleState;
        }
    }

    // Token: 0x170001FA RID: 506
    // (get) Token: 0x06000EC8 RID: 3784 RVA: 0x000A6CB5 File Offset: 0x000A4EB5
    public bool EverMetMoon
    {
        get
        {
            return this.privSlOracleState != null && this.privSlOracleState.playerEncounters > 0;
        }
    }

    public bool isImproved = false;

    public override string ToString()
    {
        string text = string.Empty;
        string text2;
        if (this.SSaiConversationsHad > 0)
        {
            text2 = text;
            text = string.Concat(new object[]
            {
                text2,
                "SSaiConversationsHad<mwB>",
                this.SSaiConversationsHad,
                "<mwA>"
            });
        }
        if (this.SSaiThrowOuts > 0)
        {
            text2 = text;
            text = string.Concat(new object[]
            {
                text2,
                "SSaiThrowOuts<mwB>",
                this.SSaiThrowOuts,
                "<mwA>"
            });
        }
        if (this.privSlOracleState != null && this.privSlOracleState.playerEncounters > 0)
        {
            text = text + "SLaiState<mwB>" + this.privSlOracleState.ToString() + "<mwA>";
        }

        if (this.privFPOracleState != null && this.privFPOracleState.playerEncounters > 0)
        {
            text = text + "FPaiState<mwB>" + this.privFPOracleState.ToString() + "<mwA>";
        }
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "playerGuideState<mwB>",
            this.playerGuideState,
            "<mwA>"
        });
        if (this.moonRevived)
        {
            text += "MOONREVIVED<mwA>";
        }
        if (this.pebblesSeenGreenNeuron)
        {
            text += "PEBBLESHELPED<mwA>";
        }
        if (this.isImproved)
        {
            text += "ISIMPROVED<mwA>";
        }
        if (this.memoryArraysFrolicked)
        {
            text += "MEMORYFROLICK<mwA>";
        }
        return text;
    }

    // Token: 0x06000ECA RID: 3786 RVA: 0x000A6DF0 File Offset: 0x000A4FF0
    public void FromString(string s)
    {
        this.moonRevived = false;
        this.pebblesSeenGreenNeuron = false;
        this.memoryArraysFrolicked = false;
        this.SSaiConversationsHad = 0;
        this.SSaiThrowOuts = 0;
        string[] array = Regex.Split(s, "<mwA>");
        for (int i = 0; i < array.Length; i++)
        {
            string[] array2 = Regex.Split(array[i], "<mwB>");
            string text = array2[0];
            switch (text)
            {
                case "SSaiConversationsHad":
                    this.SSaiConversationsHad = int.Parse(array2[1]);
                    break;
                case "SSaiThrowOuts":
                    this.SSaiThrowOuts = int.Parse(array2[1]);
                    break;
                case "SLaiState":
                    this.privSlOracleState = new SLOrcacleState(false, this.saveStateNumber);
                    this.privSlOracleState.FromString(array2[1]);
                    break;
                case "FPaiState":
                    this.privFPOracleState = new FPOracleState(false, this.saveStateNumber);
                    this.privFPOracleState.FromString(array2[1]);
                    break;
                case "playerGuideState":
                    this.playerGuideState.FromString(array2[1]);
                    break;
                case "MOONREVIVED":
                    this.moonRevived = true;
                    break;
                case "ISIMPROVED":
                    this.isImproved = true;
                    break;
                case "PEBBLESHELPED":
                    this.pebblesSeenGreenNeuron = true;
                    break;
                case "MEMORYFROLICK":
                    this.memoryArraysFrolicked = true;
                    break;
            }
        }
    }


    private FPOracleState privFPOracleState;
}
