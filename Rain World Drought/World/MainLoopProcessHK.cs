using Rain_World_Drought.Slugcat;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class MainLoopProcessHK
    {
        public static void Patch()
        {
            On.MainLoopProcess.RawUpdate += RawUpdateHK;
        }

        private static void RawUpdateHK(On.MainLoopProcess.orig_RawUpdate orig, MainLoopProcess self, float dt)
        {
            // Calculate framerate based on wanderer's ability
            if (self is RainWorldGame rwg)
            {
                foreach (AbstractCreature absPly in rwg.Players)
                {
                    Player ply = absPly.realizedCreature as Player;
                    if (ply != null)
                    {
                        if (!WandererSupplement.IsWanderer(ply)) continue;
                        WandererSupplement sub = WandererSupplement.GetSub(ply);

                        if (sub.PanicSlowdown > 0f)
                        {
                            // Slow down as a weapon gets close
                            self.framesPerSecond = Math.Min(self.framesPerSecond, Mathf.CeilToInt(32f * (1f - sub.PanicSlowdown)) + 8);
                        }
                        if (sub.Slowdown > 0f)
                        {
                            self.framesPerSecond = Math.Min(self.framesPerSecond, (int)Custom.LerpMap(sub.Slowdown, 1f, 0.8f, 40f, 15f));
                        }
                    }
                }
            }
            orig(self, dt);
        }
    }
}
