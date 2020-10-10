using RWCustom;
using UnityEngine;
using MonoMod;

class patch_GlobalRain : GlobalRain
{
    [MonoModIgnore]
    public patch_GlobalRain(RainWorldGame game) : base(game)
    {
    }

    public float Intensity;//Self explainatory

    public float ShaderLight = -1f;

    public float ScreenShake;//Larger visual effects on terrain and environment

    public float MicroScreenShake;//Small visual effects on terrain

    public float RumbleSound;//Sound control

    public float bulletRainDensity;//Physics and Graphics control

    public GlobalRain.DeathRain deathRain;//Phasing object for controlling the downpoar

    public float flood;//current additional water level added to rooms that are designated to flood

    public float floodSpeed;//derivative of additional water level (max of 0.8)

    public float lastRainDirection;//For rain graphics
    
    public float rainDirection;//For rain graphics

    public bool slowStart = false;

    public void InitDeathRain()
    {
        this.deathRain = new GlobalRain.DeathRain(this);
    }

    public void InitBurstRain()
    {
        this.deathRain = new GlobalRain.DeathRain(this);
        (this.deathRain as patch_DeathRain).BurstDeathRain();
    }

    public void LowerWaterLevel()
    {
        this.flood = -100;
    }

    public float OutsidePushAround
    {
        get
        {
            return Mathf.Pow(Mathf.InverseLerp(0.35f, 0.7f, this.Intensity), 0.8f);
        }
    }
    
    public float InsidePushAround
    {
        get
        {
            return Mathf.Pow(Mathf.InverseLerp(0.63f, 0.98f, this.Intensity), 3.5f);
        }
    }
    
    public bool AnyPushAround
    {
        get
        {
            return this.OutsidePushAround > 0f || this.InsidePushAround > 0f;
        }
    }

    public void Update()
    {
        if (UnityEngine.Random.value < 0.025f)
        {
            this.rainDirectionGetTo = Mathf.Lerp(-1f, 1f, UnityEngine.Random.value);
        }
        this.lastRainDirection = this.rainDirection;
        this.rainDirection = Mathf.Lerp(this.rainDirection, this.rainDirectionGetTo, 0.01f);
        if (this.rainDirection < this.rainDirectionGetTo)
        {
            this.rainDirection = Mathf.Min(this.rainDirection + 0.0125f, this.rainDirectionGetTo);
        }
        else if (this.rainDirection > this.rainDirectionGetTo)
        {
            this.rainDirection = Mathf.Max(this.rainDirection - 0.0125f, this.rainDirectionGetTo);
        }
        if (this.deathRain != null)
        {
            this.deathRain.DeathRainUpdate();
            if ((this.deathRain as patch_DeathRain).isBurstDeathRain())
            {
                if (this.game.IsStorySession && this.deathRain.deathRainMode >= (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstEnd)
                {
                    this.floodSpeed = Mathf.Max(0.0f, this.floodSpeed - 0.0005f);
                }
                else if (this.game.IsStorySession && this.deathRain.deathRainMode > (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstGradeABuildUp)
                {
                    this.floodSpeed = Mathf.Min(0.2f, this.floodSpeed + 0.0025f);
                }
                else if (this.game.IsArenaSession && this.deathRain.deathRainMode >= (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstEnd)
                {
                    this.floodSpeed = Mathf.Max(0.0f, this.floodSpeed - 0.005f);
                }
                else if (this.game.IsArenaSession && this.deathRain.deathRainMode >= (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstGradeABuildUp)
                {
                    this.floodSpeed = Mathf.Min(0.4f, this.floodSpeed + 0.006666667f);
                }
                //this.flood += this.floodSpeed;
                if ((this.deathRain as patch_DeathRain).destroy){
                    this.deathRain = null;
                    floodSpeed = 0;
                }
            }
            else
            {
                if (this.game.IsStorySession && this.deathRain.deathRainMode > GlobalRain.DeathRain.DeathRainMode.GradeABuildUp)
                {
                    this.floodSpeed = Mathf.Min(0.8f, this.floodSpeed + 0.0025f);
                }
                else if (this.game.IsArenaSession && this.deathRain.deathRainMode >= GlobalRain.DeathRain.DeathRainMode.GradeABuildUp)
                {
                    this.floodSpeed = Mathf.Min(1.8f, this.floodSpeed + 0.006666667f);
                }
                this.flood += this.floodSpeed;
            }
        }
        else
        {
            this.floodSpeed = 0;
            this.Intensity = Mathf.InverseLerp(600f, 200f, (float)this.game.world.rainCycle.TimeUntilRain) * 0.24f;
        }
    }

    public class patch_DeathRain : DeathRain
    {
        [MonoModIgnore]
        public patch_DeathRain(GlobalRain globalRain) : base(globalRain)
        {
            this.globalRain = globalRain;
            this.NextDeathRainMode();
            timer = 0;
        }

        // Token: 0x06000E39 RID: 3641 RVA: 0x000A0098 File Offset: 0x0009E298
        public void DeathRainUpdate()
        {
            timer++;
            this.progression += 1f / this.timeInThisMode * ((!this.globalRain.game.IsArenaSession) ? 1f : 3.2f);
            bool flag = false;
            if (this.progression > 1f)
            {
                this.progression = 1f;
                flag = true;
            }
            if (this.deathRainMode == GlobalRain.DeathRain.DeathRainMode.CalmBeforeStorm)
            {
                //this.globalRain.RumbleSound = Mathf.Max(this.globalRain.RumbleSound - 0.025f, 0f);
            }
            else
            {
                //this.globalRain.RumbleSound = Mathf.Lerp(this.globalRain.RumbleSound, 1f - Mathf.InverseLerp(0f, 0.6f, this.globalRain.game.world.rainCycle.RainApproaching), 0.2f);
            }
            switch (this.deathRainMode)
            {
                case GlobalRain.DeathRain.DeathRainMode.CalmBeforeStorm:
                    this.globalRain.Intensity = Mathf.Pow(Mathf.InverseLerp(0.15f, 0f, this.progression), 1.5f) * 0.24f;
                    this.globalRain.ShaderLight = -1f + 0.3f * Mathf.Sin(Mathf.InverseLerp(0.03f, 0.8f, this.progression) * 3.14159274f) * this.calmBeforeStormSunlight;
                    this.globalRain.bulletRainDensity = Mathf.Pow(Mathf.InverseLerp(0.3f, 1f, this.progression), 8f);
                    break;
                case GlobalRain.DeathRain.DeathRainMode.GradeABuildUp:
                    this.globalRain.Intensity = this.progression * 0.6f;
                    this.globalRain.MicroScreenShake = this.progression * 1.5f;
                    this.globalRain.bulletRainDensity = 1f - this.progression;
                    break;
                case GlobalRain.DeathRain.DeathRainMode.GradeBBuildUp:
                    this.globalRain.Intensity = Mathf.Lerp(0.6f, 0.71f, this.progression);
                    this.globalRain.MicroScreenShake = Mathf.Lerp(1.5f, 2.1f, this.progression);
                    this.globalRain.ScreenShake = this.progression * 1.2f;
                    break;
                case GlobalRain.DeathRain.DeathRainMode.FinalBuildUp:
                    this.globalRain.Intensity = Mathf.Lerp(0.71f, 1f, this.progression);
                    this.globalRain.MicroScreenShake = Mathf.Lerp(2.1f, 4f, Mathf.Pow(this.progression, 1.2f));
                    this.globalRain.ScreenShake = Mathf.Lerp(1.2f, 3f, this.progression);
                    break;
                case GlobalRain.DeathRain.DeathRainMode.AlternateBuildUp:
                    this.globalRain.Intensity = Mathf.Lerp(0.24f, 0.6f, this.progression);
                    this.globalRain.MicroScreenShake = 1f + this.progression * 0.5f;
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstCalmBeforeStorm:
                    this.globalRain.Intensity = Mathf.Pow(Mathf.InverseLerp(0.15f, 0f, this.progression), 1.5f) * 0.24f;
                    this.globalRain.ShaderLight = -1f + 0.3f * Mathf.Sin(Mathf.InverseLerp(0.03f, 0.8f, this.progression) * 3.14159274f) * this.calmBeforeStormSunlight;
                    this.globalRain.bulletRainDensity = Mathf.Pow(Mathf.InverseLerp(0.3f, 1f, this.progression), 8f);
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstGradeABuildUp:
                    this.globalRain.Intensity = this.progression * 0.6f;
                    this.globalRain.MicroScreenShake = this.progression * 1.5f;
                    this.globalRain.bulletRainDensity = 1f - this.progression;
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstGradeBBuildUp:
                    this.globalRain.Intensity = Mathf.Lerp(0.6f, 0.71f, this.progression);
                    this.globalRain.MicroScreenShake = Mathf.Lerp(1.5f, 2.1f, this.progression);
                    this.globalRain.ScreenShake = this.progression * 1.2f;
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstEnd:
                    this.globalRain.Intensity = Mathf.Lerp(0.71f, 0f, this.progression);
                    this.globalRain.MicroScreenShake = Mathf.Lerp(2.1f, 0f, this.progression);
                    this.globalRain.ScreenShake = Mathf.Lerp(1.2f, 0f, this.progression);
                    break;
            }
            if (flag)
            {
                this.NextDeathRainMode();
            }
        }

        // Token: 0x06000E3A RID: 3642 RVA: 0x000A03C8 File Offset: 0x0009E5C8
        public void NextDeathRainMode()
        {
            
            if (this.deathRainMode == GlobalRain.DeathRain.DeathRainMode.Mayhem)
            {
                return;
            }
            if (this.deathRainMode == (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstEnd)
            {
                destroy = true;
                this.globalRain.Intensity = 0f;
                this.globalRain.ScreenShake = 0f;
                this.globalRain.MicroScreenShake = 0f;
                return;
            }
            if (this.deathRainMode == GlobalRain.DeathRain.DeathRainMode.None && (UnityEngine.Random.value < 0.7f || this.globalRain.game.IsArenaSession))
            {
                this.deathRainMode = GlobalRain.DeathRain.DeathRainMode.AlternateBuildUp;
            }
            else if (this.deathRainMode == GlobalRain.DeathRain.DeathRainMode.AlternateBuildUp)
            {
                this.deathRainMode = GlobalRain.DeathRain.DeathRainMode.GradeAPlateu;
            }
            else
            {
                this.deathRainMode++;
            }
            this.progression = 0f;
            switch (this.deathRainMode)
            {
                case GlobalRain.DeathRain.DeathRainMode.CalmBeforeStorm:
                    this.timeInThisMode = Mathf.Lerp(400f, 800f, UnityEngine.Random.value);
                    if ((this.globalRain as patch_GlobalRain).slowStart)
                    {
                        this.timeInThisMode = Mathf.Lerp(600f, 1000f, UnityEngine.Random.value);
                    }
                    this.calmBeforeStormSunlight = ((UnityEngine.Random.value >= 0.5f) ? UnityEngine.Random.value : 0f);
                    break;
                case GlobalRain.DeathRain.DeathRainMode.GradeABuildUp:
                    this.timeInThisMode = 6f;
                    if ((this.globalRain as patch_GlobalRain).slowStart)
                    {
                        this.timeInThisMode = 50f;
                    }
                    this.globalRain.ShaderLight = -1f;
                    break;
                case GlobalRain.DeathRain.DeathRainMode.GradeAPlateu:
                    this.timeInThisMode = Mathf.Lerp(400f, 600f, UnityEngine.Random.value);
                    if ((this.globalRain as patch_GlobalRain).slowStart)
                    {
                        this.timeInThisMode = Mathf.Lerp(600f, 800f, UnityEngine.Random.value);
                    }
                    break;
                case GlobalRain.DeathRain.DeathRainMode.GradeBBuildUp:
                    this.timeInThisMode = ((UnityEngine.Random.value >= 0.5f) ? Mathf.Lerp(50f, 300f, UnityEngine.Random.value) : 100f);
                    if ((this.globalRain as patch_GlobalRain).slowStart)
                    {
                        this.timeInThisMode = Mathf.Lerp(100f, 300f, UnityEngine.Random.value);
                    }
                    break;
                case GlobalRain.DeathRain.DeathRainMode.GradeBPlateu:
                    this.timeInThisMode = ((UnityEngine.Random.value >= 0.5f) ? Mathf.Lerp(50f, 300f, UnityEngine.Random.value) : 100f);
                    if ((this.globalRain as patch_GlobalRain).slowStart)
                    {
                        this.timeInThisMode = Mathf.Lerp(100f, 500f, UnityEngine.Random.value);
                    }
                    break;
                case GlobalRain.DeathRain.DeathRainMode.FinalBuildUp:
                    this.timeInThisMode = ((UnityEngine.Random.value >= 0.5f) ? Mathf.Lerp(100f, 800f, UnityEngine.Random.value) : Mathf.Lerp(300f, 500f, UnityEngine.Random.value));
                    if ((this.globalRain as patch_GlobalRain).slowStart)
                    {
                        this.timeInThisMode = Mathf.Lerp(300f, 1000f, UnityEngine.Random.value);
                    }
                    break;
                case GlobalRain.DeathRain.DeathRainMode.AlternateBuildUp:
                    this.timeInThisMode = Mathf.Lerp(400f, 1200f, UnityEngine.Random.value);
                    if ((this.globalRain as patch_GlobalRain).slowStart)
                    {
                        this.timeInThisMode = Mathf.Lerp(600f, 1500f, UnityEngine.Random.value);
                    }
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstCalmBeforeStorm:
                    this.timeInThisMode = Mathf.Lerp(300f, 500f, UnityEngine.Random.value);
                    this.calmBeforeStormSunlight = ((UnityEngine.Random.value >= 0.5f) ? UnityEngine.Random.value : 0f);
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstGradeABuildUp:
                    this.timeInThisMode = 6f;
                    this.globalRain.ShaderLight = -1f;
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstGradeAPlateu:
                    this.timeInThisMode = Mathf.Lerp(100f, 200f, UnityEngine.Random.value);
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstGradeBBuildUp:
                    this.timeInThisMode = ((UnityEngine.Random.value >= 0.2f) ? Mathf.Lerp(50f, 200f, UnityEngine.Random.value) : 100f);
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstGradeBPlateu:
                    this.timeInThisMode = ((UnityEngine.Random.value >= 0.2f) ? Mathf.Lerp(50f, 200f, UnityEngine.Random.value) : 100f);
                    break;
                case (GlobalRain.DeathRain.DeathRainMode)patch_GlobalRain.patch_DeathRain.DeathRainMode.BurstEnd:
                    this.timeInThisMode = 1200-this.timer;
                    break;
            }
        }

        // Token: 0x04000C95 RID: 3221
        public GlobalRain globalRain;

        // Token: 0x04000C96 RID: 3222
        public GlobalRain.DeathRain.DeathRainMode deathRainMode;

        // Token: 0x04000C97 RID: 3223
        private float timeInThisMode;

        // Token: 0x04000C98 RID: 3224
        private float progression;

        // Token: 0x04000C99 RID: 3225
        private float calmBeforeStormSunlight;

        private bool burst = false;

        private int timer = 0;

        public bool destroy = false;

        public void BurstDeathRain()
        {
            burst = true;
            deathRainMode = (DeathRain.DeathRainMode)patch_DeathRain.DeathRainMode.BurstCalmBeforeStorm;
            this.timeInThisMode = Mathf.Lerp(300f, 500f, UnityEngine.Random.value);
            this.calmBeforeStormSunlight = ((UnityEngine.Random.value >= 0.5f) ? UnityEngine.Random.value : 0f);
        }

        public bool isBurstDeathRain()
        {
            return burst;
        }
        
        public enum DeathRainMode
        {
            None,

            CalmBeforeStorm,

            GradeABuildUp,

            GradeAPlateu,

            GradeBBuildUp,
            
            GradeBPlateu,

            FinalBuildUp,

            Mayhem,

            AlternateBuildUp,

            BurstCalmBeforeStorm,

            BurstGradeABuildUp,

            BurstGradeAPlateu,

            BurstGradeBBuildUp,

            BurstGradeBPlateu,

            BurstEnd
        }
    }
}
