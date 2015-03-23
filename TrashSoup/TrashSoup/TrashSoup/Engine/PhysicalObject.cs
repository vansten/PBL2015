using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    public enum RotationConstraintsEnum
    {
        None,
        X,
        Y,
        Z,
        XandY,
        XandZ,
        YandZ,
        All
    }

    public enum PositionConstraintsEnum
    {
        None,
        X,
        Y,
        Z,
        XandY,
        XandZ,
        YandZ,
        All
    }

    //TODO:
    //Improve Update function
    //Add function to deal with collisions
    public class PhysicalObject : ObjectComponent, IXmlSerializable
    {
        #region Variables

        private Vector3 acceleration;

        #endregion

        #region Properties

        public float Mass { get; set; }
        public float DragFactor { get; set; }
        public bool IsUsingGravity { get; set; }
        public Vector3 Velocity { get; set; }
        public RotationConstraintsEnum RotationConstraints { get; set; }
        public PositionConstraintsEnum PositionConstraints { get; set; }

        public bool Sleeping { get; set; }

        #endregion

        #region Methods

        public PhysicalObject(GameObject gameObj) : base(gameObj)
        {
            this.Sleeping = false;
            this.RotationConstraints = RotationConstraintsEnum.None;
            this.PositionConstraints = PositionConstraintsEnum.None;
            this.Velocity = Vector3.Zero;
        }

        public PhysicalObject(GameObject gameObj, float mass, float dragFactor, bool isUsingGravity) : base(gameObj)
        {
            this.Mass = mass;
            this.DragFactor = dragFactor;
            this.IsUsingGravity = isUsingGravity;
            this.Sleeping = false;
            this.RotationConstraints = RotationConstraintsEnum.None;
            this.PositionConstraints = PositionConstraintsEnum.None;
            this.Velocity = Vector3.Zero;
        }

        protected override void Start()
        {

        }

        /// <summary>
        /// 
        /// Makes physical object not sleeping so it can be affected by external forces or gravity
        /// </summary>
        public void Awake()
        {
            this.Sleeping = false;
        }

        /// <summary>
        /// 
        /// Makes physical object sleeping so it can not be affected by anything related to physics
        /// </summary>
        public void Sleep()
        {
            this.Sleeping = true;
            this.Velocity = Vector3.Zero;
        }

        /// <summary>
        /// 
        /// Adds passed force to physical object so it can jump for example
        /// </summary>
        public void AddForce(Vector3 force)
        {
            this.acceleration += force / this.Mass;
        }

        public override void Update(GameTime gameTime)
        {
            //TODO:
            //Add constraints
            //Improve velocity changing function (maybe oblique throw equation ?)
            //Find a better way to slowing down (decreasing acceleration)

            //If is not sleeping
            if(!this.Sleeping)
            {
                //Slow down (there's a angular drag or whatever)
                this.acceleration *= (1 - this.DragFactor);
                if (this.acceleration.Length() < 0.001f) this.acceleration = Vector3.Zero;

                //Accelerate
                this.Velocity += this.acceleration * (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

                //Add a gravity
                this.Velocity += PhysicsManager.Instance.Gravity * (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

                //Change game object position because of velocity
                if (this.myObject.MyTransform != null)
                {
                    this.myObject.MyTransform.Position += Velocity * (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Do nothing, we do not expect to draw something as abstract as physical object component
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
            writer.WriteElementString("Acceleration", acceleration.ToString());
            writer.WriteElementString("Mass", Mass.ToString());
            writer.WriteElementString("DragFactor", DragFactor.ToString());
            writer.WriteElementString("IsUsingGravity", IsUsingGravity.ToString());

            writer.WriteStartElement("Velocity");
            writer.WriteElementString("X", Velocity.X.ToString());
            writer.WriteElementString("Y", Velocity.Y.ToString());
            writer.WriteElementString("Z", Velocity.Z.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("RotationConstraints", RotationConstraints.ToString());
            writer.WriteElementString("PositionConstraints", PositionConstraints.ToString());
        }

        #endregion
    }
}
