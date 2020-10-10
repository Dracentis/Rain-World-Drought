using Rain_World_Drought.Enums;
using Rain_World_Drought.OverWorld;

namespace Rain_World_Drought.PlacedObjects
{
    internal static class AbstractPhysicalObjectHK
    {
        public static void Patch()
        {
            On.AbstractPhysicalObject.Realize += new On.AbstractPhysicalObject.hook_Realize(RealizeHK);
            On.Redlight.ctor += new On.Redlight.hook_ctor(RedLightCtorHK);
        }

        private static void RealizeHK(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
        {
            if (self.realizedObject != null) { return; }
            switch (EnumSwitch.GetAbstractPhysicalObjectType(self.type))
            {
                default:
                case EnumSwitch.AbstractPhysicalObjectType.DEFAULT:
                    orig.Invoke(self); return;
                case EnumSwitch.AbstractPhysicalObjectType.LMOracleSwarmer:
                    self.realizedObject = new LMOracleSwarmer(self, self.world); break;
                case EnumSwitch.AbstractPhysicalObjectType.SmallPiston:
                    self.realizedObject = new SmallPiston(self); break;
                case EnumSwitch.AbstractPhysicalObjectType.LargePiston:
                    self.realizedObject = new LargePiston(self); break;
                case EnumSwitch.AbstractPhysicalObjectType.GiantPiston:
                    self.realizedObject = new GiantPiston(self); break;
            }
            for (int i = 0; i < self.stuckObjects.Count; i++)
            {
                if (self.stuckObjects[i].A.realizedObject == null && self.stuckObjects[i].A != self)
                { self.stuckObjects[i].A.Realize(); }
                if (self.stuckObjects[i].B.realizedObject == null && self.stuckObjects[i].B != self)
                { self.stuckObjects[i].B.Realize(); }
            }
        }

        private static void RedLightCtorHK(On.Redlight.orig_ctor orig, Redlight self,
            Room placedInRoom, PlacedObject placedObject, PlacedObject.LightFixtureData lightData)
        {
            orig.Invoke(self, placedInRoom, placedObject, lightData);
            self.gravityDependent = self.gravityDependent ? true :
                DroughtMod.EnumExt && placedInRoom.roomSettings.GetEffectAmount(EnumExt_Drought.GravityPulse) > 0.1f && (lightData.randomSeed > 0f);
        }
    }
}
