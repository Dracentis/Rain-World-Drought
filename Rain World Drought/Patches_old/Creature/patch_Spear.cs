using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using UnityEngine;

class patch_Spear : Spear
{
    [MonoModIgnore]
    public patch_Spear(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
    }

    public extern void orig_Update(bool eu);

    public override void Update(bool eu)
    {
        try
        {
            orig_Update(eu);
        }
        catch (Exception e)
        {
            this.Destroy();
            Debug.LogException(e);
        }
    }
}
