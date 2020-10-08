using Rain_World_Drought.Enums;

namespace Rain_World_Drought.Creatures
{
    internal static class AbstractCreatureHK
    {
        public static void Patch()
        {
            On.AbstractCreature.ctor += new On.AbstractCreature.hook_ctor(CtorHK);
            On.AbstractCreature.InDenUpdate += new On.AbstractCreature.hook_InDenUpdate(InDenUpdateHK);
            On.AbstractCreature.Realize += new On.AbstractCreature.hook_Realize(RealizeHK);
            On.AbstractCreature.InitiateAI += new On.AbstractCreature.hook_InitiateAI(InitiateAIHK);
        }

        private static void CtorHK(On.AbstractCreature.orig_ctor orig, AbstractCreature self,
            World world, CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID ID)
        {
            orig.Invoke(self, world, creatureTemplate, realizedCreature, pos, ID);

            switch (EnumSwitch.GetCreatureTemplateType(creatureTemplate.TopAncestor().type))
            {
                default:
                case EnumSwitch.CreatureTemplateType.DEFAULT: // Vanilla / other mods
                    return;

                case EnumSwitch.CreatureTemplateType.WalkerBeast:
                    self.abstractAI = new WalkerBeastAbstractAI(world, self);
                    break;
            }
        }

        /// <summary>
        /// Allow creatures to abort staying in den for the rest of the cycle
        /// </summary>
        private static void InDenUpdateHK(On.AbstractCreature.orig_InDenUpdate orig, AbstractCreature self, int time)
        {
            if (self.remainInDenCounter == -1)
            {
                if (!self.WantToStayInDenUntilEndOfCycle())
                { self.remainInDenCounter = 500; }
            }
            else
            { orig.Invoke(self, time); }
        }

        private static void RealizeHK(On.AbstractCreature.orig_Realize orig, AbstractCreature self)
        {
            if (self.realizedCreature != null) { return; }
            switch (EnumSwitch.GetCreatureTemplateType(self.creatureTemplate.TopAncestor().type))
            {
                default:
                case EnumSwitch.CreatureTemplateType.DEFAULT:
                    orig.Invoke(self); return;

                case EnumSwitch.CreatureTemplateType.SeaDrake:
                    self.realizedCreature = new SeaDrake(self, self.world); break;
                case EnumSwitch.CreatureTemplateType.WalkerBeast:
                    self.realizedCreature = new WalkerBeast(self, self.world); break;
            }
            self.InitiateAI();
            for (int i = 0; i < self.stuckObjects.Count; i++)
            {
                if (self.stuckObjects[i].A.realizedObject == null) { self.stuckObjects[i].A.Realize(); }
                if (self.stuckObjects[i].B.realizedObject == null) { self.stuckObjects[i].B.Realize(); }
            }
        }

        private static void InitiateAIHK(On.AbstractCreature.orig_InitiateAI orig, AbstractCreature self)
        {
            switch (EnumSwitch.GetCreatureTemplateType(self.creatureTemplate.TopAncestor().type))
            {
                default:
                case EnumSwitch.CreatureTemplateType.DEFAULT:
                    orig.Invoke(self); return;

                case EnumSwitch.CreatureTemplateType.SeaDrake:
                    self.abstractAI.RealAI = new SeaDrakeAI(self, self.world); break;
                case EnumSwitch.CreatureTemplateType.WalkerBeast:
                    self.abstractAI.RealAI = new WalkerBeastAI(self, self.world); break;
            }
        }
    }
}