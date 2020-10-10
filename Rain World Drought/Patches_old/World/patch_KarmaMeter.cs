using RWCustom;
using HUD;
using Menu;
using UnityEngine;
using MonoMod;
using System;

[MonoModPatch("global::HUD.KarmaMeter")]
class patch_KarmaMeter : KarmaMeter
{
    [MonoModIgnore]
    public patch_KarmaMeter(HUD.HUD hud, FContainer fContainer, IntVector2 displayKarma, bool showAsReinforced) : base(hud, fContainer, displayKarma, showAsReinforced)
    {
    }

    public override void Update()
    {
        this.lastPos = this.pos;
        this.lastFade = this.fade;
        this.lastRad = this.rad;
        this.lastGlowyFac = this.glowyFac;
        if (this.hud.owner != null & this.hud.owner.GetOwnerType() != HUD.HUD.OwnerType.SleepScreen & this.hud.owner.GetOwnerType() != HUD.HUD.OwnerType.DeathScreen)
        {
            this.pos = new Vector2(55.01f, 45.01f);
            if (this.hud.textPrompt != null && this.hud.textPrompt.foodVisibleMode == 0f)
            {
                this.pos.y = this.pos.y + this.hud.textPrompt.LowerBorderHeight(1f);
            }
        }
        else
        {
            fade = 1f;
        }
        if (this.fade > 0f)
        {
            this.glowyFac = Custom.LerpAndTick(this.glowyFac, this.fade * ((!this.showAsReinforced) ? 0.9f : 1f), 0.1f, 0.0333333351f);
            this.timer++;
        }
        else
        {
            this.glowyFac = 0f;
        }
        this.lastReinforcementCycle = this.reinforcementCycle;
        this.reinforcementCycle += 0.0111111114f;
        if (this.hud.owner is Player && (this.hud.owner as Player).room != null && (this.hud.owner as Player).room.abstractRoom.gate && (this.hud.owner as Player).room.regionGate != null && (this.hud.owner as Player).room.regionGate.mode == RegionGate.Mode.MiddleClosed)
        {
            this.forceVisibleCounter = Math.Max(this.forceVisibleCounter, 10);
        }
        if (this.hud.foodMeter.downInCorner > 0f)
        {
            this.fade = Mathf.Max(0f, this.fade - 0.05f);
        }
        else if (this.Show)
        {
            float num = Mathf.Max((this.forceVisibleCounter <= 0) ? 0f : 1f, 0.25f + 0.75f * ((this.hud.map == null) ? 0f : this.hud.map.fade));
            if (this.hud.showKarmaFoodRain)
            {
                num = 1f;
            }
            if (this.fade < num)
            {
                this.fade = Mathf.Min(num, this.fade + 0.1f);
            }
            else
            {
                this.fade = Mathf.Max(num, this.fade - 0.1f);
            }
        }
        else
        {
            if (this.forceVisibleCounter > 0)
            {
                this.forceVisibleCounter--;
                this.fade = Mathf.Min(1f, this.fade + 0.1f);
            }
            else
            {
                this.fade = Mathf.Max(0f, this.fade - 0.0125f);
            }
            if (this.hud.foodMeter.fade > 0f && this.fade > 0f)
            {
                this.fade = Mathf.Min(this.fade + 0.1f, this.hud.foodMeter.fade);
            }
        }
        this.blinkRed = (this.hud.owner is Player && (this.hud.owner as Player).room != null && (this.hud.owner as Player).room.regionGate != null && (this.hud.owner as Player).room.regionGate.KarmaBlinkRed());
        if (this.hud.HideGeneralHud)
        {
            this.fade = 0f;
        }
        this.rad = Custom.LerpAndTick(this.rad, Custom.LerpMap(this.fade, 0f, 0.15f, 17f, 22.5f, 1.3f), 0.12f, 0.1f);
        if (this.blinkRed && this.timer % 30 > 15)
        {
            if (this.timer % 30 < 20)
            {
                this.rad *= 0.98f;
            }
            this.karmaSprite.color = new Color(1f, 0f, 0f);
            if (this.ringSprite != null)
            {
                this.ringSprite.color = new Color(1f, 0f, 0f);
            }
            this.glowSprite.color = new Color(1f, 0f, 0f);
        }
        else
        {
            this.karmaSprite.color = new Color(1f, 1f, 1f);
            if (this.ringSprite != null)
            {
                this.ringSprite.color = new Color(1f, 1f, 1f);
            }
            this.glowSprite.color = new Color(1f, 1f, 1f);
        }
        if (this.reinforceAnimation > -1)
        {
            this.rad = Custom.LerpMap(this.fade, 0f, 0.15f, 17f, 22.5f, 1.3f);
            this.forceVisibleCounter = Math.Max(this.forceVisibleCounter, 200);
            this.reinforceAnimation++;
            if (this.reinforceAnimation == 20)
            {
                this.hud.PlaySound(SoundID.HUD_Karma_Reinforce_Flicker);
            }
            if (this.reinforceAnimation > 20 && this.reinforceAnimation < 100)
            {
                this.glowyFac = 1f + Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) * 0.03f * Mathf.InverseLerp(20f, 100f, (float)this.reinforceAnimation);
            }
            else if (this.reinforceAnimation == 104)
            {
                this.hud.fadeCircles.Add(new FadeCircle(this.hud, this.rad, 11f, 0.82f, 50f, 4f, this.pos, this.hud.fContainers[1]));
                this.hud.PlaySound(SoundID.HUD_Karma_Reinforce_Small_Circle);
                this.hud.PlaySound(SoundID.HUD_Karma_Reinforce_Contract);
            }
            else if (this.reinforceAnimation > 104 && this.reinforceAnimation < 130)
            {
                this.rad -= Mathf.Pow(Mathf.Sin(Mathf.InverseLerp(104f, 130f, (float)this.reinforceAnimation) * 3.14159274f), 0.5f) * 2f;
                this.fade = 1f - Mathf.Pow(Mathf.Sin(Mathf.InverseLerp(104f, 130f, (float)this.reinforceAnimation) * 3.14159274f), 0.5f) * 0.5f;
            }
            else if (this.reinforceAnimation > 130)
            {
                this.fade = 1f;
                this.rad += Mathf.Sin(Mathf.Pow(Mathf.InverseLerp(130f, 140f, (float)this.reinforceAnimation), 0.2f) * 3.14159274f) * 5f;
                if (this.reinforceAnimation == 134)
                {
                    this.glowyFac = 1.7f;
                }
                else if (this.reinforceAnimation == 135)
                {
                    this.displayKarma = new IntVector2(((this.hud.owner as Player).abstractCreature.world.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma, ((this.hud.owner as Player).abstractCreature.world.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap);
                    this.karmaSprite.element = Futile.atlasManager.GetElementWithName(KarmaMeter.KarmaSymbolSprite(true, this.displayKarma));
                    this.showAsReinforced = ((this.hud.owner as Player).abstractCreature.world.game.session as StoryGameSession).saveState.deathPersistentSaveData.reinforcedKarma;
                    this.hud.fadeCircles.Add(new FadeCircle(this.hud, this.rad, 16f, 0.92f, 100f, 8f, this.pos, this.hud.fContainers[1]));
                    this.hud.PlaySound(SoundID.HUD_Karma_Reinforce_Bump);
                    this.reinforceAnimation = -1;
                }
            }
        }
    }

    public static string KarmaSymbolSprite(bool small, IntVector2 k)
    {
        if (k.y == 12)
        {
            return string.Concat(new object[]
            {
                "smallKarma",
                k.x,
                "-",
                k.y
            });
        }
        if (k.x < 5)
        {
            return ((!small) ? "karma" : "smallKarma") + k.x;
        }
        return string.Concat(new object[]
        {
                (!small) ? "karma" : "smallKarma",
                k.x,
                "-",
                k.y
        });
    }

    public bool Show
    {
        get
        {
            return this.hud.owner.RevealMap || this.hud.showKarmaFoodRain || this.hud.owner.GetOwnerType() == HUD.HUD.OwnerType.CharacterSelect || this.hud.owner.GetOwnerType() == HUD.HUD.OwnerType.SleepScreen || this.hud.owner.GetOwnerType() == HUD.HUD.OwnerType.DeathScreen;
        }
    }
}
