using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    public class SkyboxMaterial : Material
    {
        #region effectParameters
        protected EffectParameter epCubeMap1;
        protected EffectParameter epCubeMap2;
        protected EffectParameter epCubeMap3;
        protected EffectParameter epProbes;
        #endregion

        #region properties
        public TextureCube CubeMap1 { get; set;}
        public TextureCube CubeMap2 { get; set; }
        public TextureCube CubeMap3 { get; set; }

        public Vector4 Probes { get; set; }
        #endregion

        #region methods

        public SkyboxMaterial(string name, Effect effect)
            : base(name, effect)
        {
            this.CubeMap1 = ResourceManager.Instance.LoadTextureCube("DefaultCube");
            this.CubeMap2 = ResourceManager.Instance.LoadTextureCube("DefaultCube");
            this.CubeMap3 = ResourceManager.Instance.LoadTextureCube("DefaultCube");
            this.Probes = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        }

        public override void UpdateEffect(Effect effect, Microsoft.Xna.Framework.Matrix world, Microsoft.Xna.Framework.Matrix worldViewProj, 
            LightAmbient amb, LightDirectional[] dirs, List<LightPoint> points, Texture gSM, TextureCube point0SM, Microsoft.Xna.Framework.Vector3 eyeVector, 
            BoundingFrustumExtended frustum, Microsoft.Xna.Framework.Matrix[] bones, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(epCubeMap1 != null)
            {
                epCubeMap1.SetValue(CubeMap1);
            }
            if (epCubeMap2 != null)
            {
                epCubeMap2.SetValue(CubeMap2);
            }
            if (epCubeMap3 != null)
            {
                epCubeMap3.SetValue(CubeMap3);
            }
            if (epProbes != null)
            {
                epProbes.SetValue(Probes);
            }
            base.UpdateEffect(effect, world, worldViewProj, amb, dirs, points, gSM, point0SM, eyeVector, frustum, bones, gameTime);
        }

        public override void AssignParamsInitialize()
        {
            base.AssignParamsInitialize();

            int pNameHash;

            int cmp1 = ("CubeMap1").GetHashCode();
            int cmp2 = ("CubeMap2").GetHashCode();
            int cmp3 = ("CubeMap3").GetHashCode();
            int pr = ("Probes").GetHashCode();

            foreach (EffectParameter p in MyEffect.Parameters)
            {
                pNameHash = p.Name.GetHashCode();
                if (pNameHash == cmp1)
                {
                    epCubeMap1 = p;
                }
                else if (pNameHash == cmp2)
                {
                    epCubeMap2 = p;
                }
                else if (pNameHash == cmp3)
                {
                    epCubeMap3 = p;
                }
                else if (pNameHash == pr)
                {
                    epProbes = p;
                }
            }
        }

        #endregion
    }
}
