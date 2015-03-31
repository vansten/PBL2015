﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class Camera : GameObject, IXmlSerializable
    {
        #region variables
        protected GameObject toFollow;
        #endregion
       
        #region properties

        public Matrix ViewMatrix
        {
            get;
            protected set;
        }

        public Matrix ProjectionMatrix
        {
            get;
            protected set;
        }

        public Vector3 Position { get; set; }
        public Vector3 Translation { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Right { get; set; }
        public float Speed { get; set; }
        public float FOV { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }


        #endregion

        #region methods
        public Camera(uint uniqueID, string name, Vector3 pos, Vector3 translation, Vector3 target, Vector3 up, float fov, float near, float far) 
            : base(uniqueID, name)
        {
            this.Position = pos;
            this.Translation = translation;
            this.Direction = target - pos;
            this.Target = target;
            this.Direction.Normalize();
            this.Up = up;
            this.Up.Normalize();
            this.Right = Vector3.Cross(this.Direction, this.Up);
            this.FOV = fov;
            this.Near = near;
            this.Far = far;

            CreateLookAt();

            CreateProjection(fov, near, far);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Direction = this.Target - this.Position;
            this.Direction = this.Direction / this.Direction.Length();
            this.Right = Vector3.Cross(this.Direction, this.Up);
            this.Right = this.Right / this.Right.Length();

            CreateLookAt();
        }

        public Vector3 GetDirection() { return Direction; }

        public void CreateProjection(float fov, float near, float far)
        {
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView
            (
                this.FOV,
                (float)TrashSoupGame.Instance.Window.ClientBounds.Width / (float)TrashSoupGame.Instance.Window.ClientBounds.Height,
                this.Near,
                this.Far
            );
        }

        protected void CreateLookAt()
        {
            this.ViewMatrix = Matrix.CreateLookAt(Position + Translation, Target + Translation, Up);
        }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);

            if(reader.Name == "CameraPosition")
            {
                reader.ReadStartElement();
                Position = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            if (reader.Name == "CameraTranslation")
            {
                reader.ReadStartElement();
                Translation = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            if (reader.Name == "CameraDirection")
            {
                reader.ReadStartElement();
                Direction = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            if(reader.Name == "CameraUp")
            {
                reader.ReadStartElement();
                Up = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            if(reader.Name == "CameraTarget")
            {
                reader.ReadStartElement();
                Target = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();

            }

            if (reader.Name == "CameraRight")
            {
                reader.ReadStartElement();
                Right = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();

            }

            FOV = reader.ReadElementContentAsFloat("FOV", "");
            Near = reader.ReadElementContentAsFloat("Near", "");
            Far = reader.ReadElementContentAsFloat("Far", "");

            CreateLookAt();
            CreateProjection(FOV, Near, Far);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("CameraPosition");
            writer.WriteElementString("X", XmlConvert.ToString(Position.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Position.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Position.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("CameraTranslation");
            writer.WriteElementString("X", XmlConvert.ToString(Translation.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Translation.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Translation.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("CameraDirection");
            writer.WriteElementString("X", XmlConvert.ToString(Direction.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Direction.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Direction.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("CameraUp");
            writer.WriteElementString("X", Up.X.ToString());
            writer.WriteElementString("Y", Up.Y.ToString());
            writer.WriteElementString("Z", Up.Z.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("CameraTarget");
            writer.WriteElementString("X", XmlConvert.ToString(Target.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Target.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Target.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("CameraRight");
            writer.WriteElementString("X", XmlConvert.ToString(Right.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Right.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Right.Z));
            writer.WriteEndElement();

            writer.WriteElementString("FOV", XmlConvert.ToString(FOV));
            writer.WriteElementString("Near", XmlConvert.ToString(Near));
            writer.WriteElementString("Far", XmlConvert.ToString(Far));
        }
        #endregion
    }
}
