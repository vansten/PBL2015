using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class MutantTrigger : ObjectComponent
    {
        private Mutant myMutant;
        private bool targetSeen = false;
        private bool targetDead = false;
        private GameObject target;
        private PlayerController pc;

        public uint MyMutantID;

        public MutantTrigger(GameObject go) : base(go)
        {

        }

        public MutantTrigger(GameObject go, MutantTrigger rt)
            : base(go, rt)
        {
            this.myMutant = rt.myMutant;
            this.MyMutantID = rt.MyMutantID;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (targetDead) return;
            if(targetSeen && target != null)
            {
                myMutant.MyBlackBoard.SetVector3("TargetPosition", target.MyTransform.Position);

                if(pc != null)
                {
                    if(pc.IsDead)
                    {
                        myMutant.MyBlackBoard.SetBool("TargetSeen", false);
                        targetSeen = false;
                        targetDead = true;
                    }
                }
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
            this.myMutant = (Mutant)ResourceManager.Instance.CurrentScene.GetObject(MyMutantID).GetComponent<Mutant>();
            this.myMutant.MyObject.AddChild(this.MyObject);
            this.MyObject.MyTransform.Position = new Vector3(5.0f, 0.0f, 0.0f);
            base.Initialize();
        }

        public override void OnTrigger(GameObject other)
        {
            if (targetDead) return;
            if(other.UniqueID == 1)
            {
                myMutant.MyBlackBoard.SetBool("TargetSeen", true);
                targetSeen = true;
                target = other;
                pc = (PlayerController)target.GetComponent<PlayerController>();
                myMutant.MyBlackBoard.SetVector3("TargetPosition", other.MyTransform.Position);
            }
            base.OnTrigger(other);
        }

        public override void OnTriggerExit(GameObject other)
        {
            if (targetDead) return;
            if (other.UniqueID == 1)
            {
                myMutant.MyBlackBoard.SetBool("TargetSeen", false);
                targetSeen = false;
                target = null;
                myMutant.MyBlackBoard.SetVector3("TargetPosition", Vector3.Zero);
            }
            base.OnTriggerExit(other);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            reader.ReadStartElement("MyMutantID");
            this.MyMutantID = (uint)reader.ReadContentAsInt();
            reader.ReadEndElement();
            base.ReadXml(reader);
            

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("MyMutantID");
            writer.WriteValue(this.MyMutantID);
            writer.WriteEndElement();
            base.WriteXml(writer);
        }
    }
}
