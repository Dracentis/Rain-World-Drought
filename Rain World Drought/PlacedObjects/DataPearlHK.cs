using Rain_World_Drought.Enums;
using Rain_World_Drought.OverWorld;
using UnityEngine;

namespace Rain_World_Drought.PlacedObjects
{
    internal static class DataPearlHK
    {
        public static void Patch()
        {
            On.DataPearl.ApplyPalette += new On.DataPearl.hook_ApplyPalette(ApplyPaletteHK);
        }

        private static void ApplyPaletteHK(On.DataPearl.orig_ApplyPalette orig, DataPearl self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig.Invoke(self, sLeaser, rCam, palette);
            switch (EnumSwitch.GetAbstractDataPearlType((self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType))
            {
                default:
                case EnumSwitch.AbstractDataPearlType.DEFAULT:
                    return;

                case EnumSwitch.AbstractDataPearlType.WipedPearl:
                    self.color = new Color(0.01f, 0.01f, 0.01f);
                    self.highlightColor = new Color(1f, 1f, 1f);
                    break;
                case EnumSwitch.AbstractDataPearlType.DroughtPearl1:
                    self.color = new Color(0.5f, 0.2f, 0.2f);
                    self.highlightColor = new Color(1f, 1f, 0f);
                    break;
                case EnumSwitch.AbstractDataPearlType.DroughtPearl2:
                    self.color = new Color(0.2f, 0.5f, 0.2f);
                    self.highlightColor = new Color(0f, 1f, 1f);
                    break;
                case EnumSwitch.AbstractDataPearlType.DroughtPearl3:
                    self.color = new Color(0.2f, 0.2f, 0.5f);
                    self.highlightColor = new Color(1f, 0f, 1f);
                    break;
                case EnumSwitch.AbstractDataPearlType.MoonPearl:
                    int c = (self.abstractPhysicalObject as MoonPearl.AbstractMoonPearl).color;
                    if (c == 1) { self.color = new Color(0.7f, 0.7f, 0.7f); }
                    else if (c == 2) { self.color = new Color(0f, 0.3f, 0.5f); }
                    else { self.color = new Color(1f, 0.478431374f, 0.007843138f); }
                    self.highlightColor = new Color(1f, 1f, 1f);
                    break;
                case EnumSwitch.AbstractDataPearlType.SI_Spire1:
                    self.color = new Color(0.01f, 0.01f, 0.01f);
                    self.highlightColor = new Color(0.1f, 0.5f, 0.5f);
                    break;
                case EnumSwitch.AbstractDataPearlType.SI_Spire2:
                    self.color = new Color(0.01f, 0.01f, 0.01f);
                    self.highlightColor = new Color(0.1f, 0.3f, 0.7f);
                    break;
                case EnumSwitch.AbstractDataPearlType.SI_Spire3:
                    self.color = new Color(0.01f, 0.01f, 0.01f);
                    self.highlightColor = new Color(0.1f, 0.7f, 0.3f);
                    break;
            }
        }

        public static void ClearPearl(DataPearl self)
        {
            (self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType = EnumExt_Drought.WipedPearl;
            self.color = new Color(0.01f, 0.01f, 0.01f);
            self.highlightColor = new Color(1f, 1f, 1f);
            self.glimmerWait = 40;
            self.glimmerProg = 0f;
            self.glimmerSpeed = 1f / 30f;
        }
    }
}
