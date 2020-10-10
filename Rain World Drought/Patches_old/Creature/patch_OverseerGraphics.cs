using MonoMod;
using UnityEngine;

class patch_OverseerGraphics : OverseerGraphics
{
    [MonoModIgnore]
    public patch_OverseerGraphics(PhysicalObject ow) : base(ow)
    {
    }

    public Color MainColor
    {
        get
        {
            if (this.overseer.PlayerGuide)
            {
                return new Color(1f, 0.2f, 0.1f);//SRS
            }
            if ((this.overseer.abstractCreature.abstractAI as OverseerAbstractAI).ownerIterator == 0)
            {
                return new Color(0.447058827f, 0.9019608f, 0.768627465f); //FP
            }
            if (this.overseer.SandboxOverseer)
            {
                return PlayerGraphics.SlugcatColor(this.overseer.editCursor.playerNumber);
            }
            if ((this.overseer.abstractCreature.abstractAI as OverseerAbstractAI).ownerIterator == 3){
                return new Color(1f, 0.8f, 0.3f);//LTTM
            }
            if ((this.overseer.abstractCreature.abstractAI as OverseerAbstractAI).ownerIterator == 2)
            {
                return new Color(0f, 1f, 0f);//NSH
            }
            return new Color(0.447058827f, 0.9019608f, 0.768627465f);//FP
        }
    }
}
