using System;
using System.Collections.Generic;
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
        #endregion

        #region methods
        public Weapon(GameObject obj):base(obj)
        {
        }

        public override void OnTrigger(GameObject other)
        {
            if(other is Enemy)
            {
                (other as Enemy).HitPoints -= Damage;
                if(Type != WeaponType.FISTS && Durability > 0)
                    Durability--;
            }
            base.OnTrigger(other);
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
