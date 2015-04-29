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
        public string Name { get; set; }
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

        public virtual void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            if(this.Visible && this.Enabled)
            {
                foreach (ObjectComponent obj in Components)
                {
                    obj.Draw(cam, effect, gameTime);
                }

                //[vansten] This is for debug drawing of collider
                //[vansten] It won't be build if we build a release version
                //[vansten] COMMENT DRAWING COLLIDER TO GET HIGHER FPS RATE!!!!!!!!!!!!!
#if DEBUG
                if (this.MyCollider != null)
                {
                    this.MyCollider.Draw(cam, effect, gameTime);
                }

#endif
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            //reader.ReadStartElement();

            UniqueID = (uint)reader.ReadElementContentAsInt("UniqueID", "");
            Name = reader.ReadElementString("Name", "");

            if(reader.Name == "MyTransform")
            {
                MyTransform = new Transform(this);
                (MyTransform as IXmlSerializable).ReadXml(reader);
            }

            if(reader.Name == "MyPhysicalObject")
            {
                MyPhysicalObject = new PhysicalObject(this);
                (MyPhysicalObject as IXmlSerializable).ReadXml(reader);
            }

            //if (reader.Name == "MyCollider")
            //{
            //    reader.ReadStartElement();
            //    String s = reader.ReadElementString("Type", "");
            //    switch(s)
            //    {
            //        case "TrashSoup.Engine.BoxCollider":
            //            MyCollider = new BoxCollider(this);
            //            break;
            //        case "TrashSoup.Engine.SphereCollider":
            //            MyCollider = new SphereCollider(this);
            //            break;
            //        default:
            //            MyCollider = new Collider(this);
            //            break;
            //    }
            //    (MyCollider as IXmlSerializable).ReadXml(reader);
            //    reader.ReadEndElement();
            //}

            if(reader.Name == "MyAnimator")
            {
                reader.ReadStartElement();
                Model baseAnim = null;
                string baseAnimPath = reader.ReadElementString("BaseAnim", "");
                if(!ResourceManager.Instance.Models.TryGetValue(baseAnimPath, out baseAnim))
                {
                    baseAnim = ResourceManager.Instance.LoadModel(baseAnimPath);
                }
                MyAnimator = new Animator(this, ResourceManager.Instance.Models[baseAnimPath]);
                (MyAnimator as IXmlSerializable).ReadXml(reader);
                //MyAnimator.MyObject = ResourceManager.Instance.CurrentScene.GetObject(MyAnimator.tmp);
                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    String s = reader.ReadElementString("AnimatorClip", "");
                    MyAnimator.animationPlayers.Add(s, new SkinningModelLibrary.AnimationPlayer(MyAnimator.SkinningData, s));
                }
                reader.ReadEndElement();
            }

            if(reader.Name == "Components")
            {
                List<object> parameters = new List<object>();
                parameters.Add(this);
                reader.MoveToContent();
                reader.ReadStartElement();
                while(reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    Object obj = Activator.CreateInstance(Type.GetType(reader.Name), parameters.ToArray());
                    (obj as IXmlSerializable).ReadXml(reader);
                    Components.Add((ObjectComponent)obj);
                }

            }

            reader.ReadEndElement();

            if (reader.Name == "MyCollider")
            {
                reader.ReadStartElement();
                String s = reader.ReadElementString("Type", "");
                switch (s)
                {
                    //commented because Collider system will be changed
                    case "TrashSoup.Engine.BoxCollider":
                        MyCollider = new BoxCollider(this);
                        break;
                    default:
                        MyCollider = new Collider(this);
                        break;
                }
                (MyCollider as IXmlSerializable).ReadXml(reader);
                reader.ReadEndElement();
            }

            //reader.ReadEndElement();
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

            if(MyAnimator != null)
            {
                writer.WriteStartElement("MyAnimator");
                writer.WriteElementString("BaseAnim", ResourceManager.Instance.Models.FirstOrDefault(x => x.Value == MyAnimator.BaseAnim).Key);
                (MyAnimator as IXmlSerializable).WriteXml(writer);
                foreach (KeyValuePair<string, SkinningModelLibrary.AnimationPlayer> pair in MyAnimator.animationPlayers)
                {
                    writer.WriteElementString("AnimatorClip", pair.Key);
                }
                writer.WriteEndElement();
            }

            if(Components.Count != 0)
            {
                writer.WriteStartElement("Components");
                foreach (ObjectComponent comp in Components)
                {
                    if(comp != null)
                    {
                        writer.WriteStartElement(comp.GetType().ToString());
                        (comp as IXmlSerializable).WriteXml(writer);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

            if (MyCollider != null)
            {
                writer.WriteStartElement("MyCollider");
                writer.WriteElementString("Type", MyCollider.GetType().ToString());
                (MyCollider as IXmlSerializable).WriteXml(writer);
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
            if(this.MyPhysicalObject != null)
            {
                this.MyPhysicalObject.OnCollision(otherGO);
            }

            foreach (ObjectComponent oc in this.Components)
            {
                oc.OnCollision(otherGO);
            }
        }

        public ObjectComponent GetComponent<T>()
        {
            System.Type t = typeof(T);

            if(t == typeof(Transform) && this.MyTransform != null)
            {
                return this.MyTransform;
            }

            if(t == typeof(Animator) && this.MyAnimator != null)
            {
                return this.MyAnimator;
            }

            if(t == typeof(Collider) && this.MyCollider != null)
            {
                return this.MyCollider;
            }

            if(t == typeof(PhysicalObject) && this.MyPhysicalObject != null)
            {
                return this.MyPhysicalObject;
            }

            foreach(ObjectComponent oc in this.Components)
            {
                if(oc.GetType() == t)
                {
                    return oc;
                }
            }

            return null;
        }

        public List<ObjectComponent> GetComponents<T>()
        {
            List<ObjectComponent> componentsList = new List<ObjectComponent>();

            System.Type t = typeof(T);

            if (t == typeof(Transform) && this.MyTransform != null)
            {
                componentsList.Add(this.MyTransform);
            }

            if (t == typeof(Animator) && this.MyAnimator != null)
            {
                componentsList.Add(this.MyAnimator);
            }

            if (t == typeof(Collider) && this.MyCollider != null)
            {
                componentsList.Add(this.MyCollider);
            }

            if (t == typeof(PhysicalObject) && this.MyPhysicalObject != null)
            {
                componentsList.Add(this.MyPhysicalObject);
            }

            foreach (ObjectComponent oc in this.Components)
            {
                if (oc.GetType() == t)
                {
                    componentsList.Add(oc);
                }
            }

            return componentsList;
        }

        #endregion

    }
}
