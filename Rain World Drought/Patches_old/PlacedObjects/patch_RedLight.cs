using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using RWCustom;
using UnityEngine;

class patch_RedLight : Redlight
{
    [MonoModIgnore]
    public patch_RedLight(Room placedInRoom, PlacedObject placedObject, PlacedObject.LightFixtureData lightData) : base(placedInRoom, placedObject, lightData)
    {

    }

    public extern void orig_ctor(Room placedInRoom, PlacedObject placedObject, PlacedObject.LightFixtureData lightData);

    [MonoModConstructor]
    public void ctor(Room placedInRoom, PlacedObject placedObject, PlacedObject.LightFixtureData lightData)
    {
        orig_ctor(placedInRoom, placedObject, lightData);
        gravityDependent = this.gravityDependent ? true : placedInRoom.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.GravityPulse) > 0.1f & (lightData.randomSeed > 0f);
    }
}
