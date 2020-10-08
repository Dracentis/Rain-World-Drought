namespace Rain_World_Drought.Creatures
{
    internal static class DeerHK
    {
        public static void Patch()
        {
            On.Deer.Update += new On.Deer.hook_Update(UpdateHK);
            On.DeerAI.WantToStayInDenUntilEndOfCycle += new On.DeerAI.hook_WantToStayInDenUntilEndOfCycle(WantToStayInDenUntilEndOfCycleHK);
        }

        /// <summary>
        /// Prevents dying from burst rain
        /// </summary>
        private static void UpdateHK(On.Deer.orig_Update orig, Deer self, bool eu)
        {
            orig.Invoke(self, eu);
            self.rainDeath = 0f;
        }

        private static bool WantToStayInDenUntilEndOfCycleHK(On.DeerAI.orig_WantToStayInDenUntilEndOfCycle orig, DeerAI self)
        {
            return self.creature.world.rainCycle.TimeUntilRain < (self.creature.world.game.IsStorySession ? 60 : 15) * 40;
        }
    }
}
