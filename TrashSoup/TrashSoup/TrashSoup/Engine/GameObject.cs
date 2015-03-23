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
                        (comp as IXmlSerializable).WriteXml(writer);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

        }
        #endregion

    }
}
