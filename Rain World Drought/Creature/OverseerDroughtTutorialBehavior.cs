using System;
using System.Collections.Generic;
using OverseerHolograms;
using Rain_World_Drought.Slugcat;
using RWCustom;
using ScavTradeInstruction;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    public class OverseerDroughtTutorialBehavior : AIModule
    {
        public OverseerDroughtTutorialBehavior(OverseerAI AI) : base(AI)
        {
            this.encounterCounter = 40;
        }

        public OverseerAI overseerAI
        {
            get
            {
                return this.AI as OverseerAI;
            }
        }

        public Overseer overseer
        {
            get
            {
                return (this.AI as OverseerAI).overseer;
            }
        }

        public Room room
        {
            get
            {
                return this.overseerAI.overseer.room;
            }
        }

        public Player player
        {
            get
            {
                return this.room.game.Players[0].realizedCreature as Player;
            }
        }

        public override void Update()
        {
            if (this.room == null || this.playerHasMadeSuperJump)
            {
                return;
            }
            /*
            if (this.encounterCounter > 0)
            {
                this.overseerAI.lookAt = new Vector2(480f, 540f);
                this.overseerAI.bringUpLens = 0f;
                this.overseerAI.randomBringUpLensBonus = -100f;
                if (this.overseer.rootTile.x < 120)
                {
                    this.overseer.rootTile.x = 137;
                    for (int i = this.overseer.rootTile.y; i >= 0; i--)
                    {
                        if (this.room.GetTile(this.overseer.rootTile.x, i).Solid)
                        {
                            this.overseer.rootTile.y = i;
                            break;
                        }
                    }
                    this.overseer.rootPos = this.overseer.room.MiddleOfTile(this.overseer.rootPos);
                    this.overseer.hoverTile = this.overseer.rootTile;
                    this.overseer.nextHoverTile = this.overseer.rootTile;
                    this.overseerAI.ResetZipPathingMatrix(this.overseer.rootTile);
                }
            }
            */
            if (this.room.game.Players[0].realizedCreature == null)
            {
                return;
            }
            if (this.room.game.Players[0].realizedCreature.room != this.room)
            {
                return;
            }
            /*
            if (this.encounterCounter == 1)
            {
                this.overseerAI.lookAt = this.player.mainBodyChunk.pos;
                this.overseerAI.tempHoverTile = this.room.GetTilePosition(this.player.mainBodyChunk.pos + Custom.DirVec(this.player.mainBodyChunk.pos, this.overseer.rootPos) * 1600f);
            }
            */
            string name = this.room.abstractRoom.name;

            switch (name)
            {
                case "FS_A01":
                    if (!displayBashInstructions)
                    {
                        this.TutorialText(DroughtMod.Translate("While on the ground, tap MAP and then press JUMP to slow down time."), 200, 500, true);
                        this.TutorialText(DroughtMod.Translate("Then hold a DIRECTION and press JUMP to finish the boost."), 0, 270, true);
                        this.overseer.TryAddHologram(OverseerHologram.Message.SuperJump, this.player, float.MaxValue);
                        displayBashInstructions = true;
                    }
                    if (this.player.mainBodyChunk.pos.x > jumpThreshold)
                    {
                        this.superJumpTrouble = 0;
                    }
                    else if (this.player.mainBodyChunk.pos.x < jumpThreshold)
                    {
                        this.superJumpTrouble++;
                    }
                    if (this.superJumpTrouble > 2000)
                    {
                        this.TutorialText(DroughtMod.Translate("While in the air, hold GRAB and then press JUMP to start boosting."), 800, 500, true);
                        this.TutorialText(DroughtMod.Translate("Then hold a DIRECTION and press JUMP to finish the boost."), 0, 270, true);
                        this.overseer.TryAddHologram(OverseerHologram.Message.SuperJump, this.player, float.MaxValue);
                        this.superJumpTrouble = 0;
                        this.encounterCounter++;
                    }
                    if (this.player.mainBodyChunk.pos.x > jumpThreshold)
                    {
                        this.playerHasMadeSuperJump = true;
                        Debug.Log("Drought) Yay! Superjump done!");
                    }

                    WandererSupplement sub = WandererSupplement.GetSub(player);
                    if(sub.energy == 0)
                        sub.energy = WandererSupplement.maxEnergy;
                    break;
            }
        }

        public void TutorialText(string text, int wait, int time, bool hideHud)
        {
            this.room.game.cameras[0].hud.textPrompt.AddMessage(this.overseer.room.game.rainWorld.inGameTranslator.Translate(text), wait, time, true, hideHud);
        }

        public int encounterCounter;

        public bool playerHasMadeSuperJump;

        public int superJumpTrouble;

        public static float jumpThreshold = 800f;

        public bool displayBashInstructions;

        public int superJumpAttemptTime;

        public abstract class InputInstruction : OverseerHologram
        {
            public InputInstruction(Overseer overseer, OverseerHologram.Message message, Creature communicateWith, float importance) : base(overseer, message, communicateWith, importance)
            {
                if (overseer.AI.tutorialBehavior == null && message != OverseerHologram.Message.InWorldSuperJump && message != OverseerHologram.Message.PickupObject && message != OverseerHologram.Message.ScavengerTrade)
                {
                    this.stillRelevant = false;
                }
                else
                {
                    switch (message)
                    {
                        case OverseerHologram.Message.SuperJump:
                            this.controller = new OverseerDroughtTutorialBehavior.SuperJump(this, OverseerHK.droughtTutorialBehavior, true);
                            break;
                    }
                }
            }

            public override void Update(bool eu)
            {
                base.Update(eu);
                if (this.controller != null)
                {
                    this.controller.Update();
                }
            }

            /*
            public override float DisplayPosScore(IntVector2 testPos)
            {
                float num = base.DisplayPosScore(testPos);
                num += Mathf.Abs(200f - Vector2.Distance(this.room.MiddleOfTile(testPos), this.communicateWith.DangerPos)) / 2f;
                return num + Vector2.Distance(this.room.MiddleOfTile(testPos), this.closeToPos);
            }
            */

            public IntVector2 direction;

            public bool showPickup;

            public bool showJump;

            public bool showThrow;

            public bool directionOnly;

            public bool hideInputs;

            public Vector2 closeToPos;

            public OverseerDroughtTutorialBehavior.InputInstruction.InputInstructionController controller;

            public OverseerHologram.PlayerGhost playerGhost;

            public abstract class InputInstructionController
            {
                public InputInstructionController(OverseerDroughtTutorialBehavior.InputInstruction instructionHologram, OverseerDroughtTutorialBehavior tutBehavior)
                {
                    this.instructionHologram = instructionHologram;
                    this.tutBehavior = tutBehavior;
                }

                public Player player
                {
                    get
                    {
                        return this.instructionHologram.communicateWith as Player;
                    }
                }

                public Room room
                {
                    get
                    {
                        return this.instructionHologram.room;
                    }
                }

                public Overseer overseer
                {
                    get
                    {
                        return this.instructionHologram.overseer;
                    }
                }

                public virtual void Update()
                {
                }

                public OverseerDroughtTutorialBehavior.InputInstruction instructionHologram;

                public OverseerDroughtTutorialBehavior tutBehavior;
            }
        }

        public class SuperJump : OverseerDroughtTutorialBehavior.InputInstruction.InputInstructionController
        {
            public SuperJump(OverseerDroughtTutorialBehavior.InputInstruction instructionHologram, OverseerDroughtTutorialBehavior tutBehavior, bool inTuturialSection) : base(instructionHologram, tutBehavior)
            {
                this.inTuturialSection = inTuturialSection;
            }

            public override void Update()
            {
                base.Update();
                if (this.instructionHologram is OverseerDroughtTutorialBehavior.KeyBoardInstruction)
                {
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.KeyBoardInstruction).PickUp.timeUp = 10;
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.KeyBoardInstruction).PickUp.timeDown = 40;
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.KeyBoardInstruction).Jump.timeUp = 15;
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.KeyBoardInstruction).Jump.timeDown = 10;
                }
                else if (this.instructionHologram is OverseerDroughtTutorialBehavior.GamePadInstruction)
                {
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.GamePadInstruction).PickUp.timeUp = 10;
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.GamePadInstruction).PickUp.timeDown = 40;
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.GamePadInstruction).Jump.timeUp = 15;
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.GamePadInstruction).Jump.timeDown = 10;
                    (this.instructionHologram as OverseerDroughtTutorialBehavior.GamePadInstruction).partsRemainVisible[2] = Math.Max((this.instructionHologram as OverseerDroughtTutorialBehavior.GamePadInstruction).partsRemainVisible[2], 5);
                }
                if (this.inTuturialSection)
                {
                    this.instructionHologram.stillRelevant = (this.instructionHologram.stillRelevant && !this.tutBehavior.playerHasMadeSuperJump && OverseerDroughtTutorialBehavior.SuperJump.InZone(base.player.bodyChunks[1].pos));
                    if (base.player.bodyChunks[1].pos.x > jumpThreshold)
                    {
                        Debug.Log("Drought) Yay! Superjump done!");
                        this.tutBehavior.playerHasMadeSuperJump = true;
                    }
                }
                this.instructionHologram.direction = new IntVector2(1, 1);
            }

            public static bool InZone(Vector2 testPos)
            {
                return testPos.x < jumpThreshold;
            }

            public bool inTuturialSection;
        }

        public abstract class ButtonOrKey : OverseerHologram.HologramPart
        {
            public ButtonOrKey(OverseerHologram hologram, int firstSprite) : base(hologram, firstSprite)
            {
                this.timeUp = 10;
                this.timeDown = 10;
                this.symbolSprite = this.totalSprites;
                this.totalSprites++;
            }

            public override Color GetToColor
            {
                get
                {
                    if (this.down)
                    {
                        return new Color(1f, 1f, 1f);
                    }
                    return base.GetToColor;
                }
            }

            public override void Update()
            {
                base.Update();
                if (this.pulsate)
                {
                    this.counter--;
                    if (this.counter < 1)
                    {
                        this.down = !this.down;
                        this.counter = ((!this.down) ? this.timeUp : this.timeDown);
                    }
                }
                else
                {
                    this.down = false;
                    this.counter = 0;
                }
                this.transform = Custom.LerpAndTick(this.transform, (!this.down) ? 0f : 1f, 0.14f, 0.125f);
            }

            public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                base.InitiateSprites(sLeaser, rCam);
                sLeaser.sprites[this.firstSprite + this.symbolSprite] = new FSprite(((!(this is OverseerDroughtTutorialBehavior.KeyBoardKey)) ? "button" : "key") + this.symbol + "A", true);
                sLeaser.sprites[this.firstSprite + this.symbolSprite].rotation = this.symbolRotation;
            }

            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, Vector2 partPos, Vector2 headPos, float useFade, float popOut, Color useColor)
            {
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos, partPos, headPos, useFade, popOut, useColor);
                if (UnityEngine.Random.value > Mathf.InverseLerp(0.5f, 1f, useFade))
                {
                    sLeaser.sprites[this.firstSprite + this.symbolSprite].isVisible = false;
                    return;
                }
                sLeaser.sprites[this.firstSprite + this.symbolSprite].isVisible = true;
                Vector3 vector = Custom.Vec3FromVec2(partPos, -5f + 10f * Mathf.Lerp(this.lastTransform, this.transform, timeStacker));
                vector = Vector3.Lerp(headPos, vector, popOut);
                vector = this.hologram.ApplyDepthOnVector(vector, rCam, camPos);
                sLeaser.sprites[this.firstSprite + this.symbolSprite].x = vector.x;
                sLeaser.sprites[this.firstSprite + this.symbolSprite].y = vector.y;
                sLeaser.sprites[this.firstSprite + this.symbolSprite].element = Futile.atlasManager.GetElementWithName(((!(this is OverseerDroughtTutorialBehavior.KeyBoardKey)) ? "button" : "key") + this.symbol + ((!this.down && this.transform <= 0.5f) ? "A" : "B"));
                sLeaser.sprites[this.firstSprite + this.symbolSprite].color = useColor;
            }

            public bool pulsate;
            public int counter;
            public bool down;
            public int symbolSprite;
            public string symbol;
            public float symbolRotation;
            public int timeUp;
            public int timeDown;
        }

        public class KeyBoardKey : OverseerDroughtTutorialBehavior.ButtonOrKey
        {
            public KeyBoardKey(OverseerHologram hologram, int firstSprite) : base(hologram, firstSprite)
            {
                float num = 15f;
                float num2 = 3f;
                float num3 = 5f;
                base.AddClosed3DPolygon(new List<Vector2>
                    {
                        new Vector2(-num + num2, -num),
                        new Vector2(-num, -num + num2),
                        new Vector2(-num, num - num2),
                        new Vector2(-num + num2, num),
                        new Vector2(num - num2, num),
                        new Vector2(num, num - num2),
                        new Vector2(num, -num + num2),
                        new Vector2(num - num2, -num)
                    }, num3);
                for (int i = 0; i < this.lines.Count; i++)
                {
                    if (this.lines[i].A.z < 0f)
                    {
                        this.lines[i].A = Custom.Vec3FromVec2(this.lines[i].A * 0.9f, this.lines[i].A.z);
                        this.lines[i].A2 = Custom.Vec3FromVec2(this.lines[i].A2 * 0.9f, num3);
                    }
                    else
                    {
                        this.lines[i].A2 = Custom.Vec3FromVec2(this.lines[i].A2 * 1.1f, this.lines[i].A2.z);
                    }
                    if (this.lines[i].B.z < 0f)
                    {
                        this.lines[i].B = Custom.Vec3FromVec2(this.lines[i].B * 0.9f, this.lines[i].B.z);
                        this.lines[i].B2 = Custom.Vec3FromVec2(this.lines[i].B2 * 0.9f, num3);
                    }
                    else
                    {
                        this.lines[i].B2 = Custom.Vec3FromVec2(this.lines[i].B2 * 1.1f, this.lines[i].B2.z);
                    }
                }
            }

            public void MakeWider(float add)
            {
                for (int i = 0; i < this.lines.Count; i++)
                {
                    if (this.lines[i].A.x < 0f)
                    {
                        OverseerHologram.HologramPart.Line line = this.lines[i];
                        line.A.x = line.A.x - add;
                    }
                    else
                    {
                        OverseerHologram.HologramPart.Line line2 = this.lines[i];
                        line2.A.x = line2.A.x + add;
                    }
                    if (this.lines[i].A2.x < 0f)
                    {
                        OverseerHologram.HologramPart.Line line3 = this.lines[i];
                        line3.A2.x = line3.A2.x - add;
                    }
                    else
                    {
                        OverseerHologram.HologramPart.Line line4 = this.lines[i];
                        line4.A2.x = line4.A2.x + add;
                    }
                    if (this.lines[i].B.x < 0f)
                    {
                        OverseerHologram.HologramPart.Line line5 = this.lines[i];
                        line5.B.x = line5.B.x - add;
                    }
                    else
                    {
                        OverseerHologram.HologramPart.Line line6 = this.lines[i];
                        line6.B.x = line6.B.x + add;
                    }
                    if (this.lines[i].B2.x < 0f)
                    {
                        OverseerHologram.HologramPart.Line line7 = this.lines[i];
                        line7.B2.x = line7.B2.x - add;
                    }
                    else
                    {
                        OverseerHologram.HologramPart.Line line8 = this.lines[i];
                        line8.B2.x = line8.B2.x + add;
                    }
                }
            }
        }

        public class GamePadSilhouette : OverseerHologram.HologramPart
        {
            public GamePadSilhouette(OverseerDroughtTutorialBehavior.GamePadInstruction hologram, int firstSprite) : base(hologram, firstSprite)
            {
                this.totalSprites += 4;
                this.offset = new Vector2(0f, -40f);
                List<Vector2> list = new List<Vector2>();
                for (int i = 0; i < 26; i++)
                {
                    list.Add(default(Vector2));
                }
                List<Vector2> list2 = new List<Vector2>();
                list2.Add(new Vector2(22f, 11f));
                list2.Add(new Vector2(20f, 9f));
                list2.Add(new Vector2(15f, 10f));
                list2.Add(new Vector2(14f, 10f));
                list2.Add(new Vector2(11f, 3f));
                list2.Add(new Vector2(8f, 0f));
                list2.Add(new Vector2(5f, 0f));
                list2.Add(new Vector2(1f, 3f));
                list2.Add(new Vector2(0f, 6f));
                list2.Add(new Vector2(5f, 25f));
                list2.Add(new Vector2(9f, 30f));
                list2.Add(new Vector2(14f, 30f));
                list2.Add(new Vector2(16f, 28f));
                for (int j = 0; j < list2.Count; j++)
                {
                    list[j] = list2[j] + new Vector2(-25f, -15f);
                    list[25 - j] = new Vector2(25f - list2[j].x, list2[j].y - 15f);
                }
                base.AddClosedPolygon(list);
            }

            public override void Update()
            {
                base.Update();
            }

            public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                base.InitiateSprites(sLeaser, rCam);
                for (int i = 0; i < 4; i++)
                {
                    sLeaser.sprites[this.firstSprite + i] = new FSprite("Circle4", true);
                }
            }

            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, Vector2 partPos, Vector2 headPos, float useFade, float popOut, Color useColor)
            {
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos, partPos, headPos, useFade, popOut, useColor);
                if (UnityEngine.Random.value > Mathf.InverseLerp(0.5f, 1f, useFade))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        sLeaser.sprites[this.firstSprite + i].isVisible = false;
                    }
                    return;
                }
                for (int j = 0; j < 4; j++)
                {
                    sLeaser.sprites[this.firstSprite + j].isVisible = true;
                    Vector2 vector = partPos + new Vector2(12f, 5f) + Custom.DegToVec(-90f - 90f * (float)j) * 4f;
                    sLeaser.sprites[this.firstSprite + j].x = vector.x - camPos.x;
                    sLeaser.sprites[this.firstSprite + j].y = vector.y - camPos.y;
                    if (j < 3 && (this.hologram as OverseerDroughtTutorialBehavior.GamePadInstruction).buttons[j].pulsate)
                    {
                        sLeaser.sprites[this.firstSprite + j].color = (this.hologram as OverseerDroughtTutorialBehavior.GamePadInstruction).buttons[j].color;
                    }
                    else
                    {
                        sLeaser.sprites[this.firstSprite + j].color = useColor;
                    }
                    if (UnityEngine.Random.value > useFade)
                    {
                        sLeaser.sprites[this.firstSprite + j].element = Futile.atlasManager.GetElementWithName("pixel");
                        sLeaser.sprites[this.firstSprite + j].anchorY = 0f;
                        sLeaser.sprites[this.firstSprite + j].rotation = Custom.AimFromOneVectorToAnother(partPos, headPos);
                        sLeaser.sprites[this.firstSprite + j].scaleY = Vector2.Distance(partPos, headPos);
                    }
                    else
                    {
                        sLeaser.sprites[this.firstSprite + j].element = Futile.atlasManager.GetElementWithName("Circle4");
                        sLeaser.sprites[this.firstSprite + j].rotation = 0f;
                        sLeaser.sprites[this.firstSprite + j].scaleY = 1f;
                        sLeaser.sprites[this.firstSprite + j].anchorY = 0.5f;
                    }
                }
            }
        }

        public class GamePadStickSocket : OverseerHologram.HologramPart
        {
            public GamePadStickSocket(OverseerHologram hologram, int firstSprite) : base(hologram, firstSprite)
            {
                this.scale = 0.6f;
                float d = 35f * this.scale;
                for (int i = 0; i < 8; i++)
                {
                    float num = (float)i / 8f;
                    float num2 = (float)(i + 1) / 8f;
                    base.Add3DLine(Custom.DegToVec(num * 360f) * d, Custom.DegToVec(num2 * 360f) * d, 5f);
                }
            }

            public override void Update()
            {
                base.Update();
            }

            public float scale;
        }

        public class GamePadStick : OverseerHologram.HologramPart
        {
            public GamePadStick(OverseerDroughtTutorialBehavior.GamePadInstruction hologram, int firstSprite, OverseerDroughtTutorialBehavior.GamePadStickSocket socket) : base(hologram, firstSprite)
            {
                this.socket = socket;
                float num = 12f * socket.scale;
                float num2 = -5f;
                float num3 = 5f * socket.scale;
                for (int i = 0; i < 8; i++)
                {
                    float num4 = (float)i / 8f;
                    float num5 = (float)(i + 1) / 8f;
                    base.AddLine(Custom.Vec3FromVec2(Custom.DegToVec(num4 * 360f) * num, -5f + num2), Custom.Vec3FromVec2(Custom.DegToVec(num5 * 360f) * num, -5f + num2));
                    base.AddLine(Custom.Vec3FromVec2(Custom.DegToVec(num4 * 360f) * (num + num3), num2), Custom.Vec3FromVec2(Custom.DegToVec(num5 * 360f) * (num + num3), num2));
                    base.AddLine(Custom.Vec3FromVec2(Custom.DegToVec(num4 * 360f) * num, 5f + num2), Custom.Vec3FromVec2(Custom.DegToVec(num5 * 360f) * num, 5f + num2));
                    base.AddLine(Custom.Vec3FromVec2(Custom.DegToVec(num4 * 360f) * num, -5f + num2), Custom.Vec3FromVec2(Custom.DegToVec(num4 * 360f) * (num + num3), num2));
                    base.AddLine(Custom.Vec3FromVec2(Custom.DegToVec(num4 * 360f) * num, 5f + num2), Custom.Vec3FromVec2(Custom.DegToVec(num4 * 360f) * (num + num3), num2));
                }
            }

            public Vector2 showDirection
            {
                get
                {
                    return (this.hologram as OverseerDroughtTutorialBehavior.GamePadInstruction).direction.ToVector2().normalized;
                }
            }

            public override Color GetToColor
            {
                get
                {
                    if (this.outToSide)
                    {
                        return new Color(1f, 1f, 1f);
                    }
                    return base.GetToColor;
                }
            }

            public override void Update()
            {
                base.Update();
                if ((this.hologram as OverseerDroughtTutorialBehavior.GamePadInstruction).direction.x != 0 || (this.hologram as OverseerDroughtTutorialBehavior.GamePadInstruction).direction.y != 0)
                {
                    this.counter--;
                    if (this.counter < 1)
                    {
                        this.outToSide = !this.outToSide;
                        this.counter = 20;
                    }
                }
                else
                {
                    this.outToSide = false;
                    this.counter = 0;
                }
                if (this.outToSide)
                {
                    this.stickVel += this.showDirection * 0.75f * Mathf.InverseLerp(20f, 10f, (float)this.counter);
                }
                this.stickPos += this.stickVel;
                this.stickVel *= 0.75f;
                this.stickVel -= this.stickPos / 1.5f;
                if (this.stickPos.magnitude > 1f)
                {
                    this.socket.offset += this.stickPos;
                }
                this.stickPos = Vector2.ClampMagnitude(this.stickPos, 1f);
                this.offset = this.socket.offset + this.stickPos * 20f * this.socket.scale;
                this.visible = this.socket.visible;
            }

            private OverseerDroughtTutorialBehavior.GamePadStickSocket socket;
            public int counter;
            public bool outToSide;
            public Vector2 stickPos;
            public Vector2 stickVel;
        }

        public class GamePadButton : OverseerDroughtTutorialBehavior.ButtonOrKey
        {
            public GamePadButton(OverseerHologram hologram, int firstSprite) : base(hologram, firstSprite)
            {
                float d = 18f;
                float num = 5f;
                for (int i = 0; i < 8; i++)
                {
                    float num2 = (float)i / 8f;
                    float num3 = (float)(i + 1) / 8f;
                    base.Add3DLine(Custom.DegToVec(num2 * 360f + 22.5f) * d, Custom.DegToVec(num3 * 360f + 22.5f) * d, num);
                }
                for (int j = 0; j < this.lines.Count; j++)
                {
                    if (this.lines[j].A.z < 0f)
                    {
                        this.lines[j].A = Custom.Vec3FromVec2(this.lines[j].A * 0.9f, this.lines[j].A.z);
                        this.lines[j].A2 = Custom.Vec3FromVec2(this.lines[j].A2 * 0.9f, num);
                    }
                    else
                    {
                        this.lines[j].A2 = Custom.Vec3FromVec2(this.lines[j].A2 * 1.1f, this.lines[j].A2.z);
                    }
                    if (this.lines[j].B.z < 0f)
                    {
                        this.lines[j].B = Custom.Vec3FromVec2(this.lines[j].B * 0.9f, this.lines[j].B.z);
                        this.lines[j].B2 = Custom.Vec3FromVec2(this.lines[j].B2 * 0.9f, num);
                    }
                    else
                    {
                        this.lines[j].B2 = Custom.Vec3FromVec2(this.lines[j].B2 * 1.1f, this.lines[j].B2.z);
                    }
                }
            }

            public override Color GetToColor
            {
                get
                {
                    if (this.down)
                    {
                        return new Color(1f, 1f, 1f);
                    }
                    return this.buttonColor;
                }
            }

            public Color buttonColor;
        }

        public class KeyBoardInstruction : OverseerDroughtTutorialBehavior.InputInstruction
        {
            public KeyBoardInstruction(Overseer overseer, OverseerHologram.Message message, Creature communicateWith, float importance) : base(overseer, message, communicateWith, importance)
            {
                this.keys = new OverseerDroughtTutorialBehavior.KeyBoardKey[7];
                for (int i = 0; i < this.keys.Length; i++)
                {
                    this.keys[i] = new OverseerDroughtTutorialBehavior.KeyBoardKey(this, this.totalSprites);
                    base.AddPart(this.keys[i]);
                }
                this.Down.offset = new Vector2(35f, 0f);
                this.Right.offset = new Vector2(70f, 0f);
                this.Up.offset = new Vector2(35f, 35f);
                this.Throw.offset = new Vector2(-45f, 0f);
                this.Jump.offset = new Vector2(-80f, 0f);
                this.PickUp.offset = new Vector2(-125f, 0f);
                this.PickUp.MakeWider(10f);
                this.Down.symbol = "Arrow";
                this.Right.symbol = "Arrow";
                this.Up.symbol = "Arrow";
                this.Left.symbol = "Arrow";
                this.Left.symbolRotation = -90f;
                this.Right.symbolRotation = 90f;
                this.Down.symbolRotation = 180f;
                this.PickUp.symbol = "Shift";
                this.Jump.symbol = "Z";
                this.Throw.symbol = "X";
            }

            public OverseerDroughtTutorialBehavior.KeyBoardKey Left
            {
                get
                {
                    return this.keys[0];
                }
            }

            public OverseerDroughtTutorialBehavior.KeyBoardKey Right
            {
                get
                {
                    return this.keys[2];
                }
            }

            public OverseerDroughtTutorialBehavior.KeyBoardKey Up
            {
                get
                {
                    return this.keys[1];
                }
            }

            public OverseerDroughtTutorialBehavior.KeyBoardKey Down
            {
                get
                {
                    return this.keys[3];
                }
            }

            public OverseerDroughtTutorialBehavior.KeyBoardKey PickUp
            {
                get
                {
                    return this.keys[4];
                }
            }

            public OverseerDroughtTutorialBehavior.KeyBoardKey Jump
            {
                get
                {
                    return this.keys[5];
                }
            }

            public OverseerDroughtTutorialBehavior.KeyBoardKey Throw
            {
                get
                {
                    return this.keys[6];
                }
            }

            public override void Update(bool eu)
            {
                base.Update(eu);
                this.PickUp.visible = (!this.directionOnly && !this.hideInputs);
                this.Jump.visible = (!this.directionOnly && !this.hideInputs);
                this.Throw.visible = (!this.directionOnly && !this.hideInputs);
                this.Left.visible = !this.hideInputs;
                this.Right.visible = !this.hideInputs;
                this.Up.visible = !this.hideInputs;
                this.Down.visible = !this.hideInputs;
                this.Left.pulsate = (this.direction.x < 0);
                this.Right.pulsate = (this.direction.x > 0);
                this.Up.pulsate = (this.direction.y > 0);
                this.Down.pulsate = (this.direction.y < 0);
                this.PickUp.pulsate = this.showPickup;
                this.Jump.pulsate = this.showJump;
                this.Throw.pulsate = this.showThrow;
            }

            public override float DisplayPosScore(IntVector2 testPos)
            {
                float num = base.DisplayPosScore(testPos);
                if (!this.directionOnly && this.room.readyForAI)
                {
                    num -= (float)Math.Min(this.room.aimap.getAItile(testPos + new IntVector2(-10, 0)).terrainProximity, 5) * 50f;
                }
                return num;
            }

            public OverseerDroughtTutorialBehavior.KeyBoardKey[] keys;
        }

        public class GamePadInstruction : OverseerDroughtTutorialBehavior.InputInstruction
        {
            public GamePadInstruction(Overseer overseer, OverseerHologram.Message message, Creature communicateWith, float importance) : base(overseer, message, communicateWith, importance)
            {
                this.socket = new OverseerDroughtTutorialBehavior.GamePadStickSocket(this, this.totalSprites);
                base.AddPart(this.socket);
                base.AddPart(new OverseerDroughtTutorialBehavior.GamePadStick(this, this.totalSprites, this.socket));
                this.silhouette = new OverseerDroughtTutorialBehavior.GamePadSilhouette(this, this.totalSprites);
                base.AddPart(this.silhouette);
                this.buttons = new OverseerDroughtTutorialBehavior.GamePadButton[3];
                for (int i = 0; i < this.buttons.Length; i++)
                {
                    this.buttons[i] = new OverseerDroughtTutorialBehavior.GamePadButton(this, this.totalSprites);
                    base.AddPart(this.buttons[i]);
                }
                if (overseer.abstractCreature.world.game.rainWorld.options.controls[0].preset == Options.ControlSetup.Preset.XBox)
                {
                    this.PickUp.symbol = "X";
                    this.Jump.symbol = "A";
                    this.Throw.symbol = "B";
                    this.PickUp.buttonColor = new Color(0.2f, 0.6f, 1f);
                    this.Jump.buttonColor = new Color(0.4f, 1f, 0.2f);
                    this.Throw.buttonColor = new Color(1f, 0.2f, 0.2f);
                }
                else
                {
                    this.PickUp.symbol = "Square";
                    this.Jump.symbol = "Cross";
                    this.Throw.symbol = "Circle";
                    this.PickUp.buttonColor = new Color(0.9f, 0.3f, 1f);
                    this.Jump.buttonColor = new Color(0.5f, 0.5f, 1f);
                    this.Throw.buttonColor = new Color(1f, 0.3f, 0.3f);
                }
                this.parts = new OverseerHologram.HologramPart[4];
                this.partsRemainVisible = new int[this.parts.Length];
                this.parts[0] = this.socket;
                this.parts[1] = this.buttons[0];
                this.parts[2] = this.buttons[1];
                this.parts[3] = this.buttons[2];
            }

            public OverseerDroughtTutorialBehavior.GamePadButton PickUp
            {
                get
                {
                    return this.buttons[0];
                }
            }

            public OverseerDroughtTutorialBehavior.GamePadButton Jump
            {
                get
                {
                    return this.buttons[1];
                }
            }

            public OverseerDroughtTutorialBehavior.GamePadButton Throw
            {
                get
                {
                    return this.buttons[2];
                }
            }

            public override void Update(bool eu)
            {
                base.Update(eu);
                if (this.hideInputs)
                {
                    this.silhouette.visible = false;
                    for (int i = 0; i < this.parts.Length; i++)
                    {
                        this.partsRemainVisible[i] = 0;
                    }
                }
                else
                {
                    this.silhouette.visible = true;
                    for (int j = 0; j < this.parts.Length; j++)
                    {
                        this.partsRemainVisible[j] = Math.Max(0, this.partsRemainVisible[j] - 1);
                    }
                    if (this.directionAlwaysVisible || this.direction.x != 0 || this.direction.y != 0)
                    {
                        this.partsRemainVisible[0] = Math.Max(this.partsRemainVisible[0], 30);
                    }
                    if (this.showPickup)
                    {
                        this.partsRemainVisible[1] = (this.partsRemainVisible[1] = Math.Max(this.partsRemainVisible[1], 30));
                    }
                    if (this.showJump)
                    {
                        this.partsRemainVisible[2] = (this.partsRemainVisible[2] = Math.Max(this.partsRemainVisible[2], 30));
                    }
                    if (this.showThrow)
                    {
                        this.partsRemainVisible[3] = (this.partsRemainVisible[3] = Math.Max(this.partsRemainVisible[3], 30));
                    }
                }
                this.socket.visible = (this.partsRemainVisible[0] > 0);
                this.PickUp.visible = (this.partsRemainVisible[1] > 0);
                this.Jump.visible = (this.partsRemainVisible[2] > 0);
                this.Throw.visible = (this.partsRemainVisible[3] > 0);
                int num = 0;
                for (int k = 0; k < this.parts.Length; k++)
                {
                    if (this.parts[k].visible)
                    {
                        num++;
                    }
                }
                if (num == 1)
                {
                    for (int l = 0; l < this.parts.Length; l++)
                    {
                        if (this.parts[l].visible)
                        {
                            this.parts[l].offset = Vector2.Lerp(Custom.MoveTowards(this.parts[l].offset, new Vector2(0f, 0f), 2f), new Vector2(0f, 0f), 0.1f);
                            break;
                        }
                    }
                }
                else
                {
                    float num2 = (float)(num - 1) * 25f;
                    int num3 = 0;
                    for (int m = 0; m < this.parts.Length; m++)
                    {
                        if (this.parts[m].visible)
                        {
                            float t = (float)num3 / (float)(num - 1);
                            Vector2 vector = new Vector2(Mathf.Lerp(-num2, num2, t), 0f);
                            this.parts[m].offset = Vector2.Lerp(Custom.MoveTowards(this.parts[m].offset, vector, 2f), vector, 0.1f);
                            num3++;
                        }
                    }
                }
                this.PickUp.pulsate = this.showPickup;
                this.Jump.pulsate = this.showJump;
                this.Throw.pulsate = this.showThrow;
            }

            public override float DisplayPosScore(IntVector2 testPos)
            {
                float num = base.DisplayPosScore(testPos);
                if (!this.directionOnly && this.room.readyForAI)
                {
                    num -= (float)Math.Min(this.room.aimap.getAItile(testPos + new IntVector2(10, 0)).terrainProximity, 5) * 50f;
                }
                return num;
            }

            public OverseerDroughtTutorialBehavior.GamePadSilhouette silhouette;
            public OverseerDroughtTutorialBehavior.GamePadStickSocket socket;
            public OverseerDroughtTutorialBehavior.GamePadButton[] buttons;
            public bool directionAlwaysVisible;
            public float horizontalOffset;
            public new OverseerHologram.HologramPart[] parts;
            // Token: 0x040027F7 RID: 10231
            public int[] partsRemainVisible;
        }
    }
}
