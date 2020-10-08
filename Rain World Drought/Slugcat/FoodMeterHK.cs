using HUD;

namespace Rain_World_Drought.Slugcat
{
    internal static class FoodMeterHK
    {
        public static void Patch()
        {
            On.HUD.FoodMeter.ctor += new On.HUD.FoodMeter.hook_ctor(CtorHK);
            On.HUD.FoodMeter.UpdateShowCount += new On.HUD.FoodMeter.hook_UpdateShowCount(UpdateShowCountHK);
        }

        public static FoodMeter mainMeter;

        public static void AddHibernateCost()
        {
            mainMeter.survivalLimit++;
            mainMeter.RefuseFood();
        }

        private static void CtorHK(On.HUD.FoodMeter.orig_ctor orig, FoodMeter self, HUD.HUD hud, int maxFood, int survivalLimit)
        {
            orig.Invoke(self, hud, maxFood, survivalLimit);
            mainMeter = self;
        }

        private static void UpdateShowCountHK(On.HUD.FoodMeter.orig_UpdateShowCount orig, FoodMeter self)
        {
            if (self.showCount > self.hud.owner.CurrentFood)
            {
                self.circles[self.showCount - 1].EatFade();
                self.showCount--;
            }
            else { orig.Invoke(self); }
        }
    }
}
