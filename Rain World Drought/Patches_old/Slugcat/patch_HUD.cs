using RWCustom;
using UnityEngine;
using HUD;
using MonoMod;
using Menu;

[MonoModPatch("global::HUD.HUD")]
class patch_HUD : HUD.HUD
{
    [MonoModIgnore]
    public patch_HUD(FContainer[] fContainers, RainWorld rainWorld, IOwnAHUD owner) : base(fContainers, rainWorld, owner)
    {
    }

    public void InitDreamHud()
    {
        InitDialogBox();
    }

    public void InitSleepHud(SleepAndDeathScreen sleepAndDeathScreen, Map.MapData mapData, SlugcatStats charStats)
    {
        this.AddPart(new FoodMeter(this, charStats.maxFood, charStats.foodToHibernate));
        this.foodMeter.pos = new Vector2(sleepAndDeathScreen.FoodMeterXPos((sleepAndDeathScreen.ID != ProcessManager.ProcessID.SleepScreen) ? 1f : 0f), 0f);
        this.foodMeter.lastPos = this.foodMeter.pos;

        if (mapData != null)
        {
            this.AddPart(new Map(this, mapData));
        }

        int dayType;
        if ((this.owner as patch_SleepAndDeathScreen).nextcycleLength < 20000)
        {
            dayType = 1;
        }
        else if ((this.owner as patch_SleepAndDeathScreen).nextcycleLength < 28000)
        {
            dayType = 2;
        }else{
            dayType = 3;
        }

        this.AddPart(new KarmaMeter(this, this.fContainers[1], new IntVector2(dayType, 12), false));
        this.karmaMeter.pos = new Vector2(this.rainWorld.options.ScreenSize.x -280f, this.rainWorld.options.ScreenSize.y - 70f);
        this.karmaMeter.lastPos = this.karmaMeter.pos;
        this.karmaMeter.fade = 1f;

        if ((this.owner as patch_SleepAndDeathScreen).nextcycleLength2 < 20000)
        {
            dayType = 1;
        }
        else if ((this.owner as patch_SleepAndDeathScreen).nextcycleLength2 < 28000)
        {
            dayType = 2;
        }
        else
        {
            dayType = 3;
        }

        this.AddPart(new KarmaMeter(this, this.fContainers[1], new IntVector2(dayType, 12), false));
        this.karmaMeter.pos = new Vector2(this.rainWorld.options.ScreenSize.x - 175f, this.rainWorld.options.ScreenSize.y - 70f);
        this.karmaMeter.lastPos = this.karmaMeter.pos;
        this.karmaMeter.fade = 1f;

        if ((this.owner as patch_SleepAndDeathScreen).nextcycleLength3 < 20000)
        {
            dayType = 1;
        }
        else if ((this.owner as patch_SleepAndDeathScreen).nextcycleLength3 < 28000)
        {
            dayType = 2;
        }
        else
        {
            dayType = 3;
        }

        this.AddPart(new KarmaMeter(this, this.fContainers[1], new IntVector2(dayType, 12), false));
        this.karmaMeter.pos = new Vector2(this.rainWorld.options.ScreenSize.x - 70f, this.rainWorld.options.ScreenSize.y - 70f);
        this.karmaMeter.lastPos = this.karmaMeter.pos;
        this.karmaMeter.fade = 1f;

        this.AddPart(new RainMeter(this, this.fContainers[1]));
        this.rainMeter.pos = new Vector2(this.rainWorld.options.ScreenSize.x - 280f, this.rainWorld.options.ScreenSize.y - 70f);
        this.rainMeter.lastPos = this.rainMeter.pos;
        this.rainMeter.fade = 1f;

        this.AddPart(new RainMeter(this, this.fContainers[1]));
        this.rainMeter.pos = new Vector2(this.rainWorld.options.ScreenSize.x - 175f, this.rainWorld.options.ScreenSize.y - 70f);
        this.rainMeter.lastPos = this.rainMeter.pos;
        this.rainMeter.fade = 1f;

        this.AddPart(new RainMeter(this, this.fContainers[1]));
        this.rainMeter.pos = new Vector2(this.rainWorld.options.ScreenSize.x - 70f, this.rainWorld.options.ScreenSize.y - 70f);
        this.rainMeter.lastPos = this.rainMeter.pos;
        this.rainMeter.fade = 1f;
    }

    public enum OwnerType
    {
        Player,
        SleepScreen,
        DeathScreen,
        FastTravelScreen,
        RegionOverview,
        ArenaSession,
        CharacterSelect,
        DreamScreen
    }
}
