using HUD;
using RWCustom;
using System;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    public class NextKarmaMeter : KarmaMeter
    {
        public NextKarmaMeter(HUD.HUD hud, FContainer fContainer, IntVector2 displayKarma, bool showAsReinforced) : base(hud, fContainer, displayKarma, showAsReinforced)
        {
        }
        
        public override void Update()
        {
            this.lastPos = this.pos;
            this.lastRad = this.rad;
            this.lastGlowyFac = this.glowyFac;
            this.glowyFac = Custom.LerpAndTick(this.glowyFac, ((!this.showAsReinforced) ? 0.9f : 1f), 0.1f, 0.0333333351f);
            this.lastReinforcementCycle = this.reinforcementCycle;
            this.reinforcementCycle += 0.0111111114f;
            this.rad = Custom.LerpAndTick(this.rad, Custom.LerpMap(1f, 0f, 0.15f, 17f, 22.5f, 1.3f), 0.12f, 0.1f);

            this.karmaSprite.color = new Color(1f, 1f, 1f);
            if (this.ringSprite != null) { this.ringSprite.color = new Color(1f, 1f, 1f); }
            this.glowSprite.color = new Color(1f, 1f, 1f);
        }

        public override void Draw(float timeStacker)
        {
            Vector2 vector = this.DrawPos((float)this.timer);
            this.karmaSprite.x = vector.x;
            this.karmaSprite.y = vector.y;
            this.karmaSprite.scale = Mathf.Lerp(this.lastRad, this.rad, timeStacker) / 22.5f;
            this.karmaSprite.alpha = 1f;
            if (this.showAsReinforced)
            {
                if (this.ringSprite == null)
                {
                    this.ringSprite = new FSprite("smallKarmaRingReinforced", true);
                    this.hud.fContainers[1].AddChild(this.ringSprite);
                }
                this.ringSprite.x = vector.x;
                this.ringSprite.y = vector.y;
                this.ringSprite.scale = Mathf.Lerp(this.lastRad, this.rad, timeStacker) / 22.5f;
                float num2 = Mathf.InverseLerp(0.1f, 0.85f, this.hud.foodMeter.forceSleep);
                this.ringSprite.alpha = Mathf.InverseLerp(0.2f, 0f, num2);
                if (num2 > 0f)
                {
                    if (this.vectorRingSprite == null)
                    {
                        this.vectorRingSprite = new FSprite("Futile_White", true);
                        this.vectorRingSprite.shader = this.hud.rainWorld.Shaders["VectorCircleFadable"];
                        this.hud.fContainers[1].AddChild(this.vectorRingSprite);
                    }
                    this.vectorRingSprite.isVisible = true;
                    this.vectorRingSprite.x = vector.x;
                    this.vectorRingSprite.y = vector.y;
                    float num3 = Mathf.Lerp(this.lastRad, this.rad, (float)this.timer) + 8f + 100f * Custom.SCurve(Mathf.InverseLerp(0.2f, 1f, num2), 0.75f);
                    this.vectorRingSprite.scale = num3 / 8f;
                    float num4 = 2f * Mathf.Pow(Mathf.InverseLerp(0.4f, 0.2f, num2), 2f) + 2f * Mathf.Pow(Mathf.InverseLerp(1f, 0.2f, num2), 0.5f);
                    this.vectorRingSprite.color = new Color(0f, 1f, Mathf.Pow(Mathf.InverseLerp(1f, 0.2f, num2), 3f), num4 / num3);
                }
                else if (this.vectorRingSprite != null)
                {
                    this.vectorRingSprite.RemoveFromContainer();
                    this.vectorRingSprite = null;
                }
            }
            else
            {
                if (this.ringSprite != null)
                {
                    this.ringSprite.RemoveFromContainer();
                    this.ringSprite = null;
                }
                if (this.vectorRingSprite != null)
                {
                    this.vectorRingSprite.RemoveFromContainer();
                    this.vectorRingSprite = null;
                }
            }
            this.darkFade.x = this.DrawPos(timeStacker).x;
            this.darkFade.y = this.DrawPos(timeStacker).y;
            this.darkFade.scaleX = 18.75f;
            this.darkFade.scaleY = 18.75f;
            this.darkFade.alpha = 0.2f;
            float num5 = 0.7f + 0.3f * Mathf.Sin(6.28318548f * Mathf.Lerp(this.lastReinforcementCycle, this.reinforcementCycle, timeStacker));
            float num6 = Mathf.Lerp(this.lastGlowyFac, this.glowyFac, timeStacker);
            num5 *= Mathf.InverseLerp(0.9f, 1f, num6);
            this.glowSprite.x = this.DrawPos(timeStacker).x;
            this.glowSprite.y = this.DrawPos(timeStacker).y;
            this.glowSprite.scale = (60f + 10f * num5) * num6 / 8f;
            this.glowSprite.alpha = (0.2f + 0.2f * num5) * num6;
        }
    }
}
