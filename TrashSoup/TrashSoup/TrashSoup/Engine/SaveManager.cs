using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrashSoup.Engine
{
    [XmlRoot("Save")]
    class SaveManager : Singleton<SaveManager>, IXmlSerializable
    {
        #region variables
        public Scene scene;
        public String XmlPath { get; set; }
        #endregion

        #region methods
        public SaveManager()
        {
            this.scene = ResourceManager.Instance.CurrentScene;
            //FOR TETIN
            this.XmlPath = "save.xml";
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
            writer.WriteStartElement("Scene");
            (scene as IXmlSerializable).WriteXml(writer);
            writer.WriteEndElement();
        }

        public void SaveFileAction()
        {
            if(scene != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Scene));
                using (FileStream file = new FileStream(this.XmlPath, FileMode.Create))
                {
                    serializer.Serialize(file, this.scene);
                }
            }
        }
        #endregion
    }
}
