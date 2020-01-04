using Menu;
using MonoMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using RWCustom;
using UnityEngine;

[MonoModPatch("global::Menu.MenuScene")]
abstract class patch_MenuScene : Menu.MenuScene
{
    [MonoModIgnore]
    public patch_MenuScene(Menu.Menu menu, MenuObject owner, Menu.MenuScene.SceneID sceneID) : base(menu, owner, sceneID)
    {
    }
    private extern void orig_BuildScene();

    private void BuildScene() {
        orig_BuildScene();
        if (sceneID == (MenuScene.SceneID)patch_MenuScene.SceneID.Dream_Message)
        {
            if ((this as MenuScene) is InteractiveMenuScene)
            {
                ((this as MenuScene) as InteractiveMenuScene).idleDepths = new List<float>();
            }
            Vector2 vector = new Vector2(0f, 0f);
            this.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Dream - Message";
            if (this.flatMode)
            {
                this.AddIllustration(new MenuIllustration(this.menu, this, this.sceneFolder, "Dream - Message - Flat", new Vector2(683f, 384f), false, true));
            }
            else
            {
                this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 6", new Vector2(71f, 49f), 5f, MenuDepthIllustration.MenuShader.LightEdges));
                this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 5", new Vector2(71f, 49f), 3f, MenuDepthIllustration.MenuShader.Lighten));
                this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 4", new Vector2(71f, 49f), 2.5f, MenuDepthIllustration.MenuShader.Normal));
                this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 3", new Vector2(71f, 49f), 2.2f, MenuDepthIllustration.MenuShader.Normal));
                this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 2", new Vector2(71f, 49f), 2.3f, MenuDepthIllustration.MenuShader.Normal));
                this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 1", new Vector2(71f, 49f), 1.5f, MenuDepthIllustration.MenuShader.Lighten));
                ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(3f);
                ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.5f);
                ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.3f);
                ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.3f);
                ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(1.4f);
                this.AddIllustration(new MenuIllustration(this.menu, this, this.sceneFolder, "Message - Symbol", new Vector2(683f, 70f), true, false));
                MenuIllustration menuIllustration = this.flatIllustrations[this.flatIllustrations.Count - 1];
                menuIllustration.pos.x = menuIllustration.pos.x - this.flatIllustrations[this.flatIllustrations.Count - 1].size.x / 2f;
            }
            if (this.sceneFolder != string.Empty && File.Exists(string.Concat(new object[]
    {
        Custom.RootFolderDirectory(),
        Path.DirectorySeparatorChar,
        "Assets",
        Path.DirectorySeparatorChar,
        "Futile",
        Path.DirectorySeparatorChar,
        "Resources",
        Path.DirectorySeparatorChar,
        this.sceneFolder,
        Path.DirectorySeparatorChar,
        "positions.txt"
    })))
            {
                string[] array = File.ReadAllLines(string.Concat(new object[]
                {
            Custom.RootFolderDirectory(),
            Path.DirectorySeparatorChar,
            "Assets",
            Path.DirectorySeparatorChar,
            "Futile",
            Path.DirectorySeparatorChar,
            "Resources",
            Path.DirectorySeparatorChar,
            this.sceneFolder,
            Path.DirectorySeparatorChar,
            "positions.txt"
                }));
                int num2 = 0;
                while (num2 < array.Length && num2 < this.depthIllustrations.Count)
                {
                    this.depthIllustrations[num2].pos.x = float.Parse(Regex.Split(array[num2], ", ")[0]) + vector.x;
                    this.depthIllustrations[num2].pos.y = float.Parse(Regex.Split(array[num2], ", ")[1]) + vector.y;
                    this.depthIllustrations[num2].lastPos = this.depthIllustrations[num2].pos;
                    num2++;
                }
            }
        }
    }


    public enum SceneID
    {
        Empty,
        MainMenu,
        SleepScreen,
        RedsDeathStatisticsBkg,
        NewDeath,
        StarveScreen,
        Intro_1_Tree,
        Intro_2_Branch,
        Intro_3_In_Tree,
        Intro_4_Walking,
        Intro_5_Hunting,
        Intro_6_7_Rain_Drop,
        Intro_8_Climbing,
        Intro_9_Rainy_Climb,
        Intro_10_Fall,
        Intro_10_5_Separation,
        Intro_11_Drowning,
        Intro_12_Waking,
        Intro_13_Alone,
        Intro_14_Title,
        Endgame_Survivor,
        Endgame_Hunter,
        Endgame_Saint,
        Endgame_Traveller,
        Endgame_Chieftain,
        Endgame_Monk,
        Endgame_Outlaw,
        Endgame_DragonSlayer,
        Endgame_Martyr,
        Endgame_Scholar,
        Endgame_Mother,
        Endgame_Friend,
        Landscape_CC,
        Landscape_DS,
        Landscape_GW,
        Landscape_HI,
        Landscape_LF,
        Landscape_SB,
        Landscape_SH,
        Landscape_SI,
        Landscape_SL,
        Landscape_SS,
        Landscape_SU,
        Landscape_UW,
        Outro_1_Left_Swim,
        Outro_2_Up_Swim,
        Outro_3_Face,
        Outro_4_Tree,
        Options_Bkg,
        Dream_Sleep,
        Dream_Sleep_Fade,
        Dream_Acceptance,
        Dream_Iggy,
        Dream_Iggy_Doubt,
        Dream_Iggy_Image,
        Dream_Moon_Betrayal,
        Dream_Moon_Friend,
        Dream_Pebbles,
        Void_Slugcat_Upright,
        Void_Slugcat_Down,
        Slugcat_White,
        Slugcat_Yellow,
        Slugcat_Red,
        Ghost_White,
        Ghost_Yellow,
        Ghost_Red,
        Yellow_Intro_A,
        Yellow_Intro_B,
        Slugcat_Dead_Red,
        Red_Ascend,
        Dream_Message
    }

    /*
            case (MenuScene.SceneID)patch_MenuScene.SceneID.Dream_Message:
                this.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Dream - Message";
                if (this.flatMode)
                {
                    this.AddIllustration(new MenuIllustration(this.menu, this, this.sceneFolder, "Dream - Message - Flat", new Vector2(683f, 384f), false, true));
                }
                else
                {
                    this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 6", new Vector2(71f, 49f), 5f, MenuDepthIllustration.MenuShader.LightEdges));
                    this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 5", new Vector2(71f, 49f), 3f, MenuDepthIllustration.MenuShader.Lighten));
                    this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 4", new Vector2(71f, 49f), 2.5f, MenuDepthIllustration.MenuShader.Normal));
                    this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 3", new Vector2(71f, 49f), 2.2f, MenuDepthIllustration.MenuShader.Normal));
                    this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 2", new Vector2(71f, 49f), 2.3f, MenuDepthIllustration.MenuShader.Normal));
                    this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Message - 1", new Vector2(71f, 49f), 1.5f, MenuDepthIllustration.MenuShader.Lighten));
                    ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(3f);
                    ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.5f);
                    ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.3f);
                    ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(2.3f);
                    ((this as MenuScene) as Menu.InteractiveMenuScene).idleDepths.Add(1.4f);
                    MenuIllustration menuIllustration = this.flatIllustrations[this.flatIllustrations.Count - 1];
                    menuIllustration.pos.x = menuIllustration.pos.x - this.flatIllustrations[this.flatIllustrations.Count - 1].size.x / 2f;
                }
                break;*/
}
