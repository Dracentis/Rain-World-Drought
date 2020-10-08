using UnityEngine;
using MonoMod;


class patch_RoomRealizer : RoomRealizer
{
    [MonoModIgnore]
    public patch_RoomRealizer(AbstractCreature followCreature, World world) : base(followCreature, world)
    {
    }

    public float RoomPerformanceEstimation(AbstractRoom testRoom)
    {
        float num = Mathf.Lerp(2080f, (float)(testRoom.size.x * testRoom.size.y), 0.25f) / 40f;
        for (int i = 0; i < testRoom.nodes.Length; i++)
        {
            if (testRoom.nodes[i].submerged)
            {
                num += Mathf.Lerp(50f, (float)testRoom.size.x, 0.5f) * 0.2f;
                break;
            }
        }
        if (testRoom.singleRealizedRoom)
        {
            num += performanceBudget * 0.55f;
        }
        for (int j = 0; j < testRoom.creatures.Count; j++)
        {
            if (testRoom.creatures[j].state.alive)
            {
                switch (testRoom.creatures[j].creatureTemplate.type)
                {
                    case CreatureTemplate.Type.Fly:
                        num += 10f;
                        goto IL_253;
                    case CreatureTemplate.Type.Leech:
                        num += 10f;
                        goto IL_253;
                    case CreatureTemplate.Type.Snail:
                        num += 20f;
                        goto IL_253;
                    case CreatureTemplate.Type.Vulture:
                        num += 100f;
                        goto IL_253;
                    case CreatureTemplate.Type.GarbageWorm:
                        num += 30f;
                        goto IL_253;
                    case CreatureTemplate.Type.CicadaA:
                    case CreatureTemplate.Type.CicadaB:
                    case (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake:
                        num += 25f;
                        goto IL_253;
                    case CreatureTemplate.Type.JetFish:
                        num += 25f;
                        goto IL_253;
                    case CreatureTemplate.Type.BigEel:
                        num += 300f;
                        goto IL_253;
                    case CreatureTemplate.Type.Deer:
                    case (CreatureTemplate.Type) patch_CreatureTemplate.Type.WalkerBeast:
                    case CreatureTemplate.Type.DaddyLongLegs:
                    case CreatureTemplate.Type.BrotherLongLegs:
                        num += 200f;
                        goto IL_253;
                    case CreatureTemplate.Type.Scavenger:
                        num += 300f;
                        goto IL_253;
                    case CreatureTemplate.Type.BigSpider:
                        num += 30f;
                        goto IL_253;
                    case CreatureTemplate.Type.SpitterSpider:
                        num += 50f;
                        goto IL_253;
                    case CreatureTemplate.Type.DropBug:
                        num += 20f;
                        goto IL_253;
                    case CreatureTemplate.Type.KingVulture:
                        num += 140f;
                        goto IL_253;
                }
                if (testRoom.creatures[j].creatureTemplate.TopAncestor().type == CreatureTemplate.Type.LizardTemplate)
                {
                    num += 50f;
                }
                else
                {
                    num += 10f;
                }
            }
            IL_253:;
        }
        return num;
    }

    [MonoModIgnore]
    private float performanceBudget = 1500f;

}
