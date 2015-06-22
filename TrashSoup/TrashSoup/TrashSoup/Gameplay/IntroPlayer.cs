using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Gameplay
{
    class IntroPlayer : ObjectComponent
    {
        Video introVideo;
        VideoPlayer videoPlayer;
        Texture2D videoTexture;

        public IntroPlayer(GameObject go) : base(go)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(videoPlayer.State == MediaState.Stopped)
            {
                SaveManager.Instance.XmlPath = "../../../../TrashSoupContent/Scenes/loading.xml";
                SaveManager.Instance.LoadFileAction();
            }
            else
            {
                videoTexture = videoPlayer.GetTexture();
                GUIManager.Instance.DrawTexture(videoTexture, Vector2.Zero, 1.0f, 9.0f / 16.0f);
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
            introVideo = TrashSoupGame.Instance.Content.Load<Video>("Videos/test");
            videoPlayer = new VideoPlayer();
            videoPlayer.Play(introVideo);
            base.Initialize();
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
