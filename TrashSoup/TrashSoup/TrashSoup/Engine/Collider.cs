﻿using Microsoft.Xna.Framework;
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
    public class Collider : ObjectComponent, IXmlSerializable
    {
        #region Variables

        protected Matrix worldMatrix;

        #endregion

        #region Properties

        public bool IsTrigger
        {
            get;
            protected set;
        }

        public Vector3 IntersectionVector
        {
            get;
            protected set;
        }

        #endregion

        #region Methods
        public Collider() 
        {
            
        }

        public Collider(GameObject go) : base(go)
        {
            worldMatrix = Matrix.Identity;
            this.IsTrigger = false;
            this.CreateCollider();
        }

        public Collider(GameObject go, bool isTrigger) : base(go)
        {
            worldMatrix = Matrix.Identity;
            this.IsTrigger = isTrigger;
            this.CreateCollider();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.worldMatrix = this.MyObject.MyTransform.GetWorldMatrix();
            this.UpdateCollider();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

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

        /// <summary>
        /// 
        /// Checks if collision was detected
        /// </summary>
        public virtual bool Intersects(PhysicalObject po)
        {
            return false;
        }

        /// <summary>
        /// 
        /// Updates position of collider
        /// </summary>
        protected virtual void UpdateCollider()
        {

        }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            //worldMatrix
            worldMatrix = this.MyObject.MyTransform.GetWorldMatrix();
            reader.MoveToContent();
            reader.ReadStartElement();

            IsTrigger = reader.ReadElementContentAsBoolean("IsTrigger", "");

            reader.ReadEndElement();
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("IsTrigger", XmlConvert.ToString(IsTrigger));
        }

        #endregion
    }
}
