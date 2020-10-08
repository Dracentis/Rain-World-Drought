using System;
using RWCustom;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class patch_RainTracker : RainTracker
{
    public override float Utility()
    {
        if (this.AI.creature.world.game.IsStorySession)
        {
            return Mathf.InverseLerp(1f, 0f, Custom.SCurve(Mathf.InverseLerp(800f, 4000f, (float)Mathf.Min(this.rainCycle.TimeUntilRain, Mathf.Abs((float)(this.rainCycle as patch_RainCycle).TimeUntilBurst((this.rainCycle as patch_RainCycle).CurrentBurst())))), 0.1f));
        }
        return Mathf.InverseLerp(1f, 0f, Custom.SCurve(Mathf.InverseLerp(80f, 400f, (float)Mathf.Min(this.rainCycle.TimeUntilRain, Mathf.Abs((float)(this.rainCycle as patch_RainCycle).TimeUntilBurst((this.rainCycle as patch_RainCycle).CurrentBurst())))), 0.5f));
    }
    
    private RainCycle rainCycle;

    [MonoMod.MonoModIgnore]
    public patch_RainTracker(ArtificialIntelligence AI) : base(AI)
    {
    }
    
}