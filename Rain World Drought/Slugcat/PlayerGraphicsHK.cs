using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rain_World_Drought.Slugcat
{
    internal static class PlayerGraphicsHK
    {
        public static void Patch()
        {
            On.PlayerGraphics.InitiateSprites += new On.PlayerGraphics.hook_InitiateSprites(InitiateSpritesHK);
            On.PlayerGraphics.AddToContainer += new On.PlayerGraphics.hook_AddToContainer(AddToContainerHK);
            On.PlayerGraphics.Reset += new On.PlayerGraphics.hook_Reset(ResetHK);
            On.PlayerGraphics.Update += new On.PlayerGraphics.hook_Update(UpdateHK);
            On.PlayerGraphics.DrawSprites += new On.PlayerGraphics.hook_DrawSprites(DrawSpritesHK);
            On.PlayerGraphics.ApplyPalette += new On.PlayerGraphics.hook_ApplyPalette(ApplyPaletteHK);
            On.PlayerGraphics.SlugcatColor += new On.PlayerGraphics.hook_SlugcatColor(SlugcatColor);
        }

        public static int AddCosmetics(WandererSupplement sub, int spriteIndex, PlayerCosmetics cosmetic)
        {
            sub.cosmetics.Add(cosmetic);
            spriteIndex += cosmetic.numberOfSprites;
            sub.extraSprites += cosmetic.numberOfSprites;
            return spriteIndex;
        }

        private static void InitiateSpritesHK(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            WandererSupplement sub = WandererSupplement.GetSub(self.player);
            sub.cosmetics = new List<PlayerCosmetics>();
            orig.Invoke(self, sLeaser, rCam);

            if (!WandererSupplement.IsWanderer(self.player)) { return; }
            for (int i = 0; i < sLeaser.sprites.Length; i++) { sLeaser.sprites[i].RemoveFromContainer(); }

            sub.origSprites = sLeaser.sprites.Length;
            int num = sub.origSprites;
            sub.extraSprites = 0; sub.tailLength = 0f;
            for (int l = 0; l < self.tail.Length; l++)
            { sub.tailLength += self.tail[l].connectionRad; }

            num = AddCosmetics(sub, num, new MoonMark(self, num));
            num = AddCosmetics(sub, num, new TailRing(self, num, 0));
            num = AddCosmetics(sub, num, new TailRing(self, num, 1));
            num = AddCosmetics(sub, num, new TailRing(self, num, 2));
            num = AddCosmetics(sub, num, new FocusHalo(self, num));

            Array.Resize(ref sLeaser.sprites, num);
            for (int l = 0; l < sub.cosmetics.Count; l++) { sub.cosmetics[l].InitiateSprites(sLeaser, rCam); }

            self.AddToContainer(sLeaser, rCam, null);
        }

        private static void AddToContainerHK(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            WandererSupplement sub = WandererSupplement.GetSub(self.player);
            for (int j = 0; j < sub.cosmetics.Count; j++)
            {
                if (sub.cosmetics[j].spritesOverlap == PlayerCosmetics.SpritesOverlap.Behind)
                { sub.cosmetics[j].AddToContainer(sLeaser, rCam, newContatiner); }
            }

            orig.Invoke(self, sLeaser, rCam, newContatiner);

            for (int num = 0; num < sub.cosmetics.Count; num++)
            {
                if (sub.cosmetics[num].spritesOverlap == PlayerCosmetics.SpritesOverlap.InFront)
                { sub.cosmetics[num].AddToContainer(sLeaser, rCam, newContatiner); }
            }
        }

        private static void ResetHK(On.PlayerGraphics.orig_Reset orig, PlayerGraphics self)
        {
            orig.Invoke(self);
            WandererSupplement sub = WandererSupplement.GetSub(self.player);
            for (int l = 0; l < sub.cosmetics.Count; l++)
            { sub.cosmetics[l].Reset(); }
        }

        private static void UpdateHK(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
        {
            WandererSupplement sub = WandererSupplement.GetSub(self.player);
            if (!self.culled)
            {
                for (int l = 0; l < sub.cosmetics.Count; l++)
                { sub.cosmetics[l].Update(); }
            }
            orig.Invoke(self);
        }

        private static void DrawSpritesHK(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            WandererSupplement sub = WandererSupplement.GetSub(self.player);
            if (sub.voidEnergy) { self.ApplyPalette(sLeaser, rCam, rCam.currentPalette); }

            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);

            if (rCam != null)
            {
                for (int j = 0; j < sub.cosmetics.Count; j++)
                { sub.cosmetics[j].DrawSprites(sLeaser, rCam, timeStacker, camPos); }
            }
            if (sub.rad > 1) { sLeaser.sprites[9].element = Futile.atlasManager.GetElementWithName("FaceStunned"); }
        }

        private static void ApplyPaletteHK(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig.Invoke(self, sLeaser, rCam, palette);
            WandererSupplement sub = WandererSupplement.GetSub(self.player);

            float voidInEffect = 0f;
            if (sub.voidEnergy) { voidInEffect = (1f - WandererSupplement.maxEnergy) / 1.2f; }
            Color body = Color.Lerp(PlayerGraphics.SlugcatColor(self.player.playerState.slugcatCharacter), Color.white, voidInEffect);
            Color eye = palette.blackColor;
            if (self.malnourished > 0f)
            {
                float num = (!self.player.Malnourished) ? Mathf.Max(0f, self.malnourished - 0.005f) : self.malnourished;
                body = Color.Lerp(body, Color.gray, 0.4f * num);
                eye = Color.Lerp(eye, Color.Lerp(Color.white, palette.fogColor, 0.5f), 0.2f * num * num);
            }
            if (self.player.playerState.slugcatCharacter == WandererSupplement.SlugcatCharacter)
            {
                eye = Color.Lerp(new Color(1f, 1f, 1f), body, 0.3f);
                body = Color.Lerp(palette.blackColor, Color.Lerp(PlayerGraphics.SlugcatColor(self.player.playerState.slugcatCharacter), Color.white, voidInEffect), Mathf.Lerp(0.08f, 0.04f, palette.darkness));
            }
            else if (self.player.room.game.IsStorySession)
            {
                body = Color.Lerp(PlayerGraphics.SlugcatColor(self.player.playerState.slugcatCharacter), Color.white, voidInEffect);
                eye = Color.Lerp(new Color(1f, 1f, 1f), body, 0.3f);
                body = Color.Lerp(palette.blackColor, Color.Lerp(PlayerGraphics.SlugcatColor(self.player.playerState.slugcatCharacter), Color.white, voidInEffect), Mathf.Lerp(0.08f, 0.04f, palette.darkness));
            }

            for (int i = 0; i < sub.origSprites; i++) { sLeaser.sprites[i].color = body; }
            body = Color.Lerp(PlayerGraphics.SlugcatColor(self.player.playerState.slugcatCharacter), Color.white, voidInEffect);
            sLeaser.sprites[11].color = Color.Lerp(body, Color.white, 0.3f);
            sLeaser.sprites[10].color = body;
            sLeaser.sprites[9].color = eye;

            for (int i = 0; i < sub.cosmetics.Count; i++)
            {
                sub.cosmetics[i].ApplyPalette(sLeaser, rCam, palette);
            }
        }

        private static Color SlugcatColor(On.PlayerGraphics.orig_SlugcatColor orig, int i)
        {
            if (i == WandererSupplement.SlugcatCharacter) { return Custom.HSL2RGB(0.63055557f, 0.54f, 0.2f); }
            return orig.Invoke(i);
        }
    }
}
