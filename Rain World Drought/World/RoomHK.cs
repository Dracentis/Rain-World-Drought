using MonoMod.RuntimeDetour;
using Rain_World_Drought.Effects;
using Rain_World_Drought.Enums;
using Rain_World_Drought.PlacedObjects;
using Rain_World_Drought.Slugcat;
using RWCustom;
using System;
using System.Reflection;
using UnityEngine;

namespace Rain_World_Drought.OverWorld
{
    internal static class RoomHK
    {
        public static void Patch()
        {
            On.Room.ReadyForAI += new On.Room.hook_ReadyForAI(ReadyForAIHK);
            On.Room.Loaded += new On.Room.hook_Loaded(LoadedHK);
            IDetour hkEP = new Hook(typeof(Room).GetProperty("ElectricPower", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(),
                typeof(RoomHK).GetMethod("ElectricPowerHK", BindingFlags.Static | BindingFlags.Public));
            On.Room.Update += new On.Room.hook_Update(UpdateHK);
            On.RoomRealizer.RoomPerformanceEstimation += new On.RoomRealizer.hook_RoomPerformanceEstimation(RoomPerformanceEstimationHK);
        }

        private static void ReadyForAIHK(On.Room.orig_ReadyForAI orig, Room self)
        {
            orig.Invoke(self);
            if (self.game != null && self.abstractRoom.name == "LM_AI")
            {
                Oracle obj = new Oracle(new AbstractPhysicalObject(self.world, AbstractPhysicalObject.AbstractObjectType.Oracle, null, new WorldCoordinate(self.abstractRoom.index, 15, 15, -1), self.game.GetNewID()), self);
                self.AddObject(obj);
                self.waitToEnterAfterFullyLoaded = Math.Max(self.waitToEnterAfterFullyLoaded, 80);
            }
        }

        private static void LoadedHK(On.Room.orig_Loaded orig, Room self)
        {
            if (self.game == null) { return; }
            for (int k = 0; k < self.roomSettings.effects.Count; k++)
            {
                if (!DroughtMod.EnumExt) { break; }
                if (self.roomSettings.effects[k].type == EnumExt_Drought.TankRoomView)
                {
                    self.AddObject(new TankRoomView(self, self.roomSettings.effects[k]));
                    break;
                }
            }

            // Spawn in custom placed objects
            for (int l = 0; l < self.roomSettings.placedObjects.Count; l++)
            {
                if (self.roomSettings.placedObjects[l].active)
                {
                    switch (EnumSwitch.GetPlacedObjectType(self.roomSettings.placedObjects[l].type))
                    {
                        case EnumSwitch.PlacedObjectType.DEFAULT:
                        default:
                            continue;

                        case EnumSwitch.PlacedObjectType.GravityAmplifyer:
                            self.AddObject(new GravityAmplifier(self.roomSettings.placedObjects[l], self)); break;

                        case EnumSwitch.PlacedObjectType.SmallPiston:
                        case EnumSwitch.PlacedObjectType.SmallPistonBotDeathMode:
                        case EnumSwitch.PlacedObjectType.SmallPistonTopDeathMode:
                        case EnumSwitch.PlacedObjectType.SmallPistonDeathMode:
                            if (self.abstractRoom.firstTimeRealized)
                            {
                                self.abstractRoom.entities.Add(new Piston.AbstractPiston(self.world, Piston.PistonType.Small, null, self.GetWorldCoordinate(self.roomSettings.placedObjects[l].pos), self.game.GetNewID(), self.abstractRoom.index, l, false, false));
                            }
                            break;
                        case EnumSwitch.PlacedObjectType.LargePiston:
                        case EnumSwitch.PlacedObjectType.LargePistonBotDeathMode:
                        case EnumSwitch.PlacedObjectType.LargePistonTopDeathMode:
                        case EnumSwitch.PlacedObjectType.LargePistonDeathMode:
                            if (self.abstractRoom.firstTimeRealized)
                            {
                                self.abstractRoom.entities.Add(new Piston.AbstractPiston(self.world, Piston.PistonType.Large, null, self.GetWorldCoordinate(self.roomSettings.placedObjects[l].pos), self.game.GetNewID(), self.abstractRoom.index, l, false, false));
                            }
                            break;
                        case EnumSwitch.PlacedObjectType.GiantPiston:
                        case EnumSwitch.PlacedObjectType.GiantPistonBotDeathMode:
                        case EnumSwitch.PlacedObjectType.GiantPistonTopDeathMode:
                        case EnumSwitch.PlacedObjectType.GiantPistonDeathMode:
                            if (self.abstractRoom.firstTimeRealized)
                            {
                                self.abstractRoom.entities.Add(new Piston.AbstractPiston(self.world, Piston.PistonType.Giant, null, self.GetWorldCoordinate(self.roomSettings.placedObjects[l].pos), self.game.GetNewID(), self.abstractRoom.index, l, false, false));
                            }
                            break;
                    }
                }
            }

            orig.Invoke(self);
            
            // Change some gates to be electric
            if (self.abstractRoom.gate)
            {
                if (self.abstractRoom.name == "GATE_SL_MW" || self.abstractRoom.name == "GATE_MW_LM" || self.abstractRoom.name == "GATE_LM_MW")
                {
                    self.regionGate.RemoveFromRoom();
                    self.regionGate.Destroy();
                    self.regionGate = new ElectricGate(self);
                    self.AddObject(self.regionGate);
                }
            }
            
            // Initialize custom room effects
            for (int k = 0; k < self.roomSettings.effects.Count; k++)
            {
                switch (EnumSwitch.GetRoomEffectType(self.roomSettings.effects[k].type))
                {
                    case EnumSwitch.RoomEffectType.DEFAULT:
                    default:
                        continue;

                    case EnumSwitch.RoomEffectType.ElectricStorm:
                        self.AddObject(new ElectricStorm(self.roomSettings.effects[k], self)); break;
                    case EnumSwitch.RoomEffectType.GravityPulse:
                        self.AddObject(new GravityPulse(self)); break;
                    case EnumSwitch.RoomEffectType.Drain:
                        self.AddObject(new DrainEffect(self)); break;
                    case EnumSwitch.RoomEffectType.Pulse:
                        self.AddObject(new PulseEffect(self)); break;
                }
            }
        }

        public delegate float ElectricPower(Room self);

        public static float ElectricPowerHK(ElectricPower orig, Room self)
        {
            if (self.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) > 0f)
            {
                return orig.Invoke(self);
            }
            else if (DroughtMod.EnumExt && self.roomSettings.GetEffectAmount(EnumExt_Drought.GravityPulse) > 0.1f)
            {
                float state = (float)Math.Sin((self.world.rainCycle.timer + 1875f) % 2500f / 397.88735f) * 3f;
                state = Mathf.Clamp(state, 0f, 1f);
                state = 1f - state;
                return state;
            }
            return 1f;
        }

        private static void UpdateHK(On.Room.orig_Update orig, Room self)
        {
            orig.Invoke(self);
            if (!DroughtMod.EnumExt) { return; }
            for (int i = 0; i < self.roomSettings.placedObjects.Count; i++)
            {
                if (self.roomSettings.placedObjects[i].type == EnumExt_DroughtPlaced.Radio)
                {
                    for (int j = 0; j < self.physicalObjects.Length; j++)
                    {
                        for (int k = 0; k < self.physicalObjects[j].Count; k++)
                        {
                            if (self.physicalObjects[j][k] is Player p && Custom.DistLess(p.mainBodyChunk.pos, self.roomSettings.placedObjects[i].pos, 70f))
                            {
                                WandererSupplement sub = WandererSupplement.GetSub(p);
                                sub.Rad();
                                if (sub.rad > 525 && !p.dead)
                                {
                                    self.PlaySound(SoundID.Death_Lightning_Spark_Object, p.mainBodyChunk.pos, 1f, 1f);
                                    p.Violence(null, null, p.mainBodyChunk, null, Creature.DamageType.Electric, 1f, 1f);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static float RoomPerformanceEstimationHK(On.RoomRealizer.orig_RoomPerformanceEstimation orig, RoomRealizer self, AbstractRoom testRoom)
        {
            float res = orig.Invoke(self, testRoom);
            for (int j = 0; j < testRoom.creatures.Count; j++)
            {
                if (testRoom.creatures[j].state.alive)
                {
                    // -10f, because original code adds 10f for default
                    switch (EnumSwitch.GetCreatureTemplateType(testRoom.creatures[j].creatureTemplate.type))
                    {
                        default:
                        case EnumSwitch.CreatureTemplateType.DEFAULT:
                            continue;
                        case EnumSwitch.CreatureTemplateType.SeaDrake:
                            res += 15f; continue; // 25f
                        case EnumSwitch.CreatureTemplateType.WalkerBeast:
                            res += 190f; continue; // 200f
                    }
                }
            }
            return res;
        }
    }
}

/*
Plan
Create
Modifiy
Render
Settings
Done

Spire Rooms:
SP03 - Plan
SP04 - Plan
Spire6 - Settings
SP01 - Settings
SP02 - Settings

Intake System Rooms:
GATE_UW_IS - Settings
V01 - Settings
V04 - Render
V05 - Settings
V06 - Settings
V07 - Create
V08 - Settings
G04 - Settings
S02 - Settings
D02 - Settings
L02 - Settings
S01 - Done
V02 - Done
V03 - Done
F01 - Done
F03 - Done
F04 - Done
G02 - Done
G03 - Done
S05 - Done
Drain - Done
F01a - Done
F02 - Done
GATE_SL_IS - Done
B05 - Done
B04 - Done
B03 - Done
B02 - Done
B01 - Done
G01 - Done
CORE - Done
S06 - Done

*Rooms that need sounds*
SI_SP01*
SI_SP02*
*/
