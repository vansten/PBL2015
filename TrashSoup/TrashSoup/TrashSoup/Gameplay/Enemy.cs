using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class Enemy : ObjectComponent, IXmlSerializable
    {
        #region variables
        protected int hitPoints;
        protected bool isDead;

        public Action OnDead;
        #endregion

        #region properties
        public int HitPoints
        {
            get { return hitPoints; }
            set { hitPoints = value; }
        }

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }
        #endregion

        #region methods
        public Enemy(GameObject obj) : base(obj)
        {
            this.HitPoints = 20;
        }

        public Enemy(GameObject obj, int hitPoints):base(obj)
        {
            this.HitPoints = hitPoints;
        }
        #endregion

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (this.HitPoints <= 0)
                IsDead = true;
            if (IsDead)
            {
                if(OnDead != null)
                {
                    OnDead();
                }
                this.MyObject.Enabled = false;
                return;
            }

#if DEBUG
            GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"),
                "RAT: " + this.HitPoints, new Vector2(0.8f, 0.8f), Color.Red);
#endif
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        protected override void Start()
        {
            IsDead = false;
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
