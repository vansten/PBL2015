using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class Equipment : ObjectComponent
    {
        #region variables
        private const int MAX_JUNK_CAPACITY = 20;
        private const int MAX_FOOD_CAPACITY = 5;
        private int currentJunkCapacity;
        private int currentFoodCapacity;
        private Weapon currentWeapon;
        #endregion

        #region properties
        public int JunkCapacity
        {
            get { return currentJunkCapacity; }
            set { currentJunkCapacity = value; }
        }

        public int FoodCapacity
        {
            get { return currentFoodCapacity; }
            set { currentFoodCapacity = value; }
        }

        public Weapon CurrentWeapon
        {
            get { return currentWeapon; }
            set { currentWeapon = value; }
        }
        #endregion

        #region methods
        public Equipment()
        {
            currentJunkCapacity = 0;
            currentFoodCapacity = 0;
            currentWeapon = null; //będą pięści
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
