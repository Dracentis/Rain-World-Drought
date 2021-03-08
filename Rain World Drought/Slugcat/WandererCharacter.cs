using System;
using System.Collections.Generic;
using System.IO;
using SlugBase;
using RWCustom;
using System.Text;
using UnityEngine;

namespace Rain_World_Drought
{
    internal class WandererCharacter : SlugBaseCharacter
    {
        public WandererCharacter() : base("DroughtWanderer", FormatVersion.V1)
        {
        }

        public override string DisplayName => DroughtMod.Translate("The Wanderer");
        public override string Description => DroughtMod.Translate("Curious and calm, with a deep desire to discover the ancient mysteries around it.<LINE>In tune with the events of the world, your journey will have a significant impact on things much greater than yourself.");

        protected override void Enable()
        {
        }

        protected override void Disable()
        {
        }

        public override Color? SlugcatColor() => Custom.HSL2RGB(0.63055557f, 0.54f, 0.2f);

        public override Stream GetResource(params string[] path)
        {
            try
            {
                return File.OpenRead(JoinPaths(Custom.RootFolderDirectory(), "Mods", "DroughtAssets", "SlugBase", path));
            }
            catch
            {
                return null;
            }
        }

        // Join a bunch of strings and string arrays as parts of a path
        private static string JoinPaths(params object[] paths)
        {
            StringBuilder sb = new StringBuilder();
            char dsc = Path.DirectorySeparatorChar;

            for(int i = 0; i < paths.Length; i++)
            {
                if (sb.Length > 0 && sb[sb.Length - 1] != dsc) sb.Append(dsc);
                if (paths[i] is string[] array)
                    sb.Append(string.Join(dsc.ToString(), array));
                else if (paths[i] is string str)
                    sb.Append(str);
            }

            return sb.ToString();
        }
    }
}
