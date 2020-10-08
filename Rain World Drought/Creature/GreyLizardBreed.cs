using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    public static class GreyLizardBreed
    {
        public static CreatureTemplate BreedTemplate(CreatureTemplate.Type type, CreatureTemplate lizardAncestor, CreatureTemplate pinkTemplate, CreatureTemplate blueTemplate, CreatureTemplate greenTemplate)
        {
            List<TileTypeResistance> list = new List<TileTypeResistance>();
            List<TileConnectionResistance> list2 = new List<TileConnectionResistance>();
            LizardBreedParams lizardBreedParams = new LizardBreedParams(type);
            lizardBreedParams.terrainSpeeds = new LizardBreedParams.SpeedMultiplier[Enum.GetNames(typeof(AItile.Accessibility)).Length];
            for (int i = 0; i < lizardBreedParams.terrainSpeeds.Length; i++)
            {
                lizardBreedParams.terrainSpeeds[i] = new LizardBreedParams.SpeedMultiplier(0.1f, 1f, 1f, 1f);
            }

            float movementBasedVision = 0.3f;
            lizardBreedParams.terrainSpeeds[1] = new LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
            list.Add(new TileTypeResistance(AItile.Accessibility.Floor, 1f, PathCost.Legality.Allowed));
            lizardBreedParams.terrainSpeeds[2] = new LizardBreedParams.SpeedMultiplier(1f, 1f, 0.9f, 1f);
            list.Add(new TileTypeResistance(AItile.Accessibility.Corridor, 1f, PathCost.Legality.Allowed));
            lizardBreedParams.terrainSpeeds[3] = new LizardBreedParams.SpeedMultiplier(0.9f, 1f, 0.6f, 1f);
            list.Add(new TileTypeResistance(AItile.Accessibility.Climb, 1f, PathCost.Legality.Allowed));
            float waterPathingResistance = 2f;
            list2.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToClimb, 4f, PathCost.Legality.Allowed));
            lizardBreedParams.biteDelay = 2;
            lizardBreedParams.biteInFront = 20f;
            lizardBreedParams.biteRadBonus = 25f;
            lizardBreedParams.biteHomingSpeed = 4.7f;
            lizardBreedParams.biteChance = 1f;
            lizardBreedParams.attemptBiteRadius = 120f;
            lizardBreedParams.getFreeBiteChance = 1f;
            lizardBreedParams.biteDamage = 4f;
            lizardBreedParams.biteDamageChance = 1f;
            lizardBreedParams.toughness = 3f;
            lizardBreedParams.stunToughness = 3f;
            lizardBreedParams.regainFootingCounter = 1;
            lizardBreedParams.baseSpeed = 5f;
            lizardBreedParams.bodyMass = 3.1f;
            lizardBreedParams.bodySizeFac = 1.2f;
            lizardBreedParams.floorLeverage = 6f;
            lizardBreedParams.maxMusclePower = 9f;
            lizardBreedParams.wiggleSpeed = 0.5f;
            lizardBreedParams.wiggleDelay = 15;
            lizardBreedParams.bodyStiffnes = 0.3f;
            lizardBreedParams.swimSpeed = 1.9f;
            lizardBreedParams.idleCounterSubtractWhenCloseToIdlePos = 10;
            lizardBreedParams.danger = 0.9f;
            lizardBreedParams.aggressionCurveExponent = 0.7f;
            lizardBreedParams.headShieldAngle = 100f;
            lizardBreedParams.canExitLounge = false;
            lizardBreedParams.canExitLoungeWarmUp = true;
            lizardBreedParams.findLoungeDirection = 0.5f;
            lizardBreedParams.loungeDistance = 100f;
            lizardBreedParams.preLoungeCrouch = 25;
            lizardBreedParams.preLoungeCrouchMovement = -0.2f;
            lizardBreedParams.loungeSpeed = 1.9f;
            lizardBreedParams.loungeMaximumFrames = 20;
            lizardBreedParams.loungePropulsionFrames = 10;
            lizardBreedParams.loungeJumpyness = 0.5f;
            lizardBreedParams.loungeDelay = 90;
            lizardBreedParams.riskOfDoubleLoungeDelay = 0.1f;
            lizardBreedParams.postLoungeStun = 20;
            lizardBreedParams.loungeTendensy = 0.05f;
            float visualRadius = 2300f;
            float waterVision = 0.7f;
            float throughSurfaceVision = 0.95f;
            lizardBreedParams.perfectVisionAngle = Mathf.Lerp(1f, -1f, 0.444444448f);
            lizardBreedParams.periferalVisionAngle = Mathf.Lerp(1f, -1f, 0.7777778f);
            lizardBreedParams.biteDominance = 1f;
            lizardBreedParams.limbSize = 1.5f;
            lizardBreedParams.stepLength = 0.8f;
            lizardBreedParams.liftFeet = 0.3f;
            lizardBreedParams.feetDown = 0.5f;
            lizardBreedParams.noGripSpeed = 0.25f;
            lizardBreedParams.limbSpeed = 9f;
            lizardBreedParams.limbQuickness = 0.8f;
            lizardBreedParams.limbGripDelay = 1;
            lizardBreedParams.smoothenLegMovement = true;
            lizardBreedParams.legPairDisplacement = 0.2f;
            lizardBreedParams.standardColor = new Color(.5f, .5f, .5f);
            lizardBreedParams.walkBob = 3f;
            lizardBreedParams.tailSegments = 11;
            lizardBreedParams.tailStiffness = 200f;
            lizardBreedParams.tailStiffnessDecline = 0.3f;
            lizardBreedParams.tailLengthFactor = 1.9f;
            lizardBreedParams.tailColorationStart = 0.3f;
            lizardBreedParams.tailColorationExponent = 2f;
            lizardBreedParams.headSize = 1.2f;
            lizardBreedParams.neckStiffness = 0.2f;
            lizardBreedParams.jawOpenAngle = 140f;
            lizardBreedParams.jawOpenLowerJawFac = 0.6666667f;
            lizardBreedParams.jawOpenMoveJawsApart = 23f;
            lizardBreedParams.headGraphics = new int[5];
            lizardBreedParams.framesBetweenLookFocusChange = 20;
            lizardBreedParams.tamingDifficulty = 7f;
            CreatureTemplate creatureTemplate = new CreatureTemplate(type, lizardAncestor, list, list2, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            creatureTemplate.breedParameters = lizardBreedParams;
            creatureTemplate.baseDamageResistance = lizardBreedParams.toughness * 2f;
            creatureTemplate.baseStunResistance = lizardBreedParams.toughness;
            creatureTemplate.damageRestistances[2, 0] = 2.5f;
            creatureTemplate.damageRestistances[2, 1] = 3f;
            creatureTemplate.meatPoints = 6;
            creatureTemplate.damageRestistances[4, 0] = 1.9f;
            creatureTemplate.bodySize = 3f;
            creatureTemplate.meatPoints = 9;
            creatureTemplate.visualRadius = visualRadius;
            creatureTemplate.waterVision = waterVision;
            creatureTemplate.throughSurfaceVision = throughSurfaceVision;
            creatureTemplate.movementBasedVision = movementBasedVision;
            creatureTemplate.waterPathingResistance = waterPathingResistance;
            creatureTemplate.dangerousToPlayer = lizardBreedParams.danger;
            creatureTemplate.doPreBakedPathing = false;
            creatureTemplate.preBakedPathingAncestor = pinkTemplate;
            creatureTemplate.virtualCreature = false;
            return creatureTemplate;
        }
    }
}