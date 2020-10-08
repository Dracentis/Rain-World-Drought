using UnityEngine;
using RWCustom;
using MonoMod;

class patch_AImap : AImap
{
    [MonoModIgnore]
    public patch_AImap(Room rm, int w, int h) : base(rm, w, h)
    {
    }


    public bool TileAccessibleToCreature(IntVector2 pos, CreatureTemplate crit)
    {
        if (!crit.MovementLegalInRelationToWater(getAItile(pos).DeepWater, getAItile(pos).WaterSurface))
        {
            return false;
        }
        if (crit.PreBakedPathingIndex == -1)
        {
            return false;
        }
        for (int i = 0; i < room.accessModifiers.Count; i++)
        {
            if (!room.accessModifiers[i].IsTileAccessible(pos, crit))
            {
                return false;
            }
        }
        CreatureTemplate.Type type = crit.type;
        switch (type)
        {
            case CreatureTemplate.Type.BigEel:
                if (getAItile(pos).terrainProximity < 4)
                {
                    return false;
                }
                break;
            case CreatureTemplate.Type.Deer:
                if (getAItile(pos).terrainProximity < 3)
                {
                    return false;
                }
                if (getAItile(pos).smoothedFloorAltitude > 17)
                {
                    return false;
                }
                break;
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast:
                if (getAItile(pos).terrainProximity < 3)
                {
                    return false;
                }
                if (getAItile(pos).smoothedFloorAltitude > 17)
                {
                    return false;
                }
                break;
            default:
                if (type == CreatureTemplate.Type.Vulture || type == CreatureTemplate.Type.KingVulture)
                {
                    if (getAItile(pos).terrainProximity < 2)
                    {
                        return false;
                    }
                }
                break;
            case CreatureTemplate.Type.DaddyLongLegs:
            case CreatureTemplate.Type.BrotherLongLegs:
                if (room.GetTile(pos).Terrain == Room.Tile.TerrainType.ShortcutEntrance)
                {
                    return true;
                }
                if (getAItile(pos).terrainProximity < 2 || getAItile(pos).terrainProximity > 11)
                {
                    return false;
                }
                break;
            case CreatureTemplate.Type.MirosBird:
                if (getAItile(pos).terrainProximity < 2)
                {
                    return false;
                }
                if (getAItile(pos).smoothedFloorAltitude > 2 && (float)(getAItile(pos).smoothedFloorAltitude + getAItile(pos).floorAltitude) > Custom.LerpMap((float)getAItile(pos).terrainProximity, 2f, 6f, 6f, 4f) * 2f)
                {
                    return false;
                }
                break;
        }
        return crit.AccessibilityResistance(getAItile(pos).acc).Allowed || (crit.canSwim && getAItile(pos).acc != AItile.Accessibility.Solid && getAItile(pos).AnyWater);
    }
}

