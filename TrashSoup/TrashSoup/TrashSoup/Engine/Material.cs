using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    // A container for textures and effects associated with the model
    public class Material
    {
        #region enums

        public enum EffectType
        {
            BASIC,
            SKINNED,
            DEFAULT,
            DEFAULT_SKINNED,
            NORMAL,
            NORMAL_SKINNED,
            CUBE,
            CUBE_SKINNED
        }

        #endregion

        #region variables



        #endregion

        #region properties

        public string Name { get; set; }

        public EffectType MyEffectType { get; set; }

        public Texture2D DiffuseMap { get; set; }
        public Texture2D NormalMap { get; set; }
        public Texture2D CubeMap { get; set; }

        public Vector3 SpecularColor { get; set; }
        public float Glossiness { get; set; }

        public Effect MyEffect { get; set; }

        #endregion

        #region methods

        public Material(string name)
        {
            this.Name = name;
            this.DiffuseMap = null;
            this.NormalMap = null;
            this.CubeMap = null;
            this.Glossiness = 0.0f;
            this.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public Material(string name, Effect effect)
            : this(name)
        {
            this.MyEffect = effect;
        }

        public Material(string name, Effect effect, Texture2D diffuse) : this(name, effect)
        {
            this.DiffuseMap = diffuse;
        }

        public Material(string name, Effect effect, Texture2D diffuse, Texture2D normal)
            : this(name, effect, diffuse)
        {
            this.NormalMap = normal;
        }

        public Material(string name, Effect effect, Texture2D diffuse, Texture2D normal, Texture2D cube)
            : this(name, effect, diffuse, normal)
        {
            this.CubeMap = cube;
        }

        public void UpdateEffect()
        {
            switch (this.MyEffectType)
            {
                case Material.EffectType.SKINNED:
                    (this.MyEffect as SkinnedEffect).Texture = this.DiffuseMap;
                    break;

                case Material.EffectType.DEFAULT_SKINNED:
                    break;

                case Material.EffectType.NORMAL_SKINNED:
                    break;

                case Material.EffectType.CUBE_SKINNED:
                    break;
            }
        }

        #endregion
    }
}
