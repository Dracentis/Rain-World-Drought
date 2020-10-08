using MonoMod;
using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using System.Runtime.CompilerServices;

class patch_AboveCloudsView : AboveCloudsView
{
    [MonoModIgnore]
    patch_AboveCloudsView(Room room, RoomSettings.RoomEffect effect) : base(room, effect) { }

    [MonoModIgnore]
    private bool SIClouds;

    public extern void orig_ctor(Room room, RoomSettings.RoomEffect effect);

    [MonoModConstructor]
    public void ctor_(Room room, RoomSettings.RoomEffect effect)
    {
        Type[] constructorSignature = new Type[1];
        constructorSignature[0] = typeof(Room);
        RuntimeMethodHandle handle = typeof(BackgroundScene).GetConstructor(constructorSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action<Room> funct = (Action<Room>)Activator.CreateInstance(typeof(Action<Room>), this, ptr);
        funct(room);
        startAltitude = 20000f;
        endAltitude = 31400f;
        cloudsStartDepth = 5f;
        cloudsEndDepth = 40f;
        distantCloudsEndDepth = 200f;
        atmosphereColor = new Color(0.160784319f, 0.23137255f, 0.31764707f);
        this.effect = effect;
        SIClouds = (room.world.region != null && room.world.region.name == "SI");
        if (SIClouds)
        {
            startAltitude = 9000f;
            endAltitude = 27000f;
        }
        sceneOrigo = new Vector2(2514f, (startAltitude + endAltitude) / 2f);
        clouds = new List<AboveCloudsView.Cloud>();
        LoadGraphic("clouds1", false, false);
        LoadGraphic("clouds2", false, false);
        LoadGraphic("clouds3", false, false);
        LoadGraphic("flyingClouds1", false, false);
        generalFog = new AboveCloudsView.Fog(this);
        AddElement(generalFog);
        AddElement(new BackgroundScene.Simple2DBackgroundIllustration(this, "AtC_Sky", new Vector2(683f, 384f)));
        if (SIClouds)
        {
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire1", PosFromDrawPosAtNeutralCamPos(new Vector2(517f, -148f), 50f), 50f, -30f));
            float depth = 160f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure1", PosFromDrawPosAtNeutralCamPos(new Vector2(-520f, -85f), depth), depth, -20f));
            AddElement(new AboveCloudsView.DistantLightning(this, "AtC_Light1", PosFromDrawPosAtNeutralCamPos(new Vector2(-520f, -119f), depth), depth, 55f));
            depth = 350f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure2", PosFromDrawPosAtNeutralCamPos(new Vector2(88f, -37f), depth), depth, 0f));
            AddElement(new AboveCloudsView.DistantLightning(this, "AtC_Light2", PosFromDrawPosAtNeutralCamPos(new Vector2(88f, -53f), depth), depth, 250f));
            depth = 600f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure3", PosFromDrawPosAtNeutralCamPos(new Vector2(-316f, -24f), depth), depth, -100f));
            AddElement(new AboveCloudsView.DistantLightning(this, "AtC_Light3", PosFromDrawPosAtNeutralCamPos(new Vector2(-316f, -32f), depth), depth, 80f));
            depth = 700f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure4", PosFromDrawPosAtNeutralCamPos(new Vector2(-648f, -21f), depth), depth, -200f));
            depth = 800f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure5", PosFromDrawPosAtNeutralCamPos(new Vector2(156f, -22f), depth), depth, -350f));
            depth = 850f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure6", PosFromDrawPosAtNeutralCamPos(new Vector2(-242f, -20f), depth), depth, -350f));
            depth = 100f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire2", PosFromDrawPosAtNeutralCamPos(new Vector2(-653f, -57f), depth), depth, 10f));
            depth = 155f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire3", PosFromDrawPosAtNeutralCamPos(new Vector2(0f, -46f), depth), depth, 0f));
            depth = 190f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire4", PosFromDrawPosAtNeutralCamPos(new Vector2(-224f, -20f), depth), depth, 80f));
            depth = 360f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire5", PosFromDrawPosAtNeutralCamPos(new Vector2(-276f, -24f), depth), depth, -100f));
            depth = 280f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire6", PosFromDrawPosAtNeutralCamPos(new Vector2(-39f, -33f), depth), depth, 0f));
            depth = 370f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire7", PosFromDrawPosAtNeutralCamPos(new Vector2(155f, -7f), depth), depth, -85f));
            depth = 380f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire8", PosFromDrawPosAtNeutralCamPos(new Vector2(-380f, 3f), depth), depth, -50f));
            depth = 395f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire9", PosFromDrawPosAtNeutralCamPos(new Vector2(-207f, -1f), depth), depth, -50f));
        }
        else
        {
            float depth2 = 160f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure1", PosFromDrawPosAtNeutralCamPos(new Vector2(-520f, -85f), depth2), depth2, -20f));
            AddElement(new AboveCloudsView.DistantLightning(this, "AtC_Light1", PosFromDrawPosAtNeutralCamPos(new Vector2(-520f, -119f), depth2), depth2, 55f));
            depth2 = 350f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure2", PosFromDrawPosAtNeutralCamPos(new Vector2(88f, -37f), depth2), depth2, 0f));
            AddElement(new AboveCloudsView.DistantLightning(this, "AtC_Light2", PosFromDrawPosAtNeutralCamPos(new Vector2(88f, -53f), depth2), depth2, 250f));
            depth2 = 600f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure3", PosFromDrawPosAtNeutralCamPos(new Vector2(-316f, -24f), depth2), depth2, -100f));
            AddElement(new AboveCloudsView.DistantLightning(this, "AtC_Light3", PosFromDrawPosAtNeutralCamPos(new Vector2(-316f, -32f), depth2), depth2, 80f));
            depth2 = 700f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure4", PosFromDrawPosAtNeutralCamPos(new Vector2(-648f, -21f), depth2), depth2, -200f));
            depth2 = 800f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure5", PosFromDrawPosAtNeutralCamPos(new Vector2(156f, -22f), depth2), depth2, -350f));
            depth2 = 850f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Structure6", PosFromDrawPosAtNeutralCamPos(new Vector2(-242f, -20f), depth2), depth2, -350f));
            depth2 = 80f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire1", PosFromDrawPosAtNeutralCamPos(new Vector2(-587f, -134f), depth2), depth2, -60f));
            depth2 = 100f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire2", PosFromDrawPosAtNeutralCamPos(new Vector2(-653f, -57f), depth2), depth2, 10f));
            depth2 = 155f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire3", PosFromDrawPosAtNeutralCamPos(new Vector2(0f, -46f), depth2), depth2, 0f));
            depth2 = 190f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire4", PosFromDrawPosAtNeutralCamPos(new Vector2(-224f, -20f), depth2), depth2, 80f));
            depth2 = 360f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire5", PosFromDrawPosAtNeutralCamPos(new Vector2(-276f, -24f), depth2), depth2, -100f));
            depth2 = 280f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire6", PosFromDrawPosAtNeutralCamPos(new Vector2(-39f, -33f), depth2), depth2, 0f));
            depth2 = 370f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire7", PosFromDrawPosAtNeutralCamPos(new Vector2(155f, -7f), depth2), depth2, -85f));
            depth2 = 380f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire8", PosFromDrawPosAtNeutralCamPos(new Vector2(-380f, 3f), depth2), depth2, -50f));
            depth2 = 395f;
            AddElement(new AboveCloudsView.DistantBuilding(this, "AtC_Spire9", PosFromDrawPosAtNeutralCamPos(new Vector2(-207f, -1f), depth2), depth2, -50f));
        }
        if (effect.type == RoomSettings.RoomEffect.Type.AboveCloudsView)
        {
            int num = 7;
            for (int i = 0; i < num; i++)
            {
                float cloudDepth = (float)i / (float)(num - 1);
                AddElement(new AboveCloudsView.CloseCloud(this, new Vector2(0f, 0f), cloudDepth, i));
            }
        }
        int num2 = 11;
        for (int j = 0; j < num2; j++)
        {
            float num3 = (float)j / (float)(num2 - 1);
            AddElement(new AboveCloudsView.DistantCloud(this, new Vector2(0f, -40f * cloudsEndDepth * (1f - num3)), num3, j));
        }
        AddElement(new AboveCloudsView.FlyingCloud(this, PosFromDrawPosAtNeutralCamPos(new Vector2(0f, 75f), 355f), 355f, 0, 0.35f, 0.5f, 0.9f));
        AddElement(new AboveCloudsView.FlyingCloud(this, PosFromDrawPosAtNeutralCamPos(new Vector2(0f, 43f), 920f), 920f, 0, 0.15f, 0.3f, 0.95f));
        Shader.SetGlobalVector("_AboveCloudsAtmosphereColor", atmosphereColor);

    }

}

