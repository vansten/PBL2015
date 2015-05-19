using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public enum WeaponType
    {
        LIGHT,
        MEDIUM,
        HEAVY
    }

    class Weapon : ObjectComponent
    {
        #region variables
        protected int durability;
        protected int damage;
        protected WeaponType type; 
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
        #endregion

        #region methods
        public Weapon()
        {
        }

        public Weapon(int durability, int damage, WeaponType type)
        {
            this.durability = durability;
            this.damage = damage;
            this.type = type;
        }

        public override void OnTrigger(GameObject other)
        {
            if(other is Enemy)
            {
                //decrease amount of enemy health equal to damage
                Durability--;
            }
            base.OnTrigger(other);
        }
        #endregion

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        protected override void Start()
        {
            
        }
    }
}
