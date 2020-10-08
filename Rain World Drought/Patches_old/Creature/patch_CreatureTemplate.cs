using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using RWCustom;
using UnityEngine;
using MonoMod;
using System.Collections;

public class patch_CreatureTemplate : CreatureTemplate
{
    [MonoModIgnore]
    public patch_CreatureTemplate(CreatureTemplate.Type type, patch_CreatureTemplate ancestor, List<TileTypeResistance> tileResistances, List<TileConnectionResistance> connectionResistances, Relationship defaultRelationship) : base(type, ancestor, tileResistances, connectionResistances, defaultRelationship)
    {
        
    }

    public extern void orig_ctor(CreatureTemplate.Type type, patch_CreatureTemplate ancestor, List<TileTypeResistance> tileResistances, List<TileConnectionResistance> connectionResistances, Relationship defaultRelationship);

    [MonoModConstructor]
    public void ctor(CreatureTemplate.Type type, patch_CreatureTemplate ancestor, List<TileTypeResistance> tileResistances, List<TileConnectionResistance> connectionResistances, Relationship defaultRelationship)
    {
        this.roamInRoomChance = 0.1f;
		this.roamBetweenRoomsChance = 0.1f;
		this.waterVision = 0.4f;
		this.throughSurfaceVision = 0.8f;
		this.movementBasedVision = 0.2f;
		this.communityID = CreatureCommunities.CommunityID.All;
		this.communityInfluence = 0.5f;
		this.countsAsAKill = 2;
		this.lungCapacity = 520f;
		this.quickDeath = true;
		this.saveCreature = true;
        this.type = type;
        name = "???";
        switch (type)
        {
            case CreatureTemplate.Type.StandardGroundCreature:
                name = "StandardGroundCreature";
                break;
            case CreatureTemplate.Type.Slugcat:
                name = "Slugcat";
                break;
            case CreatureTemplate.Type.LizardTemplate:
                name = "Lizard";
                SetDoubleReachUpConnectionParams(AItile.Accessibility.Floor, AItile.Accessibility.Wall, AItile.Accessibility.Floor);
                break;
            case CreatureTemplate.Type.PinkLizard:
                name = "Pink Lizard";
                break;
            case CreatureTemplate.Type.GreenLizard:
                name = "Green Lizard";
                break;
            case CreatureTemplate.Type.BlueLizard:
                name = "Blue Lizard";
                break;
            case CreatureTemplate.Type.YellowLizard:
                name = "Yellow Lizard";
                break;
            case CreatureTemplate.Type.WhiteLizard:
                name = "White Lizard";
                break;
            case CreatureTemplate.Type.RedLizard:
                name = "Red Lizard";
                break;
            case CreatureTemplate.Type.BlackLizard:
                name = "Black Lizard";
                break;
            case CreatureTemplate.Type.Salamander:
                name = "Salamander";
                break;
            case CreatureTemplate.Type.CyanLizard:
                name = "Cyan Lizard";
                break;
            case CreatureTemplate.Type.Fly:
                name = "Fly";
                break;
            case CreatureTemplate.Type.Leech:
                name = "Leech";
                break;
            case CreatureTemplate.Type.SeaLeech:
                name = "Sea Leech";
                break;
            case CreatureTemplate.Type.Snail:
                name = "Snail";
                break;
            case CreatureTemplate.Type.Vulture:
                name = "Vulture";
                break;
            case CreatureTemplate.Type.GarbageWorm:
                name = "Garbage Worm";
                break;
            case CreatureTemplate.Type.LanternMouse:
                name = "Lantern Mouse";
                break;
            case CreatureTemplate.Type.CicadaA:
                name = "Cicada A";
                break;
            case CreatureTemplate.Type.CicadaB:
                name = "Cicada B";
                break;
            case CreatureTemplate.Type.Spider:
                name = "Spider";
                break;
            case CreatureTemplate.Type.JetFish:
                name = "Jet Fish";
                break;
            case (CreatureTemplate.Type)Type.SeaDrake:
                name = "SeaDrake";
                break;
            case CreatureTemplate.Type.BigEel:
                name = "Big Eel";
                break;
            case CreatureTemplate.Type.Deer:
                name = "Deer";
                break;
            case (CreatureTemplate.Type)Type.WalkerBeast:
                name = "WalkerBeast";
                break;
            case (CreatureTemplate.Type)Type.GreyLizard:
                name = "GreyLizard";
                break;
            case CreatureTemplate.Type.TubeWorm:
                name = "Tube Worm";
                SetDoubleReachUpConnectionParams(AItile.Accessibility.Floor, AItile.Accessibility.Wall, AItile.Accessibility.Floor);
                break;
            case CreatureTemplate.Type.DaddyLongLegs:
                name = "Daddy Long Legs";
                break;
            case CreatureTemplate.Type.BrotherLongLegs:
                name = "Brother Long Legs";
                break;
            case CreatureTemplate.Type.TentaclePlant:
                name = "Tentacle Plant";
                break;
            case CreatureTemplate.Type.PoleMimic:
                name = "Pole Mimic";
                break;
            case CreatureTemplate.Type.MirosBird:
                name = "Miros Bird";
                break;
            case CreatureTemplate.Type.TempleGuard:
                name = "Temple Guard";
                break;
            case CreatureTemplate.Type.Centipede:
                name = "Centipede";
                break;
            case CreatureTemplate.Type.RedCentipede:
                name = "Red Centipede";
                break;
            case CreatureTemplate.Type.Centiwing:
                name = "Centiwing";
                break;
            case CreatureTemplate.Type.SmallCentipede:
                name = "Small Centipede";
                break;
            case CreatureTemplate.Type.Scavenger:
                name = "Scavenger";
                SetDoubleReachUpConnectionParams(AItile.Accessibility.Climb, AItile.Accessibility.Air, AItile.Accessibility.Climb);
                break;
            case CreatureTemplate.Type.Overseer:
                name = "Overseer";
                break;
            case CreatureTemplate.Type.VultureGrub:
                name = "Vulture Grub";
                break;
            case CreatureTemplate.Type.EggBug:
                name = "Egg Bug";
                break;
            case CreatureTemplate.Type.BigSpider:
                name = "Big Spider";
                break;
            case CreatureTemplate.Type.SpitterSpider:
                name = "Spitter Spider";
                break;
            case CreatureTemplate.Type.SmallNeedleWorm:
                name = "Small Needle";
                break;
            case CreatureTemplate.Type.BigNeedleWorm:
                name = "Big Needle";
                break;
            case CreatureTemplate.Type.DropBug:
                name = "Drop Bug";
                break;
            case CreatureTemplate.Type.KingVulture:
                name = "King Vulture";
                break;
            case CreatureTemplate.Type.Hazer:
                name = "Hazer";
                break;
        }
        relationships = new CreatureTemplate.Relationship[Enum.GetNames(typeof(CreatureTemplate.Type)).Length];
        for (int i = 0; i < relationships.Length; i++)
        {
            relationships[i] = defaultRelationship;
        }
        virtualCreature = false;
        doPreBakedPathing = false;
        AI = false;
        requireAImap = false;
        quantified = false;
        canFly = false;
        grasps = 0;
        offScreenSpeed = 1f;
        abstractedLaziness = 1;
        smallCreature = false;
        mappedNodeTypes = new bool[Enum.GetNames(typeof(AbstractRoomNode.Type)).Length];
        bodySize = 1f;
        scaryness = 1f;
        deliciousness = 1f;
        shortcutColor = new Color(1f, 1f, 1f);
        shortcutSegments = 1;
        waterRelationship = WaterRelationship.Amphibious;
        waterPathingResistance = 1f;
        canSwim = false;
        shortcutAversion = new PathCost(0f, PathCost.Legality.Allowed);
        NPCTravelAversion = new PathCost(100f, PathCost.Legality.Allowed);
        damageRestistances = new float[Enum.GetNames(typeof(Creature.DamageType)).Length, 2];
        instantDeathDamageLimit = float.MaxValue;
        this.ancestor = ancestor;
        if (ancestor != null)
        {
            preBakedPathingAncestor = ancestor.preBakedPathingAncestor;
            virtualCreature = ancestor.virtualCreature;
            doPreBakedPathing = ancestor.doPreBakedPathing;
            AI = ancestor.AI;
            requireAImap = ancestor.requireAImap;
            quantified = ancestor.quantified;
            canFly = ancestor.canFly;
            grasps = ancestor.grasps;
            offScreenSpeed = ancestor.offScreenSpeed;
            abstractedLaziness = ancestor.abstractedLaziness;
            breedParameters = ancestor.breedParameters;
            stowFoodInDen = ancestor.stowFoodInDen;
            smallCreature = ancestor.smallCreature;
            roamInRoomChance = ancestor.roamInRoomChance;
            roamBetweenRoomsChance = ancestor.roamBetweenRoomsChance;
            visualRadius = ancestor.visualRadius;
            waterVision = ancestor.waterVision;
            throughSurfaceVision = ancestor.throughSurfaceVision;
            movementBasedVision = ancestor.movementBasedVision;
            dangerousToPlayer = ancestor.dangerousToPlayer;
            communityID = ancestor.communityID;
            communityInfluence = ancestor.communityInfluence;
            countsAsAKill = ancestor.countsAsAKill;
            quickDeath = ancestor.quickDeath;
            meatPoints = ancestor.meatPoints;
            wormGrassImmune = ancestor.wormGrassImmune;
            saveCreature = ancestor.saveCreature;
            hibernateOffScreen = ancestor.hibernateOffScreen;
            mappedNodeTypes = (bool[])ancestor.mappedNodeTypes.Clone();
            bodySize = ancestor.bodySize;
            scaryness = ancestor.scaryness;
            deliciousness = ancestor.deliciousness;
            shortcutColor = ancestor.shortcutColor;
            shortcutSegments = ancestor.shortcutSegments;
            waterRelationship = ancestor.waterRelationship;
            waterPathingResistance = ancestor.waterPathingResistance;
            canSwim = ancestor.canSwim;
            socialMemory = ancestor.socialMemory;
            shortcutAversion = ancestor.shortcutAversion;
            NPCTravelAversion = ancestor.NPCTravelAversion;
            doubleReachUpConnectionParams = ancestor.doubleReachUpConnectionParams;
            relationships = (CreatureTemplate.Relationship[])ancestor.relationships.Clone();
            baseDamageResistance = ancestor.baseDamageResistance;
            baseStunResistance = ancestor.baseStunResistance;
            instantDeathDamageLimit = ancestor.instantDeathDamageLimit;
            lungCapacity = ancestor.lungCapacity;
        }
        maxAccessibleTerrain = 0;
        pathingPreferencesTiles = new PathCost[Enum.GetNames(typeof(AItile.Accessibility)).Length];
        pathingPreferencesConnections = new PathCost[Enum.GetNames(typeof(MovementConnection.MovementType)).Length];
        if (ancestor == null)
        {
            for (int j = 0; j < pathingPreferencesTiles.Length; j++)
            {
                pathingPreferencesTiles[j] = new PathCost(10f * (float)j, PathCost.Legality.IllegalTile);
            }
            pathingPreferencesTiles[7] = new PathCost(100f, PathCost.Legality.SolidTile);
            for (int k = 0; k < pathingPreferencesConnections.Length; k++)
            {
                pathingPreferencesConnections[k] = new PathCost(100f, PathCost.Legality.IllegalConnection);
            }
        }
        else
        {
            pathingPreferencesTiles = (PathCost[])ancestor.pathingPreferencesTiles.Clone();
            pathingPreferencesConnections = (PathCost[])ancestor.pathingPreferencesConnections.Clone();
        }
        for (int l = 0; l < tileResistances.Count; l++)
        {
            pathingPreferencesTiles[(int)tileResistances[l].accessibility] = tileResistances[l].cost;
            if (tileResistances[l].cost.legality == PathCost.Legality.Allowed && maxAccessibleTerrain < (int)tileResistances[l].accessibility)
            {
                maxAccessibleTerrain = (int)tileResistances[l].accessibility;
            }
        }
        for (int m = 0; m <= maxAccessibleTerrain; m++)
        {
            if (pathingPreferencesTiles[m] > pathingPreferencesTiles[maxAccessibleTerrain])
            {
                pathingPreferencesTiles[m] = pathingPreferencesTiles[maxAccessibleTerrain];
            }
        }
        for (int n = 0; n < connectionResistances.Count; n++)
        {
            pathingPreferencesConnections[(int)connectionResistances[n].movementType] = connectionResistances[n].cost;
        }
        SetNodeType(AbstractRoomNode.Type.Exit, ConnectionResistance(MovementConnection.MovementType.ShortCut).Allowed);
        SetNodeType(AbstractRoomNode.Type.Den, ConnectionResistance(MovementConnection.MovementType.ShortCut).Allowed);
        SetNodeType(AbstractRoomNode.Type.SkyExit, ConnectionResistance(MovementConnection.MovementType.SkyHighway).Allowed);
        SetNodeType(AbstractRoomNode.Type.SeaExit, ConnectionResistance(MovementConnection.MovementType.SeaHighway).Allowed);
        SetNodeType(AbstractRoomNode.Type.SideExit, ConnectionResistance(MovementConnection.MovementType.SideHighway).Allowed);
        if (type == CreatureTemplate.Type.Scavenger)
        {
            SetNodeType(AbstractRoomNode.Type.Den, false);
            SetNodeType(AbstractRoomNode.Type.RegionTransportation, true);
        }
        if (type == CreatureTemplate.Type.Fly)
        {
            SetNodeType(AbstractRoomNode.Type.BatHive, true);
        }
        if (type == CreatureTemplate.Type.GarbageWorm)
        {
            for (int num = 0; num < mappedNodeTypes.Length; num++)
            {
                mappedNodeTypes[num] = false;
            }
            SetNodeType(AbstractRoomNode.Type.GarbageHoles, true);
        }
    }

    public new enum Type
    {
        StandardGroundCreature,
        Slugcat,
        LizardTemplate,
        PinkLizard,
        GreenLizard,
        BlueLizard,
        YellowLizard,
        WhiteLizard,
        RedLizard,
        BlackLizard,
        Salamander,
        CyanLizard,
        Fly,
        Leech,
        SeaLeech,
        Snail,
        Vulture,
        GarbageWorm,
        LanternMouse,
        CicadaA,
        CicadaB,
        Spider,
        JetFish,
        BigEel,
        Deer,
        TubeWorm,
        DaddyLongLegs,
        BrotherLongLegs,
        TentaclePlant,
        PoleMimic,
        MirosBird,
        TempleGuard,
        Centipede,
        RedCentipede,
        Centiwing,
        SmallCentipede,
        Scavenger,
        Overseer,
        VultureGrub,
        EggBug,
        BigSpider,
        SpitterSpider,
        SmallNeedleWorm,
        BigNeedleWorm,
        DropBug,
        KingVulture,
        Hazer,
        //New
        LightWorm,
        CrossBat,
        WalkerBeast,//LONG DOG!!!!!
        GreyLizard,
        SeaDrake

    }

    [MonoModIgnore]
    private void SetDoubleReachUpConnectionParams(AItile.Accessibility groundTile, AItile.Accessibility betweenTiles, AItile.Accessibility destinationTile)
    {
        doubleReachUpConnectionParams = new int[]
        {
            (int)groundTile,
            (int)betweenTiles,
            (int)destinationTile
        };
    }

}
