using MonoMod;
using System;
using System.Runtime.CompilerServices;
using RWCustom;
class patch_ScavengerOutpost : ScavengerOutpost
{
    [MonoModIgnore]
    public patch_ScavengerOutpost(PlacedObject placedObj, Room room) : base(placedObj, room)
    {
    }

    public override void Update(bool eu)
    {
        this.evenUpdate = eu;

        if (!this.initiated && this.room.fullyLoaded)
        {
            this.team.Clear();
            for (int i = 0; i < this.room.abstractRoom.creatures.Count; i++)
            {
                if (this.room.abstractRoom.creatures[i].realizedCreature != null && this.room.abstractRoom.creatures[i].realizedCreature is Scavenger && this.ScavToBeTracked(this.room.abstractRoom.creatures[i].realizedCreature as Scavenger))
                {
                    (this.room.abstractRoom.creatures[i].realizedCreature as Scavenger).AI.outpostModule.outpost = this;
                    this.team.Add(this.room.abstractRoom.creatures[i].realizedCreature as Scavenger);
                    this.SortTeam();
                }
            }
            for (int j = 0; j < this.pearlStrings.Count; j++)
            {
                this.pearlStrings[j].Initiate();
            }
            this.initiated = true;
        }
        for (int k = this.playerEnteredRoomWithItems.Count - 1; k >= 0; k--)
        {
            if (this.playerEnteredRoomWithItems[k].realizedObject != null)
            {
                if (this.playerEnteredRoomWithItems[k].realizedObject.grabbedBy.Count > 0 && this.playerEnteredRoomWithItems[k].realizedObject.grabbedBy[0].grabber is Scavenger && this.room.game.Players[0].realizedCreature != null)
                {
                    this.FeeRecieved(this.room.game.Players[0].realizedCreature as Player, this.playerEnteredRoomWithItems[k], (this.playerEnteredRoomWithItems[k].realizedObject.grabbedBy[0].grabber as Scavenger).AI.CollectScore(this.playerEnteredRoomWithItems[k].realizedObject, false));
                    this.playerEnteredRoomWithItems.RemoveAt(k);
                }
                else if (this.playerEnteredRoomWithItems[k].realizedObject.room != null && this.playerEnteredRoomWithItems[k].realizedObject.room != this.room)
                {
                    this.playerEnteredRoomWithItems.RemoveAt(k);
                }
            }
        }
        if (this.room.abstractRoom.creatures.Count > 0)
        {
            AbstractCreature abstractCreature = this.room.abstractRoom.creatures[UnityEngine.Random.Range(0, this.room.abstractRoom.creatures.Count)];
            if (abstractCreature.realizedCreature != null && abstractCreature.realizedCreature is Scavenger && this.ScavToBeTracked(abstractCreature.realizedCreature as Scavenger) && !this.team.Contains(abstractCreature.realizedCreature as Scavenger))
            {
                (abstractCreature.realizedCreature as Scavenger).AI.outpostModule.outpost = this;
                this.team.Add(abstractCreature.realizedCreature as Scavenger);
                this.SortTeam();
            }
        }
        for (int l = this.team.Count - 1; l >= 0; l--)
        {
            if (!this.ScavToBeTracked(this.team[l]))
            {
                if (this.team[l].AI.outpostModule.outpost == this)
                {
                    this.team[l].AI.outpostModule.outpost = null;
                }
                this.team.RemoveAt(l);
                this.SortTeam();
            }
        }
        if (this.room.abstractRoom.entities.Count > 0)
        {
            AbstractWorldEntity abstractWorldEntity = this.room.abstractRoom.entities[UnityEngine.Random.Range(0, this.room.abstractRoom.entities.Count)];
            if (abstractWorldEntity is AbstractPhysicalObject && (abstractWorldEntity as AbstractPhysicalObject).type == AbstractPhysicalObject.AbstractObjectType.Spear && (abstractWorldEntity as AbstractPhysicalObject).realizedObject != null && (abstractWorldEntity as AbstractPhysicalObject).stuckObjects.Count == 0 && Custom.DistLess((abstractWorldEntity as AbstractPhysicalObject).realizedObject.firstChunk.pos, this.placedObj.pos, this.Rad) && !this.outPostProperty.Contains(abstractWorldEntity as AbstractPhysicalObject))
            {
                this.outPostProperty.Add(abstractWorldEntity as AbstractPhysicalObject);
            }
            if (this.outPostProperty.Count > 0)
            {
                AbstractPhysicalObject abstractPhysicalObject = this.outPostProperty[UnityEngine.Random.Range(0, this.outPostProperty.Count)];
                if (abstractPhysicalObject.stuckObjects.Count > 0 || (abstractPhysicalObject.realizedObject != null && !Custom.DistLess(abstractPhysicalObject.realizedObject.firstChunk.pos, this.placedObj.pos, this.Rad + 200f)))
                {
                    this.outPostProperty.Remove(abstractPhysicalObject);
                }
            }
        }
        for (int m = 0; m < this.playerTrackers.Count; m++)
        {
            this.playerTrackers[m].Update();
        }/*
        if (this.room.game.session is StoryGameSession && !(this.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ScavTollMessage && this.room.ViewedByAnyCamera(this.antlerPos, 20f))
        {
            (this.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ScavTollMessage = true;
            this.room.game.cameras[0].hud.textPrompt.AddMessage(this.room.game.rainWorld.inGameTranslator.Translate("Scavenger Toll"), 0, 120, true, true);
        }*/
    }
}
