using System;
using System.Collections.Generic;
using CoralBrain;
using HUD;
using Music;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    public class FPOracleBehavior : OracleBehavior
    {
        public FPOracleBehavior(Oracle oracle) : base(oracle)
        {
            pickedUpItemsThisRealization = new List<EntityID>();
            oracle.health = 1f;
        }

        public void TurnOffSSMusic(bool abruptEnd)
        {
            Debug.Log("Fading out SS music " + abruptEnd);
            for (int i = 0; i < this.oracle.room.updateList.Count; i++)
            {
                if (this.oracle.room.updateList[i] is SSMusicTrigger)
                {
                    this.oracle.room.updateList[i].Destroy();
                    break;
                }
            }
            if (abruptEnd && this.oracle.room.game.manager.musicPlayer != null && this.oracle.room.game.manager.musicPlayer.song != null && this.oracle.room.game.manager.musicPlayer.song is SSSong)
            {
                this.oracle.room.game.manager.musicPlayer.song.FadeOut(2f);
            }
        }

        public FPOracleState State
        {
            get
            {
                if (oracle.room.game.session is StoryGameSession)
                {
                    return ((oracle.room.game.session as StoryGameSession).saveState.miscWorldSaveData as patch_MiscWorldSaveData).FPOracleState;
                }
                if (DEBUGSTATE == null)
                {
                    DEBUGSTATE = new FPOracleState(true, -1);
                }
                return DEBUGSTATE;
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            oracle.health = 1f;
            if (!oracle.Consious)
            {
                return;
            }
            if (!hasNoticedPlayer)
            {
                lookPoint = OracleGetToPos;
                if (player.room == oracle.room && oracle.room.GetTilePosition(player.mainBodyChunk.pos).y < 35)
                {
                    hasNoticedPlayer = true;
                    /*
                    if (this is FPOracleBehaviorHasMark)
                    {
                        (this as FPOracleBehaviorHasMark).workingGrav = false;
                    }*/
                    oracle.firstChunk.vel += Custom.DegToVec(45f) * 3f;
                    oracle.bodyChunks[1].vel += Custom.DegToVec(-90f) * 2f;
                }
            }
            if (holdingObject != null)
            {
                if (!oracle.Consious || holdingObject.grabbedBy.Count > 0)
                {
                    if (this is FPOracleBehaviorHasMark && holdingObject.grabbedBy.Count > 0)
                    {
                        (this as FPOracleBehaviorHasMark).PlayerInterruptByTakingItem();
                    }
                    holdingObject = null;
                }
            }
            //else
            //{
            //BodyChunk firstChunk = this.oracle.firstChunk;
            //firstChunk.vel.x = firstChunk.vel.x + ((this.oracle.firstChunk.pos.x >= this.OracleGetToPos.x) ? -1f : 1f) * 0.6f;
            //if (this.player.DangerPos.x < this.oracle.firstChunk.pos.x)
            //{
            //    if (this.oracle.firstChunk.ContactPoint.x != 0)
            //    {
            //        this.oracle.firstChunk.vel.y = Mathf.Lerp(this.oracle.firstChunk.vel.y, 1.2f, 0.5f) + 1.2f;
            //    }
            //    if (this.oracle.bodyChunks[1].ContactPoint.x != 0)
            //    {
            //        this.oracle.firstChunk.vel.y = Mathf.Lerp(this.oracle.firstChunk.vel.y, 1.2f, 0.5f) + 1.2f;
            //    }
            //}
            //}
            //if (this.oracle.arm.joints[2].pos.y < 140f)
            //{
            //    this.oracle.arm.joints[2].pos.y = 140f;
            //    this.oracle.arm.joints[2].vel.y = Mathf.Abs(this.oracle.arm.joints[1].vel.y) * 0.2f;
            //}
            //this.oracle.WeightedPush(0, 1, new Vector2(0f, 1f), 4f * Mathf.InverseLerp(60f, 20f, Mathf.Abs(this.OracleGetToPos.x - this.oracle.firstChunk.pos.x)));
        }

        public override Vector2 OracleGetToPos
        {
            get
            {
                return new Vector2(1585f, 168f);
            }
        }

        public override Vector2 GetToDir
        {
            get
            {
                if (InSitPosition)
                {
                    return new Vector2(0f, 1f);
                }
                return Custom.DirVec(oracle.firstChunk.pos, OracleGetToPos);
            }
        }

        public override bool EyesClosed
        {
            get
            {
                return false;
            }
        }

        public bool InSitPosition
        {
            get
            {
                return false;
            }
        }

        public void AirVoice(SoundID line)
        {
            if (voice != null)
            {
                if (voice.currentAudioSource != null)
                {
                    voice.currentAudioSource.Stop();
                }
                voice.Destroy();
            }
            voice = oracle.room.PlaySound(line, oracle.firstChunk);
            voice.requireActiveUpkeep = true;
        }

        public virtual void GrabObject(PhysicalObject obj)
        {
            bool flag = true;
            int num = 0;
            while (flag && num < pickedUpItemsThisRealization.Count)
            {
                if (obj.abstractPhysicalObject.ID == pickedUpItemsThisRealization[num])
                {
                    flag = false;
                }
                num++;
            }
            if (flag)
            {
                pickedUpItemsThisRealization.Add(obj.abstractPhysicalObject.ID);
            }
            if (obj.graphicsModule != null)
            {
                obj.graphicsModule.BringSpritesToFront();
            }
            if (obj is IDrawable)
            {
                for (int i = 0; i < oracle.abstractPhysicalObject.world.game.cameras.Length; i++)
                {
                    oracle.abstractPhysicalObject.world.game.cameras[i].MoveObjectToContainer(obj as IDrawable, null);
                }
            }
            holdingObject = obj;
        }

        public enum MovementBehavior
        {
            Idle,
            Meditate,
            KeepDistance,
            Investigate,
            Talk,
            ShowMedia
        }

        public bool hasNoticedPlayer;

        protected bool conversationAdded;

        public PhysicalObject holdingObject;

        public List<SoundID> painLines;

        public List<EntityID> pickedUpItemsThisRealization;

        private FPOracleState DEBUGSTATE;

        public SuperStructureFuses fuses;
    }
}
