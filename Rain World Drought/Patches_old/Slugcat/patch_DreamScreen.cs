using HUD;
using Menu;
using MonoMod;
using RWCustom;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

[MonoModPatch("global::Menu.DreamScreen")]
class patch_DreamScreen : Menu.DreamScreen
{
    [MonoModIgnore]
    public patch_DreamScreen(ProcessManager manager) : base(manager)
    {
    }


    public extern void orig_ctor(ProcessManager manager);

    [MonoModConstructor]
    public void ctor(ProcessManager manager)
    {
        orig_ctor(manager);
    }

    public void GetDataFromGame(DreamsState.DreamID dreamID, KarmaLadderScreen.SleepDeathScreenDataPackage package)
    {
        this.fromGameDataPackage = package;
        this.dreamID = new DreamsState.DreamID?(dreamID);
    }

    public override void Update()
    {
        //Delegate to call the base Update
        Type[] updateSignature = new Type[0];
        RuntimeMethodHandle handle = typeof(Menu.Menu).GetMethod("Update", updateSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action funct = (Action)Activator.CreateInstance(typeof(Action), this, ptr);
        funct();//Menu.Menu Update Method

        this.counter++;
        if (!this.initSound)
        {
            base.PlaySound(SoundID.MENU_Dream_Init);
            this.initSound = true;
        }
        if (this.counter > 35 && this.scene.sceneID == MenuScene.SceneID.SleepScreen && this.dreamID != null)
        {
            this.manager.fadeToBlack = Custom.LerpAndTick(this.manager.fadeToBlack, 1f, 0f, 1f / Mathf.Lerp(450f, 210f, this.manager.fadeToBlack));
            if (this.manager.fadeToBlack > 0.9f)
            {
                base.PlaySound(SoundID.MENU_Dream_Switch);
                this.manager.fadeToBlack = 1f;
                this.scene.RemoveSprites();
                this.pages[0].subObjects.Remove(this.scene);
                this.scene = new InteractiveMenuScene(this, this.pages[0], this.SceneFromDream(this.dreamID.Value));
                this.pages[0].subObjects.Add(this.scene);
                this.dreamID = null;
            }
        }
        else
        {
            this.manager.fadeToBlack = Custom.LerpAndTick(this.manager.fadeToBlack, 0f, 0f, 0.0125f);
            if (this.exitButton != null)
            {
                this.exitButton.buttonBehav.greyedOut = this.FreezeMenuFunctions;
                this.exitButton.black = Math.Max(0f, this.exitButton.black - 0.0125f);
            }
            else if (this.counter > endTime)
            {
                this.exitButton = new SimpleButton(this, this.pages[0], base.Translate("CONTINUE"), "CONTINUE", new Vector2(this.manager.rainWorld.options.ScreenSize.x - 320f + (1366f - this.manager.rainWorld.options.ScreenSize.x), 15f), new Vector2(110f, 30f));
                this.pages[0].subObjects.Add(this.exitButton);
                this.pages[0].lastSelectedObject = this.exitButton;
                this.exitButton.black = 1f;
            }
        }
    }

    public int endTime = 340;

    public MenuScene.SceneID SceneFromDream(DreamsState.DreamID dreamID)
    {
        if (dreamID >= (DreamsState.DreamID)patch_DreamsState.DreamID.SRSDreamPearlLF)
        {
            return (MenuScene.SceneID)patch_MenuScene.SceneID.Dream_Message;
        }
        switch (dreamID)
        {
            case DreamsState.DreamID.MoonFriend:
                return MenuScene.SceneID.Dream_Moon_Friend;
            case DreamsState.DreamID.MoonThief:
                return MenuScene.SceneID.Dream_Moon_Betrayal;
            case DreamsState.DreamID.Pebbles:
                return MenuScene.SceneID.Dream_Pebbles;
            case DreamsState.DreamID.GuideA:
                return MenuScene.SceneID.Dream_Iggy;
            case DreamsState.DreamID.GuideB:
                return MenuScene.SceneID.Dream_Iggy_Image;
            case DreamsState.DreamID.GuideC:
                return MenuScene.SceneID.Dream_Iggy_Doubt;
            case DreamsState.DreamID.FamilyA:
                return MenuScene.SceneID.Dream_Sleep;
            case DreamsState.DreamID.FamilyB:
                return MenuScene.SceneID.Dream_Sleep_Fade;
            case DreamsState.DreamID.FamilyC:
                return MenuScene.SceneID.Dream_Acceptance;
            case DreamsState.DreamID.VoidDreamSlugcatUp:
                return MenuScene.SceneID.Void_Slugcat_Upright;
            case DreamsState.DreamID.VoidDreamSlugcatDown:
                return MenuScene.SceneID.Void_Slugcat_Down;
            default:
                return MenuScene.SceneID.Empty;
        }
    }
    

    

}
