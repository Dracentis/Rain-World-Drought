using Menu;
using RWCustom;
using MonoMod;
using HUD;
using UnityEngine;

[MonoModPatch("global::Menu.SleepAndDeathScreen")]
class patch_SleepAndDeathScreen : SleepAndDeathScreen
{
    [MonoModIgnore]
    public patch_SleepAndDeathScreen(ProcessManager manager, ProcessManager.ProcessID ID) : base(manager, ID)
    {
    }

    public int nextcycleLength = 24000;
    public int nextcycleLength2 = 24000;
    public int nextcycleLength3 = 24000;
    public int burstNum = 0;
    public int burstNum2 = 0;
    public int burstNum3 = 0;

    public int getBurst(int index) { 
            if (burstNum <= index)
            {
                return 50;
            }
            return (int)(((float)nextcycleLength)/((float)burstNum+1f)*(burstNum-index))/1200;
    }

    public int getBurst2(int index)
    {
        if (burstNum2 <= index)
        {
            return 50;
        }
        return (int)(((float)nextcycleLength2) / ((float)burstNum2 + 1f) * (burstNum2 - index)) / 1200;
    }

    public int getBurst3(int index)
    {
        if (burstNum3 <= index)
        {
            return 50;
        }
        return (int)(((float)nextcycleLength3) / ((float)burstNum3 + 1f) * (burstNum3 - index)) / 1200;
    }

    public extern void orig_GetDataFromGame(KarmaLadderScreen.SleepDeathScreenDataPackage package);

    public override void GetDataFromGame(KarmaLadderScreen.SleepDeathScreenDataPackage package)
    {
        int oldSeed = UnityEngine.Random.seed;
        UnityEngine.Random.seed = package.saveState.seed + package.saveState.cycleNumber;
        nextcycleLength = (int)(Mathf.Lerp(300f, 1000f, UnityEngine.Random.value) / 60f * 40f * 60f);
        if (nextcycleLength > 36000)
        {
            burstNum = 3;
        }
        else if (nextcycleLength > 32000)
        {
            burstNum = 2;
        }
        else if (nextcycleLength > 28000)
        {
            burstNum = 1;
        }
        UnityEngine.Random.seed = package.saveState.seed + package.saveState.cycleNumber+1;
        nextcycleLength2 = (int)(Mathf.Lerp(300f, 1000f, UnityEngine.Random.value) / 60f * 40f * 60f);
        if (nextcycleLength2 > 36000)
        {
            burstNum2 = 3;
        }
        else if (nextcycleLength2 > 32000)
        {
            burstNum2 = 2;
        }
        else if (nextcycleLength2 > 28000)
        {
            burstNum2 = 1;
        }
        UnityEngine.Random.seed = package.saveState.seed + package.saveState.cycleNumber+2;
        nextcycleLength3 = (int)(Mathf.Lerp(300f, 1000f, UnityEngine.Random.value) / 60f * 40f * 60f);
        if (nextcycleLength3 > 36000)
        {
            burstNum3 = 3;
        }
        else if (nextcycleLength3 > 32000)
        {
            burstNum3 = 2;
        }
        else if (nextcycleLength3 > 28000)
        {
            burstNum3 = 1;
        }
        UnityEngine.Random.seed = oldSeed;
        orig_GetDataFromGame(package);
    }
}
