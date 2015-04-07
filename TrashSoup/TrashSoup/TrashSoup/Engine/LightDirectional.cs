using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.Xml;

namespace TrashSoup.Engine
{
    public class LightDirectional : GameObject, IXmlSerializable
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

        public LightDirectional(uint uniqueID, string name)
            : base(uniqueID, name)
        {

        }

        public LightDirectional(uint uniqueID, string name, Vector3 color, Vector3 specular, Vector3 direction)
            : base(uniqueID, name)
        {
            this.LightColor = color;
            this.LightSpecularColor = specular;
            this.LightDirection = direction;
        }

       
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            //reader.ReadStartElement();

            reader.ReadStartElement("LightColor");
            LightColor = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                reader.ReadElementContentAsFloat("Y", ""),
                reader.ReadElementContentAsFloat("Z", ""));
            reader.ReadEndElement();

            reader.ReadStartElement("LightSpecularColor");
            LightSpecularColor = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                reader.ReadElementContentAsFloat("Y", ""),
                reader.ReadElementContentAsFloat("Z", ""));
            reader.ReadEndElement();

            reader.ReadStartElement("LightDirection");
            LightDirection = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                reader.ReadElementContentAsFloat("Y", ""),
                reader.ReadElementContentAsFloat("Z", ""));
            reader.ReadEndElement();

            base.ReadXml(reader);
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("LightColor");
            writer.WriteElementString("X", XmlConvert.ToString(LightColor.X));
            writer.WriteElementString("Y", XmlConvert.ToString(LightColor.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(LightColor.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("LightSpecularColor");
            writer.WriteElementString("X", XmlConvert.ToString(LightSpecularColor.X));
            writer.WriteElementString("Y", XmlConvert.ToString(LightSpecularColor.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(LightSpecularColor.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("LightDirection");
            writer.WriteElementString("X", XmlConvert.ToString(LightDirection.X));
            writer.WriteElementString("Y", XmlConvert.ToString(LightDirection.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(LightDirection.Z));
            writer.WriteEndElement();

            base.WriteXml(writer);
        }

        #endregion
    }
}
