using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    public class LightDirectional : GameObject
    {
        #region variables

        private Vector3 lightDirection;

        #endregion

        #region properties

        public Vector3 LightColor { get; set; }
        public Vector3 LightSpecularColor { get; set; }
        public Vector3 LightDirection 
        { 
            get
            {
                return lightDirection;
            }
            set
            {
                lightDirection = Vector3.Normalize(value);
            }
        }

        #endregion

        #region methods

        public LightDirectional(uint uniqueID, string name, Vector3 color, Vector3 specular, Vector3 direction)
            : base(uniqueID, name)
        {
            this.LightColor = color;
            this.LightSpecularColor = specular;
            this.LightDirection = direction;
        }

        #endregion
    }
}
