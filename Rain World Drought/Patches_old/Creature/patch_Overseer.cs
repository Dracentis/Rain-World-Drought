using MonoMod;
using OverseerHolograms;
using UnityEngine;

class patch_Overseer : Overseer
{
    [MonoModIgnore]
    public patch_Overseer(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
    }

    public extern void orig_ctor(AbstractCreature abstractCreature, World world);

    [MonoModConstructor]
    public void ctor(AbstractCreature abstractCreature, World world)
    {
        orig_ctor(abstractCreature, world);
        this.size = 1f;
    }


    public void TryAddHologram(OverseerHologram.Message message, Creature communicateWith, float importance)
    {
        if (base.dead)
        {
            return;
        }
        if (this.hologram != null)
        {
            if (this.hologram.message == message)
            {
                return;
            }
            if (this.hologram.importance >= importance && importance != 3.40282347E+38f)
            {
                return;
            }
            this.hologram.stillRelevant = false;
            this.hologram = null;
        }
        switch (message)
        {
            case OverseerHologram.Message.Bats:
            case OverseerHologram.Message.TutorialFood:
                this.hologram = new OverseerHologram.BatPointer(this, message, communicateWith, importance);
                break;
            case OverseerHologram.Message.DangerousCreature:
                this.hologram = new OverseerHologram.CreaturePointer(this, message, communicateWith, importance);
                break;
            case OverseerHologram.Message.FoodObject:
                this.hologram = new OverseerHologram.FoodPointer(this, message, communicateWith, importance);
                break;
            case OverseerHologram.Message.Shelter:
                this.hologram = new OverseerHologram.ShelterPointer(this, message, communicateWith, importance);
                break;
            case OverseerHologram.Message.ProgressionDirection:
                this.hologram = new OverseerHologram.DirectionPointer(this, message, communicateWith, importance);
                break;
            case OverseerHologram.Message.GateScene:
                this.hologram = new OverseerImage(this, message, communicateWith, importance);
                break;
            case OverseerHologram.Message.InWorldSuperJump:
            case OverseerHologram.Message.PickupObject:
            case OverseerHologram.Message.ScavengerTrade:
            case OverseerHologram.Message.GetUpOnFirstBox:
            case OverseerHologram.Message.ClimbPole:
            case OverseerHologram.Message.EatInstruction:
            case OverseerHologram.Message.SuperJump:
                if (this.room.abstractRoom.name.Equals("FS_A01"))
                {
                    Debug.Log("Trying to add tutorial hologram!");
                    if (this.room.game.rainWorld.options.controls[0].gamePad)
                    {
                        this.hologram = new OverseerDroughtTutorialBehavior.GamePadInstruction(this, message, communicateWith, importance);
                    }
                    else
                    {
                        this.hologram = new OverseerDroughtTutorialBehavior.KeyBoardInstruction(this, message, communicateWith, importance);
                    }
                }
                else
                {
                    if (this.room.game.rainWorld.options.controls[0].gamePad)
                    {
                        this.hologram = new OverseerTutorialBehavior.GamePadInstruction(this, message, communicateWith, importance);
                    }
                    else
                    {
                        this.hologram = new OverseerTutorialBehavior.KeyBoardInstruction(this, message, communicateWith, importance);
                    }
                }
                break;
            case OverseerHologram.Message.Angry:
                this.hologram = new AngryHologram(this, message, communicateWith, importance);
                break;
            case OverseerHologram.Message.ForcedDirection:
                this.hologram = new OverseerHologram.ForcedDirectionPointer(this, message, communicateWith, importance);
                break;
        }
        this.room.AddObject(this.hologram);
    }
}
