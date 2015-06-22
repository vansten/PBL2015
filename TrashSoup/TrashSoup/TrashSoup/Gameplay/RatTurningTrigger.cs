﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class RatTurningTrigger : ObjectComponent
    {
        private Rat myRat;

        public RatTurningTrigger(GameObject go)
            : base(go)
        {

        }

        public RatTurningTrigger(GameObject go, RatTurningTrigger rtt) : base(go, rtt)
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
            this.myRat = (Rat)ResourceManager.Instance.CurrentScene.GetObject(359).GetComponent<Rat>();
            this.myRat.MyObject.AddChild(this.MyObject);
            base.Initialize();
        }

        public override void OnTrigger(GameObject other)
        {
            if(other.Tags.Contains("NoRatAllowed"))
            {
                this.myRat.MyBlackBoard.SetBool("ShouldTurn", true);
            }
            base.OnTrigger(other);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
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
