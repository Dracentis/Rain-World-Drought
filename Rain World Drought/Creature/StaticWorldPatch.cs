using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rain_World_Drought.Creatures
{
    public class StaticWorldPatch
    {
        public static void AddCreatureTemplate()
        { //Add New Creature Template (Runs after ProcessManager is active)
            CreatureTemplate[] backup = StaticWorld.creatureTemplates.Clone() as CreatureTemplate[]; //Backup original Creature Template
            CreatureTemplate[] extendedList = new CreatureTemplate[60]; //Extend the maximum creature number to 80
            for (int i = 0; i < 60; i++)
            { //Fill them with Placeholder Creature to prevent NullRefException
                extendedList[i] = backup[0];
            }
            for (int i = 0; i < backup.Length; i++)
            { //Copy Original Data
                extendedList[i] = backup[i];
            }

            List<TileTypeResistance> list2 = new List<TileTypeResistance>();
            List<TileConnectionResistance> list3 = new List<TileConnectionResistance>();
            list2.Add(new TileTypeResistance(AItile.Accessibility.OffScreen, 1f, PathCost.Legality.Allowed));
            list2.Add(new TileTypeResistance(AItile.Accessibility.Floor, 1f, PathCost.Legality.Allowed));
            list2.Add(new TileTypeResistance(AItile.Accessibility.Corridor, 1f, PathCost.Legality.Allowed));
            list2.Add(new TileTypeResistance(AItile.Accessibility.Climb, 2.5f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Standard, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OpenDiagonal, 3f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachOverGap, 3f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachUp, 2f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachDown, 2f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.SemiDiagonalReach, 2f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToFloor, 20f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToWater, 20f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ShortCut, 1.5f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.NPCTransportation, 25f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OffScreenMovement, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.BetweenRooms, 10f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Slope, 1.5f, PathCost.Legality.Allowed));

            //Add new Creatures
            patch_CreatureTemplate template0 = new patch_CreatureTemplate(CreatureTemplate.Type.StandardGroundCreature, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            //just using backup[0] cause every new creatures to have same template. this fixes the problem.
            template0.type = (CreatureTemplate.Type)((int)patch_CreatureTemplate.Type.LightWorm);
            template0 = AddTemplateData(template0);
            extendedList[(int)patch_CreatureTemplate.Type.LightWorm] = template0;

            patch_CreatureTemplate template1 = new patch_CreatureTemplate(CreatureTemplate.Type.StandardGroundCreature, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            template1.type = (CreatureTemplate.Type)((int)patch_CreatureTemplate.Type.CrossBat);
            template1 = AddTemplateData(template1);
            extendedList[(int)patch_CreatureTemplate.Type.CrossBat] = template1;

            list2 = new List<TileTypeResistance>();
            list3 = new List<TileConnectionResistance>();
            list2.Add(new TileTypeResistance(AItile.Accessibility.Air, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Standard, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OutsideRoom, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.SideHighway, 100f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OffScreenMovement, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.BetweenRooms, 10f, PathCost.Legality.Allowed));

            patch_CreatureTemplate template2 = new patch_CreatureTemplate(CreatureTemplate.Type.StandardGroundCreature, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            template2.type = (CreatureTemplate.Type)((int)patch_CreatureTemplate.Type.WalkerBeast);
            template2 = AddTemplateData(template2);
            template2.baseDamageResistance = 500f;
            template2.baseStunResistance = 80f;
            template2.abstractedLaziness = 1;
            template2.AI = true;
            template2.requireAImap = true;
            template2.doPreBakedPathing = true;
            template2.offScreenSpeed = 10f;
            template2.canFly = false;
            template2.bodySize = 7f;
            template2.stowFoodInDen = false;
            template2.visualRadius = 1450f;
            template2.waterVision = 0.5f;
            template2.throughSurfaceVision = 0.65f;
            template2.movementBasedVision = 0f;
            template2.hibernateOffScreen = true;
            template2.dangerousToPlayer = 0.6f;
            template2.communityInfluence = 0.1f;
            template2.waterRelationship = CreatureTemplate.WaterRelationship.AirOnly;
            extendedList[(int)patch_CreatureTemplate.Type.WalkerBeast] = template2;

            list2 = new List<TileTypeResistance>();
            list3 = new List<TileConnectionResistance>();
            list2.Add(new TileTypeResistance(AItile.Accessibility.OffScreen, 1f, PathCost.Legality.Allowed));
            list2.Add(new TileTypeResistance(AItile.Accessibility.Floor, 1f, PathCost.Legality.Allowed));
            list2.Add(new TileTypeResistance(AItile.Accessibility.Corridor, 1f, PathCost.Legality.Allowed));
            list2.Add(new TileTypeResistance(AItile.Accessibility.Climb, 2.5f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Standard, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OpenDiagonal, 3f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachOverGap, 3f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachUp, 2f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachDown, 2f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.SemiDiagonalReach, 2f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToFloor, 20f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToWater, 20f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ShortCut, 1.5f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.NPCTransportation, 25f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OffScreenMovement, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.BetweenRooms, 10f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Slope, 1.5f, PathCost.Legality.Allowed));

            patch_CreatureTemplate template3 = new patch_CreatureTemplate(CreatureTemplate.Type.StandardGroundCreature, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            template3.type = (CreatureTemplate.Type)((int)patch_CreatureTemplate.Type.GreyLizard);
            template3 = AddTemplateData(template3);
            extendedList[(int)patch_CreatureTemplate.Type.GreyLizard] = template3;

            patch_CreatureTemplate template4 = new patch_CreatureTemplate(CreatureTemplate.Type.JetFish, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            template4.type = (CreatureTemplate.Type)((int)patch_CreatureTemplate.Type.SeaDrake);
            template4 = AddTemplateData(template4);
            extendedList[(int)patch_CreatureTemplate.Type.SeaDrake] = template4;

            //patch_CreatureTemplate template4 = new patch_CreatureTemplate(CreatureTemplate.Type.StandardGroundCreature, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            //template4.type = (CreatureTemplate.Type)((int)patch_CreatureTemplate.Type.Wolf);

            //template4 = AddTemplateData(template4);
            //extendedList[(int)patch_CreatureTemplate.Type.Wolf] = template4;

            //Replace StaticWorld creatureTemplates
            StaticWorld.creatureTemplates = extendedList;
        }

        public static void ModifyRelationship()
        { //Modify Relationship (Runs after ProcessManager is active)
          //patch_CreatureTemplate.EstablishRelationship(patch_CreatureTemplate.Type.YellowLizard, patch_CreatureTemplate.Type.Slugcat, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Uncomfortable, 0.5f));
          //patch_CreatureTemplate.EstablishRelationship(patch_CreatureTemplate.Type.YellowLizard, patch_CreatureTemplate.Type.Centipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.4f));
          //patch_CreatureTemplate.EstablishRelationship(patch_CreatureTemplate.Type.YellowLizard, patch_CreatureTemplate.Type.Vulture, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 0.8f));
          //patch_CreatureTemplate.EstablishRelationship(patch_CreatureTemplate.Type.YellowLizard, patch_CreatureTemplate.Type.Deer, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 1f));

            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, 0.1f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.Slugcat, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.6f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.BigNeedleWorm, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.9f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.SmallNeedleWorm, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.9f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.Scavenger, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.6f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.SpitterSpider, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.6f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.BigSpider, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.6f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.DaddyLongLegs, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.7f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.Deer, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 1f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.SmallCentipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.Fly, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.Leech, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.Spider, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.Centipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.8f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.BigSpider, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.5f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, CreatureTemplate.Type.RedCentipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.7f));
            EstablishRelationship(CreatureTemplate.Type.Slugcat, (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.8f));
            EstablishRelationship(CreatureTemplate.Type.TentaclePlant, (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.8f));
            EstablishRelationship(CreatureTemplate.Type.Scavenger, (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.7f));
            EstablishRelationship(CreatureTemplate.Type.BigSpider, (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.9f));
            EstablishRelationship(CreatureTemplate.Type.DropBug, (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.9f));
            EstablishRelationship(CreatureTemplate.Type.BigNeedleWorm, CreatureTemplate.Type.BigNeedleWorm, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 0.9f));

            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.LizardTemplate, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.AgressiveRival, 0.1f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.Slugcat, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.SocialDependent, 0.5f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.Vulture, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.9f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.KingVulture, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 1f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.TubeWorm, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.025f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.Scavenger, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.8f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.CicadaA, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.05f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.LanternMouse, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.3f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.BigSpider, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.35f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.EggBug, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.45f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.JetFish, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.1f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.BigEel, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 1f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.DaddyLongLegs, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 1f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.Centipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.8f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.BigNeedleWorm, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.25f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.SmallNeedleWorm, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.15f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.DropBug, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.2f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.SmallNeedleWorm, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.3f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.RedCentipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.9f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.TentaclePlant, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.2f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.Hazer, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.15f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.Vulture, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 0.4f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.KingVulture, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 0.2f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.MirosBird, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 0.4f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, CreatureTemplate.Type.DaddyLongLegs, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.2f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 1f));
            EstablishRelationship(CreatureTemplate.Type.BigSpider, (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.4f));
            EstablishRelationship(CreatureTemplate.Type.DropBug, (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.4f));

            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.Slugcat, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.3f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.JetFish, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.5f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.LizardTemplate, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.5f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.Hazer, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.5f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.Salamander, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.4f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.DropBug, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.5f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.EggBug, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.3f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.SmallCentipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.5f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.Spider, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.5f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.Scavenger, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.7f));
            EstablishRelationship((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, CreatureTemplate.Type.BigEel, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 1f));
            EstablishRelationship(CreatureTemplate.Type.JetFish, (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.5f));
            EstablishRelationship(CreatureTemplate.Type.LizardTemplate, (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.5f));
            EstablishRelationship(CreatureTemplate.Type.Slugcat, (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.3f));
            EstablishRelationship(CreatureTemplate.Type.Spider, (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.3f));
            EstablishRelationship(CreatureTemplate.Type.Scavenger, (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.87f));
        }

        public static patch_CreatureTemplate AddTemplateData(patch_CreatureTemplate template)
        {
            List<TileConnectionResistance> list3 = new List<TileConnectionResistance>();
            List<TileTypeResistance> list2 = new List<TileTypeResistance>();

            switch ((int)template.type)
            {
                default:
                    break;

                case (int)patch_CreatureTemplate.Type.LightWorm:

                    break;

                case (int)patch_CreatureTemplate.Type.CrossBat:

                    break;

                case (int)patch_CreatureTemplate.Type.SeaDrake:
                    list2.Clear();
                    list3.Clear();
                    template.baseDamageResistance = 0.95f;
                    template.baseStunResistance = 0.9f;
                    template.instantDeathDamageLimit = 1f;
                    template.abstractedLaziness = 100;
                    template.AI = true;
                    template.requireAImap = true;
                    template.canSwim = true;
                    template.doPreBakedPathing = false;
                    template.offScreenSpeed = 2f;
                    template.bodySize = 2f;
                    template.preBakedPathingAncestor = new CreatureTemplate(CreatureTemplate.Type.Leech, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 1f)); ;
                    template.grasps = 1;
                    template.stowFoodInDen = true;
                    template.visualRadius = 300f;
                    template.waterVision = 1f;
                    template.throughSurfaceVision = 0.65f;
                    template.movementBasedVision = 0f;
                    template.socialMemory = true;
                    template.communityInfluence = 0f;
                    template.communityID = CreatureCommunities.CommunityID.None;
                    template.waterRelationship = CreatureTemplate.WaterRelationship.WaterOnly;
                    template.meatPoints = 6;
                    break;

                case (int)patch_CreatureTemplate.Type.GreyLizard:
                    list2.Clear();
                    list3.Clear();
                    list2.Add(new TileTypeResistance(AItile.Accessibility.OffScreen, 1f, PathCost.Legality.Allowed));
                    list2.Add(new TileTypeResistance(AItile.Accessibility.Floor, 1f, PathCost.Legality.Allowed));
                    list2.Add(new TileTypeResistance(AItile.Accessibility.Corridor, 1f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Standard, 1f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OpenDiagonal, 3f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachOverGap, 3f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachUp, 2f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.DoubleReachUp, 2f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachDown, 2f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.SemiDiagonalReach, 2f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToFloor, 20f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToWater, 20f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.ShortCut, 1.5f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.NPCTransportation, 25f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OffScreenMovement, 1f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.BetweenRooms, 10f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.LizardTurn, 10f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Slope, 1.5f, PathCost.Legality.Allowed));
                    CreatureTemplate creatureTemplate2 = new CreatureTemplate(CreatureTemplate.Type.LizardTemplate, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
                    creatureTemplate2.offScreenSpeed = 0.3f;
                    creatureTemplate2.grasps = 1;
                    creatureTemplate2.virtualCreature = true;
                    creatureTemplate2.abstractedLaziness = 50;
                    template.name = "GreyLizard";
                    creatureTemplate2.AI = true;
                    creatureTemplate2.requireAImap = true;
                    creatureTemplate2.bodySize = 2f;
                    creatureTemplate2.stowFoodInDen = true;
                    creatureTemplate2.shortcutSegments = 3;
                    creatureTemplate2.communityID = CreatureCommunities.CommunityID.Lizards;
                    creatureTemplate2.communityInfluence = 1f;
                    creatureTemplate2.waterRelationship = CreatureTemplate.WaterRelationship.AirAndSurface;
                    creatureTemplate2.canSwim = true;
                    creatureTemplate2.socialMemory = true;
                    CreatureTemplate creatureTemplate3 = LizardBreeds.BreedTemplate(CreatureTemplate.Type.PinkLizard, creatureTemplate2, null, null, null);
                    template = (patch_CreatureTemplate)GreyLizardBreed.BreedTemplate(patch_CreatureTemplate.Type.GreyLizard, creatureTemplate2, creatureTemplate3, null, null);
                    template.type = (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard;
                    break;

                case (int)patch_CreatureTemplate.Type.WalkerBeast:
                    list2.Clear();
                    list3.Clear();
                    list2.Add(new TileTypeResistance(AItile.Accessibility.Air, 1f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Standard, 1f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OutsideRoom, 1f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.SideHighway, 100000f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OffScreenMovement, 1f, PathCost.Legality.Allowed));
                    list3.Add(new TileConnectionResistance(MovementConnection.MovementType.BetweenRooms, 10f, PathCost.Legality.Allowed));
                    template = new patch_CreatureTemplate(CreatureTemplate.Type.Deer, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
                    template.baseDamageResistance = 200f;
                    template.baseStunResistance = 8f;
                    template.name = "WalkerBeast";
                    template.type = (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast;
                    template.abstractedLaziness = 10;
                    template.AI = true;
                    template.requireAImap = true;
                    template.doPreBakedPathing = true;
                    template.offScreenSpeed = 1.5f;
                    template.bodySize = 2f;
                    template.grasps = 1;
                    template.stowFoodInDen = false;
                    template.visualRadius = 1450f;
                    template.waterVision = 0.5f;
                    template.throughSurfaceVision = 0.65f;
                    template.movementBasedVision = 0f;
                    template.hibernateOffScreen = true;
                    template.communityInfluence = 0.85f;
                    template.communityID = CreatureCommunities.CommunityID.None;
                    template.waterRelationship = CreatureTemplate.WaterRelationship.AirOnly;
                    template.preBakedPathingAncestor = StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Deer).preBakedPathingAncestor;
                    break;
            }

            return template;
        }

        private static void EstablishRelationship(CreatureTemplate.Type a, CreatureTemplate.Type b, CreatureTemplate.Relationship relationship)
        {
            if (relationship == new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f))
            {
                relationship = new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f);
            }
            CreatureTemplate creatureTemplate = StaticWorld.GetCreatureTemplate(a);
            CreatureTemplate creatureTemplate2 = StaticWorld.GetCreatureTemplate(b);
            creatureTemplate.relationships[(int)creatureTemplate2.type] = relationship;
            foreach (CreatureTemplate creatureTemplate3 in StaticWorld.creatureTemplates)
            {
                if (creatureTemplate3.ancestor == creatureTemplate)
                {
                    EstablishRelationship(creatureTemplate3.type, creatureTemplate2.type, relationship);
                }
            }
            foreach (CreatureTemplate creatureTemplate4 in StaticWorld.creatureTemplates)
            {
                if (creatureTemplate4.ancestor == creatureTemplate2)
                {
                    EstablishRelationship(creatureTemplate.type, creatureTemplate4.type, relationship);
                }
            }
        }
    }
}