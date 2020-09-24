using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;

// Token: 0x0200045D RID: 1117
public class WalkerBeastAI : ArtificialIntelligence, IUseARelationshipTracker, ILookingAtCreatures
{
    // Token: 0x06001C19 RID: 7193 RVA: 0x00191098 File Offset: 0x0018F298
    public WalkerBeastAI(AbstractCreature creature, World world) : base(creature, world)
    {
        WalkerBeast = (creature.realizedCreature as WalkerBeast);
        WalkerBeast.AI = this;
        AddModule(new WalkerBeastPather(this, world, creature));
        pathFinder.accessibilityStepsPerFrame = 60;
        pathFinder.stepsPerFrame = 30;
        AddModule(new Tracker(this, 10, 5, 250, 0.5f, 5, 5, 20));
        AddModule(new PreyTracker(this, 5, 1f, 10f, 25f, 0.95f));
        AddModule(new RainTracker(this));
        AddModule(new DenFinder(this, creature));
        AddModule(new StuckTracker(this, true, false));
        AddModule(new RelationshipTracker(this, tracker));
        AddModule(new ThreatTracker(this, 3));
        AddModule(new UtilityComparer(this));
        stuckTracker.checkPastPositionsFrom = 0;
        stuckTracker.totalTrackedLastPositions = 40;
        stuckTracker.pastStuckPositionsCloseToIncrementStuckCounter = 35;
        stuckTracker.minStuckCounter = 140;
        stuckTracker.maxStuckCounter = 240;
        stuckTracker.goalSatisfactionDistance = 7;
        stuckTracker.AddSubModule(new StuckTracker.GetUnstuckPosCalculator(stuckTracker));
        utilityComparer.AddComparedModule(threatTracker, null, 0.9f, 1.1f);
        utilityComparer.AddComparedModule(preyTracker, null, 0.5f, 1.1f);
        utilityComparer.AddComparedModule(rainTracker, null, 0.9f, 1.1f);
        utilityComparer.AddComparedModule(stuckTracker, null, 1f, 1.1f);
        behavior = Behavior.Idle;
        creatureLooker = new CreatureLooker(this, tracker, creature.realizedCreature, 0.0025f, 30);
    }

    AIModule IUseARelationshipTracker.ModuleToTrackRelationship(CreatureTemplate.Relationship relationship)
    {
        CreatureTemplate.Relationship.Type type = relationship.type;
        if (type == CreatureTemplate.Relationship.Type.Eats)
        {
            return preyTracker;
        }
        if (type != CreatureTemplate.Relationship.Type.Afraid)
        {
            return null;
        }
        return threatTracker;
    }

    RelationshipTracker.TrackedCreatureState IUseARelationshipTracker.CreateTrackedCreatureState(RelationshipTracker.DynamicRelationship rel)
    {
        return null;
    }

    CreatureTemplate.Relationship IUseARelationshipTracker.UpdateDynamicRelationship(RelationshipTracker.DynamicRelationship dRelation)
    {
        return dRelation.currentRelationship;
    }

    public bool AllowMovementBetweenRooms
    {
        get
        {
            return enteredRoom;
        }
    }

    public override void NewRoom(Room newRoom)
    {
        base.NewRoom(newRoom);
        enteredRoom = false;
        boredofroom = 0;
    }

    public override void Update()
    {
        base.Update();
        
        creatureLooker.Update();
        for (int i = tracker.CreaturesCount - 1; i >= 0; i--)
        {
            if (tracker.GetRep(i).TicksSinceSeen > 160)
            {
                tracker.ForgetCreature(tracker.GetRep(i).representedCreature);
            }
        }
        if (!enteredRoom && WalkerBeast.room != null && creature.pos.x > 2 && creature.pos.x < WalkerBeast.room.TileWidth - 3)
        {
            enteredRoom = true;
        }
        if (enteredRoom)
        {
            boredofroom++;
        }
        AIModule aimodule = utilityComparer.HighestUtilityModule();
        currentUtility = utilityComparer.HighestUtility();
        if (aimodule != null)
        {
            if (aimodule is ThreatTracker)
            {
                behavior = Behavior.Flee;
            }
            else if (aimodule is RainTracker)
            {
                behavior = Behavior.EscapeRain;
            }
            else if (aimodule is PreyTracker)
            {
                behavior = Behavior.Hunt;
            }
            else if (aimodule is StuckTracker)
            {
                behavior = Behavior.GetUnstuck;
            }
        }
        if (currentUtility < 0.03f)
        {
            behavior = Behavior.Idle;
        }
        if (WalkerBeast.grasps[0] != null && behavior != Behavior.Flee && behavior != Behavior.EscapeRain)
        {
            behavior = Behavior.ReturnPrey;
        }
        switch (behavior)
        {
            case Behavior.Idle:
                //Debug.Log("time:" + (boredofroom));
                if (boredofroom > 1600)
                {
                    creature.abstractAI.SetDestination(denFinder.GetDenPosition().Value);
                }
                break;
            case Behavior.Flee:
                {
                    WorldCoordinate destination = threatTracker.FleeTo(creature.pos, 5, 30, currentUtility > 0.3f);
                    if (threatTracker.mostThreateningCreature != null)
                    {
                        focusCreature = threatTracker.mostThreateningCreature;
                    }
                    creature.abstractAI.SetDestination(destination);
                    break;
                }
            case Behavior.Hunt:
                focusCreature = preyTracker.MostAttractivePrey;
                creature.abstractAI.SetDestination(preyTracker.MostAttractivePrey.BestGuessForPosition());
                break;
            case Behavior.EscapeRain:
                if (denFinder.GetDenPosition() != null)
                {
                    creature.abstractAI.SetDestination(denFinder.GetDenPosition().Value);
                }
                break;
            case Behavior.ReturnPrey:
                if (denFinder.GetDenPosition() != null)
                {
                    creature.abstractAI.SetDestination(denFinder.GetDenPosition().Value);
                }
                break;
            case Behavior.GetUnstuck:
                creature.abstractAI.SetDestination(stuckTracker.getUnstuckPosCalculator.unstuckGoalPosition);
                break;
        }
    }


    private bool LegalInRoomDest(WorldCoordinate rndm)
    {
        return false;
    }
    
    private bool GoodInRoomDest(WorldCoordinate rndm)
    {
        return false;
    }

    public void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
    {
       
    }

    
    public override float VisualScore(Vector2 lookAtPoint, float targetSpeed)
    {
        return base.VisualScore(lookAtPoint, targetSpeed) - Mathf.InverseLerp(0.7f, 0.3f, Vector2.Dot((WalkerBeast.neck.Tip.pos - lookAtPoint).normalized, (WalkerBeast.neck.Tip.pos - WalkerBeast.Head.pos).normalized));
    }
    
    public bool DoIWantToBiteCreature(AbstractCreature creature)
    {
        return creature.creatureTemplate.type != (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast && !creature.creatureTemplate.smallCreature;
    }

    public override bool WantToStayInDenUntilEndOfCycle()
    {
        return creature.world.rainCycle.TimeUntilRain < (creature.world.game.IsStorySession ? 60 : 15) * 40;
    }
    
    public override void CreatureSpotted(bool firstSpot, Tracker.CreatureRepresentation creatureRep)
    {
        creatureLooker.ReevaluateLookObject(creatureRep, (!firstSpot) ? 2f : 6f);
    }
    
    public override Tracker.CreatureRepresentation CreateTrackerRepresentationForCreature(AbstractCreature otherCreature)
    {
        Tracker.CreatureRepresentation result;
        if (otherCreature.creatureTemplate.smallCreature)
        {
            result = new Tracker.SimpleCreatureRepresentation(tracker, otherCreature, 0f, false);
        }
        else
        {
            result = new Tracker.ElaborateCreatureRepresentation(tracker, otherCreature, 1f, 3);
        }
        return result;
    }
    
    public override PathCost TravelPreference(MovementConnection coord, PathCost cost)
    {
        if (!coord.destinationCoord.TileDefined || coord.destinationCoord.room != WalkerBeast.room.abstractRoom.index)
        {
            return cost;
        }
        return new PathCost(cost.resistance + Mathf.Abs(5f - (float)WalkerBeast.room.aimap.getAItile(coord.DestTile).floorAltitude) * 150f, cost.legality);
    }
    
    public float CreatureInterestBonus(Tracker.CreatureRepresentation crit, float score)
    {
        if (crit.representedCreature.creatureTemplate.smallCreature)
        {
            return -1f;
        }
        if (crit == focusCreature)
        {
            return score * 10f;
        }
        return score;
    }
    
    public Tracker.CreatureRepresentation ForcedLookCreature()
    {
        return null;
    }
    
    public void LookAtNothing()
    {
    }
    
    public bool TrackItem(AbstractPhysicalObject obj)
    {
        return obj.type == AbstractPhysicalObject.AbstractObjectType.PuffBall;
    }
    
    public void SeeThrownWeapon(PhysicalObject obj, Creature thrower)
    {
    }
    
    public WalkerBeast WalkerBeast;

    public CreatureLooker creatureLooker;

    public Tracker.CreatureRepresentation focusCreature;

    //Tracking variables
    public bool enteredRoom;
    public float currentUtility;
    public int seriouslyStuck;
    public int boredofroom;


    public WorldCoordinate inRoomDestination;

    private DebugDestinationVisualizer debugDestinationVisualizer;

    public WalkerBeastAI.Behavior behavior;


    public enum Behavior
    {
        // Token: 0x04002350 RID: 9040
        Idle,
        // Token: 0x04002351 RID: 9041
        Flee,
        // Token: 0x04002352 RID: 9042
        Hunt,
        // Token: 0x04002353 RID: 9043
        EscapeRain,
        // Token: 0x04002354 RID: 9044
        ReturnPrey,
        // Token: 0x04002355 RID: 9045
        GetUnstuck
    }

}
