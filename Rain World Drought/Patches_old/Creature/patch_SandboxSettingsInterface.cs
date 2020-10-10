using Menu;
using MonoMod;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using RWCustom;
using System.Collections.Generic;

[MonoModPatch("global::Menu.SandboxSettingsInterface")]
class patch_SandboxSettingsInterface : Menu.SandboxSettingsInterface
{
    [MonoModIgnore]
    public patch_SandboxSettingsInterface(Menu.Menu menu, MenuObject owner) : base(menu, owner)
    {
    }

    [MonoModIgnore]
    public extern void orig_ctor(Menu.Menu menu, MenuObject owner);

    [MonoModConstructor]
    public void ctor(Menu.Menu menu, MenuObject owner)
    {
        //Delegate to call the base constructor
        Type[] constructorSignature = new Type[3];
        constructorSignature[0] = typeof(Menu.Menu);
        constructorSignature[1] = typeof(MenuObject);
        constructorSignature[2] = typeof(Vector2);
        RuntimeMethodHandle handle = typeof(Menu.PositionedMenuObject).GetConstructor(constructorSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action<Menu.Menu, MenuObject, Vector2> funct = (Action<Menu.Menu, MenuObject, Vector2>)Activator.CreateInstance(typeof(Action<Menu.Menu, MenuObject, Vector2>), this, ptr);
        funct(menu, owner, new Vector2(440f, 385f));//RectangularMenuObject Constructor

        this.scoreControllers = new List<SandboxSettingsInterface.ScoreController>();
        IntVector2 intVector = new IntVector2(0, 0);
        //Replaced Code
        foreach (patch_MultiplayerUnlocks.SandboxUnlockID creature in patch_MultiplayerUnlocks.CreatureUnlockList)
        {
            if (creature != patch_MultiplayerUnlocks.SandboxUnlockID.Fly &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.Leech &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.SeaLeech &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.SmallNeedleWorm &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.Spider &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.VultureGrub &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.BigEel &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.Deer &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.WalkerBeast &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.SmallCentipede &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.TubeWorm &&
            creature != patch_MultiplayerUnlocks.SandboxUnlockID.Hazer)
                this.AddScoreButton((MultiplayerUnlocks.SandboxUnlockID)creature, ref intVector);
        }
        //-=-=-=-=-=-=-=-=-=-=-
        //for (int j = 0; j < 1; j++)
        //{
        //    this.AddScoreButton(null, ref intVector);
        //}
        this.AddScoreButton(new SandboxSettingsInterface.MiscScore(menu, this, menu.Translate("Food"), "FOODSCORE"), ref intVector);
        this.AddScoreButton(new SandboxSettingsInterface.MiscScore(menu, this, menu.Translate("Survive"), "SURVIVESCORE"), ref intVector);
        this.AddScoreButton(new SandboxSettingsInterface.MiscScore(menu, this, menu.Translate("Spear hit"), "SPEARHITSCORE"), ref intVector);
        if (menu.CurrLang != InGameTranslator.LanguageID.English)
        {
            for (int k = 1; k < 4; k++)
            {
                SandboxSettingsInterface.ScoreController scoreController = this.scoreControllers[this.scoreControllers.Count - k];
                scoreController.pos.x = scoreController.pos.x + 24f;
            }
        }
        this.subObjects.Add(new SymbolButton(menu, this, "Menu_Symbol_Clear_All", "CLEARSCORES", new Vector2(0f, -280f)));
        for (int l = 0; l < this.subObjects.Count; l++)
        {
            if (this.subObjects[l] is SandboxSettingsInterface.ScoreController)
            {
                (this.subObjects[l] as SandboxSettingsInterface.ScoreController).scoreDragger.UpdateScoreText();
            }
        }
    }

    private void AddScoreButton(SandboxSettingsInterface.ScoreController button, ref IntVector2 ps)
    {
        if (button != null)
        {
            this.scoreControllers.Add(button);
            this.subObjects.Add(button);
            button.pos = new Vector2((float)ps.x * 88.666f + 0.01f, (float)ps.y * -30f);
        }
        ps.y++;
        if (ps.y > 8 && ps.x != 3)
        {
            ps.y = 0;
            ps.x++;
        }
    }

    public static void DefaultKillScores(ref int[] killScores)
    {
        killScores[0] = 5;
        killScores[1] = 10;
        killScores[2] = 7;
        killScores[3] = 6;
        killScores[4] = 8;
        killScores[5] = 7;
        killScores[6] = 6;
        killScores[7] = 9;
        killScores[8] = 25;
        killScores[9] = 7;
        killScores[11] = 2;
        killScores[12] = 2;
        killScores[13] = 1;
        killScores[16] = 2;
        killScores[17] = 7;
        killScores[18] = 6;
        killScores[20] = 15;
        killScores[21] = 25;
        killScores[23] = 4;
        killScores[24] = 7;
        killScores[25] = 19;
        killScores[26] = 5;
        killScores[29] = 2;
        killScores[31] = 4;
        killScores[32] = 5;
        killScores[33] = 16;
        killScores[34] = 14;
        killScores[35] = 25;
        killScores[27] = 1;
        killScores[37] = 2;
        killScores[38] = 5;
        killScores[39] = 5;
        killScores[41] = 4;
        killScores[42] = 25;
        killScores[60] = 15;
        killScores[61] = 10;
    }
}

