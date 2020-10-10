using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;


class patch_AbstractPhysicalObject : AbstractPhysicalObject
{
    
    [MonoModIgnore]
    patch_AbstractPhysicalObject(World world, AbstractPhysicalObject.AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world, type, realizedObject, pos, ID) { }

    public extern void orig_ctor(World world, AbstractPhysicalObject.AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID);

    [MonoModConstructor]
    public void ctor(World world, AbstractPhysicalObject.AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID)
    {
        orig_ctor(world, type, realizedObject, pos, ID);
    }

    public enum AbstractObjectType
    {
        Creature,
        Rock,
        Spear,
        FlareBomb,
        VultureMask,
        PuffBall,
        DangleFruit,
        Oracle,
        PebblesPearl,
        MoonPearl,
        SLOracleSwarmer,
        SSOracleSwarmer,
        DataPearl,
        SeedCob,
        WaterNut,
        JellyFish,
        Lantern,
        KarmaFlower,
        Mushroom,
        VoidSpawn,
        FirecrackerPlant,
        SlimeMold,
        FlyLure,
        ScavengerBomb,
        SporePlant,
        AttachedBee,
        EggBugEgg,
        NeedleEgg,
        DartMaggot,
        BubbleGrass,
        NSHSwarmer,
        OverseerCarcass,
        SmallPiston,
        LargePiston,
        GiantPiston,
        LMOracleSwarmer
    }

    public extern void orig_Realize();

    public void Realize()
    {
        if (realizedObject != null)
        {
            return;
        }
        switch ((patch_AbstractPhysicalObject.AbstractObjectType) type)
        {
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.Rock:
                realizedObject = new Rock(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.Spear:
                if (((this as AbstractPhysicalObject) as AbstractSpear).explosive)
                {
                    realizedObject = new ExplosiveSpear(this, world);
                }
                else
                {
                    realizedObject = new Spear(this, world);
                }
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.FlareBomb:
                realizedObject = new FlareBomb(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.VultureMask:
                realizedObject = new VultureMask(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.PuffBall:
                realizedObject = new PuffBall(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.DangleFruit:
                realizedObject = new DangleFruit(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.PebblesPearl:
                realizedObject = new PebblesPearl(this, world);
                break;
            //case (patch_AbstractPhysicalObject.AbstractObjectType)patch_AbstractPhysicalObject.AbstractObjectType.MoonPearl:
            //    this.realizedObject = new MoonPearl(this, this.world);
            //    break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer:
                realizedObject = new SLOracleSwarmer(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer:
                realizedObject = new SSOracleSwarmer(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractObjectType.LMOracleSwarmer:
                realizedObject = new LMOracleSwarmer(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.DataPearl:
                realizedObject = new DataPearl(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.SeedCob:
                realizedObject = new SeedCob(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.WaterNut:
                if (((this as AbstractPhysicalObject) as WaterNut.AbstractWaterNut).swollen)
                {
                    realizedObject = new SwollenWaterNut(this);
                }
                else
                {
                    realizedObject = new WaterNut(this);
                }
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.JellyFish:
                realizedObject = new JellyFish(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.Lantern:
                realizedObject = new Lantern(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.KarmaFlower:
                realizedObject = new KarmaFlower(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.Mushroom:
                realizedObject = new Mushroom(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.VoidSpawn:
                realizedObject = new VoidSpawn(this, (Room.realizedRoom == null) ? 0f : Room.realizedRoom.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.VoidMelt), Room.realizedRoom != null && VoidSpawnKeeper.DayLightMode(Room.realizedRoom));
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant:
                realizedObject = new FirecrackerPlant(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)(patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.SlimeMold:
                realizedObject = new SlimeMold(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.FlyLure:
                realizedObject = new FlyLure(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.ScavengerBomb:
                realizedObject = new ScavengerBomb(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.SporePlant:
                realizedObject = new SporePlant(this, world);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.EggBugEgg:
                realizedObject = new EggBugEgg(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.NeedleEgg:
                realizedObject = new NeedleEgg(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.DartMaggot:
                realizedObject = new DartMaggot(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.BubbleGrass:
                realizedObject = new BubbleGrass(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.NSHSwarmer:
                realizedObject = new NSHSwarmer(this);
                break;
            case (patch_AbstractPhysicalObject.AbstractObjectType)AbstractPhysicalObject.AbstractObjectType.OverseerCarcass:
                realizedObject = new OverseerCarcass(this, world);
                break;
            case AbstractObjectType.SmallPiston:
                realizedObject = new SmallPiston(this);
                break;
            case AbstractObjectType.LargePiston:
                realizedObject = new LargePiston(this);
                break;
            case AbstractObjectType.GiantPiston:
                realizedObject = new GiantPiston(this);
                break;

        }
        for (int i = 0; i < stuckObjects.Count; i++)
        {
            if (stuckObjects[i].A.realizedObject == null && stuckObjects[i].A != this)
            {
                stuckObjects[i].A.Realize();
            }
            if (stuckObjects[i].B.realizedObject == null && stuckObjects[i].B != this)
            {
                stuckObjects[i].B.Realize();
            }
        }
    }

}

