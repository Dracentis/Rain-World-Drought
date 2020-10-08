using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MonoMod;

// Token: 0x02000007 RID: 7
[MonoModPatch("global::WinState")]
internal class patch_WinState : WinState
{
	// Token: 0x06000015 RID: 21 RVA: 0x000029D8 File Offset: 0x00000BD8
	[MonoModIgnore]
	public patch_WinState()
	{
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000029E0 File Offset: 0x00000BE0
	public static int NumberOfCustomRegions()
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
		string[] array = new string[0];
		new List<string>();
		if (File.Exists(string.Concat(new object[]
		{
				Custom.RootFolderDirectory(),
				"World",
				Path.DirectorySeparatorChar,
				"Regions",
				Path.DirectorySeparatorChar,
				"regions.txt"
		})))
		{
			array = File.ReadAllLines(string.Concat(new object[]
			{
					Custom.RootFolderDirectory(),
					"World",
					Path.DirectorySeparatorChar,
					"Regions",
					Path.DirectorySeparatorChar,
					"regions.txt"
			}));
		}
		List<string> list2 = new List<string>();
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < list.Count; j++)
			{
				if (array[i] == list[j])
				{
					flag = false;
				}
			}
			if (flag && num < 9)
			{
				num++;
				list2.Add(array[i]);
				Debug.Log(string.Concat(new object[]
				{
						"New Region: Added ",
						array[i],
						" (",
						num,
						") to the region list."
				}));
			}
		}
		return num;
	}

	// Token: 0x06000017 RID: 23
	public static extern WinState.EndgameTracker orig_CreateAndAddTracker(WinState.EndgameID ID, List<WinState.EndgameTracker> endgameTrackers);

	// Token: 0x06000018 RID: 24 RVA: 0x00002BBC File Offset: 0x00000DBC
	public new static WinState.EndgameTracker CreateAndAddTracker(WinState.EndgameID ID, List<WinState.EndgameTracker> endgameTrackers)
	{
		if (ID == WinState.EndgameID.Traveller)
		{
			WinState.EndgameTracker endgameTracker = new WinState.BoolArrayTracker(ID, 12 + patch_WinState.NumberOfCustomRegions());
			if (endgameTracker != null && endgameTrackers != null)
			{
				endgameTrackers.Add(endgameTracker);
			}
			return endgameTracker;
		}
		return patch_WinState.orig_CreateAndAddTracker(ID, endgameTrackers);
	}
}
