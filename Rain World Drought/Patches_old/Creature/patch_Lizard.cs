using MonoMod;
using RWCustom;
using UnityEngine;


class patch_Lizard : Lizard
{
    [MonoModIgnore]
    public patch_Lizard(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
    }
    
    private float ActAnimation()
    {
        float num = 0f;
        switch (animation)
        {
            case Animation.Standard:
                num = AI.runSpeed;
                break;
            case Animation.HearSound:
                num = 0f;
                JawOpen = 0.2f;
                break;
            case Animation.PreyReSpotted:
                num = AI.runSpeed * 0.3f;
                JawOpen = Mathf.Lerp(JawOpen, 1f, Random.value * Random.value);
                break;
            case Animation.PreySpotted:
                num = 0f;
                JawOpen = 1f;
                break;
            case Animation.FightingStance:
                JawOpen = 1f;
                num = 0f;
                if (AI.focusCreature != null && !AI.UnpleasantFallRisk(room.GetTilePosition(mainBodyChunk.pos)) && AI.focusCreature.VisualContact)
                {
                    mainBodyChunk.vel += Custom.DirVec(mainBodyChunk.pos, AI.focusCreature.representedCreature.realizedCreature.DangerPos) * 4f * (float)LegsGripping;
                    bodyChunks[1].vel -= Custom.DirVec(mainBodyChunk.pos, AI.focusCreature.representedCreature.realizedCreature.DangerPos) * 2.2f * (float)LegsGripping;
                    bodyChunks[2].vel -= Custom.DirVec(mainBodyChunk.pos, AI.focusCreature.representedCreature.realizedCreature.DangerPos) * ((!lizardParams.WallClimber) ? 2.6f : 2.2f) * (float)LegsGripping;
                }
                break;
            case Animation.ThreatSpotted:
                num = 0.5f;
                JawOpen = Random.value;
                if (AI.focusCreature != null && Template.CreatureRelationship(AI.focusCreature.representedCreature.creatureTemplate).type == CreatureTemplate.Relationship.Type.Afraid && AI.focusCreature.VisualContact)
                {
                    mainBodyChunk.vel += Custom.DirVec(mainBodyChunk.pos, AI.focusCreature.representedCreature.realizedCreature.DangerPos) * 5f * (float)LegsGripping;
                    bodyChunks[1].vel -= Custom.DirVec(mainBodyChunk.pos, AI.focusCreature.representedCreature.realizedCreature.DangerPos) * 6f * (float)LegsGripping;
                }
                break;
            case Animation.ThreatReSpotted:
                num = AI.runSpeed * 0.8f;
                JawOpen = Mathf.Lerp(JawOpen, Random.value, Random.value);
                break;
            case Animation.ShootTongue:
                bodyWiggleCounter = 0;
                num = 0.2f;
                if (AI.behavior != LizardAI.Behavior.Hunt || !AI.focusCreature.VisualContact || Vector2.Dot(Custom.DirVec(bodyChunks[1].pos, bodyChunks[0].pos), Custom.DirVec(bodyChunks[0].pos, AI.focusCreature.representedCreature.realizedCreature.mainBodyChunk.pos)) < 0.3f)
                {
                    EnterAnimation(Animation.Standard, true);
                }
                if (timeInAnimation == timeToRemainInAnimation / 2 && AI.focusCreature != null && AI.focusCreature.representedCreature.realizedCreature != null)
                {
                    JawOpen = 1f;
                    tongue.LashOut(AI.focusCreature.representedCreature.realizedCreature.bodyChunks[Random.Range(0, AI.focusCreature.representedCreature.realizedCreature.bodyChunks.Length)].pos);
                }
                break;
            case Animation.Spit:
                num = 0f;
                bodyWiggleCounter = 0;
                JawOpen = Mathf.Clamp(JawOpen + 0.2f, 0f, 1f);
                if (!AI.redSpitAI.spitting)
                {
                    EnterAnimation(Animation.Standard, true);
                }
                else
                {
                    Vector2? vector = AI.redSpitAI.AimPos();
                    if (vector != null)
                    {
                        if (AI.redSpitAI.AtSpitPos)
                        {
                            Vector2 vector2 = room.MiddleOfTile(AI.redSpitAI.spitFromPos);
                            mainBodyChunk.vel += Vector2.ClampMagnitude(vector2 - Custom.DirVec(vector2, vector.Value) * bodyChunkConnections[0].distance - mainBodyChunk.pos, 10f) / 5f;
                            bodyChunks[1].vel += Vector2.ClampMagnitude(vector2 - bodyChunks[1].pos, 10f) / 5f;
                        }
                        if (!AI.UnpleasantFallRisk(room.GetTilePosition(mainBodyChunk.pos)))
                        {
                            mainBodyChunk.vel += Custom.DirVec(mainBodyChunk.pos, vector.Value) * 4f * (float)LegsGripping;
                            bodyChunks[1].vel -= Custom.DirVec(mainBodyChunk.pos, vector.Value) * 2f * (float)LegsGripping;
                            bodyChunks[2].vel -= Custom.DirVec(mainBodyChunk.pos, vector.Value) * 2f * (float)LegsGripping;
                        }
                        if (AI.redSpitAI.delay < 1)
                        {
                            Vector2 vector3 = bodyChunks[0].pos + Custom.DirVec(bodyChunks[1].pos, bodyChunks[0].pos) * 10f;
                            Vector2 vector4 = Custom.DirVec(vector3, vector.Value);
                            if (Vector2.Dot(vector4, Custom.DirVec(bodyChunks[1].pos, bodyChunks[0].pos)) > 0.3f && Template.type == (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard)
                            {
                                room.PlaySound(SoundID.Spear_Dislodged_From_Creature, vector3);
                                room.PlaySound(SoundID.Slugcat_Throw_Spear, vector3);
                                room.PlaySound(SoundID.Spear_Stick_In_Wall, vector3);
                                room.PlaySound(SoundID.Red_Lizard_Spit, vector3);
                                //this.room.AddObject(new LizardSpit(vector3, vector4 * 40f, this));
                                AbstractSpear abstractSpear = new AbstractSpear(room.world, null, room.GetWorldCoordinate(vector3), room.game.GetNewID(), false);
                                room.abstractRoom.AddEntity(abstractSpear);
                                abstractSpear.RealizeInRoom();
                                (((abstractSpear.realizedObject as Spear) as Weapon) as patch_Weapon).Launch(this, vector3+vector4, vector3, vector4, 0.9f, false);
                                AI.redSpitAI.delay = 98;
                                bodyChunks[2].pos -= vector4 * 8f;
                                bodyChunks[1].pos -= vector4 * 4f;
                                bodyChunks[2].vel -= vector4 * 2f;
                                bodyChunks[1].vel -= vector4 * 1f;
                                JawOpen = 1f;
                            }
                            else if (Vector2.Dot(vector4, Custom.DirVec(bodyChunks[1].pos, bodyChunks[0].pos)) > 0.3f)
                            {
                                room.PlaySound(SoundID.Red_Lizard_Spit, vector3);
                                room.AddObject(new LizardSpit(vector3, vector4 * 40f, this));
                                AI.redSpitAI.delay = 12;
                                bodyChunks[2].pos -= vector4 * 8f;
                                bodyChunks[1].pos -= vector4 * 4f;
                                bodyChunks[2].vel -= vector4 * 2f;
                                bodyChunks[1].vel -= vector4 * 1f;
                                JawOpen = 1f;
                            }
                        }
                    }
                }
                break;
            case Animation.PrepareToJump:
                if (jumpModule.actOnJump != null && Consious && Custom.DistLess(bodyChunks[1].pos, room.MiddleOfTile(jumpModule.actOnJump.bestJump.startPos), 50f))
                {
                    bodyChunks[1].vel *= 0.5f;
                    bodyChunks[1].pos = Vector2.Lerp(bodyChunks[1].pos, room.MiddleOfTile(jumpModule.actOnJump.bestJump.startPos) - jumpModule.actOnJump.bestJump.initVel.normalized * (float)timeInAnimation * 0.5f, 0.2f);
                    num = 0f;
                    bodyChunks[0].vel += jumpModule.actOnJump.bestJump.initVel.normalized * 1.5f;
                    bodyChunks[2].vel -= jumpModule.actOnJump.bestJump.initVel.normalized * 2f;
                    if (graphicsModule != null)
                    {
                        (graphicsModule as LizardGraphics).head.vel += Custom.DirVec(mainBodyChunk.pos, room.MiddleOfTile(jumpModule.actOnJump.bestJump.goalCell.worldCoordinate)) * 10f;
                        Vector2 a = Custom.PerpendicularVector(bodyChunks[0].pos, bodyChunks[2].pos);
                        for (int i = 0; i < (graphicsModule as LizardGraphics).tail.Length; i++)
                        {
                            (graphicsModule as LizardGraphics).tail[i].vel -= jumpModule.actOnJump.bestJump.initVel.normalized * (float)i + a * ((timeInAnimation % 6 >= 3) ? 5f : -5f);
                        }
                    }
                    if (timeInAnimation > timeToRemainInAnimation - 5)
                    {
                        EnterAnimation(Animation.Jumping, false);
                    }
                    inAllowedTerrainCounter = lizardParams.regainFootingCounter + 10;
                }
                else
                {
                    EnterAnimation(Animation.Standard, true);
                }
                break;
            case Animation.PrepareToLounge:
                bodyWiggleCounter = 0;
                if (!lizardParams.canExitLoungeWarmUp || (AI.behavior == LizardAI.Behavior.Hunt && AI.focusCreature != null && AI.focusCreature.VisualContact))
                {
                    num = 0f;
                    if (AI.focusCreature != null && AI.focusCreature.representedCreature.realizedCreature != null)
                    {
                        if (LegsGripping > 0)
                        {
                            foreach (BodyChunk bodyChunk in bodyChunks)
                            {
                                bodyChunk.vel += Custom.DirVec(bodyChunk.pos, AI.focusCreature.representedCreature.realizedCreature.mainBodyChunk.pos) * lizardParams.preLoungeCrouchMovement;
                            }
                        }
                        if (timeInAnimation >= lizardParams.preLoungeCrouch)
                        {
                            EnterAnimation(Animation.Lounge, false);
                        }
                    }
                    else
                    {
                        EnterAnimation(Animation.Standard, true);
                    }
                }
                else
                {
                    EnterAnimation(Animation.Standard, true);
                }
                break;
            case Animation.Lounge:
                if (timeInAnimation < lizardParams.loungeMaximumFrames && (!lizardParams.canExitLounge || (AI.behavior == LizardAI.Behavior.Hunt && AI.focusCreature != null && AI.focusCreature.VisualContact)))
                {
                    num = 0f;
                    JawOpen += 0.1f;
                    if (timeInAnimation < lizardParams.loungePropulsionFrames && LegsGripping > 0)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            bodyChunks[k].vel += loungeDir * lizardParams.loungeSpeed / (float)(k + 1);
                        }
                    }
                }
                else
                {
                    JawsSnapShut(mainBodyChunk.pos + Custom.DirVec(bodyChunks[1].pos, mainBodyChunk.pos) * 10f);
                    EnterAnimation(Animation.Standard, true);
                    postLoungeStun = 40;
                    inAllowedTerrainCounter = 0;
                }
                break;
            case Animation.ShakePrey:
                num = 0.75f;
                if (grasps[0] == null && Random.value < 0.025f)
                {
                    EnterAnimation(Animation.Standard, true);
                }
                break;
        }
        if (tongue != null && tongue.StuckToSomething)
        {
            if (Template.type == CreatureTemplate.Type.WhiteLizard)
            {
                bodyWiggleCounter += 2;
                num = 0f;
                if (!applyGravity)
                {
                    float d = 1f - (float)LegsGripping / 4f;
                    for (int l = 0; l < 3; l++)
                    {
                        bodyChunks[l].vel *= d;
                    }
                    if (LegsGripping > 0)
                    {
                        bodyChunks[2].vel += Custom.DirVec(tongue.pos, bodyChunks[2].pos) * 1.2f;
                    }
                }
            }
            else
            {
                num *= 0.6f;
            }
        }
        if (postLoungeStun > 0)
        {
            num *= 0.1f;
        }
        if ((Template.type == CreatureTemplate.Type.RedLizard|| Template.type == (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard) && AI.runSpeed > 0.1f && animation != Animation.Spit)
        {
            num = Mathf.Lerp(num, 1f, Mathf.Lerp(0.2f, 0.7f, AI.hunger));
        }
        return num;
    }
}
