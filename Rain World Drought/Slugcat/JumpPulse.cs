using RWCustom;
using UnityEngine;

// A ring that moves in the opposite direction of the player when double-jumping
class JumpPulse : CosmeticSprite
{
    private const int segs = 30;
    private float rotation;
    private float alpha;
    private float lastAlpha;
    private Color color;

    public JumpPulse(Vector2 pos, Vector2 vel)
    {
        this.vel = vel;
        lastPos = pos;
        this.pos = pos + vel;
        rotation = Custom.VecToDeg(vel);
        alpha = 1f;
        lastAlpha = 1f;

        color = Color.Lerp(PlayerGraphics.SlugcatColor(0) * 3f, Color.white, 0.5f);
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        if (newContatiner == null)
            newContatiner = rCam.ReturnFContainer("ForegroundLights");
        for (int i = 1; i < sLeaser.sprites.Length; i++)
            newContatiner.AddChild(sLeaser.sprites[i]);
        rCam.ReturnFContainer("Water").AddChild(sLeaser.sprites[0]);
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = color;
        sLeaser.sprites[1].color = color;
        sLeaser.sprites[2].color = color;
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 drawPos = Vector2.Lerp(lastPos, pos, timeStacker) - camPos;
        sLeaser.sprites[0].SetPosition(drawPos);
        sLeaser.sprites[0].rotation = rotation;
        sLeaser.sprites[0].alpha = alpha * 0.5f;
        sLeaser.sprites[0].scale = 1.75f - alpha;

        for (int i = 1; i < 3; i++)
        {
            sLeaser.sprites[i].SetPosition(drawPos);
            sLeaser.sprites[i].alpha = alpha * ((i == 2) ? 0.15f : 0.35f);
            sLeaser.sprites[i].scale = ((i == 2) ? 40f : 80f) / 8f;
        }

        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[3];

        // MakeLongMesh scares me
        int verts = segs * 2;
        TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[segs * 2];
        for(int i = 0; i < segs; i++)
        {
            int o = i * 2;
            tris[o + 0] = new TriangleMesh.Triangle(o + 0, o + 1, (o + 2) % verts);
            tris[o + 1] = new TriangleMesh.Triangle(o + 1, (o + 2) % verts, (o + 3) % verts);
        }
        TriangleMesh ring = new TriangleMesh("Futile_White", tris, false);
        sLeaser.sprites[0] = ring;

        // Calculate vertex positions
        const float width = 40f;
        const float height = 5f;
        const float thickness = 1.5f;
        Vector2 dir = new Vector2();
        for(int i = 0; i < segs; i++)
        {
            float rad = i * (Mathf.PI * 2f / segs);
            dir.x = 0.5f * width  * Mathf.Cos(rad);
            dir.y = 0.5f * height * Mathf.Sin(rad);
            ring.vertices[i * 2 + 0].Set(dir.x, dir.y);
            ring.vertices[i * 2 + 1].Set(dir.x + thickness * Mathf.Cos(rad), dir.y + thickness * Mathf.Sin(rad));
        }

        // Glow sprites
        sLeaser.sprites[1] = new FSprite("Futile_White") { shader = rCam.game.rainWorld.Shaders["LightSource"] };
        sLeaser.sprites[2] = new FSprite("Futile_White") { shader = rCam.game.rainWorld.Shaders["FlatLightBehindTerrain"] };

        AddToContainer(sLeaser, rCam, null);
    }

    public override void Update(bool eu)
    {
        lastAlpha = alpha;
        alpha -= 1f / 15f;
        if (alpha == 0f && lastAlpha == 0f)
            Destroy();

        base.Update(eu);
    }
}