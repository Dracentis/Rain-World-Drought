using System;
using System.Collections.Generic;
using Noise;
using RWCustom;
using UnityEngine;

class patch_NoiseTracker : NoiseTracker
{
    [MonoMod.MonoModIgnore]
    public patch_NoiseTracker(ArtificialIntelligence AI, Tracker tracker) : base(AI, tracker)
    {
    }
    

    public void HeardNoise(InGameNoise noise)
    {
        if (this.AI is SeaDrakeAI)
        {
            if (!Custom.DistLess(this.AI.creature.realizedCreature.mainBodyChunk.pos, noise.pos, noise.strength * (1f - this.AI.creature.realizedCreature.Deaf) * this.hearingSkill) || !this.AI.creature.realizedCreature.room.PointSubmerged(noise.pos))
            {
                return;
            }
        }
        else if (!Custom.DistLess(this.AI.creature.realizedCreature.mainBodyChunk.pos, noise.pos, noise.strength * (1f - this.AI.creature.realizedCreature.Deaf) * this.hearingSkill * (1f - this.room.BackgroundNoise) * ((!this.AI.creature.realizedCreature.room.PointSubmerged(noise.pos) && !this.AI.creature.realizedCreature.room.GetTile(this.AI.creature.realizedCreature.mainBodyChunk.pos).DeepWater) ? 1f : 0.2f)))
        {
            return;
        }
        if (this.ignoreSeenNoises)
        {
            if (this.AI.VisualContact(noise.pos, 0f))
            {
                return;
            }
            Tracker.CreatureRepresentation creatureRepresentation = this.tracker.RepresentationForObject(noise.sourceObject, false);
            if (creatureRepresentation == null)
            {
                int num = 0;
                while (num < noise.sourceObject.grabbedBy.Count && creatureRepresentation == null)
                {
                    creatureRepresentation = this.tracker.RepresentationForObject(noise.sourceObject.grabbedBy[num].grabber, false);
                    num++;
                }
            }
            if (creatureRepresentation != null && creatureRepresentation.VisualContact)
            {
                return;
            }
        }
        float num2 = float.MaxValue;
        NoiseTracker.TheorizedSource theorizedSource = null;
        for (int i = 0; i < this.sources.Count; i++)
        {
            float num3 = this.sources[i].NoiseMatch(noise);
            if (num3 < num2 && num3 < ((this.sources[i].creatureRep == null) ? 300f : Custom.LerpMap((float)this.sources[i].creatureRep.TicksSinceSeen, 20f, 600f, 200f, 1000f)))
            {
                num2 = num3;
                theorizedSource = this.sources[i];
            }
        }
        if (theorizedSource != null)
        {
            if (theorizedSource.creatureRep == null && theorizedSource.age > 10)
            {
                this.mysteriousNoises += this.HowInterestingIsThisNoiseToMe(noise);
                this.mysteriousNoiseCounter = 200;
            }
            theorizedSource.Refresh(noise);
        }
        else
        {
            Tracker.CreatureRepresentation creatureRepresentation2 = null;
            num2 = float.MaxValue;
            int num4 = 0;
            for (int j = 0; j < this.tracker.CreaturesCount; j++)
            {
                float num5 = this.NoiseMatch(noise, this.tracker.GetRep(j));
                if (num5 < num2 && num5 < Custom.LerpMap((float)this.tracker.GetRep(j).TicksSinceSeen, 20f, 600f, 200f, 1000f))
                {
                    num2 = num5;
                    creatureRepresentation2 = this.tracker.GetRep(j);
                }
                if (!this.tracker.GetRep(j).VisualContact)
                {
                    num4++;
                }
            }
            if (num2 > Custom.LerpMap((float)num4, 0f, (float)this.tracker.maxTrackedCreatures, 1000f, 300f))
            {
                creatureRepresentation2 = null;
            }
            if (creatureRepresentation2 == null)
            {
                this.mysteriousNoises += this.HowInterestingIsThisNoiseToMe(noise);
                this.mysteriousNoiseCounter = 200;
            }
            theorizedSource = new NoiseTracker.TheorizedSource(this as NoiseTracker, noise.pos, creatureRepresentation2);
            this.sources.Add(theorizedSource);
            theorizedSource.Refresh(noise);
        }
        this.UpdateExamineSound();
        if (this.AI is IAINoiseReaction)
        {
            (this.AI as IAINoiseReaction).ReactToNoise(theorizedSource, noise);
        }
    }
    
    private float NoiseMatch(InGameNoise noise, Tracker.CreatureRepresentation critRep)
    {
        if (critRep.VisualContact && this.ignoreSeenNoises)
        {
            return float.MaxValue;
        }
        float num = Vector2.Distance(noise.pos, this.room.MiddleOfTile(critRep.BestGuessForPosition()));
        if (critRep is Tracker.ElaborateCreatureRepresentation)
        {
            for (int i = 0; i < (critRep as Tracker.ElaborateCreatureRepresentation).ghosts.Count; i++)
            {
                if ((critRep as Tracker.ElaborateCreatureRepresentation).ghosts[i].coord.room == this.room.abstractRoom.index && Custom.DistLess(noise.pos, this.room.MiddleOfTile((critRep as Tracker.ElaborateCreatureRepresentation).ghosts[i].coord), num))
                {
                    num = Vector2.Distance(noise.pos, this.room.MiddleOfTile((critRep as Tracker.ElaborateCreatureRepresentation).ghosts[i].coord));
                }
            }
        }
        return num;
    }

}

