using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using TrashSoup.Engine;
using TrashSoup.Gameplay;

namespace TrashSoup.Gameplay.Safehouse
{
    class EnemiesSpawner : ObjectComponent
    {
        private uint playerTimerID = 355;
        private int enemiesLeftCount = 10;

        private PlayerTime playerTime;

        public EnemiesSpawner(GameObject go) : base(go)
        {
            enemiesLeftCount = 10;
        }

        public EnemiesSpawner(GameObject go, EnemiesSpawner es) : base(go)
        {
            this.playerTimerID = es.playerTimerID;
            enemiesLeftCount = es.enemiesLeftCount;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (TrashSoupGame.Instance.EditorMode) return;
            if(this.playerTime.Hours == 22 && this.playerTime.Minutes == 0)
            {
                while(--this.enemiesLeftCount >= 0)
                {
                    Debug.Log("Selecting random spawn point to spawn enemy in that spawn point");
                }
                //this.MyObject.Enabled = false;
            }

            if(InputManager.Instance.GetKeyboardButtonDown(Microsoft.Xna.Framework.Input.Keys.P))
            {
                SafehouseController.Instance.EnemiesLeft = 0;
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
            this.playerTime = (PlayerTime)ResourceManager.Instance.CurrentScene.GetObject(this.playerTimerID).GetComponent<PlayerTime>();
            SafehouseController.Instance.EnemiesLeft = this.enemiesLeftCount;

            base.Initialize();
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
