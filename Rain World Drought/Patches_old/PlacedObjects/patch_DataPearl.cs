using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MonoMod;

    public class patch_DataPearl : DataPearl
    {
        [MonoModIgnore]
        public patch_DataPearl(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {
        }

        public extern void orig_ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);
        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig_ApplyPalette(sLeaser, rCam, palette);
            if ((abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == (DataPearl.AbstractDataPearl.DataPearlType)patch_AbstractDataPearl.DataPearlType.DroughtPearl1)
            {
                color = new Color(0.5f, 0.2f, 0.2f);
                highlightColor = new Color(1f, 1f, 0f);
            }
            else if ((abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == (DataPearl.AbstractDataPearl.DataPearlType)patch_AbstractDataPearl.DataPearlType.wipedPearl)
            {
                color = new Color(1f, 1f, 1f);
                highlightColor = new Color(1f, 1f, 1f);
            }
            else if ((abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == (DataPearl.AbstractDataPearl.DataPearlType)patch_AbstractDataPearl.DataPearlType.DroughtPearl2)
            {
                color = new Color(0.2f, 0.5f, 0.2f);
                highlightColor = new Color(0f, 1f, 1f);
            }
            else if ((abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == (DataPearl.AbstractDataPearl.DataPearlType)patch_AbstractDataPearl.DataPearlType.DroughtPearl3)
            {
                color = new Color(0.2f, 0.2f, 0.5f);
                highlightColor = new Color(1f, 0f, 1f);
            }
            else if ((abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == (DataPearl.AbstractDataPearl.DataPearlType)patch_AbstractDataPearl.DataPearlType.MoonPearl)
            {
                int color = (abstractPhysicalObject as MoonPearl.AbstractMoonPearl).color;
                if (color != 1)
                {
                    if (color != 2)
                    {
                        this.color = new Color(1f, 0.478431374f, 0.007843138f);
                    }
                    else
                    {
                        this.color = new Color(0f, 0.3f, 0.5f);
                    }
                }
                else
                {
                    this.color = new Color(0.7f, 0.7f, 0.7f);
                }
            }else if ((abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == (DataPearl.AbstractDataPearl.DataPearlType)patch_AbstractDataPearl.DataPearlType.SI_Spire1)
            {
                this.color = new Color(0.01f, 0.01f, 0.01f);
                this.color = new Color(0.1f, 0.5f, 0.5f);
            }
            else if ((abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == (DataPearl.AbstractDataPearl.DataPearlType)patch_AbstractDataPearl.DataPearlType.SI_Spire2)
            {
                this.color = new Color(0.01f, 0.01f, 0.01f);
                this.color = new Color(0.1f, 0.3f, 0.7f);
            }
            else if ((abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == (DataPearl.AbstractDataPearl.DataPearlType)patch_AbstractDataPearl.DataPearlType.SI_Spire3)
            {
                this.color = new Color(0.01f, 0.01f, 0.01f);
                this.color = new Color(0.1f, 0.7f, 0.3f);
            }
    }

    public void clearPearl()
    {
        this.color = new Color(0f, 0f, 0f);
        this.glimmerWait = 40;
        this.glimmerProg = 0f;
        this.glimmerSpeed = 1f / 30f;
    }

    public class patch_AbstractDataPearl : DataPearl.AbstractDataPearl
    {
        [MonoModIgnore]
        public patch_AbstractDataPearl(World world, AbstractObjectType objType, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedObjectIndex, PlacedObject.ConsumableObjectData consumableData, DataPearl.AbstractDataPearl.DataPearlType dataPearlType) : base(world, objType, realizedObject, pos, ID, originRoom, placedObjectIndex, consumableData, dataPearlType)
        {
        }

        public enum DataPearlType
        {
            Misc, // 38
            Misc2, // none
            CC, // 7 Message for Pebbles'
            SI_west, //
            SI_top, //
            LF_west, //
            LF_bottom, //
            HI, //
            SH, //
            DS, //
            SB_filtration,  //
            SB_ravine, //
            GW, //
            SL_bridge, //
            SL_moon, //
            SU, //
            UW, //
            PebblesPearl, //any of pebbles' pearls
            SL_chimney, //Purposed Organisms
            Red_stomach, //unused
            MoonPearl, //any of moons pearls
            DroughtPearl1, //IS Pearl
            DroughtPearl2, //FS Pearl
            DroughtPearl3, //MW Pearl
            SI_Spire1, //
            SI_Spire2, //
            SI_Spire3, //
            wipedPearl // no text
        }
    }

}