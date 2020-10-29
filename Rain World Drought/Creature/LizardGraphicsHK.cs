using LizardCosmetics;
using Rain_World_Drought.Enums;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    internal static class LizardGraphicsHK
    {
        public static void Patch()
        {
            On.LizardGraphics.ctor += new On.LizardGraphics.hook_ctor(GraphicsCtorHK);
            On.LizardGraphics.HeadColor += new On.LizardGraphics.hook_HeadColor(HeadColorHK);
            On.LizardCosmetics.SpineSpikes.ctor += new On.LizardCosmetics.SpineSpikes.hook_ctor(SpineSpikesCtorHK);
            On.LizardCosmetics.SpineSpikes.ApplyPalette += new On.LizardCosmetics.SpineSpikes.hook_ApplyPalette(SpineSpikesApplyPaletteHK);
        }

        private static void GraphicsCtorHK(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
        {
            orig.Invoke(self, ow);
            int spr = self.startOfExtraSprites + self.extraSprites;
            if (DroughtMod.EnumExt && self.lizard.Template.type == EnumExt_Drought.GreyLizard)
            {
                spr = self.AddCosmetic(spr, new LongShoulderScales(self, spr));
                spr = self.AddCosmetic(spr, new SpineSpikes(self, spr));
                if (Random.value < 0.5f) { self.AddCosmetic(spr, new TailFin(self, spr)); }
                else { self.AddCosmetic(spr, new TailTuft(self, spr)); }
            }
        }

        private static Color HeadColorHK(On.LizardGraphics.orig_HeadColor orig, LizardGraphics self, float timeStacker)
        {
            Color res = orig.Invoke(self, timeStacker);
            if (DroughtMod.EnumExt && self.lizard.Template.type == EnumExt_Drought.GreyLizard)
            { res = Color.Lerp(res, new Color(0.9f, 0.9f, 0.9f), self.blackLizardLightUpHead); }
            return res;
        }

        private static void SpineSpikesCtorHK(On.LizardCosmetics.SpineSpikes.orig_ctor orig, SpineSpikes self, LizardGraphics lGraphics, int startSprite)
        {
            orig.Invoke(self, lGraphics, startSprite);
            if (DroughtMod.EnumExt && lGraphics.lizard.Template.type == EnumExt_Drought.GreyLizard)
            {
                self.sizeRangeMin = self.sizeRangeMin * 1.2f;
                self.sizeRangeMax = self.sizeRangeMax * 1.6f;
                self.graphic = Random.value < 0.2f ? 2 : 3;
                self.colored = Random.value < 0.5f ? 1 : 2;
                self.numberOfSprites = self.bumps * 2;
            }
        }

        private static void SpineSpikesApplyPaletteHK(On.LizardCosmetics.SpineSpikes.orig_ApplyPalette orig, SpineSpikes self,
           RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig.Invoke(self, sLeaser, rCam, palette);
            if (DroughtMod.EnumExt && self.lGraphics.lizard.Template.type == EnumExt_Drought.GreyLizard)
            {
                for (int i = self.startSprite; i < self.startSprite + self.bumps; i++)
                {
                    if (self.colored == 1)
                    {
                        sLeaser.sprites[i + self.bumps].color = Color.Lerp(self.lGraphics.effectColor, new Color(0.9f, 0.9f, 0.9f), self.lGraphics.blackLizardLightUpHead);
                    }
                    else if (self.colored == 2)
                    {
                        sLeaser.sprites[i + self.bumps].color = Color.Lerp(sLeaser.sprites[i + self.bumps].color, new Color(0.9f, 0.9f, 0.9f), self.lGraphics.blackLizardLightUpHead);
                    }
                }
            }
        }
    }
}
