using VoidSea;
using MonoMod;
using RWCustom;
using UnityEngine;

[MonoModPatch("global::VoidSea.VoidSeaScene")]
class patch_VoidSeaScene : VoidSeaScene
{
    [MonoModIgnore]
    public patch_VoidSeaScene(Room room) : base(room)
    {
    }


    public void VoidSeaTreatment(Player player, float swimSpeed)
    {
        if (player.room != this.room)
        {
            return;
        }
        (player as patch_Player).voidEnergy = true;
        (player as patch_Player).maxEnergy = Custom.LerpMap(player.mainBodyChunk.pos.y, -1000f, -5000f, 1f, 0f);
        if (!(player as patch_Player).past22000 && player.mainBodyChunk.pos.y < -22000)
        {
            (player as patch_Player).past22000 = true;
        }
        else if (!(player as patch_Player).past25000 && player.mainBodyChunk.pos.y < -25000)
        {
            (player as patch_Player).past25000 = true;
        }
        if (this.deepDivePhase == VoidSeaScene.DeepDivePhase.EggScenario)
        {
            (player as patch_Player).past25000 = true;
        }
            for (int i = 0; i < player.bodyChunks.Length; i++)
        {
            player.bodyChunks[i].restrictInRoomRange = float.MaxValue;
            player.bodyChunks[i].vel *= Mathf.Lerp(swimSpeed, 1f, this.room.game.cameras[0].voidSeaGoldFilter);
            BodyChunk bodyChunk = player.bodyChunks[i];
            bodyChunk.vel.y = bodyChunk.vel.y - player.buoyancy;
            BodyChunk bodyChunk2 = player.bodyChunks[i];
            bodyChunk2.vel.y = bodyChunk2.vel.y + player.gravity;
        }
        player.airInLungs = 1f;
        player.lungsExhausted = false;
        if (player.graphicsModule != null && (player.graphicsModule as PlayerGraphics).lightSource != null)
        {
            (player.graphicsModule as PlayerGraphics).lightSource.setAlpha = new float?(Custom.LerpMap(player.mainBodyChunk.pos.y, -2000f, -8000f, 1f, 0.2f) * (1f - this.eggProximity));
            (player.graphicsModule as PlayerGraphics).lightSource.setRad = new float?(Custom.LerpMap(player.mainBodyChunk.pos.y, -2000f, -8000f, 300f, 200f) * (0.5f + 0.5f * (1f - this.eggProximity)));
        }
        if (this.deepDivePhase == VoidSeaScene.DeepDivePhase.EggScenario && UnityEngine.Random.value < 0.1f)
        {
            player.mainBodyChunk.vel += Custom.DirVec(player.mainBodyChunk.pos, this.theEgg.pos) * 0.02f * UnityEngine.Random.value;
        }
    }
}

