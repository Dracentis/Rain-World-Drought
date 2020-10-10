using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using Noise;

class patch_Creature : Creature
{

    public int rad = 0;//integer to measure amount of time spent near a radio

    [MonoModIgnore]
    patch_Creature(AbstractCreature abstractCreature, World world) : base(abstractCreature, world) { }
    
    public extern void orig_ctor(AbstractCreature abstractCreature, World world);

    [MonoModConstructor]
    public void ctor(AbstractCreature abstractCreature, World world)
    {
        orig_ctor(abstractCreature, world);
        rad = 0;
    }

    public extern void orig_Update(bool eu);

    public void Update(bool eu)
    {
        orig_Update(eu);
        if (rad > 0)
        {
            rad = rad - 1;
        }
    }

    public void Rad()
    {
        if (rad < 549)
        {
            rad = rad + 2;
        }
    }

    public int getRad()
    {
        return rad;
    }

    public virtual void HeardNoise(InGameNoise noise)
	{
		if (this.abstractCreature != null && this.abstractCreature.creatureTemplate != null && this.Template.AI && noise.sourceObject != this && this.Consious && this.abstractCreature.abstractAI != null && this.abstractCreature.abstractAI.RealAI != null)
		{
			this.abstractCreature.abstractAI.RealAI.HeardNoise(noise);
		}
	}

}
