using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TrashSoup.Engine;
using TrashSoup.Engine.AI.BehaviorTree;

namespace TrashSoup.Gameplay
{
    class Rat : ObjectComponent
    {
        private BehaviorTree myBehavior;
        public Blackboard MyBlackBoard;

        public Rat(GameObject go) : base(go)
        {
            Start();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(TrashSoupGame.Instance.EditorMode)
            {
                return;
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
            XmlSerializer serializer = new XmlSerializer(typeof(BehaviorTree));
            string path = "";
            if (TrashSoupGame.Instance != null && TrashSoupGame.Instance.EditorMode)
            {
                path += "../";
            }
            path += "../../../../TrashSoupContent/BehaviorTrees/RatAI.xml";
            try
            {
                using (FileStream file = new FileStream(Path.GetFullPath(path), FileMode.Open))
                {
                    myBehavior = (BehaviorTree)serializer.Deserialize(file);
                    MyBlackBoard = myBehavior.Blackboard;
                }
                myBehavior.Run();
            }
            catch
            {

            }
            base.Initialize();
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
