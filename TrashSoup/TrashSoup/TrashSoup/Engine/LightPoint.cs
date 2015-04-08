using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrashSoup.Engine
{
    public class LightPoint : GameObject, IXmlSerializable
    {
        #region variables

        protected float attenuation;

        #endregion

        #region properties

        public Vector3 LightColor { get; set; }
        public Vector3 LightSpecularColor { get; set; }
        public float Attenuation 
        { 
            get
            {
                return attenuation;
            }
            set
            {
                attenuation = value;
                if(MyTransform != null)
                {
                    attenuation *= MyTransform.Scale;
                }
            }
        }

        #endregion

        #region methods

        public LightPoint(uint uniqueID, string name)
            : base(uniqueID, name)
        {

        }

        public LightPoint(uint uniqueID, string name, Vector3 lightColor, Vector3 lightSpecularColor, float attenuation)
            : base(uniqueID, name)
        {
            this.LightColor = lightColor;
            this.LightSpecularColor = lightSpecularColor;
            this.Attenuation = attenuation;
        }

        public void MultiplyAttenuationByScale()
        {
            if (MyTransform != null)
            {
                this.Attenuation *= this.MyTransform.Scale;
            }
            else
            {
                Debug.Log("LightPoint: No Transfrom attached");
            }
        }

        #endregion
    }
}
