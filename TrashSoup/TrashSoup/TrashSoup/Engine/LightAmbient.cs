using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    public class LightAmbient : GameObject
    {
        public Vector3 LightColor { get; set; }
        public Vector3 LightSpecularColor { get; set; }

        public LightAmbient(uint uniqueID, string name, Vector3 color, Vector3 specular)
            : base(uniqueID, name)
        {
            this.LightColor = color;
            this.LightSpecularColor = specular;
        }
    }
}
