using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TrashSoup.Engine
{
    /// <summary>
    /// 
    /// Base class for every colliders we will have
    /// </summary>
    public abstract class Collider : ObjectComponent, IXmlSerializable
    {
        #region Variables

        protected Matrix worldMatrix;
        public BoundingSphere MyBoundingSphere;

        #endregion

        #region Properties

        public bool IsTrigger
        {
            get;
            set;
        }

        public Vector3 IntersectionVector
        {
            get;
            set;
        }

        public bool IsCollision
        {
            get;
            protected set;
        }

        public List<Collider> IgnoredColliders;

        public List<Collider> TriggerReasons
        {
            get;
            set;
        }

        #endregion

        #region Methods
        public Collider() 
        {
            
        }

        public Collider(GameObject go) : base(go)
        {
            worldMatrix = Matrix.Identity;
            this.TriggerReasons = new List<Collider>();
            this.IgnoredColliders = new List<Collider>();
            this.CreateCollider();
        }

        public Collider(GameObject go, bool isTrigger) : base(go)
        {
            worldMatrix = Matrix.Identity;
            this.IsTrigger = isTrigger;
            this.TriggerReasons = new List<Collider>();
            this.IgnoredColliders = new List<Collider>();
            this.CreateCollider();
        }

        public Collider(GameObject go, Collider c) : base(go)
        {
            this.worldMatrix = c.worldMatrix;
            this.IsTrigger = c.IsTrigger;
            this.TriggerReasons = new List<Collider>();
            this.IgnoredColliders = new List<Collider>();
            this.CreateCollider();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(this.Enabled)
            {
                this.UpdateCollider();
            }
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!this.Enabled)
            {
                return;
            }
        }

        protected override void Start()
        {

        }

        /// <summary>
        /// 
        /// Creates collider and add this collider to PhysicsManager so PM can manage collisions
        /// </summary>
        protected virtual void CreateCollider()
        {
            PhysicsManager.Instance.AddCollider(this);
        }

        public virtual bool Intersects(Collider col)
        {
            return false;
        }

        /// <summary>
        /// 
        /// Updates position of collider
        /// </summary>
        public virtual void UpdateCollider()
        {

        }

        protected abstract Vector3 GetFarthestPointInDirection(Vector3 direction);

        protected Vector3 SupportFunction(Collider a, Collider b, Vector3 direction)
        {
            Vector3 p1 = a.GetFarthestPointInDirection(direction);
            Vector3 p2 = b.GetFarthestPointInDirection(-direction);

            return (p1 - p2);
        }

        public override System.Xml.Schema.XmlSchema GetSchema() { return null; }
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            //reader.MoveToContent();
            //reader.ReadStartElement();

            base.ReadXml(reader);
            ////worldMatrix
            worldMatrix = this.MyObject.MyTransform.GetWorldMatrix();

            IsTrigger = reader.ReadElementContentAsBoolean("IsTrigger", "");
            //MyObject = ResourceManager.Instance.CurrentScene.GetObject(tmp);

            //reader.ReadEndElement();
        }
        
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("IsTrigger", XmlConvert.ToString(IsTrigger));
        }
        #endregion
    }
}
