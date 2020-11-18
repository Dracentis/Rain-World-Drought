using RWCustom;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Rain_World_Drought.PlacedObjects
{
    public static class PistonPhysics
    {
        public static List<Piston> roomPistons = new List<Piston>();
        public static bool doPistonCollision = false;

        public static void Patch()
        {
            On.PhysicalObject.IsTileSolid += IsTileSolidHK;
            On.Room.Update += UpdateHK;
            On.BodyChunk.CheckVerticalCollision += CheckVerticalCollisionHK;
            On.BodyChunk.CheckHorizontalCollision += CheckHorizontalCollisionHK;
        }

        #region Hooks
        // The player's movement depends on this function for things like wall sliding
        // Having it consider pistons as solid tiles helps make them seem consistent
        private static bool IsTileSolidHK(On.PhysicalObject.orig_IsTileSolid orig, PhysicalObject self, int bChunk, int relativeX, int relativeY)
        {
            if (orig(self, bChunk, relativeX, relativeY)) return true;
            if (doPistonCollision)
            {
                Vector2 testPoint = self.bodyChunks[bChunk].pos;
                testPoint.x += 20f * relativeX;
                testPoint.y += 20f * relativeY;
                for (int i = roomPistons.Count - 1; i >= 0; i--)
                {
                    Piston p = roomPistons[i];
                    if (Mathf.Abs(testPoint.y - p.placedPos.y) > p.collisionSize.y) continue;
                    if (Mathf.Abs(testPoint.x - p.placedPos.x) > p.collisionSize.x) continue;
                    return true;
                }
            }
            return false;
        }

        // Generate a list of pistons for objects to collide against each frame
        private static void UpdateHK(On.Room.orig_Update orig, Room self)
        {
            try
            {
                // Get a list of all pistons in the room
                for (int i = self.physicalObjects[1].Count - 1; i >= 0; i--)
                    if (self.physicalObjects[1][i] is Piston p) roomPistons.Add(p);
                doPistonCollision = roomPistons.Count > 0;
                orig(self);
            }
            finally
            {
                doPistonCollision = false;
                roomPistons.Clear();
            }
        }

        // Allow vertical chunk movement to collide with pistons
        private static void CheckVerticalCollisionHK(On.BodyChunk.orig_CheckVerticalCollision orig, BodyChunk self)
        {
            orig(self);
            if (!doPistonCollision || !self.collideWithTerrain) return;

            for (int i = roomPistons.Count - 1; i >= 0; i--)
            {
                Vector2 oldPos = self.pos;
                self.pos.y = CastChunk(new Vector2(self.lastPos.x, self.lastPos.y + Mathf.Sign(self.lastPos.y - self.pos.y)), new Vector2(self.lastPos.x, self.pos.y), self.TerrainRad, roomPistons[i]).y;
                if (Mathf.Abs(oldPos.y - self.pos.y) > 0.01f)
                {
                    int dir = Math.Sign(oldPos.y - self.pos.y);
                    if (Mathf.Abs(self.vel.y) >= self.owner.impactTreshhold) self.owner.TerrainImpact(self.index, new IntVector2(0, dir), Mathf.Abs(self.vel.y), self.contactPoint.y != dir);
                    if (self.contactPoint.y == 0) self.contactPoint.y = dir;
                    self.vel.y = Mathf.Abs(self.vel.y) * Mathf.Sign(self.pos.y - roomPistons[i].placedPos.y) * self.owner.bounce;
                    self.vel.x *= Mathf.Clamp01(self.owner.surfaceFriction * 2f);
                    if (self.index == 1 && self.owner is Player ply)
                    {
                        ply.feetStuckPos = ply.bodyChunks[1].pos;
                    }
                }
            }
        }

        // Allow horizontal chunk movement to collide with pistons
        private static void CheckHorizontalCollisionHK(On.BodyChunk.orig_CheckHorizontalCollision orig, BodyChunk self)
        {
            orig(self);
            if (!doPistonCollision || !self.collideWithTerrain) return;

            for (int i = roomPistons.Count - 1; i >= 0; i--)
            {
                Vector2 oldPos = self.pos;
                self.pos = CastChunk(new Vector2(self.lastPos.x + Mathf.Sign(self.lastPos.x - self.pos.x), self.pos.y), self.pos, self.TerrainRad, roomPistons[i]);
                if (Mathf.Abs(oldPos.x - self.pos.x) > 0.01f)
                {
                    int dir = Math.Sign(oldPos.x - self.pos.x);
                    if (Mathf.Abs(self.vel.x) >= self.owner.impactTreshhold) self.owner.TerrainImpact(self.index, new IntVector2(dir, 0), Mathf.Abs(self.vel.x), self.contactPoint.x != dir);
                    if (self.contactPoint.x == 0) self.contactPoint.x = dir;
                    self.vel.x = Mathf.Abs(self.vel.x) * Mathf.Sign(self.pos.x - roomPistons[i].placedPos.x) * self.owner.bounce;
                    self.vel.y *= Mathf.Clamp01(self.owner.surfaceFriction * 2f);
                }
            }
        }
        #endregion Hooks

        /// <summary>
        /// Determines where a chunk would contact a piston.
        /// Can only be used when the chunk has moved on a single axis.
        /// </summary>
        public static Vector2 CastChunk(Vector2 chunkStart, Vector2 chunkEnd, float rad, Piston piston)
        {
            const float shellDist = 0.5f;

            bool vert = chunkStart.x == chunkEnd.x;
            if (vert && chunkStart.y == chunkEnd.y) return chunkEnd;
            Vector2 size = piston.collisionSize;
            chunkStart -= piston.placedPos;
            chunkEnd -= piston.placedPos;

            // Mirror when vertical so the same math can be used for both directions
            if (vert)
            {
                size.Set(size.y, size.x);
                chunkStart.Set(chunkStart.y, chunkStart.x);
                chunkEnd.Set(chunkEnd.y, chunkEnd.x);
            }

            float initialEndX = chunkEnd.x;

            float distY = Mathf.Abs(chunkEnd.y);
            if (distY > size.y + rad)
            {
                // The chunk is above or below the piston
                // No change must be made
            }
            else if (distY > size.y)
            {
                // The chunk may hit the corner of the piston
                // Calculate the effective radius of the object at this Y position
                float temp = distY - size.y;
                rad = Mathf.Sqrt(rad * rad - temp * temp);
                if (Mathf.Sign(chunkStart.x) != Mathf.Sign(chunkEnd.x) || Mathf.Abs(chunkEnd.x) < size.x + rad && Mathf.Abs(chunkStart.x) >= size.x + rad)
                    chunkEnd.x = Mathf.Sign(chunkStart.x) * (size.x + rad + shellDist);
            }
            else
            {
                // The chunk may hit the side of the piston
                if (Mathf.Sign(chunkStart.x) != Mathf.Sign(chunkEnd.x) || Mathf.Abs(chunkEnd.x) < size.x + rad && Mathf.Abs(chunkStart.x) >= size.x + rad)
                    chunkEnd.x = Mathf.Sign(chunkStart.x) * (size.x + rad + shellDist);
            }

            // Cancel if the movement would result in a noticeable teleportation
            if (chunkEnd.x < Mathf.Min(chunkStart.x, initialEndX) - 1f || chunkEnd.x > Mathf.Max(chunkStart.x, initialEndX) + 1f)
                chunkEnd.x = initialEndX;

            // Reverse the mirror operation
            if (vert) chunkEnd.Set(chunkEnd.y, chunkEnd.x);
            return chunkEnd + piston.placedPos;
        }

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
