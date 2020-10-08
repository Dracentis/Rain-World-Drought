using Menu;
using MonoMod;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using RWCustom;
using System.Collections.Generic;

[MonoModPatch("global::Menu.SandboxEditorSelector")]
class patch_SandboxEditorSelector : Menu.SandboxEditorSelector
{
    [MonoModIgnore]
    public patch_SandboxEditorSelector(Menu.Menu menu, MenuObject owner, SandboxOverlayOwner overlayOwner) : base(menu, owner, overlayOwner)
    {
    }

    [MonoModIgnore]
    public extern void orig_ctor(Menu.Menu menu, MenuObject owner, SandboxOverlayOwner overlayOwner);

    [MonoModConstructor]
    public void ctor(Menu.Menu menu, MenuObject owner, SandboxOverlayOwner overlayOwner)
    {
        //Delegate to call the base constructor
        Type[] constructorSignature = new Type[4];
        constructorSignature[0] = typeof(Menu.Menu);
        constructorSignature[1] = typeof(MenuObject);
        constructorSignature[2] = typeof(Vector2);
        constructorSignature[3] = typeof(Vector2);
        RuntimeMethodHandle handle = typeof(RectangularMenuObject).GetConstructor(constructorSignature).MethodHandle;
        RuntimeHelpers.PrepareMethod(handle);
        IntPtr ptr = handle.GetFunctionPointer();
        Action<Menu.Menu, MenuObject, Vector2, Vector2> funct = (Action<Menu.Menu, MenuObject, Vector2, Vector2>)Activator.CreateInstance(typeof(Action<Menu.Menu, MenuObject, Vector2, Vector2>), this, ptr);
        funct(menu, owner, new Vector2(-1000f, -1000f), new Vector2((float)SandboxEditorSelector.Width, (float)SandboxEditorSelector.Height) * SandboxEditorSelector.ButtonSize);//RectangularMenuObject Constructor
        
        this.lastPos = new Vector2(-1000f, -1000f);
        this.overlayOwner = overlayOwner;
        overlayOwner.selector = this;
        this.bkgRect = new RoundedRect(menu, this, new Vector2(-10f, -30f), this.size + new Vector2(20f, 60f), true);
        this.subObjects.Add(this.bkgRect);
        this.infoLabel = new MenuLabel(menu, this, string.Empty, new Vector2(this.size.x / 2f - 100f, 0f), new Vector2(200f, 20f), false);
        this.subObjects.Add(this.infoLabel);
        this.buttons = new SandboxEditorSelector.Button[SandboxEditorSelector.Width, SandboxEditorSelector.Height];
        int num = 0;
        this.AddButton(new SandboxEditorSelector.RectButton(menu, this, SandboxEditorSelector.ActionButton.Action.ClearAll), ref num);
        for (int i = 0; i < 2; i++)
        {
            this.AddButton(null, ref num);
        }
        //Replaced Section
        foreach (MultiplayerUnlocks.SandboxUnlockID item in patch_MultiplayerUnlocks.ItemUnlockList)
        {
            if (this.unlocks.SandboxItemUnlocked(item))
                this.AddButton(new SandboxEditorSelector.CreatureOrItemButton(menu, this, MultiplayerUnlocks.SymbolDataForSandboxUnlock(item)), ref num);
            else
                this.AddButton(new SandboxEditorSelector.LockedButton(menu, this), ref num);
        }
        foreach (MultiplayerUnlocks.SandboxUnlockID creature in patch_MultiplayerUnlocks.CreatureUnlockList)
        {
            if (this.unlocks.SandboxItemUnlocked(creature))
                this.AddButton(new SandboxEditorSelector.CreatureOrItemButton(menu, this, MultiplayerUnlocks.SymbolDataForSandboxUnlock(creature)), ref num);
            else
                this.AddButton(new SandboxEditorSelector.LockedButton(menu, this), ref num);
        }
        //-=-=-=-=-=-=-=-=-=-=-
        this.AddButton(new SandboxEditorSelector.RectButton(menu, this, SandboxEditorSelector.ActionButton.Action.Play), SandboxEditorSelector.Width - 1, 0);
        this.AddButton(new SandboxEditorSelector.RandomizeButton(menu, this), SandboxEditorSelector.Width - 6, 0);
        this.AddButton(new SandboxEditorSelector.ConfigButton(menu, this, SandboxEditorSelector.ActionButton.Action.ConfigA, 0), SandboxEditorSelector.Width - 5, 0);
        this.AddButton(new SandboxEditorSelector.ConfigButton(menu, this, SandboxEditorSelector.ActionButton.Action.ConfigB, 1), SandboxEditorSelector.Width - 4, 0);
        this.AddButton(new SandboxEditorSelector.ConfigButton(menu, this, SandboxEditorSelector.ActionButton.Action.ConfigC, 2), SandboxEditorSelector.Width - 3, 0);
        for (int l = 0; l < SandboxEditorSelector.Width; l++)
        {
            for (int m = 0; m < SandboxEditorSelector.Height; m++)
            {
                if (this.buttons[l, m] != null)
                {
                    this.buttons[l, m].Initiate(new IntVector2(l, m));
                }
            }
        }
        this.cursors = new List<SandboxEditorSelector.ButtonCursor>();
    }
}

