using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MonoMod;


public class patch_PlayerGraphics : PlayerGraphics
{
    [MonoModIgnore]
    public Player player;
    [MonoModIgnore]
    public GenericBodyPart legs;
    [MonoModIgnore]
    public Vector2 legsDirection;


    public float fadeOutProgress = 0f;  //from 0 to 1 fade out 
    public float bumpProgress = 0f;     //from 0 to 1 bump
    public float fadeProgress = 0f;     //from 0 to 1 fade light and mark out

    [MonoModIgnore]
    public patch_PlayerGraphics(PhysicalObject ow) : base(ow)
    {
    }

    public extern void orig_ctor(PhysicalObject ow);

    [MonoModConstructor]
    public void ctor(PhysicalObject ow)
    {
        orig_ctor(ow);
        cosmetics = new List<PlayerCosmetics>();
        int num = 14;
        extraSprites = 0;
        for (int l = 0; l < tail.Length; l++)
        {
            tailLength += tail[l].connectionRad;
        }
        num = AddCosmetics(num, new TailRing(this, num, 0));
        num = AddCosmetics(num, new TailRing(this, num, 1));
        num = AddCosmetics(num, new TailRing(this, num, 2));
    }

    private float tailLength;
    public List<PlayerCosmetics> cosmetics;
    public int extraSprites;

    private int AddCosmetics(int spriteIndex, PlayerCosmetics cosmetic)
    {
        cosmetics.Add(cosmetic);
        spriteIndex += cosmetic.numberOfSprites;
        extraSprites += cosmetic.numberOfSprites;
        return spriteIndex;
    }

    public extern void orig_InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

    public new void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        //orig_InitiateSprites(sLeaser, rCam);

        sLeaser.sprites = new FSprite[14 + extraSprites];// Debug.Log(string.Concat("SLEASERLEN: ", sLeaser.sprites.Length));
        sLeaser.sprites[0] = new FSprite("BodyA", true);
        sLeaser.sprites[0].anchorY = 0.7894737f;
        sLeaser.sprites[1] = new FSprite("HipsA", true);
        TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[]
        {
            new TriangleMesh.Triangle(0, 1, 2),
            new TriangleMesh.Triangle(1, 2, 3),
            new TriangleMesh.Triangle(4, 5, 6),
            new TriangleMesh.Triangle(5, 6, 7),
            new TriangleMesh.Triangle(8, 9, 10),
            new TriangleMesh.Triangle(9, 10, 11),
            new TriangleMesh.Triangle(12, 13, 14),
            new TriangleMesh.Triangle(2, 3, 4),
            new TriangleMesh.Triangle(3, 4, 5),
            new TriangleMesh.Triangle(6, 7, 8),
            new TriangleMesh.Triangle(7, 8, 9),
            new TriangleMesh.Triangle(10, 11, 12),
            new TriangleMesh.Triangle(11, 12, 13)
        };
        TriangleMesh triangleMesh = new TriangleMesh("Futile_White", tris, false, false);
        sLeaser.sprites[2] = triangleMesh;
        sLeaser.sprites[3] = new FSprite("HeadA0", true);
        sLeaser.sprites[4] = new FSprite("LegsA0", true);
        sLeaser.sprites[4].anchorY = 0.25f;
        sLeaser.sprites[5] = new FSprite("PlayerArm0", true);
        sLeaser.sprites[5].anchorX = 0.9f;
        sLeaser.sprites[5].scaleY = -1f;
        sLeaser.sprites[6] = new FSprite("PlayerArm0", true);
        sLeaser.sprites[6].anchorX = 0.9f;
        sLeaser.sprites[7] = new FSprite("OnTopOfTerrainHand", true);
        sLeaser.sprites[8] = new FSprite("OnTopOfTerrainHand", true);
        sLeaser.sprites[8].scaleX = -1f;
        sLeaser.sprites[9] = new FSprite("FaceA0", true);
        sLeaser.sprites[11] = new FSprite("pixel", true);
        sLeaser.sprites[11].scale = 5f;
        sLeaser.sprites[10] = new FSprite("Futile_White", true);
        sLeaser.sprites[10].shader = rCam.game.rainWorld.Shaders["FlatLight"];
        sLeaser.sprites[13] = new FSprite("MoonMark", true);
        sLeaser.sprites[13].scale = 1f;
        sLeaser.sprites[12] = new FSprite("Futile_White", true);
        sLeaser.sprites[12].shader = rCam.game.rainWorld.Shaders["FlatLight"];


        for (int l = 0; l < cosmetics.Count; l++)
        {
            cosmetics[l].InitiateSprites(sLeaser, rCam);
        }

        AddToContainer(sLeaser, rCam, null);



    }

    //[MonoMod.MonoModLinkTo("System.Void GraphicsModule::DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)")]
    //public static void base_DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    //{
    //}

    public extern void orig_DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if ((this.player as patch_Player).voidEnergy)
        {
            this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }
        orig_DrawSprites(sLeaser, rCam, timeStacker, camPos);
        float num = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(this.lastBreath, this.breath, timeStacker) * 3.14159274f * 2f);
        Vector2 vector = Vector2.Lerp(this.drawPositions[0, 1], this.drawPositions[0, 0], timeStacker);
        Vector2 vector2 = Vector2.Lerp(this.drawPositions[1, 1], this.drawPositions[1, 0], timeStacker);
        Vector2 vector3 = Vector2.Lerp(this.head.lastPos, this.head.pos, timeStacker);
        if (this.player.aerobicLevel > 0.5f)
        {
            vector += Custom.DirVec(vector2, vector) * Mathf.Lerp(-1f, 1f, num) * Mathf.InverseLerp(0.5f, 1f, this.player.aerobicLevel) * 0.5f;
            vector3 -= Custom.DirVec(vector2, vector) * Mathf.Lerp(-1f, 1f, num) * Mathf.Pow(Mathf.InverseLerp(0.5f, 1f, this.player.aerobicLevel), 1.5f) * 0.75f;
        }
        if (this.player.sleepCurlUp > 0f)
        {
            vector3.y += 1f * this.player.sleepCurlUp;
            vector3.x += Mathf.Sign(vector.x - vector2.x) * 2f * this.player.sleepCurlUp;
        }
        Vector2 vector4 = vector3 + Custom.DirVec(vector2, vector3)*8f;
        sLeaser.sprites[13].x = vector4.x - camPos.x;
        sLeaser.sprites[13].y = vector4.y - camPos.y;
        sLeaser.sprites[13].alpha = Mathf.Lerp(this.lastMoonMarkAlpha, this.moonMarkAlpha, timeStacker);
        sLeaser.sprites[13].rotation = Custom.DirVec(vector2, vector3).GetAngle() + (float)(90f);
        sLeaser.sprites[12].x = vector4.x - camPos.x;
        sLeaser.sprites[12].y = vector4.y - camPos.y;
        sLeaser.sprites[12].alpha = 0.2f * Mathf.Lerp(this.lastMoonMarkAlpha, this.moonMarkAlpha, timeStacker);
        sLeaser.sprites[12].scale = 1f + Mathf.Lerp(this.lastMoonMarkAlpha, this.moonMarkAlpha, timeStacker);
        if ((this.player as patch_Player).past22000 && !(this.player as patch_Player).past25000 && fadeOutProgress == 0f)
        {
            fadeOutProgress = 0.0001f;
        }
        else if (this.fadeOutProgress > 0f && this.fadeOutProgress < 1f)
        {
            fadeOutProgress = Mathf.Clamp(fadeOutProgress + 0.01f, 0f, 1f);
            if (fadeOutProgress == 1f)
            {
                player.Stun(400);
                player.room.PlaySound(SoundID.HUD_Karma_Reinforce_Bump, 0f, 0.3f, 1f);
                player.room.PlaySound(SoundID.HUD_Karma_Reinforce_Contract, 0f, 0.3f, 1f);
                player.room.PlaySound(SoundID.MENU_Karma_Ladder_Increase_Bump, 0f, 0.3f, 1f);
                player.room.AddObject(new ShockWave(player.mainBodyChunk.pos, 600f, 0.5f, 10));
                bumpProgress = 0.0001f;
            }
        }
        else if (this.bumpProgress > 0f && this.bumpProgress < 1f)
        {
            sLeaser.sprites[12].alpha = 1f * Mathf.Lerp(this.lastMoonMarkAlpha, this.moonMarkAlpha, timeStacker);
            sLeaser.sprites[12].scale = 1f + 50f*Mathf.Lerp(this.lastMoonMarkAlpha, this.moonMarkAlpha, timeStacker);
            bumpProgress = Mathf.Clamp(bumpProgress + 0.05f, 0f, 1f);
            sLeaser.sprites[13].element = Futile.atlasManager.GetElementWithName("GiantPistonA");
            if (bumpProgress == 1f)
            {
                player.room.PlaySound(SoundID.MENU_Karma_Ladder_Reinforcement_Dissipate_A, 0f, 0.3f, 1f);
                player.room.PlaySound(SoundID.MENU_Karma_Ladder_Reinforcement_Dissipate_B, 0f, 0.3f, 1f);
                this.moonMarkAlpha = 1f;
                fadeProgress = 0.0001f;
            }
        }
        else if (this.fadeProgress > 0f && this.fadeProgress < 1f)
        {
            sLeaser.sprites[12].alpha = 1f * Mathf.Lerp(this.lastMoonMarkAlpha, this.moonMarkAlpha, timeStacker);
            sLeaser.sprites[12].scale = 1f + 50f * Mathf.Lerp(this.lastMoonMarkAlpha, this.moonMarkAlpha, timeStacker);
            fadeProgress = Mathf.Clamp(fadeProgress + 0.002f, 0f, 1f);
            if (fadeProgress == 1f)
            {
                sLeaser.sprites[13].isVisible = false;
                sLeaser.sprites[12].isVisible = false;
            }
        }
        if (rCam != null)
        {
            for (int j = 0; j < cosmetics.Count; j++)
            {
                cosmetics[j].DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
        }
        if (((player as Creature) as patch_Creature).getRad() > 1)
        {
            sLeaser.sprites[9].element = Futile.atlasManager.GetElementWithName("FaceStunned");
        }
    }

    public static Color SlugcatColor(int i)
    {
        switch (i)
        {
            case 0:
                return Custom.HSL2RGB(0.63055557f, 0.54f, 0.2f);
            case 1:
                return new Color(1f, 1f, 0.4509804f);
            case 2:
                return new Color(1f, 0.4509804f, 0.4509804f);
            case 3:
                return new Color(1f, 1f, 1f);
            default:
                return new Color(1f, 1f, 1f);
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        float voidInEffect = 0f;
        if ((this.player as patch_Player).voidEnergy)
        {
            voidInEffect = (1f - (this.player as patch_Player).maxEnergy)/1.2f;
        }
        Color color = Color.Lerp(PlayerGraphics.SlugcatColor(player.playerState.slugcatCharacter),Color.white, voidInEffect);
        Color color2 = palette.blackColor;
        if (malnourished > 0f)
        {
            float num = (!player.Malnourished) ? Mathf.Max(0f, malnourished - 0.005f) : malnourished;
            color = Color.Lerp(color, Color.gray, 0.4f * num);
            color2 = Color.Lerp(color2, Color.Lerp(Color.white, palette.fogColor, 0.5f), 0.2f * num * num);
        }
        if (player.playerState.slugcatCharacter == 0)
        {
            color2 = Color.Lerp(new Color(1f, 1f, 1f), color, 0.3f);
            color = Color.Lerp(palette.blackColor, Color.Lerp(PlayerGraphics.SlugcatColor(player.playerState.slugcatCharacter), Color.white, voidInEffect), Mathf.Lerp(0.08f, 0.04f, palette.darkness));
        }
        else if (player.room.game.IsStorySession)
        {
            color = Color.Lerp(PlayerGraphics.SlugcatColor(player.playerState.slugcatCharacter), Color.white, voidInEffect);
            color2 = Color.Lerp(new Color(1f, 1f, 1f), color, 0.3f);
            color = Color.Lerp(palette.blackColor, Color.Lerp(PlayerGraphics.SlugcatColor(player.playerState.slugcatCharacter), Color.white, voidInEffect), Mathf.Lerp(0.08f, 0.04f, palette.darkness));
        }
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].color = color;
        }
        color = Color.Lerp(PlayerGraphics.SlugcatColor(player.playerState.slugcatCharacter), Color.white, voidInEffect);
        sLeaser.sprites[11].color = Color.Lerp(color, Color.white, 0.3f);
        sLeaser.sprites[10].color = color;
        sLeaser.sprites[13].color = Color.Lerp(color, Color.white, 0.3f);
        sLeaser.sprites[12].color = color;
        if ((this.player as patch_Player).past22000)
        {
            sLeaser.sprites[13].color = Color.Lerp(color, Color.white, 0.9f);
            sLeaser.sprites[12].color = Color.Lerp(color, Color.white, 0.5f);
        }
        sLeaser.sprites[9].color = color2;

        for (int i = 0; i < cosmetics.Count; i++)
        {
            cosmetics[i].ApplyPalette(sLeaser, rCam, palette);
        }
    }

    public extern void orig_AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner);
    public new void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {

        for (int j = 0; j < cosmetics.Count; j++)
        {
            if (cosmetics[j].spritesOverlap == PlayerCosmetics.SpritesOverlap.Behind)
            {
                cosmetics[j].AddToContainer(sLeaser, rCam, newContatiner);
            }
        }

        orig_AddToContainer(sLeaser, rCam, newContatiner);

        for (int num = 0; num < cosmetics.Count; num++)
        {
            if (cosmetics[num].spritesOverlap == PlayerCosmetics.SpritesOverlap.InFront)
            {
                cosmetics[num].AddToContainer(sLeaser, rCam, newContatiner);
            }
        }
    }

    public extern void orig_Reset();
    public new void Reset()
    {
        orig_Reset();
        for (int l = 0; l < cosmetics.Count; l++)
        {
            cosmetics[l].Reset();
        }
    }

    public extern void orig_Update();
    public new void Update()
    {

        if (!culled)
        {
            for (int j = 0; j < cosmetics.Count; j++)
            {
                cosmetics[j].Update();
            }
        }
        orig_Update();
        this.lastMoonMarkAlpha = this.moonMarkAlpha;
        if (!this.player.dead && this.player.room.game.session is StoryGameSession && (this.player.room.game.session as StoryGameSession).saveState.miscWorldSaveData.moonRevived)
        {
            if ((this.player as patch_Player).pearlConversation.talking)
            {
                moonMarkAlpha = markAlpha;
            }
            else if((this.player as patch_Player).past25000)
            {
                moonMarkAlpha = 0f;
            }
            else if ((this.player as patch_Player).past22000)
            {
                if (this.fadeOutProgress > 0f && this.fadeOutProgress < 1f)
                {
                    moonMarkAlpha = Mathf.Lerp(moonMarkAlpha, 1f - fadeOutProgress, 0.1f);
                }
                else if (this.bumpProgress > 0f && this.bumpProgress < 1f)
                {
                    moonMarkAlpha = Mathf.Lerp(moonMarkAlpha, bumpProgress, 0.1f);
                }
                else if (this.fadeProgress > 0f && this.fadeProgress < 1f)
                {
                    moonMarkAlpha = Mathf.Lerp(moonMarkAlpha, 1f-fadeProgress, 0.1f);
                }
            }
            else
            {
                this.moonMarkAlpha = Custom.LerpAndTick(this.moonMarkAlpha, Mathf.Clamp(Mathf.InverseLerp(50f, 100f, (float)this.player.touchedNoInputCounter) - UnityEngine.Random.value * Mathf.InverseLerp(100f, 50f, (float)this.player.touchedNoInputCounter), 0f, 1f) * this.markBaseAlpha, 0.1f, 0.0333333351f);
            }
            
        }
        else
        {
            this.moonMarkAlpha = 0f;
        }
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

    public PlayerTailData TailPosition(int tailSegment, float timeStacker)
    {
        TailSegment tail = this.tail[tailSegment];
        Vector2 pos = Vector2.Lerp(tail.lastPos, tail.pos, timeStacker);
        float rad = tail.rad;
        Vector2 dir;

        if (tailSegment == 0)
        {
            BodyChunk btm = owner.bodyChunks[1];
            Vector2 btmPos = Vector2.Lerp(btm.lastPos, btm.pos, timeStacker);
            dir = (btmPos - pos).normalized;

        }
        else
        {
            TailSegment lastTail = this.tail[tailSegment - 1];
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

    public float moonMarkAlpha = 0f;

    public float lastMoonMarkAlpha = 0f;

}