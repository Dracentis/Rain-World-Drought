using System;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    public class WalkerBeastTentacle : Tentacle
    {
        public WalkerBeastTentacle(WalkerBeast walkerBeast, BodyChunk chunk, float length, int tentacleNumber) : base(walkerBeast, chunk, length)
        {
            setGrabDelay = 8;
            this.tentacleNumber = tentacleNumber;
            tProps = new Tentacle.TentacleProps(false, false, true, 0.5f, 0f, 0.6f, 0.8f, 1.04f, 1.2f, 10f, 1f, 5f, 15, 60, 12, 0);
            tChunks = new Tentacle.TentacleChunk[4];
            for (int i = 0; i < tChunks.Length; i++)
            {
                tChunks[i] = new Tentacle.TentacleChunk(this, i, (float)(i + 1) / (float)tChunks.Length, 4f);
            }
            side = ((tentacleNumber % 2 != 0) ? 1 : 0);
            pair = ((tentacleNumber >= 2) ? 1 : 0);
            tentacleDir = Custom.DegToVec(45f + 90f * (float)tentacleNumber);
            stretchAndSqueeze = 0.1f;
            debugViz = false;
        }

        public WalkerBeast walkerBeast
        {
            get
            {
                return owner as WalkerBeast;
            }
        }

        private WalkerBeastTentacle OtherTentacleInPair
        {
            get
            {
                return walkerBeast.legs[(tentacleNumber >= 2) ? (2 + (1 - (tentacleNumber - 1))) : (1 - tentacleNumber)];
            }
        }

        public override void NewRoom(Room room)
        {
            base.NewRoom(room);
            if (debugViz)
            {
                if (grabGoalSprites != null)
                {
                    grabGoalSprites[0].RemoveFromRoom();
                    grabGoalSprites[1].RemoveFromRoom();
                }
                grabGoalSprites = new DebugSprite[2];
                grabGoalSprites[0] = new DebugSprite(new Vector2(0f, 0f), new FSprite("pixel", true), room);
                grabGoalSprites[0].sprite.scale = 10f;
                grabGoalSprites[0].sprite.color = new Color(1f, 0f, 0f);
                room.AddObject(grabGoalSprites[0]);
                grabGoalSprites[1] = new DebugSprite(new Vector2(0f, 0f), new FSprite("pixel", true), room);
                grabGoalSprites[1].sprite.scale = 10f;
                grabGoalSprites[1].sprite.color = new Color(0f, 5f, 0f);
                room.AddObject(grabGoalSprites[1]);
            }
            IntVector2 tilePosition = room.GetTilePosition(FloatBase + new Vector2(0f, -400f) + tentacleDir * 100f);
            IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, BasePos, tilePosition);
            if (intVector != null)
            {
                Vector2 newGrabDest = Custom.RestrictInRect(FloatBase, room.TileRect(intVector.Value).Grow(2f));
                segments = SharedPhysics.RayTracedTilesArray(FloatBase, room.MiddleOfTile(intVector.Value));
                if (segments.Count > 2)
                {
                    segments.RemoveAt(segments.Count - 1);
                }
                MoveGrabDest(newGrabDest);
                for (int i = 0; i < tChunks.Length; i++)
                {
                    tChunks[i].Reset();
                }
                attachedAtTip = true;
                smoothedFootingSecurity = 1f;
            }
        }

        public override void Update()
        {
            base.Update();
            smoothedFootingSecurity = Mathf.Lerp(smoothedFootingSecurity, (!attachedAtTip) ? Mathf.InverseLerp(3f, 30f, (float)sureOfFootingCounter) : 0f, 0.2f);
            attachedAtTip = false;
            maxLength = 25f * walkerBeast.preferredHeight * 1.33333337f;
            idealLength = Mathf.Min(Mathf.Lerp(idealLength, (grabDest == null) ? maxLength : (Vector2.Distance(FloatBase, floatGrabDest.Value) * 1.5f * Mathf.InverseLerp((float)setGrabDelay, 0f, (float)grabDelay)), 0.03f), maxLength);
            retractFac = 0f;
            limp = !walkerBeast.Consious;
            if (limp)
            {
                floatGrabDest = null;
                for (int i = 0; i < tChunks.Length; i++)
                {
                    tChunks[i].vel *= 0.9f;
                    Tentacle.TentacleChunk tentacleChunk = tChunks[i];
                    tentacleChunk.vel.y = tentacleChunk.vel.y - 0.5f;
                }
            }
            for (int j = 0; j < tChunks.Length; j++)
            {
                tChunks[j].rad = 5f;
                float num = (float)j / (float)(tChunks.Length - 1);
                tChunks[j].vel *= 0.5f;
                if (backtrackFrom == -1 || j < backtrackFrom)
                {
                    if (j < tChunks.Length - 1)
                    {
                        tChunks[j].vel += walkerBeast.HeadDir * Mathf.Lerp(-1f, 1f, num) * Mathf.Pow(1f - num, 1.8f);
                    }
                    tChunks[j].vel += walkerBeast.moveDirection * Mathf.Sin(smoothedFootingSecurity * 3.14159274f) * 1.5f * Mathf.Lerp(1f, 2.5f, Mathf.Sin(num * 3.14159274f));
                }
                tChunks[j].pos = Vector2.Lerp(tChunks[j].pos, room.MiddleOfTile(segments[tChunks[j].currentSegment]), 0.03f);
                if (room.PointSubmerged(tChunks[j].pos))
                {
                    tChunks[j].vel *= 0.5f;
                }
            }
            if (backtrackFrom == -1)
            {
                tChunks[1].vel += walkerBeast.moveDirection;
            }
            if (backtrackFrom == -1 && grabDest != null)
            {
                Tip.vel += Custom.DirVec(Tip.pos, floatGrabDest.Value) * Mathf.Lerp(0.2f, 38f, smoothedFootingSecurity);
            }
            if (floatGrabDest != null && Custom.DistLess(Tip.pos, floatGrabDest.Value, 40f) && backtrackFrom == -1)
            {
                Tip.pos = floatGrabDest.Value;
                Tip.vel *= 0f;
                attachedAtTip = true;
            }
            Tip.collideWithTerrain = !attachedAtTip;
            UpdateDesiredGrabPos();
            for (int k = 0; k < tChunks.Length; k++)
            {
                Tentacle.TentacleChunk tentacleChunk2 = tChunks[k];
                tentacleChunk2.vel.y = tentacleChunk2.vel.y - 0.1f;
                tChunks[k].vel += connectedChunk.vel * 0.1f;
                if (!attachedAtTip)
                {
                    if (floatGrabDest != null)
                    {
                        tChunks[k].vel += Custom.DirVec(tChunks[k].pos, floatGrabDest.Value) * 0.3f;
                    }
                    else
                    {
                        tChunks[k].vel += Custom.DirVec(tChunks[k].pos, desiredGrabPos + Custom.DirVec(FloatBase, desiredGrabPos) * 70f) * 0.6f;
                    }
                }
            }
            if (attachedAtTip)
            {
                framesWithoutReaching = 0;
                if (SharedPhysics.RayTraceTilesForTerrain(room, BasePos, grabDest.Value))
                {
                    if (!Custom.DistLess(Tip.pos, connectedChunk.pos, maxLength))
                    {
                        ReleaseGrip();
                    }
                    if (!Custom.DistLess(Tip.pos, connectedChunk.pos, maxLength * 0.9f))
                    {
                        walkerBeast.heldBackByLeg = true;
                    }
                }
                else
                {
                    ReleaseGrip();
                }
                if (playStepSound)
                {
                    if (stepSoundVol > 0.5f)
                    {
                        room.PlaySound(SoundID.Vulture_Tentacle_Grab_Terrain, Tip.pos, Mathf.InverseLerp(0.5f, 1f, stepSoundVol) * Mathf.InverseLerp(7f, 45f, Vector2.Distance(Tip.pos, Tip.lastPos)), 1f);
                    }
                    playStepSound = false;
                }
                stepSoundVol = 0f;
            }
            else
            {
                stepSoundVol = Mathf.Min(1f, stepSoundVol + 0.025f);
                playStepSound = true;
                FindGrabPos();
                framesWithoutReaching++;
                if ((float)framesWithoutReaching > 60f && floatGrabDest == null)
                {
                    framesWithoutReaching = 0;
                }
            }
            if (debugViz)
            {
                grabGoalSprites[1].pos = desiredGrabPos;
            }
        }

        // Token: 0x06001C7F RID: 7295 RVA: 0x00184668 File Offset: 0x00182868
        public void ReleaseGrip()
        {
            if (OtherTentacleInPair.grabDelay < 1 && grabDelay < 1)
            {
                grabDelay = setGrabDelay;
            }
            floatGrabDest = null;
            sureOfFootingCounter = 0;
        }

        // Token: 0x06001C80 RID: 7296 RVA: 0x001846B0 File Offset: 0x001828B0
        private void UpdateDesiredGrabPos()
        {
            desiredGrabPos = connectedChunk.pos + (new Vector2(0f, -1f) + walkerBeast.moveDirection * ((pair != 0) ? 0.6f : 0.8f) + tentacleDir * 0.25f).normalized * Mathf.Min(walkerBeast.nextFloorHeight, maxLength * 0.9f);
        }

        // Token: 0x06001C81 RID: 7297 RVA: 0x001847C0 File Offset: 0x001829C0
        private void FindGrabPos()
        {
            if (grabDelay > 0)
            {
                grabDelay--;
                return;
            }
            IntVector2? intVector = null;
            int num = 0;
            while (num < 9 && intVector == null)
            {
                intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, BasePos, room.GetTilePosition(desiredGrabPos) + Custom.eightDirectionsAndZero[num] * 5);
                if (intVector != null && room.GetTile(intVector.Value + new IntVector2(0, 1)).Solid)
                {
                    intVector = null;
                }
                num++;
            }
            if (intVector != null)
            {
                if (grabDest == null || GripPointAttractiveness(intVector.Value) > GripPointAttractiveness(grabDest.Value))
                {
                    MoveGrabDest(Custom.RestrictInRect(FloatBase + tentacleDir * Vector2.Distance(FloatBase, room.MiddleOfTile(intVector.Value)) * 0.5f, FloatRect.MakeFromVector2(room.MiddleOfTile(intVector.Value) - new Vector2(11f, 11f), room.MiddleOfTile(intVector.Value) + new Vector2(11f, 11f))));
                    sureOfFootingCounter = 0;
                }
                else
                {
                    sureOfFootingCounter++;
                }
            }
        }

        // Token: 0x06001C82 RID: 7298 RVA: 0x00184990 File Offset: 0x00182B90
        public float Support()
        {
            if (!attachedAtTip)
            {
                return 0f;
            }
            float num = 0f;
            for (int i = 0; i < 4; i++)
            {
                if (i != tentacleNumber && walkerBeast.legs[i].attachedAtTip && walkerBeast.legs[i].Tip.pos.x < walkerBeast.bodyChunks[2].pos.x != Tip.pos.x < walkerBeast.bodyChunks[2].pos.x)
                {
                    num = Mathf.Max(num, Mathf.Pow(Mathf.Sin(Mathf.InverseLerp(400f, 0f, Mathf.Abs(walkerBeast.bodyChunks[2].pos.x - walkerBeast.legs[i].Tip.pos.x)) * 3.14159274f), 0.3f));
                }
            }
            return Mathf.InverseLerp(Mathf.Lerp(0.5f, -1f, num), 1f, Vector2.Dot(new Vector2(0f, -1f), Custom.DirVec(connectedChunk.pos, Tip.pos)));
        }

        public float ReleaseScore()
        {
            float num;
            if (grabDest == null)
            {
                num = Vector2.Distance(Tip.pos, desiredGrabPos);
            }
            else
            {
                num = Vector2.Distance(floatGrabDest.Value, desiredGrabPos) * 2f;
            }
            if (attachedAtTip)
            {
                num *= 2f;
            }
            if (!OtherTentacleInPair.attachedAtTip)
            {
                num /= 100f;
            }
            num *= 1f + (FloatBase.x - Tip.pos.x) * Mathf.Sign(walkerBeast.moveDirection.x) * 1.4f;
            return num / (1f + Support() * 10f);
        }

        private float GripPointAttractiveness(IntVector2 pos)
        {
            if (!room.GetTile(pos + new IntVector2(0, 1)).Solid)
            {
                return 100f / room.GetTilePosition(desiredGrabPos).FloatDist(pos);
            }
            return 65f / room.GetTilePosition(desiredGrabPos).FloatDist(pos);
        }

        private new void PushChunksApart(int a, int b)
        {
            Vector2 a2 = Custom.DirVec(tChunks[a].pos, tChunks[b].pos);
            float num = Vector2.Distance(tChunks[a].pos, tChunks[b].pos);
            float num2 = 10f;
            if (num < num2)
            {
                tChunks[a].pos -= a2 * (num2 - num) * 0.5f;
                tChunks[a].vel -= a2 * (num2 - num) * 0.5f;
                tChunks[b].pos += a2 * (num2 - num) * 0.5f;
                tChunks[b].vel += a2 * (num2 - num) * 0.5f;
            }
        }

        public override IntVector2 GravityDirection()
        {
            return (UnityEngine.Random.value >= 0.5f) ? new IntVector2(0, -1) : new IntVector2((int)Mathf.Sign(tentacleDir.x), -1);
        }

        // Token: 0x04001EAD RID: 7853
        private DebugSprite[] grabGoalSprites;

        // Token: 0x04001EAE RID: 7854
        public int tentacleNumber;

        // Token: 0x04001EAF RID: 7855
        public int side;

        // Token: 0x04001EB0 RID: 7856
        public int pair;

        // Token: 0x04001EB1 RID: 7857
        public Vector2 tentacleDir;

        // Token: 0x04001EB2 RID: 7858
        public float maxLength;

        // Token: 0x04001EB3 RID: 7859
        public Vector2 desiredGrabPos;

        // Token: 0x04001EB4 RID: 7860
        public bool attachedAtTip;

        // Token: 0x04001EB5 RID: 7861
        public int framesWithoutReaching;

        // Token: 0x04001EB6 RID: 7862
        public int grabDelay;

        // Token: 0x04001EB7 RID: 7863
        public int setGrabDelay;

        // Token: 0x04001EB8 RID: 7864
        public int sureOfFootingCounter;

        // Token: 0x04001EB9 RID: 7865
        public float smoothedFootingSecurity;

        // Token: 0x04001EBA RID: 7866
        public bool playStepSound;

        // Token: 0x04001EBB RID: 7867
        public float stepSoundVol;
    }
}