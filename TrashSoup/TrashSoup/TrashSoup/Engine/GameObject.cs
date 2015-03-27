using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace TrashSoup.Engine
{
    public class GameObject : IXmlSerializable
    {
        #region variables

        private PhysicalObject myPhisicalObject;

        #endregion

        #region properties
        public uint UniqueID { get; protected set; }
        public string Name { get; protected set; }
        public List<string> Tags { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        public Transform MyTransform { get; set; }
        public Animator MyAnimator { get; set; }
        public Collider MyCollider { get; set; }

        public PhysicalObject MyPhysicalObject 
        {
            get
            {
                return this.myPhisicalObject;
            }
            set
            {
                if(value == null)
                {
                    PhysicsManager.Instance.RemovePhysicalObject(this);
                }
                else
                {
                    PhysicsManager.Instance.AddPhysicalObject(this);
                }

                this.myPhisicalObject = value;
            }
        }

        public List<ObjectComponent> Components { get; set; }
        public GraphicsDeviceManager GraphicsManager { get; protected set; }

        #endregion

        #region methods
        public GameObject(uint uniqueID, string name)
        {
            this.UniqueID = uniqueID;
            this.Name = name;

            this.Components = new List<ObjectComponent>();
            this.GraphicsManager = TrashSoupGame.Instance.GraphicsManager;

            this.Enabled = true;
            this.Visible = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            if(this.Enabled)
            {
                if(this.MyPhysicalObject != null)
                {
                    this.MyPhysicalObject.Update(gameTime);
                }

                if(this.MyCollider != null)
                {
                    this.MyCollider.Update(gameTime);
                }

                if (this.MyAnimator != null)
                {
                    this.MyAnimator.Update(gameTime);
                }

                foreach (ObjectComponent obj in Components)
                {
                    obj.Update(gameTime);
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if(this.Visible)
            {
                //[vansten] This is for debug drawing of collider
                //[vansten] It won't be build if we build a release version
#if DEBUG
                if (this.MyCollider != null)
                {
                    this.MyCollider.Draw(gameTime);
                }

#endif

                foreach (ObjectComponent obj in Components)
                {
                    obj.Draw(gameTime);
                }
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            UniqueID = (uint)reader.ReadElementContentAsInt("UniqueID", "");
            Name = reader.ReadElementString("Name", "");

            if(reader.Name == "MyTransform")
            {
                (MyTransform as IXmlSerializable).ReadXml(reader);
            }

            if(reader.Name == "MyPhysicalObject")
            {
                (MyPhysicalObject as IXmlSerializable).ReadXml(reader);
            }

            //komponenty

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("UniqueID", UniqueID.ToString());
            writer.WriteElementString("Name", Name);

            if(MyTransform != null)
            {
                writer.WriteStartElement("MyTransform");
                (MyTransform as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }

            if(MyPhysicalObject != null)
            {
                writer.WriteStartElement("MyPhysicalObject");
                (MyPhysicalObject as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }

            if(Components.Count != 0)
            {
                writer.WriteStartElement("Components");
                foreach (ObjectComponent comp in Components)
                {
                    if(comp != null)
                    {
                        if(comp is CustomModel)
                        {
                            writer.WriteStartElement("CustomModel");
                        }
                        else if(comp is Gameplay.PlayerController)
                        {
                            writer.WriteStartElement("PlayerController");
                        }
                        else if(comp is PhysicalObject)
                        {
                            writer.WriteStartElement("PhysicalObject");
                        }
                        else if(comp is Gameplay.CameraBehaviourComponent)
                        {
                            writer.WriteStartElement("CameraBehaviourComponent");
                        }
                        else if(comp is Collider)
                        {
                            writer.WriteStartElement("Collider");
                        }
                        else if(comp is Transform)
                        {
                            writer.WriteStartElement("Transform");
                        }
                        (comp as IXmlSerializable).WriteXml(writer);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

        }

        /// <summary>
        /// 
        /// This function will call every OnTrigger(GameObject) in this game object components
        /// </summary>
        public void OnTrigger(GameObject otherGO)
        {
            foreach(ObjectComponent oc in this.Components)
            {
                oc.OnTrigger(otherGO);
            }
        }

        /// <summary>
        /// 
        /// This function will call every OnCollision(GameObject) in this game object components
        /// </summary>
        public void OnCollision(GameObject otherGO)
        {
            foreach (ObjectComponent oc in this.Components)
            {
                oc.OnCollision(otherGO);
            }
        }

        #endregion

    }
}
