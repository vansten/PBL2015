using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public enum WeaponType
    {
        FISTS,
        LIGHT,
        MEDIUM,
        HEAVY
    }

    public class Weapon : ObjectComponent, IXmlSerializable
    {
        #region variables
        protected int durability;
        protected int damage;
        protected WeaponType type;
        protected bool isCraftable;
        protected int craftingCost;
        protected string name;
        protected bool isAttacking = false;
        public double timerOn;

        private GameObject player;
        private PlayerController pc;
        #endregion

        #region properties
        public int Durability
        {
            get { return durability; }
            set { durability = value; }
        }

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public WeaponType Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool IsCraftable
        {
            get { return isCraftable; }
            set { isCraftable = value; }
        }

        public int CraftingCost
        {
            get { return craftingCost; }
            set { craftingCost = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsAttacking
        {
            get { return isAttacking; }
            set { isAttacking = value; }
        }
        #endregion

        #region methods
        public Weapon(GameObject obj):base(obj)
        {

        }

        public override void Initialize()
        {
            this.player = ResourceManager.Instance.CurrentScene.GetObject(1);
            if(this.player != null)
            {
                this.pc = (PlayerController)this.player.GetComponent<PlayerController>();
            }
            base.Initialize();
        }

        public override void OnTriggerEnter(GameObject other)
        {
            if(isAttacking)
            {
                /*if (other.Components.Exists(x => x is Enemy))
                {
                    (other.GetComponent<Enemy>() as Enemy).HitPoints -= Damage;
                    ParticleSystem ps = other.GetComponent<ParticleSystem>() as ParticleSystem;
                    ps.Play();
                    Rat r = other.GetComponent<Rat>() as Rat;
                    if(r != null)
                    {
                        r.MyBlackBoard.SetBool("TargetSeen", true);
                        r.MyBlackBoard.SetVector3("TargetPosition", player.MyTransform.Position);
                        if(r.MyObject.MyPhysicalObject != null)
                        {
                            Vector3 diff = r.MyObject.MyTransform.Position - player.MyTransform.Position;
                            diff.Y = 0.0f;
                            diff.Normalize();
                            r.MyObject.MyPhysicalObject.AddForce(diff * 650.0f);
                        }
                        this.pc.Popularity += 15.0f;
                    }
                    if (Type != WeaponType.FISTS && Durability > 0)
                        Durability--;
                }*/
            }
            base.OnTriggerEnter(other);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (IsAttacking)
            {
                ((PlayerController)player.GetComponent<PlayerController>()).Walk.Stop(true);   
            }
            if(gameTime.TotalGameTime.TotalSeconds - timerOn > 1.5f)
            isAttacking = false;
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        protected override void Start()
        {

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
            Durability = reader.ReadElementContentAsInt("Durability", "");
            Damage = reader.ReadElementContentAsInt("Damage", "");
            string s = reader.ReadElementString("Type", "");
            switch(s)
            {
                case "FISTS":
                    Type = WeaponType.FISTS;
                    break;
                case "LIGHT":
                    Type = WeaponType.LIGHT;
                    break;
                case "MEDIUM":
                    Type = WeaponType.MEDIUM;
                    break;
                case "HEAVY":
                    Type = WeaponType.HEAVY;
                    break;
            }

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("Durability", XmlConvert.ToString(Durability));
            writer.WriteElementString("Damage", XmlConvert.ToString(Damage));
            writer.WriteElementString("Type", Type.ToString());
        }
        #endregion
    }
}
