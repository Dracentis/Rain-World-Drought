using System;
using UnityEngine;
using MonoMod;

public class patch_LizardBreedParams : LizardBreedParams
{
    [MonoModIgnore]
    public patch_LizardBreedParams(patch_CreatureTemplate.Type template) : base((CreatureTemplate.Type)template)
    {
    }

    public extern void orig_ctor(CreatureTemplate.Type template);

    [MonoModConstructor]
    public void ctor(patch_CreatureTemplate.Type template)
    {
        orig_ctor((CreatureTemplate.Type)template);
    }

}
