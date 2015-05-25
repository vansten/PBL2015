using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class MenuController : ObjectComponent
    {
        public MenuController(GameObject go) : base(go)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(InputManager.Instance.GetKeyboardButtonDown(Microsoft.Xna.Framework.Input.Keys.N))
            {
                SaveManager.Instance.XmlPath = "../../../../TrashSoupContent/Scenes/save2.xml";
                SaveManager.Instance.LoadFileAction();
            }
        }

        public override void Draw(Engine.Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        protected override void Start()
        {

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

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
        }
    }
}
