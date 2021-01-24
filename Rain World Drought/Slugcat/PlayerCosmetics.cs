using RWCustom;
using System;
using UnityEngine;

namespace Rain_World_Drought.Slugcat
{
    public class TailRing : PlayerCosmetics
    {
        public TailRing(PlayerGraphics pGraphics, int startSprite, int tailSegment) : base(pGraphics, startSprite)
        {
            spritesOverlap = SpritesOverlap.InFront;
            this.tailSegment = tailSegment;
            numberOfSprites = 2;
        }

        public override void Update()
        {
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);
            sLeaser.sprites[startSprite] = TriangleMesh.MakeLongMesh(1, false, true);
            sLeaser.sprites[startSprite + 1] = TriangleMesh.MakeLongMesh(1, false, true);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

            PlayerTailData tailData = TailPosition(tailSegment, timeStacker);

            (sLeaser.sprites[startSprite] as TriangleMesh).MoveVertice(0, tailData.pos - camPos);
            (sLeaser.sprites[startSprite] as TriangleMesh).MoveVertice(1, tailData.pos + tailData.dir - camPos);
            (sLeaser.sprites[startSprite] as TriangleMesh).MoveVertice(2, tailData.outerPos - camPos);
            (sLeaser.sprites[startSprite] as TriangleMesh).MoveVertice(3, tailData.outerPos + tailData.dir - camPos);

            (sLeaser.sprites[startSprite + 1] as TriangleMesh).MoveVertice(0, tailData.pos - camPos);
            (sLeaser.sprites[startSprite + 1] as TriangleMesh).MoveVertice(1, tailData.pos + tailData.dir - camPos);
            (sLeaser.sprites[startSprite + 1] as TriangleMesh).MoveVertice(2, tailData.innerPos - camPos);
            (sLeaser.sprites[startSprite + 1] as TriangleMesh).MoveVertice(3, tailData.innerPos + tailData.dir - camPos);

            if (rCam != null)
            {
                ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            float voidInEffect = 0f;
            WandererSupplement sub = WandererSupplement.GetSub(pGraphics.player);

            if (sub.voidEnergy)
                voidInEffect = sub.voidEnergyAmount / 1.2f;
            Color color = Color.Lerp(PlayerGraphics.SlugcatColor(sub.self.playerState.slugcatCharacter), Color.white, voidInEffect);
            int order = -tailSegment;
            float alpha = 1f;
            if (sub.Slowdown == 0f)
            {
                alpha = sub.Energy * Mathf.Abs(Mathf.Sin(rCam.room.game.clock / 20.0375f + order * 0.5f) / 2f);
                alpha = alpha * 0.8f + 0.2f;
            }
            else if (sub.Slowdown > 0f)
            {
                alpha = 1f;
            }
            else
            {
                alpha = sub.Energy * Mathf.Abs(Mathf.Sin(rCam.room.game.clock / 40.075f)) / 1.2f;
            }
            if ((2 - tailSegment) >= sub.AirJumpsLeft)
                alpha = 0f;

            //pGraphics.owner.room.world.rainCycle.timer;
            sLeaser.sprites[startSprite].alpha = alpha;
            sLeaser.sprites[startSprite + 1].alpha = alpha;

            sLeaser.sprites[startSprite].color = Color.Lerp(color, Color.white, alpha);//palette.blackColor;
            sLeaser.sprites[startSprite + 1].color = Color.Lerp(color, Color.white, alpha);//palette.blackColor;
            base.ApplyPalette(sLeaser, rCam, palette);
        }

        public int tailSegment;

        public PlayerTailData TailPosition(int tailSegment, float timeStacker)
        {
            TailSegment tail = this.pGraphics.tail[tailSegment];
            Vector2 pos = Vector2.Lerp(tail.lastPos, tail.pos, timeStacker);
            float rad = tail.rad;
            Vector2 dir;

            if (tailSegment == 0)
            {
                BodyChunk btm = this.pGraphics.owner.bodyChunks[1];
                Vector2 btmPos = Vector2.Lerp(btm.lastPos, btm.pos, timeStacker);
                dir = (btmPos - pos).normalized;
            }
            else
            {
                TailSegment lastTail = this.pGraphics.tail[tailSegment - 1];
                Vector2 lastTailPos = Vector2.Lerp(lastTail.lastPos, lastTail.pos, timeStacker);
                dir = (lastTailPos - pos).normalized;
            }

            Vector2 perp = Custom.RotateAroundOrigo(dir, 80);
            Vector2 outerPos = pos + rad * perp;
            perp = Custom.RotateAroundOrigo(dir, -80);
            Vector2 innerPos = pos + rad * perp;
            perp = Custom.RotateAroundOrigo(dir, 90);

            return new PlayerTailData(pos, outerPos, innerPos, dir, perp, rad);
        }

        public struct PlayerTailData
        {
            public PlayerTailData(Vector2 pos, Vector2 outerPos, Vector2 innerPos, Vector2 dir, Vector2 perp, float rad)
            {
                this.pos = pos;
                this.outerPos = outerPos;
                this.innerPos = innerPos;
                this.dir = dir;
                this.perp = perp;
                this.rad = rad;
            }

            public Vector2 pos;
            public Vector2 outerPos;
            public Vector2 innerPos;
            public Vector2 dir;
            public Vector2 perp;
            public float rad;
        }
    }
    
    public class FocusHalo : PlayerCosmetics
    {
        public FocusHalo(PlayerGraphics pGraphics, int startSprite) : base(pGraphics, startSprite)
        {
            spritesOverlap = SpritesOverlap.InFront;
            numberOfSprites = 2;
        }

        public override void Update()
        {
            lastFocus = focus;
            Player ply = (Player)pGraphics.owner;
            WandererSupplement sub = WandererSupplement.GetSub(ply);
            focus = Custom.LerpAndTick(focus, (sub.Focus + sub.Slowdown > 0f) ? 1f : 0f, 0.15f, 0.05f);
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (spotDesaturate == null)
            {
                Material mat = new Material(Resource.Shaders.SpotDesaturate);
                spotDesaturate = FShader.CreateShader("SpotDesaturate", mat.shader);
            }

            sLeaser.sprites[startSprite] = new FSprite("Futile_White")
            {
                shader = spotDesaturate
            };
            sLeaser.sprites[startSprite + 1] = new FSprite("Futile_White")
            {
                shader = rCam.game.rainWorld.Shaders["FlatLight"]
            };
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
        {
            newContainer = rCam.ReturnFContainer("Bloom");
            newContainer.AddChild(sLeaser.sprites[startSprite + 0]);
            newContainer.AddChild(sLeaser.sprites[startSprite + 1]);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

            Vector2 drawPos = Vector2.Lerp(
                Vector2.Lerp(pGraphics.drawPositions[0, 1], pGraphics.drawPositions[0, 0], timeStacker),
                Vector2.Lerp(pGraphics.drawPositions[1, 1], pGraphics.drawPositions[1, 0], timeStacker),
                0.5f);

            float alpha = Mathf.Lerp(lastFocus, focus, timeStacker);
            if (alpha < 0.01f)
            {
                sLeaser.sprites[startSprite + 0].isVisible = false;
                sLeaser.sprites[startSprite + 1].isVisible = false;
            }
            else
            {
                sLeaser.sprites[startSprite + 0].SetPosition(drawPos - camPos);
                sLeaser.sprites[startSprite + 0].isVisible = true;
                sLeaser.sprites[startSprite + 0].alpha = alpha * 0.75f;
                sLeaser.sprites[startSprite + 0].scale = 100f * alpha / 8f;
                sLeaser.sprites[startSprite + 1].SetPosition(drawPos - camPos);
                sLeaser.sprites[startSprite + 1].isVisible = true;
                sLeaser.sprites[startSprite + 1].alpha = alpha * 0.15f;
                sLeaser.sprites[startSprite + 1].scale = 150f * alpha / 8f;
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[startSprite + 0].color = Color.white;
            sLeaser.sprites[startSprite + 1].color = Color.Lerp(PlayerGraphics.SlugcatColor(0), Color.white, 0.9f);
            base.ApplyPalette(sLeaser, rCam, palette);
        }

        public static FShader spotDesaturate;
        private float focus;
        private float lastFocus;
    }
    
    public class MoonMark : PlayerCosmetics
    {
        public MoonMark(PlayerGraphics pGraphics, int startSprite) : base(pGraphics, startSprite)
        {
            spritesOverlap = SpritesOverlap.InFront;
            numberOfSprites = 2;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);

            sLeaser.sprites[startSprite] = new FSprite("Futile_White", true);
            sLeaser.sprites[startSprite].shader = rCam.game.rainWorld.Shaders["FlatLight"];
            sLeaser.sprites[startSprite + 1] = new FSprite("MoonMark", true);
            sLeaser.sprites[startSprite + 1].scale = 1f;
        }

        private float fadeOutProgress = 0f; // from 0 to 1 fade out
        private float bumpProgress = 0f;    // from 0 to 1 bump
        private float fadeProgress = 0f;    // from 0 to 1 fade light and mark out

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
        {
            newContainer = rCam.ReturnFContainer("Foreground");
            newContainer.AddChild(sLeaser.sprites[startSprite]);
            newContainer.AddChild(sLeaser.sprites[startSprite + 1]);
        }

        public override void Update()
        {
            base.Update();
            this.lastAlpha = this.alpha;
            if (this.pGraphics.player?.room?.game == null || this.pGraphics.player.dead
                || !(this.pGraphics.player.room.game.session is StoryGameSession) || !(this.pGraphics.player.room.game.session as StoryGameSession).saveState.miscWorldSaveData.moonRevived)
            { this.alpha = 0f; return; }

            WandererSupplement sub = WandererSupplement.GetSub(this.pGraphics.player);
            if (sub.pearlConversation.talking)
            { this.alpha = this.pGraphics.markAlpha; }
            else if (sub.past25000)
            { this.alpha = 0f; }
            else if (sub.past22000)
            {
                if (this.fadeOutProgress > 0f && this.fadeOutProgress < 1f)
                { this.alpha = Mathf.Lerp(this.alpha, 1f - this.fadeOutProgress, 0.1f); }
                else if (this.bumpProgress > 0f && this.bumpProgress < 1f)
                { this.alpha = Mathf.Lerp(this.alpha, this.bumpProgress, 0.1f); }
                else if (this.fadeProgress > 0f && this.fadeProgress < 1f)
                { this.alpha = Mathf.Lerp(this.alpha, 1f - this.fadeProgress, 0.1f); }
            }
            else
            {
                this.alpha = Custom.LerpAndTick(this.alpha, Mathf.Clamp(Mathf.InverseLerp(50f, 100f, (float)this.pGraphics.player.touchedNoInputCounter) - UnityEngine.Random.value * Mathf.InverseLerp(100f, 50f, (float)this.pGraphics.player.touchedNoInputCounter), 0f, 1f) * this.pGraphics.markBaseAlpha, 0.1f, 0.0333333351f);
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            WandererSupplement sub = WandererSupplement.GetSub(this.pGraphics.player);
            float breath = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(this.pGraphics.lastBreath, this.pGraphics.breath, timeStacker) * 3.14159274f * 2f);
            Vector2 draw0 = Vector2.Lerp(this.pGraphics.drawPositions[0, 1], this.pGraphics.drawPositions[0, 0], timeStacker);
            Vector2 draw1 = Vector2.Lerp(this.pGraphics.drawPositions[1, 1], this.pGraphics.drawPositions[1, 0], timeStacker);
            Vector2 head = Vector2.Lerp(this.pGraphics.head.lastPos, this.pGraphics.head.pos, timeStacker);
            if (this.pGraphics.player.aerobicLevel > 0.5f)
            {
                draw0 += Custom.DirVec(draw1, draw0) * Mathf.Lerp(-1f, 1f, breath) * Mathf.InverseLerp(0.5f, 1f, this.pGraphics.player.aerobicLevel) * 0.5f;
                head -= Custom.DirVec(draw1, draw0) * Mathf.Lerp(-1f, 1f, breath) * Mathf.Pow(Mathf.InverseLerp(0.5f, 1f, this.pGraphics.player.aerobicLevel), 1.5f) * 0.75f;
            }
            if (this.pGraphics.player.sleepCurlUp > 0f)
            {
                head.y += 1f * this.pGraphics.player.sleepCurlUp;
                head.x += Mathf.Sign(draw0.x - draw1.x) * 2f * this.pGraphics.player.sleepCurlUp;
            }
            Vector2 vector4 = head + Custom.DirVec(draw1, head) * 8f;
            float moonMarkAlpha = Mathf.Lerp(this.lastAlpha, this.alpha, timeStacker);
            sLeaser.sprites[this.startSprite].x = vector4.x - camPos.x;
            sLeaser.sprites[this.startSprite].y = vector4.y - camPos.y;
            sLeaser.sprites[this.startSprite].alpha = 0.2f * moonMarkAlpha;
            sLeaser.sprites[this.startSprite].scale = 1f + moonMarkAlpha;
            sLeaser.sprites[this.startSprite + 1].x = vector4.x - camPos.x;
            sLeaser.sprites[this.startSprite + 1].y = vector4.y - camPos.y;
            sLeaser.sprites[this.startSprite + 1].alpha = moonMarkAlpha;
            sLeaser.sprites[this.startSprite + 1].rotation = Custom.DirVec(draw1, head).GetAngle() + 90f;
            if (sub.past22000 && !sub.past25000 && this.fadeOutProgress == 0f)
            {
                this.fadeOutProgress = 0.0001f;
            }
            else if (this.fadeOutProgress > 0f && this.fadeOutProgress < 1f)
            {
                this.fadeOutProgress = Mathf.Clamp(this.fadeOutProgress + 0.01f, 0f, 1f);
                if (this.fadeOutProgress == 1f)
                {
                    this.pGraphics.player.Stun(400);
                    this.pGraphics.player.room.PlaySound(SoundID.HUD_Karma_Reinforce_Bump, 0f, 0.3f, 1f);
                    this.pGraphics.player.room.PlaySound(SoundID.HUD_Karma_Reinforce_Contract, 0f, 0.3f, 1f);
                    this.pGraphics.player.room.PlaySound(SoundID.MENU_Karma_Ladder_Increase_Bump, 0f, 0.3f, 1f);
                    this.pGraphics.player.room.AddObject(new ShockWave(this.pGraphics.player.mainBodyChunk.pos, 600f, 0.5f, 10));
                    bumpProgress = 0.0001f;
                }
            }
            else if (this.bumpProgress > 0f && this.bumpProgress < 1f)
            {
                sLeaser.sprites[this.startSprite].alpha = moonMarkAlpha;
                sLeaser.sprites[this.startSprite].scale = 1f + 50f * moonMarkAlpha;
                bumpProgress = Mathf.Clamp(bumpProgress + 0.05f, 0f, 1f);
                sLeaser.sprites[this.startSprite + 1].element = Futile.atlasManager.GetElementWithName("GiantPistonA");
                if (bumpProgress == 1f)
                {
                    this.pGraphics.player.room.PlaySound(SoundID.MENU_Karma_Ladder_Reinforcement_Dissipate_A, 0f, 0.3f, 1f);
                    this.pGraphics.player.room.PlaySound(SoundID.MENU_Karma_Ladder_Reinforcement_Dissipate_B, 0f, 0.3f, 1f);
                    this.alpha = 1f;
                    fadeProgress = 0.0001f;
                }
            }
            else if (this.fadeProgress > 0f && this.fadeProgress < 1f)
            {
                sLeaser.sprites[this.startSprite].alpha = moonMarkAlpha;
                sLeaser.sprites[this.startSprite].scale = 1f + 50f * moonMarkAlpha;
                fadeProgress = Mathf.Clamp(fadeProgress + 0.002f, 0f, 1f);
                if (fadeProgress == 1f)
                {
                    sLeaser.sprites[this.startSprite].isVisible = false;
                    sLeaser.sprites[this.startSprite + 1].isVisible = false;
                }
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            base.ApplyPalette(sLeaser, rCam, palette);
            WandererSupplement sub = WandererSupplement.GetSub(this.pGraphics.player);
            Color body = sLeaser.sprites[0].color;
            sLeaser.sprites[this.startSprite].color = body;
            sLeaser.sprites[this.startSprite + 1].color = Color.Lerp(body, Color.white, 0.3f);
            if (sub.past22000)
            {
                sLeaser.sprites[this.startSprite].color = Color.Lerp(body, Color.white, 0.5f);
                sLeaser.sprites[this.startSprite + 1].color = Color.Lerp(body, Color.white, 0.9f);
            }
        }

        private float alpha, lastAlpha;
    }

    public abstract class PlayerCosmetics
    {
        public PlayerCosmetics(PlayerGraphics pGraphics, int startSprite)
        {
            this.pGraphics = pGraphics;
            this.startSprite = startSprite;
            this.init = false;
        }
        
        public virtual void Update()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            this.init = true;
        }

        public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
        }

        public virtual void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            this.palette = palette;
        }

        public virtual void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
        {
            if (!init) { return; }
            if (newContainer == null)
            {
                newContainer = rCam.ReturnFContainer("Midground");
            }
            for (int i = startSprite; i < startSprite + numberOfSprites; i++)
            {
                newContainer.AddChild(sLeaser.sprites[i]);
            }
        }

        private bool init;
        public PlayerGraphics pGraphics;
        public int numberOfSprites;
        public int startSprite;
        public RoomPalette palette;
        public PlayerCosmetics.SpritesOverlap spritesOverlap;

        public enum SpritesOverlap
        {
            Behind,
            InFront
        }
    }
}
