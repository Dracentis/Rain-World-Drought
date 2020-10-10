using System;
using RWCustom;
using UnityEngine;
using System.Text.RegularExpressions;
using MonoMod;


class patch_DreamsState : DreamsState
{

    public extern void orig_ctor();

    [MonoModConstructor]
    public void ctor()
    {
        this.integers = new int[32];
    }

    public bool everReadLF
    {
        get
        {
            return this.integers[9] == 1;
        }
        set
        {
            this.integers[9] = ((!value) ? 0 : 1);
        }
    }
    public bool everReadLF2
    {
        get
        {
            return this.integers[10] == 1;
        }
        set
        {
            this.integers[9] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadHI
    {
        get
        {
            return this.integers[11] == 1;
        }
        set
        {
            this.integers[11] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSH
    {
        get
        {
            return this.integers[12] == 1;
        }
        set
        {
            this.integers[12] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadDS
    {
        get
        {
            return this.integers[13] == 1;
        }
        set
        {
            this.integers[13] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSB
    {
        get
        {
            return this.integers[14] == 1;
        }
        set
        {
            this.integers[14] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadGW
    {
        get
        {
            return this.integers[15] == 1;
        }
        set
        {
            this.integers[15] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSL
    {
        get
        {
            return this.integers[16] == 1;
        }
        set
        {
            this.integers[16] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSL2
    {
        get
        {
            return this.integers[17] == 1;
        }
        set
        {
            this.integers[17] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSL3
    {
        get
        {
            return this.integers[18] == 1;
        }
        set
        {
            this.integers[15] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSI
    {
        get
        {
            return this.integers[19] == 1;
        }
        set
        {
            this.integers[19] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSI2
    {
        get
        {
            return this.integers[20] == 1;
        }
        set
        {
            this.integers[20] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSI3
    {
        get
        {
            return this.integers[21] == 1;
        }
        set
        {
            this.integers[21] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSI4
    {
        get
        {
            return this.integers[22] == 1;
        }
        set
        {
            this.integers[22] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSI5
    {
        get
        {
            return this.integers[23] == 1;
        }
        set
        {
            this.integers[23] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSB2
    {
        get
        {
            return this.integers[24] == 1;
        }
        set
        {
            this.integers[24] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadSU
    {
        get
        {
            return this.integers[25] == 1;
        }
        set
        {
            this.integers[25] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadUW
    {
        get
        {
            return this.integers[26] == 1;
        }
        set
        {
            this.integers[26] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadFS
    {
        get
        {
            return this.integers[27] == 1;
        }
        set
        {
            this.integers[27] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadIS
    {
        get
        {
            return this.integers[28] == 1;
        }
        set
        {
            this.integers[28] = ((!value) ? 0 : 1);
        }
    }

    public bool everReadMW
    {
        get
        {
            return this.integers[29] == 1;
        }
        set
        {
            this.integers[29] = ((!value) ? 0 : 1);
        }
    }

    public bool everSeenMissonComplete
    {
        get
        {
            return this.integers[30] == 1;
        }
        set
        {
            this.integers[30] = ((!value) ? 0 : 1);
        }
    }

    public bool everSeenTraitor
    {
        get
        {
            return this.integers[31] == 1;
        }
        set
        {
            this.integers[31] = ((!value) ? 0 : 1);
        }
    }

    public bool AnyMessageComingUp
    {
        get
        {
            Debug.Log("Any Message Coming Up: "+ (this.upcomingDream != null & this.upcomingDream >= (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlLF));
            return (this.upcomingDream != null & this.upcomingDream >= (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlLF);
        }
    }


    public void EndOfCycleProgress(SaveState saveState, string currentRegion, string denPosition)
    {
        bool everSleptInSB = this.everSleptInSB;
        bool everSleptInSB_S = this.everSleptInSB_S01;
        DreamsState.StaticEndOfCycleProgress(saveState, currentRegion, denPosition, ref this.integers[0], ref this.integers[1], ref this.integers[2], ref this.integers[5], ref this.upcomingDream, ref this.eventDream, ref everSleptInSB, ref everSleptInSB_S, ref this.guideHasShownHimselfToPlayer, ref this.integers[4], ref this.guideHasShownMoonThisRound, ref this.integers[3]);
        this.everSleptInSB = everSleptInSB;
        this.everSleptInSB_S01 = everSleptInSB_S;
        switch (this.upcomingDream)
        {
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlLF:
                if (everReadLF)
                {
                    this.upcomingDream = null;
                }
                everReadLF = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlLF2:
                if (everReadLF2)
                {
                    this.upcomingDream = null;
                }
                everReadLF2 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSH:
                if (everReadSH)
                {
                    this.upcomingDream = null;
                }
                everReadSH = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlDS:
                if (everReadDS)
                {
                    this.upcomingDream = null;
                }
                everReadDS = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSB:
                if (everReadSB)
                {
                    this.upcomingDream = null;
                }
                everReadSB = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSB2:
                if (everReadSB2)
                {
                    this.upcomingDream = null;
                }
                everReadSB2 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlGW:
                if (everReadGW)
                {
                    this.upcomingDream = null;
                }
                everReadGW = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSL:
                if (everReadSL)
                {
                    this.upcomingDream = null;
                }
                everReadSL = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSL2:
                if (everReadSL2)
                {
                    this.upcomingDream = null;
                }
                everReadSL2 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSL3:
                if (everReadSL3)
                {
                    this.upcomingDream = null;
                }
                everReadSL3 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlIS:
                if (everReadIS)
                {
                    this.upcomingDream = null;
                }
                everReadIS = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlFS:
                if (everReadFS)
                {
                    this.upcomingDream = null;
                }
                everReadFS = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlMW:
                if (everReadMW)
                {
                    this.upcomingDream = null;
                }
                everReadMW = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI:
                if (everReadSI)
                {
                    this.upcomingDream = null;
                }
                everReadSI = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI2:
                if (everReadSI2)
                {
                    this.upcomingDream = null;
                }
                everReadSI2 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI3:
                if (everReadSI3)
                {
                    this.upcomingDream = null;
                }
                everReadSI3 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI4:
                if (everReadSI4)
                {
                    this.upcomingDream = null;
                }
                everReadSI4 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI5:
                if (everReadSI5)
                {
                    this.upcomingDream = null;
                }
                everReadSI5 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSU:
                if (everReadSU)
                {
                    this.upcomingDream = null;
                }
                everReadSU = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlUW:
                if (everReadUW)
                {
                    this.upcomingDream = null;
                }
                everReadUW = true;
                break;
        }
    }
    
    
    private static void StaticEndOfCycleProgress(SaveState saveState, string currentRegion, string denPosition, ref int cyclesSinceLastDream, ref int cyclesSinceLastFamilyDream, ref int cyclesSinceLastGuideDream, ref int inGWOrSHCounter, ref DreamsState.DreamID? upcomingDream, ref DreamsState.DreamID? eventDream, ref bool everSleptInSB, ref bool everSleptInSB_S01, ref bool guideHasShownHimselfToPlayer, ref int guideThread, ref bool guideHasShownMoonThisRound, ref int familyThread)
    {
        cyclesSinceLastDream++;
        cyclesSinceLastFamilyDream++;
        cyclesSinceLastGuideDream++;
        if (inGWOrSHCounter > 0)
        {
            inGWOrSHCounter++;
        }
        else if (currentRegion == "GW" || currentRegion == "SH")
        {
            inGWOrSHCounter = 1;
        }
        upcomingDream = null;
        if (eventDream != null)
        {
            cyclesSinceLastDream = 0;
            upcomingDream = eventDream;
            return;
        }
        if (!everSleptInSB && currentRegion == "SB")
        {
            everSleptInSB = true;
            cyclesSinceLastDream = 0;
            upcomingDream = new DreamsState.DreamID?(DreamsState.DreamID.VoidDreamSlugcatUp);
            return;
        }
        if (!everSleptInSB_S01 && denPosition == "SB_S01")
        {
            everSleptInSB_S01 = true;
            cyclesSinceLastDream = 0;
            upcomingDream = new DreamsState.DreamID?(DreamsState.DreamID.VoidDreamSlugcatDown);
            return;
        }
        if (saveState.cycleNumber > 2 && cyclesSinceLastDream > 1 && guideHasShownHimselfToPlayer && saveState.miscWorldSaveData.playerGuideState.likesPlayer > -0.75f && saveState.saveStateNumber != 0)
        {
            DreamsState.DreamID? dreamID = null;
            if (guideThread == 0)
            {
                dreamID = new DreamsState.DreamID?(DreamsState.DreamID.GuideA);
            }
            else if (guideThread <= 3 && (guideHasShownMoonThisRound || inGWOrSHCounter > 1))
            {
                dreamID = new DreamsState.DreamID?(DreamsState.DreamID.GuideB);
            }
            else if (guideThread <= 4 && currentRegion == "SL")
            {
                dreamID = new DreamsState.DreamID?(DreamsState.DreamID.GuideC);
            }
            if (dreamID != null)
            {
                guideThread = (int)dreamID.Value;
                Debug.Log("guideThread increase to: " + guideThread);
                upcomingDream = dreamID;
                cyclesSinceLastDream = 0;
                cyclesSinceLastGuideDream = 0;
                return;
            }
        }

        if (cyclesSinceLastDream > 2 && saveState.saveStateNumber != 0)
        {
            DreamsState.DreamID? dreamID2 = null;
            switch (familyThread)
            {
                case 0:
                    if (cyclesSinceLastFamilyDream > 6)
                    {
                        dreamID2 = new DreamsState.DreamID?(DreamsState.DreamID.FamilyA);
                    }
                    break;
                case 1:
                    if (cyclesSinceLastFamilyDream > 7)
                    {
                        dreamID2 = new DreamsState.DreamID?(DreamsState.DreamID.FamilyB);
                    }
                    break;
                case 2:
                    if (cyclesSinceLastFamilyDream > 6)
                    {
                        dreamID2 = new DreamsState.DreamID?(DreamsState.DreamID.FamilyC);
                    }
                    break;
                case 3:
                    if (cyclesSinceLastFamilyDream > 14)
                    {
                        dreamID2 = new DreamsState.DreamID?(DreamsState.DreamID.VoidDreamSlugcatUp);
                    }
                    break;
            }
            if (dreamID2 != null)
            {
                familyThread++;
                upcomingDream = dreamID2;
                cyclesSinceLastDream = 0;
                cyclesSinceLastFamilyDream = 0;
                return;
            }
        }
        
        if (saveState.saveStateNumber == 0 && saveState.swallowedItems != null && 0 < saveState.swallowedItems.Length && saveState.swallowedItems[0] != string.Empty && saveState.swallowedItems[0] != "0")
        {
            AbstractPhysicalObject.AbstractObjectType abstractObjectType;
            string text = saveState.swallowedItems[0];
            if (text.Contains("<oA>"))
            {
                abstractObjectType = AbstractPhysicalObjectTypeFromString(text);
                if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.DataPearl)
                {
                    patch_DreamsState.DreamID? dreamID2 = null;
                    string[] array = Regex.Split(text, "<oA>");
                    patch_DataPearl.patch_AbstractDataPearl.DataPearlType pearlType = (patch_DataPearl.patch_AbstractDataPearl.DataPearlType)int.Parse(array[5]);
                    switch (pearlType)
                    {
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.LF_west: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlLF;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.LF_bottom: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlLF2;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.HI: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlHI;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SH: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSH;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.DS: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlDS;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SB_filtration:  //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSB;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SB_ravine: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSB2;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.GW: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlGW;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SL_bridge: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSL;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SL_moon: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSL2;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SL_chimney: //Purposed Organisms
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSL3;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.DroughtPearl1: //IS Pearl
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlIS;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.DroughtPearl2: //FS Pearl
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlFS;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.DroughtPearl3: //MW Pearl
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlMW;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SI_west: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSI;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SI_top: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSI2;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SI_Spire1: //
                            //dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSI3;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SI_Spire2: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSI4;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SI_Spire3: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSI5;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.SU: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlSU;
                            break;
                        case patch_DataPearl.patch_AbstractDataPearl.DataPearlType.UW: //
                            dreamID2 = patch_DreamsState.DreamID.SRSDreamPearlUW;
                            break;
                    }
                    if (dreamID2 != null)
                    {
                        upcomingDream = (DreamsState.DreamID)dreamID2;
                        cyclesSinceLastDream = 0;
                        return;
                    }
                }
            }
        }
    }

    static private AbstractPhysicalObject.AbstractObjectType AbstractPhysicalObjectTypeFromString(string objString)
    {
        string[] array = Regex.Split(objString, "<oA>");
        try
        {
            AbstractPhysicalObject.AbstractObjectType abstractObjectType = Custom.ParseEnum<AbstractPhysicalObject.AbstractObjectType>(array[1]);
            return abstractObjectType;
        }
        catch
        {
            return 0;
        }
    }

    // Token: 0x06000D6C RID: 3436 RVA: 0x0008D4A8 File Offset: 0x0008B6A8
    public void InitiateEventDream(DreamsState.DreamID evDreamID)
    {
        if (evDreamID == DreamsState.DreamID.MoonThief)
        {
            this.everAteMoonNeuron = true;
        }
        switch (evDreamID)
        {
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlLF:
                everReadLF = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlLF2:
                everReadLF2 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlHI:
                everReadHI = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSH:
                everReadSH = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlDS:
                everReadDS = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSB:
                everReadSB = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSB2:
                everReadSB2 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlGW:
                everReadGW = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSL:
                everReadSL = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSL2:
                everReadSL2 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSL3:
                //everReadSL3 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI:
                everReadSI = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI2:
                everReadSI2 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI3:
                everReadSI3 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI4:
                everReadSI4 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSI5:
                everReadSI5 = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlSU:
                everReadSU = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlUW:
                everReadUW = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlIS:
                everReadIS = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlFS:
                everReadFS = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlMW:
                everReadMW = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamTraitor:
                everSeenTraitor = true;
                everReadMW = true;
                everReadFS = true;
                everReadIS = true;
                everReadUW = true;
                everReadSU = true;
                everReadSI5 = true;
                everReadSI4 = true;
                everReadSI3 = true;
                everReadSI2 = true;
                everReadSI = true;
                everReadSL3 = true;
                everReadSL2 = true;
                everReadSL = true;
                everReadGW = true;
                everReadSB2 = true;
                everReadSB = true;
                everReadDS = true;
                everReadSH = true;
                everReadHI = true;
                everReadLF2 = true;
                everReadLF = true;
                break;
            case (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamMissonComplete:
                everSeenMissonComplete = true;
                break;
        }
        if (this.eventDream == null || evDreamID > this.eventDream.Value)
        {
            this.eventDream = new DreamsState.DreamID?(evDreamID);
        }
    }
    
    public int[] integers;

// Token: 0x04000B37 RID: 2871
    public DreamsState.DreamID? eventDream;

// Token: 0x04000B38 RID: 2872
    private DreamsState.DreamID? upcomingDream;

// Token: 0x04000B39 RID: 2873
    public bool guideHasShownMoonThisRound;

// Token: 0x04000B3A RID: 2874
    public bool guideHasShownHimselfToPlayer;

// Token: 0x02000244 RID: 580
    public enum DreamID
    {
        MoonFriend,
        MoonThief,
        Pebbles,
        GuideA,
        GuideB,
        GuideC,
        FamilyA,
        FamilyB,
        FamilyC,
        VoidDreamSlugcatUp,
        VoidDreamSlugcatDown,

        SRSDreamPearlLF,
        SRSDreamPearlLF2,
        SRSDreamPearlHI,
        SRSDreamPearlSH,
        SRSDreamPearlDS,
        SRSDreamPearlSB,
        SRSDreamPearlSB2,
        SRSDreamPearlGW,
        SRSDreamPearlSL,//Iterators and Water
        SRSDreamPearlSL2,//Moon's Essay
        SRSDreamPearlSL3,//Purposed Organisms

        SRSDreamPearlSI,//FP and SRS Frustration
        SRSDreamPearlSI2,//Erratic Pulse
        SRSDreamPearlSI3,//Silver of Straw
        SRSDreamPearlSI4,//Moon and FP relations
        SRSDreamPearlSI5,//About Messenger
        SRSDreamPearlSU,
        SRSDreamPearlUW,
        SRSDreamPearlIS,//Drought Pearl IS
        SRSDreamPearlFS,//Drought Pearl FS
        SRSDreamPearlMW,//Drought Pearl MW
        SRSDreamTraitor,
        SRSDreamMissonComplete
}

    

}

