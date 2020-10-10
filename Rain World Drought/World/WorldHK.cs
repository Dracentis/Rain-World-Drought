using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class WorldHK
    {
        public static void Patch()
        {
            On.World.ctor += new On.World.hook_ctor(CtorHK);
        }

        private static void CtorHK(On.World.orig_ctor orig, World self,
            RainWorldGame game, Region region, string name, bool singleRoomWorld)
        {
            orig.Invoke(self, game, region, name, singleRoomWorld);
            if (game != null && !singleRoomWorld && game.session is StoryGameSession)
            {
                int oldSeed = Random.seed;
                Random.seed = (game.session as StoryGameSession).saveState.seed + (game.session as StoryGameSession).saveState.cycleNumber;
                self.rainCycle = new RainCycle(self, Mathf.Lerp((float)game.rainWorld.setup.cycleTimeMin, (float)game.rainWorld.setup.cycleTimeMax, Random.value) / 60f);
                Random.seed = oldSeed;
                Debug.Log("Cycle Length: " + self.rainCycle.cycleLength);
            }
        }
    }
}
