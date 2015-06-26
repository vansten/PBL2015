using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Safehouse
{
    public class SpawnPoint : ObjectComponent
    {
        public List<uint> EnemiesToSpawn = new List<uint>();
        private List<GameObject> enemiesToSpawnObjects = new List<GameObject>();

        private int availableEnemies = 0;

        GameObject player;

        public SpawnPoint(GameObject go) : base(go)
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

        public void SpawnEnemies(int howMuch)
        {
            if(availableEnemies <= 0)
            {
                return;
            }

            player.MyPhysicalObject.IsUsingGravity = false;
            if(howMuch >= availableEnemies)
            {
                //Spawn all enemies that weren't spawned
                foreach(GameObject go in enemiesToSpawnObjects)
                {
                    Debug.Log("Hello21231");
                    if(!go.Enabled)
                    {
                        go.Enabled = true;
                        --availableEnemies;
                    }
                }
                Debug.Log("All enemies spawned");
            }
            else
            {
                int randomNumber = 0;
                for (int i = 0; i < howMuch; ++i)
                {
                    bool flag = false;
                    while (!flag)
                    {
                        randomNumber = SingleRandom.Instance.rnd.Next(0, EnemiesToSpawn.Count);
                        if (!enemiesToSpawnObjects[i].Enabled)
                        {
                            flag = true;
                            enemiesToSpawnObjects[i].Enabled = true;
                            --availableEnemies;
                        }
                    }
                    Debug.Log("Enemy SPAWNED!!!");
                }
            }
        }

        public override void Initialize()
        {
            foreach(uint id in EnemiesToSpawn)
            {
                GameObject go = ResourceManager.Instance.CurrentScene.GetObject(id);
                go.Enabled = false;
                go.Dynamic = true;
                go.MyTransform.Version = Transform.GameVersionEnum.STENGERT_PAGI;
                go.MyTransform.Position = this.MyObject.MyTransform.Position + 0.2f * Vector3.Up;
                go.MyTransform.Version = Transform.GameVersionEnum.PBL;
                enemiesToSpawnObjects.Add(go);
            }
            availableEnemies = enemiesToSpawnObjects.Count;

            player = ResourceManager.Instance.CurrentScene.GetObject(1);
            base.Initialize();
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            reader.ReadStartElement();

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                uint id = (uint)reader.ReadElementContentAsInt("Enemy", "");
                this.EnemiesToSpawn.Add(id);
            }

            reader.ReadEndElement();

            base.ReadXml(reader);

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("EnemiesToSpawn");
            foreach(uint id in EnemiesToSpawn)
            {
                writer.WriteStartElement("Enemy");
                writer.WriteValue(id);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            base.WriteXml(writer);
        }
    }
}
