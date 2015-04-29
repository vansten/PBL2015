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
        protected const float CAM_PROBE_VALUE = 0.01f;

        #endregion

        #region variables

        protected Camera cam;

        protected float tempYaw;
        protected float tempPitch;
        protected float distance = 3.0f;
        protected Vector3 collisionDisplacement = Vector3.Zero;
        protected Vector3 tempPos;
        float zoom = 1.0f;

        protected GameObject target;
        protected Transform transform;
        protected SphereCollider collider;
        protected GameTime tempGameTime;
        protected GameObject otherColl;

        #endregion

        #region methods
        public CameraBehaviourComponent(GameObject obj) : base(obj)
        {
            this.target = null;
            tempGameTime = new GameTime();
            Start();
        }

        public CameraBehaviourComponent(GameObject obj, GameObject target) : base(obj)
        {
            this.target = target;
            tempGameTime = new GameTime();
            Start();
        }

        public CameraBehaviourComponent(GameObject obj, CameraBehaviourComponent cbc) : base(obj)
        {
            this.target = cbc.target;
            tempGameTime = new GameTime();
            Start();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 camVector = InputHandler.Instance.GetCameraVector();
            tempYaw = -CAM_YAW_SENSITIVITY * (camVector.X);
            tempPitch = CAM_PITCH_SENSITIVITY * (camVector.Y);
            
            tempPos = cam.Position;
            tempPos = tempPos / Math.Max(tempPos.Length(), 0.000001f);
            tempPos = distance * tempPos;
            tempPos = Vector3.Transform(tempPos,
                Matrix.CreateFromAxisAngle(cam.Right, tempPitch));

            Vector3 dir = Vector3.Normalize(cam.Target - tempPos);
            float angle = (float)Math.Atan2((double)dir.Y, Math.Sqrt((double)(dir.X * dir.X + dir.Z * dir.Z)));

            if (angle > -1.2f && angle < 0.4f)
            {
                cam.Position = tempPos;
            }

            cam.Position = Vector3.Transform(cam.Position,
                    Matrix.CreateFromAxisAngle(cam.Up, tempYaw));

            Raycast ray = new Raycast(cam.Target + cam.Translation,
                (cam.Position - cam.Target), 
                Vector3.Distance(cam.Target, cam.Position), 0.1f);
            if (ray.Cast())
            {
                cam.Position = ray.PositionHit - cam.Translation;
            }
            //Debug.Log(ray.PositionHit.ToString());

            //if(otherColl != null)
            //{
            //    //cam.Position -= new Vector3(collisionDisplacement.X, collisionDisplacement.Y, -collisionDisplacement.Z);

            //    do
            //    {
                    
            //        cam.MyTransform.Position = cam.Position + cam.Translation;
            //        this.MyObject.MyPhysicalObject.Update(tempGameTime);
            //        this.MyObject.MyCollider.Update(tempGameTime);
            //    } while (otherColl.MyCollider.Intersects(this.MyObject.MyPhysicalObject));

            //    otherColl = null;
            //}

            cam.Translation = new Vector3(target.MyTransform.Position.X, target.MyTransform.Position.Y, -target.MyTransform.Position.Z);
        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            // do nothing since it's camera
        }

        public override void OnCollision(GameObject other)
        {
            base.OnCollision(other);

            otherColl = other;

            if (other.MyCollider.IntersectionVector != Vector3.Zero)
            {
                collisionDisplacement = other.MyCollider.IntersectionVector;
            }
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
