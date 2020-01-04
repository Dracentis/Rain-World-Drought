using System;
using RWCustom;
using UnityEngine;

public class SeaDrakeAI : ArtificialIntelligence, IUseARelationshipTracker
{
    public SeaDrakeAI(AbstractCreature creature, World world) : base(creature, world)
    {
        this.fish = (creature.realizedCreature as SeaDrake);
        this.fish.AI = this;
        base.AddModule(new FishPather(this, world, creature));
        base.AddModule(new Tracker(this, 10, 10, 250, 0.5f, 5, 5, 20));
        
        
        base.AddModule(new NoiseTracker(this, base.tracker));
        base.noiseTracker.forgetTime = 320;
        base.noiseTracker.hearingSkill = 5f;
        base.noiseTracker.ignoreSeenNoises = false;

        base.AddModule(new PreyTracker(this, 5, 1f, 50f, 250f, 0.05f));
        base.AddModule(new ThreatTracker(this, 3));
        base.AddModule(new RainTracker(this));
        base.AddModule(new DenFinder(this, creature));
        base.AddModule(new RelationshipTracker(this, base.tracker));
        base.AddModule(new UtilityComparer(this));
        base.AddModule(new StuckTracker(this, true, false));
        base.stuckTracker.AddSubModule(new StuckTracker.GetUnstuckPosCalculator(base.stuckTracker));
        base.stuckTracker.AddSubModule(new StuckTracker.StuckCloseToShortcutModule(base.stuckTracker));
        base.utilityComparer.AddComparedModule(base.threatTracker, null, 1f, 1.1f);
        base.utilityComparer.AddComparedModule(base.preyTracker, null, 2f, 1.1f);
        base.utilityComparer.AddComparedModule(base.noiseTracker, null, 1.5f, 1.1f);
        base.utilityComparer.AddComparedModule(base.rainTracker, null, 2f, 1.1f);
        base.utilityComparer.AddComparedModule(base.stuckTracker, null, 1f, 1.1f);
        this.behavior = SeaDrakeAI.Behavior.Idle;
    }
    
    AIModule IUseARelationshipTracker.ModuleToTrackRelationship(CreatureTemplate.Relationship relationship)
    {
        CreatureTemplate.Relationship.Type type = relationship.type;
        if (type != CreatureTemplate.Relationship.Type.Eats)
        {
            if (type == CreatureTemplate.Relationship.Type.Afraid)
            {
                return base.threatTracker;
            }
            if (type != CreatureTemplate.Relationship.Type.Antagonizes)
            {
                return null;
            }
        }
        return base.preyTracker;
    }

    // Token: 0x06001E67 RID: 7783 RVA: 0x0018E63A File Offset: 0x0018C83A
    RelationshipTracker.TrackedCreatureState IUseARelationshipTracker.CreateTrackedCreatureState(RelationshipTracker.DynamicRelationship rel)
    {
        return new RelationshipTracker.TrackedCreatureState();
    }

    // Token: 0x06001E68 RID: 7784 RVA: 0x001B23D4 File Offset: 0x001B05D4
    CreatureTemplate.Relationship IUseARelationshipTracker.UpdateDynamicRelationship(RelationshipTracker.DynamicRelationship dRelation)
    {
        if (dRelation.trackerRep.VisualContact)
        {
            dRelation.state.alive = dRelation.trackerRep.representedCreature.state.alive;
            if (dRelation.trackerRep.representedCreature.realizedCreature.grasps != null)
            {
                for (int i = 0; i < dRelation.trackerRep.representedCreature.realizedCreature.grasps.Length; i++)
                {
                    if (dRelation.trackerRep.representedCreature.realizedCreature.grasps[i] != null && dRelation.trackerRep.representedCreature.realizedCreature.grasps[i].grabbed is SeaDrake)
                    {
                        SocialMemory.Relationship orInitiateRelationship = this.fish.State.socialMemory.GetOrInitiateRelationship(dRelation.trackerRep.representedCreature.ID);
                        orInitiateRelationship.like = Mathf.Lerp(orInitiateRelationship.like, 0f, 5E-05f);
                        break;
                    }
                }
            }
        }
        CreatureTemplate.Relationship result = base.StaticRelationship(dRelation.trackerRep.representedCreature).Duplicate();
        return result;
    }

    // Token: 0x170004A8 RID: 1192
    // (get) Token: 0x06001E69 RID: 7785 RVA: 0x001B26D1 File Offset: 0x001B08D1
    // (set) Token: 0x06001E6A RID: 7786 RVA: 0x001B26DC File Offset: 0x001B08DC
    public Vector2? floatGoalPos
    {
        get
        {
            return this.fgp;
        }
        set
        {
            this.fgp = value;
            if (value != null)
            {
                this.creature.abstractAI.SetDestination(this.fish.room.GetWorldCoordinate(value.Value));
            }
        }
    }

    // Token: 0x06001E6B RID: 7787 RVA: 0x00177767 File Offset: 0x00175967
    public override void NewRoom(Room room)
    {
        base.NewRoom(room);
    }

    // Token: 0x06001E6C RID: 7788 RVA: 0x001B2720 File Offset: 0x001B0920
    public override void Update()
    {
        this.focusCreature = null;
        base.Update();
        if (this.getAwayCounter > 0)
        {
            this.getAwayCounter--;
        }
        AIModule aimodule = base.utilityComparer.HighestUtilityModule();
        this.currentUtility = base.utilityComparer.HighestUtility();
        if (aimodule != null)
        {
            if (aimodule is ThreatTracker)
            {
                this.behavior = SeaDrakeAI.Behavior.Flee;
            }
            else if (aimodule is RainTracker)
            {
                this.behavior = SeaDrakeAI.Behavior.EscapeRain;
            }
            else if (aimodule is PreyTracker)
            {
                if (this.creature.realizedCreature != null && preyTracker.MostAttractivePrey.representedCreature.realizedCreature != null && !this.creature.realizedCreature.room.PointSubmerged(preyTracker.MostAttractivePrey.representedCreature.realizedCreature.mainBodyChunk.pos))
                {
                    this.behavior = SeaDrakeAI.Behavior.Idle;
                }
                this.behavior = SeaDrakeAI.Behavior.Hunt;
            }
            else if (aimodule is NoiseTracker)
            {
                this.behavior = SeaDrakeAI.Behavior.ExamineSound;
            }
            else if (aimodule is StuckTracker)
            {
                this.behavior = SeaDrakeAI.Behavior.GetUnstuck;
            }
        }
        if (this.currentUtility < 0.1f)
        {
            this.behavior = SeaDrakeAI.Behavior.Idle;
        }
        if (this.behavior != SeaDrakeAI.Behavior.Flee && this.fish.grasps[0] != null)
        {
            this.behavior = SeaDrakeAI.Behavior.ReturnPrey;
        }
        switch (this.behavior)
        {
            case SeaDrakeAI.Behavior.Idle:
                if (this.exploreCoordinate != null)
                {
                    this.creature.abstractAI.SetDestination(this.exploreCoordinate.Value);
                    if (Custom.ManhattanDistance(this.creature.pos, this.exploreCoordinate.Value) < 5 || (UnityEngine.Random.value < 0.0125f && base.pathFinder.DoneMappingAccessibility && this.fish.room.aimap.TileAccessibleToCreature(this.creature.pos.x, this.creature.pos.y, this.creature.creatureTemplate) && !base.pathFinder.CoordinateReachableAndGetbackable(this.exploreCoordinate.Value)))
                    {
                        this.exploreCoordinate = null;
                    }
                }
                else if (Custom.ManhattanDistance(this.creature.pos, base.pathFinder.GetDestination) < 5 || !base.pathFinder.CoordinateReachableAndGetbackable(base.pathFinder.GetDestination))
                {
                    WorldCoordinate worldCoordinate = this.fish.room.GetWorldCoordinate(Custom.RestrictInRect(this.fish.mainBodyChunk.pos, new FloatRect(0f, 0f, this.fish.room.PixelWidth, this.fish.room.PixelHeight)) + Custom.RNV() * 200f);
                    if (this.fish.room.IsPositionInsideBoundries(worldCoordinate.Tile) && base.pathFinder.CoordinateReachableAndGetbackable(worldCoordinate) && this.fish.room.VisualContact(this.creature.pos.Tile, worldCoordinate.Tile))
                    {
                        this.creature.abstractAI.SetDestination(worldCoordinate);
                    }
                }
                if (UnityEngine.Random.value < 1f / ((this.exploreCoordinate == null) ? 80f : 1600f))
                {
                    WorldCoordinate worldCoordinate2 = new WorldCoordinate(this.fish.room.abstractRoom.index, UnityEngine.Random.Range(0, this.fish.room.TileWidth), UnityEngine.Random.Range(0, this.fish.room.TileHeight), -1);
                    if (this.fish.room.aimap.TileAccessibleToCreature(worldCoordinate2.Tile, this.creature.creatureTemplate) && base.pathFinder.CoordinateReachableAndGetbackable(worldCoordinate2))
                    {
                        this.exploreCoordinate = new WorldCoordinate?(worldCoordinate2);
                    }
                }
                if (Custom.DistLess(this.creature.pos, base.pathFinder.GetDestination, 20f) && this.fish.room.VisualContact(this.creature.pos, base.pathFinder.GetDestination))
                {
                    this.floatGoalPos = new Vector2?(this.fish.room.MiddleOfTile(base.pathFinder.GetDestination));
                }
                else
                {
                    this.floatGoalPos = null;
                }
                break;
            case SeaDrakeAI.Behavior.Flee:
                {
                    WorldCoordinate destination = base.threatTracker.FleeTo(this.creature.pos, 3, 30, this.currentUtility > 0.3f);
                    if (base.threatTracker.mostThreateningCreature != null)
                    {
                        this.focusCreature = base.threatTracker.mostThreateningCreature;
                    }
                    this.creature.abstractAI.SetDestination(destination);
                    this.floatGoalPos = null;
                    break;
                }
            case SeaDrakeAI.Behavior.Hunt:
                this.attackCounter--;
                this.focusCreature = base.preyTracker.MostAttractivePrey;
                if (this.focusCreature.VisualContact)
                {
                    this.floatGoalPos = new Vector2?(this.focusCreature.representedCreature.realizedCreature.mainBodyChunk.pos);
                }
                else
                {
                    this.creature.abstractAI.SetDestination(this.focusCreature.BestGuessForPosition());
                }
                if (this.focusCreature.VisualContact && this.focusCreature.representedCreature.realizedCreature.collisionLayer != this.fish.collisionLayer && Custom.DistLess(this.focusCreature.representedCreature.realizedCreature.mainBodyChunk.pos, this.fish.bodyChunks[2].pos, this.fish.bodyChunks[2].rad + this.focusCreature.representedCreature.realizedCreature.mainBodyChunk.rad))
                {
                    this.fish.Collide(this.focusCreature.representedCreature.realizedCreature, 2, 0);
                }
                break;
            case SeaDrakeAI.Behavior.ExamineSound:
                if (this.fish != null)
                {
                    this.floatGoalPos = new Vector2?(this.fish.room.MiddleOfTile(this.noiseTracker.ExaminePos));
                }
                else
                {
                    this.creature.abstractAI.SetDestination(base.noiseTracker.ExaminePos);
                }
                break;
            case SeaDrakeAI.Behavior.EscapeRain:
                if (base.denFinder.GetDenPosition() != null)
                {
                    this.creature.abstractAI.SetDestination(base.denFinder.GetDenPosition().Value);
                }
                this.floatGoalPos = null;
                break;
            case SeaDrakeAI.Behavior.ReturnPrey:
                if (base.denFinder.GetDenPosition() != null)
                {
                    this.creature.abstractAI.SetDestination(base.denFinder.GetDenPosition().Value);
                }
                this.floatGoalPos = null;
                break;
            case SeaDrakeAI.Behavior.GoToFood:
                this.creature.abstractAI.SetDestination(this.fish.room.GetWorldCoordinate(this.goToFood.firstChunk.pos));
                break;
            case SeaDrakeAI.Behavior.GetUnstuck:
                this.creature.abstractAI.SetDestination(base.stuckTracker.getUnstuckPosCalculator.unstuckGoalPosition);
                if (UnityEngine.Random.value < Custom.LerpMap(base.stuckTracker.Utility(), 0.9f, 1f, 0f, 0.1f) && this.fish.room.GetTile(this.fish.mainBodyChunk.pos).AnyWater && this.fish.enteringShortCut == null && base.stuckTracker.stuckCloseToShortcutModule.foundShortCut != null)
                {
                    this.fish.enteringShortCut = base.stuckTracker.stuckCloseToShortcutModule.foundShortCut;
                    base.stuckTracker.Reset();
                }
                break;
        }
    }

    // Token: 0x06001E6D RID: 7789 RVA: 0x001B2F2C File Offset: 0x001B112C
    public override float VisualScore(Vector2 lookAtPoint, float targetSpeed)
    {
        return base.VisualScore(lookAtPoint, targetSpeed) - Mathf.Pow(Mathf.InverseLerp(1f, -1f, Vector2.Dot((this.fish.bodyChunks[1].pos - this.fish.bodyChunks[0].pos).normalized, (this.fish.bodyChunks[1].pos - lookAtPoint).normalized)), 0.85f);
    }

    // Token: 0x06001E6E RID: 7790 RVA: 0x00118BD8 File Offset: 0x00116DD8
    public override bool WantToStayInDenUntilEndOfCycle()
    {
        return base.rainTracker.Utility() > 0.01f;
    }

    // Token: 0x06001E6F RID: 7791 RVA: 0x00005929 File Offset: 0x00003B29
    public override void CreatureSpotted(bool firstSpot, Tracker.CreatureRepresentation creatureRep)
    {
    }

    // Token: 0x06001E70 RID: 7792 RVA: 0x001B2FB4 File Offset: 0x001B11B4
    public override Tracker.CreatureRepresentation CreateTrackerRepresentationForCreature(AbstractCreature otherCreature)
    {
        Tracker.CreatureRepresentation result;
        if (otherCreature.creatureTemplate.smallCreature)
        {
            result = new Tracker.SimpleCreatureRepresentation(base.tracker, otherCreature, 0f, false);
        }
        else
        {
            result = new Tracker.ElaborateCreatureRepresentation(base.tracker, otherCreature, 1f, 3);
        }
        return result;
    }

    // Token: 0x06001E71 RID: 7793 RVA: 0x001B2FF8 File Offset: 0x001B11F8
    public bool WantToEatObject(PhysicalObject obj)
    {
        return obj != null && (obj is Player || obj is JetFish || obj is Lizard || obj is Hazer || obj is DropBug || obj is EggBug || obj is Centipede || obj is Spider || obj is Scavenger) && (obj.room != null && obj.room == this.fish.room && obj.grabbedBy.Count == 0 && !obj.slatedForDeletetion && (base.pathFinder.CoordinateReachableAndGetbackable(this.fish.room.GetWorldCoordinate(obj.firstChunk.pos)) || base.pathFinder.CoordinateReachableAndGetbackable(this.fish.room.GetWorldCoordinate(obj.firstChunk.pos) + new IntVector2(0, -1)) || base.pathFinder.CoordinateReachableAndGetbackable(this.fish.room.GetWorldCoordinate(obj.firstChunk.pos) + new IntVector2(0, -2)))) && base.threatTracker.ThreatOfArea(this.fish.room.GetWorldCoordinate(obj.firstChunk.pos), true) < 0.55f;
    }
    

    // Token: 0x040020CF RID: 8399
    public SeaDrake fish;

    // Token: 0x040020D0 RID: 8400
    private Vector2? fgp;

    // Token: 0x040020D1 RID: 8401
    public WorldCoordinate? exploreCoordinate;

    // Token: 0x040020D2 RID: 8402
    public AbstractCreature getAwayFromCreature;

    // Token: 0x040020D3 RID: 8403
    public int getAwayCounter;

    // Token: 0x040020D4 RID: 8404
    private DebugDestinationVisualizer debugDestinationVisualizer;

    // Token: 0x040020D5 RID: 8405
    public int attackCounter;

    // Token: 0x040020D6 RID: 8406
    public float currentUtility;

    // Token: 0x040020D7 RID: 8407
    public PhysicalObject goToFood;
    
    public SeaDrakeAI.Behavior behavior;
    
    public Tracker.CreatureRepresentation focusCreature;
    
    public DebugSprite dbSprite;
    
    public enum Behavior
    {
        Idle,
        Flee,
        Hunt,
        EscapeRain,
        ReturnPrey,
        GoToFood,
        GetUnstuck,
        ExamineSound
    }
}
