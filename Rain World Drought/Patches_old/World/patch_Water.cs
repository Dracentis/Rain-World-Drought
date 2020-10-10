using System;
using MonoMod;
using RWCustom;
using UnityEngine;
using Noise;

    class patch_Water : Water
    {

        [MonoModIgnore]
        private Water.SurfacePoint[,] surface;

        [MonoModIgnore]
        public patch_Water(Room room, int waterLevel) : base(room, waterLevel)
        {
        }

        public void GravityForce(float left, float right, float push)
        {
            int num = this.PreviousSurfacePoint(left);
            int num2 = Custom.IntClamp(this.PreviousSurfacePoint(right) + 1, 0, this.surface.GetLength(0) - 1);
            float  center = ((float)num + (float)num2) / 2;
            for (int i = num; i <= num2; i++)
            {
                    this.surface[i, 0].height += push * ( 1f - (Mathf.Abs(center-i) / Mathf.Abs(((float)num-(float)num2)/2f)) ) ;
                    //this.surface[i, 0].pos += Custom.DegToVec(UnityEngine.Random.value * 180f * Mathf.Sign(push)) * 5f;
            }
        }


}