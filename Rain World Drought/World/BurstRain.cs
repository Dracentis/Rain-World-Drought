using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    public class BurstRain : GlobalRain.DeathRain
    {
        public BurstRain(GlobalRain globalRain) : base(globalRain)
        {
            this.deathRainMode = DeathRainMode.GradeABuildUp;
            this.burstRainMode = BurstRainMode.BurstCalmBeforeStorm;
            this.timeInThisMode = Mathf.Lerp(300f, 500f, Random.value);
            this.calmBeforeStormSunlight = ((Random.value >= 0.5f) ? Random.value : 0f);
            this.timer = 0;
        }

        public BurstRainMode burstRainMode;
        public bool destroy = false;
        private int timer;

        public void BurstRainUpdate()
        {
            timer++;
            this.progression += 1f / this.timeInThisMode * ((!this.globalRain.game.IsArenaSession) ? 1f : 3.2f);
            bool next = false;
            if (this.progression > 1f)
            {
                this.progression = 1f;
                next = true;
            }

            switch (this.burstRainMode)
            {
                case BurstRainMode.BurstCalmBeforeStorm:
                    this.globalRain.Intensity = Mathf.Pow(Mathf.InverseLerp(0.15f, 0f, this.progression), 1.5f) * 0.24f;
                    this.globalRain.ShaderLight = -1f + 0.3f * Mathf.Sin(Mathf.InverseLerp(0.03f, 0.8f, this.progression) * 3.14159274f) * this.calmBeforeStormSunlight;
                    this.globalRain.bulletRainDensity = Mathf.Pow(Mathf.InverseLerp(0.3f, 1f, this.progression), 8f);
                    break;
                case BurstRainMode.BurstGradeABuildUp:
                    this.globalRain.Intensity = this.progression * 0.6f;
                    this.globalRain.MicroScreenShake = this.progression * 1.5f;
                    this.globalRain.bulletRainDensity = 1f - this.progression;
                    break;
                case BurstRainMode.BurstGradeBBuildUp:
                    this.globalRain.Intensity = Mathf.Lerp(0.6f, 0.71f, this.progression);
                    this.globalRain.MicroScreenShake = Mathf.Lerp(1.5f, 2.1f, this.progression);
                    this.globalRain.ScreenShake = this.progression * 1.2f;
                    break;
                case BurstRainMode.BurstEnd:
                    this.globalRain.Intensity = Mathf.Lerp(0.71f, 0f, this.progression);
                    this.globalRain.MicroScreenShake = Mathf.Lerp(2.1f, 0f, this.progression);
                    this.globalRain.ScreenShake = Mathf.Lerp(1.2f, 0f, this.progression);
                    break;
            }
            if (next)
            {
                this.NextBurstRainMode();
            }
        }

        private void NextBurstRainMode()
        {
            if (this.burstRainMode == BurstRainMode.BurstEnd)
            {
                destroy = true;
                this.globalRain.Intensity = 0f;
                this.globalRain.ScreenShake = 0f;
                this.globalRain.MicroScreenShake = 0f;
                return;
            }
            this.burstRainMode++;

            this.progression = 0f;
            switch (this.burstRainMode)
            {
                case BurstRainMode.BurstCalmBeforeStorm:
                    this.timeInThisMode = Mathf.Lerp(300f, 500f, Random.value);
                    this.calmBeforeStormSunlight = ((Random.value >= 0.5f) ? Random.value : 0f);
                    break;
                case BurstRainMode.BurstGradeABuildUp:
                    this.timeInThisMode = 6f;
                    this.globalRain.ShaderLight = -1f;
                    break;
                case BurstRainMode.BurstGradeAPlateu:
                    this.timeInThisMode = Mathf.Lerp(100f, 200f, Random.value);
                    break;
                case BurstRainMode.BurstGradeBBuildUp:
                    this.timeInThisMode = ((Random.value >= 0.2f) ? Mathf.Lerp(50f, 200f, Random.value) : 100f);
                    break;
                case BurstRainMode.BurstGradeBPlateu:
                    this.timeInThisMode = ((Random.value >= 0.2f) ? Mathf.Lerp(50f, 200f, Random.value) : 100f);
                    break;
                case BurstRainMode.BurstEnd:
                    this.timeInThisMode = 1200 - this.timer;
                    break;
            }
        }

        public enum BurstRainMode
        {
            None,
            BurstCalmBeforeStorm,
            BurstGradeABuildUp,
            BurstGradeAPlateu,
            BurstGradeBBuildUp,
            BurstGradeBPlateu,
            BurstEnd
        }
    }
}
