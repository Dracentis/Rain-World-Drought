using System;
using System.Collections.Generic;
using CoralBrain;
using HUD;
using Music;
using RWCustom;
using UnityEngine;

public class LMOracleBehavior : OracleBehavior
{
    public LMOracleBehavior(Oracle oracle) : base(oracle)
    {
        pickedUpItemsThisRealization = new List<EntityID>();
        painLines = new List<SoundID>();
        painLines.Add(SoundID.SL_AI_Pain_1);
        painLines.Add(SoundID.SL_AI_Pain_2);
        if (oracle.room.game.IsStorySession && oracle.room.game.GetStorySession.saveStateNumber == 2 && State.neuronsLeft < 1)
        {
            oracle.room.AddObject(new SLOracleWakeUpProcedure(oracle));
        }
        oracle.health = Mathf.InverseLerp(0f, 5f, (float)State.neuronsLeft);
        for (int i = 0; i < oracle.room.updateList.Count; i++)
        {
            if (oracle.room.updateList[i] is SuperStructureFuses)
            {
                fuses = (oracle.room.updateList[i] as SuperStructureFuses);
                fuses.power = 0f;
                fuses.powerFlicker = 0f;
                break;
            }
        }
    }
    
    public SLOrcacleState State
    {
        get
        {
            if (oracle.room.game.session is StoryGameSession)
            {
                return (oracle.room.game.session as StoryGameSession).saveState.miscWorldSaveData.SLOracleState;
            }
            if (DEBUGSTATE == null)
            {
                DEBUGSTATE = new SLOrcacleState(true, -1);
            }
            return DEBUGSTATE;
        }
    }
    
    public override void Update(bool eu)
    {
        base.Update(eu);
        oracle.health = Mathf.InverseLerp(0f, 5f, (float)State.neuronsLeft);
        if (!oracle.Consious)
        {
            return;
        }
        if (!hasNoticedPlayer)
        {
            lookPoint = OracleGetToPos;
            if (player.room == oracle.room && oracle.room.GetTilePosition(player.mainBodyChunk.pos).y < 25)
            {
                hasNoticedPlayer = true;
                oracle.firstChunk.vel += Custom.DegToVec(45f) * 3f;
                oracle.bodyChunks[1].vel += Custom.DegToVec(-90f) * 2f;
            }
        }
        if (holdingObject != null)
        {
            if (!oracle.Consious || holdingObject.grabbedBy.Count > 0)
            {
                if (this is LMOracleBehaviorHasMark && holdingObject.grabbedBy.Count > 0)
                {
                    (this as LMOracleBehaviorHasMark).PlayerInterruptByTakingItem();
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
            return (oracle.health == 0f);
        }
    }
    
    public bool InSitPosition
    {
        get
        {
            return false;
        }
    }
    
    public virtual void Pain()
    {
        if ((painLines.Count > 0 && UnityEngine.Random.value < 0.333333343f) || painLines.Count >= 2)
        {
            AirVoice(painLines[0]);
            painLines.RemoveAt(0);
        }
    }
    
    public virtual void ConvertingSSSwarmer()
    {
        State.neuronsLeft++;
        Debug.Log("Converting an SS swarmer, " + State.neuronsLeft);
        State.InfluenceLike(0.65f);
        if (oracle.room.game.session is StoryGameSession)
        {
            (oracle.room.game.session as StoryGameSession).saveState.miscWorldSaveData.playerGuideState.angryWithPlayer = false;
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
        voice.requireActiveUpkeep = (line != SoundID.SL_AI_Pain_1 && line != SoundID.SL_AI_Pain_2);
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
        // Token: 0x040025A7 RID: 9639
        Idle,
        // Token: 0x040025A8 RID: 9640
        Meditate,
        // Token: 0x040025A9 RID: 9641
        KeepDistance,
        // Token: 0x040025AA RID: 9642
        Investigate,
        // Token: 0x040025AB RID: 9643
        Talk,
        // Token: 0x040025AC RID: 9644
        ShowMedia
    }

    public bool hasNoticedPlayer;
    
    public bool protest;
    
    public float protestCounter;
    
    public bool armsProtest;
    
    protected bool conversationAdded;
    
    public PhysicalObject holdingObject;
    
    public List<SoundID> painLines;
    
    public List<EntityID> pickedUpItemsThisRealization;
    
    private SLOrcacleState DEBUGSTATE;

    public SuperStructureFuses fuses;
    
}
