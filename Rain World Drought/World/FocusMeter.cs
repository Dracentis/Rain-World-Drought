using System;
using UnityEngine;
using Menu;
using RWCustom;
using Rain_World_Drought.Slugcat;

namespace HUD
{
    public class FocusMeter : HudPart
    {
        private static FocusMeter instance;

        public FSprite[] pips;
        public int forceVisible;
        public float alpha;
        public float lastAlpha;
        public Vector2 pos;
        public Vector2 lastPos;
        public int doubleEnergyLeft;
        public int denyAnim;
        public int rechargeAnim;
        public int useAnim;
        private const int rechargeAnimLength = 40;
        private const int denyAnimLength = 30;
        private const int useAnimLength = 5;
        private const float pipScale = 5f / 8f;

        public FocusMeter(HUD hud, FContainer fContainer) : base(hud)
        {
            instance = this;
            if (hud.owner.GetOwnerType() != HUD.OwnerType.Player) {
                return;
            }
            pips = new FSprite[WandererSupplement.maxEnergy + 1];
            for(int i = 0; i < pips.Length; i++)
            {
                pips[i] = new FSprite("Futile_White")
                {
                    shader = hud.rainWorld.Shaders["VectorCircleFadable"],
                    color = new Color(0f, 0f, 0f, 0f)
                };
                fContainer.AddChild(pips[i]);
            }
        }

        public static void ShowMeter(int ticks)
        {
            if (instance == null) return;
            if (ticks > instance.forceVisible)
                instance.forceVisible = ticks;
        }

        public static void DenyAnimation()
        {
            if (instance == null) return;
            instance.denyAnim = denyAnimLength;
        }

        public static void RechargeAnimation()
        {
            if (instance == null) return;
            instance.rechargeAnim = rechargeAnimLength;
        }

        public static void UpdateFocus(int energy, bool halfPip) => UpdateFocus(2 * energy + (halfPip ? 1 : 0));
        public static void UpdateFocus(int doubleEnergy)
        {
            if (instance == null) return;
            if (instance.doubleEnergyLeft > doubleEnergy)
            {
                instance.useAnim = useAnimLength;
                // Play a sound if only a half pip was used
                // In all other cases another sound will already be playing
                if ((doubleEnergy == instance.doubleEnergyLeft - 1) && instance.hud.owner is Player ply)
                {
                    if (WandererSupplement.IsWanderer(ply))
                        instance.PlayTick(1f, 0.75f + 0.25f * WandererSupplement.GetSub(ply).Energy);
                }
            }
            instance.doubleEnergyLeft = doubleEnergy;
        }

        public bool Show => hud.owner.RevealMap || hud.showKarmaFoodRain || forceVisible > 0;

        public override void Update()
        {
            lastPos = pos;
            lastAlpha = alpha;

            Vector2 offset = new Vector2(35f + hud.rainMeter.fade * 10f, 27f);
            pos = hud.karmaMeter.pos + offset;

            if (forceVisible > 0) forceVisible--;

            // Anim timers
            if (rechargeAnim > 0)
            {
                int lastPipCount = Mathf.RoundToInt(WandererSupplement.maxEnergy * Mathf.Clamp01(1.8f - 1.8f * rechargeAnim / rechargeAnimLength));
                rechargeAnim--;
                int pipCount = Mathf.RoundToInt(WandererSupplement.maxEnergy * Mathf.Clamp01(1.8f - 1.8f * rechargeAnim / rechargeAnimLength));
                if (lastPipCount != pipCount)
                    PlayTick(0.75f, 0.75f + 0.25f * (pipCount / (float)WandererSupplement.maxEnergy));
            }
            if (denyAnim > 0)
            {
                if (denyAnim % 10 == 5)
                    PlayTick(1f, 0.75f);
                denyAnim--;
            }
            if (useAnim > 0)
                useAnim--;

            alpha = Custom.LerpAndTick(alpha, Show ? 1f : 0f, 0.1f, 0.025f);
        }
        
        private void PlayTick(float volume, float pitch)
        {
            if (hud.owner is Player ply) ply.room.PlaySound(SoundID.Overseer_Image_Small_Flicker, 0f, volume / 0.3f, pitch);
        }

        public override void Draw(float timeStacker)
        {
            const float spacing = 12f;

            // Force show during the deny animation
            float drawAlpha = Mathf.Lerp(lastAlpha, alpha, timeStacker);
            if (denyAnim > 0) drawAlpha = 1f;

            if (alpha == 0f && lastAlpha == 0f && drawAlpha == 0f) return;

            if (pips == null) return;
            float fill;
            Vector2 drawPos = Vector2.Lerp(lastPos, pos, timeStacker);
            for(int i = 0; i < pips.Length; i++)
            {
                if (doubleEnergyLeft <= i * 2) fill = 0f;
                else if (doubleEnergyLeft > i * 2 + 1) fill = 1f;
                else fill = 0.35f;

                if(useAnim > 0)
                {
                    float animProg = 1f - (useAnim / (float)useAnimLength);
                    float oldFill;
                    if (doubleEnergyLeft + 1 <= i * 2) oldFill = 0f;
                    else if (doubleEnergyLeft + 1 > i * 2 + 1) oldFill = 1f;
                    else oldFill = 0.35f;
                    fill = Mathf.Lerp(oldFill, fill, animProg);
                }

                bool denyFlash = denyAnim % 10 > 5;
                if (denyFlash && i < WandererSupplement.maxEnergy) fill = Mathf.Max(0.25f, fill);

                float yOffset = 0f;

                if (rechargeAnim > 0)
                {
                    float animProg = 1f - (rechargeAnim / (float)rechargeAnimLength);
                    animProg = Mathf.InverseLerp(0f, 0.3f, 1.5f * animProg - 0.3f * (i / (pips.Length - 1f)));
                    fill *= animProg;
                    yOffset = 5f * Mathf.Sin(animProg * Mathf.PI * 2f) * (1f - animProg);
                }

                pips[i].color = new Color(denyFlash ? (1f / 255f) : 0f, 0f, drawAlpha, fill);
                pips[i].isVisible = fill > 0.01f && drawAlpha > 0f;
                pips[i].x = drawPos.x + spacing * i;
                pips[i].y = drawPos.y + yOffset;
                pips[i].scale = pipScale;
            }
        }
        
        public override void ClearSprites()
        {
            instance = null;
            base.ClearSprites();
            foreach(FSprite sprite in pips)
                sprite.RemoveFromContainer();
            pips = null;
        }
    }
}
