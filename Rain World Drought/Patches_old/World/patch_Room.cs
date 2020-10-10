using System;
using MonoMod;
using System.Collections.Generic;
using CoralBrain;
using Noise;
using RWCustom;
using ScavTradeInstruction;
using UnityEngine;
using VoidSea;


class patch_Room : Room
{
    [MonoModIgnore]
    patch_Room(RainWorldGame game, World world, AbstractRoom abstractRoom) : base(game, world, abstractRoom) { }

    [MonoModIgnore]
    public extern void orig_ctor(RainWorldGame game, World world, AbstractRoom abstractRoom);

    [MonoModConstructor]
    public void ctor(RainWorldGame game, World world, AbstractRoom abstractRoom)
    {
        orig_ctor(game, world, abstractRoom);
    }

    public void ReadyForAI()
    {
        loadingProgress = 2;
        if (game != null)
        {
            foreach (AbstractCreature abstractCreature in abstractRoom.creatures)
            {
                if (abstractCreature.realizedCreature == null && abstractCreature.AllowedToExistInRoom(this))
                {
                    abstractCreature.RealizeInRoom();
                }
            }
        }
        if (abstractRoom.quantifiedCreatures != null && game != null)
        {
            for (int i = 0; i < StaticWorld.quantifiedCreatures.Length; i++)
            {
                if (abstractRoom.NumberOfQuantifiedCreatureInRoom(StaticWorld.quantifiedCreatures[i].type) > 0)
                {
                    PlaceQuantifiedCreaturesInRoom(StaticWorld.quantifiedCreatures[i].type);
                }
            }
        }
        loadingProgress = 3;
        readyForNonAICreaturesToEnter = true;
        for (int j = 0; j < updateList.Count; j++)
        {
            if (updateList[j] is INotifyWhenRoomIsReady)
            {
                (updateList[j] as INotifyWhenRoomIsReady).AIMapReady();
            }
        }
        if (game != null && (abstractRoom.name == "SS_AI" || abstractRoom.name == "LM_AI" || abstractRoom.name == "SL_AI"))
        {
            Oracle obj = new Oracle(new AbstractPhysicalObject(world, AbstractPhysicalObject.AbstractObjectType.Oracle, null, new WorldCoordinate(abstractRoom.index, 15, 15, -1), game.GetNewID()), this);
            AddObject(obj);
            waitToEnterAfterFullyLoaded = Math.Max(waitToEnterAfterFullyLoaded, 80);
        }
    }

    [MonoModIgnore]
    private void PlaceQuantifiedCreaturesInRoom(CreatureTemplate.Type critType)
    {
    }

    public extern void orig_Loaded();

    
    public void Loaded()
    {
        if (game == null)
        {
            return;
        }
        for (int k = 0; k < roomSettings.effects.Count; k++)
        {
            if ((patch_RoomSettings.patch_RoomEffect.Type)roomSettings.effects[k].type == patch_RoomSettings.patch_RoomEffect.Type.TankRoomView)
            {
                this.AddObject(new TankRoomView(this, this.roomSettings.effects[k]));
                break;
            }
        }
        if (water)
        {
            AddWater();
        }
        if (abstractRoom.shelter)
        {
            shelterDoor = new ShelterDoor(this);
            AddObject(shelterDoor);
        }
        else if (abstractRoom.gate)
        {
            if (abstractRoom.name == "GATE_SI_LF" || abstractRoom.name == "GATE_HI_CC" || abstractRoom.name == "GATE_SI_CC" || abstractRoom.name == "GATE_CC_UW" || abstractRoom.name == "GATE_UW_SS" || abstractRoom.name == "GATE_SH_UW" || abstractRoom.name == "GATE_SS_UW" || abstractRoom.name == "GATE_SL_MW" || abstractRoom.name == "GATE_MW_LM" || abstractRoom.name == "GATE_LM_MW")
            {
                regionGate = new ElectricGate(this);
                AddObject(regionGate);
            }
            else
            {
                regionGate = new WaterGate(this);
                AddObject(regionGate);
            }
        }
        List<IntVector2> list = new List<IntVector2>();
        for (int i = 0; i < TileWidth; i++)
        {
            for (int j = 0; j < TileHeight - 1; j++)
            {
                if (GetTile(i, j).Terrain != Tile.TerrainType.Solid && GetTile(i, j + 1).Terrain == Tile.TerrainType.Solid && GetTile(i, j - 1).Terrain != Tile.TerrainType.Solid && j > defaultWaterLevel)
                {
                    list.Add(new IntVector2(i, j));
                }
            }
        }
        ceilingTiles = list.ToArray();
        if ((!abstractRoom.shelter || world.brokenShelters[abstractRoom.shelterIndex]) && (roomSettings.DangerType == RoomRain.DangerType.Rain || roomSettings.DangerType == RoomRain.DangerType.FloodAndRain || roomSettings.DangerType == RoomRain.DangerType.Flood))
        {
            roomRain = new RoomRain(game.globalRain, this);
            AddObject(roomRain);
        }
        for (int k = 0; k < roomSettings.effects.Count; k++)
        {
            switch (roomSettings.effects[k].type)
            {
                case RoomSettings.RoomEffect.Type.SkyDandelions:
                    AddObject(new SkyDandelions(roomSettings.effects[k], this));
                    break;
                case RoomSettings.RoomEffect.Type.Lightning:
                case RoomSettings.RoomEffect.Type.BkgOnlyLightning:
                    if (lightning == null)
                    {
                        lightning = new Lightning(this, roomSettings.effects[k].amount, roomSettings.effects[k].type == RoomSettings.RoomEffect.Type.BkgOnlyLightning);
                        AddObject(lightning);
                    }
                    break;
                case RoomSettings.RoomEffect.Type.GreenSparks:
                    AddObject(new GreenSparks(this, roomSettings.effects[k].amount));
                    break;
                case RoomSettings.RoomEffect.Type.VoidMelt:
                    AddObject(new MeltLights(roomSettings.effects[k], this));
                    break;
                case RoomSettings.RoomEffect.Type.ZeroG:
                case RoomSettings.RoomEffect.Type.BrokenZeroG:
                    {
                        bool flag = false;
                        int num = 0;
                        while (num < updateList.Count && !flag)
                        {
                            if (updateList[num] is AntiGravity)
                            {
                                flag = true;
                            }
                            num++;
                        }
                        if (!flag)
                        {
                            AddObject(new AntiGravity(this));
                        }
                        break;
                    }
                case RoomSettings.RoomEffect.Type.SunBlock:
                    AddObject(new SunBlocker());
                    break;
                case RoomSettings.RoomEffect.Type.SSSwarmers:
                    {
                        bool flag2 = true;
                        int num2 = updateList.Count - 1;
                        while (num2 >= 0 && flag2)
                        {
                            flag2 = !(updateList[num2] is CoralNeuronSystem);
                            num2--;
                        }
                        if (flag2)
                        {
                            AddObject(new CoralNeuronSystem());
                        }
                        waitToEnterAfterFullyLoaded = Math.Max(waitToEnterAfterFullyLoaded, 40);
                        break;
                    }
                case RoomSettings.RoomEffect.Type.SSMusic:
                    AddObject(new SSMusicTrigger(roomSettings.effects[k]));
                    break;
                case RoomSettings.RoomEffect.Type.AboveCloudsView:
                    AddObject(new AboveCloudsView(this, roomSettings.effects[k]));
                    break;
                case RoomSettings.RoomEffect.Type.RoofTopView:
                    AddObject(new RoofTopView(this, roomSettings.effects[k]));
                    break;
                case RoomSettings.RoomEffect.Type.VoidSea:
                    AddObject(new VoidSeaScene(this));
                    break;
                case RoomSettings.RoomEffect.Type.ElectricDeath:
                    AddObject(new ElectricDeath(roomSettings.effects[k], this));
                    break;
                case (RoomSettings.RoomEffect.Type) patch_RoomSettings.patch_RoomEffect.Type.ElectricStorm:
                    AddObject(new ElectricStorm(roomSettings.effects[k], this));
                    break;
                case (RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.GravityPulse:
                    AddObject(new GravityPulse(this));
                    break;
                case RoomSettings.RoomEffect.Type.VoidSpawn:
                    if ((game.StoryCharacter != 2 || (world.region != null && world.region.name == "SB")) && ((game.session is StoryGameSession && (game.session as StoryGameSession).saveState.theGlow) || game.setupValues.playerGlowing))
                    {
                        AddObject(new VoidSpawnKeeper(this, roomSettings.effects[k]));
                    }
                    break;
                case RoomSettings.RoomEffect.Type.BorderPushBack:
                    AddObject(new RoomBorderPushBack(this));
                    break;
                case RoomSettings.RoomEffect.Type.Flies:
                case RoomSettings.RoomEffect.Type.FireFlies:
                case RoomSettings.RoomEffect.Type.TinyDragonFly:
                case RoomSettings.RoomEffect.Type.RockFlea:
                case RoomSettings.RoomEffect.Type.RedSwarmer:
                case RoomSettings.RoomEffect.Type.Ant:
                case RoomSettings.RoomEffect.Type.Beetle:
                case RoomSettings.RoomEffect.Type.WaterGlowworm:
                case RoomSettings.RoomEffect.Type.Wasp:
                case RoomSettings.RoomEffect.Type.Moth:
                    if (insectCoordinator == null)
                    {
                        insectCoordinator = new InsectCoordinator(this);
                        AddObject(insectCoordinator);
                    }
                    insectCoordinator.AddEffect(roomSettings.effects[k]);
                    break;
                case (RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.Drain:
                    AddObject(new DrainEffect(this));
                    break;
                case (RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.Pulse:
                    AddObject(new PulseEffect(this));
                    break;
            }
        }
        for (int l = 0; l < roomSettings.placedObjects.Count; l++)
        {
            if (roomSettings.placedObjects[l].active)
            {
                switch (roomSettings.placedObjects[l].type)
                {
                    case PlacedObject.Type.LightSource:
                        {
                            LightSource lightSource = new LightSource(roomSettings.placedObjects[l].pos, true, new Color(1f, 1f, 1f), null);
                            AddObject(lightSource);
                            lightSource.setRad = new float?((roomSettings.placedObjects[l].data as PlacedObject.LigthSourceData).Rad);
                            lightSource.setAlpha = new float?((roomSettings.placedObjects[l].data as PlacedObject.LigthSourceData).strength);
                            lightSource.fadeWithSun = (roomSettings.placedObjects[l].data as PlacedObject.LigthSourceData).fadeWithSun;
                            lightSource.colorFromEnvironment = ((roomSettings.placedObjects[l].data as PlacedObject.LigthSourceData).colorType == PlacedObject.LigthSourceData.ColorType.Environment);
                            lightSource.flat = (roomSettings.placedObjects[l].data as PlacedObject.LigthSourceData).flat;
                            lightSource.effectColor = Math.Max(-1, (roomSettings.placedObjects[l].data as PlacedObject.LigthSourceData).colorType - PlacedObject.LigthSourceData.ColorType.EffectColor1);
                            break;
                        }
                    case PlacedObject.Type.LightFixture:
                        switch ((roomSettings.placedObjects[l].data as PlacedObject.LightFixtureData).type)
                        {
                            case PlacedObject.LightFixtureData.Type.RedLight:
                                AddObject(new Redlight(this, roomSettings.placedObjects[l], roomSettings.placedObjects[l].data as PlacedObject.LightFixtureData));
                                break;
                            case PlacedObject.LightFixtureData.Type.HolyFire:
                                AddObject(new HolyFire(this, roomSettings.placedObjects[l], roomSettings.placedObjects[l].data as PlacedObject.LightFixtureData));
                                break;
                            case PlacedObject.LightFixtureData.Type.ZapCoilLight:
                                AddObject(new ZapCoilLight(this, roomSettings.placedObjects[l], roomSettings.placedObjects[l].data as PlacedObject.LightFixtureData));
                                break;
                            case PlacedObject.LightFixtureData.Type.DeepProcessing:
                                AddObject(new DeepProcessingLight(this, roomSettings.placedObjects[l], roomSettings.placedObjects[l].data as PlacedObject.LightFixtureData));
                                break;
                            case PlacedObject.LightFixtureData.Type.SlimeMoldLight:
                                AddObject(new SlimeMoldLight(this, roomSettings.placedObjects[l], roomSettings.placedObjects[l].data as PlacedObject.LightFixtureData));
                                break;
                        }
                        break;
                    case PlacedObject.Type.CoralStem:
                    case PlacedObject.Type.CoralStemWithNeurons:
                    case PlacedObject.Type.CoralNeuron:
                    case PlacedObject.Type.CoralCircuit:
                    case PlacedObject.Type.WallMycelia:
                        {
                            bool flag3 = true;
                            int num3 = updateList.Count - 1;
                            while (num3 >= 0 && flag3)
                            {
                                flag3 = !(updateList[num3] is CoralNeuronSystem);
                                num3--;
                            }
                            if (flag3)
                            {
                                AddObject(new CoralNeuronSystem());
                            }
                            waitToEnterAfterFullyLoaded = Math.Max(waitToEnterAfterFullyLoaded, 80);
                            break;
                        }
                    case PlacedObject.Type.ProjectedStars:
                        AddObject(new StarMatrix(roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.ZapCoil:
                        AddObject(new ZapCoil((roomSettings.placedObjects[l].data as PlacedObject.GridRectObjectData).Rect, this));
                        break;
                    case PlacedObject.Type.SuperStructureFuses:
                        AddObject(new SuperStructureFuses(roomSettings.placedObjects[l], (roomSettings.placedObjects[l].data as PlacedObject.GridRectObjectData).Rect, this));
                        break;
                    case PlacedObject.Type.GravityDisruptor:
                        AddObject(new GravityDisruptor(roomSettings.placedObjects[l], this));
                        break;
                    case (PlacedObject.Type) patch_PlacedObject.Type.GravityAmplifyer:
                        AddObject(new GravityAmplifier(roomSettings.placedObjects[l], this));
                        break;
                    case PlacedObject.Type.SpotLight:
                        AddObject(new SpotLight(roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.DeepProcessing:
                        AddObject(new DeepProcessing(roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.Corruption:
                        {
                            DaddyCorruption daddyCorruption = null;
                            int num4 = updateList.Count - 1;
                            while (num4 >= 0 && daddyCorruption == null)
                            {
                                if (updateList[num4] is DaddyCorruption)
                                {
                                    daddyCorruption = (updateList[num4] as DaddyCorruption);
                                }
                                num4--;
                            }
                            if (daddyCorruption == null)
                            {
                                daddyCorruption = new DaddyCorruption(this);
                                AddObject(daddyCorruption);
                            }
                            daddyCorruption.places.Add(roomSettings.placedObjects[l]);
                            waitToEnterAfterFullyLoaded = Math.Max(waitToEnterAfterFullyLoaded, 80);
                            break;
                        }
                    case PlacedObject.Type.CorruptionDarkness:
                        AddObject(new DaddyCorruption.CorruptionDarkness(roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.SSLightRod:
                        AddObject(new SSLightRod(roomSettings.placedObjects[l], this));
                        break;
                    case PlacedObject.Type.GhostSpot:
                        if (game.world.worldGhost != null && game.world.worldGhost.ghostRoom == abstractRoom)
                        {
                            AddObject(new Ghost(this, roomSettings.placedObjects[l], game.world.worldGhost));
                        }
                        else if (world.region != null)
                        {
                            int ghostID = (int)GhostWorldPresence.GetGhostID(world.region.name);
                            if (game.session is StoryGameSession && (game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[ghostID] == 0)
                            {
                                AddObject(new GhostHunch(this, ghostID));
                            }
                        }
                        break;
                    case PlacedObject.Type.SlimeMold:
                        {
                            float num5 = game.SeededRandom((int)(roomSettings.placedObjects[l].pos.x + roomSettings.placedObjects[l].pos.y));
                            if (num5 > 0.3f)
                            {
                                AddObject(new SlimeMold.CosmeticSlimeMold(this, roomSettings.placedObjects[l].pos, Custom.LerpMap(num5, 0.3f, 1f, 30f, 70f), false));
                            }
                            break;
                        }
                    case PlacedObject.Type.CosmeticSlimeMold:
                        AddObject(new SlimeMold.CosmeticSlimeMold(this, roomSettings.placedObjects[l].pos, (roomSettings.placedObjects[l].data as PlacedObject.ResizableObjectData).Rad, false));
                        break;
                    case PlacedObject.Type.CosmeticSlimeMold2:
                        AddObject(new SlimeMold.CosmeticSlimeMold(this, roomSettings.placedObjects[l].pos, (roomSettings.placedObjects[l].data as PlacedObject.ResizableObjectData).Rad, true));
                        break;
                    case PlacedObject.Type.SuperJumpInstruction:
                        AddObject(new SuperJumpInstruction(this, roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.LanternOnStick:
                        AddObject(new LanternStick(this, roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.ScavengerOutpost:
                        AddObject(new ScavengerOutpost(roomSettings.placedObjects[l], this));
                        break;
                    case PlacedObject.Type.TradeOutpost:
                        AddObject(new ScavengerTradeSpot(this, roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.ScavengerTreasury:
                        AddObject(new ScavengerTreasury(this, roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.ScavTradeInstruction:
                        AddObject(new ScavengerTradeInstructionTrigger(this, roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.CustomDecal:
                        AddObject(new CustomDecal(roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.InsectGroup:
                        if (insectCoordinator == null)
                        {
                            insectCoordinator = new InsectCoordinator(this);
                            AddObject(insectCoordinator);
                        }
                        insectCoordinator.AddGroup(roomSettings.placedObjects[l]);
                        break;
                    case PlacedObject.Type.PlayerPushback:
                        AddObject(new PlayerPushback(this, roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.MultiplayerItem:
                        if (game.IsArenaSession)
                        {
                            game.GetArenaGameSession.SpawnItem(this, roomSettings.placedObjects[l]);
                        }
                        break;
                    case PlacedObject.Type.GoldToken:
                    case PlacedObject.Type.BlueToken:
                        if (!(game.session is StoryGameSession) || world.singleRoomWorld || !(game.session as StoryGameSession).game.rainWorld.progression.miscProgressionData.GetTokenCollected((roomSettings.placedObjects[l].data as CollectToken.CollectTokenData).tokenString, (roomSettings.placedObjects[l].data as CollectToken.CollectTokenData).isBlue))
                        {
                            AddObject(new CollectToken(this, roomSettings.placedObjects[l]));
                        }
                        else
                        {
                            AddObject(new CollectToken.TokenStalk(this, roomSettings.placedObjects[l].pos, roomSettings.placedObjects[l].pos + (roomSettings.placedObjects[l].data as CollectToken.CollectTokenData).handlePos, null, false));
                        }
                        break;
                    case PlacedObject.Type.DeadTokenStalk:
                        AddObject(new CollectToken.TokenStalk(this, roomSettings.placedObjects[l].pos, roomSettings.placedObjects[l].pos + (roomSettings.placedObjects[l].data as PlacedObject.ResizableObjectData).handlePos, null, false));
                        break;
                    case PlacedObject.Type.ReliableIggyDirection:
                        AddObject(new ReliableIggyDirection(roomSettings.placedObjects[l]));
                        break;
                    case PlacedObject.Type.Rainbow:
                        if (world.rainCycle.CycleStartUp < 1f && (game.cameras[0] == null || game.cameras[0].ghostMode == 0f) && game.SeededRandom(world.rainCycle.rainbowSeed + abstractRoom.index) < (roomSettings.placedObjects[l].data as Rainbow.RainbowData).Chance)
                        {
                            AddObject(new Rainbow(this, roomSettings.placedObjects[l]));
                        }
                        break;
                    case PlacedObject.Type.LightBeam:
                        AddObject(new LightBeam(roomSettings.placedObjects[l]));
                        break;
                }
            }
        }
        if (abstractRoom == null)
        {
            Debug.Log("NULL ABSTRACT ROOM");
        }
        if (game.world.worldGhost != null && game.world.worldGhost.CreaturesSleepInRoom(abstractRoom))
        {
            AddObject(new GhostCreatureSedater(this));
        }
        if (roomSettings.roomSpecificScript)
        {
            RoomSpecificScript.AddRoomSpecificScript(this);
        }
        if (abstractRoom.firstTimeRealized)
        {
            for (int m = 0; m < roomSettings.placedObjects.Count; m++)
            {
                if (roomSettings.placedObjects[m].active)
                {
                    PlacedObject.Type type = roomSettings.placedObjects[m].type;
                    switch ((patch_PlacedObject.Type)type)
                    {
                        case (patch_PlacedObject.Type)PlacedObject.Type.DataPearl:
                        case (patch_PlacedObject.Type)PlacedObject.Type.UniqueDataPearl:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new DataPearl.AbstractDataPearl(world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData, (game.StoryCharacter != 1) ? (roomSettings.placedObjects[m].data as PlacedObject.DataPearlData).pearlType : DataPearl.AbstractDataPearl.DataPearlType.Misc);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                (abstractPhysicalObject as DataPearl.AbstractDataPearl).hidden = (roomSettings.placedObjects[m].data as PlacedObject.DataPearlData).hidden;
                                abstractRoom.entities.Add(abstractPhysicalObject);
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.SeedCob:
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new SeedCob.AbstractSeedCob(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, false, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.entities.Add(abstractPhysicalObject);
                                abstractPhysicalObject.Realize();
                                abstractPhysicalObject.realizedObject.PlaceInRoom(this);
                                break;
                            }
                        case (patch_PlacedObject.Type)PlacedObject.Type.DeadSeedCob:
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new SeedCob.AbstractSeedCob(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, true, null);
                                abstractRoom.entities.Add(abstractPhysicalObject);
                                abstractPhysicalObject.Realize();
                                abstractPhysicalObject.realizedObject.PlaceInRoom(this);
                                break;
                            }
                        case (patch_PlacedObject.Type)PlacedObject.Type.WaterNut:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new WaterNut.AbstractWaterNut(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData, false);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.AddEntity(abstractPhysicalObject);
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.JellyFish:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.JellyFish, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.entities.Add(abstractPhysicalObject);
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.KarmaFlower:
                            if (game.StoryCharacter != 2 && (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, true, abstractRoom.index, m)))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.entities.Add(abstractPhysicalObject);
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.Mushroom:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.Mushroom, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.entities.Add(abstractPhysicalObject);
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.SlimeMold:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.SlimeMold, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.entities.Add(abstractPhysicalObject);
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.FlyLure:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.FlyLure, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.entities.Add(abstractPhysicalObject);
                            }
                            break;
                        default:
                            switch (type)
                            {
                                case PlacedObject.Type.NeedleEgg:
                                    if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                                    {
                                        AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                        (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                        abstractRoom.entities.Add(abstractPhysicalObject);
                                    }
                                    break;
                                default:
                                    switch (type)
                                    {
                                        case PlacedObject.Type.FlareBomb:
                                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                                            {
                                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                                abstractRoom.AddEntity(abstractPhysicalObject);
                                            }
                                            break;
                                        case PlacedObject.Type.PuffBall:
                                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                                            {
                                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.PuffBall, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                                abstractRoom.AddEntity(abstractPhysicalObject);
                                            }
                                            break;
                                        case PlacedObject.Type.TempleGuard:
                                            if (game.setupValues.worldCreaturesSpawn)
                                            {
                                                AbstractPhysicalObject abstractPhysicalObject = new AbstractCreature(world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.TempleGuard), null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID());
                                                abstractRoom.AddEntity(abstractPhysicalObject);
                                            }
                                            break;
                                        case PlacedObject.Type.DangleFruit:
                                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                                            {
                                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                                abstractRoom.entities.Add(abstractPhysicalObject);
                                            }
                                            break;
                                    }
                                    break;
                                case PlacedObject.Type.BubbleGrass:
                                    if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                                    {
                                        AbstractPhysicalObject abstractPhysicalObject = new BubbleGrass.AbstractBubbleGrass(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), 1f, abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                        (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                        abstractRoom.entities.Add(abstractPhysicalObject);
                                    }
                                    break;
                                case PlacedObject.Type.Hazer:
                                case PlacedObject.Type.DeadHazer:
                                    if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                                    {
                                        AbstractCreature abstractCreature = new AbstractCreature(world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Hazer), null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID());
                                        (abstractCreature.state as VultureGrub.VultureGrubState).origRoom = abstractRoom.index;
                                        (abstractCreature.state as VultureGrub.VultureGrubState).placedObjectIndex = m;
                                        abstractRoom.AddEntity(abstractCreature);
                                        if (roomSettings.placedObjects[m].type == PlacedObject.Type.DeadHazer)
                                        {
                                            (abstractCreature.state as VultureGrub.VultureGrubState).Die();
                                        }
                                    }
                                    break;
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.FirecrackerPlant:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(world, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.AddEntity(abstractPhysicalObject);
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.VultureGrub:
                        case (patch_PlacedObject.Type)PlacedObject.Type.DeadVultureGrub:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractCreature abstractCreature2 = new AbstractCreature(world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.VultureGrub), null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID());
                                (abstractCreature2.state as VultureGrub.VultureGrubState).origRoom = abstractRoom.index;
                                (abstractCreature2.state as VultureGrub.VultureGrubState).placedObjectIndex = m;
                                abstractRoom.AddEntity(abstractCreature2);
                                if (roomSettings.placedObjects[m].type == PlacedObject.Type.DeadVultureGrub)
                                {
                                    (abstractCreature2.state as VultureGrub.VultureGrubState).Die();
                                }
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.VoidSpawnEgg:
                            if ((game.StoryCharacter != 2 || UnityEngine.Random.value < 0.05882353f) && (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m)) && (game.setupValues.playerGlowing || (game.session is StoryGameSession && (game.session as StoryGameSession).saveState.theGlow) || world.region.name == "SL"))
                            {
                                AddObject(new VoidSpawnEgg(this, m, roomSettings.placedObjects[m]));
                            }
                            break;
                        case (patch_PlacedObject.Type)PlacedObject.Type.ReliableSpear:
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new AbstractSpear(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), false);
                                abstractRoom.entities.Add(abstractPhysicalObject);
                                break;
                            }
                        case (patch_PlacedObject.Type)PlacedObject.Type.SporePlant:
                            if (!(game.session is StoryGameSession) || !(game.session as StoryGameSession).saveState.ItemConsumed(world, false, abstractRoom.index, m))
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new SporePlant.AbstractSporePlant(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, roomSettings.placedObjects[m].data as PlacedObject.ConsumableObjectData, false, false);
                                (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                                abstractRoom.entities.Add(abstractPhysicalObject);
                            }
                            break;
                        case patch_PlacedObject.Type.SmallPiston:
                            AbstractPhysicalObject abstractPhysicalObject3 = new SmallPiston.AbstractSmallPiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, false, false);
                            abstractRoom.entities.Add(abstractPhysicalObject3);
                            break;
                        case patch_PlacedObject.Type.SmallPistonBotDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject4 = new SmallPiston.AbstractSmallPiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, true, false);
                            abstractRoom.entities.Add(abstractPhysicalObject4);
                            break;
                        case patch_PlacedObject.Type.SmallPistonTopDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject5 = new SmallPiston.AbstractSmallPiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, false, true);
                            abstractRoom.entities.Add(abstractPhysicalObject5);
                            break;
                        case patch_PlacedObject.Type.SmallPistonDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject6 = new SmallPiston.AbstractSmallPiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, true, true);
                            abstractRoom.entities.Add(abstractPhysicalObject6);
                            break;
                        case patch_PlacedObject.Type.LargePiston:
                            AbstractPhysicalObject abstractPhysicalObject13 = new LargePiston.AbstractLargePiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, false, false);
                            abstractRoom.entities.Add(abstractPhysicalObject13);
                            break;
                        case patch_PlacedObject.Type.LargePistonBotDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject14 = new LargePiston.AbstractLargePiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, true, false);
                            abstractRoom.entities.Add(abstractPhysicalObject14);
                            break;
                        case patch_PlacedObject.Type.LargePistonTopDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject15 = new LargePiston.AbstractLargePiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, false, true);
                            abstractRoom.entities.Add(abstractPhysicalObject15);
                            break;
                        case patch_PlacedObject.Type.LargePistonDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject16 = new LargePiston.AbstractLargePiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, true, true);
                            abstractRoom.entities.Add(abstractPhysicalObject16);
                            break;
                        case patch_PlacedObject.Type.GiantPiston:
                            AbstractPhysicalObject abstractPhysicalObject23 = new GiantPiston.AbstractGiantPiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, false, false);
                            abstractRoom.entities.Add(abstractPhysicalObject23);
                            break;
                        case patch_PlacedObject.Type.GiantPistonBotDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject24 = new GiantPiston.AbstractGiantPiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, true, false);
                            abstractRoom.entities.Add(abstractPhysicalObject24);
                            break;
                        case patch_PlacedObject.Type.GiantPistonTopDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject25 = new GiantPiston.AbstractGiantPiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, false, true);
                            abstractRoom.entities.Add(abstractPhysicalObject25);
                            break;
                        case patch_PlacedObject.Type.GiantPistonDeathMode:
                            AbstractPhysicalObject abstractPhysicalObject26 = new GiantPiston.AbstractGiantPiston(world, null, GetWorldCoordinate(roomSettings.placedObjects[m].pos), game.GetNewID(), abstractRoom.index, m, true, true);
                            abstractRoom.entities.Add(abstractPhysicalObject26);
                            break;
                    }
                }
            }
            if (!abstractRoom.shelter && !abstractRoom.gate && game != null && (!game.IsArenaSession || game.GetArenaGameSession.GameTypeSetup.levelItems))
            {
                for (int n = (int)((float)TileWidth * (float)TileHeight * Mathf.Pow(roomSettings.RandomItemDensity, 2f) / 5f); n >= 0; n--)
                {
                    IntVector2 intVector = RandomTile();
                    if (!GetTile(intVector).Solid)
                    {
                        bool flag4 = true;
                        for (int num6 = -1; num6 < 2; num6++)
                        {
                            if (!GetTile(intVector + new IntVector2(num6, -1)).Solid)
                            {
                                flag4 = false;
                                break;
                            }
                        }
                        if (flag4)
                        {
                            EntityID newID = game.GetNewID(-abstractRoom.index);
                            AbstractPhysicalObject ent;
                            if (UnityEngine.Random.value < ((game == null || !game.IsStorySession || game.StoryCharacter != 2) ? roomSettings.RandomItemSpearChance : Mathf.Pow(roomSettings.RandomItemSpearChance, 0.85f)))
                            {
                                ent = new AbstractSpear(world, null, new WorldCoordinate(abstractRoom.index, intVector.x, intVector.y, -1), newID, game != null && game.StoryCharacter == 2 && UnityEngine.Random.value < 0.008f);
                            }
                            else
                            {
                                ent = new AbstractPhysicalObject(world, AbstractPhysicalObject.AbstractObjectType.Rock, null, new WorldCoordinate(abstractRoom.index, intVector.x, intVector.y, -1), newID);
                            }
                            abstractRoom.AddEntity(ent);
                        }
                    }
                }
            }
        }
        abstractRoom.firstTimeRealized = false;
        for (int num7 = 0; num7 < roomSettings.triggers.Count; num7++)
        {
            if (!(game.session is StoryGameSession) || ((game.session as StoryGameSession).saveState.cycleNumber >= roomSettings.triggers[num7].activeFromCycle && (game.StoryCharacter == -1 || roomSettings.triggers[num7].slugcats[game.StoryCharacter]) && (roomSettings.triggers[num7].activeToCycle < 0 || (game.session as StoryGameSession).saveState.cycleNumber <= roomSettings.triggers[num7].activeToCycle)))
            {
                AddObject(new ActiveTriggerChecker(roomSettings.triggers[num7]));
            }
        }
        if (world.rainCycle.CycleStartUp < 1f && roomSettings.CeilingDrips > 0f && roomSettings.DangerType != RoomRain.DangerType.None && !abstractRoom.shelter)
        {
            AddObject(new DrippingSound());
        }
    }

    public float ElectricPower
    {
        get
        {
            if (this.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) > 0f)
            {
                return this.world.rainCycle.brokenAntiGrav.CurrentLightsOn;
            }
            else if (this.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.GravityPulse)>0.1f)
            {
                float state = ((float)Math.Sin((double)(((float)this.world.rainCycle.timer + 1875f) % 2500f / 397.88735f)) * 3f);
                state = Mathf.Clamp(state, 0f, 1f);
                state = 1f - state;
                return state;
            }
            return 1f;
        }
    }

    public extern void orig_Update();

    public void Update()
    {
        orig_Update();
        if (game != null && world != null && abstractRoom != null)
        {
            for (int num5 = 0; num5 < roomSettings.placedObjects.Count; num5++)
            {
                if ((patch_PlacedObject.Type)roomSettings.placedObjects[num5].type == patch_PlacedObject.Type.Radio)
                {
                    for (int num6 = 0; num6 < physicalObjects.Length; num6++)
                    {
                        for (int num7 = 0; num7 < physicalObjects[num6].Count; num7++)
                        {
                            for (int num8 = 0; num8 < physicalObjects[num6][num7].bodyChunks.Length; num8++)
                            {
                                if (physicalObjects[num6][num7].bodyChunks[num8].pos.x > roomSettings.placedObjects[num5].pos.x - 50f && physicalObjects[num6][num7].bodyChunks[num8].pos.x < roomSettings.placedObjects[num5].pos.x + 50f && physicalObjects[num6][num7].bodyChunks[num8].pos.y > roomSettings.placedObjects[num5].pos.y - 50f && physicalObjects[num6][num7].bodyChunks[num8].pos.y < roomSettings.placedObjects[num5].pos.y + 50f)
                                {
                                    if (physicalObjects[num6][num7] is Creature)
                                    {
                                        (physicalObjects[num6][num7] as patch_Creature).Rad();
                                        if ((physicalObjects[num6][num7] as patch_Creature).getRad() > 525 && !(physicalObjects[num6][num7] as Creature).dead)
                                        {
                                            PlaySound(SoundID.Death_Lightning_Spark_Object, physicalObjects[num6][num7].bodyChunks[num8].pos, 1f, 1f);
                                            (physicalObjects[num6][num7] as Creature).Violence(null, null, physicalObjects[num6][num7].bodyChunks[num8], null, Creature.DamageType.Electric, 1f, 1f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }

}
/*

Plan
Create
Modifiy
Render
Settings
Done

Spire Rooms:
SP03 - Plan
SP04 - Plan
Spire6 - Settings
SP01 - Settings
SP02 - Settings

Intake System Rooms:
GATE_UW_IS - Settings
V01 - Settings
V04 - Render
V05 - Settings
V06 - Settings
V07 - Create
V08 - Settings
G04 - Settings
S02 - Settings
D02 - Settings
L02 - Settings
S01 - Done
V02 - Done
V03 - Done
F01 - Done
F03 - Done
F04 - Done
G02 - Done
G03 - Done
S05 - Done
Drain - Done
F01a - Done
F02 - Done
GATE_SL_IS - Done
B05 - Done
B04 - Done
B03 - Done
B02 - Done
B01 - Done
G01 - Done
CORE - Done
S06 - Done

*Rooms that need sounds*
SI_SP01*
SI_SP02*
*/
