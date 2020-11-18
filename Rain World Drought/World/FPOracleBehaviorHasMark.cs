using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HUD;
using RWCustom;
using UnityEngine;
using CoralBrain;
using Music;
using Rain_World_Drought.Slugcat;
using Rain_World_Drought.Enums;
using Rain_World_Drought.Resource;

namespace Rain_World_Drought.OverWorld
{
    public class FPOracleBehaviorHasMark : FPOracleBehavior, Conversation.IOwnAConversation
    {
        public FPOracleBehaviorHasMark(Oracle oracle) : base(oracle)
        {
            pearlpickupreaction = true;
            currentGetTo = oracle.firstChunk.pos;
            lastPos = oracle.firstChunk.pos;
            nextPos = oracle.firstChunk.pos;
            pathProgression = 1f;
            investigateAngle = UnityEngine.Random.value * 360f;
            movementBehavior = ((UnityEngine.Random.value >= 0.5f) ? SSOracleBehavior.MovementBehavior.Idle : SSOracleBehavior.MovementBehavior.Meditate);
            sayHelloDelay = -1;
            killFac = 0f;
            lastKillFac = 0f;
            workingGrav = true;
        }

        public bool HandTowardsPlayer()
        {
            return false;
        }

        public float working
        {
            get
            {
                return 1f - oracle.room.gravity;
            }
        }

        private Vector2 GrabPos
        {
            get
            {
                return (oracle.graphicsModule == null) ? oracle.firstChunk.pos : (oracle.graphicsModule as OracleGraphics).hands[0].pos;
            }
        }

        public DialogBox dialogBox
        {
            get
            {
                if (currentConversation != null)
                {
                    return currentConversation.dialogBox;
                }
                if (oracle.room.game.cameras[0].hud.dialogBox == null)
                {
                    oracle.room.game.cameras[0].hud.InitDialogBox();
                }
                return oracle.room.game.cameras[0].hud.dialogBox;
            }
        }

        public override Vector2 OracleGetToPos
        {
            get
            {
                if (moveToAndPickUpItem != null && moveToItemDelay > 40)
                {
                    return moveToAndPickUpItem.firstChunk.pos;
                }
                Vector2 v = currentGetTo;
                if (floatyMovement && Custom.DistLess(oracle.firstChunk.pos, nextPos, 50f))
                {
                    v = nextPos;
                }
                return ClampVectorInRoom(v);
            }
        }

        protected string NameForPlayer(bool capitalized)
        {
            string text = (capitalized) ? "Little messenger" : "little messenger";
            if (UnityEngine.Random.value > 0.3f)
            {
                switch (State.GetOpinion)
                {
                    case FPOracleState.PlayerOpinion.Dislikes:
                        text = (capitalized) ? "Little annoyance" : "little annoyance";
                        break;
                    case FPOracleState.PlayerOpinion.Likes:
                        if (UnityEngine.Random.value > 0.3f & State.totalPearlsBrought > 5)
                        {
                            text = (capitalized) ? "Little archaeologist" : "little archaeologist";
                        }
                        else
                        {
                            text = (capitalized) ? "Little helper" : "little helper";
                        }
                        break;
                }
            }
            return DroughtMod.Translate(text);
        }

        private void NewAction(Action newAction)
        {
            action = newAction;
            actionCounter = 0;
        }

        public override void Update(bool eu)
        {
            lastKillFac = killFac;
            if (talking > 0)
            {
                talking--;
            }
            base.Update(eu);
            for (int j = 0; j < oracle.room.game.cameras.Length; j++)
            {
                if (oracle.room.game.cameras[j].room == oracle.room)
                {
                    oracle.room.game.cameras[j].virtualMicrophone.volumeGroups[2] = 1f - oracle.room.gravity;
                }
            }
            if (!oracle.Consious)
            {
                return;
            }
            if (hasNoticedPlayer)
            {
                if (pearlpickupreaction)
                {
                    for (int i = 0; i < player.grasps.Length; i++)
                    {
                        if (player.grasps[i] != null && player.grasps[i].grabbed is PebblesPearl)
                        {
                            dialogBox.Interrupt(Translate("Help yourself. There not edible."), 10);
                            talking += 10;
                            pearlpickupreaction = false;
                            break;
                        }
                    }
                }
                if (intervention)
                {
                    movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
                    workingGrav = false;
                    if (playerAnnoyingCounter <= 10)
                    {
                        intervention = false;
                    }
                }
                else if ((currentConversation != null && currentConversation.id != Conversation.ID.None))
                {
                    workingGrav = false;
                    movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
                }
                else if (moveToAndPickUpItem == null)
                {
                    if (State.GetOpinion == FPOracleState.PlayerOpinion.Dislikes)
                    {
                        movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
                    }
                    else
                    {
                        movementBehavior = SSOracleBehavior.MovementBehavior.Meditate;
                    }
                }
                else
                {
                    workingGrav = true;
                    movementBehavior = SSOracleBehavior.MovementBehavior.Idle;
                }
                lookPoint = player.DangerPos;
                if (sayHelloDelay < 0)
                {
                    sayHelloDelay = 30;
                }
                else
                {
                    if (sayHelloDelay > 0)
                    {
                        sayHelloDelay--;
                    }
                    if (sayHelloDelay == 1)
                    {
                        workingGrav = false;
                        InitateConversation();
                        if (!conversationAdded && oracle.room.game.session is StoryGameSession)
                        {
                            SaveStateHK.GetFPState((oracle.room.game.session as StoryGameSession).saveState.miscWorldSaveData).playerEncounters++;
                            Debug.Log("player encounter with SS AI logged");
                            conversationAdded = true;
                        }
                    }
                }

                // Annoying pebbles must be started by hitting him with a weapon
                if (player.room == oracle.room && intervention && !Custom.DistLess(player.mainBodyChunk.lastPos, player.mainBodyChunk.pos, 1f))
                {
                    playerAnnoyingCounter++;
                }
                else
                {
                    playerAnnoyingCounter -= 2;
                }
                playerAnnoyingCounter = Custom.IntClamp(playerAnnoyingCounter, 0, 150);

                if (currentConversation != null)
                {
                    playerHoldingNeuronNoConvo = false;
                    playerIsAnnoyingWhenNoConversation = false;
                    if (currentConversation.slatedForDeletion)
                    {
                        currentConversation = null;
                    }
                    else
                    {
                        if (this.action != Action.ThrowOut_ThrowOut && this.action != Action.ThrowOut_KillOnSight && playerAnnoyingCounter > 80)
                        {
                            if (!currentConversation.paused)
                            {
                                currentConversation.paused = true;
                                if (!intervention)
                                {
                                    playerAnnoyingCounter = 150;
                                    workingGrav = false;
                                    intervention = true;
                                }
                                InterruptPlayerAnnoyingMessage();
                            }
                        }
                        else if (currentConversation.paused)
                        {
                            if (resumeConversationAfterCurrentDialoge)
                            {
                                if (dialogBox.messages.Count == 0)
                                {
                                    currentConversation.paused = false;
                                    resumeConversationAfterCurrentDialoge = false;
                                    currentConversation.RestartCurrent();
                                }
                            }
                            else if (playerAnnoyingCounter == 0)
                            {
                                resumeConversationAfterCurrentDialoge = true;
                                ResumePausedConversation();
                            }
                        }
                        currentConversation.Update();
                    }
                }
                else if (this.action != Action.ThrowOut_ThrowOut && this.action != Action.ThrowOut_KillOnSight && playerAnnoyingCounter > 80 && !playerIsAnnoyingWhenNoConversation)
                {
                    playerIsAnnoyingWhenNoConversation = true;
                    if (!intervention)
                    {
                        playerAnnoyingCounter = 150;
                        workingGrav = false;
                        intervention = true;
                    }
                    PlayerAnnoyingWhenNotTalking();
                }
                else if (playerAnnoyingCounter < 10 && playerIsAnnoyingWhenNoConversation)
                {
                    playerIsAnnoyingWhenNoConversation = false;
                    if (State.annoyances == 1)
                    {
                        dialogBox.Interrupt(DroughtMod.Translate("Are you done?"), 7);
                        talking += (int)(TEN * 0.7f);
                    }
                }

                if (holdingObject == null && moveToAndPickUpItem == null)
                {
                    for (int j = 0; j < oracle.room.socialEventRecognizer.ownedItemsOnGround.Count; j++)
                    {
                        if (WillingToInspectItem(oracle.room.socialEventRecognizer.ownedItemsOnGround[j].item))
                        {
                            bool flag2 = true;
                            for (int k = 0; k < pickedUpItemsThisRealization.Count; k++)
                            {
                                if (pickedUpItemsThisRealization[k] == oracle.room.socialEventRecognizer.ownedItemsOnGround[j].item.abstractPhysicalObject.ID)
                                {
                                    flag2 = false;
                                    break;
                                }
                            }
                            if (flag2)
                            {
                                moveToAndPickUpItem = oracle.room.socialEventRecognizer.ownedItemsOnGround[j].item;
                                if (moveToAndPickUpItem != null && moveToAndPickUpItem is DataPearl && moveToAndPickUpItem is PebblesPearl)
                                {
                                    moveToAndPickUpItem = null;
                                    break;
                                }
                                if (moveToAndPickUpItem != null && moveToAndPickUpItem is DataPearl && (moveToAndPickUpItem as DataPearl).AbstractPearl.dataPearlType == EnumExt_DroughtPlaced.WipedPearl)
                                {
                                    moveToAndPickUpItem = null;
                                    break;
                                }
                                if (currentConversation != null && currentConversation.id != Conversation.ID.MoonFirstPostMarkConversation)
                                {
                                    currentConversation.Destroy();
                                    currentConversation = null;
                                }

                                PlayerPutItemOnGround();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                workingGrav = true;
                movementBehavior = SSOracleBehavior.MovementBehavior.Meditate;
            }
            if (movementBehavior != SSOracleBehavior.MovementBehavior.Meditate)
            {
                pathProgression = Mathf.Min(1f, pathProgression + 1f / Mathf.Lerp(40f + pathProgression * 80f, Vector2.Distance(lastPos, nextPos) / 5f, 0.5f));
            }
            currentGetTo = Custom.Bezier(lastPos, ClampVectorInRoom(lastPos + lastPosHandle), nextPos, ClampVectorInRoom(nextPos + nextPosHandle), pathProgression);
            floatyMovement = false;
            investigateAngle += invstAngSpeed;
            if (moveToAndPickUpItem != null)
            {
                moveToItemDelay++;
                if (!WillingToInspectItem(moveToAndPickUpItem) || moveToAndPickUpItem.grabbedBy.Count > 0)
                {
                    moveToAndPickUpItem = null;
                }
                else if ((moveToItemDelay > 40 && Custom.DistLess(moveToAndPickUpItem.firstChunk.pos, oracle.firstChunk.pos, 40f)) || (moveToItemDelay < 20 && !Custom.DistLess(moveToAndPickUpItem.firstChunk.lastPos, moveToAndPickUpItem.firstChunk.pos, 5f) && Custom.DistLess(moveToAndPickUpItem.firstChunk.pos, oracle.firstChunk.pos, 20f)))
                {
                    GrabObject(moveToAndPickUpItem);
                    moveToAndPickUpItem = null;
                }
            }
            else
            {
                moveToItemDelay = 0;
            }
            if (holdingObject != null)
            {
                holdingObject.firstChunk.vel *= 0f;
                holdingObject.firstChunk.pos = GrabPos;
                describeItemCounter++;
                if ((currentConversation == null || !currentConversation.paused))
                {
                    lookPoint = holdingObject.firstChunk.pos + Custom.DirVec(oracle.firstChunk.pos, holdingObject.firstChunk.pos) * 100f;
                }
                if (describeItemCounter > 80 && currentConversation == null && action != Action.General_ReceiveCCPearl)
                {
                    holdingObject = null;
                    describeItemCounter = 0;
                }
            }
            else
            {
                describeItemCounter = 0;
            }

            switch (this.action)
            {
                case Action.General_Idle:
                    this.actionCounter = 0;
                    break;
                case Action.General_GiveMark:
                    this.movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
                    this.actionCounter++;
                    workingGrav = false;
                    if (this.actionCounter == 1)
                    {
                        if ((this.oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap == 9)
                        {
                            this.NewAction(Action.General_PostGiveMark);
                        }
                        else
                        {
                            this.dialogBox.Interrupt(DroughtMod.Translate("Just a minute, little messenger."), 0);
                        }
                    }
                    if (this.actionCounter > 30 && this.actionCounter < 300)
                    {
                        this.player.Stun(20);
                        this.player.mainBodyChunk.vel += Vector2.ClampMagnitude(this.oracle.room.MiddleOfTile(24, 14) - this.player.mainBodyChunk.pos, 40f) / 40f * 2.8f * Mathf.InverseLerp(30f, 160f, (float)this.actionCounter);
                    }
                    if (this.actionCounter == 30)
                    {
                        this.oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Telekenisis, 0f, 1f, 1f);
                    }
                    if (this.actionCounter == 300)
                    {
                        this.player.mainBodyChunk.vel += Custom.RNV() * 10f;
                        this.player.bodyChunks[1].vel += Custom.RNV() * 10f;
                        this.player.Stun(40);
                        (this.oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.theMark = true;
                        (this.oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap = 9;
                        (this.oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma = (this.oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap;
                        for (int l = 0; l < this.oracle.room.game.cameras.Length; l++)
                        {
                            if (this.oracle.room.game.cameras[l].hud.karmaMeter != null)
                            {
                                this.oracle.room.game.cameras[l].hud.karmaMeter.UpdateGraphic();
                            }
                        }
                        for (int m = 0; m < 20; m++)
                        {
                            this.oracle.room.AddObject(new Spark(this.player.mainBodyChunk.pos, Custom.RNV() * UnityEngine.Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                        }
                        this.oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, 0f, 1f, 1f);
                    }
                    if (this.actionCounter > 300 && this.player.graphicsModule != null)
                    {
                        (this.player.graphicsModule as PlayerGraphics).markAlpha = Mathf.Max((this.player.graphicsModule as PlayerGraphics).markAlpha, Mathf.InverseLerp(500f, 300f, (float)this.actionCounter));
                    }
                    if (this.actionCounter >= 500)
                    {
                        currentConversation = new FPOracleBehaviorHasMark.PebblesConversation(EnumExt_Drought.MoonPostMark, this, MiscItemType.NA);
                        this.NewAction(Action.General_PostGiveMark);
                    }
                    break;
                case Action.ThrowOut_ThrowOut:
                    this.playerAnnoyingCounter = 0;
                    workingGrav = true;
                    if (base.player.room == base.oracle.room)
                    {
                        this.actionCounter++;
                        base.player.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, (base.oracle.room.MiddleOfTile(28, 32))) * 0.2f * (1f - base.oracle.room.gravity) * Mathf.InverseLerp(220f, 280f, (float)actionCounter);
                        this.movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
                    }
                    else
                    {
                        this.actionCounter = 0;
                    }
                    // Do not throw out
                    //if (this.actionCounter == 700)
                    //{
                    //    this.dialogBox.Interrupt(base.Translate("That's all. You'll have to go now."), 0);
                    //}
                    //else if (this.actionCounter == 980)
                    //{
                    //    this.dialogBox.Interrupt(DroughtMod.Translate("You've helped me, and I am thankful for that, but you must go now."), 5);
                    //}
                    //else if (this.actionCounter == 1980)
                    //{
                    //    this.dialogBox.Interrupt(base.Translate("LEAVE."), 0);
                    //}
                    //else if (this.actionCounter > 2500)
                    //{
                    //    this.dialogBox.Interrupt(base.Translate("You had your chances."), 0);
                    //    this.NewAction(Action.ThrowOut_KillOnSight);
                    //}
                    break;
                case Action.General_ReceiveCCPearl:
                    workingGrav = false;
                    if (currentConversation == null && this.holdingObject != null && (this.holdingObject as DataPearl).AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.CC)
                    {
                        //this.oracle.room.AddObject(new Spark(this.holdingObject.bodyChunks[0].pos, Custom.RNV() * UnityEngine.Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                        (this.holdingObject as DataPearl).Destroy();//Five Pebbles slides the porl into his pocky. ^o^
                        this.holdingObject = null;
                        this.NewAction(Action.General_GiveMark);
                    }
                    break;
                case Action.General_PostGiveMark:
                    workingGrav = false;
                    if (currentConversation == null)
                    {
                        this.State.hasPlayerFinishedMission = true;
                        workingGrav = true;
                        this.NewAction(Action.ThrowOut_ThrowOut);
                    }
                    break;
                case Action.ThrowOut_KillOnSight:
                    workingGrav = true;
                    this.playerAnnoyingCounter = 0;
                    if ((!base.player.dead || this.killFac > 0.5f) && base.player.room == base.oracle.room)
                    {
                        this.killFac += 0.025f;
                        if (this.killFac >= 1f)
                        {
                            base.player.mainBodyChunk.vel += Custom.RNV() * 12f;
                            for (int i = 0; i < 20; i++)
                            {
                                base.oracle.room.AddObject(new Spark(base.player.mainBodyChunk.pos, Custom.RNV() * UnityEngine.Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                            }
                            base.player.Die();
                            this.killFac = 0f;
                        }
                    }
                    else
                    {
                        this.killFac *= 0.8f;
                        this.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                        if (base.player.room == base.oracle.room)
                        {
                            base.player.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(28, 32)) * 0.6f * (1f - base.oracle.room.gravity);
                            if (base.oracle.room.GetTilePosition(base.player.mainBodyChunk.pos) == new IntVector2(28, 32) && base.player.enteringShortCut == null)
                            {
                                base.player.enteringShortCut = new IntVector2?(base.oracle.room.ShortcutLeadingToNode(1).StartTile);
                            }
                        }
                        else
                        {
                            this.NewAction(FPOracleBehaviorHasMark.Action.General_Idle);
                        }
                    }
                    break;
            }
            Move();
            if (talking > 0)
            {
                lookPoint = player.DangerPos;
            }
            if (workingGrav)
            {
                if (oracle.room.gravity == 1f)
                {
                    oracle.room.PlaySound(SoundID.Broken_Anti_Gravity_Switch_On, 0f, 1f, 1f);
                }
                oracle.room.gravity = Custom.LerpAndTick(oracle.room.gravity, 0f, 0.02f, 0.01f);
            }
            else
            {
                if (oracle.room.gravity == 0f)
                {
                    oracle.room.PlaySound(SoundID.Broken_Anti_Gravity_Switch_Off, 0f, 1f, 1f);
                    TurnOffSSMusic(true);
                }
                oracle.room.gravity = Custom.LerpAndTick(oracle.room.gravity, 1f, 0.02f, 0.01f);
            }
            for (int n = 0; n < oracle.room.game.cameras.Length; n++)
            {
                if (oracle.room.game.cameras[n].room == oracle.room && !oracle.room.game.cameras[n].AboutToSwitchRoom)
                {
                    oracle.room.game.cameras[n].ChangeBothPalettes(25, 26, (1f - oracle.room.gravity) / 1.5f);
                }
            }
        }

        private void PlayerPutItemOnGround()
        {
            workingGrav = true;
            if (currentConversation != null && currentConversation.id == Conversation.ID.MoonFirstPostMarkConversation)
            {
                moveToAndPickUpItem = null;
                return;
            }
            switch (State.totalItemsBrought)
            {
                case 0:
                    dialogBox.Interrupt(DroughtMod.Translate("Oh, Something from outside?"), 10);
                    talking += TEN;
                    break;
                case 1:
                    dialogBox.Interrupt(DroughtMod.Translate("Another thing for me to inspect?"), 10);
                    talking += TEN;
                    if (State.GetOpinion != FPOracleState.PlayerOpinion.Dislikes)
                    {
                        dialogBox.NewMessage(DroughtMod.Translate("I will take a look."), 10);
                        talking += TEN;
                    }
                    break;
                case 2:
                    if (State.GetOpinion == FPOracleState.PlayerOpinion.Likes)
                    {
                        dialogBox.Interrupt(DroughtMod.Translate("What is that, <PlayerName>?"), 10);
                        talking += TEN;
                    }
                    else
                    {
                        dialogBox.Interrupt(DroughtMod.Translate("What is it, <PlayerName>?"), 10);
                        talking += TEN;
                    }
                    break;
                case 3:
                    dialogBox.Interrupt(DroughtMod.Translate("Yet another thing?"), 10);
                    talking += TEN;
                    break;
                default:
                    switch (UnityEngine.Random.Range(0, 11))
                    {
                        case 0:
                            dialogBox.Interrupt(DroughtMod.Translate("Something new you want me to investigate, <PlayerName>?"), 10);
                            talking += TEN;
                            break;
                        case 1:
                            if (State.GetOpinion == FPOracleState.PlayerOpinion.Likes)
                            {
                                dialogBox.Interrupt(DroughtMod.Translate("Another thing for me to study?"), 10);
                                talking += TEN;
                            }
                            dialogBox.NewMessage(DroughtMod.Translate("Let me see."), 10);
                            talking += TEN;
                            break;
                        case 2:
                            dialogBox.Interrupt(DroughtMod.Translate("Oh, what is that, <PlayerName>?"), 10);
                            talking += TEN;
                            break;
                        case 3:
                            if (State.GetOpinion == FPOracleState.PlayerOpinion.Likes)
                            {
                                dialogBox.Interrupt(DroughtMod.Translate("You're unusually curious, <PlayerName>!"), 10);
                                talking += TEN;
                            }
                            else
                            {
                                dialogBox.Interrupt(DroughtMod.Translate("Yet another thing?"), 10);
                                dialogBox.NewMessage(DroughtMod.Translate("Your curiosity seems expansive, <PlayerName>."), 10);
                                talking += TEN;
                            }
                            break;
                        case 4:
                            dialogBox.Interrupt(DroughtMod.Translate("Something else you want me to look at?"), 10);
                            talking += TEN;
                            break;
                        case 5:
                            if (State.GetOpinion == FPOracleState.PlayerOpinion.Likes)
                            {
                                dialogBox.Interrupt(DroughtMod.Translate("Oh... I will look at it."), 10);
                                talking += TEN;
                            }
                            else
                            {
                                dialogBox.Interrupt(DroughtMod.Translate("Something else you want me to see,<LINE>I suppose, <PlayerName>?"), 10);
                                talking += TEN;
                            }
                            break;
                        case 6:
                            if (State.GetOpinion == FPOracleState.PlayerOpinion.Likes)
                            {
                                dialogBox.Interrupt(DroughtMod.Translate("Oh... sure, I will take a look."), 10);
                                talking += TEN;
                            }
                            else
                            {
                                dialogBox.Interrupt(DroughtMod.Translate("Oh... I will take a look."), 10);
                                talking += TEN;
                            }
                            break;
                        case 7:
                            dialogBox.Interrupt(DroughtMod.Translate("Do you want me to take a look at that?"), 10);
                            talking += TEN;
                            break;
                        case 8:
                            dialogBox.Interrupt(DroughtMod.Translate("Oh... Should I look at that?"), 10);
                            talking += TEN;
                            break;
                        case 9:
                            dialogBox.Interrupt(DroughtMod.Translate("Another thing for me, <PlayerName>?"), 10);
                            talking += TEN;
                            break;
                        default:
                            dialogBox.Interrupt(DroughtMod.Translate("A new object for me, <PlayerName>?"), 10);
                            talking += TEN;
                            break;
                    }
                    break;
            }
        }

        public void PlayerInterruptByTakingItem()
        {
            if (this.currentConversation != null && this.currentConversation.id == Conversation.ID.Moon_Pearl_CC)
            {
                this.NewAction(Action.ThrowOut_KillOnSight);
                this.playerAnnoyingCounter = 0;
                dialogBox.Interrupt(DroughtMod.Translate("GIVE THAT BACK!"), 10);
                currentConversation.Destroy();
                currentConversation = null;
                workingGrav = true;
                State.totalInterruptions++;
                return;
            }
            if (State.GetOpinion == FPOracleState.PlayerOpinion.Dislikes)
            {
                if (UnityEngine.Random.value < 0.5f)
                {
                    dialogBox.Interrupt(DroughtMod.Translate("Yes, take it and don't bother me."), 10);
                    talking += TEN;
                }
                else
                {
                    dialogBox.Interrupt(DroughtMod.Translate("And now you're taking it."), 10);
                    talking += TEN;
                }
            }
            else
            {
                switch (UnityEngine.Random.Range(0, 4))
                {
                    case 0:
                        dialogBox.Interrupt(DroughtMod.Translate("Oh... Okay, have it back."), 10);
                        talking += TEN;
                        break;
                    case 1:
                        if (State.GetOpinion == FPOracleState.PlayerOpinion.Likes)
                        {
                            dialogBox.Interrupt(DroughtMod.Translate("You want it back?"), 10);
                            talking += TEN;
                        }
                        else
                        {
                            talking += TEN;
                            dialogBox.Interrupt(DroughtMod.Translate("And now you're taking it back."), 10);
                        }
                        break;
                    case 2:
                        dialogBox.Interrupt(DroughtMod.Translate("Want it back, <PlayerName>?"), 10);
                        talking += TEN;
                        break;
                    default:
                        dialogBox.Interrupt(DroughtMod.Translate("Oh..."), 10);
                        dialogBox.NewMessage(DroughtMod.Translate("Yes, you may to have it back."), 10);
                        talking += TEN;
                        break;
                }
            }
            if (currentConversation != null)
            {
                currentConversation.Destroy();
                currentConversation = null;
                workingGrav = true;
                State.totalInterruptions++;
            }
        }

        private void InitateConversation()
        {
            if (State.playerEncounters == 0)
            {
                State.InfluenceLike(1f);
                currentConversation = new FPOracleBehaviorHasMark.PebblesConversation(Conversation.ID.MoonFirstPostMarkConversation, this, MiscItemType.NA);
                workingGrav = false;
            }
            else if (State.playerEncounters == 1)
            {
                currentConversation = new FPOracleBehaviorHasMark.PebblesConversation(Conversation.ID.MoonSecondPostMarkConversation, this, MiscItemType.NA);
                workingGrav = false;
            }
            else
            {
                ThirdAndUpGreeting();
                workingGrav = false;
            }
        }

        private void ThirdAndUpGreeting()
        {
            if (State.GetOpinion == FPOracleState.PlayerOpinion.Dislikes)
            {
                switch (UnityEngine.Random.Range(0, 4))
                {
                    case 0:
                        dialogBox.Interrupt(DroughtMod.Translate("Again?"), 10);
                        talking += TEN;
                        break;
                    case 1:
                        dialogBox.Interrupt(DroughtMod.Translate("You."), 10);
                        talking += TEN;
                        break;
                    case 2:
                        dialogBox.Interrupt(DroughtMod.Translate("You again."), 10);
                        talking += TEN;
                        dialogBox.NewMessage(DroughtMod.Translate("Please stop annoying me."), 10);
                        talking += TEN;
                        break;
                    default:
                        dialogBox.Interrupt(DroughtMod.Translate("Oh, it's you, <PlayerName>."), 10);
                        talking += TEN;
                        break;
                }
            }
            else
            {
                bool flag = State.GetOpinion == FPOracleState.PlayerOpinion.Likes;
                switch (UnityEngine.Random.Range(0, 5))
                {
                    case 0:
                        dialogBox.Interrupt(DroughtMod.Translate("Hello again, <PlayerName>" + ((!flag) ? "." : "!")), 10);
                        talking += TEN;
                        break;
                    case 1:
                        dialogBox.Interrupt(DroughtMod.Translate("Hello, <PlayerName>" + ((!flag) ? "." : "!")), 10);
                        talking += TEN;
                        dialogBox.NewMessage(DroughtMod.Translate((!flag) ? "Welcome back." : "How have you been?"), 10);
                        talking += TEN;
                        break;
                    case 2:
                        dialogBox.Interrupt(DroughtMod.Translate("Oh, <PlayerName>. Hello" + ((!flag) ? "." : "!")), 10);
                        talking += TEN;
                        break;
                    case 3:
                        dialogBox.Interrupt(DroughtMod.Translate("It's you, <PlayerName>" + ((!flag) ? "." : "!") + " Hello."), 10);
                        talking += TEN;
                        break;
                    case 4:
                        dialogBox.Interrupt(DroughtMod.Translate("Ah... <PlayerName>, you're here again" + ((!flag) ? "." : "!")), 10);
                        talking += TEN;
                        break;
                    default:
                        dialogBox.Interrupt(DroughtMod.Translate("Ah... <PlayerName>, you're back" + ((!flag) ? "." : "!")), 10);
                        talking += TEN;
                        break;
                }
            }
        }

        private void InterruptPlayerAnnoyingMessage()
        {
            workingGrav = false;
            if (State.annoyances == 0)
            {
                currentConversation.Interrupt(DroughtMod.Translate("Please. Calm down."), 10);
                talking += TEN;
            }
            else if (State.annoyances == 1)
            {
                currentConversation.Interrupt(DroughtMod.Translate("Stop it!"), 10);
                talking += TEN;
            }
            else
            {
                switch (UnityEngine.Random.Range(0, 6))
                {
                    case 0:
                        currentConversation.Interrupt(DroughtMod.Translate("<CapPlayerName>! Stay still and listen."), 10);
                        talking += TEN;
                        break;
                    case 1:
                        currentConversation.Interrupt(DroughtMod.Translate("I won't let you stay here, if you continue like this."), 10);
                        talking += TEN;
                        break;
                    case 2:
                        currentConversation.Interrupt(DroughtMod.Translate("Why should I tolerate this?"), 10);
                        talking += TEN;
                        break;
                    case 3:
                        currentConversation.Interrupt(DroughtMod.Translate("STOP!"), 10);
                        talking += TEN;
                        break;
                    case 4:
                        currentConversation.Interrupt(DroughtMod.Translate("This again."), 10);
                        talking += TEN;
                        break;
                    default:
                        currentConversation.Interrupt(DroughtMod.Translate("Calm down, <PlayerName>."), 10);
                        talking += TEN;
                        break;
                }
            }
            State.InfluenceLike(-0.2f);
            State.annoyances++;
            State.totalInterruptions++;
            State.increaseLikeOnSave = false;
        }

        private void PlayerAnnoyingWhenNotTalking()
        {
            workingGrav = false;
            switch (UnityEngine.Random.Range(0, 6))
            {
                case 0:
                    dialogBox.Interrupt(DroughtMod.Translate("What are you doing?"), 10);
                    talking += TEN;
                    break;
                case 1:
                    dialogBox.Interrupt(DroughtMod.Translate("Why should I tolerate this?"), 10);
                    talking += TEN;
                    break;
                case 2:
                    dialogBox.Interrupt(DroughtMod.Translate("Down you go."), 10);
                    talking += TEN;
                    break;
                case 3:
                    dialogBox.Interrupt(DroughtMod.Translate("STOP!"), 10);
                    talking += TEN;
                    break;
                case 4:
                    dialogBox.Interrupt(DroughtMod.Translate("Calm down, <PlayerName>."), 10);
                    talking += TEN;
                    break;
                default:
                    dialogBox.Interrupt(DroughtMod.Translate("Leave me alone!"), 10);
                    talking += TEN;
                    break;
            }

            State.InfluenceLike(-0.2f);
            State.annoyances++;
            State.increaseLikeOnSave = false;
        }

        private void ResumePausedConversation()
        {
            workingGrav = false;
            if (State.annoyances < 3)
            {
                currentConversation.Interrupt(DroughtMod.Translate("Thank you."), 5);
                talking += TEN;
            }

            if (State.totalInterruptions == 1)
            {
                currentConversation.ForceAddMessage(DroughtMod.Translate("As I was saying..."), 10);
                talking += TEN;
            }
            else if (State.totalInterruptions == 2)
            {
                currentConversation.ForceAddMessage(DroughtMod.Translate("As I tried to say to you..."), 10);
                talking += TEN;
            }
            else if (State.totalInterruptions == 3)
            {
                currentConversation.ForceAddMessage(DroughtMod.Translate("Little messenger, stop that immediatly!"), 10);
                talking += TEN;
                currentConversation.ForceAddMessage(DroughtMod.Translate("Let's continue..."), 10);
                talking += TEN;
            }
            else if (State.totalInterruptions == 4)
            {
                currentConversation.ForceAddMessage(DroughtMod.Translate("And now you expect me to continue speaking?"), 10);
                talking += TEN;
                talking += TEN;
                currentConversation.ForceAddMessage(DroughtMod.Translate("Let us try again - not that it has worked well before. I was saying..."), 10);
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                switch (UnityEngine.Random.RandomRange(0, 3))
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    case 0:
                        currentConversation.ForceAddMessage(DroughtMod.Translate("As I tried to say to you..."), 10);
                        talking += TEN;
                        break;
                    case 1:
                        currentConversation.ForceAddMessage(DroughtMod.Translate("If you behave like this, why should I talk to you?"), 10);
                        talking += TEN;
                        break;
                    case 2:
                        currentConversation.ForceAddMessage(DroughtMod.Translate("You come here, but you can't be respectful enough to listen to me.<LINE>Will you listen this time?"), 0);
                        talking += TEN;
                        break;
                    case 3:
                        currentConversation.ForceAddMessage(DroughtMod.Translate("Now if you'll let me, I will try to say this again."), 0);
                        talking += TEN;
                        break;
                }
            }
        }

        public FPOracleBehaviorHasMark.MiscItemType TypeOfMiscItem(PhysicalObject testItem)
        {
            if (testItem is WaterNut || testItem is SwollenWaterNut)
            {
                return MiscItemType.WaterNut;
            }
            if (testItem is Rock)
            {
                return MiscItemType.Rock;
            }
            if (testItem is ExplosiveSpear)
            {
                return MiscItemType.FireSpear;
            }
            if (testItem is Spear)
            {
                return MiscItemType.Spear;
            }
            if (testItem is KarmaFlower)
            {
                return MiscItemType.KarmaFlower;
            }
            if (testItem is DangleFruit)
            {
                return MiscItemType.DangleFruit;
            }
            if (testItem is FlareBomb)
            {
                return MiscItemType.FlareBomb;
            }
            if (testItem is VultureMask)
            {
                return MiscItemType.VultureMask;
            }
            if (testItem is PuffBall)
            {
                return MiscItemType.PuffBall;
            }
            if (testItem is JellyFish)
            {
                return MiscItemType.JellyFish;
            }
            if (testItem is Lantern)
            {
                return MiscItemType.Lantern;
            }
            if (testItem is Mushroom)
            {
                return MiscItemType.Mushroom;
            }
            if (testItem is FirecrackerPlant)
            {
                return MiscItemType.FirecrackerPlant;
            }
            if (testItem is SlimeMold)
            {
                return MiscItemType.SlimeMold;
            }
            if (testItem is ScavengerBomb)
            {
                return MiscItemType.ScavBomb;
            }
            if (testItem is OverseerCarcass)
            {
                return MiscItemType.OverseerRemains;
            }
            if (testItem is BubbleGrass)
            {
                return MiscItemType.BubbleGrass;
            }
            if (testItem is LMOracleSwarmer)
            {
                return MiscItemType.LMOracleSwarmer;
            }
            if (testItem is SSOracleSwarmer)
            {
                return MiscItemType.SSOracleSwarmer;
            }
            return MiscItemType.NA;
        }

        public bool WillingToInspectItem(PhysicalObject item)
        {
            return holdingObject == null;
        }

        public override void GrabObject(PhysicalObject item)
        {
            base.GrabObject(item);
            if (State.HaveIAlreadyDescribedThisItem(item.abstractPhysicalObject.ID))
            {
                AlreadyDiscussedItem(item is DataPearl);
            }
            else
            {
                if (item is DataPearl)
                {
                    workingGrav = false;
                    State.increaseLikeOnSave = true;
                    State.InfluenceLike(0.1f);
                    Debug.Log((item as DataPearl).AbstractPearl.dataPearlType);
                    if ((item as DataPearl).AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Misc)
                    {
                        currentConversation = new FPOracleBehaviorHasMark.PebblesConversation(Conversation.ID.Moon_Pearl_Misc, this, MiscItemType.NA);
                    }
                    else if ((item as DataPearl).AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Misc2)
                    {
                        currentConversation = new FPOracleBehaviorHasMark.PebblesConversation(Conversation.ID.Moon_Pearl_Misc2, this, MiscItemType.NA);
                    }
                    else if ((item as DataPearl).AbstractPearl.dataPearlType == EnumExt_DroughtPlaced.MoonPearl)
                    {
                        currentConversation = new FPOracleBehaviorHasMark.PebblesConversation(Conversation.ID.Moon_Pebbles_Pearl, this, MiscItemType.NA);
                    }
                    else if (State.significantPearls[(int)(item as DataPearl).AbstractPearl.dataPearlType])
                    {
                        AlreadyDiscussedItem(true);
                    }
                    else
                    {
                        if (currentConversation != null)
                        {
                            currentConversation.Interrupt("...", 0);
                            talking += (TEN / 2);
                            currentConversation.Destroy();
                            currentConversation = null;
                        }
                        Conversation.ID id = Conversation.ID.None;
                        switch (EnumSwitch.GetAbstractDataPearlType((item as DataPearl).AbstractPearl.dataPearlType))
                        {
                            case EnumSwitch.AbstractDataPearlType.DEFAULT:
                            default:
                                switch ((item as DataPearl).AbstractPearl.dataPearlType)
                                {
                                    case DataPearl.AbstractDataPearl.DataPearlType.CC:
                                        id = Conversation.ID.Moon_Pearl_CC;
                                        State.InfluenceLike(1f);
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SI_west:
                                        id = Conversation.ID.Moon_Pearl_SI_west;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SI_top:
                                        id = Conversation.ID.Moon_Pearl_SI_top;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.LF_west:
                                        id = Conversation.ID.Moon_Pearl_LF_west;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.LF_bottom:
                                        id = Conversation.ID.Moon_Pearl_LF_bottom;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.HI:
                                        id = Conversation.ID.Moon_Pearl_HI;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SH:
                                        id = Conversation.ID.Moon_Pearl_SH;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.DS:
                                        id = Conversation.ID.Moon_Pearl_DS;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SB_filtration:
                                        id = Conversation.ID.Moon_Pearl_SB_filtration;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SB_ravine:
                                        id = Conversation.ID.Moon_Pearl_SB_ravine;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.GW:
                                        id = Conversation.ID.Moon_Pearl_GW;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SL_bridge:
                                        id = Conversation.ID.Moon_Pearl_SL_bridge;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SL_moon:
                                        id = Conversation.ID.Moon_Pearl_SL_moon;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SU:
                                        id = Conversation.ID.Moon_Pearl_SU;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.UW:
                                        id = Conversation.ID.Moon_Pearl_UW;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.SL_chimney:
                                        id = Conversation.ID.Moon_Pearl_SL_chimney;
                                        break;
                                    case DataPearl.AbstractDataPearl.DataPearlType.Red_stomach:
                                        id = Conversation.ID.Moon_Pearl_Red_stomach;
                                        break;
                                }
                                break;

                            case EnumSwitch.AbstractDataPearlType.MoonPearl:
                                id = EnumExt_Drought.Moon_Pearl_MoonPearl; break;
                            case EnumSwitch.AbstractDataPearlType.DroughtPearl1:
                                id = EnumExt_Drought.Moon_Pearl_Drought1; break;
                            case EnumSwitch.AbstractDataPearlType.DroughtPearl2:
                                id = EnumExt_Drought.Moon_Pearl_Drought2; break;
                            case EnumSwitch.AbstractDataPearlType.DroughtPearl3:
                                id = EnumExt_Drought.Moon_Pearl_Drought3; break;
                            case EnumSwitch.AbstractDataPearlType.SI_Spire1:
                                id = EnumExt_Drought.SI_Spire1; break;
                            case EnumSwitch.AbstractDataPearlType.SI_Spire2:
                                id = EnumExt_Drought.SI_Spire2; break;
                            case EnumSwitch.AbstractDataPearlType.SI_Spire3:
                                id = EnumExt_Drought.SI_Spire3; break;
                        }

                        currentConversation = new FPOracleBehaviorHasMark.PebblesConversation(id, this, MiscItemType.NA);
                        if (id == Conversation.ID.Moon_Pearl_CC)
                        {
                            this.NewAction(Action.General_ReceiveCCPearl);
                        }
                        State.significantPearls[(int)(item as DataPearl).AbstractPearl.dataPearlType] = true;
                        State.totalPearlsBrought++;
                        Debug.Log("Drought) Pearls brought up: " + State.totalPearlsBrought);
                    }
                }
                else
                {
                    workingGrav = false;
                    FPOracleBehaviorHasMark.MiscItemType miscItemType = TypeOfMiscItem(item);
                    if (miscItemType != MiscItemType.NA)
                    {
                        if (State.miscItemsDescribed[(int)miscItemType])
                        {
                            AlreadyDiscussedItem(false);
                        }
                        else
                        {
                            Conversation.ID id2 = Conversation.ID.Moon_Misc_Item;
                            currentConversation = new FPOracleBehaviorHasMark.PebblesConversation(id2, this, miscItemType);
                            State.miscItemsDescribed[(int)miscItemType] = true;
                        }
                    }
                }
                State.totalItemsBrought++;
                State.AddItemToAlreadyTalkedAbout(item.abstractPhysicalObject.ID);
            }
        }

        private void AlreadyDiscussedItem(bool pearl)
        {
            string text;
            if (pearl)
            {
                int num = UnityEngine.Random.Range(0, 3);
                switch (num)
                {
                    case 0: text = DroughtMod.Translate("I have already read this one to you, <PlayerName>."); break;
                    case 1: text = DroughtMod.Translate("This one I've already read to you, <PlayerName>."); break;
                    default: text = DroughtMod.Translate("This one, <PlayerName>?"); break;
                }
            }
            else
            {
                int num = UnityEngine.Random.Range(0, 3);
                switch (num)
                {
                    case 0: text = DroughtMod.Translate("We have already talked about this one, <PlayerName>."); break;
                    case 1: text = DroughtMod.Translate("I've told you about this one, <PlayerName>."); break;
                    default: text = DroughtMod.Translate("<CapPlayerName>, again?"); break;
                }
            }
            if (currentConversation != null)
            {
                currentConversation.Interrupt(text, 10);
            }
            else
            {
                dialogBox.Interrupt(text, 10);
                talking = TEN;
            }
        }

        public string ReplaceParts(string s)
        {
            s = Regex.Replace(s, "<PLAYERNAME>", NameForPlayer(false));
            s = Regex.Replace(s, "<CAPPLAYERNAME>", NameForPlayer(true));
            s = Regex.Replace(s, "<PlayerName>", NameForPlayer(false));
            s = Regex.Replace(s, "<CapPlayerName>", NameForPlayer(true));
            s = Regex.Replace(s, "<playername>", NameForPlayer(false));
            s = Regex.Replace(s, "<capplayername>", NameForPlayer(true));
            return s;
        }

        public override string Translate(string s)
        {
            return ReplaceParts(base.Translate(s));
        }

        public RainWorld rainWorld
        {
            get
            {
                return oracle.room.game.rainWorld;
            }
        }

        public void SpecialEvent(string eventName)
        {
        }

        public int sayHelloDelay;

        public Conversation currentConversation;

        public bool resumeConversationAfterCurrentDialoge;

        public int playerAnnoyingCounter;

        public bool playerIsAnnoyingWhenNoConversation;

        public bool playerHoldingNeuronNoConvo;

        public bool respondToNeuronFromNoSpeakMode;

        public int describeItemCounter;

        public PhysicalObject moveToAndPickUpItem;

        public int moveToItemDelay;

        public enum MiscItemType
        {
            NA,
            Rock,
            Spear,
            FireSpear,
            WaterNut,
            KarmaFlower,
            DangleFruit,
            FlareBomb,
            VultureMask,
            PuffBall,
            JellyFish,
            Lantern,
            Mushroom,
            FirecrackerPlant,
            SlimeMold,
            ScavBomb,
            BubbleGrass,
            OverseerRemains,
            LMOracleSwarmer,
            SSOracleSwarmer
        }

        private void Move()
        {
            switch (movementBehavior)
            {
                case SSOracleBehavior.MovementBehavior.Idle:
                    oracle.WeightedPush(0, 1, new Vector2(0f, 1f), 4f * Mathf.InverseLerp(60f, 20f, Mathf.Abs(OracleGetToPos.x - oracle.firstChunk.pos.x)));
                    break;
                case SSOracleBehavior.MovementBehavior.Meditate:
                    if (nextPos != oracle.room.MiddleOfTile(24, 17))
                    {
                        SetNewDestination(oracle.room.MiddleOfTile(78, 17));
                    }
                    investigateAngle = 0f;
                    lookPoint = oracle.firstChunk.pos + new Vector2(0f, -40f);
                    workingGrav = true;
                    break;
                case SSOracleBehavior.MovementBehavior.KeepDistance:
                    {
                        lookPoint = player.DangerPos;
                        Vector2 vector3 = new Vector2(UnityEngine.Random.value * oracle.room.PixelWidth, UnityEngine.Random.value * oracle.room.PixelHeight);
                        if (!oracle.room.GetTile(vector3).Solid && oracle.room.aimap.getAItile(vector3).terrainProximity > 2 && Vector2.Distance(vector3, player.DangerPos) > Vector2.Distance(nextPos, player.DangerPos) + 100f)
                        {
                            SetNewDestination(vector3);
                        }
                        break;
                    }
                case SSOracleBehavior.MovementBehavior.Investigate:
                    {
                        lookPoint = player.DangerPos;
                        if (investigateAngle < -90f || investigateAngle > 90f || (float)oracle.room.aimap.getAItile(nextPos).terrainProximity < 2f)
                        {
                            investigateAngle = Mathf.Lerp(-70f, 70f, UnityEngine.Random.value);
                            invstAngSpeed = Mathf.Lerp(0.4f, 0.8f, UnityEngine.Random.value) * ((UnityEngine.Random.value >= 0.5f) ? 1f : -1f);
                        }
                        Vector2 vector1 = player.DangerPos + Custom.DegToVec(investigateAngle) * 150f;
                        if ((float)oracle.room.aimap.getAItile(vector1).terrainProximity >= 2f)
                        {
                            if (pathProgression > 0.9f)
                            {
                                if (Custom.DistLess(oracle.firstChunk.pos, vector1, 30f))
                                {
                                    floatyMovement = true;
                                }
                                else if (!Custom.DistLess(nextPos, vector1, 30f))
                                {
                                    SetNewDestination(vector1);
                                }
                            }
                            nextPos = vector1;
                        }
                        break;
                    }
                case SSOracleBehavior.MovementBehavior.Talk:
                    {
                        lookPoint = player.DangerPos;
                        Vector2 vector2 = new Vector2(UnityEngine.Random.value * oracle.room.PixelWidth, UnityEngine.Random.value * oracle.room.PixelHeight);
                        if (CommunicatePosScore(vector2) + 40f < CommunicatePosScore(nextPos) && !Custom.DistLess(vector2, nextPos, 30f))
                        {
                            SetNewDestination(vector2);
                        }
                        break;
                    }
                case SSOracleBehavior.MovementBehavior.ShowMedia:
                    lookPoint = player.DangerPos;
                    Vector2 vector = new Vector2(UnityEngine.Random.value * oracle.room.PixelWidth, UnityEngine.Random.value * oracle.room.PixelHeight);
                    if (CommunicatePosScore(vector) + 40f < CommunicatePosScore(nextPos) && !Custom.DistLess(vector, nextPos, 30f))
                    {
                        SetNewDestination(vector);
                    }
                    consistentShowMediaPosCounter += (int)Custom.LerpMap(Vector2.Distance(showMediaPos, idealShowMediaPos), 0f, 200f, 1f, 10f);
                    vector = new Vector2(UnityEngine.Random.value * oracle.room.PixelWidth, UnityEngine.Random.value * oracle.room.PixelHeight);
                    if (ShowMediaScore(vector) + 40f < ShowMediaScore(idealShowMediaPos))
                    {
                        idealShowMediaPos = vector;
                        consistentShowMediaPosCounter = 0;
                    }
                    vector = idealShowMediaPos + Custom.RNV() * UnityEngine.Random.value * 40f;
                    if (ShowMediaScore(vector) + 20f < ShowMediaScore(idealShowMediaPos))
                    {
                        idealShowMediaPos = vector;
                        consistentShowMediaPosCounter = 0;
                    }
                    if (consistentShowMediaPosCounter > 300)
                    {
                        showMediaPos = Vector2.Lerp(showMediaPos, idealShowMediaPos, 0.1f);
                        showMediaPos = Custom.MoveTowards(showMediaPos, idealShowMediaPos, 10f);
                    }
                    break;
            }
            consistentBasePosCounter++;
            if (oracle.room.readyForAI)
            {
                Vector2 vector = new Vector2(UnityEngine.Random.value * oracle.room.PixelWidth, UnityEngine.Random.value * oracle.room.PixelHeight);
                if (!oracle.room.GetTile(vector).Solid && BasePosScore(vector) + 40f < BasePosScore(baseIdeal))
                {
                    baseIdeal = vector;
                    consistentBasePosCounter = 0;
                }
            }
            else
            {
                baseIdeal = nextPos;
            }
        }

        private float ShowMediaScore(Vector2 tryPos)
        {
            if (oracle.room.GetTile(tryPos).Solid)
            {
                return float.MaxValue;
            }
            float num = Mathf.Abs(Vector2.Distance(tryPos, player.DangerPos) - 250f);
            num -= Math.Min((float)oracle.room.aimap.getAItile(tryPos).terrainProximity, 9f) * 30f;
            num -= Vector2.Distance(tryPos, nextPos) * 0.5f;
            for (int i = 0; i < oracle.arm.joints.Length; i++)
            {
                num -= Mathf.Min(Vector2.Distance(tryPos, oracle.arm.joints[i].pos), 100f) * 10f;
            }
            if (oracle.graphicsModule != null)
            {
                for (int j = 0; j < (oracle.graphicsModule as OracleGraphics).umbCord.coord.GetLength(0); j += 3)
                {
                    num -= Mathf.Min(Vector2.Distance(tryPos, (oracle.graphicsModule as OracleGraphics).umbCord.coord[j, 0]), 100f);
                }
            }
            return num;
        }

        private float BasePosScore(Vector2 tryPos)
        {
            if (movementBehavior == SSOracleBehavior.MovementBehavior.Meditate)
            {
                return Vector2.Distance(tryPos, oracle.room.MiddleOfTile(24, 5));
            }
            if (movementBehavior == SSOracleBehavior.MovementBehavior.ShowMedia)
            {
                return -Vector2.Distance(player.DangerPos, tryPos);
            }
            float num = Mathf.Abs(Vector2.Distance(nextPos, tryPos) - 200f);
            return num + Custom.LerpMap(Vector2.Distance(player.DangerPos, tryPos), 40f, 300f, 800f, 0f);
        }

        private float CommunicatePosScore(Vector2 tryPos)
        {
            if (oracle.room.GetTile(tryPos).Solid)
            {
                return float.MaxValue;
            }
            float num = Mathf.Abs(Vector2.Distance(tryPos, player.DangerPos) - ((movementBehavior != SSOracleBehavior.MovementBehavior.Talk) ? 400f : 250f));
            num -= (float)Custom.IntClamp(oracle.room.aimap.getAItile(tryPos).terrainProximity, 0, 8) * 10f;
            if (movementBehavior == SSOracleBehavior.MovementBehavior.ShowMedia)
            {
                num += (float)(Custom.IntClamp(oracle.room.aimap.getAItile(tryPos).terrainProximity, 8, 16) - 8) * 10f;
            }
            return num;
        }

        private void SetNewDestination(Vector2 dst)
        {
            lastPos = currentGetTo;
            nextPos = dst;
            lastPosHandle = Custom.RNV() * Mathf.Lerp(0.3f, 0.65f, UnityEngine.Random.value) * Vector2.Distance(lastPos, nextPos);
            nextPosHandle = -GetToDir * Mathf.Lerp(0.3f, 0.65f, UnityEngine.Random.value) * Vector2.Distance(lastPos, nextPos);
            pathProgression = 0f;
        }

        private Vector2 ClampVectorInRoom(Vector2 v)
        {
            Vector2 result = v;
            result.x = Mathf.Clamp(result.x, oracle.arm.cornerPositions[0].x + 10f, oracle.arm.cornerPositions[1].x - 10f);
            result.y = Mathf.Clamp(result.y, oracle.arm.cornerPositions[2].y + 10f, oracle.arm.cornerPositions[1].y - 10f);
            return result;
        }

        public override Vector2 BaseGetToPos
        {
            get
            {
                return baseIdeal;
            }
        }

        public override Vector2 GetToDir
        {
            get
            {
                if (movementBehavior == SSOracleBehavior.MovementBehavior.Idle)
                {
                    return Custom.DegToVec(investigateAngle);
                }
                if (movementBehavior == SSOracleBehavior.MovementBehavior.Investigate)
                {
                    return -Custom.DegToVec(investigateAngle);
                }
                return new Vector2(0f, 1f);
            }
        }

        public override bool EyesClosed
        {
            get
            {
                return (oracle.health == 0f) || (movementBehavior == SSOracleBehavior.MovementBehavior.Meditate && talking <= 0);
            }
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

        public SSOracleBehavior.MovementBehavior movementBehavior;
        public Action action;
        public int actionCounter = 0;
        private Vector2 lastPos;
        private Vector2 nextPos;
        private Vector2 lastPosHandle;
        private Vector2 nextPosHandle;
        private Vector2 currentGetTo;
        private float pathProgression;
        private float investigateAngle;
        private float invstAngSpeed;
        private Vector2 baseIdeal;
        public bool floatyMovement;
        public float killFac;
        public float lastKillFac;
        public bool workingGrav = true;
        //VVVV MEDIA STUFF VVVV
        public ProjectedImage showImage;
        public Vector2 idealShowMediaPos;
        public Vector2 showMediaPos;
        public int consistentShowMediaPosCounter;
        public OracleChatLabel chatLabel;
        public PebblesPearl investigateMarble;
        //Other counters
        private int talking = 0;
        private bool pearlpickupreaction;
        private const int TEN = 150;
        public bool intervention = false;

        public class PebblesConversation : Conversation
        {
            public PebblesConversation(Conversation.ID id, FPOracleBehaviorHasMark FPOracleBehaviorHasMark, FPOracleBehaviorHasMark.MiscItemType describeItem) : base(FPOracleBehaviorHasMark, id, FPOracleBehaviorHasMark.dialogBox)
            {
                this.FPOracleBehaviorHasMark = FPOracleBehaviorHasMark;
                this.describeItem = describeItem;
                AddEvents();
                this.FPOracleBehaviorHasMark.workingGrav = false;
            }

            public FPOracleState State
            {
                get
                {
                    return FPOracleBehaviorHasMark.State;
                }
            }

            public string Translate(string s)
            {
                return FPOracleBehaviorHasMark.Translate(s);
            }

            public override void AddEvents()
            {
                switch (EnumSwitch.GetConversationID(id))
                {
                    default:
                    case EnumSwitch.ConversationID.DEFAULT:
                        switch (id)
                        {
                            case ID.MoonFirstPostMarkConversation:
                                if ((this.interfaceOwner as FPOracleBehaviorHasMark).rainWorld.progression.currentSaveState.miscWorldSaveData.moonRevived)
                                { // Delivered CC to Moon
                                    TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_FirstPostMarkConversation_Betrayed);
                                }
                                else
                                {
                                    TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_FirstPostMarkConversation);
                                }//LoadEventsFromFile(37);
                                break;
                            case ID.MoonSecondPostMarkConversation:
                                if ((this.interfaceOwner as FPOracleBehaviorHasMark).rainWorld.progression.currentSaveState.miscWorldSaveData.moonRevived)
                                {
                                    TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_SecondPostMarkConversation_Betrayed);
                                }
                                else
                                {
                                    if (State.GetOpinion == FPOracleState.PlayerOpinion.Dislikes)
                                    {
                                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("You again."), 10));
                                        FPOracleBehaviorHasMark.talking += TEN;
                                    }
                                    else
                                    {
                                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Hello messenger."), 10));
                                        FPOracleBehaviorHasMark.talking += TEN;
                                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Do you have that pearl Suns was talking about?"), 15));
                                        FPOracleBehaviorHasMark.talking += TEN;
                                        if (State.GetOpinion == FPOracleState.PlayerOpinion.Likes) // not dislike doesn't make sense here, logically
                                        {
                                            events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("You are welcome stay here, but I must return to my work."), 8));
                                            FPOracleBehaviorHasMark.talking += TEN;
                                        }
                                    }
                                }
                                break;
                            case ID.Moon_Pearl_Misc:
                                PearlIntro();
                                MiscPearl(false);
                                break;
                            case ID.Moon_Pearl_Misc2:
                                PearlIntro();
                                MiscPearl(true);
                                break;
                            case ID.Moon_Pebbles_Pearl: // Used for Moon's Pearl
                                MoonPearl();
                                break;
                            case ID.Moon_Pearl_CC:
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_CC);
                                break;
                            case ID.Moon_Pearl_SI_west:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SI_west);
                                break;
                            case ID.Moon_Pearl_SI_top:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SI_top);
                                break;
                            case ID.Moon_Pearl_LF_west:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_LF_west);
                                break;
                            case ID.Moon_Pearl_LF_bottom:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_LF_bottom);
                                break;
                            case ID.Moon_Pearl_HI:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_HI);
                                break;
                            case ID.Moon_Pearl_SH:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SH);
                                break;
                            case ID.Moon_Pearl_DS:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_DS);
                                break;
                            case ID.Moon_Pearl_SB_filtration:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SB_filtration);
                                break;
                            case ID.Moon_Pearl_GW:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_GW);
                                break;
                            case ID.Moon_Pearl_SL_bridge:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SL_bridge);
                                break;
                            case ID.Moon_Pearl_SL_moon:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SL_moon);
                                break;

                            case ID.Moon_Misc_Item:
                                switch (describeItem)
                                {
                                    case MiscItemType.Rock:
                                        events.Add(new Conversation.TextEvent(this, 10, DroughtMod.Translate("It's a rock. I have no use for this."), 0));
                                        break;
                                    case MiscItemType.SSOracleSwarmer:
                                        events.Add(new Conversation.TextEvent(this, 10, DroughtMod.Translate("This is one of my processing neurons, don't eat any."), 3));
                                        break;
                                    case MiscItemType.Spear:
                                        events.Add(new Conversation.TextEvent(this, 10, DroughtMod.Translate("It's a piece of sharpened rebar... What is it you want to know?<LINE>I've see you used them very effectively."), 0));
                                        break;
                                    case MiscItemType.FireSpear:
                                        events.Add(new Conversation.TextEvent(this, 10, DroughtMod.Translate("It's a weapon made with fire powder. Did the scavengers give this to you?<LINE>It could be very dangerous if used incorrectly!"), 0));
                                        break;
                                    case MiscItemType.WaterNut:
                                        events.Add(new Conversation.TextEvent(this, 10, DroughtMod.Translate("It's an edible plant. You could use the energy!"), 0));
                                        break;
                                    case MiscItemType.KarmaFlower:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_KarmaFlower); break;
                                    case MiscItemType.LMOracleSwarmer:
                                        events.Add(new Conversation.TextEvent(this, 10, DroughtMod.Translate("Is this one of Moon's neurons?"), 3));
                                        events.Add(new Conversation.TextEvent(this, 10, DroughtMod.Translate("Why bring it to me? I already have plenty."), 3));
                                        break;
                                    case MiscItemType.DangleFruit:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_DangleFruit); break;
                                    case MiscItemType.FlareBomb:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_FlareBomb); break;
                                    case MiscItemType.VultureMask:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_VultureMask); break;
                                    case MiscItemType.PuffBall:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_PuffBall); break;
                                    case MiscItemType.JellyFish:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_JellyFish); break;
                                    case MiscItemType.Lantern:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Lantern); break;
                                    case MiscItemType.Mushroom:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Mushroom); break;
                                    case MiscItemType.FirecrackerPlant:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_FirecrackerPlant); break;
                                    case MiscItemType.SlimeMold:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_SlimeMold); break;
                                    case MiscItemType.ScavBomb:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_ScavBomb); break;
                                    case MiscItemType.BubbleGrass:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_BubbleGrass); break;
                                    case MiscItemType.OverseerRemains:
                                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_OverseerRemains); break;
                                }
                                break;
                            case ID.Moon_Pearl_SU:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SU); break;
                            case ID.Moon_Pearl_SB_ravine:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SB_ravine); break;
                            case ID.Moon_Pearl_UW:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_UW); break;
                            case ID.Moon_Pearl_SL_chimney:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_SL_chimney); break;
                            case ID.Moon_Pearl_Red_stomach:
                                PearlIntro();
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_Red_stomach); break;
                            case ID.Moon_Red_First_Conversation:
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Red_First_Conversation); break;
                            case ID.Moon_Red_Second_Conversation:
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Red_Second_Conversation); break;
                            case ID.Moon_Yellow_First_Conversation:
                                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Yellow_First_Conversation); break;
                        }
                        break;
                    case EnumSwitch.ConversationID.Moon_Pearl_MoonPearl:
                        break;
                    case EnumSwitch.ConversationID.MoonPostMark:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Go to the west past the Farm Arrays, and then down into the<LINE>earth where the land fissures, as deep as you can reach, where the ancients<LINE>built their temples and danced their silly rituals. The mark I gave you will let you through."), 25));
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Best of luck to you, <PlayerName>."), 5)); // cuz [Good luck] was dupe of Moon's dialogue, which confuses translator
                        break;
                    case EnumSwitch.ConversationID.Moon_Pearl_Drought1: // IS
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_Drought1_IS);
                        break;
                    case EnumSwitch.ConversationID.Moon_Pearl_Drought2: // FS
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_Drought2_FS);
                        break;
                    case EnumSwitch.ConversationID.Moon_Pearl_Drought3: // MW
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_Moon_Pearl_Drought3_MW);
                        break;
                    case EnumSwitch.ConversationID.SI_Spire1:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_SI_Spire1); break;
                    case EnumSwitch.ConversationID.SI_Spire2:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_SI_Spire2); break;
                    case EnumSwitch.ConversationID.SI_Spire3:
                        TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_SI_Spire3); break;
                }
            }

            private void PearlIntro()
            {
                switch (State.totalPearlsBrought + State.miscPearlCounter)
                {
                    case 0:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Ah, a pearl. Not a message though. Would you like me to read it to you?"), 10));
                        break;
                    case 1:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Another pearl! You want me to read this one too? Just a moment..."), 10));
                        break;
                    case 2:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("And yet another one! I will read it to you."), 10));
                        break;
                    case 3:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Another? You're no better than the scavengers!"), 10));
                        if (State.GetOpinion == FPOracleState.PlayerOpinion.Likes)
                        {
                            events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Let us see... to be honest, I'm as curious to see it as you are."), 10));
                        }
                        break;
                    default:
                        switch (UnityEngine.Random.Range(0, 5))
                        {
                            case 0:
                                break;
                            case 1:
                                events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("The scavengers must be jealous of you, finding all these"), 10));
                                break;
                            case 2:
                                events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Here we go again, little archeologist. Let's read your pearl."), 10));
                                break;
                            case 3:
                                events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("... You're getting quite good at this you know. A little archeologist beast.<LINE>Now, let's see what it says."), 10));
                                break;
                            default:
                                events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("And yet another one! I will read it to you."), 10));
                                break;
                        }
                        break;
                }
            }

            private void MiscPearl(bool miscPearl2)
            {
                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_MiscPearl, true, (FPOracleBehaviorHasMark.holdingObject == null) ? UnityEngine.Random.Range(0, 100000) : FPOracleBehaviorHasMark.holdingObject.abstractPhysicalObject.ID.RandomSeed);
                State.miscPearlCounter++;
            }

            private void MoonPearl()
            {
                switch (UnityEngine.Random.Range(0, 5))
                { // Use Drought Translator to allow different nuance than LTTM
                    case 0:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("You would like me to read this?"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("This pearl must have been used recently. You can still feel the heat."), 10));
                        break;
                    case 1:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("A pearl... This one is crystal clear - it was used recently."), 10));
                        break;
                    case 2:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Would you like me to read this pearl?"), 10));
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("It has been recently written to."), 10));
                        break;
                    case 3:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("This pearl has been written to just now!"), 10));
                        break;
                    default:
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("Let's see... A pearl..."), 10));
                        events.Add(new Conversation.TextEvent(this, 0, DroughtMod.Translate("And this one is sharp! It was not long ago this data was written to it!"), 10));
                        break;
                }
                TextManager.LoadEventsFromFile(this, TextManager.EventID.FP_MoonPearl, true, (FPOracleBehaviorHasMark.holdingObject == null) ? UnityEngine.Random.Range(0, 100000) : FPOracleBehaviorHasMark.holdingObject.abstractPhysicalObject.ID.RandomSeed);
            }

            public int GetARandomChatLog(bool whichPearl)
            {
                List<int> list = new List<int>
            {
                0,
                1,
                2,
                3,
                4
            };
                int seed = UnityEngine.Random.seed;
                UnityEngine.Random.seed = (FPOracleBehaviorHasMark.oracle.room.game.session as StoryGameSession).saveState.seed;
                int num = list[UnityEngine.Random.Range(0, list.Count)];
                list.Remove(num);
                int num2 = list[UnityEngine.Random.Range(0, list.Count)];
                UnityEngine.Random.seed = seed;
                if (whichPearl)
                {
                    return 20 + num;
                }
                return 20 + num2;
            }

            public void Update()
            {
                if (this.paused)
                {
                    this.FPOracleBehaviorHasMark.workingGrav = false;
                    return;
                }
                if (this.events.Count == 0)
                {
                    this.FPOracleBehaviorHasMark.workingGrav = true;
                    this.Destroy();
                }
                else
                {
                    this.events[0].Update();
                    this.FPOracleBehaviorHasMark.workingGrav = false;
                    if (this.events[0].IsOver)
                    {
                        this.events.RemoveAt(0);
                    }
                }
            }

            public FPOracleBehaviorHasMark FPOracleBehaviorHasMark;

            public FPOracleBehaviorHasMark.MiscItemType describeItem;
        }

        public enum Action
        {
            General_Idle,
            General_GiveMark,
            General_PostGiveMark,
            General_ReceiveCCPearl,
            ThrowOut_ThrowOut,
            ThrowOut_KillOnSight,
            Interrupted
        }
    }
}
