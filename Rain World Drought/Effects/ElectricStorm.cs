using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;

// Token: 0x02000474 RID: 1140
public class ElectricStorm : UpdatableAndDeletable, INotifyWhenRoomIsReady, IDrawable
{
    // Token: 0x06001D2D RID: 7469 RVA: 0x00199828 File Offset: 0x00197A28
    public ElectricStorm(RoomSettings.RoomEffect effect, Room room)
    {
        this.effect = effect;
        flashes = new ElectricStorm.LightFlash[9];
        for (int i = 0; i < flashes.Length; i++)
        {
            flashes[i] = new ElectricStorm.LightFlash(this, i + 1);
        }
    }

    // Token: 0x17000486 RID: 1158
    // (get) Token: 0x06001D2E RID: 7470 RVA: 0x00199873 File Offset: 0x00197A73
    private RainCycle cycle
    {
        get
        {
            return room.game.world.rainCycle;
        }
    }

    // Token: 0x17000487 RID: 1159
    // (get) Token: 0x06001D2F RID: 7471 RVA: 0x0019988C File Offset: 0x00197A8C
    public float Intensity
    {
        get
        {
            if (room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) > 0f && room.world.rainCycle.brokenAntiGrav.to == 1f)
            {
                return effect.amount * room.world.rainCycle.brokenAntiGrav.progress;
            }
            return 0f;
        }
    }

    // Token: 0x06001D30 RID: 7472 RVA: 0x001998E4 File Offset: 0x00197AE4
    public override void Update(bool eu)
    {
        base.Update(eu);
        if (Intensity == 0f)
        {
            return;
        }
        if (soundLoop == null)
        {
            soundLoop = new DisembodiedDynamicSoundLoop(this);
            soundLoop.sound = SoundID.Death_Lightning_Heavy_Lightning_LOOP;
        }
        else
        {
            soundLoop.Update();
            soundLoop.Volume = Intensity;
        }
        if (soundLoop2 == null)
        {
            soundLoop2 = new DisembodiedDynamicSoundLoop(this);
            soundLoop2.sound = SoundID.Death_Lightning_Early_Sizzle_LOOP;
        }
        else
        {
            soundLoop2.Update();
            soundLoop2.Volume = Mathf.Pow(Intensity, 0.1f) * Mathf.Lerp(0.5f + 0.5f * Mathf.Sin(sin * 3.14159274f * 2f), 0f, Mathf.Pow(Intensity, 8f));
        }
        lastSin = sin;
        sin += Intensity * 0.1f;
        if (closeToWallTiles != null && room.BeingViewed && UnityEngine.Random.value < Mathf.InverseLerp(1000f, 9120f, (float)(room.TileWidth * room.TileHeight)) * Intensity)
        {
            IntVector2 pos = closeToWallTiles[UnityEngine.Random.Range(0, closeToWallTiles.Count)];
            Vector2 pos2 = room.MiddleOfTile(pos) + new Vector2(Mathf.Lerp(-10f, 10f, UnityEngine.Random.value), Mathf.Lerp(-10f, 10f, UnityEngine.Random.value));
            float num = UnityEngine.Random.value * Intensity;
            if (room.ViewedByAnyCamera(pos2, 50f))
            {
                room.AddObject(new ElectricStorm.SparkFlash(pos2, num));
            }
            room.PlaySound(SoundID.Death_Lightning_Spark_Spontaneous, pos2, num, 1f);
        }
        for (int i = 1; i < flashes.Length; i++)
        {
            flashes[i].Update();
        }
        lastColor = color;
        color = Vector3.Lerp(color, getToColor, UnityEngine.Random.value * 0.3f);
        if (UnityEngine.Random.value < 0.333333343f)
        {
            getToColor.x = UnityEngine.Random.value;
        }
        else if (UnityEngine.Random.value < 0.333333343f)
        {
            getToColor.y = UnityEngine.Random.value;
        }
        else if (UnityEngine.Random.value < 0.333333343f)
        {
            getToColor.z = UnityEngine.Random.value;
        }
        if (Intensity > 0.5f && UnityEngine.Random.value < Custom.LerpMap(Intensity, 0.5f, 1f, 0f, 0.5f))
        {
            for (int j = 0; j < room.physicalObjects.Length; j++)
            {
                for (int k = 0; k < room.physicalObjects[j].Count; k++)
                {
                    for (int l = 0; l < room.physicalObjects[j][k].bodyChunks.Length; l++)
                    {
                        if (UnityEngine.Random.value < Custom.LerpMap(Intensity, 0.5f, 1f, 0f, 0.5f) && (room.physicalObjects[j][k].bodyChunks[l].ContactPoint.x != 0 || room.physicalObjects[j][k].bodyChunks[l].ContactPoint.y != 0 || room.GetTile(room.physicalObjects[j][k].bodyChunks[l].pos).AnyBeam))
                        {
                            float num2 = Mathf.Pow(UnityEngine.Random.value, 0.9f) * Mathf.InverseLerp(0.5f, 1f, Intensity);
                            room.AddObject(new ElectricStorm.SparkFlash(room.physicalObjects[j][k].bodyChunks[l].pos + room.physicalObjects[j][k].bodyChunks[l].rad * room.physicalObjects[j][k].bodyChunks[l].ContactPoint.ToVector2(), Mathf.Pow(num2, 0.5f)));
                            Vector2 vector = -(room.physicalObjects[j][k].bodyChunks[l].ContactPoint.ToVector2() + Custom.RNV()).normalized;
                            vector *= 22f * num2 / room.physicalObjects[j][k].bodyChunks[l].mass;
                            room.physicalObjects[j][k].bodyChunks[l].vel += vector;
                            room.physicalObjects[j][k].bodyChunks[l].pos += vector;
                            room.PlaySound(SoundID.Death_Lightning_Spark_Object, room.physicalObjects[j][k].bodyChunks[l].pos, num2, 1f);
                            if (room.physicalObjects[j][k] is Creature)
                            {
                                (room.physicalObjects[j][k] as Creature).Violence(null, null, room.physicalObjects[j][k].bodyChunks[l], null, Creature.DamageType.Electric, num2 * 1.8f, num2 * 40f);
                            }
                        }
                    }
                }
            }
        }
    }

    // Token: 0x06001D31 RID: 7473 RVA: 0x00199F44 File Offset: 0x00198144
    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1 + flashes.Length];
        sLeaser.sprites[0] = new FSprite("Futile_White", true);
        sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["ElectricDeath"];
        sLeaser.sprites[0].scaleX = 87.5f;
        sLeaser.sprites[0].scaleY = 50f;
        for (int i = 1; i < 10; i++)
        {
            sLeaser.sprites[i] = new FSprite("Futile_White", true);
            sLeaser.sprites[i].shader = rCam.room.game.rainWorld.Shaders["LightSource"];
            sLeaser.sprites[i].color = new Color(0f, 0f, 1f);
        }
        AddToContainer(sLeaser, rCam, null);
    }

    // Token: 0x06001D32 RID: 7474 RVA: 0x0019A040 File Offset: 0x00198240
    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        sLeaser.sprites[0].x = rCam.room.game.rainWorld.screenSize.x / 2f;
        sLeaser.sprites[0].y = rCam.room.game.rainWorld.screenSize.y / 2f;
        sLeaser.sprites[0].color = new Color(Mathf.Lerp(lastColor.x, color.x, timeStacker), Mathf.Lerp(lastColor.z, color.z, timeStacker), Mathf.Lerp(lastColor.y, color.y, timeStacker));
        sLeaser.sprites[0].alpha = Mathf.Pow(Intensity, 0.5f) * Mathf.Lerp(0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(lastSin, sin, timeStacker) * 3.14159274f * 2f), 1f, Mathf.Pow(Intensity, 4f));
        sLeaser.sprites[0].alpha = 0f;
        for (int i = 1; i < flashes.Length; i++)
        {
            flashes[i].Draw(sLeaser, rCam, timeStacker, camPos);
        }
        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    // Token: 0x06001D33 RID: 7475 RVA: 0x0000592D File Offset: 0x00003B2D
    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
    }

    // Token: 0x06001D34 RID: 7476 RVA: 0x0019A1BC File Offset: 0x001983BC
    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        if (newContatiner == null)
        {
            newContatiner = rCam.ReturnFContainer("Water");
        }
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }

    // Token: 0x06001D35 RID: 7477 RVA: 0x0000592D File Offset: 0x00003B2D
    public void ShortcutsReady()
    {
    }

    // Token: 0x06001D36 RID: 7478 RVA: 0x0019A208 File Offset: 0x00198408
    public void AIMapReady()
    {
        closeToWallTiles = new List<IntVector2>();
        for (int i = 0; i < room.TileWidth; i++)
        {
            for (int j = 0; j < room.TileHeight; j++)
            {
                if (room.aimap.getAItile(i, j).terrainProximity == 1)
                {
                    closeToWallTiles.Add(new IntVector2(i, j));
                }
            }
        }
    }

    // Token: 0x04001F64 RID: 8036
    private RoomSettings.RoomEffect effect;

    // Token: 0x04001F65 RID: 8037
    public List<IntVector2> closeToWallTiles;

    // Token: 0x04001F66 RID: 8038
    public ElectricStorm.LightFlash[] flashes;

    // Token: 0x04001F67 RID: 8039
    public Vector3 color;

    // Token: 0x04001F68 RID: 8040
    public Vector3 lastColor;

    // Token: 0x04001F69 RID: 8041
    public Vector3 getToColor;

    // Token: 0x04001F6A RID: 8042
    private float sin;

    // Token: 0x04001F6B RID: 8043
    private float lastSin;

    // Token: 0x04001F6C RID: 8044
    public DisembodiedDynamicSoundLoop soundLoop;

    // Token: 0x04001F6D RID: 8045
    public DisembodiedDynamicSoundLoop soundLoop2;

    // Token: 0x02000475 RID: 1141
    public class LightFlash
    {
        // Token: 0x06001D37 RID: 7479 RVA: 0x0019A278 File Offset: 0x00198478
        public LightFlash(ElectricStorm owner, int sprite)
        {
            this.owner = owner;
            this.sprite = sprite;
        }

        // Token: 0x06001D38 RID: 7480 RVA: 0x0019A290 File Offset: 0x00198490
        public void Reset()
        {
            pos.x = owner.room.game.rainWorld.screenSize.x * UnityEngine.Random.value;
            pos.y = owner.room.game.rainWorld.screenSize.y * UnityEngine.Random.value;
            lifeTime = Mathf.Lerp(3f, Mathf.Lerp(34f, 12f, owner.Intensity), UnityEngine.Random.value);
            rad = Mathf.Lerp(40f, 800f, UnityEngine.Random.value * Mathf.Lerp(0.5f, 1f, owner.Intensity));
            life = 1f;
            lastLife = 1f;
        }

        // Token: 0x06001D39 RID: 7481 RVA: 0x0019A380 File Offset: 0x00198580
        public void Update()
        {
            if (life <= 0f && lastLife <= 0f)
            {
                Reset();
            }
            else
            {
                lastLife = life;
                life = Mathf.Max(0f, life - 1f / lifeTime);
            }
        }

        // Token: 0x06001D3A RID: 7482 RVA: 0x0019A3E0 File Offset: 0x001985E0
        public void Draw(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[sprite].x = pos.x;
            sLeaser.sprites[sprite].y = pos.y;
            float num = Mathf.Lerp(lastLife, life, timeStacker);
            sLeaser.sprites[sprite].scale = Mathf.Lerp(0.5f, 1f, Mathf.Sin(num * 3.14159274f)) * Mathf.Lerp(0.8f, 1.2f, UnityEngine.Random.value) * rad / 8f;
            sLeaser.sprites[sprite].alpha = Mathf.Sin(num * 3.14159274f) * Mathf.Lerp(0.6f, 1f, UnityEngine.Random.value) * 0.6f * owner.Intensity;
        }

        // Token: 0x04001F6E RID: 8046
        public ElectricStorm owner;

        // Token: 0x04001F6F RID: 8047
        public int sprite;

        // Token: 0x04001F70 RID: 8048
        public Vector2 pos;

        // Token: 0x04001F71 RID: 8049
        public float lifeTime;

        // Token: 0x04001F72 RID: 8050
        public float rad;

        // Token: 0x04001F73 RID: 8051
        public float life;

        // Token: 0x04001F74 RID: 8052
        public float lastLife;
    }

    // Token: 0x02000476 RID: 1142
    public class SparkFlash : CosmeticSprite
    {
        // Token: 0x06001D3B RID: 7483 RVA: 0x0019A4D0 File Offset: 0x001986D0
        public SparkFlash(Vector2 pos, float size)
        {
            this.pos = pos;
            lastPos = pos;
            this.size = size;
            life = 1f;
            lastLife = 1f;
            lifeTime = Mathf.Lerp(2f, 16f, size * UnityEngine.Random.value);
        }

        // Token: 0x06001D3C RID: 7484 RVA: 0x0019A52C File Offset: 0x0019872C
        public override void Update(bool eu)
        {
            room.AddObject(new Spark(pos, Custom.RNV() * 60f * UnityEngine.Random.value, new Color(0f, 0f, 1f), null, 4, 50));
            if (life <= 0f && lastLife <= 0f)
            {
                Destroy();
            }
            else
            {
                lastLife = life;
                life = Mathf.Max(0f, life - 1f / lifeTime);
            }
        }

        // Token: 0x06001D3D RID: 7485 RVA: 0x0019A5D4 File Offset: 0x001987D4
        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[3];
            sLeaser.sprites[0] = new FSprite("Futile_White", true);
            sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["LightSource"];
            sLeaser.sprites[0].color = new Color(0f, 0f, 1f);
            sLeaser.sprites[1] = new FSprite("Futile_White", true);
            sLeaser.sprites[1].shader = rCam.room.game.rainWorld.Shaders["FlatLight"];
            sLeaser.sprites[1].color = new Color(0f, 0f, 1f);
            sLeaser.sprites[2] = new FSprite("Futile_White", true);
            sLeaser.sprites[2].shader = rCam.room.game.rainWorld.Shaders["FlareBomb"];
            sLeaser.sprites[2].color = new Color(0f, 0f, 1f);
            AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("Water"));
        }

        // Token: 0x06001D3E RID: 7486 RVA: 0x0019A720 File Offset: 0x00198920
        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            float num = Mathf.Lerp(lastLife, life, timeStacker);
            for (int i = 0; i < 3; i++)
            {
                sLeaser.sprites[i].x = pos.x - camPos.x;
                sLeaser.sprites[i].y = pos.y - camPos.y;
            }
            float num2 = Mathf.Lerp(20f, 120f, Mathf.Pow(size, 1.5f));
            sLeaser.sprites[0].scale = Mathf.Pow(Mathf.Sin(num * 3.14159274f), 0.5f) * Mathf.Lerp(0.8f, 1.2f, UnityEngine.Random.value) * num2 * 4f / 8f;
            sLeaser.sprites[0].alpha = Mathf.Pow(Mathf.Sin(num * 3.14159274f), 0.5f) * Mathf.Lerp(0.6f, 1f, UnityEngine.Random.value);
            sLeaser.sprites[1].scale = Mathf.Pow(Mathf.Sin(num * 3.14159274f), 0.5f) * Mathf.Lerp(0.8f, 1.2f, UnityEngine.Random.value) * num2 * 4f / 8f;
            sLeaser.sprites[1].alpha = Mathf.Pow(Mathf.Sin(num * 3.14159274f), 0.5f) * Mathf.Lerp(0.6f, 1f, UnityEngine.Random.value) * 0.2f;
            sLeaser.sprites[2].scale = Mathf.Lerp(0.5f, 1f, Mathf.Sin(num * 3.14159274f)) * Mathf.Lerp(0.8f, 1.2f, UnityEngine.Random.value) * num2 / 8f;
            sLeaser.sprites[2].alpha = Mathf.Sin(num * 3.14159274f) * UnityEngine.Random.value;
        }

        // Token: 0x04001F75 RID: 8053
        public float size;

        // Token: 0x04001F76 RID: 8054
        public float life;

        // Token: 0x04001F77 RID: 8055
        public float lastLife;

        // Token: 0x04001F78 RID: 8056
        public float lifeTime;
    }
}
