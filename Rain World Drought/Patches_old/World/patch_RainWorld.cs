using System;
using System.Collections.Generic;
using System.IO;
using MonoMod;
using RWCustom;
using UnityEngine;
using System.Text.RegularExpressions;

public class patch_RainWorld : RainWorld
{
    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == UnityEngine.LogType.Exception || type == UnityEngine.LogType.Error)
        {
            File.AppendAllText("exceptionLog.txt", logString + Environment.NewLine);
            File.AppendAllText("exceptionLog.txt", stackTrace + Environment.NewLine);
            return;
        }
        File.AppendAllText("consoleLog.txt", logString + Environment.NewLine);
    }

    public void Start()
        {
            Awake();
            buildType = BuildType.Development;
            try
            {
                worldVersion = int.Parse(File.ReadAllText(string.Concat(new object[]
                {
                    Custom.RootFolderDirectory(),
                    Path.DirectorySeparatorChar,
                    "World",
                    Path.DirectorySeparatorChar,
                    "worldVersion.txt"
                })));
            }
            catch
            {
            }
            gameVersion = 1;
            setup = LoadSetupValues(buildType == BuildType.Distribution);
            options = new Options();
            progression = new PlayerProgression(this, setup.loadProg);
            if (setup.singlePlayerChar != -1)
            {
                progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat = setup.singlePlayerChar;
            }
            flatIllustrations = File.Exists(Custom.RootFolderDirectory() + "flatmode.txt");
            if (Screen.fullScreen != options.fullScreen || Screen.width != (int)options.ScreenSize.x || Screen.height != (int)options.ScreenSize.y)
            {
                Screen.SetResolution((int)options.ScreenSize.x, (int)options.ScreenSize.y, false);
                Screen.fullScreen = options.fullScreen;
            }
            if (buildType != BuildType.Development)
            {
                Screen.showCursor = !options.fullScreen;
            }
            else
            {
                Screen.showCursor = true;
            }
            if (!WorldChecksumController.ControlCheckSum(buildType))
            {
                Debug.Log("World Checksum INCORRECT!");
                progression.gameTinkeredWith = false;
            }
            FutileParams futileParams = new FutileParams(true, true, true, true);
            futileParams.AddResolutionLevel(options.ScreenSize.x, 1f, 1f, string.Empty);
            futileParams.origin = new Vector2(0f, 0f);
            Futile.instance.Init(futileParams);
            Futile.displayScale = 1f;
            persistentData = new PersistentData(this);
            Shaders = new Dictionary<string, FShader>();
            Shaders.Add("Basic", FShader.CreateShader("Basic", Shader.Find("Futile/Basic")));
            Shaders.Add("LevelColor", FShader.CreateShader("LevelColor", Shader.Find("Futile/LevelColor")));
            Shaders.Add("Background", FShader.CreateShader("Background", Shader.Find("Futile/Background")));
            Shaders.Add("WaterSurface", FShader.CreateShader("WaterSurface", Shader.Find("Futile/WaterSurface")));
            Shaders.Add("DeepWater", FShader.CreateShader("DeepWater", Shader.Find("Futile/DeepWater")));
            Shaders.Add("Shortcuts", FShader.CreateShader("ShortCut0", Shader.Find("Futile/ShortCut0")));
            Shaders.Add("DeathRain", FShader.CreateShader("DeathRain", Shader.Find("Futile/DeathRain")));
            Shaders.Add("LizardLaser", FShader.CreateShader("LizardLaser", Shader.Find("Futile/LizardLaser")));
            Shaders.Add("WaterLight", FShader.CreateShader("WaterLight", Shader.Find("Futile/WaterLight")));
            Shaders.Add("WaterFall", FShader.CreateShader("WaterFall", Shader.Find("Futile/WaterFall")));
            Shaders.Add("ShockWave", FShader.CreateShader("ShockWave", Shader.Find("Futile/ShockWave")));
            Shaders.Add("Smoke", FShader.CreateShader("Smoke", Shader.Find("Futile/Smoke")));
            Shaders.Add("Spores", FShader.CreateShader("Spores", Shader.Find("Futile/Spores")));
            Shaders.Add("Steam", FShader.CreateShader("Steam", Shader.Find("Futile/Steam")));
            Shaders.Add("ColoredSprite", FShader.CreateShader("ColoredSprite", Shader.Find("Futile/ColoredSprite")));
            Shaders.Add("ColoredSprite2", FShader.CreateShader("ColoredSprite2", Shader.Find("Futile/ColoredSprite2")));
            Shaders.Add("LightSource", FShader.CreateShader("LightSource", Shader.Find("Futile/LightSource")));
            Shaders.Add("LightBloom", FShader.CreateShader("LightBloom", Shader.Find("Futile/LightBloom")));
            Shaders.Add("SkyBloom", FShader.CreateShader("SkyBloom", Shader.Find("Futile/SkyBloom")));
            Shaders.Add("Adrenaline", FShader.CreateShader("Adrenaline", Shader.Find("Futile/Adrenaline")));
            Shaders.Add("CicadaWing", FShader.CreateShader("CicadaWing", Shader.Find("Futile/CicadaWing")));
            Shaders.Add("BulletRain", FShader.CreateShader("BulletRain", Shader.Find("Futile/BulletRain")));
            Shaders.Add("CustomDepth", FShader.CreateShader("CustomDepth", Shader.Find("Futile/CustomDepth")));
            Shaders.Add("UnderWaterLight", FShader.CreateShader("UnderWaterLight", Shader.Find("Futile/UnderWaterLight")));
            Shaders.Add("FlatLight", FShader.CreateShader("FlatLight", Shader.Find("Futile/FlatLight")));
            Shaders.Add("FlatLightBehindTerrain", FShader.CreateShader("FlatLightBehindTerrain", Shader.Find("Futile/FlatLightBehindTerrain")));
            Shaders.Add("VectorCircle", FShader.CreateShader("VectorCircle", Shader.Find("Futile/VectorCircle")));
            Shaders.Add("VectorCircleFadable", FShader.CreateShader("VectorCircleFadable", Shader.Find("Futile/VectorCircleFadable")));
            Shaders.Add("FlareBomb", FShader.CreateShader("FlareBomb", Shader.Find("Futile/FlareBomb")));
            Shaders.Add("Fog", FShader.CreateShader("Fog", Shader.Find("Futile/Fog")));
            Shaders.Add("WaterSplash", FShader.CreateShader("WaterSplash", Shader.Find("Futile/WaterSplash")));
            Shaders.Add("EelFin", FShader.CreateShader("EelFin", Shader.Find("Futile/EelFin")));
            Shaders.Add("EelBody", FShader.CreateShader("EelBody", Shader.Find("Futile/EelBody")));
            Shaders.Add("JaggedCircle", FShader.CreateShader("JaggedCircle", Shader.Find("Futile/JaggedCircle")));
            Shaders.Add("JaggedSquare", FShader.CreateShader("JaggedSquare", Shader.Find("Futile/JaggedSquare")));
            Shaders.Add("TubeWorm", FShader.CreateShader("TubeWorm", Shader.Find("Futile/TubeWorm")));
            Shaders.Add("LizardAntenna", FShader.CreateShader("LizardAntenna", Shader.Find("Futile/LizardAntenna")));
            Shaders.Add("TentaclePlant", FShader.CreateShader("TentaclePlant", Shader.Find("Futile/TentaclePlant")));
            Shaders.Add("LevelMelt", FShader.CreateShader("LevelMelt", Shader.Find("Futile/LevelMelt")));
            Shaders.Add("LevelMelt2", FShader.CreateShader("LevelMelt2", Shader.Find("Futile/LevelMelt2")));
            Shaders.Add("CoralCircuit", FShader.CreateShader("CoralCircuit", Shader.Find("Futile/CoralCircuit")));
            Shaders.Add("DeadCoralCircuit", FShader.CreateShader("DeadCoralCircuit", Shader.Find("Futile/DeadCoralCircuit")));
            Shaders.Add("CoralNeuron", FShader.CreateShader("CoralNeuron", Shader.Find("Futile/CoralNeuron")));
            Shaders.Add("Bloom", FShader.CreateShader("Bloom", Shader.Find("Futile/Bloom")));
            Shaders.Add("GravityDisruptor", FShader.CreateShader("GravityDisruptor", Shader.Find("Futile/GravityDisruptor")));
            Shaders.Add("GlyphProjection", FShader.CreateShader("GlyphProjection", Shader.Find("Futile/GlyphProjection")));
            Shaders.Add("BlackGoo", FShader.CreateShader("BlackGoo", Shader.Find("Futile/BlackGoo")));
            Shaders.Add("Map", FShader.CreateShader("Map", Shader.Find("Futile/Map")));
            Shaders.Add("MapAerial", FShader.CreateShader("MapMapAerial", Shader.Find("Futile/MapAerial")));
            Shaders.Add("MapShortcut", FShader.CreateShader("MapShortcut", Shader.Find("Futile/MapShortcut")));
            Shaders.Add("LightAndSkyBloom", FShader.CreateShader("LightAndSkyBloom", Shader.Find("Futile/LightAndSkyBloom")));
            Shaders.Add("SceneBlur", FShader.CreateShader("SceneBlur", Shader.Find("Futile/SceneBlur")));
            Shaders.Add("EdgeFade", FShader.CreateShader("EdgeFade", Shader.Find("Futile/EdgeFade")));
            Shaders.Add("HeatDistortion", FShader.CreateShader("HeatDistortion", Shader.Find("Futile/HeatDistortion")));
            Shaders.Add("Projection", FShader.CreateShader("Projection", Shader.Find("Futile/Projection")));
            Shaders.Add("SingleGlyph", FShader.CreateShader("SingleGlyph", Shader.Find("Futile/SingleGlyph")));
            Shaders.Add("DeepProcessing", FShader.CreateShader("DeepProcessing", Shader.Find("Futile/DeepProcessing")));
            Shaders.Add("Cloud", FShader.CreateShader("Cloud", Shader.Find("Futile/Cloud")));
            Shaders.Add("CloudDistant", FShader.CreateShader("CloudDistant", Shader.Find("Futile/CloudDistant")));
            Shaders.Add("DistantBkgObject", FShader.CreateShader("DistantBkgObject", Shader.Find("Futile/DistantBkgObject")));
            Shaders.Add("BkgFloor", FShader.CreateShader("BkgFloor", Shader.Find("Futile/BkgFloor")));
            Shaders.Add("House", FShader.CreateShader("House", Shader.Find("Futile/House")));
            Shaders.Add("DistantBkgObjectRepeatHorizontal", FShader.CreateShader("DistantBkgObjectRepeatHorizontal", Shader.Find("Futile/DistantBkgObjectRepeatHorizontal")));
            Shaders.Add("Dust", FShader.CreateShader("Dust", Shader.Find("Futile/Dust")));
            Shaders.Add("RoomTransition", FShader.CreateShader("RoomTransition", Shader.Find("Futile/RoomTransition")));
            Shaders.Add("VoidCeiling", FShader.CreateShader("VoidCeiling", Shader.Find("Futile/VoidCeiling")));
            Shaders.Add("FlatLightNoisy", FShader.CreateShader("FlatLightNoisy", Shader.Find("Futile/FlatLightNoisy")));
            Shaders.Add("VoidWormBody", FShader.CreateShader("VoidWormBody", Shader.Find("Futile/VoidWormBody")));
            Shaders.Add("VoidWormFin", FShader.CreateShader("VoidWormFin", Shader.Find("Futile/VoidWormFin")));
            Shaders.Add("VoidWormPincher", FShader.CreateShader("VoidWormPincher", Shader.Find("Futile/VoidWormPincher")));
            Shaders.Add("FlatWaterLight", FShader.CreateShader("FlatWaterLight", Shader.Find("Futile/FlatWaterLight")));
            Shaders.Add("WormLayerFade", FShader.CreateShader("WormLayerFade", Shader.Find("Futile/WormLayerFade")));
            Shaders.Add("OverseerZip", FShader.CreateShader("OverseerZip", Shader.Find("Futile/OverseerZip")));
            Shaders.Add("GhostSkin", FShader.CreateShader("GhostSkin", Shader.Find("Futile/GhostSkin")));
            Shaders.Add("GhostDistortion", FShader.CreateShader("GhostDistortion", Shader.Find("Futile/GhostDistortion")));
            Shaders.Add("GateHologram", FShader.CreateShader("GateHologram", Shader.Find("Futile/GateHologram")));
            Shaders.Add("OutPostAntler", FShader.CreateShader("OutPostAntler", Shader.Find("Futile/OutPostAntler")));
            Shaders.Add("WaterNut", FShader.CreateShader("WaterNut", Shader.Find("Futile/WaterNut")));
            Shaders.Add("Hologram", FShader.CreateShader("Hologram", Shader.Find("Futile/Hologram")));
            Shaders.Add("FireSmoke", FShader.CreateShader("FireSmoke", Shader.Find("Futile/FireSmoke")));
            Shaders.Add("HoldButtonCircle", FShader.CreateShader("HoldButtonCircle", Shader.Find("Futile/HoldButtonCircle")));
            Shaders.Add("GoldenGlow", FShader.CreateShader("GoldenGlow", Shader.Find("Futile/GoldenGlow")));
            Shaders.Add("ElectricDeath", FShader.CreateShader("ElectricDeath", Shader.Find("Futile/ElectricDeath")));
            Shaders.Add("VoidSpawnBody", FShader.CreateShader("VoidSpawnBody", Shader.Find("Futile/VoidSpawnBody")));
            Shaders.Add("SceneLighten", FShader.CreateShader("SceneLighten", Shader.Find("Futile/SceneLighten")));
            Shaders.Add("SceneBlurLightEdges", FShader.CreateShader("SceneBlurLightEdges", Shader.Find("Futile/SceneBlurLightEdges")));
            Shaders.Add("SceneRain", FShader.CreateShader("SceneRain", Shader.Find("Futile/SceneRain")));
            Shaders.Add("SceneOverlay", FShader.CreateShader("SceneOverlay", Shader.Find("Futile/SceneOverlay")));
            Shaders.Add("SceneSoftLight", FShader.CreateShader("SceneSoftLight", Shader.Find("Futile/SceneSoftLight")));
            Shaders.Add("HologramImage", FShader.CreateShader("HologramImage", Shader.Find("Futile/HologramImage")));
            Shaders.Add("HologramBehindTerrain", FShader.CreateShader("HologramBehindTerrain", Shader.Find("Futile/HologramBehindTerrain")));
            Shaders.Add("Decal", FShader.CreateShader("Decal", Shader.Find("Futile/Decal")));
            Shaders.Add("SpecificDepth", FShader.CreateShader("SpecificDepth", Shader.Find("Futile/SpecificDepth")));
            Shaders.Add("LocalBloom", FShader.CreateShader("LocalBloom", Shader.Find("Futile/LocalBloom")));
            Shaders.Add("MenuText", FShader.CreateShader("MenuText", Shader.Find("Futile/MenuText")));
            Shaders.Add("DeathFall", FShader.CreateShader("DeathFall", Shader.Find("Futile/DeathFall")));
            Shaders.Add("KingTusk", FShader.CreateShader("KingTusk", Shader.Find("Futile/KingTusk")));
            Shaders.Add("HoloGrid", FShader.CreateShader("HoloGrid", Shader.Find("Futile/HoloGrid")));
            Shaders.Add("SootMark", FShader.CreateShader("SootMark", Shader.Find("Futile/SootMark")));
            Shaders.Add("NewVultureSmoke", FShader.CreateShader("NewVultureSmoke", Shader.Find("Futile/NewVultureSmoke")));
            Shaders.Add("SmokeTrail", FShader.CreateShader("SmokeTrail", Shader.Find("Futile/SmokeTrail")));
            Shaders.Add("RedsIllness", FShader.CreateShader("RedsIllness", Shader.Find("Futile/RedsIllness")));
            Shaders.Add("HazerHaze", FShader.CreateShader("HazerHaze", Shader.Find("Futile/HazerHaze")));
            Shaders.Add("Rainbow", FShader.CreateShader("Rainbow", Shader.Find("Futile/Rainbow")));
            Shaders.Add("LightBeam", FShader.CreateShader("LightBeam", Shader.Find("Futile/LightBeam")));
        LoadResources();
            Shader.SetGlobalVector("_screenSize", screenSize);
            Shader.SetGlobalColor("_MapCol", MapColor);
            inGameTranslator = new InGameTranslator(this);
            processManager = new ProcessManager(this);
        }

    public void ApplyPatch()
    {
        StaticWorldPatch.AddCreatureTemplate();
        StaticWorldPatch.ModifyRelationship();
    }

    private void Awake()
    {
     /*   
        if (File.Exists("exceptionLog.txt"))
        {
            File.Delete("exceptionLog.txt");
        }
        if (File.Exists("consoleLog.txt"))
        {
            File.Delete("consoleLog.txt");
        }
        Application.RegisterLogCallback(new Application.LogCallback(this.HandleLog));
        */
        ApplyPatch();
        patch_MultiplayerUnlocks.CreatureUnlockList = new patch_MultiplayerUnlocks.SandboxUnlockID[]
        {
          patch_MultiplayerUnlocks.SandboxUnlockID.Slugcat,
          patch_MultiplayerUnlocks.SandboxUnlockID.GreenLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.PinkLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.BlueLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.WhiteLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.BlackLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.YellowLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.CyanLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.RedLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.Salamander,
          patch_MultiplayerUnlocks.SandboxUnlockID.Fly,
          patch_MultiplayerUnlocks.SandboxUnlockID.CicadaA,
          patch_MultiplayerUnlocks.SandboxUnlockID.CicadaB,
          patch_MultiplayerUnlocks.SandboxUnlockID.Snail,
          patch_MultiplayerUnlocks.SandboxUnlockID.Leech,
          patch_MultiplayerUnlocks.SandboxUnlockID.SeaLeech,
          patch_MultiplayerUnlocks.SandboxUnlockID.PoleMimic,
          patch_MultiplayerUnlocks.SandboxUnlockID.TentaclePlant,
          patch_MultiplayerUnlocks.SandboxUnlockID.Scavenger,
          patch_MultiplayerUnlocks.SandboxUnlockID.VultureGrub,
          patch_MultiplayerUnlocks.SandboxUnlockID.Vulture,
          patch_MultiplayerUnlocks.SandboxUnlockID.KingVulture,
          patch_MultiplayerUnlocks.SandboxUnlockID.SmallCentipede,
          patch_MultiplayerUnlocks.SandboxUnlockID.MediumCentipede,
          patch_MultiplayerUnlocks.SandboxUnlockID.BigCentipede,
          patch_MultiplayerUnlocks.SandboxUnlockID.RedCentipede,
          patch_MultiplayerUnlocks.SandboxUnlockID.Centiwing,
          patch_MultiplayerUnlocks.SandboxUnlockID.TubeWorm,
          patch_MultiplayerUnlocks.SandboxUnlockID.Hazer,
          patch_MultiplayerUnlocks.SandboxUnlockID.LanternMouse,
          patch_MultiplayerUnlocks.SandboxUnlockID.Spider,
          patch_MultiplayerUnlocks.SandboxUnlockID.BigSpider,
          patch_MultiplayerUnlocks.SandboxUnlockID.SpitterSpider,
          patch_MultiplayerUnlocks.SandboxUnlockID.MirosBird,
          patch_MultiplayerUnlocks.SandboxUnlockID.BrotherLongLegs,
          patch_MultiplayerUnlocks.SandboxUnlockID.DaddyLongLegs,
          patch_MultiplayerUnlocks.SandboxUnlockID.Deer,
          patch_MultiplayerUnlocks.SandboxUnlockID.EggBug,
          patch_MultiplayerUnlocks.SandboxUnlockID.DropBug,
          patch_MultiplayerUnlocks.SandboxUnlockID.BigNeedleWorm,
          patch_MultiplayerUnlocks.SandboxUnlockID.SmallNeedleWorm,
          patch_MultiplayerUnlocks.SandboxUnlockID.Jetfish,
          patch_MultiplayerUnlocks.SandboxUnlockID.BigEel,
          patch_MultiplayerUnlocks.SandboxUnlockID.WalkerBeast,
          patch_MultiplayerUnlocks.SandboxUnlockID.GreyLizard,
          patch_MultiplayerUnlocks.SandboxUnlockID.SeaDrake
        };

        patch_MultiplayerUnlocks.ItemUnlockList = new patch_MultiplayerUnlocks.SandboxUnlockID[]
        {
          patch_MultiplayerUnlocks.SandboxUnlockID.Rock,
          patch_MultiplayerUnlocks.SandboxUnlockID.Spear,
          patch_MultiplayerUnlocks.SandboxUnlockID.FireSpear,
          patch_MultiplayerUnlocks.SandboxUnlockID.ScavengerBomb,
          patch_MultiplayerUnlocks.SandboxUnlockID.SporePlant,
          patch_MultiplayerUnlocks.SandboxUnlockID.Lantern,
          patch_MultiplayerUnlocks.SandboxUnlockID.FlyLure,
          patch_MultiplayerUnlocks.SandboxUnlockID.Mushroom,
          patch_MultiplayerUnlocks.SandboxUnlockID.FlareBomb,
          patch_MultiplayerUnlocks.SandboxUnlockID.PuffBall,
          patch_MultiplayerUnlocks.SandboxUnlockID.WaterNut,
          patch_MultiplayerUnlocks.SandboxUnlockID.FirecrackerPlant,
          patch_MultiplayerUnlocks.SandboxUnlockID.DangleFruit,
          patch_MultiplayerUnlocks.SandboxUnlockID.JellyFish,
          patch_MultiplayerUnlocks.SandboxUnlockID.BubbleGrass,
          patch_MultiplayerUnlocks.SandboxUnlockID.SlimeMold
        };
    }
    public static RainWorldGame.SetupValues LoadSetupValues(bool distributionBuild)
    {
        if (distributionBuild || !System.IO.File.Exists(Custom.RootFolderDirectory() + "setup.txt"))
        {
            return new RainWorldGame.SetupValues(false, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false, 4, true, true, false, true, true, false, 0, 0, 0, false, 0, 0, 0, 0, 0, 0, true, 0, 0, false, 0, 0, 0, 0, 0, 0, true, false, 0, 0, true, 8, 18, false, 0, 0, 0, 0, 0, true, 128, true, 300, 1000, 0, false, 0, 0, 0, 0, false, 0, false, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, false, 0, 0);
        }
        string[] array = System.IO.File.ReadAllLines(Custom.RootFolderDirectory() + "setup.txt");
        int[] array2 = new int[82];
        for (int i = 1; i < array.Length; i++)
        {
            int num = -1;
            string text = Regex.Split(array[i], ": ")[0];
            switch (text)
            {
                case "player 2 active":
                    num = 0;
                    break;
                case "pink":
                    num = 1;
                    break;
                case "green":
                    num = 2;
                    break;
                case "blue":
                    num = 3;
                    break;
                case "white":
                    num = 4;
                    break;
                case "spears":
                    num = 5;
                    break;
                case "flies":
                    num = 6;
                    break;
                case "leeches":
                    num = 7;
                    break;
                case "snails":
                    num = 8;
                    break;
                case "vultures":
                    num = 9;
                    break;
                case "lantern mice":
                    num = 10;
                    break;
                case "cicadas":
                    num = 11;
                    break;
                case "palette":
                    num = 12;
                    break;
                case "lizard laser eyes":
                    num = 13;
                    break;
                case "player invincibility":
                    num = 14;
                    break;
                case "cycle time min in seconds":
                    num = 15;
                    break;
                case "cycle time max in seconds":
                    num = 59;
                    break;
                case "flies to win":
                    num = 16;
                    break;
                case "world creatures spawn":
                    num = 17;
                    break;
                case "don't bake":
                    num = 18;
                    break;
                case "widescreen":
                    num = 19;
                    break;
                case "start screen":
                    num = 20;
                    break;
                case "cycle startup":
                    num = 21;
                    break;
                case "full screen":
                    num = 22;
                    break;
                case "yellow":
                    num = 23;
                    break;
                case "red":
                    num = 24;
                    break;
                case "spiders":
                    num = 25;
                    break;
                case "player glowing":
                    num = 26;
                    break;
                case "garbage worms":
                    num = 27;
                    break;
                case "jet fish":
                    num = 28;
                    break;
                case "black":
                    num = 29;
                    break;
                case "sea leeches":
                    num = 30;
                    break;
                case "salamanders":
                    num = 31;
                    break;
                case "big eels":
                    num = 32;
                    break;
                case "default settings screen":
                    num = 33;
                    break;
                case "player 1 active":
                    num = 34;
                    break;
                case "deer":
                    num = 35;
                    break;
                case "dev tools active":
                    num = 36;
                    break;
                case "daddy long legs":
                    num = 37;
                    break;
                case "tube worms":
                    num = 38;
                    break;
                case "bro long legs":
                    num = 39;
                    break;
                case "tentacle plants":
                    num = 40;
                    break;
                case "pole mimics":
                    num = 41;
                    break;
                case "miros birds":
                    num = 42;
                    break;
                case "load game":
                    num = 43;
                    break;
                case "multi use gates":
                    num = 44;
                    break;
                case "temple guards":
                    num = 45;
                    break;
                case "centipedes":
                    num = 46;
                    break;
                case "world":
                    num = 47;
                    break;
                case "gravity flicker cycle min":
                    num = 48;
                    break;
                case "gravity flicker cycle max":
                    num = 49;
                    break;
                case "reveal map":
                    num = 50;
                    break;
                case "scavengers":
                    num = 51;
                    break;
                case "scavengers shy":
                    num = 52;
                    break;
                case "scavenger like player":
                    num = 53;
                    break;
                case "centiwings":
                    num = 54;
                    break;
                case "small centipedes":
                    num = 55;
                    break;
                case "load progression":
                    num = 56;
                    break;
                case "lungs":
                    num = 57;
                    break;
                case "play music":
                    num = 58;
                    break;
                case "cheat karma":
                    num = 60;
                    break;
                case "load all ambient sounds":
                    num = 61;
                    break;
                case "overseers":
                    num = 62;
                    break;
                case "ghosts":
                    num = 63;
                    break;
                case "fire spears":
                    num = 64;
                    break;
                case "scavenger lanterns":
                    num = 65;
                    break;
                case "always travel":
                    num = 66;
                    break;
                case "scavenger bombs":
                    num = 67;
                    break;
                case "the mark":
                    num = 68;
                    break;
                case "custom":
                    num = 69;
                    break;
                case "big spiders":
                    num = 70;
                    break;
                case "egg bugs":
                    num = 71;
                    break;
                case "single player character":
                    num = 72;
                    break;
                case "needle worms":
                    num = 73;
                    break;
                case "small needle worms":
                    num = 74;
                    break;
                case "spitter spiders":
                    num = 75;
                    break;
                case "dropwigs":
                    num = 76;
                    break;
                case "cyan":
                    num = 77;
                    break;
                case "king vultures":
                    num = 78;
                    break;
                case "log spawned creatures":
                    num = 79;
                    break;
                case "red centipedes":
                    num = 80;
                    break;
                case "proceed lineages":
                    num = 81;
                    break;
            }
            if (num != -1)
            {
                array2[num] = (int)short.Parse(Regex.Split(array[i], ": ")[1]);
            }
            else
            {
                Debug.Log("Couldn't find option: " + Regex.Split(array[i], ": ")[0]);
            }
        }
        return new RainWorldGame.SetupValues(array2[0] > 0, array2[1], array2[2], array2[3], array2[4], array2[5], array2[6], array2[7], array2[8], array2[9], array2[10], array2[11], array2[12], array2[13] != 0, array2[14] != 0, array2[16], array2[17] == 1, array2[18] == 1, array2[19] == 1, array2[20] == 1, array2[21] == 1, array2[22] == 1, array2[23], array2[24], array2[25], array2[26] == 1, array2[27], array2[28], array2[29], array2[30], array2[31], array2[32], array2[34] == 1, array2[33], array2[35], array2[36] == 1, array2[37], array2[38], array2[39], array2[40], array2[41], array2[42], array2[43] == 1, array2[44] == 1, array2[45], array2[46], array2[47] == 1, array2[48], array2[49], array2[50] == 1, array2[51], array2[52], array2[53], array2[54], array2[55], array2[56] == 1, array2[57], array2[58] == 1, array2[15], array2[59], array2[60], array2[61] == 1, array2[62], array2[63], array2[64], array2[65], array2[66] == 1, array2[67], array2[68] == 1, array2[69], array2[70], array2[71], array2[72], array2[73], array2[74], array2[75], array2[76], array2[77], array2[78], array2[79] == 1, array2[80], array2[81]);
    }
}



