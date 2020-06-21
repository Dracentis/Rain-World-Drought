using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using Menu;

[MonoModPatch("global::Menu.MainMenu")]
class patch_MainMenu : MainMenu
{
	[MonoModIgnore]
	public patch_MainMenu(ProcessManager manager, bool showRegionSpecificBkg) : base(manager, showRegionSpecificBkg)
	{
	}

	public extern MenuScene.SceneID orig_BackgroundScene();

	public MenuScene.SceneID BackgroundScene()
	{
		if (this.manager.rainWorld.progression.miscProgressionData.menuRegion == null)
		{
			return this.orig_BackgroundScene();
		}
		string menuRegion = this.manager.rainWorld.progression.miscProgressionData.menuRegion;
        switch (menuRegion)
        {
            case "IS":
                return (MenuScene.SceneID)patch_MenuScene.SceneID.Landscape_IS;
            case "FS":
                return (MenuScene.SceneID)patch_MenuScene.SceneID.Landscape_FS;
            case "MW":
                return (MenuScene.SceneID)patch_MenuScene.SceneID.Landscape_MW;
            case "LM":
                return (MenuScene.SceneID)patch_MenuScene.SceneID.Landscape_LM;
        }
        return this.orig_BackgroundScene();
    }
}

