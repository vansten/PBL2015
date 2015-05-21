using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class Enemy : GameObject
    {
        #region variables
        protected int hitPoints;
        protected bool isDead;
        #endregion

        #region properties
        public int HitPoints
        {
            get { return hitPoints; }
            set { hitPoints = value; }
        }
        #endregion

        #region methods
        public Enemy(uint uniqueID, string name, int hitPoints):base(uniqueID, name)
        {
            this.hitPoints = hitPoints;
        }
        #endregion
    }
}
