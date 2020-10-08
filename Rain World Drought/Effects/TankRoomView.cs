using System;
using RWCustom;
using UnityEngine;

namespace Rain_World_Drought.Effects
{
    public class TankRoomView : BackgroundScene
    {
        public TankRoomView(Room room, RoomSettings.RoomEffect effect) : base(room)
        {
            this.floorLevel = -2500f;
            this.atmosphereColor = new Color(0.074f, 0.086f, 0.101f);
            this.effect = effect;
            this.sceneOrigo = base.RoomToWorldPos(room.abstractRoom.size.ToVector2() * 10f);

            Shader.SetGlobalVector("_AboveCloudsAtmosphereColor", this.atmosphereColor);
            Shader.SetGlobalVector("_SceneOrigoPosition", this.sceneOrigo);

            if (this.room.abstractRoom != null && this.room.abstractRoom.name != null && this.room.abstractRoom.name.Equals("IS_V07"))
            {
                this.AddElement(new SimpleTankViewBackground(this, "Tr_TankWall", new Vector2(683f, 384f)));//Foreground
                this.AddElement(new DistantPipe(this, "Tr_PipesA", new Vector2(base.PosFromDrawPosAtNeutralCamPos(new Vector2(-600f, 0f), 15f).x, this.floorLevel - 4000f), 15f, 0f));//Foreground
                this.AddElement(new SimpleTankViewBackground(this, "Tr_TankShadow1", new Vector2(683f, 384f)));//ForegroundLights
                this.AddElement(new DistantPipe(this, "Tr_PipesB", new Vector2(base.PosFromDrawPosAtNeutralCamPos(new Vector2(-185f, 0f), 10f).x, this.floorLevel - 1700f), 10f, 0f));//ForegroundLights
                this.AddElement(new SimpleTankViewBackground(this, "Tr_TankShadow", new Vector2(683f, 384f)));//Shortcuts
                this.AddElement(new DistantPipe(this, "Tr_PipesC", new Vector2(base.PosFromDrawPosAtNeutralCamPos(new Vector2(-300f, 0f), 5f).x, this.floorLevel - 50f), 5f, 0f));//Shortcuts
            }
            else
            {
                this.AddElement(new SimpleTankViewBackground(this, "Tr_TankWall", new Vector2(683f, 384f)));//Foreground
                this.AddElement(new DistantPipe(this, "Tr_PipesA", new Vector2(base.PosFromDrawPosAtNeutralCamPos(new Vector2(-100f, 0f), 15f).x, this.floorLevel - 4000f), 15f, 0f));//Foreground
                this.AddElement(new SimpleTankViewBackground(this, "Tr_TankShadow1", new Vector2(683f, 384f)));//ForegroundLights
                this.AddElement(new DistantPipe(this, "Tr_PipesB", new Vector2(base.PosFromDrawPosAtNeutralCamPos(new Vector2(315f, 0f), 10f).x, this.floorLevel - 1700f), 10f, 0f));//ForegroundLights
                this.AddElement(new SimpleTankViewBackground(this, "Tr_TankShadow", new Vector2(683f, 384f)));//Shortcuts
                this.AddElement(new DistantPipe(this, "Tr_PipesC", new Vector2(base.PosFromDrawPosAtNeutralCamPos(new Vector2(200f, 0f), 5f).x, this.floorLevel - 50f), 5f, 0f));//Shortcuts
            }
        }

        private int imTimer = 0;

        private int imStage = 0;

        private Player player;

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (((this.room.game.session as StoryGameSession).saveState.miscWorldSaveData as patch_MiscWorldSaveData).isImproved || eu || !this.room.abstractRoom.name.Equals("IS_V04"))
            {
                return;
            }
            if (imStage == 0)
            {
                for (int j = 0; j < this.room.roomSettings.placedObjects.Count; j++)
                {
                    if ((this.room.roomSettings.placedObjects[j].type) == ((PlacedObject.Type)patch_PlacedObject.Type.ImprovementTrigger))
                    {
                        for (int k = 0; k < this.room.physicalObjects.Length; k++)
                        {
                            for (int l = 0; l < this.room.physicalObjects[k].Count; l++)
                            {
                                for (int m = 0; m < this.room.physicalObjects[k][l].bodyChunks.Length; m++)
                                {
                                    if (this.room.physicalObjects[k][l].bodyChunks[m].pos.x > this.room.roomSettings.placedObjects[j].pos.x - 150f && this.room.physicalObjects[k][l].bodyChunks[m].pos.x < this.room.roomSettings.placedObjects[j].pos.x + 150f && this.room.physicalObjects[k][l].bodyChunks[m].pos.y > this.room.roomSettings.placedObjects[j].pos.y - 150f && this.room.physicalObjects[k][l].bodyChunks[m].pos.y < this.room.roomSettings.placedObjects[j].pos.y + 150f && this.room.physicalObjects[k][l] is Player && !(this.room.physicalObjects[k][l] as Creature).dead)
                                    {
                                        Debug.Log("Improvment Activated");
                                        (this.room.physicalObjects[k][l] as Player).Stun(20);
                                        (this.room.physicalObjects[k][l] as Player).airInLungs = 1f;
                                        player = (this.room.physicalObjects[k][l] as Player);
                                        this.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Telekenisis, 0f, 0.5f, 1f);
                                        this.imStage = 1;
                                        this.imTimer = this.room.game.clock;
                                    }
                                }
                            }
                        }
                        j = this.room.roomSettings.placedObjects.Count;
                    }
                }
            }
            else
            {
                switch (imStage)
                {
                    case 1:
                        player.Stun(20);
                        player.airInLungs = 1f;
                        player.mainBodyChunk.vel += Vector2.ClampMagnitude(this.room.MiddleOfTile(123, 2) - player.mainBodyChunk.pos, 40f) / 40f * 2.8f * Mathf.InverseLerp(30f, 160f, (float)(this.room.game.clock - this.imTimer));
                        if (player.room.game.clock - this.imTimer > 300)
                        {
                            player.room.AddObject(new ShockWave(player.mainBodyChunk.pos, 600f, 0.5f, 40));
                            player.room.PlaySound(SoundID.Moon_Wake_Up_Rumble, 0f, 1f, 1f);
                            player.room.PlaySound(SoundID.Player_Coral_Circuit_Swim, 0f, 1f, 1f);
                            player.room.PlaySound(SoundID.Thunder_Close, 0f, 1f, 1f);
                            this.imStage = 3;
                            ((player.room.game.session as StoryGameSession).saveState.miscWorldSaveData as patch_MiscWorldSaveData).isImproved = true;
                        }
                        break;

                    case 3:
                        return;
                }
            }
        }

        public float AtmosphereColorAtDepth(float depth)
        {
            return Mathf.Clamp(depth / 15f, 0f, 1f) * 0.9f;
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public RoomSettings.RoomEffect effect;

        private float floorLevel;

        public Color atmosphereColor;

        public class SimpleTankViewBackground : Simple2DBackgroundIllustration
        {
            public SimpleTankViewBackground(BackgroundScene scene, string illustrationName, Vector2 pos) : base(scene, illustrationName, pos)
            {
                uglyWorkAroundName = illustrationName;
            }

            public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                sLeaser.sprites = new FSprite[1];
                sLeaser.sprites[0] = new FSprite(uglyWorkAroundName, true);
                sLeaser.sprites[0].x = this.pos.x;
                sLeaser.sprites[0].y = this.pos.y;
                sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["Background"];
                this.AddToContainer(sLeaser, rCam, null);
            }

            public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
            {
                if (newContatiner == null)
                {
                    switch (uglyWorkAroundName)
                    {
                        case "Tr_TankWall":
                            newContatiner = rCam.ReturnFContainer("Foreground");
                            break;

                        case "Tr_TankShadow1":
                            newContatiner = rCam.ReturnFContainer("ForegroundLights");
                            break;

                        case "Tr_TankShadow":
                            newContatiner = rCam.ReturnFContainer("Shortcuts");
                            break;

                        default:
                            newContatiner = rCam.ReturnFContainer("Shortcuts");
                            break;
                    }
                }
                foreach (FSprite fsprite in sLeaser.sprites)
                {
                    fsprite.RemoveFromContainer();
                    newContatiner.AddChild(fsprite);
                }
            }

            public string uglyWorkAroundName = "";
        }

        public class DistantPipe : BackgroundScene.BackgroundSceneElement
        {
            public DistantPipe(TankRoomView tankViewScene, string assetName, Vector2 pos, float depth, float atmosphericalDepthAdd) : base(tankViewScene, pos, depth)
            {
                this.assetName = assetName;
                this.atmosphericalDepthAdd = atmosphericalDepthAdd;
                this.scene.LoadGraphic(assetName, true, false);
            }

            private TankRoomView tankViewScene
            {
                get
                {
                    return this.scene as TankRoomView;
                }
            }

            public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                sLeaser.sprites = new FSprite[1];
                sLeaser.sprites[0] = new FSprite(this.assetName, true);
                //sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["Basic"];
                sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["DistantBkgObject"];
                sLeaser.sprites[0].anchorY = 0f;
                this.AddToContainer(sLeaser, rCam, null);
            }

            public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
            {
                if (newContatiner == null)
                {
                    switch (this.assetName)
                    {
                        case "Tr_PipesA":
                            newContatiner = rCam.ReturnFContainer("Foreground");
                            break;

                        case "Tr_PipesB":
                            newContatiner = rCam.ReturnFContainer("ForegroundLights");
                            break;

                        case "Tr_PipesC":
                            newContatiner = rCam.ReturnFContainer("Shortcuts");
                            break;

                        default:
                            newContatiner = rCam.ReturnFContainer("Shortcuts");
                            break;
                    }
                }
                foreach (FSprite fsprite in sLeaser.sprites)
                {
                    fsprite.RemoveFromContainer();
                    newContatiner.AddChild(fsprite);
                }
            }

            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
            {
                Vector2 vector = base.DrawPos(camPos, rCam.hDisplace);
                sLeaser.sprites[0].x = vector.x;
                sLeaser.sprites[0].y = vector.y;
                sLeaser.sprites[0].color = new Color(this.tankViewScene.AtmosphereColorAtDepth(this.depth), 0f, 0f);
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }

            public string assetName;

            public float atmosphericalDepthAdd;
        }
    }
}