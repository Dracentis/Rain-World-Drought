using System;
using UnityEngine;
using MonoMod;
using RWCustom;

class patch_RainCycle : RainCycle
{
    [MonoModIgnore]
    public patch_RainCycle(World world, float minutes) : base(world, minutes)
    {
    }

    [MonoModConstructor]
    public void ctor(World world, float minutes)
    {
        this.world = world;
        this.cycleLength = (int)(minutes * 40f * 60f);
        this.storyMode = world.game.IsStorySession;
        if (world.game.setupValues.cycleStartUp)
        {
            if (world.game.IsStorySession)
            {
                if (world.game.manager.menuSetup.startGameCondition != ProcessManager.MenuSetup.StoryGameInitCondition.Dev && (world.game.session as StoryGameSession).saveState.cycleNumber > 0)
                {
                    this.startUpTicks = 2400;
                }
            }
            else if (world.game.IsArenaSession && world.game.GetArenaGameSession.GameTypeSetup.gameType != ArenaSetup.GameTypeID.Sandbox)
            {
                this.startUpTicks = 600;
            }
        }
        this.rainbowSeed = UnityEngine.Random.Range(0, 10000);
        if (cycleLength > 36000)
        {
            burstNum = 3;
        }
        else if (cycleLength > 32000)
        {
            burstNum = 2;
        }
        else if (cycleLength > 28000)
        {
            burstNum = 1;
        }
        if (this.cycleLength < 20000)
        {
            //(this.world.game.globalRain as patch_GlobalRain).LowerWaterLevel();
        }
    }
    
    public int TimeUntilRain
    {
        get
        {
            return (this.cycleLength - this.timer);
        }
    }
    
    public float AmountLeft
    {
        get
        {
            return (float)(this.cycleLength - this.timer) / (float)this.cycleLength;
        }
    }
    
    public float RainApproaching
    {
        get
        {
            if (this.world.game.IsStorySession)
            {
                return Mathf.InverseLerp(0f, 2400f, (float)this.TimeUntilRain);
            }
            return Mathf.InverseLerp(0f, 400f, (float)this.TimeUntilRain);
        }
    }

    public float BurstApproaching
    {
        get
        {
            if (this.world.game.IsStorySession)
            {
                return Mathf.InverseLerp(0f, 1800f, Mathf.Abs((float)this.TimeUntilBurst(CurrentBurst())));
            }
            return Mathf.InverseLerp(0f, 400f, (float)this.TimeUntilBurst(CurrentBurst()));
        }
    }
    
    private float LightChangeBecauseOfRain
    {
        get
        {
            if (BurstApproaching < 0.2)
            {
                return Mathf.InverseLerp(.2f, 1f, this.BurstApproaching);
            }
            else
            {
                return Mathf.Min(Mathf.InverseLerp(0.4f, 1f, this.RainApproaching), Mathf.InverseLerp(0.4f, 1f, this.BurstApproaching));
            }
        }
    }
    
    public float ShaderLight
    {
        get
        {
            if (this.RainGameOver)
            {
                return this.world.game.globalRain.ShaderLight;
            }
            if (this.storyMode)
            {
                return -1f + Mathf.Lerp(this.CycleStartUp, 1f - this.ProximityToMiddleOfCycle, 0.2f) * 2f * Mathf.InverseLerp(0.4f, 1f, this.LightChangeBecauseOfRain);
            }
            return Custom.LerpMap((float)this.TimeUntilRain, 200f, 880f, -1f, 1f);
        }
    }
    
    public float RainDarkPalette
    {
        get
        {
            if (this.storyMode)
            {
                return Mathf.InverseLerp(1f, 0f, this.LightChangeBecauseOfRain);
            }
            return Mathf.InverseLerp(1000f, 400f, (float)this.TimeUntilRain);
        }
    }
    
    public float ScreenShake
    {
        get
        {
            if (this.RainGameOver)
            {
                return this.world.game.globalRain.ScreenShake;
            }
            return Mathf.Pow(1f - Mathf.InverseLerp(0f, 0.2f, this.RainApproaching), 2f);
        }
    }
    
    public float MicroScreenShake
    {
        get
        {
            if (this.RainGameOver)
            {
                return this.world.game.globalRain.MicroScreenShake;
            }
            return Mathf.Pow(1f - Mathf.InverseLerp(0f, 0.6f, this.RainApproaching), 1.5f);
        }
    }
    
    public bool RainGameOver
    {
        get
        {
            return this.timer >= this.cycleLength || burstRainHasHit;
        }
    }
    
    public float CycleStartUp
    {
        get
        {
            return (this.startUpTicks <= 0) ? 1f : Mathf.InverseLerp(0f, (float)this.startUpTicks, (float)this.timer);
        }
    }
    
    public float CycleProgression
    {
        get
        {
            return Mathf.InverseLerp(0f, (float)this.cycleLength, (float)this.timer);
        }
    }
    
    public float ProximityToMiddleOfCycle
    {
        get
        {
            return Mathf.Abs((float)this.timer - (float)this.cycleLength / 2f) / ((float)this.cycleLength / 2f);
        }
    }
    
    public bool MusicAllowed
    {
        get
        {
            return this.world.game.IsArenaSession || (this.TimeUntilRain >= 2400 && this.TimeUntilBurst(CurrentBurst()) > 1200);
        }
    }
    
    public void Update()
    {
        if (this.world.game.AllowRainCounterToTick())
        {
            if (this.pause > 0)
            {
                this.pause--;
            }
            else
            {
                if (this.speedUpToRain && !this.deathRainHasHit)
                {
                    if (this.timer < this.cycleLength - 800)
                    {
                        this.timer += 3;
                    }
                    else if (this.timer < this.cycleLength)
                    {
                        this.timer++;
                    }
                }
                this.timer++;
            }
        }
        if (!this.deathRainHasHit && this.timer >= this.cycleLength)
        {
            this.RainHit();
            this.deathRainHasHit = true;
        }
        if (!this.burstRainHasHit && this.TimeUntilBurst(CurrentBurst()) < 0)
        {
            this.BurstRainHit();
            this.burstRainHasHit = true;
        }
        if (this.brokenAntiGrav != null)
        {
            this.brokenAntiGrav.Update();
        }
        if (!this.MusicAllowed && this.world.game.manager.musicPlayer != null && this.world.game.cameras[0].room.roomSettings.DangerType != RoomRain.DangerType.None && (this.world.game.manager.musicPlayer.song != null || this.world.game.manager.musicPlayer.nextSong != null))
        {
            this.world.game.manager.musicPlayer.RainRequestStopSong();
        }
    }
    
    public void ArenaEndSessionRain()
    {
        this.speedUpToRain = true;
        this.timer = Math.Max(this.timer, this.cycleLength - 2000);
    }
    
    private void RainHit()
    {
        this.world.game.globalRain.InitDeathRain();
    }

    private void BurstRainHit()
    {
        (this.world.game.globalRain as patch_GlobalRain).InitBurstRain();
        Debug.Log("Burst Rain Hit");
    }

    public int getBurstIndex(int index)
    {
        if (burstNum <= index)
        {
            return 50;
        }
        return ((int)(((float)cycleLength) / ((float)burstNum + 1f) * (burstNum - index)) / 1200);
    }

    public int getBurstTime(int index)
    {
        if (burstNum <= index)
        {
            return 99999999;
        }
        return (int)(( ( (float)getBurstIndex(index)) / ((float)((int)this.cycleLength / 1200))) * (float) this.cycleLength);
    }

    public int TimeUntilBurst(int index)
    {
        if (burstNum <= index)
        {
            return 99999999;
        }
        return getBurstTime(index) - this.timer;
    }

    public int CurrentBurst()
    {
        if (burstNum >= 3 & TimeUntilBurst(2) > -1800)
        {
            if (TimeUntilBurst(2) > 0 & burstRainHasHit)
            {
                burstRainHasHit = false;
            }
            return 2;
        }
        else if (burstNum >= 2 & TimeUntilBurst(1) > -1800)
        {
            if (TimeUntilBurst(1) > 0 & burstRainHasHit)
            {
                burstRainHasHit = false;
            }
            return 1;
        }
        else if (burstNum >= 1 & TimeUntilBurst(0) > -1800)
        {
            if (TimeUntilBurst(0) > 0 & burstRainHasHit)
            {
                burstRainHasHit = false;
            }
            return 0;
        }
        return 3;
    }

    public float BurstRainApproaching
    {
        get
        {
            if ((float)this.TimeUntilBurst(CurrentBurst()) > 0)
            {
                return Mathf.InverseLerp(0f, 2400f, (float)this.TimeUntilBurst(CurrentBurst()));
            }
            else
            {
                return Mathf.InverseLerp(0f, -1200f, (float)this.TimeUntilBurst(CurrentBurst()));
            }
        }
    }

    public World world;
    public AntiGravity.BrokenAntiGravity brokenAntiGrav;
    public int timer;
    public int cycleLength;
    private bool storyMode;
    private bool speedUpToRain;
    public int rainbowSeed;
    private bool deathRainHasHit;
    private bool burstRainHasHit;
    private int startUpTicks;
    public int pause;
    public int burstNum = 0;
    
}

