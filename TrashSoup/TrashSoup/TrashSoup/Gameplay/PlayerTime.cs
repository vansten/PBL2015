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
    public class PlayerTime : ObjectComponent, IXmlSerializable
    {
        #region variables
        private int initHours;
        private int initMinutes;
        private int hours;
        private int minutes;
        private bool isTime;
        #endregion

        #region properties
        public int Hours
        {
            get { return hours; }
            set { hours = value; }
        }

        public int Minutes
        {
            get { return minutes; }
            set { minutes = value; }
        }
        #endregion

        #region methods
        public PlayerTime(GameObject obj) : base(obj)
        {
            this.initHours = 12;
            this.initMinutes = 0;
            Hours = initHours;
            Minutes = initMinutes;
        }

        public PlayerTime(GameObject obj, int initHours, int initMinutes) : base(obj)
        {
            this.initHours = initHours;
            this.initMinutes = initMinutes;
            Hours = initHours;
            Minutes = initMinutes;
        }

        public override void Update(GameTime gameTime)
        {
            Minutes = (initMinutes + gameTime.TotalGameTime.Seconds) % 60;
            Hours = initHours;
            if (Minutes == 0)
            {
                if (isTime)
                {
                    initHours += 1;
                    isTime = false;
                }
            }
            else
                isTime = true;
        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/digital-7"), "TIME: " + Hours.ToString("00") + ":" + Minutes.ToString("00"), new Vector2(0.1f, 0.4f), Color.Red);
        }

        protected override void Start()
        {
            // do nothing
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

        #endregion
    }
}
