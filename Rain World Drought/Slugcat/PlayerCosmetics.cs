using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        sLeaser.sprites[startSprite] = TriangleMesh.MakeLongMesh(1, false, true);
        sLeaser.sprites[startSprite + 1] = TriangleMesh.MakeLongMesh(1, false, true);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

        patch_PlayerGraphics.PlayerTailData tailData = (pGraphics as patch_PlayerGraphics).TailPosition(tailSegment, timeStacker);

        (sLeaser.sprites[startSprite] as TriangleMesh).MoveVertice(0, tailData.pos - camPos);
        (sLeaser.sprites[startSprite] as TriangleMesh).MoveVertice(1, tailData.pos + tailData.dir - camPos);
        (sLeaser.sprites[startSprite] as TriangleMesh).MoveVertice(2, tailData.outerPos - camPos);
        (sLeaser.sprites[startSprite] as TriangleMesh).MoveVertice(3, tailData.outerPos + tailData.dir - camPos);

        (sLeaser.sprites[startSprite+1] as TriangleMesh).MoveVertice(0, tailData.pos - camPos);
        (sLeaser.sprites[startSprite+1] as TriangleMesh).MoveVertice(1, tailData.pos + tailData.dir - camPos);
        (sLeaser.sprites[startSprite+1] as TriangleMesh).MoveVertice(2, tailData.innerPos - camPos);
        (sLeaser.sprites[startSprite+1] as TriangleMesh).MoveVertice(3, tailData.innerPos + tailData.dir - camPos);

        if (rCam != null)
        {
            ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        float voidInEffect = 0f;
        if ((pGraphics.owner as patch_Player).voidEnergy)
        {
            voidInEffect = (1f - (pGraphics.owner as patch_Player).maxEnergy) / 1.2f;
        }
        Color color = Color.Lerp(PlayerGraphics.SlugcatColor((pGraphics.owner as patch_Player).playerState.slugcatCharacter), Color.white, voidInEffect);
        int order = -tailSegment;
        float alpha = 1f;
        if ((pGraphics.owner as patch_Player).energy < (pGraphics.owner as patch_Player).maxEnergy && !(pGraphics.owner as patch_Player).bashing)
        {
            alpha = (pGraphics.owner as patch_Player).energy * Mathf.Abs((float)Math.Sin((double)((float)rCam.room.world.rainCycle.timer % 250f / 20.0375f + order)) / 2f);
        }
        else if ((pGraphics.owner as patch_Player).bashing)
        {
            alpha = 1f;
        }
        else
        {
            alpha = (pGraphics.owner as patch_Player).energy * Mathf.Abs((float)Math.Sin((double)((float)rCam.room.world.rainCycle.timer % 250f / 40.075f)) / 1.2f);
        }
        if ((pGraphics.owner as patch_Player).maxEnergy < 0.1)
        {
            alpha = 0f;
            sLeaser.sprites[startSprite].isVisible= false;
            sLeaser.sprites[startSprite + 1].isVisible = false;
        }
        //pGraphics.owner.room.world.rainCycle.timer;
        sLeaser.sprites[startSprite].alpha = alpha;
        sLeaser.sprites[startSprite + 1].alpha = alpha;

        sLeaser.sprites[startSprite].color = Color.Lerp(color, Color.white, alpha);//palette.blackColor;
        sLeaser.sprites[startSprite + 1].color = Color.Lerp(color, Color.white, alpha);//palette.blackColor;
        base.ApplyPalette(sLeaser, rCam, palette);
    }

    public int tailSegment;

}


public abstract class PlayerCosmetics
{
    public PlayerCosmetics(PlayerGraphics pGraphics, int startSprite)
    {
        this.pGraphics = pGraphics;
        this.startSprite = startSprite;
    }

    public virtual void Update()
    {
    }

    public virtual void Reset()
    {
    }

    public virtual void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
    }

    public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
    }

    public virtual void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
            this.palette = palette;
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {
        if(newContainer == null)
        {
            newContainer = rCam.ReturnFContainer("Midground");
        }
        for (int i = startSprite; i < startSprite + numberOfSprites; i++)
        {
            newContainer.AddChild(sLeaser.sprites[i]);
        }
    }

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


