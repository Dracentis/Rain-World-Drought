using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using UnityEngine;
using RWCustom;

class patch_World : World
{
    [MonoModIgnore]
    public patch_World(RainWorldGame game, Region region, string name, bool singleRoomWorld) : base(game, region, name, singleRoomWorld)
    {
    }


    public extern void orig_ctor(RainWorldGame game, Region region, string name, bool singleRoomWorld);

    [MonoModConstructor]
    public void ctor(RainWorldGame game, Region region, string name, bool singleRoomWorld)
    {
        orig_ctor(game, region, name, singleRoomWorld);
        if (game != null && !singleRoomWorld && game.session is StoryGameSession)
        {
            int oldSeed = UnityEngine.Random.seed;
            UnityEngine.Random.seed = (game.session as StoryGameSession).saveState.seed + (game.session as StoryGameSession).saveState.cycleNumber;
            this.rainCycle = new RainCycle(this, Mathf.Lerp((float)game.rainWorld.setup.cycleTimeMin, (float)game.rainWorld.setup.cycleTimeMax, UnityEngine.Random.value) / 60f);
            UnityEngine.Random.seed = oldSeed;
            Debug.Log("Cycle Length: " + this.rainCycle.cycleLength);
        }
    }

}
