using MonoMod;
using UnityEngine;

class patch_WorldLoader : WorldLoader
{
    [MonoModIgnore]
    public patch_WorldLoader(RainWorldGame game, int playerCharacter, bool singleRoomWorld, string worldName, Region region, RainWorldGame.SetupValues setupValues) : base(game, playerCharacter, singleRoomWorld, worldName, region, setupValues)
    {
    }

    public extern CreatureTemplate.Type? org_CreatureTypeFromString(string s);

    public static CreatureTemplate.Type? CreatureTypeFromString(string s)
    {
        CreatureTemplate.Type? result = null;
        switch (s)
        {
            case "Pink":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.PinkLizard);
            case "Green":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.GreenLizard);
            case "Blue":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.BlueLizard);
            case "Yellow":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.YellowLizard);
            case "White":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.WhiteLizard);
            case "Red":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.RedLizard);
            case "Black":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.BlackLizard);
            case "Cyan":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.CyanLizard);
            case "Leech":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.Leech);
            case "SeaLeech":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.SeaLeech);
            case "Snail":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.Snail);
            case "Vulture":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.Vulture);
            case "CicadaA":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.CicadaA);
            case "CicadaB":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.CicadaB);
            case "Cicada":
                return new CreatureTemplate.Type?((UnityEngine.Random.value >= 0.5f) ? CreatureTemplate.Type.CicadaB : CreatureTemplate.Type.CicadaA);
            case "Lantern Mouse":
            case "Mouse":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.LanternMouse);
            case "Spider":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.Spider);
            case "Worm":
            case "Garbage Worm":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.GarbageWorm);
            case "Leviathan":
            case "Lev":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.BigEel);
            case "Tube":
            case "TubeWorm":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.TubeWorm);
            case "Daddy":
            case "DaddyLongLegs":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.DaddyLongLegs);
            case "Bro":
            case "BroLongLegs":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.BrotherLongLegs);
            case "TentaclePlant":
            case "Tentacle":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.TentaclePlant);
            case "PoleMimic":
            case "Mimic":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.PoleMimic);
            case "MirosBird":
            case "Miros":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.MirosBird);
            case "Centipede":
            case "Cent":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.Centipede);
            case "JetFish":
            case "Jetfish":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.JetFish);
            case "EggBug":
            case "Eggbug":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.EggBug);
            case "BigSpider":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.BigSpider);
            case "SpitterSpider":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.SpitterSpider);
            case "BigNeedle":
            case "Needle":
            case "Needle Worm":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.BigNeedleWorm);
            case "SmallNeedle":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.SmallNeedleWorm);
            case "DropBug":
            case "Dropbug":
            case "Dropwig":
            case "DropWig":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.DropBug);
            case "KingVulture":
            case "King Vulture":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.KingVulture);
            case "Red Centipede":
            case "RedCentipede":
            case "RedCenti":
                return new CreatureTemplate.Type?(CreatureTemplate.Type.RedCentipede);
            case "WalkerBeast":
            case "Walker Beast":
            case "Walker":
            case "Dog":
            case "LongDog":
                return (CreatureTemplate.Type) new patch_CreatureTemplate.Type?(patch_CreatureTemplate.Type.WalkerBeast);
            case "GreyLizard":
            case "Grey Lizard":
            case "Grey":
                return (CreatureTemplate.Type)new patch_CreatureTemplate.Type?(patch_CreatureTemplate.Type.GreyLizard);
            case "SeaDrake":
            case "Sea Drake":
            case "Drake":
                return (CreatureTemplate.Type)new patch_CreatureTemplate.Type?(patch_CreatureTemplate.Type.SeaDrake);
        }
        for (int i = 0; i < StaticWorld.creatureTemplates.Length; i++)
        {
            if (s == StaticWorld.creatureTemplates[i].name || s == StaticWorld.creatureTemplates[i].type.ToString())
            {
                result = new CreatureTemplate.Type?(StaticWorld.creatureTemplates[i].type);
                break;
            }
        }
        return result;
    }

    private void GeneratePopulation(bool fresh)
    {
        Debug.Log(string.Concat(new object[]
        {
            "Generate population for : ",
            this.world.region.name,
            " FRESH: ",
            fresh
        }));
        if (this.world.game.setupValues.proceedLineages > 0)
        {
            for (int i = 0; i < this.spawners.Count; i++)
            {
                if (this.spawners[i] is World.Lineage)
                {
                    for (int j = 0; j < this.world.game.setupValues.proceedLineages; j++)
                    {
                        (this.spawners[i] as World.Lineage).ChanceToProgress(this.world);
                    }
                }
            }
        }
        //----------------------------------------Error start
        for (int k = 0; k < this.spawners.Count; k++)
        {
            if (this.spawners[k] is World.SimpleSpawner)
            {
                World.SimpleSpawner simpleSpawner = this.spawners[k] as World.SimpleSpawner;
                int num;
                if (fresh || StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType).quantified || !StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType).saveCreature)
                {
                    num = simpleSpawner.amount;
                }
                else
                {
                    num = this.HowManyOfThisCritterShouldRespawn(simpleSpawner.SpawnerID, simpleSpawner.amount);
                }
                if (num > 0)
                {
                    AbstractRoom abstractRoom = this.world.GetAbstractRoom(simpleSpawner.den);
                    if (abstractRoom != null && simpleSpawner.den.abstractNode>=0 && simpleSpawner.den.abstractNode < abstractRoom.nodes.Length && (abstractRoom.nodes[simpleSpawner.den.abstractNode].type == AbstractRoomNode.Type.Den || abstractRoom.nodes[simpleSpawner.den.abstractNode].type == AbstractRoomNode.Type.GarbageHoles))
                    {
                        if (StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType).quantified)
                        {
                            abstractRoom.AddQuantifiedCreature(simpleSpawner.den.abstractNode, simpleSpawner.creatureType, simpleSpawner.amount);
                        }
                        else
                        {
                            for (int l = 0; l < num; l++)
                            {
                                abstractRoom.MoveEntityToDen(new AbstractCreature(this.world, StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType), null, simpleSpawner.den, this.world.game.GetNewID(simpleSpawner.SpawnerID))
                                {
                                    spawnData = simpleSpawner.spawnDataString
                                });
                            }
                        }
                    }
                }
            }
            else if (this.spawners[k] is World.Lineage)
            {
                World.Lineage lineage = this.spawners[k] as World.Lineage;
                bool flag = fresh || this.ShouldThisCritterRespawn(lineage.SpawnerID);
                if (flag)
                {
                    AbstractRoom abstractRoom2 = this.world.GetAbstractRoom(lineage.den);
                    CreatureTemplate.Type? type = lineage.CurrentType((this.game.session as StoryGameSession).saveState);
                    if (type == null)
                    {
                        lineage.ChanceToProgress(this.world);
                    }
                    else if (abstractRoom2 != null && lineage.den.abstractNode >= 0 && lineage.den.abstractNode < abstractRoom2.nodes.Length && (abstractRoom2.nodes[lineage.den.abstractNode].type == AbstractRoomNode.Type.Den || abstractRoom2.nodes[lineage.den.abstractNode].type == AbstractRoomNode.Type.GarbageHoles))
                    {
                        abstractRoom2.MoveEntityToDen(new AbstractCreature(this.world, StaticWorld.GetCreatureTemplate(type.Value), null, lineage.den, this.world.game.GetNewID(lineage.SpawnerID))
                        {
                            spawnData = lineage.CurrentSpawnData((this.game.session as StoryGameSession).saveState)
                        });
                    }
                    if (type == null)
                    {
                        (this.game.session as StoryGameSession).saveState.respawnCreatures.Add(lineage.SpawnerID);
                        Debug.Log("add NONE creature to respawns for lineage " + lineage.SpawnerID);
                    }
                }
            }
        }
        //--------------------Error end
        if (this.playerCharacter != 2 && !(this.game.session as StoryGameSession).saveState.guideOverseerDead && !(this.game.session as StoryGameSession).saveState.miscWorldSaveData.playerGuideState.angryWithPlayer && UnityEngine.Random.value < this.world.region.regionParams.playerGuideOverseerSpawnChance)
        {
            WorldCoordinate worldCoordinate = new WorldCoordinate(this.world.offScreenDen.index, -1, -1, 0);
            if (this.world.region.name == "SU")
            {
                worldCoordinate = new WorldCoordinate(this.world.GetAbstractRoom("SU_C04").index, 137, 17, 0);
            }
            AbstractCreature abstractCreature = new AbstractCreature(this.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Overseer), null, worldCoordinate, new EntityID(-1, 5));
            if (this.world.GetAbstractRoom(worldCoordinate).offScreenDen)
            {
                this.world.GetAbstractRoom(worldCoordinate).entitiesInDens.Add(abstractCreature);
            }
            else
            {
                this.world.GetAbstractRoom(worldCoordinate).AddEntity(abstractCreature);
            }
            (abstractCreature.abstractAI as OverseerAbstractAI).SetAsPlayerGuide();
        }
        if (this.world.region.name == "UW" || UnityEngine.Random.value < this.world.region.regionParams.overseersSpawnChance * Mathf.InverseLerp(2f, 21f, (float)((this.game.session as StoryGameSession).saveState.cycleNumber + ((this.game.StoryCharacter != 2) ? 0 : 17))))
        {
            int num2 = UnityEngine.Random.Range(this.world.region.regionParams.overseersMin, this.world.region.regionParams.overseersMax);
            for (int m = 0; m < num2; m++)
            {
                this.world.offScreenDen.entitiesInDens.Add(new AbstractCreature(this.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Overseer), null, new WorldCoordinate(this.world.offScreenDen.index, -1, -1, 0), this.game.GetNewID()));
            }
        }
    }
}
