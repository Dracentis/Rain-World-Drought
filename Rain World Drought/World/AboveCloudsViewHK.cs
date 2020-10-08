namespace Rain_World_Drought.OverWorld
{
    internal static class AboveCloudsViewHK
    {
        public static void Patch()
        {
            On.AboveCloudsView.ctor += new On.AboveCloudsView.hook_ctor(CtorHK);
        }

        private static void CtorHK(On.AboveCloudsView.orig_ctor orig, AboveCloudsView self,
            Room room, RoomSettings.RoomEffect effect)
        {
            bool si = room.world.region != null && room.world.region.name == "SI";
            if (si) { room.world.region.name = "XX"; } // call non-SIClouds ctor for SI
            orig.Invoke(self, room, effect);
            if (si)
            {
                room.world.region.name = "SI";
                self.SIClouds = true;
                self.startAltitude = 9000f;
                self.endAltitude = 26400f;
            }
        }
    }
}
