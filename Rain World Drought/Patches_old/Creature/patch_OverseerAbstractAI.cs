using MonoMod;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using System;

public class patch_OverseerAbstractAI : OverseerAbstractAI
{
    [MonoModIgnore]
    public patch_OverseerAbstractAI(World world, AbstractCreature parent) : base(world, parent)
    {
    }

    public extern void orig_ctor(World world, AbstractCreature parent);

    [MonoModConstructor]
    public void ctor(World world, AbstractCreature parent)
    {
        patch_OverseerAbstractAI.droughtTutorialRooms = new string[]
        {
            "FS_A01"
        };
        orig_ctor(world, parent);
        this.ownerIterator = 0;
        // 3 = LTTM
        // 2 = NSH
        // 0 = FP
        if (!world.singleRoomWorld && !this.playerGuide)
        {
            if (world.region.name == "SB")
            {
                this.ownerIterator = 2;//NSH
            }
            else if (world.region.name == "UW")
            {
                this.ownerIterator = (UnityEngine.Random.value > 0.1) ? 0 : 3;//Mostly FP
            }
            else if (world.region.name == "CC" || world.region.name == "SH")
            {
                this.ownerIterator = (UnityEngine.Random.value > 0.3) ? 0 : 3;//Mostly FP
            }
            else if (world.region.name == "MW")
            {
                this.ownerIterator = (UnityEngine.Random.value > 0.9) ? 0 : 3;//Mostly LTTM
            }
            else if (world.region.name == "SL" || world.region.name == "IS")
            {
                this.ownerIterator = (UnityEngine.Random.value > 0.7) ? 0 : 3;//Mostly LTTM
            }
            else
            {
                this.ownerIterator = (UnityEngine.Random.value > 0.5) ? 0 : 3;//RANDOM
            }
        }
    }

    public static string[] droughtTutorialRooms;
}
