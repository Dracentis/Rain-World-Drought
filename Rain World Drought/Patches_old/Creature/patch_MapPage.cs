using DevInterface;
using UnityEngine;
using RWCustom;
using MonoMod;

[MonoModPatch("global::DevInterface.MapPage")]
public class patch_MapPage : DevInterface.MapPage
{
    [MonoModIgnore]
    public patch_MapPage(DevUI owner, string IDstring, DevUINode parentNode, string name) : base(owner, IDstring, parentNode, name)
    {
    }

    private static string CritString(AbstractCreature crit)
    {
        switch (crit.creatureTemplate.type)
        {
            case CreatureTemplate.Type.Slugcat:
                return "Sl";
            case CreatureTemplate.Type.PinkLizard:
                return "pL";
            case CreatureTemplate.Type.GreenLizard:
                return "gL";
            case CreatureTemplate.Type.BlueLizard:
                return "bL";
            case CreatureTemplate.Type.YellowLizard:
                return "yL";
            case CreatureTemplate.Type.WhiteLizard:
                return "wL";
            case CreatureTemplate.Type.RedLizard:
                return "rL";
            case CreatureTemplate.Type.BlackLizard:
                return "blL";
            case CreatureTemplate.Type.Salamander:
                return "A";
            case CreatureTemplate.Type.CyanLizard:
                return "cL";
            case CreatureTemplate.Type.Fly:
                return "b";
            case CreatureTemplate.Type.Leech:
                return "l";
            case CreatureTemplate.Type.SeaLeech:
                return "sl";
            case CreatureTemplate.Type.Snail:
                return "S";
            case CreatureTemplate.Type.Vulture:
                return "Vu";
            case CreatureTemplate.Type.GarbageWorm:
                return "Gw";
            case CreatureTemplate.Type.LanternMouse:
                return "m";
            case CreatureTemplate.Type.CicadaA:
                return "cA";
            case CreatureTemplate.Type.CicadaB:
                return "cB";
            case CreatureTemplate.Type.Spider:
                return "s";
            case CreatureTemplate.Type.JetFish:
                return "j";
            case CreatureTemplate.Type.BigEel:
                return "Lev";
            case CreatureTemplate.Type.Deer:
                return "Dr";
            case CreatureTemplate.Type.TubeWorm:
                return "tw";
            case CreatureTemplate.Type.DaddyLongLegs:
                return "Dll";
            case CreatureTemplate.Type.BrotherLongLegs:
                return "bll";
            case CreatureTemplate.Type.TentaclePlant:
                return "tp";
            case CreatureTemplate.Type.PoleMimic:
                return "pm";
            case CreatureTemplate.Type.MirosBird:
                return "M";
            case CreatureTemplate.Type.Centipede:
                return "c";
            case CreatureTemplate.Type.RedCentipede:
                return "RC";
            case CreatureTemplate.Type.Centiwing:
                return "cW";
            case CreatureTemplate.Type.SmallCentipede:
                return "sc";
            case CreatureTemplate.Type.Scavenger:
                if ((crit.abstractAI as ScavengerAbstractAI).squad == null)
                {
                    return "Sc";
                }
                if ((crit.abstractAI as ScavengerAbstractAI).squad.missionType != ScavengerAbstractAI.ScavengerSquad.MissionID.None)
                {
                    switch ((crit.abstractAI as ScavengerAbstractAI).squad.missionType)
                    {
                        case ScavengerAbstractAI.ScavengerSquad.MissionID.GuardOutpost:
                            return "Sc(G)";
                        case ScavengerAbstractAI.ScavengerSquad.MissionID.HuntCreature:
                            return "Sc(H)";
                        case ScavengerAbstractAI.ScavengerSquad.MissionID.ProtectCreature:
                            return "Sc(P)";
                        case ScavengerAbstractAI.ScavengerSquad.MissionID.Trade:
                            return "Sc(T)";
                    }
                }
                return "Sc";
            case CreatureTemplate.Type.Overseer:
                return "o";
            case CreatureTemplate.Type.EggBug:
                return "eb";
            case CreatureTemplate.Type.BigSpider:
                return "sp";
            case CreatureTemplate.Type.SpitterSpider:
                return "SP";
            case CreatureTemplate.Type.SmallNeedleWorm:
                return "nw";
            case CreatureTemplate.Type.BigNeedleWorm:
                return "Bnw";
            case CreatureTemplate.Type.DropBug:
                return "db";
            case CreatureTemplate.Type.KingVulture:
                return "KVu";
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast:
                return "WB";
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard:
                return "grL";
            case (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake:
                return "SD";
        }
        return string.Empty;
    }

    public class patch_CreatureVis : CreatureVis
    {

        [MonoModIgnore]
        public patch_CreatureVis(MapPage myMapPage, AbstractCreature crit) : base(myMapPage, crit)
        {
        }

        [MonoModConstructor]
        public void ctor(MapPage myMapPage, AbstractCreature crit)
        {
            this.myMapPage = myMapPage;
            this.crit = crit;
            label = new FLabel("font", CritString(crit));
            label2 = new FLabel("font", CritString(crit));
            label2.color = new Color(1f - label.color.r, 1f - label.color.g, 1f - label.color.b);
            sprite = new FSprite("pixel", true);
            sprite.anchorY = 0f;
            if (crit.abstractAI != null)
            {
                sprite2 = new FSprite("pixel", true);
                sprite2.alpha = 0.25f;
                sprite2.anchorY = 0f;
                Futile.stage.AddChild(sprite2);
            }
            Futile.stage.AddChild(sprite);
            Futile.stage.AddChild(label);
        }
        
        public void Update()
        {
            if (crit.pos != lastPos)
            {
                lastPos = crit.pos;
                if (!drag)
                {
                    dragPos = drawPos;
                }
                drag = true;
            }
            drawPos = myMapPage.CreatureVisPos(crit.pos, crit.InDen, true);
            if (crit.realizedCreature == null && crit.distanceToMyNode < 0)
            {
                drawPos.y = drawPos.y + Mathf.Lerp(-2f, 2f, Random.value);
            }
            if (crit.InDen)
            {
                drawPos.y = drawPos.y - 10f;
            }
            sprite.x = drawPos.x;
            sprite.y = drawPos.y;
            if (drag)
            {
                dragPos += Vector2.ClampMagnitude(drawPos - dragPos, 10f);
                sprite.scaleY = Vector2.Distance(drawPos, dragPos);
                sprite.rotation = Custom.AimFromOneVectorToAnother(drawPos, dragPos);
                if (Custom.DistLess(dragPos, drawPos, 5f))
                {
                    drag = false;
                }
            }
            else if (crit.pos.NodeDefined)
            {
                Vector2 vector = myMapPage.NodeVisPos(crit.pos.room, crit.pos.abstractNode);
                sprite.scaleY = Vector2.Distance(drawPos, vector);
                sprite.rotation = Custom.AimFromOneVectorToAnother(drawPos, vector);
            }
            else
            {
                sprite.scaleY = 10f;
                sprite.rotation = 135f;
            }
            if (crit.abstractAI != null)
            {
                Vector2 vector2 = myMapPage.CreatureVisPos(crit.abstractAI.destination, false, false);
                sprite2.x = drawPos.x;
                sprite2.y = drawPos.y;
                sprite2.color = Color.Lerp(new Color(1f, 1f, 1f), label.color, Random.value);
                sprite2.scaleY = Vector2.Distance(drawPos, vector2);
                sprite2.rotation = Custom.AimFromOneVectorToAnother(drawPos, vector2);
            }
            label.x = drawPos.x;
            label.y = drawPos.y;
            label.color = CritCol(crit);
            sprite.color = label.color;
            label2.x = drawPos.x + 1f;
            label2.y = drawPos.y - 1f;
            label.text = CritString(crit);
            label2.text = label.text;
            if (crit.slatedForDeletion)
            {
                Destroy();
            }
        }

        // Token: 0x06000BEB RID: 3051 RVA: 0x000768B0 File Offset: 0x00074AB0
        public void Destroy()
        {
            label.RemoveFromContainer();
            label2.RemoveFromContainer();
            sprite.RemoveFromContainer();
            if (sprite2 != null)
            {
                sprite2.RemoveFromContainer();
            }
            slatedForDeletion = true;
        }

        // Token: 0x06000BEC RID: 3052 RVA: 0x000768F8 File Offset: 0x00074AF8
        private static string CritString(AbstractCreature crit)
        {
            switch (crit.creatureTemplate.type)
            {
                case CreatureTemplate.Type.Slugcat:
                    return "Sl";
                case CreatureTemplate.Type.PinkLizard:
                    return "L";
                case CreatureTemplate.Type.GreenLizard:
                    return "L";
                case CreatureTemplate.Type.BlueLizard:
                    return "L";
                case CreatureTemplate.Type.YellowLizard:
                    return "L";
                case CreatureTemplate.Type.WhiteLizard:
                    return "L";
                case CreatureTemplate.Type.RedLizard:
                    return "L";
                case CreatureTemplate.Type.BlackLizard:
                    return "L";
                case CreatureTemplate.Type.Salamander:
                    return "A";
                case CreatureTemplate.Type.CyanLizard:
                    return "L";
                case CreatureTemplate.Type.Fly:
                    return "b";
                case CreatureTemplate.Type.Leech:
                    return "l";
                case CreatureTemplate.Type.SeaLeech:
                    return "sl";
                case CreatureTemplate.Type.Snail:
                    return "S";
                case CreatureTemplate.Type.Vulture:
                    return "Vu";
                case CreatureTemplate.Type.GarbageWorm:
                    return "Gw";
                case CreatureTemplate.Type.LanternMouse:
                    return "m";
                case CreatureTemplate.Type.CicadaA:
                    return "cA";
                case CreatureTemplate.Type.CicadaB:
                    return "cB";
                case CreatureTemplate.Type.Spider:
                    return "s";
                case (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake:
                    return "SD";
                case CreatureTemplate.Type.JetFish:
                    return "j";
                case CreatureTemplate.Type.BigEel:
                    return "Lev";
                case CreatureTemplate.Type.Deer:
                    return "Dr";
                case CreatureTemplate.Type.TubeWorm:
                    return "tw";
                case CreatureTemplate.Type.DaddyLongLegs:
                    return "Dll";
                case CreatureTemplate.Type.BrotherLongLegs:
                    return "bll";
                case CreatureTemplate.Type.TentaclePlant:
                    return "tp";
                case CreatureTemplate.Type.PoleMimic:
                    return "pm";
                case CreatureTemplate.Type.MirosBird:
                    return "M";
                case CreatureTemplate.Type.Centipede:
                    return "c";
                case CreatureTemplate.Type.RedCentipede:
                    return "RC";
                case CreatureTemplate.Type.Centiwing:
                    return "cW";
                case CreatureTemplate.Type.SmallCentipede:
                    return "sc";
                case CreatureTemplate.Type.Scavenger:
                    if ((crit.abstractAI as ScavengerAbstractAI).squad == null)
                    {
                        return "Sc";
                    }
                    if ((crit.abstractAI as ScavengerAbstractAI).squad.missionType != ScavengerAbstractAI.ScavengerSquad.MissionID.None)
                    {
                        switch ((crit.abstractAI as ScavengerAbstractAI).squad.missionType)
                        {
                            case ScavengerAbstractAI.ScavengerSquad.MissionID.GuardOutpost:
                                return "Sc(G)";
                            case ScavengerAbstractAI.ScavengerSquad.MissionID.HuntCreature:
                                return "Sc(H)";
                            case ScavengerAbstractAI.ScavengerSquad.MissionID.ProtectCreature:
                                return "Sc(P)";
                            case ScavengerAbstractAI.ScavengerSquad.MissionID.Trade:
                                return "Sc(T)";
                        }
                    }
                    return "Sc";
                case CreatureTemplate.Type.Overseer:
                    return "o";
                case CreatureTemplate.Type.EggBug:
                    return "eb";
                case CreatureTemplate.Type.BigSpider:
                    return "sp";
                case CreatureTemplate.Type.SpitterSpider:
                    return "SP";
                case CreatureTemplate.Type.SmallNeedleWorm:
                    return "nw";
                case CreatureTemplate.Type.BigNeedleWorm:
                    return "Bnw";
                case CreatureTemplate.Type.DropBug:
                    return "db";
                case CreatureTemplate.Type.KingVulture:
                    return "KVu";
                case (CreatureTemplate.Type)patch_CreatureTemplate.Type.WalkerBeast:
                    return "WB";
                case (CreatureTemplate.Type)patch_CreatureTemplate.Type.GreyLizard:
                    return "grL";
            }
            return string.Empty;
        }

        // Token: 0x06000BED RID: 3053 RVA: 0x00076B4C File Offset: 0x00074D4C
        private static Color CritCol(AbstractCreature crit)
        {
            if (crit.InDen && Random.value < 0.5f)
            {
                return new Color(0.5f, 0.5f, 0.5f);
            }
            switch (crit.creatureTemplate.type)
            {
                case CreatureTemplate.Type.Slugcat:
                    return new Color(1f, 1f, 1f);
                case CreatureTemplate.Type.PinkLizard:
                    return new Color(1f, 0f, 1f);
                case CreatureTemplate.Type.GreenLizard:
                    return new Color(0f, 1f, 0.2f);
                case CreatureTemplate.Type.BlueLizard:
                    return new Color(0f, 0.4f, 1f);
                case CreatureTemplate.Type.YellowLizard:
                    return new Color(1f, 0.7f, 0f);
                case CreatureTemplate.Type.WhiteLizard:
                    return new Color(1f, 1f, 1f);
                case CreatureTemplate.Type.RedLizard:
                    return new Color(1f, 0f, 0f);
                case CreatureTemplate.Type.BlackLizard:
                    return new Color(0.1f, 0.1f, 0.1f);
                case CreatureTemplate.Type.Salamander:
                    return new Color(1f, 0.7f, 0.7f);
                case CreatureTemplate.Type.CyanLizard:
                    return new Color(0f, 0.8f, 1f);
                case CreatureTemplate.Type.Fly:
                    return new Color(0.5f, 0.5f, 0.7f);
                case CreatureTemplate.Type.Leech:
                    return new Color(1f, 0.5f, 0.5f);
                case CreatureTemplate.Type.SeaLeech:
                    return new Color(0.5f, 0.5f, 1f);
                case CreatureTemplate.Type.Snail:
                    return new Color(0.4f, 0.8f, 0.6f);
                case CreatureTemplate.Type.Vulture:
                    return new Color(0.6f, 0.4f, 0.15f);
                case CreatureTemplate.Type.GarbageWorm:
                    return new Color(0.3f, 0.3f, 0.3f);
                case CreatureTemplate.Type.LanternMouse:
                    return new Color(1f, 1f, 0.7f);
                case CreatureTemplate.Type.CicadaA:
                    return new Color(1f, 1f, 1f);
                case CreatureTemplate.Type.CicadaB:
                    return new Color(0.1f, 0.1f, 0.1f);
                case CreatureTemplate.Type.Spider:
                    return new Color(0.1f, 0.1f, 0.1f);
                case CreatureTemplate.Type.JetFish:
                    return new Color(0.1f, 0.2f, 0.1f);
                case (CreatureTemplate.Type)patch_CreatureTemplate.Type.SeaDrake:
                    return new Color(0.7f, 0.7f, 0.7f);
                case CreatureTemplate.Type.BigEel:
                    return new Color(0.1f, 0.3f, 0.3f);
                case CreatureTemplate.Type.Deer:
                    return new Color(0.8f, 1f, 0.8f);
                case CreatureTemplate.Type.TubeWorm:
                    return new Color(0f, 0.4f, 0.6f);
                case CreatureTemplate.Type.DaddyLongLegs:
                    return new Color(0f, 0f, 1f);
                case CreatureTemplate.Type.BrotherLongLegs:
                    return new Color(0.6f, 0.8f, 0f);
                case CreatureTemplate.Type.TentaclePlant:
                    return new Color(1f, 0f, 0f);
                case CreatureTemplate.Type.PoleMimic:
                    return new Color(0f, 0f, 0f);
                case CreatureTemplate.Type.MirosBird:
                    return new Color(0.5f, 0f, 0.5f);
                case CreatureTemplate.Type.Centipede:
                    return new Color(1f, 0.7f, 0f);
                case CreatureTemplate.Type.RedCentipede:
                    return Color.red;
                case CreatureTemplate.Type.Centiwing:
                    return new Color(0f, 1f, 0.2f);
                case CreatureTemplate.Type.SmallCentipede:
                    return new Color(1f, 0.7f, 0f);
                case CreatureTemplate.Type.Scavenger:
                    if ((crit.abstractAI as ScavengerAbstractAI).freeze > 0)
                    {
                        return new Color(0.5f, 0.5f, 0.5f);
                    }
                    if ((crit.abstractAI as ScavengerAbstractAI).squad == null)
                    {
                        return new Color(0f, 0.2f, 0.14f);
                    }
                    if (Random.value < 0.3f && (crit.abstractAI as ScavengerAbstractAI).squad.missionType != ScavengerAbstractAI.ScavengerSquad.MissionID.None)
                    {
                        switch ((crit.abstractAI as ScavengerAbstractAI).squad.missionType)
                        {
                            case ScavengerAbstractAI.ScavengerSquad.MissionID.GuardOutpost:
                                return new Color(0f, 0f, 1f);
                            case ScavengerAbstractAI.ScavengerSquad.MissionID.HuntCreature:
                                return new Color(1f, 0f, 0f);
                            case ScavengerAbstractAI.ScavengerSquad.MissionID.ProtectCreature:
                                return new Color(0f, 1f, 0f);
                            case ScavengerAbstractAI.ScavengerSquad.MissionID.Trade:
                                return new Color(1f, 1f, 0f);
                        }
                    }
                    if ((crit.abstractAI as ScavengerAbstractAI).squad.leader == crit)
                    {
                        return Color.Lerp((crit.abstractAI as ScavengerAbstractAI).squad.color, new Color(1f, 1f, 1f), Random.value);
                    }
                    return (crit.abstractAI as ScavengerAbstractAI).squad.color;
                case CreatureTemplate.Type.Overseer:
                    if (Random.value < 0.5f && (crit.abstractAI as OverseerAbstractAI).playerGuide)
                    {
                        return new Color(1f, 1f, 0f);
                    }
                    return new Color(0.447058827f, 0.9019608f, 0.768627465f);
                case CreatureTemplate.Type.EggBug:
                    return new Color(0f, 1f, 0f);
                case CreatureTemplate.Type.BigSpider:
                    return new Color(1f, 1f, 0f);
                case CreatureTemplate.Type.SpitterSpider:
                    return new Color(1f, 0f, 0f);
                case CreatureTemplate.Type.SmallNeedleWorm:
                    return ((crit.abstractAI as NeedleWormAbstractAI).mother == null) ? new Color(1f, 0f, 0f) : new Color(0f, 1f, 0f);
                case CreatureTemplate.Type.KingVulture:
                    return new Color(1f, 0f, 0f);
            }
            return new Color(1f, 1f, 1f);
        }

        // Token: 0x04000A36 RID: 2614
        public MapPage myMapPage;

        // Token: 0x04000A37 RID: 2615
        public AbstractCreature crit;

        // Token: 0x04000A38 RID: 2616
        public WorldCoordinate lastPos;

        // Token: 0x04000A39 RID: 2617
        public FLabel label;

        // Token: 0x04000A3A RID: 2618
        public FLabel label2;

        // Token: 0x04000A3B RID: 2619
        public bool slatedForDeletion;

        // Token: 0x04000A3C RID: 2620
        public FSprite sprite;

        // Token: 0x04000A3D RID: 2621
        public FSprite sprite2;

        // Token: 0x04000A3E RID: 2622
        private Vector2 drawPos;

        // Token: 0x04000A3F RID: 2623
        private Vector2 dragPos;

        // Token: 0x04000A40 RID: 2624
        private bool drag;


    }
}



