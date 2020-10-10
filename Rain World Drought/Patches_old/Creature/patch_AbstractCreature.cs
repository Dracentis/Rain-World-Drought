using System;
using MonoMod;
using UnityEngine;
using System.Runtime.CompilerServices;

class patch_AbstractCreature : AbstractCreature
{
    [MonoModIgnore]
    public patch_AbstractCreature(World world, patch_CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID ID) : base( world, creatureTemplate, realizedCreature, pos,  ID)
    {
    }

    public extern void orig_ctor(World world, patch_CreatureTemplate creatureTemplate, patch_Creature realizedCreature, WorldCoordinate pos, EntityID ID);

    [MonoModConstructor]
    public void ctor(World world, patch_CreatureTemplate creatureTemplate, patch_Creature realizedCreature, WorldCoordinate pos, EntityID ID)
    {
        orig_ctor(world, creatureTemplate, realizedCreature, pos, ID);
        CreatureTemplate.Type type = creatureTemplate.TopAncestor().type;
        if (creatureTemplate.AI)
        {
            type = creatureTemplate.type;
            switch (type)
            {
                case CreatureTemplate.Type.Scavenger:
                    abstractAI = new ScavengerAbstractAI(world, this);
                    goto IL_2F4;
                case CreatureTemplate.Type.Overseer:
                    abstractAI = new OverseerAbstractAI(world, this);
                    goto IL_2F4;
                default:
                    switch (type)
                    {
                        case CreatureTemplate.Type.Vulture:
                            break;
                        default:
                            if (type != CreatureTemplate.Type.MirosBird)
                            {
                                abstractAI = new AbstractCreatureAI(world, this);
                                goto IL_2F4;
                            }
                            abstractAI = new MirosBirdAbstractAI(world, this);
                            goto IL_2F4;
                        case CreatureTemplate.Type.CicadaA:
                        case CreatureTemplate.Type.CicadaB:
                            abstractAI = new CicadaAbstractAI(world, this);
                            goto IL_2F4;
                        case CreatureTemplate.Type.BigEel:
                            abstractAI = new BigEelAbstractAI(world, this);
                            goto IL_2F4;
                        case CreatureTemplate.Type.Deer:
                            abstractAI = new DeerAbstractAI(world, this);
                            goto IL_2F4;
                        case (CreatureTemplate.Type) patch_CreatureTemplate.Type.WalkerBeast:
                            abstractAI = new WalkerBeastAbstractAI(world, this);
                            goto IL_2F4;
                    }
                    break;
                case CreatureTemplate.Type.SmallNeedleWorm:
                case CreatureTemplate.Type.BigNeedleWorm:
                    abstractAI = new NeedleWormAbstractAI(world, this);
                    goto IL_2F4;
                case CreatureTemplate.Type.DropBug:
                    abstractAI = new DropBugAbstractAI(world, this);
                    goto IL_2F4;
                case CreatureTemplate.Type.KingVulture:
                    break;
            }
            abstractAI = new VultureAbstractAI(world, this);
        }
        IL_2F4:
        if (pos.abstractNode > -1 && pos.abstractNode < Room.nodes.Length && Room.nodes[pos.abstractNode].type == AbstractRoomNode.Type.Den && !pos.TileDefined)
        {
            if (Room.offScreenDen)
            {
                remainInDenCounter = 1;
            }
            else
            {
                remainInDenCounter = UnityEngine.Random.Range(100, 1000);
            }
            if (abstractAI != null)
            {
                abstractAI.denPosition = new WorldCoordinate?(pos);
            }
            spawnDen = pos;
        }
        if (creatureTemplate.type == CreatureTemplate.Type.TentaclePlant || creatureTemplate.type == CreatureTemplate.Type.PoleMimic)
        {
            remainInDenCounter = 0;
        }
    }

    public extern void orig_InDenUpdate(int time);

    public void InDenUpdate(int time)
    {
        if(remainInDenCounter == -1)
        {
            // Allow creatures to abort staying in den for the rest of the cycle
            if (!WantToStayInDenUntilEndOfCycle())
                remainInDenCounter = 500;
        } else
            orig_InDenUpdate(time);
    }

    public override void Realize()
    {
        if (realizedCreature != null)
        {
            return;
        }
        switch (creatureTemplate.TopAncestor().type)
        {
            case CreatureTemplate.Type.Slugcat:
                realizedCreature = new Player(this, world);
                break;
            case CreatureTemplate.Type.LizardTemplate:
                realizedCreature = new Lizard(this, world);
                break;
            case CreatureTemplate.Type.Fly:
                realizedCreature = new Fly(this, world);
                break;
            case CreatureTemplate.Type.Leech:
                realizedCreature = new Leech(this, world);
                break;
            case CreatureTemplate.Type.Snail:
                realizedCreature = new Snail(this, world);
                break;
            case CreatureTemplate.Type.Vulture:
                realizedCreature = new Vulture(this, world);
                break;
            case CreatureTemplate.Type.GarbageWorm:
                GarbageWormAI.MoveAbstractCreatureToGarbage(this, Room);
                realizedCreature = new GarbageWorm(this, world);
                break;
            case CreatureTemplate.Type.LanternMouse:
                realizedCreature = new LanternMouse(this, world);
                break;
            case CreatureTemplate.Type.CicadaA:
                realizedCreature = new Cicada(this, world, creatureTemplate.type == CreatureTemplate.Type.CicadaA);
                break;
            case CreatureTemplate.Type.Spider:
                realizedCreature = new Spider(this, world);
                break;
            case CreatureTemplate.Type.JetFish:
                realizedCreature = new JetFish(this, world);
                break;
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake:
                realizedCreature = new SeaDrake(this, world);
                break;
            case CreatureTemplate.Type.BigEel:
                realizedCreature = new BigEel(this, world);
                break;
            case CreatureTemplate.Type.Deer:
                realizedCreature = new Deer(this, world);
                break;
            case (CreatureTemplate.Type) patch_CreatureTemplate.Type.WalkerBeast:
                realizedCreature = new WalkerBeast(this, world);
                break;
            case CreatureTemplate.Type.TubeWorm:
                realizedCreature = new TubeWorm(this, world);
                break;
            case CreatureTemplate.Type.DaddyLongLegs:
                realizedCreature = new DaddyLongLegs(this, world);
                break;
            case CreatureTemplate.Type.TentaclePlant:
                if (creatureTemplate.type == CreatureTemplate.Type.TentaclePlant)
                {
                    realizedCreature = new TentaclePlant(this, world);
                }
                else
                {
                    realizedCreature = new PoleMimic(this, world);
                }
                break;
            case CreatureTemplate.Type.MirosBird:
                realizedCreature = new MirosBird(this, world);
                break;
            case CreatureTemplate.Type.TempleGuard:
                realizedCreature = new TempleGuard(this, world);
                break;
            case CreatureTemplate.Type.Centipede:
            case CreatureTemplate.Type.RedCentipede:
            case CreatureTemplate.Type.Centiwing:
            case CreatureTemplate.Type.SmallCentipede:
                realizedCreature = new Centipede(this, world);
                break;
            case CreatureTemplate.Type.Scavenger:
                realizedCreature = new Scavenger(this, world);
                break;
            case CreatureTemplate.Type.Overseer:
                realizedCreature = new Overseer(this, world);
                break;
            case CreatureTemplate.Type.VultureGrub:
                if (creatureTemplate.type == CreatureTemplate.Type.VultureGrub)
                {
                    realizedCreature = new VultureGrub(this, world);
                }
                else if (creatureTemplate.type == CreatureTemplate.Type.Hazer)
                {
                    realizedCreature = new Hazer(this, world);
                }
                break;
            case CreatureTemplate.Type.EggBug:
                realizedCreature = new EggBug(this, world);
                break;
            case CreatureTemplate.Type.BigSpider:
            case CreatureTemplate.Type.SpitterSpider:
                realizedCreature = new BigSpider(this, world);
                break;
            case CreatureTemplate.Type.BigNeedleWorm:
                if (creatureTemplate.type == CreatureTemplate.Type.SmallNeedleWorm)
                {
                    realizedCreature = new SmallNeedleWorm(this, world);
                }
                else
                {
                    realizedCreature = new BigNeedleWorm(this, world);
                }
                break;
            case CreatureTemplate.Type.DropBug:
                realizedCreature = new DropBug(this, world);
                break;
        }
        InitiateAI();
        for (int i = 0; i < stuckObjects.Count; i++)
        {
            if (stuckObjects[i].A.realizedObject == null)
            {
                stuckObjects[i].A.Realize();
            }
            if (stuckObjects[i].B.realizedObject == null)
            {
                stuckObjects[i].B.Realize();
            }
        }
    }

    public void InitiateAI()
    {
        switch (creatureTemplate.TopAncestor().type)
        {
            case CreatureTemplate.Type.LizardTemplate:
                abstractAI.RealAI = new LizardAI(this, world);
                break;
            case CreatureTemplate.Type.Snail:
                abstractAI.RealAI = new SnailAI(this, world);
                break;
            case CreatureTemplate.Type.Vulture:
                abstractAI.RealAI = new VultureAI(this, world);
                break;
            case CreatureTemplate.Type.GarbageWorm:
                abstractAI.RealAI = new GarbageWormAI(this, world);
                break;
            case CreatureTemplate.Type.LanternMouse:
                abstractAI.RealAI = new MouseAI(this, world);
                break;
            case CreatureTemplate.Type.CicadaA:
                abstractAI.RealAI = new CicadaAI(this, world);
                break;
            case CreatureTemplate.Type.JetFish:
                abstractAI.RealAI = new JetFishAI(this, world);
                break;
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake:
                abstractAI.RealAI = new SeaDrakeAI(this, world);
                break;
            case CreatureTemplate.Type.BigEel:
                abstractAI.RealAI = new BigEelAI(this, world);
                break;
            case CreatureTemplate.Type.Deer:
                abstractAI.RealAI = new DeerAI(this, world);
                break;
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast:
                abstractAI.RealAI = new WalkerBeastAI(this, world);
                break;
            case CreatureTemplate.Type.TubeWorm:
                abstractAI.RealAI = new TubeWormAI(this, world);
                break;
            case CreatureTemplate.Type.DaddyLongLegs:
                abstractAI.RealAI = new DaddyAI(this, world);
                break;
            case CreatureTemplate.Type.TentaclePlant:
                if (creatureTemplate.type == CreatureTemplate.Type.TentaclePlant)
                {
                    abstractAI.RealAI = new TentaclePlantAI(this, world);
                }
                break;
            case CreatureTemplate.Type.MirosBird:
                abstractAI.RealAI = new MirosBirdAI(this, world);
                break;
            case CreatureTemplate.Type.TempleGuard:
                abstractAI.RealAI = new TempleGuardAI(this, world);
                break;
            case CreatureTemplate.Type.Centipede:
            case CreatureTemplate.Type.RedCentipede:
            case CreatureTemplate.Type.Centiwing:
            case CreatureTemplate.Type.SmallCentipede:
                abstractAI.RealAI = new CentipedeAI(this, world);
                break;
            case CreatureTemplate.Type.Scavenger:
                abstractAI.RealAI = new ScavengerAI(this, world);
                break;
            case CreatureTemplate.Type.Overseer:
                abstractAI.RealAI = new OverseerAI(this, world);
                break;
            case CreatureTemplate.Type.EggBug:
                abstractAI.RealAI = new EggBugAI(this, world);
                break;
            case CreatureTemplate.Type.BigSpider:
            case CreatureTemplate.Type.SpitterSpider:
                abstractAI.RealAI = new BigSpiderAI(this, world);
                break;
            case CreatureTemplate.Type.SmallNeedleWorm:
            case CreatureTemplate.Type.BigNeedleWorm:
                if (creatureTemplate.type == CreatureTemplate.Type.SmallNeedleWorm)
                {
                    abstractAI.RealAI = new SmallNeedleWormAI(this, world);
                }
                else
                {
                    abstractAI.RealAI = new BigNeedleWormAI(this, world);
                }
                break;
            case CreatureTemplate.Type.DropBug:
                abstractAI.RealAI = new DropBugAI(this, world);
                break;
        }
    }
}

