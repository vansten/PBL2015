﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class Food : ObjectComponent
    {
        private GameObject myTrigger;

        public Food(GameObject go) : base(go)
        {
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        protected override void Start()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnTriggerEnter(GameObject other)
        {
            if (other.UniqueID == 1)
            {
                PlayerController pc = (PlayerController)other.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.FoodSaw = true;
                    pc.Food = this;
                }
            }
            base.OnTriggerEnter(other);
        }

        public override void OnTriggerExit(GameObject other)
        {
            if (other.UniqueID == 1)
            {
                PlayerController pc = (PlayerController)other.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.FoodSaw = false;
                    pc.Food = null;
                }
            }
            base.OnTriggerExit(other);
        }

        public override System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            base.ReadXml(reader);
            //MyObject = ResourceManager.Instance.CurrentScene.GetObject(tmp);

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
        }
    }
}