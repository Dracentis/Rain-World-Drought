using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RWCustom;
using MonoMod;
using Rain_World_Drought.Enums;

namespace Rain_World_Drought.Creatures
{
    public class LightWorm : Creature
    {
        public LightWorm(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
        {
            bodyChunks = new BodyChunk[2];
            bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 8f, 0.05f);
            bodyChunks[1] = new BodyChunk(this, 0, new Vector2(0f, 0f), 8f, 0.05f);
            bodyChunks[0].rotationChunk = bodyChunks[1];
            bodyChunks[1].collideWithTerrain = false;
            bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];

            tentacle = new Tentacle(this, bodyChunks[1], 400f * bodySize);
            tentacle.tProps = new Tentacle.TentacleProps(false, false, true, 0.5f, 0f, 1.4f, 0f, 0f, 1.2f, 10f, 0.25f, 5f, 15, 60, 12, 0);
            tentacle.tChunks = new Tentacle.TentacleChunk[(int)(15f * Mathf.Lerp(bodySize, 1f, 0.5f))];
            for (int i = 0; i < tentacle.tChunks.Length; i++)
            {
                tentacle.tChunks[i] = new Tentacle.TentacleChunk(tentacle, i, (float)(i + 1) / (float)tentacle.tChunks.Length, 2f * Mathf.Lerp(bodySize, 1f, 0.5f));
            }
            tentacle.stretchAndSqueeze = 0.1f;

            GoThroughFloors = true;
            airFriction = 0.99f;
            gravity = 0.9f;
            bounce = 0.1f;
            surfaceFriction = 0.47f;
            collisionLayer = 1;
            waterFriction = 0.92f;
            buoyancy = 0.95f;
            extended = 1f;
            retractSpeed = 1f;
            lastExtended = true;
        }

        public LightWormAI AI
        {
            get
            {
                return abstractCreature.abstractAI.RealAI as LightWormAI;
            }
        }

        public float bodySize
        {
            get
            {
                return 4f;//this.State.bodySize;
            }
        }

        public new GarbageWormState State
        {
            get
            {
                return abstractCreature.state as GarbageWormState;
            }
        }

        public override void InitiateGraphicsModule()
        {
            if (graphicsModule == null)
            {
                graphicsModule = new LightWormGraphics(this);
            }
        }

        public override void NewRoom(Room room)
        {
            tentacle.NewRoom(room);
            base.NewRoom(room);
            NewHole(false);
        }

        public int hole;
        public Vector2 rootPos;
        public Tentacle tentacle;
        public Vector2 lookPoint;
        private float retractSpeed;
        public float extended;
        public Vector2? chargePos;
        public ChunkDynamicSoundLoop sound;

        private bool lastExtended;

        public void NewHole(bool burrowed)
        {
            /*
            if (this.room.garbageHoles == null)
            {
                this.AI.comeBackOutCounter = 0;
                this.retractSpeed = -0.0333333351f;
                return;
            }
            List<int> list = new List<int>();
            for (int i = 0; i < this.room.garbageHoles.Length; i++)
            {
                list.Add(i);
            }
            for (int j = 0; j < this.room.abstractRoom.creatures.Count; j++)
            {
                if (this.room.abstractRoom.creatures[j] != base.abstractCreature && this.room.abstractRoom.creatures[j].realizedCreature != null && this.room.abstractRoom.creatures[j].realizedCreature is GarbageWorm)
                {
                    list.Remove((this.room.abstractRoom.creatures[j].realizedCreature as GarbageWorm).hole);
                }
            }
            if (list.Count == 0)
            {
                this.AI.comeBackOutCounter = 0;
                this.retractSpeed = -0.0333333351f;
                return;
            }
            this.hole = list[Random.Range(0, list.Count)];
            base.abstractCreature.pos.Tile = this.room.garbageHoles[this.hole] + new IntVector2(0, 1);
            this.rootPos = this.room.MiddleOfTile(base.abstractCreature.pos.Tile) + new Vector2(0f, -10f + base.bodyChunks[1].rad);
            this.tentacle.Reset(this.rootPos);
            if (burrowed)
            {
                base.bodyChunks[0].HardSetPosition(this.rootPos);
            }
            else
            {
                IntVector2 tile = base.abstractCreature.pos.Tile;
                Tentacle tentacle = this.tentacle;
                List<IntVector2> list2 = new List<IntVector2>();
                list2.Add(base.abstractCreature.pos.Tile);
                tentacle.segments = list2;
                int num = base.abstractCreature.pos.Tile.y + 1;
                while ((float)num < (float)base.abstractCreature.pos.Tile.y + this.tentacle.idealLength / 20f)
                {
                    if (this.room.GetTile(tile).Solid)
                    {
                        break;
                    }
                    this.tentacle.segments.Add(tile);
                    tile.y = num;
                    num++;
                }
                for (int k = 0; k < this.tentacle.tChunks.Length; k++)
                {
                    this.tentacle.tChunks[k].pos = this.room.MiddleOfTile(this.tentacle.segments[this.tentacle.tChunks[k].currentSegment]);
                    this.tentacle.tChunks[k].lastPos = this.tentacle.tChunks[k].pos;
                }
                base.bodyChunks[0].HardSetPosition(this.room.MiddleOfTile(tile));
                this.tentacle.retractFac = 0f;
                this.extended = 1f;
            }
            base.bodyChunks[1].HardSetPosition(this.rootPos);
            this.AI.MapFloor(this.room);
            */
        }
    }

    public class AbstractLightWorm : AbstractCreature
    {
        public AbstractLightWorm(World world, CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID ID) : base(world, creatureTemplate, realizedCreature, pos, ID)
        {
            if (this.creatureTemplate.type != EnumExt_Drought.LightWorm)
            {
                this.creatureTemplate = StaticWorld.creatureTemplates[(int)EnumExt_Drought.LightWorm];
            }
            this.creatureTemplate.name = "LightWorm";
        }

        public override void Realize()
        {
            if (realizedCreature == null)
            {
                realizedCreature = new LightWorm(this, world);
                abstractAI.RealAI = new LightWormAI(this, world);
            }
        }
    }
}
