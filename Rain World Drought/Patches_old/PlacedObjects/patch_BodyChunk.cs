using MonoMod;
using UnityEngine;
using RWCustom;


class patch_BodyChunk : BodyChunk
{
    [MonoModIgnore]
    private IntVector2 contactPoint;
    [MonoModIgnore]
    private static int MaxRepeats = 100000;
    [MonoModIgnore]
    private float TerrainRad;
    [MonoModIgnore]
    private bool SolidFloor(int X, int Y) { return true; }

    public int contactsetX = 0;

    public int contactsetY = 0;

    public extern void orig_Update();

    public void Update()
    {
        orig_Update();
        if (contactsetX != 0)
        {
            contactPoint.x = contactsetX;
            contactsetX = 0;
        }
        if (contactsetY != 0)
        {
            contactPoint.x = contactsetY;
            contactsetY = 0;
        }
    }

    [MonoModIgnore]
    patch_BodyChunk(PhysicalObject owner, int index, Vector2 pos, float rad, float mass) : base(owner, index, pos, rad, mass) { }

    public bool CheckVerticalPistonCollision(Vector2 pistPos, float W, float H, bool dangerbot, bool dangertop)
    {
        //this.contactPoint.y = 0;
        contactsetY = 0;
        bool weaponcol = false;
        if (Mathf.Abs(pistPos.x - pos.x) < (W / 2) + TerrainRad)
        {
            if (vel.y > 0f && (pistPos.y - (H / 2)) - pos.y < TerrainRad && pistPos.y - pos.y > (H / 2) - 20)
            {
                pos.y = (float)pistPos.y - (H / 2) - TerrainRad;
                if (vel.y > owner.impactTreshhold)
                {
                    owner.TerrainImpact(index, new IntVector2(1, 0), Mathf.Abs(vel.y), lastContactPoint.y < 1);
                }
                contactsetY = 1;
                vel.y = -Mathf.Abs(vel.y) * owner.bounce;
                if (vel.y > 0f)
                {
                    vel.y = 0f;
                }
                vel.x = vel.x * Mathf.Clamp(owner.surfaceFriction * 2f, 0f, 1f);
                if (dangerbot && owner is Creature)
                {
                    (owner as Creature).Violence(null, new Vector2(0, 0), this, null, Creature.DamageType.Blunt, 999f, 999f);
                }
                if (owner is Weapon && (owner as Weapon).mode == Weapon.Mode.Thrown)
                {
                    weaponcol = true;
                }
            }
            else if (vel.y < 0f && pos.y - (pistPos.y + (H / 2)) < TerrainRad && pos.y - pistPos.y > (H / 2) - 20)
            {
                pos.y = (float)pistPos.y + (H / 2) + TerrainRad;
                if (Mathf.Abs(vel.y) > owner.impactTreshhold)
                {
                    owner.TerrainImpact(index, new IntVector2(-1, 0), Mathf.Abs(vel.y), lastContactPoint.y > -1);
                }
                contactsetY = -1;
                vel.y = Mathf.Abs(vel.y) * owner.bounce;
                if (vel.y < 0f)
                {
                    vel.y = 0f;
                }
                vel.x = vel.x * Mathf.Clamp(owner.surfaceFriction * 2f, 0f, 0.5f);
                if (dangertop && owner is Creature)
                {
                    (owner as Creature).Violence(null, new Vector2(0, 0), this, null, Creature.DamageType.Blunt, 999f, 999f);
                }
                if (owner is Weapon && (owner as Weapon).mode == Weapon.Mode.Thrown)
                {
                    weaponcol = true;
                }
            }
        }
        return weaponcol;
    }


    public bool CheckHorizontalPistonCollision(Vector2 pistPos, float W, float H)
    {
        contactsetX = 0;
        bool weaponcol = false;
        if (Mathf.Abs(pistPos.y-pos.y) < (H / 2)+TerrainRad)
        {
            if (vel.x > 0f && (pistPos.x - (W / 2)) - pos.x < TerrainRad && pistPos.x - pos.x > (W / 2) - 20)
            {
                pos.x = (float)pistPos.x-(W/2) - TerrainRad;
                if (vel.x > owner.impactTreshhold)
                {
                    owner.TerrainImpact(index, new IntVector2(1, 0), Mathf.Abs(vel.x), lastContactPoint.x < 1);
                }
                contactsetX = 1;
                vel.x = -Mathf.Abs(vel.x) * owner.bounce;
                if (Mathf.Abs(vel.x) < 1f + 9f * (1f - owner.bounce))
                {
                    vel.x = 0f;
                }
                vel.y = vel.y * Mathf.Clamp(owner.surfaceFriction * 2f, 0f, 1f); 
                if (owner is Weapon && (owner as Weapon).mode == Weapon.Mode.Thrown)
                {
                    weaponcol = true;
                }
            }
            else if (vel.x < 0f && pos.x - (pistPos.x + (W / 2)) < TerrainRad && pos.x - pistPos.x > (W / 2) - 20)
            {
                pos.x = (float)pistPos.x + (W / 2) + TerrainRad;
                if (Mathf.Abs(vel.x) > owner.impactTreshhold)
                {
                    owner.TerrainImpact(index, new IntVector2(-1, 0), Mathf.Abs(vel.x), lastContactPoint.x > -1);
                }
                contactsetX = -1;
                vel.x = Mathf.Abs(vel.x) * owner.bounce;
                if (Mathf.Abs(vel.x) < 1f + 9f * (1f - owner.bounce))
                {
                    vel.x = 0f;
                }
                vel.y = vel.y * Mathf.Clamp(owner.surfaceFriction * 2f, 0f, 1f);
                if (owner is Weapon && (owner as Weapon).mode == Weapon.Mode.Thrown)
                {
                    weaponcol = true;
                }
            }
        }
        return weaponcol;
    }

}
