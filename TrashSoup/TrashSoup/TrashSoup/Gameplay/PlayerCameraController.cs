using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using TrashSoup.Engine;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Gameplay
{
    public class CameraBehaviourComponent : ObjectComponent, IXmlSerializable
    {
        #region constants

        protected const float CAM_YAW_SENSITIVITY = MathHelper.PiOver4 / 30.0f;
        protected const float CAM_PITCH_SENSITIVITY = MathHelper.PiOver4 / 30.0f;
        protected const float CAM_TOTAL_PITCH = MathHelper.PiOver2 - MathHelper.PiOver4;
        protected const float CAM_DISTANCE = 60.0f;

        #endregion

        #region variables

        protected Camera cam;

        protected float tempYaw;
        protected float currentPitch = 0.0f;
        protected float tempPitch;

        protected GameObject target;

        #endregion

        #region methods
        public CameraBehaviourComponent(GameObject obj) : base(obj)
        {
            this.target = null;

            Start();
        }

        public CameraBehaviourComponent(GameObject obj, GameObject target) : base(obj)
        {
            this.target = target;

            Start();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 camVector = InputHandler.Instance.GetCameraVector();
            tempYaw = -CAM_YAW_SENSITIVITY * (camVector.X);
            tempPitch = CAM_PITCH_SENSITIVITY * (camVector.Y);

            // TODO: secure this differently, probably counting up actual pitch angle from vectors

            if (Math.Abs(currentPitch + tempPitch) < CAM_TOTAL_PITCH)
            {
                currentPitch += tempPitch;
                cam.Position = cam.Position / Math.Max(cam.Position.Length(), 0.000001f);
                cam.Position = CAM_DISTANCE * cam.Position;
                cam.Position = Vector3.Transform(cam.Position,
                    Matrix.CreateFromAxisAngle(cam.Right, tempPitch));
            }

            cam.Position = Vector3.Transform(cam.Position,
                    Matrix.CreateFromAxisAngle(cam.Up, tempYaw));

            cam.Translation = new Vector3(target.MyTransform.Position.X, target.MyTransform.Position.Y, -target.MyTransform.Position.Z);
        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            // do nothing since it's camera
        }

        protected override void Start()
        {
            cam = (Camera)MyObject;
        }

        public override System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            base.ReadXml(reader);

            target = ResourceManager.Instance.CurrentScene.GetObject((uint)reader.ReadElementContentAsInt("TargetID", ""));
            //target = ResourceManager.Instance.CurrentScene.GetObject(tmp);
            if(reader.Name == "TargetPosition")
            {
                reader.ReadStartElement();
                this.target.MyTransform.Position = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteElementString("TargetID", this.target.UniqueID.ToString());
            writer.WriteStartElement("TargetPosition");
            writer.WriteElementString("X", XmlConvert.ToString(this.target.MyTransform.Position.X));
            writer.WriteElementString("Y", XmlConvert.ToString(this.target.MyTransform.Position.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(this.target.MyTransform.Position.Z));
            writer.WriteEndElement();
        }

        #endregion
    }
}
