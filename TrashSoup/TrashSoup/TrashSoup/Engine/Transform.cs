using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

namespace TrashSoup.Engine
{
    public class Transform : ObjectComponent, IXmlSerializable
    {
        #region constants
        protected Matrix preRotationMatrix = Matrix.CreateRotationY(MathHelper.Pi);
        #endregion

        #region variables

        protected Matrix worldMatrix;
        protected Vector3 position;
        protected Vector3 prevPosition;
        protected Vector3 positionChangeNormal;
        protected Vector3 rotation;
        protected Vector3 forward;
        protected float scale;

        #endregion

        #region properties

        public Vector3 Position
        { 
            get
            {
                return position;
            }
            set
            {
                this.prevPosition = this.position;
                position = value;
                this.CalculatePositionChange();
                CalculateWorldMatrix();
            }
        }

        public Vector3 PositionChangeNormal
        {
            get
            {
                return this.positionChangeNormal;
            }
        }

        public Vector3 Rotation 
        { 
            get
            {
                return rotation;
            }
            set
            {
                rotation.X = MathHelper.Clamp(value.X, -MathHelper.Pi, MathHelper.Pi);
                rotation.Y = MathHelper.Clamp(value.Y, -MathHelper.Pi, MathHelper.Pi);
                rotation.Z = MathHelper.Clamp(value.Z, -MathHelper.Pi, MathHelper.Pi);
                CalculateWorldMatrix();
            }
        }
        public Vector3 Forward
        {
            get
            {
                return forward;
            }
            set
            {
                forward = value;
                CalculateWorldMatrix();
            }
        }
        public float Scale 
        { 
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                CalculateWorldMatrix();
            }
        }

        #endregion

        #region methods

        public Transform(GameObject obj) : base(obj)
        {
            this.Position = new Vector3(0.0f, 0.0f, 0.0f);
            this.prevPosition = Vector3.Zero;
            this.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
            this.Scale = 1.0f;
        }

        public Transform(GameObject obj, Vector3 position, Vector3 forward, Vector3 rotation, float scale)
            : this(obj)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.prevPosition = position;
            this.Scale = scale;
            this.Forward = forward;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // do nothing since we change worldMatrix in properties
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // do nothing
        }

        public Matrix GetWorldMatrix()
        {
            return worldMatrix;
        }

        protected override void Start()
        {
            /// do nothing
        }

        protected void CalculateWorldMatrix()
        {
            Matrix translation, rotation, scale;
            translation = Matrix.CreateTranslation(new Vector3(this.Position.X, this.Position.Y, -this.Position.Z));
            rotation = Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            scale = Matrix.CreateScale(this.Scale);

            this.worldMatrix = preRotationMatrix * scale * rotation * translation;
        }

        protected void CalculatePositionChange()
        {
            this.positionChangeNormal.X = Math.Abs(this.position.X - this.prevPosition.X);// < 0.01f ? 0.0f : 1.0f;
            this.positionChangeNormal.Y = Math.Abs(this.position.Y - this.prevPosition.Y);// < 0.01f ? 0.0f : 1.0f;
            this.positionChangeNormal.Z = Math.Abs(this.position.Z - this.prevPosition.Z);// < 0.01f ? 0.0f : 1.0f;
            if(this.positionChangeNormal.Length() > 0.0f)
            {
                this.positionChangeNormal.Normalize();
            }
            this.positionChangeNormal.Z *= -1.0f;
        }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            if(reader.Name == "Position")
            {
                reader.ReadStartElement();
                Position = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            if(reader.Name == "Rotation")
            {
                reader.ReadStartElement();
                Rotation = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            if(reader.Name == "Forward")
            {
                reader.ReadStartElement();
                Forward = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            Scale = reader.ReadElementContentAsFloat("Scale", "");

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Position");
            writer.WriteElementString("X", XmlConvert.ToString(Position.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Position.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Position.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("Rotation");
            writer.WriteElementString("X", XmlConvert.ToString(Rotation.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Rotation.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Rotation.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("Forward");
            writer.WriteElementString("X", XmlConvert.ToString(Forward.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Forward.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Forward.Z));
            writer.WriteEndElement();

            writer.WriteElementString("Scale", XmlConvert.ToString(Scale));
        }
        #endregion
    }
}
