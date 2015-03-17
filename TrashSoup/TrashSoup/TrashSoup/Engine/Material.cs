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
        #region variables



        #endregion

        #region properties

        public Texture2D Diffuse { get; set; }

        public Effect MyEffect { get; set; }

        #endregion

        #region methods

        public Material(Texture2D diffuse, Effect effect)
        {
            this.Diffuse = diffuse;
            this.MyEffect = effect;
        }

        #endregion
    }
}
