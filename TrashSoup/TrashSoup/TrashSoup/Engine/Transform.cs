﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace TrashSoup.Engine
{
    public class Transform : ObjectComponent, IXmlSerializable
    {
        #region constants

        #endregion

        #region variables

        protected Matrix worldMatrix;
        protected Vector3 position;
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
                position = value;
                CalculateWorldMatrix();
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
                rotation = value;
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
            this.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
            this.Scale = 1.0f;
        }

        public Transform(GameObject obj, Vector3 position, Vector3 forward, Vector3 rotation, float scale)
            : this(obj)
        {
            this.Position = position;
            this.Rotation = rotation;
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

            this.worldMatrix = scale * rotation * translation;
        }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }
        public void ReadXml(System.Xml.XmlReader reader)
        {

        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Position.X.ToString());
            writer.WriteElementString("Y", Position.Y.ToString());
            writer.WriteElementString("Z", Position.Z.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Rotation");
            writer.WriteElementString("X", Rotation.X.ToString());
            writer.WriteElementString("Y", Rotation.Y.ToString());
            writer.WriteElementString("Z", Rotation.Z.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Forward");
            writer.WriteElementString("X", Forward.X.ToString());
            writer.WriteElementString("Y", Forward.Y.ToString());
            writer.WriteElementString("Z", Forward.Z.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("Scale", Scale.ToString());
        }
        #endregion
    }
}
