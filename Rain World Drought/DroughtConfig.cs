using System;
using OptionalUI;
using Partiality.Modloader;
using UnityEngine;

namespace Rain_World_Drought
{
    /// <summary>
    /// Mainly for getting language settings
    /// </summary>
    public static class DroughtConfig
    {
        // To add more overrides, see the METHODS section of DroughtConfigGenerator.CreateOIType

        public static void Ctor(OptionInterface self, PartialityMod mod)
        {
        }
        
        public static void Initialize(OptionInterface self)
        {
            self.Tabs = new OpTab[1];
            self.Tabs[0] = new OpTab("Config");

            OpLabel cfgTitle = new OpLabel(new Vector2(100, 550), new Vector2(400, 40), "Put Config Here!", FLabelAlignment.Center, true);
            self.Tabs[0].AddItems(cfgTitle);
        }
        
        public static void ConfigOnChange(OptionInterface self)
        {
        }

        public static void Update(OptionInterface self, float dt)
        {
        }
    }
}
