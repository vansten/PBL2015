using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement("Color");
            LightColor = new Vector3(reader.ReadElementContentAsFloat("R", ""),
                reader.ReadElementContentAsFloat("G", ""),
                reader.ReadElementContentAsFloat("B", ""));
            reader.ReadEndElement();

            reader.ReadStartElement("SpecularColor");
            LightSpecularColor = new Vector3(reader.ReadElementContentAsFloat("R", ""),
                reader.ReadElementContentAsFloat("G", ""),
                reader.ReadElementContentAsFloat("B", ""));
            reader.ReadEndElement();

            Attenuation = reader.ReadElementContentAsFloat("Attenuation", "");

            base.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Color");
            writer.WriteElementString("R", LightColor.X.ToString());
            writer.WriteElementString("G", LightColor.Y.ToString());
            writer.WriteElementString("B", LightColor.Z.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("SpecularColor");
            writer.WriteElementString("R", LightSpecularColor.X.ToString());
            writer.WriteElementString("G", LightSpecularColor.Y.ToString());
            writer.WriteElementString("B", LightSpecularColor.Z.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("Attenuation", XmlConvert.ToString(Attenuation));

            base.WriteXml(writer);
        }

        #endregion
    }
}
