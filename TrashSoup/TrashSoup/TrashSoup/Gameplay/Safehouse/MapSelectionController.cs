using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Safehouse
{
    class MapSelectionController : ObjectComponent
    {
        private Texture2D background;
        private Texture2D activeMap;
        private Texture2D notActiveMaps;
        private Texture2D interactionTexture;

        private Vector2 backgroundPos;
        private Vector2 activeMapPos;
        private Vector2[] notActiveMapsPos;
        private Vector2 interactionPos;

        private float ar = 9.0f / 16.0f;

        public MapSelectionController(GameObject go) : base(go)
        {

        }

        public MapSelectionController(GameObject go, MapSelectionController msc) : base(go)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GUIManager.Instance.DrawTexture(this.background, this.backgroundPos, 1.0f, this.ar);

            GUIManager.Instance.DrawTexture(this.activeMap, this.activeMapPos, 0.1f, 0.1f);
            for(int i = 0; i < 3; ++i)
            {
                GUIManager.Instance.DrawTexture(this.notActiveMaps, this.notActiveMapsPos[i], 0.1f, 0.1f);
            }

            GUIManager.Instance.DrawTexture(this.interactionTexture, this.interactionPos, 0.05f, 0.05f);

            if(InputHandler.Instance.Action())
            {
                SaveManager.Instance.XmlPath = "../../../../TrashSoupContent/Scenes/loading.xml";
                SaveManager.Instance.LoadFileAction();
            }
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        protected override void Start()
        {

        }

        public override void Initialize()
        {
            background = TrashSoupGame.Instance.Content.Load<Texture2D>(@"Textures/GUITest/MapSelectionBG");
            activeMap = TrashSoupGame.Instance.Content.Load<Texture2D>(@"Textures/GUITest/ActiveMap");
            notActiveMaps = TrashSoupGame.Instance.Content.Load<Texture2D>(@"Textures/GUITest/NotActiveMap");
            interactionTexture = TrashSoupGame.Instance.Content.Load<Texture2D>(@"Textures/HUD/x_button");

            backgroundPos = new Vector2(0.0f, 0.0f);
            activeMapPos = new Vector2(0.5f, 0.6f);
            notActiveMapsPos = new Vector2[3];
            notActiveMapsPos[0] = new Vector2(0.3f, 0.4f);
            notActiveMapsPos[1] = new Vector2(0.7f, 0.4f);
            notActiveMapsPos[2] = new Vector2(0.5f, 0.2f);
            interactionPos = new Vector2(0.45f, 0.9f);

            base.Initialize();
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            base.ReadXml(reader);

            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }
    }
}
