using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TrashSoup.Engine;
using TrashSoup.Gameplay.Weapons;

namespace TrashSoup.Gameplay
{
    public class Equipment : ObjectComponent, IXmlSerializable
    {
        #region variables
        public const int MAX_JUNK_CAPACITY = 20;
        public const int MAX_FOOD_CAPACITY = 5;
        private int currentJunkCount;
        private int currentFoodCount;
        private Weapon currentWeapon;
        #endregion

        #region properties
        public int JunkCount
        {
            get { return currentJunkCount; }
            set { currentJunkCount = value; }
        }

        public int FoodCount
        {
            get { return currentFoodCount; }
            set { currentFoodCount = value; }
        }

        public Weapon CurrentWeapon
        {
            get { return currentWeapon; }
            set { currentWeapon = value; }
        }
        #endregion

        #region methods
        public Equipment(GameObject obj) : base(obj)
        {
            Start();
        }

        public void AddJunk(int count)
        {
            if (JunkCount+count < MAX_JUNK_CAPACITY)
                JunkCount+=count;
            else
                GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"), 
                    "Can't carry any more junk", new Vector2(0.5f, 0.8f), Color.Red);
            return;
        }

        public void AddFood()
        {
            if (FoodCount < MAX_FOOD_CAPACITY)
                FoodCount++;
            else
                GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"),
                    "Can't carry any more food", new Vector2(0.5f, 0.8f), Color.Red);
            return;
        }

        public void PickUpWeapon(GameObject newWeapon)
        {
            newWeapon.Dynamic = true;
            newWeapon.MyCarrierSocket = new Socket(base.MyObject, newWeapon, null, "mixamorig:RightHand");
            newWeapon.MyTransform.Position = new Vector3(0, 0, 0);
            newWeapon.MyTransform.Scale = 1;
            base.MyObject.MyCollider.IgnoredColliders.Add(newWeapon.MyCollider);
        }

        public void DropWeapon(GameObject weapon)
        {
            weapon.Dynamic = false;
            weapon.MyCarrierSocket = new Socket(weapon, weapon, null, "");
            weapon.MyTransform.Position = new Vector3(0, 0, 0);
            weapon.MyTransform.Scale = 4;
            base.MyObject.MyCollider.IgnoredColliders.Remove(weapon.MyCollider);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(currentWeapon.Durability == 0)
            {
                this.CurrentWeapon = new Fists(this.MyObject);
            }
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        protected override void Start()
        {
            currentJunkCount = 0;
            currentFoodCount = 0;
            currentWeapon = new Fists(this.MyObject);
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

            JunkCount = reader.ReadElementContentAsInt("JunkCount", "");
            FoodCount = reader.ReadElementContentAsInt("FoodCount", "");

            if (reader.Name == "Weapon")
            {
                reader.ReadStartElement();
                (CurrentWeapon as IXmlSerializable).ReadXml(reader);
                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("JunkCount", XmlConvert.ToString(JunkCount));
            writer.WriteElementString("FoodCount", XmlConvert.ToString(FoodCount));
            if(CurrentWeapon != null)
            {
                writer.WriteStartElement("Weapon");
                (CurrentWeapon as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
