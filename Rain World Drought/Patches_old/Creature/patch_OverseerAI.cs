using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using UnityEngine;

class patch_OverseerAI : OverseerAI
{
    [MonoModIgnore]
    public patch_OverseerAI(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
    }

    public extern void orig_Update();

    public void Update()
    {
        orig_Update();
        if (this.overseer.PlayerGuide && this.droughtTutorialBehavior == null && this.creature.world.game.session is StoryGameSession && (this.creature.world.game.session as StoryGameSession).saveState.cycleNumber == 0 && this.tutorialBehavior == null && this.overseer.room.game.Players.Count > 0 && this.overseer.room.abstractRoom == this.overseer.room.game.Players[0].Room && this.overseer.room.world.region.name == "FS")
        {
            for (int i = 0; i < patch_OverseerAbstractAI.droughtTutorialRooms.Length; i++)
            {
                if (this.overseer.room.game.Players[0].Room.name == patch_OverseerAbstractAI.droughtTutorialRooms[i])
                {
                    Debug.Log("Tutorial Behavior Added.");
                    this.droughtTutorialBehavior = new OverseerDroughtTutorialBehavior(this);
                    base.AddModule(this.droughtTutorialBehavior);
                    break;
                }
            }
        }
    }

    public OverseerDroughtTutorialBehavior droughtTutorialBehavior;
}

