using System;
using System.Runtime.CompilerServices;

namespace Rain_World_Drought.OverWorld
{
    internal static class OracleHK
    {
        public static void Patch()
        {
            On.Oracle.Collide += new On.Oracle.hook_Collide(CollideHK);
        }

        private static void CollideHK(On.Oracle.orig_Collide orig, Oracle self, PhysicalObject otherObject, int myChunk, int otherChunk)
        {
            //base Collide

            orig.Invoke(self, otherObject, myChunk, otherChunk);
        }
    }
}
