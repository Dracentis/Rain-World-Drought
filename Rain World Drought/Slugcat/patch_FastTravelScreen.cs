using MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;

[MonoModPatch("global::Menu.FastTravelScreen")]
class patch_FastTravelScreen : Menu.FastTravelScreen
{
    [MonoModIgnore]
    public patch_FastTravelScreen(ProcessManager manager, ProcessManager.ProcessID ID) : base(manager, ID)
    {
    }

	Dictionary<string, int> dictionaryTemp = new Dictionary<string, int>(12);

	public static List<string> GetRegionOrder()
	{
		List<string> list = new List<string>();
		list.Add("SU");
		list.Add("HI");
		list.Add("DS");
		list.Add("CC");
		list.Add("GW");
		list.Add("SH");
		list.Add("SL");
		list.Add("SI");
		list.Add("LF");
		list.Add("UW");
		list.Add("SS");
		list.Add("SB");
		list.Add("IS");
		list.Add("FS");
		list.Add("MW");
		list.Add("LM");
		return list;
	}

	private extern Menu.MenuScene.SceneID orig_TitleSceneID(string regionName);

	public Menu.MenuScene.SceneID TitleSceneID(string regionName) 
	{
		if (regionName != null)
		{
			if (dictionaryTemp == null)
		{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("SU", 0);
				dictionary.Add("HI", 1);
				dictionary.Add("DS", 2);
				dictionary.Add("CC", 3);
				dictionary.Add("GW", 4);
				dictionary.Add("SH", 5);
				dictionary.Add("SL", 6);
				dictionary.Add("SI", 7);
				dictionary.Add("LF", 8);
				dictionary.Add("UW", 9);
				dictionary.Add("SS", 10);
				dictionary.Add("SB", 11);
				dictionary.Add("IS", 12);
				dictionary.Add("FS", 13);
				dictionary.Add("MW", 14);
				dictionary.Add("LM", 15);
				dictionaryTemp = dictionary;
			}
			int num;
			if (dictionaryTemp.TryGetValue(regionName, out num))
		{
				switch (num)
				{
					case 0:
						return MenuScene.SceneID.Landscape_SU;
					case 1:
						return MenuScene.SceneID.Landscape_HI;
					case 2:
						return MenuScene.SceneID.Landscape_DS;
					case 3:
						return MenuScene.SceneID.Landscape_CC;
					case 4:
						return MenuScene.SceneID.Landscape_GW;
					case 5:
						return MenuScene.SceneID.Landscape_SH;
					case 6:
						return MenuScene.SceneID.Landscape_SL;
					case 7:
						return MenuScene.SceneID.Landscape_SI;
					case 8:
						return MenuScene.SceneID.Landscape_LF;
					case 9:
						return MenuScene.SceneID.Landscape_UW;
					case 10:
						return MenuScene.SceneID.Landscape_SS;
					case 11:
						return MenuScene.SceneID.Landscape_SB;
					case 12:
						return (MenuScene.SceneID)patch_MenuScene.SceneID.Landscape_IS;
					case 13:
						return (MenuScene.SceneID)patch_MenuScene.SceneID.Landscape_FS;
					case 14:
						return (MenuScene.SceneID)patch_MenuScene.SceneID.Landscape_MW;
					case 15:
						return (MenuScene.SceneID)patch_MenuScene.SceneID.Landscape_LM;
				}
			}
		}
		return MenuScene.SceneID.Empty;
	}

}

