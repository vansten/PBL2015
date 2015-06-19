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

        private int lastMinutesSpawn = 30;

        private bool spawnPossible = false;

        private PlayerTime playerTime;

        public EnemiesSpawner(GameObject go) : base(go)
        {
        }

        public EnemiesSpawner(GameObject go, EnemiesSpawner es) : base(go)
        {
            this.playerTimerID = es.playerTimerID;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (TrashSoupGame.Instance.EditorMode) return;
            
            if(this.playerTime.Hours == 22 && this.playerTime.Minutes == 0)
            {
                spawnPossible = true;
            }
            else if(this.playerTime.Hours == 3 && this.playerTime.Minutes == 0)
            {
                spawnPossible = false;
                SafehouseController.Instance.CanToMapSelection = true;
            }

            if(spawnPossible)
            {
                if(Math.Abs(lastMinutesSpawn - this.playerTime.Minutes) == 30)
                {
                    Spawn();
                }
            }


            if(InputManager.Instance.GetKeyboardButtonDown(Microsoft.Xna.Framework.Input.Keys.P))
            {
                SafehouseController.Instance.EnemiesLeft = 0;
            }
            if(InputManager.Instance.GetKeyboardButtonDown(Microsoft.Xna.Framework.Input.Keys.O))
            {
                SafehouseController.Instance.CanToMapSelection = true;
            }
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        protected override void Start()
        {
        }

        private void Spawn()
        {
            lastMinutesSpawn = this.playerTime.Minutes;
            int rand = SingleRandom.Instance.rnd.Next() % 4 + 1;
            while (--rand >= 0)
            {
                Debug.Log("Spawning enemy");
                SafehouseController.Instance.EnemiesLeft += 1;
            }
        }

        public override void Initialize()
        {
            this.playerTime = (PlayerTime)ResourceManager.Instance.CurrentScene.GetObject(this.playerTimerID).GetComponent<PlayerTime>();
            if (this.playerTime.Hours == 22 && this.playerTime.Minutes == 0)
            {
                spawnPossible = true;
                Spawn();
            }
            SafehouseController.Instance.CanToMapSelection = false;

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
