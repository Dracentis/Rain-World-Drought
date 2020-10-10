using MonoMod.RuntimeDetour;
using OverseerHolograms;
using System.Reflection;
using UnityEngine;

namespace Rain_World_Drought.Creatures
{
    internal static class OverseerHK
    {
        public static void Patch()
        {
            droughtTutorialRooms = new string[] { "FS_A01" };

            On.Overseer.ctor += new On.Overseer.hook_ctor(CtorHK);
            On.Overseer.TryAddHologram += new On.Overseer.hook_TryAddHologram(TryAddHologramHK);
            On.OverseerAI.Update += new On.OverseerAI.hook_Update(AIUpdateHK);
            On.OverseerAbstractAI.ctor += new On.OverseerAbstractAI.hook_ctor(AbsAICtorHK);
            IDetour hkMC = new Hook(typeof(OverseerGraphics).GetProperty("MainColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(),
                typeof(OverseerHK).GetMethod("MainColorHK", BindingFlags.Static | BindingFlags.Public));
        }

        private static void CtorHK(On.Overseer.orig_ctor orig, Overseer self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            if (self.PlayerGuide) { self.size = 1.0f; droughtTutorialBehavior = null; } //SRS
        }

        private static void TryAddHologramHK(On.Overseer.orig_TryAddHologram orig, Overseer self,
            OverseerHologram.Message message, Creature communicateWith, float importance)
        {
            if (self.dead) { return; }
            if (message == OverseerHologram.Message.SuperJump)
            {
                if (self.hologram != null)
                {
                    if (self.hologram.message == message) { return; }
                    if (self.hologram.importance >= importance && importance != 3.40282347E+38f) { return; }
                    self.hologram.stillRelevant = false;
                    self.hologram = null;
                }
                if (self.room.abstractRoom.name.Equals("FS_A01"))
                {
                    Debug.Log("Trying to add tutorial hologram!");
                    if (self.room.game.rainWorld.options.controls[0].gamePad)
                    { self.hologram = new OverseerDroughtTutorialBehavior.GamePadInstruction(self, message, communicateWith, importance); }
                    else
                    { self.hologram = new OverseerDroughtTutorialBehavior.KeyBoardInstruction(self, message, communicateWith, importance); }
                    self.room.AddObject(self.hologram);
                    return;
                }
            }
            orig.Invoke(self, message, communicateWith, importance);
        }

        public static string[] droughtTutorialRooms;
        public static OverseerDroughtTutorialBehavior droughtTutorialBehavior;

        private static void AIUpdateHK(On.OverseerAI.orig_Update orig, OverseerAI self)
        {
            orig.Invoke(self);
            if (self.overseer.PlayerGuide && droughtTutorialBehavior == null && self.creature.world.game.session is StoryGameSession && (self.creature.world.game.session as StoryGameSession).saveState.cycleNumber == 0 && self.tutorialBehavior == null && self.overseer.room.game.Players.Count > 0 && self.overseer.room.abstractRoom == self.overseer.room.game.Players[0].Room && self.overseer.room.world.region.name == "FS")
            {
                for (int i = 0; i < droughtTutorialRooms.Length; i++)
                {
                    if (self.overseer.room.game.Players[0].Room.name == droughtTutorialRooms[i])
                    {
                        Debug.Log("Tutorial Behavior Added.");
                        droughtTutorialBehavior = new OverseerDroughtTutorialBehavior(self);
                        self.AddModule(droughtTutorialBehavior);
                        break;
                    }
                }
            }
        }

        public enum OwnerIterator : int
        {
            FP = 0,
            NSH = 2,
            LTTM = 3
        }

        private static void AbsAICtorHK(On.OverseerAbstractAI.orig_ctor orig, OverseerAbstractAI self, World world, AbstractCreature parent)
        {
            orig.Invoke(self, world, parent);
            // self.ownerIterator = 0;

            if (!world.singleRoomWorld && !self.playerGuide)
            {
                if (world.region.name == "SB")
                {
                    self.ownerIterator = (int)OwnerIterator.NSH;
                }
                else if (world.region.name == "UW")
                {
                    self.ownerIterator = (UnityEngine.Random.value > 0.1f) ? (int)OwnerIterator.FP : (int)OwnerIterator.LTTM; //Mostly FP
                }
                else if (world.region.name == "CC" || world.region.name == "SH")
                {
                    self.ownerIterator = (UnityEngine.Random.value > 0.3f) ? (int)OwnerIterator.FP : (int)OwnerIterator.LTTM; //Mostly FP
                }
                else if (world.region.name == "MW")
                {
                    self.ownerIterator = (UnityEngine.Random.value > 0.9f) ? (int)OwnerIterator.FP : (int)OwnerIterator.LTTM; //Mostly LTTM
                }
                else if (world.region.name == "SL" || world.region.name == "IS")
                {
                    self.ownerIterator = (UnityEngine.Random.value > 0.7f) ? (int)OwnerIterator.FP : (int)OwnerIterator.LTTM; //Mostly LTTM
                }
                else
                {
                    self.ownerIterator = (UnityEngine.Random.value > 0.5f) ? (int)OwnerIterator.FP : (int)OwnerIterator.LTTM; //RANDOM
                }
            }
        }

        public delegate Color MainColor(OverseerGraphics self);

        public static Color MainColorHK(MainColor orig, OverseerGraphics self)
        {
            if (self.overseer.PlayerGuide) { return new Color(1f, 0.2f, 0.1f); } //SRS
            return orig.Invoke(self);
        }
    }
}
