using Rain_World_Drought.Creatures;
using System.Collections.Generic;

namespace Rain_World_Drought.Slugcat
{
    public class WandererSupplement
    {
        public WandererSupplement(Player self)
        {
            this.self = self;
            this.bashing = false;
            this.jmpDwn = false;
            this.parry = 0;
            // this.lastAnimation = Player.AnimationIndex.None;
            this.energy = 1f;
            this.maxEnergy = 1f;
            this.uses = MAX_USES;
            this.hibernationPenalty = 2;
            // this.hibernation1 = false;
            // this.hibernation2 = false;
            this.pearlConversation = new PearlConversation(self);
            this.voidEnergy = false;

            this.rad = 0;
        }

        public readonly Player self;

        private static WandererSupplement[] fields = new WandererSupplement[4];
        private static Dictionary<Player, WandererSupplement> ghostFields = new Dictionary<Player, WandererSupplement>();

        public static bool IsWanderer(Player self)
        {
            return DroughtMod.EnumExt && self.playerState.slugcatCharacter == SlugcatCharacter;
        }

        public static WandererSupplement GetSub(Player self)
        {
            if (self.playerState.isGhost)
            { // for ending
                if (ghostFields.TryGetValue(self, out WandererSupplement sub)) { return sub; }
                WandererSupplement newSub = new WandererSupplement(self);
                ghostFields.Add(self, newSub);
                return newSub;
            }
            else
            {
                if (fields[self.playerState.playerNumber] == null) { CreateSub(self); }
                return fields[self.playerState.playerNumber];
            }
        }

        private static void CreateSub(Player self)
        {
            if (fields[self.playerState.playerNumber] != null && fields[self.playerState.playerNumber].self.abstractCreature == self.abstractCreature)
            { return; }
            fields[self.playerState.playerNumber] = new WandererSupplement(self);
        }

        // Wanderer replaces orig slugcat: this can be changed with enumext but
        public const int SlugcatCharacter = 0;
        public const int StoryCharacter = 0;

        public bool bashing;
        public bool jmpDwn;
        public float energy; //varies from 0f to 1f
        public float maxEnergy = 1f;
        public const float BASH_COST = 0.4f;
        public const float RECHARGE_RATE = 0.002f;
        public const float PARRY_COST = 0.5f;
        public const int MAX_USES = 30;
        public const int ENERGY_PER_HUNGER = 10;

        #region Ending Supplement
        public bool voidEnergy = false; //true if the void effects are controlling the maxEnergy
        public bool past22000 = false; //true if the player is in the void past -22000 y
        public bool past25000 = false; //true if the player is in the void past -25000 y
        #endregion Ending Supplement

        public int hibernationPenalty;
        // public bool hibernation1;
        // public bool hibernation2;
        public int uses;
        public float parry; //varies from 45f to 0f
        public WalkerBeast.PlayerInAntlers playerInAnt;
        // public Player.AnimationIndex lastAnimation; // not needed/

        public PearlConversation pearlConversation;
        public int dropCounter = 0;

        public int rad; //integer to measure amount of time spent near a radio

        public void Rad()
        {
            if (rad < 549) { rad += 2; }
        }

        #region Graphics
        public float tailLength;
        public List<PlayerCosmetics> cosmetics;
        public int origSprites = 12;
        public int extraSprites;
        #endregion Graphics
    }
}
