using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class PlayerAttackTrigger : ObjectComponent
    {
        private List<Enemy> enemiesInTrigger = new List<Enemy>();
        private PlayerController pc;

        public delegate void AttackEventHandler(object o, CollisionEventArgs e);
        public event AttackEventHandler AttackEvent;

        public PlayerAttackTrigger(GameObject go) : base(go)
        {

        }

        public PlayerAttackTrigger(GameObject go, PlayerAttackTrigger pat) : base(go)
        {
            this.enemiesInTrigger = pat.enemiesInTrigger;
        }

        public PlayerAttackTrigger(GameObject go, PlayerController pc) : base(go)
        {
            this.pc = pc;
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

        public void Attack(int damage)
        {
            foreach(Enemy e in this.enemiesInTrigger)
            {
                if(AttackEvent != null)
                {
                    AttackEvent(this, new CollisionEventArgs(e.MyObject));
                }

                e.HitPoints -= damage;
                ParticleSystem ps = e.MyObject.GetComponent<ParticleSystem>() as ParticleSystem;
                if(ps != null)
                {
                    ps.Play();
}
                if (e.MyObject.MyPhysicalObject != null)
                {
                    Vector3 diff = e.MyObject.MyTransform.Position - pc.MyObject.MyTransform.Position;
                    diff.Y = 0.0f;
                    diff.Normalize();
                    e.MyObject.MyPhysicalObject.AddForce(diff * 50.0f * damage);
                }
            }
        }

        public override void OnTriggerEnter(GameObject other)
        {
            Enemy e = (Enemy)other.GetComponent<Enemy>();
            if(e != null)
            {
                if(!this.enemiesInTrigger.Contains(e))
                {
                    this.enemiesInTrigger.Add(e);
                }
            }
            base.OnTriggerEnter(other);
        }

        public override void OnTriggerExit(GameObject other)
        {
            Enemy e = (Enemy)other.GetComponent<Enemy>();
            if (e != null)
            {
                if (this.enemiesInTrigger.Contains(e))
                {
                    this.enemiesInTrigger.Remove(e);
                }
            }
            base.OnTriggerEnter(other);
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
