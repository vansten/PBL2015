﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class Journal : ObjectComponent, IXmlSerializable
    {
        private Texture2D interactionTexture;
        private Vector2 interactionTexturePos = new Vector2(0.475f, 0.775f);

        private Texture2D noteTexture;
        private Vector2 noteTexturePos = new Vector2(0.25f, 0.0f);

        private PlayerController player;
        private bool collisionWithPlayer = false;
        private bool journalGathered = false;

        public Journal(GameObject go) : base(go)
        {
            
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (this.collisionWithPlayer)
            {
                GUIManager.Instance.DrawTexture(this.interactionTexture, this.interactionTexturePos, 0.05f, 0.05f);
                if (InputHandler.Instance.Action())
                {
                    AudioManager.Instance.SoundBank.PlayCue("commentary");
                    journalGathered = true;
                    this.collisionWithPlayer = false;
                }
            }
            if (journalGathered)
            {
                GameObject go = new GameObject(123123123, "JournalView");
                go.MyTransform = new Transform(go);
                JournalView jv = new JournalView(go);
                go.Components.Add(jv);
                ResourceManager.Instance.CurrentScene.AddObjectRuntime(go);
                this.MyObject.Enabled = false;
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
            this.interactionTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/x_button");
            this.noteTexture = ResourceManager.Instance.LoadTexture(@"Textures/Storyline/note");

            base.Initialize();
        }

        public override void OnTriggerEnter(GameObject other)
        {
            if (other.UniqueID == 1)
            {
                player = (PlayerController) other.GetComponent<PlayerController>();
                this.collisionWithPlayer = true;
            }
            base.OnTriggerEnter(other);
        }

        public override void OnTriggerExit(GameObject other)
        {
            if (other.UniqueID == 1)
            {
                this.collisionWithPlayer = false;
            }
            base.OnTriggerExit(other);
        }

        public override XmlSchema GetSchema()
        {
            return base.GetSchema();
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
