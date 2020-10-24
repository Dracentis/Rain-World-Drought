using System;
using System.Collections.Generic;
using Rain_World_Drought.Slugcat;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    public class WalkerBeastGraphics : GraphicsModule, HasDanglers, ILookingAtCreatures
    {
        public WalkerBeastGraphics(PhysicalObject ow) : base(ow, false)
        {
            looker = new CreatureLooker(this, WalkerBeast.AI.tracker, WalkerBeast, 0.1f, 150);
            lookPoint = new Vector2[2];
            cullRange = 1400f;
            int seed = UnityEngine.Random.seed;
            UnityEngine.Random.seed = WalkerBeast.abstractCreature.ID.RandomSeed;
            neckFatness = Mathf.Lerp(2f, 2.3f, UnityEngine.Random.value);
            beakFatness = (UnityEngine.Random.value / 2f) + 0.5f;
            antlers = new WalkerBeastGraphics.Antlers(WalkerBeast.antlers.rad, 1f);
            chunksRotat = new float[4];
            for (int i = 0; i < 4; i++)
            {
                chunksRotat[i] = UnityEngine.Random.value * 360f;
            }
            bodyDanglers = 0;
            hornDanglers = 0;
            danglers = new Dangler[bodyDanglers + hornDanglers];
            danglerVals = new Dangler.DanglerProps();
            bodyDanglerPositions = new int[bodyDanglers, 2];
            bodyDanglerOrientations = new float[bodyDanglers, 5];
            hornDanglerPositions = new int[hornDanglers, 3];
            internalContainerObjects = new List<GraphicsModule.ObjectHeldInInternalContainer>();
            for (int j = 0; j < bodyDanglers + hornDanglers; j++)
            {
                danglers[j] = new Dangler(this, j, UnityEngine.Random.Range(4, UnityEngine.Random.Range(6, UnityEngine.Random.Range(8, 16))), 5f, 0f);
                float num = Mathf.Lerp(0.5f, 1f, UnityEngine.Random.value) * ((j >= bodyDanglers) ? 0.5f : 1f);
                float num2 = (j >= bodyDanglers) ? Mathf.Lerp(0.2f, 0.7f, UnityEngine.Random.value) : 1f;
                for (int k = 0; k < danglers[j].segments.Length; k++)
                {
                    float num3 = (float)k / (float)(danglers[j].segments.Length - 1);
                    danglers[j].segments[k].rad = Mathf.Lerp(Mathf.Lerp((j >= bodyDanglers) ? 6f : 11f, 2.5f, Mathf.Pow(num3, 0.7f)), 2f + Mathf.Sin(Mathf.Pow(num3, 2.5f) * 3.14159274f) * 6f, num3) * num;
                    danglers[j].segments[k].conRad = Mathf.Lerp(30f, 5f, num3) * num2 * 0.5f;
                }
                if (j < bodyDanglers)
                {
                    bodyDanglerPositions[j, 0] = UnityEngine.Random.Range(0, 5);
                    bodyDanglerPositions[j, 1] = UnityEngine.Random.Range(1, 5);
                    bodyDanglerOrientations[j, 0] = Mathf.Lerp(0f, 360f, UnityEngine.Random.value);
                    bodyDanglerOrientations[j, 1] = WalkerBeast.bodyChunks[bodyDanglerPositions[j, 0]].rad * UnityEngine.Random.value * 0.7f;
                    bodyDanglerOrientations[j, 2] = Mathf.Lerp(0f, 360f, UnityEngine.Random.value);
                    bodyDanglerOrientations[j, 3] = WalkerBeast.bodyChunks[bodyDanglerPositions[j, 1]].rad * UnityEngine.Random.value * 0.7f;
                    bodyDanglerOrientations[j, 4] = UnityEngine.Random.value;
                }
                else
                {
                    //this.hornDanglerPositions[j - this.bodyDanglers, 0] = UnityEngine.Random.Range(0, this.antlers.parts.Length);
                    //this.hornDanglerPositions[j - this.bodyDanglers, 1] = this.antlers.parts[this.hornDanglerPositions[j - this.bodyDanglers, 0]].positions.Length - 1 - UnityEngine.Random.Range(1, UnityEngine.Random.Range(1, this.antlers.parts[this.hornDanglerPositions[j - this.bodyDanglers, 0]].positions.Length));
                    hornDanglerPositions[j - bodyDanglers, 2] = ((UnityEngine.Random.value >= 0.5f) ? 1 : -1);
                }
            }
            beak = new WalkerBeastGraphics.BeakGraphic[2];
            for (int j = 1; j < 3; j++)
            {
                beak[j - 1] = new WalkerBeastGraphics.BeakGraphic(this, j - 1, JawSprite(j));
            }
            UnityEngine.Random.seed = seed;
            Reset();
        }

        static WalkerBeastGraphics()
        {
            // Note: this type is marked as 'beforefieldinit'.
            Vector2[,,] array = new Vector2[2, 2, 2];
            array[0, 0, 0] = new Vector2(19f, 44f);
            array[0, 0, 1] = new Vector2(13f, 62f);
            array[0, 1, 0] = new Vector2(32f, 69f);
            array[0, 1, 1] = new Vector2(13f, 56f);
            array[1, 0, 0] = new Vector2(8f, 5f);
            array[1, 0, 1] = new Vector2(6f, 4f);
            array[1, 1, 0] = new Vector2(3.5f, 3f);
            array[1, 1, 1] = new Vector2(6f, 3f);
            legGraphicAnchors = array;
        }

        private WalkerBeast WalkerBeast
        {
            get
            {
                return owner as WalkerBeast;
            }
        }

        private int AntlerSprite
        {
            get
            {
                return 0;
            }
        }

        private int FirstAntlerSprite
        {
            get
            {
                return 1;
            }
        }

        private int LastAntlerSprite
        {
            get
            {
                return FirstAntlerSprite + antlers.SpritesClaimed - 1;
            }
        }

        private int BodySprite(int chunk)
        {
            return LastAntlerSprite + 1 + chunk;
        }

        private int FirstDanglerSprite
        {
            get
            {
                return LastAntlerSprite + 1 + 5;
            }
        }

        private int LastDanglerSprite
        {
            get
            {
                return FirstDanglerSprite + danglers.Length - 1;
            }
        }

        private int JawSprite(int a)
        {
            return LastDanglerSprite + 2 + 12 + a;
        }

        private int LastJawSprite()
        {
            return LastDanglerSprite + 2 + 12 + 2;
        }

        private int NeckSprite
        {
            get
            {
                return LastDanglerSprite + 1 + 12;
            }
        }

        private int LegSprite(int leg, int part)
        {
            return LastDanglerSprite + 1 + leg * 3 + part;
        }

        private int TotalSprites
        {
            get
            {
                return LastDanglerSprite + 1 + 12 + 3 + 1;
            }
        }

        private float LegPartLength(int pos, int part, bool includeExtension)
        {
            if (pos == 0)
            {
                return ((part != 0) ? 55f : 35f) * ((!includeExtension) ? 1f : (WalkerBeast.preferredHeight / 15f));
            }
            return ((part != 0) ? 45f : 60f) * ((!includeExtension) ? 1f : (WalkerBeast.preferredHeight / 15f));
        }

        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < danglers.Length; i++)
            {
                danglers[i].Reset();
            }
        }

        public override void Update()
        {
            lastHeadFlip = headFlip;
            if (Custom.DistanceToLine(WalkerBeast.Head.pos, WalkerBeast.bodyChunks[0].pos, WalkerBeast.bodyChunks[1].pos) < 0f)
            {
                headFlip = Mathf.Min(1f, headFlip + 0.166666672f);
            }
            else
            {
                headFlip = Mathf.Max(-1f, headFlip - 0.166666672f);
            }
            base.Update();
            if (culled)
            {
                return;
            }
            looker.Update();
            lookPoint[1] = lookPoint[0];
            lookPoint[0] *= 0f;
            if (looker.lookCreature != null)
            {
                if (looker.lookCreature.VisualContact && looker.lookCreature.representedCreature.realizedCreature != null)
                {
                    lookPoint[0] = looker.lookCreature.representedCreature.realizedCreature.DangerPos;
                }
                else
                {
                    lookPoint[0] = WalkerBeast.room.MiddleOfTile(looker.lookCreature.BestGuessForPosition());
                }
                lookPoint[0] = Vector2.ClampMagnitude(lookPoint[0] - WalkerBeast.mainBodyChunk.pos, 200f) / 200f;
            }
            blink--;
            if (blink < -UnityEngine.Random.Range(5, 14))
            {
                blink = UnityEngine.Random.Range(15, 305);
            }
            if (!WalkerBeast.Consious)
            {
                blink = -5;
            }
            for (int i = 0; i < danglers.Length; i++)
            {
                danglers[i].Update();
                for (int j = 0; j < danglers[i].segments.Length; j++)
                {
                    WalkerBeastTentacle WalkerBeastTentacle = WalkerBeast.legs[UnityEngine.Random.Range(0, 4)];
                    if (Custom.DistLess(danglers[i].segments[j].pos, WalkerBeastTentacle.tChunks[1].pos, 10f))
                    {
                        danglers[i].segments[j].vel += WalkerBeastTentacle.tChunks[1].vel;
                    }
                }
            }
            for (int k = internalContainerObjects.Count - 1; k >= 0; k--)
            {
                if (!(internalContainerObjects[k].obj is PlayerGraphics) || ((internalContainerObjects[k].obj as PlayerGraphics).owner as Player).playerInAntlers == null || WandererSupplement.GetSub((internalContainerObjects[k].obj as PlayerGraphics).player).playerInAnt.walkerBeast != WalkerBeast)
                {
                    ReleaseSpecificInternallyContainedObjectSprites(k);
                }
            }
            float num = Mathf.Clamp(-1f + 2f * Mathf.InverseLerp(-20f, 20f, WalkerBeast.mainBodyChunk.pos.x - WalkerBeast.bodyChunks[1].pos.x) + WalkerBeast.flipDir, -1f, 1f);
            lastFlip = flip;
            if (num < 0f)
            {
                flip = Mathf.Max(-1f, flip - 1f / Custom.LerpMap(Mathf.Abs(flip - num), 0f, 1f, 120f, 20f));
            }
            else
            {
                flip = Mathf.Min(1f, flip + 1f / Custom.LerpMap(Mathf.Abs(flip - num), 0f, 1f, 120f, 20f));
            }
            antlerRandomMovementVel *= 0.8f;
            if (WalkerBeast.Consious)
            {
                antlerRandomMovementVel += Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) * WalkerBeast.mainBodyChunk.vel.magnitude * 0.01f;
            }
            antlerRandomMovementVel -= antlerRandomMovement * 0.0001f;
            lastAntlerRandomMovement = antlerRandomMovement;
            antlerRandomMovement += antlerRandomMovementVel;
            if (antlerRandomMovement < -1f)
            {
                antlerRandomMovement = -1f;
            }
            else if (antlerRandomMovement > 1f)
            {
                antlerRandomMovement = 1f;
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[TotalSprites];
            sLeaser.sprites[JawSprite(0)] = new FSprite("Circle20", true);
            sLeaser.sprites[JawSprite(0)].scaleX = 0.6f;
            sLeaser.sprites[JawSprite(0)].scaleY = 0.6f;
            for (int j = 0; j < 2; j++)
            {
                beak[j].InitiateSprites(sLeaser, rCam);
            }
            sLeaser.sprites[NeckSprite] = TriangleMesh.MakeLongMesh(WalkerBeast.neck.tChunks.Length, false, false);
            (sLeaser.sprites[NeckSprite] as TriangleMesh).alpha = 1f;
            (sLeaser.sprites[NeckSprite] as TriangleMesh).color = new Color(0.5f, 0f, 0f);
            sLeaser.sprites[AntlerSprite] = new FSprite("Circle20", true);
            sLeaser.sprites[AntlerSprite].scale = 0f;
            sLeaser.sprites[AntlerSprite].color = new Color(0.5f, 0f, 0f);
            for (int i = 0; i < 5; i++)
            {
                sLeaser.sprites[BodySprite(i)] = new FSprite("Futile_White", true);
                sLeaser.sprites[BodySprite(i)].scaleX = owner.bodyChunks[i].rad / 8f * 1.05f;
                sLeaser.sprites[BodySprite(i)].scaleY = owner.bodyChunks[i].rad / 8f * 1.3f;
                sLeaser.sprites[BodySprite(i)].shader = rCam.room.game.rainWorld.Shaders["JaggedCircle"];
                sLeaser.sprites[BodySprite(i)].alpha = 0.5f;
            }
            for (int j = 0; j < 4; j++)
            {
                sLeaser.sprites[LegSprite(j, 0)] = new FSprite("deerLeg" + ((j >= 2) ? "B" : "A") + "1", true);
                sLeaser.sprites[LegSprite(j, 0)].anchorY = legGraphicAnchors[1, (j >= 2) ? 1 : 0, 0].y / legGraphicAnchors[0, (j >= 2) ? 1 : 0, 0].y;
                sLeaser.sprites[LegSprite(j, 0)].anchorX = legGraphicAnchors[1, (j >= 2) ? 1 : 0, 0].x / legGraphicAnchors[0, (j >= 2) ? 1 : 0, 0].x;
                sLeaser.sprites[LegSprite(j, 1)] = new FSprite("deerLeg" + ((j >= 2) ? "B" : "A") + "2", true);
                sLeaser.sprites[LegSprite(j, 1)].anchorY = legGraphicAnchors[1, (j >= 2) ? 1 : 0, 1].y / legGraphicAnchors[0, (j >= 2) ? 1 : 0, 1].y;
                sLeaser.sprites[LegSprite(j, 1)].anchorX = legGraphicAnchors[1, (j >= 2) ? 1 : 0, 1].x / legGraphicAnchors[0, (j >= 2) ? 1 : 0, 1].x;
                sLeaser.sprites[LegSprite(j, 2)] = TriangleMesh.MakeLongMesh(7, false, true);
            }
            antlers.InitiateSprites(FirstAntlerSprite, sLeaser, rCam);
            for (int k = 0; k < danglers.Length; k++)
            {
                danglers[k].InitSprite(sLeaser, FirstDanglerSprite + k);
            }
            sLeaser.containers = new FContainer[]
            {
            new FContainer(),
            new FContainer(),
            new FContainer()
            };
            AddToContainer(sLeaser, rCam, null);
            base.InitiateSprites(sLeaser, rCam);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            bodyColor = Color.Lerp(palette.blackColor, palette.texture.GetPixel(5, 4), 0.2f);
            for (int i = 0; i < 5; i++)
            {
                sLeaser.sprites[BodySprite(i)].color = bodyColor;
            }
            ReColorLegs(sLeaser, rCam, palette);
            fogCol = palette.fogColor;
            sLeaser.sprites[JawSprite(2)].color = bodyColor;
            sLeaser.sprites[JawSprite(1)].color = bodyColor;
            sLeaser.sprites[JawSprite(0)].color = Color.white;
            sLeaser.sprites[NeckSprite].color = bodyColor;
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < antlers.parts.Length; k++)
                {
                    sLeaser.sprites[FirstAntlerSprite + j * antlers.parts.Length + k].color = bodyColor;
                }
            }
            for (int m = 0; m < danglers.Length; m++)
            {
                sLeaser.sprites[FirstDanglerSprite + m].color = bodyColor;
            }
        }

        private void ReColorLegs(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            Color pixel = palette.texture.GetPixel(5, 4);
            for (int i = 0; i < 4; i++)
            {
                sLeaser.sprites[LegSprite(i, 0)].color = bodyColor;
                sLeaser.sprites[LegSprite(i, 1)].color = bodyColor;
                for (int j = 0; j < 28; j++)
                {
                    (sLeaser.sprites[LegSprite(i, 2)] as TriangleMesh).verticeColors[j] = Color.Lerp(bodyColor, pixel, Mathf.Pow(Mathf.InverseLerp(0.2f, 0.9f, (float)j / 27f), 1.6f));
                }
            }
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            sLeaser.RemoveAllSpritesFromContainer();
            newContatiner = rCam.ReturnFContainer("Midground");
            for (int i = FirstAntlerSprite; i < FirstAntlerSprite + antlers.parts.Length; i++)
            {
                newContatiner.AddChild(sLeaser.sprites[i]);
            }
            newContatiner.AddChild(sLeaser.containers[0]);
            newContatiner.AddChild(sLeaser.containers[1]);
            newContatiner.AddChild(sLeaser.containers[2]);
            for (int j = 0; j < FirstAntlerSprite; j++)
            {
                newContatiner.AddChild(sLeaser.sprites[j]);
            }
            for (int k = FirstAntlerSprite + antlers.parts.Length; k < TotalSprites; k++)
            {
                newContatiner.AddChild(sLeaser.sprites[k]);
            }
        }

        public float CurrentFaceDir(float timeStacker)
        {
            return Mathf.Lerp(lastFlip, flip, timeStacker) * 0.85f + Mathf.Lerp(lastAntlerRandomMovement, antlerRandomMovement, timeStacker) * 0.25f * (1f - Mathf.Abs(Mathf.Lerp(lastFlip, flip, timeStacker)) * 0.7f);
        }

        public Color CurrentFoggedHornColor(float timeStacker)
        {
            return Color.Lerp(bodyColor, fogCol, Mathf.Clamp(Mathf.Abs(CurrentFaceDir(timeStacker)), 0f, 1f) * 0.35f);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            if (culled)
            {
                return;
            }
            float num = CurrentFaceDir(timeStacker);
            antlers.DrawSprites(FirstAntlerSprite, sLeaser, rCam, timeStacker, camPos, Vector2.Lerp(owner.bodyChunks[0].lastPos, owner.bodyChunks[0].pos, timeStacker), Vector2.Lerp(owner.bodyChunks[5].lastPos, owner.bodyChunks[5].pos, timeStacker), num, bodyColor, CurrentFoggedHornColor(timeStacker));
            Vector2 vector = Vector2.Lerp(owner.bodyChunks[0].lastPos, owner.bodyChunks[0].pos, timeStacker);
            vector += Custom.DirVec(Vector2.Lerp(owner.bodyChunks[5].lastPos, owner.bodyChunks[5].pos, timeStacker), vector) * 50f + Custom.PerpendicularVector(Custom.DirVec(Vector2.Lerp(owner.bodyChunks[5].lastPos, owner.bodyChunks[5].pos, timeStacker), vector)) * 100f * num;
            Vector2 vector66 = Vector2.Lerp(WalkerBeast.Head.lastPos, WalkerBeast.Head.pos, timeStacker);
            Vector2 HeadLocation = Vector2.Lerp((owner as WalkerBeast).Head.lastPos, (owner as WalkerBeast).Head.pos, timeStacker);
            Vector2 vector77 = Custom.DirVec(Vector2.Lerp(WalkerBeast.neck.Tip.lastPos, WalkerBeast.neck.Tip.pos, timeStacker), HeadLocation);
            float num2 = Custom.VecToDeg(vector77);
            float num3 = Mathf.Lerp(lastHeadFlip, headFlip, timeStacker);
            Vector2 headPerp = Custom.PerpendicularVector(vector77);
            for (int i = 0; i < 2; i++)
            {
                beak[i].DrawSprites(sLeaser, rCam, timeStacker, camPos, num3);
            }
            //Vector2 HeadLocation = Vector2.Lerp(base.owner.bodyChunks[this.JawSprite(2)].lastPos, base.owner.bodyChunks[this.JawSprite(2)].pos, timeStacker);
            sLeaser.sprites[JawSprite(0)].x = HeadLocation.x - camPos.x;
            sLeaser.sprites[JawSprite(0)].y = HeadLocation.y - camPos.y;
            sLeaser.sprites[JawSprite(0)].rotation = num2;
            Vector2 vector67 = Vector2.Lerp(WalkerBeast.neck.connectedChunk.lastPos, WalkerBeast.neck.connectedChunk.pos, timeStacker);
            Vector2 vector4 = Vector2.Lerp(WalkerBeast.mainBodyChunk.lastPos, WalkerBeast.mainBodyChunk.pos, timeStacker);
            Vector2 a = Custom.DirVec(vector4, Vector2.Lerp(WalkerBeast.bodyChunks[1].lastPos, WalkerBeast.bodyChunks[1].pos, timeStacker));
            vector = Vector2.Lerp(WalkerBeast.Head.lastPos, WalkerBeast.Head.pos, timeStacker);
            float num23 = 8f;
            for (int l = 0; l < WalkerBeast.neck.tChunks.Length; l++)
            {
                float thick = 1f;
                if (l < 5)
                {
                    thick = 1f + ((5f - l) / 7f);
                }
                Vector2 vector6 = Vector2.Lerp(WalkerBeast.neck.tChunks[l].lastPos, WalkerBeast.neck.tChunks[l].pos, timeStacker);
                if (l == WalkerBeast.neck.tChunks.Length - 1)
                {
                    vector6 = Vector2.Lerp(vector6, vector, 0.5f);
                }
                else if (l == 0)
                {
                    vector6 = Vector2.Lerp(vector6, vector4 + a * 40f, 0.3f);
                }
                Vector2 normalized = (vector6 - vector67).normalized;
                Vector2 z2 = Custom.PerpendicularVector(normalized);
                float d = Vector2.Distance(vector6, vector67) / 5f;
                (sLeaser.sprites[NeckSprite] as TriangleMesh).MoveVertice(l * 4, vector67 - z2 * (WalkerBeast.neck.tChunks[l].stretchedRad + num23) * 0.5f * thick * neckFatness + normalized * d * ((l != 0) ? 1f : 0f) - camPos);
                (sLeaser.sprites[NeckSprite] as TriangleMesh).MoveVertice(l * 4 + 1, vector67 + z2 * (WalkerBeast.neck.tChunks[l].stretchedRad + num23) * 0.5f * thick * neckFatness + normalized * d * ((l != 0) ? 1f : 0f) - camPos);
                if (l == WalkerBeast.neck.tChunks.Length - 1)
                {
                    thick = 0f;
                }
                (sLeaser.sprites[NeckSprite] as TriangleMesh).MoveVertice(l * 4 + 2, vector6 - z2 * WalkerBeast.neck.tChunks[l].stretchedRad * thick * neckFatness - normalized * d * ((l != WalkerBeast.neck.tChunks.Length - 1) ? 1f : 0f) - camPos);
                (sLeaser.sprites[NeckSprite] as TriangleMesh).MoveVertice(l * 4 + 3, vector6 + z2 * WalkerBeast.neck.tChunks[l].stretchedRad * thick * neckFatness - normalized * d * ((l != WalkerBeast.neck.tChunks.Length - 1) ? 1f : 0f) - camPos);
                //(sLeaser.sprites[this.NeckSprite] as TriangleMesh).
                num23 = WalkerBeast.neck.tChunks[l].stretchedRad;
                vector67 = vector6;
            }
            for (int m = 0; m < 5; m++)
            {
                Vector2 vector5 = Vector2.Lerp(owner.bodyChunks[m].lastPos, owner.bodyChunks[m].pos, timeStacker);
                sLeaser.sprites[BodySprite(m)].x = vector5.x - camPos.x;
                sLeaser.sprites[BodySprite(m)].y = vector5.y - camPos.y;
                if (m == 0)
                {
                    sLeaser.sprites[BodySprite(m)].rotation = Custom.AimFromOneVectorToAnother(vector5, vector);
                }
                else
                {
                    sLeaser.sprites[BodySprite(m)].rotation = Custom.AimFromOneVectorToAnother(Vector2.Lerp(owner.bodyChunks[m].rotationChunk.lastPos, owner.bodyChunks[m].rotationChunk.pos, timeStacker), vector5) + chunksRotat[m - 1];
                }
            }
            Vector2 a2 = Custom.DirVec(Vector2.Lerp(WalkerBeast.bodyChunks[3].lastPos, WalkerBeast.bodyChunks[3].pos, timeStacker), Vector2.Lerp(WalkerBeast.bodyChunks[1].lastPos, WalkerBeast.bodyChunks[1].pos, timeStacker));
            for (int n = 0; n < 4; n++)
            {
                Vector2 vector6 = Vector2.Lerp(WalkerBeast.legs[n].connectedChunk.lastPos, WalkerBeast.legs[n].connectedChunk.pos, timeStacker);
                Vector2 vector7 = Vector2.Lerp(WalkerBeast.legs[n].tChunks[1].lastPos, WalkerBeast.legs[n].tChunks[1].pos, timeStacker);
                if (n < 2)
                {
                    vector7 = Vector2.Lerp(vector7, vector6, 0.2f) + a2 * 20f;
                    vector6 = Vector2.Lerp(vector6, Vector2.Lerp(WalkerBeast.bodyChunks[0].lastPos, WalkerBeast.bodyChunks[0].pos, timeStacker), 0.4f) + new Vector2(0f, -20f) + a2 * 5f;
                }
                else
                {
                    vector6 = Vector2.Lerp(vector6, Vector2.Lerp(WalkerBeast.bodyChunks[4].lastPos, WalkerBeast.bodyChunks[4].pos, timeStacker), 0.5f) + new Vector2(0f, -5f);
                    vector6 -= a2 * 17f;
                    vector7 -= a2 * 10f;
                }
                Vector2 vector8 = Custom.InverseKinematic(vector6, vector7, LegPartLength((n >= 2) ? 1 : 0, 0, true), LegPartLength((n >= 2) ? 1 : 0, 1, true), ((n >= 2) ? -1f : 1f) * Mathf.Sign(num));
                vector8 = Vector2.Lerp(vector8, vector6 + Custom.DirVec(vector6, Vector2.Lerp(WalkerBeast.legs[n].tChunks[0].lastPos, WalkerBeast.legs[n].tChunks[0].pos, timeStacker)) * LegPartLength((n >= 2) ? 1 : 0, 0, true), 1f - Mathf.Abs(num));
                Vector2 vector9 = Vector2.Lerp(WalkerBeast.legs[n].tChunks[2].lastPos, WalkerBeast.legs[n].tChunks[2].pos, timeStacker);
                Vector2 vector10 = Vector2.Lerp(WalkerBeast.legs[n].tChunks[3].lastPos, WalkerBeast.legs[n].tChunks[3].pos, timeStacker);
                vector9 = Vector2.Lerp(vector9, vector10 + Custom.DirVec(vector10, Vector2.Lerp(WalkerBeast.legs[n].tChunks[2].lastPos, WalkerBeast.legs[n].tChunks[2].pos, timeStacker)) * 40f, 0.5f);
                float num4 = Vector2.Distance(vector7, vector9);
                Vector2 vector11 = Custom.InverseKinematic(vector7, vector9, Mathf.Lerp(num4 / 2f, 80f, 0.5f), Mathf.Lerp(num4 / 2f, 80f, 0.5f), -Mathf.Sign(num));
                vector11 = Vector2.Lerp(vector11, Vector2.Lerp(Vector2.Lerp(WalkerBeast.legs[n].tChunks[1].lastPos, WalkerBeast.legs[n].tChunks[1].pos, timeStacker), Vector2.Lerp(WalkerBeast.legs[n].tChunks[2].lastPos, WalkerBeast.legs[n].tChunks[2].pos, timeStacker), 0.5f), Mathf.Lerp(1f, 0.5f, Mathf.Abs(num)));
                float num5 = (num >= ((n % 2 != 0) ? -0.5f : 0.5f)) ? 1f : -1f;
                if (n >= 2 && Mathf.Abs(num) < 0.7f && Mathf.Sign(num) != num5)
                {
                    vector6 += Custom.PerpendicularVector((vector6 - vector8).normalized) * 50f * num5 * Mathf.InverseLerp(0f, 0.7f, Mathf.Abs(num));
                    vector6 = vector8 + Custom.DirVec(vector8, vector6) * LegPartLength(1, 0, true);
                }
                sLeaser.sprites[LegSprite(n, 0)].x = vector8.x - camPos.x;
                sLeaser.sprites[LegSprite(n, 0)].y = vector8.y - camPos.y;
                sLeaser.sprites[LegSprite(n, 0)].rotation = Custom.AimFromOneVectorToAnother(vector8, vector6);
                sLeaser.sprites[LegSprite(n, 0)].scaleY = Vector2.Distance(vector6, vector8) / LegPartLength((n >= 2) ? 1 : 0, 0, false);
                sLeaser.sprites[LegSprite(n, 0)].scaleX = num5 * Mathf.Min(1f, Vector2.Distance(vector6, vector8) / LegPartLength((n >= 2) ? 1 : 0, 0, false));
                sLeaser.sprites[LegSprite(n, 1)].x = vector7.x - camPos.x;
                sLeaser.sprites[LegSprite(n, 1)].y = vector7.y - camPos.y;
                sLeaser.sprites[LegSprite(n, 1)].rotation = Custom.AimFromOneVectorToAnother(vector7, vector8);
                sLeaser.sprites[LegSprite(n, 1)].scaleY = Vector2.Distance(vector8, vector7) / LegPartLength((n >= 2) ? 1 : 0, 1, false);
                sLeaser.sprites[LegSprite(n, 1)].scaleX = num5 * Mathf.Min(1f, Vector2.Distance(vector8, vector7) / LegPartLength((n >= 2) ? 1 : 0, 1, false));
                Vector2 vector12 = vector8;
                for (int num6 = 0; num6 < 7; num6++)
                {
                    Vector2 vector13 = vector8;
                    float d = Mathf.Lerp(5f, 1f, (float)num6 / 6f);
                    float d2 = Mathf.Lerp(5f, 1f, ((float)num6 + 0.5f) / 6f);
                    switch (num6)
                    {
                        case 0:
                            vector13 = vector7 - Custom.DirVec(vector8, vector11) * 2.5f;
                            d = 0f;
                            d2 = 4f;
                            break;

                        case 1:
                            vector13 = vector7 + Custom.DirVec(vector8, vector11) * 7.5f;
                            d = 4f;
                            d2 = 6f;
                            break;

                        case 2:
                            vector13 = vector11 - Custom.DirVec(vector7, vector9) * 7.5f;
                            break;

                        case 3:
                            vector13 = vector11 + Custom.DirVec(vector7, vector9) * 7.5f;
                            d2 = 4.5f;
                            break;

                        case 4:
                            vector13 = vector9 - Custom.DirVec(vector11, vector10) * 7.5f;
                            break;

                        case 5:
                            vector13 = vector9 + Custom.DirVec(vector11, vector10) * 7.5f;
                            d = 3f;
                            d2 = 2f;
                            break;

                        case 6:
                            vector13 = vector10;
                            break;
                    }
                    Vector2 normalized = (vector13 - vector12).normalized;
                    Vector2 a3 = Custom.PerpendicularVector(normalized);
                    float d3 = Vector2.Distance(vector13, vector12) / 5f;
                    (sLeaser.sprites[LegSprite(n, 2)] as TriangleMesh).MoveVertice(num6 * 4 + 2, vector13 - a3 * d2 - normalized * d3 - camPos);
                    (sLeaser.sprites[LegSprite(n, 2)] as TriangleMesh).MoveVertice(num6 * 4 + 3, vector13 + a3 * d2 - normalized * d3 - camPos);
                    if (num6 == 6)
                    {
                        d3 = Vector2.Distance(vector13, vector12);
                    }
                    (sLeaser.sprites[LegSprite(n, 2)] as TriangleMesh).MoveVertice(num6 * 4, vector12 - a3 * d + normalized * d3 - camPos);
                    (sLeaser.sprites[LegSprite(n, 2)] as TriangleMesh).MoveVertice(num6 * 4 + 1, vector12 + a3 * d + normalized * d3 - camPos);
                    vector12 = vector13;
                }
            }
            sLeaser.sprites[AntlerSprite].alpha = 0f;
        }

        public Vector2 DanglerConnection(int index, float timeStacker)
        {
            if (index < bodyDanglers)
            {
                Vector2 vector = Vector2.Lerp(WalkerBeast.bodyChunks[bodyDanglerPositions[index, 0]].lastPos, WalkerBeast.bodyChunks[bodyDanglerPositions[index, 0]].pos, timeStacker);
                Vector2 vector2 = Vector2.Lerp(WalkerBeast.bodyChunks[bodyDanglerPositions[index, 1]].lastPos, WalkerBeast.bodyChunks[bodyDanglerPositions[index, 1]].pos, timeStacker);
                vector += Custom.DegToVec(Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(WalkerBeast.bodyChunks[bodyDanglerPositions[index, 0]].rotationChunk.lastPos, WalkerBeast.bodyChunks[bodyDanglerPositions[index, 0]].rotationChunk.pos, timeStacker)) + bodyDanglerOrientations[index, 0]) * bodyDanglerOrientations[index, 1];
                vector2 += Custom.DegToVec(Custom.AimFromOneVectorToAnother(vector2, Vector2.Lerp(WalkerBeast.bodyChunks[bodyDanglerPositions[index, 1]].rotationChunk.lastPos, WalkerBeast.bodyChunks[bodyDanglerPositions[index, 1]].rotationChunk.pos, timeStacker)) + bodyDanglerOrientations[index, 2]) * bodyDanglerOrientations[index, 3];
                return Vector2.Lerp(vector, vector2, bodyDanglerOrientations[index, 4]);
            }
            index -= bodyDanglers;
            Vector2 vector3 = Vector2.Lerp(WalkerBeast.antlers.lastPos, WalkerBeast.antlers.pos, timeStacker);
            return antlers.TransformToHeadRotat(antlers.parts[hornDanglerPositions[index, 0]].GetTransoformedPos(hornDanglerPositions[index, 1], (float)hornDanglerPositions[index, 2]), vector3, Custom.AimFromOneVectorToAnother(Vector2.Lerp(WalkerBeast.mainBodyChunk.lastPos, WalkerBeast.mainBodyChunk.pos, timeStacker), vector3), (float)hornDanglerPositions[index, 2], CurrentFaceDir(timeStacker));
        }

        public Dangler.DanglerProps Props(int index)
        {
            return danglerVals;
        }

        public float CreatureInterestBonus(Tracker.CreatureRepresentation crit, float score)
        {
            return score;
        }

        public Tracker.CreatureRepresentation ForcedLookCreature()
        {
            return null;
        }

        public void LookAtNothing()
        {
        }

        private Dangler.DanglerProps danglerVals;
        private static Vector2[,,] legGraphicAnchors;
        private float lastFlip;
        private float flip;
        private float antlerRandomMovement;
        private float lastAntlerRandomMovement;
        private float antlerRandomMovementVel;
        private float[] chunksRotat;
        public WalkerBeastGraphics.Antlers antlers;
        private Color fogCol;
        private Dangler[] danglers;
        public int[,] bodyDanglerPositions;
        public float[,] bodyDanglerOrientations;
        public int bodyDanglers;
        public int hornDanglers;
        public int[,] hornDanglerPositions;
        public CreatureLooker looker;
        public Vector2[] lookPoint;
        public int blink;
        public int lastBlink;
        public Color bodyColor;
        public float headFlip;
        public float lastHeadFlip;
        public float eyeSize;
        public float neckFatness;
        public WalkerBeastGraphics.BeakGraphic[] beak;
        public float beakFatness;

        public class Antlers
        {
            public Antlers(float rad, float thickness)
            {
                this.rad = rad;
                this.thickness = thickness;
                List<WalkerBeastGraphics.Antlers.Part> list = new List<WalkerBeastGraphics.Antlers.Part>();
                WalkerBeastGraphics.Antlers.GenerateValues generateValues = new WalkerBeastGraphics.Antlers.GenerateValues(rad);
                bool spine = true;
                float startRad = 5f;
                Vector3? beforeStart = null;
                Vector3 start = new Vector3(0f, -rad);
                Vector3 goal = Custom.DegToVec(Mathf.Lerp(10f, 45f, UnityEngine.Random.value)) * rad;
                Vector3 vector = new Vector3(Mathf.Lerp(0.4f, 1f, UnityEngine.Random.value), 1f, Mathf.Lerp(-1f, 1f, UnityEngine.Random.value));
                WalkerBeastGraphics.Antlers.Part part = GeneratePart(spine, startRad, beforeStart, start, goal, vector.normalized, generateValues);
                generateValues.goalTend = Mathf.Lerp(generateValues.goalTend, 0.2f, 0.5f);
                list = new List<WalkerBeastGraphics.Antlers.Part>
            {
                part
            };
                int num = part.positions.Length;
                int num2 = 30;
                Vector3[] array = new Vector3[num2];
                Vector3 vector2 = UnityEngine.Random.onUnitSphere;
                for (int i = 0; i < num2; i++)
                {
                    array[i] = vector2;
                    vector2 = (-vector2 + UnityEngine.Random.onUnitSphere).normalized;
                    if (array[i].x < 0f)
                    {
                        Vector3[] array2 = array;
                        int num3 = i;
                        array2[num3].x = array2[num3].x * -1f;
                    }
                }
                Vector3[] array3 = array;
                int num4 = 0;
                array3[num4].z = array3[num4].z - 1f;
                array[0].Normalize();
                Vector3[] array4 = array;
                int num5 = 1;
                array4[num5].z = array4[num5].z + 1f;
                array[1].Normalize();
                int num6 = 0;
                while (num6 < num2 && num < 80)
                {
                    array[num6] *= rad * Mathf.Lerp(0.2f, 2f, Mathf.Pow(UnityEngine.Random.value, 0.45f));
                    float num7 = float.MaxValue;
                    int num8 = 0;
                    int index = 0;
                    for (int j = 0; j < list.Count; j++)
                    {
                        for (int k = 0; k < list[j].positions.Length - 1; k++)
                        {
                            float num9 = Vector3.Distance(list[j].positions[k], array[num6]);
                            float num10 = Mathf.Lerp(num9, list[j].positions[k].y + rad, Mathf.InverseLerp(0f, rad * 2.5f, num9) * 0.15f);
                            num10 *= 2f - Mathf.Pow(Mathf.Sin(Mathf.Pow((float)k / (float)(list[j].positions.Length - 2), 1f) * 3.14159274f), 0.5f);
                            if (num10 < num7)
                            {
                                num8 = k;
                                num7 = num10;
                                index = j;
                            }
                        }
                    }
                    num8 = Math.Max(1, num8 - Mathf.FloorToInt(Vector3.Distance(list[index].positions[num8], array[num6]) / 30f));
                    list[index].lastBranchingSegment = num8;
                    Vector3 start2 = list[index].positions[num8];
                    Vector3 normalized = (list[index].positions[num8] - list[index].positions[num8 - 1]).normalized;
                    Vector3? beforeStart2 = null;
                    if (num8 > 0)
                    {
                        beforeStart2 = new Vector3?(list[index].positions[num8 - 1]);
                    }
                    WalkerBeastGraphics.Antlers.Part part2 = GeneratePart(false, list[index].rads[num8], beforeStart2, start2, array[num6], normalized, generateValues);
                    if (part2 != null)
                    {
                        list.Add(part2);
                        num += part2.positions.Length;
                    }
                    num6++;
                }
                parts = list.ToArray();
                for (int l = 0; l < parts.Length; l++)
                {
                    parts[l].GenerateInds(rad);
                }
            }

            public int SpritesClaimed
            {
                get
                {
                    return parts.Length * 2;
                }
            }

            private WalkerBeastGraphics.Antlers.Part GeneratePart(bool spine, float startRad, Vector3? beforeStart, Vector3 start, Vector3 goal, Vector3 initDir, WalkerBeastGraphics.Antlers.GenerateValues genVals)
            {
                List<Vector3> list = new List<Vector3>();
                List<float> list2 = new List<float>();
                if (beforeStart != null)
                {
                    list.Add(beforeStart.Value);
                    list2.Add(startRad * 0.5f);
                }
                Vector3 vector = start;
                Vector3 a = initDir;
                float num = genVals.circumferenceTend;
                float attractRad = genVals.attractRad;
                float num2 = genVals.goalTend;
                float num3 = genVals.randomTend;
                float num4 = 10f;
                int num5 = Mathf.FloorToInt(Vector3.Distance(start, goal) / num4);
                if (num5 > 2)
                {
                    for (int i = 0; i < num5; i++)
                    {
                        float f = (float)i / (float)(num5 - 1);
                        list.Add(vector);
                        float num6 = Mathf.Lerp(startRad, 1.2f, Mathf.Pow(f, 0.5f));
                        num6 = Mathf.Lerp(num6, Custom.LerpMap(Vector3.Distance(vector, new Vector3(0f, rad * -0.5f, 0f)), 0f, rad * 1.5f, 6f, 1.2f), 0.5f);
                        list2.Add(num6);
                        vector += a * num4;
                        if (spine)
                        {
                            vector.z -= 11f * Mathf.Sin((float)i / (float)(num5 - 1) * 3.14159274f);
                        }
                        if (vector.x < 15f)
                        {
                            vector.x = 15f;
                        }
                        vector = Vector3.Lerp(vector, goal, Mathf.Pow(f, 4f));
                        a = (a + UnityEngine.Random.onUnitSphere * num3 + vector.normalized * (attractRad - vector.magnitude) * ((vector.magnitude <= rad) ? num : Mathf.Max(num, 0.2f)) + (goal - vector).normalized * num2).normalized;
                        num = Mathf.Clamp(num + genVals.circumferenceTendChange, 0f, 0.5f);
                        num2 = Mathf.Clamp(num2 + genVals.goalTendChange, 0f, 0.5f);
                        num3 = Mathf.Clamp(num3 + genVals.randomTendChange, 0f, 0.5f);
                    }
                    Vector3 vector2 = Vector3.Lerp(list[list.Count - 1], list[list.Count - 2], 0.75f);
                    vector2 = Vector3.Lerp(vector2, list[list.Count - 1] - (list[list.Count - 1] - list[list.Count - 2]).normalized * 3f, 1f);
                    list.Insert(list.Count - 1, vector2);
                    list2.Insert(list2.Count - 1, Mathf.Lerp(startRad, Custom.LerpMap(Vector3.Distance(goal, new Vector3(0f, rad * -0.5f, 0f)), 0f, rad * 1.5f, 7f, 4f), 0.5f));
                    List<float> list4;
                    List<float> list3 = list4 = list2;
                    int index2;
                    int index = index2 = list2.Count - 1;
                    float num7 = list4[index2];
                    list3[index] = num7 * 0.15f;
                    return new WalkerBeastGraphics.Antlers.Part(list.ToArray(), list2.ToArray());
                }
                return null;
            }

            public Vector2 TransformToHeadRotat(Vector3 dpPos, Vector2 antlerPos, float rotation, float flip, float WalkerBeastFaceDir)
            {
                float degAng = Mathf.Lerp(-116.999992f, 116.999992f, Mathf.InverseLerp(-1.3f, 1.3f, WalkerBeastFaceDir));
                dpPos.x *= flip;
                Vector2 vector = new Vector2(Custom.RotateAroundOrigo(new Vector2(dpPos.x, dpPos.z), degAng).x, dpPos.y);
                vector = Custom.RotateAroundOrigo(vector, rotation);
                vector += antlerPos;
                return vector;
            }

            public void InitiateSprites(int firstAntlerSprite, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < parts.Length; j++)
                    {
                        sLeaser.sprites[firstAntlerSprite + i * parts.Length + j] = TriangleMesh.MakeLongMesh(parts[j].positions.Length, false, false);
                    }
                }
            }

            public void DrawSprites(int firstAntlerSprite, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, Vector2 headPos, Vector2 antlerPos, float WalkerBeastFaceDir, Color blackCol, Color foggedCol)
            {
                float rotation = Custom.AimFromOneVectorToAnother(headPos, antlerPos);
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < parts.Length; j++)
                    {
                        int num = (WalkerBeastFaceDir >= 0f) ? (1 - i) : i;
                        WalkerBeastGraphics.Antlers.Part part = parts[j];
                        float flip = (i != 0) ? 1f : -1f;
                        int num2 = firstAntlerSprite + num * parts.Length + j;
                        Vector2 vector = TransformToHeadRotat(part.GetTransoformedPos(0, flip), antlerPos, rotation, flip, WalkerBeastFaceDir);
                        float num3 = 0.2f;
                        if (num == 0)
                        {
                            sLeaser.sprites[num2].color = foggedCol;
                            sLeaser.sprites[num2].alpha = 0f;
                        }
                        else
                        {
                            sLeaser.sprites[num2].color = blackCol;
                            sLeaser.sprites[num2].alpha = 0f;
                        }
                        for (int k = 0; k < part.positions.Length; k++)
                        {
                            Vector2 vector2 = TransformToHeadRotat(part.GetTransoformedPos(k, flip), antlerPos, rotation, flip, WalkerBeastFaceDir);
                            Vector2 normalized = (vector2 - vector).normalized;
                            Vector2 a = Custom.PerpendicularVector(normalized);
                            float d = Vector2.Distance(vector2, vector) / 5f;
                            (sLeaser.sprites[num2] as TriangleMesh).MoveVertice(k * 4, vector - a * (num3 + part.rads[k]) * 0.5f * thickness + normalized * d - camPos);
                            (sLeaser.sprites[num2] as TriangleMesh).MoveVertice(k * 4 + 1, vector + a * (num3 + part.rads[k]) * 0.5f * thickness + normalized * d - camPos);
                            (sLeaser.sprites[num2] as TriangleMesh).MoveVertice(k * 4 + 2, vector2 - a * part.rads[k] * thickness - normalized * d - camPos);
                            (sLeaser.sprites[num2] as TriangleMesh).MoveVertice(k * 4 + 3, vector2 + a * part.rads[k] * thickness - normalized * d - camPos);
                            num3 = part.rads[k];
                            vector = vector2;
                        }
                    }
                }
            }

            public WalkerBeastGraphics.Antlers.Part[] parts;
            public float rad;
            public float thickness;

            public class Part
            {
                public Part(Vector3[] positions, float[] rads)
                {
                    this.positions = positions;
                    this.rads = rads;
                }

                public Vector3 GetTransoformedPos(int pos, float flip)
                {
                    return positions[pos] + indPos[(flip >= 0f) ? 1 : 0, pos];
                }

                public void GenerateInds(float rad)
                {
                    indPos = new Vector3[2, positions.Length];
                    for (int i = 0; i < 2; i++)
                    {
                        Vector3 vector = UnityEngine.Random.onUnitSphere * 11f * Mathf.Pow(UnityEngine.Random.value, 0.75f);
                        for (int j = lastBranchingSegment + 2; j < positions.Length; j++)
                        {
                            indPos[i, j] += vector;
                            vector *= 1.1f;
                            vector += UnityEngine.Random.onUnitSphere * 2f * UnityEngine.Random.value;
                        }
                    }
                }

                public int lastBranchingSegment = 2;
                public Vector3[] positions;
                public float[] rads;
                public Vector3[,] indPos;
            }

            public class GenerateValues
            {
                public GenerateValues(float rad)
                {
                    attractRad = rad * Mathf.Lerp(0.7f, 1f, UnityEngine.Random.value);
                    circumferenceTend = Mathf.Lerp(0f, 0.01f, UnityEngine.Random.value);
                    goalTend = Mathf.Lerp(0f, 0.02f, UnityEngine.Random.value);
                    randomTend = Mathf.Lerp(0.1f, 0.4f, UnityEngine.Random.value);
                    circumferenceTendChange = Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) * 0.01f * 0.5f;
                    goalTendChange = Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) * 0.02f * 0.5f;
                    randomTendChange = Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) * 0.1f * 0.5f;
                }

                public float circumferenceTend;
                public float goalTend;
                public float randomTend;
                public float circumferenceTendChange;
                public float goalTendChange;
                public float randomTendChange;
                public float attractRad;
            }
        }

        public class BeakGraphic
        {
            public BeakGraphic(WalkerBeastGraphics owner, int index, int firstSprite)
            {
                this.owner = owner;
                this.firstSprite = firstSprite;
                this.index = index;
            }

            private float OuterShape(float f)
            {
                return Mathf.Max(1f - f, Mathf.Sin(f * 3.14159274f));
            }

            public void Update()
            {
            }

            public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                if (index == 0)
                {
                    String spritename = "WalkerHead";
                    if ((owner.owner as WalkerBeast).iVars.largeHorns)
                    {
                        spritename = spritename + "A";
                    }
                    else
                    {
                        spritename = spritename + "B";
                    }
                    sLeaser.sprites[firstSprite] = new FSprite(spritename, true);
                }
                else
                {
                    sLeaser.sprites[firstSprite] = new FSprite("WalkerJaw", true);
                }
            }

            public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, float useFlip)
            {
                Vector2 headPos = Vector2.Lerp((owner.owner as WalkerBeast).Head.lastPos, (owner.owner as WalkerBeast).Head.pos, timeStacker);
                float headAng = Custom.VecToDeg(Custom.DirVec(Vector2.Lerp(owner.WalkerBeast.neck.Tip.lastPos, owner.WalkerBeast.neck.Tip.pos, timeStacker), headPos)) - 90f;
                float jawOpen = Mathf.Lerp(owner.WalkerBeast.lastJawOpen, owner.WalkerBeast.jawOpen, timeStacker);
                float jawAng = headAng + 60f * jawOpen * (-1f) * useFlip * Mathf.Pow(Mathf.Abs(useFlip), 0.5f);
                Vector2 correctingVector;

                if ((headAng > 90 & headAng < 270) || ((headAng < -90 & headAng > -270)) || (headAng > 450 & headAng < 630) || ((headAng < -450 & headAng > -630)))
                {
                    sLeaser.sprites[firstSprite].scaleX = -0.8f;
                    headAng = headAng + 200f;
                    jawAng = jawAng + 200f;
                    if (index == 1)
                    {
                        correctingVector = new Vector2(15f, -33f);
                        //left jaw
                    }
                    else if ((owner.owner as WalkerBeast).iVars.largeHorns)
                    {
                        correctingVector = new Vector2(20f, -7f);
                        //left with large horns
                    }
                    else
                    {
                        correctingVector = new Vector2(18f, -15f);
                        //Left with small horns
                    }
                }
                else

                {
                    headAng = headAng - 20f;
                    jawAng = jawAng - 20f;
                    sLeaser.sprites[firstSprite].scaleX = 0.8f;
                    if (index == 1)
                    {
                        correctingVector = new Vector2(20f, -33f);
                        //right jaw
                    }
                    else if ((owner.owner as WalkerBeast).iVars.largeHorns)
                    {
                        correctingVector = new Vector2(10f, -7f);
                        //right with large horns
                    }
                    else
                    {
                        correctingVector = new Vector2(14f, -15f);
                        //left with small horns
                    }
                }
                correctingVector = new Vector2(correctingVector.x - 14, correctingVector.y + 18);
                //new Vector2(headPos.x-sLeaser.sprites[this.firstSprite].GetPosition().x, headPos.y-sLeaser.sprites[this.firstSprite].GetPosition().y)
                sLeaser.sprites[firstSprite].scaleY = 0.8f;
                sLeaser.sprites[firstSprite].SetPosition(headPos.x + correctingVector.x - rCam.pos.x, headPos.y + correctingVector.y - rCam.pos.y);
                sLeaser.sprites[firstSprite].rotation = 0f;
                sLeaser.sprites[firstSprite].RotateAroundPointAbsolute(new Vector2((headPos.x - rCam.pos.x) - sLeaser.sprites[firstSprite].GetPosition().x, (headPos.y - rCam.pos.y) - sLeaser.sprites[firstSprite].GetPosition().y), headAng);
                //sLeaser.sprites[this.firstSprite].rotation = 0f;
            }

            private WalkerBeastGraphics owner;
            public int firstSprite;
            public int index;
        }
    }
}
