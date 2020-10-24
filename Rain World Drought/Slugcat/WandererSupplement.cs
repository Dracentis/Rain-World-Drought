using Rain_World_Drought.Creatures;
using System.Collections.Generic;
using RWCustom;

namespace Rain_World_Drought.Slugcat
{
    public class WandererSupplement
    {
        public WandererSupplement(Player self)
        {
            this.self = self;
            this.energy = maxEnergy;
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
        
        public static WandererSupplement GetSub(Player self, bool makeNewSub = false)
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
                if (fields[self.playerState.playerNumber] == null || makeNewSub) { CreateSub(self); }
                return fields[self.playerState.playerNumber];
            }
        }

        private static void CreateSub(Player self)
        {
            //if (fields[self.playerState.playerNumber] != null && fields[self.playerState.playerNumber].self.abstractCreature == self.abstractCreature)
            //{ return; }
            fields[self.playerState.playerNumber] = new WandererSupplement(self);
        }

        // Wanderer replaces orig slugcat: this can be changed with enumext but
        public const int SlugcatCharacter = 0;
        public const int StoryCharacter = 0;

        #region Ability Suppliments
        // Number of ability uses per food pip
        public const int maxEnergy = 9;
        // Duration of initial focus, in updates
        internal const int focusDuration = 60;
        // Duration of slowdown before a double-jump, in updates
        internal const int slowdownDuration = 40;
        // Maximum distance of weapons to parry, in pixels
        internal const float parryRadius = 120f;
        // Number of jumps allowed after the first double-jump
        internal const int maxExtraJumps = 2;
        // Change in velocity that a maximum force double-jump apples, in pixels per update
        internal const float jumpForce = 15f;
        // Number of updates after a double-jump that a parry will still apply
        internal const int parryLength = 3;

        internal int jumpsSinceGrounded = 0;
        public int energy;
        public bool hasHalfEnergyPip;
        internal int focusLeft;
        internal int slowdownLeft;
        internal bool panicSlowdown;
        internal float ticksUntilPanicHit;
        internal bool canTripleJump;
        internal int noFocusJumpCounter;
        internal int wantToParry;
        public bool jumpQueued;

        internal int jumpForbidden;
        internal int mapHeld;
        #endregion Ability Suppliments

        public float Focus => focusLeft / (float)focusDuration;
        public float Slowdown => slowdownLeft / (float)slowdownDuration;
        public float PanicSlowdown => (slowdownLeft == 0 || !panicSlowdown) ? 0f : Custom.LerpMap(ticksUntilPanicHit, 6f, 1f, 0f, 1f);
        public float Energy => (energy + (hasHalfEnergyPip ? 0f : 0.5f)) / maxEnergy;
        public int AirJumpsLeft => maxExtraJumps + 1 - jumpsSinceGrounded;

        #region Ending Supplement
        public bool voidEnergy = false; //true if the void effects are controlling the maxEnergy
        public float voidEnergyAmount = 0f;
        public bool past22000 = false; //true if the player is in the void past -22000 y
        public bool past25000 = false; //true if the player is in the void past -25000 y
        #endregion Ending Supplement
        
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
