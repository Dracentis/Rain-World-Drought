using System;
using RWCustom;
using MonoMod;
using UnityEngine;
using Noise;


[MonoModPatch("global::Player")]
class patch_Player : Player
{
    public bool bashing;
    public bool jmpDwn;
    public float energy; //varies from 0f to 1f
    public float maxEnergy = 1f;
    private static float BASH_COST = 0.4f;
    private static float RECHARGE_RATE = 0.002f;
    private static float PARRY_COST = 0.5f;
    private static int MAX_USES = 30;
    //Ending code
    public bool voidEnergy = false; //true if the void effects are controlling the maxEnergy
    public bool past22000 = false; //true if the player is in the void past -22000 y
    public bool past25000 = false; //true if the player is in the void past -25000 y
    //-------------------
    bool hibernation1 = false;
    bool hibernation2 = false;
    public int uses = MAX_USES;
    private float parry;//varies from 45f to 0f
    public WalkerBeast.PlayerInAntlers playerInAnt;
    public Player.AnimationIndex lastAnimation;

    [MonoModIgnore]
    patch_Player(AbstractCreature abstractCreature, World world) : base(abstractCreature, world) { }
    
    public extern void orig_ctor(AbstractCreature abstractCreature, World world);

    [MonoModConstructor]
    public void ctor(AbstractCreature abstractCreature, World world)
    {
        orig_ctor(abstractCreature, world);
        bashing = false;
        jmpDwn = false;
        parry = 0;
        this.lastAnimation = Player.AnimationIndex.None;
        energy = 1f;
        maxEnergy = 1f;
        uses = MAX_USES;
        hibernation1 = false;
        hibernation2 = false;
        this.pearlConversation = new PearlConversation(this);
        voidEnergy = false;
    }

    public void SwallowObject(int grasp)
    {
        if (grasp < 0 || base.grasps[grasp] == null)
        {
            return;
        }
        if (this.room.game.session is StoryGameSession && (this.room.game.session as StoryGameSession).saveState.miscWorldSaveData.moonRevived)
        {
            this.pearlConversation.PlayerSwallowItem(base.grasps[grasp].grabbed);
        }
        this.objectInStomach = base.grasps[grasp].grabbed.abstractPhysicalObject;
        this.ReleaseGrasp(grasp);
        this.objectInStomach.realizedObject.RemoveFromRoom();
        this.objectInStomach.Abstractize(base.abstractCreature.pos);
        this.objectInStomach.Room.RemoveEntity(this.objectInStomach);
        BodyChunk mainBodyChunk = base.mainBodyChunk;
        mainBodyChunk.vel.y = mainBodyChunk.vel.y + 2f;
        this.room.PlaySound(SoundID.Slugcat_Swallow_Item, base.mainBodyChunk);
    }

    public extern void orig_MovementUpdate(bool eu);

        public void MovementUpdate(bool eu)
    {


        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    patch_AbstractCreature abstractCreature = new patch_AbstractCreature(this.room.world, (patch_CreatureTemplate)StaticWorld.GetCreatureTemplate((CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake), null, base.abstractCreature.pos, this.room.game.GetNewID());
        //    abstractCreature.Room.AddEntity(abstractCreature);
        //    abstractCreature.RealizeInRoom();
        //    abstractCreature.ChangeRooms(base.abstractCreature.pos);
        //    return;
        //}
        //else if (Input.GetKeyDown(KeyCode.F))
        //{
        //    patch_AbstractCreature abstractCreature = new patch_AbstractCreature(this.room.world, (patch_CreatureTemplate)StaticWorld.GetCreatureTemplate((CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast), null, base.abstractCreature.pos, this.room.game.GetNewID());
        //    abstractCreature.Room.AddEntity(abstractCreature);
        //    abstractCreature.RealizeInRoom();
        //    abstractCreature.ChangeRooms(base.abstractCreature.pos);
        //    return;
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    //patch_AbstractCreature abstractCreature = new patch_AbstractCreature(this.room.world, (patch_CreatureTemplate)StaticWorld.GetCreatureTemplate((CreatureTemplate.Type)patch_CreatureTemplate.Type.Wolf), null, base.abstractCreature.pos, this.room.game.GetNewID());
        //    //abstractCreature.Room.AddEntity(abstractCreature);
        //    //abstractCreature.RealizeInRoom();
        //    //abstractCreature.ChangeRooms(base.abstractCreature.pos);
        //    //return;
        //}
        orig_MovementUpdate(eu);
        if (animation == AnimationIndex.DeepSwim && lastAnimation != AnimationIndex.DeepSwim)
        {
            this.room.InGameNoise(new InGameNoise(base.bodyChunks[1].pos, 350f, this, 2f));
        }
        else if (animation == AnimationIndex.SurfaceSwim && lastAnimation != AnimationIndex.SurfaceSwim)
        {
            this.room.InGameNoise(new InGameNoise(base.bodyChunks[1].pos, 350f, this, 2f));
        }
        lastAnimation = this.animation;
        bool parryed = false;
        if (uses > 0 && energy > PARRY_COST && parry <= 10 && !jmpDwn && input[0].jmp && !bashing && room.game != null && room.world != null && room.abstractRoom != null && !Malnourished)
        {
            for (int num16 = 0; num16 < room.physicalObjects.Length; num16++)
            {
                for (int num17 = 0; num17 < room.physicalObjects[num16].Count; num17++)
                {
                    for (int num18 = 0; num18 < room.physicalObjects[num16][num17].bodyChunks.Length; num18++)
                    {
                        if (!parryed && (room.physicalObjects[num16][num17] is Spear) && Math.Abs(room.physicalObjects[num16][num17].bodyChunks[num18].pos.x - mainBodyChunk.pos.x) < 200f && Math.Abs(room.physicalObjects[num16][num17].bodyChunks[num18].pos.y - mainBodyChunk.pos.y) < 200f  && (room.physicalObjects[num16][num17] as Spear).mode == Weapon.Mode.Thrown)
                        {
                            
                            parry = 45f;
                            energy -= PARRY_COST;
                            parryed = true;
                            Click();
                            uses--;
                        }
                    }
                }
            }
        }
            ///
            ///Alternative method
            ///

          /*  foreach (UpdatableAndDeletable updatableAndDeletable in this.room.updateList)
            {
                if (!parryed && (updatableAndDeletable is Spear) && Custom.DistLess(this.mainBodyChunk.pos, (updatableAndDeletable as PhysicalObject).bodyChunks[0].pos, 200f) && (updatableAndDeletable as Spear).mode == Weapon.Mode.Thrown)
                {
                    Debug.Log("Click");
                    this.parry = 45f;
                    this.energy -= PARRY_COST;
                    parryed = true;
                    Click();
                }
            }*/


        if (uses > 0 && !parryed && energy > BASH_COST && (bodyMode == BodyModeIndex.Default || bodyMode == BodyModeIndex.Stand || bodyMode == BodyModeIndex.ZeroG) && canJump <= 0 && !jmpDwn && input[0].pckp && input[0].jmp && !bashing && room.game != null && room.world != null && room.abstractRoom != null)
        {
            
                if (mushroomCounter < 40)
                {
                    mushroomCounter += 40;
                }
                bashing = true;
                jmpDwn = true;
        }


        if (!bashing && !parryed && (bodyMode == BodyModeIndex.Default || bodyMode == BodyModeIndex.Stand || bodyMode == BodyModeIndex.ZeroG) && canJump <= 0 && !jmpDwn && input[0].pckp && input[0].jmp && !bashing && room.game != null && room.world != null && room.abstractRoom != null)
        {
            room.PlaySound(SoundID.MENU_Greyed_Out_Button_Clicked, mainBodyChunk, false, 1f, 1f);
            dropCounter = 0;
        }

        if (uses > 0 && bashing && input[0].jmp && !jmpDwn)
        {
            room.PlaySound(SoundID.Moon_Wake_Up_Swarmer_Ping, mainBodyChunk, false, 1f, 1f);
            this.room.InGameNoise(new InGameNoise(base.bodyChunks[1].pos, 350f, this, 4f));
            if ((input[0].y == 0 || input[0].x == 0))
            {
                bodyChunks[0].vel.y = 7.5f * (float)input[0].y * (energy + 1);
                bodyChunks[1].vel.y = 5.5f * (float)input[0].y * (energy + 1);
                bodyChunks[0].vel.x = 7.5f * (float)input[0].x * (energy + 1);
                bodyChunks[1].vel.x = 5.5f * (float)input[0].x * (energy + 1);
            }
            else
            {
                bodyChunks[0].vel.y = 7.5f * 0.8509035f * (float)input[0].y * (energy + 1);
                bodyChunks[1].vel.y = 5.5f * 0.8509035f * (float)input[0].y * (energy + 1);
                bodyChunks[0].vel.x = 7.5f * 0.8509035f * (float)input[0].x * (energy + 1);
                bodyChunks[1].vel.x = 5.5f * 0.8509035f * (float)input[0].x * (energy + 1);
            }
            bashing = false;
            energy -= BASH_COST;
            uses--;
            if (mushroomCounter <= 40)
            {
                mushroomCounter = 0;
            }
            bashing = false;
            mushroomEffect = 0f;
        }
        if (bashing && mushroomCounter <= 0)
        {
            bashing = false;
            mushroomEffect = 0f;
        }
        if (bashing)
        {
            dropCounter = 0;
            bodyChunks[0].vel.y = 0.5f * bodyChunks[0].vel.y;
            bodyChunks[1].vel.y = 0.5f * bodyChunks[1].vel.y;
            bodyChunks[0].vel.x = 0.5f * bodyChunks[0].vel.x;
            bodyChunks[1].vel.x = 0.5f * bodyChunks[1].vel.x;
        }
        jmpDwn = input[0].jmp;
        if (!hibernation1 && uses <= 20)
        {
            if (this.playerState.foodInStomach >= 1 && this.abstractCreature.world.game.GetStorySession.saveState.totFood >= 1)
            {
                if (this.abstractCreature.world.game.IsStorySession)
                {
                    this.AddFood(-1);
                    uses += 10;
                }
            }
            else
            {
                (this.slugcatStats as patch_SlugcatStats).AddHibernationCost();
                hibernation1 = true;
            }
        }else if (!hibernation2 && uses <= 10)
        {

            if (this.playerState.foodInStomach >= 1 && this.abstractCreature.world.game.GetStorySession.saveState.totFood >= 1)
            {
                if (this.abstractCreature.world.game.IsStorySession)
                {
                    this.AddFood(-1);
                    uses += 10;
                }
            }
            else
            {
                (this.slugcatStats as patch_SlugcatStats).AddHibernationCost();
                hibernation2 = true;
            }
        }
    }

        public extern void orig_Update(bool eu);

    public void setObjectDown(bool eu)
    {
        int num8 = -1;
        for (int num9 = 0; num9 < 2; num9++)
        {
            if (base.grasps[num9] != null)
            {
                num8 = num9;
                break;
            }
        }
        if (num8 > -1)
        {
            this.ReleaseObject(num8, eu);
        }
        else if (this.spearOnBack != null && this.spearOnBack.spear != null && base.mainBodyChunk.ContactPoint.y < 0)
        {
            this.room.socialEventRecognizer.CreaturePutItemOnGround(this.spearOnBack.spear, this);
            this.spearOnBack.DropSpear();
        }
    }

    public void Update(bool eu)
    {
        if (bashing)
        {
            dropCounter = 0;
            mushroomEffect = Custom.LerpAndTick(mushroomEffect, 5f, 0.2f, 0.1f);
        } else if (dropCounter == 15) {
            setObjectDown(eu);
            dropCounter = 16;
            //Debug.Log("Drop and Count from " + (dropCounter - 1) + " to " + dropCounter);
        } else if (dropCounter > 0 && dropCounter < 30)
        {
            dropCounter++;
            //Debug.Log("Count from "+(dropCounter-1)+" to " + dropCounter);
        }

        if (!Malnourished)
        {
            if (parry > 0f)
            {
                parry--;
            }
            energy = Mathf.Clamp(energy+ RECHARGE_RATE, 0, maxEnergy);
            if (uses < 10 & !voidEnergy)
            {
                maxEnergy = Mathf.Clamp((uses+5 / 14f), BASH_COST, 1f);
            }
        }
        else
        {
            energy = 0f;
        }
        orig_Update(eu);
        if (this.Consious && !Malnourished && this.room != null && this.room.game != null && this.room.game.IsStorySession)
        {
            this.pearlConversation.Update(eu);
        }
    }

        public override Color ShortCutColor()
        {
            if ((State as PlayerState).slugcatCharacter == 1)
            {
                return new Color(0.4f, 0.49411764705f, 0.8f);
            }
                return PlayerGraphics.SlugcatColor((State as PlayerState).slugcatCharacter);
        }

    public override bool SpearStick(Weapon source, float dmg, BodyChunk chunk, PhysicalObject.Appendage.Pos onAppendagePos, Vector2 direction)
    {
        if (parry > 0f)
        {
            return false;
        }
        return true;
    }

    public override void Violence(BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, Appendage.Pos hitAppendage, DamageType type, float damage, float stunBonus)
    {
        if (type == DamageType.Stab && parry > 0f)
        {
            int num3 = (int)Math.Min(UnityEngine.Random.value + 10f, 25f);
            for (int i = 0; i < num3; i++)
            {
                room.AddObject(new Spark(source.pos + Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, source.vel * -0.1f + Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(0.2f, 0.4f, UnityEngine.Random.value) * source.vel.magnitude, new Color(1f, 1f, 1f), graphicsModule as LizardGraphics, 10, 170));
            }
            return;
        }
        base.Violence(source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
    }

    public void Click()
    {
        if (bodyChunks[1].submersion == 1f)
        {
            room.AddObject(new ShockWave(bodyChunks[0].pos, 160f * Mathf.Lerp(0.65f, 1.5f, UnityEngine.Random.value), 0.07f, 9));
        }
        else
        {
            room.AddObject(new ShockWave(bodyChunks[0].pos, 100f * Mathf.Lerp(0.65f, 1.5f, UnityEngine.Random.value), 0.07f, 6));
            for (int i = 0; i < 10; i++)
            {
                room.AddObject(new WaterDrip(bodyChunks[0].pos, Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(4f, 21f, UnityEngine.Random.value), false));
            }
        }
        room.PlaySound(SoundID.Lizard_Head_Shield_Deflect, mainBodyChunk);
    }

    public void GrabUpdate(bool eu)
    {
        if (this.spearOnBack != null)
        {
            this.spearOnBack.Update(eu);
        }
        bool flag = this.input[0].x == 0 && this.input[0].y == 0 && !this.input[0].jmp && !this.input[0].thrw && base.mainBodyChunk.submersion < 0.5f;
        bool flag2 = false;
        bool flag3 = false;
        if (this.input[0].pckp && !this.input[1].pckp && this.switchHandsProcess == 0f)
        {
            bool flag4 = base.grasps[0] != null || base.grasps[1] != null;
            if (base.grasps[0] != null && (this.Grabability(base.grasps[0].grabbed) == Player.ObjectGrabability.TwoHands || this.Grabability(base.grasps[0].grabbed) == Player.ObjectGrabability.Drag))
            {
                flag4 = false;
            }
            if (flag4)
            {
                if (this.switchHandsCounter == 0)
                {
                    this.switchHandsCounter = 15;
                }
                else
                {
                    this.room.PlaySound(SoundID.Slugcat_Switch_Hands_Init, base.mainBodyChunk);
                    this.switchHandsProcess = 0.01f;
                    this.wantToPickUp = 0;
                    this.noPickUpOnRelease = 20;
                }
            }
            else
            {
                this.switchHandsProcess = 0f;
            }
        }
        if (this.switchHandsProcess > 0f)
        {
            float num = this.switchHandsProcess;
            this.switchHandsProcess += 0.0833333358f;
            if (num < 0.5f && this.switchHandsProcess >= 0.5f)
            {
                this.room.PlaySound(SoundID.Slugcat_Switch_Hands_Complete, base.mainBodyChunk);
                base.SwitchGrasps(0, 1);
            }
            if (this.switchHandsProcess >= 1f)
            {
                this.switchHandsProcess = 0f;
            }
        }
        int num2 = -1;
        if (flag)
        {
            int num3 = -1;
            int num4 = -1;
            int num5 = 0;
            while (num3 < 0 && num5 < 2)
            {
                if (base.grasps[num5] != null && base.grasps[num5].grabbed is IPlayerEdible && (base.grasps[num5].grabbed as IPlayerEdible).Edible)
                {
                    num3 = num5;
                }
                num5++;
            }
            if ((num3 == -1 || (this.FoodInStomach >= this.MaxFoodInStomach && !(base.grasps[num3].grabbed is KarmaFlower) && !(base.grasps[num3].grabbed is Mushroom))) && (this.objectInStomach == null || this.CanPutSpearToBack))
            {
                int num6 = 0;
                while (num4 < 0 && num2 < 0 && num6 < 2)
                {
                    if (base.grasps[num6] != null)
                    {
                        if (this.CanPutSpearToBack && base.grasps[num6].grabbed is Spear)
                        {
                            num2 = num6;
                        }
                        else if (this.CanBeSwallowed(base.grasps[num6].grabbed))
                        {
                            num4 = num6;
                        }
                    }
                    num6++;
                }
            }
            if (num3 > -1 && this.noPickUpOnRelease < 1)
            {
                if (!this.input[0].pckp)
                {
                    int num7 = 1;
                    while (num7 < 10 && this.input[num7].pckp)
                    {
                        num7++;
                    }
                    if (num7 > 1 && num7 < 10)
                    {
                        this.PickupPressed();
                    }
                }
            }
            else if (this.input[0].pckp && !this.input[1].pckp)
            {
                this.PickupPressed();
            }
            if (this.input[0].pckp)
            {
                if (num2 > -1 || this.CanRetrieveSpearFromBack)
                {
                    this.spearOnBack.increment = true;
                }
                else if (num4 > -1 || this.objectInStomach != null)
                {
                    flag3 = true;
                }
            }
            if (num3 > -1 && this.wantToPickUp < 1 && (this.input[0].pckp || this.eatCounter <= 15) && base.Consious && Custom.DistLess(base.mainBodyChunk.pos, base.mainBodyChunk.lastPos, 3.6f))
            {
                if (base.graphicsModule != null)
                {
                    (base.graphicsModule as PlayerGraphics).LookAtObject(base.grasps[num3].grabbed);
                }
                flag2 = true;
                if (this.FoodInStomach < this.MaxFoodInStomach || base.grasps[num3].grabbed is KarmaFlower || base.grasps[num3].grabbed is Mushroom)
                {
                    flag3 = false;
                    if (this.spearOnBack != null)
                    {
                        this.spearOnBack.increment = false;
                    }
                    if (this.eatCounter < 1)
                    {
                        this.eatCounter = 15;
                        this.BiteEdibleObject(eu);
                    }
                }
                else if (this.eatCounter < 20 && this.room.game.cameras[0].hud != null)
                {
                    this.room.game.cameras[0].hud.foodMeter.RefuseFood();
                }
            }
        }
        else if (this.input[0].pckp && !this.input[1].pckp)
        {
            this.PickupPressed();
        }
        else
        {
            if (this.CanPutSpearToBack)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (base.grasps[i] != null && base.grasps[i].grabbed is Spear)
                    {
                        num2 = i;
                        break;
                    }
                }
            }
            if (this.input[0].pckp && (num2 > -1 || this.CanRetrieveSpearFromBack))
            {
                this.spearOnBack.increment = true;
            }
        }
        if (this.input[0].pckp && base.grasps[0] != null && base.grasps[0].grabbed is Creature && this.CanEatMeat(base.grasps[0].grabbed as Creature) && (base.grasps[0].grabbed as Creature).Template.meatPoints > 0)
        {
            this.eatMeat++;
            this.EatMeatUpdate();
            if (this.spearOnBack != null)
            {
                this.spearOnBack.increment = false;
                this.spearOnBack.interactionLocked = true;
            }
            if (this.eatMeat % 80 == 0 && ((base.grasps[0].grabbed as Creature).State.meatLeft <= 0 || this.FoodInStomach >= this.MaxFoodInStomach))
            {
                this.eatMeat = 0;
                this.wantToPickUp = 0;
                this.TossObject(0, eu);
                this.ReleaseGrasp(0);
                this.standing = true;
            }
            return;
        }
        if (!this.input[0].pckp && base.grasps[0] != null && this.eatMeat > 60)
        {
            this.eatMeat = 0;
            this.wantToPickUp = 0;
            this.TossObject(0, eu);
            this.ReleaseGrasp(0);
            this.standing = true;
            return;
        }
        this.eatMeat = Custom.IntClamp(this.eatMeat - 1, 0, 50);
        if (flag2 && this.eatCounter > 0)
        {
            this.eatCounter--;
        }
        else if (!flag2 && this.eatCounter < 40)
        {
            this.eatCounter++;
        }
        if (flag3)
        {
            this.swallowAndRegurgitateCounter++;
            if (this.objectInStomach != null && this.swallowAndRegurgitateCounter > 110)
            {
                this.Regurgitate();
                if (this.spearOnBack != null)
                {
                    this.spearOnBack.interactionLocked = true;
                }
                this.swallowAndRegurgitateCounter = 0;
            }
            else if (this.objectInStomach == null && this.swallowAndRegurgitateCounter > 90)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (base.grasps[j] != null && this.CanBeSwallowed(base.grasps[j].grabbed))
                    {
                        base.bodyChunks[0].pos += Custom.DirVec(base.grasps[j].grabbed.firstChunk.pos, base.bodyChunks[0].pos) * 2f;
                        this.SwallowObject(j);
                        if (this.spearOnBack != null)
                        {
                            this.spearOnBack.interactionLocked = true;
                        }
                        this.swallowAndRegurgitateCounter = 0;
                        (base.graphicsModule as PlayerGraphics).swallowing = 20;
                        break;
                    }
                }
            }
        }
        else
        {
            this.swallowAndRegurgitateCounter = 0;
        }
        for (int k = 0; k < base.grasps.Length; k++)
        {
            if (base.grasps[k] != null && base.grasps[k].grabbed.slatedForDeletetion)
            {
                this.ReleaseGrasp(k);
            }
        }
        if (base.grasps[0] != null && this.Grabability(base.grasps[0].grabbed) == Player.ObjectGrabability.TwoHands)
        {
            this.pickUpCandidate = null;
        }
        else
        {
            PhysicalObject physicalObject = (this.dontGrabStuff >= 1) ? null : this.PickupCandidate(20f);
            if (this.pickUpCandidate != physicalObject && physicalObject != null && physicalObject is PlayerCarryableItem)
            {
                (physicalObject as PlayerCarryableItem).Blink();
            }
            this.pickUpCandidate = physicalObject;
        }
        if (this.switchHandsCounter > 0)
        {
            this.switchHandsCounter--;
        }
        if (this.wantToPickUp > 0)
        {
            this.wantToPickUp--;
        }
        if (this.wantToThrow > 0)
        {
            this.wantToThrow--;
        }
        if (this.noPickUpOnRelease > 0)
        {
            this.noPickUpOnRelease--;
        }
        if (this.input[0].thrw && !this.input[1].thrw)
        {
            this.wantToThrow = 5;
        }
        if (this.wantToThrow > 0)
        {
            for (int l = 0; l < 2; l++)
            {
                if (base.grasps[l] != null && this.IsObjectThrowable(base.grasps[l].grabbed))
                {
                    this.ThrowObject(l, eu);
                    this.wantToThrow = 0;
                    break;
                }
            }
        }
        if (this.wantToPickUp > 0)
        {
            bool flag5 = true;
            if (this.animation == Player.AnimationIndex.DeepSwim)
            {
                if (base.grasps[0] == null && base.grasps[1] == null)
                {
                    flag5 = false;
                }
                else
                {
                    for (int m = 0; m < 10; m++)
                    {
                        if (this.input[m].y > -1 || this.input[m].x != 0)
                        {
                            flag5 = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int n = 0; n < 5; n++)
                {
                    if (this.input[n].y > -1)
                    {
                        flag5 = false;
                        break;
                    }
                }
            }
            if (base.grasps[0] != null && this.HeavyCarry(base.grasps[0].grabbed))
            {
                flag5 = true;
            }
            if (!flag5)
            {
                dropCounter = 0;
            }
            if (flag5)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (base.grasps[i] != null)
                    {
                        this.wantToPickUp = 0;
                        break;
                    }
                }
                if ((bodyMode == BodyModeIndex.Default || bodyMode == BodyModeIndex.Stand || bodyMode == BodyModeIndex.ZeroG) && canJump <= 0)
                {
                    if (dropCounter <= 0)
                    {
                        //Debug.Log("Drop Started");
                        dropCounter = 1;
                    }
                    else if (dropCounter < 16)
                    {
                        //Debug.Log("Drop pressed again: double drop!");
                        setObjectDown(eu);
                        setObjectDown(eu);
                        dropCounter = 16;
                    }
                    else if (dropCounter < 30)
                    {
                        //Debug.Log("Drop pressed again after first drop: drop!");
                        setObjectDown(eu);
                        dropCounter = 16;
                    }
                    else
                    {
                        //Debug.Log("Drop Started");
                        dropCounter = 1;
                    }
                }
                else
                {
                    setObjectDown(eu);
                    if (dropCounter > 0 && dropCounter < 16)
                    {
                        setObjectDown(eu);
                    }
                    dropCounter = 16;
                }
                if (!jmpDwn && input[0].pckp && input[0].jmp && !bashing && room.game != null && room.world != null && room.abstractRoom != null)
                {
                    dropCounter = 0;//For those speedrunners out there
                }
            }
            else if (this.pickUpCandidate != null)
            {
                if (this.pickUpCandidate is Spear && this.CanPutSpearToBack && ((base.grasps[0] != null && this.Grabability(base.grasps[0].grabbed) >= Player.ObjectGrabability.BigOneHand) || (base.grasps[1] != null && this.Grabability(base.grasps[1].grabbed) >= Player.ObjectGrabability.BigOneHand) || (base.grasps[0] != null && base.grasps[1] != null)))
                {
                    //Debug.Log("spear straight to back");
                    this.room.PlaySound(SoundID.Slugcat_Switch_Hands_Init, base.mainBodyChunk);
                    this.spearOnBack.SpearToBack(this.pickUpCandidate as Spear);
                }
                else
                {
                    int num10 = 0;
                    for (int num11 = 0; num11 < 2; num11++)
                    {
                        if (base.grasps[num11] == null)
                        {
                            num10++;
                        }
                    }
                    if (this.Grabability(this.pickUpCandidate) == Player.ObjectGrabability.TwoHands && num10 < 4)
                    {
                        for (int num12 = 0; num12 < 2; num12++)
                        {
                            if (base.grasps[num12] != null)
                            {
                                this.ReleaseGrasp(num12);
                            }
                        }
                    }
                    else if (num10 == 0)
                    {
                        for (int num13 = 0; num13 < 2; num13++)
                        {
                            if (base.grasps[num13] != null && base.grasps[num13].grabbed is Fly)
                            {
                                this.ReleaseGrasp(num13);
                                break;
                            }
                        }
                    }
                    for (int num14 = 0; num14 < 2; num14++)
                    {
                        if (base.grasps[num14] == null)
                        {
                            if (this.pickUpCandidate is Creature)
                            {
                                this.room.PlaySound(SoundID.Slugcat_Pick_Up_Creature, this.pickUpCandidate.firstChunk, false, 1f, 1f);
                            }
                            else if (this.pickUpCandidate is PlayerCarryableItem)
                            {
                                for (int num15 = 0; num15 < this.pickUpCandidate.grabbedBy.Count; num15++)
                                {
                                    this.pickUpCandidate.grabbedBy[num15].grabber.GrabbedObjectSnatched(this.pickUpCandidate.grabbedBy[num15].grabbed, this);
                                    this.pickUpCandidate.grabbedBy[num15].grabber.ReleaseGrasp(this.pickUpCandidate.grabbedBy[num15].graspUsed);
                                }
                                (this.pickUpCandidate as PlayerCarryableItem).PickedUp(this);
                            }
                            else
                            {
                                this.room.PlaySound(SoundID.Slugcat_Pick_Up_Misc_Inanimate, this.pickUpCandidate.firstChunk, false, 1f, 1f);
                            }
                            this.SlugcatGrab(this.pickUpCandidate, num14);
                            if (this.pickUpCandidate.graphicsModule != null && this.Grabability(this.pickUpCandidate) < (Player.ObjectGrabability)5)
                            {
                                this.pickUpCandidate.graphicsModule.BringSpritesToFront();
                            }
                            break;
                        }
                    }
                }
                this.wantToPickUp = 0;
            }
        }
    }

    public extern void orig_Grabbed(Creature.Grasp grasp);

    public override void Grabbed(Creature.Grasp grasp)
    {
        orig_Grabbed(grasp);
        if (grasp.grabber is Lizard || grasp.grabber is Vulture || grasp.grabber is BigSpider || grasp.grabber is DropBug || grasp.grabber is SeaDrake)
        {
            this.dangerGraspTime = 0;
            this.dangerGrasp = grasp;
        }
    }

    private void LungUpdate()
    {
        this.airInLungs = Mathf.Min(this.airInLungs, 1f - this.rainDeath);
        if (base.firstChunk.submersion > 0.9f && !this.room.game.setupValues.invincibility)
        {
            if (!this.submerged)
            {
                this.swimForce = Mathf.InverseLerp(0f, 8f, Mathf.Abs(base.firstChunk.vel.x));
                this.swimCycle = 0f;
            }
            float num = this.airInLungs;
            if (this.room.game.IsStorySession)
            {
                
                this.airInLungs -= 1f / ((((this.room.game.session as StoryGameSession).saveState.miscWorldSaveData as patch_MiscWorldSaveData).isImproved ? 3f : 1f) * 40f * ((!this.lungsExhausted) ? 9f : 4.5f) * ((this.input[0].y != 1 || this.input[0].x != 0 || this.airInLungs >= 0.333333343f) ? 1f : 1.5f) * ((float)this.room.game.setupValues.lungs / 100f)) * this.slugcatStats.lungsFac;
            }
            else
            {
                this.airInLungs -= 1f / (40f * ((!this.lungsExhausted) ? 9f : 4.5f) * ((this.input[0].y != 1 || this.input[0].x != 0 || this.airInLungs >= 0.333333343f) ? 1f : 1.5f) * ((float)this.room.game.setupValues.lungs / 100f)) * this.slugcatStats.lungsFac;
            }
            if (this.airInLungs < 0.6666667f && num >= 0.6666667f)
            {
                this.room.AddObject(new Bubble(base.firstChunk.pos, base.firstChunk.vel, false, false));
            }
            bool flag = this.airInLungs <= 0f && this.input[0].y == 1 && this.room.FloatWaterLevel(base.mainBodyChunk.pos.x) - base.mainBodyChunk.pos.y < 200f;
            if (flag)
            {
                for (int i = this.room.GetTilePosition(base.mainBodyChunk.pos).y; i <= this.room.defaultWaterLevel; i++)
                {
                    if (this.room.GetTile(this.room.GetTilePosition(base.mainBodyChunk.pos).x, i).Solid)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (this.airInLungs <= ((!flag) ? 0f : -0.3f) && base.mainBodyChunk.submersion == 1f && base.bodyChunks[1].submersion > 0.5f)
            {
                this.airInLungs = 0f;
                this.Stun(10);
                this.drown += 0.008333334f;
                if (this.drown >= 1f)
                {
                    this.Die();
                }
            }
            else if (this.airInLungs < 0.333333343f)
            {
                if (this.slowMovementStun < 1)
                {
                    this.slowMovementStun = 1;
                }
                if (UnityEngine.Random.value < 0.5f)
                {
                    base.firstChunk.vel += Custom.DegToVec(UnityEngine.Random.value * 360f) * UnityEngine.Random.value;
                }
                if (this.input[0].y < 1)
                {
                    base.bodyChunks[1].vel *= Mathf.Lerp(1f, 0.9f, Mathf.InverseLerp(0f, 0.333333343f, this.airInLungs));
                }
                if ((UnityEngine.Random.value > this.airInLungs * 2f || this.lungsExhausted) && UnityEngine.Random.value > 0.5f)
                {
                    this.room.AddObject(new Bubble(base.firstChunk.pos, base.firstChunk.vel + Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(6f, 0f, this.airInLungs), false, false));
                }
            }
            this.submerged = true;
        }
        else
        {
            if (this.submerged && this.airInLungs < 0.333333343f)
            {
                this.lungsExhausted = true;
            }
            if (!this.lungsExhausted && this.airInLungs > 0.9f)
            {
                this.airInLungs = 1f;
            }
            if (this.airInLungs <= 0f)
            {
                this.airInLungs = 0f;
            }
            this.airInLungs += 1f / (float)((!this.lungsExhausted) ? 60 : 240);
            if (this.airInLungs >= 1f)
            {
                this.airInLungs = 1f;
                this.lungsExhausted = false;
                this.drown = 0f;
            }
            this.submerged = false;
            if (this.lungsExhausted && this.animation != Player.AnimationIndex.SurfaceSwim)
            {
                this.swimCycle += 0.1f;
            }
        }
        if (this.lungsExhausted)
        {
            if (this.slowMovementStun < 5)
            {
                this.slowMovementStun = 5;
            }
            if (this.drown > 0f && this.slowMovementStun < 10)
            {
                this.slowMovementStun = 10;
            }
        }
    }
    public PearlConversation pearlConversation;

    public int dropCounter = 0;
}


