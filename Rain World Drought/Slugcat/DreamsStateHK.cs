using Rain_World_Drought.Enums;
using RWCustom;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rain_World_Drought.Slugcat
{
    internal static class DreamsStateHK
    {
        public static void Patch()
        {
            On.DreamsState.ctor += new On.DreamsState.hook_ctor(CtorHK);
            On.DreamsState.EndOfCycleProgress += new On.DreamsState.hook_EndOfCycleProgress(EndOfCycleProgressHK);
            On.DreamsState.StaticEndOfCycleProgress += new On.DreamsState.hook_StaticEndOfCycleProgress(StaticEndOfCycleProgressHK);
            On.DreamsState.InitiateEventDream += new On.DreamsState.hook_InitiateEventDream(InitiateEventDreamHK);
        }

        private static void CtorHK(On.DreamsState.orig_ctor orig, DreamsState self)
        {
            orig.Invoke(self);
            origIntegers = self.integers.Length;
            droughtIntegers = Enum.GetNames(typeof(EnumSwitch.DreamsStateID)).Length - 1;
            self.integers = new int[origIntegers + droughtIntegers];
        }

        public static int origIntegers = 9; // Vanilla = 9, but made it not hardcoded
        public static int droughtIntegers = 23;

        public static bool GetEverReadMessage(DreamsState self, EnumSwitch.DreamsStateID message) => self.integers[(int)message + origIntegers] == 1;

        public static void SetEverReadMessage(DreamsState self, EnumSwitch.DreamsStateID message, bool value) => self.integers[(int)message + origIntegers] = value ? 1 : 0;

        public static bool AnyMessageComingUp(DreamsState state)
        {
            bool res = state.upcomingDream != null && EnumSwitch.GetDreamsStateID(state.upcomingDream.Value) != EnumSwitch.DreamsStateID.DEFAULT;
            Debug.Log($"Drought) Any message coming up? {res}");
            return res;
        }

        private static void EndOfCycleProgressHK(On.DreamsState.orig_EndOfCycleProgress orig, DreamsState self,
            SaveState saveState, string currentRegion, string denPosition)
        {
            orig.Invoke(self, saveState, currentRegion, denPosition);

            if (self.upcomingDream != null)
            {
                EnumSwitch.DreamsStateID id = EnumSwitch.GetDreamsStateID(self.upcomingDream.Value);
                if (id != EnumSwitch.DreamsStateID.DEFAULT)
                {
                    if (GetEverReadMessage(self, id)) { self.upcomingDream = null; }
                    SetEverReadMessage(self, id, true);
                }
            }
        }

        private static void StaticEndOfCycleProgressHK(On.DreamsState.orig_StaticEndOfCycleProgress orig, SaveState saveState, string currentRegion, string denPosition, ref int cyclesSinceLastDream, ref int cyclesSinceLastFamilyDream, ref int cyclesSinceLastGuideDream, ref int inGWOrSHCounter, ref DreamsState.DreamID? upcomingDream, ref DreamsState.DreamID? eventDream, ref bool everSleptInSB, ref bool everSleptInSB_S01, ref bool guideHasShownHimselfToPlayer, ref int guideThread, ref bool guideHasShownMoonThisRound, ref int familyThread)
        {
            orig.Invoke(saveState, currentRegion, denPosition, ref cyclesSinceLastDream, ref cyclesSinceLastFamilyDream, ref cyclesSinceLastGuideDream, ref inGWOrSHCounter, ref upcomingDream, ref eventDream, ref everSleptInSB, ref everSleptInSB_S01, ref guideHasShownHimselfToPlayer, ref guideThread, ref guideHasShownMoonThisRound, ref familyThread);

            if (DroughtMod.EnumExt && saveState.saveStateNumber == WandererSupplement.StoryCharacter && saveState.swallowedItems != null && saveState.swallowedItems.Length > 0 && !string.IsNullOrEmpty(saveState.swallowedItems[0]) && saveState.swallowedItems[0] != "0")
            {
                AbstractPhysicalObject.AbstractObjectType abstractObjectType;
                string objString = saveState.swallowedItems[0];
                if (objString.Contains("<oA>"))
                {
                    string[] array = Regex.Split(objString, "<oA>");
                    try
                    { abstractObjectType = Custom.ParseEnum<AbstractPhysicalObject.AbstractObjectType>(array[1]); }
                    catch { return; }
                    if (abstractObjectType != AbstractPhysicalObject.AbstractObjectType.DataPearl) { return; }

                    DreamsState.DreamID? dreamID2 = null;
                    DataPearl.AbstractDataPearl.DataPearlType pearlType = (DataPearl.AbstractDataPearl.DataPearlType)int.Parse(array[5]);
                    switch (EnumSwitch.GetAbstractDataPearlType(pearlType))
                    {
                        default:
                        case EnumSwitch.AbstractDataPearlType.DEFAULT:
                            switch (pearlType)
                            {
                                case DataPearl.AbstractDataPearl.DataPearlType.LF_west: dreamID2 = EnumExt_Drought.SRSDreamPearlLF; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.LF_bottom: dreamID2 = EnumExt_Drought.SRSDreamPearlLF2; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.HI: dreamID2 = EnumExt_Drought.SRSDreamPearlHI; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SH: dreamID2 = EnumExt_Drought.SRSDreamPearlSH; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.DS: dreamID2 = EnumExt_Drought.SRSDreamPearlDS; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SB_filtration: dreamID2 = EnumExt_Drought.SRSDreamPearlSB; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SB_ravine: dreamID2 = EnumExt_Drought.SRSDreamPearlSB2; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.GW: dreamID2 = EnumExt_Drought.SRSDreamPearlGW; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SL_bridge: dreamID2 = EnumExt_Drought.SRSDreamPearlSL; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SL_moon: dreamID2 = EnumExt_Drought.SRSDreamPearlSL2; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SL_chimney: dreamID2 = EnumExt_Drought.SRSDreamPearlSL3; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SI_west: dreamID2 = EnumExt_Drought.SRSDreamPearlSI; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SI_top: dreamID2 = EnumExt_Drought.SRSDreamPearlSI2; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.SU: dreamID2 = EnumExt_Drought.SRSDreamPearlSU; break;
                                case DataPearl.AbstractDataPearl.DataPearlType.UW: dreamID2 = EnumExt_Drought.SRSDreamPearlUW; break;
                            }
                            break;

                        case EnumSwitch.AbstractDataPearlType.DroughtPearl1: dreamID2 = EnumExt_Drought.SRSDreamPearlIS; break;
                        case EnumSwitch.AbstractDataPearlType.DroughtPearl2: dreamID2 = EnumExt_Drought.SRSDreamPearlFS; break;
                        case EnumSwitch.AbstractDataPearlType.DroughtPearl3: dreamID2 = EnumExt_Drought.SRSDreamPearlMW; break;
                        case EnumSwitch.AbstractDataPearlType.SI_Spire1: dreamID2 = EnumExt_Drought.SRSDreamPearlSI3; break;
                        case EnumSwitch.AbstractDataPearlType.SI_Spire2: dreamID2 = EnumExt_Drought.SRSDreamPearlSI4; break;
                        case EnumSwitch.AbstractDataPearlType.SI_Spire3: dreamID2 = EnumExt_Drought.SRSDreamPearlSI5; break;
                    }

                    if (dreamID2 != null)
                    {
                        upcomingDream = dreamID2;
                        cyclesSinceLastDream = 0;
                        return;
                    }
                }
            }
        }

        private static void InitiateEventDreamHK(On.DreamsState.orig_InitiateEventDream orig, DreamsState self, DreamsState.DreamID evDreamID)
        {
            orig.Invoke(self, evDreamID);
            switch (EnumSwitch.GetDreamsStateID(evDreamID))
            {
                case EnumSwitch.DreamsStateID.SRSDreamPearlLF:
                case EnumSwitch.DreamsStateID.SRSDreamPearlLF2:
                case EnumSwitch.DreamsStateID.SRSDreamPearlHI:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSH:
                case EnumSwitch.DreamsStateID.SRSDreamPearlDS:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSB:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSB2:
                case EnumSwitch.DreamsStateID.SRSDreamPearlGW:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSL:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSL2:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSL3: //Disabled?
                case EnumSwitch.DreamsStateID.SRSDreamPearlSI:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSI2:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSI3:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSI4:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSI5:
                case EnumSwitch.DreamsStateID.SRSDreamPearlSU:
                case EnumSwitch.DreamsStateID.SRSDreamPearlUW:
                case EnumSwitch.DreamsStateID.SRSDreamPearlIS:
                case EnumSwitch.DreamsStateID.SRSDreamPearlFS:
                case EnumSwitch.DreamsStateID.SRSDreamPearlMW:
                case EnumSwitch.DreamsStateID.SRSDreamMissonComplete:
                    SetEverReadMessage(self, EnumSwitch.GetDreamsStateID(evDreamID), true); break;

                case EnumSwitch.DreamsStateID.SRSDreamTraitor:
                    EnumSwitch.DreamsStateID[] all = new EnumSwitch.DreamsStateID[]
                    {
                        EnumSwitch.DreamsStateID.SRSDreamTraitor,
                        EnumSwitch.DreamsStateID.SRSDreamPearlMW,
                        EnumSwitch.DreamsStateID.SRSDreamPearlFS,
                        EnumSwitch.DreamsStateID.SRSDreamPearlIS,
                        EnumSwitch.DreamsStateID.SRSDreamPearlUW,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSU,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSI,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSI2,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSI3,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSI4,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSI5,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSL,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSL2,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSL3,
                        EnumSwitch.DreamsStateID.SRSDreamPearlGW,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSB,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSB2,
                        EnumSwitch.DreamsStateID.SRSDreamPearlDS,
                        EnumSwitch.DreamsStateID.SRSDreamPearlSH,
                        EnumSwitch.DreamsStateID.SRSDreamPearlHI,
                        EnumSwitch.DreamsStateID.SRSDreamPearlLF,
                        EnumSwitch.DreamsStateID.SRSDreamPearlLF2
                    };
                    foreach (EnumSwitch.DreamsStateID id in all) { SetEverReadMessage(self, id, true); }
                    break;
            }
        }
    }
}
