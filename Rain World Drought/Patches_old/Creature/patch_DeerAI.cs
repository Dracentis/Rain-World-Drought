using MonoMod;

namespace Rain_World_Drought.Creatures
{
    internal class patch_DeerAI : DeerAI
    {
        [MonoModIgnore]
        public patch_DeerAI(AbstractCreature creature, World world) : base(creature, world)
        {
        }

        public bool WantToStayInDenUntilEndOfCycle()
        {
            return creature.world.rainCycle.TimeUntilRain < (creature.world.game.IsStorySession ? 60 : 15) * 40;
        }
    }
}