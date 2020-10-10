using MonoMod;
using RWCustom;
using UnityEngine;


class patch_LizardAI : LizardAI
{
    [MonoModIgnore]
    public patch_LizardAI(AbstractCreature creature, World world) : base(creature, world)
    {
    }

    public extern void orig_ctor(AbstractCreature creature, World world);

    [MonoModConstructor]
    public void ctor(AbstractCreature creature, World world)
    {
        orig_ctor(creature, world);
        if (creature.creatureTemplate.type == (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard)
        {
            redSpitAI = new LizardAI.LizardSpitTracker(this as LizardAI);
            AddModule(redSpitAI);
        }
    }
    
    public extern void orig_Update();

    public override void Update()
    {
        orig_Update();
        if (creature.creatureTemplate.type == (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard && redSpitAI.spitting)
        {
            (creature.realizedCreature as Lizard).EnterAnimation(Lizard.Animation.Spit, false);
        }
    }
}

