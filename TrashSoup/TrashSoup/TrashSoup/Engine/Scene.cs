using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    public class SceneParams : IXmlSerializable
    {
        #region properties
        public uint UniqueID { get; set; }
        public string Name { get; set; }
        #endregion

        #region methods
        public SceneParams(uint uniqueID, string name)
        {
            this.UniqueID = uniqueID;
            this.Name = name;
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            this.UniqueID = (uint)reader.ReadElementContentAsInt("UniqueID", "");
            this.Name = reader.ReadElementString("Name", "");

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("UniqueID", UniqueID.ToString());
            writer.WriteElementString("Name", Name);
        }
        #endregion
    }

    public class Scene : IXmlSerializable
    {
        #region properties

        public SceneParams Params { get; set; }
        public Camera Cam { get; set; }

        public LightAmbient AmbientLight { get; set; }
        public LightDirectional[] DirectionalLights { get; set; }
        public Dictionary<uint, GameObject> ObjectsDictionary { get; protected set; }
        public QuadTree<GameObject> ObjectsQT { get; protected set; }
        // place for bounding sphere tree

        #endregion

        #region methods
        public Scene()
        {
        }
        public Scene(SceneParams par)
        {
            this.Params = par;

            DirectionalLights = new LightDirectional[ResourceManager.DIRECTIONAL_MAX_LIGHTS];
            for (int i = 0; i < ResourceManager.DIRECTIONAL_MAX_LIGHTS; ++i)
                DirectionalLights[i] = null;

            ObjectsDictionary = new Dictionary<uint, GameObject>();
            ObjectsQT = new QuadTree<GameObject>();
        }

        public void AddObject(GameObject obj)
        {

        }

        public bool DeleteObject(uint uniqueID)
        {
            return false;
        }

        public GameObject GetObject(uint uniqueID)
        {
            return ObjectsDictionary[uniqueID];
        }

        public List<GameObject> GetObjectsOfType(Type type)
        {
            return null;
        }

        public List<ObjectComponent> GetComponentsOfType(Type type)
        {
            return null;
        }

        public List<GameObject> GetObjectsWithinFrustum(BoundingFrustum frustum)
        {
            return null;
        }

        public List<GameObject> GetObjectsWhichCollide(BoundingSphere bSphere)
        {
            return null;
        }

        public void UpdateAll(GameTime gameTime)
        {
            //[vansten] Added testing code for physics simulation
            Cam.Update(gameTime);
            foreach (GameObject obj in ObjectsDictionary.Values)
            {
                obj.Update(gameTime);
            }
        }

        // draws all gameobjects linearly
        public void DrawAll(GameTime gameTime)
        {
            foreach (GameObject obj in ObjectsDictionary.Values)
            {
                obj.Draw(gameTime);
            }
        }

        // draws gameobjects that are inside frustum
        public void DrawAll(BoundingFrustum frustum, GameTime gameTime)
        {
            // not implemented yet
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            ResourceManager.Instance.CurrentScene.ObjectsDictionary = new Dictionary<uint, GameObject>();
            DirectionalLights = new LightDirectional[ResourceManager.DIRECTIONAL_MAX_LIGHTS];

            if(reader.Name == "SceneParams")
            {
                Params = new SceneParams(0, "null");
                (Params as IXmlSerializable).ReadXml(reader);
            }

            if (reader.Name == "AmbientLight")
            {
                AmbientLight = new LightAmbient(0, "null");
                (AmbientLight as IXmlSerializable).ReadXml(reader);
            }

            int ctr = 0;
            reader.ReadStartElement("DirectionalLights");
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (reader.Name == "DirectionalLight")
                {
                    reader.ReadStartElement();
                    if(reader.Name != "null")
                    {
                        LightDirectional obj = new LightDirectional(0, "");
                        (obj as IXmlSerializable).ReadXml(reader);
                        DirectionalLights[ctr] = obj;
                        ++ctr;
                    }
                    else
                    {
                        reader.ReadElementString("null", "");
                        reader.ReadEndElement();
                        ++ctr;
                    }
                }
            }
            reader.ReadEndElement();

            reader.ReadStartElement("ObjectsDictionary");
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (reader.Name == "GameObject")
                {
                    reader.ReadStartElement();
                    GameObject obj = new GameObject(0, "");
                    uint key = (uint)reader.ReadElementContentAsInt("GameObjectKey", "");
                    ResourceManager.Instance.CurrentScene.ObjectsDictionary.Add(key, obj);
                    (obj as IXmlSerializable).ReadXml(reader);
                }
                reader.ReadEndElement();
            }
            reader.ReadEndElement();

            if (reader.Name == "Camera")
            {
                Cam = new Camera(0, "null", Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, MathHelper.Pi / 3.0f, 0.1f, 2000.0f);
                reader.ReadStartElement();
                (Cam as IXmlSerializable).ReadXml(reader);
            }

            ObjectsDictionary = ResourceManager.Instance.CurrentScene.ObjectsDictionary;

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("SceneParams");
            (Params as IXmlSerializable).WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("AmbientLight");
            (AmbientLight as IXmlSerializable).WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("DirectionalLights");
            for (int i = 0; i < ResourceManager.DIRECTIONAL_MAX_LIGHTS; ++i )
            {
                writer.WriteStartElement("DirectionalLight");
                if (DirectionalLights[i] != null)
                    (DirectionalLights[i] as IXmlSerializable).WriteXml(writer);
                else
                    writer.WriteElementString("null","");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ObjectsDictionary");
            for (int i = 0; i < ObjectsDictionary.Count; ++i )
            {
                writer.WriteStartElement("GameObject");
                writer.WriteElementString("GameObjectKey", ObjectsDictionary.Keys.ElementAt(i).ToString());
                (ObjectsDictionary.Values.ElementAt(i) as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Camera");
            (Cam as IXmlSerializable).WriteXml(writer);
            writer.WriteEndElement();

        }
        #endregion
    }
}
