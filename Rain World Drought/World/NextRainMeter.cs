using HUD;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    public class NextRainMeter : RainMeter
    {
        public NextRainMeter(HUD.HUD hud, FContainer fContainer, int index) : base(hud, fContainer)
        {
            this.index = index;
            this.lastPos = this.pos;
            this.circles = new HUDCircle[nextcycleLength[index] / 1200];
            this.danger = new bool[this.circles.Length];
            for (int i = 0; i < this.circles.Length; i++)
            {
                this.circles[i] = new HUDCircle(hud, HUDCircle.SnapToGraphic.smallEmptyCircle, fContainer, 0);
                this.danger[i] = false;
            }

            // Add red marker
            for (int q = 0; q < 3; q++)
            {
                int b = GetBurstIndex(index, q);
                if (b < this.circles.Length) danger[b] = true;
            }
            /*
            for (int i = 1; i < 3; i++)
            {
                nextcycleLength[i - 1] = nextcycleLength[i];
                burstNum[i - 1] = burstNum[i];
            } */
        }

        private const int disableBurst = 99999999;
        private int index;

        public static int GetBurstIndex(int index, int burst)
        {
            if (burstNum[index] <= burst) { return disableBurst; }
            return Mathf.FloorToInt(((float)nextcycleLength[index] / (burstNum[index] + 1) * (burstNum[index] - burst)) / 1200f);
        }

        public bool[] danger;
        public static int[] nextcycleLength = new int[3];
        public static int[] burstNum = new int[3];

        public override void Update()
        {
            this.lastPos = this.pos;
            this.pos = this.hud.karmaMeter.pos;

            pos.x -= 105f * (2 - index) - 0.5f;

            if ((this.hud.owner as Player)?.room != null)
            {
                fRain = (this.hud.owner as Player).room.world.rainCycle.AmountLeft;
            }
            else
                fRain = 1f;
            for (int i = 0; i < this.circles.Length; i++)
            {
                this.circles[i].Update();
                float num = (float)i / (float)(this.circles.Length - 1);
                float value = Mathf.InverseLerp((float)i / (float)this.circles.Length, (float)(i + 1) / (float)this.circles.Length, this.fRain);
                float num2 = Mathf.InverseLerp(0.5f, 0.475f, Mathf.Abs(0.5f - Mathf.InverseLerp(0.0333333351f, 1f, value)));
                this.circles[i].rad = ((2f + num2) + Mathf.InverseLerp(0.075f, 0f, Mathf.Abs(num + this.fRain - 0.075f)) * 2f) * Mathf.InverseLerp(0f, 0.0333333351f, value);
                if (num2 == 0f)
                {
                    this.circles[i].thickness = -1f;
                    this.circles[i].snapGraphic = HUDCircle.SnapToGraphic.Circle4;
                    this.circles[i].snapRad = 2f;
                    this.circles[i].snapThickness = -1f;
                }
                else
                {
                    this.circles[i].thickness = Mathf.Lerp(3.5f, 1f, num2);
                    this.circles[i].snapGraphic = HUDCircle.SnapToGraphic.smallEmptyCircle;
                    this.circles[i].snapRad = 3f;
                    this.circles[i].snapThickness = 1f;
                }
                this.circles[i].pos = this.pos + Custom.DegToVec((1f - (float)i / (float)this.circles.Length) * 360f) * (this.hud.karmaMeter.Radius + 8.5f + num2);
                if (danger[i]) { this.circles[i].color = 1; }
            }
        }
        
        public override void Draw(float timeStacker)
        {
            for (int i = 0; i < this.circles.Length; i++)
            {
                this.circles[i].Draw(timeStacker);
            }
        }
    }
}
