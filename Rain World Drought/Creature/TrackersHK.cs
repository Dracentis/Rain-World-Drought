using Noise;
using Rain_World_Drought.OverWorld;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    internal static class TrackersHK
    {
        public static void Patch()
        {
            On.NoiseTracker.HeardNoise += new On.NoiseTracker.hook_HeardNoise(NoiseHeardNoiseHK);
            On.RainTracker.Utility += new On.RainTracker.hook_Utility(RainUtilityHK);
        }

        private static void NoiseHeardNoiseHK(On.NoiseTracker.orig_HeardNoise orig, NoiseTracker self, InGameNoise noise)
        {
            if (self.AI is SeaDrakeAI)
            {
                if (!Custom.DistLess(self.AI.creature.realizedCreature.mainBodyChunk.pos, noise.pos, noise.strength * (1f - self.AI.creature.realizedCreature.Deaf) * self.hearingSkill) || !self.AI.creature.realizedCreature.room.PointSubmerged(noise.pos))
                { return; }

                #region orig
                if (self.ignoreSeenNoises)
                {
                    if (self.AI.VisualContact(noise.pos, 0f)) { return; }
                    Tracker.CreatureRepresentation creatureRepresentation = self.tracker.RepresentationForObject(noise.sourceObject, false);
                    if (creatureRepresentation == null)
                    {
                        int num = 0;
                        while (num < noise.sourceObject.grabbedBy.Count && creatureRepresentation == null)
                        {
                            creatureRepresentation = self.tracker.RepresentationForObject(noise.sourceObject.grabbedBy[num].grabber, false);
                            num++;
                        }
                    }
                    if (creatureRepresentation != null && creatureRepresentation.VisualContact) { return; }
                }
                float num2 = float.MaxValue;
                NoiseTracker.TheorizedSource theorizedSource = null;
                for (int i = 0; i < self.sources.Count; i++)
                {
                    float num3 = self.sources[i].NoiseMatch(noise);
                    if (num3 < num2 && num3 < ((self.sources[i].creatureRep == null) ? 300f : Custom.LerpMap((float)self.sources[i].creatureRep.TicksSinceSeen, 20f, 600f, 200f, 1000f)))
                    {
                        num2 = num3;
                        theorizedSource = self.sources[i];
                    }
                }
                if (theorizedSource != null)
                {
                    if (theorizedSource.creatureRep == null && theorizedSource.age > 10)
                    {
                        self.mysteriousNoises += self.HowInterestingIsThisNoiseToMe(noise);
                        self.mysteriousNoiseCounter = 200;
                    }
                    theorizedSource.Refresh(noise);
                }
                else
                {
                    Tracker.CreatureRepresentation creatureRepresentation2 = null;
                    num2 = float.MaxValue;
                    int num4 = 0;
                    for (int j = 0; j < self.tracker.CreaturesCount; j++)
                    {
                        float num5 = self.NoiseMatch(noise, self.tracker.GetRep(j));
                        if (num5 < num2 && num5 < Custom.LerpMap((float)self.tracker.GetRep(j).TicksSinceSeen, 20f, 600f, 200f, 1000f))
                        {
                            num2 = num5;
                            creatureRepresentation2 = self.tracker.GetRep(j);
                        }
                        if (!self.tracker.GetRep(j).VisualContact) { num4++; }
                    }
                    if (num2 > Custom.LerpMap((float)num4, 0f, (float)self.tracker.maxTrackedCreatures, 1000f, 300f))
                    { creatureRepresentation2 = null; }
                    if (creatureRepresentation2 == null)
                    {
                        self.mysteriousNoises += self.HowInterestingIsThisNoiseToMe(noise);
                        self.mysteriousNoiseCounter = 200;
                    }
                    theorizedSource = new NoiseTracker.TheorizedSource(self, noise.pos, creatureRepresentation2);
                    self.sources.Add(theorizedSource);
                    theorizedSource.Refresh(noise);
                }
                self.UpdateExamineSound();
                #endregion orig

                return;
            }
            orig.Invoke(self, noise);
        }

        private static float RainUtilityHK(On.RainTracker.orig_Utility orig, RainTracker self)
        {
            int timeUntilBurst = RainCycleHK.TimeUntilBurst(self.rainCycle, RainCycleHK.CurrentBurst(self.rainCycle));
            if (self.AI.creature.world.game.IsStorySession)
            { return Mathf.InverseLerp(1f, 0f, Custom.SCurve(Mathf.InverseLerp(800f, 4000f, (float)Mathf.Min(self.rainCycle.TimeUntilRain, Mathf.Abs((float)timeUntilBurst))), 0.1f)); }
            return Mathf.InverseLerp(1f, 0f, Custom.SCurve(Mathf.InverseLerp(80f, 400f, (float)Mathf.Min(self.rainCycle.TimeUntilRain, Mathf.Abs((float)timeUntilBurst))), 0.5f));
        }
    }
}
