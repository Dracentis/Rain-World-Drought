using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.PlacedObjects
{
    public static class PistonPhysics
    {
        public static bool CheckVerticalPistonCollision(BodyChunk chunk, Vector2 pistPos, float W, float H, bool dangerbot, bool dangertop)
        {
            //this.contactPoint.y = 0;
            int contactsetY = 0;
            bool weaponcol = false;
            if (Mathf.Abs(pistPos.x - chunk.pos.x) < (W / 2) + chunk.TerrainRad)
            {
                if (chunk.vel.y > 0f && (pistPos.y - (H / 2)) - chunk.pos.y < chunk.TerrainRad && pistPos.y - chunk.pos.y > (H / 2) - 20)
                {
                    chunk.pos.y = (float)pistPos.y - (H / 2) - chunk.TerrainRad;
                    if (chunk.vel.y > chunk.owner.impactTreshhold)
                    {
                        chunk.owner.TerrainImpact(chunk.index, new IntVector2(1, 0), Mathf.Abs(chunk.vel.y), chunk.lastContactPoint.y < 1);
                    }
                    contactsetY = 1;
                    chunk.vel.y = -Mathf.Abs(chunk.vel.y) * chunk.owner.bounce;
                    if (chunk.vel.y > 0f)
                    {
                        chunk.vel.y = 0f;
                    }
                    chunk.vel.x = chunk.vel.x * Mathf.Clamp(chunk.owner.surfaceFriction * 2f, 0f, 1f);
                    if (dangerbot && chunk.owner is Creature)
                    {
                        (chunk.owner as Creature).Violence(null, new Vector2(0, 0), chunk, null, Creature.DamageType.Blunt, 999f, 999f);
                    }
                    if (chunk.owner is Weapon && (chunk.owner as Weapon).mode == Weapon.Mode.Thrown)
                    {
                        weaponcol = true;
                    }
                }
                else if (chunk.vel.y < 0f && chunk.pos.y - (pistPos.y + (H / 2)) < chunk.TerrainRad && chunk.pos.y - pistPos.y > (H / 2) - 20)
                {
                    chunk.pos.y = (float)pistPos.y + (H / 2) + chunk.TerrainRad;
                    if (Mathf.Abs(chunk.vel.y) > chunk.owner.impactTreshhold)
                    {
                        chunk.owner.TerrainImpact(chunk.index, new IntVector2(-1, 0), Mathf.Abs(chunk.vel.y), chunk.lastContactPoint.y > -1);
                    }
                    contactsetY = -1;
                    chunk.vel.y = Mathf.Abs(chunk.vel.y) * chunk.owner.bounce;
                    if (chunk.vel.y < 0f)
                    {
                        chunk.vel.y = 0f;
                    }
                    chunk.vel.x = chunk.vel.x * Mathf.Clamp(chunk.owner.surfaceFriction * 2f, 0f, 0.5f);
                    if (dangertop && chunk.owner is Creature)
                    {
                        (chunk.owner as Creature).Violence(null, new Vector2(0, 0), chunk, null, Creature.DamageType.Blunt, 999f, 999f);
                    }
                    if (chunk.owner is Weapon && (chunk.owner as Weapon).mode == Weapon.Mode.Thrown)
                    {
                        weaponcol = true;
                    }
                }
            }
            if (contactsetY != 0) { chunk.contactPoint.y = contactsetY; }
            return weaponcol;
        }

        public static bool CheckHorizontalPistonCollision(BodyChunk chunk, Vector2 pistPos, float W, float H)
        {
            int contactsetX = 0;
            bool weaponcol = false;
            if (Mathf.Abs(pistPos.y - chunk.pos.y) < (H / 2) + chunk.TerrainRad)
            {
                if (chunk.vel.x > 0f && (pistPos.x - (W / 2)) - chunk.pos.x < chunk.TerrainRad && pistPos.x - chunk.pos.x > (W / 2) - 20)
                {
                    chunk.pos.x = (float)pistPos.x - (W / 2) - chunk.TerrainRad;
                    if (chunk.vel.x > chunk.owner.impactTreshhold)
                    {
                        chunk.owner.TerrainImpact(chunk.index, new IntVector2(1, 0), Mathf.Abs(chunk.vel.x), chunk.lastContactPoint.x < 1);
                    }
                    contactsetX = 1;
                    chunk.vel.x = -Mathf.Abs(chunk.vel.x) * chunk.owner.bounce;
                    if (Mathf.Abs(chunk.vel.x) < 1f + 9f * (1f - chunk.owner.bounce))
                    {
                        chunk.vel.x = 0f;
                    }
                    chunk.vel.y = chunk.vel.y * Mathf.Clamp(chunk.owner.surfaceFriction * 2f, 0f, 1f);
                    if (chunk.owner is Weapon && (chunk.owner as Weapon).mode == Weapon.Mode.Thrown)
                    {
                        weaponcol = true;
                    }
                }
                else if (chunk.vel.x < 0f && chunk.pos.x - (pistPos.x + (W / 2)) < chunk.TerrainRad && chunk.pos.x - pistPos.x > (W / 2) - 20)
                {
                    chunk.pos.x = (float)pistPos.x + (W / 2) + chunk.TerrainRad;
                    if (Mathf.Abs(chunk.vel.x) > chunk.owner.impactTreshhold)
                    {
                        chunk.owner.TerrainImpact(chunk.index, new IntVector2(-1, 0), Mathf.Abs(chunk.vel.x), chunk.lastContactPoint.x > -1);
                    }
                    contactsetX = -1;
                    chunk.vel.x = Mathf.Abs(chunk.vel.x) * chunk.owner.bounce;
                    if (Mathf.Abs(chunk.vel.x) < 1f + 9f * (1f - chunk.owner.bounce))
                    {
                        chunk.vel.x = 0f;
                    }
                    chunk.vel.y = chunk.vel.y * Mathf.Clamp(chunk.owner.surfaceFriction * 2f, 0f, 1f);
                    if (chunk.owner is Weapon && (chunk.owner as Weapon).mode == Weapon.Mode.Thrown)
                    {
                        weaponcol = true;
                    }
                }
            }
            if (contactsetX != 0) { chunk.contactPoint.x = contactsetX; }
            return weaponcol;
        }
    }
}
