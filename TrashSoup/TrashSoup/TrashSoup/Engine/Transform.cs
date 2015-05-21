﻿using System;
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
                Vector3 tmp = this.prevPosition;
                this.positionChangeNormal = value - this.position;
                this.prevPosition = this.position;
                position.X = value.X;
                CalculateWorldMatrix();
                if (!TrashSoupGame.Instance.EditorMode)
                {
                    if (this.MyObject.MyCollider != null) this.MyObject.MyCollider.UpdateCollider();
                    if (!PhysicsManager.Instance.CanMove(this.MyObject))
                    {
                        this.position = this.prevPosition;
                        this.prevPosition = tmp;
                        this.positionChangeNormal = Vector3.Zero;
                        if (this.MyObject.MyCollider != null) this.MyObject.MyCollider.UpdateCollider();
                        CalculateWorldMatrix();
                    }
                }
                this.prevPosition = this.position;
                position.Y = value.Y;
                CalculateWorldMatrix();
                if (!TrashSoupGame.Instance.EditorMode)
                {
                    if (this.MyObject.MyCollider != null) this.MyObject.MyCollider.UpdateCollider();
                    if (!PhysicsManager.Instance.CanMove(this.MyObject))
                    {
                        this.position = this.prevPosition;
                        this.prevPosition = tmp;
                        this.positionChangeNormal = Vector3.Zero;
                        if (this.MyObject.MyCollider != null) this.MyObject.MyCollider.UpdateCollider();
                        CalculateWorldMatrix();
                    }
                }
                this.prevPosition = this.position;
                position.Z = value.Z;
                CalculateWorldMatrix();
                if (!TrashSoupGame.Instance.EditorMode)
                {
                    if (this.MyObject.MyCollider != null) this.MyObject.MyCollider.UpdateCollider();
                    if (!PhysicsManager.Instance.CanMove(this.MyObject))
                    {
                        this.position = this.prevPosition;
                        this.prevPosition = tmp;
                        this.positionChangeNormal = Vector3.Zero;
                        if (this.MyObject.MyCollider != null) this.MyObject.MyCollider.UpdateCollider();
                        CalculateWorldMatrix();
                    }
                }
                if(PositionChanged != null) PositionChanged(this, null);
            }
        }

        public Vector3 PreviousPosition
        {
            get
            {
                return this.prevPosition;
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

        #region events

        public delegate void PositionChangedEventHandler(object sender, EventArgs e);

        public event PositionChangedEventHandler PositionChanged;

        #endregion

        #region methods

        public Transform(GameObject obj) : base(obj)
        {
            this.Position = Vector3.Zero;
            this.prevPosition = Vector3.Zero;
            this.Rotation = Vector3.Zero;
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

        public Transform(GameObject obj, Transform t) : base(obj)
        {
            this.Position = t.Position;
            this.Rotation = t.Rotation;
            this.prevPosition = t.prevPosition;
            this.Scale = t.Scale;
            this.Forward = t.Forward;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(MyObject.Dynamic)
            {
                CalculateWorldMatrix();
            }
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
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
            Matrix translation, rotation, scale, fromSocket;
            translation = Matrix.CreateTranslation(new Vector3(this.Position.X, this.Position.Y, -this.Position.Z));
            rotation = Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            scale = Matrix.CreateScale(this.Scale);
            fromSocket = Matrix.Identity;

            if(MyObject.MyCarrierSocket != null)
            {
                fromSocket = MyObject.MyCarrierSocket.BoneTransform;

                Vector3 trans, scl;
                Quaternion quat;
                MyObject.MyCarrierSocket.Carrier.MyTransform.GetWorldMatrix().Decompose(out scl, out quat, out trans);
                fromSocket = fromSocket * Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(trans);
            }

            this.worldMatrix = preRotationMatrix * scale * rotation * translation * fromSocket;
        }

        protected void CalculatePositionChange()
        {
            this.positionChangeNormal.X = Math.Abs(this.position.X - this.prevPosition.X);// < 0.01f ? 0.0f : 1.0f;
            this.positionChangeNormal.Y = Math.Abs(this.position.Y - this.prevPosition.Y);// < 0.01f ? 0.0f : 1.0f;
            this.positionChangeNormal.Z = Math.Abs(this.position.Z - this.prevPosition.Z);// < 0.01f ? 0.0f : 1.0f;
        }

        public override System.Xml.Schema.XmlSchema GetSchema() { return null; }
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            base.ReadXml(reader);
            //MyObject = ResourceManager.Instance.CurrentScene.GetObject(tmp);

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

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);

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
