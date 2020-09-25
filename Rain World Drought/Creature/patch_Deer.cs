using MonoMod;

class patch_Deer : Deer
{
    [MonoModIgnore]
    public patch_Deer(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
    }

    public extern void orig_Update(bool eu);

    public void Update(bool eu)
    {
        orig_Update(eu);
        // Don't die from rain
        rainDeath = 0f;
    }
}