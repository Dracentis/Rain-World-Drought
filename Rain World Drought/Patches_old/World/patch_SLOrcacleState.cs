using System;
using MonoMod;
using RWCustom;
using UnityEngine;
using System.Text;
using System.Collections.Generic;


class patch_SLOrcacleState : SLOrcacleState
{
    [MonoModIgnore]
    public patch_SLOrcacleState(bool isDebugState, int saveStateNumber) : base(isDebugState, saveStateNumber)
    {
    }

    [MonoModIgnore]
    private int[] integers;

    [MonoModIgnore]
    public extern void orig_ctor(bool isDebugState, int saveStateNumber);

    [MonoModConstructor]
    public void ctor(bool isDebugState, int saveStateNumber)
    {
        increaseLikeOnSave = true;
        this.isDebugState = isDebugState;
        integers = new int[14];
        miscBools = new bool[1];
        significantPearls = new bool[Enum.GetNames(typeof(DataPearl.AbstractDataPearl.DataPearlType)).Length];
        miscItemsDescribed = new bool[Enum.GetNames(typeof(LMOracleBehaviorHasMark.MiscItemType)).Length];
        likesPlayer = 0.3f;
        neuronsLeft = ((saveStateNumber != 2) ? 5 : 0);
        playerEncountersWithMark = 0;
        alreadyTalkedAboutItems = new List<EntityID>();
        chatLogA = -1;
        chatLogB = -1;
    }
}