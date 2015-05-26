using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    public class DestructiblePart : GameObject
    {
        #region variables

        bool stopped = false;

        #endregion

        #region properties

        public Vector3 Offset { get; set; }
        public Vector3 Rotation { get; set; }

        #endregion

        #region methods

        public DestructiblePart(uint uniqueID, string name)
            :base(uniqueID, name)
        {
        }

        public override void OnCollision(GameObject otherGO)
        {
            if(!stopped)
            {
                Rotation = Vector3.Zero;
                stopped = true;
            }

            base.OnCollision(otherGO);
        }

        #endregion
    }
}
