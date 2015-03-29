using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class CameraBehaviourComponent : ObjectComponent, IXmlSerializable
    {
        #region constants

        protected const float CAM_YAW_SENSITIVITY = MathHelper.PiOver4 / 30.0f;
        protected const float CAM_PITCH_SENSITIVITY = MathHelper.PiOver4 / 30.0f;
        protected const float CAM_TOTAL_PITCH = MathHelper.PiOver2 - MathHelper.PiOver4/2.0f;
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

        public CameraBehaviourComponent(GameObject obj, GameObject target) : base(obj)
        {
            this.target = target;

            Start();
        }

        public override void Update(GameTime gameTime)
        {
            tempYaw = -CAM_YAW_SENSITIVITY * (InputManager.Instance.GetRightStickValue().X);
            tempPitch = CAM_PITCH_SENSITIVITY * (InputManager.Instance.GetRightStickValue().Y);

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

        public override void Draw(GameTime gameTime)
        {
            // do nothing since it's camera
        }

        protected override void Start()
        {
            cam = (Camera)MyObject;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("TargetID", target.UniqueID.ToString());
        }

        #endregion
    }
}
