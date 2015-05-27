using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class RatTrigger : ObjectComponent
    {
        private Rat myRat;
        private bool targetSeen = false;
        private GameObject target;

        public RatTrigger(GameObject go) : base(go)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(targetSeen && target != null)
            {
                myRat.MyBlackBoard.SetVector3("TargetPosition", target.MyTransform.Position);
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
            this.myRat = (Rat)ResourceManager.Instance.CurrentScene.GetObject(359).GetComponent<Rat>();
            this.myRat.MyObject.AddChild(this.MyObject);
            base.Initialize();
        }

        public override void OnTriggerEnter(GameObject other)
        {
            if(other.UniqueID == 1)
            {
                myRat.MyBlackBoard.SetBool("TargetSeen", true);
                target = other;
                myRat.MyBlackBoard.SetVector3("TargetPosition", other.MyTransform.Position);
            }
            base.OnTriggerEnter(other);
        }

        public override void OnTriggerExit(GameObject other)
        {
            if (other.UniqueID == 1)
            {
                myRat.MyBlackBoard.SetBool("TargetSeen", false);
                target = null;
                myRat.MyBlackBoard.SetVector3("TargetPosition", Vector3.Zero);
            }
            base.OnTriggerExit(other);
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
